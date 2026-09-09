using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Utility;
using XHS.IDAL;
using XHS.Model;

namespace XHS.MSSQL
{
    /// <summary>
    /// 批次规则 SQL Server 数据访问实现。
    /// </summary>
    public class BatchRule : IBatchRule
    {
        private const string SelectColumns = "BatchRuleId,RuleName,Format,Length,Remark";

        public List<BatchRuleInfo> GetBatchRules()
        {
            DataSet dataSet = Data.getDataSet("select " + SelectColumns + " from tb_BatchRule order by BatchRuleId asc");
            return BuildBatchRules(dataSet);
        }

        public BatchRuleInfo GetBatchRule(int batchRuleId)
        {
            List<BatchRuleInfo> result = BuildBatchRules(Data.getDataSet(
                "select " + SelectColumns + " from tb_BatchRule where BatchRuleId=" + batchRuleId));
            return result.Count == 0 ? null : result[0];
        }

        public ParamterInfo InsertBatchRules(List<BatchRuleInfo> batchRuleInfos)
        {
            const string sql = "insert into tb_BatchRule (RuleName,Format,Length,Remark) values (@RuleName,@Format,@Length,@Remark)";
            return BuildParamterInfo(batchRuleInfos, sql, BuildInsertParameters);
        }

        public ParamterInfo UpdateBatchRules(List<BatchRuleInfo> batchRuleInfos)
        {
            const string sql = "update tb_BatchRule set RuleName=@RuleName,Format=@Format,Length=@Length,Remark=@Remark where BatchRuleId=@BatchRuleId";
            return BuildParamterInfo(batchRuleInfos, sql, BuildUpdateParameters);
        }

        public ParamterInfo DeleteBatchRules(List<BatchRuleInfo> batchRuleInfos)
        {
            const string sql = "delete from tb_BatchRule where BatchRuleId=@BatchRuleId";
            return BuildParamterInfo(batchRuleInfos, sql, info => new[]
            {
                new SqlParameter("@BatchRuleId", SqlDbType.Int) { Value = ToDbValue(info.BatchRuleId) }
            });
        }

        private static List<BatchRuleInfo> BuildBatchRules(DataSet dataSet)
        {
            List<BatchRuleInfo> result = new List<BatchRuleInfo>();
            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                return result;
            }

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                result.Add(new BatchRuleInfo
                {
                    BatchRuleId = row.IsNull("BatchRuleId") ? (int?)null : Convert.ToInt32(row["BatchRuleId"]),
                    RuleName = row.IsNull("RuleName") ? null : Convert.ToString(row["RuleName"]),
                    Format = row.IsNull("Format") ? null : Convert.ToString(row["Format"]),
                    Length = row.IsNull("Length") ? (int?)null : Convert.ToInt32(row["Length"]),
                    Remark = row.IsNull("Remark") ? null : Convert.ToString(row["Remark"])
                });
            }

            return result;
        }

        private static ParamterInfo BuildParamterInfo(List<BatchRuleInfo> infos, string sql, Func<BatchRuleInfo, SqlParameter[]> parameterBuilder)
        {
            ParamterInfo paramterInfo = new ParamterInfo
            {
                Sql = sql,
                Type = CommandType.Text,
                AlSQL = new ArrayList(),
                AlPAR = new ArrayList(),
                AlCOM = new ArrayList()
            };

            if (infos == null)
            {
                return paramterInfo;
            }

            foreach (BatchRuleInfo info in infos)
            {
                if (info == null)
                {
                    continue;
                }

                paramterInfo.AlSQL.Add(sql);
                paramterInfo.AlPAR.Add(parameterBuilder(info));
                paramterInfo.AlCOM.Add(CommandType.Text);
            }

            return paramterInfo;
        }

        private static SqlParameter[] BuildInsertParameters(BatchRuleInfo info)
        {
            return new[]
            {
                new SqlParameter("@RuleName", SqlDbType.NVarChar, 100) { Value = ToDbValue(info.RuleName) },
                new SqlParameter("@Format", SqlDbType.VarChar, 100) { Value = ToDbValue(info.Format) },
                new SqlParameter("@Length", SqlDbType.Int) { Value = ToDbValue(info.Length) },
                new SqlParameter("@Remark", SqlDbType.NVarChar, 500) { Value = ToDbValue(info.Remark) }
            };
        }

        private static SqlParameter[] BuildUpdateParameters(BatchRuleInfo info)
        {
            return new[]
            {
                new SqlParameter("@BatchRuleId", SqlDbType.Int) { Value = ToDbValue(info.BatchRuleId) },
                new SqlParameter("@RuleName", SqlDbType.NVarChar, 100) { Value = ToDbValue(info.RuleName) },
                new SqlParameter("@Format", SqlDbType.VarChar, 100) { Value = ToDbValue(info.Format) },
                new SqlParameter("@Length", SqlDbType.Int) { Value = ToDbValue(info.Length) },
                new SqlParameter("@Remark", SqlDbType.NVarChar, 500) { Value = ToDbValue(info.Remark) }
            };
        }

        private static object ToDbValue(object value)
        {
            return value ?? DBNull.Value;
        }
    }
}
