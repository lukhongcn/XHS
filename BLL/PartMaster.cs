using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ModuleWorkFlow.business;
using NPOI.SS.UserModel;
using XHS.IDAL;
using XHS.Model;

namespace XHS.BLL
{
    /// <summary>
    /// 零件主数据业务层。
    /// </summary>
    public class PartMaster
    {
        private readonly IPartMaster dal;

        public PartMaster()
        {
            dal = XHS.DALFactory.PartMaster.Create();
        }

        public List<PartMasterInfo> GetPartMasters()
        {
            return dal.GetPartMasters();
        }

        public List<PartMasterInfo> GetPartMasters(string jhsPartNo, string customerMaterialNo)
        {
            return dal.GetPartMasters(jhsPartNo, customerMaterialNo);
        }

        public PartMasterInfo GetPartMaster(int partMasterId)
        {
            return dal.GetPartMaster(partMasterId);
        }

        public PartInfo GetPartMasterByJHSPartNo(string jhsPartNo)
        {
            return dal.GetPartMasterByJHSPartNo(jhsPartNo);
        }

        public PartInfo GetPartMasterByCustomerMaterialNo(string customerMaterialNo)
        {
            return dal.GetPartMasterByCustomerMaterialNo(customerMaterialNo);
        }

        /// <summary>
        /// 使用标签编码规则校验光束零件主数据中的拆分编码字段。
        /// </summary>
        public string ValidateGSPartMasterCodes(
            List<PartMasterInfo> infos,
            string customerId,
            string jhsPartNoLabelType,
            string customerMaterialNoLabelType,
            string jhsPartNoRuleName = null,
            string customerMaterialNoRuleName = null)
        {
            if (infos == null || infos.Count == 0)
            {
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(customerId))
            {
                return "未配置光束零件主数据编码规则的客户编号。";
            }

            if (string.IsNullOrWhiteSpace(jhsPartNoLabelType)
                || string.IsNullOrWhiteSpace(customerMaterialNoLabelType))
            {
                return "未配置光束零件主数据编码规则类型。";
            }

            LabelCodeRule labelCodeRule = new LabelCodeRule();
            List<LabelCodeRuleInfo> jhsRules = labelCodeRule
                .GetLabelCodeRulesByCustomerIdAndLabelType(customerId, jhsPartNoLabelType);
            List<LabelCodeRuleInfo> customerRules = labelCodeRule
                .GetLabelCodeRulesByCustomerIdAndLabelType(customerId, customerMaterialNoLabelType);

            if (!HasEnabledRule(jhsRules))
            {
                return "未配置本厂零件编号编码规则：" + jhsPartNoLabelType + "。";
            }

            if (!HasEnabledRule(customerRules))
            {
                return "未配置客户零件编号编码规则：" + customerMaterialNoLabelType + "。";
            }

            FactoryBarcodeParser barcodeParser = new FactoryBarcodeParser();
            List<string> messages = new List<string>();
            foreach (PartMasterInfo info in infos)
            {
                if (info == null)
                {
                    continue;
                }

                string rowText = info.SourceRowNumber.HasValue
                    ? "第 " + info.SourceRowNumber.Value + " 行"
                    : "零件主数据";
                if (!barcodeParser.IsConfiguredFieldMatch(
                    info.JHSPartNo,
                    jhsRules,
                    "JHSMaterialNo",
                    jhsPartNoRuleName))
                {
                    messages.Add(rowText + "【JHS 品号】不符合编码规则。值：" + (info.JHSPartNo ?? string.Empty));
                }

                if (!barcodeParser.IsConfiguredFieldMatch(
                    info.CustomerMaterialNo,
                    customerRules,
                    "CustomerMaterialNo",
                    customerMaterialNoRuleName))
                {
                    messages.Add(rowText + "【物料编号】不符合编码规则。值：" + (info.CustomerMaterialNo ?? string.Empty));
                }
            }

            return string.Join("<br />", messages.ToArray());
        }

