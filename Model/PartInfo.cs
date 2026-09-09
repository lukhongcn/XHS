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
    }
}
