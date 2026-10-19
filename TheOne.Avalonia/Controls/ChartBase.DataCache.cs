namespace TheOne.Avalonia.Controls;

public partial class ChartBase
{
    private CartesianDataCache? _cartesianData;
    private long _cartesianColumnUpdates;
    private long _cartesianFullBuilds;

    private readonly record struct DataExtents(double XMin, double XMax, double Min0, double Max0, double Min1, double Max1)
    {
        public static DataExtents Empty => new(double.PositiveInfinity, double.NegativeInfinity,
            double.PositiveInfinity, double.NegativeInfinity, double.PositiveInfinity, double.NegativeInfinity);

        public DataExtents Add(double x, double y, int axis) => new(Math.Min(XMin, x), Math.Max(XMax, x),
            axis == 0 ? Math.Min(Min0, y) : Min0, axis == 0 ? Math.Max(Max0, y) : Max0,
            axis == 1 ? Math.Min(Min1, y) : Min1, axis == 1 ? Math.Max(Max1, y) : Max1);

        public static DataExtents Merge(DataExtents a, DataExtents b) => new(Math.Min(a.XMin, b.XMin),
            Math.Max(a.XMax, b.XMax), Math.Min(a.Min0, b.Min0), Math.Max(a.Max0, b.Max0),
            Math.Min(a.Min1, b.Min1), Math.Max(a.Max1, b.Max1));
    }

    private sealed class PointMembership(double x, double y)
    {
        public double X = x, Y = y;
        public List<(int Series, int Index)> Locations { get; } = [];
    }

    private sealed class CartesianDataCache
    {
        private readonly ChartBase _chart;
        private readonly IList<ChartSeries> _source;
        private readonly DataExtents[] _tree;
        private readonly int _leaf;
        private readonly (ChartSeries Series, IList<ChartPoint>? Points, int Count, bool Visible, ChartYAxis Axis)[] _membership;
        public int[] Visible { get; }
        public int Count { get; }
        public Dictionary<ChartPoint, PointMembership> Members { get; } = new(ReferenceEqualityComparer.Instance);
        public Dictionary<(int Series, int Index), StackValue> Stacks { get; } = [];
        public Dictionary<(int Series, int Index), double> Percentages { get; } = [];
        public DataExtents Extents => _tree[1];

        public CartesianDataCache(ChartBase chart, IList<ChartSeries> source)
        {
            _chart = chart; _source = source;
            _membership = source.Select(series => (series, series.ItemsSource, series.ItemsSource?.Count ?? 0,
                series.IsVisible, series.YAxis)).ToArray();
            Visible = Enumerable.Range(0, source.Count)
                .Where(i => source[i].IsVisible && source[i].ItemsSource is { Count: > 0 }).ToArray();
            Count = Visible.Length == 0 ? 0 : Visible.Max(i => source[i].ItemsSource!.Count);
            _leaf = 1;
            while (_leaf < Count) _leaf *= 2;
            _tree = new DataExtents[_leaf * 2];
            Array.Fill(_tree, DataExtents.Empty);
            foreach (var s in Visible)
            {
                var points = source[s].ItemsSource!;
                for (var i = 0; i < points.Count; i++)
                {
                    var point = points[i];
                    if (!Members.TryGetValue(point, out var membership))
                        Members.Add(point, membership = new PointMembership(point.X, point.Y));
                    membership.Locations.Add((s, i));
                }
            }
            for (var i = 0; i < Count; i++) _tree[_leaf + i] = ComputeColumn(i);
            for (var i = _leaf - 1; i > 0; i--) _tree[i] = DataExtents.Merge(_tree[i * 2], _tree[i * 2 + 1]);
        }

        public void UpdateColumn(int index)
        {
            var slot = _leaf + index;
            _tree[slot] = ComputeColumn(index);
            while ((slot /= 2) > 0) _tree[slot] = DataExtents.Merge(_tree[slot * 2], _tree[slot * 2 + 1]);
        }

        public bool Matches(IList<ChartSeries>? source)
        {
            if (source is null || !ReferenceEquals(source, _source) || source.Count != _membership.Length) return false;
            for (var i = 0; i < _membership.Length; i++)
            {
                var old = _membership[i]; var current = source[i];
                if (!ReferenceEquals(old.Series, current) || !ReferenceEquals(old.Points, current.ItemsSource) ||
                    old.Count != (current.ItemsSource?.Count ?? 0) || old.Visible != current.IsVisible || old.Axis != current.YAxis) return false;
            }
            return true;
        }

