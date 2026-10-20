# DataGridTreeTextColumn

[English](../en/TheOne.Avalonia.Controls.DataGridTreeTextColumn.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DataGridTreeTextColumn.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DataGridTreeTextColumn` · Class · `Avalonia.Controls.DataGridTextColumn`

Prototype tree text column. Editing uses the original native TextBox and binding.

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridTreeTextColumn.cs#L17)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Indent`](../../../TheOne.Avalonia/Controls/DataGridTreeTextColumn.cs#L23) | `double` | 可写; OneWay | `16` | 每一级层次增加的 DIP 缩进；不改变父子关系。 校验：`value => double.IsFinite(value) && value >= 0` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`DataGridTreeTextColumn.DataGridTreeTextColumn()`](../../../TheOne.Avalonia/Controls/DataGridTreeTextColumn.cs#L17) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<double> DataGridTreeTextColumn.IndentProperty`](../../../TheOne.Avalonia/Controls/DataGridTreeTextColumn.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
