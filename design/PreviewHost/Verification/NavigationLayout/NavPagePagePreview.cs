using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class NavPagePagePreview
{
    internal static void Verify(Action<bool, string> check, string? output = null)
    {
        var descriptor = PreviewPageCatalog.Find(328);
        var page = new ControlPreviewPage(descriptor, PreviewPageCatalog.CreateControl(328));
        var window = new Window { Width = 1040, Height = 720, Content = new ScrollViewer { Content = page } };
        Motion.SetEnabled(window, false);
        window.Show();
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
        var shell = page.GetVisualDescendants().OfType<NavPage>().Single();
        var descriptions = page.GetVisualDescendants().OfType<Description>().ToArray();
        check(descriptor.Title == "NavPage" && shell.DefaultHome is not null && shell.SelectedPage == shell.DefaultHome,
            "NavPage/preview/registered-page-and-home");
        check(descriptions.Length == 1 && descriptions[0].Items.Count >= 9,
            "NavPage/preview/single-authored-description");
        var top = descriptions[0].TranslatePoint(default, page)!.Value.Y;
        var bottom = shell.TranslatePoint(default, page)!.Value.Y + shell.Bounds.Height;
        check(top >= bottom && shell.Bounds.Height >= 440, "NavPage/preview/natural-layout-no-overlap");
        check(shell.RightHeader is Border && shell.GetVisualDescendants().OfType<ContentPresenter>()
            .Any(p => p.Name == "PART_RightHeader" && p.IsVisible), "NavPage/preview/live-right-header");
        var group = shell.ItemsSource!.Cast<NavPageModel>().Single(p => p.Children.Count > 0);
        shell.OpenPage(group.Children.Single(p => p.IsPinned));
        shell.OpenPage(group.Children.Single(p => !p.IsPinned));
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
        if (output is not null)
        {
            foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
            foreach (var size in Enum.GetValues<ControlSize>())
            {
                window.RequestedThemeVariant = theme;
                ControlSizing.SetSize(window, size);
                Dispatcher.UIThread.RunJobs();
                AvaloniaHeadlessPlatform.ForceRenderTimerTick();
                Dispatcher.UIThread.RunJobs();
                using var image = window.CaptureRenderedFrame();
                image?.Save(Path.Combine(output, $"navpage-toolbar-pin-{theme.Key}-{size}.png"), new PngBitmapEncoderOptions());
                check(image is not null, $"NavPage/preview/toolbar-pin-rendered/{theme.Key}/{size}");
            }
        }
        var toggleHeader = page.GetVisualDescendants().OfType<Button>()
            .Single(b => Equals(b.Content, "显示／隐藏 RightHeader"));
        var point = toggleHeader.TranslatePoint(new Point(10, 10), window)!.Value;
        window.MouseDown(point, global::Avalonia.Input.MouseButton.Left);
        window.MouseUp(point, global::Avalonia.Input.MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
        check(shell.RightHeader is null && shell.GetVisualDescendants().OfType<Grid>()
            .Single(p => p.Name == "PART_RightBody").Bounds.Y == 0, "NavPage/preview/toggle-right-header-fills-body");
        window.Close();
        var host = new MainWindow { Width = 1120, Height = 800 };
        Motion.SetEnabled(host, false);
        host.Show();
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
        var navigation = host.FindControl<NavMenu>("Navigation")!;
        var sidebar = host.FindControl<Border>("Sidebar")!;
        check(Math.Abs(sidebar.Bounds.Width - 257) < 1, "NavMenu/host/expanded-auto-column");
        host.Model.NavigationCollapsed = true;
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
        check(Math.Abs(sidebar.Bounds.Width - navigation.CollapsedWidth - 25) < 1, "NavMenu/host/collapsed-auto-column");
        host.Close();
        var tabSample = NativeThemePreview.CreateProjectFeature("TabControl");
        window = new Window { Width = 800, Height = 400, Content = tabSample };
        Motion.SetEnabled(window, false);
        window.Show();
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
        var tabs = tabSample.GetVisualDescendants().OfType<TabControl>().Single();
        check(tabs.Items.Cast<TabItem>().All(t => TabFeatures.GetIcon(t) is not null) &&
              TabFeatures.GetIsPinned((TabItem)tabs.Items[0]!), "TabFeatures/preview/icons-pinning-in-live-sample");
        window.Close();
    }
}
