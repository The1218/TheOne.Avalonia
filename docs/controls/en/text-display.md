# Text display and field labels

Historical command-line checks and consumer test projects have been removed. Check coverage and measurements in this guide are historical records. Start PreviewHost normally and open the relevant control page; validate target-platform behavior in the consuming application.

[简体中文](../zh-cn/text-display.md) · **English**

TextBlock displays ordinary read-only text; SelectableTextBlock displays read-only content that users can select and copy; Label associates a field title with an input control. After loading OneTheme, all three use shared text colors, fonts, and inherited sizing.

```xml
<StackPanel Spacing="8">
  <Label Content="工作区名称" Target="{Binding #WorkspaceName}" />
  <TextBox Name="WorkspaceName" Text="{Binding Name, Mode=TwoWay}" />
  <TextBlock Text="修改后立即生效" Foreground="{DynamicResource One.Muted}" />
  <SelectableTextBlock Text="{Binding Description}" TextWrapping="Wrap" />
</StackPanel>
```

Declare the ViewModel's x:DataType on the view. Label Content / ContentTemplate accepts text or a typed template; Target points to the actual input control. Native access-key and focus behavior handles the association. Use Label for fields that need this input association.

TextBlock and SelectableTextBlock support Text, Inlines, TextWrapping, TextTrimming, FontSize, FontWeight, LineHeight, and TextAlignment. Enable wrapping and provide a finite width for long text. Titles and business emphasis can override fonts and semantic colors directly without wrapper controls.

SelectableTextBlock keeps native SelectionStart / SelectionEnd, SelectedText, pointer selection, and platform copy shortcuts. The native control adjusts selection ranges when text changes; use TextBox for editing. SelectionBrush maps to One.TextSelection. SelectionForegroundBrush defaults to null, preserving each Inline's formatting, and can be overridden locally. Theme changes preserve selection.

Body font sizes for small / normal / large are 12 / 14 / 16 px. Label shares the minimum height of an input row. Explicit FontSize, LineHeight, and other local values take precedence. Avoid a fixed single-line Height for read-only paragraphs, which can clip wrapping or scaled fonts.

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

These native controls have no same-name TheOne wrappers; project-added capabilities come from the styles below.

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

- `Label`: `Classes="accent"` / `Classes="muted"`
- `SelectableTextBlock`: `Classes="accent"` / `Classes="muted"`
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

No additional business methods; configure the control-specific properties above.

### Related models

No additional related data models.

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
