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

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节只列出本项目增加的 Classes、属性、扩展 API／事件及关联模型，不重复 Avalonia 原生继承 API。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用可见示例对照本项目附加样式，用 Description 组件说明专属属性、API 签名与模型字段；ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

本页的原生控件没有同名 TheOne 包装类；本项目增加的能力来自下列样式。

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Style 类与自动状态

| 控件 | 类名 | 使用边界 | 生效 Setter／资源 |
| --- | --- | --- | --- |
| Label | `accent` | 显式 Classes，可与尺寸类组合 | ^.accent => Foreground={DynamicResource One.Accent}; ^:disabled, ^.muted:disabled, ^.accent:disabled => Foreground={DynamicResource One.DisabledText} |
| Label | `muted` | 显式 Classes，可与尺寸类组合 | ^.muted => Foreground={DynamicResource One.Muted}; ^:disabled, ^.muted:disabled, ^.accent:disabled => Foreground={DynamicResource One.DisabledText} |
| Label | `one-large` | 模板自动类；不手动绑定 | Label.one-large => MinHeight={DynamicResource One.Size.Large.Height}; FontSize=16 |
| Label | `one-normal` | 模板自动类；不手动绑定 | Label.one-normal => MinHeight={DynamicResource One.Size.Height}; FontSize=14 |
| Label | `one-small` | 模板自动类；不手动绑定 | Label.one-small => MinHeight={DynamicResource One.Size.Small.Height}; FontSize=12 |
| SelectableTextBlock | `accent` | 显式 Classes，可与尺寸类组合 | ^.accent => Foreground={DynamicResource One.Accent}; ^:disabled, ^.muted:disabled, ^.accent:disabled => Foreground={DynamicResource One.DisabledText}; Cursor=Arrow |
| SelectableTextBlock | `muted` | 显式 Classes，可与尺寸类组合 | ^.muted => Foreground={DynamicResource One.Muted}; ^:disabled, ^.muted:disabled, ^.accent:disabled => Foreground={DynamicResource One.DisabledText}; Cursor=Arrow |
| SelectableTextBlock | `one-large` | 模板自动类；不手动绑定 | SelectableTextBlock.one-large => FontSize=16; LineHeight={DynamicResource One.Text.Large.LineHeight} |
| SelectableTextBlock | `one-normal` | 模板自动类；不手动绑定 | SelectableTextBlock.one-normal => FontSize=14; LineHeight={DynamicResource One.Text.LineHeight} |
| SelectableTextBlock | `one-small` | 模板自动类；不手动绑定 | SelectableTextBlock.one-small => FontSize=12; LineHeight={DynamicResource One.Text.Small.LineHeight} |
| TextBlock | `flow-state` | 模板自动类；不手动绑定 | TextBlock.flow-state => Foreground={DynamicResource One.Muted}; TextBlock.flow-state.completed => Foreground={DynamicResource One.Success}; TextBlock.flow-state.current => Foreground={DynamicResource One.Accent}; StackPanel.vertical > StackPanel.flow-title, StackPanel.vertical > TextBlock.flow-state => IsVisible=False |
| TextBlock | `one-color-component-label` | 模板自动类；不手动绑定 | TextBlock.one-color-component-label, Border.one-color-component-label TextBlock => Foreground={DynamicResource One.Muted}; FontSize=12 |
| TextBlock | `reached` | 模板自动类；不手动绑定 | TextBlock.reached => Foreground={DynamicResource One.Text}; FontWeight=SemiBold |

`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出附加外观类的控件使用默认主题及其专属属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
