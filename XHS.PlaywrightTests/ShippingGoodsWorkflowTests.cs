using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace XHS.PlaywrightTests;

/// <summary>
/// 出货资料维护端到端测试，对应「测试.md」的测试一 ~ 测试七。
/// 覆盖 ShippingGoodsView（新增/保存/浏览）、ShippingGoodsPrintList（KD 标签打印）。
/// 运行前通过 XHS_BASE_URL 指定目标地址，例如 http://192.168.102.10/xhs。
/// </summary>
public class ShippingGoodsWorkflowTests : PageTest
{
    private static string BaseUrl
    {
        get
        {
            string? value = Environment.GetEnvironmentVariable("XHS_BASE_URL");
            return string.IsNullOrWhiteSpace(value) ? "http://localhost/XHS" : value.TrimEnd('/');
        }
    }

    // 测试.md 公共测试资料
    private const string PartChineseName = "左拉杆总成";
    private const string QrCodeBatch0801 = "10#202004112AA$11#3051$17#6$18#1020MO-CS0720260801$21#SKD1296426081307941-10-1$22#ZF400300100$23#1020MO-CS0720260801$";
    private const string QrCodeBatch0802 = "10#202004112AA$11#3051$17#6$18#1020MO-CS0720260802$21#SKD1296426081310362-10-1$22#ZF400300100$23#1020MO-CS0720260802$";
    private const string DeliveryNo016 = "X260813120002016";
    private const string DeliveryNo209 = "X260813120002209";
    private const string DeliveryNo210 = "X260813120002210";
    private const string TestDate = "2026-08-14T08:00";

    [SetUp]
    public async Task Login()
    {
        await Page.GotoAsync($"{BaseUrl}/login.aspx");
        await Page.Locator("#TextBox_UserName").FillAsync("admin");
        await Page.Locator("#HTML_Password").FillAsync("123456");
        await Page.Locator("#Button_Login").ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Expect(Page).Not.ToHaveURLAsync(new Regex("login\\.aspx", RegexOptions.IgnoreCase));
    }

    private async Task ScanBarcodeAsync(string qrCode)
    {
        ILocator barcode = Page.GetByLabel("扫描条码");
        await barcode.FillAsync(qrCode);
        // 回车触发 onkeydown -> __doPostBack，执行 txt_barcode_TextChanged 解析
        await barcode.PressAsync("Enter");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        // 解析完成后页面会弹出“条码解析完成”提示，先关闭
        await CloseModalAsync();
    }

