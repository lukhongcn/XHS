using System;
using System.Collections.Generic;
using CheryPortHelp;
using XHS.Model;
using XHS.Model.Label;

namespace XHS.BLL
{
    /// <summary>
    /// 出货货品标签数据构建器。
    /// </summary>
    public static class ShippingGoodsLabelBuilder
    {
        public static LabelInfo BuildOuterBoxLabelInfo(ShippingGoodsInfo info)
        {
            LabelInfo labelInfo = new LabelInfo
            {
                BaseNo = CheryPortConfig.BaseNo,
                DeliveryType = CheryPortConfig.DeliveryType,
                PackageType = CheryPortConfig.PackageType.ToString(),
                SupplierCode = GetPreferredSupplierCode(info),
                PartNo = SafeValue(info == null ? null : info.PartNo),
                PartName = SafeValue(info == null ? null : info.PartChineseName),
                Qty = info != null && info.Quantity.HasValue ? info.Quantity.Value.ToString() : string.Empty,
                LotNo = SafeValue(info == null ? null : info.SupplyBatchNo),
                LayerCount = info != null && info.StackLayerCount.HasValue ? info.StackLayerCount.Value.ToString() : "1",
                ProduceDate = FormatDate(info == null ? null : info.ProductionDate),
                CheckConfirmDate = FormatDate(info == null ? null : info.InspectionConfirmDate),
                PackageCode = SafeValue(info == null ? null : info.CartonNo),
                BoxCount = "1"
            };

            if (string.IsNullOrWhiteSpace(labelInfo.CheckConfirmDate))
            {
                labelInfo.CheckConfirmDate = labelInfo.ProduceDate;
            }

            XHS.BLL.Label labelService = new XHS.BLL.Label();
            XHS.BLL.QRCode qrCodeService = new XHS.BLL.QRCode();
            labelInfo.QrContent = labelService.GetQRCodeContents(
                qrCodeService.GetOuterPackageQRCodeInfoList(),
                labelInfo);

            return labelInfo;
        }

        public static List<LabelInfo> BuildOuterBoxLabelInfos(IList<ShippingGoodsInfo> shippingGoodsInfos)
        {
            List<LabelInfo> labelInfos = new List<LabelInfo>();
            if (shippingGoodsInfos == null)
            {
                return labelInfos;
            }

            for (int i = 0; i < shippingGoodsInfos.Count; i++)
            {
                LabelInfo labelInfo = BuildOuterBoxLabelInfo(shippingGoodsInfos[i]);
                string validateMessage;
                if (!ValidateTableLabelInfo(labelInfo, out validateMessage))
                {
                    throw new InvalidOperationException(string.Format("第 {0} 条标签数据不完整：{1}", i + 1, validateMessage));
                }

                labelInfos.Add(labelInfo);
            }

            return labelInfos;
        }

        public static bool ValidateTableLabelInfo(LabelInfo labelInfo, out string message)
        {
            List<string> missingFields = new List<string>();

            if (labelInfo == null)
            {
                message = "标签信息为空。";
                return false;
            }

            if (string.IsNullOrWhiteSpace(labelInfo.SupplierCode))
            {
                missingFields.Add("供应商代码");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.PartNo))
            {
                missingFields.Add("零件号");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.PartName))
            {
                missingFields.Add("零件名称");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.Qty))
            {
                missingFields.Add("数量");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.LotNo))
            {
                missingFields.Add("供货批次号");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.LayerCount))
            {
                missingFields.Add("码放层数");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.ProduceDate))
            {
                missingFields.Add("生产日期");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.CheckConfirmDate))
            {
                missingFields.Add("检验确认日期");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.PackageCode))
            {
                missingFields.Add("纸箱编号");
            }
            if (string.IsNullOrWhiteSpace(labelInfo.QrContent))
            {
                missingFields.Add("二维码内容");
            }

            if (missingFields.Count > 0)
            {
                message = string.Join("、", missingFields.ToArray());
                return false;
            }

            message = string.Empty;
            return true;
        }

        private static string GetPreferredSupplierCode(ShippingGoodsInfo info)
        {
            string supplierCode = SafeValue(info == null ? null : info.SupplierCode);
            if (!string.IsNullOrWhiteSpace(supplierCode))
            {
                return supplierCode;
            }

            return CheryPortConfig.SupplNo;
        }

        private static string SafeValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private static string FormatDate(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("yyyyMMdd") : string.Empty;
        }
    }
}
