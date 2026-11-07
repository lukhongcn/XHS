using XHS.IDAL;

namespace XHS.DALFactory
{
    /// <summary>
    /// 零件工序数据访问实例工厂。
    /// </summary>
    public class XHSPartPartProcess
    {
        public static IXHSPartPartProcess Create()
        {
            return new XHS.MSSQL.XHSPartPartProcess();
        }
    }
}
