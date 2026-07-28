using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using Utility;
using XHS.IDAL;
using XHS.Model;

namespace XHS.MSSQL
{
    /// <summary>
    /// 装箱扫描记录 SQL Server 数据访问实现。
    /// </summary>
    public class PackingScanRecord : IPackingScanRecord
    {
        private const string PackingScanRecordSelectColumns = "psr.Id,psr.PackingId,psr.QRCodeType,psr.QRCode,psr.MaterialNo,psr.Qty,psr.ScanUser,psr.ScanTime";
        private const string PackingScanRecordOrderBy = " order by psr.ScanTime desc, psr.Id desc";

        public List<PackingScanRecordInfo> GetExPackingScanRecords(string supplyBatchNo, string partNo, string cartonNo)
        {
            string queryString = "select " + PackingScanRecordSelectColumns + " from tb_PackingScanRecord psr inner join tb_PackingRecord pr on psr.PackingId=pr.Id where 1=1";

            if (!string.IsNullOrWhiteSpace(supplyBatchNo))
            {
                queryString += string.Format(" and pr.SupplyBatchNo='{0}'", (supplyBatchNo ?? string.Empty).Replace("'", "''"));
            }

            if (!string.IsNullOrWhiteSpace(partNo))
            {
                queryString += string.Format(" and pr.PartNo='{0}'", (partNo ?? string.Empty).Replace("'", "''"));
            }

            if (!string.IsNullOrWhiteSpace(cartonNo))
            {
                queryString += string.Format(" and pr.CartonNo='{0}'", (cartonNo ?? string.Empty).Replace("'", "''"));
            }

            queryString += PackingScanRecordOrderBy;
            return GetPackingScanRecordsBySql(queryString);
        }

        public List<PackingScanRecordInfo> GetExPackingScanRecordsByQRCodes(string kdQRCode, string packingQRCode, string materialLabelQRCode)
        {
            if (string.IsNullOrWhiteSpace(packingQRCode))
            {
                return new List<PackingScanRecordInfo>();
            }

            string queryString = "select " + PackingScanRecordSelectColumns + " from tb_PackingScanRecord psr where exists (" +
                "select 1 from tb_PackingScanRecord p1 where p1.PackingId=psr.PackingId" +
                string.Format(" and p1.QRCodeType='{0}' and p1.QRCode='{1}'", (PackingScanRecordQRCodeTypeInfo.Packing ?? string.Empty).Replace("'", "''"), (packingQRCode ?? string.Empty).Replace("'", "''")) +
                ")";

            if (!string.IsNullOrWhiteSpace(kdQRCode))
            {
                queryString += " and exists (" +
                    "select 1 from tb_PackingScanRecord p2 where p2.PackingId=psr.PackingId" +
                    string.Format(" and p2.QRCodeType='{0}' and p2.QRCode='{1}'", (PackingScanRecordQRCodeTypeInfo.KD ?? string.Empty).Replace("'", "''"), (kdQRCode ?? string.Empty).Replace("'", "''")) +
                    ")";
            }

            if (!string.IsNullOrWhiteSpace(materialLabelQRCode))
            {
                queryString += " and exists (" +
                    "select 1 from tb_PackingScanRecord p3 where p3.PackingId=psr.PackingId" +
                    string.Format(" and p3.QRCodeType='{0}' and p3.QRCode='{1}'", (PackingScanRecordQRCodeTypeInfo.MaterialLabel ?? string.Empty).Replace("'", "''"), (materialLabelQRCode ?? string.Empty).Replace("'", "''")) +
                    ")";
            }

            queryString += PackingScanRecordOrderBy;
            return GetPackingScanRecordsBySql(queryString);
        }

        public ParamterInfo InsertPackingScanRecord(List<PackingScanRecordInfo> packingScanRecordInfos)
        {
            const string sql = "insert into tb_PackingScanRecord (PackingId,QRCodeType,QRCode,MaterialNo,Qty,ScanUser,ScanTime) values (@PackingId,@QRCodeType,@QRCode,@MaterialNo,@Qty,@ScanUser,@ScanTime)";
            ParamterInfo paramterInfo = new ParamterInfo
            {
                Sql = sql,
                Type = CommandType.Text,
                AlSQL = new ArrayList(),
                AlPAR = new ArrayList(),
                AlCOM = new ArrayList()
            };

            if (packingScanRecordInfos == null)
            {
                return paramterInfo;
            }

            foreach (PackingScanRecordInfo info in packingScanRecordInfos)
            {
                if (info == null)
                {
                    continue;
                }

                paramterInfo.AlSQL.Add(sql);
                paramterInfo.AlPAR.Add(BuildInsertOrUpdateParameters(info));
                paramterInfo.AlCOM.Add(CommandType.Text);
            }

            return paramterInfo;
        }

