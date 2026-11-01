---
name: auto-onboarding-tour
description: 自動分析任何 Web/HTML 網頁、C# WPF 或 C# WinForms 應用的操作流程，無縫植入原生、零依賴、支援離線的「下一步、下一步」聚光燈互動式導覽（Onboarding Tour）。
---

# Auto Onboarding Tour Skill

本 Skill 旨在指引 AI Agent 自動掃描目標應用的架構與功能（支援 **Web/HTML**、**C# WPF** 與 **C# WinForms**），依照使用者操作心理學提煉出清晰的工作流，並在不破壞現有代碼結構的前提下，無縫植入純原生、零外部套件依賴（Zero-CDN / Zero-NuGet）的聚光燈互動式操作導覽。

---

## 🔍 專案類型自動偵測規則（Environment Detection）

在執行任何注入前，AI 必須先偵測當前專案或目標檔案類型：

1. **Web / HTML 前端專案**：
   - **判定條件**：目標為 `.html`、`.jsx`、`.tsx`、`.vue` 等前端檔案。
   - **採用方案**：使用 `templates/tour-engine.js` 注入 DOM 導覽。
   - **錨點格式**：HTML 標籤加上 `data-tour="step-X"`。

2. **C# WPF 桌面專案**：
   - **判定條件**：存在 `.csproj` 且內容包含 `<UseWPF>true</UseWPF>`，或存在主視窗 `.xaml` 檔。
   - **採用方案**：複製 `templates/WpfTourEngine.cs` 至專案內，於 `MainWindow.xaml.cs` 建立 `WpfTourManager`。
   - **錨點格式**：XAML 控制項加上 `x:Name="btnImport"` 或 `Tag="Tour:step-X"`。

3. **C# WinForms 桌面專案**：
   - **判定條件**：存在 `.csproj` 且內容包含 `<UseWindowsForms>true</UseWindowsForms>`，或存在 `Form.Designer.cs`。
   - **採用方案**：複製 `templates/WinFormsTourEngine.cs` 至專案內，於 `MainForm.cs` 建立 `WinFormsTourManager`。
   - **錨點格式**：WinForms 控制項 `Name` 屬性（如 `btnImport`、`cboDateRange`）。

---

## 🎯 核心執行規範（Core Protocol）

無論何種專案類型，AI 皆必須嚴格依照以下 4 個步驟循序執行：

### Step 1: 盤點工作流（Workflow Audit）
1. 深入閱讀目標視窗/頁面的佈局結構與商業邏輯。
2. 遵循「**資料輸入/匯入 $\rightarrow$ 核心操作/參數設定 $\rightarrow$ 檢視圖表/結果 $\rightarrow$ 匯出/儲存**」的操作閉環。
3. 收斂並精選出 **4 ~ 7 個核心步驟**（避免過多步驟造成疲勞）。
4. 針對每個步驟擬定直覺的標題（Title，10 字以內）與簡明扼要的操作引導（Content，20~50 字）。

### Step 2: 植入語意錨點（Semantic Anchoring）
- **Web/HTML**：一律使用 `data-tour="step-X"`，嚴禁使用脆弱的階層 CSS Selector。
- **WPF**：確保目標控制項具備清楚的 `x:Name`（例如 `x:Name="StepImport"`）。
- **WinForms**：使用設計工具指派的 `Name` 屬性（例如 `btnUpload`、`gridData`）。

### Step 3: 動態視圖處理（Dynamic View Handling）
若目標控制項隱藏在尚未切換的頁籤（TabControl / TabPage）或摺疊面板內，**必須在該步驟的 `BeforeShow` / `beforeShow` 鉤子中先切換視圖**：
- **Web/HTML**：`beforeShow: async () => { switchTab('tabName'); await wait(100); }`
- **WPF**：`BeforeShow = () => myTabControl.SelectedIndex = 1`
- **WinForms**：`BeforeShow = () => tabControl1.SelectedTab = tabPageDetails`

