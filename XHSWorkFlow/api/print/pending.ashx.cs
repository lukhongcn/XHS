using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;
using XHS.Model;

namespace ModuleWorkFlow.Api
{
    /// <summary>
    /// 返回并锁定指定打印机最多 30 条待打印记录。
    /// </summary>
    public class PrintPendingHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();

            string machineId = (context.Request.QueryString["machineId"] ?? string.Empty).Trim();
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
                List<PrintRecordInfo> printRecordInfos = printrecord.LockPendingPrintRecords(machineId, 30);
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

        private static string FormatDateTime(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("yyyy-MM-dd HH:mm:ss") : null;
        }

        private static object BuildData(List<PrintRecordInfo> printRecordInfos)
        {
            if (printRecordInfos == null || printRecordInfos.Count == 0)
            {
                return new object[0];
            }

            List<object> data = new List<object>();
            foreach (PrintRecordInfo printRecordInfo in printRecordInfos)
            {
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
                    pdfUrl = printRecordInfo.PdfUrl,
                    pdfDownLoadUrl = printRecordInfo.PdfDownLoadUrl,
                    pdfDownloadPath = printRecordInfo.PdfDownloadPath,
                    localPath = printRecordInfo.LocalPath,
                    status = printRecordInfo.Status,
                    printCount = printRecordInfo.PrintCount,
                    printUser = printRecordInfo.PrintUser,
                    printTime = FormatDateTime(printRecordInfo.PrintTime),
                    createUser = printRecordInfo.CreateUser,
                    createTime = FormatDateTime(printRecordInfo.CreateTime),
                    lockTime = FormatDateTime(printRecordInfo.LockTime)
                });
            }

            return data;
        }

        private static void WriteJson(HttpContext context, object value)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            context.Response.Write(serializer.Serialize(value));
        }
    }
}