        public ParamterInfo UpdatePackingScanRecord(List<PackingScanRecordInfo> packingScanRecordInfos)
        {
            const string sql = "update tb_PackingScanRecord set PackingId=@PackingId,QRCodeType=@QRCodeType,QRCode=@QRCode,MaterialNo=@MaterialNo,Qty=@Qty,ScanUser=@ScanUser,ScanTime=@ScanTime where Id=@Id";
            ParamterInfo paramterInfo = new ParamterInfo
            {
                Sql = sql,
                Type = CommandType.Text,
                AlSQL = new ArrayList(),
                AlPAR = new ArrayList(),
                AlCOM = new ArrayList()
            };

            if (packingScanRecordInfos == null)
            {
                return paramterInfo;
            }

            foreach (PackingScanRecordInfo info in packingScanRecordInfos)
            {
                if (info == null)
                {
                    continue;
                }

                paramterInfo.AlSQL.Add(sql);
                paramterInfo.AlPAR.Add(BuildInsertOrUpdateParameters(info));
                paramterInfo.AlCOM.Add(CommandType.Text);
            }

            return paramterInfo;
        }

        private List<PackingScanRecordInfo> GetPackingScanRecordsBySql(string queryString)
        {
            DataSet dataSet = Data.getDataSet(queryString);
            List<PackingScanRecordInfo> result = new List<PackingScanRecordInfo>();
            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                return result;
            }

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                result.Add(new PackingScanRecordInfo
                {
                    Id = row.IsNull("Id") ? (long?)null : Convert.ToInt64(row["Id"]),
                    PackingId = row.IsNull("PackingId") ? (long?)null : Convert.ToInt64(row["PackingId"]),
                    QRCodeType = row.IsNull("QRCodeType") ? null : Convert.ToString(row["QRCodeType"]),
                    QRCode = row.IsNull("QRCode") ? null : Convert.ToString(row["QRCode"]),
                    MaterialNo = row.IsNull("MaterialNo") ? null : Convert.ToString(row["MaterialNo"]),
                    Qty = row.IsNull("Qty") ? (int?)null : Convert.ToInt32(row["Qty"]),
                    ScanUser = row.IsNull("ScanUser") ? null : Convert.ToString(row["ScanUser"]),
                    ScanTime = row.IsNull("ScanTime") ? (DateTime?)null : Convert.ToDateTime(row["ScanTime"])
                });
            }

