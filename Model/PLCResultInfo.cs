using System;

namespace XHS.Model
{
    /// <summary>
    /// PLC 结果信息。
    /// </summary>
    public class PLCResultInfo
    {
        public PLCResultInfo()
        {
            Result = 1;
            Deal = 0;
        }

        public int? Id { get; set; }
        public int? ProcessNo { get; set; }
        public int? PartProcessId { get; set; }
        public string ModuleId { get; set; }
        public string PartNo { get; set; }
        public string QRCode { get; set; }
        public DateTime? CreateDate { get; set; }
        public double? ResultValue { get; set; }
        public int Result { get; set; }
        public int Deal { get; set; }
    }
}
