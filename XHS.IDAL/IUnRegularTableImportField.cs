using System.Collections.Generic;
using XHS.Model;

namespace XHS.IDAL
{
    /// <summary>
    /// 不规则表格导入字段配置数据访问接口。
    /// </summary>
    public interface IUnRegularTableImportField
    {
        List<UnRegularTableImportFieldInfo> GetUnRegularTableImportFieldByTemplateCode(string templateCode);
    }
}
