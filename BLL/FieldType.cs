using System.Collections;
using System.Collections.Generic;
using ModuleWorkFlow.business;
using XHS.IDAL;
using XHS.Model;

namespace BLL
{
    /// <summary>
    /// 字段解析类型业务层。
    /// </summary>
    public class FieldType
    {
        private readonly IFieldType dal;

        public FieldType()
        {
            dal = XHS.DALFactory.FieldType.Create();
        }

        public List<FieldTypeInfo> GetFieldTypes()
        {
            return dal.GetFieldTypes();
        }

        public FieldTypeInfo GetFieldType(string typeCode)
        {
            return dal.GetFieldType(typeCode);
        }

        public string InsertFieldTypes(List<FieldTypeInfo> infos)
        {
            Normalize(infos);
            string validateMessage = Validate(infos);
            if (!string.IsNullOrWhiteSpace(validateMessage))
            {
                return validateMessage;
            }

            ParamterInfo paramterInfo = dal.InsertFieldTypes(infos);
            IList source = new ArrayList();
            source.Add(paramterInfo);
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string UpdateFieldTypes(List<FieldTypeInfo> infos)
        {
            Normalize(infos);
            string validateMessage = Validate(infos);
            if (!string.IsNullOrWhiteSpace(validateMessage))
            {
                return validateMessage;
            }

            ParamterInfo paramterInfo = dal.UpdateFieldTypes(infos);
            IList source = new ArrayList();
            source.Add(paramterInfo);
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string DeleteFieldTypes(List<FieldTypeInfo> infos)
        {
            ParamterInfo paramterInfo = dal.DeleteFieldTypes(infos);
            IList source = new ArrayList();
            source.Add(paramterInfo);
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        private static void Normalize(List<FieldTypeInfo> infos)
        {
            if (infos == null)
            {
                return;
            }

            foreach (FieldTypeInfo info in infos)
            {
                if (info == null)
                {
                    continue;
                }

                info.TypeCode = (info.TypeCode ?? string.Empty).Trim();
                info.TypeName = (info.TypeName ?? string.Empty).Trim();
                info.ParserClass = string.IsNullOrWhiteSpace(info.ParserClass) ? null : info.ParserClass.Trim();
            }
        }

        private static string Validate(List<FieldTypeInfo> infos)
        {
            if (infos == null)
            {
                return string.Empty;
            }

            foreach (FieldTypeInfo info in infos)
            {
                if (info == null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(info.TypeCode))
                {
                    return "请填写字段类型代码。";
                }

                if (string.IsNullOrWhiteSpace(info.TypeName))
                {
                    return "请填写字段类型名称。";
                }
            }

            return string.Empty;
        }
    }
}
