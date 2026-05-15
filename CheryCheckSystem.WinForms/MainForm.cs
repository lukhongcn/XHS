using System;
using Model.Label;
using LabelHelp.Services;
using LabelHelp.Enums;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CheryPortHelp;
using Model;

namespace CheryCheckSystem.WinForms
{
    public class MainForm : Form
    {
        private readonly BindingList<CheryScanDisplayRow> _rows = new BindingList<CheryScanDisplayRow>();
        private Panel pnlTop;
        private Label lblScanCodeTitle;
        private TextBox txtScanCode;
        private Button btnUpload;
        private Button btnTestUpload;
        private Button btnTestHaiXingYun;
        private Button btnPrintOuterBoxLabel;
        private GroupBox grpResult;
        private DataGridView dgvResult;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;

        public MainForm()
        {
            InitializeComponent();
            BindGrid();
        }

        private void InitializeComponent()
        {
            pnlTop = new Panel();
            lblScanCodeTitle = new Label();
            txtScanCode = new TextBox();
            btnUpload = new Button();
            btnTestUpload = new Button();
            btnTestHaiXingYun = new Button();
            btnPrintOuterBoxLabel = new Button();
            grpResult = new GroupBox();
            dgvResult = new DataGridView();
            statusStrip = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();

            SuspendLayout();

            Text = "奇瑞防错漏系统";
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1200, 700);
            MinimumSize = new Size(1000, 600);
            MaximizeBox = true;

            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 70;
            pnlTop.Padding = new Padding(12);

            lblScanCodeTitle.AutoSize = true;
            lblScanCodeTitle.Location = new Point(12, 24);
            lblScanCodeTitle.Text = "扫描条码：";

            txtScanCode.Name = "txtScanCode";
            txtScanCode.Font = new Font("Microsoft YaHei UI", 14F, FontStyle.Regular, GraphicsUnit.Point, 134);
            txtScanCode.Location = new Point(95, 17);
            txtScanCode.Width = 420;
            txtScanCode.KeyDown += TxtScanCode_KeyDown;

            btnUpload.Name = "btnUpload";
            btnUpload.Text = "上传接口";
            btnUpload.AutoSize = true;
            btnUpload.Location = new Point(530, 17);
            btnUpload.Padding = new Padding(10, 4, 10, 4);
            btnUpload.Click += BtnUpload_Click;

            btnTestUpload.Name = "btnTestUpload";
            btnTestUpload.Text = "测试连接/上传";
            btnTestUpload.AutoSize = true;
            btnTestUpload.Location = new Point(635, 17);
            btnTestUpload.Padding = new Padding(10, 4, 10, 4);
            btnTestUpload.Click += BtnTestUpload_Click;

            btnTestHaiXingYun.Name = "btnTestHaiXingYun";
            btnTestHaiXingYun.Text = "海行云连接测试";
            btnTestHaiXingYun.AutoSize = true;
            btnTestHaiXingYun.Location = new Point(780, 17);
            btnTestHaiXingYun.Padding = new Padding(10, 4, 10, 4);
            btnTestHaiXingYun.Click += BtnTestHaiXingYun_Click;

            btnPrintOuterBoxLabel.Name = "btnPrintOuterBoxLabel";
            btnPrintOuterBoxLabel.Text = "打印外箱标签";
            btnPrintOuterBoxLabel.AutoSize = true;
            btnPrintOuterBoxLabel.Location = new Point(930, 17);
            btnPrintOuterBoxLabel.Padding = new Padding(10, 4, 10, 4);
            btnPrintOuterBoxLabel.Click += BtnPrintOuterBoxLabel_Click;

            pnlTop.Controls.Add(lblScanCodeTitle);
            pnlTop.Controls.Add(txtScanCode);
            pnlTop.Controls.Add(btnUpload);
            pnlTop.Controls.Add(btnTestUpload);
            pnlTop.Controls.Add(btnTestHaiXingYun);
            pnlTop.Controls.Add(btnPrintOuterBoxLabel);

            grpResult.Name = "grpResult";
            grpResult.Text = "扫描与接口结果";
            grpResult.Dock = DockStyle.Fill;
            grpResult.Padding = new Padding(10);

            dgvResult.Name = "dgvResult";
            dgvResult.Dock = DockStyle.Fill;
            dgvResult.ReadOnly = true;
            dgvResult.AllowUserToAddRows = false;
            dgvResult.AllowUserToDeleteRows = false;
            dgvResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvResult.RowHeadersVisible = false;
            dgvResult.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvResult.MultiSelect = false;

            grpResult.Controls.Add(dgvResult);

            statusStrip.Items.Add(lblStatus);
            statusStrip.Dock = DockStyle.Bottom;

