using System.Collections;
using System.Collections.Generic;
using ModuleWorkFlow.business;
using XHS.IDAL;
using XHS.Model;

namespace BLL
{
    /// <summary>
    /// 补打原因字典业务层。
    /// </summary>
    public class ReprintReason
    {
        private readonly IReprintReason dal;

        public ReprintReason()
        {
            dal = XHS.DALFactory.ReprintReason.Create();
        }

        public List<ReprintReasonInfo> GetReprintReasons()
        {
            return dal.GetReprintReasons();
        }

        public List<ReprintReasonInfo> GetEnabledReprintReasons()
        {
            return dal.GetEnabledReprintReasons();
        }

        public string InsertReprintReason(List<ReprintReasonInfo> reprintReasonInfos)
        {
            NormalizeReprintReasons(reprintReasonInfos);
            string validateMessage = ValidateReprintReasons(reprintReasonInfos);
            if (!string.IsNullOrWhiteSpace(validateMessage))
            {
                return validateMessage;
            }

            ParamterInfo paramterInfo = dal.InsertReprintReason(reprintReasonInfos);
            IList source = new ArrayList();
            source.Add(paramterInfo);

            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string UpdateReprintReason(List<ReprintReasonInfo> reprintReasonInfos)
        {
            NormalizeReprintReasons(reprintReasonInfos);
            string validateMessage = ValidateReprintReasons(reprintReasonInfos);
            if (!string.IsNullOrWhiteSpace(validateMessage))
            {
                return validateMessage;
            }

            ParamterInfo paramterInfo = dal.UpdateReprintReason(reprintReasonInfos);
            IList source = new ArrayList();
            source.Add(paramterInfo);

            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string DeleteReprintReason(List<ReprintReasonInfo> reprintReasonInfos)
        {
            ParamterInfo paramterInfo = dal.DeleteReprintReason(reprintReasonInfos);
            IList source = new ArrayList();
            source.Add(paramterInfo);

            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        private static void NormalizeReprintReasons(List<ReprintReasonInfo> reprintReasonInfos)
        {
            if (reprintReasonInfos == null)
            {
                return;
            }

            foreach (ReprintReasonInfo reprintReasonInfo in reprintReasonInfos)
            {
                if (reprintReasonInfo == null)
                {
                    continue;
                }

                reprintReasonInfo.ReasonName = string.IsNullOrWhiteSpace(reprintReasonInfo.ReasonName)
                    ? string.Empty
                    : reprintReasonInfo.ReasonName.Trim();

                if (!reprintReasonInfo.SortNo.HasValue)
                {
                    reprintReasonInfo.SortNo = 0;
                }

                if (!reprintReasonInfo.IsEnable.HasValue)
                {
                    reprintReasonInfo.IsEnable = true;
                }
            }
        }

        private static string ValidateReprintReasons(List<ReprintReasonInfo> reprintReasonInfos)
        {
            if (reprintReasonInfos == null)
            {
                return string.Empty;
            }

            foreach (ReprintReasonInfo reprintReasonInfo in reprintReasonInfos)
            {
                if (reprintReasonInfo == null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(reprintReasonInfo.ReasonName))
                {
                    return "请填写补打原因。";
                }
            }

            return string.Empty;
        }
    }
}
