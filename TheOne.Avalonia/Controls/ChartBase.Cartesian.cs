using System.Globalization;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

public partial class ChartBase
{
    private readonly Dictionary<int, List<(int Series, int Index)>> _barCategories = [];
    private readonly Dictionary<(int Series, int Index), Rect> _bars = [];
    private readonly Dictionary<(int Series, int Index), Point> _normalizedPositions = [];
    private readonly Dictionary<(int X, int Y), List<(int Series, int Index)>> _numericHitBuckets = [];
    private readonly Dictionary<(int Series, int Index), double> _percentages = [];
    private CartesianFrame? _cartesianFrame;
    private Rect _cartesianPlot;

    // The owner rebuilds with a null context only when geometry is dirty, then paints this frame.
    private protected void DrawCartesian(DrawingContext? context, Rect bounds)
    {
        if (context is null)
        {
            BuildCartesianFrame(bounds);
            return;
        }

        if (_cartesianFrame is not { } frame) return;
        foreach (var line in frame.Lines)
        {
            if (line.Kind == 0 && !ShowGrid) continue;
            context.DrawLine(new Pen(line.Kind == 2 ? MutedBrush : GridBrush ?? MutedBrush), line.Start, line.End);
        }

        foreach (var label in frame.Labels)
            if (label.Clip is { } rect)
            {
                using var clip = context.PushClip(rect);
                context.DrawText(Text(label.Value, label.Title ? Foreground : MutedBrush), label.Position);
            }
            else
            {
                context.DrawText(Text(label.Value, label.Title ? Foreground : MutedBrush), label.Position);
            }

        using var plotClip = context.PushClip(CartesianDataClip);
        DrawAnimatedCartesianData(context, frame);
    }

    private void DrawCartesianData(DrawingContext context, CartesianFrame frame)
    {
        foreach (var bar in frame.Bars)
        {
            var brush = bar.Brush;
            var radius = frame.Stacked || bar.Zero ? 0 : Math.Min(3, Math.Min(bar.Bounds.Width, bar.Bounds.Height) / 4);
            context.DrawRectangle(brush, null, bar.Bounds, radius, radius);
            if (!ShowDataLabels || bar.Label is null || bar.Zero) continue;
            var text = Text(bar.Label, Background);
            if (text.Width + 8 <= bar.Bounds.Width && text.Height + 4 <= bar.Bounds.Height)
                context.DrawText(text,
                    new Point(bar.Bounds.Center.X - text.Width / 2, bar.Bounds.Center.Y - text.Height / 2));
        }

        foreach (var series in frame.Series)
        {
            var brush = series.Brush;
            var pen = new Pen(brush, StrokeThickness);
            foreach (var segment in series.Segments)
            {
                using (context.PushClip(_cartesianPlot))
                {
                    if (series.Area) DrawAreaBand(context, segment.Points, brush, pen, segment.Smooth);
                    else if (DrawsLineCartesian) DrawLineSegment(context, segment.Points, pen, segment.Smooth);
                }

                foreach (var point in segment.Points)
                {
                    if (!_cartesianPlot.Contains(point.Position)) continue;
                    if (IsScatterCartesian || (ShowMarkers && point.Index % series.MarkerStep == 0))
                        context.DrawEllipse(brush, new Pen(Background, SelectionThickness * 0.75), point.Position,
                            MarkerRadius, MarkerRadius);
                }
            }
        }

        foreach (var label in frame.DataLabels) context.DrawText(Text(label.Value, label.Brush), label.Bounds.TopLeft);
    }

    private protected void BuildCartesianFrame(Rect bounds)
    {
        _bars.Clear();
        _normalizedPositions.Clear();
        _percentages.Clear();
        _numericHitBuckets.Clear();
        _barCategories.Clear();
        ResetSharedIndex();
        _cartesianPlot = default;
        _cartesianFrame = null;
        if (SeriesSource is not { Count: > 0 } source)
        {
            SetSamplingDiagnostics(0, false);
            return;
        }

        var visible = Enumerable.Range(0, source.Count)
            .Where(i => source[i].IsVisible && source[i].ItemsSource is { Count: > 0 }).ToArray();
        if (visible.Length == 0)
        {
            SetSamplingDiagnostics(0, false);
            return;
        }

        var categorical = IsCategoricalCartesian;
        var horizontal = IsHorizontalCartesian;
        var stacked = IsStackedCartesian;
        var area = IsAreaCartesian;
        var count = visible.Max(i => source[i].ItemsSource!.Count);
        var stacks = stacked ? BuildStacks(visible, count) : null;
        var xMin = double.PositiveInfinity;
        var xMax = double.NegativeInfinity;
        var minima = new[] { double.PositiveInfinity, double.PositiveInfinity };
        var maxima = new[] { double.NegativeInfinity, double.NegativeInfinity };
        foreach (var s in visible)
        {
            var axis = (int)source[s].YAxis;
            var points = source[s].ItemsSource!;
            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                if (!IsValid(point) || (stacked && !stacks!.ContainsKey((s, i)))) continue;
                xMin = Math.Min(xMin, point.X);
                xMax = Math.Max(xMax, point.X);
                var valueTop = stacked ? stacks![(s, i)].Top : point.Y;
                minima[axis] = Math.Min(minima[axis], valueTop);
                maxima[axis] = Math.Max(maxima[axis], valueTop);
            }
        }

