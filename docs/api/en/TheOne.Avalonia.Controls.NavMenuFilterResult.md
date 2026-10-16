# NavMenuFilterResult

[English](../en/TheOne.Avalonia.Controls.NavMenuFilterResult.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.NavMenuFilterResult.md) · [API index](README.md)

`TheOne.Avalonia.Controls.NavMenuFilterResult` · Class · `object`

A lightweight suggestion containing an original menu model and its ancestor path.

[Source / 声明](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L75)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Item`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L78) | `object` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Label`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L79) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Display label, not a substitute for model identity, field paths or serialized identifiers. |
| [`Path`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L80) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Stable output-field path key matching FieldPath in data references. |
| [`SearchPath`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L81) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`override string NavMenuFilterResult.ToString()`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L83) | Returns result display text, not model identity or a serialization format. |
