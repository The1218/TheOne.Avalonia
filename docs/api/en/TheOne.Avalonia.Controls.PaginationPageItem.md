# PaginationPageItem

[English](../en/TheOne.Avalonia.Controls.PaginationPageItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.PaginationPageItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.PaginationPageItem` · Class · `object`

A bounded navigation entry. Page zero represents a non-interactive ellipsis.

[Source / 声明](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L8)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`AccessibleName`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L13) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsCurrent`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L8) | `bool` | writable; CLR / init-only | `required constructor argument` | Whether this is the current pagination page, derived from paging state. |
| [`IsEllipsis`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L11) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsPage`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L10) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Page`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L8) | `long` | writable; CLR / init-only | `required constructor argument` | One-based page number following the pagination contract. |
| [`Text`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L12) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Input/display text; notifications and validation follow the model/native editor contract. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`PaginationPageItem.PaginationPageItem(long Page, bool IsCurrent)`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L8) | A bounded navigation entry. Page zero represents a non-interactive ellipsis. |
