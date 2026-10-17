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
    private static void VerifyGaugeLayout(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        foreach (var style in Enum.GetValues<GaugeStyle>().Cast<GaugeStyle?>().Append(null))
        {
            var chart = new GaugeChart
            {
                GaugeStyle = style ?? GaugeStyle.Semicircle, Value = 70, Unit = "kg", Foreground = Brushes.Red,
                MutedBrush = Brushes.Green, AccentBrush = Brushes.Transparent, GridBrush = Brushes.Transparent,
                ShowGaugeNeedle = false, ShowGaugeTicks = false,
                AnimationDuration = TimeSpan.FromSeconds(1)
            };
            if (style is not null)
            {
                chart.GaugeValueBrush = Brushes.Red;
                chart.GaugeTrackBrush = Brushes.Transparent;
                chart.ShowGaugeProgress = false;
            }

            var window = Open(chart, theme, size);
            Motion.SetEnabled(window, true);
            Flush();
            using var initial = window.CaptureRenderedFrame()!;
            var bounds = GaugeNumberInk(initial);
            var widths = new List<int> { bounds.Width };
            var heights = new List<int> { bounds.Height };
            foreach (var value in new[] { 79d, 75, 73, 78 })
            {
                chart.Value = value;
                Flush();
                AdvanceAnimation(90);
                using var frame = window.CaptureRenderedFrame()!;
                var ink = GaugeNumberInk(frame);
                widths.Add(ink.Width);
                heights.Add(ink.Height);
                check(chart.IsAnimating && ink.Width > 0,
                    $"Gauge/{theme.Key}/{size}/{style}: interrupted slider-like updates keep drawing animated value text");
            }

            check(widths.Max() - widths.Min() <= 4 && heights.Max() - heights.Min() <= 2,
                $"Gauge/{theme.Key}/{size}/{style}: integer animation keeps the numeric precision, type size and unit width stable ({string.Join(',', widths)})");
            widths.Clear();
            foreach (var value in new[] { 71.23, 72, 74.25, 77 })
            {
                chart.Value = value;
                Flush();
                AdvanceAnimation(70);
                using var frame = window.CaptureRenderedFrame()!;
                widths.Add(GaugeNumberInk(frame).Width);
            }

            check(widths.Max() - widths.Min() <= 4,
                $"Gauge/{theme.Key}/{size}/{style}: continuous fractional dragging retains precision when crossing integer targets ({string.Join(',', widths)})");
            Motion.SetEnabled(window, false);
            chart.Value = 71.23;
            Flush();
            using var fractional = window.CaptureRenderedFrame()!;
            var fractionalWidth = GaugeNumberInk(fractional).Width;
            Motion.SetEnabled(window, true);
            chart.Value = 79;
            Flush();
            AdvanceAnimation(90);
            using var restarted = window.CaptureRenderedFrame()!;
            check(chart.IsAnimating && Math.Abs(GaugeNumberInk(restarted).Width - fractionalWidth) <= 4,
                $"Gauge/{theme.Key}/{size}/{style}: a settled fractional value animates to an integer without a precision jump");
            window.Close();
            Flush();
        }

        foreach (var width in new[] { 180, 300, 560 })
        {
            var chart = new GaugeChart
            {
                GaugeStyle = GaugeStyle.Grade, Width = width, Height = width == 180 ? 160 : width * 0.8,
                Value = 70, ShowGaugeNeedle = false, IsAnimationEnabled = false, GaugeValueBrush = Brushes.Transparent,
                MutedBrush = Brushes.Magenta
            };
            ChartControlFactory.ConfigureGauge(chart, GaugeStyle.Grade);
            chart.GaugeCaption = null;
            var window = new Window
            {
                Width = width + 32, Height = chart.Height + 32, RequestedThemeVariant = theme,
                WindowDecorations = WindowDecorations.None,
                Content = new Border { Padding = new Thickness(16), Child = chart }
            };
            ControlSizing.SetSize(window, size);
            Motion.SetEnabled(window, false);
            window.Show();
            Flush();
            using var labelled = window.CaptureRenderedFrame()!;
            var labelledPixels = Pixels(labelled);
            chart.MutedBrush = Brushes.Transparent;
            Flush();
            using var unlabelled = window.CaptureRenderedFrame()!;
            var unlabelledPixels = Pixels(unlabelled);
            var coloured = 0;
            var covered = 0;
            var labelInk = 0;
            for (var y = 0; y < unlabelled.PixelSize.Height; y++)
            for (var x = 0; x < unlabelled.PixelSize.Width; x++)
            {
                var expected = Pixel(unlabelled, unlabelledPixels, x, y);
                var actual = Pixel(labelled, labelledPixels, x, y);
                if (actual.R > 180 && actual.B > 180 && actual.G < 120) labelInk++;
                if (expected.R + expected.G + expected.B < 200 ||
                    Math.Max(expected.R, Math.Max(expected.G, expected.B)) -
                    Math.Min(expected.R, Math.Min(expected.G, expected.B)) < 60) continue;
                coloured++;
                if (Math.Abs(expected.R - actual.R) + Math.Abs(expected.G - actual.G) +
                    Math.Abs(expected.B - actual.B) > 20) covered++;
            }

            check(labelInk > 20 && coloured > 20 && covered == 0,
                $"Gauge/{theme.Key}/{size}/Grade/{width}: rotated grade labels cover none of the coloured band or tick pixels ({covered}/{coloured})");
            window.Close();
            Flush();
        }
    }

    private static PixelRect GaugeNumberInk(Bitmap bitmap)
    {
        var bytes = Pixels(bitmap);
        var left = bitmap.PixelSize.Width;
        var right = -1;
        var top = bitmap.PixelSize.Height;
        var bottom = -1;
        for (var y = 0; y < bitmap.PixelSize.Height; y++)
        for (var x = 0; x < bitmap.PixelSize.Width; x++)
        {
            var color = Pixel(bitmap, bytes, x, y);
            if (color.R < 150 || color.R <= color.G * 2 || color.R <= color.B * 2) continue;
            left = Math.Min(left, x);
            right = Math.Max(right, x);
            top = Math.Min(top, y);
            bottom = Math.Max(bottom, y);
        }

        return right < left ? default : new PixelRect(left, top, right - left + 1, bottom - top + 1);
    }
}