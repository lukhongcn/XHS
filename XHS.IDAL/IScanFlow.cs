using System.Collections.Generic;
using XHS.Model;

namespace XHS.IDAL
{
    /// <summary>
    /// 扫描流程数据访问接口。
    /// </summary>
    public interface IScanFlow
    {
        List<ScanFlowInfo> GetByFlowName(string flowName);
    }
}
