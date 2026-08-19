using System;
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
     ParsedBarcodeType, ParsedPartNo, ParsedBatchNo, ParsedQty, OperatorName, DeviceInfo,
     ClientIp, ScanTime, IsProcessed)
values
    (@BindingTaskId, @CustomerRawCode, @ScanRawCode, @ScanStage, @FailureType, @FailureReason,
     @ParsedBarcodeType, @ParsedPartNo, @ParsedBatchNo, @ParsedQty, @OperatorName, @DeviceInfo,
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

        private static object ToDb(object value) { return value ?? DBNull.Value; }
    }
}
