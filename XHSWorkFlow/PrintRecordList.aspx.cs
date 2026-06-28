using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using ModuleWorkFlow.BLL;
using XHS.Model;

namespace ModuleWorkFlow
{
    /// <summary>
    /// 打印记录浏览页面。
    /// </summary>
    public partial class PrintRecordList : Page
    {
        private const string MenuId = "B03";
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

            if (Session["userid"] == null)
            {
                Response.Redirect("login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                InitializeFiltersFromQueryString();
                BindData();
            }
        }

        protected void lnkbutton_search_Click(object sender, EventArgs e)
        {
            Search();
        }

        protected void MainDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            MainDataGrid.CurrentPageIndex = e.NewPageIndex;
            BindData();
        }

        protected void MainDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            PrintRecordGroupViewModel group = e.Item.DataItem as PrintRecordGroupViewModel;
            if (group == null)
            {
                return;
            }

            Label labelExpand = e.Item.FindControl("Label_Expand") as Label;
            Panel panelDetails = e.Item.FindControl("Panel_Details") as Panel;
            Repeater repeaterDetails = e.Item.FindControl("Repeater_PrintRecordDetails") as Repeater;

            if (labelExpand != null)
            {
                if (group.CanExpand)
                {
                    labelExpand.Attributes["onclick"] = string.Format("togglePrintRecordGroup('{0}', this);", group.GroupRowId);
                    labelExpand.Style["display"] = "inline-block";
                }
                else
                {
                    labelExpand.Text = string.Empty;
                    labelExpand.Style["display"] = "none";
                }
            }

            if (panelDetails != null)
            {
                panelDetails.Style["display"] = "none";
            }

