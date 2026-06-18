using System;
using XHS.Model;
using XHS.Model.Label;
using LabelHelp.Services;
using LabelHelp.Enums;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CheryPortHelp;
using XHS.BLL;

namespace CheryCheckSystem.WinForms
{
    public class MainForm : Form
    {
        private readonly BindingList<CheryScanDisplayRow> _rows = new BindingList<CheryScanDisplayRow>();
        private readonly Dictionary<string, TextBox> _labelInfoTextBoxes = new Dictionary<string, TextBox>();
        private readonly Dictionary<string, DateTimeEditor> _labelInfoDateEditors = new Dictionary<string, DateTimeEditor>();
        private Panel pnlTop;
        private System.Windows.Forms.Label lblScanCodeTitle;
        private TextBox txtScanCode;
        private FlowLayoutPanel pnlButtons;
        private Button btnUpload;
        private Button btnTestUpload;
        private Button btnTestHaiXingYun;
        private Button btnPrintOuterBoxLabel;
        private Button btnPrintDeliveryNote;
        private GroupBox grpLabelInfo;
        private TableLayoutPanel tblLabelInfo;
        private GroupBox grpPlatformUpload;
        private TableLayoutPanel tblPlatformUpload;
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
            lblScanCodeTitle = new System.Windows.Forms.Label();
            txtScanCode = new TextBox();
            pnlButtons = new FlowLayoutPanel();
            btnUpload = new Button();
            btnTestUpload = new Button();
            btnTestHaiXingYun = new Button();
            btnPrintOuterBoxLabel = new Button();
            btnPrintDeliveryNote = new Button();
            grpLabelInfo = new GroupBox();
            tblLabelInfo = new TableLayoutPanel();
            grpPlatformUpload = new GroupBox();
            tblPlatformUpload = new TableLayoutPanel();
            grpResult = new GroupBox();
            dgvResult = new DataGridView();
            statusStrip = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();

            SuspendLayout();

            Text = "奇瑞防错漏系统";
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1380, 920);
            MinimumSize = new Size(1200, 780);
            MaximizeBox = true;

            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 120;
            pnlTop.Padding = new Padding(12);

            lblScanCodeTitle.AutoSize = true;
            lblScanCodeTitle.Location = new Point(12, 20);
            lblScanCodeTitle.Text = "扫描条码：";

            txtScanCode.Name = "txtScanCode";
            txtScanCode.Font = new Font("Microsoft YaHei UI", 14F, FontStyle.Regular, GraphicsUnit.Point, 134);
            txtScanCode.Location = new Point(95, 12);
            txtScanCode.Width = 1075;
            txtScanCode.Height = 40;
            txtScanCode.Multiline = true;
            txtScanCode.AcceptsReturn = true;
            txtScanCode.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtScanCode.TextChanged += TxtScanCode_TextChanged;

            pnlButtons.Location = new Point(95, 62);
            pnlButtons.Size = new Size(1075, 38);
            pnlButtons.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlButtons.WrapContents = true;
            pnlButtons.FlowDirection = FlowDirection.LeftToRight;
            pnlButtons.Margin = new Padding(0);

            btnUpload.Name = "btnUpload";
            btnUpload.Text = "上传接口";
            btnUpload.AutoSize = true;
            btnUpload.Padding = new Padding(10, 4, 10, 4);
            btnUpload.Click += BtnUpload_Click;

            btnTestUpload.Name = "btnTestUpload";
            btnTestUpload.Text = "测试连接/上传";
            btnTestUpload.AutoSize = true;
            btnTestUpload.Padding = new Padding(10, 4, 10, 4);
            btnTestUpload.Click += BtnTestUpload_Click;

            btnTestHaiXingYun.Name = "btnTestHaiXingYun";
            btnTestHaiXingYun.Text = "海行云连接测试";
            btnTestHaiXingYun.AutoSize = true;
            btnTestHaiXingYun.Padding = new Padding(10, 4, 10, 4);
            btnTestHaiXingYun.Click += BtnTestHaiXingYun_Click;

            btnPrintOuterBoxLabel.Name = "btnPrintOuterBoxLabel";
            btnPrintOuterBoxLabel.Text = "打印外箱标签";
            btnPrintOuterBoxLabel.AutoSize = true;
            btnPrintOuterBoxLabel.Padding = new Padding(10, 4, 10, 4);
            btnPrintOuterBoxLabel.Click += BtnPrintOuterBoxLabel_Click;

            btnPrintDeliveryNote.Name = "btnPrintDeliveryNote";
            btnPrintDeliveryNote.Text = "配送单打印";
            btnPrintDeliveryNote.AutoSize = true;
            btnPrintDeliveryNote.Padding = new Padding(10, 4, 10, 4);
            btnPrintDeliveryNote.Click += BtnPrintDeliveryNote_Click;

            pnlButtons.Controls.Add(btnUpload);
            pnlButtons.Controls.Add(btnTestUpload);
            pnlButtons.Controls.Add(btnTestHaiXingYun);
            pnlButtons.Controls.Add(btnPrintOuterBoxLabel);
            pnlButtons.Controls.Add(btnPrintDeliveryNote);

            ApplyRunMode();

            pnlTop.Controls.Add(lblScanCodeTitle);
            pnlTop.Controls.Add(txtScanCode);
            pnlTop.Controls.Add(pnlButtons);

            grpLabelInfo.Name = "grpLabelInfo";
            grpLabelInfo.Text = "标签信息";
            grpLabelInfo.Dock = DockStyle.Top;
            grpLabelInfo.Height = 290;
            grpLabelInfo.Padding = new Padding(10);

            tblLabelInfo.Dock = DockStyle.Fill;
            tblLabelInfo.ColumnCount = 4;
            tblLabelInfo.RowCount = 8;
            tblLabelInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            tblLabelInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblLabelInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            tblLabelInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            for (int i = 0; i < 8; i++)
            {
                tblLabelInfo.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            }

            AddTextField(0, 0, "SupplierCode", "供应商代码");
            AddTextField(0, 2, "PartNo", "零件号");
            AddTextField(1, 0, "PartName", "零件名称");
            AddTextField(2, 0, "Qty", "单包装数量");
            AddTextField(2, 2, "LotNo", "供货批次号");
            AddTextField(3, 0, "PackingSlipCardNo", "随箱卡号");
            AddTextField(3, 2, "PackageCode", "包装编号");
            AddTextField(4, 0, "PackageName", "外包装箱名");
            AddTextField(4, 2, "LayerCount", "码放层数");
            AddTextField(5, 0, "BoxCount", "箱数");
            AddTextField(5, 2, "SerialNo", "流水号");
            AddDateField(6, 0, "ProduceDate", "生产日期");
            AddDateField(6, 2, "CheckDate", "到货时间");
            AddDateField(7, 0, "CheckConfirmDate", "检验确认日期");

            grpLabelInfo.Controls.Add(tblLabelInfo);

            grpPlatformUpload.Name = "grpPlatformUpload";
            grpPlatformUpload.Text = "平台上传数据";
            grpPlatformUpload.Dock = DockStyle.Top;
            grpPlatformUpload.Height = 300;
            grpPlatformUpload.Padding = new Padding(10);

            tblPlatformUpload.Dock = DockStyle.Fill;
            tblPlatformUpload.ColumnCount = 4;
            tblPlatformUpload.RowCount = 6;
            tblPlatformUpload.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
            tblPlatformUpload.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblPlatformUpload.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
            tblPlatformUpload.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            for (int i = 0; i < 6; i++)
            {
                tblPlatformUpload.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            }

            AddTextField(tblPlatformUpload, 0, 0, "BaseNo", "基地编码");
            AddTextField(tblPlatformUpload, 0, 2, "DeliveryType", "标准出货检测（配送单-随箱卡-箱码）");
            AddTextField(tblPlatformUpload, 1, 0, "DeliveryNo", "配送单号");
            AddTextField(tblPlatformUpload, 1, 2, "SxCardSeq", "随箱卡流水号");
            AddTextField(tblPlatformUpload, 2, 0, "PackageType", "标准装箱；（箱码和零件码绑定）");
            AddDateField(tblPlatformUpload, 2, 2, "CheckTime", "检测日期");
            AddTextField(tblPlatformUpload, 3, 0, "CheckUserName", "检测人员");
            AddDateField(tblPlatformUpload, 3, 2, "PackingCreateTime", "装箱时间");
            AddTextField(tblPlatformUpload, 4, 0, "PackingCreateName", "装箱人员");
            AddWideTextField(tblPlatformUpload, 5, "MaterialBarCode", "物料流水号");

            grpPlatformUpload.Controls.Add(tblPlatformUpload);

            grpResult.Name = "grpResult";
            grpResult.Text = "上传结果";
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
            dgvResult.DataBindingComplete += DgvResult_DataBindingComplete;

            grpResult.Controls.Add(dgvResult);

