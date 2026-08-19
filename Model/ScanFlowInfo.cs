namespace XHS.Model
{
    /// <summary>
    /// 可配置扫描流程。
    /// </summary>
    public class ScanFlowInfo
    {
        public int? FlowId { get; set; }
        public string FlowCode { get; set; }
        public string FlowName { get; set; }
        public string CustomerId { get; set; }
        public bool? Enabled { get; set; }
    }
}
