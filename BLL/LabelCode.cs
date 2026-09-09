using System.Collections;
using System.Collections.Generic;
using ModuleWorkFlow.business;
using XHS.IDAL;
using XHS.Model;

namespace BLL
{
    /// <summary>
    /// 标签编码解析规则业务层。
    /// </summary>
    public class LabelCode
    {
        private readonly ILabelCode dal;

        public LabelCode()
        {
            dal = XHS.DALFactory.LabelCode.Create();
        }

        public List<LabelCodeInfo> GetLabelCodes()
        {
            return dal.GetLabelCodes();
        }

        public List<LabelCodeInfo> GetEnabledLabelCodes()
        {
            return dal.GetEnabledLabelCodes();
        }

        public List<LabelCodeInfo> GetEnabledLabelCodes(int? customerId, string labelType)
        {
            return dal.GetEnabledLabelCodes(customerId, labelType);
        }

        public string InsertLabelCodes(List<LabelCodeInfo> labelCodeInfos)
        {
            NormalizeLabelCodes(labelCodeInfos);
            string validateMessage = ValidateLabelCodes(labelCodeInfos);
            if (!string.IsNullOrWhiteSpace(validateMessage))
            {
                return validateMessage;
            }

            ParamterInfo paramterInfo = dal.InsertLabelCodes(labelCodeInfos);
            IList source = new ArrayList();
            source.Add(paramterInfo);
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string UpdateLabelCodes(List<LabelCodeInfo> labelCodeInfos)
        {
            NormalizeLabelCodes(labelCodeInfos);
            string validateMessage = ValidateLabelCodes(labelCodeInfos);
            if (!string.IsNullOrWhiteSpace(validateMessage))
            {
                return validateMessage;
            }

            ParamterInfo paramterInfo = dal.UpdateLabelCodes(labelCodeInfos);
            IList source = new ArrayList();
            source.Add(paramterInfo);
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string DeleteLabelCodes(List<LabelCodeInfo> labelCodeInfos)
        {
            ParamterInfo paramterInfo = dal.DeleteLabelCodes(labelCodeInfos);
            IList source = new ArrayList();
            source.Add(paramterInfo);
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        private static void NormalizeLabelCodes(List<LabelCodeInfo> labelCodeInfos)
        {
            if (labelCodeInfos == null)
            {
                return;
            }

            foreach (LabelCodeInfo info in labelCodeInfos)
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
                if (!info.Version.HasValue)
                {
                    info.Version = 1;
                }

                if (!info.Enabled.HasValue)
                {
                    info.Enabled = true;
                }
            }
        }

        private static string ValidateLabelCodes(List<LabelCodeInfo> labelCodeInfos)
        {
            if (labelCodeInfos == null)
            {
                return string.Empty;
            }

            foreach (LabelCodeInfo info in labelCodeInfos)
            {
                if (info == null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(info.RuleName))
                {
                    return "请填写标签编码规则名称。";
                }

                if (string.IsNullOrWhiteSpace(info.LabelType))
                {
                    return "请填写标签类型。";
                }

                if (string.IsNullOrWhiteSpace(info.ParseType))
                {
                    return "请填写解析方式。";
                }
            }

            return string.Empty;
        }
    }
}
