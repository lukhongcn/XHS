using System;
using System.Configuration;

namespace CheryCheckSystem.PrintClient
{
    public class PrintClientSettings
    {
        public string PendingApiUrl { get; private set; }
        public string PendingApiPath { get; private set; }
        public string ApiBaseUrl { get; private set; }
        public string CompleteApiUrl { get; private set; }
        public string FailApiUrl { get; private set; }
        public string MachineId { get; private set; }
        public string PrinterName { get; private set; }
        public int PollIntervalSeconds { get; private set; }
        public int PrintPauseMilliseconds { get; private set; }
        public int LockTimeoutMinutes { get; private set; }
        public string LogFolder { get; private set; }

        public static PrintClientSettings Load()
        {
            return new PrintClientSettings
            {
                PendingApiUrl = GetSetting("PrintClient.PendingApiUrl", "http://localhost:55426/api/print/pending-labels?machineId=PRT-01"),
                PendingApiPath = GetSetting("PrintClient.PendingApiPath", "/api/print/pending-labels"),
                ApiBaseUrl = GetSetting("PrintClient.ApiBaseUrl", "http://localhost:55426"),
                CompleteApiUrl = GetSetting("PrintClient.CompleteApiUrl", "http://localhost:55426/api/print/complete"),
                FailApiUrl = GetSetting("PrintClient.FailApiUrl", "http://localhost:55426/api/print/fail"),
                MachineId = GetSetting("PrintClient.MachineId", "PRT-01"),
                PrinterName = GetSetting("PrintClient.PrinterName", string.Empty),
                PollIntervalSeconds = GetIntSetting("PrintClient.PollIntervalSeconds", 10, 1, 3600),
                PrintPauseMilliseconds = GetIntSetting("PrintClient.PrintPauseMilliseconds", 1500, 0, 60000),
                LockTimeoutMinutes = GetIntSetting("PrintClient.LockTimeoutMinutes", 3, 1, 60),
                LogFolder = GetSetting("PrintClient.LogFolder", @"C:\CheryMES\PrintService\logs")
            };
        }

        private static string GetSetting(string key, string defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();
        }

        private static int GetIntSetting(string key, int defaultValue, int minValue, int maxValue)
        {
            int value;
            if (!int.TryParse(ConfigurationManager.AppSettings[key], out value))
            {
                value = defaultValue;
            }

            if (value < minValue)
            {
                value = minValue;
            }

            if (value > maxValue)
            {
                value = maxValue;
            }

            return value;
        }
    }
}
