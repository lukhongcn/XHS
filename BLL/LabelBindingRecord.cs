using System;
using System.Data;
using System.Linq;
using XHS.BLL;
using XHS.Model;

namespace BLL
{
    /// <summary>
    /// 标签绑定历史记录查询及条码解析。
    /// </summary>
    public class LabelBindingRecord
    {
        private const string FlowCode = "LABEL_BINDING";
        private readonly XHS.IDAL.IScanFlowScanRecord dal;
        private readonly FactoryBarcodeParser barcodeParser = new FactoryBarcodeParser();

        public LabelBindingRecord()
        {
            dal = XHS.DALFactory.ScanFlowScanRecord.Create();
        }

        public DataTable GetRecords(string customerProductNo, string productNo, DateTime startTime, DateTime endTime)
        {
            string customerFilter = (customerProductNo ?? string.Empty).Trim();
            string productFilter = (productNo ?? string.Empty).Trim();
            string rawFilter = string.IsNullOrWhiteSpace(customerFilter) && string.IsNullOrWhiteSpace(productFilter)
                ? "%%"
                : "%" + (string.IsNullOrWhiteSpace(customerFilter) ? productFilter : customerFilter) + "%";

            DataTable source = dal.GetLabelBindingRecords(FlowCode, rawFilter, startTime, endTime);
            DataTable result = CreateResultTable();
            SetStepNames(result, source);
            if (source == null || source.Rows.Count == 0) return result;

            var groups = source.AsEnumerable()
                .Where(row => !string.IsNullOrWhiteSpace(ReadString(row, "BindingTaskId")))
                .GroupBy(row => ReadString(row, "BindingTaskId"), StringComparer.OrdinalIgnoreCase);

            foreach (var group in groups)
            {
                DataRow customer = group.FirstOrDefault(row => string.Equals(ReadString(row, "StepCode"), "CUSTOMER", StringComparison.OrdinalIgnoreCase));
                DataRow output = result.NewRow();
                output["BindingTaskId"] = ReadString(group.FirstOrDefault(), "BindingTaskId");
                output["CustomerScanTime"] = FormatDateTime(ReadDateTime(customer, "ScanTime"));
                output["ScanUser"] = ReadString(customer, "ScanUser");
                output["CustomerLabel"] = ReadString(customer, "ScanContent");
                output["FactoryLabel"] = JoinValues(group, "FACTORY", "ScanContent");
                output["WorkOrder"] = JoinValues(group, "WORKORDER", "ScanContent");
                output["CustomerProductNo"] = ParseCustomerProduct(customer);
                output["ProductNo"] = string.Join("；", group
                    .Where(row => string.Equals(ReadString(row, "StepCode"), "FACTORY", StringComparison.OrdinalIgnoreCase))
                    .Select(ParseFactoryProduct)
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .Distinct(StringComparer.OrdinalIgnoreCase));
                output["ScanStatus"] = string.Join("；", group
                    .Select(row => ReadString(row, "Status"))
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .Distinct(StringComparer.OrdinalIgnoreCase));
                result.Rows.Add(output);
            }

            var filtered = result.AsEnumerable()
                .Where(row => Contains(row.Field<string>("CustomerProductNo"), customerFilter))
                .Where(row => Contains(row.Field<string>("ProductNo"), productFilter))
                .OrderByDescending(row => row.Field<string>("CustomerScanTime"))
                .ToList();

            DataTable filteredTable = filtered.Count == 0 ? result.Clone() : filtered.CopyToDataTable();
            SetStepNames(filteredTable, result);
            return filteredTable;
        }

        private string ParseCustomerProduct(DataRow row)
        {
            if (row == null) return string.Empty;
            PartInfo part = barcodeParser.ParseCustomerBarcode(ReadString(row, "ScanContent"), ReadString(row, "CustomerId"), ReadString(row, "LabelType"), ReadString(row, "RuleName"));
            return part == null ? string.Empty : (part.MaterialNo ?? string.Empty);
        }

        private string ParseFactoryProduct(DataRow row)
        {
            PartInfo part = barcodeParser.ParseFactoryBarcode(ReadString(row, "ScanContent"), ReadString(row, "CustomerId"), ReadString(row, "LabelType"), ReadString(row, "RuleName"));
            return part == null ? string.Empty : (part.JHSMaterialNo ?? string.Empty);
        }

        private static string JoinValues(IGrouping<string, DataRow> group, string stepCode, string column)
        {
            return string.Join("\r\n", group
                .Where(row => string.Equals(ReadString(row, "StepCode"), stepCode, StringComparison.OrdinalIgnoreCase))
                .Select(row => ReadString(row, column))
                .Where(value => !string.IsNullOrWhiteSpace(value)));
        }

        private static DataTable CreateResultTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("BindingTaskId", typeof(string));
            table.Columns.Add("CustomerScanTime", typeof(string));
            table.Columns.Add("ScanUser", typeof(string));
            table.Columns.Add("CustomerLabel", typeof(string));
            table.Columns.Add("FactoryLabel", typeof(string));
            table.Columns.Add("WorkOrder", typeof(string));
            table.Columns.Add("CustomerProductNo", typeof(string));
            table.Columns.Add("ProductNo", typeof(string));
            table.Columns.Add("ScanStatus", typeof(string));
            return table;
        }

        private static void SetStepNames(DataTable target, DataTable source)
        {
            if (target == null) return;
            target.ExtendedProperties["CustomerStepName"] = FindStepName(source, "CUSTOMER", "客户标签");
            target.ExtendedProperties["FactoryStepName"] = FindStepName(source, "FACTORY", "本厂标签");
            target.ExtendedProperties["WorkOrderStepName"] = FindStepName(source, "WORKORDER", "上方工单码");
        }

        private static string FindStepName(DataTable source, string stepCode, string defaultName)
        {
            if (source == null) return defaultName;
            DataRow row = source.AsEnumerable().FirstOrDefault(item => string.Equals(ReadString(item, "StepCode"), stepCode, StringComparison.OrdinalIgnoreCase));
            string stepName = ReadString(row, "StepName");
            return string.IsNullOrWhiteSpace(stepName) ? defaultName : stepName.Trim();
        }

        private static string ReadString(DataRow row, string column)
        {
            return row == null || row.IsNull(column) ? string.Empty : Convert.ToString(row[column]);
        }

        private static DateTime? ReadDateTime(DataRow row, string column)
        {
            return row == null || row.IsNull(column) ? (DateTime?)null : Convert.ToDateTime(row[column]);
        }

        private static string FormatDateTime(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
        }

        private static bool Contains(string value, string filter)
        {
            return string.IsNullOrWhiteSpace(filter) || (value ?? string.Empty).IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
