using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using BLL;
using XHS.Model;

namespace ModuleWorkFlow.Api
{
    /// <summary>
    /// 返回并锁定指定打印机最多 30 条待打印标签数据。
    /// </summary>
    public class PrintPendingLabelsHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();

            string machineId = (context.Request.QueryString["machineId"] ?? string.Empty).Trim();
            int lockTimeoutMinutes;
            if (!int.TryParse(context.Request.QueryString["lockTimeoutMinutes"], out lockTimeoutMinutes))
            {
                lockTimeoutMinutes = 3;
            }
            lockTimeoutMinutes = Math.Max(1, Math.Min(60, lockTimeoutMinutes));
            if (string.IsNullOrWhiteSpace(machineId))
            {
                WriteJson(context, new
                {
                    success = false,
                    message = "缺少参数 machineId。",
                    data = (object)null
                });
                return;
            }

            try
            {
                XHS.BLL.PrintRecord printrecord = new XHS.BLL.PrintRecord();
                List<PrintRecordInfo> printRecordInfos = printrecord.LockPendingPrintRecords(machineId, 30, lockTimeoutMinutes);
                WriteJson(context, new
                {
                    success = true,
                    message = string.Empty,
                    data = BuildData(printRecordInfos)
                });
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                WriteJson(context, new
                {
                    success = false,
                    message = ex.Message,
                    data = (object)null
                });
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }

        private static object BuildData(List<PrintRecordInfo> printRecordInfos)
        {
            if (printRecordInfos == null || printRecordInfos.Count == 0)
            {
                return new object[0];
            }

            ShippingGoods shippingGoodsService = new ShippingGoods();
            List<object> data = new List<object>();
            foreach (PrintRecordInfo printRecordInfo in printRecordInfos)
            {
                ShippingGoodsInfo shippingGoodsInfo = GetShippingGoodsInfo(shippingGoodsService, printRecordInfo);
                if (shippingGoodsInfo == null)
                {
                    throw new InvalidOperationException(string.Format(
                        "未找到对应的出货货品数据：{0}/{1}/{2}",
                        SafeValue(printRecordInfo.SupplyBatchNo),
                        SafeValue(printRecordInfo.PartNo),
                        SafeValue(printRecordInfo.CartonNo)));
                }

                data.Add(new
                {
                    id = printRecordInfo.Id,
                    supplyBatchNo = printRecordInfo.SupplyBatchNo,
                    partNo = printRecordInfo.PartNo,
                    cartonNo = printRecordInfo.CartonNo,
                    taskId = printRecordInfo.TaskId,
                    clientId = printRecordInfo.ClientId,
                    machineId = printRecordInfo.MachineId,
                    printType = printRecordInfo.PrintType,
                    printCount = printRecordInfo.PrintCount,
                    printUser = printRecordInfo.PrintUser,
                    printTime = FormatDateTime(printRecordInfo.PrintTime),
                    createUser = printRecordInfo.CreateUser,
                    createTime = FormatDateTime(printRecordInfo.CreateTime),
                    lockTime = FormatDateTime(printRecordInfo.LockTime),
                    labelInfo = XHS.BLL.ShippingGoodsLabelBuilder.BuildOuterBoxLabelInfo(shippingGoodsInfo)
                });
            }

            return data;
        }

        private static ShippingGoodsInfo GetShippingGoodsInfo(ShippingGoods shippingGoodsService, PrintRecordInfo printRecordInfo)
        {
            List<ShippingGoodsInfo> shippingGoodsInfos = shippingGoodsService.GetShippingGoodsBySupplyBatchNo(
                SafeValue(printRecordInfo.SupplyBatchNo),
                SafeValue(printRecordInfo.CartonNo));

            return shippingGoodsInfos.FirstOrDefault(item =>
                item != null &&
                string.Equals(SafeValue(item.PartNo), SafeValue(printRecordInfo.PartNo), StringComparison.OrdinalIgnoreCase) &&
                string.Equals(SafeValue(item.SupplyBatchNo), SafeValue(printRecordInfo.SupplyBatchNo), StringComparison.OrdinalIgnoreCase) &&
                string.Equals(SafeValue(item.CartonNo), SafeValue(printRecordInfo.CartonNo), StringComparison.OrdinalIgnoreCase));
        }

        private static string FormatDateTime(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("yyyy-MM-dd HH:mm:ss") : null;
        }

        private static string SafeValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private static void WriteJson(HttpContext context, object value)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            context.Response.Write(serializer.Serialize(value));
        }
    }
}
