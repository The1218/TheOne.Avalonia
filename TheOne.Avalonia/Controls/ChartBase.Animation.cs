using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

public partial class ChartBase
{
    public static readonly StyledProperty<bool> IsAnimationEnabledProperty =
        AvaloniaProperty.Register<ChartBase, bool>(nameof(IsAnimationEnabled), true);

    public static readonly StyledProperty<TimeSpan> AnimationDurationProperty =
        AvaloniaProperty.Register<ChartBase, TimeSpan>(nameof(AnimationDuration), TimeSpan.FromMilliseconds(200),
            validate: value => value >= TimeSpan.Zero && value <= TimeSpan.FromSeconds(10));

    public static readonly StyledProperty<ChartAnimationStyle> AnimationStyleProperty =
        AvaloniaProperty.Register<ChartBase, ChartAnimationStyle>(nameof(AnimationStyle),
            validate: value => Enum.IsDefined(value));

    public static readonly DirectProperty<ChartBase, bool> IsAnimatingProperty =
        AvaloniaProperty.RegisterDirect<ChartBase, bool>(nameof(IsAnimating), chart => chart.IsAnimating);

    public static readonly DirectProperty<ChartBase, double> AnimationProgressProperty =
        AvaloniaProperty.RegisterDirect<ChartBase, double>(nameof(AnimationProgress), chart => chart.AnimationProgress);

    private readonly Stopwatch _animationClock = new();
    private Action<TimeSpan>? _animationFrameCallback;
    private readonly Dictionary<(int Series, int Index), (double Start, double Sweep)> _animationFromAngles = [];
    private bool _animateDataPending, _animationPublicationQueued, _hasPreparedGeometry;
    private RenderTargetBitmap? _animationFromBitmap;
    private CartesianFrame? _animationFromCartesian;
    private CartesianAnimationPlan? _cartesianAnimationPlan;
    private ChartAnimationStyle _activeAnimationStyle;
    private bool _initialAnimationPending, _hasPresentedChart;
    private double _animationFromGauge, _animationToGauge, _preparedGaugeValue;
    private long _animationGeneration;
    private double _animationProgress = 1;
    private int _animationSuppression;
    private bool _reportedAnimating;
    private double _reportedAnimationProgress = 1;

    public bool IsAnimationEnabled
    {
        get => GetValue(IsAnimationEnabledProperty);
        set => SetValue(IsAnimationEnabledProperty, value);
    }

    public TimeSpan AnimationDuration
    {
        get => GetValue(AnimationDurationProperty);
        set => SetValue(AnimationDurationProperty, value);
    }

    /// <summary>Selects automatic interpolation, no motion, a fade or a horizontal reveal of the plot.</summary>
    public ChartAnimationStyle AnimationStyle
    {
        get => GetValue(AnimationStyleProperty);
        set => SetValue(AnimationStyleProperty, value);
    }

    public new bool IsAnimating => _reportedAnimating;
    public double AnimationProgress => _reportedAnimationProgress;

    private protected bool IsGaugeValueAnimating { get; private set; }

    private protected virtual bool UsesCustomChartGeometry => false;

    private void PrepareInitialAnimation()
    {
        _initialAnimationPending = !_hasPresentedChart;
    }

    private void MarkDataUpdate()
    {
        _animateDataPending = _hasPreparedGeometry && !_isExportSnapshot;
        MarkGeometryDirty();
    }

    private bool CanAnimateData()
    {
        return _animationSuppression == 0 && !_isExportSnapshot && IsAnimationEnabled &&
               AnimationStyle != ChartAnimationStyle.None && Motion.GetEnabled(this) &&
               IsEffectivelyEnabled && IsVisible &&
               AnimationDuration > TimeSpan.Zero && TopLevel.GetTopLevel(this) is not null && Bounds.Width <= 4096 &&
               Bounds.Height <= 4096 && Bounds.Width * Bounds.Height <= 4_194_304;
    }

