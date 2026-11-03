# Global Rule: UI Layout, Industrial Theme & Onboarding Standards
# 規則編號：01-ui-and-onboarding-standards
# 適用範圍：Web (HTML/CSS)、C# WPF (.xaml)、C# WinForms

---

## 🔍 Step 0: 環境與版本前置偵測 (Environment & Version Pre-flight Check)

在生成、修改或注入任何代碼之前，AI **必須先執行環境與版本前置檢查**：
1. **C# 專案版本判斷 (.csproj)**：.NET Framework 4.5~4.8 強制 C# 7.3 模式；.NET 6/8 自動適配 Nullable 與 ImplicitUsings；模板 100% 採 C# 7.3 標準。
2. **第三方 UI 套件衝突防呆**：偵測到 MaterialDesignThemes / HandyControl / MahApps.Metro / DevExpress 時，僅注入色彩變數與網格佈局，不覆蓋預設 ControlTemplate。
3. **Web 專案打包架構判定**：無 `package.json` 強制 Inline 原生模式；有 `package.json` 適配 ES Module / React / Vue。

---

## 🎨 1. 雙主題工控色票規範 (Dual Industrial Theme Standards)

系統支援兩種通過 WCAG AA 對比度檢驗的專業工控主題。**嚴禁隨機硬編碼純原色（如 `#FF0000`, `#0000FF`）**：

### ☀️ 主題 A：FormatString 經典工控淺色主題 (Industrial Slate Light) —— 適用於資料密集、常規工控頁面
- **主畫布底色 (Bg-Primary)**：`#F7F9FC` (清爽護眼冷灰白)
- **頂部工具列/標題列 (Bg-Elevated)**：`#F1F5F9`，邊框 `#CBD5E1`
- **卡片/面板底色 (Bg-Surface)**：`#FFFFFF` (純白卡片)，邊框 `#E2E8F0`，圓角 `4~6px`
- **表格偶數列交替色 (Bg-RowAlt)**：`#F8FAFC`
- **主文字 (Text-Primary)**：`#1A252F` | 標題文字：`#1E293B`
- **次要文字/標籤 (Text-Muted)**：`#64748B` (Slate 500)
- **核心操作色 (Primary)**：`#0284C7` (Sky 600) / `#3498DB`
- **成功/儲存色 (Success)**：`#10B981` (Emerald 500) / `#27AE60`
- **高階/掃描色 (Accent)**：`#8B5CF6` (Violet 500) / `#8E44AD`
- **狀態摘要條 (Status Alert)**：背景 `#E0F2FE`，邊框 `#BAE6FD`，文字 `#0369A1`

### 🌙 主題 B：工業級深色主題 (Industrial Slate Dark) —— 適用於夜間、光學視覺檢測暗室
- **主畫布底色 (Bg-Primary)**：`#0F172A` (Slate 900)
- **卡片/面板底色 (Bg-Surface)**：`#1E293B` (Slate 800)
- **懸停/輸入框/表頭 (Border & Bg-Elevated)**：`#334155` (Slate 700)
- **表格偶數列交替色 (Bg-RowAlt)**：`#162032` (Slate 850)
- **主文字 (Text-Primary)**：`#F8FAFC` (Slate 50)
- **次要文字/標籤 (Text-Muted)**：`#94A3B8` (Slate 400)
- **核心操作色 (Primary)**：`#0284C7` (Sky 600) / Hover: `#0369A1`
- **語意狀態色**：正常 `#10B981` | 警告 `#F59E0B` | 異常 `#EF4444`

---

## 📐 2. 輸入框尺寸鎖定與網格排版防跑版鐵律 (Input Size & Grid Layout Protocol)

針對「每次新增操作頁面，輸入框（TextBox）大小隨字體變大變小而非合理分配大小」之問題，所有工程師與 AI 必須強制執行以下規範：

### A. 單行輸入框 (Single-line Input / TextBox)
1. **嚴格鎖定元件高度**：
   - WPF：必須顯式宣告 `Height="24"` 或 `Height="26"`，**嚴禁設為 `Height="Auto"` 或放任文字字級動態撐高**。
   - Web：必須設定 `height: 26px; box-sizing: border-box;`。
