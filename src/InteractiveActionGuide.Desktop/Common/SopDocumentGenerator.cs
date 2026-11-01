using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InteractiveActionGuide.Common
{
    public class SopStepInfo
    {
        public int StepIndex { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string TargetName { get; set; }
        public string ActionType { get; set; }
        public string ScreenshotRelativePath { get; set; }
    }

    /// <summary>
    /// 自動產出圖文並茂的標準作業程序 (SOP) 與操作手冊
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
                sb.AppendLine("**操作說明**：");
                sb.AppendLine(s.Content);
                sb.AppendLine();

                if (!string.IsNullOrEmpty(s.ScreenshotRelativePath))
                {
                    sb.AppendLine("**實體操作截圖**：");
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
                if (!Directory.Exists(exportDir)) Directory.CreateDirectory(exportDir);
                string mdPath = Path.Combine(exportDir, "USER_MANUAL.md");
                File.WriteAllText(mdPath, content, Encoding.UTF8);
            }

            return content;
        }
    }
}
