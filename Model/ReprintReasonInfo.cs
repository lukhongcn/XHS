namespace XHS.Model
{
    /// <summary>
    /// 补打原因字典信息。
    /// </summary>
    public class ReprintReasonInfo
    {
        public int? Id { get; set; }

        public string ReasonName { get; set; }

        public int? SortNo { get; set; }

        public bool? IsEnable { get; set; }
    }
}
