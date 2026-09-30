using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using ZebraLabelTest.Models;

namespace ZebraLabelTest.Engine
{
    public sealed class ZplGenerator
    {
        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        public ZplResult Generate(string templateFile, List<string> values)
        {
            try
            {
                LabelTemplate template = LoadTemplate(templateFile);
                Validate(template, values);

                int dpi = template.Printer.Dpi;
                int labelWidth = MmToDots(template.Label.WidthMm, dpi);
                int labelHeight = MmToDots(template.Label.HeightMm, dpi);
                StringBuilder zpl = new StringBuilder();
                zpl.AppendLine("^XA");
                zpl.AppendLine("^PW" + labelWidth);
                zpl.AppendLine("^LL" + labelHeight);

                foreach (LabelObject item in template.Objects)
                {
                    string value = EscapeFieldData(values[item.FieldIndex]);
                    if (string.Equals(item.Type, "qrcode", StringComparison.OrdinalIgnoreCase))
                    {
                        zpl.AppendLine(string.Format("^FO{0},{1}", item.Position.X, item.Position.Y));
                        zpl.AppendLine(string.Format("^BQN,2,{0}", item.Qrcode.Module));
                        zpl.AppendLine("^FDQA," + value + "^FS");
                    }
                    else if (string.Equals(item.Type, "text", StringComparison.OrdinalIgnoreCase))
                    {
                        zpl.AppendLine(string.Format("^FO{0},{1}", item.Position.X, item.Position.Y));
                        zpl.AppendLine(string.Format("^A0N,{0},{1}", item.Text.Height, item.Text.Width));
                        zpl.AppendLine("^FD" + value + "^FS");
                    }
                    else
                    {
                        throw new InvalidDataException("不支持的标签对象类型: " + item.Type);
                    }
                }

                zpl.Append("^XZ");
                return new ZplResult { Success = true, Zpl = zpl.ToString() };
            }
            catch (Exception ex)
            {
                return new ZplResult { Success = false, ErrorMessage = ex.Message };
            }
        }

        private LabelTemplate LoadTemplate(string templateFile)
        {
            if (string.IsNullOrWhiteSpace(templateFile) || !File.Exists(templateFile))
            {
                throw new FileNotFoundException("找不到标签模板文件。", templateFile);
            }

            LabelTemplate template = serializer.Deserialize<LabelTemplate>(File.ReadAllText(templateFile));
            if (template == null)
            {
                throw new InvalidDataException("标签模板内容为空。 ");
            }
            return template;
        }

        private static void Validate(LabelTemplate template, List<string> values)
        {
            if (template.Printer == null || template.Printer.Dpi <= 0)
                throw new InvalidDataException("模板缺少有效的 printer.dpi。");
            if (template.Label == null || template.Label.WidthMm <= 0 || template.Label.HeightMm <= 0)
                throw new InvalidDataException("模板缺少有效的 label 尺寸。");
            if (template.Objects == null || template.Objects.Count == 0)
                throw new InvalidDataException("模板缺少 objects。");
            if (values == null)
                throw new ArgumentNullException("values");

            foreach (LabelObject item in template.Objects)
            {
                if (item == null || item.Position == null)
                    throw new InvalidDataException("模板包含无效对象或坐标。");
                if (item.FieldIndex < 0 || item.FieldIndex >= values.Count)
                    throw new InvalidDataException("fieldIndex 超出 values 范围: " + item.FieldIndex);
                if (values[item.FieldIndex] == null)
                    throw new InvalidDataException("values 中存在空值，索引: " + item.FieldIndex);
                if (values[item.FieldIndex].IndexOfAny(new[] { '^', '~', '\r', '\n' }) >= 0)
                    throw new InvalidDataException("打印数据包含不能直接写入 ZPL 的特殊字符，索引: " + item.FieldIndex);

                if (string.Equals(item.Type, "qrcode", StringComparison.OrdinalIgnoreCase))
                {
                    if (item.Qrcode == null || item.Qrcode.Module <= 0)
                        throw new InvalidDataException("二维码对象缺少有效的 module。");
                }
                else if (string.Equals(item.Type, "text", StringComparison.OrdinalIgnoreCase))
                {
                    if (item.Text == null || item.Text.Height <= 0 || item.Text.Width <= 0)
                        throw new InvalidDataException("文本对象缺少有效的字体尺寸。");
                }
            }
        }

        private static int MmToDots(decimal millimeters, int dpi)
        {
            return (int)Math.Round(millimeters * dpi / 25.4m, MidpointRounding.AwayFromZero);
        }

        private static string EscapeFieldData(string value)
        {
            return value;
        }
    }
}
