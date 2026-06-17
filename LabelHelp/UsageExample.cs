using System.Collections.Generic;
using LabelHelp.Enums;
using LabelHelp.Services;
using XHS.Model.Label;

namespace LabelHelp
{
    /// <summary>
    /// 使用示例。这个文件只是参考，不一定要放进正式项目编译。
    /// </summary>
    public class UsageExample
    {
        public string GenerateA4LabelPdf()
        {
            List<LabelInfo> labels = new List<LabelInfo>();
            labels.Add(new LabelInfo
            {
                SupplierName = "示例供应商",
                SupplierCode = "8KN",
                PartNo = "J42-5402175HA",
                PartName = "传动块",
                Qty = "120",
                LotNo = "2023-10-11",
                LayerCount = "1",
                ProduceDate = "2023-10-11",
                SerialNo = "00001"
            });

            LabelPrintService service = new LabelPrintService();
            return service.Generate(labels, LabelTemplateType.TableLabel, LabelPrintMode.A4Pdf);
        }
    }
}
