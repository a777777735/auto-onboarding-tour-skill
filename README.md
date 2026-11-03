# AI Agent Central Skills & Global Rules Monorepo

> **企業級 AI Agent 技能與全域規範中央倉庫**  
> 具備 **Zero-Touch 動態掃描**、**全域路由表編譯 (Always-On Skill Router)**、**雙主題工控色票規範 (FormatString 經典淺色 / 工業深色)**、**輸入框尺寸鎖定與網格防跑版鐵律**，以及 **全平台實戰互動引導 (Click-Through Tour Engine)**。  
> 支援一鍵將所有 Skills 與 Rules 動態掛載至 Antigravity 全域環境（`~/.gemini/antigravity/`），實現「每個對話框自動強制套用」。

---

## 🎯 核心解決的兩大關鍵痛點

### 痛點 1：操作頁面色系不統一（全面導入雙工控主題標準）
- **過去痛點**：舊有規範僅注重夜間工業深色（Slate Dark），導致在淺色主控台（如 FormatString 主頁 `#F7F9FC`）新增子頁面或彈窗（如 ParseString、SmartVarPickerWindow）時，色系產生極大突兀感，缺乏統一風格。
- **解決方案**：正式確立**雙工控主題架構**：
  1. **☀️ FormatString 經典工控淺色主題 (Industrial Slate Light)**：
     - 主底色 `#F7F9FC`、純白卡片 `#FFFFFF`（邊框 `#E2E8F0`）、工具列 `#F1F5F9`（邊框 `#CBD5E1`）。
     - 文字層次：主內容 `#1A252F`、標題 `#1E293B`、次要標籤 `#64748B`。
     - 操作色彩：主操作藍 `#0284C7` / `#3498DB`、儲存執行綠 `#10B981` / `#27AE60`、規則生成紫 `#8B5CF6` / `#8E44AD`。
     - 狀態摘要條：天空藍底 `#E0F2FE`、邊框 `#BAE6FD`、文字 `#0369A1`。
     - 表格 DataGrid：表頭 `#F1F5F9`（加粗 `#1E293B`）、交替列 `#F8FAFC`、選中行 `#E0F2FE`。
  2. **🌙 工業級深色主題 (Industrial Slate Dark)**：
     - 底色 `#0F172A`、卡片 `#1E293B`、邊框/表頭 `#334155`、主文字 `#F8FAFC`，專為暗室光學檢測打造。

### 痛點 2：輸入框大小隨字體變大變小、未合理分配大小（防跑版鐵律）
- **過去痛點**：每次新增操作頁面時，TextBox 預設未設定高度或隨字級動態計算，造成更換字型或縮放視窗時輸入框高矮不一、被文字撐爆變形，或在容器內長度忽長忽短。
- **解決方案**：強制推行 **「輸入框尺寸鎖定與網格排版防跑版鐵律 (Input Size & Grid Layout Protocol)」**：
  1. **單行文字輸入框 (Single-line TextBox)**：
     - **高度嚴格固定**：WPF 必須顯式指定 `Height="24"` 或 `Height="26"`；Web 必須 `height: 26px; box-sizing: border-box;`。嚴禁使用 `Height="Auto"` 放任字級撐高。
     - **文字垂直絕對置中**：WPF 必須指定 `VerticalContentAlignment="Center"`，確保字型切換或字號微調時文字始終維持垂直居中，不抖動不貼頂。
     - **內邊距固定**：WPF 統一 `Padding="6,0,6,0"` 或 `Padding="4,2"`；Web 統一 `padding: 0 8px;`。
     - **合理比例寬度分配**：
       - 表單排版一律使用 `Grid`：標籤欄為 `Width="Auto"`，輸入欄為 `Width="*"`（彈性比例分配）並搭配 `MinWidth="80"`，絕不讓文字拉扯欄位寬度。
       - 工具列/並排排列（StackPanel/WrapPanel）：必須顯式給定具體寬度（例如前綴/後綴 `Width="38"`、名稱 `Width="85"`、鏈結 `Width="160"`、搜尋 `Width="260"`），禁止未給寬度任由文字拉伸。
  2. **多行文字與編輯器 (Multi-line Text / Code Editor)**：
     - 必須設定 `HorizontalAlignment="Stretch"` 與 `VerticalAlignment="Stretch"`，置於外層 Grid 的彈性比例列（`RowDefinition Height="*"`），內部啟用自動滾動條（`VerticalScrollBarVisibility="Auto"`），禁止因行數增加撐破整個視窗。

---

## 📁 倉庫結構全景

