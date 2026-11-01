---
name: ui-layout-theme-guard
category: UI/UX
description: 專業級 UI 佈局防跑版與工業色票守護者。自動審查並重構 Web (HTML/CSS)、C# WPF 與 C# WinForms 介面代碼，根治版面跑版、元件重疊、邊界撐破與顏色對比不佳問題。
auto_invoke: always
file_patterns: "*.html, *.css, *.jsx, *.tsx, *.vue, *.xaml, *.cs"
trigger_condition: 當生成、修改或美化任何 Web 網頁 (HTML/CSS) 或 C# (WPF/WinForms) 介面代碼時，必須主動套用此規範防止跑版與刺眼配色。
---

# UI Layout & Industrial Theme Guard

本 Skill 為 UI 佈局防跑版與工業風配色的最高執行準則。在編寫、修改或重構任何 Web 前端（HTML/CSS）或 C# 桌面應用（WPF/WinForms）時，**必須自動且無條件套用以下規範**。

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

## 🎨 1. 統一色票系統 (Industrial Slate Dark)
所有介面預設遵循此套經過 WCAG AA 對比度檢驗的專業工業級色票，**嚴禁隨機硬編碼刺眼純色（如 `#FF0000`, `#0000FF`, `#00FF00`）**：

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
| **`Success`** | `#10B981` (Emerald 500) | 正常運轉、資料成功寫入、上漲指標 | 文字搭配半透明綠底 |
| **`Warning`** | `#F59E0B` (Amber 500) | 警告狀態、數值趨近上限、待處理 | 文字搭配半透明黃底 |
| **`Danger`** | `#EF4444` (Red 500) | 錯誤、警報、刪除動作、異常中斷 | 文字搭配半透明紅底 |

---

## 📊 2. 表格與資料網格強制深色化規範 (Table & DataGrid Deep Styling)

在深色主題中，**表格標題列（Header）極易殘留系統預設白底**，必須執行以下深度深色化鐵律：

### A. Web (`<table>` / `<thead>` / `<th>`)
```css
/* 強制設定 thead th 深色底色與黏性置頂，防止白底或文字看不清 */
.table-container {
  width: 100%;
  overflow-x: auto;
  border: 1px solid var(--border-color);
  background-color: var(--bg-surface);
}

thead th {
  background-color: var(--bg-elevated) !important;
  color: var(--text-primary) !important;
  font-weight: 700;
  padding: 12px 16px;
  border-bottom: 2px solid var(--border-color);
  position: sticky;
  top: 0;
  z-index: 2;
  white-space: nowrap;
}

tbody tr:nth-child(even) { background-color: var(--bg-row-alt); }
tbody tr:hover { background-color: var(--bg-elevated); }
```

### B. C# WPF (`DataGrid` / `ListView`)
**嚴禁只設定 `DataGrid.Background`！** 必須同時設定：
1. **`DataGridColumnHeader`**：背景設為 `#334155`、文字設為 `#F8FAFC`，重寫 `ControlTemplate` 消除系統預設灰色/白底立體外框。
2. **`RowHeaderWidth="0"`**：**關鍵！** 強制消除左側預設 RowHeader 與左上角 `SelectAllButton` 殘留的系統白底。
3. **`DataGridRow`**：設定 `AlternationCount="2"`，以 `Trigger` 實現奇偶交替色（`#1E293B` / `#162032`）與懸停態（`#334155`）。
4. **`DataGridCell`**：選取態背景設為 `#0284C7`，文字設為 `#FFFFFF`。

### C. C# WinForms (`DataGridView`)
**嚴禁僅設定 `dataGridView.BackgroundColor`！** 必須強制執行以下配置：
```csharp
// 關鍵：必須先關閉作業系統原生樣式，自訂標題列樣式才會生效！
dataGridView.EnableHeadersVisualStyles = false;

// 標題列深色化 (Header)
dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 65, 85);   // #334155
dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(248, 250, 252); // #F8FAFC
dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft JhengHei UI", 9.5F, FontStyle.Bold);
dataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

// 網格主體深色化
dataGridView.BackgroundColor = Color.FromArgb(15, 23, 42);                           // #0F172A
dataGridView.DefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59);               // #1E293B
dataGridView.DefaultCellStyle.ForeColor = Color.FromArgb(248, 250, 252);
dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 32, 50); // #162032
dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(2, 132, 199);     // #0284C7
dataGridView.DefaultCellStyle.SelectionForeColor = Color.White;
dataGridView.GridColor = Color.FromArgb(51, 65, 85);
dataGridView.RowHeadersVisible = false; // 隱藏左側空白標頭
```

---

## 🌐 3. Web 網頁佈局防呆鐵律
1. **CSS 變數封裝先行**：`:root` 定義全套變數，全篇樣式嚴禁直接硬編碼 Hex 色碼。
2. **App Shell 視窗鎖定架構**：`html, body { height: 100vh; margin: 0; overflow: hidden; }`，禁止雙重捲軸。
3. **Flex / Grid 防撐破機制**：子容器必須顯式加上 `min-width: 0; min-height: 0;`。
4. **4px 幾何網格標準**：間距統一使用 `4px` 之倍數。

---

## 🖥️ 4. C# WPF 佈局防呆鐵律
1. **嚴禁絕對座標排版**：嚴禁使用大數值 `Margin="120,85,0,0"` 定位元件，一律採用 `Grid` 比例分行分列。
2. **ResourceDictionary 集中管理**：引用 `templates/WpfIndustrialTheme.xaml`，全域套用控制項樣式。
3. **垂直置中與內距規範**：輸入框與按鈕統一設定 `VerticalContentAlignment="Center"`。

---

## 🪟 5. C# WinForms 佈局防呆鐵律
1. **禁止純絕對座標**：採用 `TableLayoutPanel`、`SplitContainer` 搭配 `Dock = Fill` 或 `Anchor`。
2. **高 DPI 與統一字體**：`this.AutoScaleMode = AutoScaleMode.Dpi;` 搭配 `Microsoft JhengHei UI` 或 `Segoe UI`。

---

## 📦 零依賴樣板庫引用
- **Web App Shell 與深色表格**：`templates/web-app-shell.css`
- **WPF ResourceDictionary (含 DataGrid 深度深色化)**：`templates/WpfIndustrialTheme.xaml`
