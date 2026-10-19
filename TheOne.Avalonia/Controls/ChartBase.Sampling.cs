using Avalonia;
using Avalonia.Threading;

namespace TheOne.Avalonia.Controls;

public enum ChartSamplingMode
{
    None,
    MinMax
}

public partial class ChartBase
{
    private void PreserveSelectedSample(ChartSelection? selection)
    {
        if (_geometryDirty || _cartesianFrame is not { Categorical: false } frame) return;
        var changed = false;
        foreach (var series in frame.Series)
        foreach (var segment in series.Segments)
            changed |= SelectSegmentSample(segment, series.Series, selection?.SeriesIndex ?? -1, selection?.PointIndex ?? -1, IsScatterCartesian);
        if (!changed) return;
        RefreshCartesianLabels(frame);
        PublishFrameSampling(frame);
        InvalidateVisual();
    }

    private static bool SelectSegmentSample(CartesianSegment segment, int series, int selectedSeries, int selectedIndex, bool scatter = false)
    {
        if (segment.Originals is not { } originals || segment.BasePoints is not { } baseline ||
            ReferenceEquals(originals, baseline)) return false;
        IReadOnlyList<ChartRenderPoint> next = baseline;
        if (series == selectedSeries && FindPoint(originals, selectedIndex) is var original && original >= 0 &&
            FindPoint(baseline, selectedIndex) < 0)
        {
            // The base sample is selection-independent. Keep exactly one extra original.
            if (scatter)
            {
                next = segment.Points is ScatterSelectedSample current && ReferenceEquals(current.Baseline, baseline) &&
                       current.Selected == originals[original]
                    ? current : new ScatterSelectedSample(baseline, originals[original]);
            }
            else
            {
                var insertion = 0;
                while (insertion < baseline.Count && baseline[insertion].Index < selectedIndex) insertion++;
                var points = new ChartRenderPoint[baseline.Count + 1];
                for (var i = 0; i < insertion; i++) points[i] = baseline[i];
                points[insertion] = originals[original];
                for (var i = insertion; i < baseline.Count; i++) points[i + 1] = baseline[i];
                next = points;
            }
        }
        if (ReferenceEquals(segment.Points, next)) return false;
        segment.Points = next;
        segment.LineGeometry = null;
        segment.FillGeometry = null;
        return true;
    }

    private static int FindPoint(IReadOnlyList<ChartRenderPoint> points, int index)
    {
        var low = 0; var high = points.Count;
        while (low < high)
        {
            var middle = low + (high - low) / 2;
            if (points[middle].Index < index) low = middle + 1;
            else high = middle;
        }
        return low < points.Count && points[low].Index == index ? low : -1;
    }

    public static readonly StyledProperty<ChartSamplingMode> SamplingModeProperty =
        AvaloniaProperty.Register<ChartBase, ChartSamplingMode>(nameof(SamplingMode), ChartSamplingMode.MinMax,
            validate: value => value is ChartSamplingMode.None or ChartSamplingMode.MinMax);

    public static readonly StyledProperty<int> SamplingThresholdProperty =
        AvaloniaProperty.Register<ChartBase, int>(nameof(SamplingThreshold), 2000, validate: value => value >= 2);

    public static readonly StyledProperty<int> SamplingPointBudgetProperty =
        AvaloniaProperty.Register<ChartBase, int>(nameof(SamplingPointBudget), 2000, validate: value => value >= 8);

    public static readonly DirectProperty<ChartBase, int> RenderedPointCountProperty =
        AvaloniaProperty.RegisterDirect<ChartBase, int>(nameof(RenderedPointCount), chart => chart.RenderedPointCount);

    public static readonly DirectProperty<ChartBase, bool> SamplingActiveProperty =
        AvaloniaProperty.RegisterDirect<ChartBase, bool>(nameof(SamplingActive), chart => chart.SamplingActive);

    private bool _diagnosticsQueued;
    private (int Count, bool Active) _pendingSamplingDiagnostics;
    private int _renderedPointCount;
    private bool _samplingActive;
    private long _samplingRevision;

    public ChartSamplingMode SamplingMode
    {
        get => GetValue(SamplingModeProperty);
        set => SetValue(SamplingModeProperty, value);
    }

    public int SamplingThreshold
    {
        get => GetValue(SamplingThresholdProperty);
        set => SetValue(SamplingThresholdProperty, value);
    }

    /// <summary>A per-series sampling target; gap boundaries and selected originals can exceed it.</summary>
    public int SamplingPointBudget
    {
        get => GetValue(SamplingPointBudgetProperty);
        set => SetValue(SamplingPointBudgetProperty, value);
    }

    public int RenderedPointCount => _renderedPointCount;
    public bool SamplingActive => _samplingActive;

    private void SetSamplingDiagnostics(int count, bool active)
    {
        _samplingRevision++;
        if (_isExportSnapshot)
        {
            _renderedPointCount = count;
            _samplingActive = active;
            return;
        }

        _pendingSamplingDiagnostics = (count, active);
        // A binding listener may update the chart; Avalonia forbids that inside a render pass.
        if (_rendering)
        {
            if (!_diagnosticsQueued)
            {
                _diagnosticsQueued = true;
                Dispatcher.UIThread.Post(() =>
                {
                    _diagnosticsQueued = false;
                    PublishSamplingDiagnostics();
                });
            }

            return;
        }

        PublishSamplingDiagnostics();
    }

