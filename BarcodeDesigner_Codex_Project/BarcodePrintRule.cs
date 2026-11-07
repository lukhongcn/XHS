using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace BarcodeDesigner
{
    public sealed class BarcodePrintRule
    {
        public string TemplateFile { get; set; }
        public List<BarcodePrintValueRule> Values { get; set; }
    }

    public sealed class BarcodePrintValueRule
    {
        public string Name { get; set; }
        public List<BarcodePrintValuePart> Parts { get; set; }
    }

    public sealed class BarcodePrintValuePart
    {
        public string Field { get; set; }
        public string Value { get; set; }
        public string DateFormat { get; set; }
        public int? Start { get; set; }
        public int? Length { get; set; }
    }

    public static class BarcodePrintRuleLoader
    {
        public static BarcodePrintRule Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("找不到标签打印规则文件。", filePath);
            }

            BarcodePrintRule rule = new JavaScriptSerializer()
                .Deserialize<BarcodePrintRule>(File.ReadAllText(filePath));
            if (rule == null || string.IsNullOrWhiteSpace(rule.TemplateFile))
            {
                throw new InvalidDataException("标签打印规则缺少 TemplateFile：" + filePath);
            }

            if (rule.Values == null || rule.Values.Count == 0)
            {
                throw new InvalidDataException("标签打印规则缺少 Values：" + filePath);
            }

            foreach (BarcodePrintValueRule valueRule in rule.Values)
            {
                if (valueRule == null
                    || string.IsNullOrWhiteSpace(valueRule.Name)
                    || valueRule.Parts == null
                    || valueRule.Parts.Count == 0)
                {
                    throw new InvalidDataException("标签打印规则中存在无效 Values 项：" + filePath);
                }

                foreach (BarcodePrintValuePart part in valueRule.Parts)
                {
                    if (part == null
                        || (string.IsNullOrWhiteSpace(part.Field)
                            && string.IsNullOrWhiteSpace(part.Value)
                            && string.IsNullOrWhiteSpace(part.DateFormat)))
                    {
                        throw new InvalidDataException("标签打印规则中存在无效字段或固定值：" + filePath);
                    }

                    if (part.Start.HasValue && part.Start.Value < 0)
                    {
                        throw new InvalidDataException("标签打印规则 Start 不能小于 0：" + filePath);
                    }

                    if (part.Length.HasValue && part.Length.Value <= 0)
                    {
                        throw new InvalidDataException("标签打印规则 Length 必须大于 0：" + filePath);
                    }
                }
            }

            return rule;
        }
    }
}
