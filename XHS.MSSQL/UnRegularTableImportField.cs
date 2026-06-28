using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Utility;
using XHS.Model;
using XHS.IDAL;

namespace XHS.MSSQL
{
    /// <summary>
    /// 不规则表格导入字段配置 SQL Server 数据访问实现。
    /// </summary>
    public class UnRegularTableImportField : IUnRegularTableImportField
    {
        public List<UnRegularTableImportFieldInfo> GetUnRegularTableImportFieldByTemplateCode(string templateCode)
        {
            const string sql = "select TemplateCode,ColumnName,ExtractKeyword,FieldProperty,RowIndex,ColumnIndex,OffsetRow,OffsetColumn,IsRequired,SortNo,Comment from tb_UnRegularTableImportField where TemplateCode=@TemplateCode order by SortNo";
            SqlParameter[] parameters =
            {
                new SqlParameter("@TemplateCode", SqlDbType.NVarChar, 50) { Value = ToDbValue(templateCode) }
            };

            List<UnRegularTableImportFieldInfo> result = new List<UnRegularTableImportFieldInfo>();
            DataSet dataSet = Data.getDataSet(sql, parameters);
            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                return result;
            }

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                result.Add(new UnRegularTableImportFieldInfo
                {
                    TemplateCode = ReadString(row, "TemplateCode"),
                    ColumnName = ReadString(row, "ColumnName"),
                    ExtractKeyword = ReadString(row, "ExtractKeyword"),
                    FieldProperty = ReadString(row, "FieldProperty"),
                    RowIndex = ReadNullableInt(row, "RowIndex"),
                    ColumnIndex = ReadNullableInt(row, "ColumnIndex"),
                    OffsetRow = ReadNullableInt(row, "OffsetRow"),
                    OffsetColumn = ReadNullableInt(row, "OffsetColumn"),
                    IsRequired = ReadNullableBoolean(row, "IsRequired"),
                    SortNo = ReadNullableInt(row, "SortNo"),
                    Comment = ReadString(row, "Comment")
                });
            }

            return result;
        }

        private static object ToDbValue(object value)
        {
            return value ?? DBNull.Value;
        }

        private static string ReadString(DataRow row, string columnName)
        {
            return row.IsNull(columnName) ? null : row[columnName].ToString();
        }

        private static int? ReadNullableInt(DataRow row, string columnName)
        {
            return row.IsNull(columnName) ? (int?)null : Convert.ToInt32(row[columnName]);
        }

        private static bool? ReadNullableBoolean(DataRow row, string columnName)
        {
            return row.IsNull(columnName) ? (bool?)null : Convert.ToBoolean(row[columnName]);
        }
    }
}
