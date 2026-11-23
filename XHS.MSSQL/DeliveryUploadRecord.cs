using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using Utility;
using XHS.IDAL;
using XHS.Model;

namespace XHS.MSSQL
{
    /// <summary>配送单上传记录 SQL Server 数据访问实现。</summary>
    public class DeliveryUploadRecord : IDeliveryUploadRecord
    {
        private const string SelectColumns = "Id,PackingId,KDQRCode,DeliveryNo,PartNo,RequestJson,ResponseJson,UploadStatus,ResponseCode,ResponseMessage,UploadUser,UploadTime,CompletedTime,ErrorMessage";

        public long InsertUploadRecord(DeliveryUploadRecordInfo info, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "insert into tb_DeliveryUploadRecord (PackingId,KDQRCode,DeliveryNo,PartNo,RequestJson,UploadStatus,UploadUser,UploadTime) values (@PackingId,@KDQRCode,@DeliveryNo,@PartNo,@RequestJson,@UploadStatus,@UploadUser,GETDATE()); select SCOPE_IDENTITY();";
            object result = SqlHelper.ExecuteScalar(transaction, CommandType.Text, sql, BuildInsertParameters(info));
            return result == null || result == DBNull.Value ? 0L : Convert.ToInt64(result);
        }

        public bool UpdateUploadResult(DeliveryUploadRecordInfo info, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "update tb_DeliveryUploadRecord set ResponseJson=@ResponseJson,UploadStatus=@UploadStatus,ResponseCode=@ResponseCode,ResponseMessage=@ResponseMessage,CompletedTime=@CompletedTime,ErrorMessage=@ErrorMessage where Id=@Id";
            SqlParameter[] parameters =
            {
                new SqlParameter("@Id", SqlDbType.BigInt) { Value = info.Id.Value },
                new SqlParameter("@ResponseJson", SqlDbType.NVarChar, -1) { Value = string.IsNullOrWhiteSpace(info.ResponseJson) ? (object)DBNull.Value : info.ResponseJson },
                new SqlParameter("@UploadStatus", SqlDbType.NVarChar, 20) { Value = string.IsNullOrWhiteSpace(info.UploadStatus) ? (object)DBNull.Value : info.UploadStatus },
                new SqlParameter("@ResponseCode", SqlDbType.Int) { Value = info.ResponseCode.HasValue ? (object)info.ResponseCode.Value : DBNull.Value },
                new SqlParameter("@ResponseMessage", SqlDbType.NVarChar, 1000) { Value = string.IsNullOrWhiteSpace(info.ResponseMessage) ? (object)DBNull.Value : info.ResponseMessage },
                new SqlParameter("@CompletedTime", SqlDbType.DateTime) { Value = info.CompletedTime.HasValue ? (object)info.CompletedTime.Value : DBNull.Value },
                new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 2000) { Value = string.IsNullOrWhiteSpace(info.ErrorMessage) ? (object)DBNull.Value : info.ErrorMessage }
            };
            return SqlHelper.ExecuteNonQuery(transaction, CommandType.Text, sql, parameters) == 1;
        }

        public List<DeliveryUploadRecordInfo> GetUploadRecords(long packingId)
        {
            string querystring = "select " + SelectColumns + " from tb_DeliveryUploadRecord where PackingId=@PackingId order by UploadTime desc, Id desc";
            SqlParameter[] pars =
            {
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = packingId }
            };
            return _getDeliveryUploadRecords(querystring, pars);
        }

        private List<DeliveryUploadRecordInfo> _getDeliveryUploadRecords(string querystring, SqlParameter[] pars)
        {
            DataSet ds = pars == null ? Data.getDataSet(querystring) : Data.getDataSet(querystring, pars);
            List<DeliveryUploadRecordInfo> result = new List<DeliveryUploadRecordInfo>();
            if (ds == null || ds.Tables.Count == 0) return result;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                result.Add(new DeliveryUploadRecordInfo
                {
                    Id = row.IsNull("Id") ? (long?)null : Convert.ToInt64(row["Id"]),
                    PackingId = row.IsNull("PackingId") ? (long?)null : Convert.ToInt64(row["PackingId"]),
                    KDQRCode = row.IsNull("KDQRCode") ? string.Empty : Convert.ToString(row["KDQRCode"]),
                    DeliveryNo = row.IsNull("DeliveryNo") ? string.Empty : Convert.ToString(row["DeliveryNo"]),
                    PartNo = row.IsNull("PartNo") ? string.Empty : Convert.ToString(row["PartNo"]),
                    RequestJson = row.IsNull("RequestJson") ? string.Empty : Convert.ToString(row["RequestJson"]),
                    ResponseJson = row.IsNull("ResponseJson") ? string.Empty : Convert.ToString(row["ResponseJson"]),
                    UploadStatus = row.IsNull("UploadStatus") ? string.Empty : Convert.ToString(row["UploadStatus"]),
                    ResponseCode = row.IsNull("ResponseCode") ? (int?)null : Convert.ToInt32(row["ResponseCode"]),
                    ResponseMessage = row.IsNull("ResponseMessage") ? string.Empty : Convert.ToString(row["ResponseMessage"]),
                    UploadUser = row.IsNull("UploadUser") ? string.Empty : Convert.ToString(row["UploadUser"]),
                    UploadTime = row.IsNull("UploadTime") ? (DateTime?)null : Convert.ToDateTime(row["UploadTime"]),
                    CompletedTime = row.IsNull("CompletedTime") ? (DateTime?)null : Convert.ToDateTime(row["CompletedTime"]),
                    ErrorMessage = row.IsNull("ErrorMessage") ? string.Empty : Convert.ToString(row["ErrorMessage"])
                });
            }
            return result;
        }

        private static SqlParameter[] BuildInsertParameters(DeliveryUploadRecordInfo info)
        {
            return new[]
            {
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = info.PackingId.HasValue ? (object)info.PackingId.Value : DBNull.Value },
                new SqlParameter("@KDQRCode", SqlDbType.NVarChar, 200) { Value = string.IsNullOrWhiteSpace(info.KDQRCode) ? (object)DBNull.Value : info.KDQRCode },
                new SqlParameter("@DeliveryNo", SqlDbType.NVarChar, 100) { Value = string.IsNullOrWhiteSpace(info.DeliveryNo) ? (object)DBNull.Value : info.DeliveryNo },
                new SqlParameter("@PartNo", SqlDbType.NVarChar, 100) { Value = string.IsNullOrWhiteSpace(info.PartNo) ? (object)DBNull.Value : info.PartNo },
                new SqlParameter("@RequestJson", SqlDbType.NVarChar, -1) { Value = string.IsNullOrWhiteSpace(info.RequestJson) ? (object)DBNull.Value : info.RequestJson },
                new SqlParameter("@UploadStatus", SqlDbType.NVarChar, 20) { Value = string.IsNullOrWhiteSpace(info.UploadStatus) ? (object)DeliveryUploadStatusInfo.Pending : info.UploadStatus },
                new SqlParameter("@UploadUser", SqlDbType.NVarChar, 100) { Value = string.IsNullOrWhiteSpace(info.UploadUser) ? (object)DBNull.Value : info.UploadUser }
            };
        }

    }
}
