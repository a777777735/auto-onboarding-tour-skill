# Global Rule: UI Layout & Industrial Theme Standards
# 規則編號：01-ui-layout-and-theme
# 適用範圍：Web (HTML/CSS/React/Vue)、C# WPF (.xaml)、C# WinForms

---

## 🎯 核心宗旨
本規則為全域常駐鐵律。所有 AI 在生成、修改、美化或重構任何用戶介面（Web 網頁或 C# 桌面視窗）時，**必須無條件遵守以下防呆規範**，杜絕「介面跑版、元件重疊、邊界撐破、色彩刺眼與對比不足」等常見缺陷。

---

## 🎨 1. 統一工業級色票系統 (Industrial Slate Dark)
所有介面預設遵循此套經過 WCAG AA 對比度檢驗的專業工業級色票，**嚴禁隨機硬編碼純原色（如 `#FF0000`, `#0000FF`, `#00FF00`）**：

| 色票識別 | 色碼 (HEX) | 用途與規範說明 | 對比標準 |
| :--- | :--- | :--- | :--- |
| **`Bg-Primary`** | `#0F172A` (Slate 900) | 全域視窗畫布背景、最底層容器 | 搭配 `Text-Primary` |
| **`Bg-Surface`** | `#1E293B` (Slate 800) | 卡片容器、面板、側邊導航背景 | 搭配 `Border` 柔和分割 |
| **`Bg-Elevated`** | `#334155` (Slate 700) | 按鈕 Hover 態、輸入框底色、標籤背景 | 強化階層感 |
| **`Border`** | `#334155` (Slate 700) | 面板外框、格線邊框（1px solid） | 結構分界 |
| **`Text-Primary`** | `#F8FAFC` (Slate 50) | 標題文字、核心讀數、主按鈕文字 | 對比度 > 12:1 (極高) |
| **`Text-Muted`** | `#94A3B8` (Slate 400) | 次要文字、輸入框 Placeholder、時間標籤 | 符合 WCAG AA |
| **`Primary`** | `#0284C7` (Sky 600) | 核心動作按鈕、聚焦光圈 (Focus Ring) | Hover: `#0369A1` |
| **`Success`** | `#10B981` (Emerald 500) | 正常運轉、資料成功寫入、上漲指標 | 文字搭配半透明綠底 |
| **`Warning`** | `#F59E0B` (Amber 500) | 警告狀態、數值趨近上限、待處理 | 文字搭配半透明黃底 |
| **`Danger`** | `#EF4444` (Red 500) | 錯誤、警報、刪除動作、異常中斷 | 文字搭配半透明紅底 |

---

## 🌐 2. Web (HTML / CSS) 佈局防呆鐵律
1. **CSS 變數封裝先行**：
   - 必須於 `:root` 定義全套 `--bg-*`、`--text-*`、`--primary` 等變數，全篇樣式嚴禁直接硬編碼 Hex 色碼。
2. **App Shell 視窗鎖定架構**：
   - 工具類與儀表板網頁必須設定 `html, body { height: 100vh; margin: 0; overflow: hidden; }`。
   - 禁止出現視窗層級與內容層級的「雙重捲軸」。僅允許工作內容區（`main`）或數據表格區獨立滾動（`overflow-y: auto; overflow-x: hidden;`）。
3. **Flex / Grid 防撐破機制**：
   - 任何放置於 Flex 或 Grid 內部的子項目，**必須顯式加上 `min-width: 0; min-height: 0;`**。
   - 包含 `<canvas>`、`<table>` 或長字串文字區塊的容器，必須加上 `overflow: hidden; text-overflow: ellipsis;`，防止破壞整體網格。
4. **4px 幾何網格標準**：
   - 所有元素外距（margin）、內距（padding）與間隔（gap）統一使用 `4px` 之倍數：`4px`, `8px`, `12px`, `16px`, `24px`, `32px`。

---

## 🖥️ 3. C# WPF (.xaml) 佈局防呆鐵律
1. **嚴禁絕對座標排版 (Zero Margin Coordinate Positioning)**：
   - **嚴格禁止**使用大數值 Margin（如 `Margin="215,86,0,0"`）來定位元件。
   - 必須採用 `Grid`（顯式定義 `RowDefinitions` 與 `ColumnDefinitions`，善用 `*` 比例與 `Auto` 彈性伸縮），或組合 `DockPanel`、`StackPanel`。
2. **ResourceDictionary 集中管理**：
   - 所有筆刷必須在 `Window.Resources` 或 `App.xaml` 中宣告為 `SolidColorBrush` 資源索引鍵（Key）。
   - 控制項外觀透過全域 `Style` 統一管理（統一 `Button`、`TextBox`、`Border` 圓角 `CornerRadius="6"`），嚴禁個別硬編碼。
3. **垂直置中與內距規範**：
   - 所有 `TextBox`、`ComboBox`、`Button` 必須設定 `VerticalContentAlignment="Center"`，文字不得貼緊頂部。

---

## 🪟 4. C# WinForms 佈局防呆鐵律
1. **嚴禁純絕對座標定位 (Zero Raw Location Coupling)**：
   - 複雜表單**必須採用 `TableLayoutPanel`、`SplitContainer` 與 `FlowLayoutPanel`** 進行響應式排版。
   - 容器與元件必須正確指派 `Dock = DockStyle.Fill` 或 `Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right`。
2. **高 DPI 與統一字體修正**：
   - Form 初始化必須設定 `this.AutoScaleMode = AutoScaleMode.Dpi;` 與 `this.DoubleBuffered = true;`。
   - 預設字體一律顯式指派為現代字體：`new Font("Microsoft JhengHei UI", 9.5F, FontStyle.Regular)` 或 `Segoe UI`，嚴禁使用舊式「新細明體」。