2. **文字垂直絕對置中**：
   - WPF：必須指定 `VerticalContentAlignment="Center"`，確保字型切換或字級微調時文字始終維持垂直置中，不貼頂、不抖動。
   - Web：使用 `display: flex; align-items: center;` 或 `line-height: 26px;`。
3. **固定內邊距**：
   - WPF 統一 `Padding="6,0,6,0"` 或 `Padding="4,2"`；Web 統一 `padding: 0 8px;`。
4. **合理比例寬度分配**：
   - **表單欄位**：外層必須採用 `Grid`，標籤欄設為 `Width="Auto"`，輸入框欄設為 `Width="*"`（彈性比例分配）並搭配 `MinWidth="80"`，杜絕文字動態拉扯容器寬度。
   - **並排與工具列**：在 `StackPanel` 或 `WrapPanel` 中，每個輸入框**必須顯式給定具體寬度**（例如前綴/後綴 `Width="38"`、名稱 `Width="85"`、鏈結 `Width="160"`、搜尋 `Width="260"`），禁止未給寬度任由文字拉伸。

### B. 多行輸入框與編輯器 (Multi-line Text / Code Editor)
1. **視窗網格自適應**：
   - 必須設定 `HorizontalAlignment="Stretch"` 與 `VerticalAlignment="Stretch"`。
   - 必須由外層 `Grid` 的彈性比例列（`RowDefinition Height="*"`）承接其空間，內部啟用滾動條（`VerticalScrollBarVisibility="Auto"`），**嚴禁因輸入內容增加而將視窗撐破或推擠到底部按鈕**。

---

## 📊 3. 表格與資料網格深色/淺色深度樣式化規範 (Table & DataGrid Deep Styling)

在任何主題中，表格標題列與邊角極易殘留系統預設灰白底，必須執行深度重寫：

### A. Web (`<table>` / `<thead>` / `<th>`)
- 設定 `thead th { background-color: var(--bg-elevated) !important; color: var(--text-primary) !important; position: sticky; top: 0; }`。
- 偶數列設定 `tbody tr:nth-child(even) { background-color: var(--bg-row-alt); }`。

### B. C# WPF (`DataGrid` / `ListView`)
- **嚴禁只設定 `DataGrid.Background`！**
- 必須定義 `DataGridColumnHeader`（重寫 ControlTemplate，淺色底 `#F1F5F9` 或深色底 `#334155`）。
- 強制設定 `RowHeaderWidth="0"`，消除左側空白標頭與左上角全選按鈕的系統底色。
- 設定 `RowHeight="26"` 或 `28`，並為 DataGridCell 指定相應的高亮選取色。

### C. C# WinForms (`DataGridView`)
- 強制設定 `dataGridView.EnableHeadersVisualStyles = false;`（關鍵！否則系統白底無法覆寫）。
- 依主題指定 `ColumnHeadersDefaultCellStyle` 的 `BackColor` 與 `ForeColor`。
- `RowHeadersVisible = false;`。

---

## 🛡️ 4. 佈局防跑版三大架構鐵律
- **Web (HTML/CSS)**：CSS 變數先行；App Shell 100vh 視窗鎖定（杜絕雙重滾動條）；Flex/Grid 子容器必加 `min-width: 0; min-height: 0;`。
- **C# WPF (.xaml)**：嚴禁大數值 Margin 座標定位，一律採用 Grid 比例排版；Style 集中於 ResourceDictionary。
- **C# WinForms**：禁止純絕對座標，採用 TableLayoutPanel + Dock.Fill；啟用 AutoScaleMode.Dpi 與微軟正黑體。

---

## 🚀 5. 實戰互動導引 (Interactive Action Guide) 鐵律
- **四周背景鎖死，洞口穿透操作 (Click-Through)**。
- **監聽真實事件自動推進**：點擊按鈕 `click`、輸入驗證 `input`、選單切換 `change` 完成後自動進下一步。
- **收斂 4~7 步**，語意錨點綁定，隱藏頁籤在 `beforeShow` 鉤子中自動切換。