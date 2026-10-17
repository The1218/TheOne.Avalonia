using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Reporting;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class ReportFontsPreview
{
    internal static void Verify(Action<bool, string> check)
    {
        var systemFonts = FontManager.Current.SystemFonts.Select(family => family.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name)).Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase).ToArray();
        check(systemFonts.Length > 0,
            "ReportDesign/fonts: the native font manager exposes this computer's installed font families");
        var missingFamily = "ReportDesign unavailable server font";
        while (systemFonts.Contains(missingFamily, StringComparer.OrdinalIgnoreCase)) missingFamily += "_";
        var document = new ReportDocument();
        var title = new ReportElement
        {
            Name = "Server title", Kind = ReportElementKind.Text, FontFamily = missingFamily, X = 14, Y = 14,
            Width = 100, Height = 12
        };
        var other = new ReportElement
        {
            Name = "Local title", Kind = ReportElementKind.Text, FontFamily = systemFonts.First(), X = 14, Y = 28,
            Width = 100, Height = 12
        };
        document.Elements.Add(title);
        document.Elements.Add(other);
        var view = new ReportDesign { Document = document, SelectedElement = title };
        var window = new Window
        {
            Content = view, Width = 1586, Height = 992, RequestedThemeVariant = ThemeVariant.Light,
            WindowDecorations = WindowDecorations.None
        };
        ControlSizing.SetSize(window, ControlSize.Normal);
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        try
        {
            var picker = view.FindControl<ComboBox>("FontFamilyPicker")!;
            var names = picker.Items.OfType<string>().ToArray();
            var missing = systemFonts.Except(names, StringComparer.OrdinalIgnoreCase).ToArray();
            check(missing.Length == 0,
                $"ReportDesign/fonts: the font picker includes every native installed font family (native={systemFonts.Length}, items={names.Length}, source={(picker.ItemsSource as IEnumerable<string>)?.Count()}, missing={string.Join(", ", missing.Take(10))})");
            check(names.Length == names.Distinct(StringComparer.OrdinalIgnoreCase).Count()
                  && names.SequenceEqual(names.Order(StringComparer.OrdinalIgnoreCase)),
                "ReportDesign/fonts: font families are deduplicated and sorted");
            check((string?)picker.SelectedItem == missingFamily && view.FontFamilyName == missingFamily
                                                                && ReportDocument.Load(view.SaveTemplate()).Elements
                                                                    .First().FontFamily == missingFamily,
                "ReportDesign/fonts: an unavailable server-template font remains selected and survives JSON serialization");
            var originalSource = picker.ItemsSource;
            view.SelectedElement = other;
            Flush();
            check(
                ReferenceEquals(originalSource, picker.ItemsSource) && (string?)picker.SelectedItem == other.FontFamily,
                "ReportDesign/fonts: changing the selected element reuses the font collection and selects its real family name");
            view.SelectedElement = title;
            Flush();
            picker.SelectedItem = systemFonts.Last();
            Flush();
            check(title.FontFamily == systemFonts.Last(),
                "ReportDesign/fonts: selecting a native font updates the report through the compiled two-way binding");
            title.FontFamily = missingFamily + " external";
            Flush();
            check((string?)picker.SelectedItem == title.FontFamily && picker.Items.Contains(title.FontFamily),
                "ReportDesign/fonts: an external font change adds the unavailable family and keeps the native selection in sync");
            var canonicalFamily = systemFonts.First(name => name.Any(char.IsLetter));
            title.FontFamily = canonicalFamily.ToUpperInvariant();
            Flush();
            check((string?)picker.SelectedItem == canonicalFamily
                  && picker.Items.OfType<string>()
                      .Count(name => StringComparer.OrdinalIgnoreCase.Equals(name, canonicalFamily)) == 1,
                "ReportDesign/fonts: template casing differences reuse the installed font entry without duplicates");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }
}