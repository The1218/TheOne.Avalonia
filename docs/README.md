# 文档索引

先选任务，再读对应文件；无需通读本目录。

| 任务 | 入口 |
| --- | --- |
| 接入、版本、运行预览 | [仓库 README](../README.md) |
| 控件用法、属性及示例 | [控件索引](controls/README.md) |
| 颜色、尺寸、动效、模板维护 | [主题](theme/README.md) |
| 选择测试范围与命令 | [测试策略](workflow/testing.md) |
| NuGet 双包版本与发布 | [发布策略](workflow/releasing.md) |
| 编译绑定、裁剪、原生消费 | [NativeAOT](native-aot.md) |
| 演示页面与 ViewModel 组织 | [PreviewHost](preview-host.md) |
| DataGrid 后续实现 | [当前计划](plans/datagrid.md) |
| Avalonia / Skia 开发依据 | [开发依据](design/avalonia-skia-notes.md) |
| 旧设计和历史验证 | [归档索引](archive/README.md)，仅追溯时读取 |

项目维护流程 skill 位于 [.agents/skills/theone-maintenance/SKILL.md](../.agents/skills/theone-maintenance/SKILL.md)。提示词直接描述目标、涉及控件与验收条件，不粘贴整份文档；AGENTS 保留强约束，skill 负责路由，API 留在控件文档，测试命令只在测试策略维护。

维护时更新现有章节，不追加每轮流水账。文档检查限制 AGENTS 2.2 KB、skill 3 KB、README 入口 6 KB、普通文档 12 KB；归档免体积限制。较长 API 按功能拆分，入口给出明确链接。日志／图片放 `.preview/<任务>/`，结论报告注明测试范围和未验证边界，不用累计通过数量代替当前证据。
