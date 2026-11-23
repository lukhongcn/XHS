using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
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
        private readonly PageEventAdapter pageEventAdapter = new PageEventAdapter();
        private PdaWorkFlowDefinition definition;
        private PageJsonDefinition pageDefinition;
        private ActionDispatcher actionDispatcher;
        private string configurationError;

        private sealed class ActionButtonRow
        {
            public string Text { get; set; }
            public string Code { get; set; }
            public string Color { get; set; }
            public string TextColor { get; set; }
        }

        private ActionDispatcher Dispatcher
        {
            get
            {
                if (actionDispatcher == null)
                {
                    actionDispatcher = new ActionDispatcher(new ApiConfigurationLoader());
                }

                return actionDispatcher;
            }
        }

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

        private string ProfileId
        {
            get
            {
                string profile = Request == null || Request.QueryString == null
                    ? string.Empty
                    : Request.QueryString["profile"];
                profile = (profile ?? string.Empty).Trim();
                return profile.Length > 0 ? profile : DefaultProfileId;
            }
        }

        private string BindingName
        {
            get
            {
                string bindingName = Request == null || Request.QueryString == null
                    ? string.Empty
                    : Request.QueryString["binding"];
                bindingName = (bindingName ?? string.Empty).Trim();
                return bindingName.Length > 0 ? bindingName : ProfileId;
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

        private PageJsonDefinition PageDefinition
        {
            get
            {
                if (pageDefinition == null)
                {
                    pageDefinition = new PageConfigurationLoader().Load();
                }
                return pageDefinition;
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

            if (loadedDefinition != null && !HasPermission(loadedDefinition.Profile.Permission.Read))
            {
                RedirectToLogin("没有访问当前 PDA 的查询权限。");
                return;
            }

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

            try
            {
                ActionDispatcher dispatcher = Dispatcher;
                dispatcher.EnsureBindingName(BindingName);
                foreach (PageActionDefinition action in PageDefinition.Actions)
                {
                    dispatcher.EnsureAction(BindingName, action.Binding);
                }
            }
            catch (ApiConfigurationException ex)
            {
                ShowMessage("PDA API 配置错误：" + ex.Message, false);
                txtQRCode.Enabled = false;
                return;
            }

            if (!IsPdaPermissionAllowed()) return;

            ConfigureGridColumns();
            BindActions();

            if (!IsPostBack)
            {
                ClearCurrent();
                ShowMessage("请扫描 QRCode。", true);
            }
        }

        protected void txtQRCode_TextChanged(object sender, EventArgs e)
        {
            ExecuteConfiguredEvent(sender, "txtQRCode.TextChanged", string.Empty, null);
        }

        protected void ActionButton_Click(object sender, EventArgs e)
        {
            if (Definition == null) return;

            Button actionButton = sender as Button;
            string actionCode = actionButton == null
                ? string.Empty
                : Convert.ToString(actionButton.CommandName).Trim();
            if (actionCode.Length == 0 && actionButton != null)
            {
                actionCode = Convert.ToString(actionButton.CommandArgument).Trim();
            }
            if (actionCode.Length == 0) return;
            try
            {
                PageActionDefinition action = PageDefinition.Actions.FirstOrDefault(item =>
                    item != null && string.Equals(item.Code, actionCode, StringComparison.OrdinalIgnoreCase));
                if (action == null)
                {
                    throw new ApiExecutionException("当前页面不允许动作：" + actionCode);
                }
                ExecuteConfiguredEvent(sender, "ActionButton.Click", actionCode, action);
            }
            catch (Exception ex)
            {
                Utility.Log.WriteLog("PDAWorkFlow.log", "动作处理异常：" + ex);
                ShowActionModalMessage("动作处理失败，请联系管理员。");
            }
        }

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            ExecuteConfiguredEvent(sender, "btnCloseModal.Click", string.Empty, null);
        }

        private void ExecuteConfiguredEvent(
            object sender,
            string eventName,
            string commandName,
            object actionMetadata)
        {
            PageEventExecutionOptions options = new PageEventExecutionOptions
            {
                Page = this,
                Sender = sender,
                EventName = eventName,
                BindingName = BindingName,
                CommandName = commandName,
                ActionMetadata = actionMetadata,
                Configuration = CreatePageConfiguration(),
                Dispatcher = Dispatcher,
                PermissionEvaluator = new DelegateApiPermissionEvaluator(HasPermission),
                ErrorLogger = ex => Utility.Log.WriteLog(
                    "PDAWorkFlow.log",
                    eventName + " 执行异常：" + ex)
            };
            options.TrustedContext["userId"] = Convert.ToString(Session["userid"]);
            options.TrustedContext["menuId"] = Definition.Catalog.MenuId;
            options.TrustedContext["profileId"] = Definition.Profile.ProfileId;
            pageEventAdapter.Execute(options);
        }

        private IDictionary<string, object> CreatePageConfiguration()
        {
            return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { "processId", ConfiguredProcessId },
                { "readyStatus", ConfiguredStatusId },
                { "actions", PageDefinition.Actions },
                { "customerMap", Definition.Profile.CustomerMap
                    ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) }
            };
        }

        private void BindActions()
        {
            PageControlDefinition actionGrid = PageDefinition.Modals.Values
                .Where(modal => modal != null && modal.Controls != null)
                .SelectMany(modal => modal.Controls)
                .FirstOrDefault(control => control != null
                    && string.Equals(control.Id, gvActions.ID, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(control.Type, "GridView", StringComparison.OrdinalIgnoreCase));
            if (actionGrid == null)
            {
                gvActions.DataSource = null;
            }
            else
            {
                PageControlColumnDefinition buttonColumn = actionGrid.Columns.First(column =>
                    column != null && string.Equals(
                        column.Type, "Button", StringComparison.OrdinalIgnoreCase));
                IDictionary<string, object> configuration = CreatePageConfiguration();
                object configuredSource = new ExpressionResolver().Resolve(
                    actionGrid.DataSource,
                    new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "config", configuration }
                    });
                IEnumerable sourceRows = configuredSource as IEnumerable;
                if (sourceRows == null || configuredSource is string)
                {
                    throw new ApiConfigurationException(
                        "Page.json 的 GridView dataSource 不是集合：" + actionGrid.Id);
                }

                List<ActionButtonRow> rows = new List<ActionButtonRow>();
                foreach (object sourceRow in sourceRows)
                {
                    rows.Add(new ActionButtonRow
                    {
                        Text = Convert.ToString(ReadConfiguredMember(
                            sourceRow, buttonColumn.TextField)),
                        Code = Convert.ToString(ReadConfiguredMember(
                            sourceRow, buttonColumn.CommandField)),
                        Color = Convert.ToString(ReadConfiguredMember(
                            sourceRow, buttonColumn.BackgroundColorField)),
                        TextColor = Convert.ToString(ReadConfiguredMember(
                            sourceRow, buttonColumn.TextColorField))
                    });
                }
                gvActions.DataSource = rows;
            }

            gvActions.DataBind();
        }

        private static object ReadConfiguredMember(object source, string memberName)
        {
            if (source == null || string.IsNullOrWhiteSpace(memberName)) return string.Empty;
            IDictionary<string, object> dictionary = source as IDictionary<string, object>;
            if (dictionary != null)
            {
                object value;
                return dictionary.TryGetValue(memberName, out value) ? value : string.Empty;
            }

            PropertyInfo property = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(item => item.CanRead && string.Equals(
                    item.Name, memberName, StringComparison.OrdinalIgnoreCase));
            if (property == null)
            {
                throw new ApiConfigurationException(
                    "Page.json 配置的数据字段不存在：" + source.GetType().FullName + "." + memberName);
            }
            return property.GetValue(source, null);
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
            return Definition != null && HasPermission(Definition.Profile.Permission.Read);
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
            string url = loginUrl + "?ReturnUrl=" + Server.UrlEncode(returnUrl);
            if (!string.IsNullOrWhiteSpace(message))
            {
                url += "&LoginMessage=NoPermission&PermissionName=" + Server.UrlEncode(message);
            }

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowMessage(string message, bool success)
        {
            labMessage.Text = message ?? string.Empty;
            labMessage.Visible = true;
            labMessage.CssClass = success
                ? "workflow-status workflow-ok"
                : "workflow-status workflow-error";
        }

        private void ShowActionModalMessage(string message)
        {
            gvActions.Visible = false;
            labModalMessage.Text = message ?? string.Empty;
            labModalMessage.Visible = true;
            btnCloseModal.Visible = true;
            pnlActionModal.Visible = true;
        }

    }
}
