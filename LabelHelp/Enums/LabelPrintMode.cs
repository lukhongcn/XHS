namespace LabelHelp.Enums
{
    /// <summary>
    /// 标签输出方式。
    /// </summary>
    public enum LabelPrintMode
    {
        A4Pdf,        // 当前方案：生成 A4 PDF，一页多个标签
        RollPdf,      // 卷纸纵向 PDF，一页一张标签
        RollSinglePdf,// 卷纸纵向单标签 PDF
        SinglePdf,    // 生成单张标签 PDF
        LabelPrinter  // 后续方案：直接打印到标签打印机
    }
}
