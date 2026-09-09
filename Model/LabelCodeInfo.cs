namespace XHS.Model
{
    /// <summary>
    /// 标签编码解析规则。
    /// </summary>
    public class LabelCodeInfo
    {
        public int? RuleId { get; set; }

        public string RuleName { get; set; }

        public string LabelType { get; set; }

        public int? CustomerId { get; set; }

        public string ParseType { get; set; }

        public string Separator { get; set; }

        public string KeySeparator { get; set; }

        public int? Version { get; set; }

        public bool? Enabled { get; set; }

        public string Remark { get; set; }
    }
}
