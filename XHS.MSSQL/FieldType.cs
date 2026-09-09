using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Utility;
using XHS.IDAL;
using XHS.Model;

namespace XHS.MSSQL
{
    /// <summary>
    /// 字段解析类型 SQL Server 数据访问实现。
    /// </summary>
    public class FieldType : IFieldType
    {
        private const string SelectColumns = "TypeCode,TypeName,ParserClass";

        public List<FieldTypeInfo> GetFieldTypes()
        {
            DataSet dataSet = Data.getDataSet("select " + SelectColumns + " from tb_FieldType order by TypeCode asc");
            return BuildFieldTypes(dataSet);
        }

        public FieldTypeInfo GetFieldType(string typeCode)
        {
            if (string.IsNullOrWhiteSpace(typeCode))
            {
                return null;
            }

            List<FieldTypeInfo> result = BuildFieldTypes(Data.getDataSet(
                "select " + SelectColumns + " from tb_FieldType where TypeCode=N'" + EscapeSqlLiteral(typeCode.Trim()) + "'"));
            return result.Count == 0 ? null : result[0];
        }

        public ParamterInfo InsertFieldTypes(List<FieldTypeInfo> fieldTypeInfos)
        {
            const string sql = "insert into tb_FieldType (TypeCode,TypeName,ParserClass) values (@TypeCode,@TypeName,@ParserClass)";
            return BuildParamterInfo(fieldTypeInfos, sql, BuildParameters);
        }

        public ParamterInfo UpdateFieldTypes(List<FieldTypeInfo> fieldTypeInfos)
        {
            const string sql = "update tb_FieldType set TypeName=@TypeName,ParserClass=@ParserClass where TypeCode=@TypeCode";
            return BuildParamterInfo(fieldTypeInfos, sql, BuildParameters);
        }

        public ParamterInfo DeleteFieldTypes(List<FieldTypeInfo> fieldTypeInfos)
        {
            const string sql = "delete from tb_FieldType where TypeCode=@TypeCode";
            return BuildParamterInfo(fieldTypeInfos, sql, info => new[]
            {
                new SqlParameter("@TypeCode", SqlDbType.NVarChar, 30) { Value = ToDbValue(info.TypeCode) }
            });
        }

        private static List<FieldTypeInfo> BuildFieldTypes(DataSet dataSet)
        {
            List<FieldTypeInfo> result = new List<FieldTypeInfo>();
            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                return result;
            }

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                result.Add(new FieldTypeInfo
                {
                    TypeCode = row.IsNull("TypeCode") ? null : Convert.ToString(row["TypeCode"]),
                    TypeName = row.IsNull("TypeName") ? null : Convert.ToString(row["TypeName"]),
                    ParserClass = row.IsNull("ParserClass") ? null : Convert.ToString(row["ParserClass"])
                });
            }

            return result;
        }

        private static ParamterInfo BuildParamterInfo(List<FieldTypeInfo> infos, string sql, Func<FieldTypeInfo, SqlParameter[]> parameterBuilder)
        {
            ParamterInfo paramterInfo = new ParamterInfo
            {
                Sql = sql,
                Type = CommandType.Text,
                AlSQL = new ArrayList(),
                AlPAR = new ArrayList(),
                AlCOM = new ArrayList()
            };

            if (infos == null)
            {
                return paramterInfo;
            }

            foreach (FieldTypeInfo info in infos)
            {
                if (info == null)
                {
                    continue;
                }

                paramterInfo.AlSQL.Add(sql);
                paramterInfo.AlPAR.Add(parameterBuilder(info));
                paramterInfo.AlCOM.Add(CommandType.Text);
            }

            return paramterInfo;
        }

        private static SqlParameter[] BuildParameters(FieldTypeInfo info)
        {
            return new[]
            {
                new SqlParameter("@TypeCode", SqlDbType.NVarChar, 30) { Value = ToDbValue(info.TypeCode) },
                new SqlParameter("@TypeName", SqlDbType.NVarChar, 100) { Value = ToDbValue(info.TypeName) },
                new SqlParameter("@ParserClass", SqlDbType.NVarChar, 200) { Value = ToDbValue(info.ParserClass) }
            };
        }

        private static object ToDbValue(object value)
        {
            return value ?? DBNull.Value;
        }

        private static string EscapeSqlLiteral(string value)
        {
            return value.Replace("'", "''");
        }
    }
}
