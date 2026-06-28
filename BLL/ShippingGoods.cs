using System.Collections.Generic;
using XHS.Model;
using ModuleWorkFlow.business;
using XHS.IDAL;
using System.Collections;
using System;

namespace BLL
{
    /// <summary>
    /// 出货货品业务层。
    /// </summary>
    public class ShippingGoods
    {
        private readonly IShippingGoods dal;

        public ShippingGoods()
        {
            dal = XHS.DALFactory.ShippingGoods.Create();
        }

        public List<ShippingGoodsInfo> GetShippingGoods()
        {
            return dal.GetShippingGoods();
        }

        public List<ShippingGoodsInfo> GetShippingGoods(string partNo, string partName, string supplyBatchNo)
        {
            return dal.GetShippingGoods(partNo, partName, supplyBatchNo);
        }

        public List<ShippingGoodsInfo> GetShippingGoodsBySupplyBatchNo(string supplyBatchNo, string cartonNo)
        {
            return dal.GetShippingGoodsBySupplyBatchNo(supplyBatchNo, cartonNo);
        }

        public List<ShippingGoodsInfo> GetShippingGoodsByBusinessKey(string supplyBatchNo, string partNo, string cartonNo)
        {
            return dal.GetShippingGoodsByBusinessKey(supplyBatchNo, partNo, cartonNo);
        }

        public string InsertShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            NormalizeShippingGoods(shippingGoodsInfos);
            ParamterInfo paramterInfo = dal.InsertShippingGoods(shippingGoodsInfos);
            IList source = new ArrayList();
            source.Add(paramterInfo);

            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public ParamterInfo BuildInsertShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            NormalizeShippingGoods(shippingGoodsInfos);
            return dal.InsertShippingGoods(shippingGoodsInfos);
        }

