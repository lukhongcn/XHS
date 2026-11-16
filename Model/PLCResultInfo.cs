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
        }

        public int? Id { get; set; }
        public int? ProcessNo { get; set; }
        public int? PartProcessId { get; set; }
        public DateTime? CreateDate { get; set; }
        public double? ResultValue { get; set; }
        public int Result { get; set; }
    }
}
