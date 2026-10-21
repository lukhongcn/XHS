using XHS.Model;

namespace XHS.IDAL
{
    public interface ILabelBindingFailure
    {
        bool Insert(LabelBindingPdaScanFailureInfo info);
        System.Collections.Generic.List<LabelBindingPdaScanFailureInfo> GetRecords(
            string scanRawCode,
            string failureType,
            System.DateTime startTime,
            System.DateTime endTime);
    }
}

