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
    /// 标签编码规则重复明细子规则 SQL Server 数据访问实现。
    /// </summary>
    public class LabelCodeRuleSub : ILabelCodeRuleSub
    {
        private const string SelectColumns =
            "SubRuleId,RuleId,SubRuleName,ItemSeparator,FieldSeparator,ItemRegex,SortNo,Enabled,Remark";
        private const string OrderBy = " order by SortNo asc, SubRuleId asc";

        public List<LabelCodeRuleSubInfo> GetLabelCodeRuleSubs()
        {
            return GetBySql("select " + SelectColumns + " from tb_LabelCodeRuleSub" + OrderBy, null);
        }

        public LabelCodeRuleSubInfo GetLabelCodeRuleSub(int subRuleId)
        {
            const string sql = "select " + SelectColumns + " from tb_LabelCodeRuleSub where SubRuleId=@SubRuleId";
            List<LabelCodeRuleSubInfo> result = GetBySql(sql, new[]
            {
                new SqlParameter("@SubRuleId", SqlDbType.Int) { Value = subRuleId }
            });
            return result.Count == 0 ? null : result[0];
        }

        public List<LabelCodeRuleSubInfo> GetLabelCodeRuleSubsByRuleId(int ruleId)
        {
            const string sql = "select " + SelectColumns + " from tb_LabelCodeRuleSub where RuleId=@RuleId" + OrderBy;
            return GetBySql(sql, new[]
            {
                new SqlParameter("@RuleId", SqlDbType.Int) { Value = ruleId }
            });
        }

        public List<LabelCodeRuleSubInfo> GetLabelCodeRuleSubsByCustomerIdAndLabelType(
            string customerId,
            string labelType)
        {
            string sql =
                "select s." + SelectColumns.Replace(",", ",s.") +
                " from tb_LabelCodeRuleSub s inner join tb_LabelCodeRule r on s.RuleId=r.RuleId" +
                " where r.CustomerId=@CustomerId and r.LabelType=@LabelType" +
                " and (s.Enabled is null or s.Enabled=1)" +
                " order by s.SortNo asc,s.SubRuleId asc";
            return GetBySql(sql, new[]
            {
                new SqlParameter("@CustomerId", SqlDbType.VarChar, 100) { Value = ToDbValue(customerId) },
                new SqlParameter("@LabelType", SqlDbType.VarChar, 50) { Value = ToDbValue(labelType) }
            });
        }

        public ParamterInfo InsertLabelCodeRuleSubs(List<LabelCodeRuleSubInfo> infos)
        {
            const string sql = "insert into tb_LabelCodeRuleSub (RuleId,SubRuleName,ItemSeparator,FieldSeparator,ItemRegex,SortNo,Enabled,Remark) values (@RuleId,@SubRuleName,@ItemSeparator,@FieldSeparator,@ItemRegex,@SortNo,@Enabled,@Remark)";
            return BuildParameterInfo(infos, sql, BuildInsertParameters);
        }

        public ParamterInfo UpdateLabelCodeRuleSubs(List<LabelCodeRuleSubInfo> infos)
        {
            const string sql = "update tb_LabelCodeRuleSub set RuleId=@RuleId,SubRuleName=@SubRuleName,ItemSeparator=@ItemSeparator,FieldSeparator=@FieldSeparator,ItemRegex=@ItemRegex,SortNo=@SortNo,Enabled=@Enabled,Remark=@Remark where SubRuleId=@SubRuleId";
            return BuildParameterInfo(infos, sql, BuildUpdateParameters);
        }

        public ParamterInfo DeleteLabelCodeRuleSubs(List<LabelCodeRuleSubInfo> infos)
        {
            const string sql = "delete from tb_LabelCodeRuleSub where SubRuleId=@SubRuleId";
            return BuildParameterInfo(infos, sql, info => new[]
            {
                new SqlParameter("@SubRuleId", SqlDbType.Int) { Value = ToDbValue(info.SubRuleId) }
            });
        }

        private static List<LabelCodeRuleSubInfo> GetBySql(string queryString, SqlParameter[] parameters)
        {
            DataSet dataSet = parameters == null
                ? Data.getDataSet(queryString)
                : Data.getDataSet(queryString, parameters);
            List<LabelCodeRuleSubInfo> result = new List<LabelCodeRuleSubInfo>();
            if (dataSet == null || dataSet.Tables.Count == 0) return result;

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                result.Add(new LabelCodeRuleSubInfo
                {
                    SubRuleId = ReadInt(row, "SubRuleId"),
                    RuleId = ReadInt(row, "RuleId"),
                    SubRuleName = ReadString(row, "SubRuleName"),
                    ItemSeparator = ReadString(row, "ItemSeparator"),
                    FieldSeparator = ReadString(row, "FieldSeparator"),
                    ItemRegex = ReadString(row, "ItemRegex"),
                    SortNo = ReadInt(row, "SortNo"),
                    Enabled = ReadBool(row, "Enabled"),
                    Remark = ReadString(row, "Remark")
                });
            }

            return result;
        }

        private static ParamterInfo BuildParameterInfo(
            List<LabelCodeRuleSubInfo> infos,
            string sql,
            Func<LabelCodeRuleSubInfo, SqlParameter[]> parameterBuilder)
        {
            ParamterInfo result = new ParamterInfo
            {
                Sql = sql,
                Type = CommandType.Text,
                AlSQL = new ArrayList(),
                AlPAR = new ArrayList(),
                AlCOM = new ArrayList()
            };
            if (infos == null) return result;

            foreach (LabelCodeRuleSubInfo info in infos)
            {
                if (info == null) continue;
                result.AlSQL.Add(sql);
                result.AlPAR.Add(parameterBuilder(info));
                result.AlCOM.Add(CommandType.Text);
            }
            return result;
        }

        private static SqlParameter[] BuildInsertParameters(LabelCodeRuleSubInfo info)
        {
            return new[]
            {
                new SqlParameter("@RuleId", SqlDbType.Int) { Value = ToDbValue(info.RuleId) },
                new SqlParameter("@SubRuleName", SqlDbType.NVarChar, 100) { Value = ToDbValue(info.SubRuleName) },
                new SqlParameter("@ItemSeparator", SqlDbType.VarChar, 20) { Value = ToDbValue(info.ItemSeparator) },
                new SqlParameter("@FieldSeparator", SqlDbType.VarChar, 20) { Value = ToDbValue(info.FieldSeparator) },
                new SqlParameter("@ItemRegex", SqlDbType.VarChar, 500) { Value = ToDbValue(info.ItemRegex) },
                new SqlParameter("@SortNo", SqlDbType.Int) { Value = ToDbValue(info.SortNo) },
                new SqlParameter("@Enabled", SqlDbType.Bit) { Value = ToDbValue(info.Enabled) },
                new SqlParameter("@Remark", SqlDbType.NVarChar, 500) { Value = ToDbValue(info.Remark) }
            };
        }

        private static SqlParameter[] BuildUpdateParameters(LabelCodeRuleSubInfo info)
        {
            List<SqlParameter> parameters = new List<SqlParameter>(BuildInsertParameters(info))
            {
                new SqlParameter("@SubRuleId", SqlDbType.Int) { Value = ToDbValue(info.SubRuleId) }
            };
            return parameters.ToArray();
        }

        private static int? ReadInt(DataRow row, string column)
        {
            return row.IsNull(column) ? (int?)null : Convert.ToInt32(row[column]);
        }

        private static bool? ReadBool(DataRow row, string column)
        {
            return row.IsNull(column) ? (bool?)null : Convert.ToBoolean(row[column]);
        }

        private static string ReadString(DataRow row, string column)
        {
            return row.IsNull(column) ? null : Convert.ToString(row[column]);
        }

        private static object ToDbValue(object value)
        {
            return value ?? DBNull.Value;
        }
    }
}
