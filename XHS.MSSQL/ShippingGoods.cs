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
        private const string ShippingGoodsSelectColumns = "Id,SupplierCode,PartNo,PartChineseName,PartEnglishName,Quantity,SupplyBatchNo,StackLayerCount,ProductionDate,InspectionConfirmDate,CartonNo,SingleBoxGrossWeight,QrCode,OutBoxQRCode,Status,PrintCount,Creater,CreatDate,Closer,CloseDate,ExSupplyBatchNo,DeliveryNo,PackageName,PackingStage";
        private const string ShippingGoodsOrderBy = " order by SupplyBatchNo, PartNo, case when charindex('-', CartonNo) > 0 and substring(CartonNo, charindex('-', CartonNo) + 1, len(CartonNo)) not like '%[^0-9]%' then cast(substring(CartonNo, charindex('-', CartonNo) + 1, len(CartonNo)) as int) when CartonNo not like '%[^0-9]%' then cast(CartonNo as int) else 2147483647 end, CartonNo";

        public List<ShippingGoodsInfo> GetShippingGoods()
        {
            const string queryString = "select " + ShippingGoodsSelectColumns + " from tb_ShippingGoods" + ShippingGoodsOrderBy;
            return GetShippingGoodsBySql(queryString);
        }

        public List<ShippingGoodsInfo> GetShippingGoods(string partNo, string partName, string supplyBatchNo)
        {
            return GetShippingGoods(partNo, partName, supplyBatchNo, string.Empty);
        }

        public List<ShippingGoodsInfo> GetShippingGoods(string partNo, string partName, string supplyBatchNo, string closeStatus)
        {
            string queryString = "select " + ShippingGoodsSelectColumns + " from tb_ShippingGoods where 1=1";

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

            if (string.Equals(closeStatus, ShippingGoodsStatusInfo.Closed, StringComparison.OrdinalIgnoreCase))
            {
                queryString += string.Format(" and Status='{0}'", SafeSqlValue(ShippingGoodsStatusInfo.Closed));
            }
            else if (!string.IsNullOrWhiteSpace(closeStatus))
            {
                queryString += string.Format(" and (Status is null or Status='' or Status<>'{0}')", SafeSqlValue(ShippingGoodsStatusInfo.Closed));
            }

            queryString += ShippingGoodsOrderBy;
            return GetShippingGoodsBySql(queryString);
        }

        public List<ShippingGoodsInfo> GetShippingGoodsBySupplyBatchNo(string supplyBatchNo, string cartonNo)
        {
            string queryString = string.Format(
                "select " + ShippingGoodsSelectColumns + " from tb_ShippingGoods where SupplyBatchNo='{0}'",
                SafeSqlValue(supplyBatchNo));

            if (!string.IsNullOrWhiteSpace(cartonNo))
            {
                queryString += string.Format(" and CartonNo='{0}'", SafeSqlValue(cartonNo));
            }

            queryString += ShippingGoodsOrderBy;
            return GetShippingGoodsBySql(queryString);
        }

        public List<ShippingGoodsInfo> GetShippingGoodsByBusinessKey(string supplyBatchNo, string partNo, string cartonNo)
        {
            string queryString = string.Format(
                "select " + ShippingGoodsSelectColumns + " from tb_ShippingGoods where SupplyBatchNo='{0}' and PartNo='{1}' and CartonNo='{2}'",
                SafeSqlValue(supplyBatchNo),
                SafeSqlValue(partNo),
                SafeSqlValue(cartonNo));

            queryString += ShippingGoodsOrderBy;
            return GetShippingGoodsBySql(queryString);
        }

        public List<ShippingGoodsInfo> GetShippingGoodsByBusinessKey(string supplyBatchNo, string partNo, string cartonNo, string deliveryNo)
        {
            string queryString = string.Format(
                "select " + ShippingGoodsSelectColumns + " from tb_ShippingGoods where SupplyBatchNo='{0}' and PartNo='{1}' and CartonNo='{2}' and DeliveryNo='{3}'",
                SafeSqlValue(supplyBatchNo),
                SafeSqlValue(partNo),
                SafeSqlValue(cartonNo),
                SafeSqlValue(deliveryNo));

            queryString += ShippingGoodsOrderBy;
            return GetShippingGoodsBySql(queryString);
        }

        public string GetNextAutoDeliveryNo(DateTime date)
        {
            string datePrefix = date.ToString("yyyyMMdd");
            const string queryString = "select isnull(max(try_convert(int, right(DeliveryNo, 3))), 0) as SequenceNo from tb_ShippingGoods where DeliveryNo like @DeliveryNoPrefix + '[0-9][0-9][0-9]'";
            DataSet dataSet = Data.getDataSet(queryString, new[]
            {
                new SqlParameter("@DeliveryNoPrefix", SqlDbType.NVarChar, 8) { Value = datePrefix }
            });

            int sequenceNo = 0;
            if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0 && dataSet.Tables[0].Rows[0]["SequenceNo"] != DBNull.Value)
            {
                int.TryParse(dataSet.Tables[0].Rows[0]["SequenceNo"].ToString(), out sequenceNo);
            }

            if (sequenceNo >= 999)
            {
                throw new ApplicationException("当天自动配送单号流水号已超过 999，无法继续生成。");
            }

            return datePrefix + (sequenceNo + 1).ToString("D3");
        }

        public ParamterInfo InsertShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            const string sql = "insert into tb_ShippingGoods (SupplierCode,PartNo,PartChineseName,PartEnglishName,Quantity,SupplyBatchNo,StackLayerCount,ProductionDate,InspectionConfirmDate,CartonNo,SingleBoxGrossWeight,QrCode,OutBoxQRCode,Status,PrintCount,Creater,CreatDate,Closer,CloseDate,ExSupplyBatchNo,DeliveryNo,PackageName,PackingStage) values (@SupplierCode,@PartNo,@PartChineseName,@PartEnglishName,@Quantity,@SupplyBatchNo,@StackLayerCount,@ProductionDate,@InspectionConfirmDate,@CartonNo,@SingleBoxGrossWeight,@QrCode,@OutBoxQRCode,@Status,@PrintCount,@Creater,@CreatDate,@Closer,@CloseDate,@ExSupplyBatchNo,@DeliveryNo,@PackageName,@PackingStage)";
            return BuildParamterInfo(shippingGoodsInfos, sql, BuildInsertOrUpdateParameters);
        }

        public ParamterInfo UpdateShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            const string sql = "update tb_ShippingGoods set SupplierCode=@SupplierCode,PartNo=@PartNo,PartChineseName=@PartChineseName,PartEnglishName=@PartEnglishName,Quantity=@Quantity,StackLayerCount=@StackLayerCount,ProductionDate=@ProductionDate,InspectionConfirmDate=@InspectionConfirmDate,CartonNo=@CartonNo,SingleBoxGrossWeight=@SingleBoxGrossWeight,QrCode=@QrCode,OutBoxQRCode=@OutBoxQRCode,Status=@Status,PrintCount=@PrintCount,Creater=@Creater,CreatDate=@CreatDate,Closer=@Closer,CloseDate=@CloseDate,ExSupplyBatchNo=@ExSupplyBatchNo,DeliveryNo=@DeliveryNo,PackageName=@PackageName,PackingStage=@PackingStage where SupplyBatchNo=@SupplyBatchNo and PartNo=@PartNo and CartonNo=@CartonNo";
            return BuildParamterInfo(shippingGoodsInfos, sql, BuildInsertOrUpdateParameters);
        }

        public ParamterInfo UpdateShippingGoodsByBusinessKey(List<ShippingGoodsInfo> shippingGoodsInfos, string deliveryNo)
        {
            const string sql = "update tb_ShippingGoods set SupplierCode=@SupplierCode,PartNo=@PartNo,PartChineseName=@PartChineseName,PartEnglishName=@PartEnglishName,Quantity=@Quantity,StackLayerCount=@StackLayerCount,ProductionDate=@ProductionDate,InspectionConfirmDate=@InspectionConfirmDate,CartonNo=@CartonNo,SingleBoxGrossWeight=@SingleBoxGrossWeight,QrCode=@QrCode,OutBoxQRCode=@OutBoxQRCode,Status=@Status,PrintCount=@PrintCount,Creater=@Creater,CreatDate=@CreatDate,Closer=@Closer,CloseDate=@CloseDate,ExSupplyBatchNo=@ExSupplyBatchNo,DeliveryNo=@DeliveryNo,PackageName=@PackageName,PackingStage=@PackingStage where SupplyBatchNo=@SupplyBatchNo and PartNo=@PartNo and CartonNo=@CartonNo and DeliveryNo=@DeliveryNo";
            return BuildParamterInfo(shippingGoodsInfos, sql, BuildInsertOrUpdateParameters);
        }

        public ParamterInfo UpdatePrintedDeliveryNo(string supplyBatchNo, string partNo, string cartonNo, string oldDeliveryNo, string newDeliveryNo)
        {
            ParamterInfo paramterInfo = new ParamterInfo
            {
                AlSQL = new ArrayList(),
                AlPAR = new ArrayList(),
                AlCOM = new ArrayList()
            };
            paramterInfo.AlSQL.Add("update tb_ShippingGoods set DeliveryNo=@NewDeliveryNo where SupplyBatchNo=@SupplyBatchNo and PartNo=@PartNo and DeliveryNo=@OldDeliveryNo and (PrintCount > 0 or Status=@PrintedStatus)");
            paramterInfo.AlPAR.Add(new[]
            {
                new SqlParameter("@NewDeliveryNo", SqlDbType.NVarChar, 100) { Value = newDeliveryNo ?? string.Empty },
                new SqlParameter("@SupplyBatchNo", SqlDbType.NVarChar, 100) { Value = supplyBatchNo ?? string.Empty },
                new SqlParameter("@PartNo", SqlDbType.NVarChar, 50) { Value = partNo ?? string.Empty },
                new SqlParameter("@OldDeliveryNo", SqlDbType.NVarChar, 100) { Value = oldDeliveryNo ?? string.Empty },
                new SqlParameter("@PrintedStatus", SqlDbType.NVarChar, 20) { Value = ShippingGoodsStatusInfo.Printed }
            });
            paramterInfo.AlCOM.Add(CommandType.Text);
            return paramterInfo;
        }

        public ParamterInfo UpdateShippingGoodsClose(string supplyBatchNo, string partNo, string deliveryNo, string closer, DateTime closeDate)
        {
            ParamterInfo paramterInfo = new ParamterInfo
            {
                AlSQL = new ArrayList(),
                AlPAR = new ArrayList(),
                AlCOM = new ArrayList()
            };
            paramterInfo.AlSQL.Add("update tb_ShippingGoods set Status=@Status,Closer=@Closer,CloseDate=@CloseDate where SupplyBatchNo=@SupplyBatchNo and PartNo=@PartNo and DeliveryNo=@DeliveryNo");
            paramterInfo.AlPAR.Add(new[]
            {
                new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = ShippingGoodsStatusInfo.Closed },
                new SqlParameter("@Closer", SqlDbType.NVarChar, 50) { Value = closer ?? string.Empty },
                new SqlParameter("@CloseDate", SqlDbType.DateTime) { Value = closeDate },
                new SqlParameter("@SupplyBatchNo", SqlDbType.NVarChar, 100) { Value = supplyBatchNo ?? string.Empty },
                new SqlParameter("@PartNo", SqlDbType.NVarChar, 50) { Value = partNo ?? string.Empty },
                new SqlParameter("@DeliveryNo", SqlDbType.NVarChar, 100) { Value = deliveryNo ?? string.Empty }
            });
            paramterInfo.AlCOM.Add(CommandType.Text);
            return paramterInfo;
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

        public ParamterInfo DeleteShippingGoodsByBusinessKey(List<ShippingGoodsInfo> shippingGoodsInfos, string deliveryNo)
        {
            const string sql = "delete from tb_ShippingGoods where SupplyBatchNo=@SupplyBatchNo and PartNo=@PartNo and CartonNo=@CartonNo and DeliveryNo=@DeliveryNo";
            return BuildParamterInfo(shippingGoodsInfos, sql, info => new[]
            {
                new SqlParameter("@SupplyBatchNo", SqlDbType.NVarChar, 100) { Value = ToDbValue(info.SupplyBatchNo) },
                new SqlParameter("@PartNo", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.PartNo) },
                new SqlParameter("@CartonNo", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.CartonNo) },
                new SqlParameter("@DeliveryNo", SqlDbType.NVarChar, 100) { Value = deliveryNo ?? string.Empty }
            });
        }

        public ParamterInfo DeleteShippingGoodsByBusinessKey(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            const string sql = "delete from tb_ShippingGoods where SupplyBatchNo=@SupplyBatchNo and PartNo=@PartNo and CartonNo=@CartonNo";
            return BuildParamterInfo(shippingGoodsInfos, sql, info => new[]
            {
                new SqlParameter("@SupplyBatchNo", SqlDbType.NVarChar, 100) { Value = ToDbValue(info.SupplyBatchNo) },
                new SqlParameter("@PartNo", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.PartNo) },
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
                    OutBoxQRCode = ReadString(row, "OutBoxQRCode"),
                    Status = ReadString(row, "Status"),
                    PrintCount = ReadNullableInt(row, "PrintCount"),
                    Creater = ReadString(row, "Creater"),
                    CreatDate = ReadNullableDateTime(row, "CreatDate"),
                    Closer = ReadString(row, "Closer"),
                    CloseDate = ReadNullableDateTime(row, "CloseDate"),
                    ExSupplyBatchNo = ReadString(row, "ExSupplyBatchNo"),
                    DeliveryNo = ReadString(row, "DeliveryNo"),
                    PackageName = ReadString(row, "PackageName"),
                    PackingStage = ReadString(row, "PackingStage")
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
                new SqlParameter("@OutBoxQRCode", SqlDbType.NVarChar, 500) { Value = ToDbValue(info.OutBoxQRCode) },
                new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = ToDbValue(info.Status) },
                new SqlParameter("@PrintCount", SqlDbType.Int) { Value = ToDbValue(info.PrintCount) },
                new SqlParameter("@Creater", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.Creater) },
                new SqlParameter("@CreatDate", SqlDbType.DateTime) { Value = ToDbValue(info.CreatDate) },
                new SqlParameter("@Closer", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.Closer) },
                new SqlParameter("@CloseDate", SqlDbType.DateTime) { Value = ToDbValue(info.CloseDate) },
                new SqlParameter("@ExSupplyBatchNo", SqlDbType.NVarChar, 100) { Value = ToDbValue(info.ExSupplyBatchNo) },
                new SqlParameter("@DeliveryNo", SqlDbType.NVarChar, 100) { Value = ToDbValue(info.DeliveryNo) },
                new SqlParameter("@PackageName", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.PackageName) },
                new SqlParameter("@PackingStage", SqlDbType.NVarChar, 30) { Value = ToDbValue(info.PackingStage) }
            };
        }

        private static SqlParameter[] BuildCloseParameters(ShippingGoodsInfo info)
        {
            return new[]
            {
                new SqlParameter("@SupplyBatchNo", SqlDbType.NVarChar, 100) { Value = ToDbValue(info.SupplyBatchNo) },
                new SqlParameter("@PartNo", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.PartNo) },
                new SqlParameter("@CartonNo", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.CartonNo) },
                new SqlParameter("@DeliveryNo", SqlDbType.NVarChar, 100) { Value = ToDbValue(info.DeliveryNo) },
                new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = ToDbValue(info.Status) },
                new SqlParameter("@Closer", SqlDbType.NVarChar, 50) { Value = ToDbValue(info.Closer) },
                new SqlParameter("@CloseDate", SqlDbType.DateTime) { Value = ToDbValue(info.CloseDate) }
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

