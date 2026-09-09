using System.Collections.Generic;
using XHS.Model;

namespace XHS.IDAL
{
    /// <summary>
    /// 批次规则数据访问接口。
    /// </summary>
    public interface IBatchRule
    {
        List<BatchRuleInfo> GetBatchRules();

        BatchRuleInfo GetBatchRule(int batchRuleId);

        ParamterInfo InsertBatchRules(List<BatchRuleInfo> batchRuleInfos);

        ParamterInfo UpdateBatchRules(List<BatchRuleInfo> batchRuleInfos);

        ParamterInfo DeleteBatchRules(List<BatchRuleInfo> batchRuleInfos);
    }
}
