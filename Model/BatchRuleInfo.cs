namespace XHS.Model
{
    /// <summary>
    /// 批次规则信息。
    /// </summary>
    public class BatchRuleInfo
    {
        public int? BatchRuleId { get; set; }

        public string RuleName { get; set; }

        public string Format { get; set; }

        public int? Length { get; set; }

        public string Remark { get; set; }
    }
}
