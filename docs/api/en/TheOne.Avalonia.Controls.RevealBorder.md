# RevealBorder

[English](../en/TheOne.Avalonia.Controls.RevealBorder.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.RevealBorder.md) · [API index](README.md)

`TheOne.Avalonia.Controls.RevealBorder` · Class · `Avalonia.Controls.Border`

Fades and translates content; keeps it laid out until the closing animation completes.

[Source / 声明](../../../TheOne.Avalonia/Controls/RevealBorder.cs#L13)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Duration`](../../../TheOne.Avalonia/Controls/RevealBorder.cs#L20) | `System.TimeSpan` | writable; OneWay | `TimeSpan.FromMilliseconds(180)` | Duration; motion still follows inherited Motion.Enabled rather than using duration as its only switch. |
| [`IsOpen`](../../../TheOne.Avalonia/Controls/RevealBorder.cs#L19) | `bool` | writable; OneWay | `false` | Expands/shows content; collapsing retains the content object and transitions follow Motion.Enabled. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`RevealBorder.RevealBorder()`](../../../TheOne.Avalonia/Controls/RevealBorder.cs#L22) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<bool> RevealBorder.IsOpenProperty`](../../../TheOne.Avalonia/Controls/RevealBorder.cs#L15) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<TimeSpan> RevealBorder.DurationProperty`](../../../TheOne.Avalonia/Controls/RevealBorder.cs#L16) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

Controls accept `small` / `normal` / `large` or inherited `ControlSizing.Size`. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.
