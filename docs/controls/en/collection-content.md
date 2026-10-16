# Carousel and pull-to-refresh

[简体中文](../zh-cn/collection-content.md) · **English**

See [collections](collections.md) for lists, trees, and grids.

## Carousel

```xml
<Grid RowDefinitions="*,Auto" Height="360">
  <Carousel Name="SlidesCarousel" Classes="normal"
            ItemsSource="{Binding Slides}"
            SelectedItem="{Binding ActiveSlide, Mode=TwoWay}"
            IsSwipeEnabled="True" WrapSelection="True">
    <Carousel.ItemTemplate>
      <DataTemplate x:DataType="vm:Slide">
        <StackPanel VerticalAlignment="Center" Spacing="12">
          <TextBlock Text="{Binding Title}" FontSize="24" FontWeight="SemiBold" />
          <TextBlock Text="{Binding Description}" TextWrapping="Wrap" />
        </StackPanel>
      </DataTemplate>
    </Carousel.ItemTemplate>
  </Carousel>
  <TextBlock Grid.Row="1" Text="{Binding ActiveSlide.Title}" />
</Grid>
```

The native VirtualizingCarouselPanel is the default. Supply data models and let ItemTemplate create views on demand; no IsVirtualized property is needed. Provide a bounded viewport. small / normal / large use font sizes 12 / 14 / 16, padding 12 / 16 / 20, and minimum heights 180 / 220 / 260. Local Height, MinHeight, Padding, FontSize, colors, and radius can override these.

Switch programmatically with `SlidesCarousel.Next()`, `Previous()`, or `SelectedIndex`. Focused controls support arrows, Home / End. `WrapSelection=True` loops the ends; `ViewportFraction="0.72"` reveals adjacent pages. Native Carousel ScrollIntoView does not change the current page.

Default page transitions slide horizontally for 200 ms. `styling:Motion.Enabled="False"` disables subsequent theme transitions; consumers can supply PageTransition. `IsSwipeEnabled=True` enables dragging; this theme also enables mouse input in the native recognizer. Set `one:CarouselOptions.IsMouseSwipeEnabled="False"` to retain only touch / pen swiping.

PreviewHost's Carousel page includes a thousand items, looping, dragging, adjacent pages, rapid switching, target navigation, and actual page-pool counts.


## RefreshContainer

```xml
<RefreshContainer Height="420" Classes="normal" IsMouseEnabled="True"
                  RefreshRequested="OnRefreshRequested">
  <ListBox ItemsSource="{Binding Records}" SelectedItem="{Binding SelectedRecord, Mode=TwoWay}"
           BorderThickness="0" CornerRadius="0">
    <ListBox.ItemTemplate>
      <DataTemplate x:DataType="vm:Record">
        <TextBlock Text="{Binding Title}" />
      </DataTemplate>
    </ListBox.ItemTemplate>
  </ListBox>
</RefreshContainer>
```

The container needs a bounded viewport. Embed a collection control with its own virtualization, such as ListBox / DataGrid, without another ScrollViewer that measures it at infinite height. RefreshContainer has a single child and does not automatically virtualize arbitrary Content. PullDirection defaults to TopToBottom and supports other native directions. Mouse pulling defaults to disabled; enable IsMouseEnabled=True. RequestRefresh() provides a button or shortcut entry point.

For asynchronous work, obtain the native deferral and complete it in finally:

```csharp
private async void OnRefreshRequested(object? sender, RefreshRequestedEventArgs args)
{
    var deferral = args.GetDeferral();
    try { await ViewModel.ReloadAsync(); }
    catch (Exception error) { ViewModel.ShowError(error.Message); }
    finally { deferral.Complete(); }
}
```

The application implements ViewModel.ReloadAsync / ShowError and can prevent duplicate requests in business logic. The default Visualizer uses this theme and need not be created explicitly. small / normal / large indicator sizes are 64 / 80 / 96 and icon sizes 18 / 22 / 26. Override icon sizing with `one:RefreshOptions.IndicatorSize`. Custom Visualizer or Content must preserve native loading, rotation, and deferral behavior. Read state through `visualizer.GetValue(RefreshVisualizer.RefreshVisualizerStateProperty)`.

RefreshOptions.UseThemeVisualizerSizing defaults to enabled and handles native container-created Visualizer local sizes. Set False to use native sizes; explicitly supplied Visualizer instances are not managed by this default. RefreshVisualizer composition motion inherits `Motion.Enabled`. False keeps pull displacement and status while stopping rotation, pulsing, and transitions. Re-enabling during refresh resumes animation without another request. Custom Content is supported; `RefreshMotion.UseMotionSettings=False` opts out.

PreviewHost's Refresh page has thousand-item virtual data, real pulling, manual / automatic completion, failure / retry, distant navigation, and container counts. The refresh-motion switch can toggle repeatedly during refresh. Global controls select theme and size; reset restores inherited global motion.

PreviewHost keeps only a RefreshContainer page. Its built-in RefreshVisualizer participates in pulling, refresh, and Motion checks while retaining public themes and custom Visualizer APIs.

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section lists only Classes, properties, extension APIs/events and related models added by this project, without repeating inherited Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost visibly compares project-added styles and uses Description components for control-specific properties, API signatures and model fields. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### CarouselOptions

