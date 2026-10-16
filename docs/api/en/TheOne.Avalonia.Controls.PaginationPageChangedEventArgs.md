# PaginationPageChangedEventArgs

[English](../en/TheOne.Avalonia.Controls.PaginationPageChangedEventArgs.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.PaginationPageChangedEventArgs.md) · [API index](README.md)

`TheOne.Avalonia.Controls.PaginationPageChangedEventArgs` · Class · `Avalonia.Interactivity.RoutedEventArgs`

One notification after page and size have both been normalized.

[Source / 声明](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L19)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CurrentPage`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L22) | `long` | read-only; CLR / n/a | `page` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`OldPage`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L21) | `long` | read-only; CLR / n/a | `oldPage` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`OldPageSize`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L23) | `int` | read-only; CLR / n/a | `oldPageSize` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`PageSize`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L24) | `int` | read-only; CLR / n/a | `pageSize` | Rows per page; hierarchical views do not support paging and enforce their own restrictions. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`PaginationPageChangedEventArgs.PaginationPageChangedEventArgs(RoutedEvent routedEvent, long oldPage, long page, int oldPageSize, int pageSize)`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L19) | One notification after page and size have both been normalized. |