### Step 4: 注入引擎與開關（Injection & Activation）
1. 注入對應平台的零依賴引擎模板。
2. 在畫面顯眼位置（右上角或選單「說明」）加上「❓ 操作導覽」按鈕。
3. 利用本機儲存機制（Web: `localStorage`；C#: 本地 AppData Flag 檔案）實作**首次打開自動啟動**。

---

## 💻 各平台標準實作範本

### 1. Web / HTML 注入範本
```html
<!-- 目標標籤 -->
<button data-tour="step-import">匯入資料</button>

<script src="tour-engine.js"></script>
<script>
  document.addEventListener('DOMContentLoaded', () => {
    const tour = new OnboardingTour({
      storageKey: 'my_app_tour',
      steps: [
        {
          target: '[data-tour="step-import"]',
          title: '步驟 1：匯入數據',
          content: '點擊此處載入範例或上傳資料檔案。',
          placement: 'bottom'
        }
      ]
    });

    OnboardingTour.createLauncherButton({
      text: '操作導覽',
      onClick: () => tour.start(0)
    });

    tour.startIfFirstTime();
  });
</script>
```

### 2. C# WPF 注入範本
```csharp
// 在 MainWindow.xaml.cs 內使用 WpfTourManager
using AutoOnboardingTour.Wpf;

public partial class MainWindow : Window
{
    private WpfTourManager _tour;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _tour = new WpfTourManager(this)
        {
            StorageKey = "my_wpf_app_tour_v1"
        };

        _tour.AddStep("btnImport", "步驟 1：匯入數據", "點擊此按鈕載入營運資料檔。", TourPlacement.Bottom)
             .AddStep("cboDateRange", "步驟 2：參數設定", "選擇要統計分析的目標時間區間。", TourPlacement.Right)
             .AddStep("advancedChart", "步驟 3：進階趨勢", "系統已自動為您切換至進階漏斗分析頁籤！", TourPlacement.Top, 
                      beforeShow: () => myTabControl.SelectedIndex = 1)
             .AddStep("btnExport", "步驟 4：匯出報表", "一鍵匯出 Excel 或 PDF 分析結果。", TourPlacement.Left);

        // 首次啟動視窗時自動觸發
        _tour.StartIfFirstTime();
    }

    private void BtnHelpTour_Click(object sender, RoutedEventArgs e)
    {
        _tour.Start(0); // 隨時點擊按鈕重播
    }
}
```

### 3. C# WinForms 注入範本
```csharp
// 在 MainForm.cs 內使用 WinFormsTourManager
using AutoOnboardingTour.WinForms;

public partial class MainForm : Form
{
    private WinFormsTourManager _tour;

    public MainForm()
    {
        InitializeComponent();
        Load += MainForm_Load;
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        _tour = new WinFormsTourManager(this)
        {
            StorageKey = "my_winforms_app_tour_v1"
        };

        _tour.AddStep("btnUpload", "步驟 1：資料載入", "點擊此處上傳欲分析的 CSV 檔案。", TourPlacement.Bottom)
             .AddStep("tabDetails", "步驟 2：深度維度", "已自動切換至明細頁籤以檢視分群。", TourPlacement.Top,
                      beforeShow: () => tabControl1.SelectedTab = tabPageDetails)
             .AddStep("btnSaveReport", "步驟 3：儲存報表", "點擊即可保存您的設定並匯出日誌。", TourPlacement.Left);

        _tour.StartIfFirstTime();
    }

    private void btnTour_Click(object sender, EventArgs e)
    {
        _tour.Start(0);
    }
}
```

---

## 📋 驗收檢查清單（Quality Checklist）

- [ ] 是否零外部套件依賴？（Web 不引外部 CDN；C# 不安裝 NuGet 套件）
- [ ] 步驟總數是否維持在 4~7 步？
- [ ] 步驟間切換是否平滑且無閃爍？
- [ ] 若有隱藏的 Tab 頁籤，是否皆在 `BeforeShow` 中成功切換？
- [ ] 是否皆支援鍵盤快速鍵（方向鍵 $\rightarrow$ 下一步、$\leftarrow$ 上一步、`Esc` 退出）？
- [ ] 是否在畫面保留手動觸發按鈕，且首次打開具備自動啟動判定？
