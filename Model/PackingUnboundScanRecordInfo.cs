using System;

namespace XHS.Model
{
    /// <summary>解除绑定的零件扫描历史明细。</summary>
    public class PackingUnboundScanRecordInfo
    {
        public long? Id { get; set; }
        public long? PackingExceptionId { get; set; }
        public long? OriginalPackingId { get; set; }
        public string QRCodeType { get; set; }
        public string QRCode { get; set; }
        public string MaterialNo { get; set; }
        public int? Qty { get; set; }
        public string ScanUser { get; set; }
        public DateTime? ScanTime { get; set; }
        public string UnbindUser { get; set; }
        public DateTime? UnbindTime { get; set; }
    }
}
