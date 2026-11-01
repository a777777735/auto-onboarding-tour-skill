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

## 🚀 雙軌部署模式 (NuGet 套件 vs. 零依賴單檔)

### 方案 A (極度推薦)：引用 NuGet 套件 `InteractiveActionGuide.Desktop`
針對 C# WPF 或 WinForms 專案，優先推薦透過本機或私有 NuGet 套件引用，**AI 僅需生成 5 行 Fluent API 呼叫**，大幅減少程式碼 Token 消耗：
```csharp
using InteractiveActionGuide.Wpf; // 或 InteractiveActionGuide.WinForms

var guide = new WpfInteractiveGuideManager(this) { SopAppTitle = "產線管理系統" }
    .AddStep("btnSubmit", "送出表單", "填妥後請點擊送出按鈕")
    .AddStep("txtSearch", "查詢資料", "請輸入關鍵字", advanceOnClick: false);

guide.StartWithSelectionDialog(); // 自動彈出精簡/全覆蓋模式選擇並自動產出 SOP
```

### 方案 B：零依賴單檔模板拷貝
當專案無法新增套件依賴時，直接自 `templates/` 目錄複製以下單檔到專案中：
- WPF: `templates/WpfGuideEngine.cs`
- WinForms: `templates/WinFormsGuideEngine.cs`
- SOP 產生器: `templates/SopDocumentGenerator.cs`
- Web: `templates/guide-engine.js`

---

## 📸 工具閉環：步驟自動截圖與 Markdown SOP 生成 (Auto SOP Generator)
當導引引擎執行時，系統會自動在每個步驟完成高亮時截取操作畫面，並於導覽完成（`Complete()`）時自動在專案目錄下的 `docs/SOP/` 產出：
1. `screenshots/step_1.png` ... `screenshots/step_N.png`（高解析畫面截圖）
2. `USER_MANUAL.md`（完整 Markdown 圖文操作手冊，含目錄、目標控制項、觸發動作說明與注意事項）

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

### Step 1: 雙模式步驟盤點與模式勾選器 (Dual Modes & Mode Selector)
AI 必須同時盤點並實作以下兩套步驟清單，並在導覽啟動時提供**模式勾選選擇器 (Mode Selector)**：
- **`concise` (收斂精簡模式)**：
  - **規則**：嚴格過濾次要控制項，僅保留 4 至 6 個核心操作步驟（起點 $\rightarrow$ 核心 $\rightarrow$ 終點）。
- **`detailed` (詳細全覆蓋模式)**：
  - **規則**：執行 100% 地毯式盤點，涵蓋所有 Tab 分頁、Expander 展開面板、次要按鈕與輸入框，不限步驟數量。

**啟動流程**：在介面點擊「❓ 操作導覽」或首次開啟時，預設彈出**模式勾選器彈窗**，讓使用者自由勾選「⚡ 收斂精簡模式」或「📖 詳細全覆蓋模式」後再開始導引！代碼支援 `guide.StartWithSelectionDialog()`。

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
1. 根據專案類型注入對應的實戰導引引擎（Web: `guide-engine.js`；WPF: `WpfGuideEngine.cs`；WinForms: `WinFormsGuideEngine.cs`；或使用 NuGet 套件）。
2. 在介面右上角加上常駐「**❓ 操作導覽**」按鈕。
3. 利用本機儲存機制（Web: `localStorage`；C#: 本地 AppData Flag 檔案）實作首次打開自動啟動。

---

## 📦 實戰導引引擎資源
- **NuGet 套件**：`src/InteractiveActionGuide.Desktop/`（可執行 `dotnet pack` 封裝為 `.nupkg`）
- **Web 原生引擎**：`templates/guide-engine.js`（`tour-engine.js`）
- **C# WPF 引擎**：`templates/WpfGuideEngine.cs`（C# 7.3 相容，HitTestCore 穿透）
- **C# WinForms 引擎**：`templates/WinFormsGuideEngine.cs`（C# 7.3 相容，Region.Exclude 穿透）
- **SOP 產生器**：`templates/SopDocumentGenerator.cs`（自動排版圖文 Markdown）
