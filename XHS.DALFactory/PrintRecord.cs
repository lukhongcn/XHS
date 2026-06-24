using XHS.IDAL;

namespace XHS.DALFactory
{
    /// <summary>
    /// 打印记录数据访问实例工厂。
    /// </summary>
    public class PrintRecord
    {
        public static IPrintRecord Create()
        {
            return new XHS.MSSQL.PrintRecord();
        }
    }
}
