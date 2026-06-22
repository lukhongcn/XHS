using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using BLL;
using CheryPortHelp;
using LabelHelp.Enums;
using LabelHelp.Services;
using XHS.Model;
using XHS.Model.Label;

namespace CheryCheckSystem.WinForms
{
    public class ShippingGoodsWorkbenchForm : Form
    {
        private const int PageSize = 20;
        private const string SelectColumnName = "colSelected";
        private readonly WorkflowLoginContext _loginContext;
        private readonly BindingSource _bindingSource = new BindingSource();
        private List<ShippingGoodsInfo> _shippingGoodsInfos = new List<ShippingGoodsInfo>();
        private int _currentPageIndex;
        private bool _isAllSelected;
        private bool _isUpdatingHeaderCheckBox;
        private MenuStrip menuMain;
        private ToolStripMenuItem menuPrint;
        private ToolStripMenuItem menuPrintOuterBox;
        private ToolStripMenuItem menuPrintInnerBox;
        private ToolStripMenuItem menuUpload;
        private Panel pnlHeader;
        private Label lblPageTitle;
        private Label lblCurrentUser;
        private GroupBox grpSearch;
        private TableLayoutPanel tblSearch;
        private Label lblPartNo;
        private Label lblPartName;
        private Label lblSupplyBatchNo;
        private TextBox txtPartNo;
        private TextBox txtPartName;
        private TextBox txtSupplyBatchNo;
        private CheckBox chkShowAll;
        private Button btnSearch;
        private GroupBox grpList;
        private DataGridView dgvShippingGoods;
        private CheckBox chkSelectAllHeader;
        private Panel pnlPager;
        private FlowLayoutPanel pnlPagerButtons;
        private Button btnPreviousPage;
        private Button btnNextPage;
        private Label lblPageInfo;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblMessage;

        public ShippingGoodsWorkbenchForm(WorkflowLoginContext loginContext)
        {
            _loginContext = loginContext ?? throw new ArgumentNullException(nameof(loginContext));
            InitializeComponent();
            SwitchPage("外箱标签打印");
        }

        private void InitializeComponent()
        {
            menuMain = new MenuStrip();
            menuPrint = new ToolStripMenuItem();
            menuPrintOuterBox = new ToolStripMenuItem();
            menuPrintInnerBox = new ToolStripMenuItem();
            menuUpload = new ToolStripMenuItem();
            pnlHeader = new Panel();
            lblPageTitle = new Label();
            lblCurrentUser = new Label();
            grpSearch = new GroupBox();
            tblSearch = new TableLayoutPanel();
            lblPartNo = new Label();
            lblPartName = new Label();
            lblSupplyBatchNo = new Label();
            txtPartNo = new TextBox();
            txtPartName = new TextBox();
            txtSupplyBatchNo = new TextBox();
            chkShowAll = new CheckBox();
            btnSearch = new Button();
            grpList = new GroupBox();
            dgvShippingGoods = new DataGridView();
            chkSelectAllHeader = new CheckBox();
            pnlPager = new Panel();
            pnlPagerButtons = new FlowLayoutPanel();
            btnPreviousPage = new Button();
            btnNextPage = new Button();
            lblPageInfo = new Label();
            statusStrip = new StatusStrip();
            lblMessage = new ToolStripStatusLabel();

            SuspendLayout();

            Text = "出货货品工作台";
            Name = "ShippingGoodsWorkbenchForm";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1280, 760);

            menuMain.Items.AddRange(new ToolStripItem[] { menuPrint, menuUpload });
            menuMain.Dock = DockStyle.Top;

            menuPrint.Text = "打印";
            menuPrint.DropDownItems.AddRange(new ToolStripItem[] { menuPrintOuterBox, menuPrintInnerBox });

            menuPrintOuterBox.Text = "外箱标签打印";
            menuPrintOuterBox.Click += MenuPrintOuterBox_Click;

            menuPrintInnerBox.Text = "内箱标签打印";
            menuPrintInnerBox.Click += MenuPrintInnerBox_Click;

            menuUpload.Text = "上传";
            menuUpload.Click += MenuUpload_Click;

            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 70;
            pnlHeader.Padding = new Padding(16, 10, 16, 10);