        private DataExtents ComputeColumn(int index)
        {
            _chart._cartesianColumnUpdates++;
            Span<double> canonical = stackalloc double[] { double.NaN, double.NaN };
            Span<double> positive = stackalloc double[] { 0, 0 };
            Span<double> negative = stackalloc double[] { 0, 0 };
            Span<double> positiveMax = stackalloc double[] { 0, 0 };
            Span<double> negativeMax = stackalloc double[] { 0, 0 };
            Span<double> positiveTotal = stackalloc double[] { 0, 0 };
            Span<double> negativeTotal = stackalloc double[] { 0, 0 };
            if (_chart.IsStackedAreaCartesian)
                foreach (var s in Visible)
                {
                    var axis = (int)_source[s].YAxis;
                    if (_source[s].ItemsSource is { } points && index < points.Count &&
                        double.IsNaN(canonical[axis]) && double.IsFinite(points[index].X)) canonical[axis] = points[index].X;
                }
            if (_chart.IsPercentCartesian)
            {
                foreach (var s in Visible)
                {
                    var axis = (int)_source[s].YAxis;
                    if (_source[s].ItemsSource is not { } points || index >= points.Count) continue;
                    var point = points[index];
                    if (!_chart.IsValid(point) || (_chart.IsStackedAreaCartesian && point.X != canonical[axis])) continue;
                    var maxima = point.Y >= 0 ? positiveMax : negativeMax;
                    maxima[axis] = Math.Max(maxima[axis], Math.Abs(point.Y));
                }
                foreach (var s in Visible)
                {
                    var axis = (int)_source[s].YAxis;
                    if (_source[s].ItemsSource is not { } points || index >= points.Count) continue;
                    var point = points[index];
                    if (!_chart.IsValid(point) || point.Y == 0 || (_chart.IsStackedAreaCartesian && point.X != canonical[axis])) continue;
                    var maxima = point.Y > 0 ? positiveMax : negativeMax;
                    var totals = point.Y > 0 ? positiveTotal : negativeTotal;
                    totals[axis] += Math.Abs(point.Y) / maxima[axis];
                }
            }
            var extents = DataExtents.Empty;
            foreach (var s in Visible)
            {
                Stacks.Remove((s, index));
                Percentages.Remove((s, index));
                var axis = (int)_source[s].YAxis;
                if (_source[s].ItemsSource is not { } points || index >= points.Count) continue;
                var point = points[index];
                if (!_chart.IsValid(point) || (_chart.IsStackedAreaCartesian && point.X != canonical[axis])) continue;
                var value = point.Y;
                var top = value;
                if (_chart.IsStackedCartesian)
                {
                    if (_chart.IsPercentCartesian)
                    {
                        var maxima = value >= 0 ? positiveMax : negativeMax;
                        var totals = value >= 0 ? positiveTotal : negativeTotal;
                        value = value == 0 ? 0 : Math.CopySign(Math.Abs(value) / maxima[axis] / totals[axis], value);
                        Percentages[(s, index)] = value;
                    }
                    var basis = value >= 0 ? positive[axis] : negative[axis];
                    top = basis + value;
                    if (_chart.IsPercentCartesian) top = Math.Clamp(top, -1, 1);
                    else if (!double.IsFinite(top)) top = value >= 0 ? double.MaxValue : -double.MaxValue;
                    Stacks[(s, index)] = new StackValue(basis, top);
                    if (value >= 0) positive[axis] = top;
                    else negative[axis] = top;
                }
                extents = extents.Add(point.X, top, axis);
            }
            return extents;
        }
    }

    private (AxisRange[] Ranges, bool[] Valid) ResolveCartesianRanges(DataExtents extents)
    {
        var ranges = new AxisRange[2];
        var valid = new bool[2];
        for (var axis = 0; axis < 2; axis++)
        {
            var minimum = axis == 0 ? extents.Min0 : extents.Min1;
            var maximum = axis == 0 ? extents.Max0 : extents.Max1;
            if (!double.IsFinite(minimum)) continue;
            if (IsPercentCartesian) ranges[axis] = new AxisRange(minimum < 0 ? -1 : 0, maximum > 0 || minimum >= 0 ? 1 : 0);
            else
            {
                if (IsCategoricalCartesian || IsAreaCartesian)
                {
                    minimum = Math.Min(minimum, 0); maximum = Math.Max(maximum, 0);
                }
                ranges[axis] = ExpandRange(minimum, maximum);
                var low = axis == 0 ? YMinimum : SecondaryYMinimum;
                var high = axis == 0 ? YMaximum : SecondaryYMaximum;
                if (!double.IsNaN(low)) ranges[axis] = ranges[axis] with { Minimum = low };
                if (!double.IsNaN(high)) ranges[axis] = ranges[axis] with { Maximum = high };
            }
            valid[axis] = ranges[axis].Maximum > ranges[axis].Minimum;
        }
        return (ranges, valid);
    }
}
