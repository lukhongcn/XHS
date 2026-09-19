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
            if (!File.Exists(fileName)) { messageList.Add("File not found."); return result; }
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                IWorkbook workbook = WorkbookFactory.Create(stream);
                if (workbook.NumberOfSheets < 2) { messageList.Add("Excel must contain two worksheets."); return result; }
                string[] sheet1Headers = { "\u5e8f\u53f7", "\u7269\u6599\u7f16\u53f7", "\u7269\u6599\u540d\u79f0", "JHS\u54c1\u53f7", "\u7c7b\u578b", "\u6807\u7b7e\u4fe1\u606f", "\u94a2\u5370\u6709\u65e0\u5173\u8054" };
                string[] formatHeaders = { "\u7269\u6599\u7f16\u53f7", "JHS\u54c1\u53f7", "\u6807\u7b7e\u683c\u5f0f\uff08\u5355\u88c5/\u6df7\u88c5\uff09" };
                Dictionary<string, int> columns1;
                Dictionary<string, int> columns2;
                int header1 = FindGSHeaderRow(workbook.GetSheetAt(0), sheet1Headers, messageList, out columns1);
                int header2 = FindGSHeaderRow(workbook.GetSheetAt(1), formatHeaders, messageList, out columns2);
                if (header1 < 0 || header2 < 0) return result;
                Dictionary<string, PartMasterInfo> parts = new Dictionary<string, PartMasterInfo>(StringComparer.OrdinalIgnoreCase);
                ISheet sheet1 = workbook.GetSheetAt(0);
                for (int rowIndex = header1 + 1; rowIndex <= sheet1.LastRowNum; rowIndex++)
                {
                    IRow row = sheet1.GetRow(rowIndex); if (row == null) continue;
                    string material = GetGSCellText(row.GetCell(columns1["\u7269\u6599\u7f16\u53f7"]));
                    string jhs = GetGSCellText(row.GetCell(columns1["JHS\u54c1\u53f7"]));
                    if (material.Length == 0 && jhs.Length == 0) continue;
                    if (material.Length == 0 || jhs.Length == 0) { messageList.Add("Incomplete material row " + (rowIndex + 1)); continue; }
                    string key = BuildGSKey(material, jhs);
                    if (parts.ContainsKey(key)) { messageList.Add("Duplicate material key at row " + (rowIndex + 1)); continue; }
                    int sortOrder;
                    if (!int.TryParse(GetGSCellText(row.GetCell(columns1["\u5e8f\u53f7"])), NumberStyles.Integer, CultureInfo.InvariantCulture, out sortOrder)) { messageList.Add("Invalid sequence at row " + (rowIndex + 1)); continue; }
                    parts.Add(key, new PartMasterInfo { CustomerMaterialNo = material, JHSPartNo = jhs, CustomerAbbr = customerAbbr, MaterialName = GetGSCellText(row.GetCell(columns1["\u7269\u6599\u540d\u79f0"])), ProcessType = GetGSCellText(row.GetCell(columns1["\u7c7b\u578b"])), LabelInfo = GetGSCellText(row.GetCell(columns1["\u6807\u7b7e\u4fe1\u606f"])), HasSteelStamp = GetGSCellText(row.GetCell(columns1["\u94a2\u5370\u6709\u65e0\u5173\u8054"])), SortOrder = sortOrder });
                }
                Dictionary<string, string> formats = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                ISheet sheet2 = workbook.GetSheetAt(1);
                string formatHeader = "\u6807\u7b7e\u683c\u5f0f\uff08\u5355\u88c5/\u6df7\u88c5\uff09";
                for (int rowIndex = header2 + 1; rowIndex <= sheet2.LastRowNum; rowIndex++)
                {
                    IRow row = sheet2.GetRow(rowIndex); if (row == null) continue;
                    string material = GetGSCellText(row.GetCell(columns2["\u7269\u6599\u7f16\u53f7"])); string jhs = GetGSCellText(row.GetCell(columns2["JHS\u54c1\u53f7"])); string format = GetGSCellText(row.GetCell(columns2[formatHeader]));
                    if (material.Length == 0 && jhs.Length == 0 && format.Length == 0) continue;
                    if (material.Length == 0 || jhs.Length == 0 || format.Length == 0) { messageList.Add("Incomplete format row " + (rowIndex + 1)); continue; }
                    string key = BuildGSKey(material, jhs); if (formats.ContainsKey(key)) { messageList.Add("Duplicate format key at row " + (rowIndex + 1)); continue; } formats.Add(key, format);
                }
                foreach (KeyValuePair<string, PartMasterInfo> entry in parts) { string format; if (!formats.TryGetValue(entry.Key, out format)) { messageList.Add("Missing label format for key: " + entry.Key.Replace("\u001f", " / ")); continue; } entry.Value.LabelFormat = format; result.Add(entry.Value); }
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
    }
}
