# HeatmapPoint

[English](../en/TheOne.Avalonia.Controls.HeatmapPoint.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.HeatmapPoint.md) · [API index](README.md)

`TheOne.Avalonia.Controls.HeatmapPoint` · Class · `TheOne.Avalonia.Controls.ChartPoint`

[Inherited additional members: ChartPoint](TheOne.Avalonia.Controls.ChartPoint.md)

A heatmap cell. X and Y locate the cell; Value controls its color.

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L6)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Value`](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L19) | `double` | writable; OneWay | `0 / default` | Non-finite values leave a gap without changing the cell coordinates. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`HeatmapPoint.HeatmapPoint()`](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L11) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |
| [`HeatmapPoint.HeatmapPoint(double x, double y, double value, string? label = null)`](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L13) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<double> HeatmapPoint.ValueProperty`](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L8) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
