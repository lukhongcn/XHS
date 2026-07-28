using XHS.IDAL;

namespace XHS.DALFactory
{
    /// <summary>
    /// 装箱异常记录数据访问实例工厂。
    /// </summary>
    public class PackingException
    {
        public static IPackingException Create()
        {
            return new XHS.MSSQL.PackingException();
        }
    }
}
