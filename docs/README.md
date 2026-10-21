# TheOne.Avalonia documentation

## Start here / 从这里开始

| Task / 任务 | English | 简体中文 |
| --- | --- | --- |
| Integrate an existing project, migrate a UI, or hand work to Codex | [Integration and migration](integration/en/README.md) | [项目接入与迁移](integration/zh-cn/README.md) |
| Use project-added properties, labeled Classes examples and business actions | [Control usage](controls/en/README.md) | [控件用法](controls/zh-cn/README.md) |
| Browse actual control images and Light / Dark comparisons | [Visual index](controls/en/visual-index.md) | [控件图鉴与明暗对照](controls/zh-cn/visual-index.md) |
| Precisely inspect all public declarations, constructors, accessors and identifiers | [Complete source contract](api/en/README.md) | [完整源码契约](api/zh-cn/README.md) |
| Configure colors, sizes and motion | [Themes](theme/en/README.md) | [主题](theme/zh-cn/README.md) |
| Use the 320 built-in vector icons | [OneIcon](controls/en/one-icon.md) | [自有图标](controls/zh-cn/one-icon.md) |
| Build a breadcrumb navigation path | [Breadcrumb](controls/en/breadcrumb.md) | [面包屑](controls/zh-cn/breadcrumb.md) |
| Select a combined date and time | [DateTimePicker](controls/en/datetime-picker.md) | [日期时间选择](controls/zh-cn/datetime-picker.md) |
| Display an event history | [Timeline](controls/en/timeline.md) | [时间线](controls/zh-cn/timeline.md) |
| Review release changes, upgrade notes and verification limits | [Preview.11 release notes](releases/en/1.0.0-preview.11.md) | [Preview.11 发行说明](releases/zh-cn/1.0.0-preview.11.md) |

The integration guide contains a portable coding-agent brief. It does not depend on a maintainer's local `AGENTS.md`, skills, absolute paths, or old build outputs. Start with the target project's own instructions, then read only the control documents relevant to the migration.

接入指南包含可复制给 Codex 的任务说明，不依赖维护者本机的 `AGENTS.md`、Skill、绝对路径或历史构建产物。先遵守目标项目自己的约定，再按待迁移功能读取对应文档。

Baseline / 文档基线：TheOne.Avalonia + TheOne.Avalonia.DataGrid **1.0.0-preview.11**, **.NET 10**, **Avalonia 12.1.4**. Package versions, upstream DataGrid version and assembly identities are separate; see the integration guide. Package availability is determined by the NuGet feed, not this source version. / 是否已可安装以 NuGet 源中的实际发布状态为准。

The source contract and per-control previews follow the checked-out source; preview.11 is the release baseline for these updates. Each control page shows labeled Classes examples, project-specific properties, actual business actions and related models. Property and action descriptions include short code examples; raw style selectors, constructor/accessor lists and identifiers stay in the separate source contract. Shared sizing and motion settings are centralized on Home. Only Button, Input and Select share comparison pages. Complete demos retain their original data and layout; canvas editors keep their full viewport with reference content in a separate tab. See [reference maintenance](../tools/ControlReferenceGenerator/README.md) for regeneration and drift checks.

源码契约与各控件预览跟随当前源码，本轮更新的发行基线为 preview.11。每个控件页展示 Classes 标签样例、专属属性、实际业务动作及关联模型，属性与动作说明附短示例代码；原始样式选择器、构造／访问器清单和标识符另保留在源码契约中。尺寸、动效等通用配置集中在 Home。仅 Button、Input、Select 合并比较，完整示例保留原始数据与布局。画布编辑器保留完整视口，说明位于单独的页内标签。更新与检查方法见[文档维护工具](../tools/ControlReferenceGenerator/README.md)。

Planning / 规划：[Control and chart roadmap / 控件与图表扩展路线图](plans/control-and-chart-roadmap.md)。TreeSelect entered preview.7; OneIcon, Breadcrumb, DateTimePicker and Timeline entered preview.8. Preview.9 adds Heatmap, Radar, Funnel, BoxPlot, Candlestick, Treemap, Sunburst and AttachmentList. Other proposals remain plans. / TreeSelect 在 preview.7 落地，OneIcon、Breadcrumb、DateTimePicker 和 Timeline 在 preview.8 落地；preview.9 新增 Heatmap、Radar、Funnel、BoxPlot、Candlestick、Treemap、Sunburst 和 AttachmentList，其余建议仍为规划。
