# Text display and field labels

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

The preview is on the combined text / data-display page. `--verify-text-alignment` checks field association, real selection, range correction, and theme / size changes. Copying through the system clipboard still requires verification on the target desktop host. See [basic controls](basic.md) for input controls.

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section lists only Classes, properties, extension APIs/events and related models added by this project, without repeating inherited Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost visibly compares project-added styles and uses Description components for control-specific properties, API signatures and model fields. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

These native controls have no same-name TheOne wrappers; project-added capabilities come from the styles below.

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Style classes and automatic state

| Control | Class | Scope | Effective setters/resources |
| --- | --- | --- | --- |
| Label | `accent` | explicit Classes; combine with density | ^.accent => Foreground={DynamicResource One.Accent}; ^:disabled, ^.muted:disabled, ^.accent:disabled => Foreground={DynamicResource One.DisabledText} |
| Label | `muted` | explicit Classes; combine with density | ^.muted => Foreground={DynamicResource One.Muted}; ^:disabled, ^.muted:disabled, ^.accent:disabled => Foreground={DynamicResource One.DisabledText} |
| Label | `one-large` | automatic template class; do not bind manually | Label.one-large => MinHeight={DynamicResource One.Size.Large.Height}; FontSize=16 |
| Label | `one-normal` | automatic template class; do not bind manually | Label.one-normal => MinHeight={DynamicResource One.Size.Height}; FontSize=14 |
| Label | `one-small` | automatic template class; do not bind manually | Label.one-small => MinHeight={DynamicResource One.Size.Small.Height}; FontSize=12 |
| SelectableTextBlock | `accent` | explicit Classes; combine with density | ^.accent => Foreground={DynamicResource One.Accent}; ^:disabled, ^.muted:disabled, ^.accent:disabled => Foreground={DynamicResource One.DisabledText}; Cursor=Arrow |
| SelectableTextBlock | `muted` | explicit Classes; combine with density | ^.muted => Foreground={DynamicResource One.Muted}; ^:disabled, ^.muted:disabled, ^.accent:disabled => Foreground={DynamicResource One.DisabledText}; Cursor=Arrow |
| SelectableTextBlock | `one-large` | automatic template class; do not bind manually | SelectableTextBlock.one-large => FontSize=16; LineHeight={DynamicResource One.Text.Large.LineHeight} |
| SelectableTextBlock | `one-normal` | automatic template class; do not bind manually | SelectableTextBlock.one-normal => FontSize=14; LineHeight={DynamicResource One.Text.LineHeight} |
| SelectableTextBlock | `one-small` | automatic template class; do not bind manually | SelectableTextBlock.one-small => FontSize=12; LineHeight={DynamicResource One.Text.Small.LineHeight} |
| TextBlock | `flow-state` | automatic template class; do not bind manually | TextBlock.flow-state => Foreground={DynamicResource One.Muted}; TextBlock.flow-state.completed => Foreground={DynamicResource One.Success}; TextBlock.flow-state.current => Foreground={DynamicResource One.Accent}; StackPanel.vertical > StackPanel.flow-title, StackPanel.vertical > TextBlock.flow-state => IsVisible=False |
| TextBlock | `one-color-component-label` | automatic template class; do not bind manually | TextBlock.one-color-component-label, Border.one-color-component-label TextBlock => Foreground={DynamicResource One.Muted}; FontSize=12 |
| TextBlock | `reached` | automatic template class; do not bind manually | TextBlock.reached => Foreground={DynamicResource One.Text}; FontWeight=SemiBold |

`:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without added appearance classes use their default theme and control-specific properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
