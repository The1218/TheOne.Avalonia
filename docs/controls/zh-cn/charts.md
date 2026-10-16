# Chart 图表

**简体中文** · [English](../en/charts.md)

图表由 14 个独立绘制控件组成，位于 `TheOne.Avalonia.Controls`；每个控件有自己的具体类型、主题和视觉状态。加载 `OneTheme` 后即可使用，并共享 `ChartSeries` / `ChartPoint` 数据模型；无需额外图表包。图形、刻度、图例与数据标签直接绘制，不为每个数据点创建 Control 或容器。

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
| 更新动画、PNG 导出、更新规则、主题与尺寸 | [渲染](chart-rendering.md) |

专项为 `--verify-charts`，截图使用具体类型或场景名；命令统一见[预览专项入口](../../../design/PreviewHost/Program.cs)。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节按源码声明列出本库增加的成员；原生控件继承属性仍使用 Avalonia API。注册默认值与主题生效值不同，显式局部值优先。模型、方法、事件、枚举的完整签名可从下表的类型链接查看。PreviewHost 的“控件能力实验室”（各页右上入口）按样式、属性、API／事件和模型分类演示；复杂模型使用原业务示例。

### AreaChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.AreaChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/AreaChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### BarChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.BarChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/BarChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### CartesianChartBase

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.CartesianChartBase.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### ChartBase

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`AccentBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L78) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 主序列／强调色画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`AnimationDuration`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L18) | `System.TimeSpan` | 可写; OneWay | `TimeSpan.FromMilliseconds(200)` | 数据过渡的持续时间；是否启用动画还受控件开关和 Motion.Enabled 约束。 校验：`value => value >= TimeSpan.Zero && value <= TimeSpan.FromSeconds(10)` |
| [`AnimationProgress`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L20) | `double` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Background`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L74) | `Avalonia.Media.IBrush?` | 可写; OneWay | `inherited registration` | 背景画刷；主题相关背景使用 DynamicResource，避免固定颜色破坏明暗切换。 |
| [`ChartPadding`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L20) | `double` | 可写; OneWay | `12` | 8 ; 12 ; 16 校验：`value => double.IsFinite(value) && value >= 0` |
| [`CrosshairPosition`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L20) | `Avalonia.Point?` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`DefaultChartHeight`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L19) | `double` | 可写; OneWay | `280` | 320 / 224 ; 400 / 280 ; 480 / 336 校验：`value => double.IsFinite(value) && value > 0` |
| [`DefaultChartWidth`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L17) | `double` | 可写; OneWay | `400` | 320 / 224 ; 400 / 280 ; 480 / 336 校验：`value => double.IsFinite(value) && value > 0` |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L71) | `string` | 可写; OneWay | `"暂无数据"` | `暂无数据` |
| [`FifthBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L82) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 第五序列画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`FontFamily`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L84) | `Avalonia.Media.FontFamily` | 可写; OneWay | `inherited registration` | 文本绘制使用的字体族；字体回退和实际度量由 Avalonia 文本系统处理。 |
| [`FontSize`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L85) | `double` | 可写; OneWay | `inherited registration` | 10 ; 11 ; 12 |
| [`Foreground`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L75) | `Avalonia.Media.IBrush?` | 可写; OneWay | `inherited registration` | 前景画刷；自定义时保持正文、状态与禁用对比度。 |
| [`GaugeBandRatio`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L25) | `double` | 可写; OneWay | `0.18` | 沿用三档主题密度；基础、进度、等级、气压表按结构缩放带宽，可显式调整该比例 校验：`value => double.IsFinite(value) && value > 0 && value <= 0.5` |
| [`GaugeMajorTickCount`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L16) | `int` | 可写; OneWay | `5` | Number of major ticks, including the minimum and maximum. 校验：`value => value >= 2 && value <= 25` |
| [`GaugeMinorTickCount`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L18) | `int` | 可写; OneWay | `2` | Maximum minor marks between consecutive major ticks. Compact gauges reduce them to keep readable spacing. 校验：`value => value >= 0 && value <= 10` |
| [`GridBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L76) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 绘图区网格线画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`HoveredPoints`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L21) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.ChartSelection>` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`InnerRadius`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L63) | `double` | 可写; OneWay | `0.62` | 径向图内半径比例；控制中空区域，保留扇区数值和比例含义。 校验：`value => double.IsFinite(value) && value >= 0 && value < 1` |
| [`IsAnimating`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L19) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsAnimationEnabled`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L17) | `bool` | 可写; OneWay | `true` | 启用该图表的数据过渡；全局 Motion.Enabled=False 时仍关闭动画。 |
| [`IsLegendInteractive`](../../../TheOne.Avalonia/Controls/ChartBase.Legend.cs#L11) | `bool` | 可写; OneWay | `true` | `True`，允许坐标图图例切换系列及键盘操作 |
| [`IsPanEnabled`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L34) | `bool` | 可写; OneWay | `false` | 默认 `False`；启用鼠标左键拖动及键盘平移 |
| [`IsSmooth`](../../../TheOne.Avalonia/Controls/ChartBase.Smoothing.cs#L10) | `bool` | 可写; OneWay | `false` | `False`，折线／普通面积的单调平滑路径 |
| [`IsZoomEnabled`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L32) | `bool` | 可写; OneWay | `false` | 默认 `False`；启用滚轮及键盘缩放 |
| [`Kind`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L55) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |
| [`LegendPageCount`](../../../TheOne.Avalonia/Controls/ChartBase.Legend.cs#L13) | `int` | 只读; CLR / n/a | `accessor-defined / state` | 只读分页状态，页索引从 0 开始 |
| [`LegendPageIndex`](../../../TheOne.Avalonia/Controls/ChartBase.Legend.cs#L12) | `int` | 只读; CLR / n/a | `accessor-defined / state` | 只读分页状态，页索引从 0 开始 |
| [`MarkerRadius`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L21) | `double` | 可写; OneWay | `3.5` | 3 ; 3.5 ; 4.25 校验：`value => double.IsFinite(value) && value > 0` |
| [`Maximum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L67) | `double` | 可写; OneWay | `100` | 主数值轴上界；空值使用自动范围，不通过裁剪更改数据本身。 校验：`double.IsFinite` |
| [`Minimum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L66) | `double` | 可写; OneWay | `0` | 主数值轴下界；空值由数据自动推导，显式范围必须与数据及 Maximum 协调。 校验：`double.IsFinite` |
| [`MutedBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L77) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 次要文字与辅助标记画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`NavigationAxes`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L35) | `TheOne.Avalonia.Controls.ChartNavigationAxes` | 可写; OneWay | `ChartNavigationAxes.Both` | 默认 `Both`；可设为 `None`、`Horizontal`、`Vertical`，限制导航轴 校验：`value => (value & ~ChartNavigationAxes.Both) == 0` 可选值：`None`, `Horizontal`, `Vertical`, `Both` |
| [`QuaternaryBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L81) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 第四序列画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`RenderedPointCount`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L19) | `int` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SamplingActive`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L20) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SamplingMode`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L15) | `TheOne.Avalonia.Controls.ChartSamplingMode` | 可写; OneWay | `ChartSamplingMode.MinMax` | 密集序列的采样算法；可选策略见枚举，采样影响绘制点数，不替换原始数据。 校验：`value => value is ChartSamplingMode.None or ChartSamplingMode.MinMax` 可选值：`None`, `MinMax` |
| [`SamplingPointBudget`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L18) | `int` | 可写; OneWay | `2000` | A per-series sampling target; gap boundaries and selected originals can exceed it. 校验：`value => value >= 8` |
| [`SamplingThreshold`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L16) | `int` | 可写; OneWay | `2000` | 启用采样的点数阈值；结合视口与 SamplingMode 调整大数据绘制成本。 校验：`value => value >= 2` |
| [`SecondaryBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L79) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 第二序列画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`SecondaryUnit`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L19) | `string?` | 可写; OneWay | `null` | 第二数值轴的单位后缀，与主轴单位独立。 |
| [`SecondaryValueFormat`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L18) | `string` | 可写; OneWay | `"0.##"` | 第二数值轴的数值格式字符串；使用符合 .NET 数值格式的值。 |
| [`SecondaryYAxisTitle`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L17) | `string?` | 可写; OneWay | `null` | 第二数值轴标题；不改变序列使用哪一条数值轴。 |
| [`SecondaryYMaximum`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L15) | `double` | 可写; OneWay | `double.NaN` | 第二数值轴上界；空值自动计算，与 SecondaryYMinimum 配合使用。 校验：`value => !double.IsInfinity(value)` |
| [`SecondaryYMinimum`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L14) | `double` | 可写; OneWay | `double.NaN` | 第二数值轴下界；空值自动计算，与 SecondaryYMaximum 配合使用。 校验：`value => !double.IsInfinity(value)` |
| [`SelectedIndex`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L73) | `int` | 可写; TwoWay | `-1` | 零基系列 / 点索引，支持双向绑定；点索引 `-1` 清除选择 |
| [`SelectedSeriesIndex`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L72) | `int` | 可写; TwoWay | `0` | 零基系列 / 点索引，支持双向绑定；点索引 `-1` 清除选择 |
| [`Selection`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L86) | `TheOne.Avalonia.Controls.ChartSelection?` | 只读; OneWay | `computed / state` | 只读 `ChartSelection?`，包含索引以及原始 `Series` / `Point` 引用 |
| [`SelectionThickness`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L22) | `double` | 可写; OneWay | `2` | 1.5 ; 2 ; 2.5 校验：`value => double.IsFinite(value) && value > 0` |
| [`SeriesSource`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L56) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.ChartSeries>?` | 可写; OneWay | `null` | 模型或结果集合；集合及元素类型见签名。动态业务数据需提供通知；只读集合由控件维护，不直接替换。 |
| [`ShowCrosshair`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L18) | `bool` | 可写; OneWay | `false` | `False`，开启坐标图指针十字准线 |
| [`ShowDataLabels`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L61) | `bool` | 可写; OneWay | `false` | `False`，坐标图显示数据值，饼图／环形图显示可容纳的百分比 |
| [`ShowGaugeNeedle`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L13) | `bool` | 可写; OneWay | `false` | 主题按枚举提供默认值；显式 False 隐藏对应元素。气压表的刻度开关同时作用于两套轴 |
| [`ShowGaugeTicks`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L14) | `bool` | 可写; OneWay | `false` | 主题按枚举提供默认值；显式 False 隐藏对应元素。气压表的刻度开关同时作用于两套轴 |
| [`ShowGrid`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L59) | `bool` | 可写; OneWay | `true` | `True`，坐标图网格 |
| [`ShowLegend`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L58) | `bool` | 可写; OneWay | `true` | `True`，坐标图为系列图例，饼图／环形图为类别图例 |
| [`ShowMarkers`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L60) | `bool` | 可写; OneWay | `true` | `True`，折线／面积图的数据标记 |
| [`ShowSecondaryAxis`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L20) | `bool` | 可写; OneWay | `true` | 显示第二数值轴；序列轴归属仍由序列模型配置。 |
| [`SixthBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L83) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 第六序列画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`SliceHoverOffset`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L23) | `double` | 可写; OneWay | `6` | 4 ; 6 ; 8 校验：`value => double.IsFinite(value) && value >= 0` |
| [`SliceHoverScale`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L24) | `double` | 可写; OneWay | `1.025` | 1.02 ; 1.025 ; 1.03 校验：`value => double.IsFinite(value) && value >= 1 && value <= 1.2` |
| [`StrokeThickness`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L62) | `double` | 可写; OneWay | `2.5` | `2.5`，必须大于零且有限 校验：`value => double.IsFinite(value) && value > 0` |
| [`TertiaryBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L80) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 第三序列画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`Title`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L57) | `string?` | 可写; OneWay | `null` | 可选标题 |
| [`TooltipMode`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L19) | `TheOne.Avalonia.Controls.ChartTooltipMode` | 可写; OneWay | `ChartTooltipMode.Single` | `ChartTooltipMode.Single`，可设为 `Shared` 查看同位置的多个系列 校验：`value => value is ChartTooltipMode.Single or ChartTooltipMode.Shared` 可选值：`Single`, `Shared` |
| [`Unit`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L70) | `string?` | 可写; OneWay | `null` | `0.##` / 空，使用当前区域设置格式化数值 |
| [`Value`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L68) | `double` | 可写; OneWay | `0` | 当前值；单位、范围及空值含义以所属模型和校验条件为准。 校验：`double.IsFinite` |
| [`ValueFormat`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L69) | `string` | 可写; OneWay | `"0.##"` | `0.##` / 空，使用当前区域设置格式化数值 |
| [`Viewport`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L30) | `Avalonia.Rect` | 可写; TwoWay | `new Rect(0, 0, 1, 1)` | A normalized window into full data extents, bounded to [0,1] and at most 1,000× zoom. 校验：`value => double.IsFinite(value.X) && double.IsFinite(value.Y) && double.IsFinite(value.Width) && double.IsFinite(value.Height) && value.Width > 0 && value.Height > 0` |
| [`YAxisTitle`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L16) | `string?` | 可写; OneWay | `null` | 主数值轴标题，参与实际文本度量与轴区域布局。 |
| [`YMaximum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L65) | `double` | 可写; OneWay | `double.NaN` | `NaN` 表示自动范围，可设置有限 Y 轴范围 校验：`value => !double.IsInfinity(value)` |
| [`YMinimum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L64) | `double` | 可写; OneWay | `double.NaN` | `NaN` 表示自动范围，可设置有限 Y 轴范围 校验：`value => !double.IsInfinity(value)` |

### DoughnutChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.DoughnutChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/DoughnutChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### GaugeChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.GaugeChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`GaugeCaption`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L74) | `string?` | 可写; OneWay | `null` | 说明文字默认 null；说明顶端的垂直偏移为半径比例 `[−1, 1]`，NaN 随枚举，与顶部 Title 分开 |
| [`GaugeCaptionOffset`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L69) | `double` | 可写; OneWay | `double.NaN` | 说明文字默认 null；说明顶端的垂直偏移为半径比例 `[−1, 1]`，NaN 随枚举，与顶部 Title 分开 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -1 && value <= 1)` |
| [`GaugeGradeLabelGap`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L73) | `double` | 可写; OneWay | `double.NaN` | 等级标签默认沿切线旋转并保持可读；旋转角 NaN 自动、`[−90, 90]` 显式角度（0 水平）；标签到色带的间距 NaN 随字号、`[0, 100]` 显式像素 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= 0 && value <= 100)` |
| [`GaugeGradeLabelRotation`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L71) | `double` | 可写; OneWay | `double.NaN` | 等级标签默认沿切线旋转并保持可读；旋转角 NaN 自动、`[−90, 90]` 显式角度（0 水平）；标签到色带的间距 NaN 随字号、`[0, 100]` 显式像素 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -90 && value <= 90)` |
| [`GaugeNeedleBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L76) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 显式覆盖指针、主刻度和中心数值颜色；未设置时使用主题或所属色带颜色 |
| [`GaugeNeedleLength`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L64) | `double` | 可写; OneWay | `0.72` | 长度默认半径的 0.72，范围 `(0, 1]`；宽度 NaN 随形状，显式像素值 `(0, 50]` 校验：`value => double.IsFinite(value) && value > 0 && value <= 1` |
| [`GaugeNeedleShape`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L62) | `TheOne.Avalonia.Controls.GaugeNeedleShape` | 可写; OneWay | `GaugeNeedleShape.Auto` | Auto 随枚举；Triangle / Line / Marker 单独覆盖指针形状 校验：`Enum.IsDefined` 可选值：`Auto`, `Triangle`, `Line`, `Marker` |
| [`GaugeNeedleWidth`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L65) | `double` | 可写; OneWay | `double.NaN` | 长度默认半径的 0.72，范围 `(0, 1]`；宽度 NaN 随形状，显式像素值 `(0, 50]` 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value > 0 && value <= 50)` |
| [`GaugeSecondaryBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L83) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 气压表副刻度数默认 7（范围 `[2, 12]`），颜色默认 Foreground |
| [`GaugeSecondaryMaximum`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L81) | `double` | 可写; OneWay | `60` | 气压表副刻度范围，默认 0–60；不改变主值或指针所代表的范围。无效副范围隐藏副轴 校验：`double.IsFinite` |
| [`GaugeSecondaryMinimum`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L80) | `double` | 可写; OneWay | `0` | 气压表副刻度范围，默认 0–60；不改变主值或指针所代表的范围。无效副范围隐藏副轴 校验：`double.IsFinite` |
| [`GaugeSecondaryTickCount`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L82) | `int` | 可写; OneWay | `7` | 气压表副刻度数默认 7（范围 `[2, 12]`），颜色默认 Foreground 校验：`value => value >= 2 && value <= 12` |
| [`GaugeSegments`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L79) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.GaugeSegment>?` | 可写; OneWay | `null` | 每个实例默认独立空集合，支持直接 XAML 子项与绑定替换；有序色带列表；每项 Fraction 是相对于 Minimum–Maximum 的累计结束比例 `(0, 1]`，Brush 控制颜色，Label 用于 Grade |
| [`GaugeStartAngle`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L56) | `double` | 可写; OneWay | `double.NaN` | NaN 表示随枚举；起点允许 −360°–360°，顺时针角度 `(0, 360]`。坐标系 0° 指向右、90° 指向下。支持整圆 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -360 && value <= 360)` |
| [`GaugeStyle`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L55) | `TheOne.Avalonia.Controls.GaugeStyle` | 可写; OneWay | `GaugeStyle.Semicircle` | 结构 ; 默认起点 / 顺时针角度 ; 主题默认指针 / 刻度 / 主刻度数 校验：`Enum.IsDefined` 可选值：`Semicircle`, `Basic`, `Speed`, `Progress`, `StageSpeed`, `Grade`, `Temperature`, `Barometer` |
| [`GaugeSweepAngle`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L57) | `double` | 可写; OneWay | `double.NaN` | NaN 表示随枚举；起点允许 −360°–360°，顺时针角度 `(0, 360]`。坐标系 0° 指向右、90° 指向下。支持整圆 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value > 0 && value <= 360)` |
| [`GaugeTickBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L77) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 显式覆盖指针、主刻度和中心数值颜色；未设置时使用主题或所属色带颜色 |
| [`GaugeTickPlacement`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L63) | `TheOne.Avalonia.Controls.GaugeTickPlacement` | 可写; OneWay | `GaugeTickPlacement.Auto` | Auto 随枚举；Inside / Outside 覆盖主刻度位置，气压表副刻度保持内侧 校验：`Enum.IsDefined` 可选值：`Auto`, `Inside`, `Outside` |
| [`GaugeTrackBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L75) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 背景轴线／轨道颜色，默认 GridBrush |
| [`GaugeValueBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L78) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 显式覆盖指针、主刻度和中心数值颜色；未设置时使用主题或所属色带颜色 |
| [`GaugeValueFontSize`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L66) | `double` | 可写; OneWay | `double.NaN` | NaN 随枚举；字号 `(0, 200]`，垂直偏移为半径比例 `[−1, 1]`，正数向下。字号按实际空间缩小 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value > 0 && value <= 200)` |
| [`GaugeValueOffset`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L67) | `double` | 可写; OneWay | `double.NaN` | NaN 随枚举；字号 `(0, 200]`，垂直偏移为半径比例 `[−1, 1]`，正数向下。字号按实际空间缩小 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -1 && value <= 1)` |
| [`Kind`](../../../TheOne.Avalonia/Controls/GaugeChart.cs#L9) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |
| [`ShowGaugeAnchor`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L59) | `bool?` | 可写; OneWay | `null` | null 随枚举；控制指针中心锚点，仅指针开启时生效 |
| [`ShowGaugeProgress`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L58) | `bool?` | 可写; OneWay | `null` | null 随枚举；True / False 强制显示／隐藏进度弧 |
| [`ShowGaugeTickLabels`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L61) | `bool` | 可写; OneWay | `true` | 默认 True；分别控制数值和刻度标签，不影响数据 |
| [`ShowGaugeValue`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L60) | `bool` | 可写; OneWay | `true` | 默认 True；分别控制数值和刻度标签，不影响数据 |

### HorizontalBarChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.HorizontalBarChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/HorizontalBarChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### LineChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.LineChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/LineChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### PercentStackedAreaChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.PercentStackedAreaChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/PercentStackedAreaChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### PercentStackedBarChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.PercentStackedBarChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/PercentStackedBarChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### PercentStackedHorizontalBarChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.PercentStackedHorizontalBarChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/PercentStackedHorizontalBarChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### PieChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.PieChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/PieChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### RadialChartBase

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.RadialChartBase.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### ScatterChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.ScatterChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/ScatterChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### StackedAreaChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.StackedAreaChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/StackedAreaChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### StackedBarChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.StackedBarChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/StackedBarChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### StackedHorizontalBarChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.StackedHorizontalBarChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/StackedHorizontalBarChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

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
| ChartBase | `one-large` | 模板自动类；不手动绑定 | ^.one-large => FontSize=12; DefaultChartWidth=480; DefaultChartHeight=336; ChartPadding=16; StrokeThickness=3; MarkerRadius=4.25; SelectionThickness=2.5; SliceHoverOffset=8; SliceHoverScale=1.03; GaugeBandRatio=0.22 |
| ChartBase | `one-small` | 模板自动类；不手动绑定 | ^.one-small => FontSize=10; DefaultChartWidth=320; DefaultChartHeight=224; ChartPadding=8; StrokeThickness=2; MarkerRadius=3; SelectionThickness=1.5; SliceHoverOffset=4; SliceHoverScale=1.02; GaugeBandRatio=0.14 |

`small` / `normal` / `large` 是公开密度入口；`one-small` / `one-normal` / `one-large` 为自动生成的内部类。`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出额外外观类的控件使用默认主题及其属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.AreaChart`](../../api/zh-cn/TheOne.Avalonia.Controls.AreaChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.BarChart`](../../api/zh-cn/TheOne.Avalonia.Controls.BarChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.CartesianChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.CartesianChartBase.md) | Class | 0 | 0 | 0 |
| [`TheOne.Avalonia.Controls.ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | Class | 72 | 18 | 2 |
| [`TheOne.Avalonia.Controls.ChartKind`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartKind.md) | Enum | 14 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartNavigationAxes`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartNavigationAxes.md) | Enum | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartPoint`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartPoint.md) | Class | 3 | 2 | 0 |
| [`TheOne.Avalonia.Controls.ChartSamplingMode`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartSamplingMode.md) | Enum | 2 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartSelection`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartSelection.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartSelectionChangedEventArgs`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartSelectionChangedEventArgs.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartSeries`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartSeries.md) | Class | 7 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartTooltipMode`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartTooltipMode.md) | Enum | 2 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartViewportChangedEventArgs`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartViewportChangedEventArgs.md) | Class | 2 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartYAxis`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartYAxis.md) | Enum | 2 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DoughnutChart`](../../api/zh-cn/TheOne.Avalonia.Controls.DoughnutChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.GaugeChart`](../../api/zh-cn/TheOne.Avalonia.Controls.GaugeChart.md) | Class | 27 | 1 | 0 |
| [`TheOne.Avalonia.Controls.GaugeNeedleShape`](../../api/zh-cn/TheOne.Avalonia.Controls.GaugeNeedleShape.md) | Enum | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.GaugeSegment`](../../api/zh-cn/TheOne.Avalonia.Controls.GaugeSegment.md) | Class | 3 | 1 | 0 |
| [`TheOne.Avalonia.Controls.GaugeStyle`](../../api/zh-cn/TheOne.Avalonia.Controls.GaugeStyle.md) | Enum | 8 | 1 | 0 |
| [`TheOne.Avalonia.Controls.GaugeTickPlacement`](../../api/zh-cn/TheOne.Avalonia.Controls.GaugeTickPlacement.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Controls.HorizontalBarChart`](../../api/zh-cn/TheOne.Avalonia.Controls.HorizontalBarChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.LineChart`](../../api/zh-cn/TheOne.Avalonia.Controls.LineChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.PercentStackedAreaChart`](../../api/zh-cn/TheOne.Avalonia.Controls.PercentStackedAreaChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.PercentStackedBarChart`](../../api/zh-cn/TheOne.Avalonia.Controls.PercentStackedBarChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.PercentStackedHorizontalBarChart`](../../api/zh-cn/TheOne.Avalonia.Controls.PercentStackedHorizontalBarChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.PieChart`](../../api/zh-cn/TheOne.Avalonia.Controls.PieChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.RadialChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.RadialChartBase.md) | Class | 0 | 0 | 0 |
| [`TheOne.Avalonia.Controls.ScatterChart`](../../api/zh-cn/TheOne.Avalonia.Controls.ScatterChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.StackedAreaChart`](../../api/zh-cn/TheOne.Avalonia.Controls.StackedAreaChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.StackedBarChart`](../../api/zh-cn/TheOne.Avalonia.Controls.StackedBarChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.StackedHorizontalBarChart`](../../api/zh-cn/TheOne.Avalonia.Controls.StackedHorizontalBarChart.md) | Class | 1 | 1 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
