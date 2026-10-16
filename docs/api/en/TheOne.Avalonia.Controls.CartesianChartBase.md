# CartesianChartBase

[English](../en/TheOne.Avalonia.Controls.CartesianChartBase.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.CartesianChartBase.md) · [API index](README.md)

`TheOne.Avalonia.Controls.CartesianChartBase` · Class · `TheOne.Avalonia.Controls.ChartBase`

[Inherited additional members: ChartBase](TheOne.Avalonia.Controls.ChartBase.md)

Shared axes, data preparation and interaction for fixed Cartesian chart controls.

[Source / 声明](../../../TheOne.Avalonia/Controls/CartesianChartBase.cs#L7)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Styles and density

Controls accept `small` / `normal` / `large` or inherited `ControlSizing.Size`. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `large` | [`:is(Control).large`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L37) | :is(Control).large => styling:ControlSizing.Size=Large |
| `normal` | [`:is(Control).normal`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L34) | :is(Control).normal => styling:ControlSizing.Size=Normal |
| `small` | [`:is(Control).small`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L31) | :is(Control).small => styling:ControlSizing.Size=Small |
