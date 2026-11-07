; BarcodeDesigner 安装程序
; 使用 Inno Setup 编译。
; 编译前先执行：
; dotnet msbuild BarcodeDesigner_Codex_Project\BarcodeDesigner.csproj /t:Build /p:Configuration=Release /p:OutDir=bin\Release\ /v:minimal

#define AppName "欣旺达标签打印"
#define AppVersion "1.0.0"
#define AppPublisher "XHS"
#define AppExeName "欣旺达标签打印.exe"
#define BuildDir "..\BarcodeDesigner_Codex_Project\bin\Release"

[Setup]
AppId={{A2B93768-0A4B-4ABF-AF28-BARCODEDESIGNER}}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={autopf}\XHS\BarcodeDesigner
DefaultGroupName={#AppName}
OutputDir=output
OutputBaseFilename=BarcodeDesigner_Setup_{#AppVersion}
Compression=lzma2
SolidCompression=yes
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64compatible
DisableProgramGroupPage=yes
UninstallDisplayIcon={app}\{#AppExeName}
VersionInfoDescription={#AppName}安装程序
VersionInfoProductName={#AppName}
VersionInfoProductVersion={#AppVersion}

[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "创建桌面快捷方式"; GroupDescription: "附加快捷方式："; Flags: unchecked

[Files]
; 发布目录中的 EXE、DLL 和依赖文件
Source: "{#BuildDir}\*"; DestDir: "{app}"; Excludes: "*.config"; Flags: ignoreversion recursesubdirs createallsubdirs

; 升级时不要覆盖用户电脑已经加密的主配置文件。
; 首次安装仍会安装构建输出中的默认配置，后续可由首次运行逻辑加密。
Source: "{#BuildDir}\{#AppExeName}.config"; DestDir: "{app}"; DestName: "{#AppExeName}.config"; Flags: onlyifdoesntexist ignoreversion

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExeName}"; WorkingDir: "{app}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; WorkingDir: "{app}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExeName}"; Description: "启动{#AppName}"; WorkingDir: "{app}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
; 只删除安装程序生成的临时目录，不删除用户配置和打印输出。
Type: filesandordirs; Name: "{app}\output"
