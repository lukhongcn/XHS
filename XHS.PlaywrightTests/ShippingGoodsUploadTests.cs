using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace XHS.PlaywrightTests;

public class ShippingGoodsUploadTests : PageTest
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
    public async Task ShippingGoodsUploadPageCanOpen()
    {
        await Page.GotoAsync($"{BaseUrl}/ShippingGoodsUpload.aspx");

        await Expect(Page.Locator("#dropArea")).ToBeVisibleAsync();
        await Expect(Page.GetByText("拖拉出货单到这里")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "上传" })).ToBeVisibleAsync();
    }
}
