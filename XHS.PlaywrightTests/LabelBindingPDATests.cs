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

    [Test]
    public async Task PdaWorkflowSavesBindingRecordAndRecordListLoads()
    {
        await Page.GotoAsync($"{BaseUrl}/LabelBindingPDA.aspx");
        await LoginAsPdaAdmin();

        var scanner = Page.Locator("[id$='_txtPdaScan']");
        var state = Page.Locator("[id$='_labScanState']");

        await scanner.FillAsync("P2026061705592/CAQPL/5401995XNY03A02&120&EA&260602");
        await scanner.PressAsync("Enter");
        await Expect(state).ToContainTextAsync("工单");

        await scanner.FillAsync("5104-20260702020");
        await scanner.PressAsync("Enter");
        await Expect(state).ToContainTextAsync("本厂");

        await scanner.FillAsync("26J073|260602|120");
        await scanner.PressAsync("Enter");
        await Expect(Page.Locator("[id$='_txtPdaScan']")).ToBeDisabledAsync();

        await Page.GotoAsync($"{BaseUrl}/LabelBindingRecordList.aspx");
        await Expect(Page.Locator("[id$='_MainDataGrid']")).ToBeAttachedAsync();
        await Expect(Page.Locator("[id$='_Label_Message']")).ToContainTextAsync("绑定记录");
    }

    [Test]
    public async Task PdaRepeatedWorkOrderScanKeepsSeparateBindingRows()
    {
        await Page.GotoAsync($"{BaseUrl}/Login.aspx");
        await Page.Locator("[id$='TextBox_UserName']").FillAsync("admin");
        await Page.Locator("[id$='HTML_Password']").FillAsync("123456");
        await Page.Locator("[id$='Button_Login']").ClickAsync();

        await Page.GotoAsync($"{BaseUrl}/LabelBindingPDA.aspx");

        var scanner = Page.Locator("[id$='_txtPdaScan']");
        var gridRows = Page.Locator("[id$='_gvBindingRecords'] tr");

        await scanner.FillAsync("5103-20280721003");
        await scanner.PressAsync("Enter");
        await Expect(gridRows).ToHaveCountAsync(2);

        await scanner.FillAsync("5103-20280721004");
        await scanner.PressAsync("Enter");
        await Expect(gridRows).ToHaveCountAsync(3);
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
