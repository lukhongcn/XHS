using System.Collections;
using System.Collections.Generic;
using ModuleWorkFlow.business;
using XHS.IDAL;
using XHS.Model;

namespace BLL
{
    /// <summary>
    /// 标签编码规则字段映射业务层。
    /// </summary>
    public class LabelCodeRuleField
    {
        private readonly ILabelCodeRuleField dal;

        public LabelCodeRuleField()
        {
            dal = XHS.DALFactory.LabelCodeRuleField.Create();
        }

        public List<LabelCodeRuleFieldInfo> GetLabelCodeRuleFields()
        {
            return dal.GetLabelCodeRuleFields();
        }

        public List<LabelCodeRuleFieldInfo> GetLabelCodeRuleFieldsByRuleId(int ruleId)
        {
            return dal.GetLabelCodeRuleFieldsByRuleId(ruleId);
        }

        public List<LabelCodeRuleFieldInfo> GetRuleFieldByCustomer(string customerId)
        {
            return dal.GetRuleFieldByCustomer(customerId);
        }

        public string InsertLabelCodeRuleFields(List<LabelCodeRuleFieldInfo> infos)
        {
            Normalize(infos);
            string validateMessage = Validate(infos);
            if (!string.IsNullOrWhiteSpace(validateMessage))
            {
                return validateMessage;
            }

            ParamterInfo paramterInfo = dal.InsertLabelCodeRuleFields(infos);
            IList source = new ArrayList();
            source.Add(paramterInfo);
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string UpdateLabelCodeRuleFields(List<LabelCodeRuleFieldInfo> infos)
        {
            Normalize(infos);
            string validateMessage = Validate(infos);
            if (!string.IsNullOrWhiteSpace(validateMessage))
            {
                return validateMessage;
            }

            ParamterInfo paramterInfo = dal.UpdateLabelCodeRuleFields(infos);
            IList source = new ArrayList();
            source.Add(paramterInfo);
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string DeleteLabelCodeRuleFields(List<LabelCodeRuleFieldInfo> infos)
        {
            ParamterInfo paramterInfo = dal.DeleteLabelCodeRuleFields(infos);
            IList source = new ArrayList();
            source.Add(paramterInfo);
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        private static void Normalize(List<LabelCodeRuleFieldInfo> infos)
        {
            if (infos == null)
            {
                return;
            }

            foreach (LabelCodeRuleFieldInfo info in infos)
            {
                if (info == null)
                {
                    continue;
                }

                info.FieldName = (info.FieldName ?? string.Empty).Trim();
                info.DataType = string.IsNullOrWhiteSpace(info.DataType) ? null : info.DataType.Trim();
                info.KeyCode = string.IsNullOrWhiteSpace(info.KeyCode) ? null : info.KeyCode.Trim();
                if (!info.Required.HasValue)
                {
                    info.Required = false;
                }

                if (!info.SortNo.HasValue)
                {
                    info.SortNo = 0;
                }
            }
        }

        private static string Validate(List<LabelCodeRuleFieldInfo> infos)
        {
            if (infos == null)
            {
                return string.Empty;
            }

            foreach (LabelCodeRuleFieldInfo info in infos)
            {
                if (info == null)
                {
                    continue;
                }

                if (!info.RuleId.HasValue)
                {
                    return "请填写规则ID。";
                }

                if (string.IsNullOrWhiteSpace(info.FieldName))
                {
                    return "请填写目标字段。";
                }
            }

            return string.Empty;
        }
    }
}
