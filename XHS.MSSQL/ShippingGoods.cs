using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using XHS.Model;
using Utility;
using XHS.IDAL;

namespace XHS.MSSQL
{
    /// <summary>
    /// 出货货品 SQL Server 数据访问实现。
    /// </summary>
    public class ShippingGoods : IShippingGoods
    {
        private const string ShippingGoodsOrderBy = " order by SupplyBatchNo, PartNo, case when charindex('-', CartonNo) > 0 and substring(CartonNo, charindex('-', CartonNo) + 1, len(CartonNo)) not like '%[^0-9]%' then cast(substring(CartonNo, charindex('-', CartonNo) + 1, len(CartonNo)) as int) when CartonNo not like '%[^0-9]%' then cast(CartonNo as int) else 2147483647 end, CartonNo";

        public List<ShippingGoodsInfo> GetShippingGoods()
        {
            const string queryString = "select Id,SupplierCode,PartNo,PartChineseName,PartEnglishName,Quantity,SupplyBatchNo,StackLayerCount,ProductionDate,InspectionConfirmDate,CartonNo,SingleBoxGrossWeight,QrCode,Status,PrintCount,Creater,CreatDate from tb_ShippingGoods" + ShippingGoodsOrderBy;
            return GetShippingGoodsBySql(queryString);
        }

        public List<ShippingGoodsInfo> GetShippingGoods(string partNo, string partName, string supplyBatchNo)
        {
            string queryString = "select Id,SupplierCode,PartNo,PartChineseName,PartEnglishName,Quantity,SupplyBatchNo,StackLayerCount,ProductionDate,InspectionConfirmDate,CartonNo,SingleBoxGrossWeight,QrCode,Status,PrintCount,Creater,CreatDate from tb_ShippingGoods where 1=1";

            if (!string.IsNullOrWhiteSpace(partNo))
            {
                queryString += string.Format(" and PartNo like '%{0}%'", SafeSqlValue(partNo));
            }

            if (!string.IsNullOrWhiteSpace(partName))
            {
                string safePartName = SafeSqlValue(partName);
                queryString += string.Format(" and (PartChineseName like '%{0}%' or PartEnglishName like '%{0}%')", safePartName);
            }

            if (!string.IsNullOrWhiteSpace(supplyBatchNo))
            {
                queryString += string.Format(" and SupplyBatchNo like '%{0}%'", SafeSqlValue(supplyBatchNo));
            }

            queryString += ShippingGoodsOrderBy;
            return GetShippingGoodsBySql(queryString);
        }

        public List<ShippingGoodsInfo> GetShippingGoodsBySupplyBatchNo(string supplyBatchNo, string cartonNo)
        {
            string queryString = string.Format(
                "select Id,SupplierCode,PartNo,PartChineseName,PartEnglishName,Quantity,SupplyBatchNo,StackLayerCount,ProductionDate,InspectionConfirmDate,CartonNo,SingleBoxGrossWeight,QrCode,Status,PrintCount,Creater,CreatDate from tb_ShippingGoods where SupplyBatchNo='{0}'",
                SafeSqlValue(supplyBatchNo));

            if (!string.IsNullOrWhiteSpace(cartonNo))
            {
                queryString += string.Format(" and CartonNo='{0}'", SafeSqlValue(cartonNo));
            }

            queryString += ShippingGoodsOrderBy;
            return GetShippingGoodsBySql(queryString);
        }

        public ParamterInfo InsertShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            const string sql = "insert into tb_ShippingGoods (SupplierCode,PartNo,PartChineseName,PartEnglishName,Quantity,SupplyBatchNo,StackLayerCount,ProductionDate,InspectionConfirmDate,CartonNo,SingleBoxGrossWeight,QrCode,Status,PrintCount,Creater,CreatDate) values (@SupplierCode,@PartNo,@PartChineseName,@PartEnglishName,@Quantity,@SupplyBatchNo,@StackLayerCount,@ProductionDate,@InspectionConfirmDate,@CartonNo,@SingleBoxGrossWeight,@QrCode,@Status,@PrintCount,@Creater,@CreatDate)";
            return BuildParamterInfo(shippingGoodsInfos, sql, BuildInsertOrUpdateParameters);
        }

