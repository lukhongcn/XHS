using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace XHS.PlaywrightTests;

public class UserViewTests : PageTest
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
    public async Task UserViewPageContainsMasterMessageModal()
    {
        await Page.GotoAsync($"{BaseUrl}/admin/UserView.aspx");

        await Expect(Page.Locator("#xhsMessageModalMask")).ToBeAttachedAsync();
        await Expect(Page.Locator("#xhsMessageModalBody")).ToBeAttachedAsync();
        await Expect(Page.Locator("#val_username")).ToBeAttachedAsync();
        await Expect(Page.Locator("#Label_Message")).ToBeAttachedAsync();
    }
}
