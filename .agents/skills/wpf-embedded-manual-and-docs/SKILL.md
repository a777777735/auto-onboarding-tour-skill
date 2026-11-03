---
name: wpf-embedded-manual-and-docs
description: >-
  Use this skill when building, updating, or synchronizing in-app WPF help manuals, interactive vector workflow diagrams
  (SVG/Canvas), and technical documentation chapters. Excludes interactive spotlight tours (delegated to interactive-action-guide).
---

# WPF Embedded Help Manual & Vector Architecture Docs Kit

本 Skill 專門用於為 C# WPF 桌面應用程式或插件模組構建專業級「**內嵌式技術說明手冊 (In-App Help Manual)**」與「**互動式向量架構流程圖 (Vector Architecture Diagram)**」，確保所有操作規格與核心代碼文件隨功能升級保持 100% 同步。

> **職責邊界宣告**：
> - **實戰互動引導（四周鎖死、穿透操作、事件監聽推進）**：一律由 `interactive-action-guide` 技能負責。
> - **本技能專注領域**：WPF XAML 側邊導航說明手冊、可縮放/平移向量流程圖、Markdown/FlowDocument 結構化手冊卡片與規格同步。

---

## 📖 1. 說明手冊架構標準 (In-App Manual Architecture)

手冊介面必須設計為無邊框 `TabControl`，由左側導航欄與右側內容檢視區組成：

1. **左側導航列 (Navigation Sidebar)**：
   * 採用自訂樣式的 `RadioButton`（`HelpNavRadioStyle`），高度 `38px`，懸停背景 `#F1F5F9`，選中狀態底色 `#0284C7` 或 `#2563EB`，白字加粗。
   * 依功能重要性劃分章節：
     1. 系統定位與核心工作流 (Overview & Architecture)
     2. 語法結構與規則清單 (Syntax & Rules)
     3. 斷言比對與防呆規格 (Assertion & Error Guard)
     4. C# 腳本與二進制通訊實戰 (Script & Protocol Deep-dive)
     5. 變量庫與外部系統對接 (Variable Mapping)

2. **全螢幕/視窗化切換 (Fullscreen Toggle)**：
   * 提供獨立放大/還原按鈕，讓現場工程師能將手冊放大至全螢幕，舒適閱讀大圖與長表格。

3. **雙主題色調適配**：
   * 與主視窗一致採用 **FormatString 經典工控淺色主題**（底色 `#FFFFFF`、邊框 `#E2E8F0`、文字 `#1A252F`、標題 `#1E293B`）。

---

## 🗺️ 2. 互動式向量架構流程圖 (Interactive Vector Diagram)

為清楚呈現模組在工業通訊（MES / SFC / PLC / 機器人）中的上下游關係，手冊必須內嵌向量流程圖：

1. **技術實現**：
   * 採用 `Viewbox` 結合 `Canvas` 或乾淨向量 Path 呈現高解析架構圖。
   * 支援 `Ctrl + 滑鼠滾輪` 縮放與滑鼠左鍵拖曳平移 (Pan & Zoom)。
2. **視覺標註標準**：
   * **輸入節點**：淡天藍色卡片，標註來源變數與通訊封包。
   * **處理核心**：深海藍/紫色卡片，標註語法解析器、表達式引擎與斷言機制。
   * **輸出節點**：翠綠色卡片，標註目標賦值變數與下游執行單元。

---

## 📋 3. 結構化文檔章節卡片標準 (Documentation Chapter Standards)

每個手冊分頁內容必須以結構化卡片（Border + CornerRadius="6"）呈現：
- **卡片 1：核心功能定位與輸入輸出清單**（包含型別、預設值、必填防呆）。
- **卡片 2：語法規格與正規表示式/JSON/文字模板規範**。
- **卡片 3：運行時異常處置與報警等級**（如阻斷流程、跳過繼續、彈窗提示）。
- **卡片 4：實戰範例與 C# 底層邏輯代碼**（以 Consolas 字型與深灰背景展示）。

---

## 🔄 4. 雙向同步維護鐵律 (Documentation Synchronization Rule)

每當軟體功能有新增、修改或更名時：
1. **即時檢閱手冊章節**：確認對應的 XAML 說明手冊章節是否已加入新參數、新抽取方法或新防呆邏輯。
2. **即時檢視架構流程圖**：若資料流或外部接口改變，必須同步更新向量圖檔與說明，杜絕「代碼已更新、文檔仍是舊版」的軟體工程負債。
