using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace XHS.PlaywrightTests;

public class ShippingGoodsViewTests : PageTest
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
    public async Task ShippingGoodsViewPageCanOpen()
    {
        await Page.GotoAsync($"{BaseUrl}/ShippingGoodsView.aspx");

        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "浏览" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "保存/save" })).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("扫描条码")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("二维码内容")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("自动产生配送单号")).ToBeVisibleAsync();
    }

    [Test]
    public async Task AutoDeliveryNoMakesDeliveryNoReadOnly()
    {
        await Page.GotoAsync($"{BaseUrl}/ShippingGoodsView.aspx");

        await Page.GetByLabel("自动产生配送单号").CheckAsync();
        ILocator deliveryNo = Page.GetByLabel("配送单号");
        await Expect(deliveryNo).ToHaveAttributeAsync("readonly", "readonly");
        await Expect(deliveryNo).ToHaveClassAsync(new Regex("shipping-goods-readonly"));
    }
}


