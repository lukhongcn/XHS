using System;
using System.IO;

namespace CheryCheckSystem.PrintClient
{
    public class PrintClientFileLogger
    {
        private readonly object _syncRoot = new object();
        private readonly string _logFilePath;

        public PrintClientFileLogger(string logFolder)
        {
            string safeLogFolder = string.IsNullOrWhiteSpace(logFolder)
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs")
                : logFolder.Trim();

            Directory.CreateDirectory(safeLogFolder);
            _logFilePath = Path.Combine(safeLogFolder, "print-client.log");
        }

        public void Write(string message)
        {
            string line = string.Format("{0:yyyy-MM-dd HH:mm:ss}  {1}", DateTime.Now, message);
            lock (_syncRoot)
            {
                File.AppendAllText(_logFilePath, line + Environment.NewLine, System.Text.Encoding.UTF8);
            }
        }
    }
}
