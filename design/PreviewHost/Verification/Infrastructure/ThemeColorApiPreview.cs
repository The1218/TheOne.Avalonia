using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;
using TheOne.Avalonia.Themes;

namespace TheOne.DesignPreview;

internal static class ThemeColorApiPreview
{
    internal static void Verify(Action<bool, string> check)
    {
        var app = Application.Current!; var theme = app.Styles.OfType<OneTheme>().Single(); var original = theme.ColorTheme;
        OneTheme.SetColor(ThemeColor.Atlas); Flush();
        var view = new ThemeColorApiFixture(); var picker = view.FindControl<ThemeColorPicker>("Picker")!;
        var window = new Window { Width = 400, Height = 230, RequestedThemeVariant = ThemeVariant.Light, Content = view };
        Motion.SetEnabled(window, false); window.Show(); Flush();
        try
        {
            check(theme.ColorTheme == ThemeColor.Nova && picker.SelectedColor == ThemeColor.Nova,
                "ThemeColor/API: an explicit compiled Nova binding overrides the previous application palette");
            check(picker.ItemsSource!.Cast<object>().All(item => item is ThemeColor) && picker.SelectedItem is ThemeColor.Nova,
                "ThemeColor/API: ItemsSource and SelectedItem are actual enum values");
            var text = view.FindControl<TextBlock>("ColorText")!;
            var foreground = text.Foreground; var changesToForeground = 0;
            text.PropertyChanged += (_, args) => { if (args.Property == TextBlock.ForegroundProperty) changesToForeground++; };
            OneTheme.SetColor(ThemeColor.Vega); Flush();
            check(ReferenceEquals(foreground, text.Foreground) && changesToForeground == 0,
                "ThemeColor/API: palette changes preserve the text brush without a foreground property reset");
            check(window.TryFindResource("One.AccentColor", ThemeVariant.Light, out var raw) && raw is Color rawColor &&
                (view.FindControl<Border>("RawColor")!.Background as ISolidColorBrush)?.Color == rawColor,
                "ThemeColor/API: DynamicResource Color bindings still receive the published palette");
            check(view.Model.Color == ThemeColor.Vega && picker.SelectedColor == ThemeColor.Vega,
                "ThemeColor/API: one-call switching synchronizes the compiled two-way binding");
            view.Model.Color = ThemeColor.Terra; Flush();
            check(theme.ColorTheme == ThemeColor.Terra && picker.SelectedItem is ThemeColor.Terra,
                "ThemeColor/API: the VM still drives the selector and resources after an API switch");
            check(picker.SelectThemeColor == ThemeColor.Terra && view.FindControl<ThemeColorPicker>("LegacyPicker")!.SelectedColor == ThemeColor.Terra,
                "ThemeColor/API: SelectThemeColor and the compatible SelectedColor binding stay synchronized");
            picker.Focus(); Press(window, Key.Down, RawInputModifiers.Alt); Press(window, Key.Home); Press(window, Key.Down); Press(window, Key.Enter);
            check(theme.ColorTheme == ThemeColor.Atlas && view.Model.Color == ThemeColor.Atlas,
                "ThemeColor/API: native keyboard enum selection updates resources and the compiled VM");

            var changes = 0;
            void Changed(object? sender, ResourcesChangedEventArgs args) => changes++;
            app.ResourcesChanged += Changed;
            try
            {
                OneTheme.SetColor(ThemeColor.Helios); OneTheme.SetColor(ThemeColor.Vega); OneTheme.SetColor(ThemeColor.NovaDeep);
                check(changes == 0 && theme.ColorTheme == ThemeColor.NovaDeep,
                    "ThemeColor/API: rapid calls change the enum immediately without synchronous global refreshes");
                check(window.TryFindResource("One.Accent", ThemeVariant.Light, out var resource) && resource is ISolidColorBrush current
                    && window.TryFindResource("SystemAccentColor", ThemeVariant.Light, out var native) && native is Color accent && current.Color == accent,
                    "ThemeColor/API: One and Fluent lookups see the same final palette before notification");
                Flush();
                check(changes == 1 && view.Model.Color == ThemeColor.NovaDeep,
                    "ThemeColor/API: rapid calls publish only the final palette once at render priority");
                changes = 0; OneTheme.SetColor(ThemeColor.NovaDeep); Flush();
                check(changes == 0, "ThemeColor/API: choosing the current enum creates no resource notification");
                OneTheme.SetColor(ThemeColor.Terra); OneTheme.SetColor(ThemeColor.NovaDeep); Flush();
                check(changes == 0, "ThemeColor/API: a burst ending at the published palette skips redundant refresh");
            }
            finally { app.ResourcesChanged -= Changed; }

            var rejected = false;
            try { OneTheme.SetColor((ThemeColor)999); } catch (ArgumentOutOfRangeException) { rejected = true; }
            check(rejected && theme.ColorTheme == ThemeColor.NovaDeep, "ThemeColor/API: invalid enums fail without changing the palette");
            var live = (SolidColorBrush)text.Foreground!; var redirected = false; var notifications = 0;
            void Redirect(object? sender, AvaloniaPropertyChangedEventArgs args)
            {
                if (args.Property != SolidColorBrush.ColorProperty || redirected) return;
                redirected = true; OneTheme.SetColor(ThemeColor.Terra);
            }
            void Published(object? sender, ResourcesChangedEventArgs args) => notifications++;
            live.PropertyChanged += Redirect; app.ResourcesChanged += Published;
            try
            {
                OneTheme.SetColor(ThemeColor.Helios); Flush();
                check(theme.ColorTheme == ThemeColor.Terra && notifications == 1 && window.TryFindResource("One.TextColor", ThemeVariant.Light, out var finalText) && Equals(finalText, live.Color),
                    "ThemeColor/API: a brush observer can redirect a switch without publishing stale colors");
            }
            finally { live.PropertyChanged -= Redirect; app.ResourcesChanged -= Published; OneTheme.SetColor(ThemeColor.NovaDeep); Flush(); }
            window.RequestedThemeVariant = ThemeVariant.Dark; Flush();
            check(theme.ColorTheme == ThemeColor.NovaDeep && picker.SelectedColor == ThemeColor.NovaDeep,
                "ThemeColor/API: Dark switching preserves the color enum");
        }
        finally { window.Close(); OneTheme.SetColor(original); Flush(); }
    }
    private static void Press(TopLevel root, Key key, RawInputModifiers modifiers = RawInputModifiers.None)
    { root.KeyPress(key, modifiers, PhysicalKey.None, null); root.KeyRelease(key, modifiers, PhysicalKey.None, null); Flush(); }
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
}
