using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Reporting;

namespace TheOne.Avalonia.Controls;

internal sealed record ReportStructureNode(
    string Name,
    string Icon = "Folder",
    ReportElement? Element = null,
    ReportColumn? Column = null,
    bool Expanded = false)
{
    public ObservableCollection<ReportStructureNode> Children { get; } = [];
}

public partial class ReportDesign
{
    private ReportDocument? _structureDocument;
    private int _structureGeneration, _structureNavigationGeneration;
    private double _structureHeaderBottom = double.NaN, _structureFooterTop = double.NaN;
    private bool _updatingStructure;

    private void RefreshStructure()
    {
        var previousSelection = StructureTree.SelectedItem as ReportStructureNode;
        var expanded = new Dictionary<string, bool>();
        if (ReferenceEquals(_structureDocument, Document))
        {
            foreach (var node in State.Structure.SelectMany(StructureDescendants).Where(n => n.Children.Count > 0))
                expanded[node.Name] = node.Expanded;
            foreach (var item in StructureTree.GetVisualDescendants().OfType<TreeViewItem>())
                if (item.DataContext is ReportStructureNode { Children.Count: > 0 } node)
                    expanded[node.Name] = item.IsExpanded;
        }

        bool IsExpanded(string name, bool fallback = false)
        {
            return expanded.TryGetValue(name, out var value) ? value : fallback;
        }

        _structureDocument = Document;
        _structureHeaderBottom = Document?.HeaderBottom ?? double.NaN;
        _structureFooterTop = Document?.FooterTop ?? double.NaN;
        ++_structureGeneration;
        _updatingStructure = true;
        try
        {
            if (Document is not { } document)
            {
                State.Structure.Clear();
                StructureTree.SelectedItem = null;
                return;
            }

            var root = new ReportStructureNode("Report", Expanded: IsExpanded("Report", true));
            var title = new ReportStructureNode("报表标题", "Text",
                document.Elements.FirstOrDefault(e => e.Name == "报表标题"));
            var header = new ReportStructureNode("页眉", Expanded: IsExpanded("页眉"));
            var detail = new ReportStructureNode("明细", Expanded: IsExpanded("明细", true));
            var summary = new ReportStructureNode("报表汇总", Expanded: IsExpanded("报表汇总"));
            var footer = new ReportStructureNode("页脚", Expanded: IsExpanded("页脚"));
            foreach (var element in document.Elements)
            {
                if (ReferenceEquals(element, title.Element)) continue;
                var node = new ReportStructureNode(element.Name,
                    element.Kind == ReportElementKind.CheckBox ? "CheckBoxElement" : element.Kind.ToString(), element);
                if (element.Kind == ReportElementKind.Table)
                {
                    detail.Children.Add(node);
                    foreach (var column in document.Columns.Where(c => c.ShowSum))
                        summary.Children.Add(new ReportStructureNode(column.Header + " · 求和", "Table", element,
                            column));
                }
                else if (element.Y >= document.FooterTop)
                {
                    footer.Children.Add(node);
                }
                else if (element.Y < document.HeaderBottom)
                {
                    header.Children.Add(node);
                }
                else
                {
                    detail.Children.Add(node);
                }
            }

            foreach (var node in new[] { title, header, detail, summary, footer }) root.Children.Add(node);
            State.Structure.Clear();
            State.Structure.Add(root);
        }
        finally
        {
            _updatingStructure = false;
        }

        SynchronizeStructureSelection(previousSelection?.Column);
    }

    private static IEnumerable<ReportStructureNode> StructureDescendants(ReportStructureNode node)
    {
        return new[] { node }.Concat(node.Children.SelectMany(StructureDescendants));
    }

    private void SynchronizeStructureSelection(ReportColumn? preferredColumn = null)
    {
        if (_updatingStructure) return;
        var document = Document;
        var generation = ++_structureGeneration;
        // A changed hierarchy must be realized before TreeView can resolve a nested selection.
        Dispatcher.UIThread.Post(() =>
        {
            if (generation != _structureGeneration || !ReferenceEquals(document, Document)) return;
            var selected = State.Structure.SelectMany(StructureDescendants).FirstOrDefault(n => n.Element is not null
                && ReferenceEquals(n.Element, SelectedElement) && ReferenceEquals(n.Column, preferredColumn));
            _updatingStructure = true;
            try
            {
                StructureTree.SelectedItem = selected;
            }
            finally
            {
                _updatingStructure = false;
            }
        }, DispatcherPriority.Loaded);
    }

