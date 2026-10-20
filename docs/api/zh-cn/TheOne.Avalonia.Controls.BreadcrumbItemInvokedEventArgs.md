# BreadcrumbItemInvokedEventArgs

[English](../en/TheOne.Avalonia.Controls.BreadcrumbItemInvokedEventArgs.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.BreadcrumbItemInvokedEventArgs.md) · [API index](README.md)

`TheOne.Avalonia.Controls.BreadcrumbItemInvokedEventArgs` · Class · `Avalonia.Interactivity.RoutedEventArgs`

The original navigation item and its current path index; the caller decides how to navigate.

[Source / 声明](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L249)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Index`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L252) | `int` | 只读; CLR / n/a | `index` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Item`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L251) | `object?` | 只读; CLR / n/a | `item` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`BreadcrumbItemInvokedEventArgs.BreadcrumbItemInvokedEventArgs(RoutedEvent routedEvent, object source, object? item, int index)`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L249) | The original navigation item and its current path index; the caller decides how to navigate. |
