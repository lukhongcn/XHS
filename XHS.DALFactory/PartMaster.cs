using XHS.IDAL;

namespace XHS.DALFactory
{
    /// <summary>
    /// 零件主数据数据访问实例工厂。
    /// </summary>
    public class PartMaster
    {
        public static IPartMaster Create()
        {
            return new XHS.MSSQL.PartMaster();
        }
    }
}
