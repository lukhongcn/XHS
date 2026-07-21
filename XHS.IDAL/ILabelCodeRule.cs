using System.Collections.Generic;
using XHS.Model;

namespace XHS.IDAL
{
    /// <summary>
    /// 标签编码规则数据访问接口。
    /// </summary>
    public interface ILabelCodeRule
    {
        List<LabelCodeRuleInfo> GetLabelCodeRules();

        LabelCodeRuleInfo GetLabelCodeRule(int ruleId);

        List<LabelCodeRuleInfo> GetLabelCodeRulesByCustomerId(string customerId);

        ParamterInfo InsertLabelCodeRules(List<LabelCodeRuleInfo> labelCodeRuleInfos);

        ParamterInfo UpdateLabelCodeRules(List<LabelCodeRuleInfo> labelCodeRuleInfos);

        ParamterInfo DeleteLabelCodeRules(List<LabelCodeRuleInfo> labelCodeRuleInfos);
    }
}
