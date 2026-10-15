using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
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

internal static class ReportStructurePreview
{
    internal static void Verify(Action<bool, string> check, string? output = null)
    {
        var document = new ReportDocument { HeaderHeightMm = 35, FooterHeightMm = 18 };
        var sumColumn = new ReportColumn { Header = "金额", ShowSum = true };
        document.Columns.Add(sumColumn);
        document.Columns.Add(new() { Header = "备注" });
        var headerElement = new ReportElement { Name = "公司名称", Kind = ReportElementKind.Text, Text = "公司", X = 14, Y = 22, Width = 80, Height = 10 };
        var table = new ReportElement { Name = "明细表", Kind = ReportElementKind.Table, X = 14, Y = 60, Width = 182, Height = 24 };
        var footerElement = new ReportElement { Name = "页脚页码", Kind = ReportElementKind.PageNumber, X = 14, Y = 272, Width = 80, Height = 10 };
        document.Elements.Add(headerElement); document.Elements.Add(table); document.Elements.Add(footerElement);
        var view = new ReportDesign { Document = document };
        var window = new Window { Content = view, Width = 1586, Height = 992, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = ThemeVariant.Light };
        ControlSizing.SetSize(window, ControlSize.Normal); Motion.SetEnabled(window, false);
        window.Show(); Flush();
        try
        {
            var tree = view.FindControl<TreeView>("StructureTree")!;
            var root = Node(tree, "Report");
            foreach (var (group, child, element) in new[]
            {
                ("页眉", "公司名称", headerElement),
                ("明细", "明细表", table),
                ("页脚", "页脚页码", footerElement),
                ("报表汇总", "金额 · 求和", table)
            })
            {
                var parent = Node(tree, group); SetExpanded(window, parent, true);
                var selected = Node(tree, child); Click(window, Header(selected));
                check(parent.IsExpanded && ReferenceEquals(Node(tree, group), parent) && ReferenceEquals(Node(tree, "Report"), root),
                    $"ReportDesign/structure: clicking a child keeps {group} expanded and retains its native containers");
                check(ReferenceEquals(view.SelectedElement, element) && ReferenceEquals(tree.SelectedItem, selected.DataContext),
                    $"ReportDesign/structure: native {group} child selection updates the inspector without replacing the selected node");
            }
            check(ReferenceEquals(view.SelectedColumn, sumColumn), "ReportDesign/structure: selecting a summary child retains its exact bound column");

            view.SelectedElement = headerElement; Flush();
            check(Node(tree, "页眉").IsExpanded && Node(tree, "公司名称").IsSelected && ReferenceEquals(Node(tree, "Report"), root),
                "ReportDesign/structure: external element selection synchronizes the existing native tree without rebuilding it");

            SetExpanded(window, Node(tree, "报表汇总"), false);
            headerElement.Name = "重命名公司"; Flush();
            check(Node(tree, "页眉").IsExpanded && Node(tree, "明细").IsExpanded && Node(tree, "页脚").IsExpanded
                && !Node(tree, "报表汇总").IsExpanded && Node(tree, "重命名公司").IsSelected,
                "ReportDesign/structure: a legitimate hierarchy refresh retains both expanded and collapsed sections and its selection");
            SetExpanded(window, Node(tree, "报表汇总"), true);
            Click(window, Header(Node(tree, "页脚页码")));
            check(Node(tree, "报表汇总").IsExpanded && Node(tree, "页脚").IsExpanded && ReferenceEquals(view.SelectedElement, footerElement),
                "ReportDesign/structure: repeated native child selections leave other expanded branches open");

            SetExpanded(window, Node(tree, "Report"), false);
            footerElement.Name = "隐藏时重建一次"; Flush();
            footerElement.Name = "隐藏时重建两次"; Flush();
            check(!Node(tree, "Report").IsExpanded, "ReportDesign/structure: hierarchy refreshes keep the root explicitly collapsed");
            SetExpanded(window, Node(tree, "Report"), true);
            check(Node(tree, "页眉").IsExpanded && Node(tree, "明细").IsExpanded && Node(tree, "页脚").IsExpanded && Node(tree, "报表汇总").IsExpanded,
                "ReportDesign/structure: hidden branch expansion survives consecutive hierarchy refreshes under a collapsed root");

            var selection = tree.SelectedItem;
            for (var cycle = 0; cycle < 3; cycle++)
            {
                Click(window, view.FindControl<TabItem>("DataTabButton")!);
                var dataTree = view.FindControl<TreeView>("DataTree")!;
                check((!tree.IsAttachedToVisualTree() || !tree.IsEffectivelyVisible) && dataTree.IsAttachedToVisualTree() && dataTree.IsEffectivelyVisible,
                    "ReportDesign/sidebar: data tab displays the data tree without the report structure");
                Click(window, view.FindControl<TabItem>("ComponentsTabButton")!);
                check(tree.IsEffectivelyVisible && ReferenceEquals(tree.SelectedItem, selection),
                    "ReportDesign/sidebar: returning to components restores the structure tree and its selection");
            }

            if (output is not null)
            {
                Directory.CreateDirectory(output);
                using var frame = window.CaptureRenderedFrame()!;
                frame.Save(Path.Combine(output, "reportdesign-structure-expanded.png"), PngBitmapEncoderOptions.Default);
                foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
                foreach (var size in new[] { ControlSize.Small, ControlSize.Normal, ControlSize.Large })
                {
                    window.RequestedThemeVariant = theme; ControlSizing.SetSize(window, size);
                    window.Width = size == ControlSize.Large ? 1000 : 1586; window.Height = size == ControlSize.Large ? 780 : 992; Flush();
                    using var themed = window.CaptureRenderedFrame()!;
                    themed.Save(Path.Combine(output, $"report-components-{theme}-{size}.png"), PngBitmapEncoderOptions.Default);
                }
            }
            view.Document = null; Flush();
            check(tree.Items.Count == 0 && tree.SelectedItem is null, "ReportDesign/structure: closing the document clears its native hierarchy and selection");
        }
        finally { window.Close(); }
    }

    private static ContentPresenter Header(TreeViewItem item) => item.GetVisualDescendants().OfType<ContentPresenter>().First(part => part.Name == "PART_HeaderPresenter");
    private static TreeViewItem Node(TreeView tree, string name) => tree.GetVisualDescendants().OfType<TreeViewItem>()
        .Single(item => Header(item).GetVisualDescendants().OfType<TextBlock>().Any(text => text.Text == name));
    private static void SetExpanded(Window window, TreeViewItem item, bool expanded)
    {
        if (item.IsExpanded == expanded) return;
        Click(window, item.GetVisualDescendants().OfType<ToggleButton>().First(part => part.Name == "PART_ExpandCollapseChevron"));
    }
    private static void Click(Window window, Control control)
    {
        control.BringIntoView(); Flush();
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
        window.MouseMove(point); window.MouseDown(point, MouseButton.Left); window.MouseUp(point, MouseButton.Left); Flush();
    }
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
}
