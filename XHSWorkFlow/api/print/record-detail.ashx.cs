using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using XHS.Model;

namespace ModuleWorkFlow.Api
{
    /// <summary>
    /// 返回打印记录明细表格 HTML。
    /// </summary>
    public class PrintRecordDetailHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();

            string supplyBatchNo = SafeValue(context.Request.QueryString["supplyBatchNo"]);
            string partNo = SafeValue(context.Request.QueryString["partNo"]);
            string cartonNo = SafeValue(context.Request.QueryString["cartonNo"]);
            string printType = SafeValue(context.Request.QueryString["printType"]);

            try
            {
                XHS.BLL.PrintRecord printRecordService = new XHS.BLL.PrintRecord();
                List<PrintRecordInfo> printRecordInfos = printRecordService.GetPrintRecords(
                    supplyBatchNo,
                    partNo,
                    cartonNo,
                    printType);

                context.Response.Write(BuildHtml(printRecordInfos));
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.Write("<div style=\"padding:8px;color:#cc0000;\">");
                context.Response.Write(HttpUtility.HtmlEncode(ex.Message));
                context.Response.Write("</div>");
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }

        private static string BuildHtml(List<PrintRecordInfo> printRecordInfos)
        {
            List<PrintRecordInfo> details = (printRecordInfos ?? new List<PrintRecordInfo>())
                .Where(item => item != null)
                .OrderBy(item => item.PrintTime ?? item.CreateTime ?? DateTime.MinValue)
                .ThenBy(item => item.Id ?? 0)
                .ToList();

            if (details.Count == 0)
            {
                return "<div style=\"padding:8px;color:#666666;\">未找到打印记录。</div>";
            }

            StringBuilder html = new StringBuilder();
            html.Append("<table style=\"width:100%;border-collapse:collapse;background-color:#ffffff;\">");
            html.Append("<thead><tr>");
            html.Append("<th style=\"border:1px solid #d9d9d9;padding:6px 8px;background-color:#d9e7f7;\">打印次序</th>");
            html.Append("<th style=\"border:1px solid #d9d9d9;padding:6px 8px;background-color:#d9e7f7;\">打印人</th>");
            html.Append("<th style=\"border:1px solid #d9d9d9;padding:6px 8px;background-color:#d9e7f7;\">打印时间</th>");
            html.Append("<th style=\"border:1px solid #d9d9d9;padding:6px 8px;background-color:#d9e7f7;\">补打原因</th>");
            html.Append("<th style=\"border:1px solid #d9d9d9;padding:6px 8px;background-color:#d9e7f7;\">状态</th>");
            html.Append("</tr></thead><tbody>");

            for (int i = 0; i < details.Count; i++)
            {
                PrintRecordInfo item = details[i];
                html.Append("<tr>");
                html.AppendFormat("<td style=\"border:1px solid #d9d9d9;padding:6px 8px;text-align:center;\">第{0}次</td>", i + 1);
                html.AppendFormat("<td style=\"border:1px solid #d9d9d9;padding:6px 8px;text-align:center;\">{0}</td>", HttpUtility.HtmlEncode(SafeValue(item.PrintUser)));
                html.AppendFormat("<td style=\"border:1px solid #d9d9d9;padding:6px 8px;text-align:center;\">{0}</td>", HttpUtility.HtmlEncode(FormatDateTime(item.PrintTime)));
                html.AppendFormat("<td style=\"border:1px solid #d9d9d9;padding:6px 8px;text-align:center;\">{0}</td>", HttpUtility.HtmlEncode(SafeValue(item.ReprintReason)));
                html.AppendFormat("<td style=\"border:1px solid #d9d9d9;padding:6px 8px;text-align:center;\">{0}</td>", HttpUtility.HtmlEncode(GetStatusText(item.Status)));
                html.Append("</tr>");
            }

            html.Append("</tbody></table>");
            html.AppendFormat("<div style=\"padding:8px 0;color:#666666;\">共 {0} 条打印记录。</div>", details.Count);
            return html.ToString();
        }

        private static string SafeValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private static string FormatDateTime(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
        }

        private static string GetStatusText(int? status)
        {
            if (!status.HasValue)
            {
                return string.Empty;
            }

            switch (status.Value)
            {
                case PrintRecordStatusInfo.Pending:
                    return "待打印";
                case PrintRecordStatusInfo.Printing:
                    return "打印中";
                case PrintRecordStatusInfo.Completed:
                    return "已完成";
                case PrintRecordStatusInfo.Failed:
                    return "失败";
                default:
                    return status.Value.ToString();
            }
        }
    }
}
