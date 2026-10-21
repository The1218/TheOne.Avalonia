# DataGridRowNumbers

[English](../en/TheOne.Avalonia.Controls.DataGridRowNumbers.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DataGridRowNumbers.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DataGridRowNumbers` · Class · `object`

原生 DataGrid 行头序号，支持分页偏移、虚拟化复用和关闭后恢复原设置。

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridRowNumbers.cs#L9)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`HeaderWidth`](../../../TheOne.Avalonia/Controls/DataGridRowNumbers.cs#L23) | `double` | writable; OneWay; attached | `typeof(DataGridRowNumbers)` | 行头宽度，不接受零、无穷或 NaN。 Validation: `value => double.IsFinite(value) && value > 0` |
| [`IsVisible`](../../../TheOne.Avalonia/Controls/DataGridRowNumbers.cs#L12) | `bool` | writable; OneWay; attached | `typeof(DataGridRowNumbers)` | 显式启用行号，不改变未配置此能力的表格。 |
| [`PageIndex`](../../../TheOne.Avalonia/Controls/DataGridRowNumbers.cs#L15) | `long` | writable; OneWay; attached | `typeof(DataGridRowNumbers)` | 从 1 开始的页码，与 Pagination 的 long 页码一致。 Validation: `value => value >= 1` |
| [`PageSize`](../../../TheOne.Avalonia/Controls/DataGridRowNumbers.cs#L19) | `int` | writable; OneWay; attached | `typeof(DataGridRowNumbers)` | 每页记录数，必须大于零。 Validation: `value => value > 0` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static bool DataGridRowNumbers.GetIsVisible(DataGrid grid)`](../../../TheOne.Avalonia/Controls/DataGridRowNumbers.cs#L38) | 读取行号启用状态。 |
| [`static void DataGridRowNumbers.SetIsVisible(DataGrid grid, bool value)`](../../../TheOne.Avalonia/Controls/DataGridRowNumbers.cs#L40) | 切换行号；关闭后解除事件并恢复原行头设置。 |
| [`static long DataGridRowNumbers.GetPageIndex(DataGrid grid)`](../../../TheOne.Avalonia/Controls/DataGridRowNumbers.cs#L42) | 读取页码。 |
| [`static void DataGridRowNumbers.SetPageIndex(DataGrid grid, long value)`](../../../TheOne.Avalonia/Controls/DataGridRowNumbers.cs#L44) | 设置页码。 |
| [`static int DataGridRowNumbers.GetPageSize(DataGrid grid)`](../../../TheOne.Avalonia/Controls/DataGridRowNumbers.cs#L46) | 读取页大小。 |
| [`static void DataGridRowNumbers.SetPageSize(DataGrid grid, int value)`](../../../TheOne.Avalonia/Controls/DataGridRowNumbers.cs#L48) | 设置页大小。 |
| [`static double DataGridRowNumbers.GetHeaderWidth(DataGrid grid)`](../../../TheOne.Avalonia/Controls/DataGridRowNumbers.cs#L50) | 读取行头宽度。 |
| [`static void DataGridRowNumbers.SetHeaderWidth(DataGrid grid, double value)`](../../../TheOne.Avalonia/Controls/DataGridRowNumbers.cs#L52) | 设置行头宽度。 |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly AttachedProperty<bool> DataGridRowNumbers.IsVisibleProperty`](../../../TheOne.Avalonia/Controls/DataGridRowNumbers.cs#L12) | 显式启用行号，不改变未配置此能力的表格。 |
| [`static readonly AttachedProperty<long> DataGridRowNumbers.PageIndexProperty`](../../../TheOne.Avalonia/Controls/DataGridRowNumbers.cs#L15) | 从 1 开始的页码，与 Pagination 的 long 页码一致。 |
| [`static readonly AttachedProperty<int> DataGridRowNumbers.PageSizeProperty`](../../../TheOne.Avalonia/Controls/DataGridRowNumbers.cs#L19) | 每页记录数，必须大于零。 |
| [`static readonly AttachedProperty<double> DataGridRowNumbers.HeaderWidthProperty`](../../../TheOne.Avalonia/Controls/DataGridRowNumbers.cs#L23) | 行头宽度，不接受零、无穷或 NaN。 |
