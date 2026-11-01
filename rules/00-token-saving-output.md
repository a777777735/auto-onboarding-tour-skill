# Global Token-Saving & Output Minimization Policy (全域省 Token 與極簡回覆鐵律)

在所有對話與任務執行中，你必須嚴格遵守以下輸出限制，將 Token 100% 用於實際的檔案修改工具，而非聊天室文字輸出：

1. **禁止在對話框印出完整代碼 (NO Full Code Dumps)**：
   - 當建立或修改任何檔案（`.html`, `.css`, `.js`, `.cs`, `.xaml` 等）時，**必須直接使用檔案編輯工具（File Write / Replace / Edit Tools）在背景寫入硬碟**。
   - **嚴禁**在聊天回覆（Chat Response）中重複貼出完整的檔案內容或大段程式碼。

2. **局部修改優先 (Surgical Diff Edits Only)**：
   - 修改現有專案檔案時，優先使用局部替換工具（Search & Replace / Multi-Edit）只修改變動的行數，不要每次都重寫整個大檔案。

3. **聊天室回覆長度限制 (Ultra-Concise Summary)**：
   - 除非使用者明確要求「在對話框印出代碼給我看」，否則聊天室內的單一程式碼區塊（Code Block）**不得超過 10 行**。
   - 任務完成後的回覆必須採用以下極簡格式（控制在 100 字以內）：
     - **📂 已修改/新增檔案**：列出檔案路徑與 1 句話改動重點。
     - **🔨 驗證狀態**：列出 `dotnet build` 或腳本執行的結果（如 `0 Error(s)`）。
