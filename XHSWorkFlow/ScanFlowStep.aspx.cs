using System;
using System.Collections.Generic;
using XHS.Model;

namespace ModuleWorkFlow
{
    /// <summary>
    /// 扫描流程步骤只读查看页，供五层结构联调使用。
    /// </summary>
    public partial class ScanFlowStepPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !string.IsNullOrWhiteSpace(Request.QueryString["FlowId"]))
            {
                txtFlowId.Text = Request.QueryString["FlowId"];
                LoadSteps();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e) { LoadSteps(); }

        private void LoadSteps()
        {
            int flowId;
            if (!int.TryParse(txtFlowId.Text, out flowId) || flowId <= 0)
            {
                gvSteps.DataSource = null;
                gvSteps.DataBind();
                labMessage.Text = "请输入大于 0 的 FlowId。";
                return;
            }

            List<ScanFlowStepInfo> steps = new global::BLL.ScanFlowStep().GetStepsByFlowId(flowId);
            gvSteps.DataSource = steps;
            gvSteps.DataBind();
            labMessage.Text = steps.Count == 0 ? "未找到流程步骤。" : string.Format("共 {0} 个步骤。", steps.Count);
        }
    }
}
