using System;

namespace XHS.Model
{
    /// <summary>
    /// 装箱异常记录信息。
    /// </summary>
    public class PackingExceptionInfo
    {
        public long? Id { get; set; }

        /// <summary>关联装箱记录 Id。</summary>
        public long? PackingId { get; set; }

        /// <summary>装箱记录原有 BoxCode 字段，保留兼容。</summary>
        public string BoxCode { get; set; }

        /// <summary>箱号。</summary>
        public string CartonNo { get; set; }

        /// <summary>异常代码，如 PART001、QTY001 等。</summary>
        public string ExceptionCode { get; set; }

        /// <summary>异常描述消息。</summary>
        public string ExceptionMessage { get; set; }

        /// <summary>触发异常的二维码内容。</summary>
        public string TriggerQRCode { get; set; }

        /// <summary>锁定令牌。</summary>
        public string LockToken { get; set; }

        /// <summary>状态：0=待审核，1=已审核通过，2=已驳回，3=重新装箱已执行。</summary>
        public int? Status { get; set; }

        /// <summary>创建人。</summary>
        public string CreateUser { get; set; }

        /// <summary>创建时间。</summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>审核人。</summary>
        public string AuditUser { get; set; }

        /// <summary>审核时间。</summary>
        public DateTime? AuditTime { get; set; }

        /// <summary>审核备注。</summary>
        public string AuditRemark { get; set; }

        /// <summary>关联装箱记录的 KD 标签内容（查询时 JOIN 填充）。</summary>
        public string KDQRCode { get; set; }

        /// <summary>从 KD 标签解析出的零件编号。</summary>
        public string KDPartNo { get; set; }
    }
}
