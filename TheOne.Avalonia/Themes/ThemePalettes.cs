using global::Avalonia.Controls;
using global::Avalonia.Media;
using global::Avalonia.Media.Immutable;
using global::Avalonia.Styling;
using global::Avalonia.Themes.Fluent;

namespace TheOne.Avalonia.Themes;

internal static partial class ThemePalettes
{
    private static readonly Dictionary<ThemeColor, ResourceDictionary> Cache = [];
    private static readonly Dictionary<Color, ImmutableSolidColorBrush> SolidBrushes = [];
    private static ImmutableSolidColorBrush Solid(Color color)
    {
        if (!SolidBrushes.TryGetValue(color, out var brush)) SolidBrushes.Add(color, brush = new ImmutableSolidColorBrush(color));
        return brush;
    }
    private static readonly string[] AccentKeys = ["SystemAccentColor", "SystemAccentColorDark1", "SystemAccentColorDark2", "SystemAccentColorDark3",
        "SystemAccentColorLight1", "SystemAccentColorLight2", "SystemAccentColorLight3", "SystemRegionColor"];

    internal static IEnumerable<string> SolidKeys() => Names.Select(name => "One." + name).Concat([
        "One.SuccessSoft", "One.InfoSoft", "One.WarningSoft", "One.DangerSoft", "One.InfoCard.Footer",
        "One.Workflow.Canvas", "One.Workflow.Note", "One.Gantt.Background", "One.Gantt.Text", "One.Gantt.Grid",
        "One.Gantt.Header", "One.Gantt.Weekend", "One.Gantt.Task", "One.Gantt.Progress", "One.Gantt.Selection", "One.Gantt.Muted", "One.Gantt.OnTask"]);

    internal static ResourceDictionary Get(ThemeColor color)
    {
        if (!Cache.TryGetValue(color, out var resources)) Cache.Add(color, resources = Create(color));
        return resources;
    }

    internal static IBrush AccentBrush(ThemeColor color, bool dark)
    {
        Get(color).TryGetResource("One.Palette.AccentSwatch", dark ? ThemeVariant.Dark : ThemeVariant.Light, out var brush);
        return (IBrush)brush!;
    }

    private static ResourceDictionary Create(ThemeColor color)
    {
        var result = new ResourceDictionary();
        foreach (var dark in new[] { false, true })
        {
            var variant = dark ? ThemeVariant.Dark : ThemeVariant.Light;
            var dictionary = new ResourceDictionary();
            if (color == ThemeColor.Nova)
            {
                var nova = NovaGradientColors.ToDictionary(value => value.Name, value => Color.Parse(dark ? value.Dark : value.Light));
                AddGradients(dictionary, nova, dark);
                AddFluentColors(dictionary, variant, Color.Parse(dark ? "#63D9DE" : "#006061"), Color.Parse(dark ? "#0F0F0F" : "#FFFFFF"));
                result.ThemeDictionaries[variant] = dictionary;
                continue;
            }
            var values = (dark ? Values[color].Dark : Values[color].Light).Split(' ').Select(Color.Parse).ToArray();
            var tokens = Names.Select((name, index) => (name, value: values[index])).ToDictionary(pair => pair.name, pair => pair.value);
            foreach (var name in new[] { "Success", "Info", "Warning", "Danger" })
                tokens[name + "Soft"] = Mix(tokens["Surface"], tokens[name], dark ? .14 : .08);
            tokens["InfoCard.Footer"] = Mix(tokens["Surface"], tokens["SurfaceHover"], .2);
            foreach (var (name, value) in tokens)
            {
                dictionary["One." + name + "Color"] = value;
                dictionary["One." + name] = Solid(value);
            }
            foreach (var (name, token) in new[] { ("Background", "Surface"), ("Text", "Text"), ("Grid", "Line"), ("Header", "Subtle"),
                ("Weekend", "Subtle"), ("Task", "AccentSoft"), ("Progress", "Accent"), ("Selection", "RowSelected"), ("Muted", "Muted"), ("OnTask", "OnAccent") })
                dictionary["One.Gantt." + name] = dictionary["One." + token];
            dictionary["One.Workflow.Canvas"] = Solid(Mix(tokens["Surface"], tokens["AccentSoft"], dark ? .18 : .12));
            dictionary["One.Workflow.Note"] = Solid(Mix(tokens["Surface"], tokens["Warning"], dark ? .28 : .15));
            foreach (var (name, start, end) in new[]
            {
                ("Brand", tokens["Accent"], Mix(tokens["Accent"], tokens["Info"], .35)),
                ("BrandHover", tokens["AccentHover"], Mix(tokens["AccentHover"], tokens["Info"], .35)),
                ("BrandPressed", tokens["AccentPressed"], Mix(tokens["AccentPressed"], tokens["Info"], .35)),
                ("Glow", tokens["SurfaceHover"], tokens["RowSelected"]),
                ("Data", tokens["AccentSoft"], tokens["Accent"]),
            })
            {
                var prefix = "One.Gradient." + name;
                dictionary[prefix + ".StartColor"] = start; dictionary[prefix + ".EndColor"] = end;
                dictionary[prefix] = new LinearGradientBrush
                {
                    StartPoint = new global::Avalonia.RelativePoint(0, 0, global::Avalonia.RelativeUnit.Relative),
                    EndPoint = new global::Avalonia.RelativePoint(1, .35, global::Avalonia.RelativeUnit.Relative),
                    GradientStops = new GradientStops { new(start, 0), new(end, 1) },
                }.ToImmutable();
            }
            AddFluentColors(dictionary, variant, tokens["Accent"], tokens["Canvas"]);
            AddGradients(dictionary, tokens, dark);
            result.ThemeDictionaries[variant] = dictionary;
        }
        return result;
    }

    private static void AddFluentColors(ResourceDictionary dictionary, ThemeVariant variant, Color accent, Color region)
    {
        // Calculate native Fluent shades off-tree. Switching publishes them with the One.* resources in one notification.
        var fluent = new ColorPaletteResources { Accent = accent, RegionColor = region };
        foreach (var key in AccentKeys)
            if (fluent.TryGetResource(key, variant, out var value)) dictionary[key] = value;
        dictionary["One.Palette.AccentSwatch"] = Solid(accent);
    }

    private static Color Mix(Color a, Color b, double ratio) => Color.FromArgb(255,
        (byte)Math.Round(a.R * (1 - ratio) + b.R * ratio), (byte)Math.Round(a.G * (1 - ratio) + b.G * ratio), (byte)Math.Round(a.B * (1 - ratio) + b.B * ratio));
}
