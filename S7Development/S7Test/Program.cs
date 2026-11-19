using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Environment.ExitCode = HeartbeatTest() ? 0 : 1;
    }

    /// <summary>
    /// 心跳测试：Server 启动 5 秒后停止 PLC 心跳，客户端检测到故障并弹出提示。
    /// </summary>
    public static bool HeartbeatTest()
    {
        Process server = null;
        var monitor = new S7Help();
        using (var faultDetected = new ManualResetEventSlim(false))
        {
            try
            {
                string serverPath = FindPath("S7ServerSimulator", "S7ServerSimulator.exe");
                string configPath = FindPath("S7client", "bin", "Release", "S7client.json");
                if (!File.Exists(serverPath)) throw new FileNotFoundException("找不到 S7ServerSimulator.exe", serverPath);
                if (!File.Exists(configPath)) throw new FileNotFoundException("找不到 S7client.json", configPath);

                server = Process.Start(new ProcessStartInfo
                {
                    FileName = serverPath,
                    Arguments = "--db 50 --length 267 --stop-heartbeat-after-seconds 5",
                    WorkingDirectory = Path.GetDirectoryName(serverPath),
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                });

                Console.WriteLine("S7ServerSimulator 已启动，5 秒后停止 PLC 心跳。");
                monitor.StartHeartbeatMonitor(
                    configPath,
                    message => Console.WriteLine(message),
                    message =>
                    {
                        Console.Error.WriteLine(message);
                        if (faultDetected.IsSet) return;
                        faultDetected.Set();
                        MessageBox.Show(message, "S7Test 心跳故障", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    });

                bool detected = faultDetected.Wait(TimeSpan.FromSeconds(15));
                Console.WriteLine(detected ? "HeartbeatTest PASS" : "HeartbeatTest FAIL");
                return detected;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("HeartbeatTest FAIL: " + ex);
                return false;
            }
            finally
            {
                monitor.StopHeartbeatMonitor();
                if (server != null && !server.HasExited)
                {
                    try
                    {
                        server.Kill();
                        server.WaitForExit(2000);
                    }
                    catch { }
                }
                if (server != null) server.Dispose();
            }
        }
    }

    private static string FindPath(params string[] parts)
    {
        DirectoryInfo directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null && !string.Equals(directory.Name, "S7Development", StringComparison.OrdinalIgnoreCase))
            directory = directory.Parent;
        if (directory == null) throw new DirectoryNotFoundException("找不到 S7Development 目录。");

        string path = directory.FullName;
        foreach (string part in parts) path = Path.Combine(path, part);
        return path;
    }
}
