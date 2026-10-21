using Avalonia;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>A cached hierarchical annular chart with exact sector hit testing.</summary>
public sealed class SunburstChart : HierarchyChartBase
{
    public static readonly StyledProperty<double> RingGapProperty =
        AvaloniaProperty.Register<SunburstChart, double>(nameof(RingGap), 2,
            validate: value => double.IsFinite(value) && value >= 0);
    public static readonly StyledProperty<double> StartAngleProperty =
        AvaloniaProperty.Register<SunburstChart, double>(nameof(StartAngle), -90, validate: double.IsFinite);

    private readonly List<NodeSector> _sectors = [];
    private readonly List<SectorLabel> _labels = [];
    private readonly Dictionary<int, NodeSector> _lookup = [];
    private Pen? _selectionPen;
    private Point _center;

    static SunburstChart() => InnerRadiusProperty.OverrideDefaultValue<SunburstChart>(.18);
    public override ChartKind Kind => ChartKind.Sunburst;
    /// <summary>Separation between rings and adjacent sectors, in logical pixels.</summary>
    public double RingGap { get => GetValue(RingGapProperty); set => SetValue(RingGapProperty, value); }
    /// <summary>Clockwise start angle in degrees. -90 starts at the top.</summary>
    public double StartAngle { get => GetValue(StartAngleProperty); set => SetValue(StartAngleProperty, value); }
    private protected override bool UseDefaultMarkHitTesting => false;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == RingGapProperty || change.Property == StartAngleProperty) InvalidateChartGeometry();
    }

    private protected override void PrepareSpecializedGeometry(Rect plot)
    {
        _sectors.Clear();
        _labels.Clear();
        _lookup.Clear();
        using var labelPreparation = PrepareHierarchyLabels();
        _selectionPen = new Pen(Foreground ?? GetChartBrush(0), SelectionThickness);
        _center = plot.Center;
        var outer = Math.Max(0, Math.Min(plot.Width, plot.Height) / 2 - SelectionThickness - 2);
        if (outer <= 0) return;
        var inner = outer * InnerRadius;
        var maximumLevel = 0;
        var pending = new Stack<(HierarchyNodeState Node, int Level)>();
        foreach (var root in DisplayRoots)
            if (double.IsFinite(root.LogWeight)) pending.Push((root, 0));
        while (pending.Count > 0)
        {
            var (node, level) = pending.Pop();
            maximumLevel = Math.Max(maximumLevel, level);
            foreach (var child in node.Children)
                if (double.IsFinite(child.LogWeight)) pending.Push((child, level + 1));
        }
        var ringWidth = (outer - inner) / (maximumLevel + 1);
        if (!double.IsFinite(ringWidth) || ringWidth <= 0) return;
        var groups = new Stack<SectorGroup>();
        groups.Push(new SectorGroup(DisplayRoots, 0, StartAngle % 360 * Math.PI / 180, Math.Tau));
        while (groups.Count > 0)
        {
            var group = groups.Pop();
            var total = TotalLogWeight(group.Nodes);
            if (!double.IsFinite(total)) continue;
            var positive = group.Nodes.Where(state => double.IsFinite(state.LogWeight)).ToArray();
            var start = group.Start;
            var end = group.Start + group.Sweep;
            for (var index = 0; index < positive.Length; index++)
            {
                var state = positive[index];
                var sweep = index == positive.Length - 1 ? end - start : group.Sweep * Math.Exp(state.LogWeight - total);
                sweep = Math.Clamp(sweep, 0, Math.Max(0, end - start));
                var originalStart = start;
                start += sweep;
                if (sweep <= 0) continue;
                var radialGap = Math.Min(RingGap / 2, ringWidth * .4);
                var near = inner + group.Level * ringWidth + radialGap;
                var far = inner + (group.Level + 1) * ringWidth - radialGap;
                var angularGap = sweep >= Math.Tau - 1e-12 ? 0 : Math.Min(RingGap / (2 * far), sweep * .2);
                var paintedStart = originalStart + angularGap;
                var paintedSweep = sweep - angularGap * 2;
                if (far <= near || paintedSweep <= 0) continue;
                var geometry = AnnularSector(_center, far, near, paintedStart, paintedSweep);
                var sector = new NodeSector(state.Index, geometry, HierarchyBrush(state), near, far, paintedStart, paintedSweep);
                _sectors.Add(sector);
                _lookup[state.Index] = sector;
                var anchor = RadialPoint(_center, (near + far) / 2, paintedStart + paintedSweep / 2);
                RegisterMark(0, state.Index, anchor, new Rect(_center - new Vector(far, far), new Size(far * 2, far * 2)));
                if (ShowDataLabels && !string.IsNullOrEmpty(state.Node.Label))
                {
                    var text = HierarchyLabel(state, sector.Brush);
                    var bounds = new Rect(anchor - new Vector(text.Width / 2, text.Height / 2), new Size(text.Width, text.Height));
                    if (Contains(sector, bounds.TopLeft) && Contains(sector, bounds.TopRight) &&
                        Contains(sector, bounds.BottomLeft) && Contains(sector, bounds.BottomRight))
                        _labels.Add(new SectorLabel(text, bounds.TopLeft));
                }
                if (state.Children.Count > 0)
                    groups.Push(new SectorGroup(state.Children, group.Level + 1, originalStart, sweep));
            }
        }
    }

    private protected override void DrawChartPlot(DrawingContext context, Rect plot)
    {
        using var clip = context.PushClip(plot);
        foreach (var sector in _sectors) context.DrawGeometry(sector.Brush, null, sector.Geometry);
        foreach (var label in _labels) context.DrawText(label.Text, label.Position);
        if (Selection is { SeriesIndex: 0 } selected && _lookup.TryGetValue(selected.PointIndex, out var active))
            context.DrawGeometry(null, _selectionPen, active.Geometry);
    }

    private protected override (int Series, int Index)? HitTestMark(Point point)
    {
        for (var index = _sectors.Count - 1; index >= 0; index--)
            if (Contains(_sectors[index], point)) return (0, _sectors[index].Index);
        return null;
    }

    private bool Contains(NodeSector sector, Point point)
    {
        var delta = point - _center;
        var distance = delta.X * delta.X + delta.Y * delta.Y;
        if (distance < sector.Inner * sector.Inner || distance > sector.Outer * sector.Outer) return false;
        var angle = (Math.Atan2(delta.Y, delta.X) - sector.Start) % Math.Tau;
        if (angle < 0) angle += Math.Tau;
        return angle <= sector.Sweep;
    }

    private protected override ChartBase CreateExportSnapshot() => new SunburstChart();
    private protected override void CopyExportProperties(ChartBase snapshot)
    {
        base.CopyExportProperties(snapshot);
        var target = (SunburstChart)snapshot;
        target.RingGap = RingGap;
        target.StartAngle = StartAngle;
    }

    private readonly record struct SectorGroup(IReadOnlyList<HierarchyNodeState> Nodes, int Level, double Start, double Sweep);
    private sealed record NodeSector(int Index, StreamGeometry Geometry, IBrush Brush, double Inner, double Outer, double Start, double Sweep);
    private readonly record struct SectorLabel(FormattedText Text, Point Position);
}
