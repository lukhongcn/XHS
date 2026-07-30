using System.Collections;
using System.Collections.Generic;
using ModuleWorkFlow.business;
using XHS.IDAL;
using XHS.Model;

namespace XHS.BLL
{
    /// <summary>
    /// 标签编码规则业务层。
    /// </summary>
    public class LabelCodeRule
    {
        private readonly ILabelCodeRule dal;

        public LabelCodeRule()
        {
            dal = XHS.DALFactory.LabelCodeRule.Create();
        }

        public List<LabelCodeRuleInfo> GetLabelCodeRules()
        {
            return dal.GetLabelCodeRules();
        }

        public LabelCodeRuleInfo GetLabelCodeRule(int ruleId)
        {
            return dal.GetLabelCodeRule(ruleId);
        }

        public List<LabelCodeRuleInfo> GetLabelCodeRulesByCustomerId(string customerId)
        {
            return dal.GetLabelCodeRulesByCustomerId(customerId);
        }

        public string InsertLabelCodeRules(List<LabelCodeRuleInfo> infos)
        {
            Normalize(infos);
            string validateMessage = Validate(infos);
            if (!string.IsNullOrWhiteSpace(validateMessage))
            {
                return validateMessage;
            }

            ParamterInfo paramterInfo = dal.InsertLabelCodeRules(infos);
            IList source = new ArrayList();
            source.Add(paramterInfo);
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string UpdateLabelCodeRules(List<LabelCodeRuleInfo> infos)
        {
            Normalize(infos);
            string validateMessage = Validate(infos);
            if (!string.IsNullOrWhiteSpace(validateMessage))
            {
                return validateMessage;
            }

            ParamterInfo paramterInfo = dal.UpdateLabelCodeRules(infos);
            IList source = new ArrayList();
            source.Add(paramterInfo);
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string DeleteLabelCodeRules(List<LabelCodeRuleInfo> infos)
        {
            ParamterInfo paramterInfo = dal.DeleteLabelCodeRules(infos);
            IList source = new ArrayList();
            source.Add(paramterInfo);
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        private static void Normalize(List<LabelCodeRuleInfo> infos)
        {
            if (infos == null)
            {
                return;
            }

            foreach (LabelCodeRuleInfo info in infos)
            {
                if (info == null)
                {
                    continue;
                }

                info.RuleName = (info.RuleName ?? string.Empty).Trim();
                info.LabelType = (info.LabelType ?? string.Empty).Trim();
                info.ParseType = (info.ParseType ?? string.Empty).Trim();
                info.Separator = string.IsNullOrWhiteSpace(info.Separator) ? null : info.Separator.Trim();
                info.KeySeparator = string.IsNullOrWhiteSpace(info.KeySeparator) ? null : info.KeySeparator.Trim();
                info.Remark = string.IsNullOrWhiteSpace(info.Remark) ? null : info.Remark.Trim();
            }
        }

        private static string Validate(List<LabelCodeRuleInfo> infos)
        {
            if (infos == null)
            {
                return string.Empty;
            }

            foreach (LabelCodeRuleInfo info in infos)
            {
                if (info == null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(info.RuleName))
                {
                    return "请填写规则名称。";
                }

                if (string.IsNullOrWhiteSpace(info.LabelType))
                {
                    return "请填写标签类型。";
                }

                if (info.LabelType != "CUSTOMER" && info.LabelType != "MES")
                {
                    return "标签类型必须为 CUSTOMER 或 MES。";
                }

                if (string.IsNullOrWhiteSpace(info.ParseType))
                {
                    return "请填写解析方式。";
                }

                if (info.ParseType != "KEY_VALUE" && info.ParseType != "POSITION" && info.ParseType != "FIX_LENGTH")
                {
                    return "解析方式必须为 KEY_VALUE、POSITION 或 FIX_LENGTH。";
                }

                if (info.Version == null || info.Version.Value <= 0)
                {
                    return "版本号必须大于 0。";
                }
            }

            return string.Empty;
        }
    }
}
