# Global Rule: UI Layout, Industrial Theme & Onboarding Standards
# 規則編號：01-ui-and-onboarding-standards
# 適用範圍：Web (HTML/CSS)、C# WPF (.xaml)、C# WinForms

---

## 🔍 Step 0: 環境與版本前置偵測 (Environment & Version Pre-flight Check)

在生成、修改或注入任何代碼之前，AI **必須先執行環境與版本前置檢查**：
1. **C# 專案版本判斷 (.csproj)**：.NET Framework 4.5~4.8 強制 C# 7.3 模式；.NET 6/8 自動適配 Nullable 與 ImplicitUsings；模板 100% 採 C# 7.3 標準。
2. **第三方 UI 套件衝突防呆**：偵測到 MaterialDesignThemes / HandyControl / MahApps.Metro / DevExpress 時，僅注入色彩變數與網格佈局，不覆蓋預設 ControlTemplate。
3. **主頁面背景防穿透原則 (Background Penetration Prevention)**：
   - 任何自訂視窗或繼承 `ModuleViewBase` 的最外層 `<Grid>` 必須顯式指定 `Background="#F7F9FC"`（淺色）或 `#0F172A`（深色），**嚴禁留空 `<Grid>` 背景**，防止宿主暗色穿透。

---

## 🎨 1. 雙主題工控色票規範 (Dual Industrial Theme Standards)

系統支援兩種通過 WCAG AA 對比度檢驗的專業工控主題。**嚴禁隨機硬編碼純原色（如 `#FF0000`, `#0000FF`）**：

### ☀️ 主題 A：FormatString 經典工控淺色主題 (Industrial Slate Light)
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

### 🌙 主題 B：工業級深色主題 (Industrial Slate Dark)
- **主畫布底色 (Bg-Primary)**：`#0F172A` (Slate 900)
- **卡片/面板底色 (Bg-Surface)**：`#1E293B` (Slate 800)
- **懸停/輸入框/表頭 (Border & Bg-Elevated)**：`#334155` (Slate 700)
- **表格偶數列交替色 (Bg-RowAlt)**：`#162032` (Slate 850)
- **主文字 (Text-Primary)**：`#F8FAFC` (Slate 50)
- **次要文字/標籤 (Text-Muted)**：`#94A3B8` (Slate 400)
- **核心操作色 (Primary)**：`#0284C7` (Sky 600) / Hover: `#0369A1`

---

## 📊 2. DataGrid 兩大高可用性鐵律 (DataGrid Core Protocols)

### 鐵律 2.1：字串換行符號自適應欄高 (Dynamic Row Height on Line Breaks) —— 參考 FormatString 標竿規範
當單元格字串（如多變量鏈結、文字、數值、描述）包含換行符號（`\r\n`）時，DataGrid 欄高必須自動一同伸展增高：
1. **DataGrid 屬性**：**強制指定 `RowHeight="NaN"` 與 `MinRowHeight="26"`**，嚴格禁止寫死數值行高（如 `RowHeight="26"`），否則多行文字將被強行截斷。
2. **DataGridRow 樣式**：設定 `<Setter Property="Height" Value="Auto"/>` 與 `<Setter Property="MinHeight" Value="26"/>`。
3. **多行變量鏈結單元格 (Multi-line VarLink)**：
   - 採用 `MultiLineVarLinkTextBoxStyle`：`Height="Auto"`、`MinHeight="24"`、`AcceptsReturn="True"`、`TextWrapping="NoWrap"`（或 `Wrap`）、`VerticalScrollBarVisibility="Disabled"`。
   - 同列操作按鈕（如 🔗）設定 `VerticalAlignment="Center"`，維持置中。
4. **文字欄位 (DataGridTextColumn)**：在 `ElementStyle` 中配置 `<Setter Property="TextWrapping" Value="Wrap"/>`。

### 鐵律 2.2：懸停 (Hover) 與選中 (Selected) 防白底白字對比度崩塌
1. **未選中懸停態 (`IsMouseOver = True, IsSelected = False`)**：
   - 背景設為柔和灰藍 `#E2E8F0`。
   - **文字前景色強制鎖死為深藍黑 `#1A252F`，絕對嚴禁省略！**
2. **選中態 (`IsSelected = True`)**：
   - 背景為高對比深操作藍 `#0284C7`。
   - 文字前景色為純白高對比 `#FFFFFF`。
3. **覆寫系統反白筆刷**：在 `DataGrid.Resources` 強制覆寫 `SystemColors.HighlightBrushKey` 與 `MahApps.Brushes.DataGrid.MouseOver.Text`。

---

## 📐 3. 輸入框尺寸鎖定與網格排版防跑版鐵律 (Input Size & Grid Layout Protocol)

針對「每次新增操作頁面，輸入框（TextBox）大小隨字體變大變小而非合理分配大小」之問題，強制執行以下規範：

1. **單行輸入框高度嚴格鎖定**：WPF 強制顯式指定 `Height="24"` 或 `Height="26"`，禁止使用 `Height="Auto"`。
2. **文字垂直絕對置中**：指定 `VerticalContentAlignment="Center"`，換字型或字號微調時文字維持垂直置中，不貼頂不抖動。
3. **內距固定**：統一 `Padding="6,0,6,0"` 或 `Padding="4,2"`。
4. **合理比例寬度分配**：表單排版一律使用 `Grid`（標籤列 `Width="Auto"`，輸入欄 `Width="*"` + `MinWidth="80"`），並排工具列輸入框必須顯式指定固定寬度。

---

## 🚀 4. 實戰互動導引 (Interactive Action Guide) 鐵律
- **四周背景鎖死，洞口穿透操作 (Click-Through)**。
- **監聽真實事件自動推進**：點擊按鈕 `click`、輸入驗證 `input`、選單切換 `change` 完成後自動進下一步。
- **收斂 4~7 步**，語意錨點綁定，隱藏頁籤在 `beforeShow` 鉤子中自動切換。
