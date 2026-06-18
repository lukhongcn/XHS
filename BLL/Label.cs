using XHS.Model.Label;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace XHS.BLL
{
    public class Label
    {
        /// <summary>
        /// 根据二维码字段映射和二维码原文生成标签数据。
        /// qRCodes 定义二维码项号与 LabelInfo 字段名的对应关系。
        /// QRCodeContent 为完整二维码内容，例如：10#xxx$11#yyy$。
        /// </summary>
        public LabelInfo GetLabelInfo(List<QRCodeInfo> qRCodes, string QRCodeContent)
        {
            var labelInfo = new LabelInfo();

            if (qRCodes == null || qRCodes.Count == 0)
            {
                labelInfo.QrContent = QRCodeContent;
                return labelInfo;
            }

            labelInfo.QrContent = QRCodeContent;

            // 先把二维码原文拆成：二维码项号 -> 字段值。
            Dictionary<string, string> qrValueMap = ParseQRCodeContent(QRCodeContent);
            Type labelInfoType = typeof(LabelInfo);

            foreach (QRCodeInfo qrCode in qRCodes)
            {
                if (qrCode == null ||
                    string.IsNullOrWhiteSpace(qrCode.QRNumber) ||
                    string.IsNullOrWhiteSpace(qrCode.QRField))
                {
                    continue;
                }

                string qrNumber = qrCode.QRNumber.Trim();
                string qrField = qrCode.QRField.Trim();
                string qrFieldValue;

                if (!qrValueMap.TryGetValue(qrNumber, out qrFieldValue))
                {
                    continue;
                }

                if (qrNumber == "21#")
                {
                    ApplyPackingSlipCardInfo(labelInfo, qrFieldValue);
                    continue;
                }

                PropertyInfo propertyInfo = labelInfoType.GetProperty(qrField);
                if (propertyInfo == null || !propertyInfo.CanWrite)
                {
                    continue;
                }

                propertyInfo.SetValue(labelInfo, qrFieldValue, null);
            }

            return labelInfo;
        }

        /// <summary>
        /// 根据二维码字段映射和标签数据生成二维码原文。
        /// 输出格式示例：10#202004114AA$11#3051$。
        /// 每一项都以 $ 作为结束符。
        /// </summary>
        public string GetQRCodeContents(List<QRCodeInfo> qRCodes, LabelInfo qrcodeLabel)
        {
            if (qRCodes == null || qRCodes.Count == 0 || qrcodeLabel == null)
            {
                return string.Empty;
            }

            var qrCodeContentBuilder = new StringBuilder();
            Type labelInfoType = typeof(LabelInfo);

            foreach (QRCodeInfo qrCode in qRCodes)
            {
                if (qrCode == null ||
                    string.IsNullOrWhiteSpace(qrCode.QRNumber) ||
                    string.IsNullOrWhiteSpace(qrCode.QRField))
                {
                    continue;
                }

                string qrNumber = qrCode.QRNumber.Trim();
                string qrField = qrCode.QRField.Trim();

                PropertyInfo propertyInfo = labelInfoType.GetProperty(qrField);
                if (propertyInfo == null || !propertyInfo.CanRead)
                {
                    continue;
                }

                string qrFieldValue = GetQRCodeFieldValue(qrCode, qrcodeLabel, propertyInfo);

                qrCodeContentBuilder.Append(qrNumber);
                qrCodeContentBuilder.Append(qrFieldValue);
                qrCodeContentBuilder.Append("$");
            }

            return qrCodeContentBuilder.ToString();
        }

        /// <summary>
        /// 解析二维码原文。
        /// 例如：10#202004114AA$11#3051$ -> { "10#" : "202004114AA", "11#" : "3051" }
        /// </summary>
        private static Dictionary<string, string> ParseQRCodeContent(string qrCodeContent)
        {
            var result = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(qrCodeContent))
            {
                return result;
            }

            string[] segments = qrCodeContent.Split(new[] { '$' }, StringSplitOptions.RemoveEmptyEntries);
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

            return result;
        }

        /// <summary>
        /// 解析 21# 对应的“随箱卡号-流水号”组合字段。
        /// 例如：SKD1296626050604039-1-1 -> PackingSlipCardNo=SKD1296626050604039, SerialNo=1-1
        /// </summary>
        private static void ApplyPackingSlipCardInfo(LabelInfo labelInfo, string qrFieldValue)
        {
            if (labelInfo == null)
            {
                return;
            }

            string packingSlipCardValue = qrFieldValue == null ? string.Empty : qrFieldValue.Trim();
            if (string.IsNullOrWhiteSpace(packingSlipCardValue))
            {
                labelInfo.PackingSlipCardNo = string.Empty;
                labelInfo.SerialNo = string.Empty;
                return;
            }

            string[] parts = packingSlipCardValue.Split('-');
            if (parts.Length >= 3)
            {
                labelInfo.PackingSlipCardNo = string.Join("-", parts, 0, parts.Length - 2);
                labelInfo.SerialNo = parts[parts.Length - 2] + "-" + parts[parts.Length - 1];
                return;
            }

            labelInfo.PackingSlipCardNo = packingSlipCardValue;
            labelInfo.SerialNo = string.Empty;
        }

        /// <summary>
        /// 读取二维码项号对应的字段值。
        /// 21# 需要把随箱卡号和流水号重新拼成一个字段。
        /// </summary>
        private static string GetQRCodeFieldValue(QRCodeInfo qrCode, LabelInfo qrcodeLabel, PropertyInfo propertyInfo)
        {
            if (qrCode.QRNumber.Trim() == "21#")
            {
                string packingSlipCardNo = qrcodeLabel.PackingSlipCardNo == null
                    ? string.Empty
                    : qrcodeLabel.PackingSlipCardNo.Trim();

                string serialNo = qrcodeLabel.SerialNo == null
                    ? string.Empty
                    : qrcodeLabel.SerialNo.Trim();

                if (string.IsNullOrWhiteSpace(serialNo))
                {
                    return packingSlipCardNo;
                }

                if (string.IsNullOrWhiteSpace(packingSlipCardNo))
                {
                    return serialNo;
                }

                return packingSlipCardNo + "-" + serialNo;
            }

            object propertyValue = propertyInfo.GetValue(qrcodeLabel, null);
            return propertyValue == null ? string.Empty : propertyValue.ToString().Trim();
        }
    }
}

