using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Web.Compilation;

namespace XHS.service
{
    public sealed class ActionDispatcher
    {
        private static readonly ConcurrentDictionary<string, object> ActionLocks =
            new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        private static readonly string[] AllowedServicePrefixes =
        {
            "XHS.BLL.",
            "ModuleWorkFlow.BLL.",
            "ModuleWorkFlow.business."
        };

        private readonly Dictionary<string, ApiDefinition> apis;
        private readonly Dictionary<string, ActionBindingDefinition> actions;
        private readonly Dictionary<string, BindingActionDefinition> commands;
        private readonly ExpressionResolver resolver;
        private readonly ConditionEvaluator conditionEvaluator;

        public ActionDispatcher(ApiConfigurationLoader loader)
        {
            if (loader == null) throw new ArgumentNullException("loader");
            resolver = new ExpressionResolver();
            conditionEvaluator = new ConditionEvaluator(resolver);

            apis = BuildApiIndex(loader.LoadRegister());
            actions = BuildActionIndex(loader.LoadBinding());
            commands = BuildCommandIndex(loader.LoadBinding());
            foreach (ApiDefinition api in apis.Values) ResolveApiMethod(api);
            ValidateBindings();
        }

        public void EnsureBindingName(string bindingName)
        {
            if (string.IsNullOrWhiteSpace(bindingName)
                || !actions.Values.Any(item => string.Equals(
                    item.BindingName, bindingName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ApiConfigurationException(
                    "Binding.json 没有配置 bindingName：" + (bindingName ?? string.Empty));
            }
        }

        public void EnsureAction(string bindingName, string actionName)
        {
            if (string.IsNullOrWhiteSpace(actionName)
                || !actions.ContainsKey(GetKey(bindingName, actionName)))
            {
                throw new ApiConfigurationException(
                    "Binding.json 没有配置动作：" + (bindingName ?? string.Empty)
                    + "/" + (actionName ?? string.Empty));
            }
        }

        public BindingActionDefinition ResolveAction(string bindingName, string commandName)
        {
            BindingActionDefinition command;
            if (!commands.TryGetValue(GetKey(bindingName, commandName), out command))
            {
                throw new ApiExecutionException(
                    "CommandName 没有配置动作：" + bindingName + "/" + commandName);
            }
            return command;
        }

        public ActionDispatchResult Dispatch(
            string bindingName,
            string actionName,
            BindingExecutionContext executionContext)
        {
            if (executionContext == null) throw new ArgumentNullException("executionContext");
            ActionBindingDefinition action;
            if (!actions.TryGetValue(GetKey(bindingName, actionName), out action))
            {
                throw new ApiExecutionException(
                    "没有找到 Binding 动作：" + bindingName + "/" + actionName);
            }

            if (action.LockKey == null)
            {
                return DispatchCore(actionName, action, executionContext);
            }

            object resolvedKey = resolver.Resolve(action.LockKey, executionContext.Scopes);
            string lockKey = Convert.ToString(resolvedKey).Trim();
            if (lockKey.Length == 0)
            {
                throw new ApiExecutionException("Binding lockKey 解析为空：" + actionName);
            }

            object actionLock = ActionLocks.GetOrAdd(bindingName + "|" + lockKey, ignored => new object());
            lock (actionLock)
            {
                return DispatchCore(actionName, action, executionContext);
            }
        }

        private ActionDispatchResult DispatchCore(
            string actionName,
            ActionBindingDefinition action,
            BindingExecutionContext executionContext)
        {
            Dictionary<string, object> scopes = CopyScopes(executionContext.Scopes);
            Dictionary<string, object> steps = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, object> result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            scopes["steps"] = steps;
            scopes["result"] = result;

            int sequence = 0;
            foreach (ApiBindingStepDefinition step in action.Steps
                .Select(item => new { Step = item, Sequence = sequence++ })
                .OrderBy(item => item.Step.Order == 0 ? item.Sequence : item.Step.Order)
                .Select(item => item.Step))
            {
                string stepId = GetStepId(step);
                if (!conditionEvaluator.Evaluate(step.When, scopes)) continue;

                string stepType = (step.Type ?? string.Empty).Trim();
                if (stepType.Length == 0 && !string.IsNullOrWhiteSpace(GetApiId(step)))
                {
                    stepType = "api";
                }

                IDictionary<string, object> stepResult;
                if (string.Equals(stepType, "api", StringComparison.OrdinalIgnoreCase))
                {
                    stepResult = ExecuteApi(GetApiId(step), step.Inputs, scopes, executionContext);
                }
                else if (string.Equals(stepType, "assert", StringComparison.OrdinalIgnoreCase))
                {
                    if (!conditionEvaluator.Evaluate(step.Condition, scopes))
                    {
                        throw new BindingAssertionException(
                            step.ErrorCode,
                            string.IsNullOrWhiteSpace(step.ErrorMessage)
                                ? "Binding 条件检查失败：" + stepId
                                : step.ErrorMessage);
                    }
                    stepResult = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "passed", true }
                    };
                }
                else if (string.Equals(stepType, "select", StringComparison.OrdinalIgnoreCase))
                {
                    stepResult = ExecuteSelect(step, scopes);
                }
                else if (string.Equals(stepType, "set", StringComparison.OrdinalIgnoreCase))
                {
                    stepResult = ExecuteSet(step, scopes);
                }
                else
                {
                    throw new ApiConfigurationException("不支持的 Binding step type：" + stepType);
                }

                steps[stepId] = stepResult;
                if (step.Outputs != null)
                {
                    foreach (KeyValuePair<string, object> mapping in step.Outputs)
                    {
                        result[mapping.Key] = resolver.Resolve(mapping.Value, scopes);
                    }
                }
            }

            foreach (KeyValuePair<string, object> output
                in action.Outputs ?? new Dictionary<string, object>())
            {
                result[output.Key] = resolver.Resolve(output.Value, scopes);
            }

            return new ActionDispatchResult(actionName, result, steps);
        }

