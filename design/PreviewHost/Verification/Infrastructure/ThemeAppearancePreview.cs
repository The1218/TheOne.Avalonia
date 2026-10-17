using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;
using TheOne.Avalonia.Themes;

namespace TheOne.DesignPreview;

internal static class ThemeAppearancePreview
{
    internal static void Verify(Action<bool, string> check, string? output)
    {
        var app = Application.Current!;
        var theme = app.Styles.OfType<OneTheme>().Single();
        var originalColor = theme.ColorTheme;
        var originalVariant = app.RequestedThemeVariant;
        try
        {
            check(
                OneTheme.ColorThemes.SequenceEqual(Enum.GetValues<ThemeColor>()) &&
                OneTheme.ColorThemes[0] == ThemeColor.Nova,
                "ThemeColor: enum values include Nova and all nine other palettes without wrapper objects");
            foreach (var variant in new[] { ThemeVariant.Light, ThemeVariant.Dark })
            foreach (var size in Enum.GetValues<ControlSize>())
            foreach (var width in new[] { 600, 320 })
            {
                var picker = new ThemeColorPicker { HorizontalAlignment = HorizontalAlignment.Stretch };
                var toggle = new ThemeButton();
                var primary = new Button
                    { Content = "保存设置", Classes = { "primary" }, HorizontalAlignment = HorizontalAlignment.Stretch };
                var content = new StackPanel
                {
                    Spacing = 14, Children =
                    {
                        picker, toggle, primary,
                        new TextBox { Text = "主题资源实时更新" },
                        new ListBox { Height = 110, ItemsSource = new[] { "列表选中", "保留留白" }, SelectedIndex = 0 }
                    }
                };
                var window = new Window
                {
                    Width = width, Height = 440, RequestedThemeVariant = variant,
                    Content = new Border { Padding = new Thickness(20), Child = content }
                };
                ControlSizing.SetSize(window, size);
                Motion.SetEnabled(window, false);
                window.Show();
                Flush();
                try
                {
                    foreach (var color in OneTheme.ColorThemes)
                    {
                        var changes = 0;
                        var previous = theme.ColorTheme;

                        void Changed(object? sender, ResourcesChangedEventArgs args)
                        {
                            changes++;
                        }

                        app.ResourcesChanged += Changed;
                        try
                        {
                            OneTheme.SetColor(color);
                            Flush();
                        }
                        finally
                        {
                            app.ResourcesChanged -= Changed;
                        }

                        var label = $"ThemeColor/{color}/{variant}/{size}/{width}";
                        var swatches = picker.GetVisualDescendants().OfType<Border>()
                            .Where(border => border.Width == 12 && border.Height == 20).ToArray();
                        check(
                            swatches.Length == 2 && picker.ItemsSource!.Cast<object>().All(item => item is ThemeColor),
                            $"{label}: native selector displays enum names and both accent swatches");
                        var expected = (ISolidColorBrush)swatches[variant == ThemeVariant.Dark ? 1 : 0].Background!;
                        check(changes == (previous == color ? 0 : 1),
                            $"{label}: one-call API publishes exactly one atomic update and skips identical colors");
                        check(
                            picker.SelectedColor == color && picker.SelectedItem is ThemeColor selected &&
                            selected == color,
                            $"{label}: external palette changes synchronize the selector");
                        check(
                            window.ActualThemeVariant == variant && toggle.IsChecked == (variant == ThemeVariant.Dark),
                            $"{label}: palette changes retain Light / Dark and toggle state");
                        check(
                            window.TryFindResource("One.Accent", window.ActualThemeVariant, out var accent) &&
                            accent is ISolidColorBrush brush && brush.Color == expected.Color,
                            $"{label}: live semantic accent matches the selected resources (actual {accent}, expected {expected})");
                        check(
                            window.TryFindResource("SystemAccentColor", variant, out var nativeAccent) &&
                            nativeAccent is Color native && native == expected.Color,
                            $"{label}: Fluent fallback accent is synchronized in the same update");
                        check(picker.Bounds.Width <= width - 40 + 1 && toggle.Bounds.Width == toggle.Bounds.Height,
                            $"{label}: selector and icon toggle fit their viewport and density");
                        using var frame = window.CaptureRenderedFrame()!;
                        check(CountColor(frame, expected.Color) > 30, $"{label}: actual pixels paint the new accent");
                        if (output is not null)
                        {
                            Directory.CreateDirectory(output);
                            frame.Save(Path.Combine(output, $"palette-{color}-{variant}-{size}-{width}.png"),
                                PngBitmapEncoderOptions.Default);
                        }
                    }

                    picker.Focus();
                    Press(window, Key.Down, KeyModifiers.Alt);
                    Press(window, Key.Home);
                    Press(window, Key.Down);
                    Press(window, Key.Enter);
                    Flush();
                    check(theme.ColorTheme == ThemeColor.Atlas && picker.SelectedColor == ThemeColor.Atlas,
                        $"ThemeColor/{variant}/{size}/{width}: native keyboard selection changes resources");
                    var before = window.ActualThemeVariant;
                    Click(window, toggle);
                    check(
                        window.ActualThemeVariant != before &&
                        toggle.IsChecked == (window.ActualThemeVariant == ThemeVariant.Dark),
                        $"ThemeButton/{variant}/{size}/{width}: real pointer toggles the effective variant");
                    toggle.Focus();
                    Press(window, Key.Space);
                    check(window.ActualThemeVariant == before && theme.ColorTheme == ThemeColor.Atlas,
                        $"ThemeButton/{variant}/{size}/{width}: native Space restores variant without changing color resources");
                    check(
                        toggle is ToggleButton && toggle.Content is null && toggle.GetVisualDescendants()
                            .OfType<PathIcon>().Count(icon => icon.IsVisible) == 1,
                        $"ThemeButton/{variant}/{size}/{width}: toggle semantics and one built-in icon, without text");
                }
                finally
                {
                    window.Close();
                    Flush();
                }
            }

            VerifyApplicationScope(check, app, theme);
            VerifyPickerLifetime(check, theme);
        }
        finally
        {
            theme.ColorTheme = originalColor;
            app.RequestedThemeVariant = originalVariant;
            Flush();
        }
    }

