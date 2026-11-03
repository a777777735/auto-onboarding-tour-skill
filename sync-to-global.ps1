# ==============================================================================
# sync-to-global.ps1 - 動態掃描 Skills、更新總索引與自動掛載全域路由表 (Windows)
# ==============================================================================

[CmdletBinding()]
param ()

$ErrorActionPreference = "Stop"

$ScriptDir = $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($ScriptDir)) {
    $ScriptDir = (Get-Location).Path
}

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host " Antigravity 全域 Skills 動態掃描與自動掛載工具" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

$SkillsDir = Join-Path $ScriptDir "skills"
$RulesDir = Join-Path $ScriptDir "rules"
$GlobalAntigravityDir = Join-Path $HOME ".gemini\antigravity"

if (-not (Test-Path $SkillsDir)) {
    Write-Error "找不到 skills 目錄：$SkillsDir"
    exit 1
}

# 1. 遞迴掃描所有 skills/*/SKILL.md 並解析 YAML Frontmatter
Write-Host "`n[1/4] 正在動態掃描所有技能 YAML Frontmatter..." -ForegroundColor Yellow

$skillItems = @()
$skillFiles = Get-ChildItem -Path $SkillsDir -Recurse -Filter "SKILL.md"

foreach ($file in $skillFiles) {
    $content = Get-Content -Path $file.FullName -Raw -Encoding UTF8
    if ($content -match "(?ms)^---\s*\r?\n(.*?)\r?\n---") {
        $fmBlock = $matches[1]
        
        $name = if ($fmBlock -match "name:\s*(.+)") { $matches[1].Trim(' "\''') } else { $file.Directory.Name }
        $category = if ($fmBlock -match "category:\s*(.+)") { $matches[1].Trim(' "\''') } else { "General" }
        $desc = if ($fmBlock -match "description:\s*(.+)") { $matches[1].Trim(' "\''') } else { "" }
        $autoInvoke = if ($fmBlock -match "auto_invoke:\s*(.+)") { $matches[1].Trim(' "\''') } else { "contextual" }
        $patterns = if ($fmBlock -match "file_patterns:\s*(.+)") { $matches[1].Trim(' "\''') } else { "*.*" }
        $trigger = if ($fmBlock -match "trigger_condition:\s*(.+)") { $matches[1].Trim(' "\''') } else { "" }

        $skillItems += [PSCustomObject]@{
            Name = $name
            Category = $category
            Description = $desc
            AutoInvoke = $autoInvoke
            FilePatterns = $patterns
            Trigger = $trigger
            LocalPath = $file.FullName
            RelativeDir = $file.Directory.Name
        }
        Write-Host "  -> 識別到技能: [$name] (分類: $category | 調度: $autoInvoke)" -ForegroundColor Green
    }
}

# 2. 自動產生本倉庫 SKILLS_INDEX.md
Write-Host "`n[2/4] 正在產生中央倉庫總索引 (SKILLS_INDEX.md)..." -ForegroundColor Yellow

$sb = [System.Text.StringBuilder]::new()
[void]$sb.AppendLine("# AI Agent Skills & Rules Central Index (技能與全域規則總索引表)")
[void]$sb.AppendLine()
[void]$sb.AppendLine("> **AI 自動維護規範**：")
[void]$sb.AppendLine("> 本檔案由 sync-to-global.ps1 / sync-to-global.sh 自動產生與維護。")
[void]$sb.AppendLine("> 當使用者新增第三個以上的 Skill 時，AI 或自動化工具會自動重新掃描並更新此索引表！")
[void]$sb.AppendLine()
[void]$sb.AppendLine("---")
[void]$sb.AppendLine()
[void]$sb.AppendLine("## 1. 全域常駐規則 (Global Rules)")
[void]$sb.AppendLine()
[void]$sb.AppendLine("| 規則 ID | 類別 (Category) | 核心宗旨 | 檔案路徑 |")
[void]$sb.AppendLine("| :--- | :--- | :--- | :--- |")
[void]$sb.AppendLine("| **00-token-saving-output** | Policy & Performance | 全域省 Token 與極簡回覆鐵律：禁止在對話框輸出大段代碼、局部修改優先、聊天室摘要小於100字。 | [rules/00-token-saving-output.md](rules/00-token-saving-output.md) |")
[void]$sb.AppendLine("| **01-ui-and-onboarding-standards** | UI/UX & Onboarding | 雙主題工控色票（FormatString 淺色 / 工業深色）、輸入框尺寸鎖定防跑版與實戰引導四大規範。 | [rules/01-ui-and-onboarding-standards.md](rules/01-ui-and-onboarding-standards.md) |")
[void]$sb.AppendLine("| **02-subagent-dispatch-and-routing** | Architecture & Routing | Subagent 輕重任務分流矩陣 (Fast-Track vs Heavy Subagent) 與 5 大場景意圖路由決策樹。 | [rules/02-subagent-dispatch-and-routing.md](rules/02-subagent-dispatch-and-routing.md) |")
[void]$sb.AppendLine()
[void]$sb.AppendLine("---")
[void]$sb.AppendLine()
[void]$sb.AppendLine("## 2. AI Agent 技能動態註冊表 (Dynamic Skills Directory)")
[void]$sb.AppendLine()
[void]$sb.AppendLine("| 技能名稱 (ID) | 分類 (Category) | 適用檔案 (Patterns) | 調度等級 (Invoke) | 觸發時機 (Trigger Condition) | 本地路徑 |")
[void]$sb.AppendLine("| :--- | :--- | :--- | :--- | :--- | :--- |")

foreach ($s in $skillItems) {
    [void]$sb.AppendLine("| **$($s.Name)** | $($s.Category) | $($s.FilePatterns) | $($s.AutoInvoke) | $($s.Trigger) | [skills/$($s.RelativeDir)/SKILL.md](skills/$($s.RelativeDir)/SKILL.md) |")
}

[void]$sb.AppendLine()
[void]$sb.AppendLine("---")
[void]$sb.AppendLine()
[void]$sb.AppendLine("## 3. 模板庫清單 (Templates Directory)")
[void]$sb.AppendLine()
[void]$sb.AppendLine("| 所屬技能 | 模板檔案名稱 | 適用平台 | 核心用途 |")
[void]$sb.AppendLine("| :--- | :--- | :--- | :--- |")
[void]$sb.AppendLine("| ui-layout-theme-guard | WpfFormatStringTheme.xaml | C# WPF | FormatString 經典淺色工控主題、單行輸入框固定尺寸、多行編輯器伸展、DataGrid 淺色樣式表 |")
[void]$sb.AppendLine("| ui-layout-theme-guard | WpfIndustrialTheme.xaml | C# WPF | 高對比 Slate 深色筆刷、DataGrid 深度深色化、標準 Button/TextBox/Border 樣式表 |")
[void]$sb.AppendLine("| ui-layout-theme-guard | web-app-shell.css | Web HTML/CSS | 零依賴 CSS 變數系統 + App Shell (100vh 鎖定、防雙重捲軸、深色表格) 佈局模板 |")
[void]$sb.AppendLine("| interactive-action-guide | guide-engine.js | Web (Single-File) | 原生純 JS + 洞口穿透操作 + 真實事件監聽自動推進引擎 |")
[void]$sb.AppendLine("| interactive-action-guide | WpfGuideEngine.cs | C# WPF | C# 7.3 嚴格相容，HitTestCore 洞口穿透操作 + 真實事件自動推進 |")
[void]$sb.AppendLine("| interactive-action-guide | WinFormsGuideEngine.cs | C# WinForms | C# 7.3 嚴格相容，Region.Exclude 洞口穿透操作 + 真實事件自動推進 |")
[void]$sb.AppendLine("| wpf-embedded-manual-and-docs | SKILL.md | C# WPF | 左側導航手冊、縮放/平移向量流程圖與結構化功能卡片規格 |")

[System.IO.File]::WriteAllText((Join-Path $ScriptDir "SKILLS_INDEX.md"), $sb.ToString(), [System.Text.Encoding]::UTF8)
Write-Host "  ✓ SKILLS_INDEX.md 已更新完成！" -ForegroundColor Green

# 3. 同步至本機全域 Antigravity 目錄
Write-Host "`n[3/4] 正在同步至本機全域 Antigravity 目錄 ($GlobalAntigravityDir)..." -ForegroundColor Yellow

$targetGlobalSkills = Join-Path $GlobalAntigravityDir "skills"
$targetGlobalRules = Join-Path $GlobalAntigravityDir "rules"

if (-not (Test-Path $targetGlobalSkills)) { New-Item -ItemType Directory -Force -Path $targetGlobalSkills | Out-Null }
if (-not (Test-Path $targetGlobalRules)) { New-Item -ItemType Directory -Force -Path $targetGlobalRules | Out-Null }

Copy-Item -Path (Join-Path $SkillsDir "*") -Destination $targetGlobalSkills -Recurse -Force
Copy-Item -Path (Join-Path $RulesDir "*") -Destination $targetGlobalRules -Recurse -Force
Write-Host "  ✓ Skills 與 Rules 已同步至全域目錄！" -ForegroundColor Green

# 4. 動態編譯全域 GEMINI.md (Token Saving Policy 置頂最高優先級)
Write-Host "`n[4/4] 正在動態編譯全域 GEMINI.md..." -ForegroundColor Yellow

$globalGeminiFile = Join-Path $GlobalAntigravityDir "GEMINI.md"

$tokenPolicyPath = Join-Path $RulesDir "00-token-saving-output.md"
$tokenPolicyHeader = ""
if (Test-Path $tokenPolicyPath) {
    $tokenPolicyHeader = (Get-Content -Path $tokenPolicyPath -Raw -Encoding UTF8).Trim() + "`n`n"
}

$rb = [System.Text.StringBuilder]::new()
[void]$rb.AppendLine("<!-- <Auto-Generated-Skills-Router> -->")
[void]$rb.AppendLine("# 【全域強制技能調度協定 (Global Always-On Skill Router)】")
[void]$rb.AppendLine("在每一個對話框、每一個回合開始前，你必須先檢查以下技能路由表。只要使用者的請求或當前操作的檔案符合任一技能的 trigger_condition 或 file_patterns，你必須自動且無條件先讀取對應的 SKILL.md 並嚴格套用其規範與模板，無需等待使用者開口點名：")
[void]$rb.AppendLine()
[void]$rb.AppendLine("| 技能名稱 (ID) | 適用檔案 (File Patterns) | 調度等級 | 觸發條件 (Trigger Condition) | 全域路徑 |")
[void]$rb.AppendLine("| :--- | :--- | :--- | :--- | :--- |")

foreach ($s in $skillItems) {
    $absSkillPath = Join-Path $targetGlobalSkills "$($s.RelativeDir)\SKILL.md"
    $fileUri = "file:///" + ($absSkillPath -replace '\\', '/')
    [void]$rb.AppendLine("| **$($s.Name)** | $($s.FilePatterns) | $($s.AutoInvoke) | $($s.Trigger) | [$($s.Name)]($fileUri) |")
}

[void]$rb.AppendLine()
[void]$rb.AppendLine("<!-- </Auto-Generated-Skills-Router> -->")

$existingContent = ""
if (Test-Path $globalGeminiFile) {
    $existingContent = Get-Content -Path $globalGeminiFile -Raw -Encoding UTF8
}

$cleanContent = $existingContent -replace "(?s)# Global Token-Saving & Output Minimization Policy.*?---\r?\n?", ""
$cleanContent = $cleanContent -replace "(?s)<!-- <Auto-Generated-Skills-Router> -->.*?<!-- </Auto-Generated-Skills-Router> -->", ""
$cleanContent = $cleanContent.Trim()

$finalGemini = ($tokenPolicyHeader + $rb.ToString() + "`n`n" + $cleanContent).Trim()
[System.IO.File]::WriteAllText($globalGeminiFile, $finalGemini, [System.Text.Encoding]::UTF8)
Write-Host "  ✓ 全域 GEMINI.md 已成功注入置頂省 Token 鐵律與動態路由表！" -ForegroundColor Green

Write-Host "`n==========================================================" -ForegroundColor Cyan
Write-Host " 🎉 全域同步順利完成！" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan
