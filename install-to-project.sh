#!/usr/bin/env bash
# ==============================================================================
# install-to-project.sh - AI Agent Skills & Rules 跨平台集中同步部署腳本 (macOS / Linux)
# ==============================================================================

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SOURCE_RULES="$SCRIPT_DIR/rules"
SOURCE_SKILLS="$SCRIPT_DIR/skills"

TARGET_PROJECT="$1"

echo "=========================================================="
echo " 🚀 AI Agent Skills & Rules 集中同步部署工具 (Bash)"
echo "=========================================================="

if [ ! -d "$SOURCE_RULES" ] || [ ! -d "$SOURCE_SKILLS" ]; then
    echo "❌ 錯誤：未在腳本目錄找到 rules/ 或 skills/ 資料夾！"
    exit 1
fi

# 1. 同步至本機全域環境 (~/.agents)
GLOBAL_AGENT_DIR="$HOME/.agents"
echo -e "\n[1/2] 正在同步至本機全域設定目錄 ($GLOBAL_AGENT_DIR)..."
mkdir -p "$GLOBAL_AGENT_DIR/rules" "$GLOBAL_AGENT_DIR/skills"

cp -R "$SOURCE_RULES/"* "$GLOBAL_AGENT_DIR/rules/"
cp -R "$SOURCE_SKILLS/"* "$GLOBAL_AGENT_DIR/skills/"
echo "  [OK] 全域 Rules 與 Skills 同步完成！"

# 2. 同步至目標專案目錄 (若有提供參數)
if [ -n "$TARGET_PROJECT" ]; then
    echo -e "\n[2/2] 正在同步至目標專案 ($TARGET_PROJECT)..."
    mkdir -p "$TARGET_PROJECT/.agents/rules" "$TARGET_PROJECT/.agents/skills"
    
    cp -R "$SOURCE_RULES/"* "$TARGET_PROJECT/.agents/rules/"
    cp -R "$SOURCE_SKILLS/"* "$TARGET_PROJECT/.agents/skills/"
    echo "  [OK] 目標專案 Rules 與 Skills 同步完成！"
else
    echo -e "\n[2/2] 未指定目標專案路徑，已完成全域同步。"
fi

echo -e "\n✨ 同步作業全部順利完成！\n"