    private AnimationOrigin? CaptureAnimationOrigin()
    {
        if (((!_animateDataPending || !_hasPreparedGeometry) && !_initialAnimationPending) || !CanAnimateData()) return null;
        var angles = new Dictionary<(int Series, int Index), (double Start, double Sweep)>();
        if (_initialAnimationPending)
            return new AnimationOrigin(null, null, angles, Minimum, true);
        if (AnimationStyle != ChartAnimationStyle.Auto || UsesCustomChartGeometry)
            return new AnimationOrigin(null, null, angles, _preparedGaugeValue);
        if (IsRadial)
        {
            if (_radialSectors.Count > 6000) return null;
            foreach (var sector in _radialSectors)
                angles[(sector.Series, sector.Index)] =
                    PresentRadialAngles(sector.Series, sector.Index, sector.Start, sector.Sweep);
            return new AnimationOrigin(null, null, angles,
                IsGaugeValueAnimating ? PresentGaugeValue(_animationToGauge) : _preparedGaugeValue);
        }

        if (_cartesianFrame is not { } frame) return null;
        if (FrameMarks(frame) > 6000) return null;
        if (IsGaugeValueAnimating && _animationFromBitmap is not null)
        {
            // Flatten an interrupted crossfade once per data batch, rather than retaining an ever-growing frame chain.
            var bitmap = new RenderTargetBitmap(new PixelSize(Math.Max(1, (int)Math.Ceiling(Bounds.Width)),
                Math.Max(1, (int)Math.Ceiling(Bounds.Height))));
            using (var context = bitmap.CreateDrawingContext())
            using (context.PushClip(CartesianDataClip))
            {
                DrawAnimatedCartesianData(context, frame);
            }

            return new AnimationOrigin(null, bitmap, angles, 0);
        }

        return new AnimationOrigin(
            IsGaugeValueAnimating && _animationFromCartesian is not null
                ? InterpolateCartesianFrame(_animationFromCartesian, frame, EasedAnimationProgress())
                : frame, null, angles, 0);
    }

    private void StartDataAnimation(AnimationOrigin? origin, bool presentation)
    {
        var requested = _animateDataPending || _initialAnimationPending;
        _animateDataPending = false;
        FinishDataAnimation(false);
        var marks = UsesCustomChartGeometry ? CustomChartAnimationMarkCount :
            IsRadial ? _radialSectors.Count : _cartesianFrame is { } prepared ? FrameMarks(prepared) : 0;
        if (IsGauge) marks = 1;
        if (marks > 0 && presentation)
        {
            _initialAnimationPending = false;
            _hasPresentedChart = true;
        }
        if ((!_hasPresentedChart && !presentation) || !requested || origin is null || !CanAnimateData() || marks is 0 or > 6000 ||
            (!IsRadial && !UsesCustomChartGeometry && _cartesianFrame is null))
        {
            origin?.Bitmap?.Dispose();
            PublishAnimationState();
            return;
        }

        _animationFromGauge = origin.Gauge;
        _animationToGauge = Value;
        _activeAnimationStyle = AnimationStyle == ChartAnimationStyle.Auto && origin.Initial
            ? ChartAnimationStyle.Reveal
            : AnimationStyle == ChartAnimationStyle.Auto && UsesCustomChartGeometry
                ? ChartAnimationStyle.Fade : AnimationStyle;
        foreach (var entry in origin.Angles) _animationFromAngles[entry.Key] = entry.Value;
        if (_activeAnimationStyle == ChartAnimationStyle.Auto && !IsRadial && _cartesianFrame is { } target)
        {
            if (origin.Frame is { } from && from.Identity == target.Identity &&
                from.Categorical == target.Categorical && from.Count == target.Count &&
                from.Series.Count == target.Series.Count && from.Bars.Count == target.Bars.Count)
            {
                _animationFromCartesian = from;
            }
            else
            {
                _animationFromBitmap = origin.Bitmap;
                if (_animationFromBitmap is null && origin.Frame is { } old)
                {
                    _animationFromBitmap = new RenderTargetBitmap(new PixelSize(
                        Math.Max(1, (int)Math.Ceiling(Bounds.Width)), Math.Max(1, (int)Math.Ceiling(Bounds.Height))));
                    using var context = _animationFromBitmap.CreateDrawingContext();
                    using var clip = context.PushClip(CartesianDataClip);
                    DrawCartesianData(context, old);
                }
            }
        }

        IsGaugeValueAnimating = true;
        _animationProgress = 0;
        _animationClock.Restart();
        var generation = ++_animationGeneration;
        _animationFrameCallback = _ => AdvanceAnimationFrame(generation);
        PublishAnimationState();
        ScheduleAnimationFrame(generation);
    }

