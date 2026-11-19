using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using XHS.Model.ZPLLabel;

namespace LabelHelp.Services
{
    public sealed class ZebraRawPrinter : IPrinter
    {
        private const int RawDataType = 1;
        private readonly string printerName;

        public ZebraRawPrinter(string printerName)
        {
            if (string.IsNullOrWhiteSpace(printerName))
                throw new ArgumentException("打印机名称不能为空。", "printerName");

            this.printerName = printerName;
        }

        public string PrinterName
        {
            get { return printerName; }
        }

        public PrintResult Print(string content)
        {
            if (string.IsNullOrEmpty(content))
                return Failure("ZPL 内容不能为空。");

            IntPtr printerHandle = IntPtr.Zero;
            IntPtr jobId = IntPtr.Zero;
            bool documentStarted = false;
            bool pageStarted = false;

            try
            {
                if (!OpenPrinter(printerName, out printerHandle, IntPtr.Zero))
                    return Failure(Win32Error("OpenPrinter"));

                DOC_INFO_1 document = new DOC_INFO_1
                {
                    pDocName = "Zebra ZPL RAW Job",
                    pOutputFile = null,
                    pDataType = "RAW"
                };

                jobId = StartDocPrinter(printerHandle, 1, ref document);
                if (jobId == IntPtr.Zero)
                    return Failure(Win32Error("StartDocPrinter"));
                documentStarted = true;

                if (!StartPagePrinter(printerHandle))
                    return Failure(Win32Error("StartPagePrinter"));
                pageStarted = true;

                byte[] bytes = Encoding.ASCII.GetBytes(content);
                int written;
                if (!WritePrinter(printerHandle, bytes, bytes.Length, out written))
                    return Failure(Win32Error("WritePrinter"));
                if (written != bytes.Length)
                    return Failure("WritePrinter 未发送完整的 ZPL 数据。");

                return new PrintResult { Success = true };
            }
            catch (Exception ex)
            {
                return Failure(ex.Message);
            }
            finally
            {
                if (pageStarted)
                    EndPagePrinter(printerHandle);
                if (documentStarted)
                    EndDocPrinter(printerHandle);
                if (printerHandle != IntPtr.Zero)
                    ClosePrinter(printerHandle);
            }
        }

        private static PrintResult Failure(string message)
        {
            return new PrintResult { Success = false, ErrorMessage = message };
        }

        private static string Win32Error(string operation)
        {
            int error = Marshal.GetLastWin32Error();
            return operation + " 失败: " + new Win32Exception(error).Message + " (" + error + ")";
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DOC_INFO_1
        {
            [MarshalAs(UnmanagedType.LPWStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPWStr)] public string pOutputFile;
            [MarshalAs(UnmanagedType.LPWStr)] public string pDataType;
        }

        [DllImport("winspool.drv", EntryPoint = "OpenPrinterW", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool OpenPrinter(string printerName, out IntPtr printerHandle, IntPtr defaults);

        [DllImport("winspool.drv", EntryPoint = "ClosePrinter", SetLastError = true)]
        private static extern bool ClosePrinter(IntPtr printerHandle);

        [DllImport("winspool.drv", EntryPoint = "StartDocPrinterW", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr StartDocPrinter(IntPtr printerHandle, int level, ref DOC_INFO_1 document);

        [DllImport("winspool.drv", EntryPoint = "EndDocPrinter", SetLastError = true)]
        private static extern bool EndDocPrinter(IntPtr printerHandle);

        [DllImport("winspool.drv", EntryPoint = "StartPagePrinter", SetLastError = true)]
        private static extern bool StartPagePrinter(IntPtr printerHandle);

        [DllImport("winspool.drv", EntryPoint = "EndPagePrinter", SetLastError = true)]
        private static extern bool EndPagePrinter(IntPtr printerHandle);

        [DllImport("winspool.drv", EntryPoint = "WritePrinter", SetLastError = true)]
        private static extern bool WritePrinter(IntPtr printerHandle, byte[] buffer, int bufferLength, out int bytesWritten);
    }
}
