using System.Collections.Generic;
using System;
using XHS.Model;

namespace XHS.IDAL
{
    /// <summary>
    /// PLC 结果数据访问接口。
    /// </summary>
    public interface IPLCResult
    {
        List<PLCResultInfo> GetPLCResultsById(int id);

        List<PLCResultInfo> GetPLCResults(string partNo, string qrCode, DateTime? startDate, DateTime? endDate);

        bool InsertPLCResult(PLCResultInfo info);
        PLCResultInfo GetLatestUndealt();
        bool MarkDeal(int id);
    }
}
