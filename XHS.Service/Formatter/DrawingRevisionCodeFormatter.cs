using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace XHS.service.Formatter
{
    public sealed class DrawingRevisionCodeFormatter : ICodeFormatter
    {
        private readonly Dictionary<string, string> encodeMap;
        private readonly Dictionary<string, string> decodeMap;

        public DrawingRevisionCodeFormatter(string configPath)
        {
            if (string.IsNullOrWhiteSpace(configPath))
            {
                throw new ArgumentException("图纸版本号规则配置文件路径不能为空。", nameof(configPath));
            }

            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException("找不到图纸版本号规则配置文件。", configPath);
            }

            DrawingRevisionRule rule = new JavaScriptSerializer()
                .Deserialize<DrawingRevisionRule>(File.ReadAllText(configPath));

            ValidateRule(rule, configPath);
            encodeMap = new Dictionary<string, string>(rule.Map, StringComparer.OrdinalIgnoreCase);
            decodeMap = CreateDecodeMap(encodeMap, configPath);
        }

        public string Name
        {
            get { return "XinWangDaDrawingRevision"; }
        }

        public string Encode(object value)
        {
            return EncodeRevision(value == null ? null : Convert.ToString(value));
        }

        public object Decode(string code)
        {
            string normalizedCode = NormalizeCode(code);
            string revision;
            if (!decodeMap.TryGetValue(normalizedCode, out revision))
            {
                throw new FormatException("图纸版本代码不存在：" + code);
            }

            return revision;
        }

        public string EncodeRevision(string revision)
        {
            string normalizedRevision = NormalizeRevision(revision);
            string code;
            if (!encodeMap.TryGetValue(normalizedRevision, out code))
            {
                throw new FormatException("图纸版本号不存在：" + revision);
            }

            return code;
        }

        public string DecodeRevision(string code)
        {
            return (string)Decode(code);
        }

        private static void ValidateRule(DrawingRevisionRule rule, string configPath)
        {
            if (rule == null
                || !string.Equals(rule.Formatter, "XinWangDaDrawingRevision", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException("图纸版本号 Formatter 无效：" + configPath);
            }

            if (rule.Length != 2 || rule.Map == null || rule.Map.Count == 0)
            {
                throw new InvalidDataException("图纸版本号规则必须是 2 位且包含 Map：" + configPath);
            }

            foreach (KeyValuePair<string, string> item in rule.Map)
            {
                if (string.IsNullOrWhiteSpace(item.Key)
                    || item.Key.Trim().Length != 2
                    || string.IsNullOrWhiteSpace(item.Value)
                    || item.Value.Trim().Length != 1)
                {
                    throw new InvalidDataException("图纸版本号映射无效："
                        + item.Key + " -> " + item.Value + "，配置文件：" + configPath);
                }
            }
        }

        private static Dictionary<string, string> CreateDecodeMap(
            Dictionary<string, string> source,
            string configPath)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, string> item in source)
            {
                string code = item.Value.Trim().ToUpperInvariant();
                if (result.ContainsKey(code))
                {
                    throw new InvalidDataException("图纸版本号代码重复：" + code
                        + "，配置文件：" + configPath);
                }

                result.Add(code, item.Key.Trim().ToUpperInvariant());
            }

            return result;
        }

        private static string NormalizeRevision(string revision)
        {
            if (string.IsNullOrWhiteSpace(revision) || revision.Trim().Length != 2)
            {
                throw new FormatException("图纸版本号必须是 2 位字符：" + revision);
            }

            return revision.Trim().ToUpperInvariant();
        }

        private static string NormalizeCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code) || code.Trim().Length != 1)
            {
                throw new FormatException("图纸版本代码必须是 1 位字符：" + code);
            }

            return code.Trim().ToUpperInvariant();
        }
    }

    internal sealed class DrawingRevisionRule
    {
        public string Formatter { get; set; }

        public int Length { get; set; }

        public Dictionary<string, string> Map { get; set; }
    }
}
