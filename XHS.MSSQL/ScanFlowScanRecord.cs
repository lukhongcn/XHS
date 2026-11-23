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
    public class ScanFlowScanRecord : IScanFlowScanRecord
    {
        public List<ScanFlowScanRecordInfo> GetBindingScanRecordsByTaskId(string bindingTaskId)
        {
            const string sql = @"
select RecordId,FlowId,ScanName,BindingTaskId,StepId,StepCode,SeqNo,
       ScanContent,RuleId,RuleName,BarcodeType,Status,ScanUser,
       DeviceInfo,ClientIp,ScanTime
from tb_ScanFlowScanRecord
where BindingTaskId=@BindingTaskId
  and StepCode in ('CUSTOMER','WORKORDER','FACTORY')
order by ScanTime desc,RecordId desc";

            return _getScanRecordBySql(sql, new[]
            {
                new SqlParameter("@BindingTaskId", SqlDbType.NVarChar, 36) { Value = bindingTaskId.Trim() }
            });
        }

        public List<ScanFlowScanRecordInfo> GetLabelBindingRecords(string flowCode, string scanContentLike, DateTime startTime, DateTime endTime)
        {
            string queryString = @"
select r.RecordId,r.FlowId,r.ScanName,r.BindingTaskId,r.StepId,r.StepCode,r.SeqNo,
       r.ScanContent,r.RuleId,coalesce(r.RuleName,s.RuleName) as RuleName,r.BarcodeType,r.Status,r.ScanUser,
       r.DeviceInfo,r.ClientIp,r.ScanTime,
       s.StepName,s.CustomerId,s.LabelType
from tb_ScanFlowScanRecord r
inner join tb_ScanFlow f on f.FlowId=r.FlowId
inner join tb_ScanFlowStep s on s.StepId=r.StepId
where f.FlowCode=@FlowCode
  and f.Enabled=1
  and r.ScanTime>=@StartTime
  and r.ScanTime<@EndTime
  and r.StepCode in ('CUSTOMER','WORKORDER','FACTORY')
  and (@ScanContentLike=N'%%' or r.ScanContent like @ScanContentLike)
order by r.BindingTaskId,r.ScanTime,r.RecordId";

            SqlParameter[] pars = new[]
            {
                new SqlParameter("@FlowCode", SqlDbType.VarChar, 50) { Value = flowCode.Trim() },
                new SqlParameter("@ScanContentLike", SqlDbType.NVarChar, 1100) { Value = scanContentLike ?? "%%" },
                new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = startTime },
                new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = endTime }
            };
            return _getScanRecordBySql(queryString, pars);
        }

        public bool IsFactoryBarcodeRecorded(int flowId, string scanContent)
        {
            const string sql = "select top 1 RecordId from tb_ScanFlowScanRecord where FlowId=@FlowId and ScanContent=@ScanContent and StepCode='FACTORY' and Status=N'已完成'";
            DataSet dataSet = Data.getDataSet(sql, new[]
            {
                new SqlParameter("@FlowId", SqlDbType.Int) { Value = flowId },
                new SqlParameter("@ScanContent", SqlDbType.NVarChar, 1000) { Value = scanContent }
            });
            return dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0;
        }

        public ParamterInfo InsertRecords(List<ScanFlowScanRecordInfo> infos)
        {
            const string sql = "insert into tb_ScanFlowScanRecord (FlowId,ScanName,BindingTaskId,StepId,StepCode,SeqNo,ScanContent,RuleId,RuleName,BarcodeType,Status,ScanUser,DeviceInfo,ClientIp,ScanTime) values (@FlowId,@ScanName,@BindingTaskId,@StepId,@StepCode,@SeqNo,@ScanContent,@RuleId,@RuleName,@BarcodeType,@Status,@ScanUser,@DeviceInfo,@ClientIp,@ScanTime)";
            ParamterInfo result = new ParamterInfo { Sql = sql, Type = CommandType.Text, AlSQL = new ArrayList(), AlPAR = new ArrayList(), AlCOM = new ArrayList() };
            if (infos == null) return result;

            foreach (ScanFlowScanRecordInfo info in infos)
            {
                if (info == null) continue;
                result.AlSQL.Add(sql);
                result.AlPAR.Add(BuildParameters(info));
                result.AlCOM.Add(CommandType.Text);
            }
            return result;
        }

        private static SqlParameter[] BuildParameters(ScanFlowScanRecordInfo info)
        {
            return new[]
            {
                new SqlParameter("@FlowId", SqlDbType.Int) { Value = ToDb(info.FlowId) },
                new SqlParameter("@ScanName", SqlDbType.NVarChar, 200) { Value = ToDb(info.FlowCode) },
                new SqlParameter("@BindingTaskId", SqlDbType.NVarChar, 36) { Value = ToDb(info.BindingTaskId) },
                new SqlParameter("@StepId", SqlDbType.Int) { Value = ToDb(info.StepId) },
                new SqlParameter("@StepCode", SqlDbType.VarChar, 50) { Value = ToDb(info.StepCode) },
                new SqlParameter("@SeqNo", SqlDbType.Int) { Value = ToDb(info.SeqNo) },
                new SqlParameter("@ScanContent", SqlDbType.NVarChar, 1000) { Value = ToDb(info.ScanContent) },
                new SqlParameter("@RuleId", SqlDbType.Int) { Value = ToDb(info.RuleId) },
                new SqlParameter("@RuleName", SqlDbType.NVarChar, 200) { Value = ToDb(info.RuleName) },
                new SqlParameter("@BarcodeType", SqlDbType.NVarChar, 50) { Value = ToDb(info.BarcodeType) },
                new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = ToDb(info.Status) },
                new SqlParameter("@ScanUser", SqlDbType.NVarChar, 100) { Value = ToDb(info.ScanUser) },
                new SqlParameter("@DeviceInfo", SqlDbType.NVarChar, 500) { Value = ToDb(info.DeviceInfo) },
                new SqlParameter("@ClientIp", SqlDbType.NVarChar, 64) { Value = ToDb(info.ClientIp) },
                new SqlParameter("@ScanTime", SqlDbType.DateTime) { Value = ToDb(info.ScanTime) }
            };
        }

        private static object ToDb(object value) { return value ?? DBNull.Value; }

        private static List<ScanFlowScanRecordInfo> _getScanRecordBySql(string queryString, SqlParameter[] pars)
        {
            DataSet dataSet;
            if (pars != null)
            {
                dataSet = Data.getDataSet(queryString, pars);
            }
            else
            {
                dataSet = Data.getDataSet(queryString);
            }

            List<ScanFlowScanRecordInfo> result = new List<ScanFlowScanRecordInfo>();
            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                return result;
            }

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                result.Add(new ScanFlowScanRecordInfo
                {
                    RecordId = ReadNullableLong(row, "RecordId"),
                    FlowId = ReadNullableInt(row, "FlowId"),
                    FlowCode = ReadString(row, "ScanName"),
                    BindingTaskId = ReadString(row, "BindingTaskId"),
                    StepId = ReadNullableInt(row, "StepId"),
                    StepCode = ReadString(row, "StepCode"),
                    StepName = ReadString(row, "StepName"),
                    CustomerId = ReadString(row, "CustomerId"),
                    LabelType = ReadString(row, "LabelType"),
                    SeqNo = ReadNullableInt(row, "SeqNo"),
                    ScanContent = ReadString(row, "ScanContent"),
                    RuleId = ReadNullableInt(row, "RuleId"),
                    RuleName = ReadString(row, "RuleName"),
                    BarcodeType = ReadString(row, "BarcodeType"),
                    Status = ReadString(row, "Status"),
                    ScanUser = ReadString(row, "ScanUser"),
                    DeviceInfo = ReadString(row, "DeviceInfo"),
                    ClientIp = ReadString(row, "ClientIp"),
                    ScanTime = row.IsNull("ScanTime") ? (DateTime?)null : Convert.ToDateTime(row["ScanTime"])
                });
            }
            return result;
        }

        private static string ReadString(DataRow row, string columnName)
        {
            return row == null || row.Table == null || !row.Table.Columns.Contains(columnName) || row.IsNull(columnName)
                ? null
                : Convert.ToString(row[columnName]);
        }

        private static int? ReadNullableInt(DataRow row, string columnName)
        {
            string value = ReadString(row, columnName);
            int result;
            return int.TryParse(value, out result) ? (int?)result : null;
        }

        private static long? ReadNullableLong(DataRow row, string columnName)
        {
            string value = ReadString(row, columnName);
            long result;
            return long.TryParse(value, out result) ? (long?)result : null;
        }
    }
}

