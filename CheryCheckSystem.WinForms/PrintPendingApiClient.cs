using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace CheryCheckSystem.PrintClient
{
    public class PrintPendingApiClient
    {
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();

        public List<PrintPendingRecord> FetchPendingRecords(string apiBaseUrl, string pendingApiPath, string machineId)
        {
            string requestUrl = BuildPendingUrl(apiBaseUrl, pendingApiPath, machineId);
            using (WebClient webClient = CreateWebClient())
            {
                string responseText = webClient.DownloadString(requestUrl);
                PrintPendingApiResponse response = _serializer.Deserialize<PrintPendingApiResponse>(responseText);
                if (response == null)
                {
                    throw new InvalidOperationException("接口返回为空。");
                }

                if (!response.success)
                {
                    throw new InvalidOperationException(string.IsNullOrWhiteSpace(response.message)
                        ? "接口返回失败。"
                        : response.message);
                }

                return response.data ?? new List<PrintPendingRecord>();
            }
        }

        public List<PrintPendingRecord> FetchPendingRecordsByFullUrl(string requestUrl)
        {
            if (string.IsNullOrWhiteSpace(requestUrl))
            {
                throw new InvalidOperationException("未配置待打印接口完整地址。");
            }

            using (WebClient webClient = CreateWebClient())
            {
                string responseText = webClient.DownloadString(requestUrl);
                PrintPendingApiResponse response = _serializer.Deserialize<PrintPendingApiResponse>(responseText);
                if (response == null)
                {
                    throw new InvalidOperationException("接口返回为空。");
                }

                if (!response.success)
                {
                    throw new InvalidOperationException(string.IsNullOrWhiteSpace(response.message)
                        ? "接口返回失败。"
                        : response.message);
                }

                return response.data ?? new List<PrintPendingRecord>();
            }
        }

        public string DownloadPdf(string apiBaseUrl, PrintPendingRecord record, string downloadFolder)
        {
            if (record == null)
            {
                throw new ArgumentNullException("record");
            }

            string remotePath = SafeValue(record.pdfDownLoadUrl);
            if (string.IsNullOrWhiteSpace(remotePath))
            {
                remotePath = SafeValue(record.pdfDownloadPath);
            }

            if (string.IsNullOrWhiteSpace(remotePath))
            {
                throw new InvalidOperationException("记录未提供 PDF 下载路径。");
            }

            Directory.CreateDirectory(downloadFolder);

            Uri downloadUri = BuildUri(apiBaseUrl, remotePath);
            string remoteFileName = Path.GetFileName(downloadUri.LocalPath);
            if (string.IsNullOrWhiteSpace(remoteFileName))
            {
                remoteFileName = "label.pdf";
            }

            string prefix = record.id.HasValue ? record.id.Value.ToString() : Guid.NewGuid().ToString("N");
            string localFilePath = Path.Combine(downloadFolder, prefix + "_" + remoteFileName);

            using (WebClient webClient = CreateWebClient())
            {
                webClient.DownloadFile(downloadUri, localFilePath);
            }

            return localFilePath;
        }

        public void CompletePrint(string completeApiUrl, string taskId, string printUser)
        {
            if (string.IsNullOrWhiteSpace(completeApiUrl))
            {
                throw new InvalidOperationException("未配置打印完成接口地址。");
            }

            string requestBody = _serializer.Serialize(new
            {
                TaskId = taskId,
                PrintUser = printUser
            });

            using (WebClient webClient = CreateWebClient())
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                string responseText = webClient.UploadString(completeApiUrl, "POST", requestBody);
                PrintPendingApiResponse response = _serializer.Deserialize<PrintPendingApiResponse>(responseText);
                if (response == null)
                {
                    throw new InvalidOperationException("打印完成接口返回为空。");
                }

                if (!response.success)
                {
                    throw new InvalidOperationException(string.IsNullOrWhiteSpace(response.message) ? "打印完成接口返回失败。" : response.message);
                }
            }
        }

        public string BuildPendingUrl(string apiBaseUrl, string pendingApiPath, string machineId)
        {
            string safeMachineId = Uri.EscapeDataString(SafeValue(machineId));
            string relativePath = SafeValue(pendingApiPath);
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                relativePath = "/api/print/pending";
            }

            string separator = relativePath.IndexOf('?') >= 0 ? "&" : "?";
            return BuildUri(apiBaseUrl, relativePath + separator + "machineId=" + safeMachineId).ToString();
        }

        public string BuildPendingUrlFromTemplate(string pendingApiUrlTemplate, string machineId)
        {
            string template = SafeValue(pendingApiUrlTemplate);
            if (string.IsNullOrWhiteSpace(template))
            {
                throw new InvalidOperationException("未配置待打印接口完整地址。");
            }

            Uri uri = new Uri(template, UriKind.Absolute);
            string query = uri.Query;
            string encodedMachineId = Uri.EscapeDataString(SafeValue(machineId));

            if (query.IndexOf("machineId=", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                string updated = System.Text.RegularExpressions.Regex.Replace(
                    query.TrimStart('?'),
                    "(^|&)machineId=[^&]*",
                    "$1machineId=" + encodedMachineId,
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                return uri.GetLeftPart(UriPartial.Path) + "?" + updated;
            }

            string separator = string.IsNullOrWhiteSpace(query) ? "?" : "&";
            return template + separator + "machineId=" + encodedMachineId;
        }

        private static WebClient CreateWebClient()
        {
            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            return webClient;
        }

        private static Uri BuildUri(string baseUrl, string relativeOrAbsolutePath)
        {
            string safeBaseUrl = SafeValue(baseUrl);
            if (string.IsNullOrWhiteSpace(safeBaseUrl))
            {
                throw new InvalidOperationException("未配置接口地址。");
            }

            Uri baseUri = new Uri(AppendTrailingSlash(safeBaseUrl), UriKind.Absolute);
            Uri resultUri;
            if (Uri.TryCreate(relativeOrAbsolutePath, UriKind.Absolute, out resultUri))
            {
                return resultUri;
            }

            string relativePath = SafeValue(relativeOrAbsolutePath).TrimStart('/');
            return new Uri(baseUri, relativePath);
        }

        private static string AppendTrailingSlash(string url)
        {
            return url.EndsWith("/", StringComparison.Ordinal) ? url : url + "/";
        }

        private static string SafeValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }
    }
}
