using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace XHS.PlaywrightTests;

public class GSPartMasterListTests : PageTest
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
    public async Task GSPartMasterListPageShowsSearchAndEditControls()
    {
        await Page.GotoAsync($"{BaseUrl}/GSPartMasterList.aspx");

        ILocator editLink = Page.GetByRole(AriaRole.Link, new() { Name = "编辑/edit" });
        await Expect(editLink).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "搜索/search" })).ToBeVisibleAsync();
        await Expect(Page.GetByText("本厂零件编号")).ToBeVisibleAsync();
        await Expect(Page.GetByText("客户零件编号")).ToBeVisibleAsync();

        await editLink.ClickAsync();
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/GSPartMasterUpload.aspx");
    }
}
