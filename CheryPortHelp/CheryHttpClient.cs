using System;
using System.Net.Http;
using System.Text;
using XHS.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CheryPortHelp
{
    /// <summary>
    /// 奇瑞防错漏平台 HTTP 调用客户端。
    /// 当前实现 2.3 checkRecord 出货检测记录上传。
    /// </summary>
    public class CheryHttpClient
    {
        public CheryApiResponse TestConnection()
        {
            var now = DateTime.Now;
            var request = new CheryCheckRecordRequest
            {
                supplNo = "8KK",
                baseNo = "06",
                deliveryType = "1",
                deliveryNo = "TEST_DELIVERY_NO",
                sxCardSeq = "TEST_SX_CARD-1-1",
                materialNo = "TEST_MATERIAL",
                materialName = "接口连接测试物料",
                packingCount = "1",
                packageType = 2,
                packageBarCode = "10#TEST_MATERIAL$11#8KK$31#TEST001$",
                packageCode = "TEST_BOX",
                packageName = "测试包装",
                packingDate = now.ToString("yyyy-MM-dd HH:mm:ss"),
                checkTime = now.ToString("yyyy-MM-dd HH:mm:ss"),
                checkUserName = Environment.UserName
            };

            var appKey = CheryPortConfig.AppKey;
            var appSecret = CheryPortConfig.AppSecret;
            var signType = CheryPortConfig.SignType;
            var url = CheryPortConfig.CheckRecordUrl;
            var timeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            var original = appKey + timeStamp + appSecret;
            var sign = Md5Helper.Md5Upper32(original);
            var requestJson = SerializeRequest(request);

            using (var client = new HttpClient())
            using (var message = new HttpRequestMessage(HttpMethod.Post, url))
            {
                client.Timeout = TimeSpan.FromSeconds(CheryPortConfig.TimeoutSeconds);
                message.Headers.TryAddWithoutValidation("appKey", appKey);
                message.Headers.TryAddWithoutValidation("signType", signType);
                message.Headers.TryAddWithoutValidation("timeStamp", timeStamp);
                message.Headers.TryAddWithoutValidation("sign", sign);
                message.Headers.TryAddWithoutValidation("baseNo", request.baseNo);
                message.Content = CreateJsonContent(requestJson);

                try
                {
                    var httpResponse = client.SendAsync(message).GetAwaiter().GetResult();
                    var responseJson = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        return new CheryApiResponse
                        {
                            code = (int)httpResponse.StatusCode,
                            msg = string.IsNullOrWhiteSpace(responseJson) ? httpResponse.ReasonPhrase : responseJson,
                            data = responseJson
                        };
                    }

                    try
                    {
                        var apiResponse = JsonConvert.DeserializeObject<CheryApiResponse>(responseJson);
                        if (apiResponse != null)
                        {
                            return apiResponse;
                        }

                        return new CheryApiResponse
                        {
                            code = -2,
                            msg = "接口已响应，但返回内容无法解析：" + responseJson,
                            data = responseJson
                        };
                    }
                    catch
                    {
                        return new CheryApiResponse
                        {
                            code = -2,
                            msg = "接口已响应，但返回内容无法解析：" + responseJson,
                            data = responseJson
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new CheryApiResponse
                    {
                        code = -1,
                        msg = ex.Message,
                        data = ex.ToString()
                    };
                }
            }
        }

        /// <summary>
        /// 调用 2.3 checkRecord 接口。
        /// </summary>
        public CheryApiResponse PostCheckRecord(CheryCheckRecordRequest request)
        {
            return PostCheckRecordRaw(request).Response;
        }

        /// <summary>
        /// 调用 2.3 checkRecord 接口，并返回调试信息。
        /// 测试按钮可以用这个方法显示请求 JSON、timeStamp、sign。
        /// </summary>
        public CheryPostResult PostCheckRecordRaw(CheryCheckRecordRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var appKey = CheryPortConfig.AppKey;
            var appSecret = CheryPortConfig.AppSecret;
            var signType = CheryPortConfig.SignType;
            var url = CheryPortConfig.CheckRecordUrl;

            var timeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            var original = appKey + timeStamp + appSecret;
            var sign = Md5Helper.Md5Upper32(original);
            var requestJson = SerializeRequest(request);

            using (var client = new HttpClient())
            using (var message = new HttpRequestMessage(HttpMethod.Post, url))
            {
                client.Timeout = TimeSpan.FromSeconds(CheryPortConfig.TimeoutSeconds);

                // 文档要求的统一 Header。
                message.Headers.TryAddWithoutValidation("appKey", appKey);
                message.Headers.TryAddWithoutValidation("timeStamp", timeStamp);
                message.Headers.TryAddWithoutValidation("sign", sign);
                message.Headers.TryAddWithoutValidation("signType", signType);

                // 文档示例中额外放了 baseNo header；2.3 body 中也有 baseNo。
                if (CheryPortConfig.AddBaseNoHeader && !string.IsNullOrWhiteSpace(request.baseNo))
                {
                    message.Headers.TryAddWithoutValidation("baseNo", request.baseNo);
                }

                message.Content = CreateJsonContent(requestJson);

                try
                {
                    var httpResponse = client.SendAsync(message).GetAwaiter().GetResult();
                    var responseJson = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    CheryApiResponse apiResponse;
                    if (string.IsNullOrWhiteSpace(responseJson))
                    {
                        apiResponse = new CheryApiResponse
                        {
                            code = (int)httpResponse.StatusCode,
                            msg = string.IsNullOrWhiteSpace(httpResponse.ReasonPhrase) ? "接口返回内容为空" : httpResponse.ReasonPhrase,
                            data = null
                        };
                    }
                    else
                    {
                        try
                        {
                            apiResponse = JsonConvert.DeserializeObject<CheryApiResponse>(responseJson);
                            if (apiResponse == null)
                            {
                                apiResponse = new CheryApiResponse
                                {
                                    code = -2,
                                    msg = responseJson,
                                    data = responseJson
                                };
                            }
                        }
                        catch (Exception)
                        {
                            apiResponse = new CheryApiResponse
                            {
                                code = -2,
                                msg = responseJson,
                                data = responseJson
                            };
                        }
                    }

                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        apiResponse.code = (int)httpResponse.StatusCode;
                        apiResponse.msg = string.IsNullOrWhiteSpace(responseJson) ? httpResponse.ReasonPhrase : responseJson;
                    }

                    return new CheryPostResult
                    {
                        Url = url,
                        RequestJson = requestJson,
                        TimeStamp = timeStamp,
                        OriginalSignText = original,
                        Sign = sign,
                        ResponseJson = responseJson,
                        HttpStatusCode = (int)httpResponse.StatusCode,
                        Response = apiResponse
                    };
                }
                catch (Exception ex)
                {
                    return new CheryPostResult
                    {
                        Url = url,
                        RequestJson = requestJson,
                        TimeStamp = timeStamp,
                        OriginalSignText = original,
                        Sign = sign,
                        ResponseJson = null,
                        HttpStatusCode = 0,
                        Response = new CheryApiResponse
                        {
                            code = -1,
                            msg = "HTTP POST请求异常：" + ex.Message,
                            data = ex.ToString()
                        }
                    };
                }
            }
        }

        private static StringContent CreateJsonContent(string requestJson)
        {
            // StringContent 的第三个参数会生成 Content-Type: application/json; charset=utf-8。
            // 文档写的是 application/json;charset=UTF8，这里再把 CharSet 改成 UTF8，尽量贴近文档。
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
            content.Headers.ContentType.CharSet = "UTF8";
            return content;
        }

        private static string SerializeRequest(CheryCheckRecordRequest request)
        {
            return JsonConvert.SerializeObject(
                request,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }
    }

    /// <summary>
    /// 接口调用调试结果。正式业务可只使用 Response。
    /// </summary>
    public class CheryPostResult
    {
        public string Url { get; set; }
        public string RequestJson { get; set; }
        public string TimeStamp { get; set; }
        public string OriginalSignText { get; set; }
        public string Sign { get; set; }
        public string ResponseJson { get; set; }
        public int HttpStatusCode { get; set; }
        public CheryApiResponse Response { get; set; }
    }
}
