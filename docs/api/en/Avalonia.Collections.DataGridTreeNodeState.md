# DataGridTreeNodeState

[English](../en/Avalonia.Collections.DataGridTreeNodeState.md) · [简体中文](../zh-cn/Avalonia.Collections.DataGridTreeNodeState.md) · [API index](README.md)

`Avalonia.Collections.DataGridTreeNodeState` · Struct · `System.ValueType`

Presentation state for an original model in a local tree.

[Source / 声明](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L15)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Depth`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L15) | `int` | writable; CLR / init-only | `required constructor argument` | Node depth starting at zero for roots, used to calculate indentation. |
| [`HasChildren`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L15) | `bool` | writable; CLR / init-only | `required constructor argument` | Whether a node has children, controlling expansion affordances independently of current expansion. |
| [`IsExpanded`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L15) | `bool` | writable; CLR / init-only | `required constructor argument` | Current node expansion; bind to a stable model when it must survive recycling or reload. |
| [`IsFilterMatch`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L18) | `bool` | writable; CLR / init-only | `false` | Whether the node directly matches the tree filter, distinct from retaining ancestors. |
| [`IsForcedExpanded`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L17) | `bool` | writable; CLR / init-only | `false` | Whether filtering forces expansion to reveal matching descendants, distinct from saved user expansion. |
| [`Parent`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L15) | `object?` | writable; CLR / init-only | `required constructor argument` | Direct parent model of a tree node; null for roots, preserving original identity. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`DataGridTreeNodeState.DataGridTreeNodeState(int Depth, object? Parent, bool HasChildren, bool IsExpanded)`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L15) | Presentation state for an original model in a local tree. |
| [`DataGridTreeNodeState.DataGridTreeNodeState()`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L15) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |
