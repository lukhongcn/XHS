using System;
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
            txtScanCode.Width = 500;
            txtScanCode.KeyDown += TxtScanCode_KeyDown;

            btnUpload.Name = "btnUpload";
            btnUpload.Text = "上传接口";
            btnUpload.AutoSize = true;
            btnUpload.Location = new Point(620, 17);
            btnUpload.Padding = new Padding(10, 4, 10, 4);
            btnUpload.Click += BtnUpload_Click;

            btnTestUpload.Name = "btnTestUpload";
            btnTestUpload.Text = "测试连接/上传";
            btnTestUpload.AutoSize = true;
            btnTestUpload.Location = new Point(730, 17);
            btnTestUpload.Padding = new Padding(10, 4, 10, 4);
            btnTestUpload.Click += BtnTestUpload_Click;

            btnTestHaiXingYun.Name = "btnTestHaiXingYun";
            btnTestHaiXingYun.Text = "海行云连接测试";
            btnTestHaiXingYun.AutoSize = true;
            btnTestHaiXingYun.Location = new Point(875, 17);
            btnTestHaiXingYun.Padding = new Padding(10, 4, 10, 4);
            btnTestHaiXingYun.Click += BtnTestHaiXingYun_Click;

            pnlTop.Controls.Add(lblScanCodeTitle);
            pnlTop.Controls.Add(txtScanCode);
            pnlTop.Controls.Add(btnUpload);
            pnlTop.Controls.Add(btnTestUpload);
            pnlTop.Controls.Add(btnTestHaiXingYun);

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
