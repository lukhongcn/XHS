using System.Collections.Generic;
using System;
using XHS.IDAL;
using XHS.Model;

namespace XHS.BLL
{
    /// <summary>
    /// PLC 结果业务层。
    /// </summary>
    public class PLCResult
    {
        private readonly IPLCResult dal;

        public PLCResult()
        {
            dal = XHS.DALFactory.PLCResult.Create();
        }

        public List<PLCResultInfo> GetPLCResultsById(int id)
        {
            return dal.GetPLCResultsById(id);
        }

        public List<PLCResultInfo> GetPLCResults(string partNo, string qrCode, DateTime? startDate, DateTime? endDate)
        {
            return dal.GetPLCResults(partNo, qrCode, startDate, endDate);
        }

        public bool InsertPLCResult(PLCResultInfo info)
        {
            return dal.InsertPLCResult(info);
        }

        public PLCResultInfo GetLatestUndealt()
        {
            return dal.GetLatestUndealt();
        }

        public bool MarkDeal(int id)
        {
            return id > 0 && dal.MarkDeal(id);
        }
    }
}
