# Chart 图表

历史命令行验证入口与消费测试项目已移除，文中的专项覆盖和测量结果为历史记录。请正常启动 PreviewHost，从菜单进入对应控件演示页；目标平台行为需在实际应用中验证。

**简体中文** · [English](../en/charts.md)

图表由 21 个独立绘制控件组成，位于 `TheOne.Avalonia.Controls`；每个控件有自己的具体类型、主题和视觉状态。加载 `OneTheme` 后即可使用，并共享 `ChartSeries` / `ChartPoint` 及其专用派生模型；层级图以 `ChartHierarchyNode` 树作为输入。无需额外图表包。图形、刻度、图例与数据标签直接绘制，不为每个数据点创建 Control 或容器。

`ChartBase`、`CartesianChartBase` 与 `RadialChartBase` 是抽象基础设施。原先的可切换 `Chart` / `Kind` 属性已移除：`<Chart Kind="Line">` 改为 `<LineChart>`，`<Chart Kind="Gauge">` 改为 `<GaugeChart>`，其他类型按下表迁移。`Kind` 仅为具体控件的只读类型标识；动态切换时创建新的具体控件并替换宿主内容，保留共享数据或 ViewModel。PNG 导出快照也创建相同具体类型。

## 图表类型

| 独立控件 | 图形 | 数据 |
| --- | --- | --- |
| `LineChart` | 折线图 | 多个可见系列的 X / Y 坐标 |
| `AreaChart` | 面积图 | 折线与基线之间的填充，多系列不堆叠 |
| `BarChart` | 柱状图 | 多系列并列柱 |
| `HorizontalBarChart` | 横向条形图 | 多系列并列条 |
| `ScatterChart` | 散点图 | 多个可见系列的 X / Y 坐标 |
| `PieChart` | 饼图 | 首个可见非空系列中有效数据点的 Y 值 |
| `DoughnutChart` | 环形图 | 与饼图相同，通过 `InnerRadius` 设置内半径比例 |
| `GaugeChart` | 仪表图，默认半圆，可切换八种样式 | `Minimum`、`Maximum`、`Value`，不使用系列数据 |
| `StackedBarChart` | 堆叠柱状图 | 相同索引的可见系列，正值与负值独立堆叠 |
| `StackedHorizontalBarChart` | 堆叠横向条形图 | 与堆叠柱状图相同，数值沿水平方向排列 |
| `StackedAreaChart` | 堆叠面积图 | 相同索引、相同 X 的可见系列分层填充 |
| `PercentStackedBarChart` | 百分比堆叠柱状图 | 同轴、同索引的正负贡献分别归一化 |
| `PercentStackedHorizontalBarChart` | 百分比堆叠横向条形图 | 与百分比堆叠柱状图相同，数值轴水平排列 |
| `PercentStackedAreaChart` | 百分比堆叠面积图 | 同轴、同索引且 X 对齐的百分比分层填充 |
| `HeatmapChart` | 连续色阶网格热力图 | `HeatmapPoint.X / Y` 定位网格，`Value` 控制颜色，支持负值和稀疏格 |
| `RadarChart` | 多维雷达图 | 按点的集合顺序对应 `RadarAxis`，各维独立范围，多系列叠加 |
| `FunnelChart` | 转化漏斗图 | 原始业务顺序的非负 Y 值，可显式排序，多系列并排 |
| `BoxPlotChart` | 箱线图 | `BoxPlotPoint` 的预计算五数摘要及独立离群值，数值 X 坐标 |
| `CandlestickChart` | K 线图 | `CandlestickPoint` 的 Open / Close / Low / High，数值 X 坐标 |
| `TreemapChart` | 矩形树图 | `ChartHierarchyNode` 叶权重，稳定交替切分布局、下钻和返回 |
| `SunburstChart` | 旭日图 | 同一层级模型，父子扇区和径向层级，下钻和返回 |

新增三种图表的完整属性、模型和示例见[热力图、雷达图与漏斗图](chart-specialized.md)。它们共享选择、数据订阅、主题、三档尺寸和 PNG 导出；首版不使用 Cartesian 视口、堆积、共享 X 提示和抽样属性，不能把它们当成 ECharts 全部坐标系的替代。

第二批详见[箱线图与 K 线图](chart-statistical.md)、[矩形树图与旭日图](chart-hierarchy.md)。统计图支持数值视口；层级图使用 ItemsSource / Root 树导航，不手动设置内部 SeriesSource。四种新控件均有独立预览页。