            if (repeaterDetails != null)
            {
                repeaterDetails.DataSource = group.Details;
                repeaterDetails.DataBind();
            }
        }

        private void Search()
        {
            MainDataGrid.CurrentPageIndex = 0;
            BindData();
        }

        private void InitializeFiltersFromQueryString()
        {
            string supplyBatchNo = Request.QueryString["supplyBatchNo"];
            if (!string.IsNullOrWhiteSpace(supplyBatchNo))
            {
                TextBox_SupplyBatchNo.Text = supplyBatchNo.Trim();
            }

            string partNo = Request.QueryString["partNo"];
            if (!string.IsNullOrWhiteSpace(partNo))
            {
                TextBox_PartNo.Text = partNo.Trim();
            }

            string cartonNo = Request.QueryString["cartonNo"];
            if (!string.IsNullOrWhiteSpace(cartonNo))
            {
                TextBox_CartonNo.Text = cartonNo.Trim();
            }

            string printType = Request.QueryString["printType"];
            if (!string.IsNullOrWhiteSpace(printType))
            {
                TextBox_PrintType.Text = printType.Trim();
            }
        }

        private void BindData()
        {
            XHS.BLL.PrintRecord printRecordService = new XHS.BLL.PrintRecord();
            List<PrintRecordInfo> printRecordInfos = printRecordService.GetPrintRecords(
                TextBox_SupplyBatchNo.Text.Trim(),
                TextBox_PartNo.Text.Trim(),
                TextBox_CartonNo.Text.Trim(),
                TextBox_PrintType.Text.Trim());

            List<PrintRecordGroupViewModel> groups = BuildGroups(printRecordInfos);
            MainDataGrid.DataSource = groups;
            MainDataGrid.DataBind();
            Label_Message.Text = string.Format("共查询到 {0} 组打印记录，明细 {1} 条。", groups.Count, printRecordInfos.Count);
        }

        private static List<PrintRecordGroupViewModel> BuildGroups(List<PrintRecordInfo> printRecordInfos)
        {
            List<PrintRecordInfo> safePrintRecordInfos = printRecordInfos == null
                ? new List<PrintRecordInfo>()
                : printRecordInfos.Where(item => item != null).ToList();

            return safePrintRecordInfos
                .GroupBy(item => BuildBusinessKey(item), StringComparer.OrdinalIgnoreCase)
                .Select((group, index) =>
                {
                    List<PrintRecordInfo> details = group
                        .OrderByDescending(item => item.PrintTime ?? item.CreateTime ?? DateTime.MinValue)
                        .ThenByDescending(item => item.Id ?? 0)
                        .ToList();

                    List<PrintRecordInfo> detailSequence = details
                        .OrderBy(item => item.PrintTime ?? item.CreateTime ?? DateTime.MinValue)
                        .ThenBy(item => item.Id ?? 0)
                        .ToList();

                    PrintRecordInfo latest = details.FirstOrDefault();
                    string groupRowId = "g" + index.ToString();
                    return new PrintRecordGroupViewModel
                    {
                        GroupRowId = groupRowId,
                        SupplyBatchNo = SafeValue(latest == null ? null : latest.SupplyBatchNo),
                        PartNo = SafeValue(latest == null ? null : latest.PartNo),
                        CartonNo = SafeValue(latest == null ? null : latest.CartonNo),
                        PrintType = SafeValue(latest == null ? null : latest.PrintType),
                        RecordCount = details.Count,
                        LatestPrintCount = latest != null && latest.PrintCount.HasValue ? latest.PrintCount.Value.ToString() : "0",
                        LatestPrintUser = SafeValue(latest == null ? null : latest.PrintUser),
                        LatestPrintTimeText = FormatDateTime(latest == null ? null : latest.PrintTime),
                        LatestReprintReason = SafeValue(latest == null ? null : latest.ReprintReason),
                        CanExpand = details.Count > 1,
                        Details = detailSequence.Select((item, detailIndex) => new PrintRecordDetailViewModel
                        {
                            GroupRowId = groupRowId,
                            SequenceText = string.Format("第{0}次", detailIndex + 1),
                            PrintUser = SafeValue(item.PrintUser),
                            PrintTimeText = FormatDateTime(item.PrintTime),
                            ReprintReason = SafeValue(item.ReprintReason),
                            StatusText = GetStatusText(item.Status)
                        }).ToList()
                    };
                })
                .OrderByDescending(item => item.RecordCount)
                .ThenByDescending(item => item.LatestPrintTimeText)
                .ToList();
        }

        private static string BuildBusinessKey(PrintRecordInfo printRecordInfo)
        {
            return string.Join("|", new[]
            {
                SafeValue(printRecordInfo == null ? null : printRecordInfo.SupplyBatchNo),
                SafeValue(printRecordInfo == null ? null : printRecordInfo.PartNo),
                SafeValue(printRecordInfo == null ? null : printRecordInfo.CartonNo),
                SafeValue(printRecordInfo == null ? null : printRecordInfo.PrintType)
            });
        }

        private static string SafeValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private static string FormatDateTime(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
        }

        private static string GetStatusText(int? status)
        {
            if (!status.HasValue)
            {
                return string.Empty;
            }

            switch (status.Value)
            {
                case PrintRecordStatusInfo.Pending:
                    return "待打印";
                case PrintRecordStatusInfo.Printing:
                    return "打印中";
                case PrintRecordStatusInfo.Completed:
                    return "已完成";
                case PrintRecordStatusInfo.Failed:
                    return "失败";
                default:
                    return status.Value.ToString();
            }
        }

        private sealed class PrintRecordGroupViewModel
        {
            public string GroupRowId { get; set; }
            public string SupplyBatchNo { get; set; }
            public string PartNo { get; set; }
            public string CartonNo { get; set; }
            public string PrintType { get; set; }
            public int RecordCount { get; set; }
            public string LatestPrintCount { get; set; }
            public string LatestPrintUser { get; set; }
            public string LatestPrintTimeText { get; set; }
            public string LatestReprintReason { get; set; }
            public bool CanExpand { get; set; }
            public List<PrintRecordDetailViewModel> Details { get; set; }
        }

        private sealed class PrintRecordDetailViewModel
        {
            public string GroupRowId { get; set; }
            public string SequenceText { get; set; }
            public string PrintUser { get; set; }
            public string PrintTimeText { get; set; }
            public string ReprintReason { get; set; }
            public string StatusText { get; set; }
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
