using System.Globalization;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Utilities;

namespace TheOne.Avalonia.Controls;

public partial class ChartBase
{
    private List<CartesianFrame>? _scatterTextFrames;

    private void DrawCartesianText(DrawingContext context, CartesianFrame frame, string value, IBrush? brush, Point position)
    {
        if (!IsScatterCartesian)
        {
            context.DrawText(CartesianText(frame, value, brush), position);
            return;
        }
        if (frame.ScatterTexts is not { } texts)
        {
            frame.ScatterTexts = texts = [];
            (_scatterTextFrames ??= []).Add(frame);
        }
        var key = (value, brush);
        if (!texts.TryGetValue(key, out var layout))
        {
            if (texts.Count >= 512) ReleaseScatterTexts(frame, false);
            var typeface = new Typeface(FontFamily);
            var foreground = brush ?? Foreground ?? Brushes.Black;
            // Match FormattedText's explicit culture, font and flow while retaining the formatted lines.
            var properties = new GenericTextRunProperties(typeface, FontSize, foregroundBrush: foreground,
                cultureInfo: CultureInfo.CurrentCulture);
            ValueSpan<TextRunProperties>[] overrides = [new(0, value.Length, properties)];
            layout = new TextLayout(value, typeface, FontSize, foreground, flowDirection: FlowDirection,
                textStyleOverrides: overrides, textRunCache: null);
            texts.Add(key, layout);
        }
        layout.Draw(context, position);
    }

    private static void ReleaseScatterTexts(CartesianFrame frame, bool releaseDictionary = true)
    {
        if (frame.ScatterTexts is not { } texts) return;
        foreach (var layout in texts.Values) layout.Dispose();
        texts.Clear();
        if (releaseDictionary)
        {
            frame.ScatterTexts = null;
            frame.Texts.Clear();
        }
    }

    private void TrimScatterTextFrames()
    {
        if (_scatterTextFrames is not { } frames) return;
        for (var index = frames.Count - 1; index >= 0; index--)
        {
            var frame = frames[index];
            if (ReferenceEquals(frame, _cartesianFrame) || ReferenceEquals(frame, _animationFromCartesian)) continue;
            ReleaseScatterTexts(frame); frames.RemoveAt(index);
        }
    }

    private void ClearScatterTextFrames()
    {
        if (_scatterTextFrames is not { } frames) return;
        foreach (var frame in frames) ReleaseScatterTexts(frame);
        frames.Clear();
    }
}
