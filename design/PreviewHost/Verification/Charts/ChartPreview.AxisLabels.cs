using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static partial class ChartPreview
{
    // Compare actual chart glyph pixels with independently rendered, unclipped text.
    // This catches both omitted month labels and the former fixed 80px clipping window.
    public static void VerifyAxisLabels(Action<bool, string> check, string? output = null)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var kind in new[] { ChartKind.Line, ChartKind.Bar })
        foreach (var longLabels in new[] { false, true })
        foreach (var legend in new[] { false, true })
        {
            var names = longLabels ? new[] { "2026年10月营收", "2026年12月营收" }
                : Enumerable.Range(1, 6).Select(index => $"{index}月").ToArray();
            var width = size == ControlSize.Small ? 240 : size == ControlSize.Large ? 360 : 300;
            var chart = CreateControl(kind);
            chart.Width = width; chart.Height = 224; chart.Title = "X轴标签";
            chart.YMinimum = 0; chart.YMaximum = 100; chart.ShowLegend = legend;
            chart.ShowGrid = false; chart.ShowMarkers = false; chart.MutedBrush = Brushes.Red;
            // Long line captions also exercise decreasing X input; label collision checks use screen order.
            chart.SeriesSource = new[] { new ChartSeries { Name = "图例", ItemsSource = names.Select((name, index) => new ChartPoint(kind == ChartKind.Line && longLabels ? names.Length - 1 - index : index, 0, name)).ToArray() } };
            var reference = new AxisLabelReference { Width = width, Height = chart.Height };
            var canvas = new Canvas { Children = { chart, reference } };
            Canvas.SetLeft(chart, 16); Canvas.SetTop(chart, 16);
            Canvas.SetLeft(reference, width + 32); Canvas.SetTop(reference, 16);
            var window = new Window { Width = width * 2 + 48, Height = 256, Content = canvas,
                WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme };
            ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); Flush();
            var first = chart.GetDataPointPosition(0, 0)!.Value;
            var last = chart.GetDataPointPosition(0, names.Length - 1)!.Value;
            var plotWidth = (last.X - first.X) * (kind == ChartKind.Bar ? names.Length / (double)(names.Length - 1) : 1);
            var gap = kind == ChartKind.Bar ? Math.Min(2, plotWidth * 0.76 / names.Length * 0.12) : 0;
            var plotLeft = first.X + gap / 2 - (kind == ChartKind.Bar ? plotWidth / names.Length / 2 : 0);
            // A zero-height bar is clipped to its lower half and reports that rectangle's center.
            var baseline = first.Y + (kind == ChartKind.Bar ? 0.25 : 0);
            reference.Background = chart.Background;
            foreach (var (name, index) in names.Select((name, index) => (name, index)))
            {
                var text = new FormattedText(name, CultureInfo.CurrentCulture, chart.FlowDirection,
                    new Typeface(chart.FontFamily), chart.FontSize, Brushes.Red);
                var center = plotLeft + plotWidth * (kind == ChartKind.Bar ? (index + 0.5) / names.Length : index / (double)(names.Length - 1));
                var position = new Point(Math.Clamp(center - text.Width / 2, chart.ChartPadding,
                    width - chart.ChartPadding - text.Width), baseline + 8);
                reference.Labels.Add((text, position));
            }
            reference.InvalidateVisual(); Flush();
            using var bitmap = window.CaptureRenderedFrame()!;
            if (output is not null)
            {
                Directory.CreateDirectory(output);
                bitmap.Save(Path.Combine(output, $"axis-{kind}-{theme.Key}-{size}-{longLabels}-{legend}.png"), PngBitmapEncoderOptions.Default);
            }
            var ink = 0; var missing = 0;
            foreach (var (text, position) in reference.Labels)
            {
                for (var y = (int)Math.Floor(position.Y); y < Math.Ceiling(position.Y + text.Height); y++)
                for (var x = (int)Math.Floor(position.X); x < Math.Ceiling(position.X + text.Width); x++)
                {
                    var expected = BitmapPixel(bitmap, x + width + 32, y + 16);
                    if (expected.R < 150 || expected.R <= expected.G * 2 || expected.R <= expected.B * 2) continue;
                    ink++;
                    var actual = BitmapPixel(bitmap, x + 16, y + 16);
                    if (actual.R < 150 || actual.R <= actual.G * 2 || actual.R <= actual.B * 2) missing++;
                }
            }
            check(ink > 0 && missing == 0, $"Chart/axis-labels/{theme.Key}/{size}/{kind}/long={longLabels}/legend={legend}: every expected glyph is fully painted (missing {missing}/{ink})");
            window.Close(); Flush();
        }
    }

    private sealed class AxisLabelReference : Control
    {
        public IBrush? Background { get; set; }
        public List<(FormattedText Text, Point Position)> Labels { get; } = [];
        public override void Render(DrawingContext context)
        {
            context.DrawRectangle(Background, null, new Rect(Bounds.Size));
            foreach (var (text, position) in Labels) context.DrawText(text, position);
        }
    }
}
