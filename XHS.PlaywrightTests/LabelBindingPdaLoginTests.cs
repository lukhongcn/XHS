using Microsoft.Playwright.NUnit;

namespace XHS.PlaywrightTests;

public class LabelBindingPdaLoginTests : PageTest
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
    public async Task AnonymousUserIsRedirectedToLoginWithReturnUrl()
    {
        await Page.GotoAsync($"{BaseUrl}/LabelBindingPDA.aspx?source=playwright");

        Uri loginUri = new(Page.Url);
        Assert.That(loginUri.AbsolutePath, Does.EndWith("/Login.aspx").IgnoreCase);
        Assert.That(
            GetQueryParameter(loginUri, "ReturnUrl"),
            Is.EqualTo("/LabelBindingPDA.aspx?source=playwright"));
    }

    private static string? GetQueryParameter(Uri uri, string name)
    {
        foreach (string pair in uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            string[] parts = pair.Split('=', 2);
            if (string.Equals(Uri.UnescapeDataString(parts[0]), name, StringComparison.OrdinalIgnoreCase))
            {
                string value = parts.Length > 1 ? parts[1].Replace("+", " ") : string.Empty;
                return Uri.UnescapeDataString(value);
            }
        }

        return null;
    }
}
