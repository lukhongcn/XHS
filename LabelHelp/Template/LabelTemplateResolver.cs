using System.IO;
using LabelHelp.Config;
using LabelHelp.Enums;

namespace LabelHelp.Template
{
    /// <summary>
    /// 根据标签模板类型找到对应的 PDF 模板路径。
    /// </summary>
    public class LabelTemplateResolver
    {
        public string GetTemplatePath(LabelTemplateType templateType, LabelPrintConfig config)
        {
            string fileName;
            switch (templateType)
            {
                case LabelTemplateType.CardSingleLabel:
                    fileName = "CardSingleLabel_100x60.pdf";
                    break;
                case LabelTemplateType.CardPackageLabel:
                    fileName = "CardPackageLabel_100x60.pdf";
                    break;
                case LabelTemplateType.TableLabel:
                default:
                    fileName = "TableLabel_100x60.pdf";
                    break;
            }
            return Path.Combine(config.TemplateFolder, fileName);
        }
    }
}
