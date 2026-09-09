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
    /// 扫描流程步骤 SQL Server 数据访问实现。
    /// </summary>
    public class ScanFlowStep : IScanFlowStep
    {
        private const string SelectColumns = "StepId,FlowId,StepNo,StepCode,StepName,ScanType,RuleName,CustomerId,LabelType,AllowRepeat,EndControlId,EndCompareControlId";

        public List<ScanFlowStepInfo> GetStepsByFlowId(int flowId)
        {
            const string sql = "select " + SelectColumns + " from tb_ScanFlowStep where FlowId=@FlowId order by StepNo asc";
            return _getScanFlowStepInfo(sql, new[] { new SqlParameter("@FlowId", SqlDbType.Int) { Value = flowId } });
        }

        public List<ScanFlowStepInfo> GetStepsByFlowName(string flowName)
        {
            string queryString = "select s.StepId,s.FlowId,s.StepNo,s.StepCode,s.StepName,s.ScanType,s.RuleName,s.CustomerId,s.LabelType,s.AllowRepeat,s.EndControlId,s.EndCompareControlId from tb_ScanFlow f inner join tb_ScanFlowStep s on f.FlowId=s.FlowId where f.FlowName=@FlowName and f.Enabled=1 order by s.StepNo asc";
            return _getScanFlowStepInfo(queryString, new[]
            {
                new SqlParameter("@FlowName", SqlDbType.NVarChar, 100) { Value = flowName.Trim() }
            });
        }

        public List<ScanFlowStepInfo> GetStepsByFlowCode(string flowCode)
        {
            string queryString = "select s.StepId,s.FlowId,s.StepNo,s.StepCode,s.StepName,s.ScanType,s.RuleName,s.CustomerId,s.LabelType,s.AllowRepeat,s.EndControlId,s.EndCompareControlId from tb_ScanFlow f inner join tb_ScanFlowStep s on f.FlowId=s.FlowId where f.FlowCode=@FlowCode and f.Enabled=1 order by s.StepNo asc";
            return _getScanFlowStepInfo(queryString, new[]
            {
                new SqlParameter("@FlowCode", SqlDbType.VarChar, 50) { Value = flowCode.Trim() }
            });
        }

        public ParamterInfo InsertSteps(List<ScanFlowStepInfo> infos)
        {
            const string sql = "insert into tb_ScanFlowStep (FlowId,StepNo,StepCode,StepName,ScanType,RuleName,CustomerId,LabelType,AllowRepeat,EndControlId,EndCompareControlId) values (@FlowId,@StepNo,@StepCode,@StepName,@ScanType,@RuleName,@CustomerId,@LabelType,@AllowRepeat,@EndControlId,@EndCompareControlId)";
            return BuildParamterInfo(infos, sql, BuildInsertParameters);
        }

        public ParamterInfo UpdateSteps(List<ScanFlowStepInfo> infos)
        {
            const string sql = "update tb_ScanFlowStep set FlowId=@FlowId,StepNo=@StepNo,StepCode=@StepCode,StepName=@StepName,ScanType=@ScanType,RuleName=@RuleName,CustomerId=@CustomerId,LabelType=@LabelType,AllowRepeat=@AllowRepeat,EndControlId=@EndControlId,EndCompareControlId=@EndCompareControlId where StepId=@StepId";
            return BuildParamterInfo(infos, sql, BuildUpdateParameters);
        }

        public ParamterInfo DeleteSteps(List<ScanFlowStepInfo> infos)
        {
            const string sql = "delete from tb_ScanFlowStep where StepId=@StepId";
            return BuildParamterInfo(infos, sql, info => new[] { new SqlParameter("@StepId", SqlDbType.Int) { Value = ToDbValue(info.StepId) } });
        }

        private static List<ScanFlowStepInfo> _getScanFlowStepInfo(string sql, SqlParameter[] parameters)
        {
            DataSet dataSet = Data.getDataSet(sql, parameters);
            List<ScanFlowStepInfo> result = new List<ScanFlowStepInfo>();
            if (dataSet == null || dataSet.Tables.Count == 0) return result;
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                result.Add(new ScanFlowStepInfo
                {
                    StepId = row.IsNull("StepId") ? (int?)null : Convert.ToInt32(row["StepId"]),
                    FlowId = row.IsNull("FlowId") ? (int?)null : Convert.ToInt32(row["FlowId"]),
                    StepNo = row.IsNull("StepNo") ? (int?)null : Convert.ToInt32(row["StepNo"]),
                    StepCode = ReadString(row, "StepCode"), StepName = ReadString(row, "StepName"),
                    ScanType = ReadString(row, "ScanType"), RuleName = ReadString(row, "RuleName"),
                    CustomerId = ReadString(row, "CustomerId"), LabelType = ReadString(row, "LabelType"),
                    AllowRepeat = !row.IsNull("AllowRepeat") && Convert.ToBoolean(row["AllowRepeat"]),
                    EndControlId = ReadString(row, "EndControlId"), EndCompareControlId = ReadString(row, "EndCompareControlId")
                });
            }
            return result;
        }

        private static string ReadString(DataRow row, string name) { return row.IsNull(name) ? null : Convert.ToString(row[name]); }

        private static ParamterInfo BuildParamterInfo(List<ScanFlowStepInfo> infos, string sql, Func<ScanFlowStepInfo, SqlParameter[]> builder)
        {
            ParamterInfo result = new ParamterInfo { Sql = sql, Type = CommandType.Text, AlSQL = new ArrayList(), AlPAR = new ArrayList(), AlCOM = new ArrayList() };
            if (infos == null) return result;
            foreach (ScanFlowStepInfo info in infos)
            {
                if (info == null) continue;
                result.AlSQL.Add(sql); result.AlPAR.Add(builder(info)); result.AlCOM.Add(CommandType.Text);
            }
            return result;
        }

        private static SqlParameter[] BuildInsertParameters(ScanFlowStepInfo info)
        {
            return new[]
            {
                new SqlParameter("@FlowId", SqlDbType.Int) { Value = ToDbValue(info.FlowId) },
                new SqlParameter("@StepNo", SqlDbType.Int) { Value = ToDbValue(info.StepNo) },
                new SqlParameter("@StepCode", SqlDbType.VarChar, 50) { Value = ToDbValue(info.StepCode) },
                new SqlParameter("@StepName", SqlDbType.NVarChar, 100) { Value = ToDbValue(info.StepName) },
                new SqlParameter("@ScanType", SqlDbType.VarChar, 30) { Value = ToDbValue(info.ScanType) },
                new SqlParameter("@RuleName", SqlDbType.NVarChar, 150) { Value = ToDbValue(info.RuleName) },
                new SqlParameter("@CustomerId", SqlDbType.VarChar, 100) { Value = ToDbValue(info.CustomerId) },
                new SqlParameter("@LabelType", SqlDbType.VarChar, 20) { Value = ToDbValue(info.LabelType) },
                new SqlParameter("@AllowRepeat", SqlDbType.Bit) { Value = info.AllowRepeat },
                new SqlParameter("@EndControlId", SqlDbType.VarChar, 100) { Value = ToDbValue(info.EndControlId) },
                new SqlParameter("@EndCompareControlId", SqlDbType.VarChar, 100) { Value = ToDbValue(info.EndCompareControlId) }
            };
        }

        private static SqlParameter[] BuildUpdateParameters(ScanFlowStepInfo info)
        {
            List<SqlParameter> parameters = new List<SqlParameter>(BuildInsertParameters(info));
            parameters.Insert(0, new SqlParameter("@StepId", SqlDbType.Int) { Value = ToDbValue(info.StepId) });
            return parameters.ToArray();
        }

        private static object ToDbValue(object value) { return value ?? DBNull.Value; }
    }
}
