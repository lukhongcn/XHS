using System;

namespace XHS.Model
{
    /// <summary>
    /// 装箱扫描记录信息。
    /// </summary>
    public class PackingScanRecordInfo
    {
        public long? Id { get; set; }

        public long? PackingId { get; set; }

        public string QRCodeType { get; set; }

        public string QRCode { get; set; }

        public string MaterialNo { get; set; }

        public int? Qty { get; set; }

        public string ScanUser { get; set; }

        public DateTime? ScanTime { get; set; }
    }
}
