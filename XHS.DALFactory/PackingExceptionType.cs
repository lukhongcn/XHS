using XHS.IDAL;

namespace XHS.DALFactory
{
    /// <summary>
    /// 装箱异常类型数据访问实例工厂。
    /// </summary>
    public class PackingExceptionType
    {
        public static IPackingExceptionType Create()
        {
            return new XHS.MSSQL.PackingExceptionType();
        }
    }
}
