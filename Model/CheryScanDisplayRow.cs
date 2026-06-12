using System;

namespace XHS.Model
{
    public class CheryScanDisplayRow
    {
        public DateTime ScanTime { get; set; }
        public string ScanCode { get; set; }
        public string SupplNo { get; set; }
        public string BaseNo { get; set; }
        public string DeliveryNo { get; set; }
        public string SxCardSeq { get; set; }
        public string MaterialNo { get; set; }
        public string MaterialName { get; set; }
        public string PackingCount { get; set; }
        public string PackageBarCode { get; set; }
        public string PackageCode { get; set; }
        public string PackageName { get; set; }
        public string CheckUserName { get; set; }
        public string UploadStatus { get; set; }
        public string ReturnCode { get; set; }
        public string ReturnMsg { get; set; }
        public string UploadContent { get; set; }
        public string ResponseContent { get; set; }
    }
}
