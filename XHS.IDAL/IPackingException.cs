using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using XHS.Model;

namespace XHS.IDAL
{
    /// <summary>
    /// 装箱异常记录数据访问接口。
    /// </summary>
    public interface IPackingException
    {
        List<PackingExceptionInfo> GetExPackingExceptions(string boxCode);

        List<PackingExceptionInfo> SearchPackingExceptions(string kdCode, int? status);

        /// <summary>按多条件搜索装箱异常（支持日期范围、零件编号过滤）。</summary>
        List<PackingExceptionInfo> SearchPackingExceptions(string kdCode, string partNo, DateTime? dateFrom, DateTime? dateTo, int? status);

        List<PackingExceptionInfo> GetPackingExceptionByLockToken(string lockToken);

        ParamterInfo InsertPackingException(List<PackingExceptionInfo> packingExceptionInfos);

        ParamterInfo UpdatePackingException(List<PackingExceptionInfo> packingExceptionInfos);

        /// <summary>按装箱 Id 查询待审核异常（事务内）。</summary>
        PackingExceptionInfo GetPendingException(long packingId, SqlConnection connection, SqlTransaction transaction);

        /// <summary>只读查询装箱任务的待审核异常。</summary>
        PackingExceptionInfo GetPendingExceptionByPackingId(long packingId, SqlConnection connection, SqlTransaction transaction);

        /// <summary>事务内查询装箱任务最新的已通过重新装箱申请。</summary>
        PackingExceptionInfo GetLatestApprovedRepackException(long packingId, SqlConnection connection, SqlTransaction transaction);

        /// <summary>在事务中插入异常记录。</summary>
        bool InsertException(PackingExceptionInfo info, SqlConnection connection, SqlTransaction transaction);

        /// <summary>在事务中更新异常审核信息。</summary>
        bool AuditException(long exceptionId, string auditUser, string auditRemark, int newStatus, SqlConnection connection, SqlTransaction transaction);

        /// <summary>将审核通过的重新装箱申请标记为已执行。</summary>
        bool MarkRepackProcessed(long exceptionId, string executeUser, string executeRemark, SqlConnection connection, SqlTransaction transaction);
    }
}
