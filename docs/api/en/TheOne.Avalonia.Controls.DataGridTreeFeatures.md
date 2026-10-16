# DataGridTreeFeatures

[English](../en/TheOne.Avalonia.Controls.DataGridTreeFeatures.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DataGridTreeFeatures.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DataGridTreeFeatures` · Class · `Avalonia.AvaloniaObject`

Coordinates the prototype tree projection with native editing and selection.

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridTreeFeatures.cs#L12)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IsEnabled`](../../../TheOne.Avalonia/Controls/DataGridTreeFeatures.cs#L14) | `bool` | writable; OneWay; attached | `false` | Business availability; selection, removal and commands follow the control's disabled-item rules. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static bool DataGridTreeFeatures.GetIsEnabled(DataGrid grid)`](../../../TheOne.Avalonia/Controls/DataGridTreeFeatures.cs#L20) | Reads the attached property. Business availability; selection, removal and commands follow the control's disabled-item rules. |
| [`static void DataGridTreeFeatures.SetIsEnabled(DataGrid grid, bool value)`](../../../TheOne.Avalonia/Controls/DataGridTreeFeatures.cs#L21) | Sets the attached property. Business availability; selection, removal and commands follow the control's disabled-item rules. |
| [`DataGridTreeFeatures.DataGridTreeFeatures()`](../../../TheOne.Avalonia/Controls/DataGridTreeFeatures.cs#L12) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly AttachedProperty<bool> DataGridTreeFeatures.IsEnabledProperty`](../../../TheOne.Avalonia/Controls/DataGridTreeFeatures.cs#L14) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Related APIs and built-in models

- [`Avalonia.Collections.DataGridTreeCollectionView<T>`](Avalonia.Collections.DataGridTreeCollectionView_T_.md)
- [`Avalonia.Collections.DataGridTreeNodeState`](Avalonia.Collections.DataGridTreeNodeState.md)
- [`Avalonia.Collections.IDataGridTreeQueryView`](Avalonia.Collections.IDataGridTreeQueryView.md)
