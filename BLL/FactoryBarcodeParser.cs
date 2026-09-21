using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        public ShippingGoodsInfo ParseShippingGoodsBarcode(string rawCode, string customerId)
        {
            return new XHS.BLL.QRCode().ParseShippingGoodsInfo(rawCode);
        }

        public List<PartInfo> ParseFactoryBarcode(
            string rawCode,
            string customerId)
        {
            List<PartInfo> parts = new List<PartInfo>();
            rawCode = (rawCode ?? string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Trim();

            if (string.IsNullOrEmpty(rawCode))
            {
                return parts;
            }

            // 1. 优先匹配下方零件包装码
            List<LabelCodeRuleInfo> codeRuleInfos =
                new LabelCodeRule()
                    .GetLabelCodeRulesByCustomerId(customerId);

            if (codeRuleInfos != null)
            {
                foreach (LabelCodeRuleInfo codeRuleInfo in codeRuleInfos)
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

                    string materialNo = GetFieldValue(
                        packageResult.Fields,
                        "MaterialNo");

                    string batchNo = GetFieldValue(
                        packageResult.Fields,
                        "BatchNo");

                    string qtyText = GetFieldValue(
                        packageResult.Fields,
                        "Qty");

                    int qty;
                    if (!int.TryParse(qtyText, out qty))
                    {
                        qty = 0;
                    }

                    PartInfo partInfo = new PartInfo
                    {
                        RuleId = codeRuleInfo.RuleId,
                        RuleName = codeRuleInfo.RuleName
                    };

                    // 金鸿顺标签上的零件编号
                    partInfo.JHSMaterialNo = materialNo;

                    // 年月日批号或年周批号
                    partInfo.JHSBatchNo = batchNo;

                    // 原始条码
                    partInfo.LabelInfo = rawCode;

                    // PartInfo 增加 JHSQty 属性后启用
                    partInfo.JHSQty = qty;

                    parts.Add(partInfo);
                }
            }

            // 2. 再匹配上方工单码
            FactoryBarcodeResultInfo workOrderResult =
                TryMatchRule(
                    rawCode,
                    "FACTORY_WORK_ORDER",
                    FactoryBarcodeType.WorkOrder,
                    @"^(?<WorkOrderNo>\d{4}-\d{11})$");

            if (workOrderResult.Success)
            {
                parts.Add(new PartInfo
                {
                    ProcessOrderNo = GetFieldValue(
                        workOrderResult.Fields,
                        "WorkOrderNo"),
                    LabelInfo = rawCode
                });
            }

            // 两种条码都无法识别
            return parts;
        }

        /// <summary>
        /// 按流程步骤配置的本厂标签规则解析条码。
        /// </summary>
        public List<PartInfo> ParseFactoryBarcode(
            string rawCode,
            string customerId,
            string labelType,
            string ruleName = null)
        {
            List<PartInfo> parts = new List<PartInfo>();
            rawCode = (rawCode ?? string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Trim();
            if (string.IsNullOrEmpty(rawCode)) return parts;

            List<LabelCodeRuleInfo> rules = new LabelCodeRule()
                .GetLabelCodeRulesByCustomerIdAndLabelType(customerId, labelType);
            if (rules == null) return parts;

            foreach (LabelCodeRuleInfo rule in rules
                .Where(x => x != null && (!x.Enabled.HasValue || x.Enabled.Value))
                .OrderByDescending(x => RuleMatches(x.RuleName, ruleName)))
            {
                if (string.IsNullOrWhiteSpace(rule.MatchRegex)) continue;

                FactoryBarcodeResultInfo result = TryMatchRule(
                    rawCode,
                    "FACTORY_PACKAGE",
                    FactoryBarcodeType.Package,
                    rule.MatchRegex);
                if (!result.Success) continue;

                string materialNo = GetFieldValue(result.Fields, "MaterialNo")
                    ?? GetFieldValue(result.Fields, "JHSMaterialNo");
                string batchNo = GetFieldValue(result.Fields, "BatchNo")
                    ?? GetFieldValue(result.Fields, "JHSBatchNo");
                string qtyText = GetFieldValue(result.Fields, "Qty")
                    ?? GetFieldValue(result.Fields, "JHSQty");
                int qty;
                if (!int.TryParse(qtyText, out qty)) qty = 0;

                parts.Add(new PartInfo
                {
                    JHSMaterialNo = materialNo,
                    JHSBatchNo = batchNo,
                    BatchNo = batchNo,
                    LabelInfo = rawCode,
                    JHSQty = qty,
                    Qty = qty,
                    // 使用统一常量标识光束本厂包装标签，避免散落硬编码字符串。
                    BarcodeType = PartBarcodeType.Factory,
                    RuleId = rule.RuleId,
                    RuleName = rule.RuleName,
                    SupplierCode = GetFieldValue(result.Fields, "SupplierCode"),
                    ProductDate = GetFieldValue(result.Fields, "ProductDate")
                });
            }

            return parts;
        }

        /// <summary>
        /// 使用与 PDA 相同的标签编码规则，校验一个完整字段值。
        /// 零件主数据中的本厂零件编号、客户零件编号属于拆分字段，不能直接调用完整条码解析方法。
        /// </summary>
        public bool IsConfiguredCodeMatch(
            string rawCode,
            IEnumerable<LabelCodeRuleInfo> rules,
            string ruleName = null)
        {
            rawCode = (rawCode ?? string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Trim();
            if (string.IsNullOrEmpty(rawCode) || rules == null)
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
                    IDictionary<string, string> fields;
                    if (regexParser.TryParse(rawCode, rule.MatchRegex, out fields))
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

        /// <summary>
        /// 按流程步骤配置的客户标签规则解析条码。
        /// </summary>
        public List<PartInfo> ParseCustomerBarcode(
            string rawCode,
            string customerId,
            string labelType,
            string ruleName)
        {
            List<PartInfo> parts = new List<PartInfo>();
            rawCode = (rawCode ?? string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Trim();
            if (string.IsNullOrEmpty(rawCode)) return parts;

            List<LabelCodeRuleInfo> rules = new LabelCodeRule()
                .GetLabelCodeRulesByCustomerIdAndLabelType(customerId, labelType);
            if (rules == null) return parts;

            foreach (LabelCodeRuleInfo rule in rules
                .Where(x => x != null && (!x.Enabled.HasValue || x.Enabled.Value))
                .OrderByDescending(x => RuleMatches(x.RuleName, ruleName)))
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

                parts.Add(new PartInfo
                {
                    MaterialNo = materialNo,
                    BatchNo = batchNo,
                    Unit = unit,
                    Qty = qty,
                    LabelInfo = rawCode,
                    // 使用统一常量标识光束客户标签，便于后续统一维护和判断。
                    BarcodeType = PartBarcodeType.Customer,
                    RuleId = rule.RuleId,
                    RuleName = rule.RuleName
                });
            }

            return parts;
        }

        public List<PartInfo> ParseWorkOrderBarcode(
            string rawCode,
            string customerId,
            string labelType,
            string ruleName)
        {
            List<PartInfo> parts = new List<PartInfo>();
            rawCode = (rawCode ?? string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Trim();
            if (string.IsNullOrEmpty(rawCode)) return parts;

            List<LabelCodeRuleInfo> rules = new LabelCodeRule()
                .GetLabelCodeRulesByCustomerIdAndLabelType(customerId, labelType);
            if (rules == null) return parts;

            foreach (LabelCodeRuleInfo rule in rules
                .Where(x => x != null && (!x.Enabled.HasValue || x.Enabled.Value))
                .OrderByDescending(x => RuleMatches(x.RuleName, ruleName)))
            {
                if (string.IsNullOrWhiteSpace(rule.MatchRegex)) continue;
                IDictionary<string, string> fields;
                if (!regexParser.TryParse(rawCode, rule.MatchRegex, out fields)) continue;

                string orderNo = GetFieldValue(fields, "ProcessOrderNo")
                    ?? GetFieldValue(fields, "WorkOrderNo");
                if (string.IsNullOrWhiteSpace(orderNo)) continue;

                parts.Add(new PartInfo
                {
                    ProcessOrderNo = orderNo,
                    // 使用统一常量标识光束工单条码，避免与流程配置字符串不一致。
                    BarcodeType = PartBarcodeType.WorkOrder,
                    LabelInfo = rawCode,
                    RuleId = rule.RuleId,
                    RuleName = rule.RuleName
                });
            }
            return parts;
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
