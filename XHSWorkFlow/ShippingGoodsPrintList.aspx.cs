using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using CheryPortHelp;
using LabelHelp.Config;
using LabelHelp.Enums;
using LabelHelp.Pdf;
using LabelHelp.Services;
using ModuleWorkFlow.BLL;
using XHS.BLL;
using XHS.Model;
using XHS.Model.Label;

namespace ModuleWorkFlow
{
    /// <summary>
    /// 出货货品打印列表页面。
    /// </summary>
    public partial class ShippingGoodsPrintList : Page
    {
        private const string MenuId = "B01";
        private const string PrintTypeOuterBox = "KD标签";
        protected string menuname = "";

        private void Page_Load(object sender, EventArgs e)
        {
            menuname = new PartTmenu().findbykey(MenuId).Menuname;
            if (Master is DefaultSub master)
            {
                master.Menuname = menuname;
            }

            if (!Private.checkPrivate(this, MenuId, "PQUERY"))
            {
                return;
            }

            if (!IsPostBack)
            {
                InitializePrinterList();
                BindData();
            }
        }

        protected void lnkbutton_search_Click(object sender, EventArgs e)
        {
            Search();
        }

        protected void CheckBox_ShowAll_CheckedChanged(object sender, EventArgs e)
        {
            MainDataGrid.CurrentPageIndex = 0;
            BindData();
        }

        protected void MainDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            MainDataGrid.CurrentPageIndex = e.NewPageIndex;
            BindData();
        }

        protected void lnkbutton_print_Click(object sender, EventArgs e)
        {
            List<ShippingGoodsInfo> selectedInfos = GetSelectedShippingGoodsInfos();
            if (selectedInfos.Count == 0)
            {
                Label_Message.Text = "请先勾选需要打印的数据。";
                return;
            }

            string clientId = (hid_ClientId.Value ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(clientId))
            {
                Label_Message.Text = "未生成客户端任务号，请重新点击打印。";
                return;
            }

            List<string> duplicateMessages = new List<string>();
            foreach (ShippingGoodsInfo shippingGoodsInfo in selectedInfos)
            {
                List<PrintRecordInfo> duplicateInfos = GetDuplicatePrintRecords(
                    shippingGoodsInfo.SupplyBatchNo,
                    shippingGoodsInfo.PartNo,
                    shippingGoodsInfo.CartonNo,
                    PrintTypeOuterBox);

                if (duplicateInfos.Count > 0)
                {
                    duplicateMessages.Add(string.Format("{0}/{1}/{2}", shippingGoodsInfo.SupplyBatchNo, shippingGoodsInfo.PartNo, shippingGoodsInfo.CartonNo));
                }
            }

            if (duplicateMessages.Count > 0)
            {
                Label_Message.Text = "以下数据已存在打印任务，不能重复加入：" + string.Join("；", duplicateMessages.ToArray());
                return;
            }

            DateTime now = DateTime.Now;
            string currentUser = Session["userid"] == null ? string.Empty : Session["userid"].ToString().Trim();
            string machineId = DropDownList_PrinterName.SelectedValue.Trim();
            List<LabelInfo> labelInfos = ShippingGoodsLabelBuilder.BuildOuterBoxLabelInfos(selectedInfos);
            List<PrintRecordInfo> printRecordInfos = new List<PrintRecordInfo>();
            for (int i = 0; i < selectedInfos.Count; i++)
            {
                ShippingGoodsInfo info = selectedInfos[i];
                LabelInfo labelInfo = labelInfos[i];
                string pdfPhysicalPath = GenerateSinglePdf(labelInfo);
                string pdfDownloadUrl = BuildPdfDownloadUrl(pdfPhysicalPath);

                printRecordInfos.Add(new PrintRecordInfo
                {
                    SupplyBatchNo = SafeValue(info.SupplyBatchNo),
                    PartNo = SafeValue(info.PartNo),
                    CartonNo = SafeValue(info.CartonNo),
                    ClientId = clientId,
                    MachineId = machineId,
                    PrintType = PrintTypeOuterBox,
                    PdfUrl = pdfPhysicalPath,
                    PdfDownLoadUrl = pdfDownloadUrl,
                    PdfDownloadPath = pdfDownloadUrl,
                    LocalPath = pdfPhysicalPath,
                    Status = PrintRecordStatusInfo.Pending,
                    PrintCount = 0,
                    PrintUser = currentUser,
                    PrintTime = now,
                    CreateUser = currentUser,
                    CreateTime = now
                });
            }

            string saveMessage = InsertPrintRecords(printRecordInfos);
            if (string.IsNullOrWhiteSpace(saveMessage))
            {
                Label_Message.Text = string.Format("已生成 {0} 个独立 PDF 并加入 {0} 条待打印记录，ClientId：{1}。", printRecordInfos.Count, clientId);
                hid_ClientId.Value = string.Empty;
                BindData();
                return;
            }

            Label_Message.Text = saveMessage;
        }

