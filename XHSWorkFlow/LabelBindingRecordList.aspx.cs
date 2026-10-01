using System;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using ModuleWorkFlow.BLL;

namespace ModuleWorkFlow
{
    /// <summary>
    /// LABEL_BINDING 历史记录列表。
    /// </summary>
    public partial class LabelBindingRecordList : Page
    {
        protected string menuname = "";
        private string menuid = "C02";
        private bool bindingGridPlaceholder;

        protected override void OnInit(EventArgs e)
        {
            Load += Page_Load;
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            menuname = new PartTmenu().findbykey(menuid).Menuname;
            if (Master is DefaultSub master)
            {
                master.Menuname = menuname;
            }
            if (Session["userid"] == null)
            {
                Response.Redirect("login.aspx");
                return;
            }
            if (ModuleWorkFlow.BLL.Private.checkPrivate(this, menuid, "PQUERY"))
            {

                if (!IsPostBack)
                {
                    SetDefaultDateRange();
                    BindData();
                }
            }
        }

        protected void Button_Search_Click(object sender, EventArgs e)
        {
            MainDataGrid.PageIndex = 0;
            BindData();
        }

        protected void MainDataGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
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

            DataTable records = new LabelBindingRecord().GetRecords(
                TextBox_CustomerProductNo.Text,
                TextBox_ProductNo.Text,
                startTime,
                endDate);
            if (records == null || records.Rows.Count == 0)
            {
                BindEmptyGrid();
            }
            else
            {
                MainDataGrid.DataSource = records;
                MainDataGrid.DataBind();
            }
            Label_Message.Text = string.Format(CultureInfo.InvariantCulture, "共查询到 {0} 条绑定记录。", records.Rows.Count);
        }

        private void BindEmptyGrid()
        {
            DataTable table = new DataTable();
            table.Columns.Add("WorkOrderNo");
            table.Columns.Add("FactoryBarcode");
            table.Columns.Add("JHSPartNo");
            table.Columns.Add("JHSBatchNo");
            table.Columns.Add("LabelQtyText");
            table.Columns.Add("ScanTimeText");
            table.Columns.Add("ScanUser");
            table.Rows.Add(table.NewRow());

            bindingGridPlaceholder = true;
            MainDataGrid.DataSource = table;
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

    }
}

