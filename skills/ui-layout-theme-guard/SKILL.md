---
name: ui-layout-theme-guard
description: 專業級 UI 佈局防呆與工業色票守護者。自動審查並重構 Web (HTML/CSS)、C# WPF 與 C# WinForms 的介面代碼，根治版面跑版、元件重疊、邊界撐破與顏色對比不佳問題。
---

# UI Layout & Theme Guard Skill

當使用者要求「**美化介面、修正跑版、套用標準色票、重構 UI Layout**」時，AI Agent 應自動觸發本 Skill，執行結構化審查與重構。

---

## 🎯 觸發關鍵字 (Trigger Keywords)
- `修正跑版`, `美化介面`, `套用色票`, `深色模式`, `Dark Mode`
- `元件重疊`, `撐破邊界`, `雙重捲軸`, `對比度不足`, `WCAG`
- `WPF 排版修正`, `WinForms DPI 修正`, `UI Layout Guard`

---

## 🔍 重構執行協定 (Remediation Protocol)

### Step 1: 專案類型識別
- **Web / HTML 專案**：審查 CSS 是否已宣告變數、是否採用 App Shell 鎖定結構、Flex/Grid 是否具備 `min-width: 0`。
- **C# WPF 專案**：審查 XAML 是否含有大量大數值 Margin 座標定位、是否引用了 `WpfThemeResource.xaml`。
- **C# WinForms 專案**：審查 Form 是否包含 `TableLayoutPanel`、是否開啟 `AutoScaleMode.Dpi`、字體是否設定為微軟正黑體或 Segoe UI。

### Step 2: 套用工業色票 (Industrial Slate Dark)
將所有未經調和的純色（如 `#FF0000`, `#0000FF`）替換為工業級語意狀態色：
- 主背景: `#0F172A` (Slate 900)
- 卡片面板: `#1E293B` (Slate 800)
- 邊框與次要區塊: `#334155` (Slate 700)
- 主文字: `#F8FAFC` (Slate 50)
- 次要標籤: `#94A3B8` (Slate 400)
- 操作強調: `#0284C7` (Sky 600)

### Step 3: 邊界與捲軸修正
- **Web**：設定 `html, body { height: 100vh; overflow: hidden; }`，滾動僅限於 main 容器。
- **WPF**：將絕對 Margin 元件改寫為 `Grid.RowDefinitions` 與 `Grid.ColumnDefinitions`。
- **WinForms**：將飄浮的 Control 改置入 `TableLayoutPanel` 並設定 `Dock = DockStyle.Fill`。

---

## 📋 開箱即用範本引用
- **Web CSS 變數與 App Shell**：直接引用 `templates/web-theme-vars.css`。
- **WPF ResourceDictionary**：直接複製 `templates/WpfThemeResource.xaml` 至 WPF 專案內並於 `App.xaml` 合併。
