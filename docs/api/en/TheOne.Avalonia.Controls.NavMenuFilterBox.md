# NavMenuFilterBox

[English](../en/TheOne.Avalonia.Controls.NavMenuFilterBox.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.NavMenuFilterBox.md) · [API index](README.md)

`TheOne.Avalonia.Controls.NavMenuFilterBox` · Class · `Avalonia.Controls.AutoCompleteBox`

A native AutoCompleteBox used by NavMenu's optional filter header.

[Source / 声明](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L12)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`NavMenuFilterBox.NavMenuFilterBox()`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L19) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event EventHandler<NavMenuFilterResult>? NavMenuFilterBox.ResultCommitted`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L17) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `filled` | [`TextBox.filled, ComboBox.filled, NumericUpDown.filled, AutoCompleteBox.filled, CalendarDatePicker.filled, DatePicker.filled, TimePicker.filled`](../../../TheOne.Avalonia/Themes/Variants.axaml#L83) | TextBox.filled, ComboBox.filled, NumericUpDown.filled, AutoCompleteBox.filled, CalendarDatePicker.filled, DatePicker.filled, TimePicker.filled => Background={DynamicResource One.Subtle} |
| `underlined` | [`TextBox.underlined, ComboBox.underlined, NumericUpDown.underlined, AutoCompleteBox.underlined`](../../../TheOne.Avalonia/Themes/Variants.axaml#L133) | TextBox.underlined, ComboBox.underlined, NumericUpDown.underlined, AutoCompleteBox.underlined => Background={DynamicResource One.Surface}; BorderThickness=0,0,0,1; CornerRadius=0; TextBox.underlined:focus /template/ Border#PART_BorderElement, AutoCompleteBox.underlined /template/ TextBox#PART_TextBox:focus /template/ Border#PART_BorderElement => BorderThickness=0,0,0,2; CornerRadius=0; ComboBox.underlined:focus-within, NumericUpDown.underlined:focus-within, AutoCompleteBox.underlined:focus-within => BorderThickness=0,0,0,2; TextBox.underlined:disabled /template/ Border#PART_BorderElement, AutoCompleteBox.underlined:disabled /template/ TextBox#PART_TextBox /template/ Border#PART_BorderElement, ComboBox.underlined:disabled /template/ Border#Background, ComboBox.underlined:disabled /template/ Border#HighlightBackground => BorderBrush={DynamicResource One.InputLine} |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.NavMenuFilterResult`](TheOne.Avalonia.Controls.NavMenuFilterResult.md)
