# Buttons, inputs, selection, and feedback

Historical command-line checks and consumer test projects have been removed. Check coverage and measurements in this guide are historical records. Start PreviewHost normally and open the relevant control page; validate target-platform behavior in the consuming application.

[简体中文](../zh-cn/basic.md) · **English**

[Back to control documentation](README.md). The business ViewModel supplies the bound properties and commands below; declare `x:DataType` on the root view.

Appearance extensions include `soft` / `gradient` buttons, `filled` / `underlined` inputs, `filled` date/time controls, and status progress bars. See [theme colors and styles](../../theme/en/theme-colors.md) for supported controls, gradient resources, and choosing a style. Choose one primary appearance per control; size classes can be combined with it.

<!-- visual-guide:start -->
## Appearance and example states

[Capture scope and visual index](visual-index.md). These are actual source-rendered examples; use this page’s usage notes for interaction and binding contracts.

### Button

![Button — Semantic colors and appearances compose independently. The figure shows standalone Classes and loading; combination precedence is described below.](../images/Button-light.png)

Semantic colors and appearances compose independently. The figure shows standalone Classes and loading; combination precedence is described below.

[View Dark comparison](../images/Button-dark.png)

### RepeatButton

![RepeatButton — Shares button colors and appearances while retaining repeat-on-hold behavior; loading retains the button footprint.](../images/RepeatButton-light.png)

Shares button colors and appearances while retaining repeat-on-hold behavior; loading retains the button footprint.

[View Dark comparison](../images/RepeatButton-dark.png)

### ToggleButton

![ToggleButton — Checked state composes with semantic colors and appearance. Samples are checked; this is not a separate color enum.](../images/ToggleButton-light.png)

Checked state composes with semantic colors and appearance. Samples are checked; this is not a separate color enum.

[View Dark comparison](../images/ToggleButton-dark.png)

### TextBox

![TextBox — Compares border, fill, underline and clear/password affordances; actual button visibility also depends on focus.](../images/TextBox-light.png)

Compares border, fill, underline and clear/password affordances; actual button visibility also depends on focus.

[View Dark comparison](../images/TextBox-dark.png)

### CheckBox

![CheckBox — Checked state in the default theme; binding and command contracts remain native.](../images/CheckBox-light.png)

Checked state in the default theme; binding and command contracts remain native.

[View Dark comparison](../images/CheckBox-dark.png)

### RadioButton

![RadioButton — Selected state in the default theme; native grouping owns mutual exclusion.](../images/RadioButton-light.png)

Selected state in the default theme; native grouping owns mutual exclusion.

[View Dark comparison](../images/RadioButton-dark.png)

### ToggleSwitch

![ToggleSwitch — On-state track and thumb, colored by semantic theme resources.](../images/ToggleSwitch-light.png)

On-state track and thumb, colored by semantic theme resources.

[View Dark comparison](../images/ToggleSwitch-dark.png)

### ComboBox

![ComboBox — Compares default, filled and underlined; see the open list for item insets and selection backgrounds.](../images/ComboBox-light.png)

Compares default, filled and underlined; see the open list for item insets and selection backgrounds.

[View Dark comparison](../images/ComboBox-dark.png)

### ComboBox · Open dropdown

![ComboBox · Open dropdown — Actual dropdown: the current item is highlighted, item backgrounds have spacing, and popup content is laid out separately from the field.](../images/ComboBoxPopup-light.png)

Actual dropdown: the current item is highlighted, item backgrounds have spacing, and popup content is laid out separately from the field.

[View Dark comparison](../images/ComboBoxPopup-dark.png)

### AutoCompleteBox

![AutoCompleteBox — The suggestion field uses the same three input appearances; text and source determine the suggestions.](../images/AutoCompleteBox-light.png)

The suggestion field uses the same three input appearances; text and source determine the suggestions.

[View Dark comparison](../images/AutoCompleteBox-dark.png)

### NumericUpDown

![NumericUpDown — Numeric field, spinner buttons and dividers share one appearance; underlined does not add a full outline.](../images/NumericUpDown-light.png)

Numeric field, spinner buttons and dividers share one appearance; underlined does not add a full outline.

[View Dark comparison](../images/NumericUpDown-dark.png)

### ButtonSpinner

![ButtonSpinner — Themed native spinner container with custom content.](../images/ButtonSpinner-light.png)

Themed native spinner container with custom content.