            lblPageTitle.AutoSize = true;
            lblPageTitle.Font = new Font("Microsoft YaHei UI", 16F, FontStyle.Bold, GraphicsUnit.Point, 134);
            lblPageTitle.Location = new Point(16, 18);
            lblPageTitle.Text = "外箱标签打印";

            lblCurrentUser.AutoSize = true;
            lblCurrentUser.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblCurrentUser.Location = new Point(980, 25);
            lblCurrentUser.Text = string.Format("当前登录：{0}", _loginContext.DisplayName);

            pnlHeader.Controls.Add(lblPageTitle);
            pnlHeader.Controls.Add(lblCurrentUser);

            grpSearch.Text = "搜索条件";
            grpSearch.Dock = DockStyle.Top;
            grpSearch.Height = 90;
            grpSearch.Padding = new Padding(12);

            tblSearch.ColumnCount = 9;
            tblSearch.RowCount = 1;
            tblSearch.Dock = DockStyle.Fill;
            tblSearch.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            tblSearch.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tblSearch.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            tblSearch.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tblSearch.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            tblSearch.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 23.34F));
            tblSearch.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95F));
            tblSearch.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            tblSearch.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 1F));
            tblSearch.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            lblPartNo.Text = "零件编号";
            lblPartNo.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblPartNo.AutoSize = true;
            lblPartNo.TextAlign = ContentAlignment.MiddleRight;

            txtPartNo.Dock = DockStyle.Fill;
            txtPartNo.Margin = new Padding(3, 18, 12, 18);
            txtPartNo.KeyDown += SearchInput_KeyDown;

            lblPartName.Text = "零件名称";
            lblPartName.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblPartName.AutoSize = true;
            lblPartName.TextAlign = ContentAlignment.MiddleRight;

            txtPartName.Dock = DockStyle.Fill;
            txtPartName.Margin = new Padding(3, 18, 12, 18);
            txtPartName.KeyDown += SearchInput_KeyDown;

            lblSupplyBatchNo.Text = "批次";
            lblSupplyBatchNo.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblSupplyBatchNo.AutoSize = true;
            lblSupplyBatchNo.TextAlign = ContentAlignment.MiddleRight;

            txtSupplyBatchNo.Dock = DockStyle.Fill;
            txtSupplyBatchNo.Margin = new Padding(3, 18, 12, 18);
            txtSupplyBatchNo.KeyDown += SearchInput_KeyDown;

            chkShowAll.Text = "显示所有";
            chkShowAll.AutoSize = true;
            chkShowAll.Anchor = AnchorStyles.Left;
            chkShowAll.CheckedChanged += ChkShowAll_CheckedChanged;

            btnSearch.Text = "搜索";
            btnSearch.Dock = DockStyle.Fill;
            btnSearch.Margin = new Padding(6, 14, 6, 14);
            btnSearch.Click += BtnSearch_Click;

            tblSearch.Controls.Add(lblPartNo, 0, 0);
            tblSearch.Controls.Add(txtPartNo, 1, 0);
            tblSearch.Controls.Add(lblPartName, 2, 0);
            tblSearch.Controls.Add(txtPartName, 3, 0);
            tblSearch.Controls.Add(lblSupplyBatchNo, 4, 0);
            tblSearch.Controls.Add(txtSupplyBatchNo, 5, 0);
            tblSearch.Controls.Add(chkShowAll, 6, 0);
            tblSearch.Controls.Add(btnSearch, 7, 0);
            grpSearch.Controls.Add(tblSearch);

            grpList.Text = "列表";
            grpList.Dock = DockStyle.Fill;
            grpList.Padding = new Padding(12);

            dgvShippingGoods.Dock = DockStyle.Fill;
            dgvShippingGoods.ReadOnly = false;
            dgvShippingGoods.AllowUserToAddRows = false;
            dgvShippingGoods.AllowUserToDeleteRows = false;
            dgvShippingGoods.AllowUserToOrderColumns = false;
            dgvShippingGoods.AutoGenerateColumns = false;
            dgvShippingGoods.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvShippingGoods.RowHeadersVisible = false;
            dgvShippingGoods.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvShippingGoods.MultiSelect = false;
            dgvShippingGoods.DataSource = _bindingSource;
            dgvShippingGoods.CurrentCellDirtyStateChanged += DgvShippingGoods_CurrentCellDirtyStateChanged;
            dgvShippingGoods.CellValueChanged += DgvShippingGoods_CellValueChanged;
            dgvShippingGoods.DataBindingComplete += DgvShippingGoods_DataBindingComplete;
            dgvShippingGoods.Scroll += DgvShippingGoods_Scroll;
            dgvShippingGoods.ColumnWidthChanged += DgvShippingGoods_ColumnWidthChanged;
            dgvShippingGoods.Resize += DgvShippingGoods_Resize;

            AddCheckBoxColumn();
            AddTextColumn("SupplierCode", "供应商代码");
            AddTextColumn("PartNo", "零件编号");
            AddTextColumn("PartChineseName", "零件中文名称");
            AddTextColumn("PartEnglishName", "零件英文名称");
            AddTextColumn("Quantity", "数量");
            AddTextColumn("SupplyBatchNo", "供货批次号");
            AddTextColumn("CartonNo", "纸箱编号");
            AddTextColumn("Status", "状态");
            AddTextColumn("PrintCount", "打印次数");
            AddTextColumn("SingleBoxGrossWeight", "单箱毛重");
            AddTextColumn("ProductionDate", "生产日期", "yyyy-MM-dd");
            AddTextColumn("InspectionConfirmDate", "检验确认日期", "yyyy-MM-dd");
            AddTextColumn("Creater", "创建人");
            AddTextColumn("CreatDate", "创建时间", "yyyy-MM-dd HH:mm:ss");

            pnlPager.Dock = DockStyle.Bottom;
            pnlPager.Height = 46;
            pnlPager.Padding = new Padding(0, 8, 0, 0);

            lblPageInfo.AutoSize = true;
            lblPageInfo.Location = new Point(0, 13);
            lblPageInfo.Text = "第 1 页 / 共 1 页";

            pnlPagerButtons.Dock = DockStyle.Right;
            pnlPagerButtons.AutoSize = true;
            pnlPagerButtons.FlowDirection = FlowDirection.LeftToRight;
            pnlPagerButtons.WrapContents = false;

            btnPreviousPage.Text = "上一页";
            btnPreviousPage.AutoSize = true;
            btnPreviousPage.Click += BtnPreviousPage_Click;

            btnNextPage.Text = "下一页";
            btnNextPage.AutoSize = true;
            btnNextPage.Click += BtnNextPage_Click;

            pnlPagerButtons.Controls.Add(btnPreviousPage);
            pnlPagerButtons.Controls.Add(btnNextPage);
            pnlPager.Controls.Add(lblPageInfo);
            pnlPager.Controls.Add(pnlPagerButtons);

            grpList.Controls.Add(dgvShippingGoods);
            grpList.Controls.Add(pnlPager);

            chkSelectAllHeader.AutoSize = true;
            chkSelectAllHeader.BackColor = Color.Transparent;
            chkSelectAllHeader.CheckedChanged += ChkSelectAllHeader_CheckedChanged;
            dgvShippingGoods.Controls.Add(chkSelectAllHeader);

            statusStrip.Items.Add(lblMessage);
            statusStrip.Dock = DockStyle.Bottom;
            lblMessage.Text = "就绪";

            Controls.Add(grpList);
            Controls.Add(grpSearch);
            Controls.Add(pnlHeader);
            Controls.Add(statusStrip);
            Controls.Add(menuMain);
            MainMenuStrip = menuMain;

            ResumeLayout(false);
            PerformLayout();
        }

        private void AddCheckBoxColumn()
        {
            DataGridViewCheckBoxColumn column = new DataGridViewCheckBoxColumn();
            column.Name = SelectColumnName;
            column.HeaderText = string.Empty;
            column.Width = 55;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column.ReadOnly = false;
            column.Resizable = DataGridViewTriState.False;
            dgvShippingGoods.Columns.Add(column);
        }

        private void AddTextColumn(string dataPropertyName, string headerText, string format = null)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = dataPropertyName;
            column.HeaderText = headerText;
            column.Name = "col" + dataPropertyName;
            column.ReadOnly = true;
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            if (!string.IsNullOrWhiteSpace(format))
            {
                column.DefaultCellStyle.Format = format;
            }

            dgvShippingGoods.Columns.Add(column);
        }

        private void MenuPrintOuterBox_Click(object sender, EventArgs e)
        {
            SwitchPage("外箱标签打印");
        }

        private void MenuPrintInnerBox_Click(object sender, EventArgs e)
        {
            PrintSelectedInnerBoxLabels();
        }

        private void MenuUpload_Click(object sender, EventArgs e)
        {
            SwitchPage("上传");
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void SearchInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Search();
            }
        }

        private void SwitchPage(string pageTitle)
        {
            lblPageTitle.Text = pageTitle;
            grpList.Text = pageTitle + "列表";
            BindData();
        }

        private void Search()
        {
            _currentPageIndex = 0;
            BindData();
        }

        private void BindData()
        {
            _shippingGoodsInfos = new ShippingGoods().GetShippingGoods(
                txtPartNo.Text.Trim(),
                txtPartName.Text.Trim(),
                txtSupplyBatchNo.Text.Trim());

            BindPage();
        }

        private void BindPage()
        {
            int totalCount = _shippingGoodsInfos.Count;
            if (chkShowAll.Checked)
            {
                _bindingSource.DataSource = _shippingGoodsInfos.ToList();
                ResetPageSelection();
                pnlPager.Visible = false;
                lblMessage.Text = string.Format("共查询到 {0} 条出货货品数据，当前显示全部。", totalCount);
                return;
            }

            int totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
            if (_currentPageIndex >= totalPages)
            {
                _currentPageIndex = totalPages - 1;
            }

            List<ShippingGoodsInfo> currentPageData = _shippingGoodsInfos
                .Skip(_currentPageIndex * PageSize)
                .Take(PageSize)
                .ToList();

            _bindingSource.DataSource = currentPageData;
            ResetPageSelection();
            pnlPager.Visible = true;
            lblPageInfo.Text = string.Format("第 {0} 页 / 共 {1} 页", _currentPageIndex + 1, totalPages);
            btnPreviousPage.Enabled = _currentPageIndex > 0;
            btnNextPage.Enabled = _currentPageIndex < totalPages - 1;
            lblMessage.Text = string.Format("共查询到 {0} 条出货货品数据，当前显示第 {1} 页。", totalCount, _currentPageIndex + 1);
        }

        private void ChkShowAll_CheckedChanged(object sender, EventArgs e)
        {
            _currentPageIndex = 0;
            BindPage();
        }

        private void ResetPageSelection()
        {
            _isAllSelected = false;
            foreach (DataGridViewRow row in dgvShippingGoods.Rows)
            {
                if (!row.IsNewRow)
                {
                    row.Cells[SelectColumnName].Value = false;
                }
            }

            SyncHeaderCheckBox();
        }

        private void SetAllRowsSelected(bool isSelected)
        {
            foreach (DataGridViewRow row in dgvShippingGoods.Rows)
            {
                if (!row.IsNewRow)
                {
                    row.Cells[SelectColumnName].Value = isSelected;
                }
            }

            _isAllSelected = isSelected;
            SyncHeaderCheckBox();
            dgvShippingGoods.RefreshEdit();
        }

        private void ChkSelectAllHeader_CheckedChanged(object sender, EventArgs e)
        {
            if (_isUpdatingHeaderCheckBox)
            {
                return;
            }

            SetAllRowsSelected(chkSelectAllHeader.Checked);
        }

        private void DgvShippingGoods_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvShippingGoods.IsCurrentCellDirty)
            {
                dgvShippingGoods.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void DgvShippingGoods_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || dgvShippingGoods.Columns[e.ColumnIndex].Name != SelectColumnName)
            {
                return;
            }

            _isAllSelected = dgvShippingGoods.Rows.Cast<DataGridViewRow>()
                .Where(row => !row.IsNewRow)
                .All(row => Convert.ToBoolean(row.Cells[SelectColumnName].Value ?? false));

            SyncHeaderCheckBox();
        }

        private void DgvShippingGoods_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            UpdateHeaderCheckBoxLocation();
            SyncHeaderCheckBox();
        }

        private void DgvShippingGoods_Scroll(object sender, ScrollEventArgs e)
        {
            UpdateHeaderCheckBoxLocation();
        }

        private void DgvShippingGoods_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (e.Column != null && e.Column.Name == SelectColumnName)
            {
                UpdateHeaderCheckBoxLocation();
            }
        }

        private void DgvShippingGoods_Resize(object sender, EventArgs e)
        {
            UpdateHeaderCheckBoxLocation();
        }

        private void UpdateHeaderCheckBoxLocation()
        {
            if (!dgvShippingGoods.Columns.Contains(SelectColumnName))
            {
                return;
            }

            Rectangle headerCellRectangle = dgvShippingGoods.GetCellDisplayRectangle(
                dgvShippingGoods.Columns[SelectColumnName].Index,
                -1,
                true);

            int x = headerCellRectangle.X + (headerCellRectangle.Width - chkSelectAllHeader.Width) / 2;
            int y = headerCellRectangle.Y + (headerCellRectangle.Height - chkSelectAllHeader.Height) / 2;
            chkSelectAllHeader.Location = new Point(Math.Max(x, 0), Math.Max(y, 0));
            chkSelectAllHeader.Visible = headerCellRectangle.Width > 0 && headerCellRectangle.Height > 0;
        }

        private void SyncHeaderCheckBox()
        {
            bool hasRows = dgvShippingGoods.Rows.Cast<DataGridViewRow>().Any(row => !row.IsNewRow);
            _isUpdatingHeaderCheckBox = true;
            chkSelectAllHeader.Checked = hasRows && _isAllSelected;
            _isUpdatingHeaderCheckBox = false;
        }

        private void BtnPreviousPage_Click(object sender, EventArgs e)
        {
            if (_currentPageIndex <= 0)
            {
                return;
            }

            _currentPageIndex--;
            BindPage();
        }

        private void BtnNextPage_Click(object sender, EventArgs e)
        {
            int totalPages = Math.Max(1, (int)Math.Ceiling(_shippingGoodsInfos.Count / (double)PageSize));
            if (_currentPageIndex >= totalPages - 1)
            {
                return;
            }

            _currentPageIndex++;
            BindPage();
        }

        private void PrintSelectedInnerBoxLabels()
        {
            try
            {
                List<ShippingGoodsInfo> selectedItems = GetSelectedShippingGoodsInfos();
                if (selectedItems.Count == 0)
                {
                    MessageBox.Show(
                        "请先勾选要打印的标签数据。",
                        "内箱标签打印",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    lblMessage.Text = "未选择需要打印的标签数据";
                    return;
                }

                List<LabelInfo> labelInfos = new List<LabelInfo>();
                for (int i = 0; i < selectedItems.Count; i++)
                {
                    ShippingGoodsInfo item = selectedItems[i];
                    LabelInfo labelInfo = BuildLabelInfoFromShippingGoods(item);
                    string validateMessage;
                    if (!ValidateTableLabelInfo(labelInfo, out validateMessage))
                    {
                        MessageBox.Show(
                            string.Format("第 {0} 条标签数据不完整：{1}", i + 1, validateMessage),
                            "内箱标签打印",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        lblMessage.Text = "标签资料不完整，无法打印";
                        return;
                    }

                    labelInfos.Add(labelInfo);
                }

                lblPageTitle.Text = "内箱标签打印";
                grpList.Text = "内箱标签打印列表";
                lblMessage.Text = "正在生成内箱标签PDF...";

                LabelPrintService service = new LabelPrintService();
                string pdfPath = service.Generate(
                    labelInfos,
                    LabelTemplateType.TableLabel,
                    LabelPrintMode.RollPdf);

                lblMessage.Text = "内箱标签卷纸PDF已生成：" + pdfPath;

                TryOpenPdf(pdfPath);

                string printerName = GetDefaultPrinterName();
                service.Print(
                    labelInfos,
                    LabelTemplateType.TableLabel,
                    LabelPrintMode.LabelPrinter,
                    printerName);

                MessageBox.Show(
                    "内箱标签卷纸PDF已生成：" + Environment.NewLine +
                    pdfPath + Environment.NewLine + Environment.NewLine +
                    "已调用标签打印流程。" + Environment.NewLine +
                    "当前打印机：" + (string.IsNullOrWhiteSpace(printerName) ? "默认打印机未获取到" : printerName),
                    "内箱标签打印",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                lblMessage.Text = "内箱标签处理完成：" + pdfPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "内箱标签打印异常：" + ex.Message,
                    "错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                lblMessage.Text = "内箱标签打印异常：" + ex.Message;
            }
        }

        private List<ShippingGoodsInfo> GetSelectedShippingGoodsInfos()
        {
            return dgvShippingGoods.Rows
                .Cast<DataGridViewRow>()
                .Where(row => !row.IsNewRow && Convert.ToBoolean(row.Cells[SelectColumnName].Value ?? false))
                .Select(row => row.DataBoundItem as ShippingGoodsInfo)
                .Where(info => info != null)
                .ToList();
        }

        private static LabelInfo BuildLabelInfoFromShippingGoods(ShippingGoodsInfo info)
        {
            LabelInfo labelInfo = new LabelInfo
            {
                BaseNo = CheryPortConfig.BaseNo,
                DeliveryType = CheryPortConfig.DeliveryType,
                PackageType = CheryPortConfig.PackageType.ToString(),
                SupplierCode = GetPreferredSupplierCode(info),
                PartNo = SafeValue(info == null ? null : info.PartNo),
                PartName = SafeValue(info == null ? null : info.PartChineseName),
                Qty = info != null && info.Quantity.HasValue ? info.Quantity.Value.ToString() : string.Empty,
                LotNo = SafeValue(info == null ? null : info.SupplyBatchNo),
                LayerCount = info != null && info.StackLayerCount.HasValue ? info.StackLayerCount.Value.ToString() : "1",
                ProduceDate = FormatDate(info == null ? null : info.ProductionDate),
                CheckConfirmDate = FormatDate(info == null ? null : info.InspectionConfirmDate),
                PackageCode = SafeValue(info == null ? null : info.CartonNo),
                BoxCount = "1"
            };

            if (string.IsNullOrWhiteSpace(labelInfo.CheckConfirmDate))
            {
                labelInfo.CheckConfirmDate = labelInfo.ProduceDate;
            }

            XHS.BLL.Label labelService = new XHS.BLL.Label();
            XHS.BLL.QRCode qrCodeService = new XHS.BLL.QRCode();
            labelInfo.QrContent = labelService.GetQRCodeContents(
                qrCodeService.GetOuterPackageQRCodeInfoList(),
                labelInfo);

            return labelInfo;
        }

        private static bool ValidateTableLabelInfo(LabelInfo labelInfo, out string message)
        {
            List<string> missingFields = new List<string>();

            if (labelInfo == null)
            {
                message = "标签信息为空。";
                return false;
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
            if (string.IsNullOrWhiteSpace(labelInfo.Qty))
            {
                missingFields.Add("数量");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.LotNo))
            {
                missingFields.Add("供货批次号");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.LayerCount))
            {
                missingFields.Add("码放层数");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.ProduceDate))
            {
                missingFields.Add("生产日期");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.CheckConfirmDate))
            {
                missingFields.Add("检验确认日期");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.PackageCode))
            {
                missingFields.Add("纸箱编号");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.QrContent))
            {
                missingFields.Add("二维码内容");
            }

            if (missingFields.Count > 0)
            {
                message = string.Join("、", missingFields);
                return false;
            }

            message = string.Empty;
            return true;
        }

        private static string GetPreferredSupplierCode(ShippingGoodsInfo info)
        {
            string supplierCode = SafeValue(info == null ? null : info.SupplierCode);
            if (!string.IsNullOrWhiteSpace(supplierCode))
            {
                return supplierCode;
            }

            return CheryPortConfig.SupplNo;
        }

        private static string SafeValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private static string FormatDate(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("yyyy/M/d") : string.Empty;
        }

        private static void TryOpenPdf(string pdfPath)
        {
            if (string.IsNullOrWhiteSpace(pdfPath))
            {
                return;
            }

            try
            {
                Process.Start(pdfPath);
            }
            catch
            {
            }
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
    }
}
