# TheOne.Avalonia documentation

## Start here / 从这里开始

| Task / 任务 | English | 简体中文 |
| --- | --- | --- |
| Integrate an existing project, migrate a UI, or hand work to Codex | [Integration and migration](integration/en/README.md) | [项目接入与迁移](integration/zh-cn/README.md) |
| Find control contracts and examples | [Controls](controls/en/README.md) | [控件](controls/zh-cn/README.md) |
| Look up every declared property, style, API and built-in model | [Complete API](api/en/README.md) | [完整 API 与模型](api/zh-cn/README.md) |
| Configure colors, sizes and motion | [Themes](theme/en/README.md) | [主题](theme/zh-cn/README.md) |

The integration guide contains a portable coding-agent brief. It does not depend on a maintainer's local `AGENTS.md`, skills, absolute paths, or old build outputs. Start with the target project's own instructions, then read only the control documents relevant to the migration.

接入指南包含可复制给 Codex 的任务说明，不依赖维护者本机的 `AGENTS.md`、Skill、绝对路径或历史构建产物。先遵守目标项目自己的约定，再按待迁移功能读取对应文档。

Baseline / 文档基线：TheOne.Avalonia + TheOne.Avalonia.DataGrid **1.0.0-preview.3**, **.NET 10**, **Avalonia 12.1.4**. Package versions, upstream DataGrid version and assembly identities are separate; see the integration guide.

The API reference and laboratory follow the checked-out source; preview.3 predates these additions. In PreviewHost, open **基础控件 → 控件能力实验室**, or use the top-right action on any page. Styles have independent real samples; property editing, event observation and model contracts share the generated catalog. See [reference maintenance](../tools/ControlReferenceGenerator/README.md) for regeneration and drift checks.

API 清单与实验室跟随当前源码，preview.3 发布包早于本轮新增内容。PreviewHost 中进入 **基础控件 → 控件能力实验室**，或点击任意页右上的入口。样式使用独立的实际控件，属性编辑、事件观察和模型说明共用生成清单。更新与检查方法见[文档维护工具](../tools/ControlReferenceGenerator/README.md)。
