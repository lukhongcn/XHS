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
    /// 打印记录 SQL Server 数据访问实现。
    /// </summary>
    public class PrintRecord : IPrintRecord
    {
        private const string PrintRecordSelectColumns = "Id,SupplyBatchNo,PartNo,CartonNo,TaskId,ClientId,MachineId,PrintType,PdfUrl,PdfDownLoadUrl,PdfDownloadPath,LocalPath,Status,PrintCount,PrintUser,PrintTime,FirstPrintUser,FirstPrintTime,LastPrintUser,LastPrintTime,ReprintReason,ReprintReasonsId,CreateUser,CreateTime,UpdateUser,UpdateTime,LockTime";
        private const string PrintRecordOrderBy = " order by CreateTime desc, Id desc";

        public List<PrintRecordInfo> GetPrintRecords()
        {
            string queryString = "select " + PrintRecordSelectColumns + " from tb_PrintRecord" + PrintRecordOrderBy;
            return GetPrintRecordsBySql(queryString);
        }

        public List<PrintRecordInfo> GetPrintRecords(string supplyBatchNo, string partNo, string cartonNo, string printType)
        {
            return GetPrintRecords(supplyBatchNo, partNo, cartonNo, printType, string.Empty);
        }

        public List<PrintRecordInfo> GetPrintRecords(string supplyBatchNo, string partNo, string cartonNo, string printType, string closeStatus)
        {
            string queryString = "select distinct pr." + PrintRecordSelectColumns.Replace(",", ",pr.") + " from tb_PrintRecord pr";

            if (!string.IsNullOrWhiteSpace(closeStatus))
            {
                queryString += " inner join tb_ShippingGoods sg on pr.SupplyBatchNo=sg.SupplyBatchNo and pr.PartNo=sg.PartNo and pr.CartonNo=sg.CartonNo";
            }

            queryString += " where 1=1";

            if (!string.IsNullOrWhiteSpace(supplyBatchNo))
            {
                queryString += string.Format(" and pr.SupplyBatchNo like '%{0}%'", SafeSqlValue(supplyBatchNo));
            }

            if (!string.IsNullOrWhiteSpace(partNo))
            {
                queryString += string.Format(" and pr.PartNo like '%{0}%'", SafeSqlValue(partNo));
            }

            if (!string.IsNullOrWhiteSpace(cartonNo))
            {
                queryString += string.Format(" and pr.CartonNo like '%{0}%'", SafeSqlValue(cartonNo));
            }

            if (!string.IsNullOrWhiteSpace(printType))
            {
                queryString += string.Format(" and pr.PrintType='{0}'", SafeSqlValue(printType));
            }

            if (string.Equals(closeStatus, ShippingGoodsStatusInfo.Closed, StringComparison.OrdinalIgnoreCase))
            {
                queryString += string.Format(" and sg.Status='{0}'", SafeSqlValue(ShippingGoodsStatusInfo.Closed));
            }
            else if (!string.IsNullOrWhiteSpace(closeStatus))
            {
                queryString += string.Format(" and (sg.Status is null or sg.Status='' or sg.Status<>'{0}')", SafeSqlValue(ShippingGoodsStatusInfo.Closed));
            }

            queryString += PrintRecordOrderBy;
            return GetPrintRecordsBySql(queryString);
        }

        public List<PrintRecordInfo> GetPrintRecordsByBusinessKey(string supplyBatchNo, string partNo, string cartonNo, string printType)
        {
            string queryString = string.Format(
                "select {0} from tb_PrintRecord where SupplyBatchNo='{1}' and PartNo='{2}' and CartonNo='{3}' and PrintType='{4}'",
                PrintRecordSelectColumns,
                SafeSqlValue(supplyBatchNo),
                SafeSqlValue(partNo),
                SafeSqlValue(cartonNo),
                SafeSqlValue(printType));

            queryString += PrintRecordOrderBy;
            return GetPrintRecordsBySql(queryString);
        }

        public List<PrintRecordInfo> GetPrintRecordsByTaskId(Guid taskId)
        {
            string queryString = string.Format(
                "select {0} from tb_PrintRecord where TaskId='{1}'",
                PrintRecordSelectColumns,
                taskId);

            queryString += PrintRecordOrderBy;
            return GetPrintRecordsBySql(queryString);
        }

        public List<PrintRecordInfo> LockPendingPrintRecords(string machineId, int maxCount, int lockTimeoutMinutes)
        {
            if (string.IsNullOrWhiteSpace(machineId) || maxCount <= 0)
            {
                return new List<PrintRecordInfo>();
            }

            int takeCount = maxCount > 30 ? 30 : maxCount;
            int timeoutMinutes = lockTimeoutMinutes < 1 ? 3 : lockTimeoutMinutes;
            string outputColumns = "inserted." + PrintRecordSelectColumns.Replace(",", ",inserted.");
            string queryString = string.Format(
                "update tb_PrintRecord set Status=@FailedStatus,LockTime=null,UpdateTime=getdate()" +
                " where Status=@Status and MachineId=@MachineId and LockTime is not null" +
                " and LockTime <= dateadd(minute, -@LockTimeoutMinutes, getdate());" +
                " with NextRecord as (" +
                " select top (@TopCount) {0} from tb_PrintRecord with (updlock, rowlock, readpast)" +
                " where Status=@Status and MachineId=@MachineId and LockTime is null" +
                " order by CreateTime asc, Id asc" +
                ") " +
                "update NextRecord set LockTime=getdate()" +
                " output {1};",
                PrintRecordSelectColumns,
                outputColumns);

            DataSet dataSet = Data.getDataSet(
                queryString,
                new SqlParameter("@TopCount", SqlDbType.Int) { Value = takeCount },
                new SqlParameter("@Status", SqlDbType.Int) { Value = PrintRecordStatusInfo.Pending },
                new SqlParameter("@FailedStatus", SqlDbType.Int) { Value = PrintRecordStatusInfo.Failed },
                new SqlParameter("@LockTimeoutMinutes", SqlDbType.Int) { Value = timeoutMinutes },
                new SqlParameter("@MachineId", SqlDbType.NVarChar, 50) { Value = machineId.Trim() });

            return BuildPrintRecords(dataSet);
        }

        public ParamterInfo InsertPrintRecord(List<PrintRecordInfo> printRecordInfos)
        {
            const string sql = "insert into tb_PrintRecord (SupplyBatchNo,PartNo,CartonNo,TaskId,ClientId,MachineId,PrintType,PdfUrl,PdfDownLoadUrl,PdfDownloadPath,LocalPath,Status,PrintCount,PrintUser,PrintTime,FirstPrintUser,FirstPrintTime,LastPrintUser,LastPrintTime,ReprintReason,ReprintReasonsId,CreateUser,CreateTime,UpdateUser,UpdateTime,LockTime) values (@SupplyBatchNo,@PartNo,@CartonNo,@TaskId,@ClientId,@MachineId,@PrintType,@PdfUrl,@PdfDownLoadUrl,@PdfDownloadPath,@LocalPath,@Status,@PrintCount,@PrintUser,@PrintTime,@FirstPrintUser,@FirstPrintTime,@LastPrintUser,@LastPrintTime,@ReprintReason,@ReprintReasonsId,@CreateUser,@CreateTime,@UpdateUser,@UpdateTime,@LockTime)";
            return BuildParamterInfo(printRecordInfos, sql, BuildInsertOrUpdateParameters);
        }

        public ParamterInfo UpdatePrintRecord(List<PrintRecordInfo> printRecordInfos)
        {
            const string sql = "update tb_PrintRecord set TaskId=@TaskId,ClientId=@ClientId,MachineId=@MachineId,PdfUrl=@PdfUrl,PdfDownLoadUrl=@PdfDownLoadUrl,PdfDownloadPath=@PdfDownloadPath,LocalPath=@LocalPath,Status=@Status,PrintCount=@PrintCount,PrintUser=@PrintUser,PrintTime=@PrintTime,FirstPrintUser=@FirstPrintUser,FirstPrintTime=@FirstPrintTime,LastPrintUser=@LastPrintUser,LastPrintTime=@LastPrintTime,ReprintReason=@ReprintReason,ReprintReasonsId=@ReprintReasonsId,CreateUser=@CreateUser,CreateTime=@CreateTime,UpdateUser=@UpdateUser,UpdateTime=@UpdateTime,LockTime=@LockTime where SupplyBatchNo=@SupplyBatchNo and PartNo=@PartNo and CartonNo=@CartonNo and PrintType=@PrintType";
            return BuildParamterInfo(printRecordInfos, sql, BuildInsertOrUpdateParameters);
        }

        public ParamterInfo DeletePrintRecord(List<PrintRecordInfo> printRecordInfos)
        {
            const string sql = "delete from tb_PrintRecord where SupplyBatchNo=@SupplyBatchNo and PartNo=@PartNo and CartonNo=@CartonNo and PrintType=@PrintType";
            return BuildParamterInfo(printRecordInfos, sql, info => new[]
            {
                new SqlParameter("@SupplyBatchNo", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.SupplyBatchNo) },
                new SqlParameter("@PartNo", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.PartNo) },
                new SqlParameter("@CartonNo", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.CartonNo) },
                new SqlParameter("@PrintType", SqlDbType.NVarChar, 20) { Value = ToDbValue(info.PrintType) }
            });
        }

        private static List<PrintRecordInfo> GetPrintRecordsBySql(string queryString)
        {
            DataSet dataSet = Data.getDataSet(queryString);
            return BuildPrintRecords(dataSet);
        }

        private static List<PrintRecordInfo> BuildPrintRecords(DataSet dataSet)
        {
            List<PrintRecordInfo> result = new List<PrintRecordInfo>();
            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                return result;
            }

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                result.Add(new PrintRecordInfo
                {
                    Id = ReadNullableLong(row, "Id"),
                    SupplyBatchNo = ReadString(row, "SupplyBatchNo"),
                    PartNo = ReadString(row, "PartNo"),
                    CartonNo = ReadString(row, "CartonNo"),
                    TaskId = ReadNullableGuid(row, "TaskId"),
                    ClientId = ReadString(row, "ClientId"),
                    MachineId = ReadString(row, "MachineId"),
                    PrintType = ReadString(row, "PrintType"),
                    PdfUrl = ReadString(row, "PdfUrl"),
                    PdfDownLoadUrl = ReadString(row, "PdfDownLoadUrl"),
                    PdfDownloadPath = ReadString(row, "PdfDownloadPath"),
                    LocalPath = ReadString(row, "LocalPath"),
                    Status = ReadNullableInt(row, "Status"),
                    PrintCount = ReadNullableInt(row, "PrintCount"),
                    PrintUser = ReadString(row, "PrintUser"),
                    PrintTime = ReadNullableDateTime(row, "PrintTime"),
                    FirstPrintUser = ReadString(row, "FirstPrintUser"),
                    FirstPrintTime = ReadNullableDateTime(row, "FirstPrintTime"),
                    LastPrintUser = ReadString(row, "LastPrintUser"),
                    LastPrintTime = ReadNullableDateTime(row, "LastPrintTime"),
                    ReprintReason = ReadString(row, "ReprintReason"),
                    ReprintReasonsId = ReadNullableInt(row, "ReprintReasonsId"),
                    CreateUser = ReadString(row, "CreateUser"),
                    CreateTime = ReadNullableDateTime(row, "CreateTime"),
                    UpdateUser = ReadString(row, "UpdateUser"),
                    UpdateTime = ReadNullableDateTime(row, "UpdateTime"),
                    LockTime = ReadNullableDateTime(row, "LockTime")
                });
            }

            return result;
        }

        private static ParamterInfo BuildParamterInfo(List<PrintRecordInfo> printRecordInfos, string sql, Func<PrintRecordInfo, SqlParameter[]> parameterBuilder)
        {
            ParamterInfo paramterInfo = new ParamterInfo
            {
                Sql = sql,
                Type = CommandType.Text,
                AlSQL = new ArrayList(),
                AlPAR = new ArrayList(),
                AlCOM = new ArrayList()
            };

            if (printRecordInfos == null)
            {
                return paramterInfo;
            }

            foreach (PrintRecordInfo printRecordInfo in printRecordInfos)
            {
                paramterInfo.AlSQL.Add(sql);
                paramterInfo.AlPAR.Add(parameterBuilder(printRecordInfo));
                paramterInfo.AlCOM.Add(CommandType.Text);
            }

            return paramterInfo;
        }

        private static SqlParameter[] BuildInsertOrUpdateParameters(PrintRecordInfo info)
        {
            return new[]
            {
                new SqlParameter("@SupplyBatchNo", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.SupplyBatchNo) },
                new SqlParameter("@PartNo", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.PartNo) },
                new SqlParameter("@CartonNo", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.CartonNo) },
                new SqlParameter("@TaskId", SqlDbType.UniqueIdentifier) { Value = ToDbValue(info.TaskId) },
                new SqlParameter("@ClientId", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.ClientId) },
                new SqlParameter("@MachineId", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.MachineId) },
                new SqlParameter("@PrintType", SqlDbType.NVarChar, 20) { Value = ToDbValue(info.PrintType) },
                new SqlParameter("@PdfUrl", SqlDbType.NVarChar, 500) { Value = ToDbValue(info.PdfUrl) },
                new SqlParameter("@PdfDownLoadUrl", SqlDbType.NVarChar, 500) { Value = ToDbValue(info.PdfDownLoadUrl) },
                new SqlParameter("@PdfDownloadPath", SqlDbType.NVarChar, 500) { Value = ToDbValue(info.PdfDownloadPath) },
                new SqlParameter("@LocalPath", SqlDbType.NVarChar, 500) { Value = ToDbValue(info.LocalPath) },
                new SqlParameter("@Status", SqlDbType.Int) { Value = ToDbValue(info.Status) },
                new SqlParameter("@PrintCount", SqlDbType.Int) { Value = ToDbValue(info.PrintCount) },
                new SqlParameter("@PrintUser", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.PrintUser) },
                new SqlParameter("@PrintTime", SqlDbType.DateTime) { Value = ToDbValue(info.PrintTime) },
                new SqlParameter("@FirstPrintUser", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.FirstPrintUser) },
                new SqlParameter("@FirstPrintTime", SqlDbType.DateTime) { Value = ToDbValue(info.FirstPrintTime) },
                new SqlParameter("@LastPrintUser", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.LastPrintUser) },
                new SqlParameter("@LastPrintTime", SqlDbType.DateTime) { Value = ToDbValue(info.LastPrintTime) },
                new SqlParameter("@ReprintReason", SqlDbType.NVarChar, 200) { Value = ToDbValue(info.ReprintReason) },
                new SqlParameter("@ReprintReasonsId", SqlDbType.Int) { Value = ToDbValue(info.ReprintReasonsId) },
                new SqlParameter("@CreateUser", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.CreateUser) },
                new SqlParameter("@CreateTime", SqlDbType.DateTime) { Value = ToDbValue(info.CreateTime) },
                new SqlParameter("@UpdateUser", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.UpdateUser) },
                new SqlParameter("@UpdateTime", SqlDbType.DateTime) { Value = ToDbValue(info.UpdateTime) },
                new SqlParameter("@LockTime", SqlDbType.DateTime) { Value = ToDbValue(info.LockTime) }
            };
        }

        private static object ToDbValue(object value)
        {
            return value ?? DBNull.Value;
        }

        private static string SafeSqlValue(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }

        private static string ReadString(DataRow row, string columnName)
        {
            return row.IsNull(columnName) ? null : row[columnName].ToString();
        }

        private static int? ReadNullableInt(DataRow row, string columnName)
        {
            return row.IsNull(columnName) ? (int?)null : Convert.ToInt32(row[columnName]);
        }

        private static long? ReadNullableLong(DataRow row, string columnName)
        {
            return row.IsNull(columnName) ? (long?)null : Convert.ToInt64(row[columnName]);
        }

        private static Guid? ReadNullableGuid(DataRow row, string columnName)
        {
            return row.IsNull(columnName) ? (Guid?)null : (Guid)row[columnName];
        }

        private static DateTime? ReadNullableDateTime(DataRow row, string columnName)
        {
            return row.IsNull(columnName) ? (DateTime?)null : Convert.ToDateTime(row[columnName]);
        }
    }
}
