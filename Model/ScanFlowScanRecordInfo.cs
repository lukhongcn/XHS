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
        public string FlowCode { get; set; }
        public string BindingTaskId { get; set; }
        public int? StepId { get; set; }
        public string StepCode { get; set; }
        public int? SeqNo { get; set; }
        public string ScanContent { get; set; }
        /// <summary>实际命中的条码规则编号。</summary>
        public int? RuleId { get; set; }
        /// <summary>实际命中的条码规则名称。</summary>
        public string RuleName { get; set; }
        /// <summary>条码类型，如光束客户标签、本厂标签或工单。</summary>
        public string BarcodeType { get; set; }
        public string Status { get; set; }
        public string ScanUser { get; set; }
        public string DeviceInfo { get; set; }
        public string ClientIp { get; set; }
        public DateTime? ScanTime { get; set; }
    }
}

