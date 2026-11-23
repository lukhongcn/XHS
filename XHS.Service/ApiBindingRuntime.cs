using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;

namespace XHS.service
{
    public interface IApiHandler
    {
        ApiExecutionResult Execute(ApiExecutionContext context);
    }

    public interface IApiPermissionEvaluator
    {
        bool IsAllowed(string permission);
    }

    public sealed class DelegateApiPermissionEvaluator : IApiPermissionEvaluator
    {
        private readonly Func<string, bool> evaluator;

        public DelegateApiPermissionEvaluator(Func<string, bool> evaluator)
        {
            if (evaluator == null) throw new ArgumentNullException("evaluator");
            this.evaluator = evaluator;
        }

        public bool IsAllowed(string permission)
        {
            return evaluator(permission);
        }
    }

    public sealed class ApiExecutionContext
    {
        public ApiExecutionContext(
            ApiDefinition definition,
            IDictionary<string, object> inputs,
            IDictionary<string, object> trustedContext)
        {
            if (definition == null) throw new ArgumentNullException("definition");
            Definition = definition;
            Inputs = inputs ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            TrustedContext = trustedContext
                ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public ApiDefinition Definition { get; private set; }
        public IDictionary<string, object> Inputs { get; private set; }
        public IDictionary<string, object> TrustedContext { get; private set; }

        public T GetRequired<T>(string name)
        {
            object value;
            if (string.IsNullOrWhiteSpace(name) || !Inputs.TryGetValue(name, out value) || value == null)
            {
                throw new ApiExecutionException("API " + Definition.ApiId + " 缺少输入参数：" + name);
            }

            return ConvertValue<T>(name, value);
        }

        public T GetOptional<T>(string name, T defaultValue)
        {
            object value;
            return Inputs.TryGetValue(name, out value) && value != null
                ? ConvertValue<T>(name, value)
                : defaultValue;
        }

        private T ConvertValue<T>(string name, object value)
        {
            if (value is T) return (T)value;
            try
            {
                Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                if (targetType == typeof(bool) && value is string)
                {
                    bool parsed;
                    if (bool.TryParse(Convert.ToString(value), out parsed))
                    {
                        return (T)(object)parsed;
                    }
                }

                return (T)Convert.ChangeType(value, targetType);
            }
            catch (Exception ex)
            {
                throw new ApiExecutionException(
                    "API " + Definition.ApiId + " 输入参数类型错误：" + name,
                    ex);
            }
        }
    }

    public sealed class ApiExecutionResult
    {
        public ApiExecutionResult()
        {
            Outputs = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string, object> Outputs { get; private set; }

        public ApiExecutionResult Add(string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("API 输出名称不能为空。", "name");
            }

            Outputs[name] = value;
            return this;
        }
    }

    public sealed class ApiRegistry
    {
        private readonly Dictionary<string, Func<IApiHandler>> handlers =
            new Dictionary<string, Func<IApiHandler>>(StringComparer.OrdinalIgnoreCase);

        public void Register(string handlerName, Func<IApiHandler> factory)
        {
            if (string.IsNullOrWhiteSpace(handlerName) || factory == null)
            {
                throw new ApiConfigurationException("API Handler 白名单注册不完整。");
            }

            if (handlers.ContainsKey(handlerName))
            {
                throw new ApiConfigurationException("API Handler 重复注册：" + handlerName);
            }

            handlers.Add(handlerName, factory);
        }

        internal IApiHandler Create(string handlerName)
        {
            Func<IApiHandler> factory;
            if (string.IsNullOrWhiteSpace(handlerName) || !handlers.TryGetValue(handlerName, out factory))
            {
                throw new ApiConfigurationException(
                    "API Handler 未加入服务器白名单：" + (handlerName ?? string.Empty));
            }

            IApiHandler handler = factory();
            if (handler == null)
            {
                throw new ApiConfigurationException("API Handler 工厂返回空值：" + handlerName);
            }

            return handler;
        }

        internal bool Contains(string handlerName)
        {
            return !string.IsNullOrWhiteSpace(handlerName) && handlers.ContainsKey(handlerName);
        }
    }

    internal static class JsonConfigurationCache
    {
        private sealed class CacheItem
        {
            public DateTime LastWriteTimeUtc { get; set; }
            public object Value { get; set; }
        }

        private static readonly object SyncRoot = new object();
        private static readonly Dictionary<string, CacheItem> Items =
            new Dictionary<string, CacheItem>(StringComparer.OrdinalIgnoreCase);

        public static T Load<T>(string path, string displayName)
        {
            if (!File.Exists(path))
            {
                throw new ApiConfigurationException("JSON 配置文件不存在：" + displayName);
            }

            DateTime lastWrite = File.GetLastWriteTimeUtc(path);
            lock (SyncRoot)
            {
                CacheItem cached;
                if (Items.TryGetValue(path, out cached)
                    && cached.LastWriteTimeUtc == lastWrite
                    && cached.Value is T)
                {
                    return (T)cached.Value;
                }

                try
                {
                    T value = new JavaScriptSerializer().Deserialize<T>(File.ReadAllText(path));
                    if (value == null)
                    {
                        throw new ApiConfigurationException("JSON 配置内容为空：" + displayName);
                    }

                    Items[path] = new CacheItem { LastWriteTimeUtc = lastWrite, Value = value };
                    return value;
                }
                catch (ApiConfigurationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ApiConfigurationException("JSON 配置解析失败：" + displayName, ex);
                }
            }
        }
    }

