using System;
using System.Collections.Generic;
using Model;
using Newtonsoft.Json;

namespace BLL
{
    public class CheryUpload
    {
        /// <summary>
        /// 创建奇瑞防错漏平台上传信息。
        ///
        /// 当前使用测试数据。
        /// 后续正式使用时，把这里的测试数据替换成扫码、Excel 或 MES 中取得的数据。
        ///
        /// SupplNo、BaseNo、DeliveryType 等固定配置，不在这里填写，
        /// 由 CheryRequestBuilder 从 App.config 中读取。
        ///
        /// 当前按标准装箱处理：
        /// PackageType = 1
        /// 需要带 PackingDetails。
        /// </summary>
        public CheryUploadInfo CreateUploadInfo()
        {
            var now = DateTime.Now;

            var uploadInfo = new CheryUploadInfo
            {
                // ================================
                // 一、这里是正式上传数据来源区域
                // ================================
                // 当前先写测试数据。
                // 后续正式使用时，把下面这些字段替换成扫码、Excel 或 MES 中取得的数据。
                ScanCode = "10#WL008$11#8KK$31#TEST001$",

                DeliveryNo = "DEL202403210001",
                SxCardSeq = "SX202403210001-1-1",

                MaterialNo = "MAT001",
                MaterialName = "螺丝M4x10",
                PackingCount = "100",

                // packageType：
                // 1 = 标准装箱，箱码和零件码绑定
                // 2 = 批量装箱，无零件码
                // 3 = 随箱卡装箱，无箱码
                //
                // 当前业务按标准装箱处理，所以这里用 1。
                // 如果以后某些记录要走配置默认值，可以把 PackageType 设为 null。
                PackageType = 1,

                PackageBarCode = "10#WL008$11#8KK$31#TEST001$",
                PackageCode = "BOX-01",
                PackageName = "外包装箱",

                PackingDate = now,
                CheckTime = now,
                CheckUserName = Environment.UserName,

                // ================================
                // 二、这里是装箱明细 PackingDetails
                // ================================
                // 标准装箱 packageType = 1 时，需要有装箱明细。
                // 后续正式使用时，这里的 materialBarCode 应该来自零件码/最小包装码扫描结果。
                PackingDetails = new List<PackingDetailInfo>
                {
                    new PackingDetailInfo
                    {
                        materialBarCode = "MT202403210001",
                        materialNo = "MAT001",
                        materialName = "螺丝M4x10",
                        createTime = now.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss"),
                        createName = Environment.UserName
                    },
                    new PackingDetailInfo
                    {
                        materialBarCode = "MT202403210002",
                        materialNo = "MAT001",
                        materialName = "螺丝M4x10",
                        createTime = now.AddMinutes(-3).ToString("yyyy-MM-dd HH:mm:ss"),
                        createName = Environment.UserName
                    }
                }
            };

            return uploadInfo;
        }

        /// <summary>
        /// 生成 CheryUploadInfo 的 JSON。
        ///
        /// 注意：
        /// 这个 JSON 只是内部上传信息模型 CheryUploadInfo 的 JSON。
        /// 它不是最终提交给海行云接口的 JSON。
        ///
        /// 最终接口 JSON 需要经过 CheryRequestBuilder.BuildCheckRecordRequest，
        /// 补充 SupplNo、BaseNo、DeliveryType 等配置字段后再上传。
        /// </summary>
        public string CreateCheryUpload()
        {
            var uploadInfo = CreateUploadInfo();

            return JsonConvert.SerializeObject(
                uploadInfo,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }
    }
}
