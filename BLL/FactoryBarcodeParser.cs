using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
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

        public PartInfo ParseFactoryBarcode(
     string rawCode,
     string customerId,
     string labelType,
     string ruleName = null)
        {
            rawCode = (rawCode ?? string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Trim();

            if (string.IsNullOrEmpty(rawCode))
            {
                return null;
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

                    return partInfo;
                }
            }

            // 当前步骤没有匹配到其配置的规则时，不能使用固定格式兜底。
            return null;
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
