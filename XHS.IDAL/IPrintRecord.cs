using System.Collections.Generic;
using XHS.Model;

namespace XHS.IDAL
{
    /// <summary>
    /// 打印记录数据访问接口。
    /// </summary>
    public interface IPrintRecord
    {
        List<PrintRecordInfo> GetPrintRecords();

        List<PrintRecordInfo> GetPrintRecords(string supplyBatchNo, string partNo, string cartonNo, string printType);

        List<PrintRecordInfo> GetPrintRecordsByBusinessKey(string supplyBatchNo, string partNo, string cartonNo, string printType);

        List<PrintRecordInfo> LockPendingPrintRecords(string machineId, int maxCount);

        ParamterInfo InsertPrintRecord(List<PrintRecordInfo> printRecordInfos);

        ParamterInfo UpdatePrintRecord(List<PrintRecordInfo> printRecordInfos);

        ParamterInfo DeletePrintRecord(List<PrintRecordInfo> printRecordInfos);
    }
}
