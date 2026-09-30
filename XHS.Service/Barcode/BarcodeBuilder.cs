using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using XHS.Model.BarcodeCreateRule;
using XHS.service.Formatter;

namespace XHS.service.Barcode
{
    public sealed class BarcodeBuilder
    {
        private readonly Func<DateTime, int> getPrintCount;
        private readonly BarcodeDesignerJsonRuleLoader ruleLoader = new BarcodeDesignerJsonRuleLoader();
        private readonly FormatterFactory formatterFactory = new FormatterFactory();

        public BarcodeBuilder(Func<DateTime, int> getPrintCount)
        {
            if (getPrintCount == null)
            {
                throw new ArgumentNullException(nameof(getPrintCount));
            }

            this.getPrintCount = getPrintCount;
        }

        public DataTable Build(
            string jsonFileName,
            IDictionary<string, object> fieldValues,
            int quantity)
        {
            if (string.IsNullOrWhiteSpace(jsonFileName))
            {
                throw new ArgumentException("条码规则 JSON 文件名不能为空。", nameof(jsonFileName));
            }

            if (fieldValues == null)
            {
                throw new ArgumentNullException(nameof(fieldValues));
            }

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), quantity, "产生数量必须大于 0。");
            }

            string path = Path.IsPathRooted(jsonFileName)
                ? jsonFileName
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, jsonFileName);
            BarcodeRule rule = ruleLoader.LoadBarcodeRule(path);
            List<BarcodeFieldRule> barcodeFields = rule.Fields
                .Where(field => field.IncludeInBarcode != false)
                .ToList();

           

            DataTable result = CreateResultTable(barcodeFields);
            var formatters = barcodeFields
                .Where(field => !string.IsNullOrWhiteSpace(field.Formatter))
                .ToDictionary(
                    field => field.Name,
                    field => formatterFactory.Get(field, getPrintCount),
                    StringComparer.OrdinalIgnoreCase);

            for (int index = 0; index < quantity; index++)
            {
                var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (BarcodeFieldRule field in barcodeFields)
                {
                    object rawValue = ResolveRawFieldValue(
                        field,
                        fieldValues);

                    if (!string.IsNullOrWhiteSpace(field.Formatter))
                    {
                        ICodeFormatter code = formatters[field.Name];
                        values[field.Name] = code.Encode(rawValue);
                    }
                    else
                    {
                        values[field.Name] = Convert.ToString(rawValue).Trim();
                    }
                }

                DataRow row = result.NewRow();
                foreach (BarcodeFieldRule field in barcodeFields)
                {
                    row[field.Name] = values[field.Name];
                }

                row["Barcode"] = string.Concat(barcodeFields.Select(field => values[field.Name]));
                result.Rows.Add(row);
            }

            return result;
        }

        private object ResolveRawFieldValue(
            BarcodeFieldRule field,
            IDictionary<string, object> fieldValues)
        {
            if (string.Equals(field.ValueSource, "Today", StringComparison.OrdinalIgnoreCase)
                || string.Equals(field.Source, "Date", StringComparison.OrdinalIgnoreCase))
            {
                return DateTime.Today;
            }

            object value;
            if (!fieldValues.TryGetValue(field.Name, out value) || value == null)
            {
                throw new InvalidOperationException("字段没有输入值：" + field.Name);
            }

            if (value is string && string.Equals(field.Source, "Date", StringComparison.OrdinalIgnoreCase))
            {
                return Convert.ToDateTime(value);
            }

            return value;
        }

        private static DataTable CreateResultTable(IList<BarcodeFieldRule> fields)
        {
            var result = new DataTable("BarcodeResult");
            foreach (BarcodeFieldRule field in fields)
            {
                result.Columns.Add(field.Name, typeof(string));
            }

            result.Columns.Add("Barcode", typeof(string));
            return result;
        }

    }
}
