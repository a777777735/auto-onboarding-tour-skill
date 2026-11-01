---
name: ui-layout-theme-guard
description: 專業級 UI 佈局防呆與工業色票守護者。自動審查並重構 Web (HTML/CSS)、C# WPF 與 C# WinForms 的介面代碼，根治版面跑版、元件重疊、邊界撐破與顏色對比不佳問題。
---

# UI Layout & Theme Guard Skill

本 Skill 旨在引導 AI Agent 在生成或重構任何 **Web 前端網頁（單檔 HTML / 儀表板）**、**C# WPF (.xaml)** 與 **C# WinForms** 介面時，自動實施最高規格的佈局防呆檢查與色票標準化，杜絕「跑版、重疊、破版、對比度不足」等頑疾。

---

## 🎨 核心規範：工業級色票系統 (Industrial Slate Design System)

所有介面預設遵循此套經過 WCAG AA 對比度驗證的色票體系：

| 變數名稱 | 色碼 (HEX) | 用途說明 | 適用對比標的 |
| :--- | :--- | :--- | :--- |
| **`bg-primary`** | `#0F172A` (Slate 900) | 全域視窗畫布背景、主框架底色 | 搭配 `text-primary` |
| **`bg-surface`** | `#1E293B` (Slate 800) | 卡片容器、面板、側邊導航背景 | 搭配 `border` 分割 |
| **`bg-elevated`** | `#334155` (Slate 700) | 按鈕 Hover 態、輸入框背景、標籤底色 | 增加層次感 |
| **`border`** | `#334155` (Slate 700) | 面板外框、格線邊框（1px solid） | 柔和分割 |
| **`text-primary`** | `#F8FAFC` (Slate 50) | 標題文字、核心讀數、主要按鈕文字 | 對比度 > 12:1 (極高) |
| **`text-muted`** | `#94A3B8` (Slate 400) | 次要文字、輸入框 Placeholder、時間標籤 | 對比度 > 4.5:1 (符合 AA) |
| **`primary`** | `#0284C7` (Sky 600) | 核心動作按鈕、聚焦外框 (Focus Ring) | Hover: `#0369A1` |
| **`success`** | `#10B981` (Emerald 500) | 正常運轉、資料成功寫入、上漲指標 | 文字搭配 `#ECFDF5` 底色 |
| **`warning`** | `#F59E0B` (Amber 500) | 警告狀態、數值趨近上限、待處理 | 文字搭配 `#FFFBEB` 底色 |
| **`danger`** | `#EF4444` (Red 500) | 錯誤、警報、刪除動作、異常中斷 | 文字搭配 `#FEF2F2` 底色 |

---

## 🛡️ 各平台佈局防呆核心鐵律

### 1. Web (HTML / CSS) 守則
- **CSS 變數封裝**：`:root` 中必須定義全套 `--bg-*`、`--text-*`、`--primary` 等變數，全篇禁止硬編碼 Hex 色碼。
- **App Shell 視窗鎖定**：
  ```css
  html, body {
    height: 100vh;
    margin: 0;
    overflow: hidden; /* 防止整個視窗出現醜陋雙重捲軸 */
    background-color: var(--bg-primary);
    color: var(--text-primary);
  }
  ```
- **Flex / Grid 子元素防撐破**：
  ```css
  /* 任何放置 Table、Canvas、長字串的 Flex/Grid 項目必須加入： */
  .layout-child {
    min-width: 0;
    min-height: 0;
    overflow: hidden;
  }
  ```
- **幾何網格間距**：統一使用 `4px` 之倍數（`4px`, `8px`, `12px`, `16px`, `24px`, `32px`）。

---

### 2. C# WPF (.xaml) 守則
- **嚴禁座標 Margin 定位**：嚴禁使用類似 `Margin="215,86,0,0"` 來定位元件。
- **Grid 佈局主導**：
  ```xml
  <Grid>
      <Grid.RowDefinitions>
          <RowDefinition Height="Auto"/>    <!-- 頂部標題/選單 -->
          <RowDefinition Height="*"/>       <!-- 主要內容自適應填滿 -->
          <RowDefinition Height="48"/>      <!-- 底部狀態列 -->
      </Grid.RowDefinitions>
  </Grid>
  ```
- **資源集中化 (ResourceDictionary)**：
  必須在 `Window.Resources` 或 `App.xaml` 宣告標準筆刷：
  ```xml
  <SolidColorBrush x:Key="BgPrimary" Color="#0F172A"/>
  <SolidColorBrush x:Key="BgSurface" Color="#1E293B"/>
  <SolidColorBrush x:Key="TextPrimary" Color="#F8FAFC"/>
  <SolidColorBrush x:Key="TextMuted" Color="#94A3B8"/>
  <SolidColorBrush x:Key="PrimaryAccent" Color="#0284C7"/>
  ```
