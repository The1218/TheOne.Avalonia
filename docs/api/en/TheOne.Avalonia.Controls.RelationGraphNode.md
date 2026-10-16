# RelationGraphNode

[English](../en/TheOne.Avalonia.Controls.RelationGraphNode.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.RelationGraphNode.md) · [API index](README.md)

`TheOne.Avalonia.Controls.RelationGraphNode` · Class · `object`

The default model for a single-root relation tree.

[Source / 声明](../../../TheOne.Avalonia/Controls/RelationGraphNode.cs#L8)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Children`](../../../TheOne.Avalonia/Controls/RelationGraphNode.cs#L18) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.RelationGraphNode>?` | writable; CLR / n/a | `new ObservableCollection<RelationGraphNode>()` | Child model collection; retain identity/parent relationships and avoid cycles/shared mutable nodes. |
| [`Content`](../../../TheOne.Avalonia/Controls/RelationGraphNode.cs#L17) | `object?` | writable; CLR / n/a | `null` | Host content/presentation object; follow the content-template scope and do not persist visual objects as business data. |
| [`Icon`](../../../TheOne.Avalonia/Controls/RelationGraphNode.cs#L15) | `object?` | writable; CLR / n/a | `null` | Icon content/vector geometry as defined by the signature; the host owns external bitmap lifetimes. |
| [`Title`](../../../TheOne.Avalonia/Controls/RelationGraphNode.cs#L16) | `object?` | writable; CLR / n/a | `null` | Display title, distinct from business identity or a localization key. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`RelationGraphNode.RelationGraphNode()`](../../../TheOne.Avalonia/Controls/RelationGraphNode.cs#L8) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event PropertyChangedEventHandler? RelationGraphNode.PropertyChanged`](../../../TheOne.Avalonia/Controls/RelationGraphNode.cs#L19) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
