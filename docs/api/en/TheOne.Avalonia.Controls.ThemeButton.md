# ThemeButton

[English](../en/TheOne.Avalonia.Controls.ThemeButton.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ThemeButton.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ThemeButton` · Class · `Avalonia.Controls.Primitives.ToggleButton`

Switches the current effective light/dark theme without overriding a binding.

[Source / 声明](../../../TheOne.Avalonia/Controls/ThemeButton.cs#L11)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Scope`](../../../TheOne.Avalonia/Controls/ThemeButton.cs#L16) | `TheOne.Avalonia.Controls.ThemeButtonScope` | writable; OneWay | `0 / default` | Scope of Light/Dark switching; does not change the ThemeColor palette enum. Values: `Window`, `Application` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ThemeButton.ThemeButton()`](../../../TheOne.Avalonia/Controls/ThemeButton.cs#L18) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<ThemeButtonScope> ThemeButton.ScopeProperty`](../../../TheOne.Avalonia/Controls/ThemeButton.cs#L14) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

Controls accept `small` / `normal` / `large` or inherited `ControlSizing.Size`. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `segment` | [`ToggleButton.segment`](../../../TheOne.Avalonia/Themes/Styles.axaml#L69) | ToggleButton.segment => BorderThickness=0 |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.ThemeButtonScope`](TheOne.Avalonia.Controls.ThemeButtonScope.md)
