# StatisticalChartBase

[English](../en/TheOne.Avalonia.Controls.StatisticalChartBase.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.StatisticalChartBase.md) · [API index](README.md)

`TheOne.Avalonia.Controls.StatisticalChartBase` · Class · `TheOne.Avalonia.Controls.SpecializedChartBase`

[Inherited additional members: SpecializedChartBase](TheOne.Avalonia.Controls.SpecializedChartBase.md)

Cached numeric axes, viewport navigation and indexed hits for range-valued observations.

[Source / 声明](../../../TheOne.Avalonia/Controls/StatisticalChartBase.cs#L7)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`PointWidthRatio`](../../../TheOne.Avalonia/Controls/StatisticalChartBase.cs#L21) | `double` | writable; OneWay | `.6` | PointWidthRatio configuration on StatisticalChartBase; type and initial value are listed here. PointWidthRatioProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. Validation: `value => double.IsFinite(value) && value > 0 && value <= 1` |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<double> StatisticalChartBase.PointWidthRatioProperty`](../../../TheOne.Avalonia/Controls/StatisticalChartBase.cs#L9) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `animation-fade` | [`^.animation-fade`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L27) | ^.animation-fade => AnimationStyle=Fade |
| `animation-none` | [`^.animation-none`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L33) | ^.animation-none => AnimationStyle=None |
| `animation-reveal` | [`^.animation-reveal`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L30) | ^.animation-reveal => AnimationStyle=Reveal |
