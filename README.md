# auto-onboarding-tour AI Agent Skill

> 讓 AI Agent 能自動分析任何 **Web/HTML 網頁**、**C# WPF** 或 **C# WinForms** 應用的操作流程，並在原有代碼中無縫注入「下一步、下一步」的聚光燈互動式導覽（Onboarding Tour）。

---

## 🌟 核心特色

- **零外部依賴 (Zero-CDN, Zero-NuGet, Offline First)**：
  - Web: 純原生 Vanilla JS + CSS，單檔 HTML / 離線環境皆可運作。
  - C# WPF: 純原生 C# 代碼，使用 `AdornerLayer` 搭配 `CombinedGeometry` 挖洞，零 NuGet 套件。
  - C# WinForms: 純原生 C# 代碼，使用無邊框貼合 Overlay Form 搭配 `Region.Exclude` 鏤空，零 NuGet 套件。
- **平滑聚光燈效果 (Spotlight Mask)**：高亮目標控制項，周圍以深色半透明遮罩覆蓋，並附帶亮色外框。
- **智慧防溢出 Tooltip (Auto-collision & Flip)**：自動依據可視區域/視窗邊界判定放置方向（上/下/左/右），避免超出螢幕。
- **動態視圖鉤子 (`beforeShow` / `BeforeShow`)**：當目標元素處於隱藏的 Tab 頁籤、Modal 或 Accordion 時，先自動觸發切換再高亮。
- **全方位互動體驗**：支援視窗縮放/移動時自動重算座標，支援鍵盤快速鍵：`→`（下一步）、`←`（上一步）、`Escape`（退出）。
- **首次造訪自動啟動**：支援本機記錄記憶，並常駐「❓ 操作導覽」按鈕供隨時手動重新啟動。

---

## 📁 專案檔案結構

```text
.
├── .agents/
│   └── skills/
│       └── auto-onboarding-tour/
│           ├── SKILL.md                  # Skill 規範定義檔 (包含專案類型偵測與執行規則)
│           └── templates/
│               ├── tour-engine.js        # Web/HTML 原生導覽引擎
│               ├── WpfTourEngine.cs      # C# WPF 原生導覽引擎 (AdornerLayer + CombinedGeometry)
│               └── WinFormsTourEngine.cs # C# WinForms 原生導覽引擎 (Overlay Form + Region.Exclude)
├── examples/
│   └── demo-app.html                     # Web 單檔 HTML 示範應用
└── README.md                             # 本專案說明與跨平台載入指南
```

---

## 🔍 專案類型自動偵測

AI 在執行時會自動偵測目標專案：
1. **Web / HTML 專案**（`.html`, `.jsx`, `.vue`）$\rightarrow$ 採用 `templates/tour-engine.js`。
2. **C# WPF 專案**（`.csproj` 含 `<UseWPF>true</UseWPF>` 或 `.xaml` 檔）$\rightarrow$ 採用 `templates/WpfTourEngine.cs`。
3. **C# WinForms 專案**（`.csproj` 含 `<UseWindowsForms>true</UseWindowsForms>` 或 `Form.Designer.cs`）$\rightarrow$ 採用 `templates/WinFormsTourEngine.cs`。

---

## 🚀 跨平台 AI Agent 安裝與載入指南

### 1. 在 Google Antigravity 中使用
本專案已放置於 `.agents/skills/auto-onboarding-tour/`：
- 直接在對話中提示：
  > 「請使用 `auto-onboarding-tour` skill 為我的 WPF 視窗 `MainWindow.xaml` 加上導覽教學。」

### 2. 在 Visual Studio / Cursor 中使用
在專案根目錄 `.cursorrules` 或 `.cursor/rules/onboarding.mdc` 加入：
```markdown
當需要為 Web、WPF 或 WinForms 專案加入步驟導覽時，請參考 `.agents/skills/auto-onboarding-tour/SKILL.md` 的執行規範，並引用對應平台的 template。
```

### 3. 在 Claude Code (CLI) 中使用
在 `CLAUDE.md` 中引用此 Skill：
```markdown
# Onboarding Tour Skill
當需要為應用加入操作引導時，請依據專案類型參照 `.agents/skills/auto-onboarding-tour/SKILL.md` 注入對應的導覽引擎。
```

---

## 💻 快速使用範例

### 1. Web / HTML
```html
<script src="tour-engine.js"></script>
<script>
  document.addEventListener('DOMContentLoaded', () => {
    const tour = new OnboardingTour({
      storageKey: 'my_app_tour',
      steps: [
        {
          target: '[data-tour="step-import"]',
          title: '步驟 1：資料匯入',
          content: '點擊此處上傳 CSV 或載入範例。',
          placement: 'bottom'
        }
      ]
    });
    OnboardingTour.createLauncherButton({ text: '操作導覽', onClick: () => tour.start(0) });
    tour.startIfFirstTime();
  });
</script>
```

### 2. C# WPF 桌面應用
```csharp
using AutoOnboardingTour.Wpf;

public partial class MainWindow : Window
{
    private WpfTourManager _tour;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            _tour = new WpfTourManager(this) { StorageKey = "wpf_tour_v1" };
            _tour.AddStep("btnImport", "步驟 1：匯入數據", "點擊載入業務檔案。")
                 .AddStep("tabAdvanced", "步驟 2：進階分析", "系統自動切換至分析頁籤。", 
                          TourPlacement.Top, beforeShow: () => myTabControl.SelectedIndex = 1);
            _tour.StartIfFirstTime();
        };
    }

    private void BtnHelp_Click(object sender, RoutedEventArgs e) => _tour.Start(0);
}
```

### 3. C# WinForms 桌面應用
```csharp
using AutoOnboardingTour.WinForms;

public partial class MainForm : Form
{
    private WinFormsTourManager _tour;

    public MainForm()
    {
        InitializeComponent();
        Load += (s, e) =>
        {
            _tour = new WinFormsTourManager(this) { StorageKey = "winforms_tour_v1" };
            _tour.AddStep("btnUpload", "步驟 1：檔案上傳", "點此選擇分析資料。")
                 .AddStep("gridResult", "步驟 2：報表明細", "此處展示運算成果。",
                          beforeShow: () => tabControl1.SelectedTab = tabPageGrid);
            _tour.StartIfFirstTime();
        };
    }

    private void btnTour_Click(object sender, EventArgs e) => _tour.Start(0);
}
```
