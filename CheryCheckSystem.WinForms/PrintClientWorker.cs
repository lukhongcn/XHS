using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using LabelHelp.Config;
using LabelHelp.Enums;
using LabelHelp.Printer;
using XHS.Model.Label;

namespace CheryCheckSystem.PrintClient
{
    public class PrintClientWorker
    {
        private readonly PrintClientSettings _settings;
        private readonly PrintPendingApiClient _apiClient = new PrintPendingApiClient();
        private readonly Action<string> _writeLog;
        private readonly Action<string> _writeFetchLog;
        private int _isProcessing;

        public PrintClientWorker(PrintClientSettings settings, Action<string> writeLog, Action<string> writeFetchLog)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            _settings = settings;
            _writeLog = writeLog ?? delegate { };
            _writeFetchLog = writeFetchLog ?? delegate { };
        }

        public void ProcessOnce()
        {
            if (Interlocked.Exchange(ref _isProcessing, 1) == 1)
            {
                WriteLog("当前仍在处理上一批打印任务。");
                return;
            }

            try
            {
                List<PrintPendingRecord> records = FetchPendingRecords();
                WriteFetchLog("本次取数完成，返回 " + records.Count + " 条记录。");
                LogFetchedRecords(records);
                if (records.Count == 0)
                {
                    WriteLog("未获取到待打印记录。");
                    return;
                }

                WriteLog("获取到 " + records.Count + " 条待打印记录。");
                ProcessRecords(records);
            }
            catch (Exception ex)
            {
                WriteFetchLog("取数异常：" + ex);
                WriteLog("处理失败：" + ex.Message);
            }
            finally
            {
                Interlocked.Exchange(ref _isProcessing, 0);
            }
        }

        private List<PrintPendingRecord> FetchPendingRecords()
        {
            if (!string.IsNullOrWhiteSpace(_settings.PendingApiUrl))
            {
                string requestUrl = _apiClient.BuildPendingUrlFromTemplate(_settings.PendingApiUrl, _settings.MachineId, _settings.LockTimeoutMinutes, _settings.PendingBatchSize);
                WriteFetchLog("请求地址：" + requestUrl);
                WriteLog("开始调用接口：" + requestUrl);
                return _apiClient.FetchPendingRecordsByFullUrl(requestUrl);
            }

            string builtUrl = _apiClient.BuildPendingUrl(_settings.ApiBaseUrl, _settings.PendingApiPath, _settings.MachineId, _settings.LockTimeoutMinutes, _settings.PendingBatchSize);
            WriteFetchLog("请求地址：" + builtUrl);
            WriteLog("开始调用接口：" + builtUrl);
            return _apiClient.FetchPendingRecords(_settings.ApiBaseUrl, _settings.PendingApiPath, _settings.MachineId, _settings.LockTimeoutMinutes, _settings.PendingBatchSize);
        }

