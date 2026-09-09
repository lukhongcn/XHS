using System.Collections.Generic;
using System;
using System.Data;
using XHS.Model;

namespace XHS.IDAL
{
    public interface IScanFlowScanRecord
    {
        bool IsFactoryBarcodeRecorded(int flowId, string scanContent);
        DataTable GetLabelBindingRecords(string flowCode, string scanContentLike, DateTime startTime, DateTime endTime);
        ParamterInfo InsertRecords(List<ScanFlowScanRecordInfo> infos);
    }
}
