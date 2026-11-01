# Global Rule: UI Layout, Industrial Theme & Onboarding Standards
# 規則編號：01-ui-and-onboarding-standards
# 適用範圍：Web (HTML/CSS)、C# WPF (.xaml)、C# WinForms

---

## 🔍 Step 0: 環境與版本前置偵測 (Environment & Version Pre-flight Check)

在生成、修改或注入任何代碼之前，AI **必須先執行環境與版本前置檢查**，嚴禁產生與專案版本衝突的語法：

### 1. C# 專案版本適配規則 (讀取 `.csproj` 與 `App.config`)
- **舊版專案（`<TargetFrameworkVersion>v4.x` / .NET Framework 4.5~4.8）**：
  - **強制切換為 C# 7.3 嚴格相容模式**。
  - 使用傳統區塊命名空間 `namespace MyProject { ... }`，禁止使用 C# 10 的 file-scoped namespace。
  - 明確型別宣告 `new List<TourStep>()`，禁止使用 C# 9 的 target-typed `new()`。
  - 嚴禁使用 C# 8.0+ 的 Nullable 參考型別（如 `string?`）與 `switch` 運算式。
- **新版專案（`<TargetFramework>net6.0` / `net8.0-windows` 等）**：
  - 自動檢查 `<Nullable>` 與 `<ImplicitUsings>` 設定，動態調整生成代碼的型別標記與 `using` 引用。
- **模板最大相容性原則**：
  - 核心模板檔（如 `WpfTourEngine.cs`、`WinFormsTourEngine.cs`）本身必須以 **C# 7.3 零外部依賴標準語法** 撰寫，確保從 .NET Framework 4.5 到 .NET 8 都能 100% 免改動直接編譯通過。

### 2. UI 框架與資源字典衝突防呆
- 在修改 WPF `App.xaml` 或 `MainWindow.xaml` 前，**必須先檢查專案是否已引入第三方 UI 套件**（如 `MaterialDesignThemes`、`HandyControl`、`MahApps.Metro`、`DevExpress`）。
- **防呆保護**：若已存在第三方 UI 庫，**僅注入色彩筆刷變數（`Color` / `SolidColorBrush`）與版面結構容器（`Grid` / `DockPanel`）**，嚴禁強行覆蓋第三方元件的預設 `ControlTemplate`，避免破壞原有 UI 風格。

### 3. Web 專案打包架構自動判定
- **無 `package.json`（純單檔 HTML / 靜態頁面）**：強制採用零依賴內嵌式（Inline）原生 CSS/JS 語法，支援純本機離線開啟。
- **有 `package.json`（React / Vue / Vite / Webpack）**：自動判斷採用 ES Module 匯入風格，並適配框架的生命週期掛載（如 React `useEffect` 或 Vue `onMounted`）。

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
