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

internal static class ControlExplorerPreview
{
    internal static void Verify(Action<bool, string> check, string? output = null)
    {
        var data = ContractCatalog.Current;
        check(data.Types.Select(t => t.Id).Distinct().Count() == data.Types.Length && data.Types.Length >= 216,
            "Explorer/catalog: all exported public types have unique identities");
        check(data.Types.SelectMany(t => t.Properties).All(p => p.Summary.Length > 0 && p.Default.Length > 0),
            "Explorer/catalog: all properties include descriptions and source defaults");
        check(ContractCatalog.Models(ContractCatalog.Owners("WorkflowEditor")).Any(t => t.Name == "WorkflowDocument"),
            "Explorer/models: editor APIs resolve their built-in document model");
        foreach (var (control, model) in new[] { ("RelationGraph", "RelationGraphNode"), ("FlowStep", "FlowNodeContent"), ("VerificationStep", "StepItem"), ("Upload", "UploadModel") })
            check(ContractCatalog.Models(ContractCatalog.Owners(control)).Any(t => t.Name == model),
                $"Explorer/models/{control}: object-backed content exposes its built-in {model} model");
        var explorer = new ControlExplorerView();
        var window = new Window { Width = 1200, Height = 1150, Content = explorer };
        Motion.SetEnabled(window, false); window.Show(); Flush();
        try
        {
            foreach (var choice in ExplorerSamples.Choices)
            {
                explorer.SelectControl(choice.Id); Flush();
                check(explorer.SampleTarget is not null && explorer.Bounds.Width > 0,
                    $"Explorer/sample/{choice.Name}: categorized sample creates and renders");
                if (choice.Name is not ("Toast" or "Dialog" or "MessageBox" or "OneNotification" or "ToolTip" or "ContextMenu" or "GridSplitter"))
                    check(explorer.SampleTarget!.GetType().Name == (choice.Name == "DataGridTree" ? "DataGrid" : choice.Name),
                        $"Explorer/target/{choice.Name}: property edits and styles target the actual control");
            }
            explorer.SelectControl(323); Flush();
            var multi = (MultiSelectBox)explorer.SampleTarget!;
            SelectProperty(explorer, "MultiSelectBox", "MaxVisibleTags");
            SelectTab(explorer, 1); Flush();
            var editor = Part<TextBox>(explorer, "PropertyEditor");
            editor.Focus(); editor.SelectAll(); window.KeyTextInput("2"); Flush();
            Click(Part<Button>(explorer, "ApplyProperty"));
            check(multi.MaxVisibleTags == 2, "Explorer/property: actual keyboard input and pointer apply update the typed control property");
            Click(Part<Button>(explorer, "RestoreProperty"));
            check(multi.MaxVisibleTags != 2, "Explorer/property: restore returns to the sample's original value");
            var restored = multi.MaxVisibleTags;
            check(!explorer.ApplyValue("-1") && multi.MaxVisibleTags == restored,
                "Explorer/property: rejected values report validation without corrupting the sample");
            SelectProperty(explorer, "MultiSelectBox", "IsReadOnly");
            check(explorer.ApplyValue("true") && multi.IsReadOnly, "Explorer/property: boolean edits apply through compiled accessors");
            explorer.ApplyValue("false");
            SelectProperty(explorer, "ControlSizing", "Size");
            check(explorer.ApplyValue("Small") && ControlSizing.GetSize(multi) == ControlSize.Small,
                "Explorer/property: inherited attached enum properties use typed editors");
            multi.ClearSelection(); Flush();
            check(explorer.EventLog.Any(e => e.Contains("SelectionChanged")), "Explorer/events: real selection changes appear in the event log");
            explorer.SelectControl(1); Flush();
            check(explorer.VariantCount >= 10, "Explorer/styles: Button exposes separate real samples for every public class");
            foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
            foreach (var size in Enum.GetValues<ControlSize>())
            foreach (var width in new[] { 1200, 360 })
            {
                window.RequestedThemeVariant = theme; window.Width = width; ControlSizing.SetSize(window, size); Flush();
                check(explorer.Bounds.Width <= width && explorer.Bounds.Height <= window.ClientSize.Height,
                    $"Explorer/layout/{theme}/{size}/{width}: categorized page stays inside a finite viewport");
                using var pixels = window.CaptureRenderedFrame()!;
                check(PreviewRenderedPixels.UniqueColors(pixels) > 64, $"Explorer/pixels/{theme}/{size}/{width}: native controls and text are drawn");
                if (output is not null)
                {
                    Directory.CreateDirectory(output);
                    pixels.Save(Path.Combine(output, $"explorer-{theme}-{size}-{width}.png"), global::Avalonia.Media.Imaging.PngBitmapEncoderOptions.Default);
                    SelectTab(explorer, 0); Flush();
                    using var styles = window.CaptureRenderedFrame()!;
                    styles.Save(Path.Combine(output, $"styles-{theme}-{size}-{width}.png"), global::Avalonia.Media.Imaging.PngBitmapEncoderOptions.Default);
                    SelectTab(explorer, 1); Flush();
                }
            }
            var logBefore = explorer.EventLog.ToArray();
            var oldChanged = multi.ToggleItem(multi.ItemsSource!.Cast<object>().Last()); Flush();
            check(oldChanged && explorer.EventLog.SequenceEqual(logBefore), "Explorer/lifecycle: switching controls detaches old sample event subscriptions");
        }
        finally { window.Close(); Flush(); }
        var shell = new MainWindow { Width = 1200, Height = 1000 }; Motion.SetEnabled(shell, false); shell.Show(); Flush();
        try
        {
            var gallery = shell.FindControl<ControlGallery>("Gallery")!;
            Click(Part<Button>(gallery, "ExplorerButton"));
            check(shell.Model.CurrentPage?.Id == 500 && gallery.CurrentView is ControlExplorerView,
                "Explorer/navigation: shared page action opens the categorized laboratory");
            check(Part<ScrollViewer>(gallery, "PageScroll").VerticalScrollBarVisibility == global::Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled,
                "Explorer/navigation: shell does not add an unbounded outer scroll viewport");
            shell.Model.Search = "控件能力";
            check(shell.Model.SearchResults.Any(p => p.Id == 500), "Explorer/navigation: laboratory is discoverable from global search");
            shell.NavigateToCategory(0); Flush(); shell.NavigateToCategory(500); Flush();
            check(((ControlExplorerView)gallery.CurrentView!).SampleTarget is not null,
                "Explorer/lifecycle: cached page recreates its disposed sample on return");
        }
        finally { shell.Close(); Flush(); }
    }
    private static void SelectProperty(ControlExplorerView explorer, string owner, string name)
        => explorer.PropertyPicker.SelectedItem = explorer.PropertyPicker.ItemsSource!.Cast<ExplorerProperty>().Single(p => p.Owner.Name == owner && p.Contract.Name == name);
    private static void SelectTab(ControlExplorerView explorer, int index) => Part<TabControl>(explorer, "ReferenceTabs").SelectedIndex = index;
    private static T Part<T>(Control root, string name) where T : Control => root.GetVisualDescendants().Prepend(root).OfType<T>().Single(c => c.Name == name);
    private static void Click(Control control)
    {
        var root = TopLevel.GetTopLevel(control)!;
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), root)!.Value;
        root.MouseMove(point); root.MouseDown(point, MouseButton.Left); root.MouseUp(point, MouseButton.Left); Flush();
    }
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
}
