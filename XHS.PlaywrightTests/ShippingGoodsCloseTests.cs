using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace XHS.PlaywrightTests;

public class ShippingGoodsCloseTests : PageTest
{
    private static string BaseUrl
    {
        get
        {
            string? value = Environment.GetEnvironmentVariable("XHS_BASE_URL");
            return string.IsNullOrWhiteSpace(value) ? "http://localhost/XHS" : value.TrimEnd('/');
        }
    }

    [Test]
    public async Task ShippingGoodsClosePageCanOpen()
    {
        await Page.GotoAsync($"{BaseUrl}/ShippingGoodsClose.aspx");

        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "浏览" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "保存/save" })).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("批次")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("零件编号")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("配送单号")).ToBeVisibleAsync();
        await Expect(Page.GetByText("提示")).ToBeVisibleAsync();
    }
}
