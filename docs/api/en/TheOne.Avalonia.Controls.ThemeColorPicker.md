# ThemeColorPicker

[English](../en/TheOne.Avalonia.Controls.ThemeColorPicker.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ThemeColorPicker.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ThemeColorPicker` · Class · `Avalonia.Controls.ComboBox`

Selects the application OneTheme color palette independently of Light / Dark.

[Source / 声明](../../../TheOne.Avalonia/Controls/ThemeColorPicker.cs#L11)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`SelectThemeColor`](../../../TheOne.Avalonia/Controls/ThemeColorPicker.cs#L17) | `TheOne.Avalonia.Themes.ThemeColor` | writable; TwoWay | `0 / default` | The selected palette, suitable for a persisted two-way enum binding. Validation: `value => Enum.IsDefined(value)` Values: `Nova`, `Atlas`, `AtlasDeep`, `Helios`, `HeliosDeep`, `NovaDeep`, `Terra`, `TerraDeep`, `Vega`, `VegaDeep` |
| [`SelectedColor`](../../../TheOne.Avalonia/Controls/ThemeColorPicker.cs#L25) | `TheOne.Avalonia.Themes.ThemeColor` | writable; TwoWay | `0 / default` | Compatibility alias of SelectedThemeColor; both represent the same palette enum. Prefer binding SelectedThemeColor. Validation: `value => Enum.IsDefined(value)` Values: `Nova`, `Atlas`, `AtlasDeep`, `Helios`, `HeliosDeep`, `NovaDeep`, `Terra`, `TerraDeep`, `Vega`, `VegaDeep` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ThemeColorPicker.ThemeColorPicker()`](../../../TheOne.Avalonia/Controls/ThemeColorPicker.cs#L37) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<ThemeColor> ThemeColorPicker.SelectThemeColorProperty`](../../../TheOne.Avalonia/Controls/ThemeColorPicker.cs#L14) | The selected palette, suitable for a persisted two-way enum binding. |
| [`static readonly StyledProperty<ThemeColor> ThemeColorPicker.SelectedColorProperty`](../../../TheOne.Avalonia/Controls/ThemeColorPicker.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `filled` | [`TextBox.filled, ComboBox.filled, NumericUpDown.filled, AutoCompleteBox.filled, CalendarDatePicker.filled, DatePicker.filled, TimePicker.filled`](../../../TheOne.Avalonia/Themes/Variants.axaml#L83) | TextBox.filled, ComboBox.filled, NumericUpDown.filled, AutoCompleteBox.filled, CalendarDatePicker.filled, DatePicker.filled, TimePicker.filled => Background={DynamicResource One.Subtle}; ComboBox.filled /template/ Border#HighlightBackground => Background={DynamicResource One.Subtle} |
| `underlined` | [`TextBox.underlined, ComboBox.underlined, NumericUpDown.underlined, AutoCompleteBox.underlined`](../../../TheOne.Avalonia/Themes/Variants.axaml#L133) | TextBox.underlined, ComboBox.underlined, NumericUpDown.underlined, AutoCompleteBox.underlined => Background={DynamicResource One.Surface}; BorderThickness=0,0,0,1; CornerRadius=0; ComboBox.underlined:focus-within, NumericUpDown.underlined:focus-within, AutoCompleteBox.underlined:focus-within => BorderThickness=0,0,0,2; ComboBox.underlined /template/ TextBox#PART_EditableTextBox => MinHeight=0; FocusAdorner={x:Null}; ComboBox.underlined /template/ TextBox#PART_EditableTextBox /template/ Border#PART_BorderElement => Background=Transparent; BorderBrush=Transparent; BorderThickness=0; CornerRadius=0; ComboBox.underlined:focus-within /template/ Border#Background, ComboBox.underlined:focus-within /template/ Border#HighlightBackground => BorderBrush={DynamicResource One.Focus}; ComboBox.underlined:focus-within /template/ Border#Background, ComboBox.underlined:focus-within /template/ Border#HighlightBackground => BorderBrush={DynamicResource One.Focus}; ComboBox.underlined:error /template/ Border#Background, ComboBox.underlined:error /template/ Border#HighlightBackground => BorderBrush={DynamicResource One.Danger}; ComboBox.underlined:error /template/ Border#Background, ComboBox.underlined:error /template/ Border#HighlightBackground => BorderBrush={DynamicResource One.Danger}; TextBox.underlined:disabled /template/ Border#PART_BorderElement, AutoCompleteBox.underlined:disabled /template/ TextBox#PART_TextBox /template/ Border#PART_BorderElement, ComboBox.underlined:disabled /template/ Border#Background, ComboBox.underlined:disabled /template/ Border#HighlightBackground => BorderBrush={DynamicResource One.InputLine}; TextBox.underlined:disabled /template/ Border#PART_BorderElement, AutoCompleteBox.underlined:disabled /template/ TextBox#PART_TextBox /template/ Border#PART_BorderElement, ComboBox.underlined:disabled /template/ Border#Background, ComboBox.underlined:disabled /template/ Border#HighlightBackground => BorderBrush={DynamicResource One.InputLine} |

## Related APIs and built-in models

- [`TheOne.Avalonia.Themes.ThemeColor`](TheOne.Avalonia.Themes.ThemeColor.md)
