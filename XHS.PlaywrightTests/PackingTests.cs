using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace XHS.PlaywrightTests;

public class PackingTests : PageTest
{
    private static string BaseUrl
    {
        get
        {
            string? value = Environment.GetEnvironmentVariable("XHS_BASE_URL");
            return string.IsNullOrWhiteSpace(value) ? "http://localhost:55426" : value.TrimEnd('/');
        }
    }

    [Test]
    public async Task PackingPageHasTwoStageRepackControls()
    {
        string? taskId = Environment.GetEnvironmentVariable("XHS_PACKING_TASK_ID");
        string url = string.IsNullOrWhiteSpace(taskId)
            ? $"{BaseUrl}/Packing.aspx"
            : $"{BaseUrl}/Packing.aspx?taskId={Uri.EscapeDataString(taskId)}";

        await Page.GotoAsync(url);
        if (Page.Url.Contains("login.aspx", StringComparison.OrdinalIgnoreCase))
        {
            Assert.Ignore("当前测试会话尚未登录，跳过装箱页面交互测试。");
        }

        ILocator modal = Page.Locator("[id$='pnlRepackModal']");
        Assert.That(await Page.EvaluateAsync<bool>("typeof startPackingStatusPolling === 'function'"), Is.True);
        await Expect(modal).ToBeHiddenAsync();

        ILocator approvedRepackLinks = Page.GetByRole(AriaRole.Link, new() { Name = "重新装箱", Exact = true });
        if (await approvedRepackLinks.CountAsync() > 0)
        {
            await approvedRepackLinks.First.ClickAsync();
        }
        else
        {
            await Page.EvaluateAsync("showPackingRepackModal()");
        }
        await Expect(modal).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("产品二维码")).ToBeVisibleAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "取消" }).ClickAsync();
        await Expect(modal).ToBeHiddenAsync();

        ILocator repackLinks = Page.Locator("a[id$='lnk_Repack']");
        for (int index = 0; index < await repackLinks.CountAsync(); index++)
        {
            string text = (await repackLinks.Nth(index).InnerTextAsync()).Trim();
            Assert.That(text, Is.EqualTo("申请").Or.EqualTo("重新装箱"));
        }
    }
}
