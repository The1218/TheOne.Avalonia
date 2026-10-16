# GaugeChart

[English](../en/TheOne.Avalonia.Controls.GaugeChart.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.GaugeChart.md) · [API index](README.md)

`TheOne.Avalonia.Controls.GaugeChart` · Class · `TheOne.Avalonia.Controls.ChartBase`

[Inherited additional members: ChartBase](TheOne.Avalonia.Controls.ChartBase.md)

A directly drawn gauge with enum-selected layouts and property-based visual overrides.

[Source / 声明](../../../TheOne.Avalonia/Controls/GaugeChart.GradeLabels.cs#L6)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`GaugeCaption`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L74) | `string?` | writable; OneWay | `null` | Caption defaults to null; vertical top offset in radius units `[−1, 1]`, NaN follows the enum; separate from top Title |
| [`GaugeCaptionOffset`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L69) | `double` | writable; OneWay | `double.NaN` | Caption defaults to null; vertical top offset in radius units `[−1, 1]`, NaN follows the enum; separate from top Title Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -1 && value <= 1)` |
| [`GaugeGradeLabelGap`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L73) | `double` | writable; OneWay | `double.NaN` | Grade text follows a readable tangent by default; rotation NaN is automatic, `[−90, 90]` explicit degrees (0 horizontal). Gap from bands is NaN for font-based sizing or `[0, 100]` explicit pixels Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= 0 && value <= 100)` |
| [`GaugeGradeLabelRotation`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L71) | `double` | writable; OneWay | `double.NaN` | Grade text follows a readable tangent by default; rotation NaN is automatic, `[−90, 90]` explicit degrees (0 horizontal). Gap from bands is NaN for font-based sizing or `[0, 100]` explicit pixels Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -90 && value <= 90)` |
| [`GaugeNeedleBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L76) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Overrides needle, major ticks, and center value; otherwise theme / band colors |
| [`GaugeNeedleLength`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L64) | `double` | writable; OneWay | `0.72` | Length defaults to 0.72 radius, range `(0, 1]`; width NaN follows shape, explicit pixels `(0, 50]` Validation: `value => double.IsFinite(value) && value > 0 && value <= 1` |
| [`GaugeNeedleShape`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L62) | `TheOne.Avalonia.Controls.GaugeNeedleShape` | writable; OneWay | `GaugeNeedleShape.Auto` | Auto follows the enum; Triangle / Line / Marker overrides shape Validation: `Enum.IsDefined` Values: `Auto`, `Triangle`, `Line`, `Marker` |
| [`GaugeNeedleWidth`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L65) | `double` | writable; OneWay | `double.NaN` | Length defaults to 0.72 radius, range `(0, 1]`; width NaN follows shape, explicit pixels `(0, 50]` Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value > 0 && value <= 50)` |
| [`GaugeSecondaryBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L83) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Secondary count defaults to 7 in `[2, 12]`; color defaults to Foreground |
| [`GaugeSecondaryMaximum`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L81) | `double` | writable; OneWay | `60` | Barometer secondary range, default 0–60, independent of main value / needle. Invalid ranges hide the secondary scale Validation: `double.IsFinite` |
| [`GaugeSecondaryMinimum`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L80) | `double` | writable; OneWay | `0` | Barometer secondary range, default 0–60, independent of main value / needle. Invalid ranges hide the secondary scale Validation: `double.IsFinite` |
| [`GaugeSecondaryTickCount`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L82) | `int` | writable; OneWay | `7` | Secondary count defaults to 7 in `[2, 12]`; color defaults to Foreground Validation: `value => value >= 2 && value <= 12` |
| [`GaugeSegments`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L79) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.GaugeSegment>?` | writable; OneWay | `null` | Independent empty collection per instance, supporting XAML children or binding replacement. Ordered bands with cumulative Fraction `(0, 1]` of Minimum–Maximum; Brush sets color, Label is used by Grade |
| [`GaugeStartAngle`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L56) | `double` | writable; OneWay | `double.NaN` | NaN follows the enum; start −360°–360°, clockwise sweep `(0, 360]`. 0° points right, 90° down. Full circles supported Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -360 && value <= 360)` |
| [`GaugeStyle`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L55) | `TheOne.Avalonia.Controls.GaugeStyle` | writable; OneWay | `GaugeStyle.Semicircle` | Structure ; Default start / clockwise sweep ; Theme needle / ticks / major count Validation: `Enum.IsDefined` Values: `Semicircle`, `Basic`, `Speed`, `Progress`, `StageSpeed`, `Grade`, `Temperature`, `Barometer` |
| [`GaugeSweepAngle`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L57) | `double` | writable; OneWay | `double.NaN` | NaN follows the enum; start −360°–360°, clockwise sweep `(0, 360]`. 0° points right, 90° down. Full circles supported Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value > 0 && value <= 360)` |
| [`GaugeTickBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L77) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Overrides needle, major ticks, and center value; otherwise theme / band colors |
| [`GaugeTickPlacement`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L63) | `TheOne.Avalonia.Controls.GaugeTickPlacement` | writable; OneWay | `GaugeTickPlacement.Auto` | Auto follows the enum; Inside / Outside overrides major placement; barometer secondary ticks remain inside Validation: `Enum.IsDefined` Values: `Auto`, `Inside`, `Outside` |
| [`GaugeTrackBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L75) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Background track color, default GridBrush |
| [`GaugeValueBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L78) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Overrides needle, major ticks, and center value; otherwise theme / band colors |
| [`GaugeValueFontSize`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L66) | `double` | writable; OneWay | `double.NaN` | NaN follows the enum; font `(0, 200]`, vertical radius-relative offset `[−1, 1]`, positive downward. Fonts shrink to fit Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value > 0 && value <= 200)` |
| [`GaugeValueOffset`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L67) | `double` | writable; OneWay | `double.NaN` | NaN follows the enum; font `(0, 200]`, vertical radius-relative offset `[−1, 1]`, positive downward. Fonts shrink to fit Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -1 && value <= 1)` |
| [`Kind`](../../../TheOne.Avalonia/Controls/GaugeChart.cs#L9) | `TheOne.Avalonia.Controls.ChartKind` | read-only; CLR / n/a | `accessor-defined / state` | Kind enum for the owning field/property/element; see its concrete enum for choices. Values: `Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |
| [`ShowGaugeAnchor`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L59) | `bool?` | writable; OneWay | `null` | null follows the enum; controls the center anchor only when the needle is shown |
| [`ShowGaugeProgress`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L58) | `bool?` | writable; OneWay | `null` | null follows the enum; True / False forces progress visibility |
| [`ShowGaugeTickLabels`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L61) | `bool` | writable; OneWay | `true` | Default True; controls value / tick text without changing data |
| [`ShowGaugeValue`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L60) | `bool` | writable; OneWay | `true` | Default True; controls value / tick text without changing data |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`GaugeChart.GaugeChart()`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L26) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<GaugeStyle> GaugeChart.GaugeStyleProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L28) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GaugeChart.GaugeStartAngleProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L29) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GaugeChart.GaugeSweepAngleProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L30) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool?> GaugeChart.ShowGaugeProgressProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L31) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool?> GaugeChart.ShowGaugeAnchorProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L32) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> GaugeChart.ShowGaugeValueProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L33) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> GaugeChart.ShowGaugeTickLabelsProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L34) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<GaugeNeedleShape> GaugeChart.GaugeNeedleShapeProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L35) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<GaugeTickPlacement> GaugeChart.GaugeTickPlacementProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L36) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GaugeChart.GaugeNeedleLengthProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L37) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GaugeChart.GaugeNeedleWidthProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L38) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GaugeChart.GaugeValueFontSizeProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L39) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GaugeChart.GaugeValueOffsetProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L40) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GaugeChart.GaugeGradeLabelRotationProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L41) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GaugeChart.GaugeGradeLabelGapProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L42) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GaugeChart.GaugeCaptionOffsetProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L43) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string?> GaugeChart.GaugeCaptionProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L44) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GaugeChart.GaugeTrackBrushProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L45) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GaugeChart.GaugeNeedleBrushProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L46) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GaugeChart.GaugeTickBrushProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L47) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GaugeChart.GaugeValueBrushProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L48) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IList<GaugeSegment>?> GaugeChart.GaugeSegmentsProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L49) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GaugeChart.GaugeSecondaryMinimumProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L50) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GaugeChart.GaugeSecondaryMaximumProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L51) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<int> GaugeChart.GaugeSecondaryTickCountProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L52) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GaugeChart.GaugeSecondaryBrushProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L53) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

Controls accept `small` / `normal` / `large` or inherited `ControlSizing.Size`. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `large` | [`:is(Control).large`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L37) | :is(Control).large => styling:ControlSizing.Size=Large |
| `normal` | [`:is(Control).normal`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L34) | :is(Control).normal => styling:ControlSizing.Size=Normal |
| `small` | [`:is(Control).small`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L31) | :is(Control).small => styling:ControlSizing.Size=Small |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.ChartKind`](TheOne.Avalonia.Controls.ChartKind.md)
- [`TheOne.Avalonia.Controls.GaugeNeedleShape`](TheOne.Avalonia.Controls.GaugeNeedleShape.md)
- [`TheOne.Avalonia.Controls.GaugeSegment`](TheOne.Avalonia.Controls.GaugeSegment.md)
- [`TheOne.Avalonia.Controls.GaugeStyle`](TheOne.Avalonia.Controls.GaugeStyle.md)
- [`TheOne.Avalonia.Controls.GaugeTickPlacement`](TheOne.Avalonia.Controls.GaugeTickPlacement.md)
