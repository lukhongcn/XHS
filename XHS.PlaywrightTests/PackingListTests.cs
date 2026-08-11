using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System.Text.RegularExpressions;

namespace XHS.PlaywrightTests;

public class PackingListTests : PageTest
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
    public async Task PackingListPageCanOpenAndSearch()
    {
        await Page.GotoAsync($"{BaseUrl}/PackingList.aspx");

        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "编辑/edit" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "搜索/search" })).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("KD码")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("零件编号")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("零件码")).ToBeVisibleAsync();
        await Expect(Page.GetByText("提示")).ToBeVisibleAsync();

        ILocator modalMask = Page.Locator("#packingDetailModalMask");
        await Expect(modalMask).ToBeHiddenAsync();

        ILocator detailLinks = Page.Locator(".packing-detail-link");
        if (await detailLinks.CountAsync() > 0)
        {
            await detailLinks.First.ClickAsync();
            await Expect(modalMask).ToHaveClassAsync(new Regex("packing-detail-open"));
            await Expect(Page.Locator(".packing-detail-window")).ToBeVisibleAsync();
            await Expect(Page.Locator("#packingDetailBody")).Not.ToBeEmptyAsync();

            ILocator typeCells = Page.Locator("#packingDetailBody .packing-detail-table tbody tr td:first-child");
            IReadOnlyList<string> types = await typeCells.AllTextContentsAsync();
            if (types.Contains("KD"))
            {
                Assert.That(types.First(), Is.EqualTo("KD"));
            }
            if (types.Contains("随箱码"))
            {
                Assert.That(types.Last(), Is.EqualTo("随箱码"));
            }
        }
    }
}
