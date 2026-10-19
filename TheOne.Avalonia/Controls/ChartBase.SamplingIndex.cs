namespace TheOne.Avalonia.Controls;

public partial class ChartBase
{
    private readonly record struct SampleExtrema(int TopMin, int TopMax, int BaseMin, int BaseMax)
    {
        public static readonly SampleExtrema Empty = new(-1, -1, -1, -1);
    }

    // X order, visible interval and bucket boundaries are immutable for a mapping-stable Y update.
    // Store original offsets, including equal extrema's earliest source offset; never reorder the data.
    private sealed class SegmentSampleIndex
    {
        public static readonly SegmentSampleIndex PassThrough = new();
        private readonly int _first, _last, _size;
        private readonly SampleBucket[] _buckets = [];
        private readonly SampleExtrema[] _tree = [];
        private readonly bool[] _dirty = [];

        private SegmentSampleIndex() { }

        public SegmentSampleIndex(IReadOnlyList<ChartRenderPoint> points, int first, int last, SampleBucket[] buckets)
        {
            _first = first; _last = last;
            _buckets = buckets.OrderBy(b => b.First).ToArray();
            _dirty = new bool[_buckets.Length];
            _size = 1;
            while (_size < last - first + 1) _size <<= 1;
            _tree = new SampleExtrema[_size * 2];
            Array.Fill(_tree, SampleExtrema.Empty);
            for (var offset = first; offset <= last; offset++)
                _tree[_size + offset - first] = new SampleExtrema(offset, offset, offset, offset);
            for (var node = _size - 1; node > 0; node--)
                _tree[node] = Merge(points, _tree[node * 2], _tree[node * 2 + 1]);
        }

        public IReadOnlyList<ChartRenderPoint> Update(IReadOnlyList<ChartRenderPoint> points,
            IReadOnlyList<ChartRenderPoint> previous, IReadOnlyList<int> offsets)
        {
            if (_tree.Length == 0) return points;
            var changed = false;
            // All original coordinates have been patched before updating the index. Repair every
            // changed leaf path before querying any bucket (shared models can affect multiple leaves).
            foreach (var offset in offsets)
            {
                if (offset < _first || offset > _last) continue;
                changed = true;
                for (var node = (_size + offset - _first) / 2; node > 0; node /= 2)
                    _tree[node] = Merge(points, _tree[node * 2], _tree[node * 2 + 1]);
                var low = 0; var high = _buckets.Length;
                while (low < high)
                {
                    var middle = low + (high - low) / 2;
                    if (_buckets[middle].Last < offset) low = middle + 1;
                    else high = middle;
                }
                _dirty[low] = true;
            }
            if (!changed) return previous;
            var retained = new SortedSet<int> { _first, _last };
            for (var index = 0; index < _buckets.Length; index++)
            {
                var bucket = _buckets[index];
                if (_dirty[index])
                {
                    bucket.Refresh(Query(points, bucket.First, bucket.Last));
                    _dirty[index] = false;
                }
                bucket.Retain(retained);
            }
            if (retained.Count >= points.Count) return points;
            return retained.Select(index => points[index]).ToArray();
        }

        private SampleExtrema Query(IReadOnlyList<ChartRenderPoint> points, int first, int last)
        {
            var result = SampleExtrema.Empty;
            for (int left = _size + first - _first, right = _size + last - _first; left <= right; left /= 2, right /= 2)
            {
                if ((left & 1) != 0) result = Merge(points, result, _tree[left++]);
                if ((right & 1) == 0) result = Merge(points, result, _tree[right--]);
            }
            return result;
        }

        private static SampleExtrema Merge(IReadOnlyList<ChartRenderPoint> points, SampleExtrema a, SampleExtrema b)
        {
            if (a.TopMin < 0) return b;
            if (b.TopMin < 0) return a;
            return new SampleExtrema(Choose(a.TopMin, b.TopMin, false, false), Choose(a.TopMax, b.TopMax, false, true),
                Choose(a.BaseMin, b.BaseMin, true, false), Choose(a.BaseMax, b.BaseMax, true, true));

            int Choose(int left, int right, bool baseline, bool maximum)
            {
                var l = baseline ? points[left].Baseline.Y : points[left].Position.Y;
                var r = baseline ? points[right].Baseline.Y : points[right].Position.Y;
                if (l == r) return Math.Min(left, right);
                return (maximum ? l > r : l < r) ? left : right;
            }
        }
    }
}
