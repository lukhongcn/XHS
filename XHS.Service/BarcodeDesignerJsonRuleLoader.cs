using System;
using System.IO;
using System.Web.Script.Serialization;
using XHS.Model.BarcodeCreateRule;

namespace XHS.service
{
    public sealed class BarcodeDesignerJsonRuleLoader
    {
        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        public BarcodeRule LoadBarcodeRule(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("找不到条码界面配置文件。", filePath);
            }

            BarcodeRule rule = serializer.Deserialize<BarcodeRule>(File.ReadAllText(filePath));
            Validate(rule, filePath);
            return rule;
        }

        private static void Validate(BarcodeRule rule, string filePath)
        {
            if (rule == null || string.IsNullOrWhiteSpace(rule.RuleName))
            {
                throw new InvalidDataException("条码界面配置缺少 RuleName: " + filePath);
            }

            if (rule.Fields == null || rule.Fields.Count == 0)
            {
                throw new InvalidDataException("条码界面配置缺少 Fields: " + filePath);
            }

            foreach (BarcodeFieldRule field in rule.Fields)
            {
                if (field == null || string.IsNullOrWhiteSpace(field.Name))
                {
                    throw new InvalidDataException("Fields 中存在无效字段: " + filePath);
                }
            }
        }
    }
}
