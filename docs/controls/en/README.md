# Control documentation

[简体中文](../zh-cn/README.md) · **English**

TheOne.Avalonia provides themes and behaviors for Avalonia's native controls, plus windows, messaging, InfoCard, and chart extensions. Follow the [repository README](../../../README.en.md#quick-start) to reference the library and load `OneTheme`, then use the examples here. Your application views can continue to use native Avalonia controls.

## Documentation

| Document | Controls / capabilities |
| --- | --- |
| [IconButton and button hierarchy](icon-button.md) | Icon properties, Geometry / PathIcon / Material templates, primary and secondary actions, semantic states |
| [Split buttons](split-button.md) | SplitButton, ToggleSplitButton, primary actions, menus, keyboard input, virtualization |
| [Text display](text-display.md) | TextBlock, Label, SelectableTextBlock, selection, copying, field association |
| [Buttons, inputs, selection, and feedback](basic.md) | Button, RepeatButton, ToggleButton, TextBox, CheckBox, RadioButton, ToggleSwitch, ComboBox, AutoCompleteBox, NumericUpDown, ButtonSpinner, Slider, ProgressBar, ToolTip |
| [Dates and times](date-time.md) | Calendar, CalendarDatePicker, DatePicker, TimePicker, formatting, date/time, month, and year input |
| [Lists, trees, tabs, and grids](collections.md) | ItemsControl, ListBox, TreeView, TabControl, TabStrip, Card tabs, DataGrid, recycling, distant navigation |
| [DataGrid extensions](datagrid-extensions.md) | Automatic entity-property filters, combined column filters, right frozen columns, numeric summaries |
| [DataGrid binding and sorting](datagrid-binding.md) | Type inference, registered metadata, compiled editing, sort buttons, virtualization |
| [Loading controls](loading.md) | LoadingIndicator, button loading states, LoadingContainer |
| [Menus and layout](layout.md) | Menu, ContextMenu, MenuFlyout, Flyout, FlyoutPresenter, Expander, SplitView, GridSplitter, ScrollViewer, ScrollBar, Separator |
| [Carousel and refresh](collection-content.md) | Carousel, RefreshContainer, RefreshVisualizer |
| [Content transitions and notifications](content-feedback.md) | TransitioningContentControl, NotificationCard |
| [NavMenu](nav-menu.md) | Navigation, collapsing, cascading menus, search, and motion; follow the feature-specific links |
| [LoginWindow](login-window.md) | Login handoff and shutdown lifecycle |
| [Theme implementation](../../theme/en/README.md) | Resources, templates, animation, verification scope, implementation constraints |
| [Windows and messaging](windows.md) | OneWindow, ThemeButton, LeftContent, RightContent, static notification / Toast / dialog APIs |
| [Charts](charts.md) | Fourteen chart types, stacking, dual axes, sampling, viewport navigation, shared tooltips, legends, smoothing, animation, PNG export, needle gauges |
| [InfoCard](info-card.md) | Header, icons, actions, Content, Footer, compiled templates, three sizes |
| [GanttChart](gantt-chart.md) | Read-only hierarchy, progress and milestones, native virtual scrolling and compiled bindings |
| [RelationGraph](relation-graph.md) | Single-root relation trees, horizontal/vertical layout, default/custom models, compiled ChildrenSource and null regions |
| [FlowStep and FlowNode](flow-step.md) | Horizontal / vertical flow, three states, default and custom content templates |
| [Tag and VerificationStep](tag-verification-step.md) | Icon status tags, automatic forms from model properties, navigation with per-step validation |

ComboBoxItem, ListBoxItem, TreeViewItem, TabItem, MenuItem, calendar cells, and internal DataGrid presenters receive their styles from the theme. Bind models for dynamic collections and let templates create containers on demand.

## Using the examples

Place the XAML snippets in a view that has loaded `OneTheme`. Add these namespaces to the view root when using behavior and sizing properties:

```xml
xmlns:one="using:TheOne.Avalonia.Controls"
xmlns:styling="using:TheOne.Avalonia.Styling"
xmlns:vm="using:YourApp.ViewModels"
```

Views containing `{Binding ...}` need the corresponding `DataContext`. When compiled bindings are enabled, declare `x:DataType="vm:YourViewModel"` on the view root and declare the model type on data templates or styles for binding containers. The consuming application implements the commands, collections, and model properties shown here. Changing properties must notify `INotifyPropertyChanged`; changing collections can use `ObservableCollection<T>`.

## Themes and sizes

```csharp
using Avalonia;
using Avalonia.Styling;

Application.Current!.RequestedThemeVariant = ThemeVariant.Dark;
// ThemeVariant.Light：浅色；ThemeVariant.Default：跟随系统。
```

```xml
<StackPanel Classes="small" Spacing="8">
  <TextBox PlaceholderText="继承小尺寸" />
  <Button Classes="large primary" Content="单独使用大尺寸" />
</StackPanel>
```

You can also set `styling:ControlSizing.Size="Large"` directly; the enum values are `Small` / `Normal` / `Large`. Sizes inherit to descendants. Standard interaction heights are 30 / 36 / 44 px; multiline text and expanded content follow their content layout. Explicit local values such as `Height` and `Padding` take precedence.

Use dynamic semantic resources for custom surfaces and text so their colors follow theme changes:

```xml
<Border Background="{DynamicResource One.Surface}"
        BorderBrush="{DynamicResource One.Line}"
        BorderThickness="1" CornerRadius="12" Padding="16">
  <TextBlock Foreground="{DynamicResource One.Text}" Text="业务内容" />
</Border>
```

Other common resources include `One.Subtle`, `One.Accent`, `One.AccentSoft`, and `One.OnAccent`. See [Colors.axaml](../../../TheOne.Avalonia/Themes/Colors.axaml) and [Metrics.axaml](../../../TheOne.Avalonia/Themes/Metrics.axaml) for all keys. Load a single `OneTheme`; it already includes the Fluent support layer and DataGrid Fluent styles.

## Collections and motion

- Use `ItemsSource` and data templates for growing collections instead of creating many Control instances in advance.
- Give lists a bounded viewport: use a `*` row in a bounded Grid, or set `Height` / `MaxHeight`. Avoid putting large lists in an outer StackPanel or ScrollViewer that measures with infinite height.
- Keep the theme's virtualization panels; DataGrid uses its native row and cell presenters.
- Store expansion and selection state in models or ViewModels when it must persist.
- Set `styling:Motion.Enabled="False"` to disable theme motion in a control subtree, or call `Motion.SetEnabled(control, false)` in code.

Complete runnable examples are in [PreviewHost](../../../design/PreviewHost). The large-data examples expose container counts and distant-item operations.

- [Loading states and LoadingContainer](loading.md): Button / IconButton loading, content overlays, and the Motion switch.
