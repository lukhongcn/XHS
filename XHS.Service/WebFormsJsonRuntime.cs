using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace XHS.service
{
    public sealed class PageConfigurationLoader
    {
        private readonly ApiConfigurationLoader loader;

        public PageConfigurationLoader()
            : this(new ApiConfigurationLoader())
        {
        }

        public PageConfigurationLoader(ApiConfigurationLoader loader)
        {
            if (loader == null) throw new ArgumentNullException("loader");
            this.loader = loader;
        }

        public PageJsonDefinition Load()
        {
            PageJsonDefinition definition = JsonConfigurationCache.Load<PageJsonDefinition>(
                loader.ResolvePath("Page.json"),
                "Page.json");
            Validate(definition);
            return definition;
        }

        private static void Validate(PageJsonDefinition definition)
        {
            if (definition == null || string.IsNullOrWhiteSpace(definition.Version)
                || string.IsNullOrWhiteSpace(definition.Page)
                || definition.Events == null || definition.Events.Count == 0)
            {
                throw new ApiConfigurationException("Page.json 页面或事件配置不完整。");
            }

            if (definition.AllowedControls == null || definition.AllowedControls.Count == 0)
            {
                throw new ApiConfigurationException("Page.json 必须配置 allowedControls。");
            }

            foreach (KeyValuePair<string, PageEventDefinition> item in definition.Events)
            {
                if (string.IsNullOrWhiteSpace(item.Key) || item.Value == null)
                {
                    throw new ApiConfigurationException("Page.json 存在无效事件配置。");
                }
            }

            if (definition.Actions == null || definition.Actions.Count == 0)
            {
                throw new ApiConfigurationException("Page.json 必须配置 actions。");
            }

            HashSet<string> actionCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (PageActionDefinition action in definition.Actions)
            {
                if (action == null || string.IsNullOrWhiteSpace(action.Code)
                    || string.IsNullOrWhiteSpace(action.Text)
                    || string.IsNullOrWhiteSpace(action.Binding))
                {
                    throw new ApiConfigurationException("Page.json 存在不完整的 action。");
                }
                if (!actionCodes.Add(action.Code))
                {
                    throw new ApiConfigurationException("Page.json 存在重复 action code：" + action.Code);
                }
            }

            if (definition.Modals == null || definition.Modals.Count == 0)
            {
                throw new ApiConfigurationException("Page.json 必须配置 modals。");
            }

            foreach (KeyValuePair<string, PageModalDefinition> modalItem in definition.Modals)
            {
                PageModalDefinition modal = modalItem.Value;
                if (string.IsNullOrWhiteSpace(modalItem.Key) || modal == null
                    || string.IsNullOrWhiteSpace(modal.ControlId)
                    || modal.Controls == null || modal.Controls.Count == 0)
                {
                    throw new ApiConfigurationException("Page.json 存在不完整的 modal：" + modalItem.Key);
                }
                if (!definition.AllowedControls.Any(controlId => string.Equals(
                    controlId, modal.ControlId, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new ApiConfigurationException("modal controlId 未授权：" + modal.ControlId);
                }
                foreach (PageControlDefinition control in modal.Controls)
                {
                    if (control == null || string.IsNullOrWhiteSpace(control.Type)
                        || string.IsNullOrWhiteSpace(control.Id)
                        || !definition.AllowedControls.Any(controlId => string.Equals(
                            controlId, control.Id, StringComparison.OrdinalIgnoreCase)))
                    {
                        throw new ApiConfigurationException("modal 存在无效或未授权控件：" + modalItem.Key);
                    }
                    if (string.Equals(control.Type, "GridView", StringComparison.OrdinalIgnoreCase)
                        && (control.DataSource == null || control.Columns == null
                            || control.Columns.Count == 0))
                    {
                        throw new ApiConfigurationException(
                            "GridView 必须配置 dataSource 和 columns：" + control.Id);
                    }
                    foreach (PageControlColumnDefinition column
                        in control.Columns ?? new List<PageControlColumnDefinition>())
                    {
                        if (column == null || !string.Equals(
                            column.Type, "Button", StringComparison.OrdinalIgnoreCase)
                            || string.IsNullOrWhiteSpace(column.Id)
                            || string.IsNullOrWhiteSpace(column.TextField)
                            || string.IsNullOrWhiteSpace(column.CommandField)
                            || string.IsNullOrWhiteSpace(column.Event)
                            || !string.Equals(column.Event, "ActionButton.Click",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            throw new ApiConfigurationException(
                                "Page.json 存在不受支持的 GridView 按钮列：" + control.Id);
                        }
                    }
                }
            }
        }
    }

    public sealed class PageEventExecutionOptions
    {
        public PageEventExecutionOptions()
        {
            TrustedContext = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public Page Page { get; set; }
        public object Sender { get; set; }
        public string EventName { get; set; }
        public string BindingName { get; set; }
        public string CommandName { get; set; }
        public object ActionMetadata { get; set; }
        public object Configuration { get; set; }
        public ActionDispatcher Dispatcher { get; set; }
        public IApiPermissionEvaluator PermissionEvaluator { get; set; }
        public IDictionary<string, object> TrustedContext { get; private set; }
        public Action<Exception> ErrorLogger { get; set; }
    }

    public sealed class PageEventResult
    {
        internal PageEventResult(bool success, ActionDispatchResult dispatchResult, Exception error)
        {
            Success = success;
            DispatchResult = dispatchResult;
            Error = error;
        }

        public bool Success { get; private set; }
        public ActionDispatchResult DispatchResult { get; private set; }
        public Exception Error { get; private set; }
    }

    public sealed class PageEventAdapter
    {
        private readonly PageConfigurationLoader loader;
        private readonly ExpressionResolver resolver;
        private readonly ResponseRenderer renderer;

        public PageEventAdapter()
            : this(new PageConfigurationLoader(), new ExpressionResolver())
        {
        }

        public PageEventAdapter(PageConfigurationLoader loader, ExpressionResolver resolver)
        {
            if (loader == null) throw new ArgumentNullException("loader");
            if (resolver == null) throw new ArgumentNullException("resolver");
            this.loader = loader;
            this.resolver = resolver;
            renderer = new ResponseRenderer(resolver, new ConditionEvaluator(resolver));
        }

        public PageEventResult Execute(PageEventExecutionOptions options)
        {
            if (options == null || options.Page == null || options.Dispatcher == null)
            {
                throw new ArgumentException("PageEventAdapter 执行参数不完整。", "options");
            }

            PageJsonDefinition pageDefinition = loader.Load();
            if (!MatchesPageType(options.Page.GetType(), pageDefinition.Page))
            {
                throw new ApiConfigurationException(
                    "Page.json 页面名称不匹配：" + pageDefinition.Page);
            }

            PageEventDefinition eventDefinition;
            if (string.IsNullOrWhiteSpace(options.EventName)
                || !pageDefinition.Events.TryGetValue(options.EventName, out eventDefinition))
            {
                throw new ApiConfigurationException(
                    "Page.json 没有配置事件：" + (options.EventName ?? string.Empty));
            }

            Dictionary<string, object> controls = BuildControlScope(options.Page, pageDefinition.AllowedControls);
            Dictionary<string, object> action =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, object> input = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, object> scopes = BuildScopes(options, controls, action, input);

            try
            {
                foreach (KeyValuePair<string, object> item in BuildActionScope(options))
                {
                    action[item.Key] = item.Value;
                }
                foreach (KeyValuePair<string, object> mapping
                    in eventDefinition.Inputs ?? new Dictionary<string, object>())
                {
                    input[mapping.Key] = resolver.Resolve(mapping.Value, scopes);
                }

                object actionValue = resolver.Resolve(eventDefinition.Action, scopes);
                string actionName = Convert.ToString(actionValue).Trim();
                ActionDispatchResult dispatchResult = null;
                if (actionName.Length > 0)
                {
                    BindingExecutionContext execution = new BindingExecutionContext
                    {
                        PermissionEvaluator = options.PermissionEvaluator
                    };
                    execution.Scopes["input"] = input;
                    execution.Scopes["context"] = options.TrustedContext;
                    execution.Scopes["config"] = options.Configuration;
                    execution.Scopes["controls"] = controls;
                    execution.Scopes["action"] = action;
                    foreach (KeyValuePair<string, object> item in options.TrustedContext)
                    {
                        execution.TrustedContext[item.Key] = item.Value;
                    }

                    dispatchResult = options.Dispatcher.Dispatch(
                        options.BindingName,
                        actionName,
                        execution);
                    scopes["result"] = dispatchResult.Outputs;
                }
                else
                {
                    scopes["result"] = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                }

                renderer.Render(
                    options.Page,
                    eventDefinition.OnSuccess,
                    scopes,
                    controls,
                    pageDefinition.AllowedControls);
                return new PageEventResult(true, dispatchResult, null);
            }
            catch (Exception ex)
            {
                if (options.ErrorLogger != null) options.ErrorLogger(ex);
                scopes["error"] = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { "message", ex.Message },
                    { "type", ex.GetType().Name },
                    { "code", ex is BindingAssertionException
                        ? ((BindingAssertionException)ex).ErrorCode
                        : string.Empty }
                };
                renderer.Render(
                    options.Page,
                    eventDefinition.OnError,
                    scopes,
                    controls,
                    pageDefinition.AllowedControls);
                return new PageEventResult(false, null, ex);
            }
        }

        private static bool MatchesPageType(Type pageType, string configuredPage)
        {
            for (Type current = pageType; current != null; current = current.BaseType)
            {
                if (string.Equals(current.Name, configuredPage, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(current.FullName, configuredPage, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private Dictionary<string, object> BuildActionScope(PageEventExecutionOptions options)
        {
            Dictionary<string, object> action =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            action["metadata"] = options.ActionMetadata;
            CopyActionMetadata(action, options.ActionMetadata);

            object configuredBinding;
            bool hasConfiguredBinding = action.TryGetValue("binding", out configuredBinding)
                && !string.IsNullOrWhiteSpace(Convert.ToString(configuredBinding));
            if (!hasConfiguredBinding && !string.IsNullOrWhiteSpace(options.CommandName))
            {
                BindingActionDefinition bindingAction = options.Dispatcher.ResolveAction(
                    options.BindingName,
                    options.CommandName);
                action["commandName"] = bindingAction.CommandName;
                action["binding"] = bindingAction.Action;
                action["parameters"] = bindingAction.Parameters
                    ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            }
            else if (!string.IsNullOrWhiteSpace(options.CommandName))
            {
                action["commandName"] = options.CommandName;
            }
            return action;
        }

        private static void CopyActionMetadata(
            IDictionary<string, object> destination,
            object metadata)
        {
            if (metadata == null) return;

            IDictionary<string, object> dictionary = metadata as IDictionary<string, object>;
            if (dictionary != null)
            {
                foreach (KeyValuePair<string, object> item in dictionary)
                {
                    destination[item.Key] = item.Value;
                }
                return;
            }

            foreach (System.Reflection.PropertyInfo property in metadata.GetType()
                .GetProperties(System.Reflection.BindingFlags.Instance
                    | System.Reflection.BindingFlags.Public))
            {
                if (property.CanRead && property.GetIndexParameters().Length == 0)
                {
                    destination[property.Name] = property.GetValue(metadata, null);
                }
            }
        }

        private static Dictionary<string, object> BuildScopes(
            PageEventExecutionOptions options,
            Dictionary<string, object> controls,
            Dictionary<string, object> action,
            Dictionary<string, object> input)
        {
            return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { "input", input },
                { "context", options.TrustedContext },
                { "config", options.Configuration },
                { "controls", controls },
                { "action", action },
                { "row", null },
                { "result", new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) }
            };
        }

        private static Dictionary<string, object> BuildControlScope(
            Page page,
            IEnumerable<string> allowedControls)
        {
            Dictionary<string, object> controls =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (string controlId in allowedControls)
            {
                Control control = FindControlRecursive(page, controlId);
                if (control == null)
                {
                    throw new ApiConfigurationException(
                        "Page.json 引用了不存在的控件：" + controlId);
                }
                controls[controlId] = control;
            }
            return controls;
        }

        internal static Control FindControlRecursive(Control root, string id)
        {
            if (root == null) return null;
            if (string.Equals(root.ID, id, StringComparison.OrdinalIgnoreCase)) return root;
            foreach (Control child in root.Controls)
            {
                Control found = FindControlRecursive(child, id);
                if (found != null) return found;
            }
            return null;
        }
    }

    public sealed class ResponseRenderer
    {
        private static readonly Regex TemplateExpression =
            new Regex(@"\$\{([^}]+)\}", RegexOptions.Compiled);
        private readonly ExpressionResolver resolver;
        private readonly ConditionEvaluator conditionEvaluator;

        public ResponseRenderer(ExpressionResolver resolver, ConditionEvaluator conditionEvaluator)
        {
            this.resolver = resolver;
            this.conditionEvaluator = conditionEvaluator;
        }

        public void Render(
            Page page,
            IList<PageResponseDefinition> responses,
            IDictionary<string, object> scopes,
            IDictionary<string, object> controls,
            IList<string> allowedControls)
        {
            if (responses == null) return;
            HashSet<string> allowed = new HashSet<string>(
                allowedControls ?? new List<string>(),
                StringComparer.OrdinalIgnoreCase);
            foreach (PageResponseDefinition response in responses)
            {
                if (response == null || !conditionEvaluator.Evaluate(response.When, scopes)) continue;
                string type = (response.Type ?? string.Empty).Trim();
                Control target = null;
                if (!string.IsNullOrWhiteSpace(response.Target))
                {
                    if (!allowed.Contains(response.Target))
                    {
                        throw new ApiConfigurationException(
                            "Page.json 无权操作控件：" + response.Target);
                    }
                    object targetValue;
                    if (!ExpressionResolver.TryGetDictionaryValue(controls, response.Target, out targetValue))
                    {
                        throw new ApiConfigurationException("页面控件不存在：" + response.Target);
                    }
                    target = targetValue as Control;
                }

                if (string.Equals(type, "show", StringComparison.OrdinalIgnoreCase))
                {
                    target.Visible = true;
                }
                else if (string.Equals(type, "hide", StringComparison.OrdinalIgnoreCase))
                {
                    target.Visible = false;
                }
                else if (string.Equals(type, "setValue", StringComparison.OrdinalIgnoreCase))
                {
                    SetValue(target, resolver.Resolve(response.Value, scopes), false);
                }
                else if (string.Equals(type, "setText", StringComparison.OrdinalIgnoreCase))
                {
                    SetValue(target, ResolveText(response.Value, scopes), true);
                }
                else if (string.Equals(type, "clear", StringComparison.OrdinalIgnoreCase))
                {
                    SetValue(target, string.Empty, false);
                }
                else if (string.Equals(type, "showMessage", StringComparison.OrdinalIgnoreCase))
                {
                    Label label = target as Label;
                    if (label == null) throw new ApiConfigurationException("showMessage 目标必须是 Label。");
                    label.Text = HttpUtility.HtmlEncode(ResolveText(response.Message, scopes));
                    if (!string.IsNullOrWhiteSpace(response.CssClass)) label.CssClass = response.CssClass;
                    label.Visible = true;
                }
                else if (string.Equals(type, "bindGrid", StringComparison.OrdinalIgnoreCase))
                {
                    BindGrid(target as GridView, response, scopes);
                }
                else if (string.Equals(type, "focus", StringComparison.OrdinalIgnoreCase))
                {
                    Focus(page, target);
                }
                else
                {
                    throw new ApiConfigurationException("不支持的页面响应类型：" + type);
                }
            }
        }

        private void BindGrid(
            GridView grid,
            PageResponseDefinition response,
            IDictionary<string, object> scopes)
        {
            if (grid == null) throw new ApiConfigurationException("bindGrid 目标必须是 GridView。");
            object dataValue = resolver.Resolve(response.Data, scopes);
            IEnumerable data = dataValue as IEnumerable;
            if (data == null) throw new ApiExecutionException("bindGrid 数据不是集合。");
            if (response.Projection == null || response.Projection.Count == 0)
            {
                grid.DataSource = data;
                grid.DataBind();
                return;
            }

            DataTable table = new DataTable();
            foreach (string column in response.Projection.Keys) table.Columns.Add(column, typeof(object));
            int index = 1;
            foreach (object sourceRow in data)
            {
                Dictionary<string, object> rowScopes = CopyScopes(scopes);
                rowScopes["row"] = sourceRow;
                rowScopes["index"] = index++;
                DataRow row = table.NewRow();
                foreach (KeyValuePair<string, PageProjectionFieldDefinition> field in response.Projection)
                {
                    object value = null;
                    if (field.Value != null && conditionEvaluator.Evaluate(field.Value.When, rowScopes))
                    {
                        value = resolver.Resolve(field.Value.Value, rowScopes);
                        if (field.Value.Map != null)
                        {
                            object mapValue = resolver.Resolve(field.Value.Map, rowScopes);
                            IDictionary map = mapValue as IDictionary;
                            bool found = false;
                            if (map != null)
                            {
                                foreach (DictionaryEntry entry in map)
                                {
                                    if (string.Equals(
                                        Convert.ToString(entry.Key),
                                        Convert.ToString(value),
                                        StringComparison.OrdinalIgnoreCase))
                                    {
                                        value = entry.Value;
                                        found = true;
                                        break;
                                    }
                                }
                            }
                            if (!found && field.Value.Fallback != null)
                            {
                                value = resolver.Resolve(field.Value.Fallback, rowScopes);
                            }
                        }
                    }
                    row[field.Key] = value ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            grid.DataSource = table;
            grid.DataBind();
        }

        private string ResolveText(object value, IDictionary<string, object> scopes)
        {
            string text = value as string;
            if (text == null) return Convert.ToString(resolver.Resolve(value, scopes));
            if (text.StartsWith("$", StringComparison.Ordinal) && !text.StartsWith("${", StringComparison.Ordinal))
            {
                return Convert.ToString(resolver.Resolve(text, scopes));
            }
            return TemplateExpression.Replace(text, match =>
                Convert.ToString(resolver.Resolve("$" + match.Groups[1].Value, scopes)));
        }

        private static void SetValue(Control target, object value, bool encodeLabel)
        {
            string text = Convert.ToString(value);
            TextBox textBox = target as TextBox;
            if (textBox != null) { textBox.Text = text; return; }
            HiddenField hidden = target as HiddenField;
            if (hidden != null) { hidden.Value = text; return; }
            Label label = target as Label;
            if (label != null)
            {
                label.Text = encodeLabel ? HttpUtility.HtmlEncode(text) : HttpUtility.HtmlEncode(text);
                return;
            }
            throw new ApiConfigurationException("setValue/clear 不支持控件类型：" + target.GetType().Name);
        }

        private static void Focus(Page page, Control target)
        {
            if (page == null || target == null) return;
            string clientId = HttpUtility.JavaScriptStringEncode(target.ClientID);
            string script = "setTimeout(function(){var e=document.getElementById('"
                + clientId + "');if(e){e.focus();try{e.select()}catch(x){}}},40);";
            ScriptManager manager = ScriptManager.GetCurrent(page);
            if (manager != null)
            {
                ScriptManager.RegisterStartupScript(
                    page,
                    page.GetType(),
                    "jsonPageFocus_" + target.ClientID,
                    script,
                    true);
            }
            else
            {
                page.ClientScript.RegisterStartupScript(
                    page.GetType(),
                    "jsonPageFocus_" + target.ClientID,
                    script,
                    true);
            }
        }

        private static Dictionary<string, object> CopyScopes(IDictionary<string, object> scopes)
        {
            Dictionary<string, object> copy =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, object> item in scopes) copy[item.Key] = item.Value;
            return copy;
        }
    }

    [Serializable]
    public sealed class PageJsonDefinition
    {
        public string Version { get; set; }
        public string Page { get; set; }
        public List<string> AllowedControls { get; set; }
        public Dictionary<string, PageModalDefinition> Modals { get; set; }
        public List<PageActionDefinition> Actions { get; set; }
        public Dictionary<string, PageEventDefinition> Events { get; set; }
    }

    [Serializable]
    public sealed class PageModalDefinition
    {
        public string ControlId { get; set; }
        public List<PageControlDefinition> Controls { get; set; }
    }

    [Serializable]
    public sealed class PageControlDefinition
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public object DataSource { get; set; }
        public List<PageControlColumnDefinition> Columns { get; set; }
    }

    [Serializable]
    public sealed class PageControlColumnDefinition
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string TextField { get; set; }
        public string CommandField { get; set; }
        public string BackgroundColorField { get; set; }
        public string TextColorField { get; set; }
        public string Event { get; set; }
    }

    [Serializable]
    public sealed class PageActionDefinition
    {
        public string Code { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }
        public string Binding { get; set; }
    }

    [Serializable]
    public sealed class PageEventDefinition
    {
        public object Action { get; set; }
        public Dictionary<string, object> Inputs { get; set; }
        public List<PageResponseDefinition> OnSuccess { get; set; }
        public List<PageResponseDefinition> OnError { get; set; }
    }

    [Serializable]
    public sealed class PageResponseDefinition
    {
        public string Type { get; set; }
        public string Target { get; set; }
        public object Value { get; set; }
        public object Message { get; set; }
        public object Data { get; set; }
        public string CssClass { get; set; }
        public ConditionDefinition When { get; set; }
        public Dictionary<string, PageProjectionFieldDefinition> Projection { get; set; }
    }

    [Serializable]
    public sealed class PageProjectionFieldDefinition
    {
        public object Value { get; set; }
        public object Map { get; set; }
        public object Fallback { get; set; }
        public ConditionDefinition When { get; set; }
    }
}
