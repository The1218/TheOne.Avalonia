# MultiSelectBoxItem

[English](../en/TheOne.Avalonia.Controls.MultiSelectBoxItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.MultiSelectBoxItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.MultiSelectBoxItem` · Class · `object`

Read-only presentation state used by the default result and tag templates.

[Source / 声明](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L45)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanRemove`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L59) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L62) | `Avalonia.Controls.Templates.IDataTemplate?` | read-only; CLR / n/a | `accessor-defined / state` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`DisplayContent`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L61) | `object` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsEnabled`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L57) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Business availability; selection, removal and commands follow the control's disabled-item rules. |
| [`IsSelected`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L58) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Current-item selection state, synchronized with the selected model and not shared with unrelated items. |
| [`Item`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L55) | `object` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`RemoveLabel`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L60) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`TagContent`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L63) | `object` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`TagTemplate`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L64) | `Avalonia.Controls.Templates.IDataTemplate?` | read-only; CLR / n/a | `accessor-defined / state` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`Text`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L56) | `string` | read-only; CLR / n/a | `""` | Input/display text; notifications and validation follow the model/native editor contract. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event PropertyChangedEventHandler? MultiSelectBoxItem.PropertyChanged`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L65) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
