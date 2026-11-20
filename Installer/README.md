# BarcodeDesigner 安装程序

## 当前安装程序

发布安装包：

```text
Installer\output\BarcodeDesigner_Setup_20261120_CDrive.exe
```

运行安装包并允许管理员权限后，程序固定安装到：

```text
C:\XHS\BarcodeDesigner
```

## 直接复制 Release

本机 Release 目录：

```text
BarcodeDesigner_Codex_Project\bin\Release
```

可将目录内全部文件复制到 `C:\XHS\BarcodeDesigner`。此 Release 的 `欣旺达标签打印.exe.config` 已包含新数据库连接，并使用本机 DPAPI 加密；程序仍通过原有的 `ConfigurationManager.AppSettings` 读取，无需改代码。此加密配置只能在生成它的这台 Windows 电脑上解密，不能复制到另一台电脑使用。

安装程序会更新数据库连接，并用 Windows DPAPI 加密安装目录中的 `欣旺达标签打印.exe.config` 的 `appSettings` 节。重新安装时会将数据库连接更新为下方配置，普通配置项按安装包更新；现有配置文件会保留其他已有设置。

## 数据库连接

```text
Server: 192.168.1.71
Database: XHS
User: sa
Password: ss#12345
Connection string: server=192.168.1.71;Pooling=false;database=XHS;uid=sa;pwd=ss#12345
```

此密码是安装配置的一部分。请限制本说明文档和安装包的访问，不要上传到公开仓库或公开文件服务。

## 重新构建

在仓库根目录执行：

```powershell
dotnet msbuild BarcodeDesigner_Codex_Project\BarcodeDesigner.csproj /t:Build /p:Configuration=Release /p:Platform=AnyCPU /v:quiet
Compress-Archive -Path BarcodeDesigner_Codex_Project\bin\Release\* -DestinationPath Installer\staging\BarcodeDesignerPayload.zip -CompressionLevel Optimal -Force
```

然后使用 Windows IExpress 以 `Installer\BarcodeDesigner.sed` 生成安装包。发布前请确认 SED 中的 `TargetName` 指向 `Installer\output`，且文件名与本说明一致。

## 安装检查

- 程序文件位于 `C:\XHS\BarcodeDesigner`。
- 启动后可登录、查询数据并打印标签。
- 安装后配置文件的 `appSettings` 节显示为加密内容。
- 升级会保留用户原有的其他配置，同时更新数据库连接。
