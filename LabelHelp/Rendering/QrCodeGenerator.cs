using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Model.Label;
using ZXing;
using ZXing.Common;

namespace LabelHelp.Rendering
{
    /// <summary>
    /// 二维码生成工具。
    /// 依赖 NuGet：ZXing.Net
    /// </summary>
    public class QrCodeGenerator
    {
        public Image Generate(string content, int width, int height)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                content = "EMPTY";
            }

            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Width = width,
                    Height = height,
                    Margin = 1,
                    PureBarcode = true
                }
            };

            var pixelData = writer.Write(content);

            var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb);
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppRgb);

            try
            {
                Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            return bitmap;
        }

        public string BuildDefaultQrContent(LabelInfo info)
        {
            if (info == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(info.QrContent))
            {
                return info.QrContent;
            }

            return string.Format(
                "PartNo={0};PartName={1};SupplierCode={2};Qty={3};LotNo={4};ProduceDate={5};SerialNo={6}",
                info.PartNo,
                info.PartName,
                info.SupplierCode,
                info.Qty,
                info.LotNo,
                info.ProduceDate,
                info.SerialNo);
        }
    }
}
