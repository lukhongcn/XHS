using System;

namespace XHS.Model
{
    /// <summary>
    /// 打印记录信息。
    /// </summary>
    public class PrintRecordInfo
    {
        public long? Id { get; set; }

        public string SupplyBatchNo { get; set; }

        public string PartNo { get; set; }

        public string DeliveryNo { get; set; }

        public string CartonNo { get; set; }

        public Guid? TaskId { get; set; }

        public string ClientId { get; set; }

        public string MachineId { get; set; }

        public string PrintType { get; set; }

        public string PdfUrl { get; set; }

        public string PdfDownLoadUrl { get; set; }

        public string PdfDownloadPath { get; set; }

        public string LocalPath { get; set; }

        public int? Status { get; set; }

        public int? PrintCount { get; set; }

        public string PrintUser { get; set; }

        public DateTime? PrintTime { get; set; }

        public string FirstPrintUser { get; set; }

        public DateTime? FirstPrintTime { get; set; }

        public string LastPrintUser { get; set; }

        public DateTime? LastPrintTime { get; set; }

        public string ReprintReason { get; set; }

        public int? ReprintReasonsId { get; set; }

        public string CreateUser { get; set; }

        public DateTime? CreateTime { get; set; }

        public string UpdateUser { get; set; }

        public DateTime? UpdateTime { get; set; }

        public DateTime? LockTime { get; set; }
    }
}
