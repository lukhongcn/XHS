using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace XHS.PlaywrightTests;

public class DeliveryUploadTests : PageTest
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
    public async Task DeliveryUploadStartsWithScanningEnabledAndUploadDisabled()
    {
        await Page.GotoAsync($"{BaseUrl}/DeliveryUpload.aspx");

        await Expect(Page.Locator("[id$='txt_ScanQRCode']")).ToBeVisibleAsync();
        await Expect(Page.Locator("[id$='btn_upload']")).ToBeDisabledAsync();
    }
}
