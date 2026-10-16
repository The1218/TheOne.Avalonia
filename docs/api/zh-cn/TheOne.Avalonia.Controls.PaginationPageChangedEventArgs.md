# PaginationPageChangedEventArgs

[English](../en/TheOne.Avalonia.Controls.PaginationPageChangedEventArgs.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.PaginationPageChangedEventArgs.md) · [API index](README.md)

`TheOne.Avalonia.Controls.PaginationPageChangedEventArgs` · Class · `Avalonia.Interactivity.RoutedEventArgs`

One notification after page and size have both been normalized.

[Source / 声明](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L19)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CurrentPage`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L22) | `long` | 只读; CLR / n/a | `page` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`OldPage`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L21) | `long` | 只读; CLR / n/a | `oldPage` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`OldPageSize`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L23) | `int` | 只读; CLR / n/a | `oldPageSize` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`PageSize`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L24) | `int` | 只读; CLR / n/a | `pageSize` | 每页行数；树形视图不支持分页，此成员遵循该视图的限制。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`PaginationPageChangedEventArgs.PaginationPageChangedEventArgs(RoutedEvent routedEvent, long oldPage, long page, int oldPageSize, int pageSize)`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L19) | One notification after page and size have both been normalized. |
