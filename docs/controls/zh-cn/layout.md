# 菜单与布局

**简体中文** · [English](../en/layout.md)

[返回控件文档](README.md)。以下绑定命令与状态由宿主实现。

## Menu 与快捷键

```xml
<Menu>
  <MenuItem Header="文件">
    <MenuItem Header="新建" Command="{Binding NewCommand}" InputGesture="Ctrl+N" />
    <Separator />
    <MenuItem Header="导出">
      <MenuItem Header="CSV" Command="{Binding ExportCsvCommand}" />
      <MenuItem Header="JSON" Command="{Binding ExportJsonCommand}" />
    </MenuItem>
  </MenuItem>
</Menu>
```

InputGesture 显示快捷键提示；实际快捷键在 Window.KeyBindings 中绑定同一个命令：

```xml
<Window.KeyBindings>
  <KeyBinding Gesture="Ctrl+N" Command="{Binding NewCommand}" />
  <KeyBinding Gesture="Meta+N" Command="{Binding NewCommand}" />
</Window.KeyBindings>
```

macOS 的 ⌘ 对应 Meta。菜单保留方向键导航、Enter 执行、Esc 关闭、子菜单链、图标 / 勾选 / 快捷键列与长菜单滚动；禁用项使用 IsEnabled。需要勾选时使用官方 MenuItem 的 ToggleType 与 IsChecked，参考 [ControlGallery.axaml](../../../design/PreviewHost/Verification/Fixtures/LegacyControlGallery.axaml)。

## ContextMenu 与 MenuFlyout

```xml
<Border Background="{DynamicResource One.Subtle}" Padding="16">
  <Border.ContextMenu>
    <ContextMenu>
      <MenuItem Header="查看" Command="{Binding InspectCommand}" />
      <MenuItem Header="删除" Command="{Binding DeleteCommand}" />
    </ContextMenu>
  </Border.ContextMenu>
  <TextBlock Text="右键打开菜单" />
</Border>

<Button Content="更多操作">
  <Button.Flyout>
    <MenuFlyout>
      <MenuItem Header="刷新" Command="{Binding RefreshCommand}" />
    </MenuFlyout>
  </Button.Flyout>
</Button>
```

弹层中的命令应绑定到明确的数据上下文，不依赖弹层内查找 Window 祖先。树或列表的右键操作如需先选择目标，由宿主在 PointerPressed 中更新业务选择；参考 [ControlGallery.axaml.cs](../../../design/PreviewHost/Verification/Fixtures/LegacyControlGallery.axaml.cs)。

少量固定菜单可像示例声明子项。动态或可增长菜单使用 ItemsSource、按需模板与模型，避免预创建大量 MenuItem；菜单标题、子菜单、右键菜单和 MenuFlyoutPresenter 使用虚拟化布局。长弹层默认高度上限为 `One.DataFlyoutMaxHeight`（360 px）。

## Expander 与动画

```xml
<Expander Header="高级设置" IsExpanded="{Binding ShowAdvanced, Mode=TwoWay}"
          ExpandDirection="Down">
  <StackPanel Spacing="8">
    <TextBox PlaceholderText="可选说明" />
    <CheckBox Content="启用高级选项" />
  </StackPanel>
</Expander>
```

ExpandDirection 支持 Down / Up / Left / Right。主题按裁切比例连续改变布局占位，支持动画中反向，子内容保持完整布局。关闭动画使用 `styling:Motion.Enabled="False"`（根元素添加 Styling 命名空间）。默认画刷过渡为 120 ms，展开为 200 ms，收起为 140 ms；系统减少动画偏好由消费应用处理。

Expander 不替内部业务列表实现无限视口：大列表仍需设置 Height / MaxHeight 或放入有界布局。

## SplitView

```xml
<SplitView Height="420" IsPaneOpen="{Binding IsNavigationOpen, Mode=TwoWay}"
           DisplayMode="CompactInline" PanePlacement="Left"
           OpenPaneLength="248" CompactPaneLength="56">
  <SplitView.Pane>
    <ListBox ItemsSource="{Binding NavigationItems}"
             Background="Transparent" BorderThickness="0" CornerRadius="0" />
  </SplitView.Pane>
  <TextBlock Text="页面内容" />
</SplitView>
```

