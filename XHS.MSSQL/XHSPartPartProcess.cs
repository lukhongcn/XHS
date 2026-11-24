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
                "partprocess.processid,process.processname,partprocess.processorder," +
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
                "partprocess.processid,process.processname,partprocess.processorder," +
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
                "partprocess.customerProcessid,partprocess.processneedminutes,partprocess.totalminutes," +
                "partprocess.needproduct,partprocess.unnormalreasonsid,partprocess.departmentid," +
                "partprocess.isunnormal,partprocess.processcomment,partprocess.processmachineid," +
                "process.pricetype,partprocess.supplyid,partprocess.relationpartno,partprocess.relationpartnoid," +
                "partprocess.startdate,partprocess.enddate," +
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

        public XHSPartPartProcessInfo GetBarcodePartProcess(
            string partNoId,
            string processId,
            string statusId)
        {
            string queryString =
                "select top 1 partprocess.id,partprocess.partno_id,partprocess.processno,partprocess.statusid " +
                "from tb_partprocess partprocess " +
                "where partprocess.partno_id=@PartNoId " +
                "and partprocess.processid=@ProcessId " +
                "and partprocess.statusid=@StatusId " +
                "order by partprocess.processorder,partprocess.processno";

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

            return _getBarcodePartProcessInfo(queryString, parameters);
        }

        public ModuleWorkFlow.Model.ParamterInfo UpdatePartProcessFromModifyEdit(
            List<PartPartProcessInfo> source)
        {
            const string queryString = @"
if @ProcessNo = 0
begin
    insert into tb_partprocess
    (ModuleId, PartNo, ProcessId, PartNo_Id, StatusId, ProcessNeedMinutes,
     ProcessOrder, priority, StartDate, SelfStartDate, SelfEndDate, isConfirm,
     processComment, processMachineid, isUnnormal, unNormalReasonsId, departMentId,
     SameProcessNo, DiscardCount)
    values
    (@ModuleId, @PartNo, @ProcessId, @PartNo_Id, @StatusId, @ProcessNeedMinutes,
     @ProcessOrder, @priority, @StartDate, @SelfStartDate, @SelfEndDate, @isConfirm,
     @processComment, @processMachineid, @isUnnormal, @unNormalReasonsId, @departMentId,
     @SameProcessNo, @DiscardCount);

    declare @NewPartProcessId int;
    set @NewPartProcessId = convert(int, scope_identity());
    update tb_partprocess set ProcessNo = @NewPartProcessId where id = @NewPartProcessId;
end
else
begin
    update tb_partprocess set
        ProcessId = @ProcessId,
        StatusId = @StatusId,
        ProcessNeedMinutes = @ProcessNeedMinutes,
        ProcessOrder = @ProcessOrder,
        priority = @priority,
        StartDate = @StartDate,
        SelfStartDate = @SelfStartDate,
        SelfEndDate = @SelfEndDate,
        isConfirm = @isConfirm,
        processComment = @processComment,
        processMachineid = @processMachineid,
        isUnnormal = @isUnnormal,
        unNormalReasonsId = @unNormalReasonsId,
        departMentId = @departMentId,
        DiscardCount = @DiscardCount,
        PartNo_Id = @PartNo_Id,
        SameProcessNo = case
            when @SameProcessNo is null or @SameProcessNo=N'' then SameProcessNo
            else @SameProcessNo
        end
    where ModuleId = @ModuleId and ProcessNo = @ProcessNo;
end";

            ModuleWorkFlow.Model.ParamterInfo parameterInfo =
                new ModuleWorkFlow.Model.ParamterInfo
                {
                    Sql = queryString,
                    Type = CommandType.Text,
                    AlSQL = new ArrayList(),
                    AlPAR = new ArrayList(),
                    AlCOM = new ArrayList()
                };

            foreach (PartPartProcessInfo process in source ?? new List<PartPartProcessInfo>())
            {
                if (process == null)
                    continue;

                parameterInfo.AlSQL.Add(queryString);
                parameterInfo.AlPAR.Add(BuildPartProcessParameters(process));
                parameterInfo.AlCOM.Add(CommandType.Text);
            }

            return parameterInfo;
        }

        public ModuleWorkFlow.Model.ParamterInfo updateDiscount(string partNoId)
        {
            const string queryString =
                "update tb_partprocess set DiscardCount=1 " +
                "where PartNo_Id=@PartNo_Id";
            return new ModuleWorkFlow.Model.ParamterInfo
            {
                Sql = queryString,
                Type = CommandType.Text,
                AlSQL = new ArrayList { queryString },
                AlPAR = new ArrayList
                {
                    new[]
                    {
                        new SqlParameter("@PartNo_Id", SqlDbType.NVarChar, 100)
                        {
                            Value = (object)(partNoId ?? string.Empty).Trim()
                        }
                    }
                },
                AlCOM = new ArrayList { CommandType.Text }
            };
        }

        public ModuleWorkFlow.Model.ParamterInfo UpdatePartNoId(
            string partNoId,
            string replacementPartNoId)
        {
            const string queryString =
                "update tb_partprocess set PartNo_Id=@ReplacementPartNo_Id, " +
                "SameProcessNo=@PartNo_Id + ',' " +
                "where PartNo_Id=@PartNo_Id";
            return new ModuleWorkFlow.Model.ParamterInfo
            {
                Sql = queryString,
                Type = CommandType.Text,
                AlSQL = new ArrayList { queryString },
                AlPAR = new ArrayList
                {
                    new[]
                    {
                        new SqlParameter("@PartNo_Id", SqlDbType.NVarChar, 100)
                        {
                            Value = (object)(partNoId ?? string.Empty).Trim()
                        },
                        new SqlParameter("@ReplacementPartNo_Id", SqlDbType.NVarChar, 100)
                        {
                            Value = (object)(replacementPartNoId ?? string.Empty).Trim()
                        }
                    }
                },
                AlCOM = new ArrayList { CommandType.Text }
            };
        }

        private static SqlParameter[] BuildPartProcessParameters(
            PartPartProcessInfo process)
        {
            return new[]
            {
                new SqlParameter("@ProcessId", SqlDbType.NVarChar, 50) { Value = ToDbValue(process.ProcessId) },
                new SqlParameter("@StatusId", SqlDbType.NVarChar, 50) { Value = ToDbValue(process.StatusId) },
                new SqlParameter("@ProcessNeedMinutes", SqlDbType.Int) { Value = process.ProcessNeedMinutes },
                new SqlParameter("@ProcessOrder", SqlDbType.Int) { Value = process.ProcessOrder },
                new SqlParameter("@priority", SqlDbType.Int) { Value = process.Priority },
                new SqlParameter("@StartDate", SqlDbType.DateTime) { Value = ToDbDate(process.StartDate) },
                new SqlParameter("@SelfStartDate", SqlDbType.DateTime)
                {
                    Value = process.isConfirm == 1 ? ToDbDate(process.SelfStartDate) : DBNull.Value
                },
                new SqlParameter("@SelfEndDate", SqlDbType.DateTime)
                {
                    Value = process.isConfirm == 1 ? ToDbDate(process.SelfEndDate) : DBNull.Value
                },
                new SqlParameter("@isConfirm", SqlDbType.Int) { Value = process.isConfirm },
                new SqlParameter("@processComment", SqlDbType.NVarChar) { Value = ToDbValue(process.ProcessComment) },
                new SqlParameter("@processMachineid", SqlDbType.NVarChar) { Value = ToDbValue(process.ProcessMachineId) },
                new SqlParameter("@isUnnormal", SqlDbType.Int) { Value = process.IsUnnormal },
                new SqlParameter("@unNormalReasonsId", SqlDbType.Int) { Value = process.UnNormalReasonsId },
                new SqlParameter("@departMentId", SqlDbType.Int) { Value = process.DepartMentId },
                new SqlParameter("@ModuleId", SqlDbType.NVarChar, 50) { Value = ToDbValue(process.ModuleId) },
                new SqlParameter("@PartNo_Id", SqlDbType.NVarChar, 50) { Value = ToDbValue(process.PartNo_Id) },
                new SqlParameter("@ProcessNo", SqlDbType.Int) { Value = process.ProcessNo },
                new SqlParameter("@PartNo", SqlDbType.NVarChar, 50) { Value = ToDbValue(process.PartNo) },
                new SqlParameter("@SameProcessNo", SqlDbType.NVarChar, 500) { Value = ToDbValue(process.SameProcessNo) },
                new SqlParameter("@DiscardCount", SqlDbType.Int) { Value = process.DiscardCount }
            };
        }

        private static object ToDbDate(DateTime value)
        {
            return value == DateTime.MinValue ? (object)DBNull.Value : value;
        }

        private static object ToDbValue(object value)
        {
            return value ?? DBNull.Value;
        }

        private XHSPartPartProcessInfo _getBarcodePartProcessInfo(
            string querystring,
            SqlParameter[] pars)
        {
            DataSet dataSet = pars == null
                ? Data.getDataSet(querystring)
                : Data.getDataSet(querystring, pars);

            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                return null;
            }

            if (dataSet.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            DataRow row = dataSet.Tables[0].Rows[0];
            return new XHSPartPartProcessInfo
            {
                Id = row.IsNull("id") ? (int?)null : Convert.ToInt32(row["id"]),
                PartNo_Id = row.IsNull("partno_id") ? null : Convert.ToString(row["partno_id"]),
                ProcessNo = row.IsNull("processno") ? (int?)null : Convert.ToInt32(row["processno"]),
                StatusId = row.IsNull("statusid") ? null : Convert.ToString(row["statusid"])
            };
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
                    ProcessOrder = GetNullableInt(row, "processorder"),
                    ListOrder = GetNullableInt(row, "processorder"),
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

        private static int? GetNullableInt(DataRow row, string columnName)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(columnName) || row.IsNull(columnName))
            {
                return null;
            }

            return Convert.ToInt32(row[columnName]);
        }
    }
}
