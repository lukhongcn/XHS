using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using BLL;
using XHS.Model;

namespace ModuleWorkFlow.Api
{
    /// <summary>
    /// 按 TaskId 回写打印完成状态。
    /// </summary>
    public class PrintCompleteHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();

            if (!string.Equals(context.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 405;
                WriteJson(context, new { success = false, message = "仅支持 POST 请求。", data = (object)null });
                return;
            }

            try
            {
                CompletePrintRequest request = ReadRequest(context);
                if (request == null || request.TaskId == Guid.Empty)
                {
                    WriteJson(context, new { success = false, message = "缺少有效的 taskId。", data = (object)null });
                    return;
                }

                XHS.BLL.PrintRecord printRecordService = new XHS.BLL.PrintRecord();
                List<PrintRecordInfo> printRecordInfos = printRecordService.GetPrintRecordsByTaskId(request.TaskId);
                if (printRecordInfos == null || printRecordInfos.Count == 0)
                {
                    WriteJson(context, new { success = false, message = "未找到对应的打印记录。", data = (object)null });
                    return;
                }

                string savePrintRecordMessage = printRecordService.CompletePrintRecordByTaskId(request.TaskId, request.PrintUser);
                if (!string.IsNullOrWhiteSpace(savePrintRecordMessage))
                {
                    throw new InvalidOperationException(savePrintRecordMessage);
                }

                ShippingGoods shippingGoodsService = new ShippingGoods();
                foreach (PrintRecordInfo printRecordInfo in printRecordInfos)
                {
                    string saveShippingGoodsMessage = shippingGoodsService.CompleteShippingGoodsPrint(
                        SafeValue(printRecordInfo.SupplyBatchNo),
                        SafeValue(printRecordInfo.PartNo),
                        SafeValue(printRecordInfo.CartonNo));

                    if (!string.IsNullOrWhiteSpace(saveShippingGoodsMessage))
                    {
                        throw new InvalidOperationException(saveShippingGoodsMessage);
                    }
                }

                WriteJson(context, new
                {
                    success = true,
                    message = string.Empty,
                    data = new { taskId = request.TaskId.ToString() }
                });
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                WriteJson(context, new { success = false, message = ex.Message, data = (object)null });
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }

        private static CompletePrintRequest ReadRequest(HttpContext context)
        {
            string requestBody;
            using (StreamReader reader = new StreamReader(context.Request.InputStream))
            {
                requestBody = reader.ReadToEnd();
            }

            if (!string.IsNullOrWhiteSpace(requestBody))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                CompletePrintRequest request = serializer.Deserialize<CompletePrintRequest>(requestBody);
                if (request != null)
                {
                    return request;
                }
            }

            Guid taskId;
            return new CompletePrintRequest
            {
                TaskId = Guid.TryParse(context.Request.Form["taskId"], out taskId) ? taskId : Guid.Empty,
                PrintUser = context.Request.Form["printUser"]
            };
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

        private class CompletePrintRequest
        {
            public Guid TaskId { get; set; }

            public string PrintUser { get; set; }
        }
    }
}