    public sealed class ApiConfigurationLoader
    {
        private readonly string jsonRoot;

        public ApiConfigurationLoader() : this(HttpRuntime.AppDomainAppPath) { }

        public ApiConfigurationLoader(string applicationRoot)
        {
            if (string.IsNullOrWhiteSpace(applicationRoot))
            {
                throw new ApiConfigurationException("无法确定 Web 应用根目录。");
            }

            jsonRoot = Path.Combine(Path.GetFullPath(applicationRoot), "Json", "Pda");
        }

        public ApiRegisterDocument LoadRegister()
        {
            ApiRegisterDocument document = JsonConfigurationCache.Load<ApiRegisterDocument>(
                ResolvePath("Register.json"), "Register.json");
            if (string.IsNullOrWhiteSpace(document.Version))
            {
                throw new ApiConfigurationException("Register.json 缺少 version。");
            }
            return document;
        }

        public ApiBindingDocument LoadBinding()
        {
            ApiBindingDocument document = JsonConfigurationCache.Load<ApiBindingDocument>(
                ResolvePath("Binding.json"), "Binding.json");
            if (string.IsNullOrWhiteSpace(document.Version))
            {
                throw new ApiConfigurationException("Binding.json 缺少 version。");
            }
            return document;
        }

        internal string ResolvePath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath) || Path.IsPathRooted(relativePath))
            {
                throw new ApiConfigurationException("JSON 配置路径无效：" + relativePath);
            }

            string root = EnsureSeparator(Path.GetFullPath(jsonRoot));
            string path = Path.GetFullPath(Path.Combine(
                root, relativePath.Replace('/', Path.DirectorySeparatorChar)));
            if (!path.StartsWith(root, StringComparison.OrdinalIgnoreCase))
            {
                throw new ApiConfigurationException("JSON 配置路径越界：" + relativePath);
            }

            return path;
        }

        private static string EnsureSeparator(string path)
        {
            return path.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
                ? path : path + Path.DirectorySeparatorChar;
        }
    }

    [Serializable]
    public sealed class ApiRegisterDocument
    {
        public string Version { get; set; }
        public Dictionary<string, ApiDefinition> Apis { get; set; }
    }

    [Serializable]
    public sealed class ApiDefinition
    {
        public string ApiId { get; set; }
        public string Handler { get; set; }
        public string Service { get; set; }
        public string Method { get; set; }
        public List<string> Parameters { get; set; }
        public string Output { get; set; }
        public string OutputType { get; set; }
        public bool Static { get; set; }
        public string Operation { get; set; }
        public string Permission { get; set; }
        public Dictionary<string, ApiParameterDefinition> Inputs { get; set; }
        public Dictionary<string, ApiParameterDefinition> Outputs { get; set; }
    }

    [Serializable]
    public sealed class ApiParameterDefinition
    {
        public string Type { get; set; }
        public bool Required { get; set; }
    }

    [Serializable]
    public sealed class ApiBindingDocument
    {
        public string Version { get; set; }
        public Dictionary<string, ActionBindingDefinition> Actions { get; set; }
        public List<BindingActionDefinition> Commands { get; set; }
    }

    [Serializable]
    public sealed class ActionBindingDefinition
    {
        public string BindingName { get; set; }
        public string Action { get; set; }
        public object LockKey { get; set; }
        public List<ApiBindingStepDefinition> Steps { get; set; }
        public Dictionary<string, object> Outputs { get; set; }
    }

    [Serializable]
    public sealed class ApiBindingStepDefinition
    {
        public int Order { get; set; }
        public string StepId { get; set; }
        public string ApiId { get; set; }
        public string Id { get; set; }
        public string Api { get; set; }
        public string Type { get; set; }
        public Dictionary<string, object> Inputs { get; set; }
        public Dictionary<string, object> Outputs { get; set; }
        public ConditionDefinition When { get; set; }
        public ConditionDefinition Condition { get; set; }
        public object Source { get; set; }
        public ConditionDefinition Where { get; set; }
        public Dictionary<string, object> Set { get; set; }
        public string Output { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    [Serializable]
    public sealed class BindingActionDefinition
    {
        public string BindingName { get; set; }
        public string CommandName { get; set; }
        public string Action { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }

    [Serializable]
    public sealed class ConditionDefinition
    {
        public string Operator { get; set; }
        public string Op { get; set; }
        public object Left { get; set; }
        public object Right { get; set; }
        public object Value { get; set; }
        public List<ConditionDefinition> Conditions { get; set; }
    }

    public class ApiConfigurationException : Exception
    {
        public ApiConfigurationException(string message) : base(message) { }
        public ApiConfigurationException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    public class ApiExecutionException : Exception
    {
        public ApiExecutionException(string message) : base(message) { }
        public ApiExecutionException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    public sealed class ApiPermissionException : ApiExecutionException
    {
        public ApiPermissionException(string message) : base(message) { }
    }

    public sealed class BindingAssertionException : ApiExecutionException
    {
        public BindingAssertionException(string errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode ?? string.Empty;
        }

        public string ErrorCode { get; private set; }
    }
}
