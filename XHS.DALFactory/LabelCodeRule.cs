using XHS.IDAL;

namespace XHS.DALFactory
{
    /// <summary>
    /// 标签编码规则数据访问实例工厂。
    /// </summary>
    public class LabelCodeRule
    {
        public static ILabelCodeRule Create()
        {
            return new XHS.MSSQL.LabelCodeRule();
        }
    }
}
