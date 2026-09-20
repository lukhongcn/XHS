using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using XHS.BLL;
using XHS.Model;

namespace ModuleWorkFlow
{
    /// <summary>
    /// 光束标签零件主数据上传页面。
    /// </summary>
    public partial class GSPartMasterUpload : Page
    {
        protected string menuname = "光束标签主数据上传";
        private const string MenuId = "C";
        private const string CustomerAbbr = "DSBQ";
        private const string LabelBindingFlowCode = "LABEL_BINDING";

        protected override void OnInit(EventArgs e)
        {
            Load += Page_Load;
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ModuleWorkFlow.BLL.Private.checkPrivate(this, MenuId, "PADD")) return;
            if (Session["userid"] == null)
            {
                Response.Redirect("login.aspx");
                return;
            }
        }

        protected void btn_upload_Click(object sender, EventArgs e)
        {
            if (!FileUploadPartMaster.HasFile)
            {
                ShowMessage("请选择需要上传的光束标签 Excel 文件。");
                return;
            }

            string extension = Path.GetExtension(FileUploadPartMaster.FileName);
            if (!string.Equals(extension, ".xlsx", StringComparison.OrdinalIgnoreCase) && !string.Equals(extension, ".xls", StringComparison.OrdinalIgnoreCase))
            {
                ShowMessage("只允许上传 Excel 文件。");
                return;
            }

            string uploadPath = Server.MapPath("~/Upload/PartMaster");
            Directory.CreateDirectory(uploadPath);
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Path.GetFileName(FileUploadPartMaster.FileName);
            string fullName = Path.Combine(uploadPath, fileName);
            FileUploadPartMaster.SaveAs(fullName);

            List<string> messages;
            List<PartMasterInfo> imported;
            try
            {
                imported = new PartMaster().ReadGSPartMasterExcel(fullName, CustomerAbbr, out messages);
            }
            catch (Exception ex)
            {
                ShowMessage("读取光束标签主数据失败：" + ex.Message);
                return;
            }

            if (messages.Count > 0)
            {
                ShowMessage(string.Join("<br />", messages.ToArray()));
                return;
            }
            if (imported.Count == 0)
            {
                ShowMessage("没有读取到可导入的零件主数据。");
                return;
            }

            foreach (PartMasterInfo info in imported)
            {
                info.CustomerAbbr = CustomerAbbr;
            }

            ScanFlowStepInfo customerRuleStep;
            ScanFlowStepInfo factoryRuleStep;
            if (!TryGetBindingRuleSteps(out customerRuleStep, out factoryRuleStep))
            {
                ShowMessage("光束标签绑定流程未配置客户标签或本厂标签规则。 ");
                return;
            }

            if (string.IsNullOrWhiteSpace(customerRuleStep.CustomerId)
                || !string.Equals(customerRuleStep.CustomerId, factoryRuleStep.CustomerId, StringComparison.OrdinalIgnoreCase))
            {
                ShowMessage("光束标签绑定流程的客户标签与本厂标签客户编号不一致。 ");
                return;
            }

            PartMaster partMaster = new PartMaster();
            string codeMessage = partMaster.ValidateGSPartMasterCodes(
                imported,
                factoryRuleStep.CustomerId,
                factoryRuleStep.LabelType,
                customerRuleStep.LabelType,
                factoryRuleStep.RuleName,
                customerRuleStep.RuleName);
            if (!string.IsNullOrWhiteSpace(codeMessage))
            {
                ShowMessage(codeMessage);
                return;
            }

            List<PartMasterInfo> existing = partMaster.GetPartMasters();
            Dictionary<string, PartMasterInfo> existingByKey = existing
                .Where(info => info != null)
                .GroupBy(BuildKey, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
            List<PartMasterInfo> inserts = new List<PartMasterInfo>();
            List<PartMasterInfo> updates = new List<PartMasterInfo>();
            foreach (PartMasterInfo info in imported)
            {
                PartMasterInfo oldInfo;
                if (existingByKey.TryGetValue(BuildKey(info), out oldInfo))
                {
                    info.PartMasterId = oldInfo.PartMasterId;
                    updates.Add(info);
                }
                else
                {
                    inserts.Add(info);
                }
            }

            string message = inserts.Count == 0 ? string.Empty : partMaster.InsertPartMasters(inserts);
            if (!string.IsNullOrWhiteSpace(message)) { ShowMessage(message); return; }
            message = updates.Count == 0 ? string.Empty : partMaster.UpdatePartMasters(updates);
            if (!string.IsNullOrWhiteSpace(message)) { ShowMessage(message); return; }
            ShowMessage(string.Format("上传成功，新增 {0} 条，更新 {1} 条零件主数据。", inserts.Count, updates.Count));
        }

        protected void lnk_view_Click(object sender, EventArgs e)
        {
            Response.Redirect("GSPartMasterList.aspx");
        }

        private static bool TryGetBindingRuleSteps(out ScanFlowStepInfo customerRuleStep, out ScanFlowStepInfo factoryRuleStep)
        {
            customerRuleStep = null;
            factoryRuleStep = null;

            List<ScanFlowStepInfo> steps = new global::BLL.ScanFlowStep().GetStepsByFlowCode(LabelBindingFlowCode);
            if (steps == null)
            {
                return false;
            }

            foreach (ScanFlowStepInfo step in steps)
            {
                XHS.BLL.ScanStepHandlerType handlerType = XHS.BLL.ScanStepHandlerRegistry.Resolve(step);
                if (handlerType == XHS.BLL.ScanStepHandlerType.Customer && customerRuleStep == null)
                {
                    customerRuleStep = step;
                }
                else if (handlerType == XHS.BLL.ScanStepHandlerType.Factory && factoryRuleStep == null)
                {
                    factoryRuleStep = step;
                }
            }

            return customerRuleStep != null
                && factoryRuleStep != null
                && !string.IsNullOrWhiteSpace(customerRuleStep.CustomerId)
                && !string.IsNullOrWhiteSpace(customerRuleStep.LabelType)
                && !string.IsNullOrWhiteSpace(factoryRuleStep.LabelType);
        }

        private static string BuildKey(PartMasterInfo info)
        {
            return string.Join("\u001f", new[] { info.JHSPartNo, info.CustomerMaterialNo, info.CustomerAbbr }.Select(value => (value ?? string.Empty).Trim()));
        }

        private void ShowMessage(string message)
        {
            Label_Message.Text = message;
            string alertMessage = HttpUtility.HtmlDecode(message).Replace("<br />", "\n").Replace("<br/>", "\n").Replace("<br>", "\n");
            ClientScript.RegisterStartupScript(GetType(), "GSPartMasterUploadMessage", "showMessageModal('" + HttpUtility.JavaScriptStringEncode(alertMessage) + "');", true);
        }
    }
}
