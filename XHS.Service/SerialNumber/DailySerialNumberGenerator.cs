using System;
using System.Collections.Generic;
using XHS.service.Formatter;

namespace XHS.service.SerialNumber
{
    public sealed class DailySerialNumberGenerator : ICodeFormatter
    {
        private readonly Func<DateTime, int> getPrintCount;
        private readonly int initialValue;
        private readonly string format;
        private DateTime? currentDate;
        private int nextNumber;

        public DailySerialNumberGenerator(
            Func<DateTime, int> getPrintCount,
            string initialValue,
            string format)
        {
            if (getPrintCount == null)
            {
                throw new ArgumentNullException(nameof(getPrintCount));
            }

            this.getPrintCount = getPrintCount;
            this.initialValue = ParseInitialValue(initialValue);
            this.format = ValidateFormat(format);
        }

        public string Name
        {
            get { return "DailySerialNumber"; }
        }

        public string Encode(object value)
        {
            if (!(value is DateTime))
            {
                throw new FormatException("序列号 Formatter 需要 DateTime 类型输入。");
            }

            DateTime date = (DateTime)value;
            if (!currentDate.HasValue || currentDate.Value.Date != date.Date)
            {
                int printedCount = getPrintCount(date.Date);
                if (printedCount < 0)
                {
                    throw new InvalidOperationException("当天已打印数量不能为负数。当天已打印数量：" + printedCount);
                }

                nextNumber = printedCount == 0
                    ? initialValue
                    : printedCount + 1;
                currentDate = date.Date;
            }

            if (nextNumber > 99999)
            {
                throw new InvalidOperationException("当天序列号已超过 5 位数字范围：" + nextNumber);
            }

            return (nextNumber++).ToString(format);
        }

        public object Decode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)
                || code.Trim().Length != format.Length)
            {
                throw new FormatException("序列号编码格式无效：" + code);
            }

            return code.Trim();
        }

        private static int ParseInitialValue(string value)
        {
            int result;
            if (string.IsNullOrWhiteSpace(value)
                || value.Trim().Length != 5
                || !int.TryParse(value.Trim(), out result)
                || result < 0
                || result > 99999)
            {
                throw new FormatException(
                    "序列号 InitialValue 必须是 5 位数字，例如 00001。当前值：" + value);
            }

            return result;
        }

        private static string ValidateFormat(string value)
        {
            if (string.IsNullOrWhiteSpace(value)
                || value.Trim().Length == 0
                || value.Trim().Trim('0').Length != 0)
            {
                throw new FormatException("序列号 Format 必须是全 0 格式，例如 00000。当前值：" + value);
            }

            return value.Trim();
        }
    }
}
