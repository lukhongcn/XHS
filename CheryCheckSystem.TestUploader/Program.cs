using System;
using System.IO;
using CheryPortHelp;
using Newtonsoft.Json;
using XHS.Model;

namespace CheryCheckSystem.TestUploader
{
    /// <summary>
    /// 奇瑞防错漏平台 checkRecord 接口上传测试工具。
    /// 用法：CheryCheckSystem.TestUploader.exe [JSON文件路径]
    /// 默认路径：..\..\..\..\XHSWorkFlow\Log\F26-3301010JS.json
    /// </summary>
    internal class Program
    {
        private static int Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║   奇瑞防错漏平台 checkRecord 上传测试工具    ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝");
            Console.WriteLine();

            // 1. 确定 JSON 文件路径
            string jsonFilePath = GetJsonFilePath(args);
            Console.WriteLine("[1/4] 读取 JSON 文件：" + jsonFilePath);

            if (!File.Exists(jsonFilePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("错误：文件不存在 → " + jsonFilePath);
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("用法：CheryCheckSystem.TestUploader.exe [JSON文件路径]");
                Console.WriteLine("示例：CheryCheckSystem.TestUploader.exe D:\\project\\XHS\\XHSWorkFlow\\Log\\F26-3301010JS.json");
                Console.ReadKey();
                return 1;
            }

            // 2. 读取并反序列化 JSON
            string jsonContent = File.ReadAllText(jsonFilePath, System.Text.Encoding.UTF8);
            Console.WriteLine("[2/4] JSON 文件大小：" + jsonContent.Length + " 字节");

            CheryCheckRecordRequest request;
            try
            {
                request = JsonConvert.DeserializeObject<CheryCheckRecordRequest>(jsonContent);
                if (request == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("错误：JSON 反序列化结果为 null。");
                    Console.ResetColor();
                    Console.ReadKey();
                    return 2;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("错误：JSON 反序列化失败 → " + ex.Message);
                Console.ResetColor();
                Console.ReadKey();
                return 2;
            }

            // 3. 显示请求摘要
            Console.WriteLine("[3/4] 请求数据摘要：");
            Console.WriteLine("  supplNo        : " + request.supplNo);
            Console.WriteLine("  baseNo         : " + request.baseNo);
            Console.WriteLine("  deliveryType   : " + request.deliveryType);
            Console.WriteLine("  operateType    : " + request.operateType);
            Console.WriteLine("  deliveryNo     : " + request.deliveryNo);
            Console.WriteLine("  sxCardSeq      : " + request.sxCardSeq);
            Console.WriteLine("  materialNo     : " + request.materialNo);
            Console.WriteLine("  materialName   : " + request.materialName);
            Console.WriteLine("  packingCount   : " + request.packingCount);
            Console.WriteLine("  packageType    : " + request.packageType);
            Console.WriteLine("  packageBarCode : " + request.packageBarCode);
            Console.WriteLine("  packageCode    : " + request.packageCode);
            Console.WriteLine("  packageName    : " + request.packageName);
            Console.WriteLine("  packingDate    : " + request.packingDate);
            Console.WriteLine("  checkTime      : " + request.checkTime);
            Console.WriteLine("  checkUserName  : " + request.checkUserName);
            Console.WriteLine("  packingDetails : " + (request.packingDetails != null ? request.packingDetails.Count + " 条明细" : "无"));

            // 4. 发送请求
            Console.WriteLine();
            Console.WriteLine("[4/4] 正在发送到奇瑞平台...");
            Console.WriteLine("  目标地址：" + CheryPortConfig.CheckRecordUrl);
            Console.WriteLine("  AppKey   ：" + CheryPortConfig.AppKey);
            Console.WriteLine();

            var client = new CheryHttpClient();
            var result = client.PostCheckRecordRaw(request);

            // 5. 输出结果
            Console.WriteLine("═══════════════════ 响应结果 ═══════════════════");
            Console.WriteLine("HTTP 状态码：" + result.HttpStatusCode);
            Console.WriteLine("接口返回码 ：" + result.Response?.code);
            Console.WriteLine("接口消息   ：" + result.Response?.msg);
            Console.WriteLine("接口数据   ：" + result.Response?.data);
            Console.WriteLine("──────────────── 调试信息 ──────────────────────");
            Console.WriteLine("签名原文 (appKey+timeStamp+appSecret)：");
            Console.WriteLine("  " + result.OriginalSignText);
            Console.WriteLine("时间戳 (timeStamp)：");
            Console.WriteLine("  " + result.TimeStamp);
            Console.WriteLine("签名 (sign)：");
            Console.WriteLine("  " + result.Sign);
            Console.WriteLine("请求 JSON：");
            Console.WriteLine("  " + Truncate(result.RequestJson, 500));
            Console.WriteLine("响应 JSON：");
            Console.WriteLine("  " + (string.IsNullOrWhiteSpace(result.ResponseJson) ? "(空)" : Truncate(result.ResponseJson, 500)));
            Console.WriteLine("══════════════════════════════════════════════════");

            if (result.HttpStatusCode == 200 && result.Response != null && result.Response.code == 200)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ 上传成功！");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠ 上传可能失败，请检查上述返回信息。");
                Console.ResetColor();
            }

            Console.WriteLine();
            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
            return 0;
        }

        private static string GetJsonFilePath(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                string path = string.Join(" ", args).Trim('"').Trim();
                if (!string.IsNullOrWhiteSpace(path))
                {
                    return Path.GetFullPath(path);
                }
            }

            // 默认路径：项目根目录下的 XHSWorkFlow\Log\F26-3301010JS.json
            string defaultPath = Path.GetFullPath(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "..", "..", "..", "..",
                    "XHSWorkFlow", "Log", "F26-3301010JS.json"));
            return defaultPath;
        }

        private static string Truncate(string text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            text = text.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
            if (text.Length <= maxLength)
            {
                return text;
            }

            return text.Substring(0, maxLength) + "...";
        }
    }
}
