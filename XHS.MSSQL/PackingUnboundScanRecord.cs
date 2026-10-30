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
    /// <summary>解除绑定扫描历史 SQL Server 数据访问实现。</summary>
    public class PackingUnboundScanRecord : IPackingUnboundScanRecord
    {
        private const string SelectColumns = "Id,PackingExceptionId,OriginalPackingId,QRCodeType,QRCode,MaterialNo,Qty,ScanUser,ScanTime,UnbindUser,UnbindTime";

        public List<PackingUnboundScanRecordInfo> GetByPackingExceptionId(long packingExceptionId)
        {
            string sql = "select " + SelectColumns + " from tb_PackingUnboundScanRecord where PackingExceptionId=@PackingExceptionId order by Id";
            SqlParameter[] parameters =
            {
                new SqlParameter("@PackingExceptionId", SqlDbType.BigInt) { Value = packingExceptionId }
            };
            DataSet ds = Data.getDataSet(sql, parameters);
            List<PackingUnboundScanRecordInfo> result = new List<PackingUnboundScanRecordInfo>();
            if (ds == null || ds.Tables.Count == 0) return result;
            foreach (DataRow row in ds.Tables[0].Rows) result.Add(ToInfo(row));
            return result;
        }

        public bool ArchiveScanRecordsForUnbind(long packingId, long packingExceptionId, string unbindUser, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = @"insert into tb_PackingUnboundScanRecord
                (PackingExceptionId,OriginalPackingId,QRCodeType,QRCode,MaterialNo,Qty,ScanUser,ScanTime,UnbindUser,UnbindTime)
                select @PackingExceptionId,PackingId,QRCodeType,QRCode,MaterialNo,Qty,ScanUser,ScanTime,@UnbindUser,GETDATE()
                from tb_PackingScanRecord
                where PackingId=@PackingId;
                delete from tb_PackingScanRecord
                where PackingId=@PackingId;";
            SqlParameter[] parameters =
            {
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = packingId },
                new SqlParameter("@PackingExceptionId", SqlDbType.BigInt) { Value = packingExceptionId },
                new SqlParameter("@UnbindUser", SqlDbType.NVarChar, 50) { Value = unbindUser ?? (object)DBNull.Value }
            };
            SqlHelper.ExecuteNonQuery(transaction, CommandType.Text, sql, parameters);
            return true;
        }

        private static PackingUnboundScanRecordInfo ToInfo(DataRow row)
        {
            return new PackingUnboundScanRecordInfo
            {
                Id = row.IsNull("Id") ? (long?)null : Convert.ToInt64(row["Id"]),
                PackingExceptionId = row.IsNull("PackingExceptionId") ? (long?)null : Convert.ToInt64(row["PackingExceptionId"]),
                OriginalPackingId = row.IsNull("OriginalPackingId") ? (long?)null : Convert.ToInt64(row["OriginalPackingId"]),
                QRCodeType = row.IsNull("QRCodeType") ? null : Convert.ToString(row["QRCodeType"]),
                QRCode = row.IsNull("QRCode") ? null : Convert.ToString(row["QRCode"]),
                MaterialNo = row.IsNull("MaterialNo") ? null : Convert.ToString(row["MaterialNo"]),
                Qty = row.IsNull("Qty") ? (int?)null : Convert.ToInt32(row["Qty"]),
                ScanUser = row.IsNull("ScanUser") ? null : Convert.ToString(row["ScanUser"]),
                ScanTime = row.IsNull("ScanTime") ? (DateTime?)null : Convert.ToDateTime(row["ScanTime"]),
                UnbindUser = row.IsNull("UnbindUser") ? null : Convert.ToString(row["UnbindUser"]),
                UnbindTime = row.IsNull("UnbindTime") ? (DateTime?)null : Convert.ToDateTime(row["UnbindTime"])
            };
        }
    }
}
