# XHS Playwright Tests

运行前先启动 `XHSWorkFlow` 网站，然后执行：

```powershell
$env:XHS_BASE_URL = "http://localhost:55426"
dotnet test XHS.Playwright.sln
```

第一次运行前需要安装 Playwright 浏览器：

```powershell
dotnet build XHS.Playwright.sln
pwsh XHS.PlaywrightTests/bin/Debug/net8.0/playwright.ps1 install
```
