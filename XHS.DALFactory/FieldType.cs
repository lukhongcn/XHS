using XHS.IDAL;

namespace XHS.DALFactory
{
    /// <summary>
    /// 字段解析类型数据访问实例工厂。
    /// </summary>
    public class FieldType
    {
        public static IFieldType Create()
        {
            return new XHS.MSSQL.FieldType();
        }
    }
}
