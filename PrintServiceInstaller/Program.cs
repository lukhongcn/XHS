using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PrintServiceInstaller
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.ThreadException += delegate(object sender, System.Threading.ThreadExceptionEventArgs e)
            {
                HandleUnhandledException(e.Exception);
            };

            AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e)
            {
                HandleUnhandledException(e.ExceptionObject as Exception);
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new InstallerForm());
        }

        private static void HandleUnhandledException(Exception exception)
        {
            string message = exception == null ? "未知错误。" : exception.ToString();
            WriteCrashLog(message);
            MessageBox.Show(
                "安装程序发生异常，详细信息已写入日志。" + Environment.NewLine + message,
                "安装程序异常",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private static void WriteCrashLog(string message)
        {
            try
            {
                string logFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "MESPrintService",
                    "logs");
                Directory.CreateDirectory(logFolder);
                string logPath = Path.Combine(logFolder, "installer-crash-" + DateTime.Now.ToString("yyyyMMdd") + ".log");
                File.AppendAllText(
                    logPath,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + message + Environment.NewLine,
                    new UTF8Encoding(true));
            }
            catch
            {
            }
        }
    }
}
