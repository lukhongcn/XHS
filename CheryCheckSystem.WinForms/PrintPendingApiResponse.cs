using System.Collections.Generic;

namespace CheryCheckSystem.PrintClient
{
    public class PrintPendingApiResponse
    {
        public bool success { get; set; }

        public string message { get; set; }

        public List<PrintPendingRecord> data { get; set; }
    }
}
