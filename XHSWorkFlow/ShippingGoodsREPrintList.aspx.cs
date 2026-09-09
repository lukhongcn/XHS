using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using LabelHelp.Config;
using LabelHelp.Enums;
using LabelHelp.Pdf;
using ModuleWorkFlow.BLL;
using XHS.BLL;
using XHS.Model;
using XHS.Model.Label;

namespace ModuleWorkFlow
{
    /// <summary>
    /// 出货货品补打列表页面。
    /// </summary>
    public partial class ShippingGoodsREPrintList : Page
    {
        private const string MenuId = "B02";
        private const string PrintTypeOuterBox = "KD标签";
        private const string ReprintReasonControlId = "DropDownList_ReprintReason";
        private const string RowIdControlId = "hid_row_id";
        protected string menuname = "";
        protected global::System.Web.UI.WebControls.Literal Literal_DownloadLink;
        private List<ReprintReasonInfo> reprintReasonInfos;

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

        protected void MainDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                DropDownList headerDropDownList = e.Item.FindControl("DropDownList_HeaderReprintReason") as DropDownList;
                if (headerDropDownList != null)
                {
                    BindReprintReasonDropDownList(headerDropDownList);
                    headerDropDownList.Attributes["onchange"] = "applyHeaderReprintReason(this);";
                }

