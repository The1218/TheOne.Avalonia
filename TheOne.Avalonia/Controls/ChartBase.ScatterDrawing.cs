using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Rendering.SceneGraph;
using System.Diagnostics;

namespace TheOne.Avalonia.Controls;

public partial class ChartBase
{
    private HashSet<SolidColorBrush>? _scatterMarkerBrushes;
    private HashSet<SolidColorBrush>? _scatterUsedMarkerBrushes;
    private Predicate<SolidColorBrush>? _scatterUnusedBrush;
    // UI records/snapshots, raster batches/allocations/ticks/ellipses. Private, bounded diagnostics.
    private long[]? _scatterDrawingStats;

    private static Pen ScatterFramePen(CartesianFrame frame, int slot, IBrush? brush, double thickness)
    {
        var pens = frame.ScatterPens ??= new Pen?[4];
        var pen = pens[slot];
        if (pen is null || !ReferenceEquals(pen.Brush, brush) || pen.Thickness != thickness)
            pens[slot] = pen = new Pen(brush, thickness);
        return pen;
    }

    private bool TryDrawScatterMarkers(DrawingContext context, CartesianSeriesFrame series)
    {
        // Other brushes retain Avalonia's resource-aware drawing path, including visual/tile brushes.
        if (series.Brush is not (SolidColorBrush or ImmutableSolidColorBrush) ||
            Background is not (null or SolidColorBrush or ImmutableSolidColorBrush)) return false;
        ObserveScatterMarkerBrush(series.Brush);
        ObserveScatterMarkerBrush(Background);
        var stats = _scatterDrawingStats ??= new long[6];
        var fillKey = ScatterBrushColor.Capture(series.Brush)!.Value;
        var outlineKey = ScatterBrushColor.Capture(Background);
        var thickness = SelectionThickness * .75;
        var paint = series.ScatterPaint;
        if (paint is null || paint.FillKey != fillKey || paint.OutlineKey != outlineKey || paint.Thickness != thickness)
            series.ScatterPaint = paint = new ScatterMarkerPaint(fillKey, outlineKey, thickness,
                series.Brush.ToImmutable(), new ImmutablePen(Background?.ToImmutable(), thickness));
        var bounds = _cartesianPlot.Inflate(MarkerRadius + thickness / 2);
        foreach (var segment in series.Segments)
        {
            var snapshot = segment.ScatterMarkers;
            var selected = segment.Points as ScatterSelectedSample;
            var baseline = selected?.Baseline ?? segment.Points;
            if (snapshot is null || !ReferenceEquals(snapshot.Source, baseline))
            {
                var positions = new Point[baseline.Count]; var indices = new int[baseline.Count]; var count = 0;
                foreach (var point in baseline)
                    if (_cartesianPlot.Contains(point.Position))
                    {
                        positions[count] = point.Position; indices[count++] = point.Index;
                    }
                segment.ScatterMarkers = snapshot = new ScatterMarkerSnapshot(baseline, positions, indices, count);
                stats[1]++;
            }
            ScatterMarkerPin? pin = null;
            if (selected is not null && _cartesianPlot.Contains(selected.Selected.Position))
            {
                var insertion = Array.BinarySearch(snapshot.Indices, 0, snapshot.Count, selected.Selected.Index);
                pin = new ScatterMarkerPin(selected.Selected.Position, insertion < 0 ? ~insertion : insertion);
            }
            if (snapshot.Count > 0 || pin is not null)
            {
                stats[0]++;
                var operation = segment.ScatterOperation;
                if (operation is null || !operation.Matches(bounds, snapshot, paint, MarkerRadius, pin))
                    segment.ScatterOperation = operation = new ScatterMarkerOperation(bounds, snapshot, paint, MarkerRadius, pin, stats);
                context.Custom(operation);
            }
        }
        return true;
    }

    private void ObserveScatterMarkerBrush(IBrush? brush)
    {
        if (!_observing || brush is not SolidColorBrush solid) return;
        (_scatterUsedMarkerBrushes ??= new HashSet<SolidColorBrush>(ReferenceEqualityComparer.Instance)).Add(solid);
        if ((_scatterMarkerBrushes ??= new HashSet<SolidColorBrush>(ReferenceEqualityComparer.Instance)).Add(solid))
            solid.PropertyChanged += OnScatterMarkerBrushChanged;
    }

    private void OnScatterMarkerBrushChanged(object? sender, AvaloniaPropertyChangedEventArgs e) => InvalidateVisual();

