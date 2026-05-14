using System.Security.Cryptography;
using System.Text;

namespace CheryPortHelp
{
    /// <summary>
    /// MD5 工具类。
    /// 奇瑞接口要求：MD5 加密为 32 位大写。
    /// </summary>
    public static class Md5Helper
    {
        public static string Md5Upper32(string content)
        {
            if (content == null)
            {
                content = string.Empty;
            }

            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(content);
                var hash = md5.ComputeHash(bytes);
                var builder = new StringBuilder(hash.Length * 2);

                foreach (var b in hash)
                {
                    builder.Append(b.ToString("X2"));
                }

                return builder.ToString();
            }
        }
    }
}
