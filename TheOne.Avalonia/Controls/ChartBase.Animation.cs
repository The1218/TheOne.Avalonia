using System.Diagnostics;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Media;
using global::Avalonia.Media.Imaging;
using global::Avalonia.Threading;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

public partial class ChartBase
{
    public static readonly StyledProperty<bool> IsAnimationEnabledProperty = AvaloniaProperty.Register<ChartBase, bool>(nameof(IsAnimationEnabled), true);
    public static readonly StyledProperty<TimeSpan> AnimationDurationProperty = AvaloniaProperty.Register<ChartBase, TimeSpan>(nameof(AnimationDuration), TimeSpan.FromMilliseconds(200), validate: value => value >= TimeSpan.Zero && value <= TimeSpan.FromSeconds(10));
    public static readonly DirectProperty<ChartBase, bool> IsAnimatingProperty = AvaloniaProperty.RegisterDirect<ChartBase, bool>(nameof(IsAnimating), chart => chart.IsAnimating);
    public static readonly DirectProperty<ChartBase, double> AnimationProgressProperty = AvaloniaProperty.RegisterDirect<ChartBase, double>(nameof(AnimationProgress), chart => chart.AnimationProgress);
    public bool IsAnimationEnabled { get => GetValue(IsAnimationEnabledProperty); set => SetValue(IsAnimationEnabledProperty, value); }
    public TimeSpan AnimationDuration { get => GetValue(AnimationDurationProperty); set => SetValue(AnimationDurationProperty, value); }
    public new bool IsAnimating => _reportedAnimating;
    public double AnimationProgress => _reportedAnimationProgress;
    private bool _reportedAnimating;
    private double _reportedAnimationProgress = 1;
    private bool _animationActive, _animateDataPending, _animationPublicationQueued, _hasPreparedGeometry;
    private long _animationGeneration;
    private readonly Stopwatch _animationClock = new();
    private double _animationProgress = 1;
    private CartesianFrame? _animationFromCartesian;
    private RenderTargetBitmap? _animationFromBitmap;
    private readonly Dictionary<(int Series, int Index), (double Start, double Sweep)> _animationFromAngles = [];
    private double _animationFromGauge, _animationToGauge, _preparedGaugeValue;
    private int _animationSuppression;

    private void MarkDataUpdate()
    {
        _animateDataPending = _hasPreparedGeometry && !_isExportSnapshot;
        MarkGeometryDirty();
    }
    private bool CanAnimateData() => _animationSuppression == 0 && !_isExportSnapshot && IsAnimationEnabled && Motion.GetEnabled(this) && IsEffectivelyEnabled && IsVisible &&
        AnimationDuration > TimeSpan.Zero && TopLevel.GetTopLevel(this) is not null && Bounds.Width <= 4096 && Bounds.Height <= 4096 && Bounds.Width * Bounds.Height <= 4_194_304;

    private sealed record AnimationOrigin(CartesianFrame? Frame, RenderTargetBitmap? Bitmap, Dictionary<(int Series, int Index), (double Start, double Sweep)> Angles, double Gauge);
    private AnimationOrigin? CaptureAnimationOrigin()
    {
        if (!_animateDataPending || !_hasPreparedGeometry || !CanAnimateData()) return null;
        var angles = new Dictionary<(int Series, int Index), (double Start, double Sweep)>();
        if (IsRadial)
        {
            if (_radialSectors.Count > 6000) return null;
            foreach (var sector in _radialSectors) angles[(sector.Series, sector.Index)] = PresentRadialAngles(sector.Series, sector.Index, sector.Start, sector.Sweep);
            return new AnimationOrigin(null, null, angles, _animationActive ? PresentGaugeValue(_animationToGauge) : _preparedGaugeValue);
        }
        if (_cartesianFrame is not { } frame) return null;
        if (FrameMarks(frame) > 6000) return null;
        if (_animationActive && _animationFromBitmap is not null)
        {
            // Flatten an interrupted crossfade once per data batch, rather than retaining an ever-growing frame chain.
            var bitmap = new RenderTargetBitmap(new PixelSize(Math.Max(1, (int)Math.Ceiling(Bounds.Width)), Math.Max(1, (int)Math.Ceiling(Bounds.Height))));
            using (var context = bitmap.CreateDrawingContext())
            using (context.PushClip(CartesianDataClip)) DrawAnimatedCartesianData(context, frame);
            return new AnimationOrigin(null, bitmap, angles, 0);
        }
        return new AnimationOrigin(_animationActive && _animationFromCartesian is not null ? InterpolateCartesianFrame(_animationFromCartesian, frame, EasedAnimationProgress()) : frame, null, angles, 0);
    }

