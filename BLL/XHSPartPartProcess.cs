using System.Collections.Generic;
using XHS.IDAL;
using XHS.Model;

namespace XHS.BLL
{
    /// <summary>
    /// 零件工序业务层。
    /// </summary>
    public class XHSPartPartProcess
    {
        private readonly IXHSPartPartProcess dal;

        public XHSPartPartProcess()
        {
            dal = XHS.DALFactory.XHSPartPartProcess.Create();
        }

        public List<XHSPartPartProcessInfo> GetPartProcesses(
            string customerAbbr,
            string partNo,
            string barcode,
            IList<string> qrDateCodes)
        {
            return dal.GetPartProcesses(customerAbbr, partNo, barcode, qrDateCodes);
        }
    }
}
