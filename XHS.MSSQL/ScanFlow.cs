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
    /// 扫描流程 SQL Server 数据访问实现。
    /// </summary>
    public class ScanFlow : IScanFlow
    {
        private const string SelectColumns = "FlowId,FlowCode,FlowName,CustomerId,Enabled";

        public List<ScanFlowInfo> GetByFlowName(string flowName)
        {
            const string queryString = "select " + SelectColumns + " from tb_ScanFlow where FlowName=@FlowName and Enabled=1 order by FlowId asc";
            SqlParameter[] parameters =
            {
                new SqlParameter("@FlowName", SqlDbType.NVarChar, 100) { Value = flowName.Trim() }
            };
            return _getScanFlowInfo(queryString, parameters);
        }

        private List<ScanFlowInfo> _getScanFlowInfo(string querystring, SqlParameter[] pars)
        {
            DataSet dataSet;
            if (pars != null)
            {
                dataSet = Data.getDataSet(querystring, pars);
            }
            else
            {
                dataSet = Data.getDataSet(querystring);
            }

            List<ScanFlowInfo> result = new List<ScanFlowInfo>();
            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                return result;
            }

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                result.Add(new ScanFlowInfo
                {
                    FlowId = row.IsNull("FlowId") ? (int?)null : Convert.ToInt32(row["FlowId"]),
                    FlowCode = row.IsNull("FlowCode") ? null : Convert.ToString(row["FlowCode"]),
                    FlowName = row.IsNull("FlowName") ? null : Convert.ToString(row["FlowName"]),
                    CustomerId = row.IsNull("CustomerId") ? null : Convert.ToString(row["CustomerId"]),
                    Enabled = row.IsNull("Enabled") ? (bool?)null : Convert.ToBoolean(row["Enabled"])
                });
            }

            return result;
        }
    }
}
