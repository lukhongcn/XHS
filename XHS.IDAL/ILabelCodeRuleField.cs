using System.Collections.Generic;
using XHS.Model;

namespace XHS.IDAL
{
    /// <summary>
    /// 标签编码规则字段映射数据访问接口。
    /// </summary>
    public interface ILabelCodeRuleField
    {
        List<LabelCodeRuleFieldInfo> GetLabelCodeRuleFields();

        List<LabelCodeRuleFieldInfo> GetLabelCodeRuleFieldsByRuleId(int ruleId);

        List<LabelCodeRuleFieldInfo> GetRuleFieldByCustomer(string customerId);

        ParamterInfo InsertLabelCodeRuleFields(List<LabelCodeRuleFieldInfo> labelCodeRuleFieldInfos);

        ParamterInfo UpdateLabelCodeRuleFields(List<LabelCodeRuleFieldInfo> labelCodeRuleFieldInfos);

        ParamterInfo DeleteLabelCodeRuleFields(List<LabelCodeRuleFieldInfo> labelCodeRuleFieldInfos);
    }
}
