﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XHS.BLL;
using XHS.Model;
using ModuleWorkFlow.BLL;

namespace ModuleWorkFlow
{
    public partial class ExceptionReview : Page
    {
        private const string MenuId = "B05";
        protected string menuname = "";
        private readonly PackingOperationService packingService = new PackingOperationService();
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

            if (!IsPostBack) BindExceptions();
        }

        protected void lnk_view_Click(object sender, EventArgs e) { BindExceptions(); }

        protected void lnk_search_Click(object sender, EventArgs e) { BindExceptions(); }

        protected void chk_Processed_CheckedChanged(object sender, EventArgs e) { BindExceptions(); }

        protected void lnkbutton_save_Click(object sender, EventArgs e)
        {
            int processedCount = 0;
            int errorCount = 0;
            List<string> errors = new List<string>();

            foreach (GridViewRow row in gvExceptions.Rows)
            {
                if (row.RowType != DataControlRowType.DataRow) continue;

                CheckBox chkSelect = (CheckBox)row.FindControl("chk_Select");
                if (chkSelect == null || !chkSelect.Checked) continue;

                DropDownList ddlAction = (DropDownList)row.FindControl("ddl_Action");
                if (ddlAction == null || string.IsNullOrEmpty(ddlAction.SelectedValue)) continue;

                int newStatus;
                if (!int.TryParse(ddlAction.SelectedValue, out newStatus)) continue;

                HiddenField hidExceptionId = (HiddenField)row.FindControl("hid_ExceptionId");
                HiddenField hidPackingId = (HiddenField)row.FindControl("hid_PackingId");
                if (hidExceptionId == null || hidPackingId == null) continue;

                long exceptionId;
                long packingId;
                if (!long.TryParse(hidExceptionId.Value, out exceptionId) || !long.TryParse(hidPackingId.Value, out packingId)) continue;

                try
                {
                    PackingOperationResult result = packingService.ReviewException(
                        packingId, exceptionId, GetUserName(), string.Empty, newStatus);
                    if (result.Success || result.Status == "LOCK")
                    {
                        processedCount++;
                    }
                    else
                    {
                        errorCount++;
                        errors.Add("异常" + exceptionId + "：" + result.Message);
                    }
                }
                catch (Exception ex)
                {
                    errorCount++;
                    errors.Add("异常" + exceptionId + "：" + ex.Message);
                }
            }

            BindExceptions();

            string msg = string.Format("处理完成：成功 {0} 条", processedCount);
            if (errorCount > 0) msg += string.Format("，失败 {0} 条", errorCount);
            if (errors.Count > 0) msg += "。" + string.Join("；", errors);
            ShowMessage(msg);
        }

        protected void gvExceptions_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            int status = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "Status"));

            // 已处理的记录禁用操作
            DropDownList ddlAction = (DropDownList)e.Row.FindControl("ddl_Action");
            CheckBox chkSelect = (CheckBox)e.Row.FindControl("chk_Select");
            if (ddlAction != null) ddlAction.Enabled = (status == 0);
            if (chkSelect != null) chkSelect.Enabled = (status == 0);
        }

        public string GetStatusText(object value)
        {
            int status;
            if (!int.TryParse(Convert.ToString(value), out status)) return "未知";
            return status == 1 ? "通过" : (status == 2 ? "已取消" : (status == 3 ? "已删除" : "暂停/待处理"));
        }

        private void BindExceptions()
        {
            string kdCode = SafeValue(txt_KdCode.Text);
            int? statusFilter = chk_Processed.Checked ? (int?)null : 0;
            List<PackingExceptionInfo> records = exceptionService.SearchPackingExceptions(kdCode, statusFilter)
                ?? new List<PackingExceptionInfo>();
            gvExceptions.DataSource = records;
            gvExceptions.DataBind();
        }

        private string GetUserName() { return SafeValue(Session["userid"] == null ? string.Empty : Session["userid"].ToString()); }

        private void ShowMessage(string message)
        {
            string safe = string.IsNullOrWhiteSpace(message) ? string.Empty : message.Trim();
            Label_Message.Text = safe;
            ClientScript.RegisterStartupScript(GetType(), "ExceptionReviewMessage",
                string.Format("showMessageModal('{0}');", HttpUtility.JavaScriptStringEncode(safe)), true);
        }

        private static string SafeValue(string value) { return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim(); }

        protected override void OnInit(EventArgs e) { InitializeComponent(); base.OnInit(e); }
        private void InitializeComponent() { Load += new EventHandler(Page_Load); }
    }
}
