namespace XHS.Model
{
    /// <summary>
    /// 可配置扫描流程步骤。
    /// </summary>
    public class ScanFlowStepInfo
    {
        public int? StepId { get; set; }
        public int? FlowId { get; set; }
        public int? StepNo { get; set; }
        public string StepCode { get; set; }
        public string StepName { get; set; }
        public string ScanType { get; set; }
        public string RuleName { get; set; }
        public string CustomerId { get; set; }
        public string LabelType { get; set; }
        public bool AllowRepeat { get; set; }
        public string EndControlId { get; set; }
        public string EndCompareControlId { get; set; }
    }
}
