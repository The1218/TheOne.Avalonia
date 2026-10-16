using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;
using TheOne.DesignPreview.Reference;

namespace TheOne.DesignPreview;

internal static class ControlPagesPreview
{
    internal static void Verify(Action<bool, string> check, string? output = null)
    {
        check(PreviewPageCatalog.Pages.All(p => p.Id != 500 && !p.Title.Contains("实验室")) && PreviewPageCatalog.SearchEntries.All(p => p.Id != 500),
            "Control pages/catalog: removed laboratory has no navigation or search entry");
        check(PreviewPageCatalog.Pages.Where(p => p.ControlIds is not null).Select(p => p.Id).SequenceEqual(new[] { 400, 401, 402 }),
            "Control pages/catalog: only Button, Input and Select use combined pages");
        foreach (var pair in new[] { ("Toast", "OneToast"), ("Dialog", "OneDialog"), ("MessageBox", "OneMessageBox") })
            check(ContractCatalog.Owners(pair.Item1).Any(owner => owner.Name == pair.Item2 && owner.Methods.Length > 0)
                && ContractCatalog.Owners(pair.Item1).All(owner => owner.Name != "ControlSizing"),
                $"Control pages/services/{pair.Item1}: page aliases show the real static API instead of control-only properties");
        check(PreviewNativeContracts.Properties("TreeView").All(row => !row.Name.Contains("SelectedIndex")),
            "Control pages/native contracts: TreeView documents item selection without a nonexistent index property");
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
                var descriptions = gallery.GetVisualDescendants().OfType<ControlDetailsView>().ToArray();
                foreach (var description in descriptions)
                {
                    check(description.PropertyTable.ItemCount > 0 && description.ApiTable.ItemCount > 0,
                        $"Control pages/Description/{description.ControlName}: public properties and API are visible tables");
                    check(!description.GetVisualDescendants().OfType<Expander>().Any() && !description.GetVisualDescendants().OfType<ComboBox>().Any(),
                        $"Control pages/Description/{description.ControlName}: no collapsed inspector or property/model picker hides the contracts");
                }
            }
            shell.NavigateToCategory(309); Flush();
            var gantt = (GanttDemoView)gallery.CurrentView!;
            var chart = gantt.FindControl<GanttChart>("Schedule")!;
            check(gantt.Model.Items == chart.ItemsSource && chart.SelectedTask == gantt.Model.SelectedTask,
                "Control pages/Gantt: original data, selection and toolbar context remain together");
            var details = gallery.GetVisualDescendants().OfType<ControlDetailsView>().First(d => d.ControlName == "GanttChart");
            check(ReferenceEquals(details.Target, chart), "Control pages/Gantt: descriptions observe the real chart without reparenting it");
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
                check(details.Bounds.Height >= details.DesiredSize.Height - 1 && details.PropertyTable.Bounds.Height > 0,
                    label + ": visible descriptions grow the page naturally");
                outer.Offset = default; Flush();
                using var pixels = shell.CaptureRenderedFrame()!;
                check(PreviewRenderedPixels.UniqueColors(pixels) > 64, label + ": actual themed chart and text pixels are rendered");
                if (output is not null) { Directory.CreateDirectory(output); pixels.Save(Path.Combine(output, $"gantt-{theme}-{size}-{width}.png"), global::Avalonia.Media.Imaging.PngBitmapEncoderOptions.Default); }
            }
            shell.NavigateToCategory(323); Flush();
            var multiView = (MultiSelectBoxDemoView)gallery.CurrentView!;
            var multi = multiView.Sample;
            var multiDetails = gallery.GetVisualDescendants().OfType<ControlDetailsView>().Single(d => d.ControlName == "MultiSelectBox");
            check(ReferenceEquals(multi.SelectedItems, multiView.Model.Selected), "Control pages/property: original selection binding is retained");
            check(multiDetails.PropertyTable.Items.OfType<DescriptionItem>().Any(row => Equals(row.Header, "MultiSelectBox.MaxVisibleTags")),
                "Control pages/property: MaxVisibleTags is directly listed without choosing a property");
            multi.ToggleItem(multi.ItemsSource!.Cast<object>().Last()); Flush();
            check(multiDetails.EventCount > 0, "Control pages/events: original demo interactions feed its visible event section");
            shell.NavigateToCategory(25); Flush();
            var menuView = (NavMenuDemoView)gallery.CurrentView!;
            var menuDetails = gallery.GetVisualDescendants().OfType<ControlDetailsView>().Single(d => d.ControlName == "NavMenu");
            var menu = (NavMenu)menuDetails.Target;
            menuView.Model.IsCollapsed = true; Flush();
            check(menu.IsCollapsed, "Control pages/binding: descriptions preserve the original two-way model");
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
                    $"Control pages/window/{id}: reopening reconnects descriptions to the new window");
                shell.NavigateToCategory(0); Flush();
                check(!demo.Sample.IsVisible, $"Control pages/window/{id}: leaving the page closes its owned window");
            }
            shell.NavigateToCategory(400); shell.Width = 1280; shell.Height = 900; Flush();
            var buttonExamples = gallery.GetVisualDescendants().OfType<StackPanel>().Single(p => p.Name == "StyleShowcase_Button");
            var buttons = buttonExamples.GetVisualDescendants().OfType<Button>().ToArray();
            check(ControlStyleShowcase.PublicStyles("Button").All(style => buttons.Any(button => button.Classes.Contains(style.Name))),
                "Control pages/styles: every public Button style has a simultaneous real example");
            check(PreviewSourcePanel.ReadSources(((ShowcasePage)gallery.CurrentView!).SourceViews, true).Any(source => source.Name.EndsWith("ControlStyleShowcase.cs", StringComparison.Ordinal)),
                "Control pages/source: labelled style examples include their matching embedded C# implementation");
            check(buttons.Any(button => !button.IsEnabled) && Enum.GetValues<ControlSize>().All(size => buttons.Any(button => ControlSizing.GetSize(button) == size)),
                "Control pages/styles: disabled state and three explicit sizes are visible together");
            var toggleExamples = gallery.GetVisualDescendants().OfType<StackPanel>().Single(p => p.Name == "StyleShowcase_ToggleButton");
            var toggle = toggleExamples.GetVisualDescendants().OfType<global::Avalonia.Controls.Primitives.ToggleButton>().First(button => button.IsEnabled && button.IsChecked == true);
            toggle.BringIntoView(); Flush(); Click(toggle);
            check(toggle.IsChecked == false, "Control pages/styles: labelled examples retain native pointer interaction");
            check(ControlStyleShowcase.PublicStyles("SplitButton").All(style => style.Target != "Button"),
                "Control pages/styles: split controls do not advertise Button-only classes");
            foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
            foreach (var width in new[] { 1280, 720 })
            {
                shell.RequestedThemeVariant = theme; shell.Width = width; outer.Offset = default; Flush();
                using var frame = shell.CaptureRenderedFrame()!;
                check(PreviewRenderedPixels.UniqueColors(frame) > 64, $"Control pages/styles/{theme}/{width}: real labelled samples draw pixels");
                if (output is not null) frame.Save(Path.Combine(output, $"buttons-{theme}-{width}.png"), global::Avalonia.Media.Imaging.PngBitmapEncoderOptions.Default);
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
