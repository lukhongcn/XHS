using System;
using System.Collections.Generic;

namespace Model
{
    public class CheryUploadInfo
    {
        public string ScanCode { get; set; }

        public string DeliveryNo { get; set; }
        public string SxCardSeq { get; set; }

        public string MaterialNo { get; set; }
        public string MaterialName { get; set; }
        public string PackingCount { get; set; }

        public int? PackageType { get; set; }

        public string PackageBarCode { get; set; }
        public string PackageCode { get; set; }
        public string PackageName { get; set; }

        public DateTime? PackingDate { get; set; }
        public DateTime? CheckTime { get; set; }

        public string CheckUserName { get; set; }

        public List<PackingDetailInfo> PackingDetails { get; set; }
    }
}
