using System;
using System.Collections;
using System.Collections.Generic;
using BLL;
using ModuleWorkFlow.business;
using XHS.IDAL;
using XHS.Model;

namespace XHS.BLL
{
    /// <summary>
    /// 装箱异常记录业务层。
    /// </summary>
    public class PackingException
    {
        private readonly IPackingException dal;

        public PackingException()
        {
            dal = XHS.DALFactory.PackingException.Create();
        }

        public List<PackingExceptionInfo> GetExPackingExceptions(string boxCode)
        {
            return dal.GetExPackingExceptions(boxCode);
        }

        public List<PackingExceptionInfo> SearchPackingExceptions(string kdCode, int? status)
        {
            List<PackingExceptionInfo> records = dal.SearchPackingExceptions(kdCode, status) ?? new List<PackingExceptionInfo>();
            // 从 KD 标签解析零件编号
            ParseKDPartNo(records);
            return records;
        }

        public List<PackingExceptionInfo> SearchPackingExceptions(string kdCode, string partNo, DateTime? dateFrom, DateTime? dateTo, int? status)
        {
            List<PackingExceptionInfo> records = dal.SearchPackingExceptions(kdCode, partNo, dateFrom, dateTo, status) ?? new List<PackingExceptionInfo>();
            // 从 KD 标签解析零件编号
            ParseKDPartNo(records);
            return records;
        }

        private static void ParseKDPartNo(List<PackingExceptionInfo> records)
        {
            var barcodeParser = new FactoryBarcodeParser();
            foreach (PackingExceptionInfo record in records)
            {
                if (!string.IsNullOrWhiteSpace(record.KDQRCode))
                {
                    ShippingGoodsInfo kdInfo = barcodeParser.ParseShippingGoodsBarcode(record.KDQRCode);
                    record.KDPartNo = kdInfo != null ? kdInfo.PartNo : string.Empty;
                }
            }
        }

        public List<PackingExceptionInfo> GetPackingExceptionByLockToken(string lockToken)
        {
            return dal.GetPackingExceptionByLockToken(lockToken);
        }

        public string InsertPackingException(List<PackingExceptionInfo> packingExceptionInfos)
        {
            ParamterInfo paramterInfo = dal.InsertPackingException(packingExceptionInfos);
            IList source = new ArrayList();
            source.Add(paramterInfo);

            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string UpdatePackingException(List<PackingExceptionInfo> packingExceptionInfos)
        {
            ParamterInfo paramterInfo = dal.UpdatePackingException(packingExceptionInfos);
            IList source = new ArrayList();
            source.Add(paramterInfo);

            return Common.Save(source) ? string.Empty : "保存失败。";
        }
    }
}
