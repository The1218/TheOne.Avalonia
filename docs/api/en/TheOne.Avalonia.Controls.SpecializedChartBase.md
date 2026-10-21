# SpecializedChartBase

[English](../en/TheOne.Avalonia.Controls.SpecializedChartBase.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.SpecializedChartBase.md) · [API index](README.md)

`TheOne.Avalonia.Controls.SpecializedChartBase` · Class · `TheOne.Avalonia.Controls.ChartBase`

[Inherited additional members: ChartBase](TheOne.Avalonia.Controls.ChartBase.md)

Shared direct-drawing infrastructure for non-Cartesian statistical charts.

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartBase.Specialized.cs#L54)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `animation-fade` | [`^.animation-fade`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L27) | ^.animation-fade => AnimationStyle=Fade |
| `animation-none` | [`^.animation-none`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L33) | ^.animation-none => AnimationStyle=None |
| `animation-reveal` | [`^.animation-reveal`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L30) | ^.animation-reveal => AnimationStyle=Reveal |
