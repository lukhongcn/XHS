using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace CheryCheckSystem.PrintClient
{
    public class PdfShellPrintService
    {
        public void PrintPdf(string pdfPath, string printerName, int pauseMilliseconds, string pdfReaderPath)
        {
            if (string.IsNullOrWhiteSpace(pdfPath))
            {
                throw new ArgumentException("pdfPath 不能为空。", "pdfPath");
            }

            if (!File.Exists(pdfPath))
            {
                throw new FileNotFoundException("PDF 文件不存在。", pdfPath);
            }

            string resolvedReaderPath = ResolvePdfReaderPath(pdfReaderPath);
            if (!string.IsNullOrWhiteSpace(resolvedReaderPath))
            {
                PrintByReader(resolvedReaderPath, pdfPath, printerName);
            }
            else
            {
                PrintByShell(pdfPath, printerName);
            }

            if (pauseMilliseconds > 0)
            {
                Thread.Sleep(pauseMilliseconds);
            }
        }

        private static void PrintByReader(string pdfReaderPath, string pdfPath, string printerName)
        {
            string readerFileName = Path.GetFileName(pdfReaderPath) ?? string.Empty;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = pdfReaderPath;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            if (readerFileName.Equals("SumatraPDF.exe", StringComparison.OrdinalIgnoreCase))
            {
                startInfo.Arguments = string.IsNullOrWhiteSpace(printerName)
                    ? "-print-to-default -silent -exit-when-done \"" + pdfPath + "\""
                    : "-print-to \"" + printerName.Replace("\"", string.Empty) + "\" -silent -exit-when-done \"" + pdfPath + "\"";
            }
            else
            {
                startInfo.Arguments = string.IsNullOrWhiteSpace(printerName)
                    ? "/p /h \"" + pdfPath + "\""
                    : "/t \"" + pdfPath + "\" \"" + printerName.Replace("\"", string.Empty) + "\"";
            }

            using (Process process = Process.Start(startInfo))
            {
                if (process != null)
                {
                    process.WaitForExit(15000);
                }
            }
        }

        private static void PrintByShell(string pdfPath, string printerName)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = pdfPath;
            startInfo.UseShellExecute = true;

            if (string.IsNullOrWhiteSpace(printerName))
            {
                startInfo.Verb = "print";
            }
            else
            {
                startInfo.Verb = "printto";
                startInfo.Arguments = "\"" + printerName.Replace("\"", string.Empty) + "\"";
            }

            using (Process process = Process.Start(startInfo))
            {
                if (process != null)
                {
                    process.WaitForExit(15000);
                }
            }
        }

        private static string ResolvePdfReaderPath(string configuredPath)
        {
            string safeConfiguredPath = SafeValue(configuredPath);
            if (!string.IsNullOrWhiteSpace(safeConfiguredPath) && File.Exists(safeConfiguredPath))
            {
                return safeConfiguredPath;
            }

            string[] candidates = new[]
            {
                @"C:\Program Files\SumatraPDF\SumatraPDF.exe",
                @"C:\Program Files (x86)\SumatraPDF\SumatraPDF.exe",
                @"C:\Program Files\Adobe\Acrobat Reader DC\Reader\AcroRd32.exe",
                @"C:\Program Files (x86)\Adobe\Acrobat Reader DC\Reader\AcroRd32.exe",
                @"C:\Program Files\Adobe\Acrobat DC\Acrobat\Acrobat.exe",
                @"C:\Program Files (x86)\Adobe\Acrobat DC\Acrobat\Acrobat.exe"
            };

            foreach (string candidate in candidates)
            {
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            return string.Empty;
        }

        private static string SafeValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }
    }
}
