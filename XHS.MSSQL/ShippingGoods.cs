using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Model;
using XHS.IDAL;

namespace XHS.MSSQL
{
    /// <summary>
    /// 出货货品 SQL Server 数据访问实现。
    /// </summary>
    public class ShippingGoods : IShippingGoods
    {
        private const string ConnectionString = "server=.;Pooling=false;database=XHS;uid=sa;pwd=MES2016mj";

        public List<ShippingGoodsInfo> GetShippingGoods()
        {
            const string sql = "select SupplierCode,PartNo,PartChineseName,PartEnglishName,Quantity,SupplyBatchNo,StackLayerCount,ProductionDate,InspectionConfirmDate,CartonNo,SingleBoxGrossWeight,QrCode,Creater,CreatDate from tb_ShippingGoods order by CreatDate desc, SupplyBatchNo";
            return GetShippingGoodsBySql(sql, null);
        }

        public List<ShippingGoodsInfo> GetShippingGoodsBySupplyBatchNo(string supplyBatchNo)
        {
            const string sql = "select SupplierCode,PartNo,PartChineseName,PartEnglishName,Quantity,SupplyBatchNo,StackLayerCount,ProductionDate,InspectionConfirmDate,CartonNo,SingleBoxGrossWeight,QrCode,Creater,CreatDate from tb_ShippingGoods where SupplyBatchNo=@SupplyBatchNo";
            SqlParameter[] parameters =
            {
                new SqlParameter("@SupplyBatchNo", SqlDbType.NVarChar, 100) { Value = ToDbValue(supplyBatchNo) }
            };

            return GetShippingGoodsBySql(sql, parameters);
        }

        public bool InsertShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            if (shippingGoodsInfos == null || shippingGoodsInfos.Count == 0)
            {
                return false;
            }

            const string sql = "insert into tb_ShippingGoods (SupplierCode,PartNo,PartChineseName,PartEnglishName,Quantity,SupplyBatchNo,StackLayerCount,ProductionDate,InspectionConfirmDate,CartonNo,SingleBoxGrossWeight,QrCode,Creater,CreatDate) values (@SupplierCode,@PartNo,@PartChineseName,@PartEnglishName,@Quantity,@SupplyBatchNo,@StackLayerCount,@ProductionDate,@InspectionConfirmDate,@CartonNo,@SingleBoxGrossWeight,@QrCode,@Creater,@CreatDate)";

            return ExecuteBatch(shippingGoodsInfos, sql, BuildInsertOrUpdateParameters);
        }

        public bool UpdateShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            if (shippingGoodsInfos == null || shippingGoodsInfos.Count == 0)
            {
                return false;
            }

            const string sql = "update tb_ShippingGoods set SupplierCode=@SupplierCode,PartNo=@PartNo,PartChineseName=@PartChineseName,PartEnglishName=@PartEnglishName,Quantity=@Quantity,StackLayerCount=@StackLayerCount,ProductionDate=@ProductionDate,InspectionConfirmDate=@InspectionConfirmDate,CartonNo=@CartonNo,SingleBoxGrossWeight=@SingleBoxGrossWeight,QrCode=@QrCode,Creater=@Creater,CreatDate=@CreatDate where SupplyBatchNo=@SupplyBatchNo";

            return ExecuteBatch(shippingGoodsInfos, sql, BuildInsertOrUpdateParameters);
        }

        public bool DeleteShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            if (shippingGoodsInfos == null || shippingGoodsInfos.Count == 0)
            {
                return false;
            }

            const string sql = "delete from tb_ShippingGoods where SupplyBatchNo=@SupplyBatchNo";

            return ExecuteBatch(shippingGoodsInfos, sql, info => new[]
            {
                new SqlParameter("@SupplyBatchNo", SqlDbType.NVarChar, 100) { Value = ToDbValue(info.SupplyBatchNo) }
            });
        }

        private static List<ShippingGoodsInfo> GetShippingGoodsBySql(string sql, SqlParameter[] parameters)
        {
            List<ShippingGoodsInfo> result = new List<ShippingGoodsInfo>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new ShippingGoodsInfo
                        {
                            SupplierCode = ReadString(reader, "SupplierCode"),
                            PartNo = ReadString(reader, "PartNo"),
                            PartChineseName = ReadString(reader, "PartChineseName"),
                            PartEnglishName = ReadString(reader, "PartEnglishName"),
                            Quantity = ReadNullableInt(reader, "Quantity"),
                            SupplyBatchNo = ReadString(reader, "SupplyBatchNo"),
                            StackLayerCount = ReadNullableInt(reader, "StackLayerCount"),
                            ProductionDate = ReadNullableDateTime(reader, "ProductionDate"),
                            InspectionConfirmDate = ReadNullableDateTime(reader, "InspectionConfirmDate"),
                            CartonNo = ReadString(reader, "CartonNo"),
                            SingleBoxGrossWeight = ReadNullableDecimal(reader, "SingleBoxGrossWeight"),
                            QrCode = ReadString(reader, "QrCode"),
                            Creater = ReadString(reader, "Creater"),
                            CreatDate = ReadNullableDateTime(reader, "CreatDate")
                        });
                    }
                }
            }

            return result;
        }

        private static bool ExecuteBatch(List<ShippingGoodsInfo> shippingGoodsInfos, string sql, Func<ShippingGoodsInfo, SqlParameter[]> parameterBuilder)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int affectedRows = 0;
                        foreach (ShippingGoodsInfo shippingGoodsInfo in shippingGoodsInfos)
                        {
                            using (SqlCommand command = new SqlCommand(sql, connection, transaction))
                            {
                                command.Parameters.AddRange(parameterBuilder(shippingGoodsInfo));
                                affectedRows += command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return affectedRows > 0;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
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
                new SqlParameter("@Creater", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.Creater) },
                new SqlParameter("@CreatDate", SqlDbType.DateTime) { Value = ToDbValue(info.CreatDate) }
            };
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

        private static decimal? ReadNullableDecimal(SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? (decimal?)null : reader.GetDecimal(ordinal);
        }

        private static DateTime? ReadNullableDateTime(SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? (DateTime?)null : reader.GetDateTime(ordinal);
        }
    }
}
