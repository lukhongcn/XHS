using XHS.IDAL;

namespace XHS.DALFactory
{
    public static class ScanFlowScanRecord
    {
        public static IScanFlowScanRecord Create()
        {
            return new XHS.MSSQL.ScanFlowScanRecord();
        }
    }
}
