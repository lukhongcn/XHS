using XHS.IDAL;

namespace XHS.DALFactory
{
    /// <summary>
    /// PLC 结果数据访问实例工厂。
    /// </summary>
    public class PLCResult
    {
        public static IPLCResult Create()
        {
            return new XHS.MSSQL.PLCResult();
        }
    }
}
