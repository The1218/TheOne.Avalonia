using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace TheOne.Avalonia.Themes;

internal static partial class ThemePalettes
{
    private static void AddButtonBrushes(ResourceDictionary resources, Dictionary<string, Color> tokens)
    {
        // Build once per palette. Shared gradients and existing single-class button colors are unchanged.
        foreach (var name in new[] { "Success", "Info", "Warning", "Danger" })
        {
            var color = tokens[name];
            var foreground = ButtonLuminance(color) > .179 ? Colors.Black : Colors.White;
            var shade = foreground == Colors.White ? Colors.Black : Colors.White;
            resources["One.Button." + name + "Foreground"] = Solid(foreground);
            foreach (var (suffix, amount) in new[] { ("", 0d), ("Hover", .06), ("Pressed", .12) })
            {
                // Feedback moves away from the foreground, preserving text contrast at both gradient ends.
                var start = Mix(color, shade, amount);
                var end = Mix(color, shade, amount + .08);
                if (suffix.Length > 0) resources["One.Button." + name + suffix] = Solid(start);
                resources["One.Button.Gradient." + name + suffix] = new LinearGradientBrush
                {
                    StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                    EndPoint = new RelativePoint(1, .35, RelativeUnit.Relative),
                    GradientStops = new GradientStops { new GradientStop(start, 0), new GradientStop(end, 1) }
                }.ToImmutable();
            }
        }
    }

    private static double ButtonLuminance(Color color)
    {
        static double Linear(byte channel)
        {
            var value = channel / 255d;
            return value <= .04045 ? value / 12.92 : Math.Pow((value + .055) / 1.055, 2.4);
        }
        return .2126 * Linear(color.R) + .7152 * Linear(color.G) + .0722 * Linear(color.B);
    }
}
