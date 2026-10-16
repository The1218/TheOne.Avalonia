# RevealHost

[English](../en/TheOne.Avalonia.Controls.RevealHost.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.RevealHost.md) · [API index](README.md)

`TheOne.Avalonia.Controls.RevealHost` · Class · `Avalonia.Controls.Decorator`

Reveals a fully laid-out child through a smoothly sized, clipped viewport.

[Source / 声明](../../../TheOne.Avalonia/Controls/RevealHost.cs#L14)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ConstrainContent`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L31) | `bool` | writable; OneWay | `false` | Preserve a finite parent viewport for content such as a virtualized list. |
| [`Direction`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L28) | `Avalonia.Controls.ExpandDirection` | writable; OneWay | `ExpandDirection.Down` | Reveal/port direction enum; choices are defined by its concrete enum type. Values: `Down`, `Up`, `Left`, `Right` |
| [`Duration`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L29) | `System.TimeSpan` | writable; OneWay | `TimeSpan.FromMilliseconds(200)` | Duration; motion still follows inherited Motion.Enabled rather than using duration as its only switch. |
| [`IsOpen`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L27) | `bool` | writable; OneWay | `false` | Expands/shows content; collapsing retains the content object and transitions follow Motion.Enabled. |
| [`TransitionExtent`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L22) | `double` | writable; OneWay | `double.PositiveInfinity` | Optional visible-distance cap during animation; the settled layout retains its full extent. Validation: `value => value > 0 && !double.IsNaN(value)` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`RevealHost.RevealHost()`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L34) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<bool> RevealHost.IsOpenProperty`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L16) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<ExpandDirection> RevealHost.DirectionProperty`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L17) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<TimeSpan> RevealHost.DurationProperty`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> RevealHost.ConstrainContentProperty`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RevealHost.TransitionExtentProperty`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L21) | Optional visible-distance cap during animation; the settled layout retains its full extent. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.
