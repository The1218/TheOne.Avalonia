using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class NavigationChromePreview
{
    public static void Verify(Action<bool, string> check)
    {
        var groups = PreviewNavigation.CreatePages();
        check(groups.Select(group => group.Icon).Distinct().Count() == groups.Count,
            "Navigation: every category has a distinct semantic icon");
        check(groups.SelectMany(group => group.Entries).Select(entry => entry.Icon).Distinct().Count() >= 20,
            "Navigation: page icons distinguish buttons, inputs, layouts, charts and messages");
        check(
            groups.SelectMany(group => group.Entries)
                .All(entry => entry.Icon.Bounds.Width > 0 && entry.Icon.Bounds.Height > 0),
            "Navigation: every page has non-empty icon geometry");
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var window = new MainWindow { Width = 1080, Height = 850 };
            window.Model.Theme = theme;
            window.Model.Size = size;
            window.Model.MotionEnabled = false;
            window.Show();
            ControlPreview.Flush();
            var label = $"NavigationChrome/{theme.Key}/{size}";
            var navigation = window.FindControl<NavMenu>("Navigation")!;
            foreach (var group in window.Model.NavigationRoots) group.IsExpanded = false;
            ControlPreview.Flush();
            check(
                window.Model.NavigationRoots.All(group =>
                    navigation.GetVisualDescendants().OfType<PathIcon>().Any(icon =>
                        icon.Data == group.Icon && icon.Bounds.Width > 0 && icon.Foreground is SolidColorBrush)),
                label + ": expanded sidebar renders all category glyphs in inherited theme colors");
            window.Model.NavigationCollapsed = true;
            ControlPreview.Flush();
            check(
                navigation.IsCollapsed && window.Model.NavigationRoots.All(group =>
                    navigation.GetVisualDescendants().OfType<PathIcon>()
                        .Any(icon => icon.Data == group.Icon && icon.IsEffectivelyVisible)),
                label + ": collapsed sidebar retains recognizable category icons");
            window.Model.NavigationCollapsed = false;
            var title = Part<Control>(window, "PART_TitleBar");
            check(WindowDecorationProperties.GetElementRole(title) == WindowDecorationsElementRole.User,
                label + ": titlebar has one managed input owner and avoids native double handling");
            var point = title.TranslatePoint(new Point(title.Bounds.Width / 2, title.Bounds.Height / 2), window)!.Value;
            DoubleClick(window, point);
            check(window.WindowState == WindowState.Maximized, label + ": real titlebar double click maximizes");
            DoubleClick(window, point);
            check(window.WindowState == WindowState.Normal,
                label + ": immediate second double click restores, including click count four");
            var headerText = title.GetVisualDescendants().OfType<TextBlock>().First(text => text.Text == window.Title);
            DoubleClick(window, Center(headerText));
            check(window.WindowState == WindowState.Maximized,
                label + ": window title text shares the same double click behavior");
            window.WindowState = WindowState.Normal;
            var left = new Button { Content = "Titlebar action" };
            window.LeftContent = left;
            ControlPreview.Flush();
            DoubleClick(window, Center(left));
            check(window.WindowState == WindowState.Normal, label + ": titlebar content buttons do not maximize");
            window.CanMaximize = false;
            DoubleClick(window, point);
            check(window.WindowState == WindowState.Normal, label + ": disabled maximize capability is respected");
            window.CanMaximize = true;
            window.MouseDown(point, MouseButton.Right);
            window.MouseUp(point, MouseButton.Right);
            ControlPreview.Flush();
            check(window.WindowState == WindowState.Normal, label + ": right click does not change window state");
            var originalTemplate = window.Template;
            window.Template = null;
            window.ApplyTemplate();
            window.Template = originalTemplate;
            window.ApplyTemplate();
            ControlPreview.Flush();
            title = Part<Control>(window, "PART_TitleBar");
            point = title.TranslatePoint(new Point(title.Bounds.Width / 2, title.Bounds.Height / 2), window)!.Value;
            DoubleClick(window, point);
            check(window.WindowState == WindowState.Maximized,
                label + ": retemplating registers exactly one titlebar handler");
            window.Close();
            ControlPreview.Flush();
        }

        OneWindowChecks.Verify(check);
        Console.WriteLine("Runtime dynamic code supported: " + RuntimeFeature.IsDynamicCodeSupported);
    }

    public static void Render(string output)
    {
        Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var collapsed in new[] { false, true })
        {
            var window = new MainWindow { Width = 1080, Height = 850 };
            window.Model.Theme = theme;
            window.Model.Size = size;
            window.Model.MotionEnabled = false;
            window.Model.NavigationCollapsed = collapsed;
            window.Show();
            ControlPreview.Flush();
            using var bitmap = new RenderTargetBitmap(new PixelSize(1080, 850));
            bitmap.Render(window);
            bitmap.Save(
                Path.Combine(output,
                    $"PreviewNavigation-{theme.Key}-{size}-{(collapsed ? "Collapsed" : "Expanded")}.png"),
                PngBitmapEncoderOptions.Default);
            window.Close();
            ControlPreview.Flush();
        }
    }

    private static T Part<T>(Control owner, string name) where T : Control
    {
        return owner.GetVisualDescendants().OfType<T>().Single(control => control.Name == name);
    }

    private static Point Center(Control control)
    {
        return control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2),
            TopLevel.GetTopLevel(control)!)!.Value;
    }

    private static void DoubleClick(Window window, Point point)
    {
        window.MouseMove(point);
        for (var i = 0; i < 2; i++)
        {
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            ControlPreview.Flush();
        }
    }
}