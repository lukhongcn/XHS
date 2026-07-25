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
    /// 标签编码规则字段映射 SQL Server 数据访问实现。
    /// </summary>
    public class LabelCodeRuleField : ILabelCodeRuleField
    {
        private const string SelectColumns = "Id,RuleId,FieldName,DataType,KeyCode,Position,StartPosition,Length,BatchRuleId,Required,SortNo";
        private const string OrderBy = " order by SortNo asc, Id asc";

        public List<LabelCodeRuleFieldInfo> GetLabelCodeRuleFields()
        {
            return GetLabelCodeRuleFieldsBySql("select " + SelectColumns + " from tb_LabelCodeRuleField" + OrderBy);
        }

        public List<LabelCodeRuleFieldInfo> GetLabelCodeRuleFieldsByRuleId(int ruleId)
        {
            string queryString = "select " + SelectColumns + " from tb_LabelCodeRuleField where RuleId=" + ruleId + OrderBy;
            return GetLabelCodeRuleFieldsBySql(queryString);
        }

        public List<LabelCodeRuleFieldInfo> GetRuleFieldByCustomer(string customerId)
        {
            string queryString =
                "select " + SelectColumns + " from tb_LabelCodeRuleField f" +
                " inner join tb_LabelCodeRule r on f.RuleId = r.RuleId" +
                " where r.CustomerId='" + (customerId ?? string.Empty).Replace("'", "''") + "'" +
                OrderBy;
            return GetLabelCodeRuleFieldsBySql(queryString);
        }

        public ParamterInfo InsertLabelCodeRuleFields(List<LabelCodeRuleFieldInfo> labelCodeRuleFieldInfos)
        {
            const string sql = "insert into tb_LabelCodeRuleField (RuleId,FieldName,DataType,KeyCode,Position,StartPosition,Length,BatchRuleId,Required,SortNo) values (@RuleId,@FieldName,@DataType,@KeyCode,@Position,@StartPosition,@Length,@BatchRuleId,@Required,@SortNo)";
            return BuildParamterInfo(labelCodeRuleFieldInfos, sql, BuildInsertParameters);
        }

        public ParamterInfo UpdateLabelCodeRuleFields(List<LabelCodeRuleFieldInfo> labelCodeRuleFieldInfos)
        {
            const string sql = "update tb_LabelCodeRuleField set RuleId=@RuleId,FieldName=@FieldName,DataType=@DataType,KeyCode=@KeyCode,Position=@Position,StartPosition=@StartPosition,Length=@Length,BatchRuleId=@BatchRuleId,Required=@Required,SortNo=@SortNo where Id=@Id";
            return BuildParamterInfo(labelCodeRuleFieldInfos, sql, BuildUpdateParameters);
        }

        public ParamterInfo DeleteLabelCodeRuleFields(List<LabelCodeRuleFieldInfo> labelCodeRuleFieldInfos)
        {
            const string sql = "delete from tb_LabelCodeRuleField where Id=@Id";
            return BuildParamterInfo(labelCodeRuleFieldInfos, sql, info => new[]
            {
                new SqlParameter("@Id", SqlDbType.Int) { Value = ToDbValue(info.Id) }
            });
        }

        private static List<LabelCodeRuleFieldInfo> GetLabelCodeRuleFieldsBySql(string queryString)
        {
            DataSet dataSet = Data.getDataSet(queryString);
            List<LabelCodeRuleFieldInfo> result = new List<LabelCodeRuleFieldInfo>();
            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                return result;
            }

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                result.Add(new LabelCodeRuleFieldInfo
                {
                    Id = row.IsNull("Id") ? (int?)null : Convert.ToInt32(row["Id"]),
                    RuleId = row.IsNull("RuleId") ? (int?)null : Convert.ToInt32(row["RuleId"]),
                    FieldName = row.IsNull("FieldName") ? null : Convert.ToString(row["FieldName"]),
                    DataType = row.IsNull("DataType") ? null : Convert.ToString(row["DataType"]),
                    KeyCode = row.IsNull("KeyCode") ? null : Convert.ToString(row["KeyCode"]),
                    Position = row.IsNull("Position") ? (int?)null : Convert.ToInt32(row["Position"]),
                    StartPosition = row.IsNull("StartPosition") ? (int?)null : Convert.ToInt32(row["StartPosition"]),
                    Length = row.IsNull("Length") ? (int?)null : Convert.ToInt32(row["Length"]),
                    BatchRuleId = row.IsNull("BatchRuleId") ? (int?)null : Convert.ToInt32(row["BatchRuleId"]),
                    Required = row.IsNull("Required") ? (bool?)null : Convert.ToBoolean(row["Required"]),
                    SortNo = row.IsNull("SortNo") ? (int?)null : Convert.ToInt32(row["SortNo"])
                });
            }

            return result;
        }

        private static ParamterInfo BuildParamterInfo(List<LabelCodeRuleFieldInfo> infos, string sql, Func<LabelCodeRuleFieldInfo, SqlParameter[]> parameterBuilder)
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

            foreach (LabelCodeRuleFieldInfo info in infos)
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

        private static SqlParameter[] BuildInsertParameters(LabelCodeRuleFieldInfo info)
        {
            return new[]
            {
                new SqlParameter("@RuleId", SqlDbType.Int) { Value = ToDbValue(info.RuleId) },
                new SqlParameter("@FieldName", SqlDbType.NVarChar, 100) { Value = ToDbValue(info.FieldName) },
                new SqlParameter("@DataType", SqlDbType.NVarChar, 30) { Value = ToDbValue(info.DataType) },
                new SqlParameter("@KeyCode", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.KeyCode) },
                new SqlParameter("@Position", SqlDbType.Int) { Value = ToDbValue(info.Position) },
                new SqlParameter("@StartPosition", SqlDbType.Int) { Value = ToDbValue(info.StartPosition) },
                new SqlParameter("@Length", SqlDbType.Int) { Value = ToDbValue(info.Length) },
                new SqlParameter("@BatchRuleId", SqlDbType.Int) { Value = ToDbValue(info.BatchRuleId) },
                new SqlParameter("@Required", SqlDbType.Bit) { Value = ToDbValue(info.Required) },
                new SqlParameter("@SortNo", SqlDbType.Int) { Value = ToDbValue(info.SortNo) }
            };
        }

        private static SqlParameter[] BuildUpdateParameters(LabelCodeRuleFieldInfo info)
        {
            List<SqlParameter> parameters = new List<SqlParameter>(BuildInsertParameters(info))
            {
                new SqlParameter("@Id", SqlDbType.Int) { Value = ToDbValue(info.Id) }
            };
            return parameters.ToArray();
        }

        private static object ToDbValue(object value)
        {
            return value ?? DBNull.Value;
        }
    }
}