    private void PublishSamplingDiagnostics()
    {
        var pending = _pendingSamplingDiagnostics;
        var version = _geometryVersion;
        var revision = _samplingRevision;
        SetAndRaise(RenderedPointCountProperty, ref _renderedPointCount, pending.Count);
        if (version == _geometryVersion && revision == _samplingRevision)
            SetAndRaise(SamplingActiveProperty, ref _samplingActive, pending.Active);
    }

    private long _samplingScanPoints;

    private IReadOnlyList<ChartRenderPoint> SampleSeriesPoints(int seriesIndex, IReadOnlyList<ChartRenderPoint> points,
        Rect plot, out SegmentSampleIndex? sampleIndex)
    {
        sampleIndex = SegmentSampleIndex.PassThrough;
        var total = SeriesSource![seriesIndex].ItemsSource!.Count;
        if (SamplingMode == ChartSamplingMode.None || total < SamplingThreshold || points.Count < 3) return points;
        var ascending = true;
        var descending = true;
        _samplingScanPoints += points.Count - 1;
        for (var index = 1; index < points.Count; index++)
        {
            ascending &= points[index].Position.X >= points[index - 1].Position.X;
            descending &= points[index].Position.X <= points[index - 1].Position.X;
        }

        // Preserve the connection order of irregular/looping curves rather than sorting their input.
        if (!ascending && !descending) return points;
        var first = LowerBound(points, plot.Left, ascending);
        var last = LowerBound(points, plot.Right, ascending);
        if (!ascending) (first, last) = (last, first);
        first = Math.Clamp(first - 1, 0, points.Count - 1);
        last = Math.Clamp(last + 1, first, points.Count - 1);
        var proportionalBudget = Math.Max(8, (int)Math.Ceiling(SamplingPointBudget * (double)points.Count / total));
        var bucketCount = Math.Max(1, Math.Min((int)Math.Ceiling(plot.Width), proportionalBudget / 6));
        var buckets = new Dictionary<int, SampleBucket>();
        var finite = true;
        _samplingScanPoints += last - first + 1;
        for (var index = first; index <= last; index++)
        {
            var point = points[index];
            finite &= double.IsFinite(point.Position.Y) && double.IsFinite(point.Baseline.Y);
            var bucketIndex =
                Math.Clamp((int)Math.Floor((point.Position.X - plot.Left) / Math.Max(1, plot.Width) * bucketCount), 0,
                    bucketCount - 1);
            if (!buckets.TryGetValue(bucketIndex, out var bucket))
                buckets[bucketIndex] = new SampleBucket(index, point);
            else bucket.Add(index, point);
        }

        // Keep the full preparation linear as an independent reference for incremental queries.
        sampleIndex = finite ? new SegmentSampleIndex(points, first, last, buckets.Values.ToArray()) : null;
        var retained = new SortedSet<int> { first, last };
        foreach (var bucket in buckets.Values) bucket.Retain(retained);
        if (retained.Count >= points.Count) return points;
        return retained.Select(index => points[index]).ToArray();
    }

    private static int LowerBound(IReadOnlyList<ChartRenderPoint> points, double x, bool ascending)
    {
        var low = 0;
        var high = points.Count;
        while (low < high)
        {
            var middle = low + (high - low) / 2;
            if (ascending ? points[middle].Position.X < x : points[middle].Position.X > x) low = middle + 1;
            else high = middle;
        }

        return low;
    }

    private sealed class SampleBucket
    {
        public int First => _first;
        public int Last => _last;

        public void Refresh(SampleExtrema extrema)
        {
            _topMin = extrema.TopMin; _topMax = extrema.TopMax;
            _baseMin = extrema.BaseMin; _baseMax = extrema.BaseMax;
        }

        private readonly int _first;
        private int _last, _topMin, _topMax, _baseMin, _baseMax;
        private double _topMinimum, _topMaximum, _baseMinimum, _baseMaximum;

        public SampleBucket(int index, ChartRenderPoint point)
        {
            _first = _last = _topMin = _topMax = _baseMin = _baseMax = index;
            _topMinimum = _topMaximum = point.Position.Y;
            _baseMinimum = _baseMaximum = point.Baseline.Y;
        }

        public void Add(int index, ChartRenderPoint point)
        {
            _last = index;
            if (point.Position.Y < _topMinimum)
            {
                _topMinimum = point.Position.Y;
                _topMin = index;
            }

            if (point.Position.Y > _topMaximum)
            {
                _topMaximum = point.Position.Y;
                _topMax = index;
            }

            if (point.Baseline.Y < _baseMinimum)
            {
                _baseMinimum = point.Baseline.Y;
                _baseMin = index;
            }

            if (point.Baseline.Y > _baseMaximum)
            {
                _baseMaximum = point.Baseline.Y;
                _baseMax = index;
            }
        }

        public void Retain(SortedSet<int> indices)
        {
            indices.Add(_first);
            indices.Add(_last);
            indices.Add(_topMin);
            indices.Add(_topMax);
            indices.Add(_baseMin);
            indices.Add(_baseMax);
        }
    }
}
