# SplitButton and ToggleSplitButton

Historical command-line checks and consumer test projects have been removed. Check coverage and measurements in this guide are historical records. Start PreviewHost normally and open the relevant control page; validate target-platform behavior in the consuming application.

[简体中文](../zh-cn/split-button.md) · **English**

Both use native Avalonia controls. The primary area executes Command; the arrow opens Flyout without executing that command. ToggleSplitButton's primary area also toggles its bool IsChecked. The arrow and menu do not change the checked state. It does not provide ToggleButton's bool? three-state behavior.

```xml
<SplitButton Content="导出" Command="{Binding ExportCommand}"
             CommandParameter="PDF" Classes="primary">
  <SplitButton.Flyout>
    <MenuFlyout>
      <MenuItem Header="导出 PDF" Command="{Binding ExportCommand}" CommandParameter="PDF" />
      <MenuItem Header="导出 CSV" Command="{Binding ExportCommand}" CommandParameter="CSV" />
    </MenuFlyout>
  </SplitButton.Flyout>
</SplitButton>
<ToggleSplitButton Content="自动同步" IsChecked="{Binding AutoSync, Mode=TwoWay}">
  <SplitButton.Flyout>
    <MenuFlyout><MenuItem Header="立即同步" Command="{Binding SyncCommand}" /></MenuFlyout>
  </SplitButton.Flyout>
</ToggleSplitButton>
```

Declare the ViewModel's x:DataType; the consumer supplies commands and AutoSync. Content / ContentTemplate, Command / CommandParameter, Flyout, and IsEnabled are native properties. Without a Flyout, the arrow has no content to open. CanExecute and disabled states retain native semantics; do not execute the same command again from Click.

When focused, Enter / Space runs the primary action; F4 or Alt + ↓ opens the menu; Esc closes it and returns focus to the trigger. Each area has its own hover and pressed feedback within a continuous outline. Default, primary, and gradient appearances are supported. small / normal / large inherit minimum heights of 30 / 36 / 44 px; explicit Padding, CornerRadius, and BorderThickness take precedence.

For many options, use MenuFlyout.ItemsSource and typed data templates so native menus create containers on demand instead of pre-creating thousands of MenuItem instances. ItemContainerTheme x:DataType should be the option model, with compiled Command / CommandParameter bindings. Provide an explicit typed data context if Flyout needs a different ViewModel; keep compiled bindings enabled.

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

These native controls have no same-name TheOne wrappers; project-added capabilities come from the styles below.

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

- `SplitButton`: `Classes="accent"` / `Classes="gradient"` / `Classes="primary"`
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

No additional business methods; configure the control-specific properties above.

### Related models

No additional related data models.

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
