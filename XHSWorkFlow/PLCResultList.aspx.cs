using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using ModuleWorkFlow.BLL;
using XHS.Model;

namespace ModuleWorkFlow
{
    public partial class PLCResultList : Page
    {
        protected string menuname = "";
        private string menuid = "D01";

        protected override void OnInit(EventArgs e)
        {
            Load += Page_Load;
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            menuname = new PartTmenu().findbykey(menuid).Menuname;
            if (Master is DefaultSub master) master.Menuname = menuname;
            if (Session["userid"] == null) { Response.Redirect("login.aspx"); return; }
            if (!ModuleWorkFlow.BLL.Private.checkPrivate(this, menuid, "PQUERY")) return;
            if (!IsPostBack) { SetDefaultDateRange(); BindData(); }
        }

        protected void Button_Search_Click(object sender, EventArgs e) { MainDataGrid.PageIndex = 0; BindData(); }

        protected void MainDataGrid_PageIndexChanging(object sender, GridViewPageEventArgs e) { MainDataGrid.PageIndex = e.NewPageIndex; BindData(); }

        private void BindData()
        {
            DateTime? startDate = ParseDate(TextBox_StartDate.Text);
            DateTime? endDate = ParseDate(TextBox_EndDate.Text);
            if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value) { Label_Message.Text = "检测日期起不能晚于检测日期止。"; MainDataGrid.DataSource = null; MainDataGrid.DataBind(); return; }
            if (endDate.HasValue) endDate = endDate.Value.AddDays(1);
            List<PLCResultInfo> rows = new XHS.BLL.PLCResult().GetPLCResults(TextBox_PartNo.Text, TextBox_QRCode.Text, startDate, endDate);
            MainDataGrid.DataSource = rows;
            MainDataGrid.DataBind();
            Label_Message.Text = string.Format(CultureInfo.InvariantCulture, "共查询到 {0} 条检测记录。", rows.Count);
        }

        private void SetDefaultDateRange()
        {
            TextBox_StartDate.Text = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            TextBox_EndDate.Text = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        private static DateTime? ParseDate(string value)
        {
            DateTime result;
            return DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out result) ? result.Date : (DateTime?)null;
        }
    }
}
