# TheOne.Avalonia 项目约定

- .NET 10；Avalonia 12.1.4；DataGrid 12.1.2。公共主题／行为在 `TheOne.Avalonia`；PreviewHost 只做演示与验证。
- 保留官方控件类型、`PART_*`、键盘和选择行为；使用 Still 语义色，受影响控件覆盖 Light / Dark × small / normal / large。未重写的 Fluent 模板不算完成。
- 可增长集合使用模型 `ItemsSource`、按需模板及有界视口，默认虚拟化与回收；包括树各级、标签、建议、菜单。普通列表保留 VirtualizingStackPanel；Carousel、DataGrid、日期时间选择器保留官方专用面板／Presenter。禁止预创建大量控件或无限布局约束。
- 集合改动检查 1,000 项的实际容器数量、远处项操作及回收状态，并提供交互预览。树模型身份独立；跨重载状态双向绑定 IsExpanded；完整数据遍历模型。
- DataGrid 保留原生行／单元格 Presenter；有限 MaxColumnWidth 在创建列前配置。AOT 显式编译列绑定、关闭自动生成，并调用 `DataGridAot.PreserveRowType<T>()`。
- 模板、预览和消费示例使用编译绑定／TemplateBinding；MultiBinding 子项显式 CompiledBindingExtension。保留 IsAotCompatible；禁止 ReflectionBinding、关闭编译绑定、整程序集保留或 NoWarn 绕过问题。
- 测试按[改动范围](docs/workflow/testing.md)选择：代码／XAML 构建并跑相关专项；纯文档只做文档检查。公共基础设施或交付审查才跑全量；不因局部修复默认重复全量。截图只导出受影响控件，`--all` 仅在用户明确要求时使用。
- 默认只读本文件与[文档索引](docs/README.md)，再读取相关章节；不批量加载 docs、历史日志或全部 skills。计划只记录当前状态、下一步与验收条件；历史记录不作为当前约定。
