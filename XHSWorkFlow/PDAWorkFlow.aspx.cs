using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XHS.Model;
using XHS.service;

namespace ModuleWorkFlow
{
    /// <summary>
    /// JSON 驱动的工序质量判定 PDA 页面。
    /// </summary>
    public partial class PDAWorkFlow : Page
    {
        private const string DefaultProcessId = "QMJCPD";
        private const string ReadyStatusId = "Ready";
        private const string DefaultProfileId = "PART_PROCESS_DECISION";
        private readonly PdaWorkFlowJsonRuntime runtime = new PdaWorkFlowJsonRuntime();
        private PdaWorkFlowDefinition definition;
        private string configurationError;

        private string ConfiguredProcessId
        {
            get
            {
                return Definition != null && Definition.Profile.Query != null
                    && !string.IsNullOrWhiteSpace(Definition.Profile.Query.ProcessId)
                    ? Definition.Profile.Query.ProcessId.Trim()
                    : DefaultProcessId;
            }
        }

        private string ConfiguredStatusId
        {
            get
            {
                return Definition != null && Definition.Profile.Query != null
                    && !string.IsNullOrWhiteSpace(Definition.Profile.Query.StatusId)
                    ? Definition.Profile.Query.StatusId.Trim()
                    : ReadyStatusId;
            }
        }

        private sealed class PdaGridRow
        {
            public int RowNumber { get; set; }
            public string Customer { get; set; }
            public string PartNo_Id { get; set; }
            public string ScannedCode { get; set; }
            public string ReplaceQRCode { get; set; }
            public int? ProcessNo { get; set; }
            public string ActionText { get; set; }
        }

        private string ProfileId
        {
            get
            {
                string profile = Request == null || Request.QueryString == null
                    ? string.Empty
                    : Request.QueryString["profile"];
                return (profile ?? string.Empty).Trim();
            }
        }

        private PdaWorkFlowDefinition Definition
        {
            get
            {
                if (definition == null && string.IsNullOrWhiteSpace(configurationError))
                {
                    try
                    {
                        definition = runtime.Load(ProfileId);
                    }
                    catch (Exception ex)
                    {
                        configurationError = ex.Message;
                    }
                }

                return definition;
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            PdaWorkFlowDefinition loadedDefinition = Definition;
            if (loadedDefinition != null && Master is DefaultPDASub)
            {
                ((DefaultPDASub)Master).PageTitle = loadedDefinition.Profile.PageTitle;
            }

            if (!IsLoggedIn())
            {
                RedirectToLogin(string.Empty);
                return;
            }

            // 暂时注释 PDA 查询权限检测，便于测试页面功能。
            // if (loadedDefinition != null && !HasPermission(loadedDefinition.Profile.Permission.Read))
            // {
            //     RedirectToLogin("没有访问当前 PDA 的查询权限。");
            //     return;
            // }

            base.OnPreInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(configurationError))
            {
                ShowMessage("PDA 配置错误：" + configurationError, false);
                txtQRCode.Enabled = false;
                return;
            }

            // 暂时注释 PDA 权限检测，便于测试页面功能。
            // if (!IsPdaPermissionAllowed())
            // {
            //     return;
            // }

            if (!IsPostBack)
            {
                BindActions();
                ClearCurrent();
                ShowMessage("请扫描 QRCode。", true);
            }
            else
            {
                // GridView 必须在回发事件分发前重建行控件，RowCommand 才能收到按钮命令。
                BindActions();
            }
        }

        protected void txtQRCode_TextChanged(object sender, EventArgs e)
        {
            // 暂时注释 PDA 权限检测，便于测试页面功能。
            // if (!IsPdaPermissionAllowed()) return;

            string rawCode = NormalizeScan(txtQRCode.Text);
            txtQRCode.Text = string.Empty;
            ClearCurrent();

            if (rawCode.Length == 0)
            {
                ShowMessage("请扫描 QRCode。", false);
                return;
            }

            try
            {
                // 当前 QRCode 约定为 partno_id；如果后续二维码有专用格式，只需替换此算子。
                string partNoId = ResolvePartNoId(rawCode);
                List<XHSPartPartProcessInfo> processes =
                    new XHS.BLL.XHSPartPartProcess()
                        .GetPartProcessesByPartNoIdAndProcessIdAndStatusId(
                            partNoId,
                            ConfiguredProcessId,
                            ConfiguredStatusId);

                if (processes == null || processes.Count == 0)
                {
                    ShowMessage("未找到可处理的 " + ConfiguredProcessId + " " + ConfiguredStatusId + " 工序。", false);
                    return;
                }

                if (processes.Count != 1)
                {
                    ShowMessage("请选择要处理的待判工序。", true);
                }
                hidScannedCode.Value = rawCode;
                gvActions.Visible = true;
                gvActions.DataSource = Definition.Profile.Ui.ActionDialog.Actions;
                gvActions.DataBind();
                pnlActionModal.Visible = true;
            }
            catch (Exception ex)
            {
                Utility.Log.WriteLog("PDAWorkFlow.log", "扫描处理异常：" + ex);
                ShowMessage("扫描处理失败，请联系管理员。", false);
            }
        }

