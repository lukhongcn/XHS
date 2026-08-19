using System;

namespace XHS.Model
{
    /// <summary>LabelBindingPDA 每次扫描失败的审计记录。</summary>
    [Serializable]
    public class LabelBindingPdaScanFailureInfo
    {
        public long? FailureId { get; set; }
        public string BindingTaskId { get; set; }
        public string CustomerRawCode { get; set; }
        public string ScanRawCode { get; set; }
        public string ScanStage { get; set; }
        public string FailureType { get; set; }
        public string FailureReason { get; set; }
        public string ParsedBarcodeType { get; set; }
        public string ParsedPartNo { get; set; }
        public string ParsedBatchNo { get; set; }
        public decimal? ParsedQty { get; set; }
        public string OperatorName { get; set; }
        public string DeviceInfo { get; set; }
        public string ClientIp { get; set; }
        public DateTime? ScanTime { get; set; }
        public bool IsProcessed { get; set; }
    }
}
