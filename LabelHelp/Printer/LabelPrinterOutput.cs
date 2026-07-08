using System;
using System.Drawing;
using System.Drawing.Printing;
using LabelHelp.Config;
using LabelHelp.Enums;
using LabelHelp.Rendering;
using XHS.Model.Label;

namespace LabelHelp.Printer
{
    /// <summary>
    /// 标签打印机输出类。后续客户改用标签打印机时实现。
    /// </summary>
    public class LabelPrinterOutput
    {
        public void PrintSingleLabel(LabelInfo info, LabelTemplateType templateType, LabelPrintConfig config, string printerName)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            using (PrintDocument document = new PrintDocument())
            {
                if (!string.IsNullOrWhiteSpace(printerName))
                {
                    document.PrinterSettings.PrinterName = printerName;
                }

                if (!document.PrinterSettings.IsValid)
                {
                    throw new InvalidOperationException("打印机无效：" + printerName);
                }

                document.PrintController = new StandardPrintController();
                document.DocumentName = BuildDocumentName(info);
                document.DefaultPageSettings.Landscape = false;
                document.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                document.DefaultPageSettings.PaperSize = BuildPaperSize(config);

                document.PrintPage += delegate(object sender, PrintPageEventArgs e)
                {
                    e.PageSettings.Landscape = false;
                    e.PageSettings.Margins = new Margins(0, 0, 0, 0);
                    e.PageSettings.PaperSize = BuildPaperSize(config);

                    Graphics graphics = e.Graphics;
                    graphics.PageUnit = GraphicsUnit.Millimeter;
                    graphics.Clear(Color.White);

                    float hardMarginX = HundredthsOfInchToMm(e.PageSettings.HardMarginX);
                    float hardMarginY = HundredthsOfInchToMm(e.PageSettings.HardMarginY);
                    float renderOffsetX = config.PrintOffsetX - hardMarginX;
                    float renderOffsetY = config.PrintOffsetY - hardMarginY;

                    LabelPrintConfig renderConfig = CloneRenderConfig(config);
                    new LabelRenderer().Render(graphics, info, templateType, renderConfig, renderOffsetX, renderOffsetY);
                    e.HasMorePages = false;
                };

                document.Print();
            }
        }

        private static string BuildDocumentName(LabelInfo info)
        {
            string safePartNo = info == null || string.IsNullOrWhiteSpace(info.PartNo)
                ? "Label"
                : info.PartNo.Trim();
            string safePackageCode = info == null || string.IsNullOrWhiteSpace(info.PackageCode)
                ? "Unknown"
                : info.PackageCode.Trim();
            return string.Format("Label_{0}_{1}", safePartNo, safePackageCode);
        }

        private static PaperSize BuildPaperSize(LabelPrintConfig config)
        {
            int width = MmToHundredthsOfInch(config.LabelWidth);
            int height = MmToHundredthsOfInch(config.LabelHeight);
            string paperName = string.Format(
                "Custom_{0}x{1}mm",
                Math.Round(config.LabelWidth),
                Math.Round(config.LabelHeight));
            return new PaperSize(paperName, width, height);
        }

        private static int MmToHundredthsOfInch(float millimeter)
        {
            return Math.Max(1, (int)Math.Round(millimeter / 25.4f * 100f));
        }

        private static float HundredthsOfInchToMm(float hundredthsOfInch)
        {
            return hundredthsOfInch / 100f * 25.4f;
        }

        private static LabelPrintConfig CloneRenderConfig(LabelPrintConfig config)
        {
            return new LabelPrintConfig
            {
                LabelWidth = config.PrintRenderWidth > 0 ? config.PrintRenderWidth : config.LabelWidth,
                LabelHeight = config.PrintRenderHeight > 0 ? config.PrintRenderHeight : config.LabelHeight,
                A4PageWidth = config.A4PageWidth,
                A4PageHeight = config.A4PageHeight,
                A4MarginLeft = config.A4MarginLeft,
                A4MarginTop = config.A4MarginTop,
                A4Columns = config.A4Columns,
                A4Rows = config.A4Rows,
                A4ColumnGap = config.A4ColumnGap,
                A4RowGap = config.A4RowGap,
                FontName = config.FontName,
                TitleFontSize = config.TitleFontSize,
                ValueFontSize = config.ValueFontSize,
                TemplateFolder = config.TemplateFolder,
                OutputFolder = config.OutputFolder,
                PrintOffsetX = config.PrintOffsetX,
                PrintOffsetY = config.PrintOffsetY,
                PrintRenderWidth = config.PrintRenderWidth,
                PrintRenderHeight = config.PrintRenderHeight
            };
        }
    }
}
