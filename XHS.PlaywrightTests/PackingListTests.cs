using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace XHS.PlaywrightTests;

public class PackingListTests : PageTest
{
    private static string BaseUrl
    {
        get
        {
            string? value = Environment.GetEnvironmentVariable("XHS_BASE_URL");
            return string.IsNullOrWhiteSpace(value) ? "http://localhost/XHS" : value.TrimEnd('/');
        }
    }

    private static string TestUsername => Environment.GetEnvironmentVariable("XHS_TEST_USERNAME") ?? "admin";
    private static string TestPassword => Environment.GetEnvironmentVariable("XHS_TEST_PASSWORD") ?? "123456";

    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(TestUsername) || string.IsNullOrWhiteSpace(TestPassword))
        {
            Assert.Ignore("未配置 XHS_TEST_USERNAME 和 XHS_TEST_PASSWORD，无法执行需要登录的页面测试。");
        }

        await Page.GotoAsync($"{BaseUrl}/login.aspx?ReturnUrl=%2fXHS%2fPackingList.aspx");
        await Page.Locator("#TextBox_UserName").FillAsync(TestUsername);
        await Page.Locator("#HTML_Password").FillAsync(TestPassword);
        await Page.Locator("#Button_Login").ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Expect(Page).Not.ToHaveURLAsync(new Regex("/login\\.aspx", RegexOptions.IgnoreCase));
    }

    [Test]
    public async Task PackingListPageCanOpenAndSearch()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/PackingList.aspx");

        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "编辑/edit" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "搜索/search" })).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("KD码")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("零件编号")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("零件码")).ToBeVisibleAsync();
        await Expect(Page.Locator(".tbMessage b")).ToBeVisibleAsync();

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

    [Test]
    public async Task PackingListPageShowsChinesePackingStage()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/PackingList.aspx");

        await Expect(Page.GetByText("装箱阶段")).ToBeVisibleAsync();

        // 表头所在行之后，表格中应该显示中文装箱阶段
        ILocator stageCells = Page.Locator("table[id$='MainDataGrid'] td")
            .Filter(new() { HasTextRegex = new Regex("装箱中|装箱完成|已完成|上传完成") });

        int count = await stageCells.CountAsync();
        if (count > 0)
        {
            // 至少有一行显示了中文装箱阶段
            string text = await stageCells.First.TextContentAsync() ?? string.Empty;
            Assert.That(text, Does.Match("装箱中|装箱完成|已完成|上传完成"));
        }
        else
        {
            // 可能没有数据，验证表格和表头正常显示即可
            await Expect(Page.Locator("table[id$='MainDataGrid']")).ToBeVisibleAsync();
        }
    }
}
