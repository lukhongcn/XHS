using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace XHS.service
{
    /// <summary>
    /// PDA 工作流 JSON 配置运行时。
    ///
    /// JSON 只负责声明 Profile、动作和算子元数据；实际算子执行必须由页面或业务类使用白名单调用，
    /// 本类不根据 JSON 反射执行任意类型或方法。
    /// </summary>
    public sealed class PdaWorkFlowJsonRuntime
    {
        private const string CatalogRelativePath = "Catalog/PdaProfileCatalog.json";
        private const string RegistryRelativePath = "Registry/PdaOperatorRegistry.json";
        private readonly string jsonRoot;

        public PdaWorkFlowJsonRuntime()
            : this(HttpRuntime.AppDomainAppPath)
        {
        }

        public PdaWorkFlowJsonRuntime(string applicationRoot)
        {
            if (string.IsNullOrWhiteSpace(applicationRoot))
            {
                throw new PdaWorkFlowConfigurationException("无法确定 Web 应用根目录。");
            }

            string root = Path.GetFullPath(applicationRoot);
            jsonRoot = Path.Combine(root, "Json", "Pda");
        }

        public PdaWorkFlowDefinition Load(string profileId)
        {
            PdaProfileCatalogDocument catalog = LoadJson<PdaProfileCatalogDocument>(CatalogRelativePath);
            if (catalog.Profiles == null || catalog.Profiles.Count == 0)
            {
                throw new PdaWorkFlowConfigurationException("PdaProfileCatalog.json 没有配置 Profile。");
            }

            PdaProfileCatalogItem catalogItem = catalog.Profiles.FirstOrDefault(item =>
                item != null && string.Equals(item.ProfileId, profileId, StringComparison.OrdinalIgnoreCase));
            if (catalogItem == null)
            {
                throw new PdaWorkFlowConfigurationException("未找到 PDA Profile：" + (profileId ?? string.Empty));
            }

            if (string.IsNullOrWhiteSpace(catalogItem.MenuId))
            {
                throw new PdaWorkFlowConfigurationException(
                    "PDA Profile 没有配置 menuId：" + catalogItem.ProfileId);
            }

            PdaProfileDefinition profile = LoadJson<PdaProfileDefinition>(catalogItem.File);
            ValidateProfileIdentity(catalogItem, profile);

            PdaOperatorRegistry registry = LoadOperatorRegistry();
            ValidateProfile(profile, registry);

            return new PdaWorkFlowDefinition
            {
                Catalog = catalogItem,
                Profile = profile,
                Operators = registry
            };
        }

        public PdaOperatorRegistry LoadOperatorRegistry()
        {
            PdaOperatorRegistryDocument document =
                LoadJson<PdaOperatorRegistryDocument>(RegistryRelativePath);
            PdaOperatorRegistry registry = new PdaOperatorRegistry();

            if (document.Operators == null || document.Operators.Count == 0)
            {
                throw new PdaWorkFlowConfigurationException("PdaOperatorRegistry.json 没有配置算子。");
            }

            foreach (PdaOperatorDefinition definition in document.Operators)
            {
                registry.Register(definition);
            }

            return registry;
        }

        public PdaActionDefinition ValidateAction(
            PdaWorkFlowDefinition definition,
            string actionCode)
        {
            if (definition == null || definition.Profile == null || definition.Operators == null)
            {
                throw new PdaWorkFlowConfigurationException("PDA 工作流配置未加载。");
            }

            PdaActionDefinition action = definition.Profile.Ui == null
                || definition.Profile.Ui.ActionDialog == null
                || definition.Profile.Ui.ActionDialog.Actions == null
                ? null
                : definition.Profile.Ui.ActionDialog.Actions.FirstOrDefault(item =>
                    item != null && string.Equals(item.Code, actionCode, StringComparison.OrdinalIgnoreCase));

            if (action == null)
            {
                throw new PdaWorkFlowActionException("当前 Profile 不允许动作：" + (actionCode ?? string.Empty));
            }

            if (string.IsNullOrWhiteSpace(action.OperatorId))
            {
                throw new PdaWorkFlowActionException("动作没有绑定算子：" + action.Code);
            }

            definition.Operators.Resolve(action.OperatorId);
            return action;
        }

        private void ValidateProfileIdentity(
            PdaProfileCatalogItem catalogItem,
            PdaProfileDefinition profile)
        {
            if (profile == null)
            {
                throw new PdaWorkFlowConfigurationException("Profile JSON 为空：" + catalogItem.File);
            }

            if (!string.Equals(catalogItem.ProfileId, profile.ProfileId, StringComparison.OrdinalIgnoreCase))
            {
                throw new PdaWorkFlowConfigurationException(
                    "Catalog 的 profileId 与 Profile JSON 不一致：" + catalogItem.ProfileId);
            }
        }

        private static void ValidateProfile(
            PdaProfileDefinition profile,
            PdaOperatorRegistry registry)
        {
            if (string.IsNullOrWhiteSpace(profile.PageTitle))
            {
                throw new PdaWorkFlowConfigurationException("Profile 没有配置 pageTitle：" + profile.ProfileId);
            }

            if (profile.Permission == null
                || string.IsNullOrWhiteSpace(profile.Permission.Read)
                || string.IsNullOrWhiteSpace(profile.Permission.Write))
            {
                throw new PdaWorkFlowConfigurationException(
                    "Profile 必须同时配置 read 和 write 权限：" + profile.ProfileId);
            }

            if (profile.Ui == null || profile.Ui.ActionDialog == null
                || profile.Ui.ActionDialog.Actions == null
                || profile.Ui.ActionDialog.Actions.Count == 0)
            {
                throw new PdaWorkFlowConfigurationException(
                    "Profile 没有配置动作：" + profile.ProfileId);
            }

            HashSet<string> actionCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (PdaActionDefinition action in profile.Ui.ActionDialog.Actions)
            {
                if (action == null || string.IsNullOrWhiteSpace(action.Code)
                    || string.IsNullOrWhiteSpace(action.Text)
                    || string.IsNullOrWhiteSpace(action.OperatorId))
                {
                    throw new PdaWorkFlowConfigurationException(
                        "Profile 存在不完整的动作定义：" + profile.ProfileId);
                }

                if (!actionCodes.Add(action.Code))
                {
                    throw new PdaWorkFlowConfigurationException(
                        "Profile 存在重复动作代码：" + action.Code);
                }

                registry.Resolve(action.OperatorId);
            }
        }

        private T LoadJson<T>(string relativePath)
        {
            string path = ResolveJsonPath(relativePath);
            if (!File.Exists(path))
            {
                throw new PdaWorkFlowConfigurationException("JSON 文件不存在：" + relativePath);
            }

            try
            {
                string json = File.ReadAllText(path);
                T result = new JavaScriptSerializer().Deserialize<T>(json);
                if (result == null)
                {
                    throw new PdaWorkFlowConfigurationException("JSON 内容为空：" + relativePath);
                }

                return result;
            }
            catch (PdaWorkFlowConfigurationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PdaWorkFlowConfigurationException(
                    "JSON 解析失败：" + relativePath,
                    ex);
            }
        }

        private string ResolveJsonPath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath) || Path.IsPathRooted(relativePath))
            {
                throw new PdaWorkFlowConfigurationException("JSON 路径无效：" + (relativePath ?? string.Empty));
            }

            string normalized = relativePath.Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);
            string root = EnsureDirectorySeparator(Path.GetFullPath(jsonRoot));
            string path = Path.GetFullPath(Path.Combine(root, normalized));

            if (!path.StartsWith(root, StringComparison.OrdinalIgnoreCase))
            {
                throw new PdaWorkFlowConfigurationException("JSON 路径超出 PDA 配置目录：" + relativePath);
            }

            return path;
        }

        private static string EnsureDirectorySeparator(string path)
        {
            return path.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
                ? path
                : path + Path.DirectorySeparatorChar;
        }
    }

    public sealed class PdaOperatorRegistry
    {
        private readonly Dictionary<string, PdaOperatorDefinition> definitions =
            new Dictionary<string, PdaOperatorDefinition>(StringComparer.OrdinalIgnoreCase);

        public void Register(PdaOperatorDefinition definition)
        {
            if (definition == null || string.IsNullOrWhiteSpace(definition.OperatorId))
            {
                throw new PdaWorkFlowConfigurationException("算子定义缺少 operatorId。");
            }

            if (string.IsNullOrWhiteSpace(definition.AssemblyNamespace)
                || string.IsNullOrWhiteSpace(definition.ClassName)
                || string.IsNullOrWhiteSpace(definition.MethodName))
            {
                throw new PdaWorkFlowConfigurationException(
                    "算子定义不完整：" + definition.OperatorId);
            }

            if (!PdaOperatorIds.IsSupported(definition.OperatorId))
            {
                throw new PdaWorkFlowConfigurationException(
                    "算子未加入服务器白名单：" + definition.OperatorId);
            }

            if (definitions.ContainsKey(definition.OperatorId))
            {
                throw new PdaWorkFlowConfigurationException(
                    "算子重复注册：" + definition.OperatorId);
            }

            definitions.Add(definition.OperatorId, definition);
        }

        public PdaOperatorDefinition Resolve(string operatorId)
        {
            PdaOperatorDefinition definition;
            if (string.IsNullOrWhiteSpace(operatorId)
                || !definitions.TryGetValue(operatorId, out definition))
            {
                throw new PdaWorkFlowActionException("算子未注册：" + (operatorId ?? string.Empty));
            }

            return definition;
        }

        public bool Contains(string operatorId)
        {
            return !string.IsNullOrWhiteSpace(operatorId) && definitions.ContainsKey(operatorId);
        }

        public IList<PdaOperatorDefinition> GetAll()
        {
            return definitions.Values.ToList();
        }
    }

    public static class PdaOperatorIds
    {
        public const string ReadyRead = "partprocess.ready.read";
        public const string SnapshotRead = "partprocess.snapshot.read";
        public const string PassExecute = "partprocess.pass.execute";
        public const string RecheckExecute = "partprocess.recheck.execute";
        public const string ScrapExecute = "partprocess.scrap.execute";
        public const string PackExecute = "partprocess.pack.execute";

        private static readonly HashSet<string> SupportedIds = new HashSet<string>(
            new[]
            {
                ReadyRead,
                SnapshotRead,
                PassExecute,
                RecheckExecute,
                ScrapExecute,
                PackExecute
            },
            StringComparer.OrdinalIgnoreCase);

        public static bool IsSupported(string operatorId)
        {
            return !string.IsNullOrWhiteSpace(operatorId) && SupportedIds.Contains(operatorId);
        }
    }

    [Serializable]
    public sealed class PdaWorkFlowDefinition
    {
        public PdaProfileCatalogItem Catalog { get; set; }
        public PdaProfileDefinition Profile { get; set; }
        public PdaOperatorRegistry Operators { get; set; }
    }

    [Serializable]
    public sealed class PdaProfileCatalogDocument
    {
        public List<PdaProfileCatalogItem> Profiles { get; set; }
    }

    [Serializable]
    public sealed class PdaProfileCatalogItem
    {
        public string ProfileId { get; set; }
        public string MenuId { get; set; }
        public string File { get; set; }
    }

    [Serializable]
    public sealed class PdaProfileDefinition
    {
        public string ProfileId { get; set; }
        public string PageTitle { get; set; }
        public PdaPermissionDefinition Permission { get; set; }
        public PdaQueryDefinition Query { get; set; }
        public Dictionary<string, string> CustomerMap { get; set; }
        public PdaUiDefinition Ui { get; set; }
    }

    [Serializable]
    public sealed class PdaQueryDefinition
    {
        public string ProcessId { get; set; }
        public string StatusId { get; set; }
    }

    [Serializable]
    public sealed class PdaPermissionDefinition
    {
        public string Read { get; set; }
        public string Write { get; set; }
    }

    [Serializable]
    public sealed class PdaUiDefinition
    {
        public PdaProcessGridDefinition ProcessGrid { get; set; }
        public PdaActionDialogDefinition ActionDialog { get; set; }
    }

    [Serializable]
    public sealed class PdaProcessGridDefinition
    {
        public string Source { get; set; }
        public List<PdaGridColumnDefinition> Columns { get; set; }
    }

    [Serializable]
    public sealed class PdaGridColumnDefinition
    {
        public string Field { get; set; }
        public string Title { get; set; }
        public bool Visible { get; set; } = true;
        public string Type { get; set; }
    }

    [Serializable]
    public sealed class PdaActionDialogDefinition
    {
        public string Title { get; set; }
        public List<PdaActionDefinition> Actions { get; set; }
    }

    [Serializable]
    public sealed class PdaActionDefinition
    {
        public string Code { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }
        public string OperatorId { get; set; }
        public string TargetStatusId { get; set; }
        public List<string> Requires { get; set; }
    }

    [Serializable]
    public sealed class PdaOperatorRegistryDocument
    {
        public List<PdaOperatorDefinition> Operators { get; set; }
    }

    [Serializable]
    public sealed class PdaOperatorDefinition
    {
        public string OperatorId { get; set; }
        public string AssemblyNamespace { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public string Signature { get; set; }
        public string SourceContract { get; set; }
    }

    public class PdaWorkFlowConfigurationException : Exception
    {
        public PdaWorkFlowConfigurationException(string message)
            : base(message)
        {
        }

        public PdaWorkFlowConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public sealed class PdaWorkFlowActionException : PdaWorkFlowConfigurationException
    {
        public PdaWorkFlowActionException(string message)
            : base(message)
        {
        }
    }
}
