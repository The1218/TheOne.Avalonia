# CandlestickPoint

[English](../en/TheOne.Avalonia.Controls.CandlestickPoint.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.CandlestickPoint.md) · [API index](README.md)

`TheOne.Avalonia.Controls.CandlestickPoint` · Class · `TheOne.Avalonia.Controls.ChartPoint`

[Inherited additional members: ChartPoint](TheOne.Avalonia.Controls.ChartPoint.md)

OHLC observation at a numeric X coordinate. Close and inherited Y are the same value.

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L40)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Close`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L55) | `double` | writable; OneWay | `computed / state` | Close configuration on CandlestickPoint; type and initial value are listed here. CloseProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`High`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L57) | `double` | writable; OneWay | `0 / default` | High configuration on CandlestickPoint; type and initial value are listed here. HighProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`Low`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L56) | `double` | writable; OneWay | `0 / default` | Low configuration on CandlestickPoint; type and initial value are listed here. LowProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`Open`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L54) | `double` | writable; OneWay | `0 / default` | Open configuration on CandlestickPoint; type and initial value are listed here. OpenProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`CandlestickPoint.CandlestickPoint()`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L48) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |
| [`CandlestickPoint.CandlestickPoint(double x, double open, double close, double low, double high, string? label = null)`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L49) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<double> CandlestickPoint.OpenProperty`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L42) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<CandlestickPoint, double> CandlestickPoint.CloseProperty`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L43) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> CandlestickPoint.LowProperty`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L45) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> CandlestickPoint.HighProperty`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L46) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
