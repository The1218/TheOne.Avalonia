# DataGridSummaryPresenter

[English](../en/TheOne.Avalonia.Controls.DataGridSummaryPresenter.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DataGridSummaryPresenter.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DataGridSummaryPresenter` · Class · `Avalonia.Controls.Control`

One drawn footer, aligned and clipped with native DataGrid column geometry.

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridSummaryPresenter.cs#L14)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Background`](../../../TheOne.Avalonia/Controls/DataGridSummaryPresenter.cs#L22) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Background brush; use DynamicResource for theme-dependent surfaces. |
| [`Foreground`](../../../TheOne.Avalonia/Controls/DataGridSummaryPresenter.cs#L23) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Foreground brush; retain text, status and disabled contrast when overriding. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`override void DataGridSummaryPresenter.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/DataGridSummaryPresenter.cs#L67) | Avalonia drawing callback using the supplied DrawingContext; business code normally does not call it directly. |
| [`DataGridSummaryPresenter.DataGridSummaryPresenter()`](../../../TheOne.Avalonia/Controls/DataGridSummaryPresenter.cs#L24) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<IBrush?> DataGridSummaryPresenter.LineBrushProperty`](../../../TheOne.Avalonia/Controls/DataGridSummaryPresenter.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> DataGridSummaryPresenter.BackgroundProperty`](../../../TheOne.Avalonia/Controls/DataGridSummaryPresenter.cs#L20) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> DataGridSummaryPresenter.ForegroundProperty`](../../../TheOne.Avalonia/Controls/DataGridSummaryPresenter.cs#L21) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.
