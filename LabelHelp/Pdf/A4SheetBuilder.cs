using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using LabelHelp.Config;
using LabelHelp.Enums;
using LabelHelp.Rendering;
using Model.Label;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace LabelHelp.Pdf
{
    /// <summary>
    /// A4 拼版 PDF 生成器。负责把多张标签排版到 A4 页面上。
    /// 依赖 NuGet：PdfSharp
    /// </summary>
    public class A4SheetBuilder
    {
        private const float Dpi = 300f;

        public string GenerateA4Pdf(List<LabelInfo> labelList, LabelTemplateType templateType, LabelPrintConfig config)
        {
            if (labelList == null || labelList.Count == 0)
            {
                throw new ArgumentException("labelList 不能为空。", "labelList");
            }

            EnsureOutputFolder(config);

            string fileName = string.Format("Label_A4_{0}_{1:yyyyMMddHHmmssfff}.pdf", templateType, DateTime.Now);
            string outputPath = Path.Combine(GetOutputFolder(config), fileName);

            int labelsPerPage = GetLabelsPerPage(config);
            if (labelsPerPage <= 0)
            {
                labelsPerPage = 8;
            }

            PdfDocument document = new PdfDocument();
            document.Info.Title = "标签打印";

            int pageCount = (int)Math.Ceiling(labelList.Count / (double)labelsPerPage);
            LabelRenderer renderer = new LabelRenderer();

            for (int pageIndex = 0; pageIndex < pageCount; pageIndex++)
            {
                using (Bitmap pageBitmap = CreateBitmapByMm(config.A4PageWidth, config.A4PageHeight))
                using (Graphics graphics = Graphics.FromImage(pageBitmap))
                {
                    graphics.Clear(Color.White);
                    graphics.PageUnit = GraphicsUnit.Millimeter;
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                    int startIndex = pageIndex * labelsPerPage;
                    int endIndex = Math.Min(startIndex + labelsPerPage, labelList.Count);

                    for (int i = startIndex; i < endIndex; i++)
                    {
                        int indexInPage = i - startIndex;
                        float x = GetLabelX(indexInPage, config);
                        float y = GetLabelY(indexInPage, config);

                        renderer.Render(graphics, labelList[i], templateType, config, x, y);
                    }

                    AddBitmapPage(document, pageBitmap, config.A4PageWidth, config.A4PageHeight);
                }
            }

            document.Save(outputPath);
            document.Close();

            return outputPath;
        }

        public float GetLabelX(int indexInPage, LabelPrintConfig config)
        {
            int column = indexInPage % config.A4Columns;
            return config.A4MarginLeft + column * (config.LabelWidth + config.A4ColumnGap);
        }

        public float GetLabelY(int indexInPage, LabelPrintConfig config)
        {
            int row = indexInPage / config.A4Columns;
            return config.A4MarginTop + row * (config.LabelHeight + config.A4RowGap);
        }

        public int GetLabelsPerPage(LabelPrintConfig config)
        {
            return config.A4Columns * config.A4Rows;
        }

        private static Bitmap CreateBitmapByMm(float widthMm, float heightMm)
        {
            int widthPx = Math.Max(1, (int)Math.Round(widthMm / 25.4f * Dpi));
            int heightPx = Math.Max(1, (int)Math.Round(heightMm / 25.4f * Dpi));

            Bitmap bitmap = new Bitmap(widthPx, heightPx, PixelFormat.Format32bppRgb);
            bitmap.SetResolution(Dpi, Dpi);
            return bitmap;
        }

        private static void AddBitmapPage(PdfDocument document, Bitmap bitmap, float pageWidthMm, float pageHeightMm)
        {
            PdfPage page = document.AddPage();
            page.Width = XUnit.FromMillimeter(pageWidthMm);
            page.Height = XUnit.FromMillimeter(pageHeightMm);

            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                stream.Position = 0;

                using (XGraphics gfx = XGraphics.FromPdfPage(page))
                using (XImage image = XImage.FromStream(stream))
                {
                    gfx.DrawImage(image, 0, 0, page.Width, page.Height);
                }
            }
        }

        private void EnsureOutputFolder(LabelPrintConfig config)
        {
            string folder = GetOutputFolder(config);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        private string GetOutputFolder(LabelPrintConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.OutputFolder))
            {
                return Path.GetFullPath("Output");
            }

            return Path.GetFullPath(config.OutputFolder);
        }
    }
}
