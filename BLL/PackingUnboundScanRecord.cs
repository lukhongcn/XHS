using System.Collections.Generic;
using XHS.IDAL;
using XHS.Model;

namespace XHS.BLL
{
    /// <summary>解除绑定扫描历史业务层。</summary>
    public class PackingUnboundScanRecord
    {
        private readonly IPackingUnboundScanRecord dal;

        public PackingUnboundScanRecord()
        {
            dal = XHS.DALFactory.PackingUnboundScanRecord.Create();
        }

        public List<PackingUnboundScanRecordInfo> GetByPackingExceptionId(long packingExceptionId)
        {
            return dal.GetByPackingExceptionId(packingExceptionId);
        }
    }
}