    private static void VerifyPickerLifetime(Action<bool, string> check, OneTheme theme)
    {
        theme.ColorTheme = ThemeColor.Atlas;
        var picker = new ThemeColorPicker { SelectedColor = ThemeColor.Terra };
        var window = new Window { Width = 260, Height = 150, Content = picker };
        window.Show();
        Flush();
        try
        {
            check(theme.ColorTheme == ThemeColor.Terra,
                "ThemeColor/lifetime: explicit initial selection applies when attached");
            window.Content = null;
            theme.ColorTheme = ThemeColor.Vega;
            window.Content = picker;
            Flush();
            check(theme.ColorTheme == ThemeColor.Vega && picker.SelectedColor == ThemeColor.Vega,
                "ThemeColor/lifetime: reattachment adopts live resources instead of restoring stale selection");
            window.Content = null;
            picker.SelectedColor = ThemeColor.Helios;
            window.Content = picker;
            Flush();
            check(theme.ColorTheme == ThemeColor.Helios,
                "ThemeColor/lifetime: an explicit detached selection is applied on reattachment");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void VerifyApplicationScope(Action<bool, string> check, Application app, OneTheme theme)
    {
        app.RequestedThemeVariant = ThemeVariant.Light;
        theme.ColorTheme = ThemeColor.Terra;
        var toggle = new ThemeButton { Scope = ThemeButtonScope.Application };
        var window = new Window { Width = 260, Height = 150, Content = toggle };
        window.Show();
        Flush();
        try
        {
            Click(window, toggle);
            check(
                app.RequestedThemeVariant == ThemeVariant.Dark && toggle.IsChecked == true &&
                theme.ColorTheme == ThemeColor.Terra,
                "ThemeButton/application: pointer switches global Dark without changing the selected palette");
            app.RequestedThemeVariant = ThemeVariant.Light;
            Flush();
            check(toggle.IsChecked == false, "ThemeButton/application: external Light changes update checked state");
            toggle.IsEnabled = false;
            Click(window, toggle);
            check(app.RequestedThemeVariant == ThemeVariant.Light && toggle.IsChecked == false,
                "ThemeButton/application: disabled pointer leaves the mode unchanged");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    internal static void Click(Window window, Control control)
    {
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!
            .Value;
        window.MouseMove(point);
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        Flush();
    }

    internal static void Press(Window window, Key key, KeyModifiers modifiers = KeyModifiers.None)
    {
        window.KeyPress(key, (RawInputModifiers)modifiers, PhysicalKey.None, null);
        window.KeyRelease(key, (RawInputModifiers)modifiers, PhysicalKey.None, null);
        Flush();
    }

    internal static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private static int CountColor(Bitmap frame, Color color)
    {
        var length = frame.PixelSize.Width * frame.PixelSize.Height * 4;
        var memory = Marshal.AllocHGlobal(length);
        try
        {
            var data = new byte[length];
            frame.CopyPixels(new PixelRect(frame.PixelSize), memory, length, frame.PixelSize.Width * 4);
            Marshal.Copy(memory, data, 0, length);
            var result = 0;
            var bgra = frame.Format == PixelFormat.Bgra8888;
            for (var i = 0; i < length; i += 4)
                if (data[i] == (bgra ? color.B : color.R) && data[i + 1] == color.G &&
                    data[i + 2] == (bgra ? color.R : color.B))
                    result++;
            return result;
        }
        finally
        {
            Marshal.FreeHGlobal(memory);
        }
    }
}