                return;
            }

            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            DropDownList dropDownList = e.Item.FindControl(ReprintReasonControlId) as DropDownList;
            if (dropDownList != null)
            {
                BindReprintReasonDropDownList(dropDownList);
            }
        }

        protected void lnkbutton_print_Click(object sender, EventArgs e)
        {
            try
            {
                List<ReprintShippingGoodsInfo> selectedInfos = GetSelectedShippingGoodsInfos();
                if (selectedInfos.Count == 0)
                {
                    Label_Message.Text = "请先勾选需要补打的数据。";
                    Literal_DownloadLink.Text = "";
                    return;
                }

                foreach (ReprintShippingGoodsInfo selectedInfo in selectedInfos)
                {
                    if (!selectedInfo.ReprintReasonsId.HasValue)
                    {
                        Label_Message.Text = "补打时必须选择补打原因。";
                        Literal_DownloadLink.Text = "";
                        return;
                    }
                }

                DateTime now = DateTime.Now;
                string currentUser = Session["userid"] == null ? string.Empty : Session["userid"].ToString().Trim();
                string machineId = DropDownList_PrinterName.SelectedValue.Trim();
                string clientId = Guid.NewGuid().ToString("N").Substring(0, 8);
                List<ShippingGoodsInfo> shippingGoodsInfos = selectedInfos.Select(item => item.ShippingGoodsInfo).ToList();
                List<LabelInfo> labelInfos = ShippingGoodsLabelBuilder.BuildOuterBoxLabelInfos(shippingGoodsInfos);

                // 生成合并 PDF（每页一张独立标签）
                string pdfPhysicalPath = GenerateCombinedLabelPdf(labelInfos);
                string pdfDownloadUrl = BuildPdfDownloadUrl(pdfPhysicalPath);
                long pdfFileSize = new FileInfo(pdfPhysicalPath).Length;

                Dictionary<string, int> nextPrintCounts = BuildNextPrintCounts(selectedInfos);
                List<PrintRecordInfo> printRecordInfos = new List<PrintRecordInfo>();
                for (int i = 0; i < selectedInfos.Count; i++)
                {
                    ReprintShippingGoodsInfo selectedInfo = selectedInfos[i];
                    ShippingGoodsInfo info = selectedInfo.ShippingGoodsInfo;
                    string businessKey = BuildBusinessKey(info.SupplyBatchNo, info.PartNo, info.DeliveryNo, info.CartonNo);
                    int nextPrintCount = nextPrintCounts[businessKey];

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
                        PrintCount = nextPrintCount,
                        PrintUser = currentUser,
                        PrintTime = now,
                        FirstPrintUser = currentUser,
                        FirstPrintTime = now,
                        LastPrintUser = currentUser,
                        LastPrintTime = now,
                        ReprintReason = selectedInfo.ReprintReason,
                        ReprintReasonsId = selectedInfo.ReprintReasonsId,
                        CreateUser = currentUser,
                        CreateTime = now
                    });

                    nextPrintCounts[businessKey] = nextPrintCount + 1;
                }

                string saveMessage = InsertPrintRecords(printRecordInfos);

                // 更新 tb_ShippingGoods 的打印次数
                List<string> shippingGoodsErrors = new List<string>();
                foreach (ShippingGoodsInfo info in shippingGoodsInfos)
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

                string statusMessage = string.IsNullOrWhiteSpace(saveMessage)
                    ? "补打记录已保存，出货货品状态已更新。"
                    : saveMessage;
                if (shippingGoodsErrors.Count > 0)
                {
                    statusMessage += "（部分出货货品状态更新失败：" + string.Join("；", shippingGoodsErrors.ToArray()) + "）";
                }

                Label_Message.Text = string.Format(
                    "已生成 {0} 页标签 PDF，共 {1} 条补打记录。{2}",
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
            List<ShippingGoodsInfo> shippingGoodsInfos = GetPrintableShippingGoodsInfos(true);

            MainDataGrid.AllowPaging = !CheckBox_ShowAll.Checked;
            MainDataGrid.DataSource = shippingGoodsInfos;
            MainDataGrid.DataBind();
            if (updateMessage)
            {
                Label_Message.Text = CheckBox_ShowAll.Checked
                    ? string.Format("共查询到 {0} 条未结案且打印次数大于 0 的出货货品数据，当前显示全部。", shippingGoodsInfos.Count)
                    : string.Format("共查询到 {0} 条未结案且打印次数大于 0 的出货货品数据。", shippingGoodsInfos.Count);
            }
        }

        private List<ReprintShippingGoodsInfo> GetSelectedShippingGoodsInfos()
        {
            List<ShippingGoodsInfo> currentShippingGoodsInfos = GetPrintableShippingGoodsInfos(false);

            List<ReprintShippingGoodsInfo> selectedInfos = new List<ReprintShippingGoodsInfo>();
            foreach (DataGridItem item in MainDataGrid.Items)
            {
                CheckBox checkBox = item.FindControl("chk_datagrid") as CheckBox;
                if (checkBox == null || !checkBox.Checked)
                {
                    continue;
                }

                long id;
                if (!TryGetRowId(item, out id))
                {
                    continue;
                }

                ShippingGoodsInfo shippingGoodsInfo = currentShippingGoodsInfos.Find(info => info != null && info.Id.HasValue && info.Id.Value == id);
                if (shippingGoodsInfo == null)
                {
                    continue;
                }

                ReprintReasonInfo reprintReasonInfo = GetSelectedReprintReasonInfo(item);
                selectedInfos.Add(new ReprintShippingGoodsInfo
                {
                    ShippingGoodsInfo = shippingGoodsInfo,
                    ReprintReasonsId = reprintReasonInfo == null ? (int?)null : reprintReasonInfo.Id,
                    ReprintReason = reprintReasonInfo == null ? string.Empty : SafeValue(reprintReasonInfo.ReasonName)
                });
            }

            return selectedInfos;
        }

        private List<ShippingGoodsInfo> GetPrintableShippingGoodsInfos(bool includePendingPrintCounts)
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

            List<ShippingGoodsInfo> result = shippingGoodsInfos
                .Where(item => item != null &&
                    item.PrintCount.HasValue &&
                    item.PrintCount.Value > 0)
                .ToList();

            if (includePendingPrintCounts)
            {
                ApplyPendingPrintCounts(result);
            }

            return result;
        }

        private void ApplyPendingPrintCounts(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            if (shippingGoodsInfos == null || shippingGoodsInfos.Count == 0)
            {
                return;
            }

            Dictionary<string, int> pendingPrintCounts = GetPendingPrintCounts();
            foreach (ShippingGoodsInfo shippingGoodsInfo in shippingGoodsInfos)
            {
                if (shippingGoodsInfo == null)
                {
                    continue;
                }

                int pendingCount;
                if (!pendingPrintCounts.TryGetValue(BuildBusinessKey(
                    shippingGoodsInfo.SupplyBatchNo,
                    shippingGoodsInfo.PartNo,
                    shippingGoodsInfo.DeliveryNo,
                    shippingGoodsInfo.CartonNo), out pendingCount))
                {
                    continue;
                }

                shippingGoodsInfo.PrintCount = (shippingGoodsInfo.PrintCount ?? 0) + pendingCount;
            }
        }

        private Dictionary<string, int> GetPendingPrintCounts()
        {
            XHS.BLL.PrintRecord printRecordService = new XHS.BLL.PrintRecord();
            List<PrintRecordInfo> printRecordInfos = printRecordService.GetPrintRecords(
                TextBox_SupplyBatchNo.Text.Trim(),
                TextBox_PartNo.Text.Trim(),
                string.Empty,
                PrintTypeOuterBox);

            return printRecordInfos
                .Where(item => item != null && item.Status.HasValue && item.Status.Value == PrintRecordStatusInfo.Pending)
                .GroupBy(item => BuildBusinessKey(item.SupplyBatchNo, item.PartNo, item.DeliveryNo, item.CartonNo), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase);
        }

        private Dictionary<string, int> BuildNextPrintCounts(List<ReprintShippingGoodsInfo> selectedInfos)
        {
            Dictionary<string, int> nextPrintCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            if (selectedInfos == null || selectedInfos.Count == 0)
            {
                return nextPrintCounts;
            }

            XHS.BLL.PrintRecord printRecordService = new XHS.BLL.PrintRecord();
            foreach (ReprintShippingGoodsInfo selectedInfo in selectedInfos)
            {
                if (selectedInfo == null || selectedInfo.ShippingGoodsInfo == null)
                {
                    continue;
                }

                ShippingGoodsInfo shippingGoodsInfo = selectedInfo.ShippingGoodsInfo;
                string businessKey = BuildBusinessKey(
                    shippingGoodsInfo.SupplyBatchNo,
                    shippingGoodsInfo.PartNo,
                    shippingGoodsInfo.DeliveryNo,
                    shippingGoodsInfo.CartonNo);
                if (nextPrintCounts.ContainsKey(businessKey))
                {
                    continue;
                }

                List<PrintRecordInfo> existedPrintRecords = printRecordService.GetPrintRecordsByBusinessKey(
                    SafeValue(shippingGoodsInfo.SupplyBatchNo),
                    SafeValue(shippingGoodsInfo.PartNo),
                    SafeValue(shippingGoodsInfo.CartonNo),
                    SafeValue(shippingGoodsInfo.DeliveryNo),
                    PrintTypeOuterBox);
                int maxPrintCount = existedPrintRecords == null
                    ? 0
                    : existedPrintRecords
                        .Where(item => item != null && item.PrintCount.HasValue)
                        .Select(item => item.PrintCount.Value)
                        .DefaultIfEmpty(0)
                        .Max();

                nextPrintCounts[businessKey] = maxPrintCount + 1;
            }

            return nextPrintCounts;
        }

        private static string SafeValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private static string BuildBusinessKey(string supplyBatchNo, string partNo, string deliveryNo, string cartonNo)
        {
            return string.Join("|", new[]
            {
                SafeValue(supplyBatchNo),
                SafeValue(partNo),
                SafeValue(deliveryNo),
                SafeValue(cartonNo)
            });
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

        private void BindReprintReasonDropDownList(DropDownList dropDownList)
        {
            dropDownList.Items.Clear();
            dropDownList.Items.Add(new ListItem("请选择", string.Empty));
            foreach (ReprintReasonInfo reprintReasonInfo in GetReprintReasonInfos())
            {
                if (reprintReasonInfo == null || !reprintReasonInfo.Id.HasValue)
                {
                    continue;
                }

                dropDownList.Items.Add(new ListItem(SafeValue(reprintReasonInfo.ReasonName), reprintReasonInfo.Id.Value.ToString()));
            }
        }

        private ReprintReasonInfo GetSelectedReprintReasonInfo(DataGridItem item)
        {
            DropDownList dropDownList = item.FindControl(ReprintReasonControlId) as DropDownList;
            int reprintReasonsId;
            if (dropDownList == null || !int.TryParse(dropDownList.SelectedValue, out reprintReasonsId))
            {
                return null;
            }

            return GetReprintReasonInfos().FirstOrDefault(info => info != null && info.Id.HasValue && info.Id.Value == reprintReasonsId);
        }

        private static bool TryGetRowId(DataGridItem item, out long id)
        {
            id = 0;
            HiddenField hiddenField = item.FindControl(RowIdControlId) as HiddenField;
            if (hiddenField == null)
            {
                return false;
            }

            return long.TryParse(SafeValue(hiddenField.Value), out id);
        }

        private List<ReprintReasonInfo> GetReprintReasonInfos()
        {
            if (reprintReasonInfos == null)
            {
                reprintReasonInfos = new ReprintReason().GetEnabledReprintReasons();
            }

            return reprintReasonInfos ?? new List<ReprintReasonInfo>();
        }

        private class ReprintShippingGoodsInfo
        {
            public ShippingGoodsInfo ShippingGoodsInfo { get; set; }

            public int? ReprintReasonsId { get; set; }

            public string ReprintReason { get; set; }
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
