namespace XHS.Model
{
    /// <summary>
    /// 零件信息。
    /// </summary>
    public class PartInfo : PartMasterInfo
    {
        public int Seq { get; set; }

        public string JHSMaterialNo { get; set; }

        public string JHSBatchNo { get; set; }

        public string PartType { get; set; }

        public string ProcessOrderNo { get; set; }

        public string SupplierCode { get; set; }

        public string ProductDate { get; set; }

        public int JHSQty { get; set; }

        // 通用条码解析字段，保留历史字段以兼容现有业务。
        public string BatchNo { get; set; }
        public string Unit { get; set; }
        public decimal Qty { get; set; }
        public string BarcodeType { get; set; }

        // 实际命中的条码规则，便于页面展示和日志追踪。
        public int? RuleId { get; set; }
        public string RuleName { get; set; }
    }

    /// <summary>
    /// 光束标签绑定流程中 PartInfo.BarcodeType 的统一取值，避免在解析器和页面中直接写条码类型字符串。
    /// </summary>
    public static class PartBarcodeType
    {
        /// <summary>光束客户标签条码。</summary>
        public const string Customer = "CUSTOMER";

        /// <summary>光束本厂包装标签条码。</summary>
        public const string Factory = "FACTORY";

        /// <summary>光束工单条码。</summary>
        public const string WorkOrder = "WORKORDER";
    }
}