        private void Search()
        {
            MainDataGrid.CurrentPageIndex = 0;
            BindData();
        }

        private void InitializePrinterList()
        {
            DropDownList_PrinterName.Items.Clear();
            DropDownList_PrinterName.Items.Add(new ListItem("PRT-01", "PRT-01"));
        }

        private void BindData()
        {
            List<ShippingGoodsInfo> shippingGoodsInfos = new ShippingGoods().GetShippingGoods(
                TextBox_PartNo.Text.Trim(),
                TextBox_PartName.Text.Trim(),
                TextBox_SupplyBatchNo.Text.Trim());

            shippingGoodsInfos = shippingGoodsInfos
                .Where(item => item != null && (!item.PrintCount.HasValue || item.PrintCount.Value == 0))
                .ToList();

            MainDataGrid.AllowPaging = !CheckBox_ShowAll.Checked;
            MainDataGrid.DataSource = shippingGoodsInfos;
            MainDataGrid.DataBind();
            Label_Message.Text = CheckBox_ShowAll.Checked
                ? string.Format("共查询到 {0} 条打印次数为 0 的出货货品数据，当前显示全部。", shippingGoodsInfos.Count)
                : string.Format("共查询到 {0} 条打印次数为 0 的出货货品数据。", shippingGoodsInfos.Count);
        }

        private List<ShippingGoodsInfo> GetSelectedShippingGoodsInfos()
        {
            List<ShippingGoodsInfo> currentShippingGoodsInfos = new ShippingGoods().GetShippingGoods(
                TextBox_PartNo.Text.Trim(),
                TextBox_PartName.Text.Trim(),
                TextBox_SupplyBatchNo.Text.Trim())
                .Where(item => item != null && (!item.PrintCount.HasValue || item.PrintCount.Value == 0))
                .ToList();

            List<ShippingGoodsInfo> selectedInfos = new List<ShippingGoodsInfo>();
            foreach (DataGridItem item in MainDataGrid.Items)
            {
                CheckBox checkBox = item.FindControl("chk_datagrid") as CheckBox;
                if (checkBox == null || !checkBox.Checked)
                {
                    continue;
                }

                long id;
                if (!long.TryParse(item.Cells[1].Text.Trim(), out id))
                {
                    continue;
                }

                ShippingGoodsInfo shippingGoodsInfo = currentShippingGoodsInfos.Find(info => info != null && info.Id.HasValue && info.Id.Value == id);
                if (shippingGoodsInfo != null)
                {
                    selectedInfos.Add(shippingGoodsInfo);
                }
            }

            return selectedInfos;
        }

        private static string SafeValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private static List<PrintRecordInfo> GetDuplicatePrintRecords(string supplyBatchNo, string partNo, string cartonNo, string printType)
        {
            XHS.BLL.PrintRecord printrecord = new XHS.BLL.PrintRecord();
            return printrecord.GetPrintRecordsByBusinessKey(supplyBatchNo, partNo, cartonNo, printType);
        }

        private static string InsertPrintRecords(List<PrintRecordInfo> printRecordInfos)
        {
            XHS.BLL.PrintRecord printrecord = new XHS.BLL.PrintRecord();
            return printrecord.InsertPrintRecord(printRecordInfos);
        }

        private string GenerateSinglePdf(LabelInfo labelInfo)
        {
            string outputFolderConfig = ConfigurationManager.AppSettings["PrintRecord.LabelOutputFolder"];
            string physicalOutputFolder = Server.MapPath(string.IsNullOrWhiteSpace(outputFolderConfig) ? "~/labeloutput" : outputFolderConfig.Trim());
            if (!Directory.Exists(physicalOutputFolder))
            {
                Directory.CreateDirectory(physicalOutputFolder);
            }

            LabelPrintConfig config = LabelPrintConfig.LoadRollPaper();
            config.OutputFolder = physicalOutputFolder;

            return new LabelPdfBuilder().GenerateSingleLabelPdf(labelInfo, LabelTemplateType.TableLabel, config);
        }

        private string BuildPdfDownloadUrl(string pdfPhysicalPath)
        {
            string applicationRootPath = Server.MapPath("~");
            string fullPdfPath = Path.GetFullPath(pdfPhysicalPath);
            string fullRootPath = Path.GetFullPath(applicationRootPath);
            if (!fullRootPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
            {
                fullRootPath += Path.DirectorySeparatorChar;
            }

            if (!fullPdfPath.StartsWith(fullRootPath, StringComparison.OrdinalIgnoreCase))
            {
                return "/labeloutput/" + Path.GetFileName(pdfPhysicalPath);
            }

            string relativePath = fullPdfPath.Substring(fullRootPath.Length)
                .Replace(Path.DirectorySeparatorChar, '/')
                .Replace(Path.AltDirectorySeparatorChar, '/');

            return "/" + relativePath.TrimStart('/');
        }

        #region Web Form Designer generated code
        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            Load += new EventHandler(Page_Load);
        }
        #endregion
    }
}
