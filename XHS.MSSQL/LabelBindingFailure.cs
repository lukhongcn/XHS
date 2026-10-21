using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Utility;
using XHS.IDAL;
using XHS.Model;

namespace XHS.MSSQL
{
    public class LabelBindingFailure : ILabelBindingFailure
    {
        public bool Insert(LabelBindingPdaScanFailureInfo info)
        {
            if (info == null) throw new ArgumentNullException("info");

            const string sql = @"
insert into tb_LabelBindingPdaScanFailure
    (BindingTaskId, CustomerRawCode, ScanRawCode, ScanStage, FailureType, FailureReason,
     ParsedBarcodeType, RuleId, RuleName, ParsedPartNo, ParsedBatchNo, ParsedQty, OperatorName, DeviceInfo,
     ClientIp, ScanTime, IsProcessed)
values
    (@BindingTaskId, @CustomerRawCode, @ScanRawCode, @ScanStage, @FailureType, @FailureReason,
     @ParsedBarcodeType, @RuleId, @RuleName, @ParsedPartNo, @ParsedBatchNo, @ParsedQty, @OperatorName, @DeviceInfo,
     @ClientIp, @ScanTime, @IsProcessed)";

            using (SqlConnection connection = new SqlConnection(Data.WriteConnectionStr()))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add(new SqlParameter("@BindingTaskId", SqlDbType.NVarChar, 36) { Value = ToDb(info.BindingTaskId) });
                command.Parameters.Add(new SqlParameter("@CustomerRawCode", SqlDbType.NVarChar, 1000) { Value = ToDb(info.CustomerRawCode) });
                command.Parameters.Add(new SqlParameter("@ScanRawCode", SqlDbType.NVarChar, 1000) { Value = ToDb(info.ScanRawCode) });
                command.Parameters.Add(new SqlParameter("@ScanStage", SqlDbType.NVarChar, 20) { Value = ToDb(info.ScanStage) });
                command.Parameters.Add(new SqlParameter("@FailureType", SqlDbType.NVarChar, 50) { Value = ToDb(info.FailureType) });
                command.Parameters.Add(new SqlParameter("@FailureReason", SqlDbType.NVarChar, 1000) { Value = ToDb(info.FailureReason) });
                command.Parameters.Add(new SqlParameter("@ParsedBarcodeType", SqlDbType.NVarChar, 50) { Value = ToDb(info.ParsedBarcodeType) });
                command.Parameters.Add(new SqlParameter("@RuleId", SqlDbType.Int) { Value = ToDb(info.RuleId) });
                command.Parameters.Add(new SqlParameter("@RuleName", SqlDbType.NVarChar, 200) { Value = ToDb(info.RuleName) });
                command.Parameters.Add(new SqlParameter("@ParsedPartNo", SqlDbType.NVarChar, 100) { Value = ToDb(info.ParsedPartNo) });
                command.Parameters.Add(new SqlParameter("@ParsedBatchNo", SqlDbType.NVarChar, 100) { Value = ToDb(info.ParsedBatchNo) });
                SqlParameter qty = new SqlParameter("@ParsedQty", SqlDbType.Decimal) { Precision = 18, Scale = 4, Value = ToDb(info.ParsedQty) };
                command.Parameters.Add(qty);
                command.Parameters.Add(new SqlParameter("@OperatorName", SqlDbType.NVarChar, 100) { Value = ToDb(info.OperatorName) });
                command.Parameters.Add(new SqlParameter("@DeviceInfo", SqlDbType.NVarChar, 500) { Value = ToDb(info.DeviceInfo) });
                command.Parameters.Add(new SqlParameter("@ClientIp", SqlDbType.NVarChar, 64) { Value = ToDb(info.ClientIp) });
                command.Parameters.Add(new SqlParameter("@ScanTime", SqlDbType.DateTime) { Value = ToDb(info.ScanTime) });
                command.Parameters.Add(new SqlParameter("@IsProcessed", SqlDbType.Bit) { Value = info.IsProcessed });
                connection.Open();
                return command.ExecuteNonQuery() == 1;
            }
        }

