# GaugeChart

[English](../en/TheOne.Avalonia.Controls.GaugeChart.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.GaugeChart.md) · [API index](README.md)

`TheOne.Avalonia.Controls.GaugeChart` · Class · `TheOne.Avalonia.Controls.ChartBase`

[Inherited additional members: ChartBase](TheOne.Avalonia.Controls.ChartBase.md)

A directly drawn gauge with enum-selected layouts and property-based visual overrides.

[Source / 声明](../../../TheOne.Avalonia/Controls/GaugeChart.GradeLabels.cs#L6)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`GaugeCaption`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L298) | `string?` | writable; OneWay | `null` | Caption defaults to null; vertical top offset in radius units `[−1, 1]`, NaN follows the enum; separate from top Title |
| [`GaugeCaptionOffset`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L278) | `double` | writable; OneWay | `double.NaN` | Caption defaults to null; vertical top offset in radius units `[−1, 1]`, NaN follows the enum; separate from top Title Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -1 && value <= 1)` |
| [`GaugeGradeLabelGap`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L292) | `double` | writable; OneWay | `double.NaN` | Grade text follows a readable tangent by default; rotation NaN is automatic, `[−90, 90]` explicit degrees (0 horizontal). Gap from bands is NaN for font-based sizing or `[0, 100]` explicit pixels Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= 0 && value <= 100)` |
| [`GaugeGradeLabelRotation`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L285) | `double` | writable; OneWay | `double.NaN` | Grade text follows a readable tangent by default; rotation NaN is automatic, `[−90, 90]` explicit degrees (0 horizontal). Gap from bands is NaN for font-based sizing or `[0, 100]` explicit pixels Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -90 && value <= 90)` |
| [`GaugeNeedleBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L310) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Overrides needle, major ticks, and center value; otherwise theme / band colors |
| [`GaugeNeedleLength`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L253) | `double` | writable; OneWay | `0.72` | Length defaults to 0.72 radius, range `(0, 1]`; width NaN follows shape, explicit pixels `(0, 50]` Validation: `value => double.IsFinite(value) && value > 0 && value <= 1` |
| [`GaugeNeedleShape`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L241) | `TheOne.Avalonia.Controls.GaugeNeedleShape` | writable; OneWay | `0 / default` | Auto follows the enum; Triangle / Line / Marker overrides shape Validation: `Enum.IsDefined` Values: `Auto`, `Triangle`, `Line`, `Marker` |
| [`GaugeNeedleWidth`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L259) | `double` | writable; OneWay | `double.NaN` | Length defaults to 0.72 radius, range `(0, 1]`; width NaN follows shape, explicit pixels `(0, 50]` Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value > 0 && value <= 50)` |
| [`GaugeSecondaryBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L352) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Secondary count defaults to 7 in `[2, 12]`; color defaults to Foreground |
| [`GaugeSecondaryMaximum`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L340) | `double` | writable; OneWay | `60` | Barometer secondary range, default 0–60, independent of main value / needle. Invalid ranges hide the secondary scale Validation: `double.IsFinite` |
| [`GaugeSecondaryMinimum`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L334) | `double` | writable; OneWay | `0 / default` | Barometer secondary range, default 0–60, independent of main value / needle. Invalid ranges hide the secondary scale Validation: `double.IsFinite` |
| [`GaugeSecondaryTickCount`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L346) | `int` | writable; OneWay | `7` | Secondary count defaults to 7 in `[2, 12]`; color defaults to Foreground Validation: `value => value >= 2 && value <= 12` |
| [`GaugeSegments`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L328) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.GaugeSegment>?` | writable; OneWay | `null` | Independent empty collection per instance, supporting XAML children or binding replacement. Ordered bands with cumulative Fraction `(0, 1]` of Minimum–Maximum; Brush sets color, Label is used by Grade |
| [`GaugeStartAngle`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L205) | `double` | writable; OneWay | `double.NaN` | NaN follows the enum; start −360°–360°, clockwise sweep `(0, 360]`. 0° points right, 90° down. Full circles supported Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -360 && value <= 360)` |
| [`GaugeStyle`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L199) | `TheOne.Avalonia.Controls.GaugeStyle` | writable; OneWay | `0 / default` | Structure ; Default start / clockwise sweep ; Theme needle / ticks / major count Validation: `Enum.IsDefined` Values: `Semicircle`, `Basic`, `Speed`, `Progress`, `StageSpeed`, `Grade`, `Temperature`, `Barometer` |
| [`GaugeSweepAngle`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L211) | `double` | writable; OneWay | `double.NaN` | NaN follows the enum; start −360°–360°, clockwise sweep `(0, 360]`. 0° points right, 90° down. Full circles supported Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value > 0 && value <= 360)` |
| [`GaugeTickBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L316) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Overrides needle, major ticks, and center value; otherwise theme / band colors |
| [`GaugeTickPlacement`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L247) | `TheOne.Avalonia.Controls.GaugeTickPlacement` | writable; OneWay | `0 / default` | Auto follows the enum; Inside / Outside overrides major placement; barometer secondary ticks remain inside Validation: `Enum.IsDefined` Values: `Auto`, `Inside`, `Outside` |
| [`GaugeTrackBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L304) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Background track color, default GridBrush |
| [`GaugeValueBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L322) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Overrides needle, major ticks, and center value; otherwise theme / band colors |
| [`GaugeValueFontSize`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L265) | `double` | writable; OneWay | `double.NaN` | NaN follows the enum; font `(0, 200]`, vertical radius-relative offset `[−1, 1]`, positive downward. Fonts shrink to fit Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value > 0 && value <= 200)` |
| [`GaugeValueOffset`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L271) | `double` | writable; OneWay | `double.NaN` | NaN follows the enum; font `(0, 200]`, vertical radius-relative offset `[−1, 1]`, positive downward. Fonts shrink to fit Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -1 && value <= 1)` |
| [`Kind`](../../../TheOne.Avalonia/Controls/GaugeChart.cs#L9) | `TheOne.Avalonia.Controls.ChartKind` | read-only; CLR / n/a | `accessor-defined / state` | Kind enum for the owning field/property/element; see its concrete enum for choices. Values: `Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |
| [`ShowGaugeAnchor`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L223) | `bool?` | writable; OneWay | `null` | null follows the enum; controls the center anchor only when the needle is shown |
| [`ShowGaugeProgress`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L217) | `bool?` | writable; OneWay | `null` | null follows the enum; True / False forces progress visibility |
| [`ShowGaugeTickLabels`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L235) | `bool` | writable; OneWay | `true` | Default True; controls value / tick text without changing data |
| [`ShowGaugeValue`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L229) | `bool` | writable; OneWay | `true` | Default True; controls value / tick text without changing data |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`GaugeChart.GaugeChart()`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L194) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<GaugeStyle> GaugeChart.GaugeStyleProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L70) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GaugeChart.GaugeStartAngleProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L73) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GaugeChart.GaugeSweepAngleProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L77) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool?> GaugeChart.ShowGaugeProgressProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L81) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool?> GaugeChart.ShowGaugeAnchorProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L84) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> GaugeChart.ShowGaugeValueProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L87) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> GaugeChart.ShowGaugeTickLabelsProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L90) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<GaugeNeedleShape> GaugeChart.GaugeNeedleShapeProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L93) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<GaugeTickPlacement> GaugeChart.GaugeTickPlacementProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L96) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GaugeChart.GaugeNeedleLengthProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L99) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GaugeChart.GaugeNeedleWidthProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L103) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GaugeChart.GaugeValueFontSizeProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L107) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GaugeChart.GaugeValueOffsetProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L111) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GaugeChart.GaugeGradeLabelRotationProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L115) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GaugeChart.GaugeGradeLabelGapProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L119) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GaugeChart.GaugeCaptionOffsetProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L123) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string?> GaugeChart.GaugeCaptionProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L127) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GaugeChart.GaugeTrackBrushProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L130) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GaugeChart.GaugeNeedleBrushProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L133) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GaugeChart.GaugeTickBrushProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L136) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GaugeChart.GaugeValueBrushProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L139) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IList<GaugeSegment>?> GaugeChart.GaugeSegmentsProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L142) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GaugeChart.GaugeSecondaryMinimumProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L145) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GaugeChart.GaugeSecondaryMaximumProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L148) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<int> GaugeChart.GaugeSecondaryTickCountProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L151) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GaugeChart.GaugeSecondaryBrushProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L155) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `animation-fade` | [`^.animation-fade`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L27) | ^.animation-fade => AnimationStyle=Fade |
| `animation-none` | [`^.animation-none`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L33) | ^.animation-none => AnimationStyle=None |
| `animation-reveal` | [`^.animation-reveal`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L30) | ^.animation-reveal => AnimationStyle=Reveal |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.ChartKind`](TheOne.Avalonia.Controls.ChartKind.md)
- [`TheOne.Avalonia.Controls.GaugeNeedleShape`](TheOne.Avalonia.Controls.GaugeNeedleShape.md)
- [`TheOne.Avalonia.Controls.GaugeSegment`](TheOne.Avalonia.Controls.GaugeSegment.md)
- [`TheOne.Avalonia.Controls.GaugeStyle`](TheOne.Avalonia.Controls.GaugeStyle.md)
- [`TheOne.Avalonia.Controls.GaugeTickPlacement`](TheOne.Avalonia.Controls.GaugeTickPlacement.md)
