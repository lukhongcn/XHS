using System.Collections.Generic;
using XHS.Model;

namespace XHS.IDAL
{
    /// <summary>
    /// 标签编码解析规则数据访问接口。
    /// </summary>
    public interface ILabelCode
    {
        List<LabelCodeInfo> GetLabelCodes();

        List<LabelCodeInfo> GetEnabledLabelCodes();

        List<LabelCodeInfo> GetEnabledLabelCodes(int? customerId, string labelType);

        ParamterInfo InsertLabelCodes(List<LabelCodeInfo> labelCodeInfos);

        ParamterInfo UpdateLabelCodes(List<LabelCodeInfo> labelCodeInfos);

        ParamterInfo DeleteLabelCodes(List<LabelCodeInfo> labelCodeInfos);
    }
}