            return result;
        }

        /// <summary>事务内插入扫描记录。</summary>
        public bool InsertScanRecord(long packingId, string qrCodeType, string qrCode, string materialNo, int qty, string scanUser, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "insert into tb_PackingScanRecord (PackingId,QRCodeType,QRCode,MaterialNo,Qty,ScanUser,ScanTime) values (@PackingId,@QRCodeType,@QRCode,@MaterialNo,@Qty,@ScanUser,GETDATE())";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = packingId },
                new SqlParameter("@QRCodeType", SqlDbType.NVarChar, 20) { Value = qrCodeType ?? (object)DBNull.Value },
                new SqlParameter("@QRCode", SqlDbType.NVarChar, 200) { Value = qrCode ?? (object)DBNull.Value },
                new SqlParameter("@MaterialNo", SqlDbType.NVarChar, 50) { Value = materialNo ?? (object)DBNull.Value },
                new SqlParameter("@Qty", SqlDbType.Int) { Value = qty },
                new SqlParameter("@ScanUser", SqlDbType.NVarChar, 50) { Value = scanUser ?? (object)DBNull.Value }
            };
            int rows = SqlHelper.ExecuteNonQuery(transaction, CommandType.Text, sql, parameters);
            return rows > 0;
        }

        /// <summary>事务内汇总已扫描物料数量。</summary>
        public int GetScannedMaterialQty(long packingId, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "select ISNULL(SUM(Qty),0) from tb_PackingScanRecord where PackingId=@PackingId and QRCodeType=@QRCodeType";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = packingId },
                new SqlParameter("@QRCodeType", SqlDbType.NVarChar, 20) { Value = PackingScanRecordQRCodeTypeInfo.MaterialLabel }
            };
            object result = SqlHelper.ExecuteScalar(transaction, CommandType.Text, sql, parameters);
            return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }

        /// <summary>事务内检查二维码是否重复。</summary>
        public bool CheckDuplicateQRCode(long packingId, string qrCode, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "select COUNT(1) from tb_PackingScanRecord where PackingId=@PackingId and QRCode=@QRCode";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = packingId },
                new SqlParameter("@QRCode", SqlDbType.NVarChar, 200) { Value = qrCode ?? (object)DBNull.Value }
            };
            object result = SqlHelper.ExecuteScalar(transaction, CommandType.Text, sql, parameters);
            int count = result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
            return count > 0;
        }

        /// <summary>事务内检查随箱码是否已被其他装箱任务使用。</summary>
        public bool CheckPackingQRCodeUsedByOther(long packingId, string packingQRCode, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "select COUNT(1) from tb_PackingScanRecord where QRCodeType=@QRCodeType and QRCode=@QRCode and PackingId<>@PackingId";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@QRCodeType", SqlDbType.NVarChar, 20) { Value = PackingScanRecordQRCodeTypeInfo.Packing },
                new SqlParameter("@QRCode", SqlDbType.NVarChar, 200) { Value = packingQRCode ?? (object)DBNull.Value },
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = packingId }
            };
            object result = SqlHelper.ExecuteScalar(transaction, CommandType.Text, sql, parameters);
            int count = result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
            return count > 0;
        }

        /// <summary>事务内查询扫描记录列表。</summary>
        public List<PackingScanRecordInfo> GetScanRecordsByPackingId(long packingId, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "select " + PackingScanRecordSelectColumns + " from tb_PackingScanRecord psr where psr.PackingId=@PackingId" + PackingScanRecordOrderBy;
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = packingId }
            };
            DataSet ds = transaction == null
                ? SqlHelper.ExecuteDataset(connection, CommandType.Text, sql, parameters)
                : SqlHelper.ExecuteDataset(transaction, CommandType.Text, sql, parameters);
            List<PackingScanRecordInfo> result = new List<PackingScanRecordInfo>();
            if (ds == null || ds.Tables.Count == 0)
            {
                return result;
            }
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                result.Add(new PackingScanRecordInfo
                {
                    Id = row.IsNull("Id") ? (long?)null : Convert.ToInt64(row["Id"]),
                    PackingId = row.IsNull("PackingId") ? (long?)null : Convert.ToInt64(row["PackingId"]),
                    QRCodeType = row.IsNull("QRCodeType") ? null : Convert.ToString(row["QRCodeType"]),
                    QRCode = row.IsNull("QRCode") ? null : Convert.ToString(row["QRCode"]),
                    MaterialNo = row.IsNull("MaterialNo") ? null : Convert.ToString(row["MaterialNo"]),
                    Qty = row.IsNull("Qty") ? (int?)null : Convert.ToInt32(row["Qty"]),
                    ScanUser = row.IsNull("ScanUser") ? null : Convert.ToString(row["ScanUser"]),
                    ScanTime = row.IsNull("ScanTime") ? (DateTime?)null : Convert.ToDateTime(row["ScanTime"])
                });
            }
            return result;
        }

        private SqlParameter[] BuildInsertOrUpdateParameters(PackingScanRecordInfo info)
        {
            return new[]
            {
                new SqlParameter("@Id", SqlDbType.BigInt) { Value = info.Id ?? (object)DBNull.Value },
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = info.PackingId ?? (object)DBNull.Value },
                new SqlParameter("@QRCodeType", SqlDbType.NVarChar, 20) { Value = info.QRCodeType ?? (object)DBNull.Value },
                new SqlParameter("@QRCode", SqlDbType.NVarChar, 200) { Value = info.QRCode ?? (object)DBNull.Value },
                new SqlParameter("@MaterialNo", SqlDbType.NVarChar, 50) { Value = info.MaterialNo ?? (object)DBNull.Value },
                new SqlParameter("@Qty", SqlDbType.Int) { Value = info.Qty ?? (object)DBNull.Value },
                new SqlParameter("@ScanUser", SqlDbType.NVarChar, 50) { Value = info.ScanUser ?? (object)DBNull.Value },
                new SqlParameter("@ScanTime", SqlDbType.DateTime) { Value = info.ScanTime ?? (object)DBNull.Value }
            };
        }
    }
}
