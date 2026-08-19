using System.Collections.Generic;
using XHS.IDAL;
using XHS.Model;

namespace BLL
{
    /// <summary>
    /// 扫描流程业务层。
    /// </summary>
    public class ScanFlow
    {
        private readonly IScanFlow dal;

        public ScanFlow()
        {
            dal = XHS.DALFactory.ScanFlow.Create();
        }

        public List<ScanFlowInfo> GetByFlowName(string flowName)
        {
            return string.IsNullOrWhiteSpace(flowName)
                ? new List<ScanFlowInfo>()
                : dal.GetByFlowName(flowName.Trim());
        }
    }
}