    private void ScheduleAnimationFrame(long generation)
    {
        if (IsGaugeValueAnimating && generation == _animationGeneration && _animationFrameCallback is { } callback)
            TopLevel.GetTopLevel(this)?.RequestAnimationFrame(callback);
    }

    private void AdvanceAnimationFrame(long generation)
    {
        if (!IsGaugeValueAnimating || generation != _animationGeneration) return;
        if (!CanAnimateData())
        {
            FinishDataAnimation();
            return;
        }

        _animationProgress =
            Math.Clamp(_animationClock.Elapsed.TotalMilliseconds / AnimationDuration.TotalMilliseconds, 0, 1);
        if (_animationProgress >= 1)
        {
            FinishDataAnimation();
            return;
        }

        InvalidateVisual();
        PublishAnimationState();
        ScheduleAnimationFrame(generation);
    }

    private double EasedAnimationProgress()
    {
        return 1 - Math.Pow(1 - _animationProgress, 3);
    }

    private static double Mix(double a, double b, double progress)
    {
        if (progress <= 0 || a == b) return a;
        if (progress >= 1) return b;
        var value = (1 - progress) * a + progress * b;
        return double.IsFinite(value) ? Math.Clamp(value, Math.Min(a, b), Math.Max(a, b)) : progress < 0.5 ? a : b;
    }

    private static Point MixPoint(Point a, Point b, double progress)
    {
        return new Point(Mix(a.X, b.X, progress), Mix(a.Y, b.Y, progress));
    }

    private static Rect MixRect(Rect a, Rect b, double progress)
    {
        return new Rect(Mix(a.X, b.X, progress), Mix(a.Y, b.Y, progress), Mix(a.Width, b.Width, progress),
            Mix(a.Height, b.Height, progress));
    }

    private (double Start, double Sweep) PresentRadialAngles(int series, int index, double start, double sweep)
    {
        if (!IsGaugeValueAnimating || _activeAnimationStyle != ChartAnimationStyle.Auto) return (start, sweep);
        var from = _animationFromAngles.TryGetValue((series, index), out var previous) ? previous : (start, 0d);
        var progress = EasedAnimationProgress();
        return (Mix(from.Item1, start, progress), Mix(from.Item2, sweep, progress));
    }

    private protected double PresentGaugeValue(double target)
    {
        return IsGaugeValueAnimating && _activeAnimationStyle == ChartAnimationStyle.Auto
            ? Mix(_animationFromGauge, _animationToGauge, EasedAnimationProgress()) : target;
    }

    private void DrawAnimatedChartPlot(DrawingContext context, Rect plot)
    {
        if (!IsGaugeValueAnimating || _activeAnimationStyle == ChartAnimationStyle.Auto)
        {
            DrawChartPlot(context, plot);
            return;
        }

        var progress = EasedAnimationProgress();
        if (_activeAnimationStyle == ChartAnimationStyle.Reveal)
        {
            using var clip = context.PushClip(new Rect(plot.X, plot.Y, plot.Width * progress, plot.Height));
            DrawChartPlot(context, plot);
        }
        else
        {
            using var opacity = context.PushOpacity(progress);
            DrawChartPlot(context, plot);
        }
    }

    private void DrawAnimatedCartesianData(DrawingContext context, CartesianFrame target)
    {
        if (!IsGaugeValueAnimating || _activeAnimationStyle != ChartAnimationStyle.Auto)
        {
            DrawCartesianData(context, target);
            return;
        }

        var progress = EasedAnimationProgress();
        if (_animationFromCartesian is { } previous)
        {
            DrawCartesianData(context, InterpolateCartesianFrame(previous, target, progress));
        }
        else
        {
            if (_animationFromBitmap is { } bitmap)
                using (context.PushOpacity(1 - progress))
                {
                    context.DrawImage(bitmap, new Rect(Bounds.Size));
                }

            using (context.PushOpacity(progress))
            {
                DrawCartesianData(context, target);
            }
        }
    }

