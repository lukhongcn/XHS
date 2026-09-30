using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace XHS.service
{
    public sealed class UserFieldValuesStore
    {
        private readonly string filePath;
        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        public UserFieldValuesStore()
            : this(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BarcodeDesigner",
                "UserFieldValues.json"))
        {
        }

        public UserFieldValuesStore(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("用户字段值文件路径不能为空。", nameof(filePath));
            }

            this.filePath = filePath;
        }

        public IDictionary<string, string> Load(string ruleName)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(ruleName) || !File.Exists(filePath))
            {
                return result;
            }

            try
            {
                Dictionary<string, Dictionary<string, string>> allValues =
                    serializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(
                        File.ReadAllText(filePath));
                Dictionary<string, string> ruleValues;
                if (allValues != null && allValues.TryGetValue(ruleName, out ruleValues)
                    && ruleValues != null)
                {
                    foreach (KeyValuePair<string, string> item in ruleValues)
                    {
                        result[item.Key] = item.Value ?? string.Empty;
                    }
                }
            }
            catch (InvalidOperationException)
            {
                // 文件损坏时使用 JSON 中的默认值。
            }

            return result;
        }

        public void Save(string ruleName, IDictionary<string, string> values)
        {
            if (string.IsNullOrWhiteSpace(ruleName))
            {
                throw new ArgumentException("规则名称不能为空。", nameof(ruleName));
            }

            var allValues = new Dictionary<string, Dictionary<string, string>>(
                StringComparer.OrdinalIgnoreCase);
            if (File.Exists(filePath))
            {
                try
                {
                    allValues = serializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(
                        File.ReadAllText(filePath)) ?? allValues;
                }
                catch (InvalidOperationException)
                {
                    allValues = new Dictionary<string, Dictionary<string, string>>(
                        StringComparer.OrdinalIgnoreCase);
                }
            }

            allValues[ruleName] = new Dictionary<string, string>(
                values ?? new Dictionary<string, string>(),
                StringComparer.OrdinalIgnoreCase);

            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, serializer.Serialize(allValues));
        }
    }
}
