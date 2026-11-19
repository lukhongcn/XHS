using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace BarcodeDesigner
{
    public sealed class BarcodeCustomerProfile
    {
        public string CustomerName { get; set; }
        public string CustomerAbbr { get; set; }
        public string UiJsonName { get; set; }
        public string MenuId { get; set; }
        public string PrintRuleFile { get; set; }
        public string FormatterConfigDirectory { get; set; }

        public override string ToString()
        {
            return CustomerName;
        }
    }

    public static class BarcodeCustomerProfileLoader
    {
        public static List<BarcodeCustomerProfile> Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("找不到客户配置文件。", filePath);
            }

            List<BarcodeCustomerProfile> profiles =
                new JavaScriptSerializer().Deserialize<List<BarcodeCustomerProfile>>(
                    File.ReadAllText(filePath));

            if (profiles == null || profiles.Count == 0)
            {
                throw new InvalidDataException("客户配置文件没有可用客户。" );
            }

            foreach (BarcodeCustomerProfile profile in profiles)
            {
                if (profile == null
                    || string.IsNullOrWhiteSpace(profile.CustomerName)
                    || string.IsNullOrWhiteSpace(profile.CustomerAbbr)
                    || string.IsNullOrWhiteSpace(profile.UiJsonName)
                    || string.IsNullOrWhiteSpace(profile.MenuId)
                    || string.IsNullOrWhiteSpace(profile.PrintRuleFile)
                    || string.IsNullOrWhiteSpace(profile.FormatterConfigDirectory))
                {
                    throw new InvalidDataException("客户配置中存在无效项目。" );
                }
            }

            return profiles;
        }
    }
}
