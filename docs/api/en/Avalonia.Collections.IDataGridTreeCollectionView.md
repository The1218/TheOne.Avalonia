# IDataGridTreeCollectionView

[English](../en/Avalonia.Collections.IDataGridTreeCollectionView.md) · [简体中文](../zh-cn/Avalonia.Collections.IDataGridTreeCollectionView.md) · [API index](README.md)

`Avalonia.Collections.IDataGridTreeCollectionView` · Interface

Non-generic tree presentation contract; row objects remain the original models.

[Source / 声明](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L29)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`bool IDataGridTreeCollectionView.TryGetNodeState(object item, out DataGridTreeNodeState state)`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L33) | Attempts to read hierarchy/parent/expansion state; false means the node was not found. |
| [`bool IDataGridTreeCollectionView.TrySetExpanded(object item, bool expanded)`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L34) | Attempts to change original-node expansion; the result indicates success under tree-state restrictions. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event EventHandler<DataGridTreeChangingEventArgs>? IDataGridTreeCollectionView.TreeChanging`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L31) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
| [`event EventHandler? IDataGridTreeCollectionView.TreeChanged`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L32) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

## Related APIs and built-in models

- [`Avalonia.Collections.DataGridTreeChangingEventArgs`](Avalonia.Collections.DataGridTreeChangingEventArgs.md)
- [`Avalonia.Collections.DataGridTreeNodeState`](Avalonia.Collections.DataGridTreeNodeState.md)
