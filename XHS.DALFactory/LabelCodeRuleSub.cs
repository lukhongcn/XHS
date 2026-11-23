using XHS.IDAL;

namespace XHS.DALFactory
{
    /// <summary>
    /// 标签编码规则重复明细子规则实例工厂。
    /// </summary>
    public class LabelCodeRuleSub
    {
        public static ILabelCodeRuleSub Create()
        {
            return new XHS.MSSQL.LabelCodeRuleSub();
        }
    }
}
