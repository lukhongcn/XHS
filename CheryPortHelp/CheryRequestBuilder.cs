using System;
using Model;

namespace CheryPortHelp
{
    public static class CheryRequestBuilder
    {
        public static CheryCheckRecordRequest BuildCheckRecordRequest(CheryUploadInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            var now = DateTime.Now;
            var packageType = info.PackageType ?? CheryPortConfig.PackageType;

            // TODO: 标准装箱 packageType = 1 时，需要校验 PackingDetails 必填。
            return new CheryCheckRecordRequest
            {
                supplNo = CheryPortConfig.SupplNo,
                baseNo = CheryPortConfig.BaseNo,
                deliveryType = CheryPortConfig.DeliveryType,
                deliveryNo = info.DeliveryNo,
                sxCardSeq = info.SxCardSeq,
                materialNo = info.MaterialNo,
                materialName = info.MaterialName,
                packingCount = info.PackingCount,
                packageType = packageType,
                packageBarCode = info.PackageBarCode,
                packageCode = info.PackageCode,
                packageName = info.PackageName,
                packingDate = (info.PackingDate ?? now).ToString("yyyy-MM-dd HH:mm:ss"),
                packingDetails = info.PackingDetails,
                checkTime = (info.CheckTime ?? now).ToString("yyyy-MM-dd HH:mm:ss"),
                checkUserName = string.IsNullOrWhiteSpace(info.CheckUserName) ? Environment.UserName : info.CheckUserName
            };
        }
    }
}