[View Dark comparison](../images/ButtonSpinner-dark.png)

### Slider

![Slider — Filled and remaining track segments stay distinct; native dragging and keyboard behavior are retained.](../images/Slider-light.png)

Filled and remaining track segments stay distinct; native dragging and keyboard behavior are retained.

[View Dark comparison](../images/Slider-dark.png)

### ProgressBar

![ProgressBar — Semantic status colors are compared with the default track. This is determinate progress, not a captured looping animation.](../images/ProgressBar-light.png)

Semantic status colors are compared with the default track. This is determinate progress, not a captured looping animation.

[View Dark comparison](../images/ProgressBar-dark.png)

### ToolTip

![ToolTip — Actual hover tooltip content and themed frame; the tooltip is anchored to its target.](../images/ToolTip-light.png)

Actual hover tooltip content and themed frame; the tooltip is anchored to its target.

[View Dark comparison](../images/ToolTip-dark.png)

<!-- visual-guide:end -->

## Button, RepeatButton, ToggleButton

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
  <Button Content="普通操作" Command="{Binding OpenCommand}" />
  <Button Classes="primary" Content="保存" Command="{Binding SaveCommand}" />
  <Button Classes="ghost" Content="取消" />
  <Button Classes="link" Content="查看详情" />
  <Button Classes="danger" Content="删除" Command="{Binding DeleteCommand}" />
</StackPanel>
```

`primary` marks primary actions; `accent` is a compatibility class. `ghost` is lightweight, `link` resembles a link, and `danger` marks destructive actions. Disable with `IsEnabled="False"`. Appearance classes do not implement confirmations or business logic.

### Button: semantic colors and appearances

Button, RepeatButton, ToggleButton and [IconButton](icon-button.md) support one semantic color plus one appearance. Colors are `primary` (`accent` is an alias), `danger`, `success`, `warning`, and `info`. Appearances are `solid`, `ghost`, `link`, `outline`, `soft`, and `gradient`. Sizing follows the shared Home-page rules.

```xml
<Button Classes="danger outline" Content="Delete" />
<Button Classes="success soft" Content="Completed" />
<Button Classes="warning solid" Content="Needs attention" />
<Button Classes="info link" Content="View details" />
<Button Classes="success gradient" Content="Confirm completion" />
<Button Classes="primary ghost" Content="Cancel" />
```

| Appearance | Normal and interaction behavior |
| --- | --- |
| `solid` | Semantic fill with hover / pressed feedback and contrasting text |
| `outline` | Surface background, semantic text / border; NeutralHover / NeutralPressed feedback retains semantic text / border |
| `soft` | Semantic soft background and text; borderless normally, semantic border on hover / press. Brand soft retains its previous Text foreground while pressed |
| `ghost` / `link` | Transparent background, semantic text, no border; hover / pressed feedback backgrounds |
| `gradient` | Existing Brand gradient for primary; matching semantic gradients for other colors, retained on hover / press |

Without an appearance class, each control retains its legacy defaults. For example, Button primary/accent is filled, while ToggleButton and RepeatButton have different old defaults; use `primary solid` explicitly for a consistent solid appearance. Standalone ghost, link, outline, soft and gradient retain normal / hover / pressed appearances, including ToggleButton checked / null states. Disabled styling uses `One.DisabledSurface`, `One.DisabledText` and `One.Line`; checked / null ToggleButton uses `One.DisabledSelection` to keep selection distinguishable. Inherited disabling, class changes while disabled and re-enabling follow the same rule.

Prefer one class from each group. Conflicts resolve independently of class text / insertion order:

- Color: `info > warning > success > danger > primary/accent`.
- Appearance: `gradient > soft > outline > link > ghost > solid`.
- State: disabled overrides pressed, which overrides hover. Removing an appearance restores the current color's legacy default. `danger outline` and `outline danger` are identical.

Semantic filled / gradient feedback brushes are cached per palette rather than created on pointer events; Light / Dark and ColorTheme changes use current resources. `One.Button.{Success|Info|Warning|Danger}Foreground` supplies filled / gradient text, `One.Button.{Success|Info|Warning|Danger}Hover` / `Pressed` supplies solid feedback, and `One.Button.Gradient.{Success|Info|Warning|Danger}` with optional `Hover` / `Pressed` suffixes supplies gradients. Classes style the template ContentPresenter directly; assigning Background / Foreground on Button itself does not necessarily override the displayed colors.

```xml
<RepeatButton Content="持续增加" Command="{Binding IncrementCommand}"
              Delay="400" Interval="80" />
