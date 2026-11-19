# BarcodeDesigner 安装程序

## 当前安装包

已生成测试安装包：

```text
Installer\output\BarcodeDesigner_Setup_1.0.0.exe
```

客户端只需要运行这个 EXE。程序默认安装到当前用户目录：

```text
%LOCALAPPDATA%\XHS\BarcodeDesigner
```

## 重新构建安装包

在仓库根目录执行：

```powershell
dotnet msbuild BarcodeDesigner_Codex_Project\BarcodeDesigner.csproj /t:Build /p:Configuration=Release /p:OutDir=bin\Release\ /v:minimal
```

然后使用 Windows 自带的 IExpress，打开 `BarcodeDesigner.sed` 重新生成安装包。

## 在自己的电脑测试

可以直接在自己的测试电脑或虚拟机安装。建议先备份已有 BarcodeDesigner 配置，并验证：

- 登录；
- 生成条码；
- 写入和查询 `tb_partprocess`；
- 标签打印；
- 重启程序后配置仍然有效。

## 升级规则

安装程序会替换程序和 DLL，但不会覆盖目标电脑已经存在的：

```text
欣旺达标签打印.exe.config
```

因此加密后的数据库配置不会在升级时被明文配置覆盖。JSON、模板和 DLL 会随安装包更新。

安装程序在文件复制完成后，会自动加密目标目录中的 `欣旺达标签打印.exe.config` 的整个 `appSettings` 节。升级时会保留已经存在的配置文件，不覆盖用户电脑上的加密数据库配置。

升级时会将新安装包中的普通 `appSettings` 项合并到旧配置；`MsSQLConnString` 始终保留目标电脑原有值。因此修改普通配置后，重新生成安装包并再次运行即可生效。

注意：安装包内部的测试发布内容仍包含初始配置模板，安装过程中会立即加密安装目录中的配置；正式生产环境仍建议改用权限受限的数据库账号，不要使用 `sa`。
