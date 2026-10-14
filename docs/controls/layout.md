# 菜单与布局

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

macOS 的 ⌘ 对应 Meta。菜单保留方向键导航、Enter 执行、Esc 关闭、子菜单链、图标 / 勾选 / 快捷键列与长菜单滚动；禁用项使用 IsEnabled。需要勾选时使用官方 MenuItem 的 ToggleType 与 IsChecked，参考 [ControlGallery.axaml](../../design/PreviewHost/Verification/Fixtures/LegacyControlGallery.axaml)。

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

弹层中的命令应绑定到明确的数据上下文，不依赖弹层内查找 Window 祖先。树或列表的右键操作如需先选择目标，由宿主在 PointerPressed 中更新业务选择；参考 [ControlGallery.axaml.cs](../../design/PreviewHost/Verification/Fixtures/LegacyControlGallery.axaml.cs)。

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

完整布局示例见 [LayoutDemoView.axaml](../../design/PreviewHost/Verification/Fixtures/Views/NavigationLayout/LayoutDemoView.axaml)，交互检查见 [LayoutControlsPreview.cs](../../design/PreviewHost/Verification/NavigationLayout/LayoutControlsPreview.cs)。


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

Standard 模式聚焦内容，Transient 保留触发控件焦点；关闭可通过 Esc、点击外部或 Hide()。Closing 支持 e.Cancel。表单应用／取消与校验由宿主实现，以上片段的“应用”按钮尚未绑定业务处理。可用 FlyoutPresenterTheme（BasedOn 原生 FlyoutPresenter 主题）覆盖外观；动态资源及具体示例见[主题实现说明](../theme/README.md)。

普通长内容会按默认 MaxHeight=480 滚动。大数据 Content 仍需使用有限 Height 的 ListBox 或有界 DataGrid；不能仅在弹层中放无限高度 ItemsControl。PreviewHost 使用 10,000 条数据，实际按视口创建与回收容器，并通过稳定的源绑定保留关闭后的选择。

内容过渡与通知见[对应 API](content-feedback.md)。验证范围与命令见[预览专项入口](../../design/PreviewHost/Program.cs)。
