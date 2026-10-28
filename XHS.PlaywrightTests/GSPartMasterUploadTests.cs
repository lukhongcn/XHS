using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace XHS.PlaywrightTests;

public class GSPartMasterUploadTests : PageTest
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
    public async Task GSPartMasterUploadPageCanOpen()
    {
        await Page.GotoAsync($"{BaseUrl}/GSPartMasterUpload.aspx");

        await Expect(Page.GetByText("拖拉光板标签整理 Excel 到这里")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "上传" })).ToBeVisibleAsync();
        await Expect(Page.Locator("input[type=file]")).ToBeAttachedAsync();
    }
}
