using LabelHelp.Config;
using LabelHelp.Enums;
using Model.Label;

namespace LabelHelp.Printer
{
    /// <summary>
    /// 标签打印机指令生成器。用于 Zebra ZPL / TSC TSPL 等指令输出。
    /// </summary>
    public class LabelCommandBuilder
    {
        public string BuildZpl(LabelInfo info, LabelTemplateType templateType, LabelPrintConfig config)
        {
            // TODO: 生成 Zebra ZPL 指令。
            return string.Empty;
        }

        public string BuildTspl(LabelInfo info, LabelTemplateType templateType, LabelPrintConfig config)
        {
            // TODO: 生成 TSC TSPL 指令。
            return string.Empty;
        }
    }
}
