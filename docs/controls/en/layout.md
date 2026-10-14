# Menus and layout

[简体中文](../zh-cn/layout.md) · **English**

[Back to control documentation](README.md). The host implements bound commands and state below.

## Menu and shortcuts

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

InputGesture displays a shortcut hint; bind the same command in Window.KeyBindings for the actual shortcut:

```xml
<Window.KeyBindings>
  <KeyBinding Gesture="Ctrl+N" Command="{Binding NewCommand}" />
  <KeyBinding Gesture="Meta+N" Command="{Binding NewCommand}" />
</Window.KeyBindings>
```

macOS ⌘ is Meta. Menus retain arrow navigation, Enter execution, Esc closure, submenu chains, icon / check / shortcut columns, and long-menu scrolling. Use IsEnabled for disabled items. Native MenuItem ToggleType / IsChecked provide checks; see [ControlGallery.axaml](../../../design/PreviewHost/Verification/Fixtures/LegacyControlGallery.axaml).

## ContextMenu and MenuFlyout

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

Popup commands should bind explicit data contexts rather than finding Window ancestors inside popups. For tree / list context actions that must select first, update business selection in PointerPressed; see [ControlGallery.axaml.cs](../../../design/PreviewHost/Verification/Fixtures/LegacyControlGallery.axaml.cs).

Declare children directly for small static menus. Dynamic / growing menus use ItemsSource, on-demand templates, and models instead of prebuilt MenuItem lists. Menu headings, submenus, context menus, and MenuFlyoutPresenter use virtualized layout. Long popups default to `One.DataFlyoutMaxHeight` (360 px).

## Expander and motion

```xml
<Expander Header="高级设置" IsExpanded="{Binding ShowAdvanced, Mode=TwoWay}"
          ExpandDirection="Down">
  <StackPanel Spacing="8">
    <TextBox PlaceholderText="可选说明" />
    <CheckBox Content="启用高级选项" />
  </StackPanel>
</Expander>
```

ExpandDirection supports Down / Up / Left / Right. The theme continuously changes layout reservation through clipping, supports reversal, and keeps full child layout. Disable with `styling:Motion.Enabled="False"` and the Styling namespace on the root. Defaults: brushes 120 ms, expand 200 ms, collapse 140 ms. The application handles system reduced-motion preferences.

Expander does not bound business lists internally; large lists still need Height / MaxHeight or bounded layout.

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

IsPaneOpen controls the pane; DisplayMode supports native Inline / Overlay / CompactInline / CompactOverlay, and PanePlacement supports Left / Right / Top / Bottom. Vertical placements measure height. Bindable OpenPaneLength / CompactPaneLength override size themes. Main-content reservation stays synchronized; pane content keeps expanded dimensions and clips when collapsed instead of squeezing child controls. Compact mode does not automatically turn business panes into icon menus: bind labels to IsPaneOpen or use NavMenu collapsing.

| Mode | Expanded reservation | Collapsed reservation | Light dismiss |
| --- | --- | --- | --- |
| Inline | OpenPaneLength | 0 | No |
| Overlay | 0, overlays content | 0 | Yes |
| CompactInline | OpenPaneLength | CompactPaneLength | No |
| CompactOverlay | CompactPaneLength; remaining pane overlays content | CompactPaneLength | Yes |

Overlay LightDismissLayer is active only while open; UseLightDismissOverlayMode=True shows its color. Layout synchronizes native IsPaneOpen / TemplateSettings directly, without delayed reservation. small / normal / large open lengths are 220 / 248 / 280; compact lengths 48 / 56 / 64. Override One.SplitView.* in Metrics.axaml.

Large-data panes need bounded SplitView height and native ListBox virtualization. Embedded lists use transparent backgrounds / zero borders for continuous PaneBackground and outer borders; override for separate surfaces. PreviewHost has 1,000 items, last-item navigation, four modes / placements, and icons when collapsed with labels restored on expansion.

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

Native dragging, keyboard resizing, and previews remain. For vertical layout splitting, use RowDefinitions and `ResizeDirection="Rows"` with the splitter in the middle row. Parent Grid must have bounded layout space; set row / column minimums in business layout.

## ScrollViewer, ScrollBar, Separator

```xml
<ScrollViewer Height="240" HorizontalScrollBarVisibility="Disabled"
              VerticalScrollBarVisibility="Auto">
  <TextBlock Text="长篇说明文本" TextWrapping="Wrap" />
</ScrollViewer>
<Separator />
```

ScrollViewer suits ordinary content; its ScrollBar is themed automatically. Standalone ScrollBar can bind Minimum, Maximum, Value, and ViewportSize. For controls already providing virtualized viewports, such as ListBox, TreeView, DataGrid, bound the control itself instead of adding an infinite outer scrolling viewport.

Examples: [LayoutDemoView.axaml](../../../design/PreviewHost/Verification/Fixtures/Views/NavigationLayout/LayoutDemoView.axaml). Interaction checks: [LayoutControlsPreview.cs](../../../design/PreviewHost/Verification/NavigationLayout/LayoutControlsPreview.cs).


## Flyout and FlyoutPresenter

Flyout is popup behavior; FlyoutPresenter is the content container it creates. OneTheme applies its templates without replacing native controls.

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

Size inherits from the trigger by default: fonts 12 / 14 / 16 px and padding 12 / 16 / 20 px. To size independently, add a size class to FlyoutPresenterClasses:

```csharp
var flyout = (Flyout)editButton.Flyout!;
flyout.FlyoutPresenterClasses.Add("large");
// 改变类后，下次打开时由原生 Flyout 应用。
flyout.ShowAt(editButton);
flyout.Hide();
```

Standard mode focuses content; Transient retains trigger focus. Esc, outside clicks, or Hide() close it. Closing supports e.Cancel. Hosts implement apply / cancel / validation; the sample apply button has no business binding yet. FlyoutPresenterTheme based on the native theme overrides appearance. See [theme implementation](../../theme/en/README.md) for resources and examples.

Ordinary long content scrolls with default MaxHeight=480. Large Content still needs a finite-height ListBox or bounded DataGrid rather than unbounded ItemsControl in a popup. PreviewHost uses 10,000 models with real viewport recycling and stable source bindings preserving selection after close.

See [content transitions and notifications](content-feedback.md) and [preview entry points](../../../design/PreviewHost/Program.cs).
