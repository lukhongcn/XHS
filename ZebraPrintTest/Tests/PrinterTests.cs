using System;
using System.IO;
using XHS.Model.ZPLLabel;
using LabelHelp.Services;

namespace ZebraPrintTest.Tests
{
    internal static class PrinterTests
    {
        public static void RunAll(string outputFile)
        {
            AssertOutputFile(outputFile);
            AssertFakePrinterReceivesZpl();
            AssertPrinterNameIsConfigurable();
            Console.WriteLine("Test Success");
        }

        private static void AssertOutputFile(string outputFile)
        {
            string zpl = File.ReadAllText(outputFile);
            Assert(zpl.StartsWith("^XA"), "output.zpl 必须以 ^XA 开始。");
            Assert(zpl.TrimEnd().EndsWith("^XZ"), "output.zpl 必须以 ^XZ 结束。");
            Assert(zpl.Contains("^FDQA,"), "output.zpl 必须包含二维码数据。");
        }

        private static void AssertFakePrinterReceivesZpl()
        {
            FakePrinter printer = new FakePrinter();
            PrintResult result = printer.Print("^XA\n^FDTEST^FS\n^XZ");
            Assert(result.Success, "假打印机应返回成功。");
            Assert(printer.LastContent.Contains("^FDTEST^FS"), "假打印机未收到完整 ZPL。");
        }

        private static void AssertPrinterNameIsConfigurable()
        {
            ZebraRawPrinter printer = new ZebraRawPrinter("Test Printer");
            Assert(printer.PrinterName == "Test Printer", "打印机名称必须由构造函数配置。");
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }

        private sealed class FakePrinter : IPrinter
        {
            public string LastContent { get; private set; }

            public PrintResult Print(string content)
            {
                LastContent = content;
                return new PrintResult { Success = true };
            }
        }
    }
}