    private CartesianFrame InterpolateCartesianFrame(CartesianFrame from, CartesianFrame target, double progress)
    {
        var plan = _cartesianAnimationPlan;
        if (plan is null || !ReferenceEquals(plan.From, from) || !ReferenceEquals(plan.Target, target))
            _cartesianAnimationPlan = plan = new CartesianAnimationPlan(from, target);
        plan.Update(progress);
        return plan.Frame;
    }

    private static ChartRenderPoint FindPreviousPoint(CartesianSeriesFrame? series, ChartRenderPoint target)
    {
        if (series is not null && series.Segments.Count > 0)
        {
            var segmentLow = 0;
            var segmentHigh = series.Segments.Count;
            while (segmentLow < segmentHigh)
            {
                var middle = segmentLow + (segmentHigh - segmentLow) / 2;
                if (series.Segments[middle].Points[^1].Index < target.Index) segmentLow = middle + 1;
                else segmentHigh = middle;
            }

            if (segmentLow >= series.Segments.Count) return target with { Position = target.Baseline };
            var segment = series.Segments[segmentLow];
            var points = segment.Points;
            if (target.Index < points[0].Index || target.Index > points[^1].Index)
                return target with { Position = target.Baseline };
            var low = 0;
            var high = points.Count;
            while (low < high)
            {
                var middle = low + (high - low) / 2;
                if (points[middle].Index < target.Index) low = middle + 1;
                else high = middle;
            }

            if (low < points.Count && points[low].Index == target.Index) return points[low];
            if (low > 0 && low < points.Count)
            {
                var a = points[low - 1];
                var b = points[low];
                var p = (target.Index - a.Index) / (double)(b.Index - a.Index);
                return target with
                {
                    Position = MixPoint(a.Position, b.Position, p), Baseline = MixPoint(a.Baseline, b.Baseline, p)
                };
            }
        }

        return target with { Position = target.Baseline };
    }

    private static long FrameMarks(CartesianFrame frame)
    {
        return frame.Bars.Count +
               frame.Series.Sum(series => series.Segments.Sum(segment => (long)segment.Points.Count));
    }

    private void FinishDataAnimation(bool publish = true)
    {
        var changed = IsGaugeValueAnimating || _animationProgress != 1 || _reportedAnimating ||
                      _reportedAnimationProgress != 1;
        ++_animationGeneration;
        IsGaugeValueAnimating = false;
        _animationProgress = 1;
        _animationClock.Stop();
        _animationFrameCallback = null;
        _animationFromCartesian = null;
        _cartesianAnimationPlan = null;
        _animationFromAngles.Clear();
        _animationFromBitmap?.Dispose();
        _animationFromBitmap = null;
        if (publish && changed)
        {
            PublishAnimationState();
            InvalidateVisual();
        }
    }

    private void PublishAnimationState()
    {
        if (_isExportSnapshot) return;
        if (_rendering)
        {
            if (_animationPublicationQueued) return;
            _animationPublicationQueued = true;
            Dispatcher.UIThread.Post(() =>
            {
                _animationPublicationQueued = false;
                PublishAnimationState();
            });
            return;
        }

        var generation = _animationGeneration;
        var version = _geometryVersion;
        SetAndRaise(AnimationProgressProperty, ref _reportedAnimationProgress, _animationProgress);
        if (generation == _animationGeneration && version == _geometryVersion)
            SetAndRaise(IsAnimatingProperty, ref _reportedAnimating, IsGaugeValueAnimating);
    }

    private sealed record AnimationOrigin(
        CartesianFrame? Frame,
        RenderTargetBitmap? Bitmap,
        Dictionary<(int Series, int Index), (double Start, double Sweep)> Angles,
        double Gauge,
        bool Initial = false);

