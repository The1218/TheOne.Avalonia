# CarouselOptions

[English](../en/TheOne.Avalonia.Controls.CarouselOptions.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.CarouselOptions.md) · [API index](README.md)

`TheOne.Avalonia.Controls.CarouselOptions` · Class · `Avalonia.AvaloniaObject`

Enables mouse input on the native carousel swipe recognizer, independently of touch and pen.

[Source / 声明](../../../TheOne.Avalonia/Controls/CarouselOptions.cs#L9)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`IsMouseSwipeEnabled`](../../../TheOne.Avalonia/Controls/CarouselOptions.cs#L11) | `bool` | 可写; OneWay; 附加属性 | `false` | 允许鼠标拖动轮播翻页；触摸和原生导航行为仍遵循 Carousel 契约。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static bool CarouselOptions.GetIsMouseSwipeEnabled(Carousel carousel)`](../../../TheOne.Avalonia/Controls/CarouselOptions.cs#L27) | 读取附加属性。允许鼠标拖动轮播翻页；触摸和原生导航行为仍遵循 Carousel 契约。 |
| [`static void CarouselOptions.SetIsMouseSwipeEnabled(Carousel carousel, bool value)`](../../../TheOne.Avalonia/Controls/CarouselOptions.cs#L28) | 设置附加属性。允许鼠标拖动轮播翻页；触摸和原生导航行为仍遵循 Carousel 契约。 |
| [`CarouselOptions.CarouselOptions()`](../../../TheOne.Avalonia/Controls/CarouselOptions.cs#L9) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly AttachedProperty<bool> CarouselOptions.IsMouseSwipeEnabledProperty`](../../../TheOne.Avalonia/Controls/CarouselOptions.cs#L11) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
