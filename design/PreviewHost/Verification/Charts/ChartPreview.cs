using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

/// <summary>Runs the same rendering and input checks in managed PreviewHost and the native consumer.</summary>
internal static partial class ChartPreview
{
    private static AvaloniaProperty[] TestTransferProperties =>
    [
        ChartBase.SeriesSourceProperty,
        ChartBase.TitleProperty,
        ChartBase.ShowLegendProperty,
        ChartBase.ShowGridProperty,
        ChartBase.ShowMarkersProperty,
        ChartBase.ShowDataLabelsProperty,
        ChartBase.InnerRadiusProperty,
        ChartBase.YMinimumProperty,
        ChartBase.YMaximumProperty,
        ChartBase.MinimumProperty,
        ChartBase.MaximumProperty,
        ChartBase.ValueProperty,
        ChartBase.ValueFormatProperty,
        ChartBase.UnitProperty,
        ChartBase.SelectedSeriesIndexProperty,
        ChartBase.SelectedIndexProperty,
        ChartBase.ViewportProperty,
        ChartBase.NavigationAxesProperty,
        ChartBase.IsZoomEnabledProperty,
        ChartBase.IsPanEnabledProperty,
        ChartBase.SecondaryYMinimumProperty,
        ChartBase.SecondaryYMaximumProperty,
        ChartBase.YAxisTitleProperty,
        ChartBase.SecondaryYAxisTitleProperty,
        ChartBase.SecondaryValueFormatProperty,
        ChartBase.SecondaryUnitProperty,
        ChartBase.ShowSecondaryAxisProperty,
        ChartBase.SamplingModeProperty,
        ChartBase.SamplingThresholdProperty,
        ChartBase.SamplingPointBudgetProperty,
        ChartBase.ShowCrosshairProperty,
        ChartBase.TooltipModeProperty,
        ChartBase.IsLegendInteractiveProperty,
        ChartBase.IsSmoothProperty,
        ChartBase.IsAnimationEnabledProperty,
        ChartBase.AnimationDurationProperty,
        ChartBase.ShowGaugeNeedleProperty,
        ChartBase.ShowGaugeTicksProperty,
        ChartBase.GaugeMajorTickCountProperty,
        ChartBase.GaugeMinorTickCountProperty
    ];

    public static ChartBase CreateControl(ChartKind kind)
    {
        return ChartControlFactory.CreateControl(kind);
    }

    private static ChartBase SwitchChart(Window window, ChartBase previous, ChartKind kind)
    {
        if (previous.Kind == kind) return previous;
        var chart = CreateControl(kind);
        var focused = previous.IsFocused;
        chart.Width = previous.Width;
        chart.Height = previous.Height;
        chart.IsEnabled = previous.IsEnabled;
        foreach (var property in TestTransferProperties) chart.SetValue(property, previous.GetValue(property));
        ((Border)window.Content!).Child = chart;
        if (focused) chart.Focus();
        Flush();
        return chart;
    }

    public static ChartBase CreateSample(ChartKind kind)
    {
        var chart = CreateControl(kind);
        chart.Height = 300;
        chart.Title = kind == ChartKind.Gauge ? "目标完成度" : "月度销售";
        chart.Value = 72;
        chart.Unit = kind == ChartKind.Gauge ? "%" : "";
        chart.ShowDataLabels = kind is ChartKind.Pie or ChartKind.Doughnut or ChartKind.StackedBar
            or ChartKind.StackedHorizontalBar or ChartKind.StackedArea or ChartKind.PercentStackedBar
            or ChartKind.PercentStackedHorizontalBar or ChartKind.PercentStackedArea;
        chart.SeriesSource = new ObservableCollection<ChartSeries>
        {
            new()
            {
                Name = "本期",
                ItemsSource = new ObservableCollection<ChartPoint>(
                    new[] { 42d, 58, 51, 76, 68, 92 }.Select((value, index) =>
                        new ChartPoint(index, value, $"{index + 1}月")))
            },
            new()
            {
                Name = "上期",
                ItemsSource = new ObservableCollection<ChartPoint>(
                    new[] { 34d, 43, 49, 56, 61, 70 }.Select((value, index) =>
                        new ChartPoint(index, value, $"{index + 1}月")))
            }
        };
        return chart;
    }

    public static ChartBase CreateDualAxisSample()
    {
        return new LineChart
        {
            Height = 300, Title = "销售与访问 · 独立双轴", Unit = "万元", SecondaryUnit = "人", YAxisTitle = "销售额",
            SecondaryYAxisTitle = "访问人数",
            SeriesSource = new[]
            {
                new ChartSeries
                {
                    Name = "销售",
                    ItemsSource = new[] { 28d, 36, 31, 42, 39, 48 }
                        .Select((value, index) => new ChartPoint(index, value, $"{index + 1}月")).ToArray()
                },
                new ChartSeries
                {
                    Name = "访问", YAxis = ChartYAxis.Secondary,
                    ItemsSource = new[] { 2400d, 3200, 2700, 3800, 3400, 4600 }
                        .Select((value, index) => new ChartPoint(index, value, $"{index + 1}月")).ToArray()
                }
            }
        };
    }

    public static ChartBase CreateLargeSample()
    {
        return new LineChart
        {
            Height = 300, Title = "100,000 条记录 · MinMax 采样", ShowMarkers = false, SamplingPointBudget = 2000,
            SamplingThreshold = 2000,
            SeriesSource = new[]
            {
                new ChartSeries
                {
                    Name = "传感器", ItemsSource = Enumerable.Range(0, 100000).Select(index => new ChartPoint(index,
                        index is >= 44000 and < 46000 ? double.NaN :
                        index == 99777 ? 95 :
                        index == 99888 ? 5 : 50 + 18 * Math.Sin(index / 180d))).ToArray()
                }
            }
        };
    }

    public static ChartBase CreateCrosshairSample()
    {
        var chart = CreateSample(ChartKind.Line);
        chart.ShowCrosshair = true;
        chart.TooltipMode = ChartTooltipMode.Single;
        return chart;
    }

    public static ChartBase CreateSharedTooltipSample()
    {
        var chart = CreateDualAxisSample();
        chart.ShowCrosshair = true;
        chart.TooltipMode = ChartTooltipMode.Shared;
        return chart;
    }

    public static ChartBase CreateInteractiveLegendSample()
    {
        var chart = CreateSample(ChartKind.Line);
        chart.SeriesSource![1].IsVisible = false;
        chart.IsLegendInteractive = true;
        return chart;
    }

    public static ChartBase CreateSmoothSample()
    {
        var chart = CreateSample(ChartKind.Line);
        chart.Height = double.NaN;
        chart.IsSmooth = true;
        chart.Title = "平滑曲线 · 保留原始数据点";
        return chart;
    }

    public static ChartBase CreateLabeledLineSample()
    {
        var chart = CreateSample(ChartKind.Line);
        chart.Height = double.NaN;
        chart.IsSmooth = true;
        chart.ShowDataLabels = true;
        chart.Title = "月度销售 · 边缘标记与标签避让";
        return chart;
    }

    public static ChartBase CreateAnimatedSample()
    {
        var chart = CreateSample(ChartKind.Line);
        chart.Height = double.NaN;
        chart.YMinimum = 0;
        chart.YMaximum = 120;
        chart.IsAnimationEnabled = true;
        chart.AnimationDuration = TimeSpan.FromSeconds(1);
        return chart;
    }

    public static ChartBase CreateNeedleGaugeSample()
    {
        var chart = CreateSample(ChartKind.Gauge);
        chart.Height = double.NaN;
        chart.HorizontalAlignment = HorizontalAlignment.Left;
        chart.Value = 71.06;
        chart.ShowGaugeNeedle = true;
        chart.ShowGaugeTicks = true;
        return chart;
    }

    public static ChartBase CreateShapeHoverSample(ChartKind kind)
    {
        var chart = CreateSample(kind);
        chart.Height = double.NaN;
        if (kind == ChartKind.Bar) chart.ShowDataLabels = true;
        return chart;
    }

