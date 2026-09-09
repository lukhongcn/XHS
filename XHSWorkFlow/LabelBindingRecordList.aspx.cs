using System;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;

namespace ModuleWorkFlow
{
    /// <summary>
    /// LABEL_BINDING 历史记录列表。
    /// </summary>
    public partial class LabelBindingRecordList : Page
    {
        protected string menuname = "标签绑定记录";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] == null)
            {
                Response.Redirect("login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                SetDefaultDateRange();
                BindData();
            }
        }

        protected void Button_Search_Click(object sender, EventArgs e)
        {
            MainDataGrid.CurrentPageIndex = 0;
            BindData();
        }

        protected void MainDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            MainDataGrid.CurrentPageIndex = e.NewPageIndex;
            BindData();
        }

        private void BindData()
        {
            DateTime startTime;
            DateTime endDate;
            if (!DateTime.TryParseExact(TextBox_StartTime.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime)
                || !DateTime.TryParseExact(TextBox_EndTime.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            {
                Label_Message.Text = "请输入有效的开始日期和结束日期。";
                MainDataGrid.DataSource = null;
                MainDataGrid.DataBind();
                return;
            }

            endDate = endDate.Date.AddDays(1);
            if (startTime.Date >= endDate)
            {
                Label_Message.Text = "开始日期必须早于结束日期。";
                MainDataGrid.DataSource = null;
                MainDataGrid.DataBind();
                return;
            }

            DataTable records = new LabelBindingRecord().GetRecords(
                TextBox_CustomerProductNo.Text,
                TextBox_ProductNo.Text,
                startTime.Date,
                endDate);
            MainDataGrid.Columns[2].HeaderText = GetColumnHeader(records, "CustomerStepName", "客户标签");
            MainDataGrid.Columns[3].HeaderText = GetColumnHeader(records, "FactoryStepName", "本厂标签");
            MainDataGrid.Columns[4].HeaderText = GetColumnHeader(records, "WorkOrderStepName", "上方工单码");
            MainDataGrid.DataSource = records;
            MainDataGrid.DataBind();
            Label_Message.Text = string.Format(CultureInfo.InvariantCulture, "共查询到 {0} 条绑定记录。", records.Rows.Count);
        }

        private void SetDefaultDateRange()
        {
            DateTime today = DateTime.Today;
            TextBox_StartTime.Text = today.AddDays(-6).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            TextBox_EndTime.Text = today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        private static string GetColumnHeader(DataTable table, string key, string fallback)
        {
            if (table == null || !table.ExtendedProperties.Contains(key)) return fallback;
            string value = Convert.ToString(table.ExtendedProperties[key]);
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }
    }
}
