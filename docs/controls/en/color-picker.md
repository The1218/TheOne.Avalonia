# ColorPicker and ColorView

[简体中文](../zh-cn/color-picker.md) · [Controls](README.md)

`OneTheme` includes the official `Avalonia.Controls.ColorPicker` **12.1.4** package
and its Fluent color-editing templates, with TheOne chrome and a scoped dropdown-button template. No wrapper control
or extra theme include is required. Continue using `Avalonia.Controls.ColorPicker`
for a dropdown and `ColorView` for inline editing.

```xml
<!-- Declare your real ViewModel with x:DataType and keep compiled bindings enabled. -->
<StackPanel Spacing="16">
  <ColorPicker Color="{Binding SelectedColor, Mode=TwoWay}" />
  <ColorView Color="{Binding SelectedColor, Mode=TwoWay}"
             IsAlphaEnabled="True" />
</StackPanel>
```

`SelectedColor` is an `Avalonia.Media.Color`. The official `HsvColor` property,
`ColorChanged` event, `ColorModel`, `Palette`, visibility flags, range limits,
`IsAlphaEnabled`, `IsAlphaVisible` and `HexInputAlphaPosition` keep their existing
behavior. Follow the configured alpha position when entering eight-digit hex;
do not assume it is the same as `Color.ToString()` (`#AARRGGBB`).

The dropdown shows a transparency-aware swatch and the selected color's string
value. A locally supplied `Content`/`ContentTemplate` still overrides that default.
The compact panel takes inspiration from Eremex's desktop color editor and opens the palette by default (`SelectedIndex="1"`). Only Custom / Palette text tabs remain.
Custom (`SelectedIndex="0"`) combines the saturation/value spectrum, RGB / HSV switch, four inline channel inputs, Hex, hue and alpha sliders. There is no separate Values page (index 2).
The RGB / HSV model buttons have a 4 px gap in both inline and dropdown panels.
The native input instances, named parts and two-way bindings remain intact. `IsColorComponentsVisible` controls the input group; component-only editing remains available on Custom with `IsColorSpectrumVisible="False"`.
The default palette has ten hue columns and five shade rows, including neutrals and TheOne teal; the official `Palette` property can replace it.
The large preview strip and accent colors are hidden by default (`IsColorPreviewVisible="False"`, `IsAccentColorsVisible="False"`); consumers can enable both using the official properties.
One rounded border renders the dropdown at rest, during pointer presses and while open. Color
gradients, alpha checkerboard, color values and contrast-aware spectrum indicators
remain functional; they are not replaced with the theme accent color.

Shared `One.*` resources determine surfaces, text, borders, active tabs and focus
feedback. Light/Dark scopes follow the rest of the library. `ControlSizing.Size`
values Small/Normal/Large provide 30/36/44 px dropdown and text input
heights. The inline view adapts its native content to widths from 280 to 340 px.
Disabling a control retains the official input behavior and adds themed feedback.
The spectrum supports Tab focus and the official arrow-key color adjustment.

The PreviewHost navigation page is **ColorPicker**. `--colorpicker` opens a
standalone demo; `--verify-colorpicker [output]` runs only ColorPicker / ColorView
layout, theme, binding, pointer, keyboard and pixel checks.
`--verify-colorpicker-model-spacing [output]` checks only model-button spacing,
label fit, native pointer / keyboard switching and gap pixels across themes and sizes.

The official package supplies all control logic and base templates:
[Avalonia ColorPicker documentation](https://docs.avaloniaui.net/controls/input/selectors/colorpicker).

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section enumerates declarations added by this library; native inherited members remain Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. Type links provide full model, method, event and enum signatures. PreviewHost keeps complete demos on their own pages, with styles, additional properties, APIs/events and models on each page. Only Button, Input and Select share comparison pages.

These native controls have no same-name TheOne wrappers; additional behavior comes from the styles and shared attached properties below.

### Shared attached properties

[ControlSizing](../../api/en/TheOne.Avalonia.Styling.ControlSizing.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Size`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L11) | `TheOne.Avalonia.Styling.ControlSize` | writable; OneWay; attached; inherits | `ControlSize.Normal` | Shared Small/Normal/Large density inherited by descendants; explicit local values/classes can override it. Values: `Small`, `Normal`, `Large` |

[Motion](../../api/en/TheOne.Avalonia.Styling.Motion.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | writable; OneWay; attached; inherits | `true` | Inherited motion switch; disabling goes to final layout/state while preserving native input. |

### Style classes and automatic state

| Control | Class | Scope | Effective setters/resources |
| --- | --- | --- | --- |
| ColorPicker | `one-large` | automatic template class; do not bind manually | ColorPicker.one-large => MinHeight=44; FontSize=16; ColorPicker.one-large /template/ DropDownButton => MinHeight=44 |
| ColorPicker | `one-normal` | automatic template class; do not bind manually | ColorPicker.one-normal /template/ DropDownButton => MinHeight=36 |
| ColorPicker | `one-small` | automatic template class; do not bind manually | ColorPicker.one-small => MinHeight=30; FontSize=12; ColorPicker.one-small /template/ DropDownButton => MinHeight=30 |
| ColorView | `one-large` | automatic template class; do not bind manually | :is(ColorView).one-large /template/ TextBox#PART_HexTextBox, :is(ColorView).one-large /template/ NumericUpDown, FlyoutPresenter.one-colorpicker-large TextBox#PART_HexTextBox, FlyoutPresenter.one-colorpicker-large NumericUpDown => MinHeight=44; MaxHeight=44; FontSize=16; :is(ColorView).one-large /template/ TextBox#PART_HexTextBox, :is(ColorView).one-large /template/ NumericUpDown, FlyoutPresenter.one-colorpicker-large TextBox#PART_HexTextBox, FlyoutPresenter.one-colorpicker-large NumericUpDown => MinHeight=44; MaxHeight=44; FontSize=16; :is(ColorView).one-large /template/ Grid#HexInputGrid > Border, FlyoutPresenter.one-colorpicker-large Grid#HexInputGrid > Border => MinHeight=44; MaxHeight=44 |
| ColorView | `one-small` | automatic template class; do not bind manually | :is(ColorView).one-small /template/ TextBox#PART_HexTextBox, :is(ColorView).one-small /template/ NumericUpDown, FlyoutPresenter.one-colorpicker-small TextBox#PART_HexTextBox, FlyoutPresenter.one-colorpicker-small NumericUpDown => MinHeight=30; MaxHeight=30; FontSize=12; :is(ColorView).one-small /template/ TextBox#PART_HexTextBox, :is(ColorView).one-small /template/ NumericUpDown, FlyoutPresenter.one-colorpicker-small TextBox#PART_HexTextBox, FlyoutPresenter.one-colorpicker-small NumericUpDown => MinHeight=30; MaxHeight=30; FontSize=12; :is(ColorView).one-small /template/ Grid#HexInputGrid > Border, FlyoutPresenter.one-colorpicker-small Grid#HexInputGrid > Border => MinHeight=30; MaxHeight=30 |

`small` / `normal` / `large` are public density inputs; `one-small` / `one-normal` / `one-large` are automatic internal classes. `:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without additional appearance classes use their default theme and properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