        public List<LabelBindingPdaScanFailureInfo> GetRecords(
            string scanRawCode,
            string failureType,
            DateTime startTime,
            DateTime endTime)
        {
            const string sql = @"
select failure.FailureId, failure.BindingTaskId, failure.CustomerRawCode,
       failure.ScanRawCode, failure.ScanStage,
       coalesce(
           (
               select top 1 step.StepName
               from tb_ScanFlow flow
               inner join tb_ScanFlowStep step on step.FlowId = flow.FlowId
               where flow.FlowCode = N'LABEL_BINDING'
                 and flow.Enabled = 1
                 and step.StepCode = failure.ScanStage
               order by step.StepNo asc
           ),
           failure.ScanStage
       ) as ScanStageName,
       failure.FailureType, failure.FailureReason, failure.ParsedBarcodeType,
       failure.ParsedPartNo, failure.ParsedBatchNo, failure.ParsedQty,
       failure.OperatorName, failure.DeviceInfo, failure.ClientIp,
       failure.ScanTime, failure.IsProcessed
from tb_LabelBindingPdaScanFailure failure
where (@ScanRawCode = N'' or failure.ScanRawCode like @ScanRawCode)
  and (@FailureType = N'' or failure.FailureType = @FailureType)
  and failure.ScanTime >= @StartTime
  and failure.ScanTime < @EndTime
order by failure.ScanTime desc, failure.FailureId desc";

            string normalizedScanRawCode = (scanRawCode ?? string.Empty).Trim();
            string normalizedFailureType = (failureType ?? string.Empty).Trim();
            SqlParameter[] parameters =
            {
                new SqlParameter("@ScanRawCode", SqlDbType.NVarChar, 1000)
                {
                    Value = normalizedScanRawCode.Length == 0 ? string.Empty : "%" + normalizedScanRawCode + "%"
                },
                new SqlParameter("@FailureType", SqlDbType.NVarChar, 50) { Value = normalizedFailureType },
                new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = startTime },
                new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = endTime }
            };

            return _getLabelBindingFailureInfo(sql, parameters);
        }

        private List<LabelBindingPdaScanFailureInfo> _getLabelBindingFailureInfo(
            string querystring,
            SqlParameter[] pars)
        {
            DataSet dataSet = pars == null
                ? Data.getDataSet(querystring)
                : Data.getDataSet(querystring, pars);
            DataTable table = dataSet.Tables[0];
            List<LabelBindingPdaScanFailureInfo> result = new List<LabelBindingPdaScanFailureInfo>();
            foreach (DataRow row in table.Rows)
            {
                result.Add(new LabelBindingPdaScanFailureInfo
                {
                    FailureId = ToNullableLong(row["FailureId"]),
                    BindingTaskId = ToNullableString(row["BindingTaskId"]),
                    CustomerRawCode = ToNullableString(row["CustomerRawCode"]),
                    ScanRawCode = ToNullableString(row["ScanRawCode"]),
                    ScanStage = ToNullableString(row["ScanStageName"]),
                    FailureType = ToNullableString(row["FailureType"]),
                    FailureReason = ToNullableString(row["FailureReason"]),
                    ParsedBarcodeType = ToNullableString(row["ParsedBarcodeType"]),
                    ParsedPartNo = ToNullableString(row["ParsedPartNo"]),
                    ParsedBatchNo = ToNullableString(row["ParsedBatchNo"]),
                    ParsedQty = ToNullableDecimal(row["ParsedQty"]),
                    OperatorName = ToNullableString(row["OperatorName"]),
                    DeviceInfo = ToNullableString(row["DeviceInfo"]),
                    ClientIp = ToNullableString(row["ClientIp"]),
                    ScanTime = ToNullableDateTime(row["ScanTime"]),
                    IsProcessed = row["IsProcessed"] != DBNull.Value && Convert.ToBoolean(row["IsProcessed"])
                });
            }
            return result;
        }

        private static string ToNullableString(object value)
        {
            return value == DBNull.Value || value == null ? null : Convert.ToString(value);
        }

        private static long? ToNullableLong(object value)
        {
            return value == DBNull.Value || value == null ? (long?)null : Convert.ToInt64(value);
        }

        private static decimal? ToNullableDecimal(object value)
        {
            return value == DBNull.Value || value == null ? (decimal?)null : Convert.ToDecimal(value);
        }

        private static DateTime? ToNullableDateTime(object value)
        {
            return value == DBNull.Value || value == null ? (DateTime?)null : Convert.ToDateTime(value);
        }

        private static object ToDb(object value) { return value ?? DBNull.Value; }
    }
}

