# OneTheme

[English](../en/TheOne.Avalonia.Themes.OneTheme.md) · [简体中文](../zh-cn/TheOne.Avalonia.Themes.OneTheme.md) · [API index](README.md)

`TheOne.Avalonia.Themes.OneTheme` · Class · `Avalonia.Styling.Styles`

TheOne theme for Avalonia's built-in controls, including light and dark resources.

[Source / 声明](../../../TheOne.Avalonia/Themes/OneTheme.axaml.cs#L10)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ColorTheme`](../../../TheOne.Avalonia/Themes/OneTheme.axaml.cs#L30) | `TheOne.Avalonia.Themes.ThemeColor` | writable; OneWay | `0 / default` | Color palette; changing this does not change the requested Light / Dark variant. Validation: `value => Enum.IsDefined(value)` Values: `Nova`, `Atlas`, `AtlasDeep`, `Helios`, `HeliosDeep`, `NovaDeep`, `Terra`, `TerraDeep`, `Vega`, `VegaDeep` |
| [`ColorThemes`](../../../TheOne.Avalonia/Themes/OneTheme.axaml.cs#L36) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Themes.ThemeColor>` | read-only; CLR / n/a | `Array.AsReadOnly(Enum.GetValues<ThemeColor>())` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static void OneTheme.SetColor(ThemeColor color)`](../../../TheOne.Avalonia/Themes/OneTheme.axaml.cs#L41) | Switches the application's OneTheme palette on the UI thread, preserving Light / Dark and property bindings. |
| [`OneTheme.OneTheme()`](../../../TheOne.Avalonia/Themes/OneTheme.axaml.cs#L17) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<ThemeColor> OneTheme.ColorThemeProperty`](../../../TheOne.Avalonia/Themes/OneTheme.axaml.cs#L12) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Themes.ThemeColor`](TheOne.Avalonia.Themes.ThemeColor.md)
