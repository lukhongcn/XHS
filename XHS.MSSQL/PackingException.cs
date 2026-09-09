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
    /// 装箱异常记录 SQL Server 数据访问实现。
    /// </summary>
    public class PackingException : IPackingException
    {
        private const string PackingExceptionSelectColumns = "Id,PackingId,BoxCode,CartonNo,ExceptionCode,ExceptionMessage,TriggerQRCode,LockToken,Status,CreateUser,CreateTime,AuditUser,AuditTime,AuditRemark";
        private const string PackingExceptionOrderBy = " order by CreateTime desc, Id desc";

        private const string PackingExceptionJoinSelectColumns = "e.Id,e.PackingId,e.BoxCode,e.CartonNo,e.ExceptionCode,e.ExceptionMessage,e.TriggerQRCode,e.LockToken,e.Status,e.CreateUser,e.CreateTime,e.AuditUser,e.AuditTime,e.AuditRemark,p.KDQRCode";

        public List<PackingExceptionInfo> GetExPackingExceptions(string boxCode)
        {
            string queryString = "select " + PackingExceptionSelectColumns + " from tb_PackingException where 1=1";
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(boxCode))
            {
                queryString += " and BoxCode=@BoxCode";
                parameters.Add(new SqlParameter("@BoxCode", SqlDbType.NVarChar, 50) { Value = boxCode.Trim() });
            }

            queryString += PackingExceptionOrderBy;
            return _getPackingExceptionInfo(queryString, parameters.ToArray());
        }

        public List<PackingExceptionInfo> SearchPackingExceptions(string kdCode, int? status)
        {
            string queryString = "select " + PackingExceptionJoinSelectColumns + " from tb_PackingException e inner join tb_PackingRecord p on e.PackingId=p.Id where 1=1";
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(kdCode))
            {
                queryString += " and p.KDQRCode like @KDCode";
                parameters.Add(new SqlParameter("@KDCode", SqlDbType.NVarChar, 100) { Value = "%" + kdCode.Trim() + "%" });
            }

            if (status.HasValue)
            {
                queryString += " and e.Status=@Status";
                parameters.Add(new SqlParameter("@Status", SqlDbType.Int) { Value = status.Value });
            }

            queryString += " order by e.CreateTime desc, e.Id desc";
            return _getPackingExceptionInfo(queryString, parameters.ToArray());
        }

        public List<PackingExceptionInfo> SearchPackingExceptions(string kdCode, string partNo, DateTime? dateFrom, DateTime? dateTo, int? status)
        {
            string queryString = "select " + PackingExceptionJoinSelectColumns + " from tb_PackingException e inner join tb_PackingRecord p on e.PackingId=p.Id where 1=1";
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(kdCode))
            {
                queryString += " and p.KDQRCode like @KDCode";
                parameters.Add(new SqlParameter("@KDCode", SqlDbType.NVarChar, 100) { Value = "%" + kdCode.Trim() + "%" });
            }

            if (!string.IsNullOrWhiteSpace(partNo))
            {
                queryString += " and p.PartNo like @PartNo";
                parameters.Add(new SqlParameter("@PartNo", SqlDbType.NVarChar, 100) { Value = "%" + partNo.Trim() + "%" });
            }

            if (dateFrom.HasValue)
            {
                queryString += " and e.CreateTime>=@DateFrom";
                parameters.Add(new SqlParameter("@DateFrom", SqlDbType.DateTime) { Value = dateFrom.Value });
            }

            if (dateTo.HasValue)
            {
                queryString += " and e.CreateTime<@DateTo";
                parameters.Add(new SqlParameter("@DateTo", SqlDbType.DateTime) { Value = dateTo.Value.AddDays(1) });
            }

            if (status.HasValue)
            {
                queryString += " and e.Status=@Status";
                parameters.Add(new SqlParameter("@Status", SqlDbType.Int) { Value = status.Value });
            }

            queryString += " order by e.CreateTime desc, e.Id desc";
            return _getPackingExceptionInfo(queryString, parameters.ToArray());
        }

        public List<PackingExceptionInfo> GetPackingExceptionByLockToken(string lockToken)
        {
            string queryString = "select " + PackingExceptionSelectColumns + " from tb_PackingException where 1=1";
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(lockToken))
            {
                queryString += " and LockToken=@LockToken";
                parameters.Add(new SqlParameter("@LockToken", SqlDbType.NVarChar, 100) { Value = lockToken.Trim() });
            }

            queryString += PackingExceptionOrderBy;
            return _getPackingExceptionInfo(queryString, parameters.ToArray());
        }

        public ParamterInfo InsertPackingException(List<PackingExceptionInfo> packingExceptionInfos)
        {
            const string sql = "insert into tb_PackingException (PackingId,BoxCode,CartonNo,ExceptionCode,ExceptionMessage,TriggerQRCode,LockToken,Status,CreateUser,CreateTime,AuditUser,AuditTime,AuditRemark) values (@PackingId,@BoxCode,@CartonNo,@ExceptionCode,@ExceptionMessage,@TriggerQRCode,@LockToken,@Status,@CreateUser,@CreateTime,@AuditUser,@AuditTime,@AuditRemark)";
            ParamterInfo paramterInfo = new ParamterInfo
            {
                Sql = sql,
                Type = CommandType.Text,
                AlSQL = new ArrayList(),
                AlPAR = new ArrayList(),
                AlCOM = new ArrayList()
            };

            if (packingExceptionInfos == null)
            {
                return paramterInfo;
            }

            foreach (PackingExceptionInfo info in packingExceptionInfos)
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

        public ParamterInfo UpdatePackingException(List<PackingExceptionInfo> packingExceptionInfos)
        {
            const string sql = "update tb_PackingException set PackingId=@PackingId,BoxCode=@BoxCode,CartonNo=@CartonNo,ExceptionCode=@ExceptionCode,ExceptionMessage=@ExceptionMessage,TriggerQRCode=@TriggerQRCode,LockToken=@LockToken,Status=@Status,CreateUser=@CreateUser,CreateTime=@CreateTime,AuditUser=@AuditUser,AuditTime=@AuditTime,AuditRemark=@AuditRemark where Id=@Id";
            ParamterInfo paramterInfo = new ParamterInfo
            {
                Sql = sql,
                Type = CommandType.Text,
                AlSQL = new ArrayList(),
                AlPAR = new ArrayList(),
                AlCOM = new ArrayList()
            };

            if (packingExceptionInfos == null)
            {
                return paramterInfo;
            }

            foreach (PackingExceptionInfo info in packingExceptionInfos)
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

        /// <summary>在事务内按装箱 Id 查询待审核异常。</summary>
        public PackingExceptionInfo GetPendingException(long packingId, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "select " + PackingExceptionSelectColumns + " from tb_PackingException WITH (UPDLOCK, ROWLOCK) where PackingId=@PackingId and Status=0 order by CreateTime asc, Id asc";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = packingId }
            };

            DataSet ds = transaction == null
                ? SqlHelper.ExecuteDataset(connection, CommandType.Text, sql, parameters)
                : SqlHelper.ExecuteDataset(transaction, CommandType.Text, sql, parameters);
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            return BuildPackingExceptionFromRow(ds.Tables[0].Rows[0]);
        }

        /// <summary>只读查询装箱任务的待审核异常。</summary>
        public PackingExceptionInfo GetPendingExceptionByPackingId(long packingId, SqlConnection connection, SqlTransaction transaction)
        {
            return GetPendingException(packingId, connection, transaction);
        }

        /// <summary>事务内查询装箱任务最新的已通过重新装箱申请。</summary>
        public PackingExceptionInfo GetLatestApprovedRepackException(long packingId, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "select top 1 " + PackingExceptionSelectColumns + " from tb_PackingException where PackingId=@PackingId and ExceptionCode=@ExceptionCode and Status=1 order by AuditTime desc, Id desc";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = packingId },
                new SqlParameter("@ExceptionCode", SqlDbType.NVarChar, 50) { Value = "REPACK" }
            };
            DataSet ds = transaction == null
                ? SqlHelper.ExecuteDataset(connection, CommandType.Text, sql, parameters)
                : SqlHelper.ExecuteDataset(transaction, CommandType.Text, sql, parameters);
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            return BuildPackingExceptionFromRow(ds.Tables[0].Rows[0]);
        }

        /// <summary>在事务内插入异常记录。</summary>
        public bool InsertException(PackingExceptionInfo info, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "insert into tb_PackingException (PackingId,BoxCode,CartonNo,ExceptionCode,ExceptionMessage,TriggerQRCode,LockToken,Status,CreateUser,CreateTime) values (@PackingId,@BoxCode,@CartonNo,@ExceptionCode,@ExceptionMessage,@TriggerQRCode,@LockToken,@Status,@CreateUser,@CreateTime)";
            SqlParameter[] parameters = BuildInsertParameters(info);
            int rows = SqlHelper.ExecuteNonQuery(transaction, CommandType.Text, sql, parameters);
            return rows > 0;
        }

        /// <summary>在事务内更新异常审核信息。</summary>
        public bool AuditException(long exceptionId, string auditUser, string auditRemark, int newStatus, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "update tb_PackingException set Status=@Status,AuditUser=@AuditUser,AuditTime=GETDATE(),AuditRemark=@AuditRemark where Id=@Id and Status=0";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@Id", SqlDbType.BigInt) { Value = exceptionId },
                new SqlParameter("@Status", SqlDbType.Int) { Value = newStatus },
                new SqlParameter("@AuditUser", SqlDbType.NVarChar, 50) { Value = auditUser ?? (object)DBNull.Value },
                new SqlParameter("@AuditRemark", SqlDbType.NVarChar, 500) { Value = auditRemark ?? (object)DBNull.Value }
            };
            int rows = SqlHelper.ExecuteNonQuery(transaction, CommandType.Text, sql, parameters);
            return rows > 0;
        }

        /// <summary>将审核通过的重新装箱申请标记为已执行。</summary>
        public bool MarkRepackProcessed(long exceptionId, string executeUser, string executeRemark, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "update tb_PackingException set Status=3,AuditRemark=case when isnull(AuditRemark,'')='' then @ExecuteRemark else AuditRemark+'；'+@ExecuteRemark end where Id=@Id and ExceptionCode=@ExceptionCode and Status=1";
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@Id", SqlDbType.BigInt) { Value = exceptionId },
                new SqlParameter("@ExceptionCode", SqlDbType.NVarChar, 50) { Value = "REPACK" },
                new SqlParameter("@ExecuteRemark", SqlDbType.NVarChar, 500)
                {
                    Value = string.Format("{0}，操作人：{1}", executeRemark ?? string.Empty, executeUser ?? string.Empty)
                }
            };
            int rows = SqlHelper.ExecuteNonQuery(transaction, CommandType.Text, sql, parameters);
            return rows > 0;
        }

        private List<PackingExceptionInfo> _getPackingExceptionInfo(string querystring, SqlParameter[] pars)
        {
            DataSet dataSet;
            if (pars != null)
            {
                dataSet = Data.getDataSet(querystring, pars);
            }
            else
            {
                dataSet = Data.getDataSet(querystring);
            }

            List<PackingExceptionInfo> result = new List<PackingExceptionInfo>();
            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                return result;
            }

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                PackingExceptionInfo info = BuildPackingExceptionFromRow(row);
                if (dataSet.Tables[0].Columns.Contains("KDQRCode"))
                {
                    info.KDQRCode = row.IsNull("KDQRCode") ? null : Convert.ToString(row["KDQRCode"]);
                }

                result.Add(info);
            }

            return result;
        }

        private static PackingExceptionInfo BuildPackingExceptionFromRow(DataRow row)
        {
            return new PackingExceptionInfo
            {
                Id = row.IsNull("Id") ? (long?)null : Convert.ToInt64(row["Id"]),
                PackingId = row.IsNull("PackingId") ? (long?)null : Convert.ToInt64(row["PackingId"]),
                BoxCode = row.IsNull("BoxCode") ? null : Convert.ToString(row["BoxCode"]),
                CartonNo = row.IsNull("CartonNo") ? null : Convert.ToString(row["CartonNo"]),
                ExceptionCode = row.IsNull("ExceptionCode") ? null : Convert.ToString(row["ExceptionCode"]),
                ExceptionMessage = row.IsNull("ExceptionMessage") ? null : Convert.ToString(row["ExceptionMessage"]),
                TriggerQRCode = row.IsNull("TriggerQRCode") ? null : Convert.ToString(row["TriggerQRCode"]),
                LockToken = row.IsNull("LockToken") ? null : Convert.ToString(row["LockToken"]),
                Status = row.IsNull("Status") ? (int?)null : Convert.ToInt32(row["Status"]),
                CreateUser = row.IsNull("CreateUser") ? null : Convert.ToString(row["CreateUser"]),
                CreateTime = row.IsNull("CreateTime") ? (DateTime?)null : Convert.ToDateTime(row["CreateTime"]),
                AuditUser = row.IsNull("AuditUser") ? null : Convert.ToString(row["AuditUser"]),
                AuditTime = row.IsNull("AuditTime") ? (DateTime?)null : Convert.ToDateTime(row["AuditTime"]),
                AuditRemark = row.IsNull("AuditRemark") ? null : Convert.ToString(row["AuditRemark"])
            };
        }

        private SqlParameter[] BuildInsertOrUpdateParameters(PackingExceptionInfo info)
        {
            return new[]
            {
                new SqlParameter("@Id", SqlDbType.BigInt) { Value = info.Id ?? (object)DBNull.Value },
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = info.PackingId ?? (object)DBNull.Value },
                new SqlParameter("@BoxCode", SqlDbType.NVarChar, 50) { Value = info.BoxCode ?? (object)DBNull.Value },
                new SqlParameter("@CartonNo", SqlDbType.NVarChar, 50) { Value = info.CartonNo ?? (object)DBNull.Value },
                new SqlParameter("@ExceptionCode", SqlDbType.NVarChar, 50) { Value = info.ExceptionCode ?? (object)DBNull.Value },
                new SqlParameter("@ExceptionMessage", SqlDbType.NVarChar, 500) { Value = info.ExceptionMessage ?? (object)DBNull.Value },
                new SqlParameter("@TriggerQRCode", SqlDbType.NVarChar, 200) { Value = info.TriggerQRCode ?? (object)DBNull.Value },
                new SqlParameter("@LockToken", SqlDbType.NVarChar, 100) { Value = info.LockToken ?? (object)DBNull.Value },
                new SqlParameter("@Status", SqlDbType.Int) { Value = info.Status ?? (object)DBNull.Value },
                new SqlParameter("@CreateUser", SqlDbType.NVarChar, 50) { Value = info.CreateUser ?? (object)DBNull.Value },
                new SqlParameter("@CreateTime", SqlDbType.DateTime) { Value = info.CreateTime ?? (object)DBNull.Value },
                new SqlParameter("@AuditUser", SqlDbType.NVarChar, 50) { Value = info.AuditUser ?? (object)DBNull.Value },
                new SqlParameter("@AuditTime", SqlDbType.DateTime) { Value = info.AuditTime ?? (object)DBNull.Value },
                new SqlParameter("@AuditRemark", SqlDbType.NVarChar, 500) { Value = info.AuditRemark ?? (object)DBNull.Value }
            };
        }

        private SqlParameter[] BuildInsertParameters(PackingExceptionInfo info)
        {
            return new[]
            {
                new SqlParameter("@PackingId", SqlDbType.BigInt) { Value = info.PackingId ?? (object)DBNull.Value },
                new SqlParameter("@BoxCode", SqlDbType.NVarChar, 50) { Value = info.BoxCode ?? (object)DBNull.Value },
                new SqlParameter("@CartonNo", SqlDbType.NVarChar, 50) { Value = info.CartonNo ?? (object)DBNull.Value },
                new SqlParameter("@ExceptionCode", SqlDbType.NVarChar, 50) { Value = info.ExceptionCode ?? (object)DBNull.Value },
                new SqlParameter("@ExceptionMessage", SqlDbType.NVarChar, 500) { Value = info.ExceptionMessage ?? (object)DBNull.Value },
                new SqlParameter("@TriggerQRCode", SqlDbType.NVarChar, 200) { Value = info.TriggerQRCode ?? (object)DBNull.Value },
                new SqlParameter("@LockToken", SqlDbType.NVarChar, 100) { Value = info.LockToken ?? (object)DBNull.Value },
                new SqlParameter("@Status", SqlDbType.Int) { Value = info.Status ?? (object)DBNull.Value },
                new SqlParameter("@CreateUser", SqlDbType.NVarChar, 50) { Value = info.CreateUser ?? (object)DBNull.Value },
                new SqlParameter("@CreateTime", SqlDbType.DateTime) { Value = info.CreateTime ?? (object)DBNull.Value }
            };
        }
    }
}