```text
.
├── rules/                                      # 全域常駐規範 (Global Standards)
│   ├── 00-token-saving-output.md               # [Rule] 全域省 Token 與極簡回覆鐵律
│   ├── 01-ui-and-onboarding-standards.md       # [Rule] 雙主題色票、輸入框防跑版與實戰引導四大核心鐵律
│   └── 01-ui-layout-and-theme.md               # [Rule] UI 網格幾何排版、防撐破與雙主題規範
│
├── skills/                                     # 模組化技能庫 (Modular Agent Skills)
│   ├── ui-layout-theme-guard/                  # [Skill] UI 佈局防跑版、輸入框鎖定與雙主題守護者 (auto_invoke: always)
│   │   ├── SKILL.md                            # 檢查與重構執行規範 (包含 Step 0 版本偵測)
│   │   └── templates/
│   │       ├── WpfFormatStringTheme.xaml       # WPF 經典工控淺色主題 ResourceDictionary (FormatString 標準)
│   │       ├── WpfIndustrialTheme.xaml         # WPF 工業級深色主題 ResourceDictionary (Slate Dark)
│   │       ├── web-app-shell.css               # Web CSS 變數與 100vh App Shell 防雙重滾動條模板
│   │       └── web-theme-vars.css              # Web 雙主題色票變數定義檔
│   │
│   ├── interactive-action-guide/               # [Skill] 實戰互動導引引擎 (四周鎖死、洞口穿透、真實事件推進)
│   │   ├── SKILL.md                            # 實戰引導注入規範 (包含 Step 0 讀取 .csproj 偵測版本)
│   │   └── templates/
│   │       ├── guide-engine.js                 # Web 原生 JS 實戰引導引擎 (Click-Through 穿透、click/input/change 監聽)
│   │       ├── WpfGuideEngine.cs               # C# WPF 實戰引導引擎 (C# 7.3 相容、HitTestCore 洞口穿透)
│   │       └── WinFormsGuideEngine.cs          # C# WinForms 實戰引導引擎 (C# 7.3 相容、Region.Exclude 穿透)
│   │
│   └── skill-scaffold/                         # [Skill] 新增技能專用的「元技能 Meta-Skill」 (auto_invoke: contextual)
│       └── SKILL.md                            # 自動產生符合標準規格的技能骨架並自動觸發全域編譯
│
├── examples/                                   # 驗收與展示應用
│   ├── demo-dashboard.html                     # 工業設備智慧監控儀表板 (深色 App Shell + 實戰互動導引)
│   └── demo-app.html                           # OmniAnalytics 商業數據儀表板示範
│
├── sync-to-global.ps1                          # Windows PowerShell 動態掃描、索引更新與全域掛載腳本
├── sync-to-global.sh                           # macOS / Linux Bash 動態掃描與全域掛載腳本
├── SKILLS_INDEX.md                             # AI 自動維護之中央技能與規則總索引表
├── .gitignore                                  # 系統暫存與建置檔案排除
└── README.md                                   # 本倉庫指南
```

---

## 🛠️ 三大核心 AI 技能詳解 (Agents & Skills)

### 1. `ui-layout-theme-guard`（UI 佈局防跑版與雙主題守護者）
- **觸發等級**：`auto_invoke: always`（所有介面生成或修改自動生效）。
- **職責**：
  - **Step 0 前置審查**：偵測 C# 專案版本（.NET 4.x 切換 C# 7.3 模式；.NET 6/8 自動適配），防止第三方 UI 庫衝突。
  - **強制鎖定輸入框**：自動審查所有 `TextBox` 與 `Input`，注入固定高度（`Height="26"`）與 `VerticalContentAlignment="Center"`，以 Grid 比例列分配寬度。
  - **提供完整 XAML 樣板**：直接提供 `WpfFormatStringTheme.xaml`，快速引入包含 `LockedSingleLineTextBoxStyle`、`StretchMultiLineTextBoxStyle`、`FlexButtonStyle` 與 `LightDataGridStyle`。

### 2. `interactive-action-guide`（實戰穿透式互動導引）
- **觸發等級**：`auto_invoke: always`。
- **職責**：
  - **四周鎖死、洞口完全可穿透操作 (Click-Through)**：使用者能直接在穿透洞口中點擊按鈕、輸入文字或切換選項。
  - **監聽真實事件自動推進**：完成真實操作（`click`, `TextChanged`, `SelectedIndexChanged`）後自動觸發動畫切換至下一步，杜絕死板的純文字說明。
  - **多平台 100% 覆蓋**：提供原生 Web JavaScript、WPF（`HitTestCore` 穿透）與 WinForms（`Region.Exclude` 穿透）現成引擎。

### 3. `skill-scaffold`（技能腳手架元技能 Meta-Skill）
- **觸發等級**：`auto_invoke: contextual`。
- **職責**：
  - 當用戶要求新增或打包新技能時，自動產生包含 YAML Frontmatter、觸發條件與代碼模板的正規結構，並自動調用 `sync-to-global` 重新編譯全局索引。

---

## ⚡ 一鍵動態掃描與全域同步

本倉庫具備強大的一鍵同步機制，會自動將最新的 rules 與 skills 複製掛載至使用者的全域環境（`~/.gemini/antigravity/`），並即時重建 `SKILLS_INDEX.md`：

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

## 🏭 實戰案例：Amotion 工控專案落地驗證

本套規範已全面套用並驗證於大型工控平台實戰專案：
1. **`Plugin.FormatString`**：標準 Slate 淺色主題、多變量雙向鏈結、語法即時預覽、單行輸入框固定尺寸、多行範本編輯器彈性自適應。
2. **`Plugin.ParseString`**：報文逆向解構、多欄位斷言比對驗證、回寫全域變數，介面全面改版為清爽的 FormatString Slate 淺色系，所有輸入框鎖定尺寸不跑版。
3. **`SmartVarPickerWindow`**：跨模組模糊搜尋、型別篩選、即時數值預覽與雙擊快選，全面改用 Slate 淺色工控風格與固定 26px 搜尋列。
