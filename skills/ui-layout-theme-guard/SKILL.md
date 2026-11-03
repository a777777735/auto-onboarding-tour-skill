---
name: ui-layout-theme-guard
category: UI/UX
description: 專業級 UI 佈局防跑版、輸入框尺寸鎖定、DataGrid懸停對比防護與雙主題工控色票守護者。自動審查並重構 Web (HTML/CSS)、C# WPF 與 C# WinForms 介面代碼，根治輸入框大小失控、滑鼠懸停白底白字、背景黑色穿透與配色刺眼問題。
auto_invoke: always
file_patterns: "*.html, *.css, *.jsx, *.tsx, *.vue, *.xaml, *.cs"
trigger_condition: 當生成、修改或美化任何 Web 網頁 (HTML/CSS) 或 C# (WPF/WinForms) 介面代碼時，必須主動套用此規範防止輸入框大小暴走、DataGrid懸停白底白字、視窗底色黑透與刺眼配色。
---

# UI Layout & Industrial Theme Guard

本 Skill 為 UI 佈局防跑版、輸入框比例鎖定、DataGrid 高對比度防護與雙主題工業風配色的最高執行準則。在編寫、修改或重構任何 Web 前端（HTML/CSS）或 C# 桌面應用（WPF/WinForms）時，**必須自動且無條件套用以下規範**。

---

## 🔍 Step 0: 環境與版本前置偵測 (Environment & Version Pre-flight Check)

在產生或修改任何介面代碼前，AI **必須先執行環境與版本前置審查**：

1. **C# 專案版本判斷 (讀取 `.csproj`)**：
   - **.NET Framework 4.5~4.8**：強制切換為 **C# 7.3 嚴格相容模式**（傳統區塊 namespace、明確宣告型別、不使用 Nullable 參考型別標記 `?` 或 `switch` 運算式）。
   - **.NET 6 / .NET 8**：檢查 `<Nullable>` 與 `<ImplicitUsings>` 設定，靈活配合型別宣告與 `using` 引用。
2. **第三方 UI 套件衝突防呆**：
   - 檢查專案是否已引入第三方 UI 庫（如 `MaterialDesignThemes`、`HandyControl`、`MahApps.Metro`、`DevExpress`）。
   - **防呆保護**：若存在第三方 UI 庫，**僅注入色彩變數（`Color` / `SolidColorBrush`）與網格結構（`Grid` / `DockPanel`）**，嚴格禁止覆蓋第三方元件預設的 `ControlTemplate`，避免損毀原有視覺風格。
3. **主頁面背景防穿透原則 (Background Penetration Prevention)**：
   - 在任何自訂 UserControl、Window 或繼承基底視窗（如 `ModuleViewBase`）的 XAML 中，**最外層 `<Grid>` 必須顯式指定 `Background="#F7F9FC"`（淺色）或 `#0F172A`（深色）**。
   - **嚴禁留空 `<Grid>` 背景**，防止宿主容器或作業系統底層暗黑底色透出造成「主頁面黑底」嚴重 bug。

---

## 🎨 1. 雙主題工控色票規範 (Dual Industrial Theme Standards)

所有介面預設遵循此套經過 WCAG AA 對比度檢驗的專業工業級色票，**嚴禁隨機硬編碼刺眼純色（如 `#FF0000`, `#0000FF`）**：

### ☀️ A. FormatString 經典工控淺色主題 (Industrial Slate Light)
專為資料密集、參數配置、工控主控台打造的高清清爽色系：

| 色票名稱 | 色碼 (HEX) | 用途與規範說明 | 對比標準 |
| :--- | :--- | :--- | :--- |
| **`Bg-Primary`** | `#F7F9FC` (Slate 50) | 全域視窗畫布背景、最外層 Grid 背景 | 搭配 `Text-Primary` 清爽護眼 |
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
| **`Border`** | `#334155` (Slate 700) | 面板外框、格線邊框（1px solid） | 結構分界 |
| **`Text-Primary`** | `#F8FAFC` (Slate 50) | 標題文字、核心讀數、主按鈕文字 | 對比度 > 12:1 (極高) |

---

## 🚫 2. DataGrid 懸停 (Hover) 與選中 (Selected) 防白底白字鐵律

**致命問題成因**：WPF 或 MahApps 預設樣式在鼠標指到（Hover / MouseOver）資料行時，經常觸發高亮反白（將文字設為白色），若背景又是淺色/白色，**會導致鼠標指到的文字與背景一同變白，使用者完全看不見內容！**

### 強制執行標準：
1. **未選中懸停態 (`IsMouseOver = True, IsSelected = False`)**：
   - **背景**：必須設為柔和灰藍 `#E2E8F0`。
   - **文字前景色**：**必須顯式鎖定為深藍黑 `#1A252F`，絕對嚴禁省略！**
2. **選中態 (`IsSelected = True`)**：
   - **背景**：高對比鮮明操作藍 `#0284C7`。
   - **文字前景色**：純白高對比 `#FFFFFF`。
3. **覆寫系統反白筆刷**：在 `DataGrid.Resources` 中強制覆寫 `SystemColors.HighlightBrushKey`、`SystemColors.HighlightTextBrushKey` 與 `MahApps.Brushes.DataGrid.MouseOver.Text`。

---

## 📐 3. 輸入框尺寸鎖定與網格防跑版鐵律 (Input Size & Grid Layout Protocol)

針對「每次新增操作頁面，輸入框大小隨字體變大變小而非合理分配大小」之問題，必須強制執行以下規範：

1. **強制鎖定元件高度 (Lock Height)**：
   - WPF：必須顯式宣告 `Height="24"` 或 `Height="26"`，**嚴格禁止使用 `Height="Auto"` 放任文字字級動態撐開**。
   - Web：必須宣告 `height: 26px; box-sizing: border-box;`。
2. **文字垂直絕對置中 (Vertical Centering)**：
   - WPF：必須指定 `VerticalContentAlignment="Center"`，確保字型切換或字號微調時文字維持垂直居中，不抖動不貼頂。
3. **固定內邊距 (Padding)**：
   - WPF 統一 `Padding="6,0,6,0"` 或 `Padding="4,2"`；Web 統一 `padding: 0 8px;`。
4. **寬度合理比例分配 (Proportional Grid Width)**：
   - 表單網格一律採用 `Grid`：標籤欄設為 `Width="Auto"`，輸入欄設為 `Width="*"`（彈性比例分配）並搭配 `MinWidth="80"`。
   - 水平並排容器中，每個輸入框必須顯式指定寬度（如前綴/後綴 `Width="38"`、名稱 `Width="85"`、搜尋 `Width="260"`）。
