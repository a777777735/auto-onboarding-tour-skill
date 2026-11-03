# Global Rule: UI Layout & Industrial Theme Standards
# 規則編號：01-ui-layout-and-theme
# 適用範圍：Web (HTML/CSS/React/Vue)、C# WPF (.xaml)、C# WinForms

---

## 🎯 核心宗旨
本規則為全域常駐鐵律。所有 AI 在生成、修改、美化或重構任何用戶介面（Web 網頁或 C# 桌面視窗）時，**必須無條件遵守以下防呆規範**，杜絕「介面跑版、輸入框大小失控、元件重疊、邊界撐破、色彩刺眼與對比不足」等常見缺陷。

---

## 🎨 1. 雙主題工控色票規範 (Dual Industrial Theme Standards)
嚴禁隨機硬編碼純原色（如 `#FF0000`, `#0000FF`），一律採用標準工業級色票：

### ☀️ A. FormatString 經典工控淺色主題 (Industrial Slate Light)
| 色票識別 | 色碼 (HEX) | 用途與規範說明 | 對比標準 |
| :--- | :--- | :--- | :--- |
| **`Bg-Primary`** | `#F7F9FC` (Slate 50) | 全域視窗畫布背景、最底層容器 | 搭配 `Text-Primary` 清爽護眼 |
| **`Bg-Surface`** | `#FFFFFF` (White) | 卡片容器、面板、表單區域背景 | 搭配 `BorderLight` 輕盈分割 |
| **`Bg-Elevated`** | `#F1F5F9` (Slate 100) | 頂部工具列、表格標題列 (Header) | 結構層次明確 |
| **`Bg-RowAlt`** | `#F8FAFC` (Slate 50) | 表格偶數列交替色 | 提升資料掃視性 |
| **`BorderLight`** | `#E2E8F0` (Slate 200) | 卡片邊框、網格分隔線 | 柔和清晰 |
| **`BorderMedium`** | `#CBD5E1` (Slate 300) | 工具列外框、輸入框邊框 | 幾何界線明確 |
| **`Text-Primary`** | `#1A252F` (Slate 900) | 內容文字、輸入框文字 | 高度清晰 (>13:1) |
| **`Text-Heading`** | `#1E293B` (Slate 800) | 區塊標題、重要欄位名 | 加粗加深 |
| **`Text-Muted`** | `#64748B` (Slate 500) | 次要標籤、提示說明 | 符合 WCAG AA |
| **`Primary`** | `#0284C7` (Sky 600) | 核心動作按鈕、聚焦外框 (Focus) | Hover: `#0369A1` |
| **`Success`** | `#10B981` (Emerald 500) | 儲存、執行成功、綠色按鈕 | Hover: `#059669` |
| **`Accent`** | `#8B5CF6` (Violet 500) | 掃描、高階規則生成按鈕 | Hover: `#7C3AED` |

### 🌙 B. 工業級深色主題 (Industrial Slate Dark)
| 色票識別 | 色碼 (HEX) | 用途與規範說明 | 對比標準 |
| :--- | :--- | :--- | :--- |
| **`Bg-Primary`** | `#0F172A` (Slate 900) | 全域視窗畫布背景、最底層容器 | 搭配 `Text-Primary` |
| **`Bg-Surface`** | `#1E293B` (Slate 800) | 卡片容器、面板、側邊導航背景 | 搭配 `Border` 柔和分割 |
| **`Bg-Elevated`** | `#334155` (Slate 700) | 按鈕 Hover 態、輸入框底色、標籤背景 | 強化階層感 |
| **`Border`** | `#334155` (Slate 700) | 面板外框、格線邊框（1px solid） | 結構分界 |
| **`Text-Primary`** | `#F8FAFC` (Slate 50) | 標題文字、核心讀數、主按鈕文字 | 對比度 > 12:1 (極高) |
| **`Text-Muted`** | `#94A3B8` (Slate 400) | 次要文字、輸入框 Placeholder、時間標籤 | 符合 WCAG AA |
| **`Primary`** | `#0284C7` (Sky 600) | 核心動作按鈕、聚焦光圈 (Focus Ring) | Hover: `#0369A1` |

---

## 📐 2. 輸入框尺寸鎖定與網格防跑版鐵律 (Input Size & Grid Layout Protocol)

所有新開發或重構的操作介面，**輸入框（TextBox / Input）嚴禁隨字體大小或文字長度自由伸縮變形**，必須強制依循以下原則：

