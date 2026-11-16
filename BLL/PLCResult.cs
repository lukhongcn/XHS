using System.Collections.Generic;
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

        public bool InsertPLCResult(PLCResultInfo info)
        {
            return dal.InsertPLCResult(info);
        }
    }
}
