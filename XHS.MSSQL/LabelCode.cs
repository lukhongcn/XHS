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
    /// 标签编码解析规则 SQL Server 数据访问实现。
    /// </summary>
    public class LabelCode : ILabelCode
    {
        private const string LabelCodeSelectColumns = "RuleId,RuleName,LabelType,CustomerId,ParseType,Separator,KeySeparator,Version,Enabled,Remark";
        private const string LabelCodeOrderBy = " order by LabelType asc, CustomerId asc, Version desc, RuleId desc";

        public List<LabelCodeInfo> GetLabelCodes()
        {
            string queryString = "select " + LabelCodeSelectColumns + " from tb_LabelCode" + LabelCodeOrderBy;
            return GetLabelCodesBySql(queryString);
        }

        public List<LabelCodeInfo> GetEnabledLabelCodes()
        {
            string queryString = "select " + LabelCodeSelectColumns + " from tb_LabelCode where Enabled=1" + LabelCodeOrderBy;
            return GetLabelCodesBySql(queryString);
        }

        public List<LabelCodeInfo> GetEnabledLabelCodes(int? customerId, string labelType)
        {
            string queryString = "select " + LabelCodeSelectColumns + " from tb_LabelCode where Enabled=1";
            if (customerId.HasValue)
            {
                queryString += " and CustomerId=" + customerId.Value;
            }

            if (!string.IsNullOrWhiteSpace(labelType))
            {
                queryString += " and LabelType=N'" + EscapeSqlLiteral(labelType.Trim()) + "'";
            }

            return GetLabelCodesBySql(queryString + LabelCodeOrderBy);
        }

        public ParamterInfo InsertLabelCodes(List<LabelCodeInfo> labelCodeInfos)
        {
            const string sql = "insert into tb_LabelCode (RuleName,LabelType,CustomerId,ParseType,Separator,KeySeparator,Version,Enabled,Remark) values (@RuleName,@LabelType,@CustomerId,@ParseType,@Separator,@KeySeparator,@Version,@Enabled,@Remark)";
            return BuildParamterInfo(labelCodeInfos, sql, BuildInsertParameters);
        }

        public ParamterInfo UpdateLabelCodes(List<LabelCodeInfo> labelCodeInfos)
        {
            const string sql = "update tb_LabelCode set RuleName=@RuleName,LabelType=@LabelType,CustomerId=@CustomerId,ParseType=@ParseType,Separator=@Separator,KeySeparator=@KeySeparator,Version=@Version,Enabled=@Enabled,Remark=@Remark where RuleId=@RuleId";
            return BuildParamterInfo(labelCodeInfos, sql, BuildUpdateParameters);
        }

        public ParamterInfo DeleteLabelCodes(List<LabelCodeInfo> labelCodeInfos)
        {
            const string sql = "delete from tb_LabelCode where RuleId=@RuleId";
            return BuildParamterInfo(labelCodeInfos, sql, info => new[]
            {
                new SqlParameter("@RuleId", SqlDbType.Int) { Value = ToDbValue(info.RuleId) }
            });
        }

        private static List<LabelCodeInfo> GetLabelCodesBySql(string queryString)
        {
            DataSet dataSet = Data.getDataSet(queryString);
            List<LabelCodeInfo> result = new List<LabelCodeInfo>();
            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                return result;
            }

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                result.Add(new LabelCodeInfo
                {
                    RuleId = row.IsNull("RuleId") ? (int?)null : Convert.ToInt32(row["RuleId"]),
                    RuleName = row.IsNull("RuleName") ? null : Convert.ToString(row["RuleName"]),
                    LabelType = row.IsNull("LabelType") ? null : Convert.ToString(row["LabelType"]),
                    CustomerId = row.IsNull("CustomerId") ? (int?)null : Convert.ToInt32(row["CustomerId"]),
                    ParseType = row.IsNull("ParseType") ? null : Convert.ToString(row["ParseType"]),
                    Separator = row.IsNull("Separator") ? null : Convert.ToString(row["Separator"]),
                    KeySeparator = row.IsNull("KeySeparator") ? null : Convert.ToString(row["KeySeparator"]),
                    Version = row.IsNull("Version") ? (int?)null : Convert.ToInt32(row["Version"]),
                    Enabled = row.IsNull("Enabled") ? (bool?)null : Convert.ToBoolean(row["Enabled"]),
                    Remark = row.IsNull("Remark") ? null : Convert.ToString(row["Remark"])
                });
            }

            return result;
        }

        private static ParamterInfo BuildParamterInfo(List<LabelCodeInfo> labelCodeInfos, string sql, Func<LabelCodeInfo, SqlParameter[]> parameterBuilder)
        {
            ParamterInfo paramterInfo = new ParamterInfo
            {
                Sql = sql,
                Type = CommandType.Text,
                AlSQL = new ArrayList(),
                AlPAR = new ArrayList(),
                AlCOM = new ArrayList()
            };

            if (labelCodeInfos == null)
            {
                return paramterInfo;
            }

            foreach (LabelCodeInfo labelCodeInfo in labelCodeInfos)
            {
                if (labelCodeInfo == null)
                {
                    continue;
                }

                paramterInfo.AlSQL.Add(sql);
                paramterInfo.AlPAR.Add(parameterBuilder(labelCodeInfo));
                paramterInfo.AlCOM.Add(CommandType.Text);
            }

            return paramterInfo;
        }

        private static SqlParameter[] BuildInsertParameters(LabelCodeInfo info)
        {
            return new[]
            {
                new SqlParameter("@RuleName", SqlDbType.NVarChar, 100) { Value = ToDbValue(info.RuleName) },
                new SqlParameter("@LabelType", SqlDbType.NVarChar, 20) { Value = ToDbValue(info.LabelType) },
                new SqlParameter("@CustomerId", SqlDbType.Int) { Value = ToDbValue(info.CustomerId) },
                new SqlParameter("@ParseType", SqlDbType.NVarChar, 30) { Value = ToDbValue(info.ParseType) },
                new SqlParameter("@Separator", SqlDbType.NVarChar, 10) { Value = ToDbValue(info.Separator) },
                new SqlParameter("@KeySeparator", SqlDbType.NVarChar, 10) { Value = ToDbValue(info.KeySeparator) },
                new SqlParameter("@Version", SqlDbType.Int) { Value = ToDbValue(info.Version) },
                new SqlParameter("@Enabled", SqlDbType.Bit) { Value = ToDbValue(info.Enabled) },
                new SqlParameter("@Remark", SqlDbType.NVarChar, 500) { Value = ToDbValue(info.Remark) }
            };
        }

        private static SqlParameter[] BuildUpdateParameters(LabelCodeInfo info)
        {
            SqlParameter[] parameters = BuildInsertParameters(info);
            List<SqlParameter> result = new List<SqlParameter>(parameters)
            {
                new SqlParameter("@RuleId", SqlDbType.Int) { Value = ToDbValue(info.RuleId) }
            };
            return result.ToArray();
        }

        private static object ToDbValue(object value)
        {
            return value ?? DBNull.Value;
        }

        private static string EscapeSqlLiteral(string value)
        {
            return value.Replace("'", "''");
        }
    }
}
