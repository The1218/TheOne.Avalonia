# GanttHeaderPresenter

[English](../en/TheOne.Avalonia.Controls.GanttHeaderPresenter.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.GanttHeaderPresenter.md) · [API index](README.md)

`TheOne.Avalonia.Controls.GanttHeaderPresenter` · Class · `Avalonia.Controls.Control`

Template part for GanttChart's frozen calendar and task header.

[Source / 声明](../../../TheOne.Avalonia/Controls/GanttChart.Rendering.cs#L211)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`override void GanttHeaderPresenter.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/GanttChart.Rendering.cs#L214) | Avalonia 绘制回调；使用提供的 DrawingContext 绘制当前状态，业务代码通常不直接调用。 |
| [`GanttHeaderPresenter.GanttHeaderPresenter()`](../../../TheOne.Avalonia/Controls/GanttChart.Rendering.cs#L211) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 样式与密度

所有控件可使用 `small` / `normal` / `large` 或继承的 `ControlSizing.Size`。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

| 类名 | 选择器 | Setter／资源 |
| --- | --- | --- |
| `large` | [`:is(Control).large`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L37) | :is(Control).large => styling:ControlSizing.Size=Large |
| `normal` | [`:is(Control).normal`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L34) | :is(Control).normal => styling:ControlSizing.Size=Normal |
| `small` | [`:is(Control).small`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L31) | :is(Control).small => styling:ControlSizing.Size=Small |
