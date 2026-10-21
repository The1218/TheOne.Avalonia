using Avalonia;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>A stable slice-and-dice treemap with nested branch headers.</summary>
public sealed class TreemapChart : HierarchyChartBase
{
    public static readonly StyledProperty<double> GapProperty =
        AvaloniaProperty.Register<TreemapChart, double>(nameof(Gap), 3,
            validate: value => double.IsFinite(value) && value >= 0);
    public static readonly StyledProperty<double> HeaderHeightProperty =
        AvaloniaProperty.Register<TreemapChart, double>(nameof(HeaderHeight), 20,
            validate: value => double.IsFinite(value) && value >= 0);

    private readonly List<Tile> _tiles = [];
    private readonly List<TileLabel> _labels = [];
    private readonly Dictionary<int, Rect> _bounds = [];
    private Pen? _selectionPen;
    private Rect _plot;

    public override ChartKind Kind => ChartKind.Treemap;
    public double Gap { get => GetValue(GapProperty); set => SetValue(GapProperty, value); }
    /// <summary>Maximum parent header height; labels and padding are reserved inside the allocated tile.</summary>
    public double HeaderHeight { get => GetValue(HeaderHeightProperty); set => SetValue(HeaderHeightProperty, value); }
    private protected override bool UseDefaultMarkHitTesting => false;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == GapProperty || change.Property == HeaderHeightProperty) InvalidateChartGeometry();
    }

    private protected override void PrepareSpecializedGeometry(Rect plot)
    {
        _tiles.Clear();
        _labels.Clear();
        _bounds.Clear();
        using var labelPreparation = PrepareHierarchyLabels();
        _plot = plot;
        _selectionPen = new Pen(Foreground ?? GetChartBrush(0), SelectionThickness);
        if (plot.Width <= 0 || plot.Height <= 0) return;
        var groups = new Stack<LayoutGroup>();
        groups.Push(new LayoutGroup(DisplayRoots, plot, plot.Width >= plot.Height));
        while (groups.Count > 0)
        {
            var group = groups.Pop();
            var total = TotalLogWeight(group.Nodes);
            if (!double.IsFinite(total)) continue;
            var positive = group.Nodes.Where(state => double.IsFinite(state.LogWeight)).ToArray();
            var offset = group.Vertical ? group.Bounds.Left : group.Bounds.Top;
            var end = group.Vertical ? group.Bounds.Right : group.Bounds.Bottom;
            for (var index = 0; index < positive.Length; index++)
            {
                var state = positive[index];
                var length = index == positive.Length - 1 ? end - offset :
                    (group.Vertical ? group.Bounds.Width : group.Bounds.Height) * Math.Exp(state.LogWeight - total);
                length = Math.Clamp(length, 0, Math.Max(0, end - offset));
                var allocated = group.Vertical
                    ? new Rect(offset, group.Bounds.Top, length, group.Bounds.Height)
                    : new Rect(group.Bounds.Left, offset, group.Bounds.Width, length);
                offset += length;
                if (allocated.Width <= 0 || allocated.Height <= 0) continue;
                var inset = Math.Min(Gap / 2, Math.Min(allocated.Width, allocated.Height) / 4);
                var bounds = allocated.Deflate(inset);
                if (bounds.Width <= 0 || bounds.Height <= 0) continue;
                var brush = HierarchyBrush(state);
                _tiles.Add(new Tile(state.Index, bounds, brush));
                _bounds[state.Index] = bounds;
                RegisterMark(0, state.Index, bounds.Center, bounds);
                var hasChildren = state.Children.Any(child => double.IsFinite(child.LogWeight));
                var header = hasChildren && ShowDataLabels ? Math.Min(HeaderHeight, bounds.Height * .25) : 0;
                if (ShowDataLabels && !string.IsNullOrEmpty(state.Node.Label))
                {
                    var labelHeight = hasChildren ? header : bounds.Height;
                    if (bounds.Width >= 8 && labelHeight >= 2)
                    {
                        var text = HierarchyLabel(state, brush);
                        if (text.Width + 8 <= bounds.Width && text.Height + 2 <= labelHeight)
                            _labels.Add(new TileLabel(text, new Point(bounds.Left + 4,
                                hasChildren ? bounds.Top + (header - text.Height) / 2 : bounds.Center.Y - text.Height / 2)));
                    }
                }
                if (!hasChildren) continue;
                var childBounds = new Rect(bounds.Left, bounds.Top + header, bounds.Width, Math.Max(0, bounds.Height - header));
                if (childBounds.Width > 0 && childBounds.Height > 0)
                    groups.Push(new LayoutGroup(state.Children, childBounds, !group.Vertical));
            }
        }
    }

    private protected override void DrawChartPlot(DrawingContext context, Rect plot)
    {
        using var clip = context.PushClip(plot);
        foreach (var tile in _tiles) context.DrawRectangle(tile.Brush, null, tile.Bounds);
        foreach (var label in _labels) context.DrawText(label.Text, label.Position);
        if (Selection is { SeriesIndex: 0 } selected && _bounds.TryGetValue(selected.PointIndex, out var bounds))
            context.DrawRectangle(null, _selectionPen, bounds.Deflate(Math.Min(SelectionThickness / 2, Math.Min(bounds.Width, bounds.Height) / 4)));
    }

    private protected override (int Series, int Index)? HitTestMark(Point point)
    {
        if (!_plot.Contains(point)) return null;
        for (var index = _tiles.Count - 1; index >= 0; index--)
            if (_tiles[index].Bounds.Contains(point)) return (0, _tiles[index].Index);
        return null;
    }

    private protected override ChartBase CreateExportSnapshot() => new TreemapChart();
    private protected override void CopyExportProperties(ChartBase snapshot)
    {
        base.CopyExportProperties(snapshot);
        var target = (TreemapChart)snapshot;
        target.Gap = Gap;
        target.HeaderHeight = HeaderHeight;
    }

    private readonly record struct LayoutGroup(IReadOnlyList<HierarchyNodeState> Nodes, Rect Bounds, bool Vertical);
    private readonly record struct Tile(int Index, Rect Bounds, IBrush Brush);
    private readonly record struct TileLabel(FormattedText Text, Point Position);
}
