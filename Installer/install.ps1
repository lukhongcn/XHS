$ErrorActionPreference = 'Stop'

$appName = '欣旺达标签打印'
$installDir = Join-Path $env:LOCALAPPDATA 'XHS\BarcodeDesigner'
$payloadZip = Join-Path $PSScriptRoot 'BarcodeDesignerPayload.zip'
$extractDir = Join-Path $env:TEMP ('BarcodeDesignerPayload_' + [Guid]::NewGuid().ToString('N'))
$configName = "$appName.exe.config"

function Protect-AppSettings([string]$exePath) {
    Add-Type -AssemblyName System.Configuration

    $configPath = "$exePath.config"
    if (-not (Test-Path -LiteralPath $configPath)) {
        throw "找不到程序配置文件：$configPath"
    }

    $fileMap = New-Object System.Configuration.ExeConfigurationFileMap
    $fileMap.ExeConfigFilename = $configPath
    $config = [System.Configuration.ConfigurationManager]::OpenMappedExeConfiguration(
        $fileMap,
        [System.Configuration.ConfigurationUserLevel]::None)
    $section = $config.GetSection('appSettings')

    if ($null -eq $section) {
        throw '配置文件中没有 appSettings 节。'
    }

    if (-not $section.SectionInformation.IsProtected) {
        $section.SectionInformation.ProtectSection('DataProtectionConfigurationProvider')
        $section.SectionInformation.ForceSave = $true
        $config.Save([System.Configuration.ConfigurationSaveMode]::Full)
    }
}

function Merge-AppSettings([string]$targetExePath, [string]$templateExePath) {
    Add-Type -AssemblyName System.Configuration

    $targetMap = New-Object System.Configuration.ExeConfigurationFileMap
    $targetMap.ExeConfigFilename = "$targetExePath.config"
    $targetConfig = [System.Configuration.ConfigurationManager]::OpenMappedExeConfiguration(
        $targetMap,
        [System.Configuration.ConfigurationUserLevel]::None)

    $templateMap = New-Object System.Configuration.ExeConfigurationFileMap
    $templateMap.ExeConfigFilename = "$templateExePath.config"
    $templateConfig = [System.Configuration.ConfigurationManager]::OpenMappedExeConfiguration(
        $templateMap,
        [System.Configuration.ConfigurationUserLevel]::None)

    $targetSection = $targetConfig.GetSection('appSettings')
    $templateSection = $templateConfig.GetSection('appSettings')
    if ($null -eq $targetSection -or $null -eq $templateSection) {
        throw '目标配置或安装包配置中没有 appSettings 节。'
    }

    foreach ($key in $templateSection.Settings.AllKeys) {
        # 数据库连接由目标电脑维护，升级时不能覆盖。
        if ($key -ieq 'MsSQLConnString') {
            continue
        }

        $templateValue = $templateSection.Settings[$key].Value
        if ($null -eq $targetSection.Settings[$key]) {
            $targetSection.Settings.Add($key, $templateValue)
        }
        else {
            $targetSection.Settings[$key].Value = $templateValue
        }
    }

    if (-not $targetSection.SectionInformation.IsProtected) {
        $targetSection.SectionInformation.ProtectSection('DataProtectionConfigurationProvider')
    }
    $targetSection.SectionInformation.ForceSave = $true
    $targetConfig.Save([System.Configuration.ConfigurationSaveMode]::Full)
}

try {
    Add-Type -AssemblyName System.Windows.Forms

    if (-not (Test-Path -LiteralPath $payloadZip)) {
        throw "找不到安装内容：$payloadZip"
    }

    $running = Get-Process -Name '欣旺达标签打印' -ErrorAction SilentlyContinue
    if ($running) {
        $running | Stop-Process -Force
    }

    New-Item -ItemType Directory -Path $extractDir -Force | Out-Null
    Expand-Archive -LiteralPath $payloadZip -DestinationPath $extractDir -Force
    New-Item -ItemType Directory -Path $installDir -Force | Out-Null
    $targetExePath = Join-Path $installDir "$appName.exe"
    $hadExistingConfig = Test-Path -LiteralPath "$targetExePath.config"

    Get-ChildItem -LiteralPath $extractDir -Force | ForEach-Object {
        if ($_.Name -ieq $configName -and (Test-Path -LiteralPath (Join-Path $installDir $configName))) {
            return
        }

        Copy-Item -LiteralPath $_.FullName -Destination (Join-Path $installDir $_.Name) -Recurse -Force
    }

    if ($hadExistingConfig) {
        Merge-AppSettings $targetExePath (Join-Path $extractDir "$appName.exe")
    }
    else {
        Protect-AppSettings $targetExePath
    }

    $shell = New-Object -ComObject WScript.Shell
    $desktop = [Environment]::GetFolderPath('Desktop')
    $shortcut = $shell.CreateShortcut((Join-Path $desktop "$appName.lnk"))
    $shortcut.TargetPath = Join-Path $installDir "$appName.exe"
    $shortcut.WorkingDirectory = $installDir
    $shortcut.Save()

    [System.Windows.Forms.MessageBox]::Show(
        "安装完成。程序位置：`n$installDir",
        $appName,
        [System.Windows.Forms.MessageBoxButtons]::OK,
        [System.Windows.Forms.MessageBoxIcon]::Information) | Out-Null
}
catch {
    Add-Type -AssemblyName System.Windows.Forms
    [System.Windows.Forms.MessageBox]::Show(
        "安装失败：`n$($_.Exception.Message)",
        $appName,
        [System.Windows.Forms.MessageBoxButtons]::OK,
        [System.Windows.Forms.MessageBoxIcon]::Error) | Out-Null
    exit 1
}
finally {
    if (Test-Path -LiteralPath $extractDir) {
        Remove-Item -LiteralPath $extractDir -Recurse -Force -ErrorAction SilentlyContinue
    }
}
