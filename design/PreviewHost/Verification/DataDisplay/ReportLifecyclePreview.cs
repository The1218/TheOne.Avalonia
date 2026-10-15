using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Reporting;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class ReportLifecyclePreview
{
    internal static void Verify(Action<bool, string> check, string? output = null)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in new[] { ControlSize.Small, ControlSize.Normal, ControlSize.Large })
        {
            foreach (var card in new[] { false, true })
            {
                var tabs = new TabControl(); if (card) tabs.Classes.Add("card");
                tabs.Items.Add(new TabItem { Header = "第一项", Content = "内容一" });
                tabs.Items.Add(new TabItem { Header = "第二项", Content = "内容二" });
                VerifyCached(tabs, [tabs], check, $"standalone {(card ? "card" : "plain")} TabControl", theme, size, output);
            }
            var report = ReportDesignDemo.CreateDesignerSample();
            VerifyCached(report, new[] { "SourceTabs", "ViewTabs", "PropertyTabs" }.Select(name => report.FindControl<TabControl>(name)!).ToArray(), check, "ReportDesign", theme, size, output);
        }
        VerifyOverflow(check);
    }

    private static void VerifyCached(Control page, TabControl[] tabs, Action<bool, string> check, string label, ThemeVariant theme, ControlSize size, string? output)
    {
        var host = new ContentControl { Content = page };
        var window = new Window { Content = host, Width = 1586, Height = 992, RequestedThemeVariant = theme, WindowDecorations = WindowDecorations.None };
        Motion.SetEnabled(window, false); ControlSizing.SetSize(window, size); window.Show(); Flush();
        var reportJson = (page as ReportDesign)?.Document?.Save();
        try
        {
            for (var cycle = 0; cycle < 4; cycle++)
            {
                foreach (var tab in tabs)
                {
                    var headers = tab.GetRealizedContainers().OfType<TabItem>().ToArray();
                    var presenter = tab.GetVisualDescendants().OfType<ItemsPresenter>().First(part => part.Name == "PART_ItemsPresenter");
                    check(headers.Length == tab.Items.Count && presenter.Panel?.Children.Count == tab.Items.Count
                        && headers.All(item => item.GetVisualParent() is not null && item.Bounds.Width > 1 && item.Bounds.Height > 1 && item.IsEffectivelyVisible),
                        $"Report lifecycle/{label}/{theme}/{size}/{tab.Name}: headers remain in the visual tree after cached-page cycle {cycle}");
                    var previous = tab.SelectedIndex;
                    Click(window, headers[1 - previous]);
                    check(tab.SelectedIndex == 1 - previous, $"Report lifecycle/{label}/{tab.Name}: real pointer changes the selected tab after cycle {cycle}");
                    Click(window, headers[previous]);
                }
                if (output is not null && cycle is 0 or 1)
                {
                    Directory.CreateDirectory(output); using var frame = window.CaptureRenderedFrame()!;
                    frame.Save(Path.Combine(output, $"{label.Replace(' ', '-')}-{theme}-{size}-cycle-{cycle}.png"), PngBitmapEncoderOptions.Default);
                }
                host.Content = new TextBlock { Text = "其他预览页面" }; Flush();
                host.Content = page; Flush();
                if (page is ReportDesign report) check(report.Document?.Save() == reportJson, "Report lifecycle: cached navigation retains the report document and field bindings");
            }
        }
        finally { window.Close(); Flush(); }
    }

    private static void VerifyOverflow(Action<bool, string> check)
    {
        foreach (var placement in new[] { Dock.Top, Dock.Bottom, Dock.Left, Dock.Right })
        {
            var tabs = new TabControl { TabStripPlacement = placement }; tabs.Classes.Add("card");
            for (var i = 0; i < 30; i++) tabs.Items.Add(new TabItem { Header = "报表 " + i, Content = "内容 " + i });
            var host = new ContentControl { Content = tabs }; var window = new Window { Content = host, Width = 500, Height = 320 };
            Motion.SetEnabled(window, false); window.Show(); Flush();
            try
            {
                tabs.SelectedIndex = 29; Flush(); host.Content = new TextBlock(); Flush(); host.Content = tabs; Flush();
                var selected = tabs.ContainerFromIndex(29) as TabItem;
                check(selected?.GetVisualParent() is not null && tabs.GetRealizedContainers().Count() < 30 && tabs.SelectedIndex == 29,
                    $"Report lifecycle/card/{placement}: overflow navigation preserves selection and native virtualization after reattachment");
                var vertical = placement is Dock.Left or Dock.Right; var key = vertical ? Key.Up : Key.Left; var physical = vertical ? PhysicalKey.ArrowUp : PhysicalKey.ArrowLeft;
                selected!.Focus(); window.KeyPress(key, RawInputModifiers.None, physical, null); window.KeyRelease(key, RawInputModifiers.None, physical, null); Flush();
                check(tabs.SelectedIndex < 29, $"Report lifecycle/card/{placement}: native keyboard navigation still works");
                TabFeatures.SetCanClose(tabs, true);
                var close = tabs.ContainerFromIndex(tabs.SelectedIndex) as TabItem;
                check(close is not null && TabFeatures.Close(close) && tabs.Items.Count == 29, $"Report lifecycle/card/{placement}: native tab close remains available after reattachment");
            }
            finally { window.Close(); Flush(); }
        }
    }
    private static void Click(Window window, Control control)
    {
        var position = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
        window.MouseMove(position); window.MouseDown(position, MouseButton.Left); window.MouseUp(position, MouseButton.Left); Flush();
    }

    private static void Flush() { for (var i = 0; i < 3; i++) { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); } }
}
