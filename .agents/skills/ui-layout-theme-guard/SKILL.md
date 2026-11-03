---
name: ui-layout-theme-guard
category: UI/UX
description: 專業級 UI 佈局防跑版、輸入框尺寸鎖定與雙主題工控色票守護者。自動審查並重構 Web (HTML/CSS)、C# WPF 與 C# WinForms 介面代碼，根治輸入框大小失控隨字體縮放、版面跑版、元件重疊與配色刺眼問題。
auto_invoke: always
file_patterns: "*.html, *.css, *.jsx, *.tsx, *.vue, *.xaml, *.cs"
trigger_condition: 當生成、修改或美化任何 Web 網頁 (HTML/CSS) 或 C# (WPF/WinForms) 介面代碼時，必須主動套用此規範防止輸入框大小暴走跑版與刺眼配色。
---

# UI Layout & Industrial Theme Guard

本 Skill 為 UI 佈局防跑版、輸入框比例鎖定與雙主題工業風配色的最高執行準則。在編寫、修改或重構任何 Web 前端（HTML/CSS）或 C# 桌面應用（WPF/WinForms）時，**必須自動且無條件套用以下規範**。

---

## 🔍 Step 0: 環境與版本前置偵測 (Environment & Version Pre-flight Check)

在產生或修改任何介面代碼前，AI **必須先執行環境與版本前置審查**：

1. **C# 專案版本判斷 (讀取 `.csproj`)**：
   - **.NET Framework 4.5~4.8**：強制切換為 **C# 7.3 嚴格相容模式**（傳統區塊 namespace、明確宣告型別、不使用 Nullable 參考型別標記 `?` 或 `switch` 運算式）。
   - **.NET 6 / .NET 8**：檢查 `<Nullable>` 與 `<ImplicitUsings>` 設定，靈活配合型別宣告與 `using` 引用。
2. **第三方 UI 套件衝突防呆**：
   - 檢查專案是否已引入第三方 UI 庫（如 `MaterialDesignThemes`、`HandyControl`、`MahApps.Metro`、`DevExpress`）。
   - **防呆保護**：若存在第三方 UI 庫，**僅注入色彩變數（`Color` / `SolidColorBrush`）與網格結構（`Grid` / `DockPanel`）**，嚴格禁止覆蓋第三方元件預設的 `ControlTemplate`，避免損毀原有視覺風格。
3. **Web 打包架構判定**：
   - **無 `package.json`（純單檔 HTML）**：強制採用零依賴原生內嵌 CSS 變數與 App Shell。
   - **有 `package.json`（React / Vue / Vite）**：以模組化樣式（CSS Modules、Tailwind 或變數檔案）輸出，並適配框架生命週期。

---

## 🎨 1. 雙主題工控色票規範 (Dual Industrial Theme Standards)

所有介面預設遵循此套經過 WCAG AA 對比度檢驗的專業工業級色票，**嚴禁隨機硬編碼刺眼純色（如 `#FF0000`, `#0000FF`）**：

### ☀️ A. FormatString 經典工控淺色主題 (Industrial Slate Light)
專為資料密集、參數配置、工控主控台打造的高清清爽色系：