    private void TrimScatterMarkerBrushes()
    {
        if (_scatterMarkerBrushes is not null)
            _scatterMarkerBrushes.RemoveWhere(_scatterUnusedBrush ??= RemoveUnusedScatterBrush);
    }

    private bool RemoveUnusedScatterBrush(SolidColorBrush brush)
    {
        if (_scatterUsedMarkerBrushes?.Contains(brush) == true) return false;
        brush.PropertyChanged -= OnScatterMarkerBrushChanged;
        return true;
    }

    private void ClearScatterMarkerBrushes()
    {
        if (_scatterMarkerBrushes is null) return;
        foreach (var brush in _scatterMarkerBrushes) brush.PropertyChanged -= OnScatterMarkerBrushChanged;
        _scatterMarkerBrushes.Clear();
        _scatterUsedMarkerBrushes?.Clear();
    }

    // A compositor operation owns an immutable coordinate snapshot, never a live model/list.
    private sealed record ScatterMarkerSnapshot(IReadOnlyList<ChartRenderPoint> Source, Point[] Positions, int[] Indices, int Count);

    private readonly record struct ScatterBrushColor(Color Color, double Opacity)
    {
        public static ScatterBrushColor? Capture(IBrush? brush) => brush is ISolidColorBrush solid
            ? new ScatterBrushColor(solid.Color, solid.Opacity) : null;
    }

    private sealed record ScatterMarkerPaint(ScatterBrushColor FillKey, ScatterBrushColor? OutlineKey, double Thickness,
        IImmutableBrush Fill, ImmutablePen Outline);

    private readonly record struct ScatterMarkerPin(Point Position, int Insertion);

    private sealed class ScatterMarkerOperation(Rect bounds, ScatterMarkerSnapshot snapshot, ScatterMarkerPaint paint,
        double radius, ScatterMarkerPin? pin, long[] stats) : ICustomDrawOperation
    {
        public Rect Bounds => bounds;
        public void Dispose() { }

        public bool Equals(ICustomDrawOperation? other) => ReferenceEquals(this, other) ||
            other is ScatterMarkerOperation operation && operation.Matches(bounds, snapshot, paint, radius, pin);

        public bool Matches(Rect otherBounds, ScatterMarkerSnapshot otherSnapshot, ScatterMarkerPaint otherPaint,
            double otherRadius, ScatterMarkerPin? otherPin) => bounds == otherBounds &&
            ReferenceEquals(snapshot, otherSnapshot) && ReferenceEquals(paint, otherPaint) && radius == otherRadius && pin == otherPin;

        public bool HitTest(Point p)
        {
            if (!bounds.Contains(p)) return false;
            var extent = radius + paint.Outline.Thickness / 2;
            if (pin is { } selected)
            {
                var delta = p - selected.Position;
                if (delta.X * delta.X + delta.Y * delta.Y <= extent * extent) return true;
            }
            for (var i = 0; i < snapshot.Count; i++)
            {
                var delta = p - snapshot.Positions[i];
                if (delta.X * delta.X + delta.Y * delta.Y <= extent * extent) return true;
            }
            return false;
        }

        public void Render(ImmediateDrawingContext context)
        {
            var allocated = GC.GetAllocatedBytesForCurrentThread(); var started = Stopwatch.GetTimestamp();
            // Keep separate ellipses in original order: merging geometry changes alpha overlap.
            if (pin is { } selected)
            {
                for (var i = 0; i < selected.Insertion; i++)
                    context.DrawEllipse(paint.Fill, paint.Outline, snapshot.Positions[i], radius, radius);
                context.DrawEllipse(paint.Fill, paint.Outline, selected.Position, radius, radius);
                for (var i = selected.Insertion; i < snapshot.Count; i++)
                    context.DrawEllipse(paint.Fill, paint.Outline, snapshot.Positions[i], radius, radius);
            }
            else
            {
                for (var i = 0; i < snapshot.Count; i++)
                    context.DrawEllipse(paint.Fill, paint.Outline, snapshot.Positions[i], radius, radius);
            }
            Interlocked.Increment(ref stats[2]);
            Interlocked.Add(ref stats[3], GC.GetAllocatedBytesForCurrentThread() - allocated);
            Interlocked.Add(ref stats[4], Stopwatch.GetTimestamp() - started);
            Interlocked.Add(ref stats[5], snapshot.Count + (pin is null ? 0 : 1));
        }
    }
}