        var ranges = new AxisRange[2];
        var validAxes = new bool[2];
        for (var axis = 0; axis < 2; axis++)
        {
            if (!double.IsFinite(minima[axis])) continue;
            if (IsPercentCartesian)
            {
                ranges[axis] = new AxisRange(minima[axis] < 0 ? -1 : 0, maxima[axis] > 0 || minima[axis] >= 0 ? 1 : 0);
            }
            else
            {
                if (categorical || area)
                {
                    minima[axis] = Math.Min(minima[axis], 0);
                    maxima[axis] = Math.Max(maxima[axis], 0);
                }

                ranges[axis] = ExpandRange(minima[axis], maxima[axis]);
                var minimum = axis == 0 ? YMinimum : SecondaryYMinimum;
                var maximum = axis == 0 ? YMaximum : SecondaryYMaximum;
                if (!double.IsNaN(minimum)) ranges[axis] = ranges[axis] with { Minimum = minimum };
                if (!double.IsNaN(maximum)) ranges[axis] = ranges[axis] with { Maximum = maximum };
            }

            validAxes[axis] = ranges[axis].Maximum > ranges[axis].Minimum;
        }

        if (!validAxes.Any(valid => valid))
        {
            SetSamplingDiagnostics(0, false);
            return;
        }

        var xRange = ExpandRange(xMin, xMax);
        var viewport = GetCartesianViewport();
        var secondaryShown = validAxes[1] && ShowSecondaryAxis;
        // Margins use full extents and remain stable while the viewport changes.
        var left = AxisMargin(bounds.Width, validAxes[0] ? ranges[0] : ranges[1], ChartYAxis.Primary);
        var right = !horizontal && secondaryShown ? AxisMargin(bounds.Width, ranges[1], ChartYAxis.Secondary) : 12;
        var titleRow =
            !string.IsNullOrEmpty(YAxisTitle) || (secondaryShown && !string.IsNullOrEmpty(SecondaryYAxisTitle))
                ? FontSize + 14
                : 0;
        var top = 8 + titleRow + (horizontal && secondaryShown ? FontSize + 18 : 0);
        var axisTextHeight = source[visible[0]].ItemsSource!.Take(128)
            .Max(point => Text(point.Label ?? FormatValue(point.X), MutedBrush).Height);
        var bottom = Math.Max(FontSize + 30, axisTextHeight + 16) +
                     (horizontal && !string.IsNullOrEmpty(YAxisTitle) ? FontSize + 14 : 0);
        var plot = _cartesianPlot = new Rect(bounds.Left + left, bounds.Top + top,
            Math.Max(0, bounds.Width - left - right), Math.Max(0, bounds.Height - top - bottom));
        if (plot.Width < 12 || plot.Height < 12)
        {
            SetSamplingDiagnostics(0, false);
            return;
        }

        var frame = _cartesianFrame = new CartesianFrame(viewport, count, categorical, horizontal, stacked);
        frame.Identity = 14695981039346656037UL;
        foreach (var s in visible)
        {
            frame.Identity = (frame.Identity ^ (uint)RuntimeHelpers.GetHashCode(source[s])) * 1099511628211UL;
            foreach (var point in source[s].ItemsSource!)
                frame.Identity = (frame.Identity ^ (uint)RuntimeHelpers.GetHashCode(point)) * 1099511628211UL;
        }

