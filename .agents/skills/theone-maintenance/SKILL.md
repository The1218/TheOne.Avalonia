---
name: theone-maintenance
description: 在 TheOne.Avalonia 仓库维护主题、控件、预览和文档时，按任务读取接口与选择必要的专项验证。
---

# TheOne 项目维护

适用于本仓库。仓库根目录为本文件的 `../../..`。

1. 先读根目录 [AGENTS.md](../../../AGENTS.md) 与[文档索引](../../../docs/README.md)；通过 `rg` 定位受影响控件和调用处，只读对应 API 章节。模板改动再读主题文档，绑定／裁剪改动再读 NativeAOT 文档。
2. 延续现有控件、模板和 ViewModel 分层。公共逻辑放类库，演示与验证放 PreviewHost；集合改动以实际回收和远处操作为验收依据。
3. 按[测试策略](../../../docs/workflow/testing.md)选择构建、专项、截图和原生验证。局部改动不默认跑完整控件回归或完整 AOT；已有检查通过后，仅在新增改动或疑点出现时重跑。
4. API 变化更新对应文档；计划只维护状态、待办与验收，不重复 API。日志保存 `.preview/<任务>/`。报告完成内容、测试范围与未验证部分。

纯文档整理运行 `python3 scripts/check-docs.py`。不要自动读取归档、所有文档或其他无关 skills。
