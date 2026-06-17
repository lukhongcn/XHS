using System;
using System.Drawing;
using LabelHelp.Config;
using LabelHelp.Enums;
using XHS.Model.Label;

namespace LabelHelp.Rendering
{
    /// <summary>
    /// 标签渲染器。负责把一张标签画在指定位置。
    /// 当前实现重点支持 TableLabel 外箱/零件表格标签。
    /// </summary>
    public class LabelRenderer
    {
        public void Render(Graphics graphics, LabelInfo info, LabelTemplateType templateType, LabelPrintConfig config, float offsetX, float offsetY)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException("graphics");
            }

            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            graphics.PageUnit = GraphicsUnit.Millimeter;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            switch (templateType)
            {
                case LabelTemplateType.CardSingleLabel:
                    RenderCardSingleLabel(graphics, info, config, offsetX, offsetY);
                    break;

                case LabelTemplateType.CardPackageLabel:
                    RenderCardPackageLabel(graphics, info, config, offsetX, offsetY);
                    break;

                case LabelTemplateType.TableLabel:
                default:
                    RenderTableLabel(graphics, info, config, offsetX, offsetY);
                    break;
            }
        }

        private void RenderTableLabel(Graphics graphics, LabelInfo info, LabelPrintConfig config, float offsetX, float offsetY)
        {
            float x = offsetX;
            float y = offsetY;
            float width = config.LabelWidth;
            float height = config.LabelHeight;

            float margin = 1f;
            float tableX = x + margin;
            float tableY = y + margin;
            float tableWidth = width - margin * 2f;
            float tableHeight = height - margin * 2f;

            float titleRowHeight = tableHeight * 0.12f;
            float rowHeight = (tableHeight - titleRowHeight) / 9f;
            float leftColWidth = tableWidth * 0.24f;
            float middleColWidth = tableWidth * 0.31f;
            float rightColWidth = tableWidth - leftColWidth - middleColWidth;

            float col1X = tableX;
            float col2X = tableX + leftColWidth;
            float col3X = tableX + leftColWidth + middleColWidth;
            float tableRight = tableX + tableWidth;
            float tableBottom = tableY + tableHeight;
            float row0Y = tableY + titleRowHeight;

            using (Pen borderPen = new Pen(Color.Black, 0.45f))
            using (Pen innerPen = new Pen(Color.Black, 0.35f))
            using (Font headerFont = new Font(config.FontName, 12f, FontStyle.Bold, GraphicsUnit.Point))
            using (Font titleFont = new Font(config.FontName, 9f, FontStyle.Bold, GraphicsUnit.Point))
            using (Font valueFont = new Font(config.FontName, 9.5f, FontStyle.Bold, GraphicsUnit.Point))
            using (Brush textBrush = new SolidBrush(Color.Black))
            using (Brush backgroundBrush = new SolidBrush(Color.FromArgb(0, 176, 226)))
            {
                graphics.FillRectangle(backgroundBrush, x, y, width, height);

                graphics.DrawRectangle(borderPen, tableX, tableY, tableWidth, tableHeight);
                graphics.DrawLine(innerPen, tableX, row0Y, tableRight, row0Y);

                for (int i = 1; i <= 9; i++)
                {
                    float lineY = row0Y + rowHeight * i;

                    if (i <= 2)
                    {
                        graphics.DrawLine(innerPen, tableX, lineY, tableRight, lineY);
                    }
                    else
                    {
                        graphics.DrawLine(innerPen, tableX, lineY, col3X, lineY);
                    }
                }

                graphics.DrawLine(innerPen, col2X, row0Y, col2X, tableBottom);
                graphics.DrawLine(innerPen, col3X, row0Y + rowHeight * 2f, col3X, tableBottom);

                DrawFittedText(graphics, "KD专用", headerFont, textBrush, new RectangleF(tableX + 1.5f, tableY, tableWidth - 3f, titleRowHeight), ContentAlignment.MiddleLeft);
                DrawRow(graphics, titleFont, valueFont, textBrush, "供应商代码", Safe(info.SupplierCode), col1X, col2X, tableRight, row0Y, rowHeight, 0);
                DrawRow(graphics, titleFont, valueFont, textBrush, "零件号", Safe(info.PartNo), col1X, col2X, col3X, row0Y, rowHeight, 1);
                DrawRow(graphics, titleFont, valueFont, textBrush, "零件名称", Safe(info.PartName), col1X, col2X, col3X, row0Y, rowHeight, 2);
                DrawRow(graphics, titleFont, valueFont, textBrush, "数量", Safe(info.Qty), col1X, col2X, col3X, row0Y, rowHeight, 3);
                DrawRow(graphics, titleFont, valueFont, textBrush, "供货批次号", Safe(info.LotNo), col1X, col2X, col3X, row0Y, rowHeight, 4);
                DrawRow(graphics, titleFont, valueFont, textBrush, "码放层数", Safe(info.LayerCount), col1X, col2X, col3X, row0Y, rowHeight, 5);
                DrawRow(graphics, titleFont, valueFont, textBrush, "生产日期", ValueOrSlash(info.ProduceDate), col1X, col2X, col3X, row0Y, rowHeight, 6);
                DrawRow(graphics, titleFont, valueFont, textBrush, "检验确认/日期", ValueOrSlash(info.CheckConfirmDate), col1X, col2X, col3X, row0Y, rowHeight, 7);
                DrawRow(graphics, titleFont, valueFont, textBrush, "纸箱编号", GetBoxNumber(info), col1X, col2X, col3X, row0Y, rowHeight, 8);

                // 二维码
                string qrContent = new QrCodeGenerator().BuildDefaultQrContent(info);
                using (Image qrImage = new QrCodeGenerator().Generate(qrContent, 300, 300))
                {
                    float qrAreaX = col3X;
                    float qrAreaY = row0Y + rowHeight * 2f;
                    float qrAreaWidth = rightColWidth;
                    float qrAreaHeight = tableBottom - qrAreaY;
                    float qrSize = Math.Min(qrAreaWidth - 8f, qrAreaHeight - 8f);

                    float qrX = qrAreaX + (qrAreaWidth - qrSize) / 2f;
                    float qrY = qrAreaY + (qrAreaHeight - qrSize) / 2f;
                    graphics.DrawImage(qrImage, qrX, qrY, qrSize, qrSize);
                }
            }
        }

        private void RenderCardSingleLabel(Graphics graphics, LabelInfo info, LabelPrintConfig config, float offsetX, float offsetY)
        {
            RenderSimpleCardLabel(graphics, info, config, offsetX, offsetY, "单件标签");
        }

        private void RenderCardPackageLabel(Graphics graphics, LabelInfo info, LabelPrintConfig config, float offsetX, float offsetY)
        {
            RenderSimpleCardLabel(graphics, info, config, offsetX, offsetY, "最小包装标签");
        }

        private void RenderSimpleCardLabel(Graphics graphics, LabelInfo info, LabelPrintConfig config, float offsetX, float offsetY, string title)
        {
            float x = offsetX;
            float y = offsetY;
            float width = config.LabelWidth;
            float height = config.LabelHeight;

            using (Pen pen = new Pen(Color.Black, 0.25f))
            using (Font titleFont = new Font(config.FontName, 8f, FontStyle.Bold, GraphicsUnit.Point))
            using (Font textFont = new Font(config.FontName, 7f, FontStyle.Regular, GraphicsUnit.Point))
            using (Brush brush = new SolidBrush(Color.Black))
            {
                graphics.FillRectangle(Brushes.White, x, y, width, height);
                graphics.DrawRectangle(pen, x + 3f, y + 3f, width - 6f, height - 6f);

                DrawText(graphics, title, titleFont, brush, x + 5f, y + 5f);
                DrawText(graphics, "MADE IN CHINA", textFont, brush, x + width - 35f, y + 5f);

                float textX = x + 5f;
                float textY = y + 15f;
                float lineHeight = 5f;

                DrawText(graphics, "零件号:" + Safe(info.PartNo), textFont, brush, textX, textY + lineHeight * 0);
                DrawText(graphics, "零件名称:" + Safe(info.PartName), textFont, brush, textX, textY + lineHeight * 1);
                DrawText(graphics, "供应商代码:" + Safe(info.SupplierCode), textFont, brush, textX, textY + lineHeight * 2);
                DrawText(graphics, "材料代码:" + Safe(info.MaterialCode), textFont, brush, textX, textY + lineHeight * 3);
                DrawText(graphics, "生产日期:" + Safe(info.ProduceDate), textFont, brush, textX, textY + lineHeight * 4);
                DrawText(graphics, "流水号:" + Safe(info.SerialNo), textFont, brush, textX, textY + lineHeight * 5);

                string qrContent = new QrCodeGenerator().BuildDefaultQrContent(info);
                using (Image qrImage = new QrCodeGenerator().Generate(qrContent, 300, 300))
                {
                    float qrSize = 25f;
                    graphics.DrawImage(qrImage, x + width - qrSize - 7f, y + 18f, qrSize, qrSize);
                }
            }
        }

        private static void DrawText(Graphics graphics, string text, Font font, Brush brush, float x, float y)
        {
            graphics.DrawString(text ?? string.Empty, font, brush, x, y);
        }

        private static void DrawRow(Graphics graphics, Font titleFont, Font valueFont, Brush brush, string title, string value, float labelLeft, float valueLeft, float valueRight, float firstRowY, float rowHeight, int rowIndex)
        {
            float rowTop = firstRowY + rowHeight * rowIndex;
            RectangleF titleRect = new RectangleF(labelLeft + 1f, rowTop, valueLeft - labelLeft - 2f, rowHeight);
            RectangleF valueRect = new RectangleF(valueLeft + 1f, rowTop, valueRight - valueLeft - 2f, rowHeight);

            DrawFittedText(graphics, title, titleFont, brush, titleRect, ContentAlignment.MiddleCenter);
            DrawFittedText(graphics, value, valueFont, brush, valueRect, ContentAlignment.MiddleCenter);
        }

        private static void DrawFittedText(Graphics graphics, string text, Font font, Brush brush, RectangleF bounds, ContentAlignment alignment)
        {
            string safeText = text ?? string.Empty;
            using (StringFormat format = CreateStringFormat(alignment))
            {
                Font drawFont = font;
                Font scaledFont = null;

                try
                {
                    for (float size = font.Size; size >= 4f; size -= 0.25f)
                    {
                        SizeF measured = graphics.MeasureString(safeText, drawFont, Size.Ceiling(bounds.Size), format);
                        if (measured.Width <= bounds.Width + 0.5f && measured.Height <= bounds.Height + 0.5f)
                        {
                            break;
                        }

                        if (size <= 4f)
                        {
                            break;
                        }

                        if (scaledFont != null)
                        {
                            scaledFont.Dispose();
                        }

                        scaledFont = new Font(font.FontFamily, size - 0.25f, font.Style, GraphicsUnit.Point);
                        drawFont = scaledFont;
                    }

                    graphics.DrawString(safeText, drawFont, brush, bounds, format);
                }
                finally
                {
                    if (scaledFont != null)
                    {
                        scaledFont.Dispose();
                    }
                }
            }
        }

        private static StringFormat CreateStringFormat(ContentAlignment alignment)
        {
            StringFormat format = new StringFormat();
            format.Trimming = StringTrimming.None;
            format.FormatFlags = StringFormatFlags.NoWrap;

            switch (alignment)
            {
                case ContentAlignment.MiddleLeft:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleCenter:
                default:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    break;
            }

            return format;
        }

        private static string Safe(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private static string ValueOrSlash(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "/" : value.Trim();
        }

        private static string GetBoxNumber(LabelInfo info)
        {
            if (info == null)
            {
                return string.Empty;
            }

            string packingSlipCardNo = Safe(info.PackingSlipCardNo);
            if (!string.IsNullOrEmpty(packingSlipCardNo))
            {
                int lastDashIndex = packingSlipCardNo.LastIndexOf('-');
                if (lastDashIndex > 0)
                {
                    int secondLastDashIndex = packingSlipCardNo.LastIndexOf('-', lastDashIndex - 1);
                    if (secondLastDashIndex >= 0 && secondLastDashIndex < packingSlipCardNo.Length - 1)
                    {
                        return packingSlipCardNo.Substring(secondLastDashIndex + 1);
                    }
                }
            }

            return !string.IsNullOrWhiteSpace(info.PackageCode) ? info.PackageCode.Trim() : Safe(info.SerialNo);
        }

        private static string CombineSupplier(LabelInfo info)
        {
            if (info == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(info.SupplierName) && !string.IsNullOrWhiteSpace(info.SupplierCode))
            {
                return info.SupplierName.Trim() + "/" + info.SupplierCode.Trim();
            }

            if (!string.IsNullOrWhiteSpace(info.SupplierName))
            {
                return info.SupplierName.Trim();
            }

            return Safe(info.SupplierCode);
        }
    }
}
