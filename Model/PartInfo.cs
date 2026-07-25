namespace XHS.Model
{
    /// <summary>
    /// 零件信息。
    /// </summary>
    public class PartInfo
    {
        public int Seq { get; set; }

        public string MaterialNo { get; set; }

        public string MaterialName { get; set; }

        public string JHSMaterialNo { get; set; }

        public string JHSBatchNo { get; set; }

        public string PartType { get; set; }

        public string LabelInfo { get; set; }

        public bool? HasSteelStamp { get; set; }

        public string ProcessOrderNo { get; set; }

        public int JHSQty { get; set; }
    }
}
