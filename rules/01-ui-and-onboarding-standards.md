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

## 🎨 1. 工業級深色主題 (Industrial Slate Dark)
- **主畫布底色 (Bg-Primary)**：`#0F172A` (Slate 900)
- **卡片/面板底色 (Bg-Surface)**：`#1E293B` (Slate 800)
- **懸停/輸入框/表頭 (Border & Bg-Elevated)**：`#334155` (Slate 700)
- **表格偶數列交替色 (Bg-RowAlt)**：`#162032` (Slate 850)
- **主文字 (Text-Primary)**：`#F8FAFC` (Slate 50) —— 嚴禁在深色背景使用深灰色字
- **次要文字/標籤 (Text-Muted)**：`#94A3B8` (Slate 400)
- **核心操作色 (Primary)**：`#0284C7` (Sky 600) / Hover: `#0369A1`
- **語意狀態色**：正常 `#10B981` | 警告 `#F59E0B` | 異常 `#EF4444`

---

## 📊 2. 表格與資料網格強制深色化規範 (Table & DataGrid Deep Styling)

在深色主題中，**表格標題列（Header）極易殘留系統預設白底**，必須執行以下深度深色化鐵律：

### A. Web (`<table>` / `<thead>` / `<th>`)
- 強制設定 `thead th { background-color: var(--bg-elevated) !important; color: var(--text-primary) !important; position: sticky; top: 0; }`。
- 偶數列設定 `tbody tr:nth-child(even) { background-color: var(--bg-row-alt); }`。

### B. C# WPF (`DataGrid` / `ListView`)
- **嚴禁只設定 `DataGrid.Background`！**
- 必須定義 `DataGridColumnHeader`（背景 `#334155`、文字 `#F8FAFC`、重寫 ControlTemplate 消除系統白框）。
- 強制設定 `RowHeaderWidth="0"`，消除左側空白標頭與左上角全選按鈕的白底。
- 設定 `AlternationCount="2"` 與 `DataGridCell` 選取態背景 `#0284C7`。

### C. C# WinForms (`DataGridView`)
- 強制設定 `dataGridView.EnableHeadersVisualStyles = false;`（關鍵！否則系統白底無法覆寫）。
- 設定 `ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 65, 85);`、`ForeColor = Color.FromArgb(248, 250, 252);`。
- 網格底色設為 `#0F172A`，交替列設為 `#162032`，`RowHeadersVisible = false;`。

---

## 🛡️ 3. 佈局防跑版三大鐵律
- **Web (HTML/CSS)**：CSS 變數先行；App Shell 100vh 視窗鎖定（杜絕雙重滾動條）；Flex/Grid 子容器必加 `min-width: 0; min-height: 0;`。
- **C# WPF (.xaml)**：嚴禁大數值 Margin 座標定位，一律採用 Grid 比例排版；Style 集中於 ResourceDictionary。
- **C# WinForms**：禁止純絕對座標，採用 TableLayoutPanel + Dock.Fill；啟用 AutoScaleMode.Dpi 與微軟正黑體。

---

## 🚀 4. 實戰互動導引 (Interactive Action Guide) 鐵律
- **四周背景鎖死，洞口穿透操作 (Click-Through)**。
- **監聽真實事件自動推進**：點擊按鈕 `click`、輸入驗證 `input`、選單切換 `change` 完成後自動進下一步。
- **收斂 4~7 步**，語意錨點綁定，隱藏頁籤在 `beforeShow` 鉤子中自動切換。
