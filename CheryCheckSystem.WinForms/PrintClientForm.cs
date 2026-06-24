using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using LabelHelp.Config;
using LabelHelp.Enums;
using LabelHelp.Printer;

namespace CheryCheckSystem.PrintClient
{
    public class PrintClientForm : Form
    {
        private readonly PrintPendingApiClient _apiClient = new PrintPendingApiClient();
        private readonly Timer _pollTimer = new Timer();
        private bool _isProcessing;

        private Label lblApiBaseUrl;
        private TextBox txtApiBaseUrl;
        private Label lblMachineId;
        private TextBox txtMachineId;
        private Label lblPrinterName;
        private ComboBox cboPrinterName;
        private Button btnRefreshPrinters;
        private Label lblPollInterval;
        private NumericUpDown nudPollInterval;
        private Button btnFetchAndPrint;
        private Button btnStartPolling;
        private Button btnStopPolling;
        private TextBox txtLog;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;
        public PrintClientForm()
        {
            InitializeComponent();
            LoadSettings();
            LoadPrinters();
            _pollTimer.Tick += PollTimer_Tick;
        }

        private void InitializeComponent()
        {
            lblApiBaseUrl = new Label();
            txtApiBaseUrl = new TextBox();
            lblMachineId = new Label();
            txtMachineId = new TextBox();
            lblPrinterName = new Label();
            cboPrinterName = new ComboBox();
            btnRefreshPrinters = new Button();
            lblPollInterval = new Label();
            nudPollInterval = new NumericUpDown();
            btnFetchAndPrint = new Button();
            btnStartPolling = new Button();
            btnStopPolling = new Button();
            txtLog = new TextBox();
            statusStrip = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            SuspendLayout();

            Text = "打印客户端";
            Name = "PrintClientForm";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(900, 620);
            MinimumSize = new Size(900, 620);

            lblApiBaseUrl.AutoSize = true;
            lblApiBaseUrl.Location = new Point(24, 24);
            lblApiBaseUrl.Text = "接口地址";

            txtApiBaseUrl.Location = new Point(110, 20);
            txtApiBaseUrl.Size = new Size(520, 23);

            lblMachineId.AutoSize = true;
            lblMachineId.Location = new Point(24, 62);
            lblMachineId.Text = "机器编号";

            txtMachineId.Location = new Point(110, 58);
            txtMachineId.Size = new Size(180, 23);

            lblPrinterName.AutoSize = true;
            lblPrinterName.Location = new Point(320, 62);
            lblPrinterName.Text = "打印机名";

            cboPrinterName.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPrinterName.Location = new Point(390, 58);
            cboPrinterName.Size = new Size(240, 25);

            btnRefreshPrinters.Location = new Point(650, 57);
            btnRefreshPrinters.Size = new Size(96, 27);
            btnRefreshPrinters.Text = "刷新打印机";
            btnRefreshPrinters.Click += BtnRefreshPrinters_Click;

            lblPollInterval.AutoSize = true;
            lblPollInterval.Location = new Point(24, 100);
            lblPollInterval.Text = "轮询秒数";

            nudPollInterval.Location = new Point(110, 96);
            nudPollInterval.Minimum = 1;
            nudPollInterval.Maximum = 3600;
            nudPollInterval.Size = new Size(120, 23);

            btnFetchAndPrint.Location = new Point(24, 138);
            btnFetchAndPrint.Size = new Size(120, 32);
            btnFetchAndPrint.Text = "获取并打印";
            btnFetchAndPrint.Click += BtnFetchAndPrint_Click;

            btnStartPolling.Location = new Point(160, 138);
            btnStartPolling.Size = new Size(120, 32);
            btnStartPolling.Text = "启动轮询";
            btnStartPolling.Click += BtnStartPolling_Click;

            btnStopPolling.Location = new Point(296, 138);
            btnStopPolling.Size = new Size(120, 32);
            btnStopPolling.Text = "停止轮询";
            btnStopPolling.Click += BtnStopPolling_Click;

            txtLog.Location = new Point(24, 188);
            txtLog.Multiline = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.ReadOnly = true;
            txtLog.Size = new Size(840, 358);

            statusStrip.Items.Add(lblStatus);
            statusStrip.Dock = DockStyle.Bottom;
            lblStatus.Text = "就绪";

            Controls.Add(lblApiBaseUrl);
            Controls.Add(txtApiBaseUrl);
            Controls.Add(lblMachineId);
            Controls.Add(txtMachineId);
            Controls.Add(lblPrinterName);
            Controls.Add(cboPrinterName);
            Controls.Add(btnRefreshPrinters);
            Controls.Add(lblPollInterval);
            Controls.Add(nudPollInterval);
            Controls.Add(btnFetchAndPrint);
            Controls.Add(btnStartPolling);
            Controls.Add(btnStopPolling);
            Controls.Add(txtLog);
            Controls.Add(statusStrip);

            ResumeLayout(false);
            PerformLayout();
        }