        protected void ActionButton_Click(object sender, EventArgs e)
        {
            // 暂时注释 PDA 权限检测，便于测试页面功能。
            if (Definition == null) return;

            // 暂时注释 PDA 写入权限检测，便于测试页面功能。
            // if (!HasPermission(Definition.Profile.Permission.Write))
            // {
            //     ShowMessage("没有执行 PDA 动作的权限。", false);
            //     return;
            // }

            Button actionButton = sender as Button;
            string actionCode = actionButton == null
                ? string.Empty
                : Convert.ToString(actionButton.CommandArgument).Trim();
            if (actionCode.Length == 0) return;

            pnlActionModal.Visible = false;

            if (string.IsNullOrWhiteSpace(hidPartNoId.Value))
            {
                string scannedCode = (hidScannedCode.Value ?? string.Empty).Trim();
                List<XHSPartPartProcessInfo> scannedProcesses =
                    new XHS.BLL.XHSPartPartProcess()
                        .GetPartProcessesByPartNoIdAndProcessIdAndStatusId(
                            scannedCode,
                            ConfiguredProcessId,
                            ConfiguredStatusId)
                    ?? new List<XHSPartPartProcessInfo>();

                if (scannedProcesses.Count != 1)
                {
                    ShowMessage("当前扫码匹配到多条待判工序，无法仅凭动作按钮确定记录。", false);
                    return;
                }

                XHSPartPartProcessInfo selectedProcess = scannedProcesses[0];
                hidPartNoId.Value = selectedProcess.PartNo_Id ?? scannedCode;
                hidPartProcessId.Value = selectedProcess.Id.HasValue
                    ? selectedProcess.Id.Value.ToString()
                    : string.Empty;
                SetCurrentProcess(selectedProcess, scannedCode);
            }

            try
            {
                PdaActionDefinition action = runtime.ValidateAction(Definition, actionCode);
                if (string.IsNullOrWhiteSpace(action.TargetStatusId))
                {
                    ShowMessage("动作没有配置目标状态，未执行更新。", false);
                    return;
                }

                XHSPartPartProcessInfo current = LoadCurrentReadyProcess();
                if (current == null)
                {
                    ShowMessage("当前工序状态已经改变，请重新扫描。", false);
                    return;
                }

                // 先把所选工序和按钮文字显示到主表；业务更新报错时也保留这次选择。
                BindProcesses(
                    new List<XHSPartPartProcessInfo> { current },
                    hidScannedCode.Value,
                    action.Text,
                    current.ProcessNo);

                int affectedRows = UpdateByExistingSource(current, action, actionCode);
                if (affectedRows <= 0)
                {
                    ShowMessage("没有更新到工序记录，可能已被其他人处理，请重新扫描。", false);
                    return;
                }

                RebindAfterAction(current.PartNo_Id, current.ProcessNo, action.Text);
                ShowMessage("动作“" + action.Text + "”处理成功。", true);
            }
            catch (PdaWorkFlowActionException ex)
            {
                ShowMessage(ex.Message, false);
            }
            catch (Exception ex)
            {
                Utility.Log.WriteLog("PDAWorkFlow.log", "动作处理异常：" + ex);
                ShowMessage("动作处理失败，请联系管理员。", false);
            }
        }

        private int UpdateByExistingSource(
            XHSPartPartProcessInfo current,
            PdaActionDefinition action,
            string actionCode)
        {
            if (!current.ProcessNo.HasValue || !current.ProcessOrder.HasValue)
            {
                throw new InvalidOperationException("当前工序缺少 processno 或 processorder，不能更新。");
            }

            List<ModuleWorkFlow.Model.PartPartProcessInfo> existingProcesses =
                new ModuleWorkFlow.BLL.PartPartProcess().getListPartProcessInfo(
                    current.ModuleId,
                    current.PartNo_Id) ?? new List<ModuleWorkFlow.Model.PartPartProcessInfo>();

            ModuleWorkFlow.Model.PartPartProcessInfo source = existingProcesses.FirstOrDefault(item =>
                item != null
                && item.ProcessNo == current.ProcessNo.Value
                && string.Equals(item.PartNo_Id, current.PartNo_Id, StringComparison.OrdinalIgnoreCase)
                && string.Equals(item.ProcessId, ConfiguredProcessId, StringComparison.OrdinalIgnoreCase)
                && string.Equals(item.StatusId, ConfiguredStatusId, StringComparison.OrdinalIgnoreCase));

            if (source == null)
            {
                throw new PdaWorkFlowActionException("动作执行前重新读取不到 Ready 工序，请重新扫描。");
            }

            source.StatusId = action.TargetStatusId.Trim();
            if (string.Equals(actionCode, "SCRAP", StringComparison.OrdinalIgnoreCase))
            {
                source.IsUnnormal = 1;
            }

            return ModuleWorkFlow.business.PartProcess.UpdatePartProcessFromModifyEdit(
                new List<ModuleWorkFlow.Model.PartPartProcessInfo> { source });
        }

