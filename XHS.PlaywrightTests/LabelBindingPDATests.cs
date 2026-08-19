using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace XHS.PlaywrightTests;

public class LabelBindingPDATests : PageTest
{
    private static string BaseUrl => (Environment.GetEnvironmentVariable("XHS_BASE_URL") ?? "http://localhost/XHS").TrimEnd('/');

    [Test]
    public async Task PdaPageLoadsWithScannerControlsAndNoHorizontalOverflow()
    {
        await Page.SetViewportSizeAsync(360, 640);
        var response = await Page.GotoAsync($"{BaseUrl}/LabelBindingPDA.aspx");

        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Status, Is.EqualTo(200));
        await Expect(Page.GetByRole(AriaRole.Dialog, new() { Name = "PDA 登录" })).ToBeVisibleAsync();
        await LoginAsPdaAdmin();
        await Expect(Page.Locator("[id$='_txtPdaScan']")).ToBeAttachedAsync();
        await Expect(Page.Locator("[id$='_labScanState']")).ToHaveTextAsync("请扫描客户标签");
        await Expect(Page.Locator("[id$='_btnClear']")).ToBeAttachedAsync();

        var overflow = await Page.EvaluateAsync<bool>("document.documentElement.scrollWidth > document.documentElement.clientWidth");
        Assert.That(overflow, Is.False);
    }

    [Test]
    public async Task PdaPageSupportsTallViewportAndEnterHandlers()
    {
        await Page.SetViewportSizeAsync(480, 800);
        await Page.GotoAsync($"{BaseUrl}/LabelBindingPDA.aspx");
        await LoginAsPdaAdmin();

        var scanner = Page.Locator("[id$='_txtPdaScan']");

        Assert.That(await scanner.GetAttributeAsync("autocomplete"), Is.EqualTo("off"));
        Assert.That(await scanner.GetAttributeAsync("autocorrect"), Is.EqualTo("off"));

        var script = await Page.Locator("script").AllTextContentsAsync();
        var combined = string.Join("\n", script);
        StringAssert.Contains("keyCode", combined);
        StringAssert.Contains("13", combined);
        StringAssert.Contains("__doPostBack", combined);
        StringAssert.Contains("add_endRequest", combined);
    }

    private async Task LoginAsPdaAdmin()
    {
        var dialog = Page.GetByRole(AriaRole.Dialog, new() { Name = "PDA 登录" });
        if (!await dialog.IsVisibleAsync()) return;

        await dialog.GetByRole(AriaRole.Textbox, new() { Name = "员工编号" }).FillAsync("admin");
        await dialog.GetByRole(AriaRole.Textbox, new() { Name = "密码" }).FillAsync("123456");
        await dialog.GetByRole(AriaRole.Button, new() { Name = "登录" }).ClickAsync();
        await Expect(dialog).ToBeHiddenAsync();
    }
}