1. **單行輸入框高度鎖定 (Lock Height)**：
   - WPF：強制顯式宣告 `Height="24"` 或 `Height="26"`，嚴禁設為 `Height="Auto"`。
   - Web：強制設定 `height: 26px; box-sizing: border-box;`。
2. **文字垂直置中 (Vertical Center Alignment)**：
   - WPF：必須加上 `VerticalContentAlignment="Center"`，文字不得偏靠頂端或底端。
   - Web：使用 `line-height` 或 Flex 垂直對齊。
3. **固定內邊距 (Consistent Padding)**：
   - WPF 統一 `Padding="6,0,6,0"` 或 `Padding="4,2"`；Web 統一 `padding: 0 8px;`。
4. **寬度合理比例分配 (Ratio & Min-Width Allocation)**：
   - **表單網格**：一律使用 `Grid`，標籤欄為 `Width="Auto"`，輸入欄為 `Width="*"`（搭配 `MinWidth="80"`），由 Grid 自動適配視窗縮放。
   - **水平並排**：在 `StackPanel` 或 `WrapPanel` 內，每個輸入框**必須顯式指定寬度**（如 `Width="38"`, `Width="85"`, `Width="160"`, `Width="260"`），杜絕無限制撐破外層容器。
5. **多行文字與編輯器 (Multi-line Stretch)**：
   - 必須透過 `HorizontalAlignment="Stretch"` 與 `VerticalAlignment="Stretch"` 依附在 `Grid` 的 `*` 比例列，內部啟用 `VerticalScrollBarVisibility="Auto"`，嚴禁因內容行數變多撐爆對話框。

---

## 🌐 3. Web (HTML / CSS) 佈局防呆鐵律
1. **CSS 變數封裝先行**：`:root` 定義全套變數，全篇樣式嚴禁直接硬編碼 Hex 色碼。
2. **App Shell 視窗鎖定架構**：`html, body { height: 100vh; margin: 0; overflow: hidden; }`，禁止出現雙重捲軸。
3. **Flex / Grid 防撐破機制**：子容器必須顯式加上 `min-width: 0; min-height: 0;`。
4. **4px 幾何網格標準**：元素外距、內距與間隔統一使用 `4px` 之倍數。

---

## 🖥️ 4. C# WPF (.xaml) 佈局防呆鐵律
1. **嚴禁絕對座標排版**：嚴禁使用大數值 `Margin="215,86,0,0"` 定位元件，一律採用 `Grid` 比例分行分列。
2. **ResourceDictionary 集中管理**：引用主題資源字典（`WpfFormatStringTheme.xaml` 或 `WpfIndustrialTheme.xaml`），全域套用控制項樣式。
3. **垂直置中與內距規範**：輸入框與按鈕統一設定 `VerticalContentAlignment="Center"`。

---

## 🪟 5. C# WinForms 佈局防呆鐵律
1. **嚴禁純絕對座標定位**：採用 `TableLayoutPanel`、`SplitContainer` 搭配 `Dock = DockStyle.Fill` 或 `Anchor`。
2. **高 DPI 與統一字體修正**：Form 初始化設定 `this.AutoScaleMode = AutoScaleMode.Dpi;` 與 `this.DoubleBuffered = true;`，預設字體一律顯式指派為現代字體（`Microsoft JhengHei UI` 或 `Segoe UI`）。

---

## 📊 6. DataGrid 與輸入欄位自適應及超長內容滾動規範
1. **同行超長不換行原則 (NoWrap)**：
   - 變數鍵值、單元格字串展示欄位（如 DataGrid 中的即時數值、變數鏈結），同行文字長度超過欄寬時嚴禁自動折行（`TextWrapping="NoWrap"`）。
   - 只有在字串中存在顯式換行符號（`\r\n` 或 `\n`）時才進行分行計算高度；同行文字橫向超長時保持單行，可搭配水平捲軸或浮動 `ToolTip` 檢視完整內容。
2. **杜絕下拉滾動跳整行 (Pixel Smooth Scrolling)**：
   - 當單行因多行內容變高時，WPF 預設的邏輯滾動（`ScrollViewer.CanContentScroll="True"`）會導致下拉捲軸一次跳過整行，使得中間過長內容無法被看清。
   - **強制設定**：在 DataGrid 上必須顯式設定 `ScrollViewer.CanContentScroll="False"` 與 `VirtualizingPanel.ScrollUnit="Pixel"`，並啟用 `HorizontalScrollBarVisibility="Auto"` 與 `VerticalScrollBarVisibility="Auto"`。
   - 此設定將邏輯項目滾動轉換為物理像素級平滑滾動，使用者滾動時可平滑停留在任何高度，完整檢視過長行內的每一個字元。
