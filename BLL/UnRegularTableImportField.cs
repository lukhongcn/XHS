using System.Collections.Generic;
using Model;

namespace BLL
{
    /// <summary>
    /// 不规则表格导入字段配置业务层。
    /// </summary>
    public class UnRegularTableImportField
    {
        public List<UnRegularTableImportFieldInfo> GetUnRegularTableImportFieldByTemplateCode(string templateCode)
        {
            return XHS.DALFactory.UnRegularTableImportField.Create().GetUnRegularTableImportFieldByTemplateCode(templateCode);
        }
    }
}