        public ParamterInfo UpdateShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            const string sql = "update tb_ShippingGoods set SupplierCode=@SupplierCode,PartNo=@PartNo,PartChineseName=@PartChineseName,PartEnglishName=@PartEnglishName,Quantity=@Quantity,StackLayerCount=@StackLayerCount,ProductionDate=@ProductionDate,InspectionConfirmDate=@InspectionConfirmDate,CartonNo=@CartonNo,SingleBoxGrossWeight=@SingleBoxGrossWeight,QrCode=@QrCode,Status=@Status,PrintCount=@PrintCount,Creater=@Creater,CreatDate=@CreatDate where SupplyBatchNo=@SupplyBatchNo and CartonNo=@CartonNo";
            return BuildParamterInfo(shippingGoodsInfos, sql, BuildInsertOrUpdateParameters);
        }

        public ParamterInfo DeleteShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            const string sql = "delete from tb_ShippingGoods where SupplyBatchNo=@SupplyBatchNo and CartonNo=@CartonNo";
            return BuildParamterInfo(shippingGoodsInfos, sql, info => new[]
            {
                new SqlParameter("@SupplyBatchNo", SqlDbType.NVarChar, 100) { Value = ToDbValue(info.SupplyBatchNo) },
                new SqlParameter("@CartonNo", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.CartonNo) }
            });
        }

        private static List<ShippingGoodsInfo> GetShippingGoodsBySql(string queryString)
        {
            List<ShippingGoodsInfo> result = new List<ShippingGoodsInfo>();
            DataSet dataSet = Data.getDataSet(queryString);
            if (dataSet.Tables.Count == 0)
            {
                return result;
            }

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                result.Add(new ShippingGoodsInfo
                {
                    Id = ReadNullableInt(row, "Id"),
                    SupplierCode = ReadString(row, "SupplierCode"),
                    PartNo = ReadString(row, "PartNo"),
                    PartChineseName = ReadString(row, "PartChineseName"),
                    PartEnglishName = ReadString(row, "PartEnglishName"),
                    Quantity = ReadNullableInt(row, "Quantity"),
                    SupplyBatchNo = ReadString(row, "SupplyBatchNo"),
                    StackLayerCount = ReadNullableInt(row, "StackLayerCount"),
                    ProductionDate = ReadNullableDateTime(row, "ProductionDate"),
                    InspectionConfirmDate = ReadNullableDateTime(row, "InspectionConfirmDate"),
                    CartonNo = ReadString(row, "CartonNo"),
                    SingleBoxGrossWeight = ReadNullableDecimal(row, "SingleBoxGrossWeight"),
                    QrCode = ReadString(row, "QrCode"),
                    Status = ReadString(row, "Status"),
                    PrintCount = ReadNullableInt(row, "PrintCount"),
                    Creater = ReadString(row, "Creater"),
                    CreatDate = ReadNullableDateTime(row, "CreatDate")
                });
            }

            return result;
        }

        private static ParamterInfo BuildParamterInfo(List<ShippingGoodsInfo> shippingGoodsInfos, string sql, Func<ShippingGoodsInfo, SqlParameter[]> parameterBuilder)
        {
            ParamterInfo paramterInfo = new ParamterInfo
            {
                Sql = sql,
                Type = CommandType.Text,
                AlSQL = new ArrayList(),
                AlPAR = new ArrayList(),
                AlCOM = new ArrayList()
            };

            if (shippingGoodsInfos == null)
            {
                return paramterInfo;
            }

            foreach (ShippingGoodsInfo shippingGoodsInfo in shippingGoodsInfos)
            {
                paramterInfo.AlSQL.Add(sql);
                paramterInfo.AlPAR.Add(parameterBuilder(shippingGoodsInfo));
                paramterInfo.AlCOM.Add(CommandType.Text);
            }

            return paramterInfo;
        }

        private static SqlParameter[] BuildInsertOrUpdateParameters(ShippingGoodsInfo info)
        {
            return new[]
            {
                new SqlParameter("@SupplierCode", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.SupplierCode) },
                new SqlParameter("@PartNo", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.PartNo) },
                new SqlParameter("@PartChineseName", SqlDbType.NVarChar, 200) { Value = ToDbValue(info.PartChineseName) },
                new SqlParameter("@PartEnglishName", SqlDbType.NVarChar, 200) { Value = ToDbValue(info.PartEnglishName) },
                new SqlParameter("@Quantity", SqlDbType.Int) { Value = ToDbValue(info.Quantity) },
                new SqlParameter("@SupplyBatchNo", SqlDbType.NVarChar, 100) { Value = ToDbValue(info.SupplyBatchNo) },
                new SqlParameter("@StackLayerCount", SqlDbType.Int) { Value = ToDbValue(info.StackLayerCount) },
                new SqlParameter("@ProductionDate", SqlDbType.DateTime) { Value = ToDbValue(info.ProductionDate) },
                new SqlParameter("@InspectionConfirmDate", SqlDbType.DateTime) { Value = ToDbValue(info.InspectionConfirmDate) },
                new SqlParameter("@CartonNo", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.CartonNo) },
                new SqlParameter("@SingleBoxGrossWeight", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = ToDbValue(info.SingleBoxGrossWeight) },
                new SqlParameter("@QrCode", SqlDbType.NVarChar, 200) { Value = ToDbValue(info.QrCode) },
                new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = ToDbValue(info.Status) },
                new SqlParameter("@PrintCount", SqlDbType.Int) { Value = ToDbValue(info.PrintCount) },
                new SqlParameter("@Creater", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.Creater) },
                new SqlParameter("@CreatDate", SqlDbType.DateTime) { Value = ToDbValue(info.CreatDate) }
            };
        }

        private static object ToDbValue(object value)
        {
            return value ?? DBNull.Value;
        }

        private static string SafeSqlValue(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }

        private static string ReadString(DataRow row, string columnName)
        {
            return row.IsNull(columnName) ? null : row[columnName].ToString();
        }

        private static int? ReadNullableInt(DataRow row, string columnName)
        {
            return row.IsNull(columnName) ? (int?)null : Convert.ToInt32(row[columnName]);
        }

        private static decimal? ReadNullableDecimal(DataRow row, string columnName)
        {
            return row.IsNull(columnName) ? (decimal?)null : Convert.ToDecimal(row[columnName]);
        }

        private static DateTime? ReadNullableDateTime(DataRow row, string columnName)
        {
            return row.IsNull(columnName) ? (DateTime?)null : Convert.ToDateTime(row[columnName]);
        }
    }
}
