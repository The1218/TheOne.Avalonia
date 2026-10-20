# PaginationPageItem

[English](../en/TheOne.Avalonia.Controls.PaginationPageItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.PaginationPageItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.PaginationPageItem` · Class · `object`

A bounded navigation entry. Page zero represents a non-interactive ellipsis.

[Source / 声明](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L13)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`AccessibleName`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L18) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsCurrent`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L13) | `bool` | 可写; CLR / init-only | `required constructor argument` | 是否为分页器当前页；由分页状态派生。 |
| [`IsEllipsis`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L16) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsPage`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L15) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Page`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L13) | `long` | 可写; CLR / init-only | `required constructor argument` | 页码值；按分页器文档使用从 1 开始的页号。 |
| [`Text`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L17) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 文本输入／显示值；变更通知和校验按所在模型或原生输入契约处理。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`PaginationPageItem.PaginationPageItem(long Page, bool IsCurrent)`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L13) | A bounded navigation entry. Page zero represents a non-interactive ellipsis. |
