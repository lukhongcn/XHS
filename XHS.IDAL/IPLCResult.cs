using System.Collections.Generic;
using XHS.Model;

namespace XHS.IDAL
{
    /// <summary>
    /// PLC 结果数据访问接口。
    /// </summary>
    public interface IPLCResult
    {
        List<PLCResultInfo> GetPLCResultsById(int id);

        bool InsertPLCResult(PLCResultInfo info);
    }
}
