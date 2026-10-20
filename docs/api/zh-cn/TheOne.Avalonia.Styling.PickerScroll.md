# PickerScroll

[English](../en/TheOne.Avalonia.Styling.PickerScroll.md) · [简体中文](../zh-cn/TheOne.Avalonia.Styling.PickerScroll.md) · [API index](README.md)

`TheOne.Avalonia.Styling.PickerScroll` · Class · `Avalonia.AvaloniaObject`

Finishes fractional wheel offsets without replacing native picker selection or recycling.

[Source / 声明](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L15)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L17) | `bool` | 可写; OneWay; 附加属性 | `false` | 日期／时间选择滚轮的交互开关；不替换原生提交、取消和焦点行为。 |
| [`HighlightClip`](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L20) | `Avalonia.Media.Geometry` | 可写; OneWay; 附加属性 | `new RectangleGeometry(new Rect())` | 滚轮高亮区域的裁剪配置；通常由选择器模板维护。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static bool PickerScroll.GetEnabled(DateTimePickerPanel panel)`](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L54) | 读取附加属性。日期／时间选择滚轮的交互开关；不替换原生提交、取消和焦点行为。 |
| [`static void PickerScroll.SetEnabled(DateTimePickerPanel panel, bool value)`](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L59) | 设置附加属性。日期／时间选择滚轮的交互开关；不替换原生提交、取消和焦点行为。 |
| [`static Geometry PickerScroll.GetHighlightClip(ListBoxItem item)`](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L64) | 读取附加属性。滚轮高亮区域的裁剪配置；通常由选择器模板维护。 |
| [`static void PickerScroll.SetHighlightClip(ListBoxItem item, Geometry value)`](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L69) | 设置附加属性。滚轮高亮区域的裁剪配置；通常由选择器模板维护。 |
| [`PickerScroll.PickerScroll()`](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L15) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly AttachedProperty<bool> PickerScroll.EnabledProperty`](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L17) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<Geometry> PickerScroll.HighlightClipProperty`](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
