using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Threading;
using global::Avalonia.VisualTree;
using TheOne.Avalonia.Reporting;

namespace TheOne.Avalonia.Controls;

public partial class ReportDesign
{
    private sealed record DataTreeViewState(ReportDataNode[] Roots, (ReportDataNode Node, bool Expanded)[] Nodes, object? Selection, Vector Offset);
    private DataTreeViewState? _dataTreeViewState;
    private int _dataTreeRestoreVersion;
    private bool _restoringDataTree;

    private void InitializeDataTreeState()
    {
        DataTree.DetachedFromVisualTree += (_, _) => { if (State.IsDataTab) CaptureDataTreeState(); };
        DataTree.Loaded += (_, _) => RestoreDataTreeState();
    }
    private void ResetDataTreeState()
    {
        ++_dataTreeRestoreVersion; _restoringDataTree = false; _dataTreeViewState = null;
    }
    private void CaptureDataTreeState()
    {
        if (_restoringDataTree || DataTree.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault() is not { } scroll) return;
        IEnumerable<ReportDataNode> All(ReportDataNode node) => new[] { node }.Concat(node.Children.SelectMany(All));
        var roots = State.DataNodes.ToArray();
        _dataTreeViewState = new(roots, roots.SelectMany(All).Select(node => (node, node.IsExpanded)).ToArray(), DataTree.SelectedItem, scroll.Offset);
    }
    private void RestoreDataTreeState()
    {
        if (_dataTreeViewState is not { } saved || !saved.Roots.SequenceEqual(State.DataNodes)) return;
        var version = ++_dataTreeRestoreVersion;
        _restoringDataTree = true;
        foreach (var (node, expanded) in saved.Nodes) node.IsExpanded = expanded;
        DataTree.SelectedItem = saved.Selection;
        // Selection and expanded containers can bring themselves into view. Apply the
        // saved offset after their layout, with a second pass for native focus scrolling.
        void RestoreOffset()
        {
            if (version != _dataTreeRestoreVersion) return;
            if (DataTree.IsAttachedToVisualTree() && saved.Roots.SequenceEqual(State.DataNodes)
                && DataTree.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault() is { } scroll)
                scroll.Offset = saved.Offset;
        }
        Dispatcher.UIThread.Post(() =>
        {
            RestoreOffset();
            Dispatcher.UIThread.Post(() => { RestoreOffset(); if (version == _dataTreeRestoreVersion) _restoringDataTree = false; }, DispatcherPriority.Background);
        }, DispatcherPriority.Loaded);
    }
}
