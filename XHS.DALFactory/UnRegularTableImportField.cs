using XHS.IDAL;

namespace XHS.DALFactory
{
    /// <summary>
    /// 不规则表格导入字段配置数据访问实例工厂。
    /// </summary>
    public class UnRegularTableImportField
    {
        public static IUnRegularTableImportField Create()
        {
            return new XHS.MSSQL.UnRegularTableImportField();
        }
    }
}