    internal static void AdvanceAnimation(int milliseconds)
    {
        var frame = new DispatcherFrame();
        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(milliseconds) };
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            frame.Continue = false;
        };
        timer.Start();
        Dispatcher.UIThread.PushFrame(frame);
        Flush();
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private static Window Open(ChartBase chart, ThemeVariant theme, ControlSize size)
    {
        chart.Height = 340;
        var window = new Window
        {
            Width = 640, Height = 380, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
            Content = new Border { Padding = new Thickness(16), Child = chart }
        };
        ControlSizing.SetSize(window, size);
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        return window;
    }

    public static void Verify(Action<bool, string>? check = null)
    {
        check ??= (condition, message) =>
        {
            if (!condition) throw new InvalidOperationException("Failed: " + message);
            Console.WriteLine("PASS: " + message);
        };
        VerifyAxisLabels(check);
        VerifyGaugeStyles(check);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            foreach (var kind in Enum.GetValues<ChartKind>()) VerifyKind(theme, size, kind, check);
            VerifyData(theme, size, check);
            VerifyEdges(theme, size, check);
            VerifyNavigation(theme, size, check);
            VerifyStacks(theme, size, check);
            VerifyPercentStacks(theme, size, check);
            VerifySecondaryAxes(theme, size, check);
            VerifySampling(theme, size, check);
            VerifyReentrancy(theme, size, check);
            VerifySharedHover(theme, size, check);
            VerifyInteractiveLegend(theme, size, check);
            VerifyHoverReentrancy(theme, size, check);
            VerifySmoothCurve(theme, size, check);
            VerifyUpdateAnimation(theme, size, check);
            VerifyExport(theme, size, check);
            VerifyGraphicSizing(theme, size, check);
            VerifyShapeHoverAndGauge(theme, size, check);
            VerifyLineLabels(theme, size, check);
        }
    }

    private static void VerifyKind(ThemeVariant theme, ControlSize size, ChartKind kind, Action<bool, string> check)
    {
        var chart = CreateSample(kind);
        var window = Open(chart, theme, size);
        var label = $"Chart/{theme.Key}/{size}/{kind}";
        check(chart.GetType().Name == kind + "Chart", $"{label}: independent concrete chart type and fixed profile");
        check(chart.Bounds.Width > 500 && chart.Bounds.Height == 340 && !chart.GetVisualDescendants().Any(),
            $"{label}: bounded viewport draws data without item controls");
        check(chart.FontSize == (size == ControlSize.Small ? 10 : size == ControlSize.Large ? 12 : 11),
            $"{label}: inherited three-size typography");
        check(
            Colour(chart.Background) == Resource(chart, "One.Chart.Background") &&
            Colour(chart.Foreground) == Resource(chart, "One.Chart.Text") &&
            Colour(chart.GridBrush) == Resource(chart, "One.Chart.Grid") &&
            Colour(chart.AccentBrush) == Resource(chart, "One.Chart.Series1"),
            $"{label}: TheOne semantic surface, labels, grid and series");
        var before = Frame(window);
        check(before.Length > 0, $"{label}: Skia renders the chart");
        if (kind == ChartKind.Gauge)
        {
            chart.Value = 95;
            Flush();
            check(!before.SequenceEqual(Frame(window)), $"{label}: gauge value changes rendered arc and number");
            chart.Value = -10;
            Flush();
            chart.Value = 110;
            Flush();
            chart.Minimum = 100;
            chart.Maximum = 100;
            Flush();
            check(chart.Selection is null && chart.GetDataPointPosition(0, 0) is null,
                $"{label}: clamped and zero-range gauges retain safe geometry without point selection");
        }
        else
        {
            if (kind is not (ChartKind.Pie or ChartKind.Doughnut))
            {
                chart.NavigationAxes = ChartNavigationAxes.Horizontal;
                chart.Zoom(2);
                chart.Pan(0.1, 0);
                Flush();
                var transformed = chart.GetDataPointPosition(0, 2);
                check(transformed is { } mark && chart.HitTestDataPoint(mark) is { SeriesIndex: 0, PointIndex: 2 },
                    $"{label}: zoomed and panned marks still hit their original data");
                chart.ResetViewport();
                chart.NavigationAxes = ChartNavigationAxes.Both;
                Flush();
            }

            var point = chart.GetDataPointPosition(0, 2);
            check(
                point is { } local && double.IsFinite(local.X) && double.IsFinite(local.Y) &&
                chart.HitTestDataPoint(local) is { SeriesIndex: 0, PointIndex: 2 },
                $"{label}: geometry and hit testing identify the same data model");
            var events = 0;
            chart.SelectionChanged += (_, _) => events++;
            if (point is { } hit)
            {
                var location = chart.TranslatePoint(hit, window)!.Value;
                window.MouseMove(location);
                window.MouseDown(location, MouseButton.Left);
                window.MouseUp(location, MouseButton.Left);
                Flush();
                check(
                    chart.SelectedIndex == 2 && chart.Selection?.Point == chart.SeriesSource![0].ItemsSource![2] &&
                    events > 0, $"{label}: real pointer selection reports the data model");
                check(ToolTip.GetTip(chart) is string tooltip && tooltip.Contains("3月"),
                    $"{label}: hover describes the selected label and value");
            }

            chart.Focus();
            Press(window, Key.End, PhysicalKey.End);
            check(chart.SelectedIndex == 5 && chart.Selection?.PointIndex == 5,
                $"{label}: End selects the last data item");
            Press(window, Key.Home, PhysicalKey.Home);
            Press(window, Key.Right, PhysicalKey.ArrowRight);
            check(chart.SelectedIndex == 1, $"{label}: Home and Right navigate data without item containers");
            if (kind is not (ChartKind.Pie or ChartKind.Doughnut))
            {
                Press(window, Key.Down, PhysicalKey.ArrowDown);
                check(chart.SelectedSeriesIndex == 1 && chart.SelectedIndex == 1,
                    $"{label}: Down navigates to the comparison series");
            }

            Press(window, Key.Escape, PhysicalKey.Escape);
            check(chart.Selection is null && chart.SelectedIndex == -1, $"{label}: Escape clears selection");
            chart.IsEnabled = false;
            Flush();
            check(!Frame(window).SequenceEqual(before), $"{label}: disabled chart visibly changes opacity");
        }

        window.Close();
        Flush();
    }

    private static void VerifyData(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var points = new ObservableCollection<ChartPoint>
        {
            new(0, -20, "negative"), new(1, double.NaN, "gap"), new(double.NaN, 5), new(3, 20, "positive"),
            new(4, double.PositiveInfinity)
        };
        var series = new ChartSeries { Name = "Live", ItemsSource = points };
        var source = new ObservableCollection<ChartSeries> { series };
        ChartBase chart = new LineChart { Title = "Live data", SeriesSource = source };
        var window = Open(chart, theme, size);
        var label = $"Chart/{theme.Key}/{size}/data";
        foreach (var kind in new[]
                 {
                     ChartKind.Line, ChartKind.Area, ChartKind.Bar, ChartKind.HorizontalBar, ChartKind.Scatter,
                     ChartKind.StackedBar, ChartKind.StackedHorizontalBar, ChartKind.StackedArea,
                     ChartKind.PercentStackedBar, ChartKind.PercentStackedHorizontalBar, ChartKind.PercentStackedArea
                 })
        {
            chart = SwitchChart(window, chart, kind);
            Flush();
            check(
                chart.GetDataPointPosition(0, 0) is not null && chart.GetDataPointPosition(0, 3) is not null &&
                chart.GetDataPointPosition(0, 1) is null && chart.GetDataPointPosition(0, 2) is null &&
                chart.GetDataPointPosition(0, 4) is null,
                $"{label}/{kind}: negative values render; non-finite coordinates form gaps");
        }

        chart = SwitchChart(window, chart, ChartKind.Line);
        chart.SelectedIndex = 0;
        chart.Focus();
        Press(window, Key.Right, PhysicalKey.ArrowRight);
        check(chart.SelectedIndex == 3, $"{label}: keyboard skips missing data points");
        var before = Frame(window);
        points[0].Y = -5;
        Flush();
        check(!before.SequenceEqual(Frame(window)), $"{label}: observable point changes repaint automatically");
        points.Add(new ChartPoint(5, 10, "added"));
        Flush();
        check(chart.GetDataPointPosition(0, 5) is not null, $"{label}: collection insertion updates geometry");
        chart.SelectedIndex = 5;
        points.RemoveAt(5);
        Flush();
        check(chart.Selection is null, $"{label}: removing selected data clears the selected model");
        series.IsVisible = false;
        Flush();
        check(chart.GetDataPointPosition(0, 0) is null && chart.Selection is null,
            $"{label}: hidden series participates in neither drawing nor selection");
        series.IsVisible = true;
        foreach (var kind in new[] { ChartKind.Pie, ChartKind.Doughnut })
        {
            chart = SwitchChart(window, chart, kind);
            Flush();
            check(
                chart.GetDataPointPosition(0, 0) is null && chart.GetDataPointPosition(0, 3) is not null &&
                chart.GetDataPointPosition(0, 4) is null,
                $"{label}/{kind}: radial charts exclude negative and non-finite values");
        }

        chart = SwitchChart(window, chart, ChartKind.Line);
        var thousand = new ObservableCollection<ChartPoint>(Enumerable.Range(0, 1000)
            .Select(index => new ChartPoint(index, 50 + 25 * Math.Sin(index / 20d), $"Point {index}")));
        series.ItemsSource = thousand;
        chart.SelectedIndex = -1;
        Flush();
        foreach (var kind in Enum.GetValues<ChartKind>().Where(kind => kind != ChartKind.Gauge))
        {
            chart = SwitchChart(window, chart, kind);
            Flush();
            check(chart.GetDataPointPosition(0, 999) is not null && !chart.GetVisualDescendants().Any(),
                $"{label}/{kind}: thousand-point geometry stays bounded without item controls");
        }

        chart = SwitchChart(window, chart, ChartKind.Line);
        Flush();
        var denseFrame = Frame(window);
        chart.Focus();
        Press(window, Key.End, PhysicalKey.End);
        check(
            chart.SelectedIndex == 999 && chart.Selection?.Point == thousand[999] &&
            chart.GetDataPointPosition(0, 999) is not null && !chart.GetVisualDescendants().Any(),
            $"{label}: 1,000 points create zero item controls; End reaches the final model");
        thousand[999].Y = 100;
        Flush();
        check(!denseFrame.SequenceEqual(Frame(window)), $"{label}: distant data changes reach the renderer");
        chart.SelectedIndex = 0;
        var host = (Border)window.Content!;
        host.Child = null;
        Flush();
        thousand[0].Y = double.NaN;
        check(chart.Selection is not null, $"{label}: detached chart releases collection and point subscriptions");
        host.Child = chart;
        Flush();
        check(chart.Selection is null && chart.GetDataPointPosition(0, 0) is null,
            $"{label}: reattached chart observes current data and clears stale selection");
        var plain = new List<ChartPoint> { new(0, 12) };
        series.ItemsSource = plain;
        chart.SelectedIndex = 0;
        Flush();
        plain.Clear();
        chart.RefreshData();
        Flush();
        check(chart.Selection is null && chart.GetDataPointPosition(0, 0) is null,
            $"{label}: explicit refresh supports ordinary IList mutation");
        source.Clear();
        Flush();
        check(chart.GetDataPointPosition(0, 0) is null && Frame(window).Length > 0,
            $"{label}: empty collection renders without invalid geometry");
        window.Close();
        Flush();
    }

    private static void VerifyEdges(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var chart = CreateSample(ChartKind.Line);
        var window = Open(chart, theme, size);
        var label = $"Chart/{theme.Key}/{size}/edges";
        chart.SeriesSource = new ObservableCollection<ChartSeries>
        {
            new() { IsVisible = false, ItemsSource = new[] { new ChartPoint(0, 1) } },
            new() { ItemsSource = Array.Empty<ChartPoint>() },
            new() { ItemsSource = new[] { new ChartPoint(0, 1), new ChartPoint(1, 2), new ChartPoint(2, 3) } },
            new() { ItemsSource = new[] { new ChartPoint(0, 4) } }
        };
        chart.SelectedSeriesIndex = 0;
        chart.SelectedIndex = -1;
        chart.Focus();
        Press(window, Key.Right, PhysicalKey.ArrowRight);
        check(chart.SelectedSeriesIndex == 2 && chart.SelectedIndex == 0,
            $"{label}: keyboard starts in the first visible populated series");
        Press(window, Key.End, PhysicalKey.End);
        Press(window, Key.Down, PhysicalKey.ArrowDown);
        check(chart.SelectedSeriesIndex == 3 && chart.SelectedIndex == 0,
            $"{label}: series navigation clamps to a shorter destination");
        chart.SeriesSource = new[]
            { new ChartSeries { ItemsSource = new[] { new ChartPoint(0, 0), new ChartPoint(1, 0) } } };
        foreach (var kind in new[]
                 {
                     ChartKind.Bar, ChartKind.HorizontalBar, ChartKind.StackedBar, ChartKind.StackedHorizontalBar,
                     ChartKind.PercentStackedBar, ChartKind.PercentStackedHorizontalBar
                 })
        {
            chart = SwitchChart(window, chart, kind);
            Flush();
            var zero = chart.GetDataPointPosition(0, 1);
            check(zero is { } point && chart.HitTestDataPoint(point)?.PointIndex == 1,
                $"{label}/{kind}: zero-only bars retain a visible selectable mark");
        }

        chart.SeriesSource = new[]
        {
            new ChartSeries
            {
                ItemsSource = Enumerable.Range(0, 4).Select(index => new ChartPoint(index, 1, $"Quarter {index}"))
                    .ToArray()
            }
        };
        foreach (var kind in new[] { ChartKind.Pie, ChartKind.Doughnut })
        {
            chart = SwitchChart(window, chart, kind);
            Flush();
            var positions = Enumerable.Range(0, 4).Select(index => chart.GetDataPointPosition(0, index)).ToArray();
            check(
                positions.Select((position, index) =>
                    position is { } point && chart.HitTestDataPoint(point)?.PointIndex == index).All(hit => hit),
                $"{label}/{kind}: every quadrant maps to its original data model");
            if (kind == ChartKind.Doughnut)
            {
                var center = new Point(positions.Average(position => position!.Value.X),
                    positions.Average(position => position!.Value.Y));
                check(chart.HitTestDataPoint(center) is null, $"{label}: doughnut hole does not select a wedge");
            }

            var oldPosition = positions[0]!.Value;
            chart.Width = 20;
            chart.Height = 20;
            Flush();
            check(chart.HitTestDataPoint(oldPosition) is null && chart.GetDataPointPosition(0, 0) is null,
                $"{label}/{kind}: shrinking a populated chart clears old hit geometry");
            chart.Width = double.NaN;
            chart.Height = 340;
            Flush();
        }

        chart = SwitchChart(window, chart, ChartKind.Pie);
        chart.SeriesSource = new[] { new ChartSeries { ItemsSource = new[] { new ChartPoint(0, double.MaxValue) } } };
        Flush();
        var full = chart.GetDataPointPosition(0, 0);
        check(full is { } fullPoint && chart.HitTestDataPoint(fullPoint)?.PointIndex == 0 && Frame(window).Length > 0,
            $"{label}: a single full-circle slice renders and selects");
        chart.SeriesSource![0].ItemsSource = new[]
            { new ChartPoint(0, double.MaxValue), new ChartPoint(1, double.MaxValue) };
        Flush();
        check(
            Enumerable.Range(0, 2).All(index =>
                chart.GetDataPointPosition(0, index) is { } point &&
                chart.HitTestDataPoint(point)?.PointIndex == index),
            $"{label}: extreme positive slice values avoid overflow");
        window.Close();
        Flush();
    }

    private static void VerifyNavigation(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        ChartBase chart = new LineChart
        {
            SeriesSource = new[]
            {
                new ChartSeries
                {
                    ItemsSource = Enumerable.Range(0, 5).Select(index => new ChartPoint(index, index, $"Point {index}"))
                        .ToArray()
                }
            }
        };
        var window = Open(chart, theme, size);
        var label = $"Chart/{theme.Key}/{size}/navigation";
        var full = new Rect(0, 0, 1, 1);
        var events = 0;
        var oldEvent = full;
        var newEvent = full;
        chart.ViewportChanged += (_, args) =>
        {
            events++;
            oldEvent = args.OldViewport;
            newEvent = args.NewViewport;
        };
        var center = chart.GetDataPointPosition(0, 2)!.Value;
        var location = chart.TranslatePoint(center, window)!.Value;
        window.MouseMove(location);
        window.MouseWheel(location, new Vector(0, 1));
        Flush();
        check(chart.Viewport == full, $"{label}: opt-in zoom defaults to leaving wheel input unchanged");
        chart.Zoom(2);
        Flush();
        check(
            chart.Viewport.Width == 0.5 && chart.Viewport.Height == 0.5 && events == 1 && oldEvent == full &&
            newEvent == chart.Viewport,
            $"{label}: programmatic zoom and viewport event work with input flags disabled");
        chart.Pan(0.2, -0.2);
        Flush();
        check(chart.Viewport.X > 0.25 && chart.Viewport.Y < 0.25,
            $"{label}: programmatic pan uses fractions of the current view");
        chart.ResetViewport();
        chart.IsZoomEnabled = true;
        Flush();
        center = chart.GetDataPointPosition(0, 2)!.Value;
        location = chart.TranslatePoint(center, window)!.Value;
        window.MouseMove(location);
        window.MouseWheel(location, new Vector(0, 1));
        Flush();
        var anchored = chart.GetDataPointPosition(0, 2);
        check(
            chart.Viewport.Width < 1 && chart.Viewport.Height < 1 && anchored is { } point &&
            Distance(point, center) < 0.5, $"{label}: real wheel zoom preserves the data under the pointer");
        chart.Viewport = new Rect(0.2, 0.3, 0.4, 0.4);
        chart.NavigationAxes = ChartNavigationAxes.Horizontal;
        chart.Pan(0.25, 0.25);
        Flush();
        check(Math.Abs(chart.Viewport.X - 0.3) < 0.0001 && chart.Viewport.Y == 0.3,
            $"{label}: horizontal-only pan preserves vertical view");
        chart.Zoom(2);
        Flush();
        check(Math.Abs(chart.Viewport.Width - 0.2) < 0.0001 && chart.Viewport.Height == 0.4,
            $"{label}: horizontal-only zoom preserves vertical span");
        chart.NavigationAxes = ChartNavigationAxes.Vertical;
        var prior = chart.Viewport;
        chart.Pan(1, 0.1);
        chart.Zoom(2);
        Flush();
        check(
            chart.Viewport.X == prior.X && chart.Viewport.Width == prior.Width && chart.Viewport.Height < prior.Height,
            $"{label}: vertical-only navigation preserves horizontal view");
        chart.NavigationAxes = ChartNavigationAxes.None;
        prior = chart.Viewport;
        chart.Zoom(2);
        chart.Pan(1, 1);
        window.MouseWheel(location, new Vector(0, 1));
        Flush();
        check(chart.Viewport == prior, $"{label}: no-axis mode locks API and wheel navigation");
        chart.NavigationAxes = ChartNavigationAxes.Both;
        chart.ResetViewport();
        chart.Zoom(1e12);
        Flush();
        check(chart.Viewport.Width >= 0.001 && chart.Viewport.Height >= 0.001 && ValidViewport(chart.Viewport),
            $"{label}: extreme zoom respects minimum span and normalized bounds");
        chart.Pan(1e12, -1e12);
        Flush();
        check(ValidViewport(chart.Viewport), $"{label}: extreme pan clamps to data bounds");
        chart.Viewport = new Rect(-2, 3, 2, 0.00001);
        Flush();
        check(ValidViewport(chart.Viewport), $"{label}: bound viewport values are coerced to safe spans and bounds");
        chart.ResetViewport();
        chart.Zoom(2);
        chart.IsPanEnabled = true;
        chart.SelectedIndex = -1;
        Flush();
        center = chart.GetDataPointPosition(0, 2)!.Value;
        location = chart.TranslatePoint(center, window)!.Value;
        prior = chart.Viewport;
        IPointer? pointer = null;
        chart.PointerMoved += (_, args) => pointer = args.Pointer;
        window.MouseMove(location);
        window.MouseDown(location, MouseButton.Left);
        window.MouseMove(location + new Vector(50, 20), RawInputModifiers.LeftMouseButton);
        Flush();
        var captured = pointer?.Captured == chart;
        window.MouseMove(new Point(window.Width + 20, location.Y), RawInputModifiers.LeftMouseButton);
        Flush();
        window.MouseUp(new Point(window.Width + 20, location.Y), MouseButton.Left);
        Flush();
        check(captured && pointer?.Captured is null && chart.Viewport != prior && chart.SelectedIndex == -1,
            $"{label}: real drag captures outside the control, pans and suppresses click selection");
        chart.ResetViewport();
        Flush();
        center = chart.GetDataPointPosition(0, 2)!.Value;
        location = chart.TranslatePoint(center, window)!.Value;
        window.MouseMove(location);
        window.MouseDown(location, MouseButton.Left);
        window.MouseUp(location, MouseButton.Left);
        Flush();
        check(chart.SelectedIndex == 2, $"{label}: clicks still select with drag panning enabled");
        chart.IsPanEnabled = false;
        prior = chart.Viewport;
        window.MouseDown(location, MouseButton.Left);
        window.MouseMove(location + new Vector(80, 20), RawInputModifiers.LeftMouseButton);
        window.MouseUp(location + new Vector(80, 20), MouseButton.Left);
        Flush();
        check(chart.Viewport == prior, $"{label}: disabled pan flag leaves viewport unchanged on drag");
        chart.IsPanEnabled = true;
        chart.Focus();
        Press(window, Key.Add, PhysicalKey.NumPadAdd);
        check(chart.Viewport.Width < 1, $"{label}: plus key zooms the focused chart");
        prior = chart.Viewport;
        Press(window, Key.Right, PhysicalKey.ArrowRight, RawInputModifiers.Control);
        check(chart.Viewport.X > prior.X, $"{label}: Ctrl + Right pans the focused chart");
        Press(window, Key.D0, PhysicalKey.Digit0, RawInputModifiers.Control);
        check(chart.Viewport == full, $"{label}: Ctrl + 0 resets the viewport");
        chart.SelectedIndex = 2;
        chart.Viewport = new Rect(0, 0, 0.1, 0.1);
        prior = chart.Viewport;
        chart.SeriesSource![0].ItemsSource![2].Y += 0.1;
        Flush();
        check(chart.Viewport == prior && chart.GetDataPointPosition(0, 2) is null,
            $"{label}: manual viewport changes and live updates do not snap back to existing selection");
        chart.SeriesSource = new[]
        {
            new ChartSeries
                { ItemsSource = Enumerable.Range(0, 1000).Select(index => new ChartPoint(index, index % 20)).ToArray() }
        };
        chart.NavigationAxes = ChartNavigationAxes.Horizontal;
        chart.Viewport = new Rect(0, 0, 0.1, 1);
        chart.SelectedIndex = -1;
        Flush();
        check(chart.GetDataPointPosition(0, 999) is null, $"{label}: zoomed geometry excludes distant offscreen data");
        Press(window, Key.End, PhysicalKey.End);
        var far = chart.GetDataPointPosition(0, 999);
        check(
            chart.SelectedIndex == 999 && far is { } mark && chart.HitTestDataPoint(mark)?.PointIndex == 999 &&
            chart.Viewport.Right > 0.999 && !chart.GetVisualDescendants().Any(),
            $"{label}: End reveals, hits and selects distant data while keeping zero item controls");
        foreach (var kind in new[] { ChartKind.Pie, ChartKind.Doughnut, ChartKind.Gauge })
        {
            chart = SwitchChart(window, chart, kind);
            prior = chart.Viewport;
            window.MouseWheel(location, new Vector(0, 1));
            Flush();
            check(chart.Viewport == prior, $"{label}/{kind}: radial input does not consume navigation");
        }

        window.Close();
        Flush();
        var initial = new LineChart
        {
            Viewport = new Rect(0, 0, 0.1, 1), SelectedIndex = 4,
            SeriesSource = new[]
            {
                new ChartSeries
                    { ItemsSource = Enumerable.Range(0, 5).Select(index => new ChartPoint(index, index)).ToArray() }
            }
        };
        var mounted = Open(initial, theme, size);
        check(
            initial.Selection?.PointIndex == 4 && initial.GetDataPointPosition(0, 4) is not null &&
            Math.Abs(initial.Viewport.Width - 0.1) < 0.0001,
            $"{label}: a selection set before first layout is revealed without resetting zoom");
        mounted.Close();
        Flush();
    }

    private static bool ValidViewport(Rect viewport)
    {
        return viewport.X >= -0.000001 && viewport.Y >= -0.000001 && viewport.Right <= 1.000001 &&
               viewport.Bottom <= 1.000001 && viewport.Width >= 0.001 && viewport.Height >= 0.001 &&
               viewport.Width <= 1 && viewport.Height <= 1;
    }

    private static void VerifyStacks(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var a = new ChartSeries
        {
            Name = "First",
            ItemsSource = new ObservableCollection<ChartPoint>
                { new(0, 10), new(1, -10), new(2, 20), new(3, double.NaN) }
        };
        var b = new ChartSeries
        {
            Name = "Second",
            ItemsSource = new ObservableCollection<ChartPoint>
                { new(0, 20), new(1, -20), new(2, double.NaN), new(3, 15) }
        };
        var c = new ChartSeries
        {
            Name = "Third",
            ItemsSource = new ObservableCollection<ChartPoint> { new(0, -5), new(1, 5), new(2, -4), new(3, 5) }
        };
        ChartBase chart = new LineChart { SeriesSource = new ObservableCollection<ChartSeries> { a, b, c } };
        var window = Open(chart, theme, size);
        var label = $"Chart/{theme.Key}/{size}/stack";
        foreach (var kind in new[] { ChartKind.StackedBar, ChartKind.StackedHorizontalBar, ChartKind.StackedArea })
        {
            b.IsVisible = true;
            b.ItemsSource = new ObservableCollection<ChartPoint>
                { new(0, 20), new(1, -20), new(2, double.NaN), new(3, 15) };
            chart = SwitchChart(window, chart, kind);
            chart.YMinimum = -40;
            chart.YMaximum = 40;
            Flush();
            var positiveA = chart.GetDataPointPosition(0, 0)!.Value;
            var positiveB = chart.GetDataPointPosition(1, 0)!.Value;
            var negativeA = chart.GetDataPointPosition(0, 1)!.Value;
            var negativeB = chart.GetDataPointPosition(1, 1)!.Value;
            check(
                kind == ChartKind.StackedHorizontalBar
                    ? positiveB.X > positiveA.X && negativeB.X < negativeA.X
                    : positiveB.Y < positiveA.Y && negativeB.Y > negativeA.Y,
                $"{label}/{kind}: positive and negative contributions accumulate separately");
            var oppositeNegative = chart.GetDataPointPosition(2, 0)!.Value;
            var oppositePositive = chart.GetDataPointPosition(2, 1)!.Value;
            var p = kind == ChartKind.StackedHorizontalBar ? positiveA.X : positiveA.Y;
            var n = kind == ChartKind.StackedHorizontalBar ? negativeA.X : negativeA.Y;
            var crossN = kind == ChartKind.StackedHorizontalBar ? oppositeNegative.X : oppositeNegative.Y;
            var crossP = kind == ChartKind.StackedHorizontalBar ? oppositePositive.X : oppositePositive.Y;
            var zero = (p + n) / 2;
            check(Math.Abs(crossN - (zero + (n - zero) / 2)) < 0.5 && Math.Abs(crossP - (zero + (p - zero) / 2)) < 0.5,
                $"{label}/{kind}: opposite-sign contributions each begin at zero instead of cancelling prior stacks");
            check(
                new[] { (0, 0), (1, 0), (0, 1), (1, 1), (2, 0), (2, 1) }.All(index =>
                    chart.GetDataPointPosition(index.Item1, index.Item2) is { } point &&
                    chart.HitTestDataPoint(point) is { } hit && hit.SeriesIndex == index.Item1 &&
                    hit.PointIndex == index.Item2),
                $"{label}/{kind}: each stack segment identifies its original series and point");
            check(
                chart.GetDataPointPosition(0, 3) is null && chart.GetDataPointPosition(1, 2) is null &&
                chart.GetDataPointPosition(1, 3) is not null,
                $"{label}/{kind}: gaps omit contributions while later data remains visible");
            b.IsVisible = false;
            Flush();
            check(chart.GetDataPointPosition(1, 0) is null && chart.GetDataPointPosition(0, 0) == positiveA,
                $"{label}/{kind}: hidden series leaves no mark or contribution");
            // This negative point changes its normalized position when cumulative limits shrink.
            b.IsVisible = true;
            chart.YMinimum = double.NaN;
            chart.YMaximum = double.NaN;
            Flush();
            var autoBefore = chart.GetDataPointPosition(0, 1)!.Value;
            b.IsVisible = false;
            Flush();
            var autoAfter = chart.GetDataPointPosition(0, 1)!.Value;
            check(Distance(autoAfter, autoBefore) > 1,
                $"{label}/{kind}: automatic axis extents follow cumulative visible totals");
            b.IsVisible = true;
            b.ItemsSource![3].X = 99;
            Flush();
            check(
                kind == ChartKind.StackedArea
                    ? chart.GetDataPointPosition(1, 3) is null
                    : chart.GetDataPointPosition(1, 3) is not null,
                $"{label}/{kind}: numeric areas require canonical X; categorical bars align by index");
            b.ItemsSource = new ObservableCollection<ChartPoint> { new(0, 30), new(1, -15), new(2, 12), new(3, 10) };
            Flush();
            check(
                chart.GetDataPointPosition(1, 2) is not null && !chart.GetVisualDescendants().Any() &&
                Frame(window).Length > 0,
                $"{label}/{kind}: reloaded observable stack data rebuilds geometry without item controls");
        }

        window.Close();
        Flush();
    }

    private static void VerifyPercentStacks(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var first = new ChartSeries
        {
            Name = "One", Unit = "kWh", ValueFormat = "0.0",
            ItemsSource = new ObservableCollection<ChartPoint>
                { new(0, 1, "share"), new(1, -2), new(2, 0), new(3, double.MaxValue), new(4, double.NaN), new(5, 2) }
        };
        var second = new ChartSeries
        {
            Name = "Two",
            ItemsSource = new ObservableCollection<ChartPoint>
                { new(0, 2), new(1, -3), new(2, 0), new(3, double.MaxValue), new(4, 4), new(5, 1) }
        };
        var secondaryA = new ChartSeries
        {
            Name = "Secondary one", YAxis = ChartYAxis.Secondary,
            ItemsSource = new ObservableCollection<ChartPoint>
                { new(0, 7), new(1, -8), new(2, 0), new(3, double.MaxValue), new(4, 5), new(5, 9) }
        };
        var secondaryB = new ChartSeries
        {
            Name = "Secondary two", YAxis = ChartYAxis.Secondary,
            ItemsSource = new ObservableCollection<ChartPoint>
                { new(0, 21), new(1, -2), new(2, 0), new(3, double.MaxValue), new(4, 5), new(5, 1) }
        };
        ChartBase chart = new LineChart
        {
            SeriesSource = new ObservableCollection<ChartSeries> { first, second, secondaryA, secondaryB },
            ShowDataLabels = true
        };
        var window = Open(chart, theme, size);
        var label = $"Chart/{theme.Key}/{size}/percent";
        foreach (var kind in new[]
                 {
                     ChartKind.PercentStackedBar, ChartKind.PercentStackedHorizontalBar, ChartKind.PercentStackedArea
                 })
        {
            second.ItemsSource = new ObservableCollection<ChartPoint>
                { new(0, 2), new(1, -3), new(2, 0), new(3, double.MaxValue), new(4, 4), new(5, 1) };
            first.IsVisible = second.IsVisible = secondaryA.IsVisible = secondaryB.IsVisible = true;
            chart = SwitchChart(window, chart, kind);
            Flush();
            check(
                Near(chart.GetStackPercentage(0, 0), 1d / 3) && Near(chart.GetStackPercentage(1, 0), 2d / 3) &&
                Near(chart.GetStackPercentage(0, 1), -0.4) && Near(chart.GetStackPercentage(1, 1), -0.6),
                $"{label}/{kind}: positive and negative shares independently total 100 percent");
            check(
                Near(chart.GetStackPercentage(2, 0), 0.25) && Near(chart.GetStackPercentage(2, 1), -0.8) &&
                Near(chart.GetStackPercentage(3, 1), -0.2),
                $"{label}/{kind}: secondary-axis shares use independent totals");
            check(
                Near(chart.GetStackPercentage(0, 2), 0) && chart.GetStackPercentage(0, 4) is null &&
                Near(chart.GetStackPercentage(0, 3), 0.5) && Near(chart.GetStackPercentage(1, 3), 0.5),
                $"{label}/{kind}: zero, missing and extreme finite shares remain safe");
            secondaryA.IsVisible = secondaryB.IsVisible = false;
            Flush();
            var position = chart.GetDataPointPosition(0, 0)!.Value;
            var location = chart.TranslatePoint(position, window)!.Value;
            window.MouseMove(location);
            Flush();
            check(
                chart.HitTestDataPoint(position) is { SeriesIndex: 0, PointIndex: 0 } &&
                ToolTip.GetTip(chart) is string tooltip && tooltip.Contains("1.0kWh") && tooltip.Contains("33") &&
                tooltip.Contains('%'), $"{label}/{kind}: percentage tooltip retains original value, unit and share");
            var initial = Frame(window);
            chart.YMinimum = -500;
            chart.YMaximum = 500;
            chart.SecondaryYMinimum = -10000;
            chart.SecondaryYMaximum = 10000;
            Flush();
            check(chart.GetDataPointPosition(0, 0) == position,
                $"{label}/{kind}: percent geometry ignores raw numeric axis limits");
            second.IsVisible = false;
            secondaryA.IsVisible = secondaryB.IsVisible = true;
            Flush();
            check(
                Near(chart.GetStackPercentage(0, 0), 1) && chart.GetStackPercentage(1, 0) is null &&
                Near(chart.GetStackPercentage(2, 0), 0.25) && !initial.SequenceEqual(Frame(window)),
                $"{label}/{kind}: visibility updates totals and pixels independently for each axis");
            second.IsVisible = true;
            second.ItemsSource = new ObservableCollection<ChartPoint> { new(0, 3), new(1, -6) };
            Flush();
            check(
                Near(chart.GetStackPercentage(0, 0), 0.25) && Near(chart.GetStackPercentage(0, 1), -0.25) &&
                !chart.GetVisualDescendants().Any(),
                $"{label}/{kind}: reloaded percent source rebuilds cumulative shares without item controls");
        }

        chart = SwitchChart(window, chart, ChartKind.StackedBar);
        Flush();
        check(chart.GetStackPercentage(0, 0) is null, $"{label}: percentage API is limited to percent chart kinds");
        window.Close();
        Flush();
    }

    private static bool Near(double? actual, double expected)
    {
        return actual is { } value && Math.Abs(value - expected) < 0.000001;
    }

    private static void VerifySecondaryAxes(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var primary = new ChartSeries
        {
            Name = "Revenue", ItemsSource = new[] { new ChartPoint(0, 1), new ChartPoint(1, 2), new ChartPoint(2, 3) }
        };
        var secondary = new ChartSeries
        {
            Name = "Visitors", YAxis = ChartYAxis.Secondary,
            ItemsSource = new[] { new ChartPoint(0, 1000), new ChartPoint(1, 2000), new ChartPoint(2, 3000) }
        };
        ChartBase chart = new LineChart
        {
            SeriesSource = new[] { primary, secondary }, Unit = "万元", SecondaryUnit = "人", ValueFormat = "0.0",
            SecondaryValueFormat = "0", YAxisTitle = "销售", SecondaryYAxisTitle = "访问"
        };
        var window = Open(chart, theme, size);
        var label = $"Chart/{theme.Key}/{size}/dual-axis";
        foreach (var kind in new[]
                 {
                     ChartKind.Line, ChartKind.Bar, ChartKind.HorizontalBar, ChartKind.StackedBar,
                     ChartKind.StackedHorizontalBar, ChartKind.StackedArea
                 })
        {
            chart = SwitchChart(window, chart, kind);
            chart.YMinimum = 0;
            chart.YMaximum = 4;
            chart.SecondaryYMinimum = 0;
            chart.SecondaryYMaximum = 4000;
            chart.ShowSecondaryAxis = true;
            Flush();
            var p = chart.GetDataPointPosition(0, 1)!.Value;
            var s = chart.GetDataPointPosition(1, 1)!.Value;
            check(
                kind is ChartKind.HorizontalBar or ChartKind.StackedHorizontalBar
                    ? Math.Abs(p.X - s.X) < 0.5
                    : Math.Abs(p.Y - s.Y) < 0.5,
                $"{label}/{kind}: independent scales align values with very different units");
            var before = Frame(window);
            chart.ShowSecondaryAxis = false;
            Flush();
            var hidden = chart.GetDataPointPosition(1, 1)!.Value;
            check(!before.SequenceEqual(Frame(window)) && double.IsFinite(hidden.X) && double.IsFinite(hidden.Y),
                $"{label}/{kind}: secondary labels can hide while its scale remains applied");
            chart.ShowSecondaryAxis = true;
            chart.SecondaryYMaximum = 8000;
            Flush();
            var limited = chart.GetDataPointPosition(1, 1)!.Value;
            var primaryLimited = chart.GetDataPointPosition(0, 1)!.Value;
            check(
                kind is ChartKind.HorizontalBar or ChartKind.StackedHorizontalBar
                    ? limited.X < primaryLimited.X
                    : limited.Y > primaryLimited.Y,
                $"{label}/{kind}: explicit secondary limits affect only the assigned series");
            secondary.YAxis = ChartYAxis.Primary;
            Flush();
            check(chart.GetDataPointPosition(1, 1) is null || chart.GetDataPointPosition(1, 1) != limited,
                $"{label}/{kind}: observable axis reassignment rebuilds the series geometry");
            secondary.YAxis = ChartYAxis.Secondary;
            chart.SecondaryYMaximum = 4000;
            Flush();
        }

        chart = SwitchChart(window, chart, ChartKind.Line);
        chart.YMinimum = double.NaN;
        chart.YMaximum = double.NaN;
        chart.SecondaryYMinimum = double.NaN;
        chart.SecondaryYMaximum = double.NaN;
        Flush();
        var point = chart.GetDataPointPosition(1, 1)!.Value;
        var location = chart.TranslatePoint(point, window)!.Value;
        window.MouseMove(location);
        Flush();
        check(ToolTip.GetTip(chart) is string axisTip && axisTip.Contains("2000人"),
            $"{label}: tooltip inherits the secondary unit and format");
        secondary.Unit = "visits";
        secondary.ValueFormat = "0.00";
        window.MouseMove(new Point(1, 1));
        window.MouseMove(location);
        Flush();
        check(ToolTip.GetTip(chart) is string seriesTip && seriesTip.Contains("2000.00visits"),
            $"{label}: per-series format and unit override axis defaults");
        var titles = Frame(window);
        chart.YAxisTitle = "收入（万元）";
        chart.SecondaryYAxisTitle = "访问（人）";
        Flush();
        check(!titles.SequenceEqual(Frame(window)), $"{label}: both axis titles are actually rendered");
        chart.IsZoomEnabled = true;
        var center = chart.GetDataPointPosition(1, 1)!.Value;
        location = chart.TranslatePoint(center, window)!.Value;
        window.MouseMove(location);
        window.MouseWheel(location, new Vector(0, 1));
        Flush();
        check(
            chart.GetDataPointPosition(1, 1) is { } anchor &&
            new Vector(anchor.X - center.X, anchor.Y - center.Y).Length < 0.5,
            $"{label}: pointer zoom preserves data on the secondary scale");
        secondary.ItemsSource =
            Enumerable.Range(0, 1000).Select(index => new ChartPoint(index, 1000 + index)).ToArray();
        chart.NavigationAxes = ChartNavigationAxes.Horizontal;
        chart.Viewport = new Rect(0, 0, 0.1, 1);
        chart.SelectedSeriesIndex = 1;
        chart.SelectedIndex = -1;
        chart.Focus();
        Flush();
        Press(window, Key.End, PhysicalKey.End);
        check(
            chart.Selection is { SeriesIndex: 1, PointIndex: 999 } && chart.GetDataPointPosition(1, 999) is not null &&
            !chart.GetVisualDescendants().Any(), $"{label}: far secondary-series selection reveals the original model");
        window.Close();
        Flush();
    }

    private static void VerifySampling(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var points = new ObservableCollection<ChartPoint>(Enumerable.Range(0, 100000).Select(index =>
            new ChartPoint(index,
                index is >= 45000 and < 47000 ? double.NaN : index == 99777 ? 90 : index == 99888 ? -90 : 10,
                $"Original {index}")));
        var source = new ChartSeries
            { Name = "100k", Brush = new SolidColorBrush(Color.Parse("#F01010")), ItemsSource = points };
        ChartBase chart = new LineChart
        {
            SeriesSource = new[] { source }, YMinimum = -100, YMaximum = 100, ShowGrid = false, ShowLegend = false,
            ShowMarkers = false, SamplingThreshold = 2000, SamplingPointBudget = 2000
        };
        var window = Open(chart, theme, size);
        var label = $"Chart/{theme.Key}/{size}/sampling";
        check(
            chart.SamplingActive && chart.RenderedPointCount is > 0 and <= 2016 && !chart.GetVisualDescendants().Any(),
            $"{label}: 100,000 originals draw a bounded sampled vertex count with zero item controls");
        var peak = chart.GetDataPointPosition(0, 99777)!.Value;
        var valley = chart.GetDataPointPosition(0, 99888)!.Value;
        check(RedPixelNear(window, chart, peak) && RedPixelNear(window, chart, valley),
            $"{label}: min/max sampling retains distant sharp peak and valley pixels");
        var beforeGap = chart.GetDataPointPosition(0, 44999)!.Value;
        var afterGap = chart.GetDataPointPosition(0, 47000)!.Value;
        check(
            chart.GetDataPointPosition(0, 46000) is null && !RedPixelNear(window, chart,
                new Point((beforeGap.X + afterGap.X) / 2, beforeGap.Y)),
            $"{label}: broad missing interval remains a visual gap without a joining segment");
        var omitted = chart.GetDataPointPosition(0, 54321)!.Value;
        check(chart.HitTestDataPoint(omitted) is { PointIndex: 54321 } hit && ReferenceEquals(hit.Point, points[54321]),
            $"{label}: omitted originals retain exact hit identity");
        chart.ShowCrosshair = true;
        chart.TooltipMode = ChartTooltipMode.Shared;
        Move(window, chart, omitted);
        check(
            chart.HoveredPoints.Count == 1 && chart.HoveredPoints[0].Point == points[54321] &&
            chart.GetSharedDataPoints(omitted)[0].PointIndex == 54321 && chart.CrosshairPosition is not null,
            $"{label}: sampled shared hover and crosshair use the omitted original model");
        chart.SelectedIndex = 54321;
        Flush();
        check(chart.Selection?.Point == points[54321] && chart.RenderedPointCount <= 2017,
            $"{label}: selected original point is retained without expanding the full source");
        var before = Frame(window);
        points[54321].Y = 70;
        Flush();
        var changed = chart.GetDataPointPosition(0, 54321)!.Value;
        check(!before.SequenceEqual(Frame(window)) && RedPixelNear(window, chart, changed),
            $"{label}: source updates invalidate the sampled cache and render the selected peak");
        chart.SamplingPointBudget = 512;
        Flush();
        check(chart.SamplingActive && chart.RenderedPointCount <= 528,
            $"{label}: changed per-series budget rebuilds the prepared vertices");
        var oldX = changed.X;
        window.Width = 480;
        Flush();
        var resized = chart.GetDataPointPosition(0, 54321)!.Value;
        check(
            resized.X != oldX && chart.HitTestDataPoint(resized)?.Point == points[54321] &&
            chart.RenderedPointCount <= 528,
            $"{label}: resize rebuilds transforms while preserving sampled hit identity");
        chart.NavigationAxes = ChartNavigationAxes.Horizontal;
        chart.Zoom(4);
        chart.BringDataPointIntoView(0, 54321);
        Flush();
        check(
            chart.GetDataPointPosition(0, 54321) is { } zoomed && chart.HitTestDataPoint(zoomed)?.PointIndex == 54321 &&
            chart.RenderedPointCount <= 528, $"{label}: zoom rebuilds sampled geometry with exact original selection");
        var oldTheme = Frame(window);
        window.RequestedThemeVariant = theme == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light;
        Flush();
        check(!oldTheme.SequenceEqual(Frame(window)) && chart.Selection?.Point == points[54321],
            $"{label}: theme changes refresh the sampled rendering without changing the source model");
        chart.ResetViewport();
        chart.SamplingMode = ChartSamplingMode.None;
        Flush();
        check(!chart.SamplingActive && chart.RenderedPointCount >= 97000,
            $"{label}: disabling sampling draws the original finite vertices");
        chart.SamplingMode = ChartSamplingMode.MinMax;
        points[30000].X = -10;
        points[50000].X = -10;
        Flush();
        check(!chart.SamplingActive && chart.RenderedPointCount >= 97000,
            $"{label}: unsorted X falls back to exact source order");
        points[30000].X = 30000;
        points[50000].X = 50000;
        Flush();
        chart.SamplingThreshold = 200000;
        Flush();
        check(!chart.SamplingActive && chart.RenderedPointCount >= 97000,
            $"{label}: threshold controls whether reduction activates");
        chart.SamplingThreshold = 2000;
        foreach (var kind in new[]
                     { ChartKind.Area, ChartKind.Scatter, ChartKind.StackedArea, ChartKind.PercentStackedArea })
        {
            chart = SwitchChart(window, chart, kind);
            chart.SelectedIndex = -1;
            Flush();
            check(
                chart.SamplingActive && chart.RenderedPointCount is > 0 and <= 544 &&
                chart.GetDataPointPosition(0, 99777) is not null && !chart.GetVisualDescendants().Any(),
                $"{label}/{kind}: numeric sampling reduces drawing vertices while retaining original positions and gaps");
        }

        window.Close();
        Flush();
    }

    private static void VerifySmoothCurve(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var source = new ChartSeries
        {
            Brush = new SolidColorBrush(Color.Parse("#F01010")),
            ItemsSource = new[]
                { new ChartPoint(0, 20), new ChartPoint(1, 70), new ChartPoint(3, 40), new ChartPoint(4, 90) }
        };
        ChartBase chart = new LineChart
            { SeriesSource = new[] { source }, ShowGrid = false, ShowMarkers = false, YMinimum = 0, YMaximum = 100 };
        var window = Open(chart, theme, size);
        var label = $"Chart/{theme.Key}/{size}/smooth";
        foreach (var kind in new[] { ChartKind.Line, ChartKind.Area })
        {
            chart = SwitchChart(window, chart, kind);
            chart.IsSmooth = false;
            Flush();
            var linear = Frame(window);
            var original = chart.GetDataPointPosition(0, 1)!.Value;
            chart.IsSmooth = true;
            Flush();
            check(
                !linear.SequenceEqual(Frame(window)) && chart.GetDataPointPosition(0, 1) == original &&
                chart.HitTestDataPoint(original)?.Point == source.ItemsSource![1],
                $"{label}/{kind}: cubic rendering changes curve pixels while preserving original point positions and hits");
        }

        chart = SwitchChart(window, chart, ChartKind.Line);
        foreach (var x in new[] { new[] { 0d, 2, 1, 3 }, new[] { 0d, 1, 1, 3 } })
        {
            source.ItemsSource = x.Select((value, index) => new ChartPoint(value, new[] { 20d, 70, 40, 90 }[index]))
                .ToArray();
            chart.IsSmooth = false;
            Flush();
            var exact = Frame(window);
            chart.IsSmooth = true;
            Flush();
            check(exact.SequenceEqual(Frame(window)),
                $"{label}: unsorted or duplicate X keeps the exact linear connection order");
        }

        source.ItemsSource = new[]
        {
            new ChartPoint(0, 20), new ChartPoint(1, 70), new ChartPoint(2, double.NaN), new ChartPoint(3, 40),
            new ChartPoint(4, 90)
        };
        Flush();
        check(chart.GetDataPointPosition(0, 2) is null && Frame(window).Length > 0,
            $"{label}: smooth segments preserve non-finite gaps");
        chart = SwitchChart(window, chart, ChartKind.StackedArea);
        chart.IsSmooth = false;
        Flush();
        var stacked = Frame(window);
        chart.IsSmooth = true;
        Flush();
        check(stacked.SequenceEqual(Frame(window)), $"{label}: stacked bands retain straight shared boundaries");
        window.Close();
        Flush();
    }

    private static void VerifyUpdateAnimation(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var point = new ChartPoint(1, 20);
        var source = new ChartSeries { ItemsSource = new[] { new ChartPoint(0, 10), point, new ChartPoint(2, 40) } };
        ChartBase chart = new LineChart
        {
            SeriesSource = new[] { source }, YMinimum = 0, YMaximum = 100, IsAnimationEnabled = true,
            AnimationDuration = TimeSpan.FromSeconds(2)
        };
        var window = Open(chart, theme, size);
        Motion.SetEnabled(window, true);
        Flush();
        var label = $"Chart/{theme.Key}/{size}/update-animation";
        var old = Frame(window);
        point.Y = 80;
        Flush();
        AdvanceAnimation(120);
        var middle = Frame(window);
        check(chart.IsAnimating && chart.AnimationProgress is > 0 and < 1 && !old.SequenceEqual(middle),
            $"{label}: actual intermediate Skia pixels and progress change during a data update");
        point.Y = 35;
        source.ItemsSource![2].Y = 65;
        Flush();
        AdvanceAnimation(120);
        var retargeted = Frame(window);
        check(chart.IsAnimating && !middle.SequenceEqual(retargeted),
            $"{label}: consecutive updates retarget from the current displayed data frame");
        Motion.SetEnabled(window, false);
        Flush();
        var settled = Frame(window);
        check(
            !chart.IsAnimating && chart.AnimationProgress == 1 && !retargeted.SequenceEqual(settled) &&
            chart.GetDataPointPosition(0, 1) is { } target && chart.HitTestDataPoint(target)?.Point == point,
            $"{label}: global motion off immediately displays the latest target and stable original hit model");
        Motion.SetEnabled(window, true);
        chart.AnimationDuration = TimeSpan.Zero;
        point.Y = 75;
        Flush();
        check(!chart.IsAnimating && chart.AnimationProgress == 1,
            $"{label}: zero duration performs an immediate target update");
        chart.AnimationDuration = TimeSpan.FromSeconds(2);
        chart.IsAnimationEnabled = false;
        point.Y = 45;
        Flush();
        check(!chart.IsAnimating, $"{label}: per-chart animation preference disables interpolation");
        chart.IsAnimationEnabled = true;
        source.ItemsSource = new[]
            { new ChartPoint(0, 25), new ChartPoint(1, 55), new ChartPoint(2, 35), new ChartPoint(3, 75) };
        Flush();
        AdvanceAnimation(100);
        check(chart.IsAnimating && Frame(window).Length > 0,
            $"{label}: collection replacement uses a real bounded data-layer transition");
        chart.GetDataPointPosition(0, 1);
        Flush();
        check(!chart.IsAnimating && chart.Selection is null,
            $"{label}: interaction queries finish animation before returning target geometry");
        Motion.SetEnabled(window, false);
        var dense = Enumerable.Range(0, 100000).Select(index => new ChartPoint(index, index == 99777 ? 90 : 10))
            .ToArray();
        source.ItemsSource = dense;
        chart.SamplingPointBudget = 512;
        Flush();
        Motion.SetEnabled(window, true);
        dense[99777].Y = 60;
        Flush();
        AdvanceAnimation(100);
        check(
            chart.IsAnimating && chart.SamplingActive && chart.RenderedPointCount is > 0 and <= 528 &&
            !chart.GetVisualDescendants().Any(),
            $"{label}: a sampled 100,000-point update animates a bounded prepared frame without item controls");
        Motion.SetEnabled(window, false);
        Flush();
        chart.SamplingMode = ChartSamplingMode.None;
        Flush();
        Motion.SetEnabled(window, true);
        dense[99777].Y = 70;
        Flush();
        check(!chart.IsAnimating && chart.RenderedPointCount >= 99000,
            $"{label}: oversized exact data uses immediate rendering instead of an unbounded animation layer");
        window.Close();
        Flush();
        foreach (var kind in new[] { ChartKind.Pie, ChartKind.Doughnut })
        {
            var radialSource = new ChartSeries
                { ItemsSource = Enumerable.Range(0, 4).Select(index => new ChartPoint(index, 1)).ToArray() };
            var radialChart = CreateControl(kind);
            radialChart.SeriesSource = new[] { radialSource };
            radialChart.ShowLegend = false;
            radialChart.ShowDataLabels = false;
            radialChart.AnimationDuration = TimeSpan.FromSeconds(2);
            var radialWindow = Open(radialChart, theme, size);
            // Resolve probes before the update: stable geometry queries intentionally finish an active animation.
            var resting = Enumerable.Range(0, 4).Select(index => radialChart.GetDataPointPosition(0, index)!.Value)
                .ToArray();
            var center = new Point(resting.Average(position => position.X), resting.Average(position => position.Y));
            var radius = new Vector(resting[0].X - center.X, resting[0].Y - center.Y).Length;
            var angularProbe = center + new Vector(Math.Cos(Math.PI / 8), Math.Sin(Math.PI / 8)) * radius;
            var firstColor = PixelAt(radialWindow, radialChart, resting[0]);
            var oldColor = PixelAt(radialWindow, radialChart, angularProbe);
            var before = Frame(radialWindow);
            Motion.SetEnabled(radialWindow, true);
            radialSource.ItemsSource![0].Y = 7;
            Flush();
            AdvanceAnimation(350);
            var intermediate = Frame(radialWindow);
            check(
                radialChart.IsAnimating && radialChart.AnimationProgress is > 0 and < 1 && oldColor != firstColor &&
                PixelAt(radialWindow, radialChart, angularProbe) == firstColor && !before.SequenceEqual(intermediate),
                $"{label}/{kind}: an intermediate moving sector boundary replaces the adjacent slice with its exact fill color");
            var targetExport = ExportBytes(radialChart);
            var exportPreservedAnimation = radialChart.IsAnimating;
            Motion.SetEnabled(radialWindow, false);
            Flush();
            check(
                exportPreservedAnimation && !radialChart.IsAnimating && radialChart.AnimationProgress == 1 &&
                !intermediate.SequenceEqual(Frame(radialWindow)) &&
                targetExport.SequenceEqual(ExportBytes(radialChart)),
                $"{label}/{kind}: the final angular frame settles and animation-time export already contains those target pixels");
            radialWindow.Close();
            Flush();
        }

        var gauge = new GaugeChart
        {
            Value = 20, ShowGaugeNeedle = true, ShowGaugeTicks = true,
            AccentBrush = new SolidColorBrush(Color.Parse("#F01010")), AnimationDuration = TimeSpan.FromSeconds(2)
        };
        var gaugeWindow = Open(gauge, theme, size);
        var gaugeCenter = new Point(gauge.Bounds.Width / 2, gauge.Bounds.Height / 2);
        var gaugeSide =
            (int)Math.Ceiling(Math.Max(gauge.Bounds.Width, gauge.Bounds.Height) * gaugeWindow.RenderScaling) + 4;
        var initialGaugePixels = RedPixelCount(gaugeWindow, gauge, gaugeCenter, gaugeSide);
        var initialGaugeFrame = Frame(gaugeWindow);
        Motion.SetEnabled(gaugeWindow, true);
        gauge.Value = 80;
        Flush();
        AdvanceAnimation(350);
        var intermediateGaugePixels = RedPixelCount(gaugeWindow, gauge, gaugeCenter, gaugeSide);
        var intermediateGaugeFrame = Frame(gaugeWindow);
        check(
            gauge.IsAnimating && gauge.AnimationProgress is > 0 and < 1 &&
            intermediateGaugePixels > initialGaugePixels && !initialGaugeFrame.SequenceEqual(intermediateGaugeFrame),
            $"{label}/Gauge: Value changes draw a genuinely growing colored arc and moving needle during the intermediate frame");
        var gaugeTargetExport = ExportBytes(gauge);
        var gaugeExportPreservedAnimation = gauge.IsAnimating;
        Motion.SetEnabled(gaugeWindow, false);
        Flush();
        check(
            gaugeExportPreservedAnimation && !gauge.IsAnimating && gauge.AnimationProgress == 1 &&
            RedPixelCount(gaugeWindow, gauge, gaugeCenter, gaugeSide) > intermediateGaugePixels &&
            !intermediateGaugeFrame.SequenceEqual(Frame(gaugeWindow)) &&
            gaugeTargetExport.SequenceEqual(ExportBytes(gauge)),
            $"{label}/Gauge: the final Value completes the arc and needle, and export preserves the final target while animation is active");
        gaugeWindow.Close();
        Flush();
    }

    private static void VerifyLineLabels(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var chart = (LineChart)CreateSample(ChartKind.Line);
        chart.IsSmooth = true;
        chart.ShowDataLabels = false;
        var window = Open(chart, theme, size);
        var label = $"Chart/{theme.Key}/{size}/line-labels";
        var first = chart.GetDataPointPosition(0, 0)!.Value;
        var last = chart.GetDataPointPosition(0, 5)!.Value;
        var brush = (SolidColorBrush)chart.AccentBrush!;
        check(
            PixelAt(window, chart, first + new Vector(-chart.MarkerRadius * 0.5, 0)) == brush.Color &&
            PixelAt(window, chart, last + new Vector(chart.MarkerRadius * 0.5, 0)) == brush.Color,
            $"{label}: first and last markers retain both halves of their original filled circles");
        check(
            chart.HitTestDataPoint(first + new Vector(-chart.MarkerRadius * 0.5, 0))?.PointIndex == 0 &&
            chart.HitTestDataPoint(last + new Vector(chart.MarkerRadius * 0.5, 0))?.PointIndex == 5,
            $"{label}: the newly visible edge portions keep their original hit identity");
        chart.TooltipMode = ChartTooltipMode.Shared;
        chart.ShowCrosshair = true;
        Move(window, chart, first + new Vector(-chart.MarkerRadius * 0.5, 0));
        check(chart.HoveredPoints.Count == 2 && chart.CrosshairPosition is null,
            $"{label}: shared tooltip still works on the complete edge marker without drawing crosshairs outside the axes");
        window.MouseMove(new Point(1, 1));
        Flush();
        using var before = window.CaptureRenderedFrame()!;
        chart.ShowDataLabels = true;
        Flush();
        var bounds = new List<Rect>();
        for (var series = 0; series < 2; series++)
        for (var index = 0; index < 6; index++)
        {
            var annotation = chart.GetDataLabelBounds(series, index);
            check(annotation is not null,
                $"{label}/{series}/{index}: original monthly value has a complete visible label");
            if (annotation is { } rect) bounds.Add(rect);
        }

        check(
            bounds.Count == 12 && !bounds
                .Where((rect, index) => bounds.Skip(index + 1).Any(other => rect.Intersects(other))).Any(),
            $"{label}: all twelve labels remain separate including the nearby 51 / 49 and 68 / 61 pairs");
        using var after = window.CaptureRenderedFrame()!;
        check(
            bounds.All(rect => LabelChangedPixels(before, after, chart.TranslatePoint(rect.TopLeft, window)!.Value,
                rect.Size, window.RenderScaling)),
            $"{label}: each reported label rectangle contains actual newly rendered glyph pixels");
        chart.ShowDataLabels = false;
        Flush();
        check(chart.GetDataLabelBounds(0, 0) is null, $"{label}: disabling annotations removes their layout bounds");
        window.Close();
        Flush();
    }

    private static bool LabelChangedPixels(Bitmap before, Bitmap after, Point origin, Size size, double scale)
    {
        var left = Math.Max(0, (int)Math.Floor(origin.X * scale));
        var top = Math.Max(0, (int)Math.Floor(origin.Y * scale));
        var right = Math.Min(after.PixelSize.Width, (int)Math.Ceiling((origin.X + size.Width) * scale));
        var bottom = Math.Min(after.PixelSize.Height, (int)Math.Ceiling((origin.Y + size.Height) * scale));
        for (var y = top; y < bottom; y++)
        for (var x = left; x < right; x++)
            if (BitmapPixel(before, x, y) != BitmapPixel(after, x, y))
                return true;
        return false;
    }

    private static byte[] ExportBytes(ChartBase chart, Size? size = null, double dpi = 96, bool transparent = false)
    {
        using var output = new MemoryStream();
        chart.ExportPng(output, size, dpi, transparent);
        return output.ToArray();
    }

    private static void VerifyExport(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var chart = CreateSample(ChartKind.Line);
        chart.ShowCrosshair = true;
        chart.TooltipMode = ChartTooltipMode.Shared;
        var window = Open(chart, theme, size);
        var label = $"Chart/{theme.Key}/{size}/export";
        var clean = ExportBytes(chart);
        using (var image = new Bitmap(new MemoryStream(clean)))
        {
            check(
                image.PixelSize == new PixelSize((int)Math.Ceiling(chart.Bounds.Width),
                    (int)Math.Ceiling(chart.Bounds.Height)),
                $"{label}: default PNG contains only the chart's logical bounds");
        }

        var hover = chart.GetDataPointPosition(0, 2)!.Value;
        Move(window, chart, hover);
        chart.Focus();
        chart.SelectedIndex = 2;
        ToolTip.SetShouldUseOverlayLayer(chart, true);
        ToolTip.SetIsOpen(chart, true);
        Flush();
        var hovered = chart.CrosshairPosition;
        var selected = chart.Selection;
        var focused = chart.IsFocused;
        check(
            clean.SequenceEqual(ExportBytes(chart)) && chart.CrosshairPosition == hovered &&
            chart.Selection == selected && chart.IsFocused == focused && ToolTip.GetIsOpen(chart),
            $"{label}: exported pixels exclude tooltip, crosshair, selection and focus while leaving the live control untouched");
        var scaled = ExportBytes(chart, new Size(321, 123), 144);
        using (var image = new Bitmap(new MemoryStream(scaled)))
        {
            check(image.PixelSize == new PixelSize(482, 185),
                $"{label}: explicit logical size and DPI produce correctly scaled PNG pixels");
        }

        using (var image = new Bitmap(new MemoryStream(ExportBytes(chart, new Size(0.01, 0.01), 16))))
        {
            check(image.PixelSize == new PixelSize(1, 1),
                $"{label}: tiny positive logical dimensions retain the minimum one-pixel PNG allocation");
        }

        var transparent = ExportBytes(chart, new Size(321, 123), 96, true);
        using (var image = new Bitmap(new MemoryStream(transparent)))
        {
            check(BitmapPixel(image, 0, 0).A == 0 && transparent.Length > 1000,
                $"{label}: transparent PNG removes canvas fill while retaining rendered chart content");
        }

        check(
            RejectedExport(() => ExportBytes(chart, new Size(0, 100))) &&
            RejectedExport(() => ExportBytes(chart, new Size(100, 100), 1000)) &&
            RejectedExport(() => ExportBytes(chart, new Size(8193, 1))) &&
            RejectedExport(() => ExportBytes(chart, new Size(5000, 5000))),
            $"{label}: invalid DPI, zero dimensions and oversized pixel allocations are rejected before rendering");
        var file = Path.Combine(Path.GetTempPath(), $"chart-export-{Guid.NewGuid():N}.png");
        try
        {
            chart.ExportPng(file, new Size(160, 100));
            using var image = new Bitmap(file);
            check(image.PixelSize == new PixelSize(160, 100),
                $"{label}: path export saves a real PNG with the requested size");
        }
        finally
        {
            if (File.Exists(file)) File.Delete(file);
        }

        ToolTip.SetIsOpen(chart, false);
        chart.SelectedIndex = -1;
        window.Focus();
        window.MouseMove(new Point(1, 1));
        Flush();
        Motion.SetEnabled(window, true);
        chart.AnimationDuration = TimeSpan.FromSeconds(2);
        chart.SeriesSource![0].ItemsSource![2].Y = 95;
        Flush();
        AdvanceAnimation(100);
        var wasAnimating = chart.IsAnimating;
        var progress = chart.AnimationProgress;
        var finalTarget = ExportBytes(chart);
        var afterExport = chart.IsAnimating;
        check(wasAnimating && afterExport && chart.AnimationProgress >= progress,
            $"{label}: target-state export preserves an in-flight live animation");
        Motion.SetEnabled(window, false);
        Flush();
        check(finalTarget.SequenceEqual(ExportBytes(chart)),
            $"{label}: animation-time PNG already contains the settled target pixels");
        window.Close();
        Flush();
    }

    private static bool RejectedExport(Action action)
    {
        try
        {
            action();
            return false;
        }
        catch (ArgumentException)
        {
            return true;
        }
    }

    private static void VerifyGraphicSizing(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        ChartBase chart = new ScatterChart
        {
            ShowLegend = false, ShowGrid = false, YMinimum = 0, YMaximum = 20,
            HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top,
            SeriesSource = new[]
            {
                new ChartSeries
                {
                    Brush = new SolidColorBrush(Color.Parse("#F01010")), ItemsSource = new[] { new ChartPoint(0, 10) }
                }
            }
        };
        var window = new Window
        {
            Width = 640, Height = 440, Content = new Border { Padding = new Thickness(16), Child = chart },
            RequestedThemeVariant = theme, WindowDecorations = WindowDecorations.None
        };
        ControlSizing.SetSize(window, size);
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        var label = $"Chart/{theme.Key}/{size}/graphic-sizing";
        var expected = size switch
        {
            ControlSize.Small => new Size(320, 224), ControlSize.Large => new Size(480, 336),
            _ => new Size(400, 280)
        };
        check(
            chart.Bounds.Size == expected && chart.DesiredSize == expected && double.IsNaN(chart.Height) &&
            double.IsNaN(chart.Width), $"{label}: unconstrained charts use density-specific natural dimensions");
        check(
            chart.ChartPadding == (size == ControlSize.Small ? 8 : size == ControlSize.Large ? 16 : 12) &&
            chart.StrokeThickness == (size == ControlSize.Small ? 2 : size == ControlSize.Large ? 3 : 2.5) &&
            chart.MarkerRadius == (size == ControlSize.Small ? 3 : size == ControlSize.Large ? 4.25 : 3.5) &&
            chart.SelectionThickness == (size == ControlSize.Small ? 1.5 : size == ControlSize.Large ? 2.5 : 2),
            $"{label}: padding, line width, markers and emphasis all follow density");
        var mark = chart.GetDataPointPosition(0, 0)!.Value;
        var pixels = RedPixelCount(window, chart, mark, 24);
        var originalRadius = chart.MarkerRadius;
        chart.MarkerRadius = originalRadius * 1.5;
        Flush();
        check(RedPixelCount(window, chart, chart.GetDataPointPosition(0, 0)!.Value, 24) > pixels,
            $"{label}: marker-radius metrics change actual painted glyph pixels");
        check(
            chart.GaugeBandRatio == (size == ControlSize.Small ? 0.14 : size == ControlSize.Large ? 0.22 : 0.18) &&
            chart.SliceHoverOffset == (size == ControlSize.Small ? 4 : size == ControlSize.Large ? 8 : 6),
            $"{label}: gauge bands and radial lift use the same density scale");
        window.Close();
        Flush();
    }

    private static void VerifyShapeHoverAndGauge(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        ChartBase chart = new BarChart
        {
            ShowCrosshair = false, ShowGrid = false, YMinimum = 0, YMaximum = 100,
            SeriesSource = new[]
                { new ChartSeries { ItemsSource = new[] { new ChartPoint(0, 60), new ChartPoint(1, 0) } } }
        };
        var window = Open(chart, theme, size);
        var label = $"Chart/{theme.Key}/{size}/shape-hover";
        var bar = chart.GetDataPointPosition(0, 0)!.Value;
        var baseline = chart.GetDataPointPosition(0, 1)!.Value.Y;
        var top = new Point(bar.X, 2 * bar.Y - baseline + 0.5);
        var edgeBefore = PixelAt(window, chart, top);
        var centerBefore = PixelAt(window, chart, bar + new Vector(5, 0));
        Move(window, chart, bar);
        check(
            PixelAt(window, chart, top) != edgeBefore && PixelAt(window, chart, bar + new Vector(5, 0)) == centerBefore,
            $"{label}: bar hover emphasizes the entire visible rectangle without an inner point circle");
        foreach (var kind in new[] { ChartKind.Pie, ChartKind.Doughnut })
        {
            chart = SwitchChart(window, chart, kind);
            chart.SeriesSource = new[]
            {
                new ChartSeries
                    { ItemsSource = Enumerable.Range(0, 4).Select(index => new ChartPoint(index, 1)).ToArray() }
            };
            window.MouseMove(new Point(1, 1));
            Flush();
            var resting = Enumerable.Range(0, 4).Select(index => chart.GetDataPointPosition(0, index)!.Value).ToArray();
            var center = new Point(resting.Average(point => point.X), resting.Average(point => point.Y));
            var before = Frame(window);
            Move(window, chart, resting[0]);
            var raised = chart.GetDataPointPosition(0, 0)!.Value;
            check(
                new Vector(raised.X - resting[0].X, raised.Y - resting[0].Y).Length >= chart.SliceHoverOffset * 0.5 &&
                !before.SequenceEqual(Frame(window)) && chart.HitTestDataPoint(raised)?.PointIndex == 0 &&
                chart.HitTestDataPoint(resting[0])?.PointIndex == 0,
                $"{label}/{kind}: the full raised sector changes pixels and preserves lifted/resting hit identity");
            if (kind == ChartKind.Doughnut)
                check(chart.HitTestDataPoint(center) is null,
                    $"{label}: raising a doughnut slice leaves the complete original hole empty");
        }

        chart = SwitchChart(window, chart, ChartKind.Gauge);
        chart.Value = 65;
        chart.ShowGaugeNeedle = false;
        chart.ShowGaugeTicks = false;
        Flush();
        var plain = Frame(window);
        chart.ShowGaugeNeedle = true;
        Flush();
        var needle = Frame(window);
        chart.ShowGaugeTicks = true;
        Flush();
        check(!plain.SequenceEqual(needle) && !needle.SequenceEqual(Frame(window)) && chart.Selection is null,
            $"{label}: opt-in gauge needle and ticks each change actual rendered geometry");
        chart.GaugeMajorTickCount = 12;
        chart.GaugeMinorTickCount = 10;
        chart.Width = 180;
        chart.Height = 160;
        Flush();
        check(Frame(window).Length > 0 && chart.GetDataPointPosition(0, 0) is null,
            $"{label}: dense gauge ticks remain safe in a compact viewport without point selection");
        window.Close();
        Flush();
    }

    private static Color BitmapPixel(Bitmap bitmap, int x, int y)
    {
        var bytes = new byte[4];
        var pin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            bitmap.CopyPixels(new PixelRect(x, y, 1, 1), pin.AddrOfPinnedObject(), 4, 4);
        }
        finally
        {
            pin.Free();
        }

        return bitmap.Format == PixelFormat.Rgba8888
            ? Color.FromArgb(bytes[3], bytes[0], bytes[1], bytes[2])
            : Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
    }

    private static int RedPixelCount(Window window, ChartBase chart, Point local, int side)
    {
        using var bitmap = window.CaptureRenderedFrame()!;
        var point = chart.TranslatePoint(local, window)!.Value;
        var x = (int)(point.X * window.RenderScaling);
        var y = (int)(point.Y * window.RenderScaling);
        var count = 0;
        var left = Math.Max(0, x - side / 2);
        var top = Math.Max(0, y - side / 2);
        var width = Math.Min(bitmap.PixelSize.Width, x + side / 2) - left;
        var height = Math.Min(bitmap.PixelSize.Height, y + side / 2) - top;
        if (width <= 0 || height <= 0) return 0;
        var bytes = new byte[width * height * 4];
        var pin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            bitmap.CopyPixels(new PixelRect(left, top, width, height), pin.AddrOfPinnedObject(), bytes.Length,
                width * 4);
        }
        finally
        {
            pin.Free();
        }

        var red = bitmap.Format == PixelFormat.Rgba8888 ? 0 : 2;
        var blue = 2 - red;
        for (var offset = 0; offset < bytes.Length; offset += 4)
            if (bytes[offset + red] > 180 && bytes[offset + 1] < 80 && bytes[offset + blue] < 80)
                count++;
        return count;
    }

    private static void VerifySharedHover(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var primary = new ChartSeries
        {
            Name = "Energy",
            ItemsSource = new[]
                { new ChartPoint(0, 10), new ChartPoint(1, 20), new ChartPoint(2, 30), new ChartPoint(3, 40) }
        };
        var secondary = new ChartSeries
        {
            Name = "Visitors", YAxis = ChartYAxis.Secondary,
            ItemsSource = new[]
            {
                new ChartPoint(0, 1000), new ChartPoint(1, 2000), new ChartPoint(2, double.NaN), new ChartPoint(4, 4000)
            }
        };
        var duplicates = new ChartSeries
        {
            Name = "Duplicate X",
            ItemsSource = new[]
            {
                new ChartPoint(1, 99, "first duplicate"), new ChartPoint(1, 88, "second duplicate"),
                new ChartPoint(3, 15)
            }
        };
        ChartBase chart = new LineChart
        {
            SeriesSource = new[] { primary, secondary, duplicates }, ShowCrosshair = true,
            TooltipMode = ChartTooltipMode.Shared, Unit = "kWh", SecondaryUnit = "人", SecondaryValueFormat = "0.0"
        };
        var window = Open(chart, theme, size);
        var label = $"Chart/{theme.Key}/{size}/shared-hover";
        var point = chart.GetDataPointPosition(0, 1)!.Value;
        var before = Frame(window);
        Move(window, chart, point);
        check(
            chart.CrosshairPosition is { } crosshair && NearPoint(crosshair, point) && chart.HoveredPoints.Count == 3 &&
            chart.HoveredPoints.Any(hit => hit.Point == duplicates.ItemsSource![0] && hit.PointIndex == 0),
            $"{label}: real hover draws the crosshair and shares exact X with the first valid duplicate");
        check(
            !before.SequenceEqual(Frame(window)) && ToolTip.GetTip(chart) is string tip && tip.Contains("20kWh") &&
            tip.Contains("2000.0人") && tip.Contains("first duplicate"),
            $"{label}: shared tooltip renders each original value with its assigned axis unit and format");
        var aligned = chart.GetSharedDataPoints(new Point(point.X, point.Y + 12));
        check(aligned.Count == 3 && aligned.Any(hit => hit.Point == primary.ItemsSource![1]),
            $"{label}: nearest X works away from a data mark without interpolation");
        point = chart.GetDataPointPosition(0, 2)!.Value;
        Move(window, chart, point);
        check(chart.HoveredPoints.Count == 1 && chart.HoveredPoints[0].Point == primary.ItemsSource![2],
            $"{label}: a missing value and absent exact X produce no invented shared rows");
        chart.NavigationAxes = ChartNavigationAxes.Horizontal;
        chart.Zoom(2);
        chart.BringDataPointIntoView(0, 1);
        Flush();
        point = chart.GetDataPointPosition(0, 1)!.Value;
        Move(window, chart, point);
        check(chart.HoveredPoints.Count == 3 && chart.HoveredPoints.Any(hit => hit.Point == secondary.ItemsSource![1]),
            $"{label}: zoomed shared hover preserves source identities and secondary scales");
        chart.ResetViewport();
        Flush();
        point = chart.GetDataPointPosition(0, 1)!.Value;
        chart.Viewport = new Rect(0, 0, 1, 0.1);
        Flush();
        check(
            chart.GetDataPointPosition(2, 0) is null &&
            chart.GetSharedDataPoints(point).Any(hit => hit.Point == duplicates.ItemsSource![0]),
            $"{label}: shared rows include exact-X values clipped by the vertical viewport");
        chart.ResetViewport();
        chart.TooltipMode = ChartTooltipMode.Single;
        Flush();
        point = chart.GetDataPointPosition(0, 1)!.Value;
        Move(window, chart, point);
        check(chart.HoveredPoints.Count == 1 && chart.HoveredPoints[0].Point == primary.ItemsSource![1],
            $"{label}: single tooltip mode exposes only the directly hit model");
        chart.ShowCrosshair = false;
        chart.TooltipMode = ChartTooltipMode.Shared;
        Flush();
        Move(window, chart, point + new Vector(1, 0));
        check(chart.CrosshairPosition is null && chart.HoveredPoints.Count == 3,
            $"{label}: crosshair visibility can be disabled without losing shared rows");
        chart.ShowCrosshair = true;
        foreach (var kind in new[]
                 {
                     ChartKind.Bar, ChartKind.HorizontalBar, ChartKind.StackedBar, ChartKind.StackedHorizontalBar,
                     ChartKind.PercentStackedBar, ChartKind.PercentStackedHorizontalBar
                 })
        {
            chart = SwitchChart(window, chart, kind);
            Flush();
            point = chart.GetDataPointPosition(0, 1)!.Value;
            var solidProbe = point +
                             (kind is ChartKind.HorizontalBar or ChartKind.StackedHorizontalBar
                                 or ChartKind.PercentStackedHorizontalBar
                                 ? new Vector(10, 0)
                                 : new Vector(0, 10));
            var solidBefore = PixelAt(window, chart, solidProbe);
            Move(window, chart, point);
            check(chart.HoveredPoints.Count == 3 && chart.HoveredPoints.All(hit => hit.PointIndex == 1),
                $"{label}/{kind}: category sharing aligns source index using the displayed category axis");
            check(PixelAt(window, chart, solidProbe) != solidBefore,
                $"{label}/{kind}: crosshair line and halo visibly change pixels inside a solid bar away from the hover marker");
            if (kind is ChartKind.PercentStackedBar or ChartKind.PercentStackedHorizontalBar)
                check(
                    ToolTip.GetTip(chart) is string percentTip && percentTip.Contains('%') &&
                    percentTip.Contains("20kWh") && percentTip.Contains("2000.0人"),
                    $"{label}/{kind}: shared percent rows include both original values and shares");
        }

        chart = SwitchChart(window, chart, ChartKind.Line);
        primary.ItemsSource = new[] { new ChartPoint(2, 30), new ChartPoint(0, 10), new ChartPoint(1, 20) };
        Flush();
        point = chart.GetDataPointPosition(0, 2)!.Value;
        Move(window, chart, point);
        check(chart.HoveredPoints.Any(hit => hit.Point == primary.ItemsSource[2] && hit.PointIndex == 2),
            $"{label}: unsorted sources retain original shared indices");
        foreach (var kind in new[] { ChartKind.Pie, ChartKind.Doughnut, ChartKind.Gauge })
        {
            chart = SwitchChart(window, chart, kind);
            Flush();
            var radialPoint = chart.GetDataPointPosition(0, 0) ??
                              new Point(chart.Bounds.Width / 2, chart.Bounds.Height / 2);
            Move(window, chart, radialPoint);
            check(chart.CrosshairPosition is null && chart.GetSharedDataPoints(radialPoint).Count == 0,
                $"{label}/{kind}: radial charts retain single-value hover without Cartesian crosshairs");
        }

        chart = SwitchChart(window, chart, ChartKind.Line);
        chart.IsEnabled = false;
        Flush();
        Move(window, chart, new Point(chart.Bounds.Width / 2, chart.Bounds.Height / 2));
        check(chart.CrosshairPosition is null && chart.HoveredPoints.Count == 0,
            $"{label}: disabled charts release hover overlays and shared rows");
        window.Close();
        Flush();
    }

    private static void VerifyInteractiveLegend(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var chart = CreateSample(ChartKind.Line);
        chart.IsPanEnabled = true;
        var window = Open(chart, theme, size);
        var label = $"Chart/{theme.Key}/{size}/legend";
        var first = chart.SeriesSource![0];
        var second = chart.SeriesSource[1];
        var bounds = chart.GetLegendItemBounds(1);
        check(bounds is { } item && chart.HitTestLegend(item.Center) == 1,
            $"{label}: bounded legend geometry maps to its series index");
        chart.SelectedIndex = 5;
        chart.NavigationAxes = ChartNavigationAxes.Horizontal;
        chart.Viewport = new Rect(0, 0, 0.3, 1);
        Flush();
        var manualView = chart.Viewport;
        check(chart.GetDataPointPosition(0, 5) is null,
            $"{label}: manually panning can leave an existing selected model outside the view");
        ClickAt(window, chart, chart.GetLegendItemBounds(0)!.Value.Center);
        ClickAt(window, chart, chart.GetLegendItemBounds(0)!.Value.Center);
        check(
            first.IsVisible && chart.SelectedIndex == 5 && chart.Viewport == manualView &&
            chart.GetDataPointPosition(0, 5) is null,
            $"{label}: hiding and restoring a selected series preserves the user's manually positioned viewport");
        var callbackView = new Rect(0.5, 0, 0.3, 1);
        EventHandler<AvaloniaPropertyChangedEventArgs> externalVisibility = (_, args) =>
        {
            if (args.Property == ChartSeries.IsVisibleProperty && first.IsVisible) chart.Viewport = callbackView;
        };
        first.PropertyChanged += externalVisibility;
        ClickAt(window, chart, chart.GetLegendItemBounds(0)!.Value.Center);
        ClickAt(window, chart, chart.GetLegendItemBounds(0)!.Value.Center);
        first.PropertyChanged -= externalVisibility;
        check(chart.Viewport == callbackView,
            $"{label}: visibility callbacks retain their own viewport changes without a post-toggle overwrite");
        chart.SelectedIndex = -1;
        chart.ResetViewport();
        chart.NavigationAxes = ChartNavigationAxes.Both;
        Flush();
        var viewport = chart.Viewport;
        ClickAt(window, chart, bounds!.Value.Center);
        check(!second.IsVisible && chart.GetLegendItemBounds(1) is not null && chart.Viewport == viewport,
            $"{label}: clicking hides a series, retains its restore item and does not pan");
        ClickAt(window, chart, chart.GetLegendItemBounds(0)!.Value.Center);
        check(!first.IsVisible && !second.IsVisible && chart.GetLegendItemBounds(0) is not null,
            $"{label}: all-hidden charts retain their complete legend");
        ClickAt(window, chart, chart.GetLegendItemBounds(1)!.Value.Center);
        check(second.IsVisible && !first.IsVisible,
            $"{label}: clicking a hidden legend restores the series from an empty plot");
        var point = chart.GetLegendItemBounds(1)!.Value.Center;
        var location = chart.TranslatePoint(point, window)!.Value;
        window.MouseDown(location, MouseButton.Left);
        window.MouseMove(location + new Vector(30, -20), RawInputModifiers.LeftMouseButton);
        window.MouseUp(location + new Vector(30, -20), MouseButton.Left);
        Flush();
        check(second.IsVisible && chart.Viewport == viewport,
            $"{label}: a dragged legend press changes neither visibility nor viewport");
        chart.IsLegendInteractive = false;
        Flush();
        ClickAt(window, chart, chart.GetLegendItemBounds(1)!.Value.Center);
        check(second.IsVisible, $"{label}: disabling legend interaction preserves series state on click");
        chart.IsLegendInteractive = true;
        chart.IsEnabled = false;
        Flush();
        ClickAt(window, chart, chart.GetLegendItemBounds(1)!.Value.Center);
        check(second.IsVisible, $"{label}: disabled chart input cannot toggle a legend item");
        chart.IsEnabled = true;
        foreach (var kind in new[] { ChartKind.Pie, ChartKind.Doughnut })
        {
            chart = SwitchChart(window, chart, kind);
            Flush();
            var radialBounds = chart.GetLegendItemBounds(0);
            if (radialBounds is { } radialItem) ClickAt(window, chart, radialItem.Center);
            check(second.IsVisible && !first.IsVisible, $"{label}/{kind}: slice legends remain read-only");
        }

        chart = SwitchChart(window, chart, ChartKind.Line);
        var visibilitySource = new ChartSeries();
        var boundSeries = new ChartSeries
            { Name = "Bound visibility", ItemsSource = new[] { new ChartPoint(0, 1), new ChartPoint(1, 2) } };
        using var visibilityBinding = boundSeries.Bind(ChartSeries.IsVisibleProperty,
            visibilitySource.GetObservable(ChartSeries.IsVisibleProperty));
        chart.SeriesSource = new[] { boundSeries };
        Flush();
        ClickAt(window, chart, chart.GetLegendItemBounds(0)!.Value.Center);
        check(!boundSeries.IsVisible, $"{label}: legend updates the effective visibility of a strongly bound series");
        visibilitySource.IsVisible = false;
        Flush();
        visibilitySource.IsVisible = true;
        Flush();
        check(boundSeries.IsVisible,
            $"{label}: toggling retains the series' original observable binding for later source updates");
        var many = Enumerable.Range(0, 1000).Select(index => new ChartSeries
                { Name = $"Series {index:0000}", ItemsSource = new[] { new ChartPoint(0, 10), new ChartPoint(1, 20) } })
            .ToArray();
        chart.SeriesSource = many;
        Flush();
        check(
            chart.LegendPageCount > 1 &&
            Enumerable.Range(0, 1000).Count(index => chart.GetLegendItemBounds(index) is not null) is > 0 and < 12 &&
            chart.GetLegendItemBounds(999) is null && !chart.GetVisualDescendants().Any(),
            $"{label}: 1,000 series render a bounded legend page with zero legend controls");
        chart.PageLegend(chart.LegendPageCount);
        Flush();
        check(chart.LegendPageIndex == chart.LegendPageCount - 1 && chart.GetLegendItemBounds(999) is not null,
            $"{label}: distant legend pagination exposes the original final series");
        chart.PageLegend(-chart.LegendPageCount);
        chart.Focus();
        Flush();
        Press(window, Key.F6, PhysicalKey.F6);
        Press(window, Key.End, PhysicalKey.End);
        Press(window, Key.Space, PhysicalKey.Space);
        check(!many[999].IsVisible && chart.GetLegendItemBounds(999) is not null && chart.SelectedIndex == -1,
            $"{label}: F6 and End navigate distant legend entries; Space toggles the original series");
        Press(window, Key.Enter, PhysicalKey.Enter);
        check(many[999].IsVisible, $"{label}: Enter restores the keyboard-focused hidden series");
        Press(window, Key.Escape, PhysicalKey.Escape);
        chart.TooltipMode = ChartTooltipMode.Shared;
        chart.ShowCrosshair = true;
        Flush();
        point = chart.GetDataPointPosition(0, 1)!.Value;
        Move(window, chart, point);
        check(
            chart.HoveredPoints.Count == 1000 && ToolTip.GetTip(chart) is string largeTip && largeTip.Length < 2000 &&
            largeTip.Split('\n').Length <= 14,
            $"{label}: shared diagnostics retain 1,000 models while tooltip text remains bounded");
        chart.Width = 80;
        Flush();
        var previous = chart.GetLegendPageNavigationBounds(false);
        var next = chart.GetLegendPageNavigationBounds(true);
        var tinyItem = chart.GetLegendItemBounds(999);
        check(
            (previous is null || (previous.Value.Width >= 0 && previous.Value.Height >= 0)) &&
            (next is null || (next.Value.Width >= 0 && next.Value.Height >= 0)) &&
            (tinyItem is null || (tinyItem.Value.Width >= 0 && tinyItem.Value.Height >= 0)) &&
            Frame(window).Length > 0 && !chart.GetVisualDescendants().Any(),
            $"{label}: an 80-pixel legend keeps nonnegative page geometry and renders without child controls");
        window.Close();
        Flush();
    }

    private static void VerifyHoverReentrancy(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var label = $"Chart/{theme.Key}/{size}/hover-reentrancy";
        foreach (var mutateSource in new[] { true, false })
        {
            var chart = CreateSample(ChartKind.Line);
            chart.ShowCrosshair = true;
            chart.TooltipMode = ChartTooltipMode.Shared;
            var replacement = new[]
            {
                new ChartSeries
                {
                    Name = "Replacement hover",
                    ItemsSource = new[] { new ChartPoint(0, 42), new ChartPoint(1, 44), new ChartPoint(2, 46) }
                }
            };
            var window = Open(chart, theme, size);
            var changed = false;
            chart.PropertyChanged += (_, args) =>
            {
                if (changed || args.Property !=
                    (mutateSource ? ChartBase.HoveredPointsProperty : ChartBase.CrosshairPositionProperty)) return;
                if (mutateSource ? chart.HoveredPoints.Count == 0 : chart.CrosshairPosition is null) return;
                changed = true;
                if (mutateSource) chart.SeriesSource = replacement;
                else chart.Viewport = new Rect(0.1, 0, 0.8, 1);
            };
            Move(window, chart, chart.GetDataPointPosition(0, 2)!.Value);
            check(
                changed && chart.HoveredPoints.Count == 0 && chart.CrosshairPosition is null &&
                ToolTip.GetTip(chart) is null,
                $"{label}/{(mutateSource ? "source" : "viewport")}: synchronous hover listeners cannot restore stale rows, tooltip or crosshair");
            var next = chart.GetDataPointPosition(0, 1)!.Value;
            Move(window, chart, next);
            check(
                chart.HoveredPoints.Any(hit => ReferenceEquals(hit.Point, chart.SeriesSource![0].ItemsSource![1])) &&
                (!mutateSource || chart.HoveredPoints.All(hit => ReferenceEquals(hit.Series, replacement[0]))),
                $"{label}/{(mutateSource ? "source" : "viewport")}: the next pointer move uses the current source identity");
            window.Close();
            Flush();
        }

        var legendChart = CreateSample(ChartKind.Line);
        var legendWindow = Open(legendChart, theme, size);
        var pressed = legendChart.GetLegendItemBounds(0)!.Value.Center;
        var location = legendChart.TranslatePoint(pressed, legendWindow)!.Value;
        legendWindow.MouseMove(location);
        legendWindow.MouseDown(location, MouseButton.Left);
        var newSeries = new ChartSeries
            { Name = "Replacement", ItemsSource = new[] { new ChartPoint(0, 1), new ChartPoint(1, 2) } };
        legendChart.SeriesSource = new[] { newSeries };
        Flush();
        legendWindow.MouseUp(location, MouseButton.Left);
        Flush();
        check(newSeries.IsVisible,
            $"{label}: replacing a pressed legend source cannot toggle the new object at the same index");
        var a = new ChartSeries { Name = "A", ItemsSource = new[] { new ChartPoint(0, 1) } };
        var b = new ChartSeries { Name = "B", ItemsSource = new[] { new ChartPoint(0, 2) } };
        var reordered = new ObservableCollection<ChartSeries> { a, b };
        legendChart.SeriesSource = reordered;
        Flush();
        pressed = legendChart.GetLegendItemBounds(0)!.Value.Center;
        location = legendChart.TranslatePoint(pressed, legendWindow)!.Value;
        legendWindow.MouseMove(location);
        legendWindow.MouseDown(location, MouseButton.Left);
        reordered.Move(0, 1);
        Flush();
        legendWindow.MouseUp(location, MouseButton.Left);
        Flush();
        check(a.IsVisible && b.IsVisible,
            $"{label}: reordering during a legend press cannot toggle a different series identity");
        legendWindow.Close();
        Flush();
    }

    private static void Move(Window window, ChartBase chart, Point local)
    {
        window.MouseMove(chart.TranslatePoint(local, window)!.Value);
        Flush();
    }

    private static void ClickAt(Window window, ChartBase chart, Point local)
    {
        var point = chart.TranslatePoint(local, window)!.Value;
        window.MouseMove(point);
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        Flush();
    }

    private static bool NearPoint(Point first, Point second)
    {
        return new Vector(first.X - second.X, first.Y - second.Y).Length < 0.5;
    }

    private static Color PixelAt(Window window, ChartBase chart, Point local)
    {
        using var bitmap = window.CaptureRenderedFrame()!;
        var point = chart.TranslatePoint(local, window)!.Value;
        var x = Math.Clamp((int)(point.X * window.RenderScaling), 0, bitmap.PixelSize.Width - 1);
        var y = Math.Clamp((int)(point.Y * window.RenderScaling), 0, bitmap.PixelSize.Height - 1);
        var bytes = new byte[4];
        var pin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            bitmap.CopyPixels(new PixelRect(x, y, 1, 1), pin.AddrOfPinnedObject(), 4, 4);
        }
        finally
        {
            pin.Free();
        }

        return bitmap.Format == PixelFormat.Rgba8888
            ? Color.FromArgb(bytes[3], bytes[0], bytes[1], bytes[2])
            : Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
    }

    private static void VerifyReentrancy(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var replacement = new ObservableCollection<ChartSeries>
        {
            new()
            {
                Name = "Replacement",
                ItemsSource = new[]
                {
                    new ChartPoint(0, 42, "new first"), new ChartPoint(1, 46, "new middle"),
                    new ChartPoint(2, 44, "new last")
                }
            }
        };
        ChartBase chart = new LineChart
        {
            ShowDataLabels = true,
            SeriesSource = Enumerable.Range(0, 3).Select(series => new ChartSeries
            {
                Name = $"Old {series}",
                ItemsSource = Enumerable.Range(0, 50).Select(index => new ChartPoint(index, index + series * 20))
                    .ToArray()
            }).ToArray()
        };
        var replaced = false;
        chart.PropertyChanged += (_, args) =>
        {
            if (!replaced && args.Property == ChartBase.RenderedPointCountProperty && chart.RenderedPointCount > 0)
            {
                replaced = true;
                chart.SeriesSource = replacement;
            }
        };
        var window = Open(chart, theme, size);
        Flush();
        Flush();
        var label = $"Chart/{theme.Key}/{size}/reentrancy";
        var point = chart.GetDataPointPosition(0, 1);
        check(
            replaced && ReferenceEquals(chart.SeriesSource, replacement) && chart.RenderedPointCount == 3 &&
            chart.GetDataPointPosition(2, 49) is null && point is { } mark &&
            chart.HitTestDataPoint(mark)?.Point == replacement[0].ItemsSource![1],
            $"{label}: a diagnostic listener can synchronously shrink the source without painting or hitting old frame indices");
        chart.SelectedIndex = 1;
        Flush();
        check(chart.Selection?.Series == replacement[0] && Frame(window).Length > 0,
            $"{label}: the next stable frame and selection use only the replacement model");
        window.Close();
        Flush();
        var budgetChart = new LineChart
        {
            ShowDataLabels = true, SamplingPointBudget = 128, SamplingThreshold = 16,
            SeriesSource = new[]
            {
                new ChartSeries
                {
                    ItemsSource = Enumerable.Range(0, 5000)
                        .Select(index => new ChartPoint(index, 50 + Math.Sin(index / 10d) * 40)).ToArray()
                }
            }
        };
        var initialCount = 0;
        budgetChart.PropertyChanged += (_, args) =>
        {
            if (initialCount == 0 && args.Property == ChartBase.RenderedPointCountProperty &&
                budgetChart.RenderedPointCount > 0)
            {
                initialCount = budgetChart.RenderedPointCount;
                budgetChart.SamplingPointBudget = 8;
            }
        };
        window = Open(budgetChart, theme, size);
        Flush();
        Flush();
        check(
            initialCount > 8 && budgetChart.SamplingPointBudget == 8 &&
            budgetChart.RenderedPointCount is > 0 and <= 8 && budgetChart.SamplingActive,
            $"{label}: budget changes inside diagnostic listeners leave preparation dirty and rebuild the smaller stable frame");
        window.Close();
        Flush();
    }

    private static bool RedPixelNear(Window window, ChartBase chart, Point local)
    {
        using var bitmap = window.CaptureRenderedFrame()!;
        var point = chart.TranslatePoint(local, window)!.Value;
        var scale = window.RenderScaling;
        var left = Math.Clamp((int)(point.X * scale) - 3, 0, bitmap.PixelSize.Width - 1);
        var top = Math.Clamp((int)(point.Y * scale) - 3, 0, bitmap.PixelSize.Height - 1);
        var width = Math.Min(7, bitmap.PixelSize.Width - left);
        var height = Math.Min(7, bitmap.PixelSize.Height - top);
        var bytes = new byte[width * height * 4];
        var pin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            bitmap.CopyPixels(new PixelRect(left, top, width, height), pin.AddrOfPinnedObject(), bytes.Length,
                width * 4);
        }
        finally
        {
            pin.Free();
        }

        for (var offset = 0; offset < bytes.Length; offset += 4)
        {
            var red = bitmap.Format == PixelFormat.Rgba8888 ? bytes[offset] : bytes[offset + 2];
            var green = bytes[offset + 1];
            var blue = bitmap.Format == PixelFormat.Rgba8888 ? bytes[offset + 2] : bytes[offset];
            if (red > 180 && green < 80 && blue < 80) return true;
        }

        return false;
    }

    private static void Press(Window window, Key key, PhysicalKey physical,
        RawInputModifiers modifiers = RawInputModifiers.None)
    {
        window.KeyPress(key, modifiers, physical, null);
        window.KeyRelease(key, modifiers, physical, null);
        Flush();
    }

    private static double Distance(Point first, Point second)
    {
        var delta = first - second;
        return Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
    }

    private static byte[] Frame(Window window)
    {
        using var frame = window.CaptureRenderedFrame()!;
        using var stream = new MemoryStream();
        frame.Save(stream, PngBitmapEncoderOptions.Default);
        return stream.ToArray();
    }

    private static Color? Colour(IBrush? brush)
    {
        return (brush as ISolidColorBrush)?.Color;
    }

    private static Color? Resource(Control control, string key)
    {
        return control.TryFindResource(key, control.ActualThemeVariant, out var value) ? Colour(value as IBrush) : null;
    }
}