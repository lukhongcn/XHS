using System.Collections;
using System.Collections.Generic;
using ModuleWorkFlow.business;
using XHS.IDAL;
using XHS.Model;

namespace BLL
{
    /// <summary>
    /// 批次规则业务层。
    /// </summary>
    public class BatchRule
    {
        private readonly IBatchRule dal;

        public BatchRule()
        {
            dal = XHS.DALFactory.BatchRule.Create();
        }

        public List<BatchRuleInfo> GetBatchRules()
        {
            return dal.GetBatchRules();
        }

        public BatchRuleInfo GetBatchRule(int batchRuleId)
        {
            return dal.GetBatchRule(batchRuleId);
        }

        public string InsertBatchRules(List<BatchRuleInfo> infos)
        {
            Normalize(infos);
            string validateMessage = Validate(infos);
            if (!string.IsNullOrWhiteSpace(validateMessage))
            {
                return validateMessage;
            }

            ParamterInfo paramterInfo = dal.InsertBatchRules(infos);
            IList source = new ArrayList();
            source.Add(paramterInfo);
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string UpdateBatchRules(List<BatchRuleInfo> infos)
        {
            Normalize(infos);
            string validateMessage = Validate(infos);
            if (!string.IsNullOrWhiteSpace(validateMessage))
            {
                return validateMessage;
            }

            ParamterInfo paramterInfo = dal.UpdateBatchRules(infos);
            IList source = new ArrayList();
            source.Add(paramterInfo);
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string DeleteBatchRules(List<BatchRuleInfo> infos)
        {
            ParamterInfo paramterInfo = dal.DeleteBatchRules(infos);
            IList source = new ArrayList();
            source.Add(paramterInfo);
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        private static void Normalize(List<BatchRuleInfo> infos)
        {
            if (infos == null)
            {
                return;
            }

            foreach (BatchRuleInfo info in infos)
            {
                if (info == null)
                {
                    continue;
                }

                info.RuleName = (info.RuleName ?? string.Empty).Trim();
                info.Format = (info.Format ?? string.Empty).Trim();
                info.Remark = string.IsNullOrWhiteSpace(info.Remark) ? null : info.Remark.Trim();
            }
        }

        private static string Validate(List<BatchRuleInfo> infos)
        {
            if (infos == null)
            {
                return string.Empty;
            }

            foreach (BatchRuleInfo info in infos)
            {
                if (info == null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(info.RuleName))
                {
                    return "请填写规则名称。";
                }

                if (string.IsNullOrWhiteSpace(info.Format))
                {
                    return "请填写格式。";
                }

                if (info.Length == null || info.Length.Value <= 0)
                {
                    return "长度必须大于 0。";
                }
            }

            return string.Empty;
        }
    }
}
