namespace LabelHelp.Enums
{
    /// <summary>
    /// 标签输出方式。
    /// </summary>
    public enum LabelPrintMode
    {
        A4Pdf,        // 当前方案：生成 A4 PDF，一页多个标签
        SinglePdf,    // 生成单张标签 PDF
        LabelPrinter  // 后续方案：直接打印到标签打印机
    }
}
