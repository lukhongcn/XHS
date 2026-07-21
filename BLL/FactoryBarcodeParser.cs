using System;
using System.Collections.Generic;
using System.Linq;
using Utility;
using XHS.Model;

namespace BLL
{
    /// <summary>
    /// 本厂条码解析器，通过正则规则判断条码类型并提取字段。
    /// </summary>
    public class FactoryBarcodeParser
    {
        private readonly RegexFieldParser regexParser = new RegexFieldParser();

        public PartInfo ParseFactoryBarcode(string rawCode, string customerId)
        {
            rawCode = (rawCode ?? string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Trim();

            // 优先匹配下方包装码
            List<LabelCodeRuleInfo> codeRuleInfos = new LabelCodeRule().GetLabelCodeRulesByCustomerId(customerId);
            foreach (var cri in codeRuleInfos)
            {
                FactoryBarcodeResultInfo packageResult = TryMatchRule(
                    rawCode,
                    "FACTORY_PACKAGE",
                    FactoryBarcodeType.Package,
                    cri.MatchRegex);

                if (packageResult.Success)
                {
                    List<LabelCodeRuleFieldInfo> labelCodeRuleFieldInfos =
                        new LabelCodeRuleField().GetLabelCodeRuleFieldsByRuleId(cri.RuleId.GetValueOrDefault());

                    return BuildPartInfo(cri, labelCodeRuleFieldInfos, packageResult.Fields);
                }
            }

            // 再匹配上方工单码
            FactoryBarcodeResultInfo workOrderResult = TryMatchRule(
                rawCode,
                "FACTORY_WORK_ORDER",
                FactoryBarcodeType.WorkOrder,
                @"^(?<WorkOrderNo>\d{4}-\d{11})$");

            if (workOrderResult.Success)
            {
                PartInfo partInfo = new PartInfo();
                partInfo.ProcessOrderNo = rawCode;
                return partInfo;
            }

            return null;
        }

        private FactoryBarcodeResultInfo TryMatchRule(
            string rawCode,
            string ruleCode,
            FactoryBarcodeType barcodeType,
            string matchRegex)
        {
            IDictionary<string, string> fields;
            bool success = regexParser.TryParse(
                rawCode,
                matchRegex,
                out fields);

            return new FactoryBarcodeResultInfo
            {
                Success = success,
                RuleCode = success ? ruleCode : null,
                BarcodeType = success ? barcodeType : FactoryBarcodeType.Unknown,
                Fields = fields
            };
        }

        private static PartInfo BuildPartInfo(
            LabelCodeRuleInfo rule,
            List<LabelCodeRuleFieldInfo> ruleFields,
            IDictionary<string, string> parsedValues)
        {
            var partInfo = new PartInfo();

            foreach (var field in ruleFields)
            {
                if (string.IsNullOrWhiteSpace(field.KeyCode))
                    continue;

                string value;
                if (!parsedValues.TryGetValue(field.KeyCode, out value))
                {
                    if (field.Required == true)
                        return null;
                    continue;
                }

                Type propertyType = Reflector.getPropertyType(partInfo, field.FieldName);
                object convertedValue = Convert.ChangeType(value, propertyType);
                Reflector.SetProperty(partInfo, field.FieldName, convertedValue);
            }

            return partInfo;
        }

        private static PartInfo BuildWorkOrderPartInfo(IDictionary<string, string> fields)
        {
            var partInfo = new PartInfo();

            string workOrderNo;
            if (fields.TryGetValue("WorkOrderNo", out workOrderNo))
            {
                partInfo.ProcessOrderNo = workOrderNo;
            }

            return partInfo;
        }
    }
}
