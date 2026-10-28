using System;
using System.Collections.Generic;
using LabelHelp.Config;
using LabelHelp.Enums;
using XHS.Model.Label;

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

        /// <summary>
        /// 生成多页单标签 PDF，每页一张独立的标签。
        /// </summary>
        public string GenerateMultiPageSingleLabelPdf(List<LabelInfo> labelList, LabelTemplateType templateType, LabelPrintConfig config)
        {
            if (labelList == null || labelList.Count == 0)
            {
                throw new ArgumentException("labelList 不能为空。", "labelList");
            }

            LabelPrintConfig singleConfig = CloneForSingleLabel(config);

            return new A4SheetBuilder().GenerateA4Pdf(labelList, templateType, singleConfig);
        }

        private LabelPrintConfig CloneForSingleLabel(LabelPrintConfig config)
        {
            float tableMarginV = 2f;
            float tableMarginLeft = 3f;

            return new LabelPrintConfig
            {
                LabelWidth = config.LabelWidth - tableMarginV * 2f,
                LabelHeight = config.LabelHeight - tableMarginV * 2f,

                A4PageWidth = config.LabelWidth,
                A4PageHeight = config.LabelHeight,
                A4MarginLeft = tableMarginLeft,
                A4MarginTop = tableMarginV,
                A4Columns = 1,
                A4Rows = 1,
                A4ColumnGap = 0f,
                A4RowGap = 0f,

                FontName = config.FontName,
                TitleFontSize = config.TitleFontSize,
                ValueFontSize = config.ValueFontSize,
                TableLeftColumnRatio = config.TableLeftColumnRatio,
                TableMiddleColumnRatio = config.TableMiddleColumnRatio,
                TableQrColumnRatio = config.TableQrColumnRatio,

                TemplateFolder = config.TemplateFolder,
                OutputFolder = config.OutputFolder
            };
        }
    }
}