| 色票名稱 | 色碼 (HEX) | 用途與規範說明 | 對比標準 |
| :--- | :--- | :--- | :--- |
| **`Bg-Primary`** | `#F7F9FC` (Slate 50) | 全域視窗畫布背景、最底層容器 | 搭配 `Text-Primary` 清爽護眼 |
| **`Bg-Surface`** | `#FFFFFF` (White) | 卡片容器、面板、表單區域背景 | 搭配 `BorderLight` 輕盈分割 |
| **`Bg-Elevated`** | `#F1F5F9` (Slate 100) | 頂部工具列、表格標題列 (Header) | 結構層次明確 |
| **`Bg-RowAlt`** | `#F8FAFC` (Slate 50) | 表格偶數列交替色 | 提升大量資料辨識度 |
| **`BorderLight`** | `#E2E8F0` (Slate 200) | 卡片邊框、網格分隔線 | 柔和清晰 |
| **`BorderMedium`** | `#CBD5E1` (Slate 300) | 工具列外框、輸入框邊框 | 幾何界線明確 |
| **`Text-Primary`** | `#1A252F` (Slate 900) | 內容文字、輸入框文字 | 高度清晰 (>13:1) |
| **`Text-Heading`** | `#1E293B` (Slate 800) | 區塊標題、重要欄位名 | 加粗加深 |
| **`Text-Muted`** | `#64748B` (Slate 500) | 次要標籤、提示說明 | 符合 WCAG AA |
| **`Primary`** | `#0284C7` (Sky 600) | 核心動作按鈕、聚焦外框 (Focus) | Hover: `#0369A1` |
| **`Success`** | `#10B981` (Emerald 500) | 儲存、執行成功、綠色按鈕 | Hover: `#059669` |
| **`Accent`** | `#8B5CF6` (Violet 500) | 掃描、高階規則生成按鈕 | Hover: `#7C3AED` |
| **`StatusAlert`** | `#E0F2FE` (Light Sky) | 狀態資訊條（底色 `#E0F2FE`，邊框 `#BAE6FD`，文字 `#0369A1`） | 高可讀性 |

### 🌙 B. 工業級深色主題 (Industrial Slate Dark)
適用於夜間、光學視覺檢測暗室等場景：

| 色票名稱 | 色碼 (HEX) | 用途與規範說明 | 對比標準 |
| :--- | :--- | :--- | :--- |
| **`Bg-Primary`** | `#0F172A` (Slate 900) | 全域視窗畫布背景、最底層容器 | 搭配 `Text-Primary` |
| **`Bg-Surface`** | `#1E293B` (Slate 800) | 卡片容器、面板、表格背景 | 搭配 `Border` 柔和分割 |
| **`Bg-Elevated`** | `#334155` (Slate 700) | 表格標題列 (Header)、按鈕 Hover 態、輸入框底色 | 強化階層感 |
| **`Bg-RowAlt`** | `#162032` (Slate 850) | 表格/網格偶數列交替底色 | 提升大量數據可讀性 |
| **`Border`** | `#334155` (Slate 700) | 面板外框、格線邊框（1px solid） | 結構分界 |
| **`Text-Primary`** | `#F8FAFC` (Slate 50) | 標題文字、核心讀數、主按鈕文字 | 對比度 > 12:1 (極高) |
| **`Text-Muted`** | `#94A3B8` (Slate 400) | 次要文字、輸入框 Placeholder、時間標籤 | 符合 WCAG AA |
| **`Primary`** | `#0284C7` (Sky 600) | 核心動作按鈕、聚焦光圈 (Focus Ring) | Hover: `#0369A1` |

---

## 📐 2. 輸入框尺寸鎖定與網格防跑版鐵律 (Input Size & Grid Layout Protocol)

針對「每次新增操作頁面，輸入框大小隨字體變大變小而非合理分配大小」之問題，必須強制執行以下規範：

### A. 單行輸入框 (Single-line Input / TextBox)
1. **強制鎖定元件高度 (Lock Height)**：
   - WPF：必須顯式宣告 `Height="24"` 或 `Height="26"`，**嚴格禁止使用 `Height="Auto"` 放任文字字級動態撐開**。
   - Web：必須宣告 `height: 26px; box-sizing: border-box;`。
2. **文字垂直絕對置中 (Vertical Centering)**：
   - WPF：必須指定 `VerticalContentAlignment="Center"`，確保字型切換或字號微調時文字維持垂直居中，不抖動不貼頂。
   - Web：使用 `line-height: 26px;` 或 Flex 垂直居中。
3. **固定內邊距 (Padding)**：
   - WPF 統一 `Padding="6,0,6,0"` 或 `Padding="4,2"`；Web 統一 `padding: 0 8px;`。
