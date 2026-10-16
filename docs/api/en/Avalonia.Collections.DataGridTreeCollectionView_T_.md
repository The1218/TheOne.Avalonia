# DataGridTreeCollectionView

[English](../en/Avalonia.Collections.DataGridTreeCollectionView_T_.md) · [简体中文](../zh-cn/Avalonia.Collections.DataGridTreeCollectionView_T_.md) · [API index](README.md)

`Avalonia.Collections.DataGridTreeCollectionView<T>` · Class · `Avalonia.Collections.DataGridCollectionView`

Local tree projection with sibling sorting, ancestor-preserving filtering, native editing and currency. Grouping, paging and view-based structural edits are unsupported.

[Source / 声明](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.Query.cs#L10)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanAddNew`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L80) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`CanChangePage`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L86) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`CanFilter`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L83) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`CanGroup`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L85) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`CanRemove`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L81) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`CanSort`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L82) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Filter`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.Query.cs#L55) | `System.Func<object, bool>` | writable; CLR / n/a | `accessor-defined / state` | Direct match predicate. Ancestors of matches remain visible without matching this predicate. |
| [`LoadedItems`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L96) | `System.Collections.IEnumerable` | read-only; CLR / n/a | `accessor-defined / state` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |
| [`PageSize`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L87) | `int` | writable; CLR / n/a | `accessor-defined / state` | Rows per page; hierarchical views do not support paging and enforce their own restrictions. |
| [`QueryError`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.Query.cs#L25) | `string?` | read-only; CLR / n/a | `accessor-defined / state` | Failure of a live query or sort change. The last valid projection is retained. |
| [`SortDescriptions`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.Query.cs#L27) | `Avalonia.Collections.DataGridSortDescriptionCollection` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`override IDisposable DataGridTreeCollectionView<T>.DeferRefresh()`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.Query.cs#L73) | Defers collection refresh to batch changes; dispose the returned token at the end of a using scope. |
| [`bool DataGridTreeCollectionView<T>.IsFilterMatch(object item)`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L97) | Checks whether an original node directly matches the current filter. |
| [`bool DataGridTreeCollectionView<T>.IsLeaf(object item)`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L98) | Checks whether an original node is a leaf with no children. |
| [`bool DataGridTreeCollectionView<T>.TryGetNodeState(object item, out DataGridTreeNodeState state)`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L100) | Attempts to read hierarchy/parent/expansion state; false means the node was not found. |
| [`bool DataGridTreeCollectionView<T>.TrySetExpanded(object item, bool expanded)`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L112) | Attempts to change original-node expansion; the result indicates success under tree-state restrictions. |
| [`bool DataGridTreeCollectionView<T>.RefreshTree()`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L135) | Refreshes plain collections or reconnects replaced child collections. Returns false when an edit or a cancellable presentation change prevents refresh. |
| [`void DataGridTreeCollectionView<T>.Dispose()`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L252) | Detaches subscriptions and releases owned resources; do not reuse it as an active view afterwards. |
| [`DataGridTreeCollectionView<T>.DataGridTreeCollectionView(IEnumerable<T> roots, Func<T, IEnumerable<T>?> childrenSelector)`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L61) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event EventHandler<DataGridTreeChangingEventArgs>? DataGridTreeCollectionView<T>.TreeChanging`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L93) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
| [`event EventHandler? DataGridTreeCollectionView<T>.TreeChanged`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L94) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
| [`event EventHandler? DataGridTreeCollectionView<T>.LoadedItemsChanged`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L95) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

## Related APIs and built-in models

- [`Avalonia.Collections.DataGridTreeChangingEventArgs`](Avalonia.Collections.DataGridTreeChangingEventArgs.md)
- [`Avalonia.Collections.DataGridTreeNodeState`](Avalonia.Collections.DataGridTreeNodeState.md)
