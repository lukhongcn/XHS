using System.Collections.Generic;

namespace XHS.Model.ZPLLabel
{
    public sealed class ZplLabelTemplate
    {
        public string TemplateName { get; set; }
        public PrinterSettings Printer { get; set; }
        public LabelSettings Label { get; set; }
        public List<ZplLabelObject> Objects { get; set; }
    }

    public sealed class PrinterSettings
    {
        public string Type { get; set; }
        public int Dpi { get; set; }
    }

    public sealed class LabelSettings
    {
        public decimal WidthMm { get; set; }
        public decimal HeightMm { get; set; }
    }
}
