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
            const string sql = "insert into tb_ScanFlowScanRecord (FlowId,ScanName,BindingTaskId,StepId,StepCode,SeqNo,ScanContent,Status,ScanUser,DeviceInfo,ClientIp,ScanTime) values (@FlowId,@ScanName,@BindingTaskId,@StepId,@StepCode,@SeqNo,@ScanContent,@Status,@ScanUser,@DeviceInfo,@ClientIp,@ScanTime)";
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
                new SqlParameter("@ScanName", SqlDbType.NVarChar, 100) { Value = ToDb(info.ScanName) },
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