    private void StartDataAnimation(AnimationOrigin? origin)
    {
        var requested = _animateDataPending; _animateDataPending = false;
        FinishDataAnimation(publish: false);
        var marks = IsRadial ? _radialSectors.Count : _cartesianFrame is { } prepared ? FrameMarks(prepared) : 0;
        if (IsGauge) marks = 1;
        if (!requested || origin is null || !CanAnimateData() || marks > 6000 || !IsRadial && _cartesianFrame is null)
        {
            origin?.Bitmap?.Dispose(); PublishAnimationState(); return;
        }
        _animationFromGauge = origin.Gauge; _animationToGauge = Value;
        foreach (var entry in origin.Angles) _animationFromAngles[entry.Key] = entry.Value;
        if (!IsRadial && _cartesianFrame is { } target)
        {
            if (origin.Frame is { } from && from.Identity == target.Identity && from.Categorical == target.Categorical && from.Count == target.Count && from.Series.Count == target.Series.Count && from.Bars.Count == target.Bars.Count)
                _animationFromCartesian = from;
            else
            {
                _animationFromBitmap = origin.Bitmap;
                if (_animationFromBitmap is null && origin.Frame is { } old)
                {
                    _animationFromBitmap = new RenderTargetBitmap(new PixelSize(Math.Max(1, (int)Math.Ceiling(Bounds.Width)), Math.Max(1, (int)Math.Ceiling(Bounds.Height))));
                    using var context = _animationFromBitmap.CreateDrawingContext();
                    using var clip = context.PushClip(CartesianDataClip);
                    DrawCartesianData(context, old);
                }
            }
        }
        _animationActive = true; _animationProgress = 0; _animationClock.Restart();
        var generation = ++_animationGeneration;
        PublishAnimationState(); ScheduleAnimationFrame(generation);
    }
    private void ScheduleAnimationFrame(long generation)
    {
        TopLevel.GetTopLevel(this)?.RequestAnimationFrame(_ =>
        {
            if (!_animationActive || generation != _animationGeneration) return;
            if (!CanAnimateData()) { FinishDataAnimation(); return; }
            _animationProgress = Math.Clamp(_animationClock.Elapsed.TotalMilliseconds / AnimationDuration.TotalMilliseconds, 0, 1);
            if (_animationProgress >= 1) { FinishDataAnimation(); return; }
            InvalidateVisual(); PublishAnimationState();
            if (_animationActive && generation == _animationGeneration) ScheduleAnimationFrame(generation);
        });
    }
    private double EasedAnimationProgress() => 1 - Math.Pow(1 - _animationProgress, 3);
    private static double Mix(double a, double b, double progress)
    {
        if (progress <= 0 || a == b) return a;
        if (progress >= 1) return b;
        var value = (1 - progress) * a + progress * b;
        return double.IsFinite(value) ? Math.Clamp(value, Math.Min(a, b), Math.Max(a, b)) : progress < 0.5 ? a : b;
    }
    private static Point MixPoint(Point a, Point b, double progress) => new(Mix(a.X, b.X, progress), Mix(a.Y, b.Y, progress));
    private static Rect MixRect(Rect a, Rect b, double progress) => new(Mix(a.X, b.X, progress), Mix(a.Y, b.Y, progress), Mix(a.Width, b.Width, progress), Mix(a.Height, b.Height, progress));
    private (double Start, double Sweep) PresentRadialAngles(int series, int index, double start, double sweep)
    {
        if (!_animationActive) return (start, sweep);
        var from = _animationFromAngles.TryGetValue((series, index), out var previous) ? previous : (start, 0d);
        var progress = EasedAnimationProgress();
        return (Mix(from.Item1, start, progress), Mix(from.Item2, sweep, progress));
    }
    private protected double PresentGaugeValue(double target) => _animationActive ? Mix(_animationFromGauge, _animationToGauge, EasedAnimationProgress()) : target;

