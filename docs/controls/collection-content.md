# 轮播与下拉刷新

列表、树及表格见[集合入口](collections.md)。

## Carousel 内容轮播

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

默认使用官方 VirtualizingCarouselPanel；只传入数据模型，通过 ItemTemplate 按需生成视图，无需 IsVirtualized 属性。为轮播提供有限视口。small / normal / large 分别使用 12 / 14 / 16 字号、12 / 16 / 20 内边距和 180 / 220 / 260 最小高度；本地 Height、MinHeight、Padding、FontSize、颜色和圆角可继续覆盖。

程序切换使用 `SlidesCarousel.Next()`、`Previous()` 或 `SelectedIndex`；控件聚焦后支持方向键、Home / End。`WrapSelection=True` 开启首尾循环，`ViewportFraction="0.72"` 显示相邻页。官方 Carousel 的 ScrollIntoView 不切换当前页。

默认提供 200 ms 横向切页动画，`styling:Motion.Enabled="False"` 关闭主题的后续切页动效，消费项目可自行指定 PageTransition。`IsSwipeEnabled=True` 开启拖动；本库主题同时启用原生识别器的鼠标输入。如只保留触摸／笔滑动，可设置 `one:CarouselOptions.IsMouseSwipeEnabled="False"`。

PreviewHost 的“内容轮播 Carousel”页包含千条数据、循环／拖动／相邻页／快速切换、目标定位与实际页面池数量。


## RefreshContainer 下拉刷新

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

容器必须有有限视口；嵌入 ListBox / DataGrid 等具有自身虚拟化的集合控件，避免再嵌套给集合无限高度的 ScrollViewer。RefreshContainer 本身是单内容容器，不会为任意 Content 自动提供列表虚拟化。默认刷新方向 TopToBottom，支持官方其他 PullDirection。鼠标下拉默认关闭，通过 IsMouseEnabled=True 开启；也可调用 RequestRefresh() 提供按钮或快捷键入口。

异步处理需获取并在 finally 中完成官方 deferral：

```csharp
private async void OnRefreshRequested(object? sender, RefreshRequestedEventArgs args)
{
    var deferral = args.GetDeferral();
    try { await ViewModel.ReloadAsync(); }
    catch (Exception error) { ViewModel.ShowError(error.Message); }
    finally { deferral.Complete(); }
}
```

示例中的 ViewModel.ReloadAsync / ShowError 由消费项目实现；可在业务层防止重复请求。默认 Visualizer 使用本库主题，无需显式创建。small / normal / large 的指示器尺寸为 64 / 80 / 96、图标为 18 / 22 / 26；可以设置 `one:RefreshOptions.IndicatorSize` 覆盖图标尺寸。使用自定义 Visualizer 或 Content 时保留官方加载、旋转和 deferral 行为。状态可通过 `visualizer.GetValue(RefreshVisualizer.RefreshVisualizerStateProperty)` 读取。

RefreshOptions.UseThemeVisualizerSizing 默认启用，处理官方容器创建的 Visualizer 固定局部尺寸；可设为 False 使用原生尺寸。用户显式提供的 Visualizer 不受此默认尺寸管理。RefreshVisualizer 的合成动画遵循可继承的 `Motion.Enabled`，设为 False 保留下拉位移和状态提示，停止旋转、脉冲与过渡；刷新中重新开启会恢复动画，不重复请求。自定义 Content 同样支持；`RefreshMotion.UseMotionSettings=False` 可退出适配。

PreviewHost 的“下拉刷新 Refresh”页提供千条虚拟数据、真实下拉、手动／自动完成、失败重试、远距离定位与容器数。“刷新动效”开关支持在刷新中反复关闭与恢复；全局工具栏切换明暗主题和三档尺寸，重置恢复全局动效继承。

PreviewHost 仅保留 RefreshContainer 页面；RefreshVisualizer 作为其内置指示器参与下拉、刷新和 Motion 切换验证，仍提供公共主题与自定义 Visualizer API。
