using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace XHS.PlaywrightTests;

/// <summary>
/// 标签绑定完整流程测试：
/// 登录页登录 → LabelBindingPDA 三步扫描（客户标签 → 上方工单码 → 本厂标签）→
/// LabelBindingRecordList 验证最近 7 天记录及栏位。
/// 只读测试：只新增一条绑定记录，不删除、不清空任何数据。
/// </summary>
[NonParallelizable]
public class LabelBindingFullFlowTests : PageTest
{
    private static string BaseUrl
    {
        get
        {
            string? value = Environment.GetEnvironmentVariable("XHS_BASE_URL");
            return string.IsNullOrWhiteSpace(value) ? "http://localhost/XHS" : value.TrimEnd('/');
        }
    }

    private const string LoginUserName = "admin";
    private const string LoginPassword = "123456";

    private const string CustomerLabel = "P2026061705592/CAQPL/5401995XNY03A02&120&EA&260602";
    private const string WorkOrderCode = "5104-20260702020";
    private const string FactoryLabel = "26J073|260602|120";

    private static readonly string ExpectedCustomerProductNo = "5401995XNY03A02";
    private static readonly string ExpectedProductNo = "26J073";

    [Test]
    public async Task FullBindingFlow_Login_PdaScan_RecordList()
    {
        string currentStep = "初始化";
        try
        {
            // ---------- 1. 打开登录页并登录 ----------
            currentStep = "步骤1：打开登录页并登录";
            Step(currentStep);
            var loginResponse = await Page.GotoAsync($"{BaseUrl}/login.aspx");
            Assert.That(loginResponse, Is.Not.Null, "登录页响应为空");
            Assert.That(loginResponse!.Status, Is.EqualTo(200), "登录页未返回 200");
            await Expect(Page.Locator("#TextBox_UserName")).ToBeVisibleAsync();
            await Expect(Page.Locator("#Button_Login")).ToBeVisibleAsync();

            await Page.Locator("#TextBox_UserName").FillAsync(LoginUserName);
            await Page.Locator("#HTML_Password").FillAsync(LoginPassword);
            await Page.Locator("#Button_Login").ClickAsync();
            // 等待离开登录页（登录成功后跳转）
            await Expect(Page).Not.ToHaveURLAsync(new Regex(@"login\.aspx"));
            StepOk("登录成功，当前 URL: " + Page.Url);
            // 登录后落地的 default.aspx 是 frameset（main frame src 为空导致递归加载，
            // load 事件永不触发），先停止当前加载再导航到 PDA 页面。
            await Page.EvaluateAsync("window.stop()");
            StepOk("已停止 default.aspx 的递归框架加载");

            // ---------- 2. 进入 LabelBindingPDA.aspx ----------
            currentStep = "步骤2：进入 LabelBindingPDA.aspx";
            var pdaResponse = await Page.GotoAsync($"{BaseUrl}/LabelBindingPDA.aspx");
            Assert.That(pdaResponse, Is.Not.Null);
            Assert.That(pdaResponse!.Status, Is.EqualTo(200), "PDA 页面未返回 200");
            StepOk("PDA 页面已打开");

            // ---------- 3. 确认初始状态：请扫描客户标签、扫描框存在且自动聚焦 ----------
            currentStep = "步骤3：确认初始状态（请扫描客户标签 / 扫描框聚焦）";
            var stateLabel = Page.Locator("[id$='_labScanState']");
            var scanner = Page.Locator("[id$='_txtPdaScan']");
            await Expect(stateLabel).ToHaveTextAsync("请扫描客户标签");
            await Expect(scanner).ToBeAttachedAsync();
            await Expect(scanner).ToBeEnabledAsync();
            await Expect(scanner).ToBeFocusedAsync();
            string? scanId = await scanner.GetAttributeAsync("id");
            StepOk($"初始状态正确：显示“请扫描客户标签”，扫描框 {scanId ?? "(未知)"} 存在且已获得焦点");

            // ---------- 4. 扫描客户标签 ----------
            currentStep = "步骤4：扫描客户标签";
            await scanner.FillAsync(CustomerLabel);
            await scanner.PressAsync("Enter");

            // ---------- 5. 确认进入“上方工单码”步骤 ----------
            currentStep = "步骤5：确认进入“上方工单码”步骤";
            await Expect(stateLabel).ToHaveTextAsync("请扫描上方工单码");
            StepOk("已进入“上方工单码”步骤，当前状态：请扫描上方工单码");

            // ---------- 6. 扫描工单 ----------
            currentStep = "步骤6：扫描工单";
            await scanner.FillAsync(WorkOrderCode);
            await scanner.PressAsync("Enter");

            // ---------- 7. 确认进入“本厂标签”步骤 ----------
            currentStep = "步骤7：确认进入“本厂标签”步骤";
            await Expect(stateLabel).ToHaveTextAsync("请扫描本厂标签");
            StepOk("已进入“本厂标签”步骤，当前状态：请扫描本厂标签");

            // ---------- 8. 扫描本厂标签 ----------
            currentStep = "步骤8：扫描本厂标签";
            await scanner.FillAsync(FactoryLabel);
            await scanner.PressAsync("Enter");

            // ---------- 9. 确认绑定完成、扫描框禁用 ----------
            currentStep = "步骤9：确认绑定完成";
            await Expect(stateLabel).ToContainTextAsync("已完成");
            await Expect(scanner).ToBeDisabledAsync();
            string finalStateText = (await stateLabel.InnerTextAsync()).Trim();
            string messageText = (await Page.Locator("[id$='_labMessage']").InnerTextAsync()).Trim();
            StepOk($"绑定完成：状态={finalStateText}；消息={messageText}；扫描框已禁用");

            // ---------- 10. 打开 LabelBindingRecordList.aspx ----------
            currentStep = "步骤10：打开 LabelBindingRecordList.aspx";
            var listResponse = await Page.GotoAsync($"{BaseUrl}/LabelBindingRecordList.aspx");
            Assert.That(listResponse, Is.Not.Null);
            Assert.That(listResponse!.Status, Is.EqualTo(200), "记录列表页未返回 200");
            await Expect(Page.Locator("[id$='_MainDataGrid']")).ToBeAttachedAsync();
            StepOk("记录列表页已打开，网格已加载");

            // ---------- 11. 确认最近 7 天日期范围 ----------
            currentStep = "步骤11：确认最近 7 天日期范围";
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            string start = DateTime.Today.AddDays(-6).ToString("yyyy-MM-dd");
            await Expect(Page.Locator("[id$='_TextBox_StartTime']")).ToHaveValueAsync(start);
            await Expect(Page.Locator("[id$='_TextBox_EndTime']")).ToHaveValueAsync(today);
            await Expect(Page.Locator("[id$='_Label_Message']")).ToContainTextAsync("绑定记录");
            StepOk($"日期范围正确：{start} ~ {today}（最近 7 天）");

            // ---------- 12. 在列表中定位本次绑定记录并校验栏位 ----------
            currentStep = "步骤12：校验绑定记录栏位";
            string[] rowCells = await FindRecordRowAsync(CustomerLabel);
            Assert.That(rowCells.Length, Is.GreaterThanOrEqualTo(8), "未找到刚产生的绑定记录或列数不足");

            string[] headers = { "客户标签扫描时间", "扫描人", "客户标签", "本厂标签", "上方工单码", "客户产品编号", "产品编号", "扫描状态" };
            for (int i = 0; i < headers.Length; i++)
            {
                Step($"{headers[i]} = {rowCells[i]}");
            }

            Assert.That(rowCells[0], Is.Not.Empty, "客户标签扫描时间为空");
            Assert.That(rowCells[1], Is.EqualTo(LoginUserName), "扫描人不符");
            Assert.That(rowCells[2], Is.EqualTo(CustomerLabel), "客户标签不符");
            Assert.That(rowCells[3], Is.EqualTo(FactoryLabel), "本厂标签不符");
            Assert.That(rowCells[4], Is.EqualTo(WorkOrderCode), "上方工单码不符");
            Assert.That(rowCells[5], Is.EqualTo(ExpectedCustomerProductNo), "客户产品编号不符");
            Assert.That(rowCells[6], Is.EqualTo(ExpectedProductNo), "产品编号不符");
            StringAssert.Contains("已完成", rowCells[7], "扫描状态不符");
            StepOk("绑定记录全部栏位校验通过");

            TestContext.Progress.WriteLine("[SUMMARY] 全部 12 个步骤执行成功。");
        }
        catch (Exception ex)
        {
            await CaptureFailureAsync(currentStep, ex);
            throw;
        }
    }

