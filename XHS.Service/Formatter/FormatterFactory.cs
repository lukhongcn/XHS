using System;
using System.IO;
using XHS.Model.BarcodeCreateRule;
using XHS.service.DateCode;
using XHS.service.SerialNumber;

namespace XHS.service.Formatter
{
    public sealed class FormatterFactory
    {
        private readonly string configDirectory;

        public FormatterFactory()
            : this(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config"))
        {
        }

        public FormatterFactory(string configDirectory)
        {
            if (string.IsNullOrWhiteSpace(configDirectory))
            {
                throw new ArgumentException("Formatter 配置目录不能为空。", nameof(configDirectory));
            }

            this.configDirectory = configDirectory;
        }

        public ICodeFormatter Get(
            BarcodeFieldRule field,
            Func<DateTime, int> getPrintCount)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            string formatter = field.Formatter;
            if (string.IsNullOrWhiteSpace(formatter))
            {
                throw new ArgumentException("Formatter 不能为空。", nameof(formatter));
            }

            if (string.Equals(formatter, "XinWangDaDrawingRevision", StringComparison.OrdinalIgnoreCase))
            {
                string configPath = Path.Combine(configDirectory, "XinWangDaDrawingRevision.json");
                return new DrawingRevisionCodeFormatter(configPath);
            }

            if (string.Equals(formatter, "XinWangDaDate", StringComparison.OrdinalIgnoreCase))
            {
                return new XinWangDaDateCodeProvider(
                    Path.Combine(configDirectory, "XinWangDaDate.json"));
            }

            if (string.Equals(formatter, "DailySerialNumber", StringComparison.OrdinalIgnoreCase))
            {
                if (getPrintCount == null)
                {
                    throw new ArgumentNullException(nameof(getPrintCount));
                }

                if (!string.Equals(field.StartGenerator, "PrintCountPlusOne", StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotSupportedException(
                        "不支持的序列号 StartGenerator：" + field.StartGenerator);
                }

                return new DailySerialNumberGenerator(
                    getPrintCount,
                    field.InitialValue,
                    field.Format);
            }

            throw new NotSupportedException("不支持的 Formatter：" + formatter);
        }
    }
}
