# AI Agent Skills & Rules Central Index (技能與全域規則總索引表)

> **⚠️ AI 維護規範**：  
> 當使用者新增第三個以上的 Skill 或 Rule 時，**AI 必須自動更新此索引表**，確保所有 ID、分類、支援環境、觸發關鍵字與檔案路徑維持最新狀態！

---

## 📚 1. 全域常駐規則索引 (Global Rules)

| 規則 ID | 類別 (Category) | 支援環境 | 核心宗旨 | 檔案路徑 |
| :--- | :--- | :--- | :--- | :--- |
| **`01-ui-layout-and-theme`** | `UI/UX Standards` | Web / WPF / WinForms | 強制實施 Industrial Slate Dark 色票、禁止純原色硬編碼、App Shell 視窗防雙重捲軸、Flex/Grid 防撐破、WPF 禁 Margin 座標排版、WinForms DPI/微軟正黑體修正。 | [`rules/01-ui-layout-and-theme.md`](rules/01-ui-layout-and-theme.md) |

---

## 🛠️ 2. AI Agent Skills 索引

| Skill ID (名稱) | 分類 (Category) | 支援環境 | 觸發關鍵字 (Trigger Keywords) | 核心功能說明 | 檔案路徑 |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **`ui-layout-theme-guard`** | `UI/UX Design` | Web / WPF / WinForms | `修正跑版`, `美化介面`, `套用色票`, `深色模式`, `Dark Mode`, `元件重疊`, `撐破邊界`, `雙重捲軸`, `對比度不足`, `WCAG`, `WPF 排版修正`, `WinForms DPI 修正` | 自動審查並重構現有介面，套用 Industrial Slate Dark 色票，修復破版、雙重滾動條與控制項重疊問題。內附 Web CSS 變數與 WPF XAML 資源字典模板。 | [`skills/ui-layout-theme-guard/SKILL.md`](skills/ui-layout-theme-guard/SKILL.md) |
| **`auto-onboarding-tour`** | `Onboarding & Guide` | Web / WPF / WinForms | `操作導覽`, `新手教學`, `互動引導`, `下一步`, `Onboarding Tour`, `SOP 引導`, `聚光燈教學`, `Spotlight` | 自動掃描應用工作流，在原代碼中無縫注入零依賴、支援離線的「下一步、下一步」聚光燈挖洞導覽，具備自動防溢出 Tooltip 與動態視圖 (Tab/Modal) 切換。 | [`skills/auto-onboarding-tour/SKILL.md`](skills/auto-onboarding-tour/SKILL.md) |

---

## 📂 模板庫索引 (Templates Directory)

| 所屬 Skill | 模板檔案名稱 | 適用平台 | 技術特性與用途 |
| :--- | :--- | :--- | :--- |
| `ui-layout-theme-guard` | `web-theme-vars.css` | Web HTML/CSS | 零依賴 CSS 變數系統 + App Shell (100vh 鎖定、防雙重捲軸) 佈局模板 |
| `ui-layout-theme-guard` | `WpfThemeResource.xaml` | C# WPF | 高對比 Slate 筆刷、標準 Button/TextBox/Border 樣式之 ResourceDictionary |
| `auto-onboarding-tour` | `tour-engine.js` | Web (Single-File) | 原生純 JS + box-shadow 聚光燈挖洞導覽引擎，支援自動防溢出與鍵盤操作 |
| `auto-onboarding-tour` | `WpfTourEngine.cs` | C# WPF | 零 NuGet 依賴，採用 `AdornerLayer` + `CombinedGeometry` 挖洞半透明遮罩 |
| `auto-onboarding-tour` | `WinFormsTourEngine.cs` | C# WinForms | 零 NuGet 依賴，採用無邊框貼合 Overlay Form + `Region.Exclude` 鏤空高亮 |