            lblStatus.Name = "lblStatus";
            lblStatus.Text = "就绪";

            Controls.Add(grpResult);
            Controls.Add(pnlTop);
            Controls.Add(statusStrip);

            ResumeLayout(false);
            PerformLayout();
        }

        private void BindGrid()
        {
            dgvResult.AutoGenerateColumns = false;
            dgvResult.Columns.Add(CreateTextColumn("ScanTime", "扫描时间"));
            dgvResult.Columns.Add(CreateTextColumn("ScanCode", "扫描内容"));
            dgvResult.Columns.Add(CreateTextColumn("SupplNo", "供应商编码"));
            dgvResult.Columns.Add(CreateTextColumn("BaseNo", "基地编码"));
            dgvResult.Columns.Add(CreateTextColumn("DeliveryNo", "配送单号"));
            dgvResult.Columns.Add(CreateTextColumn("SxCardSeq", "随箱卡号"));
            dgvResult.Columns.Add(CreateTextColumn("MaterialNo", "零件号"));
            dgvResult.Columns.Add(CreateTextColumn("MaterialName", "零件名称"));
            dgvResult.Columns.Add(CreateTextColumn("PackingCount", "数量"));
            dgvResult.Columns.Add(CreateTextColumn("PackageBarCode", "箱码/包装流水号"));
            dgvResult.Columns.Add(CreateTextColumn("PackageCode", "包装编码"));
            dgvResult.Columns.Add(CreateTextColumn("PackageName", "包装名称"));
            dgvResult.Columns.Add(CreateTextColumn("CheckUserName", "检测人"));
            dgvResult.Columns.Add(CreateTextColumn("UploadStatus", "上传状态"));
            dgvResult.Columns.Add(CreateTextColumn("ReturnCode", "返回Code"));
            dgvResult.Columns.Add(CreateTextColumn("ReturnMsg", "返回Msg"));
            dgvResult.DataSource = _rows;
        }

        private static DataGridViewTextBoxColumn CreateTextColumn(string propertyName, string headerText)
        {
            return new DataGridViewTextBoxColumn
            {
                DataPropertyName = propertyName,
                HeaderText = headerText,
                Name = "col" + propertyName,
                ReadOnly = true
            };
        }

        private void TxtScanCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;
            e.Handled = true;

            var scanCode = txtScanCode.Text.Trim();
            if (string.IsNullOrWhiteSpace(scanCode))
            {
                txtScanCode.Clear();
                txtScanCode.Focus();
                return;
            }

            _rows.Insert(0, new CheryScanDisplayRow
            {
                ScanTime = DateTime.Now,
                ScanCode = scanCode,
                UploadStatus = "未上传"
            });

