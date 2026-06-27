using XHS.IDAL;

namespace XHS.DALFactory
{
    /// <summary>
    /// 补打原因字典数据访问实例工厂。
    /// </summary>
    public class ReprintReason
    {
        public static IReprintReason Create()
        {
            return new XHS.MSSQL.ReprintReason();
        }
    }
}
