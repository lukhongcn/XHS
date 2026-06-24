using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace XHS.PlaywrightTests;

public class ShippingGoodsViewTests : PageTest
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
    public async Task ShippingGoodsViewPageCanOpen()
    {
        await Page.GotoAsync($"{BaseUrl}/ShippingGoodsView.aspx");

        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "浏览" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "保存/save" })).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("扫描条码")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("二维码内容")).ToBeVisibleAsync();
    }
}


