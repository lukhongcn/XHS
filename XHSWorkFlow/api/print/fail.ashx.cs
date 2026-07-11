using System;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;

namespace ModuleWorkFlow.Api
{
    public class PrintFailHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();
            try
            {
                FailPrintRequest request = ReadRequest(context);
                if (request == null || request.TaskId == Guid.Empty)
                {
                    WriteJson(context, new { success = false, message = "缺少有效的 taskId。", data = (object)null });
                    return;
                }

                string message = new XHS.BLL.PrintRecord().FailPrintRecordByTaskId(request.TaskId, request.FailureMessage);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    throw new InvalidOperationException(message);
                }

                WriteJson(context, new { success = true, message = string.Empty, data = new { taskId = request.TaskId.ToString() } });
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                WriteJson(context, new { success = false, message = ex.Message, data = (object)null });
            }
        }

        public bool IsReusable { get { return false; } }

        private static FailPrintRequest ReadRequest(HttpContext context)
        {
            using (StreamReader reader = new StreamReader(context.Request.InputStream))
            {
                string body = reader.ReadToEnd();
                if (!string.IsNullOrWhiteSpace(body))
                {
                    return new JavaScriptSerializer().Deserialize<FailPrintRequest>(body);
                }
            }

            Guid taskId;
            return new FailPrintRequest
            {
                TaskId = Guid.TryParse(context.Request.Form["taskId"], out taskId) ? taskId : Guid.Empty,
                FailureMessage = context.Request.Form["failureMessage"]
            };
        }

        private static void WriteJson(HttpContext context, object value)
        {
            context.Response.Write(new JavaScriptSerializer().Serialize(value));
        }

        private class FailPrintRequest
        {
            public Guid TaskId { get; set; }
            public string FailureMessage { get; set; }
        }
    }
}
