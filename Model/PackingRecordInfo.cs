using System;

namespace XHS.Model
{
    /// <summary>
    /// 装箱记录信息。
    /// </summary>
    public class PackingRecordInfo
    {
        public long? Id { get; set; }

        public string SupplyBatchNo { get; set; }

        public string PartNo { get; set; }

        public string CartonNo { get; set; }

        public string KDQRCode { get; set; }

        public string PackingQRCode { get; set; }

        public Guid? TaskId { get; set; }

        public int? Status { get; set; }

        public int? PlanQty { get; set; }

        public int? PackingQty { get; set; }

        public string PackingUser { get; set; }

        public DateTime? PackingTime { get; set; }

        public int? ExceptionStatus { get; set; }

        public string LockToken { get; set; }

        public DateTime? LockTime { get; set; }

        public string LockUser { get; set; }

        public string LockMachine { get; set; }

        public string ClientId { get; set; }

        public string MachineId { get; set; }

        public string CreateUser { get; set; }

        public DateTime? CreateTime { get; set; }

        public string UpdateUser { get; set; }

        public DateTime? UpdateTime { get; set; }
    }
}
