using System.Collections.Generic;
using XHS.Model;

namespace XHS.IDAL
{
    /// <summary>
    /// 标签编码规则重复明细子规则数据访问接口。
    /// </summary>
    public interface ILabelCodeRuleSub
    {
        List<LabelCodeRuleSubInfo> GetLabelCodeRuleSubs();

        LabelCodeRuleSubInfo GetLabelCodeRuleSub(int subRuleId);

        List<LabelCodeRuleSubInfo> GetLabelCodeRuleSubsByRuleId(int ruleId);

        List<LabelCodeRuleSubInfo> GetLabelCodeRuleSubsByCustomerIdAndLabelType(
            string customerId,
            string labelType);

        ParamterInfo InsertLabelCodeRuleSubs(List<LabelCodeRuleSubInfo> infos);

        ParamterInfo UpdateLabelCodeRuleSubs(List<LabelCodeRuleSubInfo> infos);

        ParamterInfo DeleteLabelCodeRuleSubs(List<LabelCodeRuleSubInfo> infos);
    }
}
