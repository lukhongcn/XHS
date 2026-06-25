using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using XHS.Model;
using XHS.Model.Label;

namespace XHS.BLL
{
    public class QRCode
    {
        /// <summary>
        /// 直接把二维码内容解析成出货货品信息。
        /// 当前页面使用的字段映射如下：
        /// 10# -> 零件号
        /// 11# -> 供应商代码
        /// 17# -> 数量
        /// 18#/23# -> 供货批次号
        /// 19# -> 码放层数
        /// 20# -> 生产日期
        /// 31#/22# -> 纸箱编号
        /// </summary>
        public ShippingGoodsInfo ParseShippingGoodsInfo(string qrCodeContent)
        {
            var shippingGoodsInfo = new ShippingGoodsInfo
            {
                QrCode = qrCodeContent
            };

            Dictionary<string, string> qrValueMap = ParseQRCodeContent(qrCodeContent);

            shippingGoodsInfo.PartNo = GetValue(qrValueMap, "10#");
            shippingGoodsInfo.SupplierCode = GetValue(qrValueMap, "11#");
            shippingGoodsInfo.SupplyBatchNo = GetFirstNonEmpty(
                GetValue(qrValueMap, "18#"),
                GetValue(qrValueMap, "23#"));
            shippingGoodsInfo.CartonNo = GetFirstNonEmpty(
                GetValue(qrValueMap, "31#"),
                GetValue(qrValueMap, "22#"));

            int quantity;
            if (int.TryParse(GetValue(qrValueMap, "17#"), out quantity))
            {
                shippingGoodsInfo.Quantity = quantity;
            }

            int stackLayerCount;
            if (int.TryParse(GetValue(qrValueMap, "19#"), out stackLayerCount))
            {
                shippingGoodsInfo.StackLayerCount = stackLayerCount;
            }

            DateTime productionDate;
            if (TryParseDate(GetValue(qrValueMap, "20#"), out productionDate))
            {
                shippingGoodsInfo.ProductionDate = productionDate;
            }

            return shippingGoodsInfo;
        }

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
                },
                new QRCodeInfo
                {
                    QRNumber = "31#",
                    QRField = nameof(LabelInfo.PackageCode)
                }
            };
        }

        private static Dictionary<string, string> ParseQRCodeContent(string qrCodeContent)
        {
            var result = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(qrCodeContent))
            {
                return result;
            }

            string normalizedContent = qrCodeContent.Trim();
            string[] segments = normalizedContent.Split(new[] { '$' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string segment in segments)
            {
                if (string.IsNullOrWhiteSpace(segment))
                {
                    continue;
                }

                int splitIndex = segment.IndexOf('#');
                if (splitIndex < 0)
                {
                    continue;
                }

                string qrNumber = segment.Substring(0, splitIndex + 1).Trim();
                string qrFieldValue = string.Empty;
                if (splitIndex + 1 < segment.Length)
                {
                    qrFieldValue = segment.Substring(splitIndex + 1).Trim();
                }

                result[qrNumber] = qrFieldValue;
            }

            if (result.Count > 1)
            {
                return result;
            }

            result.Clear();
            MatchCollection matches = Regex.Matches(normalizedContent, @"\d{2}#");
            for (int i = 0; i < matches.Count; i++)
            {
                Match current = matches[i];
                int valueStart = current.Index + current.Length;
                int valueEnd = i + 1 < matches.Count ? matches[i + 1].Index : normalizedContent.Length;
                if (valueEnd < valueStart)
                {
                    continue;
                }

                string qrNumber = current.Value.Trim();
                string qrFieldValue = normalizedContent.Substring(valueStart, valueEnd - valueStart).Trim().TrimEnd('$');
                result[qrNumber] = qrFieldValue;
            }

            return result;
        }

        private static string GetValue(Dictionary<string, string> qrValueMap, string qrNumber)
        {
            if (qrValueMap == null || string.IsNullOrWhiteSpace(qrNumber))
            {
                return string.Empty;
            }

            string value;
            return qrValueMap.TryGetValue(qrNumber, out value) ? value : string.Empty;
        }

        private static string GetFirstNonEmpty(params string[] values)
        {
            if (values == null)
            {
                return string.Empty;
            }

            foreach (string value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value.Trim();
                }
            }

            return string.Empty;
        }

        private static bool TryParseDate(string value, out DateTime dateValue)
        {
            return DateTime.TryParse(value, out dateValue) ||
                   DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue) ||
                   DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue) ||
                   DateTime.TryParseExact(value, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue);
        }
    }
}


