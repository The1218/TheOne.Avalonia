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