        PrepareCartesianAxes(frame, bounds, xRange, ranges, validAxes, secondaryShown, visible[0]);
        var axisSlots = Enumerable.Range(0, 2).Where(axis => validAxes[axis]).ToArray();
        var rendered = 0;
        var originalNumeric = 0;
        var sampledNumeric = 0;
        for (var slot = 0; slot < visible.Length; slot++)
        {
            var s = visible[slot];
            var axis = (int)source[s].YAxis;
            if (!validAxes[axis]) continue;
            var points = source[s].ItemsSource!;
            var range = ranges[axis];
            var markerStep = Math.Max(1,
                (int)Math.Ceiling(points.Count * viewport.Width / Math.Max(1, plot.Width / 14)));
            var labelStep = Math.Max(1,
                (int)Math.Ceiling(points.Count * (horizontal ? viewport.Height : viewport.Width) /
                                  Math.Max(1, (horizontal ? plot.Height : plot.Width) / 70)));
            var seriesFrame = new CartesianSeriesFrame(s, area, markerStep, labelStep, SeriesBrush(s), [], []);
            var segment = new List<ChartRenderPoint>();
            bool? previousSign = null;

            void FinishSegment()
            {
                if (segment.Count == 0) return;
                originalNumeric += segment.Count;
                var sampled = SampleSeriesPoints(s, segment, plot);
                sampledNumeric += sampled.Count;
                rendered += sampled.Count;
                if (sampled.Count > 0)
                {
                    var ascending = true;
                    var descending = true;
                    for (var n = 1; n < segment.Count; n++)
                    {
                        ascending &= points[segment[n].Index].X > points[segment[n - 1].Index].X;
                        descending &= points[segment[n].Index].X < points[segment[n - 1].Index].X;
                    }

                    seriesFrame.Segments.Add(new CartesianSegment(sampled, ascending || descending));
                    if (ShowDataLabels)
                        foreach (var p in sampled)
                            if (p.Index % labelStep == 0)
                                seriesFrame.Labels[p.Index] = CartesianDataLabel(s, p.Index);
                }

                segment = [];
                previousSign = null;
            }

            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                if (!IsValid(point) || (stacked && !stacks!.ContainsKey((s, i))))
                {
                    FinishSegment();
                    continue;
                }

                var values = stacked ? stacks![(s, i)] : new StackValue(0, point.Y);
                var topRatio = range.Ratio(values.Top);
                var baseRatio = range.Ratio(values.Base);
                AddSharedIndex(s, i, point.X, MapX(xRange.Ratio(point.X), plot, viewport), categorical);
                if (categorical)
                {
                    var slots = stacked ? axisSlots.Length : visible.Length;
                    var categorySlot = stacked ? Array.IndexOf(axisSlots, axis) : slot;
                    var category = (i + 0.12 + 0.76 * (categorySlot + 0.5) / slots) / count;
                    var midpoint = values.Base / 2 + values.Top / 2;
                    _normalizedPositions[(s, i)] = horizontal
                        ? new Point(range.Ratio(midpoint), category)
                        : new Point(category, range.Ratio(midpoint));
                    var start = (i + 0.12 + 0.76 * categorySlot / slots) / count;
                    var end = start + 0.76 / slots / count;
                    var low = horizontal ? MapYCategory(start, plot, viewport) : MapX(start, plot, viewport);
                    var high = horizontal ? MapYCategory(end, plot, viewport) : MapX(end, plot, viewport);
                    var gap = Math.Min(2, Math.Max(0, high - low) * 0.12);
                    var topValue = horizontal ? MapX(topRatio, plot, viewport) : MapY(topRatio, plot, viewport);
                    var baseline = horizontal ? MapX(baseRatio, plot, viewport) : MapY(baseRatio, plot, viewport);
                    var zero = point.Y == 0;
                    var bar = horizontal
                        ? new Rect(zero ? baseline - 0.5 : Math.Min(topValue, baseline), low,
                            zero ? 1 : Math.Abs(topValue - baseline), Math.Max(0, high - low - gap))
                        : new Rect(low, zero ? baseline - 0.5 : Math.Min(topValue, baseline),
                            Math.Max(0, high - low - gap), zero ? 1 : Math.Abs(topValue - baseline));
                    bar = bar.Intersect(plot);
                    if (bar.Width <= 0 || bar.Height <= 0) continue;
                    _positions[(s, i)] = bar.Center;
                    _bars[(s, i)] =
                        zero
                            ? (horizontal
                                ? new Rect(bar.X - 3, bar.Y, bar.Width + 6, bar.Height)
                                : new Rect(bar.X, bar.Y - 3, bar.Width, bar.Height + 6)).Intersect(plot)
                            : bar;
                    if (!_barCategories.TryGetValue(i, out var categoryBars)) _barCategories[i] = categoryBars = [];
                    categoryBars.Add((s, i));
                    frame.Bars.Add(new CartesianBar(s, i, bar, zero,
                        ShowDataLabels && i % labelStep == 0 ? CartesianDataLabel(s, i) : null, SeriesBrush(s)));
                    rendered++;
                }
                else
                {
                    var x = xRange.Ratio(point.X);
                    _normalizedPositions[(s, i)] = new Point(x, topRatio);
                    var position = new Point(MapX(x, plot, viewport), MapY(topRatio, plot, viewport));
                    if (IsStackedAreaCartesian && previousSign is { } previous && previous != point.Y >= 0)
                        FinishSegment();
                    previousSign = point.Y >= 0;
                    segment.Add(new ChartRenderPoint(i, position,
                        new Point(position.X, MapY(baseRatio, plot, viewport))));
                    if (!plot.Contains(position)) continue;
                    _positions[(s, i)] = position;
                    var bucket = HitBucket(position);
                    if (!_numericHitBuckets.TryGetValue(bucket, out var hits)) _numericHitBuckets[bucket] = hits = [];
                    hits.Add((s, i));
                }
            }

