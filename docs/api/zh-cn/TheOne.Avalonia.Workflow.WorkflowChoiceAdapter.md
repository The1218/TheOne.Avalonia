# WorkflowChoiceAdapter

[English](../en/TheOne.Avalonia.Workflow.WorkflowChoiceAdapter.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowChoiceAdapter.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowChoiceAdapter` · Class · `object`

Compiled host projections for stable user/role keys and display text, without property reflection.

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowChoice.cs#L4)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`abstract string WorkflowChoiceAdapter.GetKey(object item)`](../../../TheOne.Avalonia/Workflow/WorkflowChoice.cs#L6) | 签名列明参数类型、可选初值及返回类型；具体操作与异常边界见所属控件用法和链接处实现。 |
| [`abstract string WorkflowChoiceAdapter.GetText(object item)`](../../../TheOne.Avalonia/Workflow/WorkflowChoice.cs#L7) | 使用已配置的类型化适配器读取原始模型显示／搜索文本。 |
| [`static WorkflowChoiceAdapter WorkflowChoiceAdapter.Create<T>(Func<T, string> key, Func<T, string> text)`](../../../TheOne.Avalonia/Workflow/WorkflowChoice.cs#L9) | 通过显式类型参数、模型及委托创建适配器／数据源；签名中的 getter 保留编译类型信息。 |
