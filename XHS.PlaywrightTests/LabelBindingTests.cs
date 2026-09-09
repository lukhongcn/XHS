using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace XHS.PlaywrightTests;

public class LabelBindingTests : PageTest
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
    public async Task LabelBindingPageRequiresTwoFactoryBarcodesBeforeBinding()
    {
        await Page.GotoAsync($"{BaseUrl}/LabelBinding.aspx");

        await Expect(Page.GetByLabel("本厂条码")).ToBeAttachedAsync();
        await Expect(Page.GetByLabel("本次绑定数量")).ToBeAttachedAsync();
        await Expect(Page.GetByText("本厂工单号", new PageGetByTextOptions { Exact = true })).ToBeAttachedAsync();
        await Expect(Page.GetByText("保存绑定", new PageGetByTextOptions { Exact = true })).ToBeAttachedAsync();
    }

    [Test]
    public async Task LabelBindingPageExposesPendingBindingGridAndClearAction()
    {
        await Page.GotoAsync($"{BaseUrl}/LabelBinding.aspx");

        await Expect(Page.GetByText("客户数量：", new PageGetByTextOptions { Exact = false })).ToBeAttachedAsync();
        await Expect(Page.GetByText("清空", new PageGetByTextOptions { Exact = true })).ToBeAttachedAsync();
        await Expect(Page.GetByLabel("本厂条码")).ToBeAttachedAsync();
    }
}
