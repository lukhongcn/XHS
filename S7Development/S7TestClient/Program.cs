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
                Console.WriteLine("CONNECT ip={0} status={1}", ip, plc.IsConnected ? "OK" : "FAILED");
                byte[] actual = plc.ReadBytes(DataType.DataBlock, db, offset, length);
                if (!readOnly)
                {
                    plc.WriteBytes(DataType.DataBlock, db, offset, expected);
                    actual = plc.ReadBytes(DataType.DataBlock, db, offset, length);
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

    private static string ToHex(byte[] bytes) { return string.Join(" ", bytes.Select(b => b.ToString("X2"))); }
    private static int ReadInt(string[] args, string name, int fallback) { for (int i = 0; i + 1 < args.Length; i++) if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase)) return int.Parse(args[i + 1]); return fallback; }
    private static string ReadString(string[] args, string name, string fallback) { for (int i = 0; i + 1 < args.Length; i++) if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase)) return args[i + 1]; return fallback; }
}
