using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>A radar chart whose point indexes map to independently scaled axes.</summary>
public sealed class RadarChart : SpecializedChartBase
{
    public static readonly StyledProperty<IList<RadarAxis>?> AxesProperty =
        AvaloniaProperty.Register<RadarChart, IList<RadarAxis>?>(nameof(Axes));
    public static readonly StyledProperty<int> GridLevelsProperty =
        AvaloniaProperty.Register<RadarChart, int>(nameof(GridLevels), 5, validate: value => value is >= 1 and <= 20);
    public static readonly StyledProperty<double> FillOpacityProperty =
        AvaloniaProperty.Register<RadarChart, double>(nameof(FillOpacity), .16,
            validate: value => double.IsFinite(value) && value >= 0 && value <= 1);

    private readonly List<RadarShape> _shapes = [];
    private readonly List<RadarLabel> _labels = [];
    private readonly Dictionary<(int Series, int Index), RadarMarker> _markers = [];
    private readonly HashSet<RadarAxis> _observedAxes = new(ReferenceEqualityComparer.Instance);
    private INotifyCollectionChanged? _axisCollection;
    private bool _observingAxes;
    private StreamGeometry? _grid;
    private Pen? _gridPen;
    private Pen? _selectionPen;

    public RadarChart() => Axes = new ObservableCollection<RadarAxis>();

