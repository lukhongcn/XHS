using System.Collections.Generic;
using XHS.IDAL;
using XHS.Model;

namespace BLL
{
    /// <summary>
    /// 不规则表格导入字段配置业务层。
    /// </summary>
    public class UnRegularTableImportField
    {
        private readonly IUnRegularTableImportField dal;

        public UnRegularTableImportField()
        {
            dal = XHS.DALFactory.UnRegularTableImportField.Create();
        }

        public List<UnRegularTableImportFieldInfo> GetUnRegularTableImportFieldByTemplateCode(string templateCode)
        {
            return dal.GetUnRegularTableImportFieldByTemplateCode(templateCode);
        }
    }
}
