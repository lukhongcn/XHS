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
    /// 补打原因字典 SQL Server 数据访问实现。
    /// </summary>
    public class ReprintReason : IReprintReason
    {
        private const string ReprintReasonSelectColumns = "Id,ReasonName,SortNo,IsEnable";
        private const string ReprintReasonOrderBy = " order by SortNo asc, Id asc";

        public List<ReprintReasonInfo> GetReprintReasons()
        {
            string queryString = "select " + ReprintReasonSelectColumns + " from tb_ReprintReason" + ReprintReasonOrderBy;
            return GetReprintReasonsBySql(queryString);
        }

        public List<ReprintReasonInfo> GetEnabledReprintReasons()
        {
            string queryString = "select " + ReprintReasonSelectColumns + " from tb_ReprintReason where IsEnable=1" + ReprintReasonOrderBy;
            return GetReprintReasonsBySql(queryString);
        }

        public ParamterInfo InsertReprintReason(List<ReprintReasonInfo> reprintReasonInfos)
        {
            const string sql = "insert into tb_ReprintReason (ReasonName,SortNo,IsEnable) values (@ReasonName,@SortNo,@IsEnable)";
            return BuildParamterInfo(reprintReasonInfos, sql, BuildInsertOrUpdateParameters);
        }

        public ParamterInfo UpdateReprintReason(List<ReprintReasonInfo> reprintReasonInfos)
        {
            const string sql = "update tb_ReprintReason set ReasonName=@ReasonName,SortNo=@SortNo,IsEnable=@IsEnable where Id=@Id";
            return BuildParamterInfo(reprintReasonInfos, sql, BuildInsertOrUpdateParameters);
        }

        public ParamterInfo DeleteReprintReason(List<ReprintReasonInfo> reprintReasonInfos)
        {
            const string sql = "delete from tb_ReprintReason where Id=@Id";
            return BuildParamterInfo(reprintReasonInfos, sql, info => new[]
            {
                new SqlParameter("@Id", SqlDbType.Int) { Value = ToDbValue(info.Id) }
            });
        }

        private static List<ReprintReasonInfo> GetReprintReasonsBySql(string queryString)
        {
            DataSet dataSet = Data.getDataSet(queryString);
            return BuildReprintReasons(dataSet);
        }

        private static List<ReprintReasonInfo> BuildReprintReasons(DataSet dataSet)
        {
            List<ReprintReasonInfo> result = new List<ReprintReasonInfo>();
            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                return result;
            }

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                result.Add(new ReprintReasonInfo
                {
                    Id = ReadNullableInt(row, "Id"),
                    ReasonName = ReadString(row, "ReasonName"),
                    SortNo = ReadNullableInt(row, "SortNo"),
                    IsEnable = ReadNullableBool(row, "IsEnable")
                });
            }

            return result;
        }

        private static ParamterInfo BuildParamterInfo(List<ReprintReasonInfo> reprintReasonInfos, string sql, Func<ReprintReasonInfo, SqlParameter[]> parameterBuilder)
        {
            ParamterInfo paramterInfo = new ParamterInfo
            {
                Sql = sql,
                Type = CommandType.Text,
                AlSQL = new ArrayList(),
                AlPAR = new ArrayList(),
                AlCOM = new ArrayList()
            };

            if (reprintReasonInfos == null)
            {
                return paramterInfo;
            }

            foreach (ReprintReasonInfo reprintReasonInfo in reprintReasonInfos)
            {
                if (reprintReasonInfo == null)
                {
                    continue;
                }

                paramterInfo.AlSQL.Add(sql);
                paramterInfo.AlPAR.Add(parameterBuilder(reprintReasonInfo));
                paramterInfo.AlCOM.Add(CommandType.Text);
            }

            return paramterInfo;
        }

        private static SqlParameter[] BuildInsertOrUpdateParameters(ReprintReasonInfo info)
        {
            return new[]
            {
                new SqlParameter("@Id", SqlDbType.Int) { Value = ToDbValue(info.Id) },
                new SqlParameter("@ReasonName", SqlDbType.NVarChar, 200) { Value = ToDbValue(info.ReasonName) },
                new SqlParameter("@SortNo", SqlDbType.Int) { Value = ToDbValue(info.SortNo) },
                new SqlParameter("@IsEnable", SqlDbType.Bit) { Value = ToDbValue(info.IsEnable) }
            };
        }

        private static object ToDbValue(object value)
        {
            return value ?? DBNull.Value;
        }

        private static string ReadString(DataRow row, string columnName)
        {
            return row.IsNull(columnName) ? null : row[columnName].ToString();
        }

        private static int? ReadNullableInt(DataRow row, string columnName)
        {
            return row.IsNull(columnName) ? (int?)null : Convert.ToInt32(row[columnName]);
        }

        private static bool? ReadNullableBool(DataRow row, string columnName)
        {
            return row.IsNull(columnName) ? (bool?)null : Convert.ToBoolean(row[columnName]);
        }
    }
}
