# Buttons, inputs, selection, and feedback

Historical command-line checks and consumer test projects have been removed. Check coverage and measurements in this guide are historical records. Start PreviewHost normally and open the relevant control page; validate target-platform behavior in the consuming application.

[简体中文](../zh-cn/basic.md) · **English**

[Back to control documentation](README.md). The business ViewModel supplies the bound properties and commands below; declare `x:DataType` on the root view.

Appearance extensions include `soft` / `gradient` buttons, `filled` / `underlined` inputs, `filled` date/time controls, and status progress bars. See [theme colors and styles](../../theme/en/theme-colors.md) for supported controls, gradient resources, and choosing a style. Choose one primary appearance per control; size classes can be combined with it.

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

```xml
<RepeatButton Content="持续增加" Command="{Binding IncrementCommand}"
              Delay="400" Interval="80" />
<ToggleButton Classes="segment" Content="网格视图"
              IsChecked="{Binding IsGridView, Mode=TwoWay}" />
```

RepeatButton repeats while held. `segment` provides only a segmented appearance; implement mutual exclusion in business state or with grouped RadioButton controls.

All three buttons center their content horizontally and vertically by default and use actual content dimensions at every size. Set `HorizontalContentAlignment` / `VerticalContentAlignment` explicitly for left or top alignment.

With `ToggleButton.IsThreeState="True"`, click or Space cycles false → true → null → false. The null state uses the Primary surface and a separate short dash at the bottom. soft, gradient, primary, outline, ghost, and other appearances keep that mark. It follows actual text color and remains distinguishable in gradients and disabled states, so partial selection is not conveyed by background color alone.

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
| [`IsLoading`](../../../TheOne.Avalonia/Controls/ButtonLoading.cs#L13) | `bool` | writable; OneWay; attached | `false` | Shows busy feedback; buttons retain size and prevent repeated actions, while containers show a loading layer. The host resets it when work finishes. |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

- `AutoCompleteBox`: `Classes="filled"` / `Classes="underlined"`
- `Button`: `Classes="accent"` / `Classes="danger"` / `Classes="ghost"` / `Classes="gradient"` / `Classes="info"` / `Classes="link"` / `Classes="outline"` / `Classes="primary"` / `Classes="soft"` / `Classes="success"` / `Classes="warning"`
- `ComboBox`: `Classes="filled"` / `Classes="underlined"`
- `NumericUpDown`: `Classes="filled"` / `Classes="underlined"`
- `ProgressBar`: `Classes="danger"` / `Classes="gradient"` / `Classes="info"` / `Classes="success"` / `Classes="warning"`
- `TextBox`: `Classes="clearButton"` / `Classes="error"` / `Classes="filled"` / `Classes="revealPasswordButton"` / `Classes="underlined"`
- `ToggleButton`: `Classes="segment"`
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

No additional business methods; configure the control-specific properties above.

### Related models

| Model | Properties/enum values |
| --- | ---: |
| [`TheOne.Avalonia.Controls.ButtonLoading`](../../api/en/TheOne.Avalonia.Controls.ButtonLoading.md) | 1 |

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
