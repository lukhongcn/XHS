using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Model;
using XHS.IDAL;

namespace XHS.MSSQL
{
    /// <summary>
    /// 不规则表格导入字段配置 SQL Server 数据访问实现。
    /// </summary>
    public class UnRegularTableImportField : IUnRegularTableImportField
    {
        private const string ConnectionString = "server=.;Pooling=false;database=XHS;uid=sa;pwd=MES2016mj";

        public List<UnRegularTableImportFieldInfo> GetUnRegularTableImportFieldByTemplateCode(string templateCode)
        {
            const string sql = "select TemplateCode,ColumnName,ExtractKeyword,FieldProperty,RowIndex,ColumnIndex,OffsetRow,OffsetColumn,IsRequired,SortNo,Comment from tb_UnRegularTableImportField where TemplateCode=@TemplateCode order by SortNo";
            SqlParameter[] parameters =
            {
                new SqlParameter("@TemplateCode", SqlDbType.NVarChar, 50) { Value = ToDbValue(templateCode) }
            };

            List<UnRegularTableImportFieldInfo> result = new List<UnRegularTableImportFieldInfo>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddRange(parameters);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new UnRegularTableImportFieldInfo
                        {
                            TemplateCode = ReadString(reader, "TemplateCode"),
                            ColumnName = ReadString(reader, "ColumnName"),
                            ExtractKeyword = ReadString(reader, "ExtractKeyword"),
                            FieldProperty = ReadString(reader, "FieldProperty"),
                            RowIndex = ReadNullableInt(reader, "RowIndex"),
                            ColumnIndex = ReadNullableInt(reader, "ColumnIndex"),
                            OffsetRow = ReadNullableInt(reader, "OffsetRow"),
                            OffsetColumn = ReadNullableInt(reader, "OffsetColumn"),
                            IsRequired = ReadNullableBoolean(reader, "IsRequired"),
                            SortNo = ReadNullableInt(reader, "SortNo"),
                            Comment = ReadString(reader, "Comment")
                        });
                    }
                }
            }

            return result;
        }

        private static object ToDbValue(object value)
        {
            return value ?? DBNull.Value;
        }

        private static string ReadString(SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        private static int? ReadNullableInt(SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? (int?)null : reader.GetInt32(ordinal);
        }

        private static bool? ReadNullableBoolean(SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? (bool?)null : reader.GetBoolean(ordinal);
        }
    }
}
