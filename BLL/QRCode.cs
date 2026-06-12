using System.Collections.Generic;
using XHS.Model.Label;

namespace BLL
{
    public class QRCode
    {
        /// <summary>
        /// 获取随箱卡二维码字段列表。
        /// 当前先按固定内容写死，后续可改为数据库读取。
        /// </summary>
        public List<QRCodeInfo> GetPackingSlipCardQRCodeInfoList()
        {
            return new List<QRCodeInfo>
            {
                new QRCodeInfo
                {
                    QRNumber = "10#",
                    QRField = nameof(LabelInfo.PartNo)
                },
                new QRCodeInfo
                {
                    QRNumber = "11#",
                    QRField = nameof(LabelInfo.SupplierCode)
                },
                new QRCodeInfo
                {
                    QRNumber = "17#",
                    QRField = nameof(LabelInfo.Qty)
                },
                new QRCodeInfo
                {
                    QRNumber = "18#",
                    QRField = nameof(LabelInfo.LotNo)
                },
                new QRCodeInfo
                {
                    QRNumber = "21#",
                    QRField = nameof(LabelInfo.PackingSlipCardNo)
                },
                new QRCodeInfo
                {
                    QRNumber = "22#",
                    QRField = nameof(LabelInfo.PackageCode)
                },
                new QRCodeInfo
                {
                    QRNumber = "23#",
                    QRField = nameof(LabelInfo.LotNo)
                }
            };
        }

        /// <summary>
        /// 获取外包装二维码字段列表。
        /// 当前按外箱标签二维码规则返回，后续可改为数据库读取。
        /// </summary>
        public List<QRCodeInfo> GetOuterPackageQRCodeInfoList()
        {
            return new List<QRCodeInfo>
            {
                new QRCodeInfo
                {
                    QRNumber = "10#",
                    QRField = nameof(LabelInfo.PartNo)
                },
                new QRCodeInfo
                {
                    QRNumber = "11#",
                    QRField = nameof(LabelInfo.SupplierCode)
                },
                new QRCodeInfo
                {
                    QRNumber = "17#",
                    QRField = nameof(LabelInfo.Qty)
                },
                new QRCodeInfo
                {
                    QRNumber = "18#",
                    QRField = nameof(LabelInfo.LotNo)
                },
                new QRCodeInfo
                {
                    QRNumber = "19#",
                    QRField = nameof(LabelInfo.LayerCount)
                },
                new QRCodeInfo
                {
                    QRNumber = "20#",
                    QRField = nameof(LabelInfo.ProduceDate)
                }
            };
        }
    }
}
