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
                "select partprocess.id,partprocess.moduleid,partprocess.partno,partprocess.partno_id,partprocess.processno," +
                "partprocess.processid,process.processname,partprocess.processorder listorder,partprocess.processorder," +
                "partprocess.customerProcessid,partprocess.statusid," +
                "status.StatusDescription statusname," +
                "partprocess.processneedminutes,partprocess.totalminutes,partprocess.needproduct," +
                "partprocess.unnormalreasonsid,partprocess.departmentid,partprocess.isunnormal," +
                "partprocess.processcomment,partprocess.processmachineid,process.pricetype,partprocess.supplyid," +
                "partprocess.relationpartno,partprocess.relationpartnoid,partprocess.startdate,partprocess.enddate," +
                "partprocess.factstartdate,partprocess.factenddate " +
                "from tb_partprocess partprocess " +
                "inner join tb_process process on partprocess.processid=process.processid " +
                "inner join tb_status status on partprocess.statusid=status.statusid " +
                "where partprocess.moduleid=@CustomerAbbr " +
                "and (@PartNo=N'' or partprocess.partno=@PartNo) " +
                "and (@Barcode=N'' or partprocess.partno_id like @Barcode) ";

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

        public List<XHSPartPartProcessInfo> GetPartProcessesByPartNoId(string partNoId)
        {
            string queryString =
                "select partprocess.id,partprocess.moduleid,partprocess.partno,partprocess.partno_id,partprocess.processno," +
                "partprocess.processid,process.processname,partprocess.processorder listorder,partprocess.processorder," +
                "partprocess.customerProcessid,partprocess.statusid," +
                "status.StatusDescription statusname," +
                "partprocess.processneedminutes,partprocess.totalminutes,partprocess.needproduct," +
                "partprocess.unnormalreasonsid,partprocess.departmentid,partprocess.isunnormal," +
                "partprocess.processcomment,partprocess.processmachineid,process.pricetype,partprocess.supplyid," +
                "partprocess.relationpartno,partprocess.relationpartnoid,partprocess.startdate,partprocess.enddate," +
                "partprocess.factstartdate,partprocess.factenddate " +
                "from tb_partprocess partprocess " +
                "left join tb_process process on partprocess.processid=process.processid " +
                "left join tb_status status on partprocess.statusid=status.statusid " +
                "where partprocess.partno_id=@PartNoId " +
                "order by partno_id,processorder,id";

            SqlParameter[] parameters =
            {
                new SqlParameter("@PartNoId", SqlDbType.NVarChar, 100)
                {
                    Value = (object)(partNoId ?? string.Empty).Trim()
                }
            };

            return _getXHSPartPartProcessInfo(queryString, parameters);
        }

        public List<XHSPartPartProcessInfo> GetPartProcessesByPartNoIdAndProcessIdAndStatusId(
            string partNoId,
            string processId,
            string statusId)
        {
            string queryString =
                "select partprocess.id,partprocess.moduleid,partprocess.partno,partprocess.partno_id,partprocess.processno," +
                "partprocess.processid,process.processname,partprocess.processorder,partprocess.statusid," +
                "status.StatusDescription statusname," +
                "partprocess.processneedminutes,partprocess.startdate,partprocess.enddate," +
                "partprocess.factstartdate,partprocess.factenddate " +
                "from tb_partprocess partprocess " +
                "inner join tb_process process on partprocess.processid=process.processid " +
                "inner join tb_status status on partprocess.statusid=status.statusid " +
                "where partprocess.partno_id=@PartNoId " +
                "and partprocess.processid=@ProcessId " +
                "and partprocess.statusid=@StatusId " +
                "order by partno_id,processorder,id";

            SqlParameter[] parameters =
            {
                new SqlParameter("@PartNoId", SqlDbType.NVarChar, 100)
                {
                    Value = (object)(partNoId ?? string.Empty).Trim()
                },
                new SqlParameter("@ProcessId", SqlDbType.NVarChar, 100)
                {
                    Value = (object)(processId ?? string.Empty).Trim()
                },
                new SqlParameter("@StatusId", SqlDbType.NVarChar, 100)
                {
                    Value = (object)(statusId ?? string.Empty).Trim()
                }
            };

            return _getXHSPartPartProcessInfo(queryString, parameters);
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
                    ProcessNo = row.IsNull("processno") ? (int?)null : Convert.ToInt32(row["processno"]),
                    ProcessId = row.IsNull("processid") ? null : Convert.ToString(row["processid"]),
                    ProcessName = row.IsNull("processname") ? null : Convert.ToString(row["processname"]),
                    ProcessOrder = row.IsNull("processorder") ? (int?)null : Convert.ToInt32(row["processorder"]),
                    ListOrder = row.IsNull("listorder") ? (int?)null : Convert.ToInt32(row["listorder"]),
                    CustomerProcessId = row.IsNull("customerProcessid") ? null : Convert.ToString(row["customerProcessid"]),
                    CustomerProcessName = row.IsNull("processname") ? null : Convert.ToString(row["processname"]),
                    StatusId = row.IsNull("statusid") ? null : Convert.ToString(row["statusid"]),
                    StatusName = row.IsNull("statusname") ? null : Convert.ToString(row["statusname"]),
                    ProcessNeedMinutes = row.IsNull("processneedminutes") ? (int?)null : Convert.ToInt32(row["processneedminutes"]),
                    TotalMinutes = row.IsNull("totalminutes") ? (int?)null : Convert.ToInt32(row["totalminutes"]),
                    NeedProduct = row.IsNull("needproduct") ? (int?)null : Convert.ToInt32(row["needproduct"]),
                    UnNormalReasonsId = row.IsNull("unnormalreasonsid") ? (int?)null : Convert.ToInt32(row["unnormalreasonsid"]),
                    DepartmentId = row.IsNull("departmentid") ? (int?)null : Convert.ToInt32(row["departmentid"]),
                    IsUnnormal = row.IsNull("isunnormal") ? (int?)null : Convert.ToInt32(row["isunnormal"]),
                    ProcessComment = row.IsNull("processcomment") ? null : Convert.ToString(row["processcomment"]),
                    ProcessMachineId = row.IsNull("processmachineid") ? null : Convert.ToString(row["processmachineid"]),
                    PriceType = row.IsNull("pricetype") ? null : Convert.ToString(row["pricetype"]),
                    SupplyId = row.IsNull("supplyid") ? (int?)null : Convert.ToInt32(row["supplyid"]),
                    RelationPartNo = row.IsNull("relationpartno") ? null : Convert.ToString(row["relationpartno"]),
                    RelationPartNoId = row.IsNull("relationpartnoid") ? null : Convert.ToString(row["relationpartnoid"]),
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
