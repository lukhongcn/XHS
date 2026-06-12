using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using XHS.Model;
using XHS.Model.Label;

namespace BLL
{
    public class CheryUpload
    {
        /// <summary>
        /// 根据界面标签数据创建奇瑞上传信息。
        /// 当前上传来源统一改为 LabelInfo。
        /// 如果后续数据库能提供更多明细，可继续扩展 PackingDetails。
        /// </summary>
        public CheryUploadInfo CreateUploadInfo(LabelInfo labelInfo)
        {
            if (labelInfo == null)
            {
                throw new ArgumentNullException(nameof(labelInfo));
            }

            var now = DateTime.Now;
            // 接口 packingDate 要求的是装箱完成时间，不是生产日期。
            var packingDate = ParseDateTime(labelInfo.PackingCreateTime);
            var checkTime = ParseDateTime(string.IsNullOrWhiteSpace(labelInfo.CheckTime) ? labelInfo.CheckConfirmDate : labelInfo.CheckTime);
            var createTime = ParseDateTime(labelInfo.PackingCreateTime);
            var sxCardSeq = string.IsNullOrWhiteSpace(labelInfo.SxCardSeq)
                ? BuildPackingSlipSequence(labelInfo)
                : SafeValue(labelInfo.SxCardSeq);
            int packageType = 1;
            if (!int.TryParse(labelInfo.PackageType, out packageType))
            {
                packageType = 1;
            }

            return new CheryUploadInfo
            {
                // 当前扫码内容对外包装上传等于外包装二维码内容。
                ScanCode = labelInfo.QrContent,

                DeliveryNo = SafeValue(labelInfo.DeliveryNo),
                SxCardSeq = sxCardSeq,

                MaterialNo = SafeValue(labelInfo.PartNo),
                MaterialName = SafeValue(labelInfo.PartName),
                PackingCount = SafeValue(labelInfo.Qty),

                // packageType 统一从配置/界面默认值读取。
                PackageType = packageType,

                // 外包装二维码内容就是接口中的 packageBarCode。
                PackageBarCode = SafeValue(labelInfo.QrContent),
                PackageCode = SafeValue(labelInfo.PackageCode),
                PackageName = SafeValue(labelInfo.PackageName),

                PackingDate = packingDate ?? now,
                CheckTime = checkTime ?? now,
                CheckUserName = string.IsNullOrWhiteSpace(labelInfo.CheckUserName) ? Environment.UserName : SafeValue(labelInfo.CheckUserName),

                // packageType=2 批量装箱时无需传 packingDetails。
                PackingDetails = packageType == 2
                    ? null
                    : new List<PackingDetailInfo>
                    {
                        new PackingDetailInfo
                        {
                            // 接口要求这里传奇瑞零件码/物料流水号的完整内容。
                            materialBarCode = SafeValue(labelInfo.MaterialBarCode),
                            materialNo = SafeValue(labelInfo.PartNo),
                            materialName = SafeValue(labelInfo.PartName),
                            createTime = (createTime ?? now).ToString("yyyy-MM-dd HH:mm:ss"),
                            createName = string.IsNullOrWhiteSpace(labelInfo.PackingCreateName) ? Environment.UserName : SafeValue(labelInfo.PackingCreateName)
                        }
                    }
            };
        }

        /// <summary>
        /// 保留一个无参重载作为测试数据入口。
        /// 后续如果不再需要测试数据，可删除此方法。
        /// </summary>
        public CheryUploadInfo CreateUploadInfo()
        {
            var qrCodeService = new QRCode();
            var labelService = new Label();

            var labelInfo = new LabelInfo
            {
                BaseNo = "1133",
                DeliveryType = "1",
                DeliveryNo = "1020MO-CS07260506",
                SxCardSeq = "SKD1296626050604039-1-1",
                PackageType = "1",
                CheckTime = "2026-05-06 10:12:53",
                CheckUserName = Environment.UserName,
                PackingCreateTime = "2026-05-06 10:10:00",
                PackingCreateName = Environment.UserName,
                SupplierCode = "3051",
                PartNo = "202004114AA",
                PartName = "后下控制臂总成",
                MaterialBarCode = "10#WL008$11#8KK$12#40000284$13#TEST$30#001$",
                Qty = "120",
                LotNo = "1020MO-CS07260506",
                PackingSlipCardNo = "SKD1296626050604039",
                PackageCode = "ZF700300180",
                PackageName = "A型号成品箱",
                LayerCount = "1",
                BoxCount = "1",
                ProduceDate = "2026-05-06 10:12:53",
                CheckDate = "2026-05-06 10:12:53",
                CheckConfirmDate = "2026-05-06 10:12:53",
                SerialNo = "1-1"
            };

            labelInfo.QrContent = labelService.GetQRCodeContents(
                qrCodeService.GetOuterPackageQRCodeInfoList(),
                labelInfo);

            return CreateUploadInfo(labelInfo);
        }

        /// <summary>
        /// 生成 CheryUploadInfo 的 JSON。
        /// </summary>
        public string CreateCheryUpload(LabelInfo labelInfo)
        {
            var uploadInfo = CreateUploadInfo(labelInfo);
            return SerializeUploadInfo(uploadInfo);
        }

        /// <summary>
        /// 保留无参 JSON 生成入口，兼容现有测试调用。
        /// </summary>
        public string CreateCheryUpload()
        {
            var uploadInfo = CreateUploadInfo();
            return SerializeUploadInfo(uploadInfo);
        }

        private static string BuildPackingSlipSequence(LabelInfo labelInfo)
        {
            string packingSlipCardNo = labelInfo == null ? string.Empty : SafeValue(labelInfo.PackingSlipCardNo);
            string serialNo = labelInfo == null ? string.Empty : SafeValue(labelInfo.SerialNo);

            if (string.IsNullOrWhiteSpace(packingSlipCardNo))
            {
                return serialNo;
            }

            if (string.IsNullOrWhiteSpace(serialNo))
            {
                return packingSlipCardNo;
            }

            return packingSlipCardNo + "-" + serialNo;
        }

        private static DateTime? ParseDateTime(string value)
        {
            DateTime result;
            return DateTime.TryParse(value, out result) ? result : (DateTime?)null;
        }

        private static string SafeValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private static string SerializeUploadInfo(CheryUploadInfo uploadInfo)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(uploadInfo);
        }
    }
}
