# 文本展示与字段标签

**简体中文** · [English](../en/text-display.md)

TextBlock 用于普通只读文字；SelectableTextBlock 用于需要选择和复制的只读内容；Label 用于关联输入控件的字段标题。三者加载 OneTheme 后使用统一文字色、字体和继承的尺寸。

```xml
<StackPanel Spacing="8">
  <Label Content="工作区名称" Target="{Binding #WorkspaceName}" />
  <TextBox Name="WorkspaceName" Text="{Binding Name, Mode=TwoWay}" />
  <TextBlock Text="修改后立即生效" Foreground="{DynamicResource One.Muted}" />
  <SelectableTextBlock Text="{Binding Description}" TextWrapping="Wrap" />
</StackPanel>
```

视图声明 VM 的 x:DataType。Label 的 Content / ContentTemplate 支持文字或有类型模板，Target 关联实际输入控件；访问键及焦点交给原生机制处理。不要用普通 TextBlock 替代需要输入关联的 Label。

TextBlock 和 SelectableTextBlock 支持 Text、Inlines、TextWrapping、TextTrimming、FontSize、FontWeight、LineHeight、TextAlignment。长文本启用换行并提供有限宽度；标题或业务强调可显式覆盖字体和语义色，不需要另建包装控件。

SelectableTextBlock 保留原生 SelectionStart / SelectionEnd、SelectedText、鼠标选择及平台复制快捷键。文本变化后选择范围由原生控件校正；需要编辑时使用 TextBox。SelectionBrush 映射 One.TextSelection；SelectionForegroundBrush 默认为空，保留各 Inline 的文字格式，也可局部覆盖。明暗切换保留选择范围。

small / normal / large 正文字号为 12 / 14 / 16 px，Label 与输入行共享最小高度。显式 FontSize、LineHeight 等局部值优先。不要为只读段落固定单行 Height，以免换行或字体缩放被裁切。

预览位于文本／数据展示组合页；`--verify-text-alignment` 验证字段关联、实际选择、范围校正和主题／尺寸切换。复制访问系统剪贴板的行为仍需在目标桌面宿主验证。输入控件见[基础控件](basic.md)。
