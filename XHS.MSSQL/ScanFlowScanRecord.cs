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
        public DataTable GetLabelBindingRecords(string flowCode, string scanContentLike, DateTime startTime, DateTime endTime)
        {
            const string sql = @"
with MatchingTasks as
(
    select distinct r.BindingTaskId
    from tb_ScanFlowScanRecord r
    inner join tb_ScanFlow f on f.FlowId=r.FlowId
    where f.FlowCode=@FlowCode
      and f.Enabled=1
      and r.ScanTime>=@StartTime
      and r.ScanTime<@EndTime
      and r.StepCode in ('CUSTOMER','WORKORDER','FACTORY')
      and (@ScanContentLike=N'%%' or r.ScanContent like @ScanContentLike)
)
select r.RecordId,r.FlowId,r.BindingTaskId,r.StepId,r.StepCode,r.SeqNo,
       r.ScanContent,r.Status,r.ScanUser,r.ScanTime,
       s.StepName,s.RuleName,s.CustomerId,s.LabelType
from tb_ScanFlowScanRecord r
inner join tb_ScanFlow f on f.FlowId=r.FlowId
inner join MatchingTasks mt on mt.BindingTaskId=r.BindingTaskId
left join tb_ScanFlowStep s on s.StepId=r.StepId
where f.FlowCode=@FlowCode
  and f.Enabled=1
  and r.StepCode in ('CUSTOMER','WORKORDER','FACTORY')
order by r.BindingTaskId,r.ScanTime,r.RecordId";

            DataSet dataSet = Data.getDataSet(sql, new[]
            {
                new SqlParameter("@FlowCode", SqlDbType.VarChar, 50) { Value = flowCode.Trim() },
                new SqlParameter("@ScanContentLike", SqlDbType.NVarChar, 1100) { Value = scanContentLike ?? "%%" },
                new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = startTime },
                new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = endTime }
            });

            return dataSet != null && dataSet.Tables.Count > 0 ? dataSet.Tables[0] : new DataTable();
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
            const string sql = "insert into tb_ScanFlowScanRecord (FlowId,FlowCode,BindingTaskId,StepId,StepCode,SeqNo,ScanContent,Status,ScanUser,DeviceInfo,ClientIp,ScanTime) values (@FlowId,@FlowCode,@BindingTaskId,@StepId,@StepCode,@SeqNo,@ScanContent,@Status,@ScanUser,@DeviceInfo,@ClientIp,@ScanTime)";
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
                new SqlParameter("@FlowCode", SqlDbType.VarChar, 50) { Value = ToDb(info.FlowCode) },
                new SqlParameter("@BindingTaskId", SqlDbType.NVarChar, 36) { Value = ToDb(info.BindingTaskId) },
                new SqlParameter("@StepId", SqlDbType.Int) { Value = ToDb(info.StepId) },
                new SqlParameter("@StepCode", SqlDbType.VarChar, 50) { Value = ToDb(info.StepCode) },
                new SqlParameter("@SeqNo", SqlDbType.Int) { Value = ToDb(info.SeqNo) },
                new SqlParameter("@ScanContent", SqlDbType.NVarChar, 1000) { Value = ToDb(info.ScanContent) },
                new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = ToDb(info.Status) },
                new SqlParameter("@ScanUser", SqlDbType.NVarChar, 100) { Value = ToDb(info.ScanUser) },
                new SqlParameter("@DeviceInfo", SqlDbType.NVarChar, 500) { Value = ToDb(info.DeviceInfo) },
                new SqlParameter("@ClientIp", SqlDbType.NVarChar, 64) { Value = ToDb(info.ClientIp) },
                new SqlParameter("@ScanTime", SqlDbType.DateTime) { Value = ToDb(info.ScanTime) }
            };
        }

        private static object ToDb(object value) { return value ?? DBNull.Value; }
    }
}
