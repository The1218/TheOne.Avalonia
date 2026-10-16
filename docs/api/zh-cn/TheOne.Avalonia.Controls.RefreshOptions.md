# RefreshOptions

[English](../en/TheOne.Avalonia.Controls.RefreshOptions.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.RefreshOptions.md) · [API index](README.md)

`TheOne.Avalonia.Controls.RefreshOptions` · Class · `Avalonia.AvaloniaObject`

Density for the native refresh indicator, including the container-created visualizer.

[Source / 声明](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L10)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`IndicatorSize`](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L12) | `double` | 可写; OneWay; 附加属性; 向后代继承 | `22` | 刷新指示器的 DIP 尺寸；不改变触发刷新所需的拉动距离。 |
| [`UseThemeVisualizerSizing`](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L14) | `bool` | 可写; OneWay; 附加属性 | `false` | 让刷新指示器跟随共享主题密度；显式定制时可关闭。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static double RefreshOptions.GetIndicatorSize(Control control)`](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L23) | 读取附加属性。刷新指示器的 DIP 尺寸；不改变触发刷新所需的拉动距离。 |
| [`static void RefreshOptions.SetIndicatorSize(Control control, double value)`](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L24) | 设置附加属性。刷新指示器的 DIP 尺寸；不改变触发刷新所需的拉动距离。 |
| [`static bool RefreshOptions.GetUseThemeVisualizerSizing(RefreshContainer container)`](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L25) | 读取附加属性。让刷新指示器跟随共享主题密度；显式定制时可关闭。 |
| [`static void RefreshOptions.SetUseThemeVisualizerSizing(RefreshContainer container, bool value)`](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L26) | 设置附加属性。让刷新指示器跟随共享主题密度；显式定制时可关闭。 |
| [`RefreshOptions.RefreshOptions()`](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L10) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly AttachedProperty<double> RefreshOptions.IndicatorSizeProperty`](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L12) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<bool> RefreshOptions.UseThemeVisualizerSizingProperty`](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L14) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
