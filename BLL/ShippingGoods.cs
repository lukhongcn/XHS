using System.Collections.Generic;
using XHS.Model;
using ModuleWorkFlow.business;
using XHS.IDAL;
using System.Collections;
using System;
using System.Linq;

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

        public List<ShippingGoodsInfo> GetShippingGoods(string partNo, string partName, string supplyBatchNo, string closeStatus)
        {
            return dal.GetShippingGoods(partNo, partName, supplyBatchNo, closeStatus);
        }

        public List<ShippingGoodsInfo> GetShippingGoodsBySupplyBatchNo(string supplyBatchNo, string cartonNo)
        {
            return dal.GetShippingGoodsBySupplyBatchNo(supplyBatchNo, cartonNo);
        }

        public List<ShippingGoodsInfo> GetShippingGoodsByBusinessKey(string supplyBatchNo, string partNo, string cartonNo)
        {
            return dal.GetShippingGoodsByBusinessKey(supplyBatchNo, partNo, cartonNo);
        }

        public List<ShippingGoodsInfo> GetShippingGoodsByBusinessKey(string supplyBatchNo, string partNo, string cartonNo, string deliveryNo)
        {
            return dal.GetShippingGoodsByBusinessKey(supplyBatchNo, partNo, cartonNo, deliveryNo);
        }

        public string GetNextAutoDeliveryNo(DateTime date)
        {
            return dal.GetNextAutoDeliveryNo(date);
        }

        public List<ShippingGoodsInfo> BuildNewShippingGoodsInfos(
            ShippingGoodsInfo templateShippingGoodsInfo,
            int orderQuantity,
            int boxQuantity,
            int boxCount)
        {
            if (templateShippingGoodsInfo == null)
            {
                throw new ArgumentNullException("templateShippingGoodsInfo");
            }

            if (orderQuantity <= 0)
            {
                throw new ArgumentException("订单数量必须为大于 0 的整数。", "orderQuantity");
            }

            if (boxQuantity <= 0)
            {
                throw new ArgumentException("数量必须为大于 0 的整数。", "boxQuantity");
            }

            if (boxCount <= 0)
            {
                throw new ArgumentException("纸箱数量必须为大于 0 的整数。", "boxCount");
            }

            var shippingGoodsInfos = new List<ShippingGoodsInfo>();
            int producedQuantity = 0;
            for (int i = 1; i <= boxCount; i++)
            {
                int currentQuantity = i == boxCount
                    ? orderQuantity - producedQuantity
                    : boxQuantity;

                if (currentQuantity <= 0)
                {
                    throw new ArgumentException("订单数量不足以生成指定的纸箱数量。", "orderQuantity");
                }

                shippingGoodsInfos.Add(new ShippingGoodsInfo
                {
                    SupplierCode = templateShippingGoodsInfo.SupplierCode,
                    PartNo = templateShippingGoodsInfo.PartNo,
                    PartChineseName = templateShippingGoodsInfo.PartChineseName,
                    PartEnglishName = templateShippingGoodsInfo.PartEnglishName,
                    Quantity = currentQuantity,
                    SupplyBatchNo = templateShippingGoodsInfo.SupplyBatchNo,
                    DeliveryNo = templateShippingGoodsInfo.DeliveryNo,
                    StackLayerCount = templateShippingGoodsInfo.StackLayerCount,
                    ProductionDate = templateShippingGoodsInfo.ProductionDate,
                    InspectionConfirmDate = templateShippingGoodsInfo.InspectionConfirmDate,
                    CartonNo = boxCount.ToString() + "-" + i.ToString(),
                    QrCode = templateShippingGoodsInfo.QrCode,
                    OutBoxQRCode = templateShippingGoodsInfo.OutBoxQRCode,
                    Status = templateShippingGoodsInfo.Status,
                    PrintCount = templateShippingGoodsInfo.PrintCount,
                    Creater = templateShippingGoodsInfo.Creater,
                    CreatDate = templateShippingGoodsInfo.CreatDate,
                    SingleBoxGrossWeight = templateShippingGoodsInfo.SingleBoxGrossWeight
                });

                producedQuantity += currentQuantity;
            }

            if (producedQuantity != orderQuantity)
            {
                throw new ArgumentException("生成的出货数量合计与订单数量不一致。", "orderQuantity");
            }

            return shippingGoodsInfos;
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

        public ParamterInfo UpdateShippingGoodsByBusinessKey(List<ShippingGoodsInfo> shippingGoodsInfos, string deliveryNo)
        {
            NormalizeShippingGoods(shippingGoodsInfos);
            return dal.UpdateShippingGoodsByBusinessKey(shippingGoodsInfos, deliveryNo);
        }

        public string UpdatePrintedDeliveryNo(string supplyBatchNo, string partNo, string cartonNo, string oldDeliveryNo, string newDeliveryNo)
        {
            if (string.IsNullOrWhiteSpace(supplyBatchNo) || string.IsNullOrWhiteSpace(partNo) || string.IsNullOrWhiteSpace(cartonNo))
            {
                return "编辑模式缺少供货批次号、零件编号或箱号。";
            }

            List<ShippingGoodsInfo> infos = dal.GetShippingGoods(partNo.Trim(), string.Empty, supplyBatchNo.Trim());
            infos = infos == null
                ? new List<ShippingGoodsInfo>()
                : infos.FindAll(info => info != null && string.Equals((info.DeliveryNo ?? string.Empty).Trim(), (oldDeliveryNo ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase));
            if (infos == null || infos.Count == 0)
            {
                return "未找到对应的已打印出货货品数据。";
            }

            if (infos.Any(info => (info.PrintCount ?? 0) <= 0 && !string.Equals(info.Status, ShippingGoodsStatusInfo.Printed, StringComparison.OrdinalIgnoreCase)))
            {
                return "当前出货货品尚未打印，请使用普通编辑。";
            }

            IList source = new ArrayList { dal.UpdatePrintedDeliveryNo(supplyBatchNo.Trim(), partNo.Trim(), cartonNo.Trim(), oldDeliveryNo ?? string.Empty, newDeliveryNo ?? string.Empty) };
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string UpdateShippingGoodsClose(string supplyBatchNo, string partNo, string deliveryNo, string closer, DateTime closeDate)
        {
            if (string.IsNullOrWhiteSpace(supplyBatchNo) ||
                string.IsNullOrWhiteSpace(partNo) ||
                string.IsNullOrWhiteSpace(deliveryNo))
            {
                return "结案缺少供货批次号、零件编号或配送单号。";
            }

            ParamterInfo paramterInfo = dal.UpdateShippingGoodsClose(
                supplyBatchNo.Trim(),
                partNo.Trim(),
                deliveryNo.Trim(),
                closer == null ? string.Empty : closer.Trim(),
                closeDate);
            IList source = new ArrayList();
            source.Add(paramterInfo);

            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public ParamterInfo DeleteShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            return dal.DeleteShippingGoods(shippingGoodsInfos);
        }

        public ParamterInfo DeleteShippingGoodsByBusinessKey(List<ShippingGoodsInfo> shippingGoodsInfos, string deliveryNo)
        {
            return dal.DeleteShippingGoodsByBusinessKey(shippingGoodsInfos, deliveryNo);
        }

        public ParamterInfo DeleteShippingGoodsByBusinessKey(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            return dal.DeleteShippingGoodsByBusinessKey(shippingGoodsInfos);
        }

        public string SaveDeleteShippingGoods(string supplyBatchNo, string partNo, string deliveryNo)
        {
            if (string.IsNullOrWhiteSpace(supplyBatchNo) || string.IsNullOrWhiteSpace(partNo) || string.IsNullOrWhiteSpace(deliveryNo))
            {
                return "删除模式缺少供货批次号、零件编号或配送单号。";
            }

            string normalizedSupplyBatchNo = supplyBatchNo.Trim();
            string normalizedPartNo = partNo.Trim();
            string normalizedDeliveryNo = deliveryNo.Trim();
            List<ShippingGoodsInfo> existingPartShippingGoodsInfos = dal.GetShippingGoods(normalizedPartNo, string.Empty, normalizedSupplyBatchNo);
            existingPartShippingGoodsInfos = existingPartShippingGoodsInfos == null
                ? new List<ShippingGoodsInfo>()
                : existingPartShippingGoodsInfos.FindAll(info => string.Equals((info.DeliveryNo ?? string.Empty).Trim(), normalizedDeliveryNo, StringComparison.OrdinalIgnoreCase));
            if (existingPartShippingGoodsInfos == null || existingPartShippingGoodsInfos.Count == 0)
            {
                return string.Format("未找到批次“{0}”下零件编号“{1}”的出货货品数据。", normalizedSupplyBatchNo, normalizedPartNo);
            }

            foreach (ShippingGoodsInfo existingShippingGoodsInfo in existingPartShippingGoodsInfos)
            {
                if (existingShippingGoodsInfo == null)
                {
                    continue;
                }

                if (string.Equals(existingShippingGoodsInfo.Status, ShippingGoodsStatusInfo.Closed, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!string.Equals(existingShippingGoodsInfo.Status, ShippingGoodsStatusInfo.UnPrinted, StringComparison.OrdinalIgnoreCase))
                {
                    return string.Format("批次“{0}”下零件编号“{1}”的状态不是未打印，不允许删除。", normalizedSupplyBatchNo, normalizedPartNo);
                }
            }

            ParamterInfo printRecordDelete = XHS.DALFactory.PrintRecord.Create().DeletePrintRecordsByBusinessKey(normalizedSupplyBatchNo, normalizedPartNo, normalizedDeliveryNo);
            ParamterInfo packingScanDelete = XHS.DALFactory.PackingScanRecord.Create().DeletePackingScanRecordsByBusinessKey(normalizedSupplyBatchNo, normalizedPartNo, normalizedDeliveryNo);
            ParamterInfo packingRecordDelete = XHS.DALFactory.PackingRecord.Create().DeletePackingRecordsByBusinessKey(normalizedSupplyBatchNo, normalizedPartNo, normalizedDeliveryNo);
            ParamterInfo paramterInfo = dal.DeleteShippingGoodsByBusinessKey(existingPartShippingGoodsInfos, normalizedDeliveryNo);
            IList source = new ArrayList();
            source.Add(printRecordDelete);
            source.Add(packingScanDelete);
            source.Add(packingRecordDelete);
            source.Add(paramterInfo);

            return Common.Save(source) ? string.Empty : "保存失败。";
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
                ShippingGoodsInfo firstExistingShippingGoodsInfo = existingPartShippingGoodsInfos != null && existingPartShippingGoodsInfos.Count > 0
                    ? existingPartShippingGoodsInfos[0]
                    : null;
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
                    uploadShippingGoodsInfo.Creater = firstExistingShippingGoodsInfo == null ||
                        string.IsNullOrWhiteSpace(firstExistingShippingGoodsInfo.Creater)
                        ? creater
                        : firstExistingShippingGoodsInfo.Creater;
                    uploadShippingGoodsInfo.CreatDate = firstExistingShippingGoodsInfo == null ||
                        !firstExistingShippingGoodsInfo.CreatDate.HasValue
                        ? uploadDate
                        : firstExistingShippingGoodsInfo.CreatDate;
                }

                source.Add(BuildInsertShippingGoods(entry.Value));
            }

            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string SaveViewEditShippingGoods(string supplyBatchNo, string partNo, List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            if (string.IsNullOrWhiteSpace(supplyBatchNo) || string.IsNullOrWhiteSpace(partNo))
            {
                return "编辑模式缺少供货批次号或零件编号。";
            }

            if (shippingGoodsInfos == null || shippingGoodsInfos.Count == 0)
            {
                return "没有可保存的出货货品数据。";
            }

            string normalizedSupplyBatchNo = supplyBatchNo.Trim();
            string normalizedPartNo = partNo.Trim();
            List<ShippingGoodsInfo> existingPartShippingGoodsInfos = dal.GetShippingGoods(normalizedPartNo, string.Empty, normalizedSupplyBatchNo);
            if (existingPartShippingGoodsInfos != null && existingPartShippingGoodsInfos.Count > 0)
            {
                foreach (ShippingGoodsInfo existingShippingGoodsInfo in existingPartShippingGoodsInfos)
                {
                    if ((existingShippingGoodsInfo.PrintCount ?? 0) > 0)
                    {
                        return string.Format("批次“{0}”下零件编号“{1}”已有打印记录，不允许修改。", normalizedSupplyBatchNo, normalizedPartNo);
                    }
                }
            }

            ShippingGoodsInfo firstExistingShippingGoodsInfo = existingPartShippingGoodsInfos != null && existingPartShippingGoodsInfos.Count > 0
                ? existingPartShippingGoodsInfos[0]
                : null;
            IList source = new ArrayList();
            if (existingPartShippingGoodsInfos != null && existingPartShippingGoodsInfos.Count > 0)
            {
                source.Add(dal.DeleteShippingGoodsByBusinessKey(existingPartShippingGoodsInfos));
            }

            foreach (ShippingGoodsInfo shippingGoodsInfo in shippingGoodsInfos)
            {
                if (shippingGoodsInfo == null)
                {
                    continue;
                }

                shippingGoodsInfo.Id = null;
                shippingGoodsInfo.SupplyBatchNo = normalizedSupplyBatchNo;
                shippingGoodsInfo.PartNo = normalizedPartNo;
                shippingGoodsInfo.QrCode = null;
                shippingGoodsInfo.Status = ShippingGoodsStatusInfo.UnPrinted;
                shippingGoodsInfo.PrintCount = 0;
                shippingGoodsInfo.Creater = firstExistingShippingGoodsInfo == null ||
                    string.IsNullOrWhiteSpace(firstExistingShippingGoodsInfo.Creater)
                    ? shippingGoodsInfo.Creater
                    : firstExistingShippingGoodsInfo.Creater;
                shippingGoodsInfo.CreatDate = firstExistingShippingGoodsInfo != null &&
                    firstExistingShippingGoodsInfo.CreatDate.HasValue
                    ? firstExistingShippingGoodsInfo.CreatDate
                    : shippingGoodsInfo.CreatDate;
            }

            source.Add(BuildInsertShippingGoods(shippingGoodsInfos));
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string CompleteShippingGoodsPrint(string supplyBatchNo, string partNo, string cartonNo, string deliveryNo)
        {
            List<ShippingGoodsInfo> shippingGoodsInfos = dal.GetShippingGoodsByBusinessKey(supplyBatchNo, partNo, cartonNo, deliveryNo);
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

            ParamterInfo paramterInfo = dal.UpdateShippingGoodsByBusinessKey(shippingGoodsInfos, deliveryNo);
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
