using global::Avalonia;
using global::Avalonia.Threading;

namespace TheOne.Avalonia.Controls;

public enum ChartSamplingMode { None, MinMax }

public partial class ChartBase
{
    public static readonly StyledProperty<ChartSamplingMode> SamplingModeProperty = AvaloniaProperty.Register<ChartBase, ChartSamplingMode>(nameof(SamplingMode), ChartSamplingMode.MinMax, validate: value => value is ChartSamplingMode.None or ChartSamplingMode.MinMax);
    public static readonly StyledProperty<int> SamplingThresholdProperty = AvaloniaProperty.Register<ChartBase, int>(nameof(SamplingThreshold), 2000, validate: value => value >= 2);
    public static readonly StyledProperty<int> SamplingPointBudgetProperty = AvaloniaProperty.Register<ChartBase, int>(nameof(SamplingPointBudget), 2000, validate: value => value >= 8);
    public static readonly DirectProperty<ChartBase, int> RenderedPointCountProperty = AvaloniaProperty.RegisterDirect<ChartBase, int>(nameof(RenderedPointCount), chart => chart.RenderedPointCount);
    public static readonly DirectProperty<ChartBase, bool> SamplingActiveProperty = AvaloniaProperty.RegisterDirect<ChartBase, bool>(nameof(SamplingActive), chart => chart.SamplingActive);
    public ChartSamplingMode SamplingMode { get => GetValue(SamplingModeProperty); set => SetValue(SamplingModeProperty, value); }
    public int SamplingThreshold { get => GetValue(SamplingThresholdProperty); set => SetValue(SamplingThresholdProperty, value); }
    /// <summary>A per-series sampling target; gap boundaries and selected originals can exceed it.</summary>
    public int SamplingPointBudget { get => GetValue(SamplingPointBudgetProperty); set => SetValue(SamplingPointBudgetProperty, value); }
    public int RenderedPointCount => _renderedPointCount;
    public bool SamplingActive => _samplingActive;
    private int _renderedPointCount;
    private bool _samplingActive;
    private bool _samplingReduced;
    private bool _diagnosticsQueued;
    private (int Count, bool Active) _pendingSamplingDiagnostics;

    private void SetSamplingDiagnostics(int count, bool active)
    {
        if (_isExportSnapshot) { _renderedPointCount = count; _samplingActive = active || _samplingReduced; return; }
        _pendingSamplingDiagnostics = (count, active || _samplingReduced);
        // A binding listener may update the chart; Avalonia forbids that inside a render pass.
        if (_rendering)
        {
            if (!_diagnosticsQueued)
            {
                _diagnosticsQueued = true;
                Dispatcher.UIThread.Post(() => { _diagnosticsQueued = false; PublishSamplingDiagnostics(); });
            }
            return;
        }
        PublishSamplingDiagnostics();
    }
    private void PublishSamplingDiagnostics()
    {
        var pending = _pendingSamplingDiagnostics;
        var version = _geometryVersion;
        SetAndRaise(RenderedPointCountProperty, ref _renderedPointCount, pending.Count);
        if (version == _geometryVersion) SetAndRaise(SamplingActiveProperty, ref _samplingActive, pending.Active);
    }
    private IReadOnlyList<ChartRenderPoint> SampleSeriesPoints(int seriesIndex, IReadOnlyList<ChartRenderPoint> points, Rect plot)
    {
        var total = SeriesSource![seriesIndex].ItemsSource!.Count;
        if (SamplingMode == ChartSamplingMode.None || total < SamplingThreshold || points.Count < 3) return points;
        var ascending = true; var descending = true;
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
        for (var index = first; index <= last; index++)
        {
            var point = points[index];
            var bucketIndex = Math.Clamp((int)Math.Floor((point.Position.X - plot.Left) / Math.Max(1, plot.Width) * bucketCount), 0, bucketCount - 1);
            if (!buckets.TryGetValue(bucketIndex, out var bucket)) buckets[bucketIndex] = new SampleBucket(index, point);
            else bucket.Add(index, point);
        }
        var retained = new SortedSet<int> { first, last };
        foreach (var bucket in buckets.Values) bucket.Retain(retained);
        if (SelectedSeriesIndex == seriesIndex && SelectedIndex >= points[first].Index && SelectedIndex <= points[last].Index)
            for (var index = first; index <= last; index++)
                if (points[index].Index == SelectedIndex) { retained.Add(index); break; }
        if (retained.Count >= points.Count) return points;
        _samplingReduced = true;
        return retained.Select(index => points[index]).ToArray();
    }
    private static int LowerBound(IReadOnlyList<ChartRenderPoint> points, double x, bool ascending)
    {
        var low = 0; var high = points.Count;
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
            if (point.Position.Y < _topMinimum) { _topMinimum = point.Position.Y; _topMin = index; }
            if (point.Position.Y > _topMaximum) { _topMaximum = point.Position.Y; _topMax = index; }
            if (point.Baseline.Y < _baseMinimum) { _baseMinimum = point.Baseline.Y; _baseMin = index; }
            if (point.Baseline.Y > _baseMaximum) { _baseMaximum = point.Baseline.Y; _baseMax = index; }
        }
        public void Retain(SortedSet<int> indices)
        {
            indices.Add(_first); indices.Add(_last); indices.Add(_topMin); indices.Add(_topMax); indices.Add(_baseMin); indices.Add(_baseMax);
        }
    }
}
