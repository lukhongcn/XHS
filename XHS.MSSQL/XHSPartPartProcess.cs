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
    /// 零件工序 SQL Server 数据访问实现。
    /// </summary>
    public class XHSPartPartProcess : IXHSPartPartProcess
    {
        public List<XHSPartPartProcessInfo> GetPartProcesses(
            string customerAbbr,
            string partNo,
            string barcode,
            IList<string> qrDateCodes)
        {
            string queryString =
                "select partprocess.id,partprocess.moduleid,partprocess.partno,partprocess.partno_id," +
                "partprocess.processid,process.processname,partprocess.processorder,partprocess.statusid," +
                "status.StatusDescription statusname," +
                "partprocess.processneedminutes,partprocess.startdate,partprocess.enddate," +
                "partprocess.factstartdate,partprocess.factenddate " +
                "from tb_partprocess partprocess " +
                "inner join tb_process process on partprocess.processid=process.processid " +
                "inner join tb_status status on partprocess.statusid=status.statusid " +
                "where partprocess.moduleid=@CustomerAbbr " +
                "and (@PartNo=N'' or partprocess.partno=@PartNo) " +
                "and (@Barcode=N'' or partprocess.partno_id like @Barcode) ";

            queryString += "and partprocess.statusid in (N'Ready',N'Pending') ";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@CustomerAbbr", SqlDbType.NVarChar, 100)
                {
                    Value = (object)(customerAbbr ?? string.Empty).Trim()
                },
                new SqlParameter("@PartNo", SqlDbType.NVarChar, 100)
                {
                    Value = (object)(partNo ?? string.Empty).Trim()
                },
                new SqlParameter("@Barcode", SqlDbType.NVarChar, 500)
                {
                    Value = string.IsNullOrWhiteSpace(barcode)
                        ? (object)string.Empty
                        : (object)("%" + barcode.Trim() + "%")
                }
            };

            List<string> codes = new List<string>();
            if (qrDateCodes != null)
            {
                foreach (string code in qrDateCodes)
                {
                    if (!string.IsNullOrWhiteSpace(code) && !codes.Contains(code.Trim()))
                    {
                        codes.Add(code.Trim());
                    }
                }
            }

            if (codes.Count > 0)
            {
                queryString += "and (";
                for (int i = 0; i < codes.Count; i++)
                {
                    if (i > 0)
                    {
                        queryString += " or ";
                    }

                    string parameterName = "@QrDate" + i;
                    queryString += "partprocess.partno_id like " + parameterName;
                    parameters.Add(new SqlParameter(parameterName, SqlDbType.NVarChar, 20)
                    {
                        Value = "%" + codes[i] + "%"
                    });
                }

                queryString += ") ";
            }

            queryString += "order by partno_id,processorder,id";
            return _getXHSPartPartProcessInfo(queryString, parameters.ToArray());
        }

        private List<XHSPartPartProcessInfo> _getXHSPartPartProcessInfo(
            string querystring,
            SqlParameter[] pars)
        {
            DataSet dataSet = pars == null
                ? Data.getDataSet(querystring)
                : Data.getDataSet(querystring, pars);

            List<XHSPartPartProcessInfo> result = new List<XHSPartPartProcessInfo>();
            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                return result;
            }

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                result.Add(new XHSPartPartProcessInfo
                {
                    Id = row.IsNull("id") ? (int?)null : Convert.ToInt32(row["id"]),
                    ModuleId = row.IsNull("moduleid") ? null : Convert.ToString(row["moduleid"]),
                    PartNo = row.IsNull("partno") ? null : Convert.ToString(row["partno"]),
                    PartNo_Id = row.IsNull("partno_id") ? null : Convert.ToString(row["partno_id"]),
                    ProcessId = row.IsNull("processid") ? null : Convert.ToString(row["processid"]),
                    ProcessName = row.IsNull("processname") ? null : Convert.ToString(row["processname"]),
                    ProcessOrder = row.IsNull("processorder") ? (int?)null : Convert.ToInt32(row["processorder"]),
                    StatusId = row.IsNull("statusid") ? null : Convert.ToString(row["statusid"]),
                    StatusName = row.IsNull("statusname") ? null : Convert.ToString(row["statusname"]),
                    ProcessNeedMinutes = row.IsNull("processneedminutes") ? (int?)null : Convert.ToInt32(row["processneedminutes"]),
                    StartDate = row.IsNull("startdate") ? (DateTime?)null : Convert.ToDateTime(row["startdate"]),
                    EndDate = row.IsNull("enddate") ? (DateTime?)null : Convert.ToDateTime(row["enddate"]),
                    FactStartDate = row.IsNull("factstartdate") ? (DateTime?)null : Convert.ToDateTime(row["factstartdate"]),
                    FactEndDate = row.IsNull("factenddate") ? (DateTime?)null : Convert.ToDateTime(row["factenddate"])
                });
            }

            return result;
        }
    }
}
