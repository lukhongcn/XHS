using System.Collections.Generic;
using System.Data.SqlClient;
using XHS.Model;

namespace XHS.IDAL
{
    /// <summary>解除绑定扫描历史数据访问接口。</summary>
    public interface IPackingUnboundScanRecord
    {
        List<PackingUnboundScanRecordInfo> GetByPackingExceptionId(long packingExceptionId);

        /// <summary>事务内归档并移除当前装箱的全部扫描明细。</summary>
        bool ArchiveScanRecordsForUnbind(long packingId, long packingExceptionId, string unbindUser, SqlConnection connection, SqlTransaction transaction);
    }
}