        private IDictionary<string, object> ExecuteApi(
            string apiId,
            Dictionary<string, object> inputMappings,
            IDictionary<string, object> scopes,
            BindingExecutionContext executionContext)
        {
            ApiDefinition api;
            if (string.IsNullOrWhiteSpace(apiId) || !apis.TryGetValue(apiId, out api))
            {
                throw new ApiConfigurationException("Binding 引用了未注册 API：" + apiId);
            }

            EnsurePermission(api, executionContext.PermissionEvaluator);
            Dictionary<string, object> inputs = ResolveMap(inputMappings, scopes);
            ValidateParameters(api.ApiId, api.Inputs, inputs, "输入");
            IDictionary<string, object> outputs = InvokeService(api, inputs);
            ValidateParameters(api.ApiId, api.Outputs, outputs, "输出");
            return outputs;
        }

        private IDictionary<string, object> InvokeService(
            ApiDefinition api,
            IDictionary<string, object> inputs)
        {
            Type serviceType = ResolveServiceType(api.Service);
            List<string> parameterNames = api.Parameters ?? new List<string>();
            MethodInfo method = ResolveApiMethod(api);
            ParameterInfo[] methodParameters = method.GetParameters();
            object[] arguments = new object[parameterNames.Count];
            for (int index = 0; index < parameterNames.Count; index++)
            {
                string inputName = parameterNames[index];
                object value;
                if (!inputs.TryGetValue(inputName, out value))
                {
                    throw new ApiExecutionException("API " + api.ApiId + " 缺少方法参数：" + inputName);
                }
                arguments[index] = ConvertArgument(value, methodParameters[index].ParameterType, inputName);
            }

            object service = api.Static ? null : Activator.CreateInstance(serviceType);
            object valueResult;
            try
            {
                valueResult = method.Invoke(service, arguments);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                }
                throw;
            }