    public override ChartKind Kind => ChartKind.Radar;
    /// <summary>At least three dimensions. Each series point at index i belongs to axis i.</summary>
    public IList<RadarAxis>? Axes { get => GetValue(AxesProperty); set => SetValue(AxesProperty, value); }
    public int GridLevels { get => GetValue(GridLevelsProperty); set => SetValue(GridLevelsProperty, value); }
    public double FillOpacity { get => GetValue(FillOpacityProperty); set => SetValue(FillOpacityProperty, value); }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _observingAxes = true;
        ReconcileAxes();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _observingAxes = false;
        ReleaseAxes();
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == AxesProperty)
        {
            ReconcileAxes();
            NotifyChartDataChanged();
        }
        else if (change.Property == GridLevelsProperty || change.Property == FillOpacityProperty)
            InvalidateChartGeometry();
    }

    private void ReconcileAxes()
    {
        if (!_observingAxes) return;
        var nextCollection = Axes as INotifyCollectionChanged;
        if (!ReferenceEquals(nextCollection, _axisCollection))
        {
            if (_axisCollection is not null) _axisCollection.CollectionChanged -= AxesChanged;
            _axisCollection = nextCollection;
            if (_axisCollection is not null) _axisCollection.CollectionChanged += AxesChanged;
        }
        var current = Axes is { } axes
            ? new HashSet<RadarAxis>(axes, ReferenceEqualityComparer.Instance)
            : new HashSet<RadarAxis>(ReferenceEqualityComparer.Instance);
        foreach (var axis in _observedAxes.ToArray())
            if (!current.Contains(axis))
            {
                axis.PropertyChanged -= AxisChanged;
                _observedAxes.Remove(axis);
            }
        foreach (var axis in current)
            if (_observedAxes.Add(axis)) axis.PropertyChanged += AxisChanged;
    }

    private protected override void OnRefreshChartData()
    {
        base.OnRefreshChartData();
        ReconcileAxes();
    }

    private void ReleaseAxes()
    {
        if (_axisCollection is not null) _axisCollection.CollectionChanged -= AxesChanged;
        _axisCollection = null;
        foreach (var axis in _observedAxes) axis.PropertyChanged -= AxisChanged;
        _observedAxes.Clear();
    }

    private void AxesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ReconcileAxes();
        NotifyChartDataChanged();
    }

    private void AxisChanged(object? sender, AvaloniaPropertyChangedEventArgs e) => NotifyChartDataChanged();

    private protected override bool IsValidSelectionPoint(int series, int index, ChartPoint point) =>
        Axes is { Count: >= 3 } axes && index >= 0 && index < axes.Count &&
        axes[index].Maximum > axes[index].Minimum && double.IsFinite(point.X) && double.IsFinite(point.Y);

    private protected override string? DescribeSpecializedPoint(ChartSelection selection)
    {
        var axis = Axes is { } axes && selection.PointIndex < axes.Count ? axes[selection.PointIndex].Name : null;
        return $"{selection.Series.Name} · {axis ?? selection.Point.Label ?? FormatValue(selection.Point.X)}: {FormatChartValue(selection.Series, selection.Point.Y)}{selection.Series.Unit ?? Unit}";
    }

    private protected override void PrepareSpecializedGeometry(Rect plot)
    {
        _shapes.Clear();
        _labels.Clear();
        _markers.Clear();
        _grid = null;
        _gridPen = new Pen(GridBrush ?? MutedBrush ?? GetChartBrush(0), 1);
        _selectionPen = new Pen(Foreground ?? GetChartBrush(0), SelectionThickness);
        if (Axes is not { Count: >= 3 } axes || SeriesSource is not { } source || plot.Width <= 0 || plot.Height <= 0) return;
        var texts = new FormattedText[axes.Count];
        var maximumWidth = 0d;
        var maximumHeight = 0d;
        for (var index = 0; index < axes.Count; index++)
        {
            texts[index] = Text(axes[index].Name, MutedBrush);
            maximumWidth = Math.Max(maximumWidth, texts[index].Width);
            maximumHeight = Math.Max(maximumHeight, texts[index].Height);
        }
        var center = plot.Center;
        var radius = Math.Max(0, Math.Min(plot.Width / 2 - maximumWidth - 12, plot.Height / 2 - maximumHeight - 12));
        if (radius < 2) return;
        var directions = new Vector[axes.Count];
        _grid = new StreamGeometry();
        using (var path = _grid.Open())
        {
            for (var index = 0; index < axes.Count; index++)
            {
                var angle = -Math.PI / 2 + Math.Tau * index / axes.Count;
                var direction = new Vector(Math.Cos(angle), Math.Sin(angle));
                directions[index] = direction;
                path.BeginFigure(center, false);
                path.LineTo(center + direction * radius);
                path.EndFigure(false);
                var text = texts[index];
                var anchor = center + direction * (radius + 8);
                var x = anchor.X - (direction.X > .15 ? 0 : direction.X < -.15 ? text.Width : text.Width / 2);
                var y = anchor.Y - (direction.Y > .15 ? 0 : direction.Y < -.15 ? text.Height : text.Height / 2);
                _labels.Add(new RadarLabel(text, new Point(x, y)));
            }
            for (var level = 1; level <= GridLevels; level++)
            {
                var distance = radius * level / GridLevels;
                path.BeginFigure(center + directions[0] * distance, false);
                for (var index = 1; index < axes.Count; index++) path.LineTo(center + directions[index] * distance);
                path.EndFigure(true);
            }
        }
        for (var seriesIndex = 0; seriesIndex < source.Count; seriesIndex++)
        {
            var series = source[seriesIndex];
            if (!series.IsVisible || series.ItemsSource is not { } points) continue;
            var positions = new Point?[axes.Count];
            var brush = GetChartBrush(seriesIndex);
            var complete = true;
            for (var index = 0; index < axes.Count; index++)
            {
                if (index >= points.Count || !double.IsFinite(points[index].Y) || !double.IsFinite(points[index].X) ||
                    axes[index].Maximum <= axes[index].Minimum)
                {
                    complete = false;
                    continue;
                }
                var value = points[index].Y;
                var minimum = axes[index].Minimum;
                var maximum = axes[index].Maximum;
                var span = maximum - minimum;
                var fraction = Math.Clamp(double.IsFinite(span) ? (value - minimum) / span :
                    (value / 2 - minimum / 2) / (maximum / 2 - minimum / 2), 0, 1);
                var position = center + directions[index] * (radius * fraction);
                positions[index] = position;
                _markers[(seriesIndex, index)] = new RadarMarker(position, brush);
                var hitRadius = Math.Max(6, MarkerRadius + SelectionThickness);
                RegisterMark(seriesIndex, index, position, new Rect(position - new Vector(hitRadius, hitRadius),
                    new Size(hitRadius * 2, hitRadius * 2)));
                if (ShowDataLabels)
                {
                    var label = Text(FormatChartValue(series, value), brush);
                    _labels.Add(new RadarLabel(label, position + new Vector(4, -label.Height - 2)));
                }
            }
            var geometry = new StreamGeometry();
            using (var path = geometry.Open())
            {
                if (complete)
                {
                    path.BeginFigure(positions[0]!.Value, true);
                    for (var index = 1; index < positions.Length; index++) path.LineTo(positions[index]!.Value);
                    path.EndFigure(true);
                }
                else
                {
                    // Draw adjacent valid dimensions only; do not bridge a missing dimension.
                    for (var index = 0; index < positions.Length; index++)
                    {
                        if (positions[index] is not { } start || positions[(index + 1) % positions.Length] is not { } end) continue;
                        path.BeginFigure(start, false);
                        path.LineTo(end);
                        path.EndFigure(false);
                    }
                }
            }
            _shapes.Add(new RadarShape(geometry, brush, new Pen(brush, StrokeThickness), complete));
        }
    }

    private protected override void DrawChartPlot(DrawingContext context, Rect plot)
    {
        if (ShowGrid && _grid is not null) context.DrawGeometry(null, _gridPen, _grid);
        foreach (var shape in _shapes)
        {
            if (shape.IsClosed && FillOpacity > 0)
            {
                using var opacity = context.PushOpacity(FillOpacity);
                context.DrawGeometry(shape.Brush, null, shape.Geometry);
            }
            context.DrawGeometry(null, shape.Pen, shape.Geometry);
        }
        if (ShowMarkers)
            foreach (var marker in _markers.Values)
                context.DrawEllipse(marker.Brush, null, marker.Position, MarkerRadius, MarkerRadius);
        foreach (var label in _labels) context.DrawText(label.Text, label.Position);
        if (Selection is { } selected && _markers.TryGetValue((selected.SeriesIndex, selected.PointIndex), out var active))
            context.DrawEllipse(null, _selectionPen, active.Position, MarkerRadius + SelectionThickness, MarkerRadius + SelectionThickness);
    }

    private protected override ChartBase CreateExportSnapshot() => new RadarChart();
    private protected override void CopyExportProperties(ChartBase snapshot)
    {
        base.CopyExportProperties(snapshot);
        var target = (RadarChart)snapshot;
        target.Axes = Axes?.Select(axis => new RadarAxis { Name = axis.Name, Minimum = axis.Minimum, Maximum = axis.Maximum }).ToArray();
        target.GridLevels = GridLevels;
        target.FillOpacity = FillOpacity;
    }

    private sealed record RadarShape(StreamGeometry Geometry, IBrush Brush, Pen Pen, bool IsClosed);
    private readonly record struct RadarMarker(Point Position, IBrush Brush);
    private readonly record struct RadarLabel(FormattedText Text, Point Position);
}
