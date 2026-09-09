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
    /// 装箱记录 SQL Server 数据访问实现。
    /// </summary>
    public class PackingRecord : IPackingRecord
    {
        private const string PackingRecordSelectColumns = "Id,SupplyBatchNo,PartNo,CartonNo,KDQRCode,PackingQRCode,TaskId,Status,PlanQty,PackingQty,PackingUser,PackingTime,ExceptionStatus,LockToken,LockTime,LockUser,LockMachine,PackingStage,ClientId,MachineId,CreateUser,CreateTime,UpdateUser,UpdateTime";
        private const string PackingRecordOrderBy = " order by CreateTime desc, Id desc";

        public List<PackingRecordInfo> GetExPackingRecords(string supplyBatchNo, string partNo)
        {
            string queryString = "select " + PackingRecordSelectColumns + " from tb_PackingRecord where 1=1";

            if (!string.IsNullOrWhiteSpace(supplyBatchNo))
            {
                queryString += string.Format(" and SupplyBatchNo='{0}'", (supplyBatchNo ?? string.Empty).Replace("'", "''"));
            }

            if (!string.IsNullOrWhiteSpace(partNo))
            {
                queryString += string.Format(" and PartNo='{0}'", (partNo ?? string.Empty).Replace("'", "''"));
            }

            queryString += PackingRecordOrderBy;
            return GetPackingRecordsBySql(queryString);
        }

        /// <summary>按 KD 码、零件编号和零件码查询装箱记录，每个装箱记录只返回一行。</summary>
        public List<PackingRecordInfo> SearchPackingRecords(string kdQRCode, string partNo, string partCode)
        {
            const string sql = "select " + PackingRecordSelectColumns +
                " from tb_PackingRecord pr where (@KDQRCode='' or pr.KDQRCode like @KDLike)" +
                " and (@PartNo='' or pr.PartNo like @PartNoLike)" +
                " and (@PartCode='' or exists (select 1 from tb_PackingScanRecord psr where psr.PackingId=pr.Id and psr.QRCode like @PartCodeLike))" +
                PackingRecordOrderBy;

            string kdValue = (kdQRCode ?? string.Empty).Trim();
            string partNoValue = (partNo ?? string.Empty).Trim();
            string partCodeValue = (partCode ?? string.Empty).Trim();
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@KDQRCode", SqlDbType.NVarChar, 100) { Value = kdValue },
                new SqlParameter("@KDLike", SqlDbType.NVarChar, 202) { Value = "%" + kdValue + "%" },
                new SqlParameter("@PartNo", SqlDbType.NVarChar, 50) { Value = partNoValue },
                new SqlParameter("@PartNoLike", SqlDbType.NVarChar, 152) { Value = "%" + partNoValue + "%" },
                new SqlParameter("@PartCode", SqlDbType.NVarChar, 200) { Value = partCodeValue },
                new SqlParameter("@PartCodeLike", SqlDbType.NVarChar, 402) { Value = "%" + partCodeValue + "%" }
            };

            using (var connection = new SqlConnection(Data.WriteConnectionStr()))
            {
                connection.Open();
                DataSet dataSet = ExecuteDatasetInternal(connection, null, sql, parameters);
                List<PackingRecordInfo> result = new List<PackingRecordInfo>();
                if (dataSet == null || dataSet.Tables.Count == 0)
                {
                    return result;
                }

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    result.Add(BuildPackingRecordFromRow(row));
                }

                return result;
            }
        }

        public ParamterInfo InsertPackingRecord(List<PackingRecordInfo> packingRecordInfos)
        {
            const string sql = "insert into tb_PackingRecord (SupplyBatchNo,PartNo,CartonNo,KDQRCode,PackingQRCode,TaskId,Status,PlanQty,PackingQty,PackingUser,PackingTime,ExceptionStatus,LockToken,LockTime,LockUser,LockMachine,PackingStage,ClientId,MachineId,CreateUser,CreateTime,UpdateUser,UpdateTime) values (@SupplyBatchNo,@PartNo,@CartonNo,@KDQRCode,@PackingQRCode,@TaskId,@Status,@PlanQty,@PackingQty,@PackingUser,@PackingTime,@ExceptionStatus,@LockToken,@LockTime,@LockUser,@LockMachine,@PackingStage,@ClientId,@MachineId,@CreateUser,@CreateTime,@UpdateUser,@UpdateTime)";
            ParamterInfo paramterInfo = new ParamterInfo
            {
                Sql = sql,
                Type = CommandType.Text,
                AlSQL = new ArrayList(),
                AlPAR = new ArrayList(),
                AlCOM = new ArrayList()
            };

            if (packingRecordInfos == null)
            {
                return paramterInfo;
            }

            foreach (PackingRecordInfo info in packingRecordInfos)
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

        public ParamterInfo UpdatePackingRecord(List<PackingRecordInfo> packingRecordInfos)
        {
            const string sql = "update tb_PackingRecord set KDQRCode=@KDQRCode,PackingQRCode=@PackingQRCode,TaskId=@TaskId,Status=@Status,PlanQty=@PlanQty,PackingQty=@PackingQty,PackingUser=@PackingUser,PackingTime=@PackingTime,ExceptionStatus=@ExceptionStatus,LockToken=@LockToken,LockTime=@LockTime,LockUser=@LockUser,LockMachine=@LockMachine,PackingStage=@PackingStage,ClientId=@ClientId,MachineId=@MachineId,CreateUser=@CreateUser,CreateTime=@CreateTime,UpdateUser=@UpdateUser,UpdateTime=@UpdateTime where SupplyBatchNo=@SupplyBatchNo and PartNo=@PartNo and CartonNo=@CartonNo";
            ParamterInfo paramterInfo = new ParamterInfo
            {
                Sql = sql,
                Type = CommandType.Text,
                AlSQL = new ArrayList(),
                AlPAR = new ArrayList(),
                AlCOM = new ArrayList()
            };

            if (packingRecordInfos == null)
            {
                return paramterInfo;
            }

            foreach (PackingRecordInfo info in packingRecordInfos)
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

        private List<PackingRecordInfo> GetPackingRecordsBySql(string queryString)
        {
            DataSet dataSet = Data.getDataSet(queryString);
            List<PackingRecordInfo> result = new List<PackingRecordInfo>();
            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                return result;
            }

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                result.Add(BuildPackingRecordFromRow(row));
            }

            return result;
        }

        /// <summary>在事务内以 UPDLOCK 读取装箱记录。</summary>
        public PackingRecordInfo GetPackingRecordForUpdate(long packingId, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "select " + PackingRecordSelectColumns + " from tb_PackingRecord WITH (UPDLOCK, ROWLOCK) where Id=@PackingId";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = packingId }
            };
            DataSet ds = ExecuteDatasetInternal(connection, transaction, sql, parameters);
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            return BuildPackingRecordFromRow(ds.Tables[0].Rows[0]);
        }

        /// <summary>在事务内查询装箱记录（共享锁）。</summary>
        public PackingRecordInfo GetPackingRecordById(long packingId, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "select " + PackingRecordSelectColumns + " from tb_PackingRecord where Id=@PackingId";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = packingId }
            };
            DataSet ds = ExecuteDatasetInternal(connection, transaction, sql, parameters);
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            return BuildPackingRecordFromRow(ds.Tables[0].Rows[0]);
        }

        /// <summary>按 TaskId 查询装箱记录。</summary>
        public PackingRecordInfo GetPackingRecordByTaskId(Guid taskId, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "select " + PackingRecordSelectColumns + " from tb_PackingRecord where TaskId=@TaskId";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@TaskId", SqlDbType.UniqueIdentifier) { Value = taskId }
            };
            DataSet ds = ExecuteDatasetInternal(connection, transaction, sql, parameters);
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            return BuildPackingRecordFromRow(ds.Tables[0].Rows[0]);
        }

        /// <summary>按 KD 二维码查询装箱记录。</summary>
        public PackingRecordInfo GetPackingRecordByKdQRCode(string kdQRCode, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "select " + PackingRecordSelectColumns + " from tb_PackingRecord where KDQRCode=@KDQRCode";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@KDQRCode", SqlDbType.NVarChar, 100) { Value = kdQRCode ?? (object)DBNull.Value }
            };
            DataSet ds = ExecuteDatasetInternal(connection, transaction, sql, parameters);
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            return BuildPackingRecordFromRow(ds.Tables[0].Rows[0]);
        }

        /// <summary>事务内插入装箱记录，返回新 Id。</summary>
        public long InsertPackingRecord(PackingRecordInfo info, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "insert into tb_PackingRecord (SupplyBatchNo,PartNo,CartonNo,KDQRCode,PackingQRCode,TaskId,Status,PlanQty,PackingQty,PackingUser,PackingTime,ExceptionStatus,LockToken,LockTime,LockUser,LockMachine,PackingStage,ClientId,MachineId,CreateUser,CreateTime,UpdateUser,UpdateTime) values (@SupplyBatchNo,@PartNo,@CartonNo,@KDQRCode,@PackingQRCode,@TaskId,@Status,@PlanQty,@PackingQty,@PackingUser,@PackingTime,@ExceptionStatus,@LockToken,@LockTime,@LockUser,@LockMachine,@PackingStage,@ClientId,@MachineId,@CreateUser,@CreateTime,@UpdateUser,@UpdateTime); select SCOPE_IDENTITY();";
            SqlParameter[] parameters = BuildInsertOrUpdateParameters(info);
            object result = SqlHelper.ExecuteScalar(transaction, CommandType.Text, sql, parameters);
            return result != null && result != DBNull.Value ? Convert.ToInt64(result) : 0L;
        }

        /// <summary>事务内更新装箱扫描数量和新 Token。</summary>
        public bool UpdatePackingRecordScan(long packingId, string pageToken, int newPackingQty, string newToken, string userName, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "update tb_PackingRecord set PackingQty=@PackingQty,LockToken=@NewToken,UpdateUser=@UserName,UpdateTime=GETDATE() where Id=@PackingId and LockToken=@PageToken and ExceptionStatus=0 and Status=0";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = packingId },
                new SqlParameter("@PageToken", SqlDbType.NVarChar, 100) { Value = pageToken ?? (object)DBNull.Value },
                new SqlParameter("@NewToken", SqlDbType.NVarChar, 100) { Value = newToken },
                new SqlParameter("@PackingQty", SqlDbType.Int) { Value = newPackingQty },
                new SqlParameter("@UserName", SqlDbType.NVarChar, 50) { Value = userName ?? (object)DBNull.Value }
            };
            int rows = SqlHelper.ExecuteNonQuery(transaction, CommandType.Text, sql, parameters);
            return rows == 1;
        }

        /// <summary>事务内锁定装箱记录。</summary>
        public bool LockPackingRecord(long packingId, string pageToken, string newToken, string userName, string machineId, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "update tb_PackingRecord set ExceptionStatus=1,LockToken=@NewToken,LockTime=GETDATE(),LockUser=@UserName,LockMachine=@MachineId,UpdateUser=@UserName,UpdateTime=GETDATE() where Id=@PackingId and LockToken=@PageToken and ExceptionStatus=0 and Status=0";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = packingId },
                new SqlParameter("@PageToken", SqlDbType.NVarChar, 100) { Value = pageToken ?? (object)DBNull.Value },
                new SqlParameter("@NewToken", SqlDbType.NVarChar, 100) { Value = newToken },
                new SqlParameter("@UserName", SqlDbType.NVarChar, 50) { Value = userName ?? (object)DBNull.Value },
                new SqlParameter("@MachineId", SqlDbType.NVarChar, 50) { Value = machineId ?? (object)DBNull.Value }
            };
            int rows = SqlHelper.ExecuteNonQuery(transaction, CommandType.Text, sql, parameters);
            return rows == 1;
        }

        public ParamterInfo DeletePackingRecordsByBusinessKey(string supplyBatchNo, string partNo, string deliveryNo)
        {
            const string sql = "delete from tb_PackingRecord where exists (select 1 from tb_ShippingGoods sg where sg.SupplyBatchNo=tb_PackingRecord.SupplyBatchNo and sg.PartNo=tb_PackingRecord.PartNo and sg.CartonNo=tb_PackingRecord.CartonNo and sg.SupplyBatchNo=@SupplyBatchNo and sg.PartNo=@PartNo and sg.DeliveryNo=@DeliveryNo)";
            return BuildBusinessKeyDeleteParameterInfo(sql, supplyBatchNo, partNo, deliveryNo);
        }

        private ParamterInfo BuildBusinessKeyDeleteParameterInfo(string sql, string supplyBatchNo, string partNo, string deliveryNo)
        {
            return new ParamterInfo
            {
                Sql = sql,
                Type = CommandType.Text,
                AlSQL = new ArrayList { sql },
                AlPAR = new ArrayList
                {
                    new SqlParameter[]
                    {
                        new SqlParameter("@SupplyBatchNo", SqlDbType.NVarChar, 50) { Value = supplyBatchNo ?? string.Empty },
                        new SqlParameter("@PartNo", SqlDbType.NVarChar, 50) { Value = partNo ?? string.Empty },
                        new SqlParameter("@DeliveryNo", SqlDbType.NVarChar, 100) { Value = deliveryNo ?? string.Empty }
                    }
                },
                AlCOM = new ArrayList { CommandType.Text }
            };
        }

        /// <summary>事务内为重新装箱审核锁定记录，允许当前记录已经完成但不允许已上传。</summary>
        public bool LockPackingRecordForRepack(long packingId, string pageToken, string newToken, string userName, string machineId, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "update tb_PackingRecord set ExceptionStatus=1,LockToken=@NewToken,LockTime=GETDATE(),LockUser=@UserName,LockMachine=@MachineId,UpdateUser=@UserName,UpdateTime=GETDATE() where Id=@PackingId and LockToken=@PageToken and ExceptionStatus=0 and ISNULL(PackingStage,'')<>@UploadedStage";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = packingId },
                new SqlParameter("@PageToken", SqlDbType.NVarChar, 100) { Value = pageToken ?? (object)DBNull.Value },
                new SqlParameter("@NewToken", SqlDbType.NVarChar, 100) { Value = newToken },
                new SqlParameter("@UserName", SqlDbType.NVarChar, 50) { Value = userName ?? (object)DBNull.Value },
                new SqlParameter("@MachineId", SqlDbType.NVarChar, 50) { Value = machineId ?? (object)DBNull.Value },
                new SqlParameter("@UploadedStage", SqlDbType.NVarChar, 30) { Value = PackingStageInfo.上传完成.Status }
            };
            int rows = SqlHelper.ExecuteNonQuery(transaction, CommandType.Text, sql, parameters);
            return rows == 1;
        }

        /// <summary>事务内完成装箱。</summary>
        public bool CompletePackingRecord(long packingId, string pageToken, string newToken, string userName, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "update tb_PackingRecord set Status=1,PackingStage='Completed',PackingUser=@UserName,PackingTime=GETDATE(),LockToken=@NewToken,UpdateUser=@UserName,UpdateTime=GETDATE() where Id=@PackingId and LockToken=@PageToken and ExceptionStatus=0 and Status=0";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = packingId },
                new SqlParameter("@PageToken", SqlDbType.NVarChar, 100) { Value = pageToken ?? (object)DBNull.Value },
                new SqlParameter("@NewToken", SqlDbType.NVarChar, 100) { Value = newToken },
                new SqlParameter("@UserName", SqlDbType.NVarChar, 50) { Value = userName ?? (object)DBNull.Value }
            };
            int rows = SqlHelper.ExecuteNonQuery(transaction, CommandType.Text, sql, parameters);
            return rows == 1;
        }

        /// <summary>事务内解锁装箱记录（审核通过）。</summary>
        public bool UnlockPackingRecord(long packingId, string exceptionLockToken, string newToken, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "update tb_PackingRecord set ExceptionStatus=0,LockToken=@NewToken,LockTime=NULL,LockUser=NULL,LockMachine=NULL,UpdateUser=@NewToken,UpdateTime=GETDATE() where Id=@PackingId and ExceptionStatus=1 and LockToken=@ExceptionLockToken";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = packingId },
                new SqlParameter("@ExceptionLockToken", SqlDbType.NVarChar, 100) { Value = exceptionLockToken ?? (object)DBNull.Value },
                new SqlParameter("@NewToken", SqlDbType.NVarChar, 100) { Value = newToken }
            };
            int rows = SqlHelper.ExecuteNonQuery(transaction, CommandType.Text, sql, parameters);
            return rows == 1;
        }

        /// <summary>事务内审核通过重新装箱申请后重新开放记录。</summary>
        public bool ReopenPackingRecordForRepack(long packingId, string exceptionLockToken, string newToken, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "update tb_PackingRecord set ExceptionStatus=0,Status=0,PackingStage=@PackingStage,LockToken=@NewToken,LockTime=NULL,LockUser=NULL,LockMachine=NULL,UpdateUser=@NewToken,UpdateTime=GETDATE() where Id=@PackingId and ExceptionStatus=1 and LockToken=@ExceptionLockToken and ISNULL(PackingStage,'')<>@UploadedStage";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = packingId },
                new SqlParameter("@ExceptionLockToken", SqlDbType.NVarChar, 100) { Value = exceptionLockToken ?? (object)DBNull.Value },
                new SqlParameter("@NewToken", SqlDbType.NVarChar, 100) { Value = newToken },
                new SqlParameter("@PackingStage", SqlDbType.NVarChar, 30) { Value = PackingStageInfo.装箱中.Status },
                new SqlParameter("@UploadedStage", SqlDbType.NVarChar, 30) { Value = PackingStageInfo.上传完成.Status }
            };
            int rows = SqlHelper.ExecuteNonQuery(transaction, CommandType.Text, sql, parameters);
            return rows == 1;
        }

        /// <summary>事务内更新装箱随箱码。</summary>
        public bool UpdatePackingQRCode(long packingId, string pageToken, string packingQRCode, string newToken, string userName, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "update tb_PackingRecord set PackingQRCode=@PackingQRCode,LockToken=@NewToken,UpdateUser=@UserName,UpdateTime=GETDATE() where Id=@PackingId and LockToken=@PageToken and ExceptionStatus=0 and Status=0";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = packingId },
                new SqlParameter("@PageToken", SqlDbType.NVarChar, 100) { Value = pageToken ?? (object)DBNull.Value },
                new SqlParameter("@PackingQRCode", SqlDbType.NVarChar, 100) { Value = packingQRCode ?? (object)DBNull.Value },
                new SqlParameter("@NewToken", SqlDbType.NVarChar, 100) { Value = newToken },
                new SqlParameter("@UserName", SqlDbType.NVarChar, 50) { Value = userName ?? (object)DBNull.Value }
            };
            int rows = SqlHelper.ExecuteNonQuery(transaction, CommandType.Text, sql, parameters);
            return rows == 1;
        }

        /// <summary>事务内更新装箱阶段。</summary>
        public bool UpdatePackingStage(long packingId, string pageToken, string packingStage, string newToken, string userName, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "update tb_PackingRecord set PackingStage=@PackingStage,LockToken=@NewToken,UpdateUser=@UserName,UpdateTime=GETDATE() where Id=@PackingId and LockToken=@PageToken and ExceptionStatus=0 and Status=0";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = packingId },
                new SqlParameter("@PageToken", SqlDbType.NVarChar, 100) { Value = pageToken ?? (object)DBNull.Value },
                new SqlParameter("@PackingStage", SqlDbType.NVarChar, 30) { Value = packingStage ?? (object)DBNull.Value },
                new SqlParameter("@NewToken", SqlDbType.NVarChar, 100) { Value = newToken },
                new SqlParameter("@UserName", SqlDbType.NVarChar, 50) { Value = userName ?? (object)DBNull.Value }
            };
            int rows = SqlHelper.ExecuteNonQuery(transaction, CommandType.Text, sql, parameters);
            return rows == 1;
        }

        public bool MarkPackingRecordUploaded(long packingId, string userName, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "update tb_PackingRecord set PackingStage=@PackingStage,UpdateUser=@UserName,UpdateTime=GETDATE() where Id=@PackingId and Status=1 and ISNULL(ExceptionStatus,0)=0";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = packingId },
                new SqlParameter("@PackingStage", SqlDbType.NVarChar, 30) { Value = PackingStageInfo.上传完成.Status },
                new SqlParameter("@UserName", SqlDbType.NVarChar, 50) { Value = userName ?? (object)DBNull.Value }
            };
            int rows = SqlHelper.ExecuteNonQuery(transaction, CommandType.Text, sql, parameters);
            return rows == 1;
        }

        public bool UpdateShippingGoodsPackingStage(string supplyBatchNo, string partNo, string cartonNo, string packingStage, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "update tb_ShippingGoods set PackingStage=@PackingStage where SupplyBatchNo=@SupplyBatchNo and PartNo=@PartNo and CartonNo=@CartonNo";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@PackingStage", SqlDbType.NVarChar, 30) { Value = packingStage ?? (object)DBNull.Value },
                new SqlParameter("@SupplyBatchNo", SqlDbType.NVarChar, 100) { Value = supplyBatchNo ?? (object)DBNull.Value },
                new SqlParameter("@PartNo", SqlDbType.NVarChar, 50) { Value = partNo ?? (object)DBNull.Value },
                new SqlParameter("@CartonNo", SqlDbType.NVarChar, 50) { Value = cartonNo ?? (object)DBNull.Value }
            };
            int rows = SqlHelper.ExecuteNonQuery(transaction, CommandType.Text, sql, parameters);
            return rows > 0;
        }

        public bool MarkShippingGoodsUploaded(string supplyBatchNo, string partNo, string cartonNo, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "update tb_ShippingGoods set Status=@Status,PackingStage=@PackingStage where SupplyBatchNo=@SupplyBatchNo and PartNo=@PartNo and CartonNo=@CartonNo";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = ShippingGoodsStatusInfo.Uploaded },
                new SqlParameter("@PackingStage", SqlDbType.NVarChar, 30) { Value = PackingStageInfo.上传完成.Status },
                new SqlParameter("@SupplyBatchNo", SqlDbType.NVarChar, 100) { Value = supplyBatchNo ?? (object)DBNull.Value },
                new SqlParameter("@PartNo", SqlDbType.NVarChar, 50) { Value = partNo ?? (object)DBNull.Value },
                new SqlParameter("@CartonNo", SqlDbType.NVarChar, 50) { Value = cartonNo ?? (object)DBNull.Value }
            };
            int rows = SqlHelper.ExecuteNonQuery(transaction, CommandType.Text, sql, parameters);
            return rows > 0;
        }

        private static DataSet ExecuteDatasetInternal(SqlConnection connection, SqlTransaction transaction, string sql, SqlParameter[] parameters)
        {
            if (transaction != null)
            {
                return SqlHelper.ExecuteDataset(transaction, CommandType.Text, sql, parameters);
            }
            return SqlHelper.ExecuteDataset(connection, CommandType.Text, sql, parameters);
        }

        private PackingRecordInfo BuildPackingRecordFromRow(DataRow row)
        {
            return new PackingRecordInfo
            {
                Id = row.IsNull("Id") ? (long?)null : Convert.ToInt64(row["Id"]),
                SupplyBatchNo = row.IsNull("SupplyBatchNo") ? null : Convert.ToString(row["SupplyBatchNo"]),
                PartNo = row.IsNull("PartNo") ? null : Convert.ToString(row["PartNo"]),
                CartonNo = row.IsNull("CartonNo") ? null : Convert.ToString(row["CartonNo"]),
                KDQRCode = row.IsNull("KDQRCode") ? null : Convert.ToString(row["KDQRCode"]),
                PackingQRCode = row.IsNull("PackingQRCode") ? null : Convert.ToString(row["PackingQRCode"]),
                TaskId = row.IsNull("TaskId") ? (Guid?)null : (Guid)row["TaskId"],
                Status = row.IsNull("Status") ? (int?)null : Convert.ToInt32(row["Status"]),
                PlanQty = row.IsNull("PlanQty") ? (int?)null : Convert.ToInt32(row["PlanQty"]),
                PackingQty = row.IsNull("PackingQty") ? (int?)null : Convert.ToInt32(row["PackingQty"]),
                PackingUser = row.IsNull("PackingUser") ? null : Convert.ToString(row["PackingUser"]),
                PackingTime = row.IsNull("PackingTime") ? (DateTime?)null : Convert.ToDateTime(row["PackingTime"]),
                ExceptionStatus = row.IsNull("ExceptionStatus") ? (int?)null : Convert.ToInt32(row["ExceptionStatus"]),
                LockToken = row.IsNull("LockToken") ? null : Convert.ToString(row["LockToken"]),
                LockTime = row.IsNull("LockTime") ? (DateTime?)null : Convert.ToDateTime(row["LockTime"]),
                LockUser = row.IsNull("LockUser") ? null : Convert.ToString(row["LockUser"]),
                LockMachine = row.IsNull("LockMachine") ? null : Convert.ToString(row["LockMachine"]),
                PackingStage = row.IsNull("PackingStage") ? null : Convert.ToString(row["PackingStage"]),
                ClientId = row.IsNull("ClientId") ? null : Convert.ToString(row["ClientId"]),
                MachineId = row.IsNull("MachineId") ? null : Convert.ToString(row["MachineId"]),
                CreateUser = row.IsNull("CreateUser") ? null : Convert.ToString(row["CreateUser"]),
                CreateTime = row.IsNull("CreateTime") ? (DateTime?)null : Convert.ToDateTime(row["CreateTime"]),
                UpdateUser = row.IsNull("UpdateUser") ? null : Convert.ToString(row["UpdateUser"]),
                UpdateTime = row.IsNull("UpdateTime") ? (DateTime?)null : Convert.ToDateTime(row["UpdateTime"])
            };
        }

        private SqlParameter[] BuildInsertOrUpdateParameters(PackingRecordInfo info)
        {
            return new[]
            {
                new SqlParameter("@SupplyBatchNo", SqlDbType.NVarChar, 50) { Value = info.SupplyBatchNo ?? (object)DBNull.Value },
                new SqlParameter("@PartNo", SqlDbType.NVarChar, 50) { Value = info.PartNo ?? (object)DBNull.Value },
                new SqlParameter("@CartonNo", SqlDbType.NVarChar, 50) { Value = info.CartonNo ?? (object)DBNull.Value },
                new SqlParameter("@KDQRCode", SqlDbType.NVarChar, 100) { Value = info.KDQRCode ?? (object)DBNull.Value },
                new SqlParameter("@PackingQRCode", SqlDbType.NVarChar, 100) { Value = info.PackingQRCode ?? (object)DBNull.Value },
                new SqlParameter("@TaskId", SqlDbType.UniqueIdentifier) { Value = info.TaskId ?? (object)DBNull.Value },
                new SqlParameter("@Status", SqlDbType.Int) { Value = info.Status ?? (object)DBNull.Value },
                new SqlParameter("@PlanQty", SqlDbType.Int) { Value = info.PlanQty ?? (object)DBNull.Value },
                new SqlParameter("@PackingQty", SqlDbType.Int) { Value = info.PackingQty ?? (object)DBNull.Value },
                new SqlParameter("@PackingUser", SqlDbType.NVarChar, 50) { Value = info.PackingUser ?? (object)DBNull.Value },
                new SqlParameter("@PackingTime", SqlDbType.DateTime) { Value = info.PackingTime ?? (object)DBNull.Value },
                new SqlParameter("@ExceptionStatus", SqlDbType.Int) { Value = info.ExceptionStatus ?? (object)DBNull.Value },
                new SqlParameter("@LockToken", SqlDbType.NVarChar, 100) { Value = info.LockToken ?? (object)DBNull.Value },
                new SqlParameter("@LockTime", SqlDbType.DateTime) { Value = info.LockTime ?? (object)DBNull.Value },
                new SqlParameter("@LockUser", SqlDbType.NVarChar, 50) { Value = info.LockUser ?? (object)DBNull.Value },
                new SqlParameter("@LockMachine", SqlDbType.NVarChar, 50) { Value = info.LockMachine ?? (object)DBNull.Value },
                new SqlParameter("@PackingStage", SqlDbType.NVarChar, 30) { Value = info.PackingStage ?? (object)DBNull.Value },
                new SqlParameter("@ClientId", SqlDbType.NVarChar, 50) { Value = info.ClientId ?? (object)DBNull.Value },
                new SqlParameter("@MachineId", SqlDbType.NVarChar, 50) { Value = info.MachineId ?? (object)DBNull.Value },
                new SqlParameter("@CreateUser", SqlDbType.NVarChar, 50) { Value = info.CreateUser ?? (object)DBNull.Value },
                new SqlParameter("@CreateTime", SqlDbType.DateTime) { Value = info.CreateTime ?? (object)DBNull.Value },
                new SqlParameter("@UpdateUser", SqlDbType.NVarChar, 50) { Value = info.UpdateUser ?? (object)DBNull.Value },
                new SqlParameter("@UpdateTime", SqlDbType.DateTime) { Value = info.UpdateTime ?? (object)DBNull.Value }
            };
        }
    }
}
