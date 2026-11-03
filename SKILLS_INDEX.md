# AI Agent Skills & Rules Central Index (技能與全域規則總索引表)

> **AI 自動維護規範**：
> 本檔案由 sync-to-global.ps1 / sync-to-global.sh 自動產生與維護。
> 當使用者新增第三個以上的 Skill 時，AI 或自動化工具會自動重新掃描並更新此索引表！

---

## 1. 全域常駐規則 (Global Rules)

| 規則 ID | 類別 (Category) | 核心宗旨 | 檔案路徑 |
| :--- | :--- | :--- | :--- |
| **00-token-saving-output** | Policy & Performance | 全域省 Token 與極簡回覆鐵律：禁止在對話框輸出大段代碼、局部修改優先、聊天室摘要小於100字。 | [rules/00-token-saving-output.md](rules/00-token-saving-output.md) |
| **01-ui-and-onboarding-standards** | UI/UX & Onboarding | 雙主題工控色票（FormatString 淺色 / 工業深色）、輸入框尺寸鎖定防跑版與實戰引導四大規範。 | [rules/01-ui-and-onboarding-standards.md](rules/01-ui-and-onboarding-standards.md) |
| **02-subagent-dispatch-and-routing** | Architecture & Routing | Subagent 輕重任務分流矩陣 (Fast-Track vs Heavy Subagent) 與 5 大場景意圖路由決策樹。 | [rules/02-subagent-dispatch-and-routing.md](rules/02-subagent-dispatch-and-routing.md) |

---

## 2. AI Agent 技能動態註冊表 (Dynamic Skills Directory)

| 技能名稱 (ID) | 分類 (Category) | 適用檔案 (Patterns) | 調度等級 (Invoke) | 觸發時機 (Trigger Condition) | 本地路徑 |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **interactive-action-guide** | Onboarding | *.html, *.jsx, *.tsx, *.vue, *.xaml, *.cs" | always | 當使用者要求加入實戰互動導引、操作教學、新手引導、下一步引導或介面 SOP 時，自動分析工作流並注入四周背景鎖死、中間聚光燈洞口可穿透操作且監聽真實事件的互動引導引擎。 | [skills/interactive-action-guide/SKILL.md](skills/interactive-action-guide/SKILL.md) |
| **skill-scaffold** | Meta | SKILL.md, *.ps1, *.sh" | contextual | 當使用者要求新增、建立、打包或擴充一個新的 AI Agent Skill 時，必須主動調用此技能以產生符合標準規範的骨架並自動同步全域。 | [skills/skill-scaffold/SKILL.md](skills/skill-scaffold/SKILL.md) |
| **ui-layout-theme-guard** | UI/UX | *.html, *.css, *.jsx, *.tsx, *.vue, *.xaml, *.cs | always | 當生成、修改或美化任何 Web 網頁 (HTML/CSS) 或 C# (WPF/WinForms) 介面代碼時，必須主動套用此規範防止輸入框大小暴走、DataGrid懸停白底白字、視窗底色黑透與刺眼配色。 | [skills/ui-layout-theme-guard/SKILL.md](skills/ui-layout-theme-guard/SKILL.md) |
| **wpf-embedded-manual-and-docs** | Documentation | *.xaml, *.cs, *.md | contextual | 當使用者要求建立、更新或同步 WPF 應用程式內嵌說明手冊、可縮放平移向量流程圖或技術手冊章節卡片時調用。 | [skills/wpf-embedded-manual-and-docs/SKILL.md](skills/wpf-embedded-manual-and-docs/SKILL.md) |

---

## 3. 模板庫清單 (Templates Directory)

| 所屬技能 | 模板檔案名稱 | 適用平台 | 核心用途 |
| :--- | :--- | :--- | :--- |
| ui-layout-theme-guard | WpfFormatStringTheme.xaml | C# WPF | FormatString 經典淺色工控主題、單行輸入框固定尺寸、多行編輯器伸展、DataGrid 淺色樣式表 |
| ui-layout-theme-guard | WpfIndustrialTheme.xaml | C# WPF | 高對比 Slate 深色筆刷、DataGrid 深度深色化、標準 Button/TextBox/Border 樣式表 |
| ui-layout-theme-guard | web-app-shell.css | Web HTML/CSS | 零依賴 CSS 變數系統 + App Shell (100vh 鎖定、防雙重捲軸、深色表格) 佈局模板 |
| interactive-action-guide | guide-engine.js | Web (Single-File) | 原生純 JS + 洞口穿透操作 + 真實事件監聽自動推進引擎 |
| interactive-action-guide | WpfGuideEngine.cs | C# WPF | C# 7.3 嚴格相容，HitTestCore 洞口穿透操作 + 真實事件自動推進 |
| interactive-action-guide | WinFormsGuideEngine.cs | C# WinForms | C# 7.3 嚴格相容，Region.Exclude 洞口穿透操作 + 真實事件自動推進 |
| wpf-embedded-manual-and-docs | SKILL.md | C# WPF | 左側導航手冊、縮放/平移向量流程圖與結構化功能卡片規格 |
