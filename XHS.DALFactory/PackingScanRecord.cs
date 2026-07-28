using XHS.IDAL;

namespace XHS.DALFactory
{
    /// <summary>
    /// 装箱扫描记录数据访问实例工厂。
    /// </summary>
    public class PackingScanRecord
    {
        public static IPackingScanRecord Create()
        {
            return new XHS.MSSQL.PackingScanRecord();
        }
    }
}