        private void LoadSettings()
        {
            txtApiBaseUrl.Text = GetSetting("PrintClient.PendingApiUrl", "http://localhost:55426/api/print/pending-labels?machineId=PRT-01");
            txtMachineId.Text = GetSetting("PrintClient.MachineId", "PRT-01");
            nudPollInterval.Value = GetIntSetting("PrintClient.PollIntervalSeconds", 10, 1, 3600);
        }

        private void LoadPrinters()
        {
            string configuredPrinterName = GetSetting("PrintClient.PrinterName", string.Empty);
            string defaultPrinterName = string.IsNullOrWhiteSpace(configuredPrinterName)
                ? GetDefaultPrinterName()
                : configuredPrinterName.Trim();

            cboPrinterName.Items.Clear();
            cboPrinterName.Items.Add(string.Empty);

            foreach (string installedPrinter in PrinterSettings.InstalledPrinters)
            {
                string safePrinterName = SafeValue(installedPrinter);
                if (!string.IsNullOrWhiteSpace(safePrinterName))
                {
                    cboPrinterName.Items.Add(safePrinterName);
                }
            }

            if (!string.IsNullOrWhiteSpace(defaultPrinterName) && !cboPrinterName.Items.Contains(defaultPrinterName))
            {
                cboPrinterName.Items.Add(defaultPrinterName);
            }

            cboPrinterName.SelectedItem = cboPrinterName.Items.Contains(defaultPrinterName)
                ? defaultPrinterName
                : string.Empty;
        }

        private void BtnRefreshPrinters_Click(object sender, EventArgs e)
        {
            LoadPrinters();
            AppendLog("已刷新打印机列表。");
        }

        private void BtnFetchAndPrint_Click(object sender, EventArgs e)
        {
            FetchAndPrint();
        }

        private void BtnStartPolling_Click(object sender, EventArgs e)
        {
            _pollTimer.Interval = (int)nudPollInterval.Value * 1000;
            _pollTimer.Start();
            SetStatus("轮询已启动。");
            AppendLog("轮询已启动，间隔 " + nudPollInterval.Value + " 秒。");
        }

        private void BtnStopPolling_Click(object sender, EventArgs e)
        {
            _pollTimer.Stop();
            SetStatus("轮询已停止。");
            AppendLog("轮询已停止。");
        }

        private void PollTimer_Tick(object sender, EventArgs e)
        {
            FetchAndPrint();
        }

