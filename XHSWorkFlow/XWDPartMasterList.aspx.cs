using BLL;
using ModuleWorkFlow.business;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XHS.BLL;
using XHS.Model;
using BLL;
using ModuleWorkFlow.BLL;

namespace ModuleWorkFlow
{
    /// <summary>
    /// XWD 标签零件主数据列表页面。
    /// </summary>
    public partial class XWDPartMasterList : Page
    {
        protected string menuname = "";
        private const string MenuId = "C01";

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
            if (!ModuleWorkFlow.BLL.Private.checkPrivate(this, MenuId, "PQUERY"))
            {
                return;
            }

            if (!IsPostBack)
            {
                BindData();
            }
        }

        protected void lnkbutton_search_Click(object sender, EventArgs e)
        {
            MainDataGrid.CurrentPageIndex = 0;
            BindData();
        }

        protected void lnkbutton_edit_Click(object sender, EventArgs e)
        {
            Response.Redirect("GSPartMasterUpload.aspx");
        }

        protected void MainDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            MainDataGrid.CurrentPageIndex = e.NewPageIndex;
            BindData();
        }

        protected void MainDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
        {
            if (!ModuleWorkFlow.BLL.Private.checkPrivate(this, MenuId, "PEDIT"))
            {
                return;
            }

            MainDataGrid.EditItemIndex = e.Item.ItemIndex;
            BindData();
        }

        protected void MainDataGrid_CancelCommand(object source, DataGridCommandEventArgs e)
        {
            MainDataGrid.EditItemIndex = -1;
            BindData();
        }

        protected void MainDataGrid_UpdateCommand(object source, DataGridCommandEventArgs e)
        {
            if (!ModuleWorkFlow.BLL.Private.checkPrivate(this, MenuId, "PEDIT"))
            {
                return;
            }

            int partMasterId = Convert.ToInt32(MainDataGrid.DataKeys[e.Item.ItemIndex]);
            TextBox remarkTextBox = (TextBox)e.Item.FindControl("TextBox_Remark");
            PartMaster partMaster = new PartMaster();
            PartMasterInfo partMasterInfo = partMaster.GetPartMaster(partMasterId);
            if (partMasterInfo == null)
            {
                ShowMessage("未找到需要修改的零件主数据。");
                return;
            }

            partMasterInfo.Remark = remarkTextBox == null ? string.Empty : remarkTextBox.Text.Trim();
            string message = partMaster.UpdatePartMasters(new List<PartMasterInfo> { partMasterInfo });
            if (!string.IsNullOrWhiteSpace(message))
            {
                ShowMessage(message);
                return;
            }

            MainDataGrid.EditItemIndex = -1;
            BindData();
            ShowMessage("版本号修改成功。");
        }

        private void BindData()
        {
            List<PartMasterInfo> partMasterInfos = new PartMaster()
                .GetPartByCustomerAbbr("XWD");

            MainDataGrid.DataKeyField = "PartMasterId";
            MainDataGrid.DataSource = partMasterInfos;
            MainDataGrid.DataBind();
            Label_Message.Text = string.Format("共查询到 {0} 条零件主数据。", partMasterInfos.Count);
        }

        private void ShowMessage(string message)
        {
            Label_Message.Text = message;
            string script = string.Format(
                "showMessageModal('{0}');",
                HttpUtility.JavaScriptStringEncode(message ?? string.Empty));
            ClientScript.RegisterStartupScript(GetType(), "XWDPartMasterListMessage", script, true);
        }
    }
}
