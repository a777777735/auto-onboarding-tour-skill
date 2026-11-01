# AI Agent Central Skills & Global Rules Monorepo

> 具備 **Zero-Touch 動態掃描**、**全域路由表自動編譯 (Always-On Skill Router)** 與 **全平台實戰互動引導 / 防跑版規範** 的 AI Agent 中央倉庫。
> 支援一鍵將所有 Skills 與 Rules 動態掛載至 Antigravity 全域環境（`~/.gemini/antigravity/`），實現「每個對話框自動強制套用」。

---

## 📁 倉庫結構全景

```text
.
├── rules/                                      # 全域常駐規範 (Global Standards)
│   └── 01-ui-and-onboarding-standards.md       # Step 0 版本偵測、UI 佈局防跑版與實戰引導規範
│
├── skills/                                     # 模組化技能庫 (Modular Agent Skills)
│   ├── interactive-action-guide/               # [Skill] 實戰互動導引引擎 (四周鎖死、洞口穿透、真實事件推進)
│   │   ├── SKILL.md                            # 實戰引導注入規範 (包含 Step 0 讀取 .csproj 偵測版本)
│   │   └── templates/
│   │       ├── guide-engine.js                 # Web 原生 JS 實戰引導引擎 (Click-Through 穿透、click/input/change 監聽)
│   │       ├── WpfGuideEngine.cs               # C# WPF 實戰引導引擎 (C# 7.3 相容、HitTestCore 洞口穿透)
│   │       └── WinFormsGuideEngine.cs          # C# WinForms 實戰引導引擎 (C# 7.3 相容、Region.Exclude 穿透)
│   │
│   ├── ui-layout-theme-guard/                  # [Skill] UI 佈局防跑版與工業風配色守門員 (auto_invoke: always)
│   │   ├── SKILL.md                            # 檢查與重構執行規範
│   │   └── templates/
│   │       ├── web-app-shell.css               # Web CSS 變數與 100vh App Shell 防溢出模板
│   │       └── WpfIndustrialTheme.xaml         # WPF 高對比深色主題 ResourceDictionary 樣式表
│   │
│   └── skill-scaffold/                         # [Skill] 新增技能專用的「元技能 Meta-Skill」 (auto_invoke: contextual)
│       └── SKILL.md                            # 自動產生符合標準規格的技能骨架並自動觸發全域編譯
│
├── examples/                                   # 驗收與展示應用
│   ├── demo-dashboard.html                     # 工業設備智慧監控儀表板 (深色 App Shell + 實戰互動導引)
│   └── demo-app.html                           # OmniAnalytics 商業數據儀表板示範
│
├── sync-to-global.ps1                          # Windows PowerShell 動態掃描、索引更新與全域掛載腳本
├── sync-to-global.sh                           # macOS / Linux Bash 動態掃描與全域掛載腳本
├── SKILLS_INDEX.md                             # AI 自動維護之中央技能與規則總索引表
├── .gitignore                                  # 系統暫存與建置檔案排除
└── README.md                                   # 本倉庫指南
```

---

## 🚀 核心升級亮點：實戰互動導引 (`interactive-action-guide`)

與傳統只能「看文字、點下一步」的純說明導覽不同，本引擎實施「**實戰操作驅動**」：
1. **四周背景鎖死，洞口完全穿透操作 (Click-Through)**：
   - **Web**：目標控制項動態提升層級（`iag-target-active`），周圍 backdrop 鎖死其餘點擊，使用者可直接點選高亮元素或在輸入框打字。
   - **WPF**：`Adorner` 重寫 `HitTestCore`，當滑鼠點擊落在 `_targetHoleRect` 內時返回 `null`，滑鼠事件完全穿透到真實按鈕與文字方塊。
   - **WinForms**：Overlay Form 透過 `Region.Exclude` 鏤空，洞口具備作業系統級別事件穿透，點擊直接直通用戶控制項。
2. **監聽真實事件自動推進**：
   - 支援 `advanceOn`: 點擊按鈕 (`click`)、輸入文字驗證 (`input` / `TextChanged`)、切換下拉選單 (`change` / `SelectedIndexChanged`)，完成真實操作後自動觸發微動畫並推進到下一步！
3. **版本前置偵測 (Step 0)**：
   - 讀取 `.csproj` 自動判斷 .NET Framework (強制 C# 7.3 模式) 或 .NET 6/8，引擎模板 100% 以 C# 7.3 撰寫，無縫支援所有版本。

---

## ⚡ 一鍵動態掃描與全域部署

### Windows (PowerShell)
```powershell
powershell -ExecutionPolicy Bypass -File .\sync-to-global.ps1
```

### macOS / Linux (Bash)
```bash
chmod +x sync-to-global.sh
./sync-to-global.sh
```
