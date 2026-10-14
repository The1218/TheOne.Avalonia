using Avalonia;
using Avalonia.VisualTree;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using TheOne.Avalonia.Controls;
namespace TheOne.DesignPreview;
internal static class ChartDemoPreview
{
    public static void Verify(Action<bool, string> check)
    {
        var demo = new ChartDemoView();
        var window = new Window { Width = 900, Height = 1300, WindowDecorations = WindowDecorations.None, Content = demo };
        window.Show(); ControlPreview.Flush();
        foreach (var kind in Enum.GetValues<ChartKind>())
        {
            var former = demo.ChartSample;
            demo.Model.KindIndex = (int)kind; ControlPreview.Flush();
            check(demo.Model.Kind == kind && demo.ChartSample.Kind == kind && demo.ChartSample.GetType().Name == kind + "Chart", $"Chart/demo: page model hosts the independent {kind} control");
            if (former.Kind != kind) check(!ReferenceEquals(former, demo.ChartSample) && former.GetVisualParent() is null, $"Chart/demo: switching to {kind} releases the previous control instance");
        }
        demo.Model.KindIndex = (int)ChartKind.Gauge; ControlPreview.Flush();
        var gauge = demo.ChartSample;
        foreach (var style in Enum.GetValues<GaugeStyle>())
        {
            demo.ChartGaugeStyle.SelectedItem = style; ControlPreview.Flush();
            check(ReferenceEquals(gauge, demo.ChartSample) && ((GaugeChart)gauge).GaugeStyle == style && demo.Model.GaugeStyle == style,
                $"Chart/demo: compiled enum selector switches {style} on the same gauge instance");
        }
        demo.ChartGaugeSlider.Value = 86; ControlPreview.Flush();
        check(demo.ChartSample.Value == 86 && demo.Model.GaugeValue == 86, "Chart/demo: gauge slider updates the bound model and chart");
        demo.ChartGaugeNeedle.IsChecked = false; demo.ChartGaugeTicks.IsChecked = false; ControlPreview.Flush(); check(!demo.ChartSample.ShowGaugeNeedle && !demo.ChartSample.ShowGaugeTicks, "Chart/demo: gauge feature switches update compiled needle and tick bindings");
        demo.ChartGaugeNeedle.IsChecked = true; demo.ChartGaugeTicks.IsChecked = true; ControlPreview.Flush();
        demo.Model.KindIndex = 0; demo.ChartComparison.IsChecked = false; ControlPreview.Flush();
        demo.ChartSmooth.IsChecked = true; demo.ChartAnimateUpdates.IsChecked = false; demo.ChartAnimationSpeed.SelectedIndex = 2; ControlPreview.Flush();
        check(demo.ChartSample.IsSmooth && !demo.ChartSample.IsAnimationEnabled && demo.ChartSample.AnimationDuration == TimeSpan.FromSeconds(1) && double.IsNaN(demo.ChartSample.Height), "Chart/demo: smoothing, animation duration and natural graph height use compiled preferences");
        demo.ChartAnimateUpdates.IsChecked = true; ControlPreview.Flush();
        check(!demo.Model.Series[1].IsVisible, "Chart/demo: comparison checkbox updates series visibility");
        var oldComparison = demo.Model.Series[1]; var legend = demo.ChartSample.GetLegendItemBounds(1)!.Value.Center;
        var legendLocation = demo.ChartSample.TranslatePoint(legend, window)!.Value; window.MouseMove(legendLocation); window.MouseDown(legendLocation, MouseButton.Left); window.MouseUp(legendLocation, MouseButton.Left); ControlPreview.Flush();
        check(oldComparison.IsVisible && demo.Model.ShowComparison && demo.ChartComparison.IsChecked == true, "Chart/demo: legend visibility changes synchronize the model and comparison checkbox");
        check(demo.ChartSample.ShowCrosshair && demo.ChartSample.TooltipMode == ChartTooltipMode.Shared && demo.ChartSample.IsLegendInteractive, "Chart/demo: compiled interaction preferences start with crosshair and shared tips enabled");
        var hover = demo.ChartSample.GetDataPointPosition(0, 2)!.Value; window.MouseMove(demo.ChartSample.TranslatePoint(hover, window)!.Value); ControlPreview.Flush();
        check(demo.ChartSample.HoveredPoints.Count == 2 && demo.ChartSample.CrosshairPosition is not null && demo.ChartHoveredCount.Text!.Contains("2"), "Chart/demo: readonly shared diagnostics update compiled hover status");
        demo.ChartCrosshair.IsChecked = false; demo.ChartSharedTooltip.IsChecked = false; demo.ChartLegendInteractive.IsChecked = false; ControlPreview.Flush();
        check(!demo.ChartSample.ShowCrosshair && demo.ChartSample.TooltipMode == ChartTooltipMode.Single && !demo.ChartSample.IsLegendInteractive, "Chart/demo: interaction switches update all three compiled chart preferences");
        demo.ChartCrosshair.IsChecked = true; demo.ChartSharedTooltip.IsChecked = true; demo.ChartLegendInteractive.IsChecked = true; ControlPreview.Flush();
        Click(demo.ZoomInChart); check(demo.Model.Viewport == demo.ChartSample.Viewport && demo.Model.Viewport.Width < 1, "Chart/demo: zoom writes the compiled two-way viewport model");
        var beforePan = demo.Model.Viewport; Click(demo.PanRightChart); check(demo.Model.Viewport.X > beforePan.X, "Chart/demo: pan controls update the bound viewport");
        demo.ChartAxesPicker.SelectedIndex = 1; ControlPreview.Flush(); check(demo.ChartSample.NavigationAxes == ChartNavigationAxes.Horizontal, "Chart/demo: direction picker updates compiled axes binding");
        demo.ChartZoomEnabled.IsChecked = false; demo.ChartPanEnabled.IsChecked = false; ControlPreview.Flush();
        check(!demo.ChartSample.IsZoomEnabled && !demo.ChartSample.IsPanEnabled, "Chart/demo: input toggles update chart flags");
        Click(demo.ResetChartViewport); check(demo.Model.Viewport == new Rect(0, 0, 1, 1), "Chart/demo: full-view action restores viewport");
        Click(demo.ThousandChartPoints); check(demo.Model.Series[0].ItemsSource?.Count == 1000 && !demo.ChartSample.GetVisualDescendants().Any(), "Chart/demo: large-data action renders a thousand models without item controls");
        var comparisonState = demo.Model.ShowComparison; oldComparison.IsVisible = false;
        check(demo.Model.ShowComparison == comparisonState, "Chart/demo: replacing the source releases old comparison visibility subscriptions");
        Click(demo.ZoomInChart);
        Click(demo.LastChartPoint); check(demo.Model.SelectedPoint == 999 && demo.ChartSample.Selection?.PointIndex == 999 && demo.ChartSelectionLabel.Text!.Contains("1000"), "Chart/demo: distant selection updates compiled detail text");
        var previous = demo.Model.Series[0].ItemsSource![999].Y; Click(demo.UpdateChartData);
        check(demo.Model.Series[0].ItemsSource![999].Y != previous, "Chart/demo: update action changes live distant data");
        Click(demo.ChartGapData); check(demo.Model.Series[0].ItemsSource!.Any(point => !double.IsFinite(point.Y)) && demo.ChartSample.GetDataPointPosition(0, 3) is null, "Chart/demo: missing-value action supplies visible line gaps");
        Click(demo.ChartStackData); check(demo.Model.Kind == ChartKind.StackedBar && demo.Model.Series.Count == 2 && demo.Model.Series[0].ItemsSource!.Any(point => point.Y < 0) && demo.ChartSample.GetDataPointPosition(1, 2) is not null, "Chart/demo: stack action supplies cumulative positive and negative series");
        demo.Model.KindIndex = (int)ChartKind.PercentStackedBar; ControlPreview.Flush(); check(Math.Abs(demo.ChartSample.GetStackPercentage(0, 0)!.Value - 2d / 3) < 0.00001, "Chart/demo: percent type uses the existing mixed-sign source values");
        Click(demo.ChartSecondaryData); check(demo.Model.Series[1].YAxis == ChartYAxis.Secondary && demo.ChartSample.SecondaryUnit == "人" && demo.ChartSample.SecondaryYAxisTitle == demo.Model.SecondaryAxisTitle, "Chart/demo: dual-axis action supplies independent units and compiled titles");
        demo.ChartSecondaryAxis.IsChecked = false; ControlPreview.Flush(); check(demo.Model.Series[1].YAxis == ChartYAxis.Primary, "Chart/demo: axis toggle reassigns the observable second series");
        Click(demo.HundredThousandChartPoints); check(demo.Model.Series[0].ItemsSource?.Count == 100000 && demo.ChartSample.SamplingActive && demo.ChartSample.RenderedPointCount is > 0 and <= 2016 && demo.ChartRenderedCount.Text!.Contains("实际绘制"), "Chart/demo: 100,000-point action displays live sampling diagnostics");
        demo.ChartSamplingEnabled.IsChecked = false; ControlPreview.Flush(); check(!demo.ChartSample.SamplingActive && demo.ChartSample.RenderedPointCount >= 97000, "Chart/demo: sampling toggle draws the original finite source vertices");
        demo.ChartSamplingEnabled.IsChecked = true; ControlPreview.Flush(); check(demo.ChartSample.SamplingActive && demo.ChartSample.RenderedPointCount <= 2016, "Chart/demo: sampling can be restored without replacing source data");
        Click(demo.ThousandChartSeries); check(demo.Model.Series.Count == 1000 && demo.ChartSample.LegendPageCount > 1 && !demo.ChartSample.GetVisualDescendants().Any(), "Chart/demo: large-series action renders a bounded legend page with no legend controls");
        Click(demo.ResetChartData); Click(demo.RapidChartUpdate); var rapidTimeout = System.Diagnostics.Stopwatch.StartNew();
        while (demo.IsRapidUpdating && rapidTimeout.Elapsed < TimeSpan.FromSeconds(5)) ChartPreview.AdvanceAnimation(30);
        check(!demo.IsRapidUpdating && demo.Model.Status.Contains("更新 5 次"), "Chart/demo: continuous-update action completes five bounded batches and stops its timer");
        using (var image = new MemoryStream()) { demo.ChartSample.ExportPng(image); check(image.Length > 1000, "Chart/demo: current compiled chart exports a real PNG without invoking a file dialog"); }
        Click(demo.ResetChartData); check(demo.Model.Series.Count == 2 && demo.Model.Series[0].ItemsSource?.Count == 6 && demo.Model.SelectedPoint == -1 && demo.Model.GaugeValue == 72, "Chart/demo: reset restores data, gauge and selection");
        window.Close(); ControlPreview.Flush();
        void Click(Control control)
        {
            var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
            window.MouseMove(point); window.MouseDown(point, MouseButton.Left); window.MouseUp(point, MouseButton.Left); ControlPreview.Flush();
        }
    }
}