所有 21 种图表支持 `AnimationStyle="Auto|None|Fade|Reveal"`，也可使用 `animation-fade`、`animation-reveal`、`animation-none` Classes。关闭 Motion、动画开关或设置零时长时立即呈现；PNG 始终导出目标状态。详见[动画与渲染](chart-rendering.md)、[首批性能数据](chart-echarts-performance-20261020.md)及[第二批与预览打开优化](chart-next-batch-performance-20261020.md)。

`ChartPoint.X` / `Y` 为 `double`，`Label` 为可选文本。`ChartSeries.Name` 为图例名称，`ItemsSource` 为 `IList<ChartPoint>`，`IsVisible=False` 隐藏系列，`Brush` 可覆盖该系列的默认颜色。`YAxis` 指定主轴或次轴；可选 `Unit` / `ValueFormat` 覆盖该系列的原始值显示，`null` 时继承所属坐标轴的配置。默认系列颜色来自 TheOne 主题，随 Light / Dark 切换。

## 编译绑定示例

消费项目启用编译绑定，并在视图根元素声明 ViewModel 类型。把下面的控件放在有限高度的 Grid 中；图表会根据可用宽高重新布局：

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:one="using:TheOne.Avalonia.Controls"
             xmlns:vm="using:YourApp.ViewModels"
             x:Class="YourApp.Views.ChartView"
             x:DataType="vm:ChartViewModel"
             x:CompileBindings="True">
  <Grid RowDefinitions="*,Auto" RowSpacing="12">
    <one:LineChart x:Name="RevenueChart" Title="月度营收"
               SeriesSource="{CompiledBinding Revenue}"
               SelectedSeriesIndex="{CompiledBinding SelectedSeriesIndex, Mode=TwoWay}"
               SelectedIndex="{CompiledBinding SelectedIndex, Mode=TwoWay}"
               Viewport="{CompiledBinding Viewport, Mode=TwoWay}"
               IsZoomEnabled="True" IsPanEnabled="True"
               ValueFormat="0.##" Unit=" 万元" MinHeight="260" />
    <TextBlock Grid.Row="1"
               Text="{CompiledBinding SelectedIndex, StringFormat='选中数据索引：{0}'}" />
  </Grid>
</UserControl>
```

对应的最小 ViewModel；视图 code-behind 通过 `DataContext = new ChartViewModel()` 设置实例：

```csharp
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia;
using TheOne.Avalonia.Controls;

namespace YourApp.ViewModels;

public sealed class ChartViewModel : INotifyPropertyChanged
{
    public ObservableCollection<ChartSeries> Revenue { get; } =
    [
        new ChartSeries
        {
            Name = "本年",
            ItemsSource = new ObservableCollection<ChartPoint>
            {
                new(1, 42, "一月"), new(2, 58, "二月"),
                new(3, 51, "三月"), new(4, 76, "四月")
            }
        },
        new ChartSeries
        {
            Name = "上年",
            ItemsSource = new ObservableCollection<ChartPoint>
            {
                new(1, 35, "一月"), new(2, 46, "二月"),
                new(3, 49, "三月"), new(4, 61, "四月")
            }
        }
    ];

