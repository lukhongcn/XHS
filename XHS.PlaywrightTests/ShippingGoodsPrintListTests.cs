using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace XHS.PlaywrightTests;

public class ShippingGoodsPrintListTests : PageTest
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
    public async Task ShippingGoodsPrintListPageCanOpen()
    {
        await Page.GotoAsync($"{BaseUrl}/ShippingGoodsPrintList.aspx");

        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "打印/print" })).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("零件编号")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("零件名称")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("批次")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("全部列出")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShippingGoodsREPrintListPageCanOpen()
    {
        await Page.GotoAsync($"{BaseUrl}/ShippingGoodsREPrintList.aspx");

        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "补打/reprint" })).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("零件编号")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("零件名称")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("批次")).ToBeVisibleAsync();
        await Expect(Page.GetByText("补打原因")).ToBeVisibleAsync();
    }
}
