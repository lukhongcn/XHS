using System;
using System.Collections.Generic;
using LabelHelp.Config;
using LabelHelp.Enums;
using Model.Label;

namespace LabelHelp.Pdf
{
    /// <summary>
    /// 单张标签 PDF 生成器。
    /// 当前实现方式：复用 A4SheetBuilder 的绘制逻辑，生成单张标签尺寸的 PDF。
    /// </summary>
    public class LabelPdfBuilder
    {
        public string GenerateSingleLabelPdf(LabelInfo info, LabelTemplateType templateType, LabelPrintConfig config)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            LabelPrintConfig singleConfig = CloneForSingleLabel(config);

            return new A4SheetBuilder().GenerateA4Pdf(
                new List<LabelInfo> { info },
                templateType,
                singleConfig);
        }

        private LabelPrintConfig CloneForSingleLabel(LabelPrintConfig config)
        {
            return new LabelPrintConfig
            {
                LabelWidth = config.LabelWidth,
                LabelHeight = config.LabelHeight,

                A4PageWidth = config.LabelWidth,
                A4PageHeight = config.LabelHeight,
                A4MarginLeft = 0f,
                A4MarginTop = 0f,
                A4Columns = 1,
                A4Rows = 1,
                A4ColumnGap = 0f,
                A4RowGap = 0f,

                FontName = config.FontName,
                TitleFontSize = config.TitleFontSize,
                ValueFontSize = config.ValueFontSize,

                TemplateFolder = config.TemplateFolder,
                OutputFolder = config.OutputFolder
            };
        }
    }
}
