using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using Utility;
using XHS.Model;

namespace XHS.BLL
{
    /// <summary>
    /// 本厂条码解析器，通过正则规则判断条码类型并提取字段。
    /// </summary>
    public class FactoryBarcodeParser
    {
        private readonly RegexFieldParser regexParser = new RegexFieldParser();

        /// <summary>兼容装箱业务使用的出货二维码解析入口。</summary>
        public ShippingGoodsInfo ParseShippingGoodsBarcode(string rawCode)
        {
            return new XHS.BLL.QRCode().ParseShippingGoodsInfo(rawCode);
        }

        /// <summary>按 KD 标签规则解析出货二维码（保留客户规则参数兼容入口）。</summary>
        public ShippingGoodsInfo ParseShippingGoodsBarcode(string rawCode, string customerId)
        {
            return new XHS.BLL.QRCode().ParseShippingGoodsInfo(rawCode);
        }

        public List<PartInfo> ParseFactoryBarcode(
            string rawCode,
            string customerId,
            string labelType)
        {
            return ParseFactoryBarcode(rawCode, customerId, labelType, null);
        }

        public List<PartInfo> ParseFactoryBarcode(
            string rawCode,
            string customerId,
            string labelType,
            string ruleName = null)
        {
            List<PartInfo> result = new List<PartInfo>();
            rawCode = (rawCode ?? string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Trim();

            if (string.IsNullOrEmpty(rawCode))
            {
                return result;
            }

            // 1. 优先匹配下方零件包装码
            List<LabelCodeRuleInfo> codeRuleInfos =
                new LabelCodeRule()
                    .GetLabelCodeRulesByCustomerIdAndLabelType(customerId, labelType);

            if (codeRuleInfos != null)
            {
                foreach (LabelCodeRuleInfo codeRuleInfo in codeRuleInfos
                    .Where(x => x != null && (!x.Enabled.HasValue || x.Enabled.Value))
                    .OrderByDescending(x => RuleMatches(x.RuleName, ruleName)))
                {
                    if (codeRuleInfo == null ||
                        string.IsNullOrWhiteSpace(codeRuleInfo.MatchRegex))
                    {
                        continue;
                    }

                    FactoryBarcodeResultInfo packageResult =
                        TryMatchRule(
                            rawCode,
                            "FACTORY_PACKAGE",
                            FactoryBarcodeType.Package,
                            codeRuleInfo.MatchRegex);

                    if (!packageResult.Success)
                    {
                        continue;
                    }

                    string materialNo = GetFieldValue(packageResult.Fields, "MaterialNo")
                        ?? GetFieldValue(packageResult.Fields, "JHSMaterialNo");

                    string batchNo = GetFieldValue(packageResult.Fields, "BatchNo")
                        ?? GetFieldValue(packageResult.Fields, "JHSBatchNo");

                    string qtyText = GetFieldValue(packageResult.Fields, "Qty")
                        ?? GetFieldValue(packageResult.Fields, "JHSQty");

                    string supplierCode = GetFieldValue(
                        packageResult.Fields,
                        "SupplierCode");

                    string productDate = GetFieldValue(
                        packageResult.Fields,
                        "ProductDate");

                    int qty;
                    if (!int.TryParse(qtyText, out qty))
                    {
                        qty = 0;
                    }

                    PartInfo partInfo = new PartInfo();

                    // 金鸿顺标签上的零件编号
                    partInfo.JHSMaterialNo = materialNo;

                    // 年月日批号或年周批号
                    partInfo.JHSBatchNo = batchNo;
                    partInfo.BatchNo = batchNo;

                    // 原始条码
                    partInfo.LabelInfo = rawCode;

                    // PartInfo 增加 JHSQty 属性后启用
                    partInfo.JHSQty = qty;
                    partInfo.Qty = qty;
                    partInfo.BarcodeType = "FACTORY";

                    partInfo.SupplierCode = supplierCode;
                    partInfo.ProductDate = productDate;

                    result.Add(partInfo);
                    return result;
                }
            }

            // 当前步骤没有匹配到其配置的规则时，不能使用固定格式兜底。
            return result;
        }

        public PartInfo ParseWorkOrderBarcode(string rawCode, string customerId, string labelType, string ruleName)
        {
            rawCode = (rawCode ?? string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();
            if (string.IsNullOrEmpty(rawCode)) return null;

            List<LabelCodeRuleInfo> rules = new LabelCodeRule()
                .GetLabelCodeRulesByCustomerIdAndLabelType(customerId, labelType);
            if (rules == null) return null;

            foreach (LabelCodeRuleInfo rule in rules.Where(x => x != null
                && (!x.Enabled.HasValue || x.Enabled.Value))
                .OrderByDescending(x => RuleMatches(x.RuleName, ruleName)))
            {
                if (string.IsNullOrWhiteSpace(rule.MatchRegex)) continue;
                IDictionary<string, string> fields;
                if (!regexParser.TryParse(rawCode, rule.MatchRegex, out fields)) continue;

                string orderNo = GetFieldValue(fields, "ProcessOrderNo")
                    ?? GetFieldValue(fields, "WorkOrderNo");
                if (string.IsNullOrWhiteSpace(orderNo)) continue;

                return new PartInfo
                {
                    ProcessOrderNo = orderNo,
                    BarcodeType = "WORKORDER",
                    LabelInfo = rawCode
                };
            }

            return null;
        }

        /// <summary>
        /// 解析前六位为 JHS 零件号的客户条码。
        /// </summary>
        public PartInfo ParseJHSPartnoBarcode(
            string rawCode,
            string customerId,
            string labelType,
            string ruleName)
        {
            rawCode = (rawCode ?? string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Trim();
            if (string.IsNullOrEmpty(rawCode)) return null;

            List<LabelCodeRuleInfo> rules = new LabelCodeRule()
                .GetLabelCodeRulesByCustomerIdAndLabelType(customerId, labelType);
            if (rules == null) return null;

            foreach (LabelCodeRuleInfo rule in rules
                .Where(x => x != null && (!x.Enabled.HasValue || x.Enabled.Value))
                .OrderByDescending(x => RuleMatches(x.RuleName, ruleName)))
            {
                if (string.IsNullOrWhiteSpace(rule.MatchRegex)) continue;

                IDictionary<string, string> fields;
                if (!regexParser.TryParse(rawCode, rule.MatchRegex, out fields)) continue;

                string jhsPartNo = GetFieldValue(fields, "JHSMaterialNo");
                string batchNo = GetFieldValue(fields, "JHSBatchNo")
                    ?? GetFieldValue(fields, "BatchNo")
                    ?? GetFieldValue(fields, "CustomerBatchNo");
                string qtyText = GetFieldValue(fields, "JHSQty")
                    ?? GetFieldValue(fields, "Qty")
                    ?? GetFieldValue(fields, "CustomerQty");
                int qty;
                if (string.IsNullOrWhiteSpace(jhsPartNo)
                    || !int.TryParse(qtyText, NumberStyles.Any, CultureInfo.InvariantCulture, out qty)
                    || qty <= 0)
                {
                    continue;
                }

                return new PartInfo
                {
                    JHSMaterialNo = jhsPartNo,
                    JHSBatchNo = batchNo,
                    BatchNo = batchNo,
                    JHSQty = qty,
                    Qty = qty,
                    LabelInfo = rawCode,
                    BarcodeType = "CUSTOMER",
                    RuleId = rule.RuleId,
                    RuleName = rule.RuleName
                };
            }

            return null;
        }

        /// <summary>
        /// 按主规则和重复明细子规则解析客户标签。
        /// 子规则的命名分组可以重复出现，重复分组按 Captures 下标组成多个零件。
        /// </summary>
        public List<PartInfo> ParseCustomerBarcodes(
            string rawCode,
            string customerId,
            string labelType,
            string ruleName)
        {
            rawCode = (rawCode ?? string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Trim();
            List<PartInfo> result = new List<PartInfo>();
            if (string.IsNullOrEmpty(rawCode)) return result;

            List<LabelCodeRuleInfo> rules = new LabelCodeRule()
                .GetLabelCodeRulesByCustomerIdAndLabelType(customerId, labelType);
            if (rules == null) return result;

            LabelCodeRuleSub subRuleService = new LabelCodeRuleSub();
            foreach (LabelCodeRuleInfo rule in rules
                .Where(x => x != null && (!x.Enabled.HasValue || x.Enabled.Value))
                .OrderByDescending(x => string.Equals(x.RuleName, ruleName, StringComparison.Ordinal)))
            {
                if (string.IsNullOrWhiteSpace(rule.MatchRegex)) continue;

                IDictionary<string, string> parentFields;
                if (!regexParser.TryParse(rawCode, rule.MatchRegex, out parentFields)) continue;

                List<LabelCodeRuleSubInfo> subRules = rule.RuleId.HasValue
                    ? subRuleService.GetLabelCodeRuleSubsByRuleId(rule.RuleId.Value)
                    : new List<LabelCodeRuleSubInfo>();
                subRules = subRules == null
                    ? new List<LabelCodeRuleSubInfo>()
                    : subRules.Where(x => x != null && (!x.Enabled.HasValue || x.Enabled.Value))
                        .OrderBy(x => x.SortNo ?? 0)
                        .ThenBy(x => x.SubRuleId ?? 0)
                        .ToList();

                if (subRules.Count == 0) continue;

                string itemText = GetFieldValue(parentFields, "ItemList");
                if (string.IsNullOrWhiteSpace(itemText))
                {
                    itemText = rawCode;
                }

                foreach (LabelCodeRuleSubInfo subRule in subRules)
                {
                    List<PartInfo> parsed = ParseSubRuleItems(itemText, rawCode, rule, subRule);
                    if (parsed.Count > 0) return parsed;
                }
            }

            return result;
        }

        private static List<PartInfo> ParseSubRuleItems(
            string itemText,
            string rawCode,
            LabelCodeRuleInfo rule,
            LabelCodeRuleSubInfo subRule)
        {
            List<PartInfo> result = new List<PartInfo>();
            if (string.IsNullOrWhiteSpace(itemText) || string.IsNullOrWhiteSpace(subRule.ItemRegex))
                return result;

            string[] itemTexts = string.IsNullOrWhiteSpace(subRule.ItemSeparator)
                ? new[] { itemText }
                : itemText.Split(new[] { subRule.ItemSeparator }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string currentItemText in itemTexts)
            {
                Match match;
                try
                {
                    match = Regex.Match(currentItemText, subRule.ItemRegex, RegexOptions.CultureInvariant);
                }
                catch (ArgumentException)
                {
                    return new List<PartInfo>();
                }

                if (!match.Success || match.Length != currentItemText.Length) continue;

                Group materialGroup = GetGroup(match, "MaterialNo", "CustomerMaterialNo");
                Group qtyGroup = GetGroup(match, "Qty", "CustomerQty");
                Group unitGroup = GetGroup(match, "Unit", "CustomerUnit");
                Group batchGroup = GetGroup(match, "BatchNo", "CustomerBatchNo");
                if (materialGroup == null || qtyGroup == null) continue;

                List<Capture> materialCaptures = materialGroup.Captures.Cast<Capture>().ToList();
                List<Capture> qtyCaptures = qtyGroup.Captures.Cast<Capture>().ToList();
                List<Capture> unitCaptures = unitGroup == null
                    ? new List<Capture>()
                    : unitGroup.Captures.Cast<Capture>().ToList();
                List<Capture> batchCaptures = batchGroup == null
                    ? new List<Capture>()
                    : batchGroup.Captures.Cast<Capture>().ToList();
                int count = materialCaptures.Count;
                if (count == 0 || qtyCaptures.Count != count) continue;

                for (int index = 0; index < count; index++)
                {
                    decimal qty;
                    if (!decimal.TryParse(qtyCaptures[index].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out qty)
                        || qty <= 0)
                    {
                        return new List<PartInfo>();
                    }

                    result.Add(new PartInfo
                    {
                        MaterialNo = materialCaptures[index].Value,
                        BatchNo = batchCaptures.Count > index ? batchCaptures[index].Value : string.Empty,
                        Unit = unitCaptures.Count > index ? unitCaptures[index].Value : string.Empty,
                        Qty = qty,
                        LabelInfo = rawCode,
                        BarcodeType = "CUSTOMER",
                        RuleId = rule.RuleId,
                        RuleName = rule.RuleName
                    });
                }
                return result;
            }

            return result;
        }

        private static Group GetGroup(Match match, params string[] names)
        {
            foreach (string name in names)
            {
                Group group = match.Groups[name];
                if (group != null && group.Success) return group;
            }
            return null;
        }

        /// <summary>
        /// 按流程步骤配置的规则解析客户标签，结果统一返回 PartInfo。
        /// </summary>
        public PartInfo ParseCustomerBarcode(string rawCode, string customerId, string labelType, string ruleName)
        {
            rawCode = (rawCode ?? string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();
            if (string.IsNullOrEmpty(rawCode)) return null;

            List<LabelCodeRuleInfo> rules = new LabelCodeRule()
                .GetLabelCodeRulesByCustomerIdAndLabelType(customerId, labelType);
            if (rules == null) return null;

            foreach (LabelCodeRuleInfo rule in rules
                .Where(x => x != null && (!x.Enabled.HasValue || x.Enabled.Value))
                .OrderByDescending(x => string.Equals(x.RuleName, ruleName, StringComparison.Ordinal)))
            {
                if (string.IsNullOrWhiteSpace(rule.MatchRegex)) continue;

                IDictionary<string, string> fields;
                if (!regexParser.TryParse(rawCode, rule.MatchRegex, out fields)) continue;

                string materialNo = GetFieldValue(fields, "MaterialNo")
                    ?? GetFieldValue(fields, "CustomerMaterialNo");
                string batchNo = GetFieldValue(fields, "BatchNo")
                    ?? GetFieldValue(fields, "CustomerBatchNo");
                string unit = GetFieldValue(fields, "Unit")
                    ?? GetFieldValue(fields, "CustomerUnit");
                string qtyText = GetFieldValue(fields, "Qty")
                    ?? GetFieldValue(fields, "CustomerQty");
                decimal qty;
                if (string.IsNullOrWhiteSpace(materialNo)
                    || !decimal.TryParse(qtyText, NumberStyles.Any, CultureInfo.InvariantCulture, out qty)
                    || qty <= 0)
                {
                    continue;
                }

                return new PartInfo
                {
                    MaterialNo = materialNo,
                    BatchNo = batchNo,
                    Unit = unit,
                    Qty = qty,
                    LabelInfo = rawCode,
                    BarcodeType = "CUSTOMER"
                };
            }

            return null;
        }

        /// <summary>
        /// 使用完整条码规则中的命名分组，校验已经从 Excel 拆分出来的字段值。
        /// </summary>
        public bool IsConfiguredFieldMatch(
            string rawValue,
            IEnumerable<LabelCodeRuleInfo> rules,
            string fieldName,
            string ruleName = null)
        {
            rawValue = (rawValue ?? string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Trim();
            if (string.IsNullOrEmpty(rawValue)
                || string.IsNullOrWhiteSpace(fieldName)
                || rules == null)
            {
                return false;
            }

            foreach (LabelCodeRuleInfo rule in rules
                .Where(x => x != null && (!x.Enabled.HasValue || x.Enabled.Value))
                .OrderByDescending(x => RuleMatches(x.RuleName, ruleName)))
            {
                if (string.IsNullOrWhiteSpace(rule.MatchRegex))
                {
                    continue;
                }

                try
                {
                    if (regexParser.TryMatchNamedField(rawValue, rule.MatchRegex, fieldName))
                    {
                        return true;
                    }
                }
                catch (ArgumentException)
                {
                    // 无效规则不应导致上传页面直接抛出异常，继续尝试其他启用规则。
                }
            }

            return false;
        }

        private string GetFieldValue(
            IDictionary<string, string> fields,
            string fieldName)
        {
            if (fields == null ||
                string.IsNullOrEmpty(fieldName))
            {
                return null;
            }

            string value;

            if (fields.TryGetValue(fieldName, out value))
            {
                return value;
            }

            return null;
        }

        private static bool RuleMatches(string configuredRuleName, string stepRuleName)
        {
            return string.IsNullOrWhiteSpace(stepRuleName)
                || string.Equals(
                    (configuredRuleName ?? string.Empty).Trim(),
                    stepRuleName.Trim(),
                    StringComparison.OrdinalIgnoreCase);
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
