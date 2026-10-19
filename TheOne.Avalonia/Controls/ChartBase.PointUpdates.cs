using Avalonia;

namespace TheOne.Avalonia.Controls;

public partial class ChartBase
{
    private bool TryUpdateCartesianPoint(ChartPoint point, bool yOnly)
    {
        if (IsRadial || _cartesianData is not { } data) return false;
        if (!data.Matches(SeriesSource))
        {
            _cartesianData = null; _percentages = [];
            return false;
        }
        if (!data.Members.TryGetValue(point, out var membership)) return true; // No visible owner.
        // Another collection listener can mutate a point before our structural notification arrives.
        foreach (var location in membership.Locations)
            if (!ReferenceEquals(SeriesSource![location.Series].ItemsSource![location.Index], point))
            {
                _cartesianData = null; _percentages = [];
                return false;
            }
        var oldX = membership.X; var oldY = membership.Y;
        membership.X = point.X; membership.Y = point.Y;
        var columns = membership.Locations.Select(location => location.Index).Distinct().ToArray();
        foreach (var index in columns) data.UpdateColumn(index);
        if (!yOnly || !double.IsFinite(oldX) || !double.IsFinite(oldY) || !IsValid(point) ||
            (IsStackedAreaCartesian && (oldY >= 0) != (point.Y >= 0)) ||
            (IsCategoricalCartesian && (oldY == 0) != (point.Y == 0)) ||
            _geometryDirty || _geometrySize != Bounds.Size || _cartesianFrame is not { } frame ||
            (CanAnimateData() && FrameMarks(frame) <= 6000)) return false;
        var (ranges, valid) = ResolveCartesianRanges(data.Extents);
        if (!ranges.SequenceEqual(frame.Ranges) || !valid.SequenceEqual(frame.ValidAxes)) return false;

        var affected = new HashSet<(int Series, int Index)>();
        foreach (var location in membership.Locations)
        {
            if (!IsStackedCartesian) affected.Add(location);
            else
                foreach (var s in data.Visible)
                    if (SeriesSource![s].YAxis == SeriesSource[location.Series].YAxis &&
                        SeriesSource[s].ItemsSource!.Count > location.Index && data.Stacks.ContainsKey((s, location.Index)))
                        affected.Add((s, location.Index));
        }
        var numeric = new List<((int Series, int Index) Key, CartesianSegment Segment, int Offset, ChartRenderPoint Point, Point Normal)>();
        var bars = new List<(int Slot, CartesianBar Bar, Point Normal)>();
        foreach (var key in affected)
        {
            var item = SeriesSource![key.Series];
            var model = item.ItemsSource![key.Index];
            var range = ranges[(int)item.YAxis];
            var values = IsStackedCartesian ? data.Stacks[key] : new StackValue(0, model.Y);
            if (frame.Categorical)
            {
                if (!frame.BarSlots.TryGetValue(key, out var slot)) return false;
                var old = frame.Bars[slot];
                var top = frame.Horizontal ? MapX(range.Ratio(values.Top), _cartesianPlot, frame.Viewport)
                    : MapY(range.Ratio(values.Top), _cartesianPlot, frame.Viewport);
                var basis = frame.Horizontal ? MapX(range.Ratio(values.Base), _cartesianPlot, frame.Viewport)
                    : MapY(range.Ratio(values.Base), _cartesianPlot, frame.Viewport);
                var bounds = frame.Horizontal
                    ? new Rect(old.Zero ? basis - .5 : Math.Min(top, basis), old.Bounds.Y, old.Zero ? 1 : Math.Abs(top - basis), old.Bounds.Height)
                    : new Rect(old.Bounds.X, old.Zero ? basis - .5 : Math.Min(top, basis), old.Bounds.Width, old.Zero ? 1 : Math.Abs(top - basis));
                bounds = bounds.Intersect(_cartesianPlot);
                if (bounds.Width <= 0 || bounds.Height <= 0) return false;
                var previous = _normalizedPositions[key];
                var middle = range.Ratio(values.Base / 2 + values.Top / 2);
                var normal = frame.Horizontal ? new Point(middle, previous.Y) : new Point(previous.X, middle);
                var label = old.Label is null ? null : CartesianDataLabel(key.Series, key.Index);
                bars.Add((slot, old with { Bounds = bounds, Label = label }, normal));
            }
            else
            {
                var slot = FindSegmentSlot(frame, key);
                if (slot.Segment?.Originals is null) return false;
                var normal = new Point(frame.XRange.Ratio(model.X), range.Ratio(values.Top));
                var position = new Point(MapX(normal.X, _cartesianPlot, frame.Viewport), MapY(normal.Y, _cartesianPlot, frame.Viewport));
                var prepared = new ChartRenderPoint(key.Index, position,
                    new Point(position.X, MapY(range.Ratio(values.Base), _cartesianPlot, frame.Viewport)));
                numeric.Add((key, slot.Segment, slot.Offset, prepared, normal));
            }
        }
        // Validate every affected mark before changing the live frame; clipping/topology failures fall back atomically.
        var segments = new Dictionary<CartesianSegment, List<int>>();
        foreach (var update in numeric)
        {
            _normalizedPositions[update.Key] = update.Normal;
            var previous = update.Segment.Originals![update.Offset];
            if (previous == update.Point) continue;
            update.Segment.Originals[update.Offset] = update.Point;
            UpdateNumericHit(update.Key, update.Point.Position);
            if (!segments.TryGetValue(update.Segment, out var offsets)) segments[update.Segment] = offsets = [];
            offsets.Add(update.Offset);
        }
        foreach (var (slot, bar, normal) in bars)
        {
            frame.Bars[slot] = bar;
            var key = (bar.Series, bar.Index);
            _normalizedPositions[key] = normal;
            _positions[key] = bar.Bounds.Center;
            _bars[key] = bar.Zero
                ? (frame.Horizontal ? new Rect(bar.Bounds.X - 3, bar.Bounds.Y, bar.Bounds.Width + 6, bar.Bounds.Height)
                    : new Rect(bar.Bounds.X, bar.Bounds.Y - 3, bar.Bounds.Width, bar.Bounds.Height + 6)).Intersect(_cartesianPlot)
                : bar.Bounds;
        }
        foreach (var series in frame.Series)
        foreach (var segment in series.Segments)
        {
            if (!segments.TryGetValue(segment, out var offsets)) continue;
            segment.Points = segment.BasePoints = segment.SampleIndex is { } index
                ? index.Update(segment.Originals!, segment.BasePoints!, offsets)
                : SampleSeriesPoints(series.Series, segment.Originals!, _cartesianPlot, out _);
            SelectSegmentSample(segment, series.Series, SelectedSeriesIndex, SelectedIndex, IsScatterCartesian);
            segment.LineGeometry = null; segment.FillGeometry = null;
            segment.ScatterMarkers = null;
        }
        RefreshCartesianLabels(frame);
        PublishFrameSampling(frame);
        InvalidateVisual();
        return true;
    }