    private void DrawAnimatedCartesianData(DrawingContext context, CartesianFrame target)
    {
        if (!_animationActive) { DrawCartesianData(context, target); return; }
        var progress = EasedAnimationProgress();
        if (_animationFromCartesian is { } previous) DrawCartesianData(context, InterpolateCartesianFrame(previous, target, progress));
        else
        {
            if (_animationFromBitmap is { } bitmap) using (context.PushOpacity(1 - progress)) context.DrawImage(bitmap, new Rect(Bounds.Size));
            using (context.PushOpacity(progress)) DrawCartesianData(context, target);
        }
    }
    private CartesianFrame InterpolateCartesianFrame(CartesianFrame from, CartesianFrame target, double progress)
    {
        var result = new CartesianFrame(target.Viewport, target.Count, target.Categorical, target.Horizontal, target.Stacked) { Identity = target.Identity };
        var bars = from.Bars.ToDictionary(bar => (bar.Series, bar.Index));
        foreach (var bar in target.Bars)
            result.Bars.Add(bar with { Bounds = bars.TryGetValue((bar.Series, bar.Index), out var previous) ? MixRect(previous.Bounds, bar.Bounds, progress) : bar.Bounds });
        var sources = from.Series.ToDictionary(series => series.Series);
        foreach (var series in target.Series)
        {
            var presented = series with { Segments = [] };
            sources.TryGetValue(series.Series, out var source);
            foreach (var segment in series.Segments)
            {
                var points = new ChartRenderPoint[segment.Points.Count];
                for (var n = 0; n < points.Length; n++)
                {
                    var point = segment.Points[n]; var origin = FindPreviousPoint(source, point);
                    points[n] = point with { Position = MixPoint(origin.Position, point.Position, progress), Baseline = MixPoint(origin.Baseline, point.Baseline, progress) };
                }
                presented.Segments.Add(new CartesianSegment(points, segment.Smooth));
            }
            result.Series.Add(presented);
        }
        PrepareCartesianDataLabels(result);
        return result;
    }
    private static ChartRenderPoint FindPreviousPoint(CartesianSeriesFrame? series, ChartRenderPoint target)
    {
        if (series is not null && series.Segments.Count > 0)
        {
            var segmentLow = 0; var segmentHigh = series.Segments.Count;
            while (segmentLow < segmentHigh)
            {
                var middle = segmentLow + (segmentHigh - segmentLow) / 2;
                if (series.Segments[middle].Points[^1].Index < target.Index) segmentLow = middle + 1; else segmentHigh = middle;
            }
            if (segmentLow >= series.Segments.Count) return target with { Position = target.Baseline };
            var segment = series.Segments[segmentLow];
            var points = segment.Points;
            if (target.Index < points[0].Index || target.Index > points[^1].Index) return target with { Position = target.Baseline };
            var low = 0; var high = points.Count;
            while (low < high) { var middle = low + (high - low) / 2; if (points[middle].Index < target.Index) low = middle + 1; else high = middle; }
            if (low < points.Count && points[low].Index == target.Index) return points[low];
            if (low > 0 && low < points.Count)
            {
                var a = points[low - 1]; var b = points[low]; var p = (target.Index - a.Index) / (double)(b.Index - a.Index);
                return target with { Position = MixPoint(a.Position, b.Position, p), Baseline = MixPoint(a.Baseline, b.Baseline, p) };
            }
        }
        return target with { Position = target.Baseline };
    }
    private static long FrameMarks(CartesianFrame frame) => frame.Bars.Count + frame.Series.Sum(series => series.Segments.Sum(segment => (long)segment.Points.Count));

    private void FinishDataAnimation(bool publish = true)
    {
        var changed = _animationActive || _animationProgress != 1 || _reportedAnimating || _reportedAnimationProgress != 1;
        ++_animationGeneration; _animationActive = false; _animationProgress = 1; _animationClock.Stop();
        _animationFromCartesian = null; _animationFromAngles.Clear();
        _animationFromBitmap?.Dispose(); _animationFromBitmap = null;
        if (publish && changed) { PublishAnimationState(); InvalidateVisual(); }
    }
    private readonly struct StableDataFrame : IDisposable
    {
        private readonly ChartBase _chart;
        public StableDataFrame(ChartBase chart) { _chart = chart; chart._animationSuppression++; chart.FinishDataAnimation(); }
        public void Dispose() => _chart._animationSuppression--;
    }
    private void PublishAnimationState()
    {
        if (_isExportSnapshot) return;
        if (_rendering)
        {
            if (_animationPublicationQueued) return;
            _animationPublicationQueued = true;
            Dispatcher.UIThread.Post(() => { _animationPublicationQueued = false; PublishAnimationState(); });
            return;
        }
        var generation = _animationGeneration; var version = _geometryVersion;
        SetAndRaise(AnimationProgressProperty, ref _reportedAnimationProgress, _animationProgress);
        if (generation == _animationGeneration && version == _geometryVersion) SetAndRaise(IsAnimatingProperty, ref _reportedAnimating, _animationActive);
    }
}
