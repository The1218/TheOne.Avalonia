# GanttHeaderPresenter

[English](../en/TheOne.Avalonia.Controls.GanttHeaderPresenter.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.GanttHeaderPresenter.md) · [API index](README.md)

`TheOne.Avalonia.Controls.GanttHeaderPresenter` · Class · `Avalonia.Controls.Control`

Template part for GanttChart's frozen calendar and task header.

[Source / 声明](../../../TheOne.Avalonia/Controls/GanttChart.Rendering.cs#L211)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`override void GanttHeaderPresenter.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/GanttChart.Rendering.cs#L214) | Avalonia drawing callback using the supplied DrawingContext; business code normally does not call it directly. |
| [`GanttHeaderPresenter.GanttHeaderPresenter()`](../../../TheOne.Avalonia/Controls/GanttChart.Rendering.cs#L211) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Styles and density

Controls accept `small` / `normal` / `large` or inherited `ControlSizing.Size`. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `large` | [`:is(Control).large`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L37) | :is(Control).large => styling:ControlSizing.Size=Large |
| `normal` | [`:is(Control).normal`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L34) | :is(Control).normal => styling:ControlSizing.Size=Normal |
| `small` | [`:is(Control).small`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L31) | :is(Control).small => styling:ControlSizing.Size=Small |
