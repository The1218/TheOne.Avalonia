# Default colors and control appearances

[简体中文](../zh-cn/theme-colors.md) · **English**

This page describes the default palette. See [ThemeColorPicker / ThemeButton](theme-color-picker.md) for nine additional palettes, the palette dropdown, and independent Light / Dark switching.

Default colors map individually from [Eremex DeltaDesign Nova resources](https://github.com/Eremex/controlthemes/blob/5f6cb3ad8b904cead6bd432f62596648cc9a1039/Source/Eremex.Avalonia.Themes.DeltaDesign/PaletteResources/Nova.axaml), cross-checked against runtime resources from `Eremex.Avalonia.Themes.DeltaDesign 1.5.53-preview3`. The reference demo's theme entry is [App.axaml](https://github.com/Eremex/controls-demo/blob/2edf87b8eea9f58f935b0383bbf724abfd43122a/DemoCenter/DemoCenter/App.axaml). Colors are not approximated from old screenshots or artificially darkened / saturated.

Colors live directly in library `Themes/Colors.axaml` and take effect with OneTheme. Shared `One.*` names remain; buttons, inputs, selection, lists, grids, navigation, menus, popups, windows, and messaging reuse them without host injection. Native Avalonia templates, behaviors, and sizes remain; no Eremex dependency or copied template implementation is added.

| One semantics / Nova role | Light | Dark |
| --- | --- | --- |
| Canvas, Surface / Primary Enabled | `#FFFFFF` | `#0F0F0F` |
| Subtle, CardTabStrip / Secondary Enabled | `#EDEDED` | `#282929` |
| NeutralHover / Secondary Hover | `#D6D7D7` | `#3F4141` |
| NeutralPressed / Secondary Pressed | `#CECFCF` | `#494B4B` |
| Text / Text Neutral Primary | `#212222` | `#F2F2F2` |
| Muted / Text Neutral Secondary | `#474D4D` | `#CACECE` |
| Line, InputLine / Neutral Light | `#D2D5D5` | `#3D4242` |
| CardTabBorder / On Secondary Light | `#CACECE` | `#474D4D` |
| Accent / Enabled | `#006061` | `#63D9DE` |
| AccentHover / Hover | `#006A6B` | `#5DD2D5` |
| AccentPressed / Pressed | `#007475` | `#55CBCE` |
| AccentSoft / Item Enabled | `#C1DCDC` | `#284343` |
| AccentSoftHover / Item Hover | `#B9D4D4` | `#324E4E` |
| AccentSoftPressed / Item Pressed | `#B1CDCD` | `#3C5858` |
| RowSelected / Row Enabled | `#DFF1F1` | `#162C2C` |
| RowSelectedHover / Row Hover | `#D6EBEB` | `#1F3838` |
| RowSelectedPressed / Row Pressed | `#CDE4E5` | `#264040` |
| Focus / Outline Accent Focus | `#129190` | `#36AFB0` |
| OnAccent / Text On Accent | `#F7F8F8` | `#181B1B` |
| Success, Info / Outline Green, Blue | `#007A06`, `#395DC6` | `#57D15D`, `#9EB4FF` |
| Warning, Danger / Outline Yellow, Error | `#855D00`, `#99000D` | `#DDB35F`, `#FFBDC2` |

All 48 Light / Dark mappings, original resource names, and source commits are in [theme-reference-nova.json](../theme-reference-nova.json). Static checks cover 96 source values; runtime checks cover both themes and all densities to prevent fallback to old colors. Ordinary buttons / editors use Secondary fill and buttons use accent text; dialogs / content use Primary. Hover / pressed no longer reuse separator colors. Lists / grids use Item / Row highlights respectively.

Default geometry follows reference roles instead of applying one radius everywhere:

| Area | Radius / outline |
| --- | --- |
| Buttons, editors, list borders | 6 px; editor outlines transparent by default, 1 px focus outline |
| ListBoxItem, ComboBoxItem | 4 px Small; 6 px Normal / Large |
| Menus, Flyout, date/time popups, notifications, dialogs | 8 px; popups use OutlineStrong |
| ToolTip, CheckBox indicator | 4 px and 3 px respectively |
| DataGrid | Native square border, OutlineMedium separators |
| Custom InfoCard | Existing 12 px and region separators |

CheckBox / RadioButton unchecked fill, selected icons, and disabled states map to Secondary, IconOnAccent, and DisabledSelection. Switch track / thumb map separately rather than reusing text colors. Notification side fills use `SuccessFill / InfoFill / WarningFill / DangerFill`; icons use semantic outline colors. Unselected list / menu hover uses a neutral background.

Charts separately map reference `Variants/Light|Dark/ChartColors.axaml`: white / black backgrounds, text `#385252 / #DBE6E6`, and grid `#CADADA / #707575`, instead of ordinary Surface / Line. Existing series combinations and gradients remain extensions.

Card tab headers have no outer border; content retains a thin border with 4 px corners. Selected headers have 6 px top corners and concave joins toward content. All four tab orientations keep content corners with Secondary showing behind the radii.

Original Nova neutral borders have contrast below 3:1; Light ToggleButton pressed Accent / AccentSoftPressed text is about 4.39:1. To preserve source colors, values remain and [contrast-report.json](../contrast-report.json) records actual ratios / exceptions rather than claiming full WCAG compliance. Other body, focus, status, and gradient text continues to be checked for readability. Hosts needing stronger boundaries can override InputLine / CardTabBorder.

## Gradients and extended colors

All ten `ThemeColor` palettes provide eight additional gradients in both Light and Dark: four subtle and four vivid variants. Use `DynamicResource` to follow palette and mode changes.

| Use | Subtle | Vivid |
| --- | --- | --- |
| Brand | `One.Gradient.Subtle.Brand` | `One.Gradient.Vivid.Brand` |
| Cool | `One.Gradient.Subtle.Cool` | `One.Gradient.Vivid.Cool` |
| Natural | `One.Gradient.Subtle.Natural` | `One.Gradient.Vivid.Natural` |
| Warm | `One.Gradient.Subtle.Warm` | `One.Gradient.Vivid.Warm` |

Each key holds an immutable LinearGradientBrush and exposes matching `.StartColor` and `.EndColor` keys. Subtle blends small semantic-color proportions into the surface; use `One.Text` for card text. Vivid transitions from the palette accent toward a light/dark endpoint or Info / Success / Warning, suitable for accents and decoration. Check contrast for any overlaid text in the chosen palette. Existing Brand / BrandHover / BrandPressed / Glow / Data keys remain available.

```xml
<Border Background="{DynamicResource One.Gradient.Subtle.Brand}" Padding="16">
  <TextBlock Text="Themed card" Foreground="{DynamicResource One.Text}" />
</Border>
```

Override keys in host Application.Resources or local control Resources; use ThemeDictionaries for independent Light / Dark overrides. Use the [enum API](theme-color-picker.md) instead of removing or reloading OneTheme on each selection; existing controls and bindings remain intact.

Gradients, soft semantic backgrounds, and chart series combinations are TheOne extensions, not claims of native Eremex effects:

- `One.Gradient.Brand` transitions from Nova Accent toward adjacent cyan, with Hover / Pressed variants and OnAccent text.
- `One.Gradient.Glow` transitions from Primary Hover to Row Enabled for card headings, using Text / Muted.
- `One.Gradient.Data` transitions from Item Enabled to Accent for data fills only.
- Semantic `*Soft` colors mix Primary with the original status colors. InfoCard body uses Primary; Footer mixes 20% Primary Hover into Primary for a subtle distinction.
- Six chart colors come from Eremex Graphics Blue Green, Blue, Yellow, Purple, Purple Red, and Green scales. Light uses Highest and Dark Lowest for readable pie percentages. This combination is the library's choice, not the reference chart's default order.

Gradients travel at a shallow horizontal angle (0%,0% → 100%,35%), with narrower hue differences at brand endpoints and separate Hover / Pressed brightness levels. ToggleButton null state uses reference Primary surfaces plus a dash, distinguishing it from unchecked.

All 65 control theme files were reviewed. Defaults use shared semantic resources; window overlays, gradients, and custom controls are explicit extensions. See [reference-audit.json](../reference-audit.json) for scope. Color consistency does not imply identical Eremex proprietary templates, layouts, or interactions; native Avalonia structures remain.

Brand gradients expose matching `.StartColor` / `.EndColor` resources. Check every state and intermediate gradient color when overriding brushes. DynamicResource templates support live theme changes.

## Appearance choices

| Control | Added styles | Rationale |
| --- | --- | --- |
| Button / IconButton / ToggleButton / RepeatButton | `soft`, `gradient` | Secondary / emphasized actions, keeping primary / outline / ghost / link and semantic classes |
| SplitButton / ToggleSplitButton | `gradient` | Shared hierarchy across action areas, retaining native Flyout |
| TextBox / ComboBox / NumericUpDown / AutoCompleteBox | `filled`, `underlined` | Filled / bottom-line appearances with native input, focus, validation |
| CalendarDatePicker / DatePicker / TimePicker | `filled` | Filled input with native pickers and centered text |
| TabControl | `pills` | Pill headers with native virtualization, keyboard input, presenters |
| ProgressBar | `gradient`, `success`, `info`, `warning`, `danger` | Ordinary progress and task status |
| InfoCard | `glow`, `flat`, `outlined` | Emphasized information or embedded layouts; no expansion / collapse |
| ListBox / DataGrid | `borderless`; ListBox also `cards` | Avoid nested borders while retaining native virtualization / selection |

Navigation, date/time, menus, and selected grid states use solid fills for clear focus / selection / disabled feedback. Tag already has Soft / Outline / Solid and status properties, so duplicate classes are unnecessary. Charts use six independent series colors and brush / type options instead of identical style classes on every chart.

```xml
<Button Classes="gradient" Content="创建工作区" Command="{Binding CreateCommand}" />
<Button Classes="soft small" Content="稍后处理" />
<TextBox Classes="filled" Text="{Binding Name, Mode=TwoWay}" />
<ProgressBar Classes="success" Value="100" />
<one:InfoCard Classes="glow" Header="概览" Content="内容" />
```

Choose one primary appearance class per control; avoid mutually exclusive combinations such as `primary gradient danger`. Size classes can be added. Local properties and custom themes retain Avalonia property priorities.

Default chart brushes map to `One.Chart.Series1..6`, cycling after six. For many categories, combine colors with legends, marks, and line styles rather than relying on color alone. `ChartSeries.Brush` wins; `FifthBrush` / `SixthBrush` can be overridden and exported like the original four.

Appearances are shown on their existing control pages, with no separate theme-preview page. Checks / targeted rendering: [preview entry points](../../../design/PreviewHost/Program.cs). Contrast results: [contrast-report.json](../contrast-report.json).

`underlined` input focus uses a 2 px bottom line; validation still uses Danger and disabled baselines retain InputLine. Editable ComboBox uses one outer baseline, with no separate editor outline; its focus and validation overlays share the baseline color. NumericUpDown spin buttons stay borderless in this variant. PreviewHost's `--verify-underlined-inputs [output]` checks TextBox, editable/noneditable ComboBox, and AutoCompleteBox independently of other controls. Selected `pills` headers use Accent / OnAccent. `cards` only changes ListBox native container spacing, radii, and borders, retaining VirtualizingStackPanel. `outlined` cards keep content slots, separators, and subtle footer color differences.

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### OneTheme

[Complete source contract](../../api/en/TheOne.Avalonia.Themes.OneTheme.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ColorTheme`](../../../TheOne.Avalonia/Themes/OneTheme.axaml.cs#L14) | `TheOne.Avalonia.Themes.ThemeColor` | writable; OneWay | `0 / default` | Color palette; changing this does not change the requested Light / Dark variant. Validation: `value => Enum.IsDefined(value)` Values: `Nova`, `Atlas`, `AtlasDeep`, `Helios`, `HeliosDeep`, `NovaDeep`, `Terra`, `TerraDeep`, `Vega`, `VegaDeep` |
| [`ColorThemes`](../../../TheOne.Avalonia/Themes/OneTheme.axaml.cs#L15) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Themes.ThemeColor>` | read-only; CLR / n/a | `Array.AsReadOnly(Enum.GetValues<ThemeColor>())` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

### ThemeColor

[Complete source contract](../../api/en/TheOne.Avalonia.Themes.ThemeColor.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

No added public appearance classes; use the default theme and control-specific properties.
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

| Type | Action signature | Purpose |
| --- | --- | --- |
| [`OneTheme`](../../api/en/TheOne.Avalonia.Themes.OneTheme.md) | `static void OneTheme.SetColor(ThemeColor color)` | Switches the application's OneTheme palette on the UI thread, preserving Light / Dark and property bindings. |

### Related models

| Model | Properties/enum values |
| --- | ---: |
| [`TheOne.Avalonia.Themes.OneTheme`](../../api/en/TheOne.Avalonia.Themes.OneTheme.md) | 2 |
| [`TheOne.Avalonia.Themes.ThemeColor`](../../api/en/TheOne.Avalonia.Themes.ThemeColor.md) | 10 |

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
