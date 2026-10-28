using System.Collections.Generic;
using System.Data.SqlClient;
using XHS.Model;

namespace XHS.IDAL
{
    /// <summary>配送单上传记录数据访问接口。</summary>
    public interface IDeliveryUploadRecord
    {
        long InsertUploadRecord(DeliveryUploadRecordInfo info, SqlConnection connection, SqlTransaction transaction);

        bool UpdateUploadResult(DeliveryUploadRecordInfo info, SqlConnection connection, SqlTransaction transaction);

        List<DeliveryUploadRecordInfo> GetUploadRecords(long packingId);
    }
}
