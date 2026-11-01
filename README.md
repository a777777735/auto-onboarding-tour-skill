# AI Agent Central Skills & Global Rules Repository (中央技能與規範倉庫)

> 一個集中管理、維護並一鍵分發所有 AI Agent Skills、全域 Rules 與 UI/UX 範本的現代化 Monorepo。支援一鍵同步至本機全域環境（`~/.agents`）與任何個別前端或 C# 專案。

---

## 📁 集中管理架構全景

```text
.
├── rules/                                      # 全域常駐規範 (Global Standards)
│   └── 01-ui-layout-and-theme.md               # 佈局防跑版鐵律與 Industrial Slate Dark 標準色票
│
├── skills/                                     # 各項專業技能 (Modular Agent Skills)
│   ├── ui-layout-theme-guard/                  # [Skill] UI 佈局防呆與工業色票守護者
│   │   ├── SKILL.md                            # 審查與重構指令規範
│   │   └── templates/
│   │       ├── web-theme-vars.css              # Web CSS 變數與 App Shell 鎖定模板
│   │       └── WpfThemeResource.xaml           # WPF 高對比深色主題 ResourceDictionary
│   │
│   └── auto-onboarding-tour/                   # [Skill] 自動互動導覽注入引擎
│       ├── SKILL.md                            # 導覽注入四大步驟規範
│       └── templates/
│           ├── tour-engine.js                  # Web 原生純 JS + 聚光燈挖洞導覽引擎
│           ├── WpfTourEngine.cs                # C# WPF Adorner + CombinedGeometry 導覽引擎
│           └── WinFormsTourEngine.cs           # C# WinForms Overlay + Region.Exclude 導覽引擎
│
├── examples/                                   # 示範應用
│   └── demo-app.html                           # 完美整合 Industrial Dark 與 Onboarding Tour 的單檔儀表板
│
├── install-to-project.ps1                      # Windows PowerShell 一鍵同步部署腳本
├── install-to-project.sh                       # macOS / Linux Bash 一鍵同步部署腳本
├── SKILLS_INDEX.md                             # AI 技能與全域規則總索引表 (新增 Skill 必須同步更新)
└── README.md                                   # 本倉庫指南
```

---

## ⚡ 一鍵同步與部署 (Deploy & Sync)

### 1. Windows (PowerShell)
```powershell
# 同步至特定專案目錄（自動建立 .agents/rules 與 .agents/skills，並同步更新本機 ~/.agents）
.\install-to-project.ps1 "D:\Projects\MyWpfApp"

# 僅同步至本機全域環境 (~/.agents)
.\install-to-project.ps1 -GlobalOnly
```

### 2. macOS / Linux (Bash)
```bash
# 賦予執行權限並同步至目標專案
chmod +x install-to-project.sh
./install-to-project.sh /path/to/my-web-project

# 僅同步至全域 (~/.agents)
./install-to-project.sh
```

---

## 🧠 日常開發呼叫範例 (Cheat Sheet)

當您在任何已安裝此套規範的專案中與 AI 對話時，可直接使用以下提示詞觸發對應技能：

### 🎨 場景 A：介面美化、修正破版或套用色票 (`ui-layout-theme-guard`)
- **Web 網頁開發**：
  > 「請根據 `ui-layout-theme-guard` skill 審查並重構我的 `dashboard.html`，套用 Industrial Slate Dark 色票，並修正手機版與縮放時的邊界撐破問題。」
- **C# WPF 視窗開發**：
  > 「請依照 `ui-layout-theme-guard` 修正 `MainWindow.xaml` 的排版，將所有硬編碼 Margin 座標改為標準 Grid 比例佈局，並套用 `WpfThemeResource.xaml`。」
- **C# WinForms 開發**：
  > 「請幫我的 WinForms 主視窗套用工業級深色主題，並修正 DPI 縮放與 TableLayoutPanel 佈局。」

### 🚀 場景 B：為現有應用無縫加上互動步驟導覽 (`auto-onboarding-tour`)
- **Web 網頁**：
  > 「請掃描我的單檔 HTML 工具操作流程，使用 `auto-onboarding-tour` skill 在原代碼中注入 5 個步驟的聚光燈互動教學。」
- **C# WPF / WinForms**：
  > 「請為我的 WPF 視窗加上新手教學導覽，當引導到『進階分析』步驟時，自動幫使用者切換到第二個 Tab 頁籤！」

---

## ➕ 如何新增未來的第 3 個以上 Skill？（標準作業流程）

當您或 AI Agent 要在此中央倉庫中新增技能時，**請嚴格遵循以下 3 步標準 SOP**：

1. **建立 Skill 目錄**：
   在 `skills/<your-skill-name>/` 下建立 `SKILL.md`（必須包含 YAML frontmatter：`name` 與 `description`），若有相關代碼模板則放置於 `templates/` 子目錄。
2. **若有專案級規則**：
   若屬於全域開發鐵律，於 `rules/` 新增編號文件（如 `rules/02-api-architecture.md`）。
3. **強制更新索引檔 (`SKILLS_INDEX.md`)**：
   在 [`SKILLS_INDEX.md`](SKILLS_INDEX.md) 中登記新增的 Skill ID、分類、支援環境、觸發關鍵字與路徑。
4. **執行本地部署**：
   執行 `.\install-to-project.ps1 -GlobalOnly` 將新 Skill 即刻推送到本機全域環境！
