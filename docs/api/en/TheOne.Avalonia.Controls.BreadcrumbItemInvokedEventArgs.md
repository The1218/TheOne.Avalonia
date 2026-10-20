# BreadcrumbItemInvokedEventArgs

[English](../en/TheOne.Avalonia.Controls.BreadcrumbItemInvokedEventArgs.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.BreadcrumbItemInvokedEventArgs.md) · [API index](README.md)

`TheOne.Avalonia.Controls.BreadcrumbItemInvokedEventArgs` · Class · `Avalonia.Interactivity.RoutedEventArgs`

The original navigation item and its current path index; the caller decides how to navigate.

[Source / 声明](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L249)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Index`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L252) | `int` | read-only; CLR / n/a | `index` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Item`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L251) | `object?` | read-only; CLR / n/a | `item` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`BreadcrumbItemInvokedEventArgs.BreadcrumbItemInvokedEventArgs(RoutedEvent routedEvent, object source, object? item, int index)`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L249) | The original navigation item and its current path index; the caller decides how to navigate. |
