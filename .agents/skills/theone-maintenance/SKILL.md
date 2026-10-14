---
name: theone-maintenance
description: 维护 TheOne.Avalonia 的控件、主题、图表、DataGrid AOT 与 PreviewHost；用于这个仓库的修复、功能调整和按范围验证，不用于其他 Avalonia 项目或自动发布。
---

# TheOne 控件库维护

从工作目录向上找到包含 TheOne.Avalonia.slnx 的仓库，读取根 AGENTS.md。只读取当前控件及直接复用的模板、绘制或绑定路径。

按任务定位：

- 控件状态／布局：Controls、Themes/Controls、Sizing 和对应预览页；保留原生输入、焦点、命令和回收。
- 图表：ChartBase 的对应 partial、具体图表类型和 ChartPreview；同时检查自然尺寸与组合页窄宽，比较实际绘制像素，不能只验证默认大窗口。
- DataGrid：源码 vendor 与主库附加行为一起检查；先追警告调用点，采用编译访问器、明确转换或逐类型元数据，不扩大保留范围。
- 桌面启动：先检查 PreviewHost.csproj 和 Program.cs，再检查宿主产物；Windows GUI 输出检查 apphost PE 子系统，命令行消费者单独保留。

专项入口以 `design/PreviewHost/Program.cs` 和 `design/PreviewHost/Consumers/NativeAotSmoke/Program.cs` 为准；按当前控件文档的命令选择受影响范围。验证失败先判断产品缺陷、错误假设或时序依赖；修复后重跑失败范围，不删除有效断言以换取通过。新专项只有在能重复定位真实行为问题时才增加。

公开控件契约同步 `docs/controls/`，主题契约同步 `docs/theme/`；验证日志和平台证据放入 `.preview/`，不恢复独立测试策略、发布或预览结构文档。结论列出改动、关键兼容条件、实际验证和未覆盖宿主；不要将用户平台反馈扩写成未提供的 RID／AOT／GPU 证据。

需要可直接复用的任务输入时读取 [references/prompts.md](references/prompts.md)。提示词是任务起点，不自动授权推送或发布。
