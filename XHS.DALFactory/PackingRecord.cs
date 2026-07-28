using XHS.IDAL;

namespace XHS.DALFactory
{
    /// <summary>
    /// 装箱记录数据访问实例工厂。
    /// </summary>
    public class PackingRecord
    {
        public static IPackingRecord Create()
        {
            return new XHS.MSSQL.PackingRecord();
        }
    }
}
