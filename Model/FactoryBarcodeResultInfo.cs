using System.Collections.Generic;

namespace XHS.Model
{
    public class FactoryBarcodeResultInfo
    {
        public bool Success { get; set; }
        public FactoryBarcodeType BarcodeType { get; set; }
        public string RuleCode { get; set; }
        public IDictionary<string, string> Fields { get; set; }
        public string ErrorMessage { get; set; }
    }
}
