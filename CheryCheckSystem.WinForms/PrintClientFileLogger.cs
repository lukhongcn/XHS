using System;
using System.IO;

namespace CheryCheckSystem.PrintClient
{
    public class PrintClientFileLogger
    {
        private readonly object _syncRoot = new object();
        private readonly string _logFilePath;
        private readonly string _fetchLogFilePath;

        public PrintClientFileLogger(string logFolder)
        {
            string safeLogFolder = string.IsNullOrWhiteSpace(logFolder)
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs")
                : logFolder.Trim();

            Directory.CreateDirectory(safeLogFolder);
            _logFilePath = Path.Combine(safeLogFolder, "print-client.log");
            _fetchLogFilePath = Path.Combine(safeLogFolder, "print-client-fetch.log");
        }

        public void Write(string message)
        {
            WriteToFile(_logFilePath, message);
        }

        public void WriteFetch(string message)
        {
            WriteToFile(_fetchLogFilePath, message);
        }

        private void WriteToFile(string filePath, string message)
        {
            string line = string.Format("{0:yyyy-MM-dd HH:mm:ss.fff}  {1}", DateTime.Now, message);
            lock (_syncRoot)
            {
                File.AppendAllText(filePath, line + Environment.NewLine, System.Text.Encoding.UTF8);
            }
        }
    }
}
