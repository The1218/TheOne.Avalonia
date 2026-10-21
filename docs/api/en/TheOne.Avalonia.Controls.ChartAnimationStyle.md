# ChartAnimationStyle

[English](../en/TheOne.Avalonia.Controls.ChartAnimationStyle.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ChartAnimationStyle.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ChartAnimationStyle` · Enum · `System.Enum`

Controls the presentation of chart data changes without changing target data or hit testing.

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartAnimationStyle.cs#L4)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ChartAnimationStyle.ChartAnimationStyle()`](../../../TheOne.Avalonia/Controls/ChartAnimationStyle.cs#L4) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Enum values

| Complete signature/value | Contract |
| --- | --- |
| [`Auto = 0`](../../../TheOne.Avalonia/Controls/ChartAnimationStyle.cs#L7) | Reveal the first frame; interpolate compatible data changes, otherwise crossfade. |
| [`None = 1`](../../../TheOne.Avalonia/Controls/ChartAnimationStyle.cs#L9) | Present the complete target frame immediately. |
| [`Fade = 2`](../../../TheOne.Avalonia/Controls/ChartAnimationStyle.cs#L11) | Fade in the prepared target plot while keeping its geometry cached. |
| [`Reveal = 3`](../../../TheOne.Avalonia/Controls/ChartAnimationStyle.cs#L13) | Reveal the prepared target plot from left to right. |
