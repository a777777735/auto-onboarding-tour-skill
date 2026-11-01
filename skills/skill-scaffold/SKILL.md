---
name: skill-scaffold
category: Meta
description: 自動腳手架元技能。當使用者要求新增任何自訂 AI Skill 時，自動在中央倉庫建立標準結構與 YAML Frontmatter，並觸發全域同步。
auto_invoke: contextual
file_patterns: "SKILL.md, *.ps1, *.sh"
trigger_condition: 當使用者要求新增、建立、打包或擴充一個新的 AI Agent Skill 時，必須主動調用此技能以產生符合標準規範的骨架並自動同步全域。
---

# Skill Scaffold (元技能 Meta-Skill)

本 Skill 專門用於「**新增技能**」時的自動骨架產生與全域動態註冊。確保所有擴充的技能皆遵循嚴格的 YAML Frontmatter 規格，並透過自動化腳本實現 Zero-Touch 自動掛載。

---

## 🛠️ 新增技能 3 大標準流程

### Step 1: 解析與定義 Frontmatter
在 `skills/<new-skill-name>/SKILL.md` 必須包含 6 大欄位：
```yaml
---
name: <kebab-case-name>
category: UI/UX | Onboarding | Meta | Architecture | Automation
description: <簡潔明確的功能描述>
auto_invoke: always | contextual
file_patterns: "<例如 *.html, *.cs, *.py>"
trigger_condition: <一句話明確說明 AI 在何時必須主動讀取並執行此技能>
---
```

### Step 2: 建立結構與模板
1. 建立目錄：`skills/<new-skill-name>/`
2. 建立主檔：`skills/<new-skill-name>/SKILL.md`（包含核心規範、防呆機制、執行步驟與代碼範例）
3. 若包含可重複利用的組件或腳本，建立 `skills/<new-skill-name>/templates/`。

### Step 3: 執行全域自動編譯與同步
新增完成後，**AI 必須主動執行中央倉庫的同步腳本**：
- Windows: `powershell -ExecutionPolicy Bypass -File .\sync-to-global.ps1`
- macOS/Linux: `./sync-to-global.sh`

腳本將自動：
1. 掃描新技能的 Frontmatter。
2. 自動更新中央倉庫的 [`SKILLS_INDEX.md`](../../SKILLS_INDEX.md)。
3. 同步至本機全域 `~/.gemini/antigravity/`。
4. 自動將新技能編譯寫入全域 `GEMINI.md` 動態路由表中，立即生效！
