using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XHS.BLL;
using XHS.Model;
using ModuleWorkFlow.BLL;

namespace ModuleWorkFlow
{
    public partial class PackingExceptionList : Page
    {
        private const string MenuId = "B05";
        protected string menuname = "";
        private readonly XHS.BLL.PackingException exceptionService = new XHS.BLL.PackingException();

        protected void Page_Load(object sender, EventArgs e)
        {
            menuname = new PartTmenu().findbykey(MenuId).Menuname;
            if (Master is DefaultSub master) master.Menuname = menuname;

            if (!Private.checkPrivate(this, MenuId, "PEDIT"))
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
                SetDefaultDateRange();
                chk_Processed.Checked = true;
                BindExceptions();
            }
        }

        protected void lnk_search_Click(object sender, EventArgs e) { BindExceptions(); }

        protected void chk_Processed_CheckedChanged(object sender, EventArgs e) { BindExceptions(); }

        public string GetStatusText(object value)
        {
            int status;
            if (!int.TryParse(Convert.ToString(value), out status)) return "未知";
            return status == 1 ? "通过" : (status == 2 ? "取消" : (status == 3 ? "已执行" : "暂停/待处理"));
        }

        private void BindExceptions()
        {
            string kdCode = SafeValue(txt_KdCode.Text);
            string partNo = SafeValue(txt_PartNo.Text);
            DateTime? dateFrom = ParseDate(txt_DateFrom.Text);
            DateTime? dateTo = ParseDate(txt_DateTo.Text);
            int? statusFilter = chk_Processed.Checked ? (int?)null : 0;

            List<PackingExceptionInfo> records = exceptionService.SearchPackingExceptions(kdCode, partNo, dateFrom, dateTo, statusFilter)
                ?? new List<PackingExceptionInfo>();
            gvExceptions.DataSource = records;
            gvExceptions.DataBind();
        }

        private void SetDefaultDateRange()
        {
            txt_DateFrom.Text = DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd");
            txt_DateTo.Text = DateTime.Today.ToString("yyyy-MM-dd");
        }

        private static DateTime? ParseDate(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            DateTime value;
            return DateTime.TryParse(text, out value) ? value : (DateTime?)null;
        }

        private static string SafeValue(string value) { return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim(); }

        protected override void OnInit(EventArgs e) { InitializeComponent(); base.OnInit(e); }
        private void InitializeComponent() { Load += new EventHandler(Page_Load); }
    }
}
