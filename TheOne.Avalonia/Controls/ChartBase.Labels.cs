using global::Avalonia;
using global::Avalonia.Media;

namespace TheOne.Avalonia.Controls;

public abstract partial class ChartBase
{
    private readonly record struct PreparedDataLabel(int Series, int Index, string Value, Rect Bounds, IBrush? Brush);
    private readonly record struct DataLabelCandidate(int Series, int Index, string Value, Point Position, IBrush? Brush);
    private Rect CartesianDataClip => IsCategoricalCartesian ? _cartesianPlot : _cartesianPlot.Inflate(MarkerRadius + SelectionThickness + 2);

    /// <summary>Returns the target bounds of a visible numeric data label, or null when it is omitted or labels are disabled.</summary>
    public Rect? GetDataLabelBounds(int seriesIndex, int pointIndex)
    {
        using var stable = new StableDataFrame(this);
        BuildGeometry(null);
        return !_geometryDirty && _cartesianFrame is { } frame && frame.DataLabelBounds.TryGetValue((seriesIndex, pointIndex), out var bounds) ? bounds : null;
    }

    private void PrepareCartesianDataLabels(CartesianFrame frame)
    {
        if (!ShowDataLabels || frame.Categorical || _cartesianPlot.Width <= 8 || _cartesianPlot.Height <= 8) return;
        // Bound annotation work independently from original source size and the sampling soft budget.
        var candidates = new List<DataLabelCandidate>();
        foreach (var series in frame.Series)
        foreach (var segment in series.Segments)
        foreach (var point in segment.Points)
        {
            if (candidates.Count == 2048) break;
            if (_cartesianPlot.Contains(point.Position) && series.Labels.TryGetValue(point.Index, out var value))
                candidates.Add(new(series.Series, point.Index, value, point.Position, series.Area ? Foreground : MutedBrush));
        }
        var occupied = new LabelOccupancy();
        var radius = MarkerRadius + SelectionThickness / 2;
        foreach (var candidate in candidates)
            occupied.Add(new Rect(candidate.Position.X - radius, candidate.Position.Y - radius, radius * 2, radius * 2));
        var gap = radius + 4;
        foreach (var candidate in candidates)
        {
            if (frame.DataLabels.Count == 512) break;
            var text = Text(candidate.Value, candidate.Brush);
            if (text.Width > _cartesianPlot.Width - 8 || text.Height > _cartesianPlot.Height - 4) continue;
            var point = candidate.Position;
            var centered = point.X - text.Width / 2;
            var above = point.Y - gap - text.Height;
            var below = point.Y + gap;
            Point[] anchors =
            [
                new(centered, above), new(centered, below),
                new(point.X + gap, above), new(point.X - gap - text.Width, above),
                new(point.X + gap, below), new(point.X - gap - text.Width, below),
                new(point.X + gap, point.Y - text.Height / 2), new(point.X - gap - text.Width, point.Y - text.Height / 2),
                new(centered, above - text.Height - 4), new(centered, below + text.Height + 4)
            ];
            foreach (var anchor in anchors)
            {
                if (anchor.Y < _cartesianPlot.Top + 2 || anchor.Y + text.Height > _cartesianPlot.Bottom - 2) continue;
                var bounds = new Rect(Math.Clamp(anchor.X, _cartesianPlot.Left + 4, _cartesianPlot.Right - text.Width - 4), anchor.Y, text.Width, text.Height);
                if (occupied.Intersects(bounds.Inflate(2))) continue;
                occupied.Add(bounds);
                frame.DataLabels.Add(new(candidate.Series, candidate.Index, candidate.Value, bounds, candidate.Brush));
                frame.DataLabelBounds[(candidate.Series, candidate.Index)] = bounds;
                break;
            }
        }
    }

    private sealed class LabelOccupancy
    {
        private const double CellSize = 32;
        private readonly Dictionary<(int X, int Y), List<Rect>> _cells = [];
        private readonly HashSet<Rect> _seen = [];
        public void Add(Rect bounds)
        {
            if (!_seen.Add(bounds)) return;
            var left = (int)Math.Floor(bounds.Left / CellSize); var right = (int)Math.Floor(bounds.Right / CellSize);
            var top = (int)Math.Floor(bounds.Top / CellSize); var bottom = (int)Math.Floor(bounds.Bottom / CellSize);
            for (var x = left; x <= right; x++)
            for (var y = top; y <= bottom; y++)
            {
                if (!_cells.TryGetValue((x, y), out var entries)) _cells[(x, y)] = entries = [];
                entries.Add(bounds);
            }
        }
        public bool Intersects(Rect bounds)
        {
            var left = (int)Math.Floor(bounds.Left / CellSize); var right = (int)Math.Floor(bounds.Right / CellSize);
            var top = (int)Math.Floor(bounds.Top / CellSize); var bottom = (int)Math.Floor(bounds.Bottom / CellSize);
            for (var x = left; x <= right; x++)
            for (var y = top; y <= bottom; y++)
                if (_cells.TryGetValue((x, y), out var entries) && entries.Any(bounds.Intersects)) return true;
            return false;
        }
    }
}
