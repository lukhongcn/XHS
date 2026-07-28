using System;
using System.Collections.Generic;
using System.Data;
using Utility;
using XHS.IDAL;
using XHS.Model;

namespace XHS.MSSQL
{
    /// <summary>
    /// 装箱异常类型 SQL Server 数据访问实现。
    /// </summary>
    public class PackingExceptionType : IPackingExceptionType
    {
        private const string PackingExceptionTypeSelectColumns = "Id,ExceptionCode,ExceptionName,IsLock,Enable,CreateTime";
        private const string PackingExceptionTypeOrderBy = " order by Id asc";

        public List<PackingExceptionTypeInfo> GetExPackingExceptionTypes()
        {
            string queryString = "select " + PackingExceptionTypeSelectColumns + " from tb_PackingExceptionType" + PackingExceptionTypeOrderBy;
            return GetPackingExceptionTypesBySql(queryString);
        }

        private static List<PackingExceptionTypeInfo> GetPackingExceptionTypesBySql(string queryString)
        {
            DataSet dataSet = Data.getDataSet(queryString);
            return BuildPackingExceptionTypes(dataSet);
        }

        private static List<PackingExceptionTypeInfo> BuildPackingExceptionTypes(DataSet dataSet)
        {
            List<PackingExceptionTypeInfo> result = new List<PackingExceptionTypeInfo>();
            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                return result;
            }

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                result.Add(new PackingExceptionTypeInfo
                {
                    Id = row.IsNull("Id") ? 0 : Convert.ToInt32(row["Id"]),
                    ExceptionCode = row.IsNull("ExceptionCode") ? null : Convert.ToString(row["ExceptionCode"]),
                    ExceptionName = row.IsNull("ExceptionName") ? null : Convert.ToString(row["ExceptionName"]),
                    IsLock = !row.IsNull("IsLock") && Convert.ToBoolean(row["IsLock"]),
                    Enable = !row.IsNull("Enable") && Convert.ToBoolean(row["Enable"]),
                    CreateTime = row.IsNull("CreateTime") ? DateTime.MinValue : Convert.ToDateTime(row["CreateTime"])
                });
            }

            return result;
        }
    }
}
