[CmdletBinding()]
param (
    [Parameter(Position = 0, Mandatory = $false)]
    [string]$TargetProjectPath = "",

    [switch]$GlobalOnly
)

$ErrorActionPreference = "Stop"
$ScriptDir = $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($ScriptDir)) {
    $ScriptDir = (Get-Location).Path
}

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host " 🚀 AI Agent Skills & Rules 集中同步部署工具" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

$SourceRules = Join-Path $ScriptDir "rules"
$SourceSkills = Join-Path $ScriptDir "skills"

if (-not (Test-Path $SourceRules) -or -not (Test-Path $SourceSkills)) {
    Write-Error "錯誤：未在腳本所在目錄找到 rules/ 或 skills/ 資料夾！"
    exit 1
}

function Sync-DirectoryContents {
    param (
        [string]$Src,
        [string]$Dst,
        [string]$Label
    )
    if (-not (Test-Path $Dst)) {
        New-Item -ItemType Directory -Force -Path $Dst | Out-Null
    }

    Copy-Item -Path (Join-Path $Src "*") -Destination $Dst -Recurse -Force
    Write-Host "  [OK] 已同步 $Label -> $Dst" -ForegroundColor Green
}

# 1. 同步至本機全域環境 (~/.agents)
$GlobalAgentDir = Join-Path $HOME ".agents"
Write-Host "`n[1/2] 正在同步至本機全域設定目錄 ($GlobalAgentDir)..." -ForegroundColor Yellow

Sync-DirectoryContents -Src $SourceRules -Dst (Join-Path $GlobalAgentDir "rules") -Label "全域 Rules"
Sync-DirectoryContents -Src $SourceSkills -Dst (Join-Path $GlobalAgentDir "skills") -Label "全域 Skills"

# 2. 同步至目標專案目錄
if (-not $GlobalOnly -and -not [string]::IsNullOrWhiteSpace($TargetProjectPath)) {
    if (-not (Test-Path $TargetProjectPath)) {
        Write-Warning "目標專案路徑不存在，自動建立: $TargetProjectPath"
        New-Item -ItemType Directory -Force -Path $TargetProjectPath | Out-Null
    }

    $ProjectAgentsDir = Join-Path $TargetProjectPath ".agents"
    Write-Host "`n[2/2] 正在同步至目標專案目錄 ($ProjectAgentsDir)..." -ForegroundColor Yellow

    Sync-DirectoryContents -Src $SourceRules -Dst (Join-Path $ProjectAgentsDir "rules") -Label "專案 Rules"
    Sync-DirectoryContents -Src $SourceSkills -Dst (Join-Path $ProjectAgentsDir "skills") -Label "專案 Skills"
}
else {
    Write-Host "`n[2/2] 略過個別專案同步（已完成全域同步）。" -ForegroundColor Gray
}

Write-Host "`n✨ 同步作業全部順利完成！`n" -ForegroundColor Cyan