<ToggleButton Classes="segment" Content="网格视图"
              IsChecked="{Binding IsGridView, Mode=TwoWay}" />
```

RepeatButton repeats while held. `segment` provides only a segmented appearance; implement mutual exclusion in business state or with grouped RadioButton controls.

All three buttons center their content horizontally and vertically by default and use actual content dimensions at every size. Set `HorizontalContentAlignment` / `VerticalContentAlignment` explicitly for left or top alignment.

With `ToggleButton.IsThreeState="True"`, click or Space cycles false → true → null → false. The null state shows a separate bottom dash that follows the actual foreground.

With an explicit semantic color and appearance combination, checked / null states retain that color: solid uses its pressed fill, soft uses a solid semantic fill, gradient uses its pressed gradient, outline uses a semantic soft background and border, and ghost/link use a semantic soft background without a border. Hover and press retain their feedback; null additionally shows the dash. Appearance-only classes retain legacy checked / null styling; the new standalone `solid` opts into brand-colored combinations. Auxiliary classes such as `segment` and sizing keep their meaning.

```xml
<ToggleButton Classes="success soft" Content="Approve"
              IsChecked="{CompiledBinding Approved, Mode=TwoWay}" />
<RepeatButton Classes="danger outline" Content="Decrease continuously"
              Command="{CompiledBinding DecrementCommand}" Delay="400" Interval="80" />
<one:IconButton Classes="info gradient" Icon="{CompiledBinding RefreshIcon}"
                Content="Refresh" Command="{CompiledBinding RefreshCommand}" />
```

Combinations only change template colors. ToggleButton three-state and two-way binding, RepeatButton native hold/release behavior, and IconButton placement, loading replacement, commands and focus remain native. Their three separate PreviewHost pages each show 30 combinations, disabled examples and related interactions, wrapping to narrow viewports.

## TextBox

```xml
<TextBox Classes="clearButton" PlaceholderText="邮箱"
         Text="{Binding Email, Mode=TwoWay}" />
<TextBox Classes="revealPasswordButton" PasswordChar="●"
         Text="{Binding Password, Mode=TwoWay}" />
<TextBox Classes="error" Text="需要修正的值" />
<TextBox IsReadOnly="True" Text="只读说明" />
<TextBox AcceptsReturn="True" TextWrapping="Wrap" MinHeight="96"
         Text="{Binding Notes, Mode=TwoWay}" />
```

`clearButton` adds clearing, `revealPasswordButton` toggles password visibility, and `error` sets explicit error appearance only. Real binding validation uses Avalonia's validation mechanism and also triggers the error theme state. Applications should provide specific error text.

Single-line text, placeholders, and passwords are vertically centered by default, including AutoCompleteBox's embedded input. Multiline input with `AcceptsReturn="True"` defaults to top alignment. Explicit `VerticalContentAlignment` overrides it.

## CheckBox, RadioButton, ToggleSwitch

```xml
<CheckBox Content="接收通知" IsChecked="{Binding Notify, Mode=TwoWay}" />
<CheckBox Content="部分选择" IsThreeState="True" IsChecked="{x:Null}" />
<RadioButton GroupName="ExportFormat" Content="CSV" IsChecked="True" />
<RadioButton GroupName="ExportFormat" Content="JSON" />
<ToggleSwitch OnContent="已启用" OffContent="已停用"
              IsChecked="{Binding Enabled, Mode=TwoWay}" />
```

CheckBox's text row has no background by default; the checkbox itself keeps fill and state marks. Override Background locally if needed. CheckBox supports `bool?` three-state values. RadioButton's text row is transparent, with the circular indicator showing selected / hover / pressed / disabled states; local Background can override it. `GroupName` defines mutually exclusive groups. ToggleSwitch's row is transparent; its track and thumb show on / off and interaction states, with distinguishable colors when disabled. `IsChecked` represents the value, retaining pointer, Space, and thumb dragging.

## ComboBox and AutoCompleteBox

```xml
<ComboBox ItemsSource="{Binding Categories}"
          SelectedItem="{Binding SelectedCategory, Mode=TwoWay}" />
<AutoCompleteBox ItemsSource="{Binding Suggestions}"
                 Text="{Binding SearchText, Mode=TwoWay}"
                 MinimumPrefixLength="1" />