            FinishSegment();
            if (!categorical) frame.Series.Add(seriesFrame);
        }

        FinishSharedIndex();
        PrepareCartesianDataLabels(frame);
        SetSamplingDiagnostics(rendered, sampledNumeric < originalNumeric);
    }

    private Dictionary<(int Series, int Index), StackValue> BuildStacks(int[] visible, int count)
    {
        var result = new Dictionary<(int Series, int Index), StackValue>();
        var positive = new[] { new double[count], new double[count] };
        var negative = new[] { new double[count], new double[count] };
        var canonical = new[]
            { Enumerable.Repeat(double.NaN, count).ToArray(), Enumerable.Repeat(double.NaN, count).ToArray() };
        if (IsStackedAreaCartesian)
            foreach (var s in visible)
            {
                var axis = (int)SeriesSource![s].YAxis;
                var points = SeriesSource[s].ItemsSource!;
                for (var i = 0; i < points.Count; i++)
                    if (double.IsNaN(canonical[axis][i]) && double.IsFinite(points[i].X))
                        canonical[axis][i] = points[i].X;
            }

        bool Valid(int s, int i, ChartPoint point)
        {
            return IsValid(point) && (!IsStackedAreaCartesian || point.X == canonical[(int)SeriesSource![s].YAxis][i]);
        }

        var positiveMax = new[] { new double[count], new double[count] };
        var negativeMax = new[] { new double[count], new double[count] };
        var positiveTotal = new[] { new double[count], new double[count] };
        var negativeTotal = new[] { new double[count], new double[count] };
        if (IsPercentCartesian)
        {
            // Maximum-first sums normalize several MaxValue contributions without overflow.
            foreach (var s in visible)
            {
                var axis = (int)SeriesSource![s].YAxis;
                var points = SeriesSource[s].ItemsSource!;
                for (var i = 0; i < points.Count; i++)
                    if (Valid(s, i, points[i]))
                    {
                        var maxima = points[i].Y >= 0 ? positiveMax : negativeMax;
                        maxima[axis][i] = Math.Max(maxima[axis][i], Math.Abs(points[i].Y));
                    }
            }

            foreach (var s in visible)
            {
                var axis = (int)SeriesSource![s].YAxis;
                var points = SeriesSource[s].ItemsSource!;
                for (var i = 0; i < points.Count; i++)
                    if (Valid(s, i, points[i]) && points[i].Y != 0)
                    {
                        var maxima = points[i].Y > 0 ? positiveMax : negativeMax;
                        var totals = points[i].Y > 0 ? positiveTotal : negativeTotal;
                        totals[axis][i] += Math.Abs(points[i].Y) / maxima[axis][i];
                    }
            }
        }

        foreach (var s in visible)
        {
            var axis = (int)SeriesSource![s].YAxis;
            var points = SeriesSource[s].ItemsSource!;
            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                if (!Valid(s, i, point)) continue;
                var value = point.Y;
                if (IsPercentCartesian)
                {
                    var maxima = value >= 0 ? positiveMax : negativeMax;
                    var totals = value >= 0 ? positiveTotal : negativeTotal;
                    value = value == 0 ? 0 : Math.CopySign(Math.Abs(value) / maxima[axis][i] / totals[axis][i], value);
                    _percentages[(s, i)] = value;
                }

                var basis = value >= 0 ? positive[axis][i] : negative[axis][i];
                var sum = basis + value;
                if (IsPercentCartesian) sum = Math.Clamp(sum, -1, 1);
                else if (!double.IsFinite(sum)) sum = value >= 0 ? double.MaxValue : -double.MaxValue;
                result[(s, i)] = new StackValue(basis, sum);
                if (value >= 0) positive[axis][i] = sum;
                else negative[axis][i] = sum;
            }
        }

        return result;
    }

    private bool HasCanonicalStackX(int seriesIndex, int pointIndex)
    {
        if (SeriesSource is not { } source || seriesIndex < 0 || seriesIndex >= source.Count ||
            source[seriesIndex].ItemsSource is not { } target || pointIndex < 0 ||
            pointIndex >= target.Count) return false;
        foreach (var series in source)
            if (series.IsVisible && series.YAxis == source[seriesIndex].YAxis && series.ItemsSource is { } points &&
                pointIndex < points.Count && double.IsFinite(points[pointIndex].X))
                return target[pointIndex].X == points[pointIndex].X;
        return false;
    }

    private double AxisMargin(double width, AxisRange range, ChartYAxis axis)
    {
        // Reserve the widest readable tick across supported densities; zoom keeps margins stable.
        var format = axis == ChartYAxis.Primary ? ValueFormat : SecondaryValueFormat;
        var unit = axis == ChartYAxis.Primary ? Unit : SecondaryUnit;
        var labelWidth = Enumerable.Range(2, 4).SelectMany(divisions => AxisTicks(range, divisions))
            .Max(tick =>
                Text(IsPercentCartesian
                    ? CartesianAxisText(tick.Value, axis)
                    : TickText(tick.Value, tick.Step, format) + unit).Width);
        return Math.Min(width * 0.24, Math.Max(36, labelWidth + 12));
    }

    private string CartesianAxisText(double value, ChartYAxis axis)
    {
        if (IsPercentCartesian) return value.ToString("P0", CultureInfo.CurrentCulture);
        var format = axis == ChartYAxis.Primary ? ValueFormat : SecondaryValueFormat;
        var unit = axis == ChartYAxis.Primary ? Unit : SecondaryUnit;
        return CartesianFormat(value, format) + unit;
    }

    private string CartesianDataLabel(int seriesIndex, int index)
    {
        if (IsPercentCartesian && _percentages.TryGetValue((seriesIndex, index), out var percentage))
            return percentage.ToString("P1", CultureInfo.CurrentCulture);
        var series = SeriesSource![seriesIndex];
        var secondary = series.YAxis == ChartYAxis.Secondary;
        return CartesianFormat(series.ItemsSource![index].Y,
                   series.ValueFormat ?? (secondary ? SecondaryValueFormat : ValueFormat)) +
               (series.Unit ?? (secondary ? SecondaryUnit : Unit));
    }

    private static string CartesianFormat(double value, string? format)
    {
        try
        {
            return value.ToString(format ?? "0.##", CultureInfo.CurrentCulture);
        }
        catch (FormatException)
        {
            return value.ToString("0.##", CultureInfo.CurrentCulture);
        }
    }

    private static AxisRange ExpandRange(double minimum, double maximum)
    {
        if (minimum != maximum) return new AxisRange(minimum, maximum);
        var padding = Math.Max(1, Math.Abs(minimum) * 0.1);
        var low = minimum - padding;
        var high = maximum + padding;
        return new AxisRange(double.IsFinite(low) ? low : minimum, double.IsFinite(high) ? high : maximum);
    }

    private static double ViewRatio(double normalized, double start, double length)
    {
        return Math.Clamp((normalized - start) / length, -1e6, 1e6);
    }

    private static double MapX(double normalized, Rect plot, Rect viewport)
    {
        return plot.Left + ViewRatio(normalized, viewport.Left, viewport.Width) * plot.Width;
    }

    private static double MapY(double normalized, Rect plot, Rect viewport)
    {
        return plot.Bottom - ViewRatio(normalized, viewport.Top, viewport.Height) * plot.Height;
    }

    private static double MapYCategory(double normalized, Rect plot, Rect viewport)
    {
        return plot.Top + ViewRatio(normalized, viewport.Top, viewport.Height) * plot.Height;
    }

    private void DrawLineSegment(DrawingContext context, IReadOnlyList<ChartRenderPoint> points, Pen pen, bool smooth)
    {
        if (points.Count < 2) return;
        var geometry = new StreamGeometry();
        using (var path = geometry.Open())
        {
            path.BeginFigure(points[0].Position, false);
            AppendCurve(path, points, smooth);
            path.EndFigure(false);
        }

        context.DrawGeometry(null, pen, geometry);
    }

    private void DrawAreaBand(DrawingContext context, IReadOnlyList<ChartRenderPoint> points, IBrush brush, Pen pen,
        bool smooth)
    {
        if (points.Count < 2) return;
        var fill = new StreamGeometry();
        using (var path = fill.Open())
        {
            path.BeginFigure(points[0].Baseline);
            path.LineTo(points[0].Position);
            AppendCurve(path, points, smooth);
            path.LineTo(points[^1].Baseline);
            AppendCurve(path, points, smooth, true, true);
            path.EndFigure(true);
        }

        using (context.PushOpacity(IsStackedAreaCartesian ? 0.28 : 0.16))
        {
            context.DrawGeometry(brush, null, fill);
        }

        DrawLineSegment(context, points, pen, smooth);
    }

    private void PrepareCartesianAxes(CartesianFrame frame, Rect bounds, AxisRange xRange, AxisRange[] ranges,
        bool[] valid, bool secondaryShown, int firstSeries)
    {
        var plot = _cartesianPlot;
        var viewport = frame.Viewport;

        void Label(string value, Point position, bool centered = false, Rect? clip = null, bool title = false)
        {
            var text = Text(value, title ? Foreground : MutedBrush);
            frame.Labels.Add(new CartesianLabel(value, centered ? position - new Vector(text.Width / 2, 0) : position,
                clip, title));
        }

        var tickCount = Math.Clamp((int)((frame.Horizontal ? plot.Width : plot.Height) / (FontSize + 26)), 2, 5);
        for (var axis = 0; axis < 2; axis++)
        {
            if (!valid[axis] || (axis == 1 && !secondaryShown)) continue;
            var displayed = ranges[axis].Slice(frame.Horizontal ? viewport.Left : viewport.Top,
                frame.Horizontal ? viewport.Right : viewport.Bottom);
            foreach (var tick in AxisTicks(displayed, tickCount))
            {
                var fraction = displayed.Ratio(tick.Value);
                var format = axis == 0 ? ValueFormat : SecondaryValueFormat;
                var value = IsPercentCartesian
                    ? CartesianAxisText(tick.Value, (ChartYAxis)axis)
                    : TickText(tick.Value, tick.Step, format) + (axis == 0 ? Unit : SecondaryUnit);
                var text = Text(value, MutedBrush);
                if (frame.Horizontal)
                {
                    var x = plot.Left + fraction * plot.Width;
                    if (axis == 0 || !valid[0])
                        frame.Lines.Add(new CartesianLine(new Point(x, plot.Top), new Point(x, plot.Bottom), 0));
                    Label(value, new Point(x, axis == 0 ? plot.Bottom + 8 : plot.Top - text.Height - 8), true);
                }
                else
                {
                    var y = plot.Bottom - fraction * plot.Height;
                    if (axis == 0 || !valid[0])
                        frame.Lines.Add(new CartesianLine(new Point(plot.Left, y), new Point(plot.Right, y), 0));
                    var x = axis == 0 ? plot.Left - text.Width - 10 : plot.Right + 10;
                    var clip = axis == 0
                        ? new Rect(bounds.Left, y - text.Height / 2, Math.Max(0, plot.Left - bounds.Left - 6),
                            text.Height)
                        : new Rect(plot.Right + 6, y - text.Height / 2, Math.Max(0, bounds.Right - plot.Right - 6),
                            text.Height);
                    Label(value, new Point(x, y - text.Height / 2), clip: clip);
                }
            }

            var title = axis == 0 ? YAxisTitle : SecondaryYAxisTitle;
            if (!string.IsNullOrEmpty(title))
            {
                var text = Text(title, Foreground);
                Label(title, new Point(axis == 0 ? plot.Left : Math.Max(plot.Left, plot.Right - text.Width),
                    frame.Horizontal && axis == 0
                        ? plot.Bottom + FontSize + 18
                        : plot.Top - (frame.Horizontal ? FontSize * 2 + 24 : FontSize + 14)), title: true);
            }

            var zero = ViewRatio(ranges[axis].Ratio(0), frame.Horizontal ? viewport.Left : viewport.Top,
                frame.Horizontal ? viewport.Width : viewport.Height);
            if (zero > 0 && zero < 1 && (axis == 0 || !valid[0]))
                frame.Lines.Add(frame.Horizontal
                    ? new CartesianLine(new Point(plot.Left + zero * plot.Width, plot.Top),
                        new Point(plot.Left + zero * plot.Width, plot.Bottom), 2)
                    : new CartesianLine(new Point(plot.Left, plot.Bottom - zero * plot.Height),
                        new Point(plot.Right, plot.Bottom - zero * plot.Height), 2));
        }

        frame.Lines.Add(new CartesianLine(plot.BottomLeft, plot.BottomRight, 1));
        frame.Lines.Add(new CartesianLine(plot.TopLeft, plot.BottomLeft, 1));
        if (secondaryShown)
            frame.Lines.Add(new CartesianLine(frame.Horizontal ? plot.TopLeft : plot.TopRight,
                frame.Horizontal ? plot.TopRight : plot.BottomRight, 1));
        var categories = SeriesSource![firstSeries].ItemsSource!;
        var widestCategory = categories.Take(128)
            .Max(point => Text(point.Label ?? FormatValue(point.X), MutedBrush).Width);
        var capacity = Math.Max(2,
            (int)((frame.Horizontal ? plot.Height : plot.Width) /
                  (frame.Horizontal ? FontSize + 8 : Math.Max(FontSize + 8, widestCategory + 6))));
        var labels = new List<(double Fraction, string Text)>();
        if (frame.Categorical)
        {
            var start = frame.Horizontal ? viewport.Top : viewport.Left;
            var end = frame.Horizontal ? viewport.Bottom : viewport.Right;
            var first = Math.Clamp((int)Math.Floor(start * frame.Count), 0, frame.Count - 1);
            var last = Math.Clamp((int)Math.Ceiling(end * frame.Count) - 1, first, frame.Count - 1);
            var labelCount = Math.Min(last - first + 1, capacity);
            for (var label = 0; label < labelCount; label++)
            {
                var index = labelCount == 1
                    ? first
                    : first + (int)Math.Round(label * (last - first) / (double)(labelCount - 1));
                var fraction = ViewRatio((index + 0.5) / frame.Count, start, end - start);
                if (fraction < 0 || fraction > 1) continue;
                labels.Add((fraction,
                    index < categories.Count
                        ? categories[index].Label ?? FormatValue(categories[index].X)
                        : (index + 1).ToString(CultureInfo.CurrentCulture)));
            }
        }
        else
        {
            var named = categories.Where(point => IsValid(point) && point.Label is not null)
                .Select(point => (Fraction: ViewRatio(xRange.Ratio(point.X), viewport.Left, viewport.Width),
                    Text: point.Label!))
                .Where(label => label.Fraction >= 0 && label.Fraction <= 1).ToArray();
            if (named.Length > 0)
            {
                var labelCount = Math.Min(named.Length, capacity);
                for (var label = 0; label < labelCount; label++)
                    labels.Add(named[
                        labelCount == 1 ? 0 : (int)Math.Round(label * (named.Length - 1d) / (labelCount - 1))]);
            }
            else
            {
                var displayed = xRange.Slice(viewport.Left, viewport.Right);
                for (var label = 0; label < capacity; label++)
                {
                    var fraction = label / (double)(capacity - 1);
                    labels.Add((fraction, FormatValue(displayed.At(fraction))));
                }
            }
        }

        var previousLabelRight = double.NegativeInfinity;
        foreach (var label in labels.OrderBy(label => label.Fraction))
        {
            var text = Text(label.Text, MutedBrush);
            if (frame.Horizontal)
            {
                var y = plot.Top + label.Fraction * plot.Height;
                Label(label.Text, new Point(Math.Max(bounds.Left, plot.Left - text.Width - 10), y - text.Height / 2),
                    clip: new Rect(bounds.Left, y - text.Height / 2, Math.Max(0, plot.Left - bounds.Left - 8),
                        text.Height));
            }
            else
            {
                var x = plot.Left + label.Fraction * plot.Width;
                var labelLeft = Math.Clamp(x - text.Width / 2, bounds.Left,
                    Math.Max(bounds.Left, bounds.Right - text.Width));
                if (labelLeft < previousLabelRight + 6) continue;
                previousLabelRight = labelLeft + text.Width;
                Label(label.Text, new Point(labelLeft, plot.Bottom + 8),
                    clip: new Rect(bounds.Left, plot.Bottom + 6, bounds.Width,
                        Math.Max(0, bounds.Bottom - plot.Bottom - 6)));
            }
        }
    }

    private (int X, int Y) HitBucket(Point point)
    {
        return ((int)Math.Floor((point.X - _cartesianPlot.Left) / 16),
            (int)Math.Floor((point.Y - _cartesianPlot.Top) / 16));
    }

    private (int Series, int Index)? HitTestCartesian(Point pointer)
    {
        if (_positions.Count == 0 || !double.IsFinite(pointer.X) || !double.IsFinite(pointer.Y) ||
            !new Rect(Bounds.Size).Contains(pointer) || !CartesianDataClip.Contains(pointer) ||
            _cartesianFrame is not { } frame) return null;
        if (frame.Categorical)
        {
            var category = frame.Horizontal
                ? frame.Viewport.Top + (pointer.Y - _cartesianPlot.Top) / _cartesianPlot.Height * frame.Viewport.Height
                : frame.Viewport.Left + (pointer.X - _cartesianPlot.Left) / _cartesianPlot.Width * frame.Viewport.Width;
            var index = Math.Clamp((int)Math.Floor(category * frame.Count), 0, frame.Count - 1);
            if (_barCategories.TryGetValue(index, out var bars))
                foreach (var key in bars)
                    if (_bars[key].Contains(pointer))
                        return key;
            return null;
        }

        var bucket = HitBucket(pointer);
        (int Series, int Index)? nearest = null;
        var distance = 14d * 14;
        for (var x = bucket.X - 1; x <= bucket.X + 1; x++)
        for (var y = bucket.Y - 1; y <= bucket.Y + 1; y++)
            if (_numericHitBuckets.TryGetValue((x, y), out var points))
                foreach (var key in points)
                {
                    var delta = _positions[key] - pointer;
                    var squared = delta.X * delta.X + delta.Y * delta.Y;
                    if (squared <= distance)
                    {
                        distance = squared;
                        nearest = key;
                    }
                }

        return nearest;
    }

    private readonly record struct AxisRange(double Minimum, double Maximum)
    {
        public double Ratio(double value)
        {
            var span = Maximum - Minimum;
            var ratio = double.IsFinite(span)
                ? (value - Minimum) / span
                : ScaledRatio(value);
            return double.IsNaN(ratio) ? 0 : Math.Clamp(ratio, -1e12, 1e12);
        }

        private double ScaledRatio(double value)
        {
            var scale = Math.Max(Math.Abs(Minimum), Math.Abs(Maximum));
            return (value / scale - Minimum / scale) / (Maximum / scale - Minimum / scale);
        }

        public double At(double fraction)
        {
            return fraction <= 0 ? Minimum : fraction >= 1 ? Maximum : (1 - fraction) * Minimum + fraction * Maximum;
        }

        public AxisRange Slice(double start, double end)
        {
            return new AxisRange(At(start), At(end));
        }
    }

    private readonly record struct StackValue(double Base, double Top);

    private readonly record struct CartesianBar(
        int Series,
        int Index,
        Rect Bounds,
        bool Zero,
        string? Label,
        IBrush Brush);

    private readonly record struct CartesianLabel(string Value, Point Position, Rect? Clip, bool Title);

    private readonly record struct CartesianLine(Point Start, Point End, int Kind);

    private sealed record CartesianSegment(IReadOnlyList<ChartRenderPoint> Points, bool Smooth);

    private sealed record CartesianSeriesFrame(
        int Series,
        bool Area,
        int MarkerStep,
        int LabelStep,
        IBrush Brush,
        Dictionary<int, string> Labels,
        List<CartesianSegment> Segments);

    private sealed class CartesianFrame(Rect viewport, int count, bool categorical, bool horizontal, bool stacked)
    {
        public Rect Viewport { get; } = viewport;
        public int Count { get; } = count;
        public bool Categorical { get; } = categorical;
        public bool Horizontal { get; } = horizontal;
        public bool Stacked { get; } = stacked;
        public ulong Identity { get; set; }
        public List<CartesianBar> Bars { get; } = [];
        public List<CartesianSeriesFrame> Series { get; } = [];
        public List<CartesianLabel> Labels { get; } = [];
        public List<CartesianLine> Lines { get; } = [];
        public List<PreparedDataLabel> DataLabels { get; } = [];
        public Dictionary<(int Series, int Index), Rect> DataLabelBounds { get; } = [];
    }
}