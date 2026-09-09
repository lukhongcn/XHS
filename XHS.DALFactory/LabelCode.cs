using XHS.IDAL;

namespace XHS.DALFactory
{
    /// <summary>
    /// 标签编码解析规则数据访问实例工厂。
    /// </summary>
    public class LabelCode
    {
        public static ILabelCode Create()
        {
            return new XHS.MSSQL.LabelCode();
        }
    }
}
