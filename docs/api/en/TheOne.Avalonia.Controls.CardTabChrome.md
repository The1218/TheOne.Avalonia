# CardTabChrome

[English](../en/TheOne.Avalonia.Controls.CardTabChrome.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.CardTabChrome.md) · [API index](README.md)

`TheOne.Avalonia.Controls.CardTabChrome` · Class · `Avalonia.Controls.Decorator`

Draws a card tab with an open content edge and small concave joining corners.

[Source / 声明](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L8)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Background`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L35) | `Avalonia.Media.IBrush?` | writable; OneWay | `inherited registration` | Background brush; use DynamicResource for theme-dependent surfaces. |
| [`BorderBrush`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L41) | `Avalonia.Media.IBrush?` | writable; OneWay | `inherited registration` | Border stroke brush; use dynamic theme resources to follow brightness changes. |
| [`CornerRadius`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L47) | `Avalonia.CornerRadius` | writable; OneWay | `inherited registration` | Corner radius in Avalonia DIPs; sizing and background clipping remain template responsibilities. |
| [`IsSelected`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L53) | `bool` | writable; OneWay | `false` | Current-item selection state, synchronized with the selected model and not shared with unrelated items. |
| [`Placement`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L59) | `Avalonia.Controls.Dock` | writable; OneWay | `Dock.Top` | Placement enum selecting a decoration/content edge rather than string coordinates. Values: `Left`, `Bottom`, `Right`, `Top` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`override void CardTabChrome.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L65) | Avalonia drawing callback using the supplied DrawingContext; business code normally does not call it directly. |
| [`CardTabChrome.CardTabChrome()`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L8) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<IBrush?> CardTabChrome.BackgroundProperty`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L10) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> CardTabChrome.BorderBrushProperty`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L13) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<CornerRadius> CardTabChrome.CornerRadiusProperty`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L16) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> CardTabChrome.IsSelectedProperty`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<Dock> CardTabChrome.PlacementProperty`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.
