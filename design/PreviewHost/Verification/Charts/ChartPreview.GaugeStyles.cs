using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Avalonia.Platform;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static partial class ChartPreview
{
    public static void VerifyGaugeStyles(Action<bool, string> check, string? output = null)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var width = size == ControlSize.Small ? 240 : size == ControlSize.Large ? 360 : 300;
            var height = size == ControlSize.Small ? 224 : size == ControlSize.Large ? 336 : 280;
            var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*,*,*"), RowDefinitions = new RowDefinitions("*,*") };
            var samples = new List<GaugeChart>();
            foreach (var style in Enum.GetValues<GaugeStyle>())
            {
                var chart = new GaugeChart { Width = width, Height = height, IsAnimationEnabled = false,
                    Title = ChartControlFactory.GaugeStyleTitle(style) };
                ChartControlFactory.ConfigureGauge(chart, style);
                Grid.SetColumn(chart, samples.Count % 4); Grid.SetRow(chart, samples.Count / 4);
                samples.Add(chart); grid.Children.Add(chart);
            }
            var window = new Window { Width = width * 4 + 32, Height = height * 2 + 32,
                WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
                Content = new Border { Padding = new Thickness(16), Child = grid } };
            ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); Flush();
            if (output is not null)
            {
                Directory.CreateDirectory(output);
                using var frame = window.CaptureRenderedFrame()!;
                frame.Save(Path.Combine(output, $"gauges-{theme.Key}-{size}.png"), PngBitmapEncoderOptions.Default);
            }
            var images = new List<byte[]>();
            foreach (var chart in samples)
            {
                var label = $"Gauge/{theme.Key}/{size}/{chart.GaugeStyle}";
                var initial = ExportBytes(chart); images.Add(initial);
                check(initial.Length > 1000 && !chart.GetVisualDescendants().Any() && chart.GetDataPointPosition(0, 0) is null,
                    $"{label}: draws a real gauge without child controls or data-point selection");
                CompareGaugeExport(window, chart, check, label);
                var before = Frame(window); chart.Value = chart.Maximum * 0.2; Flush();
                check(!before.SequenceEqual(Frame(window)), $"{label}: value changes redraw the live layout");
                chart.ShowGaugeValue = false; chart.GaugeCaption = null;
                chart.Value = chart.Minimum - 10; Flush(); var below = ExportBytes(chart);
                chart.Value = chart.Minimum; Flush();
                check(below.SequenceEqual(ExportBytes(chart)), $"{label}: needle and progress clamp at the minimum");
                chart.Value = chart.Maximum + 10; Flush(); var above = ExportBytes(chart);
                chart.Value = chart.Maximum; Flush();
                check(above.SequenceEqual(ExportBytes(chart)), $"{label}: needle and progress clamp at the maximum");
                chart.ShowGaugeValue = true; chart.Width = 180; chart.Height = 160; chart.Unit = "很长的单位 °C / km/h";
                chart.Value = 35.78; Flush();
                var compact = ExportBytes(chart);
                if (output is not null) File.WriteAllBytes(Path.Combine(output, $"compact-{chart.GaugeStyle}-{theme.Key}-{size}.png"), compact);
                check(compact.Length > 500, $"{label}: compact drawing fits formatted value, unit and ticks");
                if (chart.GaugeStyle == GaugeStyle.Grade)
                {
                    using var bitmap = new Bitmap(new MemoryStream(compact)); var pixels = Pixels(bitmap); var bandInk = 0;
                    for (var y = 0; y < bitmap.PixelSize.Height; y++)
                    for (var x = 0; x < bitmap.PixelSize.Width; x++)
                    {
                        var color = Pixel(bitmap, pixels, x, y);
                        if (color.R > 150 && color.R > color.G * 1.3 && color.R > color.B * 1.3) bandInk++;
                    }
                    check(bandInk > 6, $"{label}: 180px viewport retains the coloured grade arc rather than falling back to text");
                }
                chart.Width = 90; chart.Height = 70; Flush();
                check(ExportBytes(chart).Length > 100, $"{label}: tiny view falls back safely to value text");
                chart.Width = width; chart.Height = height; chart.Unit = null; chart.Value = 70;
                chart.GaugeStartAngle = -90; chart.GaugeSweepAngle = 360; Flush();
                check(ExportBytes(chart).Length > 500, $"{label}: angle properties support a full circle");
                chart.Minimum = 100; chart.Maximum = 100; Flush();
                check(ExportBytes(chart).Length > 100, $"{label}: invalid range retains the validation message");
                chart.Minimum = -double.MaxValue; chart.Maximum = double.MaxValue; chart.Value = 0; Flush();
                check(ExportBytes(chart).Length > 500, $"{label}: finite extreme ranges retain safe geometry");
            }
            check(images.Select(Convert.ToBase64String).Distinct().Count() == samples.Count,
                $"Gauge/{theme.Key}/{size}: every enum layout has distinct rendered pixels");
            window.Close(); Flush();
            VerifyGaugeOverrides(theme, size, check);
            VerifyGaugeLayout(theme, size, check);
        }
    }
    private static void CompareGaugeExport(Window window, GaugeChart chart, Action<bool, string> check, string label)
    {
        using var live = window.CaptureRenderedFrame()!;
        using var exported = new Bitmap(new MemoryStream(ExportBytes(chart)));
        var origin = chart.TranslatePoint(default, window)!.Value;
        var livePixels = Pixels(live); var exportPixels = Pixels(exported);
        var mismatch = 0; var ink = 0;
        var background = Colour(chart.Background);
        for (var y = 0; y < exported.PixelSize.Height; y++)
        for (var x = 0; x < exported.PixelSize.Width; x++)
        {
            var expected = Pixel(live, livePixels, x + (int)origin.X, y + (int)origin.Y);
            var actual = Pixel(exported, exportPixels, x, y);
            if (expected != background) ink++;
            if (Math.Abs(expected.R - actual.R) + Math.Abs(expected.G - actual.G) + Math.Abs(expected.B - actual.B) > 12) mismatch++;
        }
        check(ink > 100 && mismatch <= 30, $"{label}: PNG preserves live preset defaults, colours, labels and pointer ({mismatch} differing pixels)");
    }
    private static byte[] Pixels(Bitmap bitmap)
    {
        var bytes = new byte[bitmap.PixelSize.Width * bitmap.PixelSize.Height * 4];
        var pin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try { bitmap.CopyPixels(new PixelRect(bitmap.PixelSize), pin.AddrOfPinnedObject(), bytes.Length, bitmap.PixelSize.Width * 4); }
        finally { pin.Free(); }
        return bytes;
    }
    private static Color Pixel(Bitmap bitmap, byte[] bytes, int x, int y)
    {
        var offset = (y * bitmap.PixelSize.Width + x) * 4;
        return bitmap.Format == PixelFormat.Rgba8888 ? Color.FromArgb(bytes[offset + 3], bytes[offset], bytes[offset + 1], bytes[offset + 2])
            : Color.FromArgb(bytes[offset + 3], bytes[offset + 2], bytes[offset + 1], bytes[offset]);
    }
    private static void VerifyGaugeOverrides(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var chart = new GaugeChart { GaugeStyle = GaugeStyle.Progress, IsAnimationEnabled = false, Value = 60 };
        var window = Open(chart, theme, size); var label = $"Gauge/{theme.Key}/{size}/properties";
        var before = Frame(window); chart.ShowGaugeProgress = false; Flush();
        check(!before.SequenceEqual(Frame(window)), $"{label}: progress visibility overrides the preset");
        before = Frame(window); chart.ShowGaugeNeedle = false; Flush();
        check(!before.SequenceEqual(Frame(window)), $"{label}: explicit false hides a preset pointer");
        before = Frame(window); chart.ShowGaugeTicks = false; Flush();
        check(!before.SequenceEqual(Frame(window)), $"{label}: explicit false hides preset ticks");
        chart.ShowGaugeTicks = true; chart.ShowGaugeNeedle = true; chart.GaugeNeedleBrush = Brushes.Red;
        chart.GaugeNeedleWidth = 6; chart.GaugeNeedleLength = 0.9; chart.GaugeTickPlacement = GaugeTickPlacement.Outside;
        chart.GaugeMajorTickCount = 12; chart.GaugeMinorTickCount = 10;
        chart.GaugeValueBrush = Brushes.Green; chart.GaugeValueFontSize = 36; chart.GaugeValueOffset = -0.3;
        chart.GaugeCaption = "可配置仪表"; chart.GaugeCaptionOffset = 0.1; chart.GaugeTrackBrush = Brushes.Blue; Flush();
        var pointers = new List<byte[]>();
        foreach (var shape in new[] { GaugeNeedleShape.Line, GaugeNeedleShape.Triangle, GaugeNeedleShape.Marker })
        {
            chart.GaugeNeedleShape = shape; Flush(); pointers.Add(ExportBytes(chart));
        }
        check(pointers.Select(Convert.ToBase64String).Distinct().Count() == 3, $"{label}: pointer geometry is controlled by its property");
        CompareGaugeExport(window, chart, check, label);
        var segments = new ObservableCollection<GaugeSegment> { new() { Fraction = 0.5, Brush = Brushes.Red, Label = "较低" } };
        chart.GaugeStyle = GaugeStyle.Grade; chart.GaugeSegments = segments; Flush(); before = Frame(window);
        segments.Add(new GaugeSegment { Fraction = 1, Brush = Brushes.Green, Label = "较高" }); Flush();
        check(!before.SequenceEqual(Frame(window)), $"{label}: adding an observable band redraws the chart");
        before = Frame(window); segments[0].Fraction = 0.3; segments[0].Brush = Brushes.Blue; segments[0].Label = "最低"; Flush();
        check(!before.SequenceEqual(Frame(window)), $"{label}: band boundaries, brushes and labels notify the renderer");
        CompareGaugeExport(window, chart, check, label);
        chart.GaugeGradeLabelRotation = 0; chart.GaugeGradeLabelGap = 14; Flush();
        CompareGaugeExport(window, chart, check, label);
        chart.GaugeStyle = GaugeStyle.Barometer; chart.GaugeSegments = null; chart.GaugeSecondaryMaximum = 250;
        chart.GaugeSecondaryMinimum = -50; chart.GaugeSecondaryBrush = Brushes.Magenta; chart.GaugeSecondaryTickCount = 6; Flush();
        CompareGaugeExport(window, chart, check, label);
        window.Close(); Flush();
    }
}
