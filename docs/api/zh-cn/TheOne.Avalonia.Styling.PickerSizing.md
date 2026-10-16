# PickerSizing

[English](../en/TheOne.Avalonia.Styling.PickerSizing.md) · [简体中文](../zh-cn/TheOne.Avalonia.Styling.PickerSizing.md) · [API index](README.md)

`TheOne.Avalonia.Styling.PickerSizing` · Class · `Avalonia.AvaloniaObject`

Shared row metrics keep the native picker viewport and selection band aligned.

[Source / 声明](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L7)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ItemHeight`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L9) | `double` | 可写; OneWay; 附加属性; 向后代继承 | `40` | 单项／滚轮单元的 DIP 高度；主题密度和显式局部值可能覆盖注册初值。 |
| [`PopupHeight`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L10) | `double` | 可写; OneWay; 附加属性; 向后代继承 | `340` | 选择弹层的 DIP 高度；与 ItemHeight 和 ViewportHeight 协调。 |
| [`ViewportHeight`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L11) | `double` | 可写; OneWay; 附加属性; 向后代继承 | `200` | 选择滚轮可见区域的 DIP 高度；不等于数据项数量。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static double PickerSizing.GetViewportHeight(Control control)`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L12) | 读取附加属性。选择滚轮可见区域的 DIP 高度；不等于数据项数量。 |
| [`static void PickerSizing.SetViewportHeight(Control control, double value)`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L13) | 设置附加属性。选择滚轮可见区域的 DIP 高度；不等于数据项数量。 |
| [`static double PickerSizing.GetItemHeight(Control control)`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L14) | 读取附加属性。单项／滚轮单元的 DIP 高度；主题密度和显式局部值可能覆盖注册初值。 |
| [`static void PickerSizing.SetItemHeight(Control control, double value)`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L15) | 设置附加属性。单项／滚轮单元的 DIP 高度；主题密度和显式局部值可能覆盖注册初值。 |
| [`static double PickerSizing.GetPopupHeight(Control control)`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L16) | 读取附加属性。选择弹层的 DIP 高度；与 ItemHeight 和 ViewportHeight 协调。 |
| [`static void PickerSizing.SetPopupHeight(Control control, double value)`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L17) | 设置附加属性。选择弹层的 DIP 高度；与 ItemHeight 和 ViewportHeight 协调。 |
| [`PickerSizing.PickerSizing()`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L7) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly AttachedProperty<double> PickerSizing.ItemHeightProperty`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L9) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<double> PickerSizing.PopupHeightProperty`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L10) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<double> PickerSizing.ViewportHeightProperty`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L11) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
