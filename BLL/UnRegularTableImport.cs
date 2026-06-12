using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using XHS.Model;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Utility;

namespace BLL
{
    /// <summary>
    /// 不规则表格通用导入。
    /// </summary>
    public class UnRegularTableImport
    {
        public List<T> GetList<T>(string fileName, out List<string> messageList) where T : new()
        {
            messageList = new List<string>();
            List<T> result = new List<T>();

            if (!File.Exists(fileName))
            {
                messageList.Add("文件不存在：" + fileName);
                return result;
            }

            string templateCode = GetTemplateCode<T>();
            List<UnRegularTableImportFieldInfo> fields = new UnRegularTableImportField()
                .GetUnRegularTableImportFieldByTemplateCode(templateCode)
                .OrderBy(p => p.SortNo)
                .ToList();

            if (fields.Count == 0)
            {
                messageList.Add("未找到导入模板配置：" + templateCode);
                return result;
            }

            int maxOffsetColumn = GetAppSettingInt("UnRegularTableImportMaxOffsetColumn");
            int maxOffsetRow = GetAppSettingInt("UnRegularTableImportMaxOffsetRow");
            UnRegularTableImportFieldInfo startField = fields[0];

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fs);
                for (int sheetIndex = 0; sheetIndex < workbook.NumberOfSheets; sheetIndex++)
                {
                    ISheet sheet = workbook.GetSheetAt(sheetIndex);
                    if (sheet == null)
                    {
                        continue;
                    }

                    ReadSheet(sheet, fields, startField, maxOffsetRow, maxOffsetColumn, result, messageList);
                }
            }

            return result;
        }

        private static void ReadSheet<T>(
            ISheet sheet,
            List<UnRegularTableImportFieldInfo> fields,
            UnRegularTableImportFieldInfo startField,
            int maxOffsetRow,
            int maxOffsetColumn,
            List<T> result,
            List<string> messageList) where T : new()
        {
            for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                IRow row = sheet.GetRow(rowIndex);
                if (row == null)
                {
                    continue;
                }

                for (int columnIndex = 0; columnIndex < row.LastCellNum; columnIndex++)
                {
                    if (GetCellText(sheet, rowIndex, columnIndex) != startField.ExtractKeyword)
                    {
                        continue;
                    }

                    T item;
                    if (TryReadItem(sheet, rowIndex, columnIndex, fields, maxOffsetRow, maxOffsetColumn, messageList, out item))
                    {
                        result.Add(item);
                    }
                }
            }
        }

        private static bool TryReadItem<T>(
            ISheet sheet,
            int startRow,
            int startColumn,
            List<UnRegularTableImportFieldInfo> fields,
            int maxOffsetRow,
            int maxOffsetColumn,
            List<string> messageList,
            out T item) where T : new()
        {
            item = new T();

            foreach (UnRegularTableImportFieldInfo field in fields)
            {
                int keyRow = startRow + (field.RowIndex ?? 0);
                int keyColumn = startColumn + (field.ColumnIndex ?? 0);

                if (keyRow > startRow + maxOffsetRow || keyColumn > startColumn + maxOffsetColumn)
                {
                    messageList.Add(GetPosition(sheet, startRow, startColumn) + " 字段超出模板扫描范围：" + field.ColumnName);
                    return false;
                }

                string actualKeyword = GetCellText(sheet, keyRow, keyColumn);
                string expectedKeyword = (field.ExtractKeyword ?? string.Empty).Trim();
                if (actualKeyword != expectedKeyword)
                {
                    messageList.Add(GetPosition(sheet, keyRow, keyColumn) + " 关键字不匹配，应为：" + expectedKeyword + "，实际：" + actualKeyword);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(field.FieldProperty))
                {
                    continue;
                }

                string cellText = GetCellText(sheet, keyRow + (field.OffsetRow ?? 0), keyColumn + (field.OffsetColumn ?? 0));
                if (field.IsRequired == true && string.IsNullOrWhiteSpace(cellText))
                {
                    messageList.Add(GetPosition(sheet, keyRow, keyColumn) + " 必填字段为空：" + field.ColumnName);
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(cellText) && Reflector.hasPropertyMember(item, field.FieldProperty))
                {
                    Reflector.SetProperty(item, field.FieldProperty, ConvertValue(Reflector.getPropertyType(item, field.FieldProperty), cellText));
                }
            }

            return true;
        }

        private static string GetCellText(ISheet sheet, int rowIndex, int columnIndex)
        {
            if (sheet == null || rowIndex < 0 || columnIndex < 0)
            {
                return string.Empty;
            }

            IRow row = sheet.GetRow(rowIndex);
            if (row == null)
            {
                return string.Empty;
            }

            ICell cell = row.GetCell(columnIndex);
            if (cell == null)
            {
                return string.Empty;
            }

            if (cell.CellType == CellType.Numeric)
            {
                return DateUtil.IsCellDateFormatted(cell)
                    ? cell.DateCellValue.ToString("yyyy/MM/dd")
                    : cell.NumericCellValue.ToString(CultureInfo.InvariantCulture);
            }

            return cell.ToString().Trim();
        }

        private static object ConvertValue(Type propertyType, string cellText)
        {
            Type targetType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
            string value = cellText == null ? string.Empty : cellText.Trim();

            if (targetType == typeof(string))
            {
                return value;
            }

            if (targetType == typeof(int))
            {
                return string.IsNullOrWhiteSpace(value) ? 0 : Convert.ToInt32(decimal.Parse(value, CultureInfo.InvariantCulture));
            }

            if (targetType == typeof(decimal))
            {
                value = value.Replace("g", string.Empty).Replace("G", string.Empty).Trim();
                return string.IsNullOrWhiteSpace(value) ? 0M : decimal.Parse(value, CultureInfo.InvariantCulture);
            }

            if (targetType == typeof(DateTime))
            {
                return DateTime.Parse(value, CultureInfo.InvariantCulture);
            }

            if (targetType == typeof(bool))
            {
                return value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase);
            }

            return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }

        private static string GetTemplateCode<T>()
        {
            string typeName = typeof(T).Name;
            return typeName.EndsWith("Info") ? typeName.Substring(0, typeName.Length - 4) : typeName;
        }

        private static int GetAppSettingInt(string key)
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings[key]);
        }

        private static string GetPosition(ISheet sheet, int rowIndex, int columnIndex)
        {
            return string.Format("Sheet:{0} 行:{1} 列:{2}", sheet.SheetName, rowIndex + 1, columnIndex + 1);
        }
    }
}
