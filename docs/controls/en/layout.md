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

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section enumerates declarations added by this library; native inherited members remain Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. Type links provide full model, method, event and enum signatures. PreviewHost's Control Laboratory (top-right action on each page) groups live examples by styles, properties, APIs/events and models; complex models use the complete business samples.

These native controls have no same-name TheOne wrappers; additional behavior comes from the styles and shared attached properties below.

### Shared attached properties

[ControlSizing](../../api/en/TheOne.Avalonia.Styling.ControlSizing.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Size`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L11) | `TheOne.Avalonia.Styling.ControlSize` | writable; OneWay; attached; inherits | `ControlSize.Normal` | Shared Small/Normal/Large density inherited by descendants; explicit local values/classes can override it. Values: `Small`, `Normal`, `Large` |

[Motion](../../api/en/TheOne.Avalonia.Styling.Motion.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | writable; OneWay; attached; inherits | `true` | Inherited motion switch; disabling goes to final layout/state while preserving native input. |

### Style classes and automatic state

| Control | Class | Scope | Effective setters/resources |
| --- | --- | --- | --- |
| ContextMenu | `one-large` | automatic template class; do not bind manually | ToolTip.one-large, TabControl.one-large, TreeView.one-large, ListBox.one-large, Menu.one-large, ContextMenu.one-large, Calendar.one-large => FontSize=16; ContextMenu.one-large => Padding=8 |
| ContextMenu | `one-normal` | automatic template class; do not bind manually | ToolTip.one-normal, TabControl.one-normal, TreeView.one-normal, ListBox.one-normal, Menu.one-normal, ContextMenu.one-normal, Calendar.one-normal => FontSize=14; ContextMenu.one-normal => Padding=6 |
| ContextMenu | `one-small` | automatic template class; do not bind manually | ToolTip.one-small, TabControl.one-small, TreeView.one-small, ListBox.one-small, Menu.one-small, ContextMenu.one-small, Calendar.one-small => FontSize=12; ContextMenu.one-small => Padding=4 |
| Expander | `one-large` | automatic template class; do not bind manually | :is(Button).one-large, ToggleButton.one-large, RepeatButton.one-large, TextBox.one-large, ComboBox.one-large, ComboBoxItem.one-large, AutoCompleteBox.one-large, NumericUpDown.one-large, ButtonSpinner.one-large, CheckBox.one-large, RadioButton.one-large, ToggleSwitch.one-large, ListBoxItem.one-large, TreeViewItem.one-large, MenuItem.one-large, TabItem.one-large, CalendarDatePicker.one-large, DatePicker.one-large, TimePicker.one-large, Expander.one-large => MinHeight={DynamicResource One.Size.Large.Height}; FontSize=16; Expander.one-large /template/ ToggleButton /template/ Border#ExpandCollapseChevronBorder => Width=32; Height=32 |
| Expander | `one-no-motion` | automatic template class; do not bind manually | ToggleSwitch.one-no-motion /template/ Thumb, Expander.one-no-motion /template/ Path, TreeViewItem.one-no-motion /template/ Path, one&#124;NavMenuItem.one-no-motion /template/ Path => Transitions={x:Null} |
| Expander | `one-normal` | automatic template class; do not bind manually | :is(Button).one-normal, ToggleButton.one-normal, RepeatButton.one-normal, TextBox.one-normal, ComboBox.one-normal, ComboBoxItem.one-normal, AutoCompleteBox.one-normal, NumericUpDown.one-normal, ButtonSpinner.one-normal, CheckBox.one-normal, RadioButton.one-normal, ToggleSwitch.one-normal, ListBoxItem.one-normal, TreeViewItem.one-normal, MenuItem.one-normal, TabItem.one-normal, CalendarDatePicker.one-normal, DatePicker.one-normal, TimePicker.one-normal, Expander.one-normal => MinHeight={DynamicResource One.Size.Height}; FontSize=14; Expander.one-normal /template/ ToggleButton /template/ Border#ExpandCollapseChevronBorder => Width=28; Height=28 |
| Expander | `one-small` | automatic template class; do not bind manually | :is(Button).one-small, ToggleButton.one-small, RepeatButton.one-small, TextBox.one-small, ComboBox.one-small, ComboBoxItem.one-small, AutoCompleteBox.one-small, NumericUpDown.one-small, ButtonSpinner.one-small, CheckBox.one-small, RadioButton.one-small, ToggleSwitch.one-small, ListBoxItem.one-small, TreeViewItem.one-small, MenuItem.one-small, TabItem.one-small, CalendarDatePicker.one-small, DatePicker.one-small, TimePicker.one-small, Expander.one-small => MinHeight={DynamicResource One.Size.Small.Height}; FontSize=12; Expander.one-small /template/ ToggleButton /template/ Border#ExpandCollapseChevronBorder => Width=24; Height=24 |
| FlyoutPresenter | `one-colorpicker` | automatic template class; do not bind manually | :is(ColorView)[IsVisible=True] /template/ Border#TabBackgroundBorder, FlyoutPresenter.one-colorpicker Border#TabBackgroundBorder => Background={DynamicResource One.Surface}; CornerRadius=8; :is(ColorView)[IsVisible=True] /template/ Border#ContentBackgroundBorder, FlyoutPresenter.one-colorpicker Border#ContentBackgroundBorder => Background={DynamicResource One.Surface}; BorderBrush={DynamicResource One.InputLine}; Margin=0; BorderThickness=1; CornerRadius=8; :is(ColorView)[IsVisible=True] /template/ TabControl#PART_TabControl, FlyoutPresenter.one-colorpicker TabControl#PART_TabControl => Height=NaN; MinHeight=0; Margin=1; Background=Transparent; FlyoutPresenter.one-colorpicker TabControl#PART_TabControl => Width=320; FlyoutPresenter.one-colorpicker => Padding=0; CornerRadius=8; BorderThickness=0; Background={DynamicResource One.Surface}; :is(ColorView)[IsVisible=True] /template/ TabControl#PART_TabControl /template/ ScrollViewer#PART_StandardHeaderScroll, FlyoutPresenter.one-colorpicker TabControl#PART_TabControl /template/ ScrollViewer#PART_StandardHeaderScroll => Background={DynamicResource One.Surface}; Margin=12,8,12,0; :is(ColorView)[IsVisible=True] /template/ TabItem:nth-child(1), FlyoutPresenter.one-colorpicker TabItem:nth-child(1) => Header=Custom; AutomationProperties.Name=Custom color; IsVisible=template/resource; :is(ColorView)[IsVisible=True] /template/ TabItem:nth-child(2), FlyoutPresenter.one-colorpicker TabItem:nth-child(2) => Header=Palette; AutomationProperties.Name=Color palette; :is(ColorView)[IsVisible=True] /template/ TabItem /template/ Border#PART_LayoutRoot, FlyoutPresenter.one-colorpicker TabItem /template/ Border#PART_LayoutRoot => MinHeight=32; MaxHeight=32; Margin=0; :is(ColorView)[IsVisible=True] /template/ TabItem:nth-child(1) > Grid, FlyoutPresenter.one-colorpicker TabItem:nth-child(1) > Grid => Margin=16,12,16,16; :is(ColorView)[IsVisible=True] /template/ p&#124;ColorSpectrum, FlyoutPresenter.one-colorpicker p&#124;ColorSpectrum => Grid.Column=0; Grid.Row=0; IsVisible={Binding $parent[ColorView].IsColorSpectrumVisible}; :is(ColorView)[IsVisible=True] /template/ p&#124;ColorSlider#ColorSpectrumThirdComponentSlider, FlyoutPresenter.one-colorpicker p&#124;ColorSlider#ColorSpectrumThirdComponentSlider => Grid.Column=0; Orientation=Horizontal; HorizontalAlignment=Stretch; VerticalAlignment=Center; Margin=0,14,0,0; IsVisible=template/resource; :is(ColorView)[IsVisible=True] /template/ p&#124;ColorSlider#ColorSpectrumAlphaSlider, FlyoutPresenter.one-colorpicker p&#124;ColorSlider#ColorSpectrumAlphaSlider => Grid.Column=0; Orientation=Horizontal; HorizontalAlignment=Stretch; VerticalAlignment=Center; Margin=0,10,0,0; IsVisible=template/resource; :is(ColorView)[IsVisible=True] /template/ ListBox, FlyoutPresenter.one-colorpicker ListBox => Margin=12,12,12,16; Padding=0; ItemTemplate=template/resource; :is(ColorView)[IsVisible=True] /template/ ListBox ListBoxItem, FlyoutPresenter.one-colorpicker ListBoxItem => Margin=2; Height=24; MinHeight=24; MaxHeight=24; Padding=0; HorizontalContentAlignment=Stretch; VerticalContentAlignment=Stretch; CornerRadius=4; :is(ColorView)[IsVisible=True] /template/ ListBox ListBoxItem /template/ ContentPresenter#PART_ContentPresenter, FlyoutPresenter.one-colorpicker ListBoxItem /template/ ContentPresenter#PART_ContentPresenter => Padding=0; :is(ColorView)[IsVisible=True] /template/ ListBox ListBoxItem Border, FlyoutPresenter.one-colorpicker ListBoxItem Border => CornerRadius=4; :is(ColorView)[IsVisible=True] /template/ ListBox ListBoxItem /template/ Rectangle#BorderRectangle, FlyoutPresenter.one-colorpicker ListBoxItem /template/ Rectangle#BorderRectangle => RadiusX=4; RadiusY=4; StrokeThickness=2; :is(ColorView)[IsVisible=True] /template/ TabItem /template/ Border#PART_LayoutRoot, FlyoutPresenter.one-colorpicker TabItem /template/ Border#PART_LayoutRoot => Background=Transparent; CornerRadius={DynamicResource One.Radius.Control}; :is(ColorView)[IsVisible=True] /template/ TabItem /template/ ContentPresenter#PART_ContentPresenter, FlyoutPresenter.one-colorpicker TabItem /template/ ContentPresenter#PART_ContentPresenter => Foreground={DynamicResource One.Muted}; :is(ColorView) /template/ TabItem:pointerover /template/ Border#PART_LayoutRoot, FlyoutPresenter.one-colorpicker TabItem:pointerover /template/ Border#PART_LayoutRoot => Background={DynamicResource One.NeutralHover}; :is(ColorView) /template/ TabItem:selected /template/ Border#PART_LayoutRoot, FlyoutPresenter.one-colorpicker TabItem:selected /template/ Border#PART_LayoutRoot => Background={DynamicResource One.Subtle}; :is(ColorView) /template/ TabItem:selected /template/ ContentPresenter#PART_ContentPresenter, FlyoutPresenter.one-colorpicker TabItem:selected /template/ ContentPresenter#PART_ContentPresenter => Foreground={DynamicResource One.Accent}; :is(ColorView) /template/ TabItem:selected /template/ Border#PART_SelectedPipe, FlyoutPresenter.one-colorpicker TabItem:selected /template/ Border#PART_SelectedPipe => IsVisible=False; :is(ColorView)[IsVisible=True] /template/ RadioButton /template/ ContentPresenter#PART_ContentPresenter, FlyoutPresenter.one-colorpicker RadioButton /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.Subtle}; Foreground={DynamicResource One.Text}; BorderBrush={DynamicResource One.InputLine}; CornerRadius={DynamicResource One.Radius.Control}; :is(ColorView) /template/ RadioButton:checked /template/ ContentPresenter#PART_ContentPresenter, FlyoutPresenter.one-colorpicker RadioButton:checked /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.AccentSoft}; Foreground={DynamicResource One.Accent}; BorderBrush={DynamicResource One.Accent}; :is(ColorView) /template/ RadioButton:pointerover /template/ ContentPresenter#PART_ContentPresenter, FlyoutPresenter.one-colorpicker RadioButton:pointerover /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.NeutralHover}; :is(ColorView) /template/ TextBox#PART_HexTextBox, :is(ColorView) /template/ NumericUpDown, FlyoutPresenter.one-colorpicker TextBox#PART_HexTextBox, FlyoutPresenter.one-colorpicker NumericUpDown => MinHeight=36; MaxHeight=36; FontSize=14; :is(ColorView) /template/ TextBox#PART_HexTextBox, :is(ColorView) /template/ NumericUpDown, FlyoutPresenter.one-colorpicker TextBox#PART_HexTextBox, FlyoutPresenter.one-colorpicker NumericUpDown => MinHeight=36; MaxHeight=36; FontSize=14; :is(ColorView) /template/ Grid#HexInputGrid > Border, FlyoutPresenter.one-colorpicker Grid#HexInputGrid > Border => MinHeight=36; MaxHeight=36; :is(ColorView)[IsVisible=True] /template/ Grid#HexInputGrid > Border, :is(ColorView)[IsVisible=True] /template/ TabItem:nth-child(3) > Grid > Border, FlyoutPresenter.one-colorpicker Grid#HexInputGrid > Border, FlyoutPresenter.one-colorpicker TabItem:nth-child(3) > Grid > Border => Background={DynamicResource One.Subtle}; BorderBrush={DynamicResource One.InputLine}; :is(ColorView)[IsVisible=True] /template/ Grid#HexInputGrid > Border, :is(ColorView)[IsVisible=True] /template/ TabItem:nth-child(3) > Grid > Border, FlyoutPresenter.one-colorpicker Grid#HexInputGrid > Border, FlyoutPresenter.one-colorpicker TabItem:nth-child(3) > Grid > Border => Background={DynamicResource One.Subtle}; BorderBrush={DynamicResource One.InputLine}; :is(ColorView)[IsVisible=True] /template/ Grid#HexInputGrid > Border TextBlock, :is(ColorView)[IsVisible=True] /template/ TabItem:nth-child(3) > Grid > Border TextBlock, FlyoutPresenter.one-colorpicker Grid#HexInputGrid > Border TextBlock, FlyoutPresenter.one-colorpicker TabItem:nth-child(3) > Grid > Border TextBlock => Foreground={DynamicResource One.Muted}; :is(ColorView)[IsVisible=True] /template/ Grid#HexInputGrid > Border TextBlock, :is(ColorView)[IsVisible=True] /template/ TabItem:nth-child(3) > Grid > Border TextBlock, FlyoutPresenter.one-colorpicker Grid#HexInputGrid > Border TextBlock, FlyoutPresenter.one-colorpicker TabItem:nth-child(3) > Grid > Border TextBlock => Foreground={DynamicResource One.Muted} |
| FlyoutPresenter | `one-colorpicker-large` | automatic template class; do not bind manually | :is(ColorView).one-large /template/ TextBox#PART_HexTextBox, :is(ColorView).one-large /template/ NumericUpDown, FlyoutPresenter.one-colorpicker-large TextBox#PART_HexTextBox, FlyoutPresenter.one-colorpicker-large NumericUpDown => MinHeight=44; MaxHeight=44; FontSize=16; :is(ColorView).one-large /template/ TextBox#PART_HexTextBox, :is(ColorView).one-large /template/ NumericUpDown, FlyoutPresenter.one-colorpicker-large TextBox#PART_HexTextBox, FlyoutPresenter.one-colorpicker-large NumericUpDown => MinHeight=44; MaxHeight=44; FontSize=16; :is(ColorView).one-large /template/ Grid#HexInputGrid > Border, FlyoutPresenter.one-colorpicker-large Grid#HexInputGrid > Border => MinHeight=44; MaxHeight=44 |
| FlyoutPresenter | `one-colorpicker-small` | automatic template class; do not bind manually | :is(ColorView).one-small /template/ TextBox#PART_HexTextBox, :is(ColorView).one-small /template/ NumericUpDown, FlyoutPresenter.one-colorpicker-small TextBox#PART_HexTextBox, FlyoutPresenter.one-colorpicker-small NumericUpDown => MinHeight=30; MaxHeight=30; FontSize=12; :is(ColorView).one-small /template/ TextBox#PART_HexTextBox, :is(ColorView).one-small /template/ NumericUpDown, FlyoutPresenter.one-colorpicker-small TextBox#PART_HexTextBox, FlyoutPresenter.one-colorpicker-small NumericUpDown => MinHeight=30; MaxHeight=30; FontSize=12; :is(ColorView).one-small /template/ Grid#HexInputGrid > Border, FlyoutPresenter.one-colorpicker-small Grid#HexInputGrid > Border => MinHeight=30; MaxHeight=30 |
| FlyoutPresenter | `one-large` | automatic template class; do not bind manually | FlyoutPresenter.one-large => FontSize=16; Padding={DynamicResource One.Flyout.Large.Padding} |
| FlyoutPresenter | `one-normal` | automatic template class; do not bind manually | FlyoutPresenter.one-normal => FontSize=14; Padding={DynamicResource One.Flyout.Padding} |
| FlyoutPresenter | `one-small` | automatic template class; do not bind manually | FlyoutPresenter.one-small => FontSize=12; Padding={DynamicResource One.Flyout.Small.Padding} |
| GridSplitter | `one-large` | automatic template class; do not bind manually | GridSplitter.one-large => MinWidth={DynamicResource One.GridSplitter.Large.Thickness}; MinHeight={DynamicResource One.GridSplitter.Large.Thickness} |
| GridSplitter | `one-normal` | automatic template class; do not bind manually | GridSplitter.one-normal => MinWidth={DynamicResource One.GridSplitter.Thickness}; MinHeight={DynamicResource One.GridSplitter.Thickness} |
| GridSplitter | `one-small` | automatic template class; do not bind manually | GridSplitter.one-small => MinWidth={DynamicResource One.GridSplitter.Small.Thickness}; MinHeight={DynamicResource One.GridSplitter.Small.Thickness} |
| Menu | `one-large` | automatic template class; do not bind manually | ToolTip.one-large, TabControl.one-large, TreeView.one-large, ListBox.one-large, Menu.one-large, ContextMenu.one-large, Calendar.one-large => FontSize=16 |
| Menu | `one-normal` | automatic template class; do not bind manually | ToolTip.one-normal, TabControl.one-normal, TreeView.one-normal, ListBox.one-normal, Menu.one-normal, ContextMenu.one-normal, Calendar.one-normal => FontSize=14 |
| Menu | `one-small` | automatic template class; do not bind manually | ToolTip.one-small, TabControl.one-small, TreeView.one-small, ListBox.one-small, Menu.one-small, ContextMenu.one-small, Calendar.one-small => FontSize=12 |
| MenuItem | `one-large` | automatic template class; do not bind manually | :is(Button).one-large, ToggleButton.one-large, RepeatButton.one-large, TextBox.one-large, ComboBox.one-large, ComboBoxItem.one-large, AutoCompleteBox.one-large, NumericUpDown.one-large, ButtonSpinner.one-large, CheckBox.one-large, RadioButton.one-large, ToggleSwitch.one-large, ListBoxItem.one-large, TreeViewItem.one-large, MenuItem.one-large, TabItem.one-large, CalendarDatePicker.one-large, DatePicker.one-large, TimePicker.one-large, Expander.one-large => MinHeight={DynamicResource One.Size.Large.Height}; FontSize=16; :is(Button).one-large, ToggleButton.one-large, RepeatButton.one-large, TextBox.one-large, ComboBox.one-large, ComboBoxItem.one-large, AutoCompleteBox.one-large, NumericUpDown.one-large, CalendarDatePicker.one-large, ListBoxItem.one-large, MenuItem.one-large => Padding=16,10 |
| MenuItem | `one-normal` | automatic template class; do not bind manually | :is(Button).one-normal, ToggleButton.one-normal, RepeatButton.one-normal, TextBox.one-normal, ComboBox.one-normal, ComboBoxItem.one-normal, AutoCompleteBox.one-normal, NumericUpDown.one-normal, ButtonSpinner.one-normal, CheckBox.one-normal, RadioButton.one-normal, ToggleSwitch.one-normal, ListBoxItem.one-normal, TreeViewItem.one-normal, MenuItem.one-normal, TabItem.one-normal, CalendarDatePicker.one-normal, DatePicker.one-normal, TimePicker.one-normal, Expander.one-normal => MinHeight={DynamicResource One.Size.Height}; FontSize=14; :is(Button).one-normal, ToggleButton.one-normal, RepeatButton.one-normal, TextBox.one-normal, ComboBox.one-normal, ComboBoxItem.one-normal, AutoCompleteBox.one-normal, NumericUpDown.one-normal, CalendarDatePicker.one-normal, ListBoxItem.one-normal, MenuItem.one-normal => Padding=12,7 |
| MenuItem | `one-small` | automatic template class; do not bind manually | :is(Button).one-small, ToggleButton.one-small, RepeatButton.one-small, TextBox.one-small, ComboBox.one-small, ComboBoxItem.one-small, AutoCompleteBox.one-small, NumericUpDown.one-small, ButtonSpinner.one-small, CheckBox.one-small, RadioButton.one-small, ToggleSwitch.one-small, ListBoxItem.one-small, TreeViewItem.one-small, MenuItem.one-small, TabItem.one-small, CalendarDatePicker.one-small, DatePicker.one-small, TimePicker.one-small, Expander.one-small => MinHeight={DynamicResource One.Size.Small.Height}; FontSize=12; :is(Button).one-small, ToggleButton.one-small, RepeatButton.one-small, TextBox.one-small, ComboBox.one-small, ComboBoxItem.one-small, AutoCompleteBox.one-small, NumericUpDown.one-small, CalendarDatePicker.one-small, ListBoxItem.one-small, MenuItem.one-small => Padding=10,4 |
| SplitView | `one-large` | automatic template class; do not bind manually | SplitView.one-large => OpenPaneLength={DynamicResource One.SplitView.Large.OpenPaneLength}; CompactPaneLength={DynamicResource One.SplitView.Large.CompactPaneLength}; Padding=20; FontSize=16 |
| SplitView | `one-normal` | automatic template class; do not bind manually | SplitView.one-normal => OpenPaneLength={DynamicResource One.SplitView.OpenPaneLength}; CompactPaneLength={DynamicResource One.SplitView.CompactPaneLength}; Padding=16; FontSize=14 |
| SplitView | `one-small` | automatic template class; do not bind manually | SplitView.one-small => OpenPaneLength={DynamicResource One.SplitView.Small.OpenPaneLength}; CompactPaneLength={DynamicResource One.SplitView.Small.CompactPaneLength}; Padding=12; FontSize=12 |

`small` / `normal` / `large` are public density inputs; `one-small` / `one-normal` / `one-large` are automatic internal classes. `:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without additional appearance classes use their default theme and properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