        public ParamterInfo UpdateShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            NormalizeShippingGoods(shippingGoodsInfos);
            return dal.UpdateShippingGoods(shippingGoodsInfos);
        }

        public ParamterInfo DeleteShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            return dal.DeleteShippingGoods(shippingGoodsInfos);
        }

        public ParamterInfo DeleteShippingGoodsByBusinessKey(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            return dal.DeleteShippingGoodsByBusinessKey(shippingGoodsInfos);
        }

        public string SaveUploadEditShippingGoods(string supplyBatchNo, string creater, List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            if (string.IsNullOrWhiteSpace(supplyBatchNo))
            {
                return "上传修改模式缺少供货批次号。";
            }

            if (shippingGoodsInfos == null || shippingGoodsInfos.Count == 0)
            {
                return "没有可保存的出货货品数据。";
            }

            DateTime uploadDate = DateTime.Now;
            IList source = new ArrayList();
            Dictionary<string, List<ShippingGoodsInfo>> groupedUploadInfos = new Dictionary<string, List<ShippingGoodsInfo>>(StringComparer.OrdinalIgnoreCase);
            List<ShippingGoodsInfo> existingBatchShippingGoodsInfos = dal.GetShippingGoodsBySupplyBatchNo(supplyBatchNo, string.Empty);

            if (existingBatchShippingGoodsInfos == null || existingBatchShippingGoodsInfos.Count == 0)
            {
                return string.Format("批次“{0}”不存在，请使用新增方法。", supplyBatchNo);
            }

            foreach (ShippingGoodsInfo shippingGoodsInfo in shippingGoodsInfos)
            {
                if (shippingGoodsInfo == null)
                {
                    continue;
                }

                if (!string.Equals((shippingGoodsInfo.SupplyBatchNo ?? string.Empty).Trim(), supplyBatchNo, StringComparison.OrdinalIgnoreCase))
                {
                    return string.Format("上传文件中存在批次“{0}”，与当前上传修改批次“{1}”不一致。", shippingGoodsInfo.SupplyBatchNo, supplyBatchNo);
                }

                string currentSupplyBatchNo = (shippingGoodsInfo.SupplyBatchNo ?? string.Empty).Trim();
                string partNo = (shippingGoodsInfo.PartNo ?? string.Empty).Trim();
                string groupKey = string.Format("{0}|{1}", currentSupplyBatchNo, partNo);
                if (!groupedUploadInfos.ContainsKey(groupKey))
                {
                    groupedUploadInfos[groupKey] = new List<ShippingGoodsInfo>();
                }

                groupedUploadInfos[groupKey].Add(shippingGoodsInfo);
            }

            foreach (KeyValuePair<string, List<ShippingGoodsInfo>> entry in groupedUploadInfos)
            {
                ShippingGoodsInfo firstUploadShippingGoodsInfo = entry.Value[0];
                string currentSupplyBatchNo = (firstUploadShippingGoodsInfo.SupplyBatchNo ?? string.Empty).Trim();
                string currentPartNo = (firstUploadShippingGoodsInfo.PartNo ?? string.Empty).Trim();
                List<ShippingGoodsInfo> existingPartShippingGoodsInfos = dal.GetShippingGoods(currentPartNo, string.Empty, currentSupplyBatchNo);
                if (existingPartShippingGoodsInfos != null && existingPartShippingGoodsInfos.Count > 0)
                {
                    foreach (ShippingGoodsInfo existingShippingGoodsInfo in existingPartShippingGoodsInfos)
                    {
                        if ((existingShippingGoodsInfo.PrintCount ?? 0) > 0)
                        {
                            return string.Format("批次“{0}”下零件编号“{1}”已有打印记录，不允许修改。", currentSupplyBatchNo, existingShippingGoodsInfo.PartNo);
                        }
                    }

                    source.Add(dal.DeleteShippingGoodsByBusinessKey(existingPartShippingGoodsInfos));
                }

                foreach (ShippingGoodsInfo uploadShippingGoodsInfo in entry.Value)
                {
                    uploadShippingGoodsInfo.Id = null;
                    uploadShippingGoodsInfo.SupplyBatchNo = currentSupplyBatchNo;
                    uploadShippingGoodsInfo.QrCode = null;
                    uploadShippingGoodsInfo.OutBoxQRCode = null;
                    uploadShippingGoodsInfo.Status = ShippingGoodsStatusInfo.UnPrinted;
                    uploadShippingGoodsInfo.PrintCount = 0;
                    uploadShippingGoodsInfo.Creater = creater;
                    uploadShippingGoodsInfo.CreatDate = uploadDate;
                }

                source.Add(BuildInsertShippingGoods(entry.Value));
            }

            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string CompleteShippingGoodsPrint(string supplyBatchNo, string partNo, string cartonNo)
        {
            List<ShippingGoodsInfo> shippingGoodsInfos = dal.GetShippingGoodsByBusinessKey(supplyBatchNo, partNo, cartonNo);
            if (shippingGoodsInfos == null || shippingGoodsInfos.Count == 0)
            {
                return "未找到对应的出货货品数据。";
            }

            foreach (ShippingGoodsInfo shippingGoodsInfo in shippingGoodsInfos)
            {
                if (shippingGoodsInfo == null)
                {
                    continue;
                }

                shippingGoodsInfo.Status = ShippingGoodsStatusInfo.Printed;
                shippingGoodsInfo.PrintCount = (shippingGoodsInfo.PrintCount ?? 0) + 1;
            }

            ParamterInfo paramterInfo = dal.UpdateShippingGoods(shippingGoodsInfos);
            IList source = new ArrayList();
            source.Add(paramterInfo);

            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        private static void NormalizeShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            if (shippingGoodsInfos == null)
            {
                return;
            }

            foreach (ShippingGoodsInfo shippingGoodsInfo in shippingGoodsInfos)
            {
                if (shippingGoodsInfo == null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(shippingGoodsInfo.Status))
                {
                    shippingGoodsInfo.Status = ShippingGoodsStatusInfo.UnPrinted;
                }

                if (!shippingGoodsInfo.PrintCount.HasValue)
                {
                    shippingGoodsInfo.PrintCount = 0;
                }
            }
        }
    }
}
