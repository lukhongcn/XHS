namespace XHS.Model
{
    /// <summary>
    /// 标签编码规则的重复明细子规则。
    /// </summary>
    public class LabelCodeRuleSubInfo
    {
        public int? SubRuleId { get; set; }

        public int? RuleId { get; set; }

        public string SubRuleName { get; set; }

        public string ItemSeparator { get; set; }

        public string FieldSeparator { get; set; }

        public string ItemRegex { get; set; }

        public int? SortNo { get; set; }

        public bool? Enabled { get; set; }

        public string Remark { get; set; }
    }
}