        public string InsertPartMasters(List<PartMasterInfo> infos)
        {
            string validateMessage = Validate(infos);
            if (!string.IsNullOrWhiteSpace(validateMessage))
            {
                return validateMessage;
            }

            ParamterInfo paramterInfo = dal.InsertPartMasters(infos);
            IList source = new ArrayList();
            source.Add(paramterInfo);
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string UpdatePartMasters(List<PartMasterInfo> infos)
        {
            string validateMessage = Validate(infos);
            if (!string.IsNullOrWhiteSpace(validateMessage))
            {
                return validateMessage;
            }

            ParamterInfo paramterInfo = dal.UpdatePartMasters(infos);
            IList source = new ArrayList();
            source.Add(paramterInfo);
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public string DeletePartMasters(List<PartMasterInfo> infos)
        {
            ParamterInfo paramterInfo = dal.DeletePartMasters(infos);
            IList source = new ArrayList();
            source.Add(paramterInfo);
            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public List<PartMasterInfo> ReadGSPartMasterExcel(string fileName, string customerAbbr, out List<string> messageList)
        {
            messageList = new List<string>();
            List<PartMasterInfo> result = new List<PartMasterInfo>();
            if (!File.Exists(fileName))
            {
                messageList.Add("文件不存在：" + fileName);
                return result;
            }

            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                IWorkbook workbook = WorkbookFactory.Create(stream);
                if (workbook.NumberOfSheets < 1)
                {
                    messageList.Add("Excel 文件至少需要包含一个工作表。");
                    return result;
                }

                string[] sheet1Headers = { "\u5e8f\u53f7", "\u7269\u6599\u7f16\u53f7", "\u7269\u6599\u540d\u79f0", "JHS\u54c1\u53f7", "\u7c7b\u578b", "\u6807\u7b7e\u4fe1\u606f", "\u94a2\u5370\u6709\u65e0\u5173\u8054" };
                string[] formatHeaders = { "\u7269\u6599\u7f16\u53f7", "JHS\u54c1\u53f7", "\u6807\u7b7e\u683c\u5f0f\uff08\u5355\u88c5/\u6df7\u88c5\uff09" };
                Dictionary<string, int> columns1;
                int header1 = FindGSHeaderRow(workbook.GetSheetAt(0), sheet1Headers, messageList, out columns1);
                if (header1 < 0)
                {
                    return result;
                }

                Dictionary<string, PartMasterInfo> parts = new Dictionary<string, PartMasterInfo>(StringComparer.OrdinalIgnoreCase);
                ISheet sheet1 = workbook.GetSheetAt(0);
                for (int rowIndex = header1 + 1; rowIndex <= sheet1.LastRowNum; rowIndex++)
                {
                    IRow row = sheet1.GetRow(rowIndex);
                    if (row == null)
                    {
                        break;
                    }

                    string sequence = GetGSCellText(row.GetCell(columns1["\u5e8f\u53f7"]));
                    string material = GetGSCellText(row.GetCell(columns1["\u7269\u6599\u7f16\u53f7"]));
                    string materialName = GetGSCellText(row.GetCell(columns1["\u7269\u6599\u540d\u79f0"]));
                    string jhs = GetGSCellText(row.GetCell(columns1["JHS\u54c1\u53f7"]));
                    string processType = GetGSCellText(row.GetCell(columns1["\u7c7b\u578b"]));
                    string labelInfo = GetGSCellText(row.GetCell(columns1["\u6807\u7b7e\u4fe1\u606f"]));
                    string hasSteelStamp = GetGSCellText(row.GetCell(columns1["\u94a2\u5370\u6709\u65e0\u5173\u8054"]));
                    if (string.IsNullOrWhiteSpace(sequence) && string.IsNullOrWhiteSpace(material) && string.IsNullOrWhiteSpace(materialName) && string.IsNullOrWhiteSpace(jhs) && string.IsNullOrWhiteSpace(processType) && string.IsNullOrWhiteSpace(labelInfo) && string.IsNullOrWhiteSpace(hasSteelStamp))
                    {
                        break;
                    }

                    if (material.Length == 0 || jhs.Length == 0)
                    {
                        messageList.Add(string.Format("第 {0} 行物料编号和 JHS 品号必须同时填写。", rowIndex + 1));
                        continue;
                    }

                    string key = BuildGSKey(material, jhs);
                    if (parts.ContainsKey(key))
                    {
                        messageList.Add(string.Format("第 {0} 行物料编号和 JHS 品号重复。", rowIndex + 1));
                        continue;
                    }

                    int sortOrder;
                    if (!int.TryParse(sequence, NumberStyles.Integer, CultureInfo.InvariantCulture, out sortOrder))
                    {
                        messageList.Add(string.Format("第 {0} 行序号格式不正确。", rowIndex + 1));
                        continue;
                    }

                    parts.Add(key, new PartMasterInfo
                    {
                        SourceRowNumber = rowIndex + 1,
                        CustomerMaterialNo = material,
                        JHSPartNo = jhs,
                        CustomerAbbr = customerAbbr,
                        MaterialName = materialName,
                        ProcessType = processType,
                        LabelInfo = labelInfo,
                        HasSteelStamp = hasSteelStamp,
                        SortOrder = sortOrder
                    });
                }

                Dictionary<string, string> formats = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                bool hasFormatSheet = workbook.NumberOfSheets > 1;
                if (hasFormatSheet)
                {
                    Dictionary<string, int> columns2;
                    int header2 = FindGSHeaderRow(workbook.GetSheetAt(1), formatHeaders, messageList, out columns2);
                    if (header2 < 0)
                    {
                        return result;
                    }

                    ISheet sheet2 = workbook.GetSheetAt(1);
                    string formatHeader = "\u6807\u7b7e\u683c\u5f0f\uff08\u5355\u88c5/\u6df7\u88c5\uff09";
                    for (int rowIndex = header2 + 1; rowIndex <= sheet2.LastRowNum; rowIndex++)
                    {
                        IRow row = sheet2.GetRow(rowIndex);
                        if (row == null)
                        {
                            break;
                        }

                        string material = GetGSCellText(row.GetCell(columns2["\u7269\u6599\u7f16\u53f7"]));
                        string jhs = GetGSCellText(row.GetCell(columns2["JHS\u54c1\u53f7"]));
                        string format = GetGSCellText(row.GetCell(columns2[formatHeader]));
                        if (string.IsNullOrWhiteSpace(material) && string.IsNullOrWhiteSpace(jhs) && string.IsNullOrWhiteSpace(format))
                        {
                            break;
                        }

                        if (material.Length == 0 || jhs.Length == 0 || format.Length == 0)
                        {
                            messageList.Add(string.Format("标签格式工作表第 {0} 行数据不完整。", rowIndex + 1));
                            continue;
                        }

                        string key = BuildGSKey(material, jhs);
                        if (formats.ContainsKey(key))
                        {
                            messageList.Add(string.Format("标签格式工作表第 {0} 行物料编号和 JHS 品号重复。", rowIndex + 1));
                            continue;
                        }

                        formats.Add(key, format);
                    }
                }

                foreach (KeyValuePair<string, PartMasterInfo> entry in parts)
                {
                    if (hasFormatSheet)
                    {
                        string format;
                        if (!formats.TryGetValue(entry.Key, out format))
                        {
                            messageList.Add("标签格式工作表中未找到对应的标签格式：" + entry.Key.Replace("\u001f", " / "));
                            continue;
                        }

                        entry.Value.LabelFormat = format;
                    }

                    result.Add(entry.Value);
                }
            }
            return messageList.Count == 0 ? result : new List<PartMasterInfo>();
        }

        private static int FindGSHeaderRow(ISheet sheet, string[] headers, List<string> messages, out Dictionary<string, int> columns)
        {
            columns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                IRow row = sheet.GetRow(rowIndex); if (row == null) continue;
                Dictionary<string, int> current = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                for (int col = 0; col < row.LastCellNum; col++) { string value = GetGSCellText(row.GetCell(col)); if (!current.ContainsKey(value)) current[value] = col; }
                if (!headers.All(current.ContainsKey)) continue;
                columns = current; return rowIndex;
            }
            messages.Add("Missing required worksheet headers: " + string.Join(", ", headers)); return -1;
        }

        private static string GetGSCellText(ICell cell)
        {
            if (cell == null) return string.Empty;
            if (cell.CellType == CellType.Numeric) return DateUtil.IsCellDateFormatted(cell) ? cell.DateCellValue.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) : cell.NumericCellValue.ToString(CultureInfo.InvariantCulture);
            return (cell.ToString() ?? string.Empty).Trim();
        }

        private static string BuildGSKey(string material, string jhs)
        {
            return (material ?? string.Empty).Trim() + "\u001f" + (jhs ?? string.Empty).Trim();
        }

        private static string Validate(List<PartMasterInfo> infos)
        {
            if (infos == null)
            {
                return string.Empty;
            }

            foreach (PartMasterInfo info in infos)
            {
                if (info == null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(info.JHSPartNo))
                {
                    return "JHS 品号不能为空。";
                }
            }

            return string.Empty;
        }

        private static bool HasEnabledRule(List<LabelCodeRuleInfo> rules)
        {
            return rules != null && rules.Any(rule => rule != null
                && (!rule.Enabled.HasValue || rule.Enabled.Value)
                && !string.IsNullOrWhiteSpace(rule.MatchRegex));
        }
    }
}
