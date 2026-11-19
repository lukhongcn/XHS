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

        /// <summary>
        /// 使用正则表达式中指定的命名分组校验一个已拆分的字段值。
        /// </summary>
        public bool TryMatchNamedField(
            string rawValue,
            string parseExpression,
            string fieldName)
        {
            if (string.IsNullOrWhiteSpace(rawValue)
                || string.IsNullOrWhiteSpace(parseExpression)
                || string.IsNullOrWhiteSpace(fieldName))
            {
                return false;
            }

            string fieldExpression;
            if (!TryGetNamedGroupExpression(parseExpression, fieldName, out fieldExpression))
            {
                return false;
            }

            Regex fieldRegex = new Regex(
                "^(?:" + fieldExpression + ")$",
                RegexOptions.CultureInvariant);
            return fieldRegex.IsMatch(rawValue.Trim());
        }

        private static bool TryGetNamedGroupExpression(
            string parseExpression,
            string fieldName,
            out string fieldExpression)
        {
            fieldExpression = null;
            string marker = "(?<" + fieldName.Trim() + ">";
            int groupStart = parseExpression.IndexOf(marker, StringComparison.Ordinal);
            if (groupStart < 0)
            {
                return false;
            }

            int expressionStart = groupStart + marker.Length;
            int depth = 1;
            bool escaped = false;
            bool inCharacterClass = false;
            for (int index = expressionStart; index < parseExpression.Length; index++)
            {
                char current = parseExpression[index];
                if (escaped)
                {
                    escaped = false;
                    continue;
                }

                if (current == '\\')
                {
                    escaped = true;
                    continue;
                }

                if (current == '[')
                {
                    inCharacterClass = true;
                    continue;
                }

                if (current == ']' && inCharacterClass)
                {
                    inCharacterClass = false;
                    continue;
                }

                if (inCharacterClass)
                {
                    continue;
                }

                if (current == '(')
                {
                    depth++;
                    continue;
                }

                if (current == ')')
                {
                    depth--;
                    if (depth == 0)
                    {
                        fieldExpression = parseExpression.Substring(
                            expressionStart,
                            index - expressionStart);
                        return !string.IsNullOrWhiteSpace(fieldExpression);
                    }
                }
            }

            return false;
        }
    }
}
