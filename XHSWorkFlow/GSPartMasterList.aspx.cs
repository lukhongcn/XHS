using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using XHS.BLL;
using XHS.Model;

namespace ModuleWorkFlow
{
    /// <summary>
    /// 光束标签零件主数据列表页面。
    /// </summary>
    public partial class GSPartMasterList : Page
    {
        protected string menuname = "光束标签主数据列表";
        private const string MenuId = "C";

        protected override void OnInit(EventArgs e)
        {
            Load += Page_Load;
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ModuleWorkFlow.BLL.Private.checkPrivate(this, MenuId, "PQUERY"))
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

        private void BindData()
        {
            List<PartMasterInfo> partMasterInfos = new PartMaster().GetPartMasters(
                TextBox_JHSPartNo.Text.Trim(),
                TextBox_CustomerMaterialNo.Text.Trim());

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
            ClientScript.RegisterStartupScript(GetType(), "GSPartMasterListMessage", script, true);
        }
    }
}
