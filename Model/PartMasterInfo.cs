namespace XHS.Model
{
    /// <summary>
    /// 零件主数据信息，用于查询 JHS 品号与物料名称的对应关系。
    /// </summary>
    public class PartMasterInfo
    {
        public int? PartMasterId { get; set; }

        public string JHSPartNo { get; set; }

        public string MaterialNo { get; set; }

        public string MaterialName { get; set; }

        public string ProcessType { get; set; }

        public string LabelInfo { get; set; }

        public string HasSteelStamp { get; set; }

        public string LabelFormat { get; set; }

        public int? SortOrder { get; set; }

        public string Remark { get; set; }
    }
}
