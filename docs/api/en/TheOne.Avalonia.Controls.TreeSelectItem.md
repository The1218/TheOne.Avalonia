# TreeSelectItem

[English](../en/TheOne.Avalonia.Controls.TreeSelectItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.TreeSelectItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.TreeSelectItem` · Class · `object`

Read-only row state. Item is the original application model, never a copied model.

[Source / 声明](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L8)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanRemove`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L28) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L33) | `Avalonia.Controls.Templates.IDataTemplate?` | read-only; CLR / n/a | `accessor-defined / state` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`Depth`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L21) | `int` | read-only; CLR / n/a | `accessor-defined / state` | Node depth starting at zero for roots, used to calculate indentation. |
| [`DisplayContent`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L32) | `object` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ExpandText`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L30) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ExpansionGlyph`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L29) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HasChildren`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L23) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Whether a node has children, controlling expansion affordances independently of current expansion. |
| [`Indent`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L22) | `Avalonia.Thickness` | read-only; CLR / n/a | `accessor-defined / state` | Additional DIP indentation per hierarchy level, without changing parent/child relationships. |
| [`IsChecked`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L26) | `bool?` | read-only; CLR / n/a | `accessor-defined / state` | Checked state of a report checkbox element. |
| [`IsEnabled`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L24) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Business availability; selection, removal and commands follow the control's disabled-item rules. |
| [`IsExpanded`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L25) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Current node expansion; bind to a stable model when it must survive recycling or reload. |
| [`Item`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L18) | `object` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Key`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L19) | `object` | read-only; CLR / n/a | `accessor-defined / state` | Field/parameter key used to resolve business values, distinct from display names. |
| [`RemoveText`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L31) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ShowCheckBox`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L27) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`TagContent`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L34) | `object` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`TagTemplate`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L35) | `Avalonia.Controls.Templates.IDataTemplate?` | read-only; CLR / n/a | `accessor-defined / state` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`Text`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L20) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Input/display text; notifications and validation follow the model/native editor contract. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event PropertyChangedEventHandler? TreeSelectItem.PropertyChanged`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L36) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
