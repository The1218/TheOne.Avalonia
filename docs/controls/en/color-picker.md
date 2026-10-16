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
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

These native controls have no same-name TheOne wrappers; project-added capabilities come from the styles below.

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

No added public appearance classes; use the default theme and control-specific properties.
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

No additional business methods; configure the control-specific properties above.

### Related models

No additional related data models.

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
