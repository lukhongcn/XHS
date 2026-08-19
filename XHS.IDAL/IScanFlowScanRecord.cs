using System.Collections.Generic;
using XHS.Model;

namespace XHS.IDAL
{
    public interface IScanFlowScanRecord
    {
        bool IsFactoryBarcodeRecorded(int flowId, string scanContent);
        ParamterInfo InsertRecords(List<ScanFlowScanRecordInfo> infos);
    }
}
