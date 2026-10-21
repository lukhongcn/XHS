using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using ModuleWorkFlow.BLL;
using XHS.Model;

namespace ModuleWorkFlow
{
    /// <summary>
    /// LABEL_BINDING PDA 扫描失败记录列表。
    /// </summary>
    public partial class LabelBindingFailRecordList : Page
    {
        protected string menuname = "";
        private const string MenuId = "C03";
        private bool bindingGridPlaceholder;

        protected override void OnInit(EventArgs e)
        {
            Load += Page_Load;
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            menuname = new PartTmenu().findbykey(MenuId).Menuname;
            if (Master is DefaultSub master)
            {
                master.Menuname = menuname;
            }
            if (Session["userid"] == null)
            {
                Response.Redirect("login.aspx");
                return;
            }
            if (ModuleWorkFlow.BLL.Private.checkPrivate(this, MenuId, "PQUERY") && !IsPostBack)
            {
                SetDefaultDateRange();
                BindData();
            }
        }

        protected void Button_Search_Click(object sender, EventArgs e)
        {
            if (!ModuleWorkFlow.BLL.Private.checkPrivate(this, MenuId, "PQUERY")) return;
            MainDataGrid.PageIndex = 0;
            BindData();
        }

        protected void MainDataGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (!ModuleWorkFlow.BLL.Private.checkPrivate(this, MenuId, "PQUERY")) return;
            MainDataGrid.PageIndex = e.NewPageIndex;
            BindData();
        }

        protected void MainDataGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (bindingGridPlaceholder && e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Visible = false;
            }
        }

        private void BindData()
        {
            DateTime startTime;
            DateTime endDate;
            if (!TryParseSearchTime(TextBox_StartTime.Text, out startTime)
                || !TryParseSearchTime(TextBox_EndTime.Text, out endDate))
            {
                Label_Message.Text = "请输入有效的开始时间和结束时间。";
                BindEmptyGrid();
                return;
            }

            if (!TextBox_StartTime.Text.Contains("T")) startTime = startTime.Date;
            if (!TextBox_EndTime.Text.Contains("T")) endDate = endDate.Date.AddDays(1);
            if (startTime >= endDate)
            {
                Label_Message.Text = "开始时间必须早于结束时间。";
                BindEmptyGrid();
                return;
            }

            List<LabelBindingPdaScanFailureInfo> records = new LabelBindingFailure().GetRecords(
                TextBox_ScanRawCode.Text,
                TextBox_FailureType.Text,
                startTime,
                endDate);
            if (records == null || records.Count == 0)
            {
                BindEmptyGrid();
            }
            else
            {
                MainDataGrid.DataSource = records.Select(ToRow).ToList();
                MainDataGrid.DataBind();
            }
            Label_Message.Text = string.Format(CultureInfo.InvariantCulture, "共查询到 {0} 条失败记录。", records == null ? 0 : records.Count);
        }

        private void BindEmptyGrid()
        {
            bindingGridPlaceholder = true;
            MainDataGrid.DataSource = new List<FailureRecordRow> { new FailureRecordRow() };
            MainDataGrid.DataBind();
            bindingGridPlaceholder = false;
        }

        private void SetDefaultDateRange()
        {
            DateTime today = DateTime.Today;
            TextBox_StartTime.Text = today.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
            TextBox_EndTime.Text = today.AddDays(1).ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
        }

        private static bool TryParseSearchTime(string text, out DateTime value)
        {
            return DateTime.TryParseExact(
                text,
                new[] { "yyyy-MM-ddTHH:mm", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-dd" },
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out value);
        }

        private static FailureRecordRow ToRow(LabelBindingPdaScanFailureInfo record)
        {
            return new FailureRecordRow
            {
                ScanTimeText = record.ScanTime.HasValue ? record.ScanTime.Value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) : "",
                ScanStage = record.ScanStage,
                FailureType = record.FailureType,
                FailureReason = record.FailureReason,
                ScanRawCode = record.ScanRawCode,
                CustomerRawCode = record.CustomerRawCode,
                ParsedBarcodeType = record.ParsedBarcodeType,
                ParsedPartNo = record.ParsedPartNo,
                ParsedBatchNo = record.ParsedBatchNo,
                ParsedQtyText = record.ParsedQty.HasValue ? record.ParsedQty.Value.ToString("0.####", CultureInfo.InvariantCulture) : "",
                OperatorName = record.OperatorName,
                IsProcessedText = record.IsProcessed ? "是" : "否"
            };
        }

        private sealed class FailureRecordRow
        {
            public string ScanTimeText { get; set; }
            public string ScanStage { get; set; }
            public string FailureType { get; set; }
            public string FailureReason { get; set; }
            public string ScanRawCode { get; set; }
            public string CustomerRawCode { get; set; }
            public string ParsedBarcodeType { get; set; }
            public string ParsedPartNo { get; set; }
            public string ParsedBatchNo { get; set; }
            public string ParsedQtyText { get; set; }
            public string OperatorName { get; set; }
            public string IsProcessedText { get; set; }
        }
    }
}
