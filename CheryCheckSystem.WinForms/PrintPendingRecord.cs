using XHS.Model.Label;

namespace CheryCheckSystem.PrintClient
{
    public class PrintPendingRecord
    {
        public long? id { get; set; }

        public string supplyBatchNo { get; set; }

        public string partNo { get; set; }

        public string cartonNo { get; set; }

        public string taskId { get; set; }

        public string clientId { get; set; }

        public string machineId { get; set; }

        public string printType { get; set; }

        public string pdfUrl { get; set; }

        public string pdfDownLoadUrl { get; set; }

        public string pdfDownloadPath { get; set; }

        public string localPath { get; set; }

        public int? status { get; set; }

        public int? printCount { get; set; }

        public string printUser { get; set; }

        public string printTime { get; set; }

        public string createUser { get; set; }

        public string createTime { get; set; }

        public string lockTime { get; set; }

        public LabelInfo labelInfo { get; set; }
    }
}
