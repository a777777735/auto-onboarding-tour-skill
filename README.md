# AI Agent Central Skills & Global Rules Monorepo

> **企業級 AI Agent 技能與全域規範中央倉庫**  
> 具備 **Zero-Touch 動態掃描**、**全域路由表編譯 (Always-On Skill Router)**、**Subagent 輕重任務分流機制 (Fast-Track vs. Subagent Track)**、**5 大場景意圖決策樹**、**雙主題工控色票規範 (FormatString 經典淺色 / 工業深色)**、**輸入框尺寸鎖定與網格防跑版鐵律**，以及 **全平台實戰互動引導 (Click-Through Tour Engine)**。  
> 支援一鍵將所有 Skills 與 Rules 動態掛載至 Antigravity 全域環境（`~/.gemini/antigravity/`），實現「每個對話框自動強制套用」。

---

## 🎯 架構合理性三大優化落地 (Architecture Rationality Enhancements)

根據《AI Agent、Skills 與自動化調用機制架構合理性分析報告》，本倉庫已全面完成以下三大架構升級：

### 1. 盲點 A 解決：Subagent 調用成本與即時性權衡（輕重任務分流矩陣）
- **過去盲點**：微小修改（如修改一行代碼或調整按鈕顏色）時，盲目調用 Subagent 會產生額外的上下文初始化開銷與等待延遲。
- **優化落地（`rules/02-subagent-dispatch-and-routing.md`）**：
  * **⚡ 輕量任務快速通道 (Fast Track - 主執行緒直接執行)**：局部樣式/色系調整、修復單一 TextBox 尺寸、編寫數行 XAML/C#、單次 MSBuild 編譯。零延遲、即時交付結果。
  * **🛡️ 重量任務深度隔離通道 (Subagent Track - 派發專屬 Subagent)**：
    * `binary-and-flow-analyst`：閉源二進制分析、Mono.Cecil / REA 反組譯 IL 指令、自訂二進制檔逆向、多模組變量生命週期審計。
    * `build-and-qa-guardian`：跨專案大規模批次建置、進程鎖死自動解除、單 DLL 嵌入發布、端對端全流程迴歸驗證。

### 2. 盲點 B 解決：技能職責收斂與互補重構
- **過去盲點**：目錄中 `wpf-guide-and-manual-kit` 與 `interactive-action-guide` 職責存在重疊。
- **優化落地**：
  * **`interactive-action-guide`**：專注於 **全平臺實戰穿透式引導引擎 (Web/WPF/WinForms)**，四周鎖死、聚光燈洞口可穿透操作、監聽真實事件自動推進下一步。
  * **`wpf-embedded-manual-and-docs`**：收斂重構為專門負責 **WPF XAML 內嵌說明手冊、可縮放/平移向量流程圖 (Pan & Zoom) 與技術文件章節卡片**，兩者 100% 互補協同，邊界清晰。

### 3. 盲點 C 解決：5 大場景意圖路由決策樹 (Intent-to-Skill Router)
建立無歧義的關鍵字與意圖決策樹，確保 AI 零猶豫自動調用：
1. **介面佈局、色系、防跑版** ➔ 調用 `ui-layout-theme-guard`
2. **操作教學、新手引導、下一步 SOP** ➔ 調用 `interactive-action-guide`
3. **說明手冊、技術文檔、向量流程圖** ➔ 調用 `wpf-embedded-manual-and-docs`
4. **逆向工程、IL 指令、閉源 DLL、變量審計** ➔ 調用 `dotnet-binary-il-forensics` / `binary-and-flow-analyst`
5. **安全編譯、進程鎖死防護、打包發布** ➔ 調用 `dotnet-safe-build-deploy` / `build-and-qa-guardian`

---

## 🎨 雙主題工控色票與輸入框防跑版鐵律

### 1. 雙工控主題標準
* **☀️ FormatString 經典工控淺色主題 (Industrial Slate Light)**：
  * 主底色 `#F7F9FC`、純白卡片 `#FFFFFF`（邊框 `#E2E8F0`）、工具列 `#F1F5F9`（邊框 `#CBD5E1`）。
  * 主文字 `#1A252F`、標題 `#1E293B`、次要標籤 `#64748B`。
  * 操作主色藍 `#0284C7` / `#3498DB`、儲存執行綠 `#10B981` / `#27AE60`、規則紫 `#8B5CF6` / `#8E44AD`。
  * 狀態摘要條：天空藍底 `#E0F2FE`、邊框 `#BAE6FD`、文字 `#0369A1`。
  * 淺色 DataGrid：表頭 `#F1F5F9`（字體加粗 `#1E293B`）、交替列 `#F8FAFC`、選中行 `#E0F2FE`。
* **🌙 工業級深色主題 (Industrial Slate Dark)**：
  * 底色 `#0F172A`、卡片 `#1E293B`、邊框/表頭 `#334155`、主文字 `#F8FAFC`，專為暗室光學檢測打造。

### 2. 輸入框尺寸鎖定與網格防跑版鐵律 (Input Size & Grid Layout Protocol)
* **單行輸入框 (Single-line TextBox)**：
  * **高度嚴格固定**：WPF 強制指定 `Height="24"` 或 `Height="26"`；Web 強制 `height: 26px; box-sizing: border-box;`。嚴禁設為 `Height="Auto"`。
  * **文字垂直絕對置中**：必須指定 `VerticalContentAlignment="Center"`，換字型或字號微調時文字維持置中，不貼頂不抖動。
  * **內距固定**：統一 `Padding="6,0,6,0"` 或 `Padding="4,2"`。
  * **寬度比例分配**：表單排版一律使用 `Grid`（標籤列 `Width="Auto"`，輸入列 `Width="*"` + `MinWidth="80"`），禁止放任文字動態撐開容器。
* **多行輸入框與編輯器 (Multi-line / Code Editor)**：
  * 透過 `HorizontalAlignment="Stretch"` 與 `VerticalAlignment="Stretch"` 置於外層 Grid 比例列（`RowDefinition Height="*"`），內部啟用自動滾動條（`VerticalScrollBarVisibility="Auto"`），禁止因行數增加撐破整個視窗。

---

## 📁 倉庫結構全景

```text
.
├── rules/                                      # 全域常駐規範 (Global Standards)
│   ├── 00-token-saving-output.md               # [Rule] 全域省 Token 與極簡回覆鐵律
│   ├── 01-ui-and-onboarding-standards.md       # [Rule] 雙主題色票、輸入框防跑版與實戰引導四大核心鐵律
│   ├── 01-ui-layout-and-theme.md               # [Rule] UI 網格幾何排版、防撐破與雙主題規範
│   └── 02-subagent-dispatch-and-routing.md     # [Rule] Subagent 輕重分流矩陣與 5 大場景意圖路由決策樹
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
│   ├── wpf-embedded-manual-and-docs/           # [Skill] 嵌入式說明手冊與向量流程圖系統 (auto_invoke: contextual)
│   │   └── SKILL.md                            # 左側導航手冊、縮放/平移向量流程圖與結構化功能卡片規格
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

## ⚡ 一鍵動態掃描與全域同步

本倉庫具備強大的一鍵同步機制，會自動將最新的 rules 與 skills 複製掛載至使用者的全域環境（`~/.gemini/antigravity/`），並即時重建 `SKILLS_INDEX.md` 與全域 `GEMINI.md`：

### Windows (PowerShell)
```powershell
powershell -ExecutionPolicy Bypass -File .\sync-to-global.ps1
```

### macOS / Linux (Bash)
```bash
chmod +x sync-to-global.sh
./sync-to-global.sh
```