        private XHSPartPartProcessInfo LoadCurrentReadyProcess()
        {
            string partNoId = (hidPartNoId.Value ?? string.Empty).Trim();
            if (partNoId.Length == 0) return null;

            List<XHSPartPartProcessInfo> processes =
                new XHS.BLL.XHSPartPartProcess()
                    .GetPartProcessesByPartNoIdAndProcessIdAndStatusId(
                        partNoId,
                            ConfiguredProcessId,
                            ConfiguredStatusId);
            if (processes == null || processes.Count != 1) return null;

            string expectedId = hidPartProcessId.Value.Trim();
            if (expectedId.Length > 0 && (!processes[0].Id.HasValue || processes[0].Id.Value.ToString() != expectedId))
            {
                return null;
            }

            return processes[0];
        }

        private void RebindAfterAction(string partNoId, int? processNo, string actionText)
        {
            List<XHSPartPartProcessInfo> processes =
                new XHS.BLL.XHSPartPartProcess().GetPartProcessesByPartNoId(partNoId)
                ?? new List<XHSPartPartProcessInfo>();
            BindProcesses(processes, string.Empty, actionText, processNo);
            hidPartNoId.Value = string.Empty;
            hidPartProcessId.Value = string.Empty;
            labScanState.Text = "动作完成，请扫描下一个 QRCode";
            FocusScan();
        }

        private void BindActions()
        {
            if (Definition == null || Definition.Profile.Ui == null || Definition.Profile.Ui.ActionDialog == null)
            {
                gvActions.DataSource = null;
            }
            else
            {
                gvActions.DataSource = Definition.Profile.Ui.ActionDialog.Actions;
            }

            gvActions.DataBind();
        }

        private void ConfigureGridColumns()
        {
            gvPartProcesses.Columns.Clear();
            IList<PdaGridColumnDefinition> columns = Definition != null
                && Definition.Profile.Ui != null
                && Definition.Profile.Ui.ProcessGrid != null
                ? Definition.Profile.Ui.ProcessGrid.Columns
                : null;

            if (columns == null) return;

            foreach (PdaGridColumnDefinition column in columns)
            {
                if (column == null || string.IsNullOrWhiteSpace(column.Field)) continue;
                if (string.Equals(column.Type, "dropdown", StringComparison.OrdinalIgnoreCase))
                {
                    gvPartProcesses.Columns.Add(new TemplateField
                    {
                        HeaderText = column.Title,
                        Visible = column.Visible,
                        ItemTemplate = new ActionDropDownTemplate(this)
                    });
                }
                else
                {
                    gvPartProcesses.Columns.Add(new BoundField
                    {
                        DataField = column.Field,
                        HeaderText = column.Title,
                        Visible = column.Visible
                    });
                }
            }
        }

        private sealed class ActionDropDownTemplate : ITemplate
        {
            private readonly PDAWorkFlow owner;

            public ActionDropDownTemplate(PDAWorkFlow owner)
            {
                this.owner = owner;
            }

            public void InstantiateIn(Control container)
            {
                DropDownList list = new DropDownList
                {
                    ID = "ddlGridAction",
                    CssClass = "workflow-action-select"
                };
                if (owner.Definition != null
                    && owner.Definition.Profile.Ui != null
                    && owner.Definition.Profile.Ui.ActionDialog != null)
                {
                    list.DataSource = owner.Definition.Profile.Ui.ActionDialog.Actions;
                    list.DataTextField = "Text";
                    list.DataValueField = "Code";
                    list.DataBind();
                }

                container.Controls.Add(list);
            }
        }

