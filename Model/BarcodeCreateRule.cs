using System.Collections.Generic;

namespace XHS.Model.BarcodeCreateRule
{
    public sealed class BarcodeRule
    {
        public string RuleName { get; set; }
        public string Description { get; set; }
        public string BarcodeDescription { get; set; }
        public int BarcodeLength { get; set; }
        public UiRule Ui { get; set; }
        public List<BarcodeFieldRule> Fields { get; set; }
    }

    public sealed class BarcodeFieldRule
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Length { get; set; }
        public string Source { get; set; }
        public string ValueSource { get; set; }
        public string DataType { get; set; }
        public bool Required { get; set; }
        public string Formatter { get; set; }
        public string InitialValue { get; set; }
        public bool RememberLastValue { get; set; }
        public string Format { get; set; }
        public bool? IncludeInBarcode { get; set; }
        public string StartGenerator { get; set; }
        public UiFieldRule Ui { get; set; }
        public ValidationRule Validation { get; set; }
    }

    public sealed class UiRule
    {
        public string FormTitle { get; set; }
        public string Layout { get; set; }
        public string GenerateButtonText { get; set; }
    }

    public sealed class UiFieldRule
    {
        public string Control { get; set; }
        public int Order { get; set; }
        public int Width { get; set; }
        public string Placeholder { get; set; }
        public string DefaultValue { get; set; }
        public string Format { get; set; }
        public bool ReadOnly { get; set; }
    }

    public sealed class ValidationRule
    {
        public int MaxLength { get; set; }
        public string Regex { get; set; }
    }
}