通过 IsPaneOpen 控制侧栏，DisplayMode 使用官方 Inline / Overlay / CompactInline / CompactOverlay 模式，PanePlacement 支持 Left / Right / Top / Bottom。上下方向按高度布局；OpenPaneLength / CompactPaneLength 可绑定并优先于尺寸主题。主题会同步保留主内容的布局空间，侧栏内容保持展开尺寸，在收起时裁切，避免子控件被挤压变形。不会自动把业务侧栏变成图标菜单：Compact 模式下需要为标签绑定 IsPaneOpen，或使用 NavMenu 的折叠功能。

| 模式 | 展开占位 | 收起占位 | 轻触关闭 |
| --- | --- | --- | --- |
| Inline | OpenPaneLength | 0 | 否 |
| Overlay | 0（覆盖主内容） | 0 | 是 |
| CompactInline | OpenPaneLength | CompactPaneLength | 否 |
| CompactOverlay | CompactPaneLength（其余覆盖主内容） | CompactPaneLength | 是 |

Overlay 的 LightDismissLayer 仅在展开时启用；UseLightDismissOverlayMode=True 时显示遮罩色。该状态布局直接同步原生 IsPaneOpen / TemplateSettings，不引入延迟占位。small / normal / large 的展开长度为 220 / 248 / 280，紧凑长度为 48 / 56 / 64，可通过 Metrics.axaml 的 One.SplitView.* 资源覆盖。

侧栏大数据必须给 SplitView 有限高度，内部 ListBox 保留原生虚拟化面板。内嵌列表使用透明背景和零边框，让 PaneBackground 与 SplitView 外框保持连贯；业务需要独立列表表面时可自行覆盖。PreviewHost 独立页面提供 1,000 项、定位末项、四种模式及四个方向；收起时显示图标，展开恢复标签。

## GridSplitter

```xml
<Grid ColumnDefinitions="*,Auto,2*" Height="360">
  <Border Grid.Column="0" Background="{DynamicResource One.Subtle}" />
  <GridSplitter Grid.Column="1" Width="10"
                HorizontalAlignment="Center" VerticalAlignment="Stretch"
                ResizeDirection="Columns" ResizeBehavior="PreviousAndNext"
                ShowsPreview="True" />
  <Border Grid.Column="2" Background="{DynamicResource One.Surface}" />
</Grid>
```

保留原生拖动、键盘调整与预览。上下分割时使用 RowDefinitions 和 `ResizeDirection="Rows"`，把分隔条放在中间行。父级 Grid 必须有有限布局空间，业务可在行 / 列定义上设置最小尺寸。

## ScrollViewer、ScrollBar、Separator

```xml
<ScrollViewer Height="240" HorizontalScrollBarVisibility="Disabled"
              VerticalScrollBarVisibility="Auto">
  <TextBlock Text="长篇说明文本" TextWrapping="Wrap" />
</ScrollViewer>
<Separator />
```

ScrollViewer 适用于普通内容，内部 ScrollBar 自动使用主题。独立 ScrollBar 可绑定 Minimum、Maximum、Value 与 ViewportSize。对本身已带虚拟化视口的 ListBox、TreeView、DataGrid 等，直接约束控件尺寸，避免再套无限滚动视口。

完整布局示例见 [LayoutDemoView.axaml](../../../design/PreviewHost/Verification/Fixtures/Views/NavigationLayout/LayoutDemoView.axaml)，交互检查见 [LayoutControlsPreview.cs](../../../design/PreviewHost/Verification/NavigationLayout/LayoutControlsPreview.cs)。


## Flyout 与 FlyoutPresenter

Flyout 是弹出行为对象，FlyoutPresenter 是由它创建的内容容器。加载 OneTheme 即会应用 TheOne 模板，无需替换官方控件。

```xml
<Button Content="编辑工作区" Classes="small">
  <Button.Flyout>
    <Flyout Placement="Bottom">
      <StackPanel Width="280" Spacing="12">
        <TextBlock Text="工作区设置" FontWeight="SemiBold" />
        <TextBox PlaceholderText="工作区名称" />
        <CheckBox Content="通知成员" />
        <Button Content="应用" Classes="primary" />
      </StackPanel>
    </Flyout>
  </Button.Flyout>
</Button>
```

