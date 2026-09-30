using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using ZebraPrintTest.Models;
using ZebraPrintTest.Printer;
using ZebraPrintTest.Tests;

namespace ZebraPrintTest
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            string outputFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.zpl");
            if (args.Length == 0 || string.Equals(args[0], "--test", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    PrinterTests.RunAll(outputFile);
                    return 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Test Failed: " + ex.Message);
                    return 1;
                }
            }

            if (string.Equals(args[0], "--print", StringComparison.OrdinalIgnoreCase))
            {
                string printerName = args.Length > 1 ? args[1] : GetDefaultPrinterName();
                string zpl = File.ReadAllText(outputFile);
                IPrinter printer = new ZebraRawPrinter(printerName);
                PrintResult result = printer.Print(zpl);
                Console.WriteLine(result.Success ? "Print Success" : "Print Failed: " + result.ErrorMessage);
                return result.Success ? 0 : 1;
            }

            Console.WriteLine("用法: ZebraPrintTest.exe [--test | --print [printerName]]");
            return 2;
        }

        private static string GetDefaultPrinterName()
        {
            var printerName = new StringBuilder(512);
            uint capacity = (uint)printerName.Capacity;

            if (!GetDefaultPrinter(printerName, ref capacity))
            {
                int error = Marshal.GetLastWin32Error();
                throw new InvalidOperationException(
                    "读取 Windows 默认打印机失败: " +
                    new Win32Exception(error).Message + " (" + error + ")");
            }

            string name = printerName.ToString();
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Windows 当前没有设置默认打印机。");

            Console.WriteLine("使用默认打印机: " + name);
            return name;
        }

        [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool GetDefaultPrinter(
            StringBuilder printerName,
            ref uint bufferSize);
    }
}
