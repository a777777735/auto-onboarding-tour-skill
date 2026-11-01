---
name: auto-onboarding-tour
category: Onboarding
description: 跨平台「下一步、下一步」聚光燈互動導覽產生器。自動分析 Web 或 C# (WPF/WinForms) 應用操作流程，無縫注入零依賴、支援離線的新手互動教學。
auto_invoke: always
file_patterns: "*.html, *.jsx, *.tsx, *.vue, *.xaml, *.cs"
trigger_condition: 當使用者要求加入操作教學、新手引導、下一步教學或介面 SOP 互動導覽時，自動分析工作流並注入對應平台的零依賴導覽引擎。
---

# Auto Onboarding Tour Skill

本 Skill 旨在引導 AI Agent 自動分析 Web 網頁或 C# 桌面應用（WPF / WinForms）的操作工作流程，並在原有代碼中無縫注入零依賴、支援離線的「下一步、下一步」聚光燈互動式導覽（Onboarding Tour）。

---

## 🔍 Step 0: 環境與版本前置偵測 (Environment & Version Pre-flight Check)

在產生、注入或調整任何導覽邏輯前，AI **必須先執行環境與版本前置審查**：

1. **C# 專案版本適配 (讀取 `.csproj`)**：
   - **.NET Framework 4.5~4.8**：強制以 **C# 7.3 嚴格相容語法** 撰寫或注入調用代碼（使用傳統區塊 namespace、明確宣告型別 `new List<WpfTourStep>()`、不使用 Nullable 參考型別標記 `?` 或 `switch` 運算式）。
   - **.NET 6 / .NET 8**：自動適配 Nullable 與 ImplicitUsings。
   - **模板最大相容性原則**：`templates/WpfTourEngine.cs` 與 `templates/WinFormsTourEngine.cs` 100% 採 C# 7.3 標準撰寫，確保免修改即可在 .NET Framework 4.5 至 .NET 8 直接編譯。
2. **專案類型自動判定**：
   - **Web / HTML 專案**（`.html`, `.jsx`, `.vue`）$\rightarrow$ 採用 `templates/tour-engine.js`。無 `package.json` 時採用 Inline 原生內嵌；有 `package.json` 時採用 ES Module 匯入。
   - **C# WPF 專案**（`.csproj` 含 `<UseWPF>true</UseWPF>` 或 `.xaml` 檔）$\rightarrow$ 採用 `templates/WpfTourEngine.cs`。
   - **C# WinForms 專案**（`.csproj` 含 `<UseWindowsForms>true</UseWindowsForms>` 或 `Form.Designer.cs`）$\rightarrow$ 採用 `templates/WinFormsTourEngine.cs`。

---

## 🎯 4 大注入執行步驟 (Core Protocol)

### Step 1: 盤點工作流 (Workflow Audit)
遵循「**資料匯入/輸入 $\rightarrow$ 參數設定/核心操作 $\rightarrow$ 檢視圖表/結果 $\rightarrow$ 匯出/儲存**」的操作閉環，收斂出 **4~7 個核心步驟**，擬定標題（10 字內）與引導說明（20~50 字）。

### Step 2: 綁定語意錨點 (Semantic Anchoring)
- **Web/HTML**：在目標元素加上 `data-tour="step-X"`（嚴禁使用脆弱的層級 CSS Selector）。
- **C# WPF**：在目標 XAML 控制項加上 `Tag="Tour_Step_X"` 或 `x:Name`。
- **C# WinForms**：直接綁定目標控制項實例或其 `Name` 屬性。

### Step 3: 動態視圖切換 (Dynamic View Handling)
若目標元素位於尚未啟動的 Tab 頁籤、Modal 彈窗或摺疊面板內，**必須在該步驟的 `beforeShow` / `BeforeShow` 鉤子中先觸發視圖切換**：
- **Web**：`beforeShow: async () => { switchTab('tabName'); await delay(100); }`
- **WPF**：`BeforeShow = () => myTabControl.SelectedIndex = 1`
- **WinForms**：`BeforeShow = () => tabControl1.SelectedTab = tabPageDetails`

### Step 4: 自動注入引擎與觸發鈕 (Injection & Activation)
1. 根據專案類型注入對應的零依賴引擎代碼（Web 內嵌 `tour-engine.js`；C# 複製 `WpfTourEngine.cs` 或 `WinFormsTourEngine.cs`）。
2. 在介面右上角加上常駐「**❓ 操作導覽**」按鈕。
3. 利用本機儲存機制（Web: `localStorage`；C#: 本地 AppData Flag 檔案）實作首次打開自動啟動。

---

## 📦 零依賴導覽引擎模板引用
- **Web 導覽引擎**：`templates/tour-engine.js`（`box-shadow` 挖洞遮罩、智慧防溢出 Tooltip、鍵盤操作）。
- **WPF 導覽引擎**：`templates/WpfTourEngine.cs`（C# 7.3 相容、`AdornerLayer` + `CombinedGeometry` 挖洞、防溢出卡片）。
- **WinForms 導覽引擎**：`templates/WinFormsTourEngine.cs`（C# 7.3 相容、無邊框 Overlay Form + `Region.Exclude` 鏤空高亮）。
