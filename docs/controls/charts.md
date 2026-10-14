# Chart 图表

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
| `GaugeChart` | 半圆仪表图 | `Minimum`、`Maximum`、`Value`，不使用系列数据 |
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

专项为 `--verify-charts`，截图使用具体类型或场景名；命令统一见[预览专项入口](../../design/PreviewHost/Program.cs)。
