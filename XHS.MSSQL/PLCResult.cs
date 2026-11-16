using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Utility;
using XHS.IDAL;
using XHS.Model;

namespace XHS.MSSQL
{
    /// <summary>
    /// PLC 结果 SQL Server 数据访问实现。
    /// </summary>
    public class PLCResult : IPLCResult
    {
        public List<PLCResultInfo> GetPLCResultsById(int id)
        {
            string queryString =
                "select id,processno,partprocessid,createdate,resultvalue,[result] " +
                "from tb_PLCResult where id=@Id order by id";

            SqlParameter[] parameters =
            {
                new SqlParameter("@Id", SqlDbType.Int)
                {
                    Value = id
                }
            };

            return _getPLCResultInfo(queryString, parameters);
        }

        public bool InsertPLCResult(PLCResultInfo info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            string queryString =
                "insert into tb_PLCResult (processno,partprocessid,createdate,resultvalue,[result]) " +
                "values (@ProcessNo,@PartProcessId,@CreateDate,@ResultValue,@Result)";

            SqlParameter[] parameters =
            {
                new SqlParameter("@ProcessNo", SqlDbType.Int) { Value = (object)info.ProcessNo ?? DBNull.Value },
                new SqlParameter("@PartProcessId", SqlDbType.Int) { Value = (object)info.PartProcessId ?? DBNull.Value },
                new SqlParameter("@CreateDate", SqlDbType.DateTime) { Value = (object)info.CreateDate ?? DateTime.Now },
                new SqlParameter("@ResultValue", SqlDbType.Float) { Value = (object)info.ResultValue ?? DBNull.Value },
                new SqlParameter("@Result", SqlDbType.Int) { Value = info.Result }
            };

            try
            {
                return Data.ExecuteNonQuery(queryString, parameters) > 0;
            }
            catch
            {
                return false;
            }
        }

        private List<PLCResultInfo> _getPLCResultInfo(
            string querystring,
            SqlParameter[] pars)
        {
            DataSet dataSet = pars == null
                ? Data.getDataSet(querystring)
                : Data.getDataSet(querystring, pars);

            List<PLCResultInfo> result = new List<PLCResultInfo>();
            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                return result;
            }

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                result.Add(new PLCResultInfo
                {
                    Id = row.IsNull("id") ? (int?)null : Convert.ToInt32(row["id"]),
                    ProcessNo = row.IsNull("processno") ? (int?)null : Convert.ToInt32(row["processno"]),
                    PartProcessId = row.IsNull("partprocessid") ? (int?)null : Convert.ToInt32(row["partprocessid"]),
                    CreateDate = row.IsNull("createdate") ? (DateTime?)null : Convert.ToDateTime(row["createdate"]),
                    ResultValue = row.IsNull("resultvalue") ? (double?)null : Convert.ToDouble(row["resultvalue"]),
                    Result = row.IsNull("result") ? 1 : Convert.ToInt32(row["result"])
                });
            }

            return result;
        }
    }
}
