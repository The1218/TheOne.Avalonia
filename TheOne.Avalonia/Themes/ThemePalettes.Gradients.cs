using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace TheOne.Avalonia.Themes;

internal static partial class ThemePalettes
{
    // Canonical Nova tokens from Colors.axaml; avoid a runtime AXAML loader in trimmed consumers.
    private static readonly (string Name, string Light, string Dark)[] NovaGradientColors =
    [
        ("Surface", "#FFFFFF", "#0F0F0F"),
        ("SurfaceHover", "#F2F2F2", "#212222"),
        ("Accent", "#006061", "#63D9DE"),
        ("AccentHover", "#006A6B", "#5DD2D5"),
        ("AccentPressed", "#007475", "#55CBCE"),
        ("AccentSoft", "#C1DCDC", "#284343"),
        ("Info", "#395DC6", "#9EB4FF"),
        ("Success", "#007A06", "#57D15D"),
        ("Warning", "#855D00", "#DDB35F"),
        ("Danger", "#99000D", "#FFBDC2"),
        ("RowSelected", "#DFF1F1", "#162C2C")
    ];

    private static void AddGradients(ResourceDictionary resources, Dictionary<string, Color> tokens, bool dark)
    {
        AddButtonBrushes(resources, tokens);

        // Subtle backgrounds keep their contrast gentle; vivid variants emphasize the palette's accent.
        foreach (var (name, target) in new[]
                     { ("Brand", "Accent"), ("Cool", "Info"), ("Natural", "Success"), ("Warm", "Warning") })
        {
            Add("Subtle." + name, Mix(tokens["Surface"], tokens[target], dark ? .08 : .025),
                Mix(tokens["Surface"], tokens[target], dark ? .16 : .09));
            Add("Vivid." + name, tokens["Accent"], name == "Brand"
                ? Mix(tokens["Accent"], dark ? Colors.Black : Colors.White, .42)
                : tokens[target]);
        }

        void Add(string name, Color start, Color end)
        {
            var prefix = "One.Gradient." + name;
            resources[prefix + ".StartColor"] = start;
            resources[prefix + ".EndColor"] = end;
            resources[prefix] = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, .35, RelativeUnit.Relative),
                GradientStops = new GradientStops { new GradientStop(start, 0), new GradientStop(end, 1) }
            }.ToImmutable();
        }
    }
}