    private void OnStructureSelection(object? sender, SelectionChangedEventArgs e)
    {
        if (_updatingStructure) return;
        ++_structureGeneration;
        if (StructureTree.SelectedItem is ReportStructureNode node) NavigateToStructureNode(node);
    }

    private void OnStructurePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton != MouseButton.Left || e.Source is not Control source) return;
        var chain = source.GetVisualAncestors().Prepend(source)
            .TakeWhile(control => !ReferenceEquals(control, StructureTree)).ToArray();
        if (chain.OfType<ToggleButton>().Any()) return;
        if (chain.OfType<TreeViewItem>().FirstOrDefault()?.DataContext is ReportStructureNode node)
            NavigateToStructureNode(node);
    }

    private void NavigateToStructureNode(ReportStructureNode node)
    {
        if (Document is not { } document || (node.Element is { } target && !document.Elements.Contains(target))) return;
        ++_structureGeneration;
        _updatingStructure = true;
        try
        {
            ShowDesign();
            SetCurrentValue(SelectedElementProperty, node.Element);
            if (node.Element?.Kind == ReportElementKind.Table) SetCurrentValue(SelectedColumnProperty, node.Column);
        }
        finally
        {
            _updatingStructure = false;
        }

        var region = node.Element is { } element ? new Rect(element.X, element.Y, element.Width, element.Height)
            : node.Name == "页脚" ? new Rect(document.LeftMargin, document.FooterTop,
                document.PageWidthMm - document.LeftMargin - document.RightMargin, document.FooterHeightMm)
            : node.Name == "页眉" ? new Rect(document.LeftMargin, document.TopMargin,
                document.PageWidthMm - document.LeftMargin - document.RightMargin, document.HeaderHeightMm)
            : new Rect(document.LeftMargin,
                node.Name == "明细" || node.Name == "报表汇总" ? document.HeaderBottom : document.TopMargin, 20, 20);
        if (node.Element is { Kind: ReportElementKind.Table } table && node.Column is { } column)
        {
            var index = document.Columns.IndexOf(column);
            var total = document.Columns.Sum(c => c.WidthWeight);
            if (index >= 0 && total > 0)
                region = new Rect(table.X + table.Width * document.Columns.Take(index).Sum(c => c.WidthWeight) / total,
                    table.Y + (Layout?.HeaderHeight ?? 0) / ReportPaginator.DipsPerMillimetre,
                    table.Width * column.WidthWeight / total,
                    Math.Max(5, table.Height - (Layout?.HeaderHeight ?? 0) / ReportPaginator.DipsPerMillimetre));
        }

        var generation = ++_structureNavigationGeneration;
        Dispatcher.UIThread.Post(() =>
        {
            if (generation != _structureNavigationGeneration || !ReferenceEquals(document, Document) ||
                State.IsPreview) return;
            var local = PageSurface.PageRect(region);
            if (PageSurface.TranslatePoint(local.TopLeft, PageScroll) is not { } position) return;
            var current = PageScroll.Offset;

            double Reveal(double start, double length, double offset, double viewport, double extent)
            {
                var next = length > viewport - 40 || start < offset + 20 ? start - 20 :
                    start + length > offset + viewport - 20 ? start + length - viewport + 20 : offset;
                return Math.Clamp(next, 0, Math.Max(0, extent - viewport));
            }

            PageScroll.Offset = new Vector(
                Reveal(position.X + current.X, local.Width, current.X, PageScroll.Viewport.Width,
                    PageScroll.Extent.Width),
                Reveal(position.Y + current.Y, local.Height, current.Y, PageScroll.Viewport.Height,
                    PageScroll.Extent.Height));
        }, DispatcherPriority.Loaded);
    }
}