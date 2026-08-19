using System;

namespace XHS.Model
{
    /// <summary>
    /// 扫描流程绑定记录。
    /// </summary>
    [Serializable]
    public class ScanFlowScanRecordInfo
    {
        public long? RecordId { get; set; }
        public int? FlowId { get; set; }
        public string ScanName { get; set; }
        public string BindingTaskId { get; set; }
        public int? StepId { get; set; }
        public string StepCode { get; set; }
        public int? SeqNo { get; set; }
        public string ScanContent { get; set; }
        public string Status { get; set; }
        public string ScanUser { get; set; }
        public string DeviceInfo { get; set; }
        public string ClientIp { get; set; }
        public DateTime? ScanTime { get; set; }
    }
}
