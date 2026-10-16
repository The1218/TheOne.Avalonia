using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class ControlPagesPreview
{
    internal static void Verify(Action<bool, string> check, string? output = null)
    {
        check(PreviewPageCatalog.Pages.All(p => p.Id != 500 && !p.Title.Contains("实验室")) && PreviewPageCatalog.SearchEntries.All(p => p.Id != 500),
            "Control pages/catalog: removed laboratory has no navigation or search entry");
        check(PreviewPageCatalog.Pages.Where(p => p.ControlIds is not null).Select(p => p.Id).SequenceEqual(new[] { 400, 401, 402 }),
            "Control pages/catalog: only Button, Input and Select use combined pages");
        foreach (var id in new[] { 309, 308, 118, 9, 7, 18, 24, 23, 201, 202, 205, 206, 27, 307, 315, 318, 319 })
            check(PreviewPageCatalog.Find(id).Id == id, $"Control pages/catalog/{id}: control has its own complete preview page");

        var shell = new MainWindow { Width = 1280, Height = 900 }; Motion.SetEnabled(shell, false); shell.Show(); Flush();
        try
        {
            var gallery = shell.FindControl<ControlGallery>("Gallery")!;
            var outer = Part<ScrollViewer>(gallery, "PageScroll");
            foreach (var page in PreviewPageCatalog.Pages.Where(p => p.Id != 0))
            {
                shell.NavigateToCategory(page.Id); Flush();
                var view = gallery.CurrentView!;
                check(view is not null && view.Bounds.Width > 0 && view.Bounds.Height > 0,
                    $"Control pages/render/{page.Title}: original complete demo renders in its page");
                check(gallery.GetVisualDescendants().OfType<ControlDetailsView>().Any() || gallery.GetVisualDescendants().OfType<ControlPreviewPage>().Any(p => p.UsesViewport && p.Details.ControlName == page.Title),
                    $"Control pages/details/{page.Title}: styles, properties and models belong to this page");
            }
            shell.NavigateToCategory(309); Flush();
            var gantt = (GanttDemoView)gallery.CurrentView!;
            var chart = gantt.FindControl<GanttChart>("Schedule")!;
            check(gantt.Model.Items == chart.ItemsSource && chart.SelectedTask == gantt.Model.SelectedTask,
                "Control pages/Gantt: original data, selection and toolbar context remain together");
            var details = gallery.GetVisualDescendants().OfType<ControlDetailsView>().First(d => d.ControlName == "GanttChart");
            check(ReferenceEquals(details.Target, chart), "Control pages/Gantt: property editors target the real chart without reparenting it");
            var previousHeight = chart.Bounds.Height;
            shell.Height = 1200; Flush();
            check(chart.Bounds.Height > previousHeight, "Control pages/Gantt: virtual viewport adapts to window height instead of a fixed 300/520px sample");
            foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
            foreach (var size in Enum.GetValues<ControlSize>())
            foreach (var width in new[] { 1280, 720 })
            {
                shell.RequestedThemeVariant = theme; ControlSizing.SetSize(shell, size); shell.Width = width; shell.Height = 780; Flush();
                var label = $"Control pages/Gantt/{theme}/{size}/{width}";
                var bottom = chart.TranslatePoint(new Point(0, chart.Bounds.Height), gantt)!.Value.Y;
                check(bottom <= gantt.Bounds.Height + 1 && gantt.Bounds.Height <= details.TranslatePoint(default, gantt)!.Value.Y + 1,
                    label + ": full demo owns its height and the following sections do not overlap it");
                details.PropertySection.IsExpanded = true; Flush();
                check(details.Bounds.Height >= details.DesiredSize.Height - 1, label + ": expanding properties grows the page naturally");
                details.PropertySection.IsExpanded = false; outer.Offset = default; Flush();
                using var pixels = shell.CaptureRenderedFrame()!;
                check(PreviewRenderedPixels.UniqueColors(pixels) > 64, label + ": actual themed chart and text pixels are rendered");
                if (output is not null) { Directory.CreateDirectory(output); pixels.Save(Path.Combine(output, $"gantt-{theme}-{size}-{width}.png"), global::Avalonia.Media.Imaging.PngBitmapEncoderOptions.Default); }
            }
            shell.NavigateToCategory(323); Flush();
            var multiView = (MultiSelectBoxDemoView)gallery.CurrentView!;
            var multi = multiView.Sample;
            var multiDetails = gallery.GetVisualDescendants().OfType<ControlDetailsView>().Single(d => d.ControlName == "MultiSelectBox");
            multiDetails.PropertySection.IsExpanded = true;
            multiDetails.PropertyPicker.SelectedItem = multiDetails.PropertyPicker.ItemsSource!.Cast<PreviewProperty>().Single(p => p.Owner.Name == "MultiSelectBox" && p.Contract.Name == "MaxVisibleTags");
            Flush(); multiDetails.BringIntoView(); Flush();
            var editor = Part<TextBox>(multiDetails, "PropertyEditor"); editor.BringIntoView(); Flush(); editor.Focus(); editor.SelectAll(); shell.KeyTextInput("2");
            var apply = Part<Button>(multiDetails, "ApplyProperty"); apply.BringIntoView(); Flush(); Click(apply);
            check(multi.MaxVisibleTags == 2 && ReferenceEquals(multi.SelectedItems, multiView.Model.Selected),
                "Control pages/property: real keyboard/pointer editing updates the original demo and retains selection binding");
            check(!multiDetails.ApplyValue("-1") && multi.MaxVisibleTags == 2, "Control pages/property: invalid values do not corrupt the demo");
            var before = multiDetails.EventCount; multi.ToggleItem(multi.ItemsSource!.Cast<object>().Last()); Flush();
            check(multiDetails.EventCount > before, "Control pages/events: original demo interactions feed its own event section");
            shell.NavigateToCategory(25); Flush();
            var menuView = (NavMenuDemoView)gallery.CurrentView!;
            var menuDetails = gallery.GetVisualDescendants().OfType<ControlDetailsView>().Single(d => d.ControlName == "NavMenu");
            var menu = (NavMenu)menuDetails.Target;
            menuDetails.PropertyPicker.SelectedItem = menuDetails.PropertyPicker.ItemsSource!.Cast<PreviewProperty>().Single(p => p.Owner.Name == "NavMenu" && p.Contract.Name == "IsCollapsed");
            check(menuDetails.ApplyValue("true") && menuView.Model.IsCollapsed, "Control pages/binding: typed property edits update the original two-way model");
            menuView.Model.IsCollapsed = false; Flush();
            check(!menu.IsCollapsed, "Control pages/binding: later model updates still reach the original control after editing");
            shell.NavigateToCategory(315); Flush();
            check(outer.VerticalScrollBarVisibility == global::Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled,
                "Control pages/editor: workflow editor retains a finite native viewport");
            var editorPage = gallery.GetVisualDescendants().OfType<ControlPreviewPage>().Single();
            var tabs = (TabControl)editorPage.Content!;
            var editorHeight = gallery.CurrentView!.Bounds.Height;
            shell.Height += 200; Flush();
            check(gallery.CurrentView.Bounds.Height > editorHeight, "Control pages/editor: workspace grows with its window");
            tabs.SelectedIndex = 1; Flush();
            check(gallery.GetVisualDescendants().OfType<ControlDetailsView>().Any(), "Control pages/editor: documentation has its own scrollable tab without covering the canvas");
            tabs.SelectedIndex = 0; Flush();
            check(ReferenceEquals(gallery.CurrentView, editorPage.Demo), "Control pages/editor: switching tabs preserves the complete original editor instance");
            foreach (var id in new[] { 22, 326 })
            {
                shell.NavigateToCategory(id); Flush();
                var demo = (WindowControlDemoView)gallery.CurrentView!;
                Click(Part<Button>(demo, "OpenOwnedWindow"));
                var first = demo.Sample;
                check(first.IsVisible && ReferenceEquals(first.Owner, shell), $"Control pages/window/{id}: real open button creates an owned demo window");
                first.Close(); Flush(); Click(Part<Button>(demo, "OpenOwnedWindow"));
                var windowDetails = gallery.GetVisualDescendants().OfType<ControlDetailsView>().Single();
                check(demo.Sample.IsVisible && !ReferenceEquals(first, demo.Sample) && ReferenceEquals(windowDetails.Target, demo.Sample),
                    $"Control pages/window/{id}: reopening reconnects property editors to the new window");
                shell.NavigateToCategory(0); Flush();
                check(!demo.Sample.IsVisible, $"Control pages/window/{id}: leaving the page closes its owned window");
            }
        }
        finally { shell.Close(); Flush(); }
    }
    private static T Part<T>(Control root, string name) where T : Control => root.GetVisualDescendants().Prepend(root).OfType<T>().Single(c => c.Name == name);
    private static void Click(Control control)
    {
        var root = TopLevel.GetTopLevel(control)!;
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), root)!.Value;
        root.MouseMove(point); root.MouseDown(point, MouseButton.Left); root.MouseUp(point, MouseButton.Left); Flush();
    }
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
}
