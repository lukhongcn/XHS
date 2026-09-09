using System.Collections.Generic;
using XHS.Model;

namespace XHS.IDAL
{
    /// <summary>
    /// 字段解析类型数据访问接口。
    /// </summary>
    public interface IFieldType
    {
        List<FieldTypeInfo> GetFieldTypes();

        FieldTypeInfo GetFieldType(string typeCode);

        ParamterInfo InsertFieldTypes(List<FieldTypeInfo> fieldTypeInfos);

        ParamterInfo UpdateFieldTypes(List<FieldTypeInfo> fieldTypeInfos);

        ParamterInfo DeleteFieldTypes(List<FieldTypeInfo> fieldTypeInfos);
    }
}
