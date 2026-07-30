using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace XHS.BLL
{
    /// <summary>
    /// 正则表达式字段解析器，根据正则表达式从条码字符串中提取命名分组。
    /// </summary>
    public class RegexFieldParser
    {
        public IDictionary<string, string> Parse(
            string rawCode,
            string parseExpression)
        {
            IDictionary<string, string> fields;

            if (!TryParse(rawCode, parseExpression, out fields))
                throw new FormatException("条码不符合正则解析规则。");

            return fields;
        }

        public bool TryParse(
            string rawCode,
            string parseExpression,
            out IDictionary<string, string> fields)
        {
            var regex = new Regex(
                parseExpression,
                RegexOptions.CultureInvariant);

            var match = regex.Match(rawCode);

            if (!match.Success)
            {
                fields = null;
                return false;
            }

            fields = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);

            foreach (string groupName in regex.GetGroupNames())
            {
                if (int.TryParse(groupName, out _))
                    continue;

                Group group = match.Groups[groupName];

                if (group.Success)
                {
                    fields[groupName] = group.Value;
                }
            }

            return true;
        }
    }
}