        private void BindProcesses(IList<XHSPartPartProcessInfo> processes, string scannedCode, string actionText, int? actionProcessNo)
        {
            ConfigureGridColumns();
            List<PdaGridRow> rows = new List<PdaGridRow>();
            int rowNumber = 1;
            foreach (XHSPartPartProcessInfo process in processes ?? new List<XHSPartPartProcessInfo>())
            {
                rows.Add(new PdaGridRow
                {
                    RowNumber = rowNumber++,
                    Customer = ResolveCustomer(process),
                    PartNo_Id = process.PartNo_Id,
                    ScannedCode = scannedCode,
                    ReplaceQRCode = process.RelationPartNoId,
                    ProcessNo = process.ProcessNo,
                    ActionText = actionProcessNo.HasValue && process.ProcessNo == actionProcessNo
                        ? actionText
                        : string.Empty
                });
            }

            gvPartProcesses.DataSource = rows;
            gvPartProcesses.DataBind();
        }

        private string ResolveCustomer(XHSPartPartProcessInfo process)
        {
            if (process == null) return string.Empty;
            string customer;
            if (Definition != null && Definition.Profile.CustomerMap != null
                && Definition.Profile.CustomerMap.TryGetValue(process.ModuleId ?? string.Empty, out customer))
            {
                return customer;
            }

            return process.ModuleId ?? string.Empty;
        }

        private void SetCurrentProcess(XHSPartPartProcessInfo process, string scannedCode)
        {
        }

        private void ClearCurrent()
        {
            hidPartNoId.Value = string.Empty;
            hidPartProcessId.Value = string.Empty;
            hidScannedCode.Value = string.Empty;
            pnlActionModal.Visible = false;
            labScanState.Text = "等待扫描 QRCode";
        }

        private bool IsPdaPermissionAllowed()
        {
            // 暂时旁路 PDA 访问权限检测，便于测试页面功能。
            return true;
        }

        private bool HasPermission(string privilegeName)
        {
            if (Session != null && Session["isadmin"] is bool && (bool)Session["isadmin"])
            {
                return true;
            }

            IList privileges = Session == null ? null : Session["iprivate"] as IList;
            if (privileges == null || string.IsNullOrWhiteSpace(privilegeName) || Definition == null)
            {
                return false;
            }

            foreach (object privilege in privileges)
            {
                string menuId = Convert.ToString(GetPrivilegeMember(privilege, "menuid"));
                if (!string.Equals(menuId, Definition.Catalog.MenuId, StringComparison.OrdinalIgnoreCase)) continue;

                int value;
                return int.TryParse(
                    Convert.ToString(GetPrivilegeMember(privilege, privilegeName)),
                    out value) && value > 0;
            }

            return false;
        }

        private static object GetPrivilegeMember(object privilege, string memberName)
        {
            if (privilege == null) return null;
            Type type = privilege.GetType();
            PropertyInfo property = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(item => string.Equals(item.Name, memberName, StringComparison.OrdinalIgnoreCase));
            if (property != null) return property.GetValue(privilege, null);

            FieldInfo field = type.GetFields(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(item => string.Equals(item.Name, memberName, StringComparison.OrdinalIgnoreCase));
            return field == null ? null : field.GetValue(privilege);
        }

        private bool IsLoggedIn()
        {
            return Session != null
                && Session["islogin"] is bool
                && (bool)Session["islogin"]
                && !string.IsNullOrWhiteSpace(Convert.ToString(Session["userid"]));
        }

        private void RedirectToLogin(string message)
        {
            string loginUrl = ResolveUrl("~/login.aspx");
            string returnUrl = Request.RawUrl;
            if (string.IsNullOrWhiteSpace(ProfileId))
            {
                returnUrl = ResolveUrl(
                    "~/PDAWorkFlow.aspx?profile=" + Server.UrlEncode(DefaultProfileId));
            }

            string url = loginUrl + "?ReturnUrl=" + Server.UrlEncode(returnUrl);
            if (!string.IsNullOrWhiteSpace(message))
            {
                url += "&LoginMessage=NoPermission&PermissionName=" + Server.UrlEncode(message);
            }

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private static string ResolvePartNoId(string rawCode)
        {
            return (rawCode ?? string.Empty).Trim();
        }

        private static bool Requires(PdaActionDefinition action, string requirement)
        {
            return action.Requires != null
                && action.Requires.Any(item => string.Equals(item, requirement, StringComparison.OrdinalIgnoreCase));
        }

        private static string NormalizeScan(string value)
        {
            return (value ?? string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();
        }

        private void ShowMessage(string message, bool success)
        {
            labMessage.Text = message ?? string.Empty;
            labMessage.CssClass = success
                ? "workflow-status workflow-ok"
                : "workflow-status workflow-error";
        }

        private void FocusScan()
        {
            if (Page == null) return;
            ScriptManager scriptManager = ScriptManager.GetCurrent(Page);
            if (scriptManager != null)
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "focusWorkflowScan",
                    "setTimeout(function(){var e=document.getElementById('" + txtQRCode.ClientID + "');if(e){e.focus();try{e.select()}catch(x){}}},40);",
                    true);
            }
        }
    }
}
