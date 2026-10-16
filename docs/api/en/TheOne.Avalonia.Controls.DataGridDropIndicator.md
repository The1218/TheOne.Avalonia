# DataGridDropIndicator

[English](../en/TheOne.Avalonia.Controls.DataGridDropIndicator.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DataGridDropIndicator.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DataGridDropIndicator` · Class · `Avalonia.Controls.Control`

Native column insertion caret with a short, cancellable settling motion.

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridDropIndicator.cs#L12)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Foreground`](../../../TheOne.Avalonia/Controls/DataGridDropIndicator.cs#L15) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Foreground brush; retain text, status and disabled contrast when overriding. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`override void DataGridDropIndicator.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/DataGridDropIndicator.cs#L52) | Avalonia drawing callback using the supplied DrawingContext; business code normally does not call it directly. |
| [`DataGridDropIndicator.DataGridDropIndicator()`](../../../TheOne.Avalonia/Controls/DataGridDropIndicator.cs#L18) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<IBrush?> DataGridDropIndicator.ForegroundProperty`](../../../TheOne.Avalonia/Controls/DataGridDropIndicator.cs#L14) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.