4. **寬度合理比例分配 (Proportional Grid Width)**：
   - **表單網格**：一律採用 `Grid`，標籤欄設為 `Width="Auto"`，輸入欄設為 `Width="*"`（彈性比例分配）並搭配 `MinWidth="80"`，絕不讓輸入文字拉扯欄位寬度。
   - **水平並排**：在 `StackPanel` 或 `WrapPanel` 內，每個輸入框**必須顯式給定具體寬度**（如前綴/後綴 `Width="38"`、名稱 `Width="85"`、鏈結 `Width="160"`、搜尋 `Width="260"`），禁止未給寬度任由文字拉伸。

### B. 多行輸入框與編輯器 (Multi-line / Code Editor)
1. **視窗網格自適應**：
   - 必須透過 `HorizontalAlignment="Stretch"` 與 `VerticalAlignment="Stretch"` 依附在 `Grid` 的 `*` 比例列（`RowDefinition Height="*"`）。
   - 內部啟用滾動條（`VerticalScrollBarVisibility="Auto"`），**嚴禁因輸入內容增加而將視窗撐破或推擠到底部按鈕**。

---

## 📊 3. 表格與資料網格深色/淺色深度樣式化規範 (Table & DataGrid Deep Styling)

在任何主題中，表格標題列（Header）極易殘留系統預設灰白底，必須執行深度樣式化：

### A. C# WPF (`DataGrid` / `ListView`)
1. **`DataGridColumnHeader`**：
   - 淺色主題：背景設為 `#F1F5F9`、文字設為 `#1E293B`、邊框 `#CBD5E1`。
   - 深色主題：背景設為 `#334155`、文字設為 `#F8FAFC`、邊框 `#475569`。
2. **`RowHeaderWidth="0"`**：**關鍵！** 強制消除左側預設 RowHeader 與左上角 `SelectAllButton` 殘留的系統白底。
3. **`RowHeight="26"` 或 `28`**：固定行高，消除文字長度變動造成的行高跳動。
4. **`DataGridRow`**：設定 `AlternationCount="2"`，以 Trigger 實現奇偶交替色與懸停色。

---

## 🌐 4. Web 網頁佈局防呆鐵律
1. **CSS 變數封裝先行**：`:root` 定義全套變數，全篇樣式嚴禁直接硬編碼 Hex 色碼。
2. **App Shell 視窗鎖定架構**：`html, body { height: 100vh; margin: 0; overflow: hidden; }`，禁止雙重捲軸。
3. **Flex / Grid 防撐破機制**：子容器必須顯式加上 `min-width: 0; min-height: 0;`。
4. **4px 幾何網格標準**：間距統一使用 `4px` 之倍數。

---

## 🖥️ 5. C# WPF 佈局防呆鐵律
1. **嚴禁絕對座標排版**：嚴禁使用大數值 `Margin="120,85,0,0"` 定位元件，一律採用 `Grid` 比例分行分列。
2. **ResourceDictionary 集中管理**：引用 `templates/WpfFormatStringTheme.xaml` (淺色) 或 `templates/WpfIndustrialTheme.xaml` (深色)。
3. **垂直置中與內距規範**：輸入框與按鈕統一設定 `VerticalContentAlignment="Center"`。

---

## 🪟 6. C# WinForms 佈局防呆鐵律
1. **禁止純絕對座標**：採用 `TableLayoutPanel`、`SplitContainer` 搭配 `Dock = Fill` 或 `Anchor`。
2. **高 DPI 與統一字體**：`this.AutoScaleMode = AutoScaleMode.Dpi;` 搭配 `Microsoft JhengHei UI` 或 `Segoe UI`。

---

## 📦 樣板庫引用清單
- **WPF 經典淺色主題 (FormatString 標準)**：`templates/WpfFormatStringTheme.xaml`
- **WPF 工業級深色主題**：`templates/WpfIndustrialTheme.xaml`
- **Web App Shell 與深色表格**：`templates/web-app-shell.css`