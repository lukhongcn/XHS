using System;

namespace XHS.Model
{
    /// <summary>
    /// 零件工序查询结果。
    /// </summary>
    public class XHSPartPartProcessInfo
    {
        public int? Id { get; set; }
        public string ModuleId { get; set; }
        public string PartNo { get; set; }
        public string PartNo_Id { get; set; }
        public string ProcessId { get; set; }
        public string ProcessName { get; set; }
        public int? ProcessOrder { get; set; }
        public string StatusId { get; set; }
        public string StatusName { get; set; }
        public int? ProcessNeedMinutes { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? FactStartDate { get; set; }
        public DateTime? FactEndDate { get; set; }
    }
}