```

`Categories` and `Suggestions` in the example are string collections. Object collections can use ItemTemplate; AutoCompleteBox also needs an object-to-text mapping, such as `ValueMemberBinding`, with an explicit item type for compiled binding. Templates display candidates; the host implements search and remote loading.

Dropdowns and suggestions use the theme's virtualized templates. Supply model collections rather than prebuilt ComboBoxItem instances. `One.DataFlyoutMaxHeight` bounds long popup lists.

## NumericUpDown and ButtonSpinner

```xml
<NumericUpDown Minimum="0" Maximum="100" Increment="1"
               FormatString="0" Value="{Binding Quantity, Mode=TwoWay}" />
```

`Value` is nullable numeric data; matching business properties should generally use `decimal?`. Native spinner buttons and input validation are provided. NumericUpDown already includes ButtonSpinner; for a standalone ButtonSpinner, handle Spin in the host to update its value:

```xml
<ButtonSpinner Spin="OnSpin">
  <TextBlock Text="自定义数值" />
</ButtonSpinner>
```

## Slider, ProgressBar, ToolTip

Slider and ScrollBar templates manage Thumb dragging, capture, and positioning; the theme changes appearance only. Change business values through the parent Value and preserve native Track / Thumb structure in replacement templates.

```xml
<Slider Minimum="0" Maximum="100" TickFrequency="5"
        IsSnapToTickEnabled="True" Value="{Binding Progress, Mode=TwoWay}"
        ToolTip.Tip="拖动或使用方向键调整进度" />
<ProgressBar Minimum="0" Maximum="100" Value="{Binding Progress}" />
<ProgressBar IsIndeterminate="True" />
```

Slider retains native pointer and keyboard operation. ProgressBar displays the same business progress; set `IsIndeterminate` when the total is unknown. Add hints through `ToolTip.Tip` without manually creating popups.

PreviewHost's selection comparison page shows fixed states in all sizes and light / dark themes, with live click, keyboard, three-state, mutual exclusion, switch dragging, disabled, and reset scenarios.

The button comparison page shows sizes, themes, normal / hover / pressed / disabled states, ToggleButton selection, and primary buttons. The live area tests single actions, toggling, pointer / Space hold-to-repeat, stopping on release / disable, and reset.

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### ButtonLoading

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.ButtonLoading.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/ButtonLoading.cs#L12) | `bool` | writable; OneWay; attached | `false` | Shows busy feedback; buttons retain size and prevent repeated actions, while containers show a loading layer. The host resets it when work finishes. |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

- `AutoCompleteBox`: `Classes="filled"` / `Classes="underlined"`
- `Button`: `Classes="accent"` / `Classes="danger"` / `Classes="ghost"` / `Classes="gradient"` / `Classes="info"` / `Classes="link"` / `Classes="outline"` / `Classes="primary"` / `Classes="soft"` / `Classes="solid"` / `Classes="success"` / `Classes="warning"`
- `ComboBox`: `Classes="filled"` / `Classes="underlined"`
- `NumericUpDown`: `Classes="filled"` / `Classes="underlined"`
- `ProgressBar`: `Classes="danger"` / `Classes="gradient"` / `Classes="info"` / `Classes="success"` / `Classes="warning"`
- `RepeatButton`: `Classes="danger"` / `Classes="ghost"` / `Classes="gradient"` / `Classes="info"` / `Classes="link"` / `Classes="outline"` / `Classes="soft"` / `Classes="solid"` / `Classes="success"` / `Classes="warning"`
- `TextBox`: `Classes="clearButton"` / `Classes="error"` / `Classes="filled"` / `Classes="revealPasswordButton"` / `Classes="underlined"`
- `ToggleButton`: `Classes="accent"` / `Classes="danger"` / `Classes="ghost"` / `Classes="gradient"` / `Classes="info"` / `Classes="link"` / `Classes="outline"` / `Classes="primary"` / `Classes="segment"` / `Classes="soft"` / `Classes="solid"` / `Classes="success"` / `Classes="warning"`
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

No additional business methods; configure the control-specific properties above.

### Related models

| Model | Properties/enum values |
| --- | ---: |
| [`TheOne.Avalonia.Controls.ButtonLoading`](../../api/en/TheOne.Avalonia.Controls.ButtonLoading.md) | 1 |

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