- **對齊與控制項微調**：所有輸入框與按鈕皆設定 `VerticalContentAlignment="Center"`，並統一設置圓角。

---

### 3. C# WinForms 守則
- **禁止純絕對座標 (Location)**：使用 `TableLayoutPanel`、`SplitContainer` 與 `FlowLayoutPanel`，所有容器必須設定 `Dock = DockStyle.Fill` 或設置 `Anchor`。
- **DPI 與現代字體指派**：
  ```csharp
  public MainForm()
  {
      InitializeComponent();
      this.AutoScaleMode = AutoScaleMode.Dpi;
      this.Font = new Font("Microsoft JhengHei UI", 9.5F, FontStyle.Regular);
      this.DoubleBuffered = true;
      ApplyTheme();
  }
  ```

---

## 📋 一鍵修復現有專案 UI 與配色的標準檢查清單 (Audit Checklist)

當使用者指示「**檢查/修復目前的 UI 跑版或色彩配置**」時，AI 應依照以下檢查清單依序過濾與重構：

```markdown
### [ ] Phase 1: 色彩與對比度審查
- [ ] 1.1 是否存在 `#FF0000`、`#0000FF`、`#00FF00` 等刺眼純色？（全數替換為語意狀態色）
- [ ] 1.2 深色背景上的文字是否清晰？（主文字須使用 #F8FAFC，次要文字使用 #94A3B8）
- [ ] 1.3 是否已將顏色抽象為 CSS 變數 / WPF ResourceDictionary / WinForms Theme 類別？

### [ ] Phase 2: 容器邊界與捲軸審查
- [ ] 2.1 瀏覽器是否出現「外層與內層同時出現滾動條」的雙重捲軸現象？（設定 body 100vh + overflow: hidden）
- [ ] 2.2 當視窗縮小至 1024px 或放大至 4K 時，內容是否會擠壓破版？
- [ ] 2.3 Flex / Grid 內包含的 Canvas、圖表或表格是否加上 `min-width: 0; min-height: 0;`？

### [ ] Phase 3: 桌面控制項對齊審查 (WPF / WinForms)
- [ ] 3.1 WPF XAML 中是否仍有隨機大數值 Margin？（重構為 Grid.Row/Column 比例配置）
- [ ] 3.2 WinForms 控制項是否在視窗縮放時未跟隨擴展？（設定 TableLayoutPanel + Dock.Fill）
- [ ] 3.3 WinForms 是否已啟用 DPI 縮放與指派微軟正黑體/Segoe UI？
```

---

## 🧩 平台開箱即用代碼模板

### 1. Web CSS 變數模板
```css
:root {
  --bg-primary: #0f172a;
  --bg-surface: #1e293b;
  --bg-elevated: #334155;
  --border: #334155;
  --text-primary: #f8fafc;
  --text-muted: #94a3b8;
  --primary: #0284c7;
  --primary-hover: #0369a1;
  --success: #10b981;
  --warning: #f59e0b;
  --danger: #ef4444;
  --radius-md: 8px;
  --radius-lg: 12px;
}
```

### 2. C# WinForms Theme 輔助類別
```csharp
public static class IndustrialTheme
{
    public static readonly Color BgPrimary = Color.FromArgb(15, 23, 42);
    public static readonly Color BgSurface = Color.FromArgb(30, 41, 59);
    public static readonly Color BgElevated = Color.FromArgb(51, 65, 85);
    public static readonly Color Border = Color.FromArgb(51, 65, 85);
    public static readonly Color TextPrimary = Color.FromArgb(248, 250, 252);
    public static readonly Color TextMuted = Color.FromArgb(148, 163, 184);
    public static readonly Color Primary = Color.FromArgb(2, 132, 199);

    public static void Apply(Form form)
    {
        form.BackColor = BgPrimary;
        form.ForeColor = TextPrimary;
        form.Font = new Font("Microsoft JhengHei UI", 9.5F);
        ApplyToControls(form.Controls);
    }

    private static void ApplyToControls(Control.ControlCollection controls)
    {
        foreach (Control c in controls)
        {
            if (c is Panel pnl) { pnl.BackColor = BgSurface; }
            else if (c is Button btn)
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.BackColor = Primary;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderSize = 0;
            }
            if (c.HasChildren) ApplyToControls(c.Controls);
        }
    }
}
```