默认继承触发控件的尺寸，small / normal / large 对应字号 12 / 14 / 16 px、内边距 12 / 16 / 20 px。需要独立尺寸时，在 Flyout 的 FlyoutPresenterClasses 中加入尺寸类：

```csharp
var flyout = (Flyout)editButton.Flyout!;
flyout.FlyoutPresenterClasses.Add("large");
// 改变类后，下次打开时由原生 Flyout 应用。
flyout.ShowAt(editButton);
flyout.Hide();
```

Standard 模式聚焦内容，Transient 保留触发控件焦点；关闭可通过 Esc、点击外部或 Hide()。Closing 支持 e.Cancel。表单应用／取消与校验由宿主实现，以上片段的“应用”按钮尚未绑定业务处理。可用 FlyoutPresenterTheme（BasedOn 原生 FlyoutPresenter 主题）覆盖外观；动态资源及具体示例见[主题实现说明](../../theme/zh-cn/README.md)。

普通长内容会按默认 MaxHeight=480 滚动。大数据 Content 仍需使用有限 Height 的 ListBox 或有界 DataGrid；不能仅在弹层中放无限高度 ItemsControl。PreviewHost 使用 10,000 条数据，实际按视口创建与回收容器，并通过稳定的源绑定保留关闭后的选择。

内容过渡与通知见[对应 API](content-feedback.md)。验证范围与命令见[预览专项入口](../../../design/PreviewHost/Program.cs)。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节按源码声明列出本库增加的成员；原生控件继承属性仍使用 Avalonia API。注册默认值与主题生效值不同，显式局部值优先。模型、方法、事件、枚举的完整签名可从下表的类型链接查看。PreviewHost 在各控件页直接标明示例的 Classes、尺寸及状态；公共属性、API／事件和模型字段使用 Description 组件说明，包含属性类型、默认值、绑定模式、校验条件及 API 签名。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

本页的原生控件没有同名 TheOne 包装类；额外能力来自以下样式及公共附加属性。

### 公共附加属性

