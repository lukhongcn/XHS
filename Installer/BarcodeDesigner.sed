[Version]
Class=IEXPRESS
SEDVersion=3

[Options]
PackagePurpose=InstallApp
ShowInstallProgramWindow=1
HideExtractAnimation=1
UseLongFileName=1
InsideCompressed=1
TargetName=D:\project\XHS\Installer\output\BarcodeDesigner_Setup_20261120_CDrive.exe
FriendlyName=欣旺达标签打印安装程序
AppLaunched=cmd.exe /d /s /c install.cmd
PostInstallCmd=<None>
AdminQuietInstCmd=
UserQuietInstCmd=
SourceFiles=SourceFiles

[Strings]
FILE0="install.cmd"
FILE1="install.ps1"
FILE2="BarcodeDesignerPayload.zip"

[SourceFiles]
SourceFiles0=D:\project\xhs\Installer\staging

[SourceFiles0]
%FILE0%=
%FILE1%=
%FILE2%=
