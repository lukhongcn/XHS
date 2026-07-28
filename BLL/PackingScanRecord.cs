using System.Collections;
using System.Collections.Generic;
using ModuleWorkFlow.business;
using XHS.IDAL;
using XHS.Model;

namespace XHS.BLL
{
    /// <summary>
    /// 装箱扫描记录业务层。
    /// </summary>
    public class PackingScanRecord
    {
        private readonly IPackingScanRecord dal;

        public PackingScanRecord()
        {
            dal = XHS.DALFactory.PackingScanRecord.Create();
        }

        public List<PackingScanRecordInfo> GetExPackingScanRecords(string supplyBatchNo, string partNo, string cartonNo)
        {
            return dal.GetExPackingScanRecords(supplyBatchNo, partNo, cartonNo);
        }

        public List<PackingScanRecordInfo> GetExPackingScanRecordsByQRCodes(string kdQRCode, string packingQRCode, string materialLabelQRCode)
        {
            return dal.GetExPackingScanRecordsByQRCodes(kdQRCode, packingQRCode, materialLabelQRCode);
        }

        public string InsertPackingScanRecord(List<PackingScanRecordInfo> packingScanRecordInfos)
        {
            ParamterInfo paramterInfo = dal.InsertPackingScanRecord(packingScanRecordInfos);
            IList source = new ArrayList();
            source.Add(paramterInfo);

            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string UpdatePackingScanRecord(List<PackingScanRecordInfo> packingScanRecordInfos)
        {
            ParamterInfo paramterInfo = dal.UpdatePackingScanRecord(packingScanRecordInfos);
            IList source = new ArrayList();
            source.Add(paramterInfo);

            return Common.Save(source) ? string.Empty : "保存失败。";
        }
    }
}
