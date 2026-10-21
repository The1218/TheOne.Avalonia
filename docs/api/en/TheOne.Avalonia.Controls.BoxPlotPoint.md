# BoxPlotPoint

[English](../en/TheOne.Avalonia.Controls.BoxPlotPoint.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.BoxPlotPoint.md) · [API index](README.md)

`TheOne.Avalonia.Controls.BoxPlotPoint` · Class · `TheOne.Avalonia.Controls.ChartPoint`

[Inherited additional members: ChartPoint](TheOne.Avalonia.Controls.ChartPoint.md)

Precomputed five-number summary. Median and inherited Y refer to the same observable value.

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L7)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`LowerQuartile`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L26) | `double` | writable; OneWay | `0 / default` | LowerQuartile configuration on BoxPlotPoint; type and initial value are listed here. LowerQuartileProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`Maximum`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L29) | `double` | writable; OneWay | `0 / default` | Maximum configuration on BoxPlotPoint; type and initial value are listed here. MaximumProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`Median`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L27) | `double` | writable; OneWay | `computed / state` | Median configuration on BoxPlotPoint; type and initial value are listed here. MedianProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`Minimum`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L25) | `double` | writable; OneWay | `0 / default` | Minimum configuration on BoxPlotPoint; type and initial value are listed here. MinimumProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`Outliers`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L30) | `System.Collections.Generic.IList<double>?` | writable; OneWay | `null` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |
| [`UpperQuartile`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L28) | `double` | writable; OneWay | `0 / default` | UpperQuartile configuration on BoxPlotPoint; type and initial value are listed here. UpperQuartileProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`BoxPlotPoint.BoxPlotPoint()`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L17) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |
| [`BoxPlotPoint.BoxPlotPoint(double x, double minimum, double lowerQuartile, double median, double upperQuartile, double maximum, string? label = null)`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L18) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<double> BoxPlotPoint.MinimumProperty`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L9) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> BoxPlotPoint.LowerQuartileProperty`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L10) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<BoxPlotPoint, double> BoxPlotPoint.MedianProperty`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L11) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> BoxPlotPoint.UpperQuartileProperty`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L13) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> BoxPlotPoint.MaximumProperty`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L14) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IList<double>?> BoxPlotPoint.OutliersProperty`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L15) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