    /// <summary>
    /// 在 MainDataGrid 中查找“客户标签”列等于指定值的记录行，返回该行各单元格文本。
    /// 列表按客户标签扫描时间倒序，取最新匹配的一行。
    /// </summary>
    private async Task<string[]> FindRecordRowAsync(string customerLabel)
    {
        ILocator rows = Page.Locator("[id$='_MainDataGrid'] tr");
        int count = await rows.CountAsync();
        for (int index = 0; index < count; index++)
        {
            ILocator cells = rows.Nth(index).Locator("td");
            int cellCount = await cells.CountAsync();
            if (cellCount < 8) continue; // 跳过表头/分页行
            string[] values = new string[cellCount];
            for (int c = 0; c < cellCount; c++)
            {
                values[c] = (await cells.Nth(c).InnerTextAsync()).Trim();
            }
            if (string.Equals(values[2], customerLabel, StringComparison.Ordinal))
            {
                return values;
            }
        }
        return Array.Empty<string>();
    }

    private void Step(string text) => TestContext.Progress.WriteLine($"[步骤] {text}");
    private void StepOk(string text) => TestContext.Progress.WriteLine($"[OK] {text}");

    /// <summary>
    /// 失败时记录失败步骤、当前 URL、页面可见文本，并保存整页截图。
    /// </summary>
    private async Task CaptureFailureAsync(string step, Exception ex)
    {
        TestContext.Error.WriteLine("===== 测试失败诊断信息 =====");
        TestContext.Error.WriteLine($"失败步骤: {step}");
        TestContext.Error.WriteLine($"异常: {ex}");
        try
        {
            TestContext.Error.WriteLine($"当前 URL: {Page.Url}");
            string bodyText = await Page.Locator("body").InnerTextAsync(new LocatorInnerTextOptions { Timeout = 5000 });
            TestContext.Error.WriteLine($"页面可见文本: {bodyText}");
            string dir = Path.Combine(AppContext.BaseDirectory, "test-results");
            Directory.CreateDirectory(dir);
            string fileName = $"failure_{DateTime.Now:yyyyMMdd_HHmmss}_{Sanitize(step)}.png";
            string shot = Path.Combine(dir, fileName);
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = shot, FullPage = true });
            TestContext.Error.WriteLine($"截图已保存: {shot}");
        }
        catch (Exception inner)
        {
            TestContext.Error.WriteLine($"收集失败诊断信息时出错: {inner}");
        }
        TestContext.Error.WriteLine("============================");
    }

    private static string Sanitize(string text)
    {
        string safe = Regex.Replace(text ?? "unknown", @"[^\w\u4e00-\u9fa5-]", "_");
        return safe.Length > 60 ? safe[..60] : safe;
    }
}
