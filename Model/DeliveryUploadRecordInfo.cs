using System;

namespace XHS.Model
{
    /// <summary>配送单平台上传记录。</summary>
    public class DeliveryUploadRecordInfo
    {
        public long? Id { get; set; }
        public long? PackingId { get; set; }
        public string KDQRCode { get; set; }
        public string DeliveryNo { get; set; }
        public string PartNo { get; set; }
        public string RequestJson { get; set; }
        public string ResponseJson { get; set; }
        public string UploadStatus { get; set; }
        public int? ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string UploadUser { get; set; }
        public DateTime? UploadTime { get; set; }
        public DateTime? CompletedTime { get; set; }
        public string ErrorMessage { get; set; }
    }

    public static class DeliveryUploadStatusInfo
    {
        public const string Pending = "Pending";
        public const string Success = "Success";
        public const string Failed = "Failed";
    }
}
