using System.Collections;
using System.Collections.Generic;
using ModuleWorkFlow.business;
using XHS.IDAL;
using XHS.Model;

namespace XHS.BLL
{
    /// <summary>
    /// 装箱记录业务层。
    /// </summary>
    public class PackingRecord
    {
        private readonly IPackingRecord dal;

        public PackingRecord()
        {
            dal = XHS.DALFactory.PackingRecord.Create();
        }

        public List<PackingRecordInfo> GetExPackingRecords(string supplyBatchNo, string partNo)
        {
            return dal.GetExPackingRecords(supplyBatchNo, partNo);
        }

        public string InsertPackingRecord(List<PackingRecordInfo> packingRecordInfos)
        {
            ParamterInfo paramterInfo = dal.InsertPackingRecord(packingRecordInfos);
            IList source = new ArrayList();
            source.Add(paramterInfo);

            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string UpdatePackingRecord(List<PackingRecordInfo> packingRecordInfos)
        {
            ParamterInfo paramterInfo = dal.UpdatePackingRecord(packingRecordInfos);
            IList source = new ArrayList();
            source.Add(paramterInfo);

            return Common.Save(source) ? string.Empty : "保存失败。";
        }
    }
}
