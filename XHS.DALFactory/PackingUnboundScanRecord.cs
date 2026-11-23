using XHS.IDAL;

namespace XHS.DALFactory
{
    /// <summary>解除绑定扫描历史数据访问实例工厂。</summary>
    public class PackingUnboundScanRecord
    {
        public static IPackingUnboundScanRecord Create()
        {
            return new XHS.MSSQL.PackingUnboundScanRecord();
        }
    }
}
