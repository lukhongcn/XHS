using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
            // 先取出完整绑定记录，再在组装后的结果上分别按工单号和本厂品号过滤。
            // 不能把任一筛选值直接传给扫描记录 SQL，否则会过滤掉同一绑定任务的客户标签记录。
            string rawFilter = "%%";

            List<ScanFlowScanRecordInfo> sourceRecords = dal.GetLabelBindingRecords(FlowCode, rawFilter, startTime, endTime);
            DataTable result = CreateResultTable();
            if (sourceRecords == null || sourceRecords.Count == 0) return result;

            var groups = sourceRecords
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.BindingTaskId))
                .GroupBy(x => x.BindingTaskId, StringComparer.OrdinalIgnoreCase);

            foreach (var group in groups)
            {
                List<ScanFlowScanRecordInfo> ordered = group
                    .OrderBy(x => x.ScanTime ?? DateTime.MinValue)
                    .ThenBy(x => x.RecordId ?? 0)
                    .ToList();
                List<ScanFlowScanRecordInfo> currentRound = new List<ScanFlowScanRecordInfo>();
                foreach (ScanFlowScanRecordInfo record in ordered)
                {
                    if (string.Equals(record.StepCode, "CUSTOMER", StringComparison.OrdinalIgnoreCase)
                        && currentRound.Any(x => string.Equals(x.StepCode, "CUSTOMER", StringComparison.OrdinalIgnoreCase)))
                    {
                        AppendResultRows(result, currentRound);
                        currentRound = new List<ScanFlowScanRecordInfo>();
                    }
                    currentRound.Add(record);
                }
                AppendResultRows(result, currentRound);
            }

            var filtered = result.AsEnumerable()
                .Where(row => Contains(row.Field<string>("WorkOrderNo"), customerFilter))
                .Where(row => Contains(row.Field<string>("JHSPartNo"), productFilter))
                .OrderByDescending(row => row.Field<string>("ScanTimeText"))
                .ToList();

            return filtered.Count == 0 ? result.Clone() : filtered.CopyToDataTable();
        }

        private void AppendResultRows(DataTable result, List<ScanFlowScanRecordInfo> round)
        {
            if (round == null || round.Count == 0) return;
            ScanFlowScanRecordInfo customer = round.FirstOrDefault(x =>
                string.Equals(x.StepCode, "CUSTOMER", StringComparison.OrdinalIgnoreCase));
            if (customer == null) return;

            PartInfo customerPart = barcodeParser.ParseJHSPartnoBarcode(
                customer.ScanContent, customer.CustomerId, customer.LabelType, customer.RuleName);
            string partNo = customerPart == null ? string.Empty : customerPart.JHSMaterialNo;
            string batchNo = customerPart == null ? string.Empty : customerPart.JHSBatchNo;
            decimal customerQty = customerPart == null ? 0M : customerPart.Qty;
            string workOrder = string.Join(", ", round
                .Where(x => string.Equals(x.StepCode, "WORKORDER", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.ScanTime ?? DateTime.MinValue)
                .Select(x => x.ScanContent)
                .Where(x => !string.IsNullOrWhiteSpace(x)));
            List<ScanFlowScanRecordInfo> factoryRecords = round
                .Where(x => string.Equals(x.StepCode, "FACTORY", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.ScanTime ?? DateTime.MinValue)
                .ToList();

            if (factoryRecords.Count == 0)
            {
                ScanFlowScanRecordInfo latest = round.OrderByDescending(x => x.ScanTime ?? DateTime.MinValue)
                    .ThenByDescending(x => x.RecordId ?? 0).FirstOrDefault();
                AddResultRow(result, customer.ScanUser, customer.ScanContent, workOrder,
                    partNo, batchNo, customerQty, latest == null ? customer.ScanTime : latest.ScanTime);
                return;
            }

            foreach (ScanFlowScanRecordInfo factory in factoryRecords)
            {
                List<PartInfo> matches = barcodeParser.ParseFactoryBarcode(
                    factory.ScanContent, factory.CustomerId, factory.LabelType, factory.RuleName);
                PartInfo factoryPart = matches == null || matches.Count == 0 ? null : matches[0];
                AddResultRow(result, customer.ScanUser, customer.ScanContent, workOrder,
                    factoryPart == null ? partNo : factoryPart.JHSMaterialNo,
                    factoryPart == null ? batchNo : factoryPart.JHSBatchNo,
                    factoryPart == null ? customerQty : factoryPart.JHSQty,
                    factory.ScanTime);
            }
        }

        private static void AddResultRow(DataTable result, string scanUser, string customerRaw,
            string workOrder, string partNo, string batchNo, decimal qty, DateTime? scanTime)
        {
            DataRow row = result.NewRow();
            row["WorkOrderNo"] = workOrder ?? string.Empty;
            row["FactoryBarcode"] = customerRaw ?? string.Empty;
            row["JHSPartNo"] = partNo ?? string.Empty;
            row["JHSBatchNo"] = batchNo ?? string.Empty;
            row["LabelQtyText"] = qty <= 0 ? string.Empty : qty.ToString("0.####", CultureInfo.InvariantCulture);
            row["ScanTimeText"] = scanTime.HasValue
                ? scanTime.Value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                : string.Empty;
            row["ScanUser"] = scanUser ?? string.Empty;
            result.Rows.Add(row);
        }

        private string ParseCustomerProduct(DataRow row)
        {
            if (row == null) return string.Empty;
            PartInfo part = barcodeParser.ParseCustomerBarcode(ReadString(row, "ScanContent"), ReadString(row, "CustomerId"), ReadString(row, "LabelType"), ReadString(row, "RuleName"));
            return part == null ? string.Empty : (part.MaterialNo ?? string.Empty);
        }

        private string ParseFactoryProduct(DataRow row)
        {
            List<PartInfo> matches = barcodeParser.ParseFactoryBarcode(
                ReadString(row, "ScanContent"),
                ReadString(row, "CustomerId"),
                ReadString(row, "LabelType"),
                ReadString(row, "RuleName"));
            PartInfo part = matches != null && matches.Count > 0 ? matches[0] : null;
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
            table.Columns.Add("WorkOrderNo", typeof(string));
            table.Columns.Add("FactoryBarcode", typeof(string));
            table.Columns.Add("JHSPartNo", typeof(string));
            table.Columns.Add("JHSBatchNo", typeof(string));
            table.Columns.Add("LabelQtyText", typeof(string));
            table.Columns.Add("ScanTimeText", typeof(string));
            table.Columns.Add("ScanUser", typeof(string));
            return table;
        }

        private static DataTable CreateSourceTable(List<ScanFlowScanRecordInfo> records)
        {
            DataTable table = new DataTable();
            table.Columns.Add("RecordId", typeof(long));
            table.Columns.Add("FlowId", typeof(int));
            table.Columns.Add("BindingTaskId", typeof(string));
            table.Columns.Add("StepId", typeof(int));
            table.Columns.Add("StepCode", typeof(string));
            table.Columns.Add("SeqNo", typeof(int));
            table.Columns.Add("ScanContent", typeof(string));
            table.Columns.Add("RuleId", typeof(int));
            table.Columns.Add("RuleName", typeof(string));
            table.Columns.Add("BarcodeType", typeof(string));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("ScanUser", typeof(string));
            table.Columns.Add("ScanTime", typeof(DateTime));
            table.Columns.Add("StepName", typeof(string));
            table.Columns.Add("CustomerId", typeof(string));
            table.Columns.Add("LabelType", typeof(string));
            if (records == null) return table;

            foreach (ScanFlowScanRecordInfo info in records)
            {
                if (info == null) continue;
                DataRow row = table.NewRow();
                row["RecordId"] = info.RecordId.HasValue ? (object)info.RecordId.Value : DBNull.Value;
                row["FlowId"] = info.FlowId.HasValue ? (object)info.FlowId.Value : DBNull.Value;
                row["BindingTaskId"] = info.BindingTaskId ?? string.Empty;
                row["StepId"] = info.StepId.HasValue ? (object)info.StepId.Value : DBNull.Value;
                row["StepCode"] = info.StepCode ?? string.Empty;
                row["SeqNo"] = info.SeqNo.HasValue ? (object)info.SeqNo.Value : DBNull.Value;
                row["ScanContent"] = info.ScanContent ?? string.Empty;
                row["RuleId"] = info.RuleId.HasValue ? (object)info.RuleId.Value : DBNull.Value;
                row["RuleName"] = info.RuleName ?? string.Empty;
                row["BarcodeType"] = info.BarcodeType ?? string.Empty;
                row["Status"] = info.Status ?? string.Empty;
                row["ScanUser"] = info.ScanUser ?? string.Empty;
                row["ScanTime"] = info.ScanTime.HasValue ? (object)info.ScanTime.Value : DBNull.Value;
                row["StepName"] = info.StepName ?? string.Empty;
                row["CustomerId"] = info.CustomerId ?? string.Empty;
                row["LabelType"] = info.LabelType ?? string.Empty;
                table.Rows.Add(row);
            }
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
