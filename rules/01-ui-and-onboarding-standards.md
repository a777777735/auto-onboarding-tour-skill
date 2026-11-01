# Global Rule: UI Layout, Industrial Theme & Onboarding Standards
# 規則編號：01-ui-and-onboarding-standards
# 適用範圍：Web (HTML/CSS)、C# WPF (.xaml)、C# WinForms

---

## 🎨 1. 工業級深色主題 (Industrial Slate Dark)
所有前端網頁與 C# 桌面視窗**嚴禁使用刺眼純色（如 `#FF0000`, `#0000FF`）**，統一預設採用以下符合 WCAG AA 規範之色票：
- **主畫布底色 (Bg-Primary)**：`#0F172A` (Slate 900)
- **卡片/面板底色 (Bg-Surface)**：`#1E293B` (Slate 800)
- **懸停/輸入框/邊框 (Border & Bg-Elevated)**：`#334155` (Slate 700)
- **主文字 (Text-Primary)**：`#F8FAFC` (Slate 50) —— 嚴禁在深色背景使用深灰色字
- **次要文字/標籤 (Text-Muted)**：`#94A3B8` (Slate 400)
- **核心操作色 (Primary)**：`#0284C7` (Sky 600) / Hover: `#0369A1`
- **語意狀態色**：正常 `#10B981` | 警告 `#F59E0B` | 異常 `#EF4444`

---

## 🛡️ 2. 佈局防跑版三大鐵律

### A. Web (HTML / CSS)
1. **CSS 變數先行**：`:root` 定義色票與間距變數，全篇嚴禁寫死 Hex。
2. **App Shell 視窗鎖定**：工具類網頁設定 `height: 100vh; overflow: hidden;`，杜絕全域雙重捲軸；僅主要工作區獨立滾動（`overflow-y: auto`）。
3. **Flex / Grid 防撐破**：子容器必須加上 `min-width: 0; min-height: 0;`，防止 Table 或 Canvas 破版。

### B. C# WPF (.xaml)
1. **嚴禁絕對座標排版**：嚴禁使用大數值 `Margin="120,85,0,0"` 定位控制項。
2. **Grid 比例主導**：必須顯式宣告 `RowDefinitions` 與 `ColumnDefinitions`，善用 `*` 與 `Auto`。
3. **ResourceDictionary 集中管理**：色票筆刷與控制項 Style 集中於 `Window.Resources` 或 `App.xaml`。

### C. C# WinForms
1. **禁止純絕對座標**：必須採用 `TableLayoutPanel`、`SplitContainer` 搭配 `Dock = Fill` 或 `Anchor`。
2. **高 DPI 與字體修正**：Form 必須設定 `AutoScaleMode = AutoScaleMode.Dpi`，預設字體指派為 `Microsoft JhengHei UI` 或 `Segoe UI`。

---

## 🚀 3. 新手互動導覽 (Onboarding Tour) 鐵律
1. **收斂 4~7 步**：按「資料匯入 $\rightarrow$ 參數設定 $\rightarrow$ 檢視圖表 $\rightarrow$ 匯出儲存」規劃流暢引導。
2. **語意錨點綁定**：Web 使用 `data-tour="step-X"`；WPF 使用 `Tag="Tour_Step_X"` 或 `x:Name`；WinForms 使用控制項實例。嚴禁階層 CSS 選擇器。
3. **動態視圖切換**：隱藏頁籤目標必須在步驟的 `beforeShow` 鉤子中先觸發切換（如 `switchTab`、`myTabControl.SelectedIndex`）。
4. **常駐觸發按鈕**：介面右上角保留「❓ 操作導覽」按鈕，首次打開以本機儲存旗標自動啟動。
