# UI Layout & Theme Standards (全平台介面佈局與配色防呆鐵律)

本規則檔適用於所有前端 Web 應用（HTML / CSS / React / Vue）以及 C# 桌面應用（WPF / WinForms）。所有 AI 在生成、修改或重構介面時，**必須無條件遵守以下防呆鐵律**。

---

## 🎨 1. 統一色票系統（禁止隨機硬編碼色碼）

所有介面必須預設採用「**工業級深色主題（Industrial Slate Dark）**」或「**高對比專業淺色主題**」，**嚴禁使用未經調和的刺眼純色**（如 `#FF0000`, `#0000FF`, `#00FF00`）。

### 1.1 工業級深色主題（Dark Mode 預設標準）
- **背景階層 (Background Hierarchy)**：
  - 主背景 (Bg-Primary)：`#0F172A` (Slate 900) —— 視窗主畫布、最底層容器
  - 卡片/面板背景 (Bg-Surface)：`#1E293B` (Slate 800) —— 內容卡片、側邊欄、對話框
  - 懸停/次要區塊 (Bg-Elevated)：`#334155` (Slate 700) —— 按鈕 Hover、表格標題、標籤背景
  - 邊框色 (Border)：`#334155` —— 1px solid 結構分割線
- **文字階層 (確保通過 WCAG AA 高對比標準)**：
  - 主文字 (Text-Primary)：`#F8FAFC` (Slate 50) —— 嚴禁在深色背景使用深灰色或黑色字
  - 次要文字/標籤 (Text-Muted)：`#94A3B8` (Slate 400) —— 輔助說明、副標題、單位
- **語意狀態色 (Semantic Status Colors)**：
  - 主操作色 (Primary)：`#0284C7` (Sky 600) / 懸停態 (Hover)：`#0369A1` (Sky 700)
  - 正常 / 運轉中 (Success)：`#10B981` (Emerald 500)
  - 警告 / 待機 (Warning)：`#F59E0B` (Amber 500)
  - 異常 / 警報 (Danger)：`#EF4444` (Red 500)

---

## 🌐 2. Web 網頁 (HTML/CSS) 佈局防呆鐵律

1. **CSS 變數先行**：
   - 必須在 `:root` 中宣告色彩與間距變數，全站 CSS 嚴禁直接硬編碼（Hardcode）Hex 色碼。
2. **App Shell 視窗鎖定架構**：
   - 工具類、儀表板類網頁必須設定 `html, body { height: 100vh; overflow: hidden; }`。
   - 採用 `display: flex` 或 `display: grid` 構建頂部導航、側邊欄與主要工作區。
   - **全域禁止雙重滾動條**：僅允許主要內容區（`main`）或特定數據表格區塊獨立捲動（`overflow-y: auto; overflow-x: hidden;`）。
3. **Flex / Grid 防撐破機制**：
   - 所有放置於 Flex 或 Grid 內部的子容器，**必須顯式加上 `min-width: 0; min-height: 0;`**。
   - 包含 `<canvas>`、`<table>` 或長字串文字區塊的容器，必須加上 `overflow: hidden; text-overflow: ellipsis;`，防止破壞整體 Grid 佈局。
4. **4px 幾何網格間距標準**：
   - 所有元素外距（margin）、內距（padding）與間隔（gap）統一使用 `4px` 之倍數：`4px`, `8px`, `12px`, `16px`, `24px`, `32px`。

---

## 🖥️ 3. C# WPF (.xaml) 佈局與配色防呆鐵律

1. **嚴禁絕對座標排版 (Zero Margin Coordinate Positioning)**：
   - **嚴格禁止**使用大數值 Margin（如 `Margin="120,85,0,0"`、`Margin="240,310,40,20"`）進行「假排版」。
   - 必須使用 `Grid`（顯式定義 `RowDefinitions` 與 `ColumnDefinitions`，善用 `*` 比例與 `Auto` 彈性伸縮），或組合 `DockPanel`、`StackPanel`。
2. **ResourceDictionary 集中管理**：
   - 所有色票必須在 `Window.Resources` 或 `App.xaml` 中定義成 `SolidColorBrush` 資源索引鍵（Key），例如 `Brush.Bg.Primary`、`Brush.Text.Primary`。
   - 控制項外觀透過全域 `Style` 統一管理（統一 `Button`、`TextBox`、`Border` 圓角 `CornerRadius="6"`），嚴禁在各控制項上散落硬編碼屬性。
3. **垂直置中與內距規範**：
   - 所有 `TextBox`、`ComboBox`、`Button` 必須設定 `VerticalContentAlignment="Center"`，避免文字貼緊頂部。
   - 控制項周圍間距透過統一外層 `Margin="0,0,0,8"` 或容器 `Padding="12"` 控制。

---

## 🪟 4. C# WinForms 佈局防呆鐵律

1. **嚴禁純絕對座標定位 (Zero Raw Location Coupling)**：
   - 複雜商務或儀表板表單**必須採用 `TableLayoutPanel`、`SplitContainer` 與 `FlowLayoutPanel`** 進行流動與響應式排版。
   - 各區塊容器必須正確設定 `Dock = DockStyle.Fill` / `Top` / `Left` 或透過 `Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right` 支援視窗縮放。
2. **高 DPI 與統一字體修正**：
   - 主 Form 構造函式或 `Main()` 中必須明確設定 `this.AutoScaleMode = AutoScaleMode.Dpi`。
   - 預設字體一律顯式指派為現代字體：`new Font("Microsoft JhengHei UI", 9.5F, FontStyle.Regular)` 或 `Segoe UI`，嚴禁使用舊式「新細明體」。
3. **元件雙緩衝與防閃爍**：
   - 自訂繪圖或複雜 Form 必須啟用 `this.DoubleBuffered = true;`。

---

## 🛑 違規代碼 vs 正確代碼對照表

| 平台 | 違規寫法（嚴禁） | 正確寫法（標準規範） |
| :--- | :--- | :--- |
| **Web** | `background: #111; color: #888;`（對比不足） | `background: var(--bg-surface); color: var(--text-primary);` |
| **Web** | 未加 `min-width: 0` 導致 Flex 內的 Table 撐爆視窗 | `.flex-child { flex: 1; min-width: 0; overflow-x: auto; }` |
| **WPF** | `<Button Margin="214,120,0,0" Width="100"/>` | `<Grid><Grid.RowDefinitions>...<Button Grid.Row="1"/>` |
| **WinForms** | 幾十個 Control 使用設計器座標硬卡在固定 Form 上 | 使用 `TableLayoutPanel` 分割行列 + `Dock = Fill` |
