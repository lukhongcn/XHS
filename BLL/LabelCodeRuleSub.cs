using System.Collections;
using System.Collections.Generic;
using ModuleWorkFlow.business;
using XHS.IDAL;
using XHS.Model;

namespace XHS.BLL
{
    /// <summary>
    /// 标签编码规则重复明细子规则业务层。
    /// </summary>
    public class LabelCodeRuleSub
    {
        private readonly ILabelCodeRuleSub dal;

        public LabelCodeRuleSub()
        {
            dal = XHS.DALFactory.LabelCodeRuleSub.Create();
        }

        public List<LabelCodeRuleSubInfo> GetLabelCodeRuleSubs()
        {
            return dal.GetLabelCodeRuleSubs();
        }

        public LabelCodeRuleSubInfo GetLabelCodeRuleSub(int subRuleId)
        {
            return dal.GetLabelCodeRuleSub(subRuleId);
        }

        public List<LabelCodeRuleSubInfo> GetLabelCodeRuleSubsByRuleId(int ruleId)
        {
            return dal.GetLabelCodeRuleSubsByRuleId(ruleId);
        }

        public List<LabelCodeRuleSubInfo> GetLabelCodeRuleSubsByCustomerIdAndLabelType(
            string customerId,
            string labelType)
        {
            return dal.GetLabelCodeRuleSubsByCustomerIdAndLabelType(customerId, labelType);
        }

        public string InsertLabelCodeRuleSubs(List<LabelCodeRuleSubInfo> infos)
        {
            Normalize(infos);
            string message = Validate(infos, false);
            if (!string.IsNullOrWhiteSpace(message)) return message;

            ParamterInfo parameterInfo = dal.InsertLabelCodeRuleSubs(infos);
            IList source = new ArrayList { parameterInfo };
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string UpdateLabelCodeRuleSubs(List<LabelCodeRuleSubInfo> infos)
        {
            Normalize(infos);
            string message = Validate(infos, true);
            if (!string.IsNullOrWhiteSpace(message)) return message;

            ParamterInfo parameterInfo = dal.UpdateLabelCodeRuleSubs(infos);
            IList source = new ArrayList { parameterInfo };
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string DeleteLabelCodeRuleSubs(List<LabelCodeRuleSubInfo> infos)
        {
            ParamterInfo parameterInfo = dal.DeleteLabelCodeRuleSubs(infos);
            IList source = new ArrayList { parameterInfo };
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        private static void Normalize(List<LabelCodeRuleSubInfo> infos)
        {
            if (infos == null) return;

            foreach (LabelCodeRuleSubInfo info in infos)
            {
                if (info == null) continue;
                info.SubRuleName = (info.SubRuleName ?? string.Empty).Trim();
                info.ItemSeparator = string.IsNullOrWhiteSpace(info.ItemSeparator) ? null : info.ItemSeparator;
                info.FieldSeparator = string.IsNullOrWhiteSpace(info.FieldSeparator) ? null : info.FieldSeparator;
                info.ItemRegex = (info.ItemRegex ?? string.Empty).Trim();
                info.Remark = string.IsNullOrWhiteSpace(info.Remark) ? null : info.Remark.Trim();
                if (!info.SortNo.HasValue) info.SortNo = 0;
                if (!info.Enabled.HasValue) info.Enabled = true;
            }
        }

        private static string Validate(List<LabelCodeRuleSubInfo> infos, bool update)
        {
            if (infos == null) return string.Empty;

            foreach (LabelCodeRuleSubInfo info in infos)
            {
                if (info == null) continue;
                if (update && (!info.SubRuleId.HasValue || info.SubRuleId.Value <= 0))
                    return "请填写子规则ID。";
                if (!info.RuleId.HasValue || info.RuleId.Value <= 0)
                    return "请填写主规则ID。";
                if (string.IsNullOrWhiteSpace(info.SubRuleName))
                    return "请填写子规则名称。";
                if (string.IsNullOrWhiteSpace(info.ItemRegex))
                    return "请填写子规则正则表达式。";
            }

            return string.Empty;
        }
    }
}
