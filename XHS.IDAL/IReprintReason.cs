using System.Collections.Generic;
using XHS.Model;

namespace XHS.IDAL
{
    /// <summary>
    /// 补打原因字典数据访问接口。
    /// </summary>
    public interface IReprintReason
    {
        List<ReprintReasonInfo> GetReprintReasons();

        List<ReprintReasonInfo> GetEnabledReprintReasons();

        ParamterInfo InsertReprintReason(List<ReprintReasonInfo> reprintReasonInfos);

        ParamterInfo UpdateReprintReason(List<ReprintReasonInfo> reprintReasonInfos);

        ParamterInfo DeleteReprintReason(List<ReprintReasonInfo> reprintReasonInfos);
    }
}