    // The sampling result and source-point lookup are fixed for an animation. Only positions change per tick.
    private sealed class CartesianAnimationPlan
    {
        public CartesianFrame From { get; }
        public CartesianFrame Target { get; }
        public CartesianFrame Frame { get; }
        private readonly Rect[] _barOrigins;
        private readonly List<AnimatedSegment> _segments = [];
        private readonly Rect[] _labelOrigins;
        private double _progress = double.NaN;

        public CartesianAnimationPlan(CartesianFrame from, CartesianFrame target)
        {
            From = from;
            Target = target;
            Frame = new CartesianFrame(target.Viewport, target.Count, target.Categorical, target.Horizontal,
                target.Stacked) { Identity = target.Identity };
            var bars = from.Bars.ToDictionary(bar => (bar.Series, bar.Index));
            _barOrigins = new Rect[target.Bars.Count];
            for (var n = 0; n < target.Bars.Count; n++)
            {
                var bar = target.Bars[n];
                Frame.Bars.Add(bar);
                _barOrigins[n] = bars.TryGetValue((bar.Series, bar.Index), out var previous)
                    ? previous.Bounds : bar.Bounds;
            }

            var sources = from.Series.ToDictionary(series => series.Series);
            foreach (var series in target.Series)
            {
                var presented = series with { Segments = [] };
                sources.TryGetValue(series.Series, out var source);
                foreach (var segment in series.Segments)
                {
                    var points = new ChartRenderPoint[segment.Points.Count];
                    var origins = new ChartRenderPoint[points.Length];
                    for (var n = 0; n < points.Length; n++) origins[n] = FindPreviousPoint(source, segment.Points[n]);
                    var rendered = new CartesianSegment(points, segment.Smooth);
                    presented.Segments.Add(rendered);
                    _segments.Add(new AnimatedSegment(segment, rendered, points, origins));
                }
                Frame.Series.Add(presented);
            }

            // Keep the target's collision-free label layout and interpolate existing label anchors only.
            _labelOrigins = new Rect[target.DataLabels.Count];
            for (var n = 0; n < target.DataLabels.Count; n++)
            {
                var label = target.DataLabels[n];
                Frame.DataLabels.Add(label);
                Frame.DataLabelBounds[(label.Series, label.Index)] = label.Bounds;
                _labelOrigins[n] = from.DataLabelBounds.TryGetValue((label.Series, label.Index), out var previous)
                    ? previous : label.Bounds;
            }
        }

        public void Update(double progress)
        {
            // Several render passes can observe the same animation tick. Retain their prepared paths and snapshots.
            if (_progress == progress) return;
            _progress = progress;
            for (var n = 0; n < Target.Bars.Count; n++)
            {
                var bar = Target.Bars[n];
                Frame.Bars[n] = bar with { Bounds = MixRect(_barOrigins[n], bar.Bounds, progress) };
            }
            foreach (var segment in _segments)
            {
                for (var n = 0; n < segment.Points.Length; n++)
                {
                    var point = segment.Target.Points[n];
                    var origin = segment.Origins[n];
                    segment.Points[n] = point with
                    {
                        Position = MixPoint(origin.Position, point.Position, progress),
                        Baseline = MixPoint(origin.Baseline, point.Baseline, progress)
                    };
                }
                segment.Rendered.LineGeometry = null;
                segment.Rendered.FillGeometry = null;
                segment.Rendered.ScatterMarkers = null;
                segment.Rendered.ScatterOperation = null;
            }
            for (var n = 0; n < Target.DataLabels.Count; n++)
            {
                var label = Target.DataLabels[n];
                var bounds = MixRect(_labelOrigins[n], label.Bounds, progress);
                Frame.DataLabels[n] = label with { Bounds = bounds };
                Frame.DataLabelBounds[(label.Series, label.Index)] = bounds;
            }
        }

        private sealed record AnimatedSegment(CartesianSegment Target, CartesianSegment Rendered,
            ChartRenderPoint[] Points, ChartRenderPoint[] Origins);
    }

    private readonly struct StableDataFrame : IDisposable
    {
        private readonly ChartBase _chart;

        public StableDataFrame(ChartBase chart)
        {
            _chart = chart;
            chart._animationSuppression++;
            chart.FinishDataAnimation();
        }

        public void Dispose()
        {
            _chart._animationSuppression--;
        }
    }
}
