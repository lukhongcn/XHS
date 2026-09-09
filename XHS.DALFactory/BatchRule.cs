using XHS.IDAL;

namespace XHS.DALFactory
{
    /// <summary>
    /// 批次规则数据访问实例工厂。
    /// </summary>
    public class BatchRule
    {
        public static IBatchRule Create()
        {
            return new XHS.MSSQL.BatchRule();
        }
    }
}
