---
name: interactive-action-guide
category: Onboarding
description: 實戰互動導引引擎（四周鎖死、洞口穿透操作、監聽真實事件自動推進）。自動分析 Web 或 C# (WPF/WinForms) 操作流程，無縫注入高沉浸式新手實戰引導。
auto_invoke: always
file_patterns: "*.html, *.jsx, *.tsx, *.vue, *.xaml, *.cs"
trigger_condition: 當使用者要求加入實戰互動導引、操作教學、新手引導、下一步引導或介面 SOP 時，自動分析工作流並注入四周背景鎖死、中間聚光燈洞口可穿透操作且監聽真實事件的互動引導引擎。
---

# Interactive Action Guide Skill (實戰互動導引)

本 Skill 為新一代「**實戰互動導引**」核心。不同於傳統單純看說明的導覽，本導引引擎強制實施「**四周背景鎖死、中間聚光燈洞口完全穿透（Click-Through）**」，並**監聽目標控制項的真實事件**（按鈕點擊 `click`、輸入驗證 `input`、選單切換 `change`），使用者必須在畫面上完成真實操作，系統才自動流暢進入下一步！

---

## 🔍 Step 0: 讀取 `.csproj` 自動偵測專案版本 (Version-Aware Adaptation)

在注入或修改任何 C# / XAML 代碼前，AI **必須先讀取專案的 `.csproj` 與 `App.config`**：

1. **舊版專案（`<TargetFrameworkVersion>v4.x` / .NET Framework 4.5~4.8）**：
   - **強制切換為 C# 7.3 嚴格相容模式**。
   - 使用傳統區塊命名空間 `namespace X { ... }`，禁止使用 C# 10 的 file-scoped namespace。
   - 明確宣告型別 `new List<GuideStep>()`，禁止使用 C# 9 的 target-typed `new()`。
   - 嚴禁使用 C# 8.0+ 的 Nullable 參考型別（`string?`）與 `switch` 運算式。
2. **新版專案（`<TargetFramework>net6.0` / `net8.0-windows` 等）**：
   - 自動適配 `<Nullable>` 與 `<ImplicitUsings>` 設定。
3. **模板最大相容性保證**：
   - `templates/WpfGuideEngine.cs` 與 `templates/WinFormsGuideEngine.cs` 本身 100% 採 **C# 7.3 標準語法** 撰寫，確保從 .NET Framework 4.5 到 .NET 8 皆能免修改直接編譯。

---

## 🎯 4 大注入執行步驟 (Core Protocol)

### Step 1: 盤點工作流 (Workflow Audit)
遵循「**資料匯入/輸入 $\rightarrow$ 參數設定/核心操作 $\rightarrow$ 檢視圖表/結果 $\rightarrow$ 匯出/儲存**」閉環，收斂出 **4~7 個核心步驟**，為每一步設定期望的「使用者實體操作」。

### Step 2: 綁定語意錨點與事件 (Semantic Anchoring)
- **Web/HTML**：目標元素標記 `data-tour="step-X"`，並設定 `advanceOn`:
  ```javascript
  {
    target: '[data-tour="step-btn"]',
    title: '請點擊此按鈕',
    content: '點擊此按鈕後系統將自動進入下一步。',
    advanceOn: { type: 'click' }
  }
  ```
- **C# WPF**：目標控制項標記 `Tag="Tour_Step_X"` 或 `x:Name`，設定 `AdvanceOnClick = true` 或 `AdvanceOnTextValidate = (txt) => txt.Length > 3`。
- **C# WinForms**：綁定控制項實例，設定 `AdvanceOnClick = true` 或 `AdvanceOnSelectionChange = true`。

### Step 3: 動態視圖切換 (Dynamic View Handling)
若目標元素位於尚未啟動的 Tab 頁籤、Modal 彈窗或摺疊面板內，**必須在該步驟的 `beforeShow` / `BeforeShow` 鉤子中先切換頁籤**：
- **Web**：`beforeShow: async () => { switchTab('tabName'); await delay(100); }`
- **WPF**：`BeforeShow = () => myTabControl.SelectedIndex = 1`
- **WinForms**：`BeforeShow = () => tabControl1.SelectedTab = tabPageDetails`

### Step 4: 注入實戰導引引擎 (Engine Injection)
1. 根據專案類型注入對應的實戰導引引擎（Web: `guide-engine.js`；WPF: `WpfGuideEngine.cs`；WinForms: `WinFormsGuideEngine.cs`）。
2. 在介面右上角加上常駐「**❓ 操作導覽**」按鈕。
3. 利用本機儲存機制（Web: `localStorage`；C#: 本地 AppData Flag 檔案）實作首次打開自動啟動。

---

## 📦 實戰導引引擎模板引用
- **Web 原生引擎**：`templates/guide-engine.js`（`tour-engine.js`）
- **C# WPF 引擎**：`templates/WpfGuideEngine.cs`（`WpfTourEngine.cs`，C# 7.3 相容，HitTestCore 穿透）
- **C# WinForms 引擎**：`templates/WinFormsGuideEngine.cs`（`WinFormsTourEngine.cs`，C# 7.3 相容，Region.Exclude 穿透）
