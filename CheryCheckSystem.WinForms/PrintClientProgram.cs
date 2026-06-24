using System;
using System.ServiceProcess;
using System.Threading;

namespace CheryCheckSystem.PrintClient
{
    internal static class PrintClientProgram
    {
        private static void Main(string[] args)
        {
            if (Environment.UserInteractive || HasConsoleArgument(args))
            {
                RunAsConsole();
                return;
            }

            ServiceBase.Run(new PrintClientService());
        }

        private static void RunAsConsole()
        {
            using (PrintClientService service = new PrintClientService())
            {
                service.StartAsConsole();
                Console.WriteLine("XHS 打印客户端服务已启动，按 Ctrl+C 退出。");

                using (ManualResetEvent quitEvent = new ManualResetEvent(false))
                {
                    Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e)
                    {
                        e.Cancel = true;
                        quitEvent.Set();
                    };

                    quitEvent.WaitOne();
                }

                service.StopAsConsole();
            }
        }

        private static bool HasConsoleArgument(string[] args)
        {
            if (args == null)
            {
                return false;
            }

            foreach (string arg in args)
            {
                if (string.Equals(arg, "/console", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(arg, "-console", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
