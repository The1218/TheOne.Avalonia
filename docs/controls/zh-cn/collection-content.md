# 轮播与下拉刷新

**简体中文** · [English](../en/collection-content.md)

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

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节按源码声明列出本库增加的成员；原生控件继承属性仍使用 Avalonia API。注册默认值与主题生效值不同，显式局部值优先。模型、方法、事件、枚举的完整签名可从下表的类型链接查看。PreviewHost 在各控件页直接标明示例的 Classes、尺寸及状态；公共属性、API／事件和模型字段使用 Description 组件说明，包含属性类型、默认值、绑定模式、校验条件及 API 签名。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### CarouselOptions

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.CarouselOptions.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`IsMouseSwipeEnabled`](../../../TheOne.Avalonia/Controls/CarouselOptions.cs#L11) | `bool` | 可写; OneWay; 附加属性 | `false` | 允许鼠标拖动轮播翻页；触摸和原生导航行为仍遵循 Carousel 契约。 |

### RefreshMotion

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.RefreshMotion.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`UseMotionSettings`](../../../TheOne.Avalonia/Controls/RefreshMotion.cs#L22) | `bool` | 可写; OneWay; 附加属性 | `false` | 使用库的共享动效配置；全局关闭或目标控件关闭时不强制播放。 |

### RefreshOptions

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.RefreshOptions.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`IndicatorSize`](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L12) | `double` | 可写; OneWay; 附加属性; 向后代继承 | `22` | 刷新指示器的 DIP 尺寸；不改变触发刷新所需的拉动距离。 |
| [`UseThemeVisualizerSizing`](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L14) | `bool` | 可写; OneWay; 附加属性 | `false` | 让刷新指示器跟随共享主题密度；显式定制时可关闭。 |

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
| Carousel | `one-large` | 模板自动类；不手动绑定 | Carousel.one-large => FontSize=16; Padding={DynamicResource One.Carousel.Large.Padding}; MinHeight={DynamicResource One.Carousel.Large.MinHeight} |
| Carousel | `one-no-motion` | 模板自动类；不手动绑定 | ^.one-no-motion => PageTransition={x:Null} |
| Carousel | `one-normal` | 模板自动类；不手动绑定 | Carousel.one-normal => FontSize=14; Padding={DynamicResource One.Carousel.Padding}; MinHeight={DynamicResource One.Carousel.MinHeight} |
| Carousel | `one-small` | 模板自动类；不手动绑定 | Carousel.one-small => FontSize=12; Padding={DynamicResource One.Carousel.Small.Padding}; MinHeight={DynamicResource One.Carousel.Small.MinHeight} |
| RefreshContainer | `one-large` | 模板自动类；不手动绑定 | RefreshContainer.one-large, RefreshVisualizer.one-large => FontSize=16 |
| RefreshContainer | `one-normal` | 模板自动类；不手动绑定 | RefreshContainer.one-normal, RefreshVisualizer.one-normal => FontSize=14 |
| RefreshContainer | `one-small` | 模板自动类；不手动绑定 | RefreshContainer.one-small, RefreshVisualizer.one-small => FontSize=12 |
| RefreshVisualizer | `one-large` | 模板自动类；不手动绑定 | RefreshContainer.one-large, RefreshVisualizer.one-large => FontSize=16; RefreshVisualizer.one-large => Height={DynamicResource One.Refresh.Large.Height}; one:RefreshOptions.IndicatorSize={DynamicResource One.Refresh.Large.IndicatorSize}; RefreshVisualizer.one-large.one-refresh-horizontal => Height=NaN; Width={DynamicResource One.Refresh.Large.Height} |
| RefreshVisualizer | `one-normal` | 模板自动类；不手动绑定 | RefreshContainer.one-normal, RefreshVisualizer.one-normal => FontSize=14; RefreshVisualizer.one-normal => Height={DynamicResource One.Refresh.Height}; one:RefreshOptions.IndicatorSize={DynamicResource One.Refresh.IndicatorSize}; RefreshVisualizer.one-normal.one-refresh-horizontal => Height=NaN; Width={DynamicResource One.Refresh.Height} |
| RefreshVisualizer | `one-refresh-bottom` | 模板自动类；不手动绑定 | ^.one-refresh-bottom => BorderThickness=0,1,0,0 |
| RefreshVisualizer | `one-refresh-horizontal` | 模板自动类；不手动绑定 | ^.one-refresh-horizontal => Height=NaN; Width={DynamicResource One.Refresh.Height} |
| RefreshVisualizer | `one-refresh-left` | 模板自动类；不手动绑定 | ^.one-refresh-left => BorderThickness=0,0,1,0 |
| RefreshVisualizer | `one-refresh-right` | 模板自动类；不手动绑定 | ^.one-refresh-right => BorderThickness=1,0,0,0 |
| RefreshVisualizer | `one-small` | 模板自动类；不手动绑定 | RefreshContainer.one-small, RefreshVisualizer.one-small => FontSize=12; RefreshVisualizer.one-small => Height={DynamicResource One.Refresh.Small.Height}; one:RefreshOptions.IndicatorSize={DynamicResource One.Refresh.Small.IndicatorSize}; RefreshVisualizer.one-small.one-refresh-horizontal => Height=NaN; Width={DynamicResource One.Refresh.Small.Height} |

`small` / `normal` / `large` 是公开密度入口；`one-small` / `one-normal` / `one-large` 为自动生成的内部类。`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出额外外观类的控件使用默认主题及其属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.CarouselOptions`](../../api/zh-cn/TheOne.Avalonia.Controls.CarouselOptions.md) | Class | 1 | 3 | 0 |
| [`TheOne.Avalonia.Controls.RefreshMotion`](../../api/zh-cn/TheOne.Avalonia.Controls.RefreshMotion.md) | Class | 1 | 3 | 0 |
| [`TheOne.Avalonia.Controls.RefreshOptions`](../../api/zh-cn/TheOne.Avalonia.Controls.RefreshOptions.md) | Class | 2 | 5 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