        private void FetchAndPrint()
        {
            if (_isProcessing)
            {
                AppendLog("当前仍在处理上一批打印任务。");
                return;
            }

            try
            {
                _isProcessing = true;
                string pendingApiUrlTemplate = SafeValue(txtApiBaseUrl.Text);
                string machineId = SafeValue(txtMachineId.Text);
                string pendingApiPath = GetSetting("PrintClient.PendingApiPath", "/api/print/pending-labels");
                string apiBaseUrl = GetSetting("PrintClient.ApiBaseUrl", "http://localhost:55426");
                string completeApiUrl = GetSetting("PrintClient.CompleteApiUrl", "http://localhost:55426/api/print/complete");
                string printerName = SafeValue(Convert.ToString(cboPrinterName.SelectedItem));
                int pauseMilliseconds = Convert.ToInt32(GetIntSetting("PrintClient.PrintPauseMilliseconds", 1500, 0, 60000));
                LabelPrintConfig printConfig = LabelPrintConfig.LoadRollPaper();

                string requestUrl = _apiClient.BuildPendingUrlFromTemplate(pendingApiUrlTemplate, machineId);
                AppendLog("开始调用接口：" + requestUrl);
                SetStatus("正在获取待打印数据...");

                List<PrintPendingRecord> records;
                if (!string.IsNullOrWhiteSpace(pendingApiUrlTemplate))
                {
                    records = FetchPendingRecordsByFullUrl(requestUrl);
                }
                else
                {
                    records = _apiClient.FetchPendingRecords(apiBaseUrl, pendingApiPath, machineId);
                }
                if (records.Count == 0)
                {
                    AppendLog("未获取到待打印记录。");
                    SetStatus("未获取到待打印记录。");
                    return;
                }

                AppendLog("获取到 " + records.Count + " 条待打印记录。");
                int successCount = 0;
                int failedCount = 0;
                foreach (PrintPendingRecord record in records.Where(item => item != null))
                {
                    try
                    {
                        string displayId = record.id.HasValue ? record.id.Value.ToString() : "0";
                        if (record.labelInfo == null)
                        {
                            throw new InvalidOperationException("接口未返回标签数据。");
                        }

                        new LabelPrinterOutput().PrintSingleLabel(
                            record.labelInfo,
                            LabelTemplateType.TableLabel,
                            printConfig,
                            printerName);

                        if (pauseMilliseconds > 0)
                        {
                            System.Threading.Thread.Sleep(pauseMilliseconds);
                        }

                        _apiClient.CompletePrint(completeApiUrl, SafeValue(record.taskId), string.Empty);
                        AppendLog("记录 " + displayId + " 已直接发送到打印机：" + (string.IsNullOrWhiteSpace(printerName) ? "默认打印机" : printerName));
                        AppendLog("记录 " + displayId + " 已回写打印完成状态。");
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        failedCount++;
                        AppendLog("单条处理失败：" + ex.Message);
                    }
                }

                SetStatus("打印任务处理完成。成功 " + successCount + " 条，失败 " + failedCount + " 条。");
            }
            catch (Exception ex)
            {
                AppendLog("处理失败：" + ex.Message);
                SetStatus("处理失败：" + ex.Message);
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private List<PrintPendingRecord> FetchPendingRecordsByFullUrl(string requestUrl)
        {
            using (System.Net.WebClient webClient = new System.Net.WebClient())
            {
                webClient.Encoding = System.Text.Encoding.UTF8;
                string responseText = webClient.DownloadString(requestUrl);
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                PrintPendingApiResponse response = serializer.Deserialize<PrintPendingApiResponse>(responseText);
                if (response == null)
                {
                    throw new InvalidOperationException("接口返回为空。");
                }

                if (!response.success)
                {
                    throw new InvalidOperationException(string.IsNullOrWhiteSpace(response.message) ? "接口返回失败。" : response.message);
                }

                return response.data ?? new List<PrintPendingRecord>();
            }
        }

        private void AppendLog(string message)
        {
            string line = string.Format("{0:yyyy-MM-dd HH:mm:ss}  {1}", DateTime.Now, message);
            txtLog.AppendText(line + Environment.NewLine);
        }

        private void SetStatus(string message)
        {
            lblStatus.Text = message;
        }

        private static string GetSetting(string key, string defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();
        }

        private static decimal GetIntSetting(string key, int defaultValue, int minValue, int maxValue)
        {
            int value;
            if (!int.TryParse(ConfigurationManager.AppSettings[key], out value))
            {
                value = defaultValue;
            }

            if (value < minValue)
            {
                value = minValue;
            }

            if (value > maxValue)
            {
                value = maxValue;
            }

            return value;
        }

        private static string GetDefaultPrinterName()
        {
            try
            {
                foreach (string installedPrinter in PrinterSettings.InstalledPrinters)
                {
                    string safePrinterName = SafeValue(installedPrinter);
                    if (string.Equals(safePrinterName, "DL-740C(NEW)", StringComparison.OrdinalIgnoreCase) ||
                        safePrinterName.IndexOf("DL-740C", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return safePrinterName;
                    }
                }

                PrinterSettings printerSettings = new PrinterSettings();
                return SafeValue(printerSettings.PrinterName);
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string SafeValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }
    }
}