    private async Task SaveAsync()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "保存/save" }).ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    private async Task CloseModalAsync()
    {
        ILocator mask = Page.Locator("#xhsMessageModalMask");
        await Expect(mask).ToBeVisibleAsync();
        await mask.Locator("button").ClickAsync();
        await Expect(mask).ToBeHiddenAsync();
    }

    [Test, Order(1)]
    public async Task 测试一_配送单号必填校验()
    {
        await Page.GotoAsync($"{BaseUrl}/ShippingGoodsView.aspx");
        await ScanBarcodeAsync(QrCodeBatch0801);

        await Page.GetByLabel("零件中文名称").FillAsync(PartChineseName);
        await Page.GetByLabel("纸箱数量").FillAsync("20");
        await Page.GetByLabel("单箱毛重").FillAsync("7.4");

        // 第一次保存：缺少配送单号、生产日期、检验确认日期
        await SaveAsync();
        ILocator modal = Page.Locator("#xhsMessageModalBody");
        await Expect(modal).ToContainTextAsync("配送单号");
        await Expect(modal).ToContainTextAsync("生产日期");
        await Expect(modal).ToContainTextAsync("检验确认日期");
        await CloseModalAsync();

        // 补充生产日期、检验确认日期后再次保存：只剩配送单号
        await Page.GetByLabel("生产日期").FillAsync(TestDate);
        await Page.GetByLabel("检验确认日期").FillAsync(TestDate);
        await SaveAsync();
        await Expect(modal).ToContainTextAsync("配送单号");
        await Expect(modal).Not.ToContainTextAsync("生产日期");
        await CloseModalAsync();
    }

    [Test, Order(2)]
    public async Task 测试二_正常保存并浏览()
    {
        await Page.GotoAsync($"{BaseUrl}/ShippingGoodsView.aspx");
        await ScanBarcodeAsync(QrCodeBatch0801);

        await Page.GetByLabel("零件中文名称").FillAsync(PartChineseName);
        await Page.GetByLabel("配送单号").FillAsync(DeliveryNo016);
        await Page.GetByLabel("纸箱数量").FillAsync("20");
        await Page.GetByLabel("单箱毛重").FillAsync("7.4");
        await Page.GetByLabel("生产日期").FillAsync(TestDate);
        await Page.GetByLabel("检验确认日期").FillAsync(TestDate);

        await SaveAsync();
        await Expect(Page.Locator("#xhsMessageModalBody")).ToContainTextAsync("保存成功，共生成 20 条数据。");
        await CloseModalAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "浏览" }).ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        Assert.That(Page.Url, Does.Contain("ShippingGoodsList.aspx"));
        await Expect(Page.Locator("span[id$='Label_Message']")).ToContainTextAsync("共查询到 20 条出货货品数据。");
    }

    [Test, Order(3)]
    public async Task 测试三_新批次正常保存()
    {
        await Page.GotoAsync($"{BaseUrl}/ShippingGoodsView.aspx");
        await ScanBarcodeAsync(QrCodeBatch0802);

        // 零件名称由系统自动带出
        await Expect(Page.GetByLabel("零件中文名称")).ToHaveValueAsync(PartChineseName);

        await Page.GetByLabel("配送单号").FillAsync(DeliveryNo016);
        await Page.GetByLabel("纸箱数量").FillAsync("10");
        await Page.GetByLabel("单箱毛重").FillAsync("7.4");
        await Page.GetByLabel("生产日期").FillAsync(TestDate);
        await Page.GetByLabel("检验确认日期").FillAsync(TestDate);

        await SaveAsync();
        await Expect(Page.Locator("#xhsMessageModalBody")).ToContainTextAsync("保存成功，共生成 10 条数据。");
        await CloseModalAsync();
    }

    [Test, Order(4)]
    public async Task 测试四_更换配送单号()
    {
        await Page.GotoAsync($"{BaseUrl}/ShippingGoodsView.aspx");
        await ScanBarcodeAsync(QrCodeBatch0802);

        await Page.GetByLabel("配送单号").FillAsync(DeliveryNo209);
        await Page.GetByLabel("纸箱数量").FillAsync("10");
        await Page.GetByLabel("单箱毛重").FillAsync("7.4");
        await Page.GetByLabel("生产日期").FillAsync(TestDate);
        await Page.GetByLabel("检验确认日期").FillAsync(TestDate);

        await SaveAsync();
        await Expect(Page.Locator("#xhsMessageModalBody")).ToContainTextAsync("保存成功，共生成 10 条数据。");
        await CloseModalAsync();
    }

    [Test, Order(5)]
    public async Task 测试五_重复数据校验()
    {
        await Page.GotoAsync($"{BaseUrl}/ShippingGoodsView.aspx");
        await ScanBarcodeAsync(QrCodeBatch0802);

        await Page.GetByLabel("配送单号").FillAsync(DeliveryNo209);
        await Page.GetByLabel("纸箱数量").FillAsync("10");
        await Page.GetByLabel("单箱毛重").FillAsync("7.4");
        await Page.GetByLabel("生产日期").FillAsync(TestDate);
        await Page.GetByLabel("检验确认日期").FillAsync(TestDate);

        await SaveAsync();
        await Expect(Page.Locator("#xhsMessageModalBody")).ToContainTextAsync("出货货品已存在");
        await CloseModalAsync();
    }

    [Test, Order(6)]
    public async Task 测试六_KD标签批量打印()
    {
        await Page.GotoAsync($"{BaseUrl}/ShippingGoodsPrintList.aspx");

        // 勾选“全部列出”以显示全部 40 条未打印记录
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "全部列出" }).CheckAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // 表头全选
        await Page.Locator("input[id$='checkall']").CheckAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "打印/print" }).ClickAsync();

        // 打印需生成 40 页标签 PDF，耗时较长，用较长超时等待结果
        await Expect(Page.Locator("span[id$='Label_Message']")).ToContainTextAsync("共 40 条记录", new() { Timeout = 60_000 });
    }

    [Test, Order(7)]
    public async Task 测试七_独立打印复测()
    {
        // 保存独立业务数据（新配送单号，避免与测试三/四重复触发重复校验）
        await Page.GotoAsync($"{BaseUrl}/ShippingGoodsView.aspx");
        await ScanBarcodeAsync(QrCodeBatch0802);

        await Page.GetByLabel("配送单号").FillAsync(DeliveryNo210);
        await Page.GetByLabel("纸箱数量").FillAsync("10");
        await Page.GetByLabel("单箱毛重").FillAsync("7.4");
        await Page.GetByLabel("生产日期").FillAsync(TestDate);
        await Page.GetByLabel("检验确认日期").FillAsync(TestDate);

        await SaveAsync();
        await Expect(Page.Locator("#xhsMessageModalBody")).ToContainTextAsync("保存成功，共生成 10 条数据。");
        await CloseModalAsync();

        // 按批次查询打印列表
        await Page.GotoAsync($"{BaseUrl}/ShippingGoodsPrintList.aspx");
        await Page.GetByLabel("批次").FillAsync("1020MO-CS0720260802");
        await Page.GetByRole(AriaRole.Link, new() { Name = "搜索/search" }).ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // 新配送单号的数据应独立显示为可打印（不被同批次其它配送单号的打印记录误判）
        await Expect(Page.Locator("span[id$='Label_Message']")).ToContainTextAsync("共查询到 10 条");
    }
}
