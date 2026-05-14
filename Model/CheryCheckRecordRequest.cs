using System.Collections.Generic;

namespace Model
{
    public class CheryCheckRecordRequest
    {
        public string supplNo { get; set; }
        public string baseNo { get; set; }
        public string deliveryType { get; set; }
        public string deliveryNo { get; set; }
        public string sxCardSeq { get; set; }
        public string materialNo { get; set; }
        public string materialName { get; set; }
        public string packingCount { get; set; }
        public int packageType { get; set; }
        public string packageBarCode { get; set; }
        public string packageCode { get; set; }
        public string packageName { get; set; }
        public string packingDate { get; set; }
        public List<PackingDetailInfo> packingDetails { get; set; }
        public string checkTime { get; set; }
        public string checkUserName { get; set; }
    }
}
