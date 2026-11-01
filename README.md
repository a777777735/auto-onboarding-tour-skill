# AI Agent Central Skills & Global Rules Monorepo

> 具備 **Zero-Touch 動態掃描**、**全域路由表自動編譯 (Always-On Skill Router)** 與 **全平台防跑版/導覽規範** 的 AI Agent 中央倉庫。
> 支援一鍵將所有 Skills 與 Rules 動態掛載至 Antigravity 全域環境（`~/.gemini/antigravity/`），實現「每個對話框自動強制套用」。

---

## 🌟 核心設計與自動調度機制

1. **標準化 YAML 調度標頭 (Frontmatter Schema)**：
   本倉庫中所有 `skills/<name>/SKILL.md` 皆宣告標準的 `name`、`category`、`auto_invoke`、`file_patterns` 與 `trigger_condition`。
2. **零接觸動態掃描 (Zero-Touch Auto-Discovery)**：
   執行 `sync-to-global.ps1` 或 `sync-to-global.sh` 時，會自動遞迴掃描所有技能、產生 `SKILLS_INDEX.md`、並自動將最高優先級動態路由表寫入全域 `~/.gemini/antigravity/GEMINI.md`。
3. **無條件強制執行 (Always-On Execution)**：
   當 AI 在任何對話框中生成或修改代碼，只要涉及的檔案副檔名或情境符合條件，AI 將**自動且無條件先載入對應規範**，徹底杜絕介面跑版與刺眼配色！

---

## 📁 倉庫標準分類結構

```text
.
├── rules/                                      # 全域常駐規範 (Global Standards)
│   └── 01-ui-and-onboarding-standards.md       # UI 佈局防跑版、Industrial Dark 色票與導覽 SOP
│
├── skills/                                     # 模組化技能庫 (Modular Agent Skills)
│   ├── ui-layout-theme-guard/                  # [Skill] UI 佈局防跑版與工業風配色守門員 (auto_invoke: always)
│   │   ├── SKILL.md                            # 檢查與重構執行規範
│   │   └── templates/
│   │       ├── web-app-shell.css               # Web CSS 變數與 100vh App Shell 防溢出模板
│   │       └── WpfIndustrialTheme.xaml         # WPF 高對比深色主題 ResourceDictionary 樣式表
│   │
│   ├── auto-onboarding-tour/                   # [Skill] 跨平台「下一步」聚光燈互動導覽產生器 (auto_invoke: always)
│   │   ├── SKILL.md                            # 4 大注入步驟（工作流 -> 語意錨點 -> 動態視圖 -> 注入引擎）
│   │   └── templates/
│   │       ├── tour-engine.js                  # Web 原生純 JS + 聚光燈挖洞導覽引擎 (Zero-CDN)
│   │       ├── WpfTourEngine.cs                # C# WPF Adorner + CombinedGeometry 導覽引擎
│   │       └── WinFormsTourEngine.cs           # C# WinForms Overlay + Region.Exclude 導覽引擎
│   │
│   └── skill-scaffold/                         # [Skill] 新增技能專用的「元技能 Meta-Skill」 (auto_invoke: contextual)
│       └── SKILL.md                            # 自動產生符合標準規格的技能骨架並自動觸發全域編譯
│
├── examples/                                   # 驗收與展示應用
│   ├── demo-dashboard.html                     # 工業設備智慧監控儀表板 (套用 web-app-shell.css 與 tour-engine.js)
│   └── demo-app.html                           # OmniAnalytics 商業數據儀表板示範
│
├── sync-to-global.ps1                          # Windows PowerShell 動態掃描、索引更新與全域掛載腳本
├── sync-to-global.sh                           # macOS / Linux Bash 動態掃描與全域掛載腳本
├── SKILLS_INDEX.md                             # AI 自動維護之中央技能與規則總索引表
├── .gitignore                                  # 系統暫存與建置檔案排除
└── README.md                                   # 本倉庫指南
```

---

## ⚡ 一鍵動態掃描與全域部署

在終端機中執行對應 OS 的腳本，即可完成：
1. 自動解析所有 `skills/*/SKILL.md` 的 YAML 標頭。
2. 自動重新編譯並產生根目錄的 [`SKILLS_INDEX.md`](SKILLS_INDEX.md)。
3. 同步所有技能與規則至本機 Antigravity 全域目錄 (`~/.gemini/antigravity/`)。
4. 在全域 `~/.gemini/antigravity/GEMINI.md` 注入最高優先級的動態路由表！

### Windows (PowerShell)
```powershell
powershell -ExecutionPolicy Bypass -File .\sync-to-global.ps1
```

### macOS / Linux (Bash)
```bash
chmod +x sync-to-global.sh
./sync-to-global.sh
```

---

## 🛠️ 三大核心技能說明

### 1. `ui-layout-theme-guard`（UI 佈局防跑版與工業風配色守門員）
- **調度模式**：`auto_invoke: always`（處理網頁或 C# UI 代碼時無條件強制執行）
- **核心色票 (Industrial Slate Dark)**：主背景 `#0F172A`、面板 `#1E293B`、次要/邊框 `#334155`、主字 `#F8FAFC`、次要字 `#94A3B8`、操作色 `#0284C7`、狀態色 `#10B981` / `#F59E0B` / `#EF4444`。
- **三大防呆鐵律**：
  - **Web**：`:root` CSS 變數封裝；`height: 100vh; overflow: hidden;` App Shell 鎖定（杜絕雙重滾動條）；Flex/Grid 子項目必加 `min-width: 0; min-height: 0;`。
  - **WPF**：嚴禁大數值 `Margin="120,85,0,0"` 假定位；採用 `Grid` 比例分行分列；Style 集中於 `ResourceDictionary`。
  - **WinForms**：禁止純絕對座標，使用 `TableLayoutPanel` + `Dock.Fill`；啟用 `AutoScaleMode = AutoScaleMode.Dpi` 與微軟正黑體。

### 2. `auto-onboarding-tour`（跨平台「下一步」聚光燈互動導覽產生器）
- **調度模式**：`auto_invoke: always`（被要求教學引導或 SOP 導覽時自動調用）
- **四大步驟**：
  1. **盤點工作流**：收斂 4~7 個核心操作步驟。
  2. **語意錨點綁定**：Web 使用 `data-tour="step-X"`；WPF 使用 `Tag="Tour_Step_X"` 或 `x:Name`；WinForms 使用控制項實例。
  3. **動態視圖切換**：隱藏 Tab 或 Modal 在 `beforeShow` 鉤子中自動切換。
  4. **注入零依賴引擎**：右上角常駐「❓ 操作導覽」按鈕，首次打開以本機 Storage 自動啟動。

### 3. `skill-scaffold`（新增技能專用元技能 Meta-Skill）
- **調度模式**：`auto_invoke: contextual`
- **使用方式**：當您在對話中說：「**幫我新增一個關於 XXX 的 Skill**」時，AI 將：
  1. 在 `skills/<name>/` 建立符合 YAML 規範的 `SKILL.md` 與模板。
  2. 自動在背景觸發 `sync-to-global` 腳本完成全域動態掛載與路由註冊！

---

## 🖥️ 體驗驗收範例

直接在瀏覽器雙擊打開：
- [`examples/demo-dashboard.html`](examples/demo-dashboard.html)
您將看到一個融合 **Industrial Slate Dark 工業深色主題**、**100vh App Shell 防雙重捲軸佈局**、以及內建 **5 步聚光燈操作導覽** 的完整工業監控戰情室！