[API / model reference](../../api/en/TheOne.Avalonia.Controls.CarouselOptions.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IsMouseSwipeEnabled`](../../../TheOne.Avalonia/Controls/CarouselOptions.cs#L11) | `bool` | writable; OneWay; attached | `false` | Allows mouse dragging to swipe the carousel, while touch/native navigation follow Carousel semantics. |

### RefreshMotion

[API / model reference](../../api/en/TheOne.Avalonia.Controls.RefreshMotion.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`UseMotionSettings`](../../../TheOne.Avalonia/Controls/RefreshMotion.cs#L22) | `bool` | writable; OneWay; attached | `false` | Uses shared motion configuration without forcing animation when globally/locally disabled. |

### RefreshOptions

[API / model reference](../../api/en/TheOne.Avalonia.Controls.RefreshOptions.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IndicatorSize`](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L12) | `double` | writable; OneWay; attached; inherits | `22` | Refresh-indicator size in DIPs, independent of the pull threshold. |
| [`UseThemeVisualizerSizing`](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L14) | `bool` | writable; OneWay; attached | `false` | Makes the refresh visualizer follow shared theme density; disable for explicit sizing. |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Style classes and automatic state

| Control | Class | Scope | Effective setters/resources |
| --- | --- | --- | --- |
| Carousel | `one-large` | automatic template class; do not bind manually | Carousel.one-large => FontSize=16; Padding={DynamicResource One.Carousel.Large.Padding}; MinHeight={DynamicResource One.Carousel.Large.MinHeight} |
| Carousel | `one-no-motion` | automatic template class; do not bind manually | ^.one-no-motion => PageTransition={x:Null} |
| Carousel | `one-normal` | automatic template class; do not bind manually | Carousel.one-normal => FontSize=14; Padding={DynamicResource One.Carousel.Padding}; MinHeight={DynamicResource One.Carousel.MinHeight} |
| Carousel | `one-small` | automatic template class; do not bind manually | Carousel.one-small => FontSize=12; Padding={DynamicResource One.Carousel.Small.Padding}; MinHeight={DynamicResource One.Carousel.Small.MinHeight} |
| RefreshContainer | `one-large` | automatic template class; do not bind manually | RefreshContainer.one-large, RefreshVisualizer.one-large => FontSize=16 |
| RefreshContainer | `one-normal` | automatic template class; do not bind manually | RefreshContainer.one-normal, RefreshVisualizer.one-normal => FontSize=14 |
| RefreshContainer | `one-small` | automatic template class; do not bind manually | RefreshContainer.one-small, RefreshVisualizer.one-small => FontSize=12 |
| RefreshVisualizer | `one-large` | automatic template class; do not bind manually | RefreshContainer.one-large, RefreshVisualizer.one-large => FontSize=16; RefreshVisualizer.one-large => Height={DynamicResource One.Refresh.Large.Height}; one:RefreshOptions.IndicatorSize={DynamicResource One.Refresh.Large.IndicatorSize}; RefreshVisualizer.one-large.one-refresh-horizontal => Height=NaN; Width={DynamicResource One.Refresh.Large.Height} |
| RefreshVisualizer | `one-normal` | automatic template class; do not bind manually | RefreshContainer.one-normal, RefreshVisualizer.one-normal => FontSize=14; RefreshVisualizer.one-normal => Height={DynamicResource One.Refresh.Height}; one:RefreshOptions.IndicatorSize={DynamicResource One.Refresh.IndicatorSize}; RefreshVisualizer.one-normal.one-refresh-horizontal => Height=NaN; Width={DynamicResource One.Refresh.Height} |
| RefreshVisualizer | `one-refresh-bottom` | automatic template class; do not bind manually | ^.one-refresh-bottom => BorderThickness=0,1,0,0 |
| RefreshVisualizer | `one-refresh-horizontal` | automatic template class; do not bind manually | ^.one-refresh-horizontal => Height=NaN; Width={DynamicResource One.Refresh.Height} |
| RefreshVisualizer | `one-refresh-left` | automatic template class; do not bind manually | ^.one-refresh-left => BorderThickness=0,0,1,0 |
| RefreshVisualizer | `one-refresh-right` | automatic template class; do not bind manually | ^.one-refresh-right => BorderThickness=1,0,0,0 |
| RefreshVisualizer | `one-small` | automatic template class; do not bind manually | RefreshContainer.one-small, RefreshVisualizer.one-small => FontSize=12; RefreshVisualizer.one-small => Height={DynamicResource One.Refresh.Small.Height}; one:RefreshOptions.IndicatorSize={DynamicResource One.Refresh.Small.IndicatorSize}; RefreshVisualizer.one-small.one-refresh-horizontal => Height=NaN; Width={DynamicResource One.Refresh.Small.Height} |

`:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without added appearance classes use their default theme and control-specific properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.CarouselOptions`](../../api/en/TheOne.Avalonia.Controls.CarouselOptions.md) | Class | 1 | 3 | 0 |
| [`TheOne.Avalonia.Controls.RefreshMotion`](../../api/en/TheOne.Avalonia.Controls.RefreshMotion.md) | Class | 1 | 3 | 0 |
| [`TheOne.Avalonia.Controls.RefreshOptions`](../../api/en/TheOne.Avalonia.Controls.RefreshOptions.md) | Class | 2 | 5 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
