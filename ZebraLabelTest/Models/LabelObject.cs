namespace ZebraLabelTest.Models
{
    public sealed class LabelObject
    {
        public string Type { get; set; }
        public int FieldIndex { get; set; }
        public Position Position { get; set; }
        public QrCodeSettings Qrcode { get; set; }
        public TextSettings Text { get; set; }
    }

    public sealed class Position
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public sealed class QrCodeSettings
    {
        public int Module { get; set; }
    }

    public sealed class TextSettings
    {
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
