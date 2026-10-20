# ChartSelection

[English](../en/TheOne.Avalonia.Controls.ChartSelection.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ChartSelection.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ChartSelection` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartData.cs#L148)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Point`](../../../TheOne.Avalonia/Controls/ChartData.cs#L148) | `TheOne.Avalonia.Controls.ChartPoint` | writable; CLR / init-only | `required constructor argument` | Original hit-tested chart point for selection events/business details. |
| [`PointIndex`](../../../TheOne.Avalonia/Controls/ChartData.cs#L148) | `int` | writable; CLR / init-only | `required constructor argument` | Zero-based index of the selected point within its series. |
| [`Series`](../../../TheOne.Avalonia/Controls/ChartData.cs#L148) | `TheOne.Avalonia.Controls.ChartSeries` | writable; CLR / init-only | `required constructor argument` | Original hit-tested chart-series model. |
| [`SeriesIndex`](../../../TheOne.Avalonia/Controls/ChartData.cs#L148) | `int` | writable; CLR / init-only | `required constructor argument` | Zero-based index of the selected series in the source collection. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ChartSelection.ChartSelection(int SeriesIndex, int PointIndex, ChartSeries Series, ChartPoint Point)`](../../../TheOne.Avalonia/Controls/ChartData.cs#L148) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.ChartPoint`](TheOne.Avalonia.Controls.ChartPoint.md)
- [`TheOne.Avalonia.Controls.ChartSeries`](TheOne.Avalonia.Controls.ChartSeries.md)
