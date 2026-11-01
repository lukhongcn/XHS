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
    /// 标签编码规则 SQL Server 数据访问实现。
    /// </summary>
    public class LabelCodeRule : ILabelCodeRule
    {
        private const string SelectColumns = "RuleId,RuleName,LabelType,CustomerId,ParseType,Separator,KeySeparator,Version,Enabled,Remark,MatchRegex";

        public List<LabelCodeRuleInfo> GetLabelCodeRules()
        {
            DataSet dataSet = Data.getDataSet("select " + SelectColumns + " from tb_LabelCodeRule order by RuleId asc");
            return BuildLabelCodeRules(dataSet);
        }

        public LabelCodeRuleInfo GetLabelCodeRule(int ruleId)
        {
            List<LabelCodeRuleInfo> result = BuildLabelCodeRules(Data.getDataSet(
                "select " + SelectColumns + " from tb_LabelCodeRule where RuleId=" + ruleId));
            return result.Count == 0 ? null : result[0];
        }

        public List<LabelCodeRuleInfo> GetLabelCodeRulesByCustomerId(string customerId)
        {
            DataSet dataSet = Data.getDataSet(
                "select " + SelectColumns + " from tb_LabelCodeRule where CustomerId='" + customerId.Replace("'", "''") + "' order by RuleId asc");
            return BuildLabelCodeRules(dataSet);
        }

        public List<LabelCodeRuleInfo> GetLabelCodeRulesByCustomerIdAndLabelType(string customerId, string labelType)
        {
            const string sql = "select " + SelectColumns + " from tb_LabelCodeRule where CustomerId=@customerId and LabelType=@labelType order by RuleId asc";
            SqlParameter[] pars = new SqlParameter[]
            {
                new SqlParameter("@customerId", SqlDbType.VarChar, 100) { Value = customerId },
                new SqlParameter("@labelType", SqlDbType.VarChar, 20) { Value = labelType }
            };
            DataSet dataSet = Data.getDataSet(sql, pars);
            return BuildLabelCodeRules(dataSet);
        }

        public ParamterInfo InsertLabelCodeRules(List<LabelCodeRuleInfo> labelCodeRuleInfos)
        {
            const string sql = "insert into tb_LabelCodeRule (RuleName,LabelType,CustomerId,ParseType,Separator,KeySeparator,Version,Enabled,Remark,MatchRegex) values (@RuleName,@LabelType,@CustomerId,@ParseType,@Separator,@KeySeparator,@Version,@Enabled,@Remark,@MatchRegex)";
            return BuildParamterInfo(labelCodeRuleInfos, sql, BuildInsertParameters);
        }

        public ParamterInfo UpdateLabelCodeRules(List<LabelCodeRuleInfo> labelCodeRuleInfos)
        {
            const string sql = "update tb_LabelCodeRule set RuleName=@RuleName,LabelType=@LabelType,CustomerId=@CustomerId,ParseType=@ParseType,Separator=@Separator,KeySeparator=@KeySeparator,Version=@Version,Enabled=@Enabled,Remark=@Remark,MatchRegex=@MatchRegex where RuleId=@RuleId";
            return BuildParamterInfo(labelCodeRuleInfos, sql, BuildUpdateParameters);
        }

        public ParamterInfo DeleteLabelCodeRules(List<LabelCodeRuleInfo> labelCodeRuleInfos)
        {
            const string sql = "delete from tb_LabelCodeRule where RuleId=@RuleId";
            return BuildParamterInfo(labelCodeRuleInfos, sql, info => new[]
            {
                new SqlParameter("@RuleId", SqlDbType.Int) { Value = ToDbValue(info.RuleId) }
            });
        }

        private static List<LabelCodeRuleInfo> BuildLabelCodeRules(DataSet dataSet)
        {
            List<LabelCodeRuleInfo> result = new List<LabelCodeRuleInfo>();
            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                return result;
            }

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                result.Add(new LabelCodeRuleInfo
                {
                    RuleId = row.IsNull("RuleId") ? (int?)null : Convert.ToInt32(row["RuleId"]),
                    RuleName = row.IsNull("RuleName") ? null : Convert.ToString(row["RuleName"]),
                    LabelType = row.IsNull("LabelType") ? null : Convert.ToString(row["LabelType"]),
                    CustomerId = row.IsNull("CustomerId") ? null : Convert.ToString(row["CustomerId"]),
                    ParseType = row.IsNull("ParseType") ? null : Convert.ToString(row["ParseType"]),
                    Separator = row.IsNull("Separator") ? null : Convert.ToString(row["Separator"]),
                    KeySeparator = row.IsNull("KeySeparator") ? null : Convert.ToString(row["KeySeparator"]),
                    Version = row.IsNull("Version") ? (int?)null : Convert.ToInt32(row["Version"]),
                    Enabled = row.IsNull("Enabled") ? (bool?)null : Convert.ToBoolean(row["Enabled"]),
                    Remark = row.IsNull("Remark") ? null : Convert.ToString(row["Remark"]),
                    MatchRegex = row.IsNull("MatchRegex") ? null : Convert.ToString(row["MatchRegex"])
                });
            }

            return result;
        }

        private static ParamterInfo BuildParamterInfo(List<LabelCodeRuleInfo> infos, string sql, Func<LabelCodeRuleInfo, SqlParameter[]> parameterBuilder)
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

            foreach (LabelCodeRuleInfo info in infos)
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

        private static SqlParameter[] BuildInsertParameters(LabelCodeRuleInfo info)
        {
            return new[]
            {
                new SqlParameter("@RuleName", SqlDbType.NVarChar, 150) { Value = ToDbValue(info.RuleName) },
                new SqlParameter("@LabelType", SqlDbType.VarChar, 20) { Value = ToDbValue(info.LabelType) },
                new SqlParameter("@CustomerId", SqlDbType.VarChar, 100) { Value = ToDbValue(info.CustomerId) },
                new SqlParameter("@ParseType", SqlDbType.VarChar, 20) { Value = ToDbValue(info.ParseType) },
                new SqlParameter("@Separator", SqlDbType.NVarChar, 20) { Value = ToDbValue(info.Separator) },
                new SqlParameter("@KeySeparator", SqlDbType.NVarChar, 20) { Value = ToDbValue(info.KeySeparator) },
                new SqlParameter("@Version", SqlDbType.Int) { Value = ToDbValue(info.Version) },
                new SqlParameter("@Enabled", SqlDbType.Bit) { Value = ToDbValue(info.Enabled) },
                new SqlParameter("@Remark", SqlDbType.NVarChar, 1000) { Value = ToDbValue(info.Remark) },
                new SqlParameter("@MatchRegex", SqlDbType.VarChar, 500) { Value = ToDbValue(info.MatchRegex) }
            };
        }

        private static SqlParameter[] BuildUpdateParameters(LabelCodeRuleInfo info)
        {
            return new[]
            {
                new SqlParameter("@RuleId", SqlDbType.Int) { Value = ToDbValue(info.RuleId) },
                new SqlParameter("@RuleName", SqlDbType.NVarChar, 150) { Value = ToDbValue(info.RuleName) },
                new SqlParameter("@LabelType", SqlDbType.VarChar, 20) { Value = ToDbValue(info.LabelType) },
                new SqlParameter("@CustomerId", SqlDbType.VarChar, 100) { Value = ToDbValue(info.CustomerId) },
                new SqlParameter("@ParseType", SqlDbType.VarChar, 20) { Value = ToDbValue(info.ParseType) },
                new SqlParameter("@Separator", SqlDbType.NVarChar, 20) { Value = ToDbValue(info.Separator) },
                new SqlParameter("@KeySeparator", SqlDbType.NVarChar, 20) { Value = ToDbValue(info.KeySeparator) },
                new SqlParameter("@Version", SqlDbType.Int) { Value = ToDbValue(info.Version) },
                new SqlParameter("@Enabled", SqlDbType.Bit) { Value = ToDbValue(info.Enabled) },
                new SqlParameter("@Remark", SqlDbType.NVarChar, 1000) { Value = ToDbValue(info.Remark) },
                new SqlParameter("@MatchRegex", SqlDbType.VarChar, 500) { Value = ToDbValue(info.MatchRegex) }
            };
        }

        private static object ToDbValue(object value)
        {
            return value ?? DBNull.Value;
        }
    }
}