            statusStrip.Items.Add(lblStatus);
            statusStrip.Dock = DockStyle.Bottom;

            lblStatus.Name = "lblStatus";
            lblStatus.Text = "就绪";

            Controls.Add(grpResult);
            Controls.Add(grpPlatformUpload);
            Controls.Add(grpLabelInfo);
            Controls.Add(pnlTop);
            Controls.Add(statusStrip);

            ResumeLayout(false);
            PerformLayout();
        }

        private void AddTextField(int rowIndex, int columnIndex, string propertyName, string displayName)
        {
            AddTextField(tblLabelInfo, rowIndex, columnIndex, propertyName, displayName);
        }

        private void AddWideTextField(int rowIndex, string propertyName, string displayName)
        {
            AddWideTextField(tblLabelInfo, rowIndex, propertyName, displayName);
        }

        private void AddWideTextField(TableLayoutPanel container, int rowIndex, string propertyName, string displayName)
        {
            var rowPanel = new Panel();
            rowPanel.Dock = DockStyle.Fill;
            rowPanel.Margin = Padding.Empty;
            rowPanel.Padding = Padding.Empty;

            var lblTitle = new System.Windows.Forms.Label();
            lblTitle.AutoSize = false;
            lblTitle.Location = new Point(0, 6);
            lblTitle.Size = new Size(220, 20);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblTitle.Text = displayName;

            var txtValue = new TextBox();
            txtValue.Name = "txt" + propertyName;
            txtValue.Location = new Point(220, 3);
            txtValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtValue.Width = Math.Max(0, rowPanel.Width - 220);
            txtValue.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 134);
            ApplyReadOnlyStyle(txtValue, propertyName);

            rowPanel.Controls.Add(lblTitle);
            rowPanel.Controls.Add(txtValue);

            container.Controls.Add(rowPanel, 0, rowIndex);
            container.SetColumnSpan(rowPanel, 4);
            _labelInfoTextBoxes[propertyName] = txtValue;
        }

        private void AddTextField(TableLayoutPanel container, int rowIndex, int columnIndex, string propertyName, string displayName)
        {
            var lblTitle = new System.Windows.Forms.Label();
            lblTitle.AutoSize = false;
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblTitle.Text = displayName;

            var txtValue = new TextBox();
            txtValue.Name = "txt" + propertyName;
            txtValue.Dock = DockStyle.Fill;
            txtValue.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 134);
            ApplyReadOnlyStyle(txtValue, propertyName);

            container.Controls.Add(lblTitle, columnIndex, rowIndex);
            container.Controls.Add(txtValue, columnIndex + 1, rowIndex);
            _labelInfoTextBoxes[propertyName] = txtValue;
        }

        private static void ApplyReadOnlyStyle(TextBox textBox, string propertyName)
        {
            if (!IsReadOnlyTextField(propertyName))
            {
                return;
            }

            textBox.ReadOnly = true;
            textBox.BackColor = Color.Gainsboro;
        }

        private static bool IsReadOnlyTextField(string propertyName)
        {
            return propertyName == "SupplierCode" ||
                   propertyName == "BaseNo" ||
                   propertyName == "DeliveryType" ||
                   propertyName == "PackageType";
        }

        private void ApplyRunMode()
        {
            bool isReleaseMode = string.Equals(
                ConfigurationManager.AppSettings["RunMode"],
                "release",
                StringComparison.OrdinalIgnoreCase);

            btnTestUpload.Visible = !isReleaseMode;
            btnTestHaiXingYun.Visible = !isReleaseMode;
        }

        private void AddDateField(int rowIndex, int columnIndex, string propertyName, string displayName)
        {
            AddDateField(tblLabelInfo, rowIndex, columnIndex, propertyName, displayName);
        }

        private void AddDateField(TableLayoutPanel container, int rowIndex, int columnIndex, string propertyName, string displayName)
        {
            var lblTitle = new System.Windows.Forms.Label();
            lblTitle.AutoSize = true;
            lblTitle.Anchor = AnchorStyles.Left;
            lblTitle.Text = displayName;

            var dateEditor = new DateTimeEditor();
            dateEditor.Name = "dte" + propertyName;
            dateEditor.Dock = DockStyle.Fill;

            container.Controls.Add(lblTitle, columnIndex, rowIndex);
            container.Controls.Add(dateEditor, columnIndex + 1, rowIndex);
            _labelInfoDateEditors[propertyName] = dateEditor;
        }

        private void BindGrid()
        {
            dgvResult.AutoGenerateColumns = false;
            dgvResult.Columns.Add(CreateTextColumn("ScanTime", "扫描时间"));
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

        private void TxtScanCode_TextChanged(object sender, EventArgs e)
        {
            if (!txtScanCode.Text.Contains("\r") && !txtScanCode.Text.Contains("\n"))
            {
                return;
            }

            ParseScanCodeToLabelInfo();
        }

        private void ParseScanCodeToLabelInfo()
        {
            string scanCode = txtScanCode.Text
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Trim();

            if (string.IsNullOrWhiteSpace(scanCode))
            {
                txtScanCode.Clear();
                txtScanCode.Focus();
                return;
            }

            var qrCodeService = new QRCode();
            List<QRCodeInfo> qrCodes = qrCodeService.GetPackingSlipCardQRCodeInfoList();

            var labelService = new XHS.BLL.Label();
            LabelInfo labelInfo = labelService.GetLabelInfo(qrCodes, scanCode);
            ApplyLabelInfoDefaults(labelInfo);

            UpdatePlatformUploadFields(labelInfo);
            UpdateLabelInfoDisplay(labelInfo);

            lblStatus.Text = "已解析条码：" + scanCode;
            txtScanCode.Clear();
            txtScanCode.Focus();
        }

        /// <summary>
        /// 根据系统配置和当前业务规则补全 LabelInfo 默认值。
        /// </summary>
        private void ApplyLabelInfoDefaults(LabelInfo labelInfo)
        {
            if (labelInfo == null)
            {
                return;
            }

            labelInfo.BaseNo = CheryPortConfig.BaseNo;
            labelInfo.SupplierCode = CheryPortConfig.SupplNo;
            labelInfo.DeliveryType = CheryPortConfig.DeliveryType;
            labelInfo.PackageType = CheryPortConfig.PackageType.ToString();
            if (string.IsNullOrWhiteSpace(labelInfo.LayerCount))
            {
                labelInfo.LayerCount = "1";
            }
            if (string.IsNullOrWhiteSpace(labelInfo.BoxCount))
            {
                labelInfo.BoxCount = "1";
            }
            if (string.IsNullOrWhiteSpace(labelInfo.SxCardSeq))
            {
                labelInfo.SxCardSeq = BuildPackingSlipSequence(labelInfo.PackingSlipCardNo, labelInfo.SerialNo);
            }
            if (string.IsNullOrWhiteSpace(labelInfo.CheckTime))
            {
                labelInfo.CheckTime = labelInfo.CheckConfirmDate;
            }
            if (string.IsNullOrWhiteSpace(labelInfo.CheckUserName))
            {
                labelInfo.CheckUserName = Environment.UserName;
            }
            if (string.IsNullOrWhiteSpace(labelInfo.PackingCreateTime))
            {
                labelInfo.PackingCreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.PackingCreateName))
            {
                labelInfo.PackingCreateName = Environment.UserName;
            }
        }

        private void UpdateLabelInfoDisplay(LabelInfo labelInfo)
        {
            foreach (KeyValuePair<string, TextBox> pair in _labelInfoTextBoxes)
            {
                string value = string.Empty;

                if (labelInfo != null)
                {
                    var property = typeof(LabelInfo).GetProperty(pair.Key);
                    if (property != null)
                    {
                        object propertyValue = property.GetValue(labelInfo, null);
                        value = propertyValue == null ? string.Empty : propertyValue.ToString();
                    }
                }

                if (pair.Key == "LayerCount" && string.IsNullOrWhiteSpace(value))
                {
                    value = "1";
                }
                if (pair.Key == "BoxCount" && string.IsNullOrWhiteSpace(value))
                {
                    value = "1";
                }

                pair.Value.Text = value;
            }

            foreach (KeyValuePair<string, DateTimeEditor> pair in _labelInfoDateEditors)
            {
                pair.Value.ClearValue();

                if (labelInfo == null)
                {
                    continue;
                }

                var property = typeof(LabelInfo).GetProperty(pair.Key);
                if (property == null)
                {
                    continue;
                }

                object propertyValue = property.GetValue(labelInfo, null);
                DateTime dateValue;
                if (propertyValue != null && DateTime.TryParse(propertyValue.ToString(), out dateValue))
                {
                    pair.Value.SetValue(dateValue);
                }
            }
        }

        /// <summary>
        /// 根据业务字段同步平台上传分区中依赖计算的字段。
        /// </summary>
        private void UpdatePlatformUploadFields(LabelInfo labelInfo)
        {
            if (labelInfo == null)
            {
                return;
            }

            if (_labelInfoTextBoxes.ContainsKey("BaseNo"))
            {
                _labelInfoTextBoxes["BaseNo"].Text = labelInfo.BaseNo;
            }
            if (_labelInfoTextBoxes.ContainsKey("DeliveryType"))
            {
                _labelInfoTextBoxes["DeliveryType"].Text = labelInfo.DeliveryType;
            }
            if (_labelInfoTextBoxes.ContainsKey("DeliveryNo"))
            {
                _labelInfoTextBoxes["DeliveryNo"].Text = labelInfo.DeliveryNo;
            }
            if (_labelInfoTextBoxes.ContainsKey("SxCardSeq"))
            {
                _labelInfoTextBoxes["SxCardSeq"].Text = labelInfo.SxCardSeq;
            }
            if (_labelInfoTextBoxes.ContainsKey("PackageType"))
            {
                _labelInfoTextBoxes["PackageType"].Text = labelInfo.PackageType;
            }
            if (_labelInfoTextBoxes.ContainsKey("CheckUserName"))
            {
                _labelInfoTextBoxes["CheckUserName"].Text = labelInfo.CheckUserName;
            }
            if (_labelInfoTextBoxes.ContainsKey("PackingCreateName"))
            {
                _labelInfoTextBoxes["PackingCreateName"].Text = labelInfo.PackingCreateName;
            }
        }

        private void AddScanResultRow(string scanCode, LabelInfo labelInfo)
        {
            _rows.Insert(0, new CheryScanDisplayRow
            {
                ScanTime = DateTime.Now,
                ScanCode = scanCode,
                SupplNo = labelInfo == null ? string.Empty : labelInfo.SupplierCode,
                BaseNo = labelInfo == null ? string.Empty : labelInfo.BaseNo,
                DeliveryNo = labelInfo == null ? string.Empty : labelInfo.DeliveryNo,
                SxCardSeq = labelInfo == null ? string.Empty : labelInfo.SxCardSeq,
                MaterialNo = labelInfo == null ? string.Empty : labelInfo.PartNo,
                MaterialName = labelInfo == null ? string.Empty : labelInfo.PartName,
                PackingCount = labelInfo == null ? string.Empty : labelInfo.Qty,
                PackageBarCode = labelInfo == null ? string.Empty : labelInfo.SerialNo,
                PackageCode = labelInfo == null ? string.Empty : labelInfo.PackageCode,
                PackageName = string.Empty,
                UploadStatus = "已解析"
            });
        }

        private void BtnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "正在上传接口...";

                LabelInfo labelInfo = BuildLabelInfoFromEditors();
                string validateMessage;
                if (!ValidateOuterPackageLabelInfo(labelInfo, out validateMessage))
                {
                    MessageBox.Show(
                        validateMessage,
                        "上传接口",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    lblStatus.Text = "上传资料不完整";
                    return;
                }

                var qrCodeService = new XHS.BLL.QRCode();
                var labelService = new XHS.BLL.Label();
                labelInfo.QrContent = labelService.GetQRCodeContents(
                    qrCodeService.GetOuterPackageQRCodeInfoList(),
                    labelInfo);

                var uploadService = new XHS.BLL.CheryUpload();
                var uploadInfo = uploadService.CreateUploadInfo(labelInfo);

                var request = CheryRequestBuilder.BuildCheckRecordRequest(uploadInfo);
                var client = new CheryHttpClient();
                var postResult = client.PostCheckRecordRaw(request);
                var result = postResult.Response;
                WriteUploadJsonLog(labelInfo == null ? string.Empty : labelInfo.PartNo, postResult == null ? string.Empty : postResult.RequestJson);
                AddUploadResultRow(labelInfo, uploadInfo, postResult);

                MessageBox.Show(
                    "返回Code：" + result.code + Environment.NewLine +
                    "返回Msg：" + result.msg,
                    "上传接口",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                lblStatus.Text = "上传完成：" + result.msg;
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

        private void BtnTestUpload_Click(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "正在生成测试数据并上传海行云接口...";

                LabelInfo labelInfo = BuildLabelInfoFromEditors();
                string validateMessage;
                if (!ValidateOuterPackageLabelInfo(labelInfo, out validateMessage))
                {
                    MessageBox.Show(
                        validateMessage,
                        "测试连接/上传",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    lblStatus.Text = "上传资料不完整";
                    return;
                }

                var qrCodeService = new XHS.BLL.QRCode();
                var labelService = new XHS.BLL.Label();
                labelInfo.QrContent = labelService.GetQRCodeContents(
                    qrCodeService.GetOuterPackageQRCodeInfoList(),
                    labelInfo);

                var uploadService = new XHS.BLL.CheryUpload();
                var uploadInfo = uploadService.CreateUploadInfo(labelInfo);
                var request = CheryRequestBuilder.BuildCheckRecordRequest(uploadInfo);

                var client = new CheryHttpClient();
                var postResult = client.PostCheckRecordRaw(request);
                var result = postResult.Response;
                WriteUploadJsonLog(labelInfo == null ? string.Empty : labelInfo.PartNo, postResult == null ? string.Empty : postResult.RequestJson);
                AddUploadResultRow(labelInfo, uploadInfo, postResult);

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

        private void AddUploadResultRow(LabelInfo labelInfo, CheryUploadInfo uploadInfo, CheryPostResult postResult)
        {
            string uploadStatus = "上传成功";
            string returnCode = string.Empty;
            string returnMsg = string.Empty;

            if (postResult != null && postResult.Response != null)
            {
                returnCode = postResult.Response.code.ToString();
                returnMsg = postResult.Response.msg;

                if (postResult.Response.code != 200)
                {
                    uploadStatus = "上传失败";
                }
            }
            else
            {
                uploadStatus = "上传失败";
            }

            _rows.Insert(0, new CheryScanDisplayRow
            {
                ScanTime = DateTime.Now,
                ScanCode = labelInfo == null ? string.Empty : labelInfo.QrContent,
                SupplNo = labelInfo == null ? string.Empty : labelInfo.SupplierCode,
                BaseNo = labelInfo == null ? string.Empty : labelInfo.BaseNo,
                DeliveryNo = uploadInfo == null ? string.Empty : uploadInfo.DeliveryNo,
                SxCardSeq = uploadInfo == null ? string.Empty : uploadInfo.SxCardSeq,
                MaterialNo = uploadInfo == null ? string.Empty : uploadInfo.MaterialNo,
                MaterialName = uploadInfo == null ? string.Empty : uploadInfo.MaterialName,
                PackingCount = uploadInfo == null ? string.Empty : uploadInfo.PackingCount,
                PackageBarCode = uploadInfo == null ? string.Empty : uploadInfo.PackageBarCode,
                PackageCode = uploadInfo == null ? string.Empty : uploadInfo.PackageCode,
                PackageName = uploadInfo == null ? string.Empty : uploadInfo.PackageName,
                CheckUserName = uploadInfo == null ? string.Empty : uploadInfo.CheckUserName,
                UploadStatus = uploadStatus,
                ReturnCode = returnCode,
                ReturnMsg = returnMsg
            });
        }

        private void DgvResult_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvResult.Rows)
            {
                row.DefaultCellStyle.BackColor = Color.White;
                row.DefaultCellStyle.ForeColor = Color.Black;

                var data = row.DataBoundItem as CheryScanDisplayRow;
                if (data == null)
                {
                    continue;
                }

                if (string.Equals(data.UploadStatus, "上传失败", StringComparison.OrdinalIgnoreCase))
                {
                    row.DefaultCellStyle.BackColor = Color.MistyRose;
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                }
            }
        }

        private void BtnPrintOuterBoxLabel_Click(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "正在生成外箱标签PDF...";

                LabelInfo labelInfo = BuildLabelInfoFromEditors();
                string validateMessage;
                if (!ValidateOuterPackageLabelInfo(labelInfo, out validateMessage))
                {
                    MessageBox.Show(
                        validateMessage,
                        "打印外箱标签",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    lblStatus.Text = "外箱标签资料不完整";
                    return;
                }

                // 外包装二维码内容统一由字段映射生成，避免再手工拼接。
                var qrCodeService = new XHS.BLL.QRCode();
                var labelService = new XHS.BLL.Label();
                List<QRCodeInfo> qrCodes = qrCodeService.GetOuterPackageQRCodeInfoList();
                string qrContent = labelService.GetQRCodeContents(qrCodes, labelInfo);
                labelInfo.QrContent = qrContent;

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

        private void BtnPrintDeliveryNote_Click(object sender, EventArgs e)
        {
            LabelInfo labelInfo = BuildLabelInfoFromEditors();
            string deliveryNo = labelInfo == null ? string.Empty : SafeValue(labelInfo.DeliveryNo);

            MessageBox.Show(
                "配送单打印按钮已添加。" + Environment.NewLine +
                "当前配送单号：" + (string.IsNullOrWhiteSpace(deliveryNo) ? "未填写" : deliveryNo) + Environment.NewLine +
                "如需接入正式打印模板，请再补充配送单版式。",
                "配送单打印",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private static string SafeValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "" : value.Trim();
        }

        /// <summary>
        /// 按“随箱卡号-流水号”规则生成平台 sxCardSeq。
        /// </summary>
        private static string BuildPackingSlipSequence(string packingSlipCardNo, string serialNo)
        {
            string safePackingSlipCardNo = SafeValue(packingSlipCardNo);
            string safeSerialNo = SafeValue(serialNo);

            if (string.IsNullOrWhiteSpace(safePackingSlipCardNo))
            {
                return safeSerialNo;
            }

            if (string.IsNullOrWhiteSpace(safeSerialNo))
            {
                return safePackingSlipCardNo;
            }

            return safePackingSlipCardNo + "-" + safeSerialNo;
        }

        /// <summary>
        /// 根据界面上的编辑值组装 LabelInfo。
        /// 打印外箱标签时直接使用当前界面内容。
        /// </summary>
        private LabelInfo BuildLabelInfoFromEditors()
        {
            var labelInfo = new LabelInfo();

            foreach (KeyValuePair<string, TextBox> pair in _labelInfoTextBoxes)
            {
                var property = typeof(LabelInfo).GetProperty(pair.Key);
                if (property == null || !property.CanWrite)
                {
                    continue;
                }

                property.SetValue(labelInfo, SafeValue(pair.Value.Text), null);
            }

            foreach (KeyValuePair<string, DateTimeEditor> pair in _labelInfoDateEditors)
            {
                var property = typeof(LabelInfo).GetProperty(pair.Key);
                if (property == null || !property.CanWrite)
                {
                    continue;
                }

                property.SetValue(labelInfo, pair.Value.GetFormattedValue(), null);
            }

            ApplyLabelInfoDefaults(labelInfo);

            return labelInfo;
        }

        /// <summary>
        /// 校验外箱标签必填字段。
        /// 只要存在空值，就提示用户先补齐后再打印。
        /// </summary>
        private static bool ValidateOuterPackageLabelInfo(LabelInfo labelInfo, out string message)
        {
            var missingFields = new List<string>();

            if (labelInfo == null)
            {
                message = "标签信息为空，不能打印。";
                return false;
            }

            if (string.IsNullOrWhiteSpace(labelInfo.BaseNo))
            {
                missingFields.Add("基地编号");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.DeliveryType))
            {
                missingFields.Add("出货检验标准");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.DeliveryNo))
            {
                missingFields.Add("配送单号");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.SxCardSeq) && labelInfo.DeliveryType != "3")
            {
                missingFields.Add("随箱卡流水号");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.PackageType))
            {
                missingFields.Add("装箱标准");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.SupplierCode))
            {
                missingFields.Add("供应商代码");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.PartNo))
            {
                missingFields.Add("零件号");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.PartName))
            {
                missingFields.Add("零件名称");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.MaterialBarCode) && labelInfo.PackageType != "2")
            {
                missingFields.Add("物料流水号");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.Qty))
            {
                missingFields.Add("单包装数量");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.LotNo))
            {
                missingFields.Add("供货批次号");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.PackingSlipCardNo))
            {
                missingFields.Add("随箱卡号");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.PackageCode) && labelInfo.PackageType != "3")
            {
                missingFields.Add("包装编号");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.PackageName) && labelInfo.PackageType != "3")
            {
                missingFields.Add("外包装箱名");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.LayerCount))
            {
                missingFields.Add("码放层数");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.BoxCount))
            {
                missingFields.Add("箱数");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.CheckUserName))
            {
                missingFields.Add("检测人员");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.PackingCreateName))
            {
                missingFields.Add("装箱人员");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.SerialNo))
            {
                missingFields.Add("流水号");
            }

            if (missingFields.Count > 0)
            {
                message = "以下字段不能为空：" + Environment.NewLine + string.Join(Environment.NewLine, missingFields);
                return false;
            }

            message = string.Empty;
            return true;
        }

        /// <summary>
        /// 记录本次上传请求 JSON，便于人工核对。
        /// 文件名规则：零件编号_yyyyMMddHHmmss.json
        /// </summary>
        private void WriteUploadJsonLog(string partNo, string requestJson)
        {
            if (string.IsNullOrWhiteSpace(requestJson))
            {
                return;
            }

            string safePartNo = string.IsNullOrWhiteSpace(partNo) ? "UnknownPart" : partNo.Trim();
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                safePartNo = safePartNo.Replace(invalidChar.ToString(), string.Empty);
            }

            string logsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            Directory.CreateDirectory(logsFolder);

            string fileName = safePartNo + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".json";
            string filePath = Path.Combine(logsFolder, fileName);

            File.WriteAllText(filePath, requestJson);
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

        private sealed class DateTimeEditor : UserControl
        {
            private readonly CheckBox chkEnabled;
            private readonly DateTimePicker dtpDate;
            private readonly NumericUpDown nudHour;
            private readonly NumericUpDown nudMinute;
            private readonly NumericUpDown nudSecond;
            private readonly System.Windows.Forms.Label lblHourMinuteSep;
            private readonly System.Windows.Forms.Label lblMinuteSecondSep;

            public DateTimeEditor()
            {
                chkEnabled = new CheckBox();
                dtpDate = new DateTimePicker();
                nudHour = new NumericUpDown();
                nudMinute = new NumericUpDown();
                nudSecond = new NumericUpDown();
                lblHourMinuteSep = new System.Windows.Forms.Label();
                lblMinuteSecondSep = new System.Windows.Forms.Label();

                Height = 24;

                chkEnabled.AutoSize = true;
                chkEnabled.Location = new Point(0, 4);
                chkEnabled.CheckedChanged += ChkEnabled_CheckedChanged;

                dtpDate.Format = DateTimePickerFormat.Custom;
                dtpDate.CustomFormat = "yyyy-MM-dd";
                dtpDate.Location = new Point(22, 0);
                dtpDate.Width = 120;

                nudHour.Minimum = 0;
                nudHour.Maximum = 23;
                nudHour.Location = new Point(150, 0);
                nudHour.Width = 42;

                lblHourMinuteSep.AutoSize = true;
                lblHourMinuteSep.Text = ":";
                lblHourMinuteSep.Location = new Point(194, 4);

                nudMinute.Minimum = 0;
                nudMinute.Maximum = 59;
                nudMinute.Location = new Point(204, 0);
                nudMinute.Width = 42;

                lblMinuteSecondSep.AutoSize = true;
                lblMinuteSecondSep.Text = ":";
                lblMinuteSecondSep.Location = new Point(248, 4);

                nudSecond.Minimum = 0;
                nudSecond.Maximum = 59;
                nudSecond.Location = new Point(258, 0);
                nudSecond.Width = 42;

                Controls.Add(chkEnabled);
                Controls.Add(dtpDate);
                Controls.Add(nudHour);
                Controls.Add(lblHourMinuteSep);
                Controls.Add(nudMinute);
                Controls.Add(lblMinuteSecondSep);
                Controls.Add(nudSecond);

                ClearValue();
            }

            public void SetValue(DateTime value)
            {
                chkEnabled.Checked = true;
                dtpDate.Value = value.Date;
                nudHour.Value = value.Hour;
                nudMinute.Value = value.Minute;
                nudSecond.Value = value.Second;
                SetEditorsEnabled(true);
            }

            public void ClearValue()
            {
                DateTime now = DateTime.Now;
                chkEnabled.Checked = false;
                dtpDate.Value = now.Date;
                nudHour.Value = now.Hour;
                nudMinute.Value = now.Minute;
                nudSecond.Value = now.Second;
                SetEditorsEnabled(false);
            }

            /// <summary>
            /// 读取当前组合控件的日期时间文本。
            /// 未勾选时返回空字符串。
            /// </summary>
            public string GetFormattedValue()
            {
                if (!chkEnabled.Checked)
                {
                    return string.Empty;
                }

                DateTime value = dtpDate.Value.Date
                    .AddHours((double)nudHour.Value)
                    .AddMinutes((double)nudMinute.Value)
                    .AddSeconds((double)nudSecond.Value);

                return value.ToString("yyyy-MM-dd HH:mm:ss");
            }

            private void ChkEnabled_CheckedChanged(object sender, EventArgs e)
            {
                SetEditorsEnabled(chkEnabled.Checked);
            }

            private void SetEditorsEnabled(bool enabled)
            {
                dtpDate.Enabled = enabled;
                nudHour.Enabled = enabled;
                nudMinute.Enabled = enabled;
                nudSecond.Enabled = enabled;
                lblHourMinuteSep.Enabled = enabled;
                lblMinuteSecondSep.Enabled = enabled;
            }
        }
    }
}
