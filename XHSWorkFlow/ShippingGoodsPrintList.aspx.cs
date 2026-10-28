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
        private const string MenuId = "B011";
        private const string PrintTypeOuterBox = "KD标签";
        protected string menuname = "";
        protected global::System.Web.UI.WebControls.Literal Literal_DownloadLink;

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

            if (Session["userid"] == null)
            {
                Response.Redirect("login.aspx");
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
            try
            {
                List<ShippingGoodsInfo> selectedInfos = GetSelectedShippingGoodsInfos();
                if (selectedInfos.Count == 0)
                {
                    Label_Message.Text = "请先勾选需要打印的数据。";
                    Literal_DownloadLink.Text = "";
                    return;
                }

                // 重复检查
                List<string> duplicateMessages = new List<string>();
                foreach (ShippingGoodsInfo shippingGoodsInfo in selectedInfos)
                {
                    List<PrintRecordInfo> duplicateInfos = GetDuplicatePrintRecords(
                        shippingGoodsInfo.SupplyBatchNo,
                        shippingGoodsInfo.PartNo,
                        shippingGoodsInfo.CartonNo,
                        shippingGoodsInfo.DeliveryNo,
                        PrintTypeOuterBox);

                    if (duplicateInfos.Count > 0)
                    {
                        duplicateMessages.Add(string.Format("{0}/{1}/{2}/{3}", shippingGoodsInfo.SupplyBatchNo, shippingGoodsInfo.PartNo, shippingGoodsInfo.DeliveryNo, shippingGoodsInfo.CartonNo));
                    }
                }

                if (duplicateMessages.Count > 0)
                {
                    Label_Message.Text = "以下数据已存在打印任务，不能重复加入：" + string.Join("；", duplicateMessages.ToArray());
                    Literal_DownloadLink.Text = "";
                    return;
                }

                // 构建标签数据
                List<LabelInfo> labelInfos = ShippingGoodsLabelBuilder.BuildOuterBoxLabelInfos(selectedInfos);

                // 生成合并 PDF（每页一张独立标签）
                string pdfPhysicalPath = GenerateCombinedLabelPdf(labelInfos);
                string pdfDownloadUrl = BuildPdfDownloadUrl(pdfPhysicalPath);
                string pdfFileName = Path.GetFileName(pdfPhysicalPath);
                long pdfFileSize = new FileInfo(pdfPhysicalPath).Length;

                // 插入打印记录
                DateTime now = DateTime.Now;
                string currentUser = Session["userid"] == null ? string.Empty : Session["userid"].ToString().Trim();
                string machineId = DropDownList_PrinterName.SelectedValue.Trim();
                string clientId = Guid.NewGuid().ToString("N").Substring(0, 8);
                List<PrintRecordInfo> printRecordInfos = new List<PrintRecordInfo>();
                for (int i = 0; i < selectedInfos.Count; i++)
                {
                    ShippingGoodsInfo info = selectedInfos[i];
                    printRecordInfos.Add(new PrintRecordInfo
                    {
                        SupplyBatchNo = SafeValue(info.SupplyBatchNo),
                        PartNo = SafeValue(info.PartNo),
                        DeliveryNo = SafeValue(info.DeliveryNo),
                        CartonNo = SafeValue(info.CartonNo),
                        ClientId = clientId,
                        MachineId = machineId,
                        PrintType = PrintTypeOuterBox,
                        PdfUrl = pdfPhysicalPath,
                        PdfDownLoadUrl = pdfDownloadUrl,
                        PdfDownloadPath = pdfDownloadUrl,
                        LocalPath = pdfPhysicalPath,
                        Status = PrintRecordStatusInfo.Completed,
                        PrintCount = 1,
                        PrintUser = currentUser,
                        PrintTime = now,
                        FirstPrintUser = currentUser,
                        FirstPrintTime = now,
                        LastPrintUser = currentUser,
                        LastPrintTime = now,
                        CreateUser = currentUser,
                        CreateTime = now
                    });
                }

                string saveMessage = InsertPrintRecords(printRecordInfos);

                // 更新 tb_ShippingGoods 的状态和打印次数
                List<string> shippingGoodsErrors = new List<string>();
                foreach (ShippingGoodsInfo info in selectedInfos)
                {
                    string error = new global::BLL.ShippingGoods().CompleteShippingGoodsPrint(
                        SafeValue(info.SupplyBatchNo),
                        SafeValue(info.PartNo),
                        SafeValue(info.CartonNo),
                        SafeValue(info.DeliveryNo));
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        shippingGoodsErrors.Add(SafeValue(info.CartonNo) + "：" + error);
                    }
                }

                // 显示下载链接
                string statusMessage = string.IsNullOrWhiteSpace(saveMessage)
                    ? "打印记录已保存，出货货品状态已更新。"
                    : saveMessage;
                if (shippingGoodsErrors.Count > 0)
                {
                    statusMessage += "（部分出货货品状态更新失败：" + string.Join("；", shippingGoodsErrors.ToArray()) + "）";
                }

                Label_Message.Text = string.Format(
                    "已生成 {0} 页标签 PDF，共 {1} 条记录。{2}",
                    labelInfos.Count,
                    printRecordInfos.Count,
                    statusMessage);

                Literal_DownloadLink.Text = string.Format(
                    "&nbsp;&nbsp;<a href=\"{0}\" target=\"_blank\" class=\"btn btn-sm btn-primary\" style=\"text-decoration:none;padding:4px 12px;\">📥 下载 PDF（{1} 页，{2:F1} KB）</a>",
                    pdfDownloadUrl,
                    labelInfos.Count,
                    pdfFileSize / 1024.0);
            }
            finally
            {
                BindData(false);
            }
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
            BindData(true);
        }

        private void BindData(bool updateMessage)
        {
            List<ShippingGoodsInfo> shippingGoodsInfos = GetPrintableShippingGoodsInfos();

            MainDataGrid.AllowPaging = !CheckBox_ShowAll.Checked;
            MainDataGrid.DataSource = shippingGoodsInfos;
            MainDataGrid.DataBind();
            if (updateMessage)
            {
                Label_Message.Text = CheckBox_ShowAll.Checked
                    ? string.Format("共查询到 {0} 条未结案且打印次数为 0 的出货货品数据，当前显示全部。", shippingGoodsInfos.Count)
                    : string.Format("共查询到 {0} 条未结案且打印次数为 0 的出货货品数据。", shippingGoodsInfos.Count);
            }
        }

        private List<ShippingGoodsInfo> GetSelectedShippingGoodsInfos()
        {
            List<ShippingGoodsInfo> currentShippingGoodsInfos = GetPrintableShippingGoodsInfos();

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

        private List<ShippingGoodsInfo> GetPrintableShippingGoodsInfos()
        {
        List<ShippingGoodsInfo> shippingGoodsInfos = new ShippingGoods().GetShippingGoods(
                TextBox_PartNo.Text.Trim(),
                TextBox_PartName.Text.Trim(),
                TextBox_SupplyBatchNo.Text.Trim(),
                "未结案");

            string deliveryNo = TextBox_DeliveryNo.Text.Trim();
            if (!string.IsNullOrWhiteSpace(deliveryNo))
            {
                shippingGoodsInfos = shippingGoodsInfos.FindAll(item =>
                    item != null &&
                    (item.DeliveryNo ?? string.Empty).IndexOf(deliveryNo, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            HashSet<string> existingPrintRecordKeys = GetExistingPrintRecordKeys(shippingGoodsInfos);
            return shippingGoodsInfos
                .Where(item => item != null &&
                    (!item.PrintCount.HasValue || item.PrintCount.Value == 0) &&
                    !existingPrintRecordKeys.Contains(BuildPrintRecordKey(
                        item.SupplyBatchNo,
                        item.PartNo,
                        item.CartonNo,
                        item.DeliveryNo,
                        item.PrintCount)))
                .ToList();
        }

        private HashSet<string> GetExistingPrintRecordKeys(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            XHS.BLL.PrintRecord printrecord = new XHS.BLL.PrintRecord();
            HashSet<string> keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (ShippingGoodsInfo shippingGoodsInfo in shippingGoodsInfos.Where(item => item != null))
            {
                List<PrintRecordInfo> printRecordInfos = printrecord.GetPrintRecordsByBusinessKey(
                    shippingGoodsInfo.SupplyBatchNo,
                    shippingGoodsInfo.PartNo,
                    shippingGoodsInfo.CartonNo,
                    shippingGoodsInfo.DeliveryNo,
                    PrintTypeOuterBox);
                if (printRecordInfos.Count > 0)
                {
                    keys.Add(BuildPrintRecordKey(
                        shippingGoodsInfo.SupplyBatchNo,
                        shippingGoodsInfo.PartNo,
                        shippingGoodsInfo.CartonNo,
                        shippingGoodsInfo.DeliveryNo,
                        shippingGoodsInfo.PrintCount));
                }
            }

            return keys;
        }

        private static string BuildPrintRecordKey(string supplyBatchNo, string partNo, string cartonNo, string deliveryNo, int? printCount)
        {
            return string.Join("|", new[]
            {
                SafeValue(supplyBatchNo),
                SafeValue(partNo),
                SafeValue(cartonNo),
                SafeValue(deliveryNo),
                (printCount ?? 0).ToString()
            });
        }

        private static string SafeValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private static List<PrintRecordInfo> GetDuplicatePrintRecords(string supplyBatchNo, string partNo, string cartonNo, string deliveryNo, string printType)
        {
            XHS.BLL.PrintRecord printrecord = new XHS.BLL.PrintRecord();
            return printrecord.GetPrintRecordsByBusinessKey(supplyBatchNo, partNo, cartonNo, deliveryNo, printType);
        }

        private static string InsertPrintRecords(List<PrintRecordInfo> printRecordInfos)
        {
            XHS.BLL.PrintRecord printrecord = new XHS.BLL.PrintRecord();
            return printrecord.InsertPrintRecord(printRecordInfos);
        }

        private string GenerateCombinedLabelPdf(List<LabelInfo> labelInfos)
        {
            string outputFolderConfig = ConfigurationManager.AppSettings["PrintRecord.LabelOutputFolder"];
            string physicalOutputFolder = Server.MapPath(string.IsNullOrWhiteSpace(outputFolderConfig) ? "~/labeloutput" : outputFolderConfig.Trim());
            if (!Directory.Exists(physicalOutputFolder))
            {
                Directory.CreateDirectory(physicalOutputFolder);
            }

            LabelPrintConfig config = LabelPrintConfig.LoadRollPaper();
            config.OutputFolder = physicalOutputFolder;

            return new LabelPdfBuilder().GenerateMultiPageSingleLabelPdf(labelInfos, LabelTemplateType.TableLabel, config);
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
                return ResolveUrl("~/labeloutput/" + Path.GetFileName(pdfPhysicalPath));
            }

            string relativePath = fullPdfPath.Substring(fullRootPath.Length)
                .Replace(Path.DirectorySeparatorChar, '/')
                .Replace(Path.AltDirectorySeparatorChar, '/');

            return ResolveUrl("~/" + relativePath.TrimStart('/'));
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
