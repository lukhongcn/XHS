using System;
using System.Collections.Generic;
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

        public PartInfo ParseFactoryBarcode(
     string rawCode,
     string customerId)
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

                    PartInfo partInfo = new PartInfo();

                    // 金鸿顺标签上的零件编号
                    partInfo.JHSMaterialNo = materialNo;

                    // 年月日批号或年周批号
                    partInfo.JHSBatchNo = batchNo;

                    // 原始条码
                    partInfo.LabelInfo = rawCode;

                    // PartInfo 增加 JHSQty 属性后启用
                    partInfo.JHSQty = qty;

                    return partInfo;
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
                return new PartInfo
                {
                    ProcessOrderNo = GetFieldValue(
                        workOrderResult.Fields,
                        "WorkOrderNo"),

                    LabelInfo = rawCode
                };
            }

            // 两种条码都无法识别
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
