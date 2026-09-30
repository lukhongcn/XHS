using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using XHS.service.Formatter;

namespace XHS.service.DateCode
{
    public sealed class XinWangDaDateCodeProvider : ICodeFormatter
    {
        private readonly Dictionary<int, string> yearMap;
        private readonly Dictionary<int, string> monthMap;
        private readonly Dictionary<int, string> dayMap;

        public XinWangDaDateCodeProvider(string configPath)
        {
            if (string.IsNullOrWhiteSpace(configPath))
            {
                throw new ArgumentException("日期规则配置文件路径不能为空。", nameof(configPath));
            }

            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException("找不到欣旺达日期规则配置文件。", configPath);
            }

            DateCodeRule rule = new JavaScriptSerializer()
                .Deserialize<DateCodeRule>(File.ReadAllText(configPath));

            ValidateRule(rule, configPath);
            yearMap = CreateMap(rule.Fields["Year"].Map, "年份", configPath);
            monthMap = CreateMap(rule.Fields["Month"].Map, "月份", configPath);
            dayMap = CreateMap(rule.Fields["Day"].Map, "日期", configPath);
        }

        public string Name
        {
            get { return "XinWangDaDate"; }
        }

        public string Encode(DateTime date)
        {
            string yearCode = GetCode(yearMap, date.Year, "年份", date);
            string monthCode = GetCode(monthMap, date.Month, "月份", date);
            string dayCode = GetCode(dayMap, date.Day, "日期", date);
            return yearCode + monthCode + dayCode;
        }

        public string Encode(object value)
        {
            if (!(value is DateTime))
            {
                throw new FormatException("日期 Formatter 需要 DateTime 类型输入。");
            }

            return Encode((DateTime)value);
        }

        public object Decode(string code)
        {
            if (string.IsNullOrWhiteSpace(code) || code.Trim().Length != 3)
            {
                throw new FormatException("欣旺达日期编码必须是 3 位字符。编码：" + code);
            }

            string normalizedCode = code.Trim().ToUpperInvariant();
            int year = DecodeUniqueValue(yearMap, normalizedCode[0].ToString(), "年份", code);
            int month = DecodeUniqueValue(monthMap, normalizedCode[1].ToString(), "月份", code);
            int day = DecodeUniqueValue(dayMap, normalizedCode[2].ToString(), "日期", code);

            return new DateTime(year, month, day);
        }

        private static void ValidateRule(DateCodeRule rule, string configPath)
        {
            if (rule == null || !string.Equals(rule.RuleName, "XinWangDaDate", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException("日期规则名称无效：" + configPath);
            }

            if (rule.Length != 3 || rule.Fields == null)
            {
                throw new InvalidDataException("欣旺达日期规则必须是 3 位且包含 Fields：" + configPath);
            }

            foreach (string fieldName in new[] { "Year", "Month", "Day" })
            {
                DateCodeFieldRule field;
                if (!rule.Fields.TryGetValue(fieldName, out field)
                    || field == null
                    || field.Map == null
                    || field.Map.Count == 0)
                {
                    throw new InvalidDataException("日期规则缺少 " + fieldName + " 映射：" + configPath);
                }
            }
        }

        private static Dictionary<int, string> CreateMap(
            Dictionary<string, string> source,
            string fieldName,
            string configPath)
        {
            var result = new Dictionary<int, string>();
            foreach (KeyValuePair<string, string> item in source)
            {
                int value;
                if (!int.TryParse(item.Key, NumberStyles.None, CultureInfo.InvariantCulture, out value)
                    || string.IsNullOrWhiteSpace(item.Value)
                    || item.Value.Length != 1)
                {
                    throw new InvalidDataException(fieldName + "映射无效：" + item.Key + " -> " + item.Value
                        + "，配置文件：" + configPath);
                }

                result[value] = item.Value.ToUpperInvariant();
            }

            return result;
        }

        private static string GetCode(
            Dictionary<int, string> map,
            int value,
            string fieldName,
            DateTime date)
        {
            string code;
            if (!map.TryGetValue(value, out code))
            {
                throw new ArgumentOutOfRangeException(
                    "date",
                    date,
                    fieldName + "没有配置对应编码。日期：" + date.ToString("yyyy-MM-dd"));
            }

            return code;
        }

        private static int DecodeUniqueValue(
            Dictionary<int, string> map,
            string code,
            string fieldName,
            string originalCode)
        {
            List<int> values = map
                .Where(item => string.Equals(item.Value, code, StringComparison.OrdinalIgnoreCase))
                .Select(item => item.Key)
                .ToList();

            if (values.Count == 0)
            {
                throw new FormatException(fieldName + "编码不存在：" + code + "。完整编码：" + originalCode);
            }

            if (values.Count > 1)
            {
                throw new FormatException(fieldName + "编码对应多个年份，无法仅凭 3 位编码确定日期："
                    + code + "。完整编码：" + originalCode);
            }

            return values[0];
        }
    }
}
