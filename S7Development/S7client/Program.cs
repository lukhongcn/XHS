using System;
using System.Diagnostics;
using System.Linq;
using S7.Net;
using System.Windows.Forms;

internal static class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            return;
        }

        string configPath = ReadString(args, "--config", null);
        if (!string.IsNullOrWhiteSpace(configPath))
        {
            RunConfigured(configPath,
                args.Any(a => string.Equals(a, "--monitor", StringComparison.OrdinalIgnoreCase)),
                args.Any(a => string.Equals(a, "--popup", StringComparison.OrdinalIgnoreCase)));
            return;
        }

        RunCli(args);
    }

    internal static void RunCli(string[] args)
    {
        string ip = ReadString(args, "--ip", "127.0.0.1");
        int db = ReadInt(args, "--db", 1), offset = ReadInt(args, "--offset", 8), length = ReadInt(args, "--length", 8);
        byte[] expected = Enumerable.Range(1, length).Select(i => (byte)(i * 0x10)).ToArray();
        bool readOnly = args.Any(a => string.Equals(a, "--read-only", StringComparison.OrdinalIgnoreCase));
        var timer = Stopwatch.StartNew();
        try
        {
            using (var plc = new Plc(CpuType.S71200, ip, 0, 1))
            {
                plc.Open();
                var s7Help = new S7Help();
                Console.WriteLine("CONNECT ip={0} status={1}", ip, plc.IsConnected ? "OK" : "FAILED");
                byte[] actual = s7Help.ReadBlock(plc, db, offset, length);
                if (!readOnly)
                {
                    s7Help.WriteBlock(plc, db, offset, expected);
                    actual = s7Help.ReadBlock(plc, db, offset, length);
                    Console.WriteLine("WRITE db={0} offset={1} length={2} hex={3}", db, offset, length, ToHex(expected));
                }
                Console.WriteLine("READ  db={0} offset={1} length={2} hex={3}", db, offset, length, ToHex(actual));
                if (!readOnly && !expected.SequenceEqual(actual)) throw new InvalidOperationException("Read-back bytes differ from written bytes.");
                Console.WriteLine("VERIFY SUCCESS elapsedMs={0}", timer.ElapsedMilliseconds);
                plc.Close();
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("VERIFY FAILED elapsedMs={0} error={1}", timer.ElapsedMilliseconds, ex);
            Environment.ExitCode = 1;
        }
    }

    private static void RunConfigured(string configPath, bool monitor, bool popup)
    {
        var s7Help = new S7Help();
        Action<string> output = message => Console.WriteLine(message);
        Action<string> fault = message =>
        {
            Console.Error.WriteLine(message);
            if (popup) MessageBox.Show(message, "MES 通讯报警", MessageBoxButtons.OK, MessageBoxIcon.Error);
        };

        if (!monitor)
        {
            s7Help.CheckHeartbeats(configPath, output, fault);
            return;
        }

        s7Help.StartHeartbeatMonitor(configPath, output, fault);
        using (var stopEvent = new System.Threading.ManualResetEventSlim(false))
        {
            ConsoleCancelEventHandler cancelHandler = (sender, e) => { e.Cancel = true; stopEvent.Set(); };
            Console.CancelKeyPress += cancelHandler;
            try { stopEvent.Wait(); }
            finally
            {
                Console.CancelKeyPress -= cancelHandler;
                s7Help.StopHeartbeatMonitor();
            }
        }
    }

    private static string ToHex(byte[] bytes) { return string.Join(" ", bytes.Select(b => b.ToString("X2"))); }
    private static int ReadInt(string[] args, string name, int fallback) { for (int i = 0; i + 1 < args.Length; i++) if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase)) return int.Parse(args[i + 1]); return fallback; }
    private static string ReadString(string[] args, string name, string fallback) { for (int i = 0; i + 1 < args.Length; i++) if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase)) return args[i + 1]; return fallback; }
}