    private void UpdateNumericHit((int Series, int Index) key, Point position)
    {
        var visible = _cartesianPlot.Contains(position);
        var hadPrevious = _positions.TryGetValue(key, out var previous);
        var previousBucket = HitBucket(previous);
        var nextBucket = HitBucket(position);
        if (hadPrevious && (!visible || previousBucket != nextBucket) && _numericHitBuckets.TryGetValue(previousBucket, out var old))
        {
            old.Remove(key);
            if (old.Count == 0) _numericHitBuckets.Remove(previousBucket);
        }
        if (!visible) _positions.Remove(key);
        else
        {
            _positions[key] = position;
            if (hadPrevious && previousBucket == nextBucket) return;
            if (!_numericHitBuckets.TryGetValue(nextBucket, out var hits)) _numericHitBuckets[nextBucket] = hits = [];
            var index = hits.FindIndex(hit => hit.Series > key.Series || (hit.Series == key.Series && hit.Index > key.Index));
            hits.Insert(index < 0 ? hits.Count : index, key);
        }
    }

    private static (CartesianSegment? Segment, int Offset) FindSegmentSlot(CartesianFrame frame, (int Series, int Index) key)
    {
        foreach (var series in frame.Series)
        {
            if (series.Series != key.Series) continue;
            var low = 0; var high = series.Segments.Count;
            while (low < high)
            {
                var middle = low + (high - low) / 2;
                var originals = series.Segments[middle].Originals!;
                if (originals[^1].Index < key.Index) low = middle + 1;
                else high = middle;
            }
            if (low < series.Segments.Count)
            {
                var segment = series.Segments[low];
                var offset = FindPoint(segment.Originals!, key.Index);
                if (offset >= 0) return (segment, offset);
            }
            break;
        }
        return (null, -1);
    }
}
