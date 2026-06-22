using System;
using System.Collections.Generic;
using LabelHelp.Config;
using LabelHelp.Enums;
using LabelHelp.Pdf;
using LabelHelp.Printer;
using XHS.Model.Label;

namespace LabelHelp.Services
{
    /// <summary>
    /// 标签打印统一入口。业务代码建议只调用这个类。
    /// </summary>
    public class LabelPrintService
    {
        public string Generate(List<LabelInfo> labelList, LabelTemplateType templateType, LabelPrintMode printMode)
        {
            if (labelList == null || labelList.Count == 0) throw new ArgumentException("labelList 不能为空。", "labelList");

            switch (printMode)
            {
                case LabelPrintMode.RollPdf:
                    return new A4SheetBuilder().GenerateA4Pdf(labelList, templateType, LabelPrintConfig.LoadRollPaper());
                case LabelPrintMode.SinglePdf:
                    return new LabelPdfBuilder().GenerateSingleLabelPdf(labelList[0], templateType, LabelPrintConfig.Load());
                case LabelPrintMode.LabelPrinter:
                    throw new NotSupportedException("LabelPrinter 模式当前暂未实现。");
                case LabelPrintMode.A4Pdf:
                default:
                    return new A4SheetBuilder().GenerateA4Pdf(labelList, templateType, LabelPrintConfig.Load());
            }
        }

        public void Print(List<LabelInfo> labelList, LabelTemplateType templateType, LabelPrintMode printMode, string printerName)
        {
            if (labelList == null || labelList.Count == 0) throw new ArgumentException("labelList 不能为空。", "labelList");

            switch (printMode)
            {
                case LabelPrintMode.SinglePdf:
                    new PdfPrintService().PrintPdf(new LabelPdfBuilder().GenerateSingleLabelPdf(labelList[0], templateType, LabelPrintConfig.Load()), printerName);
                    break;
                case LabelPrintMode.LabelPrinter:
                    LabelPrintConfig printerConfig = templateType == LabelTemplateType.TableLabel
                        ? LabelPrintConfig.LoadRollPaper()
                        : LabelPrintConfig.Load();

                    foreach (LabelInfo info in labelList)
                    {
                        new LabelPrinterOutput().PrintSingleLabel(info, templateType, printerConfig, printerName);
                    }
                    break;
                case LabelPrintMode.A4Pdf:
                default:
                    new PdfPrintService().PrintPdf(new A4SheetBuilder().GenerateA4Pdf(labelList, templateType, LabelPrintConfig.Load()), printerName);
                    break;
            }
        }
    }
}