            lblStatus.Text = "已扫描：" + scanCode;
            txtScanCode.Clear();
            txtScanCode.Focus();
        }

        private void BtnUpload_Click(object sender, EventArgs e)
        {
            MessageBox.Show("上传功能尚未实现", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            lblStatus.Text = "上传功能尚未实现";
        }

        private void BtnTestUpload_Click(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "正在生成测试数据并上传海行云接口...";

                var uploadService = new BLL.CheryUpload();
                var uploadInfo = uploadService.CreateUploadInfo();

                var request = CheryRequestBuilder.BuildCheckRecordRequest(uploadInfo);

                var client = new CheryHttpClient();
                var result = client.PostCheckRecord(request);

                MessageBox.Show(
                    "返回Code：" + result.code + Environment.NewLine +
                    "返回Msg：" + result.msg,
                    "海行云上传测试",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                lblStatus.Text = "海行云上传测试完成：" + result.msg;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "上传异常：" + ex.Message,
                    "错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblStatus.Text = "上传异常：" + ex.Message;
            }
        }



        private void BtnPrintOuterBoxLabel_Click(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "正在生成外箱标签PDF...";

                var uploadService = new BLL.CheryUpload();

                // 外箱标签打印内容来源：
                // 与测试上传中 var uploadInfo = uploadService.CreateUploadInfo(); 保持一致。
                var uploadInfo = uploadService.CreateUploadInfo();

                // ================================
                // 包装箱标签二维码规则
                // ================================
                // 客户资料中的包装箱标签二维码内容格式为：
                // 10#零件号$11#供应商代码$17#包装数量$18#发运批次$19#码放层数$20#生产日期$
                //
                // 对应关系：
                // 10# = uploadInfo.MaterialNo      零件号
                // 11# = supplierCode               供应商代码
                // 17# = uploadInfo.PackingCount    包装数量
                // 18# = lotNo                      发运批次 / 供货批次
                // 19# = layerCount                 码放层数
                // 20# = PackingDate yyyyMMdd       生产日期
                //
                // 注意：
                // CheryUploadInfo 目前没有单独的 SupplierCode、LayerCount、ShippingLotNo 字段。
                // 所以下面三个值暂时用常量/现有字段代替。
                // 正式上线前建议确认：
                // 1. supplierCode 是否应从 App.config 读取 Chery.SupplNo；
                // 2. lotNo 是否等于 DeliveryNo，还是另有“发运批次/销售批次”字段；
                // 3. layerCount 是否固定为 1，还是由业务录入/计算。
                string supplierCode = "8KN";
                string layerCount = "1";
                string lotNo = uploadInfo.DeliveryNo;

                string produceDateText = uploadInfo.PackingDate.HasValue
                    ? uploadInfo.PackingDate.Value.ToString("yyyy-MM-dd")
                    : "";

                string produceDateQr = uploadInfo.PackingDate.HasValue
                    ? uploadInfo.PackingDate.Value.ToString("yyyyMMdd")
                    : "";

                string checkDateText = uploadInfo.CheckTime.HasValue
                    ? uploadInfo.CheckTime.Value.ToString("yyyy-MM-dd")
                    : "";

                string qrContent = BuildOuterBoxQrContent(
                    uploadInfo.MaterialNo,
                    supplierCode,
                    uploadInfo.PackingCount,
                    lotNo,
                    layerCount,
                    produceDateQr);

                var labelInfo = new LabelInfo
                {
                    SupplierName = "",
                    SupplierCode = supplierCode,
                    PartNo = uploadInfo.MaterialNo,
                    PartName = uploadInfo.MaterialName,
                    Qty = uploadInfo.PackingCount,
                    LotNo = lotNo,
                    LayerCount = layerCount,
                    ProduceDate = produceDateText,
                    CheckDate = checkDateText,
                    MaterialCode = uploadInfo.PackageName,
                    SerialNo = uploadInfo.PackageBarCode,

                    // 二维码内容不要放整个 uploadInfo，也不要直接使用 PackageBarCode。
                    // 根据客户包装箱标签资料，二维码必须按固定项号规则拼接。
                    QrContent = qrContent
                };

                var service = new LabelPrintService();
                string pdfPath = service.Generate(
                    new List<LabelInfo> { labelInfo },
                    LabelTemplateType.TableLabel,
                    LabelPrintMode.A4Pdf);

                MessageBox.Show(
                    "外箱标签PDF已生成：" + Environment.NewLine +
                    pdfPath + Environment.NewLine + Environment.NewLine +
                    "二维码内容：" + Environment.NewLine +
                    qrContent,
                    "打印外箱标签",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                lblStatus.Text = "外箱标签PDF已生成：" + pdfPath;

                try
                {
                    Process.Start(pdfPath);
                }
                catch
                {
                    // 某些环境可能不允许自动打开，忽略即可。
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "生成外箱标签PDF异常：" + ex.Message,
                    "错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblStatus.Text = "生成外箱标签PDF异常：" + ex.Message;
            }
        }

        /// <summary>
        /// 生成包装箱标签二维码内容。
        /// 格式：
        /// 10#零件号$11#供应商代码$17#包装数量$18#发运批次$19#码放层数$20#生产日期$
        /// </summary>
        private static string BuildOuterBoxQrContent(
            string materialNo,
            string supplierCode,
            string packingCount,
            string lotNo,
            string layerCount,
            string produceDate)
        {
            return
                "10#" + SafeValue(materialNo) + "$" +
                "11#" + SafeValue(supplierCode) + "$" +
                "17#" + SafeValue(packingCount) + "$" +
                "18#" + SafeValue(lotNo) + "$" +
                "19#" + SafeValue(layerCount) + "$" +
                "20#" + SafeValue(produceDate) + "$";
        }

        private static string SafeValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "" : value.Trim();
        }

        private void BtnTestHaiXingYun_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "正在测试海行云接口连接...";
            btnTestHaiXingYun.Enabled = false;

            try
            {
                var client = new CheryHttpClient();
                var result = client.TestConnection();

                MessageBox.Show(
                    "返回Code：" + result.code + Environment.NewLine + "返回Msg：" + result.msg,
                    "海行云连接测试",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                if (result.code == 200)
                {
                    lblStatus.Text = "海行云接口测试成功";
                }
                else
                {
                    lblStatus.Text = "海行云接口已返回：" + result.msg;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(
                    "返回Code：-1" + Environment.NewLine + "返回Msg：" + ex.Message,
                    "海行云连接测试",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                lblStatus.Text = "海行云接口已返回：" + ex.Message;
            }
            finally
            {
                btnTestHaiXingYun.Enabled = true;
            }
        }
    }
}
