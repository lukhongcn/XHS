using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ModuleWorkFlow.Model;
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
        public List<PLCResultInfo> GetPLCResults(string partNo, string qrCode, DateTime? startDate, DateTime? endDate)
        {
            string queryString =
                "select result.id,result.processno,result.partprocessid," +
                "partprocess.moduleid,partprocess.partno,partprocess.partno_id qrCode," +
                "result.createdate,result.resultvalue,result.[result],result.deal,result.dealtmethod " +
                "from tb_PLCResult result " +
                "left join tb_partprocess partprocess on result.processno=partprocess.processno " +
                "where (@PartNo=N'' or partprocess.partno like @PartNo) " +
                "and (@QRCode=N'' or partprocess.partno_id like @QRCode) ";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@PartNo", SqlDbType.NVarChar, 100)
                {
                    Value = string.IsNullOrWhiteSpace(partNo) ? (object)string.Empty : "%" + partNo.Trim() + "%"
                },
                new SqlParameter("@QRCode", SqlDbType.NVarChar, 500)
                {
                    Value = string.IsNullOrWhiteSpace(qrCode) ? (object)string.Empty : "%" + qrCode.Trim() + "%"
                }
            };

            if (startDate.HasValue)
            {
                queryString += "and result.createdate>=@StartDate ";
                parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime) { Value = startDate.Value });
            }

            if (endDate.HasValue)
            {
                queryString += "and result.createdate<@EndDate ";
                parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime) { Value = endDate.Value });
            }

            queryString += "order by result.createdate desc,result.id desc";
            return _getPLCResultInfo(queryString, parameters.ToArray());
        }

        public List<PLCResultInfo> GetPLCResultsById(int id)
        {
            string queryString =
                "select id,processno,partprocessid,createdate,resultvalue,[result],deal,dealtmethod " +
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
                new SqlParameter("@PartProcessId", SqlDbType.NVarChar, 50) { Value = (object)(info.PartProcessId.HasValue ? info.PartProcessId.Value.ToString() : null) ?? DBNull.Value },
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

        public PLCResultInfo GetLatestUndealt()
        {
            const string queryString = @"
select top 1 result.id,result.processno,result.partprocessid,
       partprocess.moduleid,partprocess.partno,partprocess.partno_id qrCode,
       result.createdate,result.resultvalue,result.[result],result.deal,result.dealtmethod
from tb_PLCResult result
left join tb_partprocess partprocess on partprocess.processno=result.processno
where result.deal=0
order by result.createdate desc,result.id desc";

            List<PLCResultInfo> records = _getPLCResultInfo(queryString, null);
            return records.Count == 0 ? null : records[0];
        }

        public bool MarkDeal(int id)
        {
            const string queryString = "update tb_PLCResult set deal=1 where id=@Id and deal=0";
            SqlParameter[] parameters =
            {
                new SqlParameter("@Id", SqlDbType.Int) { Value = id }
            };
            return Data.ExecuteNonQuery(queryString, parameters) > 0;
        }

        public ModuleWorkFlow.Model.ParamterInfo UpdateDeal(int processNo)
        {
            const string queryString =
                "update tb_PLCResult " +
                "set deal=1 " +
                "where processno=@ProcessNo";

            return new ModuleWorkFlow.Model.ParamterInfo
            {
                Sql = queryString,
                Type = CommandType.Text,
                AlSQL = new ArrayList { queryString },
                AlPAR = new ArrayList
                {
                    new[]
                    {
                        new SqlParameter("@ProcessNo", SqlDbType.Int) { Value = processNo }
                    }
                },
                AlCOM = new ArrayList { CommandType.Text }
            };
        }

        public ModuleWorkFlow.Model.ParamterInfo UpdateDealtMethod(
            int processNo,
            string dealtMethod)
        {
            const string queryString =
                "update tb_PLCResult " +
                "set dealtmethod=@DealtMethod " +
                "where processno=@ProcessNo and deal=1";

            return new ModuleWorkFlow.Model.ParamterInfo
            {
                Sql = queryString,
                Type = CommandType.Text,
                AlSQL = new ArrayList { queryString },
                AlPAR = new ArrayList
                {
                    new[]
                    {
                        new SqlParameter("@ProcessNo", SqlDbType.Int) { Value = processNo },
                        new SqlParameter("@DealtMethod", SqlDbType.NVarChar, 500)
                        {
                            Value = (object)dealtMethod ?? DBNull.Value
                        }
                    }
                },
                AlCOM = new ArrayList { CommandType.Text }
            };
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
                    ModuleId = row.IsNull("moduleid") ? null : Convert.ToString(row["moduleid"]),
                    PartNo = row.IsNull("partno") ? null : Convert.ToString(row["partno"]),
                    QRCode = row.IsNull("qrCode") ? null : Convert.ToString(row["qrCode"]),
                    CreateDate = row.IsNull("createdate") ? (DateTime?)null : Convert.ToDateTime(row["createdate"]),
                    ResultValue = row.IsNull("resultvalue") ? (double?)null : Convert.ToDouble(row["resultvalue"]),
                    Result = row.IsNull("result") ? 1 : Convert.ToInt32(row["result"]),
                    Deal = row.IsNull("deal") ? 0 : Convert.ToInt32(row["deal"]),
                    DealtMethod = row.IsNull("dealtmethod") ? null : Convert.ToString(row["dealtmethod"])
                });
            }

            return result;
        }
    }
}
