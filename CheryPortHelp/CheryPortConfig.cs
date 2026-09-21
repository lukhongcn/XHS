using System;
using System.Configuration;

namespace CheryPortHelp
{
    /// <summary>
    /// 奇瑞防错漏平台接口配置读取。
    /// 配置项请放在启动项目 App.config 的 appSettings 中。
    /// </summary>
    public static class CheryPortConfig
    {
        public static string AppKey => GetRequired("Chery.AppKey");
        public static string AppSecret => GetRequired("Chery.AppSecret");
        public static string SupplNo => GetRequired("Chery.SupplNo");
        public static string BaseNo => GetRequired("Chery.BaseNo");
        public static string SignType => Get("Chery.SignType", "MD5");
        public static string CheckRecordUrl => GetRequired("Chery.CheckRecordUrl");

        public static string DeliveryType => Get("Chery.DeliveryType", "1");
        public static int PackageType => GetInt("Chery.PackageType", 2);

        public static string ContentType => Get("Chery.ContentType", "application/json;charset=UTF8");
        public static int TimeoutSeconds => GetInt("Chery.TimeoutSeconds", 60);

        /// <summary>
        /// 可选配置：是否把 baseNo 也放到 Header。
        /// 文档示例代码里 addHeader("baseNo", baseNo)，Body 中 2.3 也有 baseNo。
        /// 默认 true，兼容示例。
        /// </summary>
        public static bool AddBaseNoHeader => GetBool("Chery.AddBaseNoHeader", true);

        public static string Get(string key, string defaultValue = "")
        {
            var value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();
        }

        public static int GetInt(string key, int defaultValue)
        {
            int value;
            return int.TryParse(Get(key), out value) ? value : defaultValue;
        }

        public static bool GetBool(string key, bool defaultValue)
        {
            bool value;
            return bool.TryParse(Get(key), out value) ? value : defaultValue;
        }

        private static string GetRequired(string key)
        {
            var value = Get(key);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException("缺少配置项：" + key);
            }

            return value;
        }
    }
}
