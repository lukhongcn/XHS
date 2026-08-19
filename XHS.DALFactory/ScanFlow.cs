using XHS.IDAL;

namespace XHS.DALFactory
{
    /// <summary>
    /// 扫描流程数据访问实例工厂。
    /// </summary>
    public class ScanFlow
    {
        public static IScanFlow Create()
        {
            return new XHS.MSSQL.ScanFlow();
        }
    }
}
