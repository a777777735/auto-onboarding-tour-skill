using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InteractiveActionGuide.Common
{
    /// <summary>
    /// 解耦的步驟元數據資訊模型（供 UI 引導、SOP 手冊、JSON Manifest 獨立取用）
    /// </summary>
    public class SopStepInfo
    {
        public int StepIndex { get; set; }
        public string Title { get; set; }
        
        /// <summary>
        /// UI 實戰引導即時提示文字（短小精幹，適合浮動提示卡）
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 獨立的 SOP / 說明文檔深度業務說明（若未設定則自動 Fallback 到 Content）
        /// </summary>
        public string DetailedDescription { get; set; }

        /// <summary>
        /// 注意事項 / 警告鐵律（選填）
        /// </summary>
        public string Caution { get; set; }

        /// <summary>
        /// 預期操作結果與驗證狀態（選填）
        /// </summary>
        public string ExpectedResult { get; set; }

        public string TargetName { get; set; }
        public string ActionType { get; set; }
        public string ScreenshotRelativePath { get; set; }
    }

    /// <summary>
    /// 自動產出圖文並茂的標準作業程序 (SOP)、Markdown 說明文檔與獨立 JSON 結構資料集
    /// </summary>
    public static class SopDocumentGenerator
    {
        public static string GenerateMarkdown(string appTitle, string guideMode, List<SopStepInfo> steps, string exportDir)
        {
            if (steps == null || steps.Count == 0) return string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine("# " + (string.IsNullOrEmpty(appTitle) ? "系統操作導引與標準作業程序 (SOP)" : appTitle + " - 操作手冊 (SOP)"));
            sb.AppendLine();
            sb.AppendLine("> **產出時間**：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  ");
            sb.AppendLine("> **導引模式**：" + (guideMode == "Concise" ? "⚡ 收斂精簡模式 (4~6 步核心操作)" : "📖 詳細全覆蓋模式 (全功能地毯式引導)") + "  ");
            sb.AppendLine("> **自動產出**：由 `InteractiveActionGuide` 自動截圖與排版引擎產生");
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine("## 📑 操作流程目錄");
            sb.AppendLine();

            for (int i = 0; i < steps.Count; i++)
            {
                var s = steps[i];
                sb.AppendLine(string.Format("{0}. [步驟 {0}：{1}](#步驟-{0}{2})", i + 1, s.Title, s.Title.Replace(" ", "-")));
            }

            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine("## 🛠️ 詳細操作步驟指引");
            sb.AppendLine();

            for (int i = 0; i < steps.Count; i++)
            {
                var s = steps[i];
                sb.AppendLine("### 步驟 " + (i + 1) + "：" + s.Title);
                sb.AppendLine();
                sb.AppendLine("- **目標控制項**：`" + (string.IsNullOrEmpty(s.TargetName) ? "全局視窗" : s.TargetName) + "`");
                sb.AppendLine("- **預期動作**：" + (string.IsNullOrEmpty(s.ActionType) ? "點擊或檢視" : s.ActionType));
                sb.AppendLine();
                
                // 優先使用獨立的 DetailedDescription，若未填則 fallback 到 Content
                string desc = !string.IsNullOrEmpty(s.DetailedDescription) ? s.DetailedDescription : s.Content;
                sb.AppendLine("**📝 操作說明**：");
                sb.AppendLine(desc);
                sb.AppendLine();

                if (!string.IsNullOrEmpty(s.ExpectedResult))
                {
                    sb.AppendLine("> ✅ **預期結果**：" + s.ExpectedResult);
                    sb.AppendLine();
                }

                if (!string.IsNullOrEmpty(s.Caution))
                {
                    sb.AppendLine("> ⚠️ **注意事項**：" + s.Caution);
                    sb.AppendLine();
                }

                if (!string.IsNullOrEmpty(s.ScreenshotRelativePath))
                {
                    sb.AppendLine("**📸 實體操作截圖**：");
                    sb.AppendLine(string.Format("![步驟 {0} 實體畫面]({1})", i + 1, s.ScreenshotRelativePath.Replace('\\', '/')));
                    sb.AppendLine();
                }

                sb.AppendLine("---");
                sb.AppendLine();
            }

            sb.AppendLine("## 💡 總結與注意事項");
            sb.AppendLine("- 請依照上述步驟循序操作，若畫面未即時響應，請檢查網路連線或授權狀態。");
            sb.AppendLine("- 若需重新複習互動流程，可在系統右上角點擊「❓ 操作導覽」隨時喚起引導。");

            string content = sb.ToString();

            if (!string.IsNullOrEmpty(exportDir))
            {
                try
                {
                    if (!Directory.Exists(exportDir))
                    {
                        Directory.CreateDirectory(exportDir);
                    }
                    string mdPath = Path.Combine(exportDir, "USER_MANUAL.md");
                    File.WriteAllText(mdPath, content, Encoding.UTF8);

                    // 同步匯出獨立的 JSON 結構資料集，方便外部系統、API 或 Wiki 二次解析
                    ExportManifestJson(appTitle, guideMode, steps, exportDir);
                }
                catch { }
            }

            return content;
        }

        /// <summary>
        /// 將步驟資訊獨立匯出為結構化 JSON Manifest（純 C# 7.3 零外部依賴序列化）
        /// 供後續外部文檔工具、企業知識庫或其他 Agent 二次處理
        /// </summary>
        public static string ExportManifestJson(string appTitle, string guideMode, List<SopStepInfo> steps, string exportDir)
        {
            if (steps == null) return string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine(string.Format("  \"appTitle\": {0},", EscapeJson(appTitle ?? "系統操作導引")));
            sb.AppendLine(string.Format("  \"guideMode\": {0},", EscapeJson(guideMode ?? "Concise")));
            sb.AppendLine(string.Format("  \"generatedAt\": {0},", EscapeJson(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"))));
            sb.AppendLine("  \"steps\": [");

            for (int i = 0; i < steps.Count; i++)
            {
                var s = steps[i];
                sb.AppendLine("    {");
                sb.AppendLine(string.Format("      \"stepIndex\": {0},", s.StepIndex));
                sb.AppendLine(string.Format("      \"title\": {0},", EscapeJson(s.Title)));
                sb.AppendLine(string.Format("      \"content\": {0},", EscapeJson(s.Content)));
                sb.AppendLine(string.Format("      \"detailedDescription\": {0},", EscapeJson(s.DetailedDescription)));
                sb.AppendLine(string.Format("      \"caution\": {0},", EscapeJson(s.Caution)));
                sb.AppendLine(string.Format("      \"expectedResult\": {0},", EscapeJson(s.ExpectedResult)));
                sb.AppendLine(string.Format("      \"targetName\": {0},", EscapeJson(s.TargetName)));
                sb.AppendLine(string.Format("      \"actionType\": {0},", EscapeJson(s.ActionType)));
                sb.AppendLine(string.Format("      \"screenshotPath\": {0}", EscapeJson(s.ScreenshotRelativePath)));
                sb.Append("    }");
                if (i < steps.Count - 1) sb.Append(",");
                sb.AppendLine();
            }

            sb.AppendLine("  ]");
            sb.AppendLine("}");

            string json = sb.ToString();

            if (!string.IsNullOrEmpty(exportDir))
            {
                try
                {
                    if (!Directory.Exists(exportDir))
                    {
                        Directory.CreateDirectory(exportDir);
                    }
                    string jsonPath = Path.Combine(exportDir, "guide_manifest.json");
                    File.WriteAllText(jsonPath, json, Encoding.UTF8);
                }
                catch { }
            }

            return json;
        }

        private static string EscapeJson(string str)
        {
            if (str == null) return "null";
            var sb = new StringBuilder("\"");
            foreach (char c in str)
            {
                switch (c)
                {
                    case '\\': sb.Append("\\\\"); break;
                    case '\"': sb.Append("\\\""); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        if (c < ' ')
                        {
                            sb.AppendFormat("\\u{0:x4}", (int)c);
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            sb.Append("\"");
            return sb.ToString();
        }
    }
}
