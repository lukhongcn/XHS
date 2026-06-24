using System;
using System.Drawing.Printing;

namespace CheryCheckSystem.PrintClient
{
    public static class PrintClientPrinterResolver
    {
        public static string Resolve(string configuredPrinterName)
        {
            string safeConfiguredPrinterName = SafeValue(configuredPrinterName);
            if (!string.IsNullOrWhiteSpace(safeConfiguredPrinterName))
            {
                return safeConfiguredPrinterName;
            }

            string labelPrinterName = FindLabelPrinterName();
            if (!string.IsNullOrWhiteSpace(labelPrinterName))
            {
                return labelPrinterName;
            }

            return GetDefaultPrinterName();
        }

        private static string FindLabelPrinterName()
        {
            try
            {
                foreach (string installedPrinter in PrinterSettings.InstalledPrinters)
                {
                    string safePrinterName = SafeValue(installedPrinter);
                    if (string.Equals(safePrinterName, "DL-740C(NEW)", StringComparison.OrdinalIgnoreCase) ||
                        safePrinterName.IndexOf("DL-740C", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return safePrinterName;
                    }
                }
            }
            catch
            {
                return string.Empty;
            }

            return string.Empty;
        }

        private static string GetDefaultPrinterName()
        {
            try
            {
                PrinterSettings printerSettings = new PrinterSettings();
                return SafeValue(printerSettings.PrinterName);
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string SafeValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }
    }
}
