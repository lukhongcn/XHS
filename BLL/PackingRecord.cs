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

        /// <summary>按 KD 码、零件编号和零件码查询装箱记录。</summary>
        public List<PackingRecordInfo> SearchPackingRecords(string kdQRCode, string partNo, string partCode)
        {
            return dal.SearchPackingRecords(kdQRCode, partNo, partCode);
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

        public PackingRecordInfo GetPackingRecordByKdQRCode(string kdQRCode)
        {
            try
            {
                using (var connection = new System.Data.SqlClient.SqlConnection(
                    System.Configuration.ConfigurationManager.AppSettings["MsSQLConnString"]))
                {
                    connection.Open();
                    return dal.GetPackingRecordByKdQRCode(kdQRCode, connection, null);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>按主键只读获取装箱记录。</summary>
        public PackingRecordInfo GetPackingRecordById(long packingId)
        {
            try
            {
                using (var connection = new System.Data.SqlClient.SqlConnection(
                    System.Configuration.ConfigurationManager.AppSettings["MsSQLConnString"]))
                {
                    connection.Open();
                    return dal.GetPackingRecordById(packingId, connection, null);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
