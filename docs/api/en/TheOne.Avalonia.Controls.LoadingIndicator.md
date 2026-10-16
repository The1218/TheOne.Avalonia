# LoadingIndicator

[English](../en/TheOne.Avalonia.Controls.LoadingIndicator.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.LoadingIndicator.md) · [API index](README.md)

`TheOne.Avalonia.Controls.LoadingIndicator` · Class · `Avalonia.Controls.Control`

A lightweight spinner that becomes a static status glyph when motion is disabled.

[Source / 声明](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L14)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Foreground`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L19) | `Avalonia.Media.IBrush?` | writable; OneWay | `inherited registration` | Foreground brush; retain text, status and disabled contrast when overriding. |
| [`IsActive`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L20) | `bool` | writable; OneWay | `true` | Whether the indicator animates; disabling preserves layout and the global motion switch still applies. |
| [`IsSpinning`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L21) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`override void LoadingIndicator.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L36) | Avalonia drawing callback using the supplied DrawingContext; business code normally does not call it directly. |
| [`LoadingIndicator.LoadingIndicator()`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L26) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<IBrush?> LoadingIndicator.ForegroundProperty`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L16) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> LoadingIndicator.IsActiveProperty`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L17) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<LoadingIndicator, bool> LoadingIndicator.IsSpinningProperty`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.