            return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { string.IsNullOrWhiteSpace(api.Output) ? "result" : api.Output, valueResult }
            };
        }

        private IDictionary<string, object> ExecuteSelect(
            ApiBindingStepDefinition step,
            IDictionary<string, object> scopes)
        {
            object source = resolver.Resolve(step.Source, scopes);
            IEnumerable enumerable = source as IEnumerable;
            if (enumerable == null || source is string)
            {
                throw new ApiExecutionException("select step 的 source 不是集合：" + GetStepId(step));
            }

            Type elementType = source.GetType().IsGenericType
                ? source.GetType().GetGenericArguments().FirstOrDefault()
                : null;
            IList selected = elementType == null
                ? (IList)new ArrayList()
                : (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
            foreach (object row in enumerable)
            {
                Dictionary<string, object> rowScopes = CopyScopes(scopes);
                rowScopes["row"] = row;
                if (conditionEvaluator.Evaluate(step.Where, rowScopes)) selected.Add(row);
            }
            return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { string.IsNullOrWhiteSpace(step.Output) ? "items" : step.Output, selected }
            };
        }

        private IDictionary<string, object> ExecuteSet(
            ApiBindingStepDefinition step,
            IDictionary<string, object> scopes)
        {
            object source = resolver.Resolve(step.Source, scopes);
            IEnumerable rows = source as IEnumerable;
            if (source == null || source is string)
            {
                throw new ApiExecutionException("set step 的 source 为空：" + GetStepId(step));
            }

            if (rows != null)
            {
                foreach (object row in rows) SetProperties(row, step.Set, scopes);
            }
            else
            {
                SetProperties(source, step.Set, scopes);
            }

            return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { string.IsNullOrWhiteSpace(step.Output) ? "value" : step.Output, source }
            };
        }

        private void SetProperties(
            object target,
            Dictionary<string, object> mappings,
            IDictionary<string, object> scopes)
        {
            if (target == null) throw new ApiExecutionException("set step 不能修改空对象。");
            foreach (KeyValuePair<string, object> mapping
                in mappings ?? new Dictionary<string, object>())
            {
                PropertyInfo property = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(item => item.CanWrite
                        && string.Equals(item.Name, mapping.Key, StringComparison.OrdinalIgnoreCase));
                if (property == null)
                {
                    throw new ApiConfigurationException(
                        "set step 找不到可写属性：" + target.GetType().FullName + "." + mapping.Key);
                }
                property.SetValue(
                    target,
                    ConvertArgument(resolver.Resolve(mapping.Value, scopes), property.PropertyType, mapping.Key),
                    null);
            }
        }

        private static Type ResolveServiceType(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName)
                || !AllowedServicePrefixes.Any(prefix => serviceName.StartsWith(
                    prefix, StringComparison.Ordinal)))
            {
                throw new ApiConfigurationException("API service 不在允许范围：" + serviceName);
            }

            Type type = BuildManager.GetType(serviceName, false, true)
                ?? AppDomain.CurrentDomain.GetAssemblies()
                    .Select(assembly => assembly.GetType(serviceName, false, true))
                    .FirstOrDefault(item => item != null);
            if (type == null)
            {
                throw new ApiConfigurationException("找不到 API service：" + serviceName);
            }
            return type;
        }

        private static MethodInfo ResolveApiMethod(ApiDefinition api)
        {
            Type serviceType = ResolveServiceType(api.Service);
            int parameterCount = (api.Parameters ?? new List<string>()).Count;
            MethodInfo[] candidates = serviceType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                .Where(candidateMethod => string.Equals(candidateMethod.Name, api.Method, StringComparison.Ordinal)
                    && candidateMethod.IsStatic == api.Static
                    && !candidateMethod.ContainsGenericParameters
                    && candidateMethod.GetParameters().Length == parameterCount
                    && candidateMethod.GetParameters().All(parameter => !parameter.IsOut
                        && !parameter.ParameterType.IsByRef
                        && !parameter.ParameterType.IsPointer
                        && !parameter.IsOptional))
                .ToArray();
            if (candidates.Length != 1)
            {
                throw new ApiConfigurationException(
                    candidates.Length == 0
                        ? "找不到受支持的 API 方法：" + api.Service + "." + api.Method
                        : "API 方法存在无法确定的重载：" + api.Service + "." + api.Method);
            }
            return candidates[0];
        }

        private static object ConvertArgument(object value, Type targetType, string name)
        {
            if (value == null)
            {
                if (targetType.IsValueType && Nullable.GetUnderlyingType(targetType) == null)
                {
                    throw new ApiExecutionException("方法参数不能为 null：" + name);
                }
                return null;
            }
            if (targetType.IsInstanceOfType(value)) return value;

            Type actualType = Nullable.GetUnderlyingType(targetType) ?? targetType;
            try
            {
                if (actualType.IsEnum) return Enum.Parse(actualType, Convert.ToString(value), true);
                return Convert.ChangeType(value, actualType);
            }
            catch (Exception ex)
            {
                throw new ApiExecutionException("方法参数类型错误：" + name, ex);
            }
        }

        private void EnsurePermission(ApiDefinition api, IApiPermissionEvaluator evaluator)
        {
            if (string.IsNullOrWhiteSpace(api.Permission)) return;
            if (evaluator == null || !evaluator.IsAllowed(api.Permission))
            {
                throw new ApiPermissionException("没有执行 API 的权限：" + api.ApiId);
            }
        }

        private Dictionary<string, object> ResolveMap(
            Dictionary<string, object> mappings,
            IDictionary<string, object> scopes)
        {
            Dictionary<string, object> values =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, object> mapping
                in mappings ?? new Dictionary<string, object>())
            {
                values[mapping.Key] = resolver.Resolve(mapping.Value, scopes);
            }
            return values;
        }

        private static void ValidateParameters(
            string apiId,
            Dictionary<string, ApiParameterDefinition> contract,
            IDictionary<string, object> values,
            string direction)
        {
            Dictionary<string, ApiParameterDefinition> definitions = contract
                ?? new Dictionary<string, ApiParameterDefinition>(StringComparer.OrdinalIgnoreCase);
            foreach (string suppliedName in values.Keys)
            {
                if (!definitions.Keys.Any(name => string.Equals(
                    name, suppliedName, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new ApiExecutionException(
                        "API " + apiId + " 包含未注册的" + direction + "参数：" + suppliedName);
                }
            }
            foreach (KeyValuePair<string, ApiParameterDefinition> item in definitions)
            {
                if (item.Value == null)
                {
                    throw new ApiConfigurationException(
                        "API " + apiId + " 的" + direction + "契约为空：" + item.Key);
                }
                object value;
                if (!values.TryGetValue(item.Key, out value))
                {
                    if (item.Value.Required)
                    {
                        throw new ApiExecutionException(
                            "API " + apiId + " 缺少必填" + direction + "：" + item.Key);
                    }
                    continue;
                }
                if (item.Value.Required && (value == null
                    || (value is string && string.IsNullOrWhiteSpace((string)value))))
                {
                    throw new ApiExecutionException(
                        "API " + apiId + " 的必填" + direction + "为空：" + item.Key);
                }
            }
        }

        private static Dictionary<string, ApiDefinition> BuildApiIndex(ApiRegisterDocument document)
        {
            if (document == null || document.Apis == null || document.Apis.Count == 0)
            {
                throw new ApiConfigurationException("Register.json 没有配置 API。");
            }
            Dictionary<string, ApiDefinition> index =
                new Dictionary<string, ApiDefinition>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, ApiDefinition> item in document.Apis)
            {
                ApiDefinition api = item.Value;
                if (api == null || string.IsNullOrWhiteSpace(item.Key)
                    || string.IsNullOrWhiteSpace(api.Service)
                    || string.IsNullOrWhiteSpace(api.Method)
                    || string.IsNullOrWhiteSpace(api.Output))
                {
                    throw new ApiConfigurationException("Register.json 存在不完整的 API 定义：" + item.Key);
                }
                api.ApiId = item.Key;
                index.Add(item.Key, api);
            }
            return index;
        }

        private static Dictionary<string, ActionBindingDefinition> BuildActionIndex(
            ApiBindingDocument document)
        {
            if (document == null || document.Actions == null || document.Actions.Count == 0)
            {
                throw new ApiConfigurationException("Binding.json 没有配置 actions。");
            }
            Dictionary<string, ActionBindingDefinition> index =
                new Dictionary<string, ActionBindingDefinition>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, ActionBindingDefinition> item in document.Actions)
            {
                ActionBindingDefinition action = item.Value;
                if (action == null || string.IsNullOrWhiteSpace(action.BindingName)
                    || action.Steps == null || action.Steps.Count == 0)
                {
                    throw new ApiConfigurationException("Binding action 配置不完整：" + item.Key);
                }
                action.Action = item.Key;
                HashSet<string> stepIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (ApiBindingStepDefinition step in action.Steps)
                {
                    if (step == null || !stepIds.Add(GetStepId(step)))
                    {
                        throw new ApiConfigurationException("Binding step 无效或重复：" + item.Key);
                    }
                }
                index.Add(GetKey(action.BindingName, item.Key), action);
            }
            return index;
        }

        private static Dictionary<string, BindingActionDefinition> BuildCommandIndex(
            ApiBindingDocument document)
        {
            Dictionary<string, BindingActionDefinition> index =
                new Dictionary<string, BindingActionDefinition>(StringComparer.OrdinalIgnoreCase);
            foreach (BindingActionDefinition command
                in document.Commands ?? new List<BindingActionDefinition>())
            {
                if (command == null || string.IsNullOrWhiteSpace(command.BindingName)
                    || string.IsNullOrWhiteSpace(command.CommandName)
                    || string.IsNullOrWhiteSpace(command.Action))
                {
                    throw new ApiConfigurationException("Binding.json 存在不完整的 command。");
                }
                index.Add(GetKey(command.BindingName, command.CommandName), command);
            }
            return index;
        }

        private void ValidateBindings()
        {
            foreach (ActionBindingDefinition action in actions.Values)
            {
                foreach (ApiBindingStepDefinition step in action.Steps)
                {
                    string apiId = GetApiId(step);
                    if (!string.IsNullOrWhiteSpace(apiId) && !apis.ContainsKey(apiId))
                    {
                        throw new ApiConfigurationException("Binding 引用了未注册 API：" + apiId);
                    }
                }
            }
            foreach (BindingActionDefinition command in commands.Values)
            {
                if (!actions.ContainsKey(GetKey(command.BindingName, command.Action)))
                {
                    throw new ApiConfigurationException("command 引用了不存在的 action：" + command.Action);
                }
            }
        }

        private static string GetStepId(ApiBindingStepDefinition step)
        {
            string value = !string.IsNullOrWhiteSpace(step.Id) ? step.Id : step.StepId;
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ApiConfigurationException("Binding step 缺少 id。");
            }
            return value.Trim();
        }

        private static string GetApiId(ApiBindingStepDefinition step)
        {
            return !string.IsNullOrWhiteSpace(step.Api) ? step.Api.Trim() : (step.ApiId ?? string.Empty).Trim();
        }

        private static Dictionary<string, object> CopyScopes(IDictionary<string, object> source)
        {
            Dictionary<string, object> copy =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (source != null)
            {
                foreach (KeyValuePair<string, object> item in source) copy[item.Key] = item.Value;
            }
            return copy;
        }

        private static string GetKey(string bindingName, string name)
        {
            if (string.IsNullOrWhiteSpace(bindingName) || string.IsNullOrWhiteSpace(name))
            {
                throw new ApiConfigurationException("bindingName 和名称不能为空。");
            }
            return bindingName.Trim() + "\n" + name.Trim();
        }
    }
}
