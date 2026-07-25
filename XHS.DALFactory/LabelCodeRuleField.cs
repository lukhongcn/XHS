using XHS.IDAL;

namespace XHS.DALFactory
{
    /// <summary>
    /// 标签编码规则字段映射数据访问实例工厂。
    /// </summary>
    public class LabelCodeRuleField
    {
        public static ILabelCodeRuleField Create()
        {
            return new XHS.MSSQL.LabelCodeRuleField();
        }
    }
}
