using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LabelHelp.Config;
using LabelHelp.Enums;
using LabelHelp.Printer;

namespace CheryCheckSystem.PrintClient
{
    public class PrintClientWorker
    {
        private readonly PrintClientSettings _settings;
        private readonly PrintPendingApiClient _apiClient = new PrintPendingApiClient();
        private readonly Action<string> _writeLog;
        private int _isProcessing;

        public PrintClientWorker(PrintClientSettings settings, Action<string> writeLog)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            _settings = settings;
            _writeLog = writeLog ?? delegate { };
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
                string requestUrl = _apiClient.BuildPendingUrlFromTemplate(_settings.PendingApiUrl, _settings.MachineId);
                WriteLog("开始调用接口：" + requestUrl);
                return _apiClient.FetchPendingRecordsByFullUrl(requestUrl);
            }

            string builtUrl = _apiClient.BuildPendingUrl(_settings.ApiBaseUrl, _settings.PendingApiPath, _settings.MachineId);
            WriteLog("开始调用接口：" + builtUrl);
            return _apiClient.FetchPendingRecords(_settings.ApiBaseUrl, _settings.PendingApiPath, _settings.MachineId);
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

                    WriteLog("记录 " + displayId + " 开始发送到打印机。");
                    printerOutput.PrintSingleLabel(
                        record.labelInfo,
                        LabelTemplateType.TableLabel,
                        printConfig,
                        printerName);

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
                }
            }

            WriteLog("打印任务处理完成。成功 " + successCount + " 条，失败 " + failedCount + " 条。");
        }

        private void WriteLog(string message)
        {
            _writeLog(message);
        }

        private static string SafeValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }
    }
}
