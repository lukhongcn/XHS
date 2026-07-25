namespace XHS.Model
{
    /// <summary>
    /// 标签编码规则字段映射信息。
    /// </summary>
    public class LabelCodeRuleFieldInfo
    {
        public int? Id { get; set; }

        public int? RuleId { get; set; }

        public string FieldName { get; set; }

        public string DataType { get; set; }

        public string KeyCode { get; set; }

        public int? Position { get; set; }

        public int? StartPosition { get; set; }

        public int? Length { get; set; }

        public int? BatchRuleId { get; set; }

        public bool? Required { get; set; }

        public int? SortNo { get; set; }
    }
}
