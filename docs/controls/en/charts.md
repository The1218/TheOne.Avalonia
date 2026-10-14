# Charts

[简体中文](../zh-cn/charts.md) · **English**

Fourteen independently drawn controls live in `TheOne.Avalonia.Controls`, each with its own concrete type, theme, and visual state. Load `OneTheme` and use shared `ChartSeries` / `ChartPoint` models without an extra chart package. Shapes, ticks, legends, and labels are drawn directly instead of creating a Control or container per point.

`ChartBase`, `CartesianChartBase`, and `RadialChartBase` are abstract infrastructure. The former switchable `Chart` / `Kind` API was removed: replace `<Chart Kind="Line">` with `<LineChart>` and `<Chart Kind="Gauge">` with `<GaugeChart>`, migrating other types as listed below. `Kind` is only a read-only type identifier. Dynamic switching creates a new concrete control and replaces host content while retaining shared data / ViewModels. PNG snapshots also use the same concrete type.

## Chart types

| Control | Shape | Data |
| --- | --- | --- |
| `LineChart` | Line | X / Y coordinates from visible series |
| `AreaChart` | Area | Fill between line and baseline; multiple series do not stack |
| `BarChart` | Columns | Grouped columns from multiple series |
| `HorizontalBarChart` | Horizontal bars | Grouped bars from multiple series |
| `ScatterChart` | Scatter | X / Y coordinates from visible series |
| `PieChart` | Pie | Valid point Y values in the first visible nonempty series |
| `DoughnutChart` | Doughnut | Same as pie; `InnerRadius` sets the inner-radius ratio |
| `GaugeChart` | Gauge, semicircle by default with eight selectable layouts | `Minimum`, `Maximum`, `Value`; no series data |
| `StackedBarChart` | Stacked columns | Visible series at matching indices; positive and negative values stack separately |
| `StackedHorizontalBarChart` | Stacked horizontal bars | Same stacking rules, with a horizontal value axis |
| `StackedAreaChart` | Stacked area | Layered fills for visible series with matching indices and X |
| `PercentStackedBarChart` | Percent stacked columns | Positive / negative contributions normalized separately per axis and index |
| `PercentStackedHorizontalBarChart` | Percent stacked horizontal bars | Same percentage rules, with a horizontal value axis |
| `PercentStackedAreaChart` | Percent stacked area | Percentage layers for matching axis, index, and X |

`ChartPoint.X` / `Y` are `double`; `Label` is optional text. `ChartSeries.Name` names the legend, `ItemsSource` is `IList<ChartPoint>`, `IsVisible=False` hides the series, and `Brush` overrides its color. `YAxis` selects primary / secondary axes. Optional `Unit` / `ValueFormat` override raw-value display; `null` inherits the corresponding axis configuration. Default series colors come from TheOne and follow Light / Dark.

## Compiled binding example

Enable compiled bindings and declare the ViewModel type on the view root. Place the control in a bounded-height Grid; the chart relayouts to available dimensions:

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

A minimal ViewModel follows. Set its instance in code-behind with `DataContext = new ChartViewModel()`:

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

`AreaChart`, `BarChart`, `HorizontalBarChart`, and `ScatterChart` can use the same series data as separate control instances. Line / area / scatter use numeric X positions, with `Label` at the corresponding X. Column / horizontal-bar charts use list indices for categories, grouping series at each index; category text comes from the first visible series. Keep category order consistent across series; `Label` or X values do not automatically merge categories.

Cartesian charts ignore nonfinite coordinates; line / area break at `NaN` / `Infinity`. Input order determines line connections, so sort data for business needs. Automatic ranges for columns / areas include zero. `YMinimum` / `YMaximum` override the range and marks clip to it. A final maximum no greater than the minimum shows an empty state.

## Feature guides

| Need | Documentation |
| --- | --- |
| Smoothing, stacking, percentages, dual axes, sampling | [Cartesian charts and sampling](chart-cartesian.md) |
| Pie, doughnut, gauge needles / ticks | [Radial charts](chart-radial.md) |
| Labels, selection, crosshairs, shared tooltips, legends, zoom / pan | [Interaction](chart-interaction.md) |
| Update animation, PNG export, update rules, themes, sizes | [Rendering](chart-rendering.md) |

Use `--verify-charts`; rendering uses concrete type or scenario names. Commands are in the [preview entry points](../../../design/PreviewHost/Program.cs).
