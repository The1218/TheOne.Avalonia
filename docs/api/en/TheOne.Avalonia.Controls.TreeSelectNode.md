# TreeSelectNode

[English](../en/TheOne.Avalonia.Controls.TreeSelectNode.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.TreeSelectNode.md) · [API index](README.md)

`TheOne.Avalonia.Controls.TreeSelectNode` · Class · `object`

Optional default model. Keys must be non-null, unique and stable across reloads.

[Source / 声明](../../../TheOne.Avalonia/Controls/TreeSelectNode.cs#L8)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Children`](../../../TheOne.Avalonia/Controls/TreeSelectNode.cs#L17) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Controls.TreeSelectNode>?` | writable; CLR / n/a | `[]` | Child model collection; retain identity/parent relationships and avoid cycles/shared mutable nodes. |
| [`IsEnabled`](../../../TheOne.Avalonia/Controls/TreeSelectNode.cs#L16) | `bool` | writable; CLR / n/a | `true` | Business availability; selection, removal and commands follow the control's disabled-item rules. |
| [`Key`](../../../TheOne.Avalonia/Controls/TreeSelectNode.cs#L14) | `object` | writable; CLR / n/a | `Guid.NewGuid().ToString("N")` | Field/parameter key used to resolve business values, distinct from display names. |
| [`Text`](../../../TheOne.Avalonia/Controls/TreeSelectNode.cs#L15) | `string` | writable; CLR / n/a | `""` | Input/display text; notifications and validation follow the model/native editor contract. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`TreeSelectNode.TreeSelectNode()`](../../../TheOne.Avalonia/Controls/TreeSelectNode.cs#L8) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event PropertyChangedEventHandler? TreeSelectNode.PropertyChanged`](../../../TheOne.Avalonia/Controls/TreeSelectNode.cs#L18) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