[ControlSizing](../../api/zh-cn/TheOne.Avalonia.Styling.ControlSizing.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Size`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L11) | `TheOne.Avalonia.Styling.ControlSize` | 可写; OneWay; 附加属性; 向后代继承 | `ControlSize.Normal` | Small / Normal / Large 共享密度，向后代继承；显式单控件值或尺寸类可覆盖。 可选值：`Small`, `Normal`, `Large` |

[Motion](../../api/zh-cn/TheOne.Avalonia.Styling.Motion.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | 可写; OneWay; 附加属性; 向后代继承 | `true` | 向后代继承的动效开关；关闭时转入最终布局和状态，保留原生输入。 |

### Style 类与自动状态

| 控件 | 类名 | 使用边界 | 生效 Setter／资源 |
| --- | --- | --- | --- |
| ContextMenu | `one-large` | 模板自动类；不手动绑定 | ToolTip.one-large, TabControl.one-large, TreeView.one-large, ListBox.one-large, Menu.one-large, ContextMenu.one-large, Calendar.one-large => FontSize=16; ContextMenu.one-large => Padding=8 |
| ContextMenu | `one-normal` | 模板自动类；不手动绑定 | ToolTip.one-normal, TabControl.one-normal, TreeView.one-normal, ListBox.one-normal, Menu.one-normal, ContextMenu.one-normal, Calendar.one-normal => FontSize=14; ContextMenu.one-normal => Padding=6 |
| ContextMenu | `one-small` | 模板自动类；不手动绑定 | ToolTip.one-small, TabControl.one-small, TreeView.one-small, ListBox.one-small, Menu.one-small, ContextMenu.one-small, Calendar.one-small => FontSize=12; ContextMenu.one-small => Padding=4 |
| Expander | `one-large` | 模板自动类；不手动绑定 | :is(Button).one-large, ToggleButton.one-large, RepeatButton.one-large, TextBox.one-large, ComboBox.one-large, ComboBoxItem.one-large, AutoCompleteBox.one-large, NumericUpDown.one-large, ButtonSpinner.one-large, CheckBox.one-large, RadioButton.one-large, ToggleSwitch.one-large, ListBoxItem.one-large, TreeViewItem.one-large, MenuItem.one-large, TabItem.one-large, CalendarDatePicker.one-large, DatePicker.one-large, TimePicker.one-large, Expander.one-large => MinHeight={DynamicResource One.Size.Large.Height}; FontSize=16; Expander.one-large /template/ ToggleButton /template/ Border#ExpandCollapseChevronBorder => Width=32; Height=32 |
| Expander | `one-no-motion` | 模板自动类；不手动绑定 | ToggleSwitch.one-no-motion /template/ Thumb, Expander.one-no-motion /template/ Path, TreeViewItem.one-no-motion /template/ Path, one&#124;NavMenuItem.one-no-motion /template/ Path => Transitions={x:Null} |
| Expander | `one-normal` | 模板自动类；不手动绑定 | :is(Button).one-normal, ToggleButton.one-normal, RepeatButton.one-normal, TextBox.one-normal, ComboBox.one-normal, ComboBoxItem.one-normal, AutoCompleteBox.one-normal, NumericUpDown.one-normal, ButtonSpinner.one-normal, CheckBox.one-normal, RadioButton.one-normal, ToggleSwitch.one-normal, ListBoxItem.one-normal, TreeViewItem.one-normal, MenuItem.one-normal, TabItem.one-normal, CalendarDatePicker.one-normal, DatePicker.one-normal, TimePicker.one-normal, Expander.one-normal => MinHeight={DynamicResource One.Size.Height}; FontSize=14; Expander.one-normal /template/ ToggleButton /template/ Border#ExpandCollapseChevronBorder => Width=28; Height=28 |
| Expander | `one-small` | 模板自动类；不手动绑定 | :is(Button).one-small, ToggleButton.one-small, RepeatButton.one-small, TextBox.one-small, ComboBox.one-small, ComboBoxItem.one-small, AutoCompleteBox.one-small, NumericUpDown.one-small, ButtonSpinner.one-small, CheckBox.one-small, RadioButton.one-small, ToggleSwitch.one-small, ListBoxItem.one-small, TreeViewItem.one-small, MenuItem.one-small, TabItem.one-small, CalendarDatePicker.one-small, DatePicker.one-small, TimePicker.one-small, Expander.one-small => MinHeight={DynamicResource One.Size.Small.Height}; FontSize=12; Expander.one-small /template/ ToggleButton /template/ Border#ExpandCollapseChevronBorder => Width=24; Height=24 |
| FlyoutPresenter | `one-colorpicker` | 模板自动类；不手动绑定 | :is(ColorView)[IsVisible=True] /template/ Border#TabBackgroundBorder, FlyoutPresenter.one-colorpicker Border#TabBackgroundBorder => Background={DynamicResource One.Surface}; CornerRadius=8; :is(ColorView)[IsVisible=True] /template/ Border#ContentBackgroundBorder, FlyoutPresenter.one-colorpicker Border#ContentBackgroundBorder => Background={DynamicResource One.Surface}; BorderBrush={DynamicResource One.InputLine}; Margin=0; BorderThickness=1; CornerRadius=8; :is(ColorView)[IsVisible=True] /template/ TabControl#PART_TabControl, FlyoutPresenter.one-colorpicker TabControl#PART_TabControl => Height=NaN; MinHeight=0; Margin=1; Background=Transparent; FlyoutPresenter.one-colorpicker TabControl#PART_TabControl => Width=320; FlyoutPresenter.one-colorpicker => Padding=0; CornerRadius=8; BorderThickness=0; Background={DynamicResource One.Surface}; :is(ColorView)[IsVisible=True] /template/ TabControl#PART_TabControl /template/ ScrollViewer#PART_StandardHeaderScroll, FlyoutPresenter.one-colorpicker TabControl#PART_TabControl /template/ ScrollViewer#PART_StandardHeaderScroll => Background={DynamicResource One.Surface}; Margin=12,8,12,0; :is(ColorView)[IsVisible=True] /template/ TabItem:nth-child(1), FlyoutPresenter.one-colorpicker TabItem:nth-child(1) => Header=Custom; AutomationProperties.Name=Custom color; IsVisible=template/resource; :is(ColorView)[IsVisible=True] /template/ TabItem:nth-child(2), FlyoutPresenter.one-colorpicker TabItem:nth-child(2) => Header=Palette; AutomationProperties.Name=Color palette; :is(ColorView)[IsVisible=True] /template/ TabItem /template/ Border#PART_LayoutRoot, FlyoutPresenter.one-colorpicker TabItem /template/ Border#PART_LayoutRoot => MinHeight=32; MaxHeight=32; Margin=0; :is(ColorView)[IsVisible=True] /template/ TabItem:nth-child(1) > Grid, FlyoutPresenter.one-colorpicker TabItem:nth-child(1) > Grid => Margin=16,12,16,16; :is(ColorView)[IsVisible=True] /template/ p&#124;ColorSpectrum, FlyoutPresenter.one-colorpicker p&#124;ColorSpectrum => Grid.Column=0; Grid.Row=0; IsVisible={Binding $parent[ColorView].IsColorSpectrumVisible}; :is(ColorView)[IsVisible=True] /template/ p&#124;ColorSlider#ColorSpectrumThirdComponentSlider, FlyoutPresenter.one-colorpicker p&#124;ColorSlider#ColorSpectrumThirdComponentSlider => Grid.Column=0; Orientation=Horizontal; HorizontalAlignment=Stretch; VerticalAlignment=Center; Margin=0,14,0,0; IsVisible=template/resource; :is(ColorView)[IsVisible=True] /template/ p&#124;ColorSlider#ColorSpectrumAlphaSlider, FlyoutPresenter.one-colorpicker p&#124;ColorSlider#ColorSpectrumAlphaSlider => Grid.Column=0; Orientation=Horizontal; HorizontalAlignment=Stretch; VerticalAlignment=Center; Margin=0,10,0,0; IsVisible=template/resource; :is(ColorView)[IsVisible=True] /template/ ListBox, FlyoutPresenter.one-colorpicker ListBox => Margin=12,12,12,16; Padding=0; ItemTemplate=template/resource; :is(ColorView)[IsVisible=True] /template/ ListBox ListBoxItem, FlyoutPresenter.one-colorpicker ListBoxItem => Margin=2; Height=24; MinHeight=24; MaxHeight=24; Padding=0; HorizontalContentAlignment=Stretch; VerticalContentAlignment=Stretch; CornerRadius=4; :is(ColorView)[IsVisible=True] /template/ ListBox ListBoxItem /template/ ContentPresenter#PART_ContentPresenter, FlyoutPresenter.one-colorpicker ListBoxItem /template/ ContentPresenter#PART_ContentPresenter => Padding=0; :is(ColorView)[IsVisible=True] /template/ ListBox ListBoxItem Border, FlyoutPresenter.one-colorpicker ListBoxItem Border => CornerRadius=4; :is(ColorView)[IsVisible=True] /template/ ListBox ListBoxItem /template/ Rectangle#BorderRectangle, FlyoutPresenter.one-colorpicker ListBoxItem /template/ Rectangle#BorderRectangle => RadiusX=4; RadiusY=4; StrokeThickness=2; :is(ColorView)[IsVisible=True] /template/ TabItem /template/ Border#PART_LayoutRoot, FlyoutPresenter.one-colorpicker TabItem /template/ Border#PART_LayoutRoot => Background=Transparent; CornerRadius={DynamicResource One.Radius.Control}; :is(ColorView)[IsVisible=True] /template/ TabItem /template/ ContentPresenter#PART_ContentPresenter, FlyoutPresenter.one-colorpicker TabItem /template/ ContentPresenter#PART_ContentPresenter => Foreground={DynamicResource One.Muted}; :is(ColorView) /template/ TabItem:pointerover /template/ Border#PART_LayoutRoot, FlyoutPresenter.one-colorpicker TabItem:pointerover /template/ Border#PART_LayoutRoot => Background={DynamicResource One.NeutralHover}; :is(ColorView) /template/ TabItem:selected /template/ Border#PART_LayoutRoot, FlyoutPresenter.one-colorpicker TabItem:selected /template/ Border#PART_LayoutRoot => Background={DynamicResource One.Subtle}; :is(ColorView) /template/ TabItem:selected /template/ ContentPresenter#PART_ContentPresenter, FlyoutPresenter.one-colorpicker TabItem:selected /template/ ContentPresenter#PART_ContentPresenter => Foreground={DynamicResource One.Accent}; :is(ColorView) /template/ TabItem:selected /template/ Border#PART_SelectedPipe, FlyoutPresenter.one-colorpicker TabItem:selected /template/ Border#PART_SelectedPipe => IsVisible=False; :is(ColorView)[IsVisible=True] /template/ RadioButton /template/ ContentPresenter#PART_ContentPresenter, FlyoutPresenter.one-colorpicker RadioButton /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.Subtle}; Foreground={DynamicResource One.Text}; BorderBrush={DynamicResource One.InputLine}; CornerRadius={DynamicResource One.Radius.Control}; :is(ColorView) /template/ RadioButton:checked /template/ ContentPresenter#PART_ContentPresenter, FlyoutPresenter.one-colorpicker RadioButton:checked /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.AccentSoft}; Foreground={DynamicResource One.Accent}; BorderBrush={DynamicResource One.Accent}; :is(ColorView) /template/ RadioButton:pointerover /template/ ContentPresenter#PART_ContentPresenter, FlyoutPresenter.one-colorpicker RadioButton:pointerover /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.NeutralHover}; :is(ColorView) /template/ TextBox#PART_HexTextBox, :is(ColorView) /template/ NumericUpDown, FlyoutPresenter.one-colorpicker TextBox#PART_HexTextBox, FlyoutPresenter.one-colorpicker NumericUpDown => MinHeight=36; MaxHeight=36; FontSize=14; :is(ColorView) /template/ TextBox#PART_HexTextBox, :is(ColorView) /template/ NumericUpDown, FlyoutPresenter.one-colorpicker TextBox#PART_HexTextBox, FlyoutPresenter.one-colorpicker NumericUpDown => MinHeight=36; MaxHeight=36; FontSize=14; :is(ColorView) /template/ Grid#HexInputGrid > Border, FlyoutPresenter.one-colorpicker Grid#HexInputGrid > Border => MinHeight=36; MaxHeight=36; :is(ColorView)[IsVisible=True] /template/ Grid#HexInputGrid > Border, :is(ColorView)[IsVisible=True] /template/ TabItem:nth-child(3) > Grid > Border, FlyoutPresenter.one-colorpicker Grid#HexInputGrid > Border, FlyoutPresenter.one-colorpicker TabItem:nth-child(3) > Grid > Border => Background={DynamicResource One.Subtle}; BorderBrush={DynamicResource One.InputLine}; :is(ColorView)[IsVisible=True] /template/ Grid#HexInputGrid > Border, :is(ColorView)[IsVisible=True] /template/ TabItem:nth-child(3) > Grid > Border, FlyoutPresenter.one-colorpicker Grid#HexInputGrid > Border, FlyoutPresenter.one-colorpicker TabItem:nth-child(3) > Grid > Border => Background={DynamicResource One.Subtle}; BorderBrush={DynamicResource One.InputLine}; :is(ColorView)[IsVisible=True] /template/ Grid#HexInputGrid > Border TextBlock, :is(ColorView)[IsVisible=True] /template/ TabItem:nth-child(3) > Grid > Border TextBlock, FlyoutPresenter.one-colorpicker Grid#HexInputGrid > Border TextBlock, FlyoutPresenter.one-colorpicker TabItem:nth-child(3) > Grid > Border TextBlock => Foreground={DynamicResource One.Muted}; :is(ColorView)[IsVisible=True] /template/ Grid#HexInputGrid > Border TextBlock, :is(ColorView)[IsVisible=True] /template/ TabItem:nth-child(3) > Grid > Border TextBlock, FlyoutPresenter.one-colorpicker Grid#HexInputGrid > Border TextBlock, FlyoutPresenter.one-colorpicker TabItem:nth-child(3) > Grid > Border TextBlock => Foreground={DynamicResource One.Muted} |
| FlyoutPresenter | `one-colorpicker-large` | 模板自动类；不手动绑定 | :is(ColorView).one-large /template/ TextBox#PART_HexTextBox, :is(ColorView).one-large /template/ NumericUpDown, FlyoutPresenter.one-colorpicker-large TextBox#PART_HexTextBox, FlyoutPresenter.one-colorpicker-large NumericUpDown => MinHeight=44; MaxHeight=44; FontSize=16; :is(ColorView).one-large /template/ TextBox#PART_HexTextBox, :is(ColorView).one-large /template/ NumericUpDown, FlyoutPresenter.one-colorpicker-large TextBox#PART_HexTextBox, FlyoutPresenter.one-colorpicker-large NumericUpDown => MinHeight=44; MaxHeight=44; FontSize=16; :is(ColorView).one-large /template/ Grid#HexInputGrid > Border, FlyoutPresenter.one-colorpicker-large Grid#HexInputGrid > Border => MinHeight=44; MaxHeight=44 |
| FlyoutPresenter | `one-colorpicker-small` | 模板自动类；不手动绑定 | :is(ColorView).one-small /template/ TextBox#PART_HexTextBox, :is(ColorView).one-small /template/ NumericUpDown, FlyoutPresenter.one-colorpicker-small TextBox#PART_HexTextBox, FlyoutPresenter.one-colorpicker-small NumericUpDown => MinHeight=30; MaxHeight=30; FontSize=12; :is(ColorView).one-small /template/ TextBox#PART_HexTextBox, :is(ColorView).one-small /template/ NumericUpDown, FlyoutPresenter.one-colorpicker-small TextBox#PART_HexTextBox, FlyoutPresenter.one-colorpicker-small NumericUpDown => MinHeight=30; MaxHeight=30; FontSize=12; :is(ColorView).one-small /template/ Grid#HexInputGrid > Border, FlyoutPresenter.one-colorpicker-small Grid#HexInputGrid > Border => MinHeight=30; MaxHeight=30 |
| FlyoutPresenter | `one-large` | 模板自动类；不手动绑定 | FlyoutPresenter.one-large => FontSize=16; Padding={DynamicResource One.Flyout.Large.Padding} |
| FlyoutPresenter | `one-normal` | 模板自动类；不手动绑定 | FlyoutPresenter.one-normal => FontSize=14; Padding={DynamicResource One.Flyout.Padding} |
| FlyoutPresenter | `one-small` | 模板自动类；不手动绑定 | FlyoutPresenter.one-small => FontSize=12; Padding={DynamicResource One.Flyout.Small.Padding} |
| GridSplitter | `one-large` | 模板自动类；不手动绑定 | GridSplitter.one-large => MinWidth={DynamicResource One.GridSplitter.Large.Thickness}; MinHeight={DynamicResource One.GridSplitter.Large.Thickness} |
| GridSplitter | `one-normal` | 模板自动类；不手动绑定 | GridSplitter.one-normal => MinWidth={DynamicResource One.GridSplitter.Thickness}; MinHeight={DynamicResource One.GridSplitter.Thickness} |
| GridSplitter | `one-small` | 模板自动类；不手动绑定 | GridSplitter.one-small => MinWidth={DynamicResource One.GridSplitter.Small.Thickness}; MinHeight={DynamicResource One.GridSplitter.Small.Thickness} |
| Menu | `one-large` | 模板自动类；不手动绑定 | ToolTip.one-large, TabControl.one-large, TreeView.one-large, ListBox.one-large, Menu.one-large, ContextMenu.one-large, Calendar.one-large => FontSize=16 |
| Menu | `one-normal` | 模板自动类；不手动绑定 | ToolTip.one-normal, TabControl.one-normal, TreeView.one-normal, ListBox.one-normal, Menu.one-normal, ContextMenu.one-normal, Calendar.one-normal => FontSize=14 |
| Menu | `one-small` | 模板自动类；不手动绑定 | ToolTip.one-small, TabControl.one-small, TreeView.one-small, ListBox.one-small, Menu.one-small, ContextMenu.one-small, Calendar.one-small => FontSize=12 |
| MenuItem | `one-large` | 模板自动类；不手动绑定 | :is(Button).one-large, ToggleButton.one-large, RepeatButton.one-large, TextBox.one-large, ComboBox.one-large, ComboBoxItem.one-large, AutoCompleteBox.one-large, NumericUpDown.one-large, ButtonSpinner.one-large, CheckBox.one-large, RadioButton.one-large, ToggleSwitch.one-large, ListBoxItem.one-large, TreeViewItem.one-large, MenuItem.one-large, TabItem.one-large, CalendarDatePicker.one-large, DatePicker.one-large, TimePicker.one-large, Expander.one-large => MinHeight={DynamicResource One.Size.Large.Height}; FontSize=16; :is(Button).one-large, ToggleButton.one-large, RepeatButton.one-large, TextBox.one-large, ComboBox.one-large, ComboBoxItem.one-large, AutoCompleteBox.one-large, NumericUpDown.one-large, CalendarDatePicker.one-large, ListBoxItem.one-large, MenuItem.one-large => Padding=16,10 |
| MenuItem | `one-normal` | 模板自动类；不手动绑定 | :is(Button).one-normal, ToggleButton.one-normal, RepeatButton.one-normal, TextBox.one-normal, ComboBox.one-normal, ComboBoxItem.one-normal, AutoCompleteBox.one-normal, NumericUpDown.one-normal, ButtonSpinner.one-normal, CheckBox.one-normal, RadioButton.one-normal, ToggleSwitch.one-normal, ListBoxItem.one-normal, TreeViewItem.one-normal, MenuItem.one-normal, TabItem.one-normal, CalendarDatePicker.one-normal, DatePicker.one-normal, TimePicker.one-normal, Expander.one-normal => MinHeight={DynamicResource One.Size.Height}; FontSize=14; :is(Button).one-normal, ToggleButton.one-normal, RepeatButton.one-normal, TextBox.one-normal, ComboBox.one-normal, ComboBoxItem.one-normal, AutoCompleteBox.one-normal, NumericUpDown.one-normal, CalendarDatePicker.one-normal, ListBoxItem.one-normal, MenuItem.one-normal => Padding=12,7 |
| MenuItem | `one-small` | 模板自动类；不手动绑定 | :is(Button).one-small, ToggleButton.one-small, RepeatButton.one-small, TextBox.one-small, ComboBox.one-small, ComboBoxItem.one-small, AutoCompleteBox.one-small, NumericUpDown.one-small, ButtonSpinner.one-small, CheckBox.one-small, RadioButton.one-small, ToggleSwitch.one-small, ListBoxItem.one-small, TreeViewItem.one-small, MenuItem.one-small, TabItem.one-small, CalendarDatePicker.one-small, DatePicker.one-small, TimePicker.one-small, Expander.one-small => MinHeight={DynamicResource One.Size.Small.Height}; FontSize=12; :is(Button).one-small, ToggleButton.one-small, RepeatButton.one-small, TextBox.one-small, ComboBox.one-small, ComboBoxItem.one-small, AutoCompleteBox.one-small, NumericUpDown.one-small, CalendarDatePicker.one-small, ListBoxItem.one-small, MenuItem.one-small => Padding=10,4 |
| SplitView | `one-large` | 模板自动类；不手动绑定 | SplitView.one-large => OpenPaneLength={DynamicResource One.SplitView.Large.OpenPaneLength}; CompactPaneLength={DynamicResource One.SplitView.Large.CompactPaneLength}; Padding=20; FontSize=16 |
| SplitView | `one-normal` | 模板自动类；不手动绑定 | SplitView.one-normal => OpenPaneLength={DynamicResource One.SplitView.OpenPaneLength}; CompactPaneLength={DynamicResource One.SplitView.CompactPaneLength}; Padding=16; FontSize=14 |
| SplitView | `one-small` | 模板自动类；不手动绑定 | SplitView.one-small => OpenPaneLength={DynamicResource One.SplitView.Small.OpenPaneLength}; CompactPaneLength={DynamicResource One.SplitView.Small.CompactPaneLength}; Padding=12; FontSize=12 |

`small` / `normal` / `large` 是公开密度入口；`one-small` / `one-normal` / `one-large` 为自动生成的内部类。`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出额外外观类的控件使用默认主题及其属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
