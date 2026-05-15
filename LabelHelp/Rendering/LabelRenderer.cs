using System;
using System.Drawing;
using LabelHelp.Config;
using LabelHelp.Enums;
using Model.Label;

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

            float margin = 3f;
            float tableX = x + margin;
            float tableY = y + margin;
            float tableWidth = width - margin * 2f;
            float tableHeight = height - margin * 2f;

            float leftColWidth = 30f;
            float middleColWidth = 40f;
            float rightColWidth = tableWidth - leftColWidth - middleColWidth;
            float rowHeight = tableHeight / 8f;

            float col1X = tableX;
            float col2X = tableX + leftColWidth;
            float col3X = tableX + leftColWidth + middleColWidth;
            float tableRight = tableX + tableWidth;
            float tableBottom = tableY + tableHeight;

            using (Pen borderPen = new Pen(Color.Black, 0.35f))
            using (Pen innerPen = new Pen(Color.Black, 0.25f))
            using (Font titleFont = new Font(config.FontName, config.TitleFontSize, FontStyle.Bold, GraphicsUnit.Point))
            using (Font valueFont = new Font(config.FontName, config.ValueFontSize, FontStyle.Bold, GraphicsUnit.Point))
            using (Brush textBrush = new SolidBrush(Color.Black))
            {
                graphics.FillRectangle(Brushes.White, x, y, width, height);

                // 外框
                graphics.DrawRectangle(borderPen, tableX, tableY, tableWidth, tableHeight);

                // 竖线
                graphics.DrawLine(innerPen, col2X, tableY, col2X, tableBottom);
                // 右侧二维码区域从第4行开始合并，所以竖线从第4行顶部开始
                graphics.DrawLine(innerPen, col3X, tableY + rowHeight * 3f, col3X, tableBottom);

                // 横线
                for (int i = 1; i <= 7; i++)
                {
                    float lineY = tableY + rowHeight * i;

                    if (i <= 3)
                    {
                        graphics.DrawLine(innerPen, tableX, lineY, tableRight, lineY);
                    }
                    else
                    {
                        graphics.DrawLine(innerPen, tableX, lineY, col3X, lineY);
                    }
                }

                float textX1 = col1X + 1.5f;
                float textX2 = col2X + 1.5f;
                float textOffsetY = 1.7f;

                DrawText(graphics, "供应商名称/代码", titleFont, textBrush, textX1, tableY + rowHeight * 0 + textOffsetY);
                DrawText(graphics, CombineSupplier(info), valueFont, textBrush, textX2, tableY + rowHeight * 0 + textOffsetY);

                DrawText(graphics, "零件号", titleFont, textBrush, textX1, tableY + rowHeight * 1 + textOffsetY);
                DrawText(graphics, Safe(info.PartNo), valueFont, textBrush, textX2, tableY + rowHeight * 1 + textOffsetY);

                DrawText(graphics, "零件名称", titleFont, textBrush, textX1, tableY + rowHeight * 2 + textOffsetY);
                DrawText(graphics, Safe(info.PartName), valueFont, textBrush, textX2, tableY + rowHeight * 2 + textOffsetY);

                DrawText(graphics, "单包装数量(QTY)", titleFont, textBrush, textX1, tableY + rowHeight * 3 + textOffsetY);
                DrawText(graphics, Safe(info.Qty), valueFont, textBrush, textX2, tableY + rowHeight * 3 + textOffsetY);

                DrawText(graphics, "供货批次号(LOT NO.)", titleFont, textBrush, textX1, tableY + rowHeight * 4 + textOffsetY);
                DrawText(graphics, Safe(info.LotNo), valueFont, textBrush, textX2, tableY + rowHeight * 4 + textOffsetY);

                DrawText(graphics, "码放层数", titleFont, textBrush, textX1, tableY + rowHeight * 5 + textOffsetY);
                DrawText(graphics, Safe(info.LayerCount), valueFont, textBrush, textX2, tableY + rowHeight * 5 + textOffsetY);

                DrawText(graphics, "生产日期", titleFont, textBrush, textX1, tableY + rowHeight * 6 + textOffsetY);
                DrawText(graphics, Safe(info.ProduceDate), valueFont, textBrush, textX2, tableY + rowHeight * 6 + textOffsetY);

                DrawText(graphics, "检验确认/日期", titleFont, textBrush, textX1, tableY + rowHeight * 7 + textOffsetY);
                DrawText(graphics, Safe(info.CheckDate), valueFont, textBrush, textX2, tableY + rowHeight * 7 + textOffsetY);

                // 二维码
                string qrContent = new QrCodeGenerator().BuildDefaultQrContent(info);
                using (Image qrImage = new QrCodeGenerator().Generate(qrContent, 300, 300))
                {
                    float qrAreaX = col3X;
                    float qrAreaY = tableY + rowHeight * 3f;
                    float qrAreaWidth = rightColWidth;
                    float qrAreaHeight = tableHeight - rowHeight * 3f;
                    float qrSize = Math.Min(24f, Math.Min(qrAreaWidth - 4f, qrAreaHeight - 4f));
                    if (qrSize < 15f)
                    {
                        qrSize = Math.Min(qrAreaWidth, qrAreaHeight) - 2f;
                    }

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

        private static string Safe(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
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