    private int _selectedSeriesIndex;
    private int _selectedIndex = -1;
    private Rect _viewport = new(0, 0, 1, 1);
    public Rect Viewport
    {
        get => _viewport;
        set
        {
            if (_viewport == value) return;
            _viewport = value;
            PropertyChanged?.Invoke(this, new(nameof(Viewport)));
        }
    }
    public int SelectedSeriesIndex
    {
        get => _selectedSeriesIndex;
        set
        {
            if (_selectedSeriesIndex == value) return;
            _selectedSeriesIndex = value;
            PropertyChanged?.Invoke(this, new(nameof(SelectedSeriesIndex)));
        }
    }
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (_selectedIndex == value) return;
            _selectedIndex = value;
            PropertyChanged?.Invoke(this, new(nameof(SelectedIndex)));
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
}
```

`AreaChart`、`BarChart`、`HorizontalBarChart` 或 `ScatterChart` 可使用相同系列数据，但分别是独立控件实例。折线／面积／散点图按数值 X 定位，`Label` 出现在对应的 X 位置；柱状／横向条形图按列表索引排列类别，同一索引的多个系列并列显示，类别文字来自第一个可见系列。不同系列应保持相同的类别顺序；不会按 `Label` 或 X 自动合并类别。

坐标图忽略非有限坐标；折线和面积图在 `NaN` / `Infinity` 数据处断开。输入顺序决定折线连接顺序，数据源应按业务需要排序。柱状图与面积图自动范围包含零基线；`YMinimum` / `YMaximum` 可覆盖范围，数据标记按范围裁剪。最终上限不大于下限时显示空状态。

## 按功能查阅

| 需求 | 文档 |
| --- | --- |
| 平滑、堆叠、百分比、双轴、采样 | [坐标图与采样](chart-cartesian.md) |
| 饼图、环形图、仪表指针／刻度 | [径向图](chart-radial.md) |
| 标签、选择、十字准线、联动提示、图例、缩放／平移 | [交互](chart-interaction.md) |
| 更新动画、PNG 导出、范围／堆积／绘制缓存、抽样桶增量维护、Scatter 尾延迟／选择 p99、性能记录、主题与尺寸 | [渲染](chart-rendering.md) |
| Scatter 选择分配、经身份核验的 p99 数据及剩余光栅尾延迟 | [Scatter 选择性能](chart-rendering.md#scatter-selection-p99) |

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### AreaChart

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.AreaChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/AreaChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |

### BarChart

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.BarChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/BarChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |

### CartesianChartBase

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.CartesianChartBase.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### ChartBase

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`AccentBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L297) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 主序列／强调色画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`AnimationDuration`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L52) | `System.TimeSpan` | 可写; OneWay | `TimeSpan.FromMilliseconds(200)` | 数据过渡的持续时间；是否启用动画还受控件开关和 Motion.Enabled 约束。 校验：`value => value >= TimeSpan.Zero && value <= TimeSpan.FromSeconds(10)` |
| [`AnimationProgress`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L66) | `double` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`AnimationStyle`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L59) | `TheOne.Avalonia.Controls.ChartAnimationStyle` | 可写; OneWay | `0 / default` | 首次非空展示 ; 后续数据变化 校验：`value => Enum.IsDefined(value)` 可选值：`Auto`, `None`, `Fade`, `Reveal` |
| [`Background`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L273) | `Avalonia.Media.IBrush?` | 可写; OneWay | `inherited registration` | 背景画刷；主题相关背景使用 DynamicResource，避免固定颜色破坏明暗切换。 |
| [`ChartPadding`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L53) | `double` | 可写; OneWay | `12` | 8 ; 12 ; 16 校验：`value => double.IsFinite(value) && value >= 0` |
| [`CrosshairPosition`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L54) | `Avalonia.Point?` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`DefaultChartHeight`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L47) | `double` | 可写; OneWay | `280` | 320 / 224 ; 400 / 280 ; 480 / 336 校验：`value => double.IsFinite(value) && value > 0` |
| [`DefaultChartWidth`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L40) | `double` | 可写; OneWay | `400` | 320 / 224 ; 400 / 280 ; 480 / 336 校验：`value => double.IsFinite(value) && value > 0` |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L255) | `string` | 可写; OneWay | `"暂无数据"` | `暂无数据` |
| [`FifthBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L321) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 第五序列画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`FontFamily`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L333) | `Avalonia.Media.FontFamily` | 可写; OneWay | `inherited registration` | 文本绘制使用的字体族；字体回退和实际度量由 Avalonia 文本系统处理。 |
| [`FontSize`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L339) | `double` | 可写; OneWay | `inherited registration` | 10 ; 11 ; 12 |
| [`Foreground`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L279) | `Avalonia.Media.IBrush?` | 可写; OneWay | `inherited registration` | 前景画刷；自定义时保持正文、状态与禁用对比度。 |
| [`GaugeBandRatio`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L83) | `double` | 可写; OneWay | `0.18` | 沿用三档主题密度；基础、进度、等级、气压表按结构缩放带宽，可显式调整该比例 校验：`value => double.IsFinite(value) && value > 0 && value <= 0.5` |
| [`GaugeMajorTickCount`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L34) | `int` | 可写; OneWay | `5` | Number of major ticks, including the minimum and maximum. 校验：`value => value >= 2 && value <= 25` |
| [`GaugeMinorTickCount`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L41) | `int` | 可写; OneWay | `2` | Maximum minor marks between consecutive major ticks. Compact gauges reduce them to keep readable spacing. 校验：`value => value >= 0 && value <= 10` |
| [`GridBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L285) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 绘图区网格线画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`HoveredPoints`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L55) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.ChartSelection>` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`InnerRadius`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L207) | `double` | 可写; OneWay | `0.62` | 径向图内半径比例；控制中空区域，保留扇区数值和比例含义。 校验：`value => double.IsFinite(value) && value >= 0 && value < 1` |
| [`IsAnimating`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L65) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsAnimationEnabled`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L46) | `bool` | 可写; OneWay | `true` | 启用该图表的数据过渡；全局 Motion.Enabled=False 时仍关闭动画。 |
| [`IsLegendInteractive`](../../../TheOne.Avalonia/Controls/ChartBase.Legend.cs#L27) | `bool` | 可写; OneWay | `true` | `True`，允许坐标图图例切换系列及键盘操作 |
| [`IsPanEnabled`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L67) | `bool` | 可写; OneWay | `false` | 默认 `False`；启用鼠标左键拖动及键盘平移 |
| [`IsSmooth`](../../../TheOne.Avalonia/Controls/ChartBase.Smoothing.cs#L12) | `bool` | 可写; OneWay | `false` | `False`，折线／普通面积的单调平滑路径 |
| [`IsZoomEnabled`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L60) | `bool` | 可写; OneWay | `false` | 默认 `False`；启用滚轮及键盘缩放 |
| [`Kind`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L163) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |
| [`LegendPageCount`](../../../TheOne.Avalonia/Controls/ChartBase.Legend.cs#L42) | `int` | 只读; CLR / n/a | `accessor-defined / state` | 只读分页状态，页索引从 0 开始 |
| [`LegendPageIndex`](../../../TheOne.Avalonia/Controls/ChartBase.Legend.cs#L33) | `int` | 只读; CLR / n/a | `accessor-defined / state` | 只读分页状态，页索引从 0 开始 |
| [`MarkerRadius`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L59) | `double` | 可写; OneWay | `3.5` | 3 ; 3.5 ; 4.25 校验：`value => double.IsFinite(value) && value > 0` |
| [`Maximum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L231) | `double` | 可写; OneWay | `100` | 主数值轴上界；空值使用自动范围，不通过裁剪更改数据本身。 校验：`double.IsFinite` |
| [`Minimum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L225) | `double` | 可写; OneWay | `0 / default` | 主数值轴下界；空值由数据自动推导，显式范围必须与数据及 Maximum 协调。 校验：`double.IsFinite` |
| [`MutedBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L291) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 次要文字与辅助标记画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`NavigationAxes`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L73) | `TheOne.Avalonia.Controls.ChartNavigationAxes` | 可写; OneWay | `ChartNavigationAxes.Both` | 默认 `Both`；可设为 `None`、`Horizontal`、`Vertical`，限制导航轴 校验：`value => (value & ~ChartNavigationAxes.Both) == 0` 可选值：`None`, `Horizontal`, `Vertical`, `Both` |
| [`QuaternaryBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L315) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 第四序列画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`RenderedPointCount`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L113) | `int` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SamplingActive`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L114) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SamplingMode`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L94) | `TheOne.Avalonia.Controls.ChartSamplingMode` | 可写; OneWay | `ChartSamplingMode.MinMax` | 密集序列的采样算法；可选策略见枚举，采样影响绘制点数，不替换原始数据。 校验：`value => value is ChartSamplingMode.None or ChartSamplingMode.MinMax` 可选值：`None`, `MinMax` |
| [`SamplingPointBudget`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L107) | `int` | 可写; OneWay | `2000` | A per-series sampling target; gap boundaries and selected originals can exceed it. 校验：`value => value >= 8` |
| [`SamplingThreshold`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L100) | `int` | 可写; OneWay | `2000` | 启用采样的点数阈值；结合视口与 SamplingMode 调整大数据绘制成本。 校验：`value => value >= 2` |
| [`SecondaryBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L303) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 第二序列画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`SecondaryUnit`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L60) | `string?` | 可写; OneWay | `null` | 第二数值轴的单位后缀，与主轴单位独立。 |
| [`SecondaryValueFormat`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L54) | `string` | 可写; OneWay | `"0.##"` | 第二数值轴的数值格式字符串；使用符合 .NET 数值格式的值。 |
| [`SecondaryYAxisTitle`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L48) | `string?` | 可写; OneWay | `null` | 第二数值轴标题；不改变序列使用哪一条数值轴。 |
| [`SecondaryYMaximum`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L36) | `double` | 可写; OneWay | `double.NaN` | 第二数值轴上界；空值自动计算，与 SecondaryYMinimum 配合使用。 校验：`value => !double.IsInfinity(value)` |
| [`SecondaryYMinimum`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L30) | `double` | 可写; OneWay | `double.NaN` | 第二数值轴下界；空值自动计算，与 SecondaryYMaximum 配合使用。 校验：`value => !double.IsInfinity(value)` |
| [`SelectedIndex`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L267) | `int` | 可写; TwoWay | `-1` | 零基系列 / 点索引，支持双向绑定；点索引 `-1` 清除选择 |
| [`SelectedSeriesIndex`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L261) | `int` | 可写; TwoWay | `0 / default` | 零基系列 / 点索引，支持双向绑定；点索引 `-1` 清除选择 |
| [`Selection`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L345) | `TheOne.Avalonia.Controls.ChartSelection?` | 只读; OneWay | `computed / state` | 只读 `ChartSelection?`，包含索引以及原始 `Series` / `Point` 引用 |
| [`SelectionThickness`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L65) | `double` | 可写; OneWay | `2` | 1.5 ; 2 ; 2.5 校验：`value => double.IsFinite(value) && value > 0` |
| [`SeriesSource`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L165) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.ChartSeries>?` | 可写; OneWay | `null` | 模型或结果集合；集合及元素类型见签名。动态业务数据需提供通知；只读集合由控件维护，不直接替换。 |
| [`ShowCrosshair`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L42) | `bool` | 可写; OneWay | `false` | `False`，开启坐标图指针十字准线 |
| [`ShowDataLabels`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L195) | `bool` | 可写; OneWay | `false` | `False`，坐标图显示数据值，饼图／环形图显示可容纳的百分比 |
| [`ShowGaugeNeedle`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L21) | `bool` | 可写; OneWay | `false` | 主题按枚举提供默认值；显式 False 隐藏对应元素。气压表的刻度开关同时作用于两套轴 |
| [`ShowGaugeTicks`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L27) | `bool` | 可写; OneWay | `false` | 主题按枚举提供默认值；显式 False 隐藏对应元素。气压表的刻度开关同时作用于两套轴 |
| [`ShowGrid`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L183) | `bool` | 可写; OneWay | `true` | `True`，坐标图网格 |
| [`ShowLegend`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L177) | `bool` | 可写; OneWay | `true` | `True`，坐标图为系列图例，饼图／环形图为类别图例 |
| [`ShowMarkers`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L189) | `bool` | 可写; OneWay | `true` | `True`，折线／面积图的数据标记 |
| [`ShowSecondaryAxis`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L66) | `bool` | 可写; OneWay | `true` | 显示第二数值轴；序列轴归属仍由序列模型配置。 |
| [`SixthBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L327) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 第六序列画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`SliceHoverOffset`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L71) | `double` | 可写; OneWay | `6` | 4 ; 6 ; 8 校验：`value => double.IsFinite(value) && value >= 0` |
| [`SliceHoverScale`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L77) | `double` | 可写; OneWay | `1.025` | 1.02 ; 1.025 ; 1.03 校验：`value => double.IsFinite(value) && value >= 1 && value <= 1.2` |
| [`StrokeThickness`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L201) | `double` | 可写; OneWay | `2.5` | `2.5`，必须大于零且有限 校验：`value => double.IsFinite(value) && value > 0` |
| [`TertiaryBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L309) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 第三序列画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`Title`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L171) | `string?` | 可写; OneWay | `null` | 可选标题 |
| [`TooltipMode`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L48) | `TheOne.Avalonia.Controls.ChartTooltipMode` | 可写; OneWay | `0 / default` | `ChartTooltipMode.Single`，可设为 `Shared` 查看同位置的多个系列 校验：`value => value is ChartTooltipMode.Single or ChartTooltipMode.Shared` 可选值：`Single`, `Shared` |
| [`Unit`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L249) | `string?` | 可写; OneWay | `null` | `0.##` / 空，使用当前区域设置格式化数值 |
| [`Value`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L237) | `double` | 可写; OneWay | `0 / default` | 当前值；单位、范围及空值含义以所属模型和校验条件为准。 校验：`double.IsFinite` |
| [`ValueFormat`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L243) | `string` | 可写; OneWay | `"0.##"` | `0.##` / 空，使用当前区域设置格式化数值 |
| [`Viewport`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L53) | `Avalonia.Rect` | 可写; TwoWay | `new Rect(0, 0, 1, 1)` | A normalized window into full data extents, bounded to [0,1] and at most 1,000× zoom. 校验：`value => double.IsFinite(value.X) && double.IsFinite(value.Y) && double.IsFinite(value.Width) && double.IsFinite(value.Height) && value.Width > 0 && value.Height > 0` |
| [`YAxisTitle`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L42) | `string?` | 可写; OneWay | `null` | 主数值轴标题，参与实际文本度量与轴区域布局。 |
| [`YMaximum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L219) | `double` | 可写; OneWay | `double.NaN` | `NaN` 表示自动范围，可设置有限 Y 轴范围 校验：`value => !double.IsInfinity(value)` |
| [`YMinimum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L213) | `double` | 可写; OneWay | `double.NaN` | `NaN` 表示自动范围，可设置有限 Y 轴范围 校验：`value => !double.IsInfinity(value)` |

### DoughnutChart

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.DoughnutChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/DoughnutChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |

### GaugeChart

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.GaugeChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`GaugeCaption`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L298) | `string?` | 可写; OneWay | `null` | 说明文字默认 null；说明顶端的垂直偏移为半径比例 `[−1, 1]`，NaN 随枚举，与顶部 Title 分开 |
| [`GaugeCaptionOffset`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L278) | `double` | 可写; OneWay | `double.NaN` | 说明文字默认 null；说明顶端的垂直偏移为半径比例 `[−1, 1]`，NaN 随枚举，与顶部 Title 分开 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -1 && value <= 1)` |
| [`GaugeGradeLabelGap`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L292) | `double` | 可写; OneWay | `double.NaN` | 等级标签默认沿切线旋转并保持可读；旋转角 NaN 自动、`[−90, 90]` 显式角度（0 水平）；标签到色带的间距 NaN 随字号、`[0, 100]` 显式像素 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= 0 && value <= 100)` |
| [`GaugeGradeLabelRotation`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L285) | `double` | 可写; OneWay | `double.NaN` | 等级标签默认沿切线旋转并保持可读；旋转角 NaN 自动、`[−90, 90]` 显式角度（0 水平）；标签到色带的间距 NaN 随字号、`[0, 100]` 显式像素 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -90 && value <= 90)` |
| [`GaugeNeedleBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L310) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 显式覆盖指针、主刻度和中心数值颜色；未设置时使用主题或所属色带颜色 |
| [`GaugeNeedleLength`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L253) | `double` | 可写; OneWay | `0.72` | 长度默认半径的 0.72，范围 `(0, 1]`；宽度 NaN 随形状，显式像素值 `(0, 50]` 校验：`value => double.IsFinite(value) && value > 0 && value <= 1` |
| [`GaugeNeedleShape`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L241) | `TheOne.Avalonia.Controls.GaugeNeedleShape` | 可写; OneWay | `0 / default` | Auto 随枚举；Triangle / Line / Marker 单独覆盖指针形状 校验：`Enum.IsDefined` 可选值：`Auto`, `Triangle`, `Line`, `Marker` |
| [`GaugeNeedleWidth`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L259) | `double` | 可写; OneWay | `double.NaN` | 长度默认半径的 0.72，范围 `(0, 1]`；宽度 NaN 随形状，显式像素值 `(0, 50]` 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value > 0 && value <= 50)` |
| [`GaugeSecondaryBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L352) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 气压表副刻度数默认 7（范围 `[2, 12]`），颜色默认 Foreground |
| [`GaugeSecondaryMaximum`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L340) | `double` | 可写; OneWay | `60` | 气压表副刻度范围，默认 0–60；不改变主值或指针所代表的范围。无效副范围隐藏副轴 校验：`double.IsFinite` |
| [`GaugeSecondaryMinimum`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L334) | `double` | 可写; OneWay | `0 / default` | 气压表副刻度范围，默认 0–60；不改变主值或指针所代表的范围。无效副范围隐藏副轴 校验：`double.IsFinite` |
| [`GaugeSecondaryTickCount`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L346) | `int` | 可写; OneWay | `7` | 气压表副刻度数默认 7（范围 `[2, 12]`），颜色默认 Foreground 校验：`value => value >= 2 && value <= 12` |
| [`GaugeSegments`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L328) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.GaugeSegment>?` | 可写; OneWay | `null` | 每个实例默认独立空集合，支持直接 XAML 子项与绑定替换；有序色带列表；每项 Fraction 是相对于 Minimum–Maximum 的累计结束比例 `(0, 1]`，Brush 控制颜色，Label 用于 Grade |
| [`GaugeStartAngle`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L205) | `double` | 可写; OneWay | `double.NaN` | NaN 表示随枚举；起点允许 −360°–360°，顺时针角度 `(0, 360]`。坐标系 0° 指向右、90° 指向下。支持整圆 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -360 && value <= 360)` |
| [`GaugeStyle`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L199) | `TheOne.Avalonia.Controls.GaugeStyle` | 可写; OneWay | `0 / default` | 结构 ; 默认起点 / 顺时针角度 ; 主题默认指针 / 刻度 / 主刻度数 校验：`Enum.IsDefined` 可选值：`Semicircle`, `Basic`, `Speed`, `Progress`, `StageSpeed`, `Grade`, `Temperature`, `Barometer` |
| [`GaugeSweepAngle`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L211) | `double` | 可写; OneWay | `double.NaN` | NaN 表示随枚举；起点允许 −360°–360°，顺时针角度 `(0, 360]`。坐标系 0° 指向右、90° 指向下。支持整圆 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value > 0 && value <= 360)` |
| [`GaugeTickBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L316) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 显式覆盖指针、主刻度和中心数值颜色；未设置时使用主题或所属色带颜色 |
| [`GaugeTickPlacement`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L247) | `TheOne.Avalonia.Controls.GaugeTickPlacement` | 可写; OneWay | `0 / default` | Auto 随枚举；Inside / Outside 覆盖主刻度位置，气压表副刻度保持内侧 校验：`Enum.IsDefined` 可选值：`Auto`, `Inside`, `Outside` |
| [`GaugeTrackBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L304) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 背景轴线／轨道颜色，默认 GridBrush |
| [`GaugeValueBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L322) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 显式覆盖指针、主刻度和中心数值颜色；未设置时使用主题或所属色带颜色 |
| [`GaugeValueFontSize`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L265) | `double` | 可写; OneWay | `double.NaN` | NaN 随枚举；字号 `(0, 200]`，垂直偏移为半径比例 `[−1, 1]`，正数向下。字号按实际空间缩小 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value > 0 && value <= 200)` |
| [`GaugeValueOffset`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L271) | `double` | 可写; OneWay | `double.NaN` | NaN 随枚举；字号 `(0, 200]`，垂直偏移为半径比例 `[−1, 1]`，正数向下。字号按实际空间缩小 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -1 && value <= 1)` |
| [`Kind`](../../../TheOne.Avalonia/Controls/GaugeChart.cs#L9) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |
| [`ShowGaugeAnchor`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L223) | `bool?` | 可写; OneWay | `null` | null 随枚举；控制指针中心锚点，仅指针开启时生效 |
| [`ShowGaugeProgress`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L217) | `bool?` | 可写; OneWay | `null` | null 随枚举；True / False 强制显示／隐藏进度弧 |
| [`ShowGaugeTickLabels`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L235) | `bool` | 可写; OneWay | `true` | 默认 True；分别控制数值和刻度标签，不影响数据 |
| [`ShowGaugeValue`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L229) | `bool` | 可写; OneWay | `true` | 默认 True；分别控制数值和刻度标签，不影响数据 |

### HorizontalBarChart

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.HorizontalBarChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/HorizontalBarChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |

### LineChart

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.LineChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/LineChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |

### PercentStackedAreaChart

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.PercentStackedAreaChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/PercentStackedAreaChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |

### PercentStackedBarChart

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.PercentStackedBarChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/PercentStackedBarChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |

### PercentStackedHorizontalBarChart

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.PercentStackedHorizontalBarChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/PercentStackedHorizontalBarChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |

### PieChart

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.PieChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/PieChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |

### RadialChartBase

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.RadialChartBase.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### ScatterChart

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.ScatterChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/ScatterChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |

### StackedAreaChart

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.StackedAreaChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/StackedAreaChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |

### StackedBarChart

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.StackedBarChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/StackedBarChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |

### StackedHorizontalBarChart

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.StackedHorizontalBarChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/StackedHorizontalBarChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

- `ChartBase`: `Classes="animation-fade"` / `Classes="animation-none"` / `Classes="animation-reveal"`

以上是分别使用的公开类名；悬停、按下、禁用和选中等状态由控件维护，不手工添加模板类。实际外观见 PreviewHost 的标签样例。

### 业务 API

| 类型 | 动作签名 | 用途 |
| --- | --- | --- |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `double? ChartBase.GetStackPercentage(int seriesIndex, int pointIndex)` | A signed share of the matching axis/category/sign total in a percentage stack, or null otherwise. |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `void ChartBase.ExportPng(Stream stream, Size? size = null, double dpi = 96, bool transparentBackground = false)` | Render a stable target-state PNG on the UI thread without altering this chart. Size is logical pixels; DPI scales the output. |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `void ChartBase.ExportPng(string path, Size? size = null, double dpi = 96, bool transparentBackground = false)` | 将当前图表绘制为 PNG；目标尺寸、缩放与输出流参数见签名，调用方负责流生命周期。 |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `IReadOnlyList<ChartSelection> ChartBase.GetSharedDataPoints(Point point)` | Nearest visible numeric X (exact matches per series), or the pointed categorical index. No interpolation. |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `Rect? ChartBase.GetDataLabelBounds(int seriesIndex, int pointIndex)` | Returns the target bounds of a visible numeric data label, or null when it is omitted or labels are disabled. |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `Rect? ChartBase.GetLegendItemBounds(int seriesIndex)` | 当前页系列图例的局部 `Rect?` |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `int? ChartBase.HitTestLegend(Point point)` | 指针下的系列索引；没有命中时为 `null` |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `Rect? ChartBase.GetLegendPageNavigationBounds(bool next)` | 获取下一页（True）或上一页（False）区域的局部 `Rect?` |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `bool ChartBase.PageLegend(int delta)` | 按页移动，返回是否改变当前页 |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `bool ChartBase.ToggleSeriesVisibility(int seriesIndex)` | Toggle model visibility, retaining the series' binding. Independent of input/legend flags. |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `void ChartBase.Zoom(double factor, Point? anchor = null)` | Zoom by a positive factor around fractional coordinates in the current view. Factors above one zoom in. |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `void ChartBase.Pan(double horizontal, double vertical)` | Pan by fractions of the visible spans. Positive deltas move towards larger axis values/category indices. |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `void ChartBase.ResetViewport()` | 恢复完整数据视图 |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `bool ChartBase.BringDataPointIntoView(int seriesIndex, int pointIndex)` | Reveal a valid data mark without changing the current zoom. This explicit operation can move both axes. |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `void ChartBase.RefreshData()` | Refresh a plain IList after modifying its contents. Use observable collections for automatic updates. |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `Point? ChartBase.GetDataPointPosition(int seriesIndex, int pointIndex)` | The center of a visible mark in local coordinates; null for a missing or clipped point. |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `ChartSelection? ChartBase.HitTestDataPoint(Point point)` | Find the data mark under a local-coordinate pointer. |

### 事件订阅

按需订阅下列通知，并在宿主释放时解除订阅；这里只列事件入口，不列路由事件标识字段。

| 类型 | 事件 | 用途 |
| --- | --- | --- |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `ViewportChanged` | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `SelectionChanged` | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

### 关联模型

| 模型 | 属性／枚举值 |
| --- | ---: |
| [`TheOne.Avalonia.Controls.ChartAnimationStyle`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartAnimationStyle.md) | 4 |
| [`TheOne.Avalonia.Controls.ChartHierarchyNode`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartHierarchyNode.md) | 3 |
| [`TheOne.Avalonia.Controls.ChartKind`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartKind.md) | 21 |
| [`TheOne.Avalonia.Controls.ChartNavigationAxes`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartNavigationAxes.md) | 4 |
| [`TheOne.Avalonia.Controls.ChartPoint`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartPoint.md) | 3 |
| [`TheOne.Avalonia.Controls.ChartSamplingMode`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartSamplingMode.md) | 2 |
| [`TheOne.Avalonia.Controls.ChartSelection`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartSelection.md) | 4 |
| [`TheOne.Avalonia.Controls.ChartSelectionChangedEventArgs`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartSelectionChangedEventArgs.md) | 1 |
| [`TheOne.Avalonia.Controls.ChartSeries`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartSeries.md) | 7 |
| [`TheOne.Avalonia.Controls.ChartTooltipMode`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartTooltipMode.md) | 2 |
| [`TheOne.Avalonia.Controls.ChartViewportChangedEventArgs`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartViewportChangedEventArgs.md) | 2 |
| [`TheOne.Avalonia.Controls.ChartYAxis`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartYAxis.md) | 2 |
| [`TheOne.Avalonia.Controls.GaugeNeedleShape`](../../api/zh-cn/TheOne.Avalonia.Controls.GaugeNeedleShape.md) | 4 |
| [`TheOne.Avalonia.Controls.GaugeSegment`](../../api/zh-cn/TheOne.Avalonia.Controls.GaugeSegment.md) | 3 |
| [`TheOne.Avalonia.Controls.GaugeStyle`](../../api/zh-cn/TheOne.Avalonia.Controls.GaugeStyle.md) | 8 |
| [`TheOne.Avalonia.Controls.GaugeTickPlacement`](../../api/zh-cn/TheOne.Avalonia.Controls.GaugeTickPlacement.md) | 3 |

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
