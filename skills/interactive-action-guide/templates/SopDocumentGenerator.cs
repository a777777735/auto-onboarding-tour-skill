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

    public static class SopDocumentGenerator
    {
        public static string GenerateMarkdown(string appTitle, string modeName, List<SopStepInfo> steps, string outputDirectory)
        {
            if (steps == null || steps.Count == 0) return string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine(string.Format("# {0} 操作指引手冊 (SOP)", string.IsNullOrEmpty(appTitle) ? "系統" : appTitle));
            sb.AppendLine();
            sb.AppendLine(string.Format("> 產生模式：`{0}` | 步驟總數：`{1}` | 產出時間：`{2}`",
                modeName, steps.Count, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            sb.AppendLine();
            sb.AppendLine("## 目錄");
            for (int i = 0; i < steps.Count; i++)
            {
                var s = steps[i];
                sb.AppendLine(string.Format("- [步驟 {0}：{1}](#步驟-{0}{2})", s.StepIndex, s.Title, s.Title.Replace(" ", "-")));
            }
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();

            for (int i = 0; i < steps.Count; i++)
            {
                var s = steps[i];
                sb.AppendLine(string.Format("### 步驟 {0}：{1}", s.StepIndex, s.Title));
                sb.AppendLine();
                sb.AppendLine(string.Format("**🎯 目標元件**：`{0}` &nbsp;&nbsp;|&nbsp;&nbsp; **觸發動作**：`{1}`",
                    string.IsNullOrEmpty(s.TargetName) ? "未指定控制項" : s.TargetName,
                    string.IsNullOrEmpty(s.ActionType) ? "點擊或操作" : s.ActionType));
                sb.AppendLine();
                sb.AppendLine(string.Format("**📝 操作說明**：  \n{0}", s.Content));
                sb.AppendLine();

                if (!string.IsNullOrEmpty(s.ScreenshotRelativePath))
                {
                    sb.AppendLine(string.Format("![步驟 {0} 操作畫面]({1})", s.StepIndex, s.ScreenshotRelativePath.Replace("\\", "/")));
                    sb.AppendLine();
                }

                sb.AppendLine("---");
                sb.AppendLine();
            }

            sb.AppendLine("## 產出備註");
            sb.AppendLine("- 本手冊由 `InteractiveActionGuide` 實戰互動導引引擎自動截圖與排版生成。");
            sb.AppendLine("- 如有任何操作疑問，請洽詢系統管理員或再次點擊系統右上方「使用導覽」按鈕重播。");

            string docContent = sb.ToString();

            try
            {
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }
                string mdPath = Path.Combine(outputDirectory, "USER_MANUAL.md");
                File.WriteAllText(mdPath, docContent, Encoding.UTF8);
            }
            catch { }

            return docContent;
        }
    }
}
