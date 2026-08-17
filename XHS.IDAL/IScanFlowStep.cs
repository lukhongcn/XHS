using System.Collections.Generic;
using XHS.Model;

namespace XHS.IDAL
{
    /// <summary>
    /// 扫描流程步骤数据访问接口。
    /// </summary>
    public interface IScanFlowStep
    {
        List<ScanFlowStepInfo> GetStepsByFlowId(int flowId);
        ParamterInfo InsertSteps(List<ScanFlowStepInfo> infos);
        ParamterInfo UpdateSteps(List<ScanFlowStepInfo> infos);
        ParamterInfo DeleteSteps(List<ScanFlowStepInfo> infos);
    }
}