        private void ProcessRecords(IEnumerable<PrintPendingRecord> records)
        {
            LabelPrintConfig printConfig = LabelPrintConfig.LoadRollPaper();
            LabelPrinterOutput printerOutput = new LabelPrinterOutput();
            string printerName = PrintClientPrinterResolver.Resolve(_settings.PrinterName);
            int successCount = 0;
            int failedCount = 0;

            WriteLog("本次使用打印机：" + (string.IsNullOrWhiteSpace(printerName) ? "默认打印机" : printerName));
            foreach (PrintPendingRecord record in records.Where(item => item != null))
            {
                try
                {
                    string displayId = record.id.HasValue ? record.id.Value.ToString() : "0";
                    if (record.labelInfo == null)
                    {
                        throw new InvalidOperationException("接口未返回标签数据。");
                    }

                    WriteLabelInfoLog(displayId, record.labelInfo);
                    WriteLog("记录 " + displayId + " 开始发送到打印机。");
                    DateTime printStartTime = DateTime.Now;
                    printerOutput.PrintSingleLabel(
                        record.labelInfo,
                        LabelTemplateType.TableLabel,
                        printConfig,
                        printerName);
                    WriteLog("记录 " + displayId + " 打印调用返回，耗时 " + (DateTime.Now - printStartTime).TotalSeconds.ToString("0.0") + " 秒。");

                    if (_settings.PrintPauseMilliseconds > 0)
                    {
                        Thread.Sleep(_settings.PrintPauseMilliseconds);
                    }

                    _apiClient.CompletePrint(_settings.CompleteApiUrl, SafeValue(record.taskId), string.Empty);
                    WriteLog("记录 " + displayId + " 已直接发送到打印机：" + (string.IsNullOrWhiteSpace(printerName) ? "默认打印机" : printerName));
                    WriteLog("记录 " + displayId + " 已回写打印完成状态。");
                    successCount++;
                }
                catch (Exception ex)
                {
                    failedCount++;
                    WriteLog("单条处理失败：" + ex.Message);
                    try
                    {
                        _apiClient.FailPrint(_settings.FailApiUrl, SafeValue(record.taskId), ex.ToString());
                        WriteLog("记录 " + (record.id.HasValue ? record.id.Value.ToString() : "0") + " 已释放锁并标记为失败。");
                    }
                    catch (Exception releaseException)
                    {
                        WriteLog("记录失败状态回写失败，任务仍可能被锁定：" + releaseException.Message);
                    }
                }
            }

            WriteLog("打印任务处理完成。成功 " + successCount + " 条，失败 " + failedCount + " 条。");
        }

        private void WriteLog(string message)
        {
            _writeLog(message);
        }

        private void WriteFetchLog(string message)
        {
            _writeFetchLog(message);
        }

        private void LogFetchedRecords(IEnumerable<PrintPendingRecord> records)
        {
            int index = 0;
            foreach (PrintPendingRecord record in records ?? Enumerable.Empty<PrintPendingRecord>())
            {
                index++;
                if (record == null)
                {
                    WriteFetchLog("第 " + index + " 条记录：null");
                    continue;
                }

                WriteFetchLog(string.Format(
                    "第 {0} 条记录：Id={1}, TaskId={2}, SupplyBatchNo={3}, PartNo={4}, CartonNo={5}, LabelInfo={6}",
                    index,
                    record.id.HasValue ? record.id.Value.ToString() : "<null>",
                    SafeValue(record.taskId),
                    SafeValue(record.supplyBatchNo),
                    SafeValue(record.partNo),
                    SafeValue(record.cartonNo),
                    record.labelInfo == null ? "null" : "已返回"));

                if (record.labelInfo != null)
                {
                    WriteFetchLog(string.Format(
                        "第 {0} 条标签数据：SupplierCode={1}, Qty={2}, LotNo={3}, PackageCode={4}, GrossWeight={5}, QrContentLength={6}",
                        index,
                        SafeValue(record.labelInfo.SupplierCode),
                        SafeValue(record.labelInfo.Qty),
                        SafeValue(record.labelInfo.LotNo),
                        SafeValue(record.labelInfo.PackageCode),
                        SafeValue(record.labelInfo.GrossWeight),
                        record.labelInfo.QrContent == null ? 0 : record.labelInfo.QrContent.Length));
                }
            }
        }

        private void WriteLabelInfoLog(string displayId, LabelInfo labelInfo)
        {
            WriteLog("记录 " + displayId + " LabelInfo开始。");
            PropertyInfo[] properties = labelInfo.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .OrderBy(property => property.Name)
                .ToArray();

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(labelInfo, null);
                WriteLog("记录 " + displayId + " LabelInfo." + property.Name + "=" + SafeLogValue(value));
            }

            WriteLog("记录 " + displayId + " LabelInfo结束。");
        }

        private static string SafeLogValue(object value)
        {
            if (value == null)
            {
                return "<null>";
            }

            return value.ToString()
                .Replace("\r", "\\r")
                .Replace("\n", "\\n");
        }

        private static string SafeValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }
    }
}
