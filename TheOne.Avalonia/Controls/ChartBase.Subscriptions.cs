using System.Collections;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;

namespace TheOne.Avalonia.Controls;

public abstract partial class ChartBase
{
    private CollectionSubscription<ChartSeries>? _seriesSubscription;
    private readonly Dictionary<ChartSeries, IList<ChartPoint>?> _seriesCollections = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<IList<ChartPoint>, CollectionSubscription<ChartPoint>> _pointCollections = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<AvaloniaObject, int> _dataObjects = new(ReferenceEqualityComparer.Instance);

    private void ReconcileSubscriptions()
    {
        if (!_observing || IsGauge) return;
        var existing = _pointCollections.Values.ToArray();
        _seriesSubscription ??= new CollectionSubscription<ChartSeries>(AddSeries, RemoveSeries, DataChanged);
        _seriesSubscription.SetSource(SeriesSource);
        foreach (var series in _seriesCollections.Keys.ToArray())
            ReplacePointSource(series);
        foreach (var subscription in existing)
            if (subscription.Users > 0) subscription.Refresh();
    }

    private void AddSeries(ChartSeries series)
    {
        AddDataObject(series);
        _seriesCollections.Add(series, series.ItemsSource);
        AddPointSource(series.ItemsSource);
    }

    private void RemoveSeries(ChartSeries series)
    {
        var source = _seriesCollections[series];
        _seriesCollections.Remove(series);
        RemovePointSource(source);
        RemoveDataObject(series);
    }

    private void AddPointSource(IList<ChartPoint>? source)
    {
        if (source is null) return;
        if (_pointCollections.TryGetValue(source, out var subscription))
        {
            subscription.Users++;
            return;
        }
        subscription = new CollectionSubscription<ChartPoint>(AddDataObject, RemoveDataObject, DataChanged) { Users = 1 };
        _pointCollections.Add(source, subscription);
        subscription.SetSource(source);
    }

    private void RemovePointSource(IList<ChartPoint>? source)
    {
        if (source is null || !_pointCollections.TryGetValue(source, out var subscription)) return;
        if (--subscription.Users > 0) return;
        _pointCollections.Remove(source);
        subscription.Dispose();
    }

    private void ReplacePointSource(ChartSeries series)
    {
        var previous = _seriesCollections[series];
        var next = series.ItemsSource;
        if (ReferenceEquals(previous, next)) return;
        // Acquire first so points shared with the previous source keep their subscriptions.
        AddPointSource(next);
        _seriesCollections[series] = next;
        RemovePointSource(previous);
    }

    private void AddDataObject(AvaloniaObject source)
    {
        if (_dataObjects.TryGetValue(source, out var count)) _dataObjects[source] = count + 1;
        else
        {
            _dataObjects.Add(source, 1);
            source.PropertyChanged += OnDataObjectChanged;
            OnChartDataObjectSubscribed(source);
        }
    }

    private void RemoveDataObject(AvaloniaObject source)
    {
        var count = _dataObjects[source];
        if (count > 1) _dataObjects[source] = count - 1;
        else
        {
            _dataObjects.Remove(source);
            source.PropertyChanged -= OnDataObjectChanged;
            OnChartDataObjectUnsubscribed(source);
        }
    }

    private void OnDataObjectChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (!_observing || sender is not AvaloniaObject source || !_dataObjects.ContainsKey(source)) return;
        if (!ShouldProcessChartDataChange(args.Property)) return;
        if (source is ChartSeries && (args.Property == ChartSeries.IsVisibleProperty || args.Property == ChartSeries.YAxisProperty))
            _cartesianData = null;
        if (args.Property == ChartSeries.ItemsSourceProperty && source is ChartSeries series)
        {
            ReplacePointSource(series);
            DataChanged();
            return;
        }
        if (UsesCustomChartGeometry && source is ChartPoint)
        {
            MarkDataUpdate();
        }
        else if (source is ChartPoint point && (args.Property == ChartPoint.XProperty || args.Property == ChartPoint.YProperty))
        {
            if (!TryUpdateCartesianPoint(point, args.Property == ChartPoint.YProperty)) MarkDataUpdate();
        }
        else if (args.Property == ChartSeries.IsVisibleProperty) MarkDataUpdate();
        else
        {
            _animateDataPending = false;
            FinishDataAnimation();
            MarkGeometryDirty();
        }
        CancelLegendPress();
        ClearPointerHover();
        UpdateSelection();
        InvalidateVisual();
    }

    private void DataChanged()
    {
        _cartesianData = null;
        _percentages = [];
        CancelPan();
        CancelLegendPress();
        ClearPointerHover();
        MarkDataUpdate();
        _hover = null;
        ToolTip.SetTip(this, null);
        UpdateSelection();
        InvalidateVisual();
    }

    private void Unsubscribe()
    {
        _cartesianData = null;
        _percentages = [];
        _seriesSubscription?.Dispose();
        _seriesSubscription = null;
    }

    private sealed class CollectionSubscription<T>(Action<T> added, Action<T> removed, Action changed) : IDisposable where T : class
    {
        private IList<T>? _source;
        private Dictionary<T, int> _members = new(ReferenceEqualityComparer.Instance);
        private bool _disposed;
        public int Users { get; set; }

        public void SetSource(IList<T>? source)
        {
            if (!ReferenceEquals(_source, source))
            {
                if (_source is INotifyCollectionChanged old) old.CollectionChanged -= OnCollectionChanged;
                _source = source;
                if (source is INotifyCollectionChanged next) next.CollectionChanged += OnCollectionChanged;
            }
            Refresh();
        }

        public void Refresh()
        {
            var next = new Dictionary<T, int>(ReferenceEqualityComparer.Instance);
            if (_source is not null)
                foreach (var item in _source) next[item] = next.GetValueOrDefault(item) + 1;
            foreach (var item in next.Keys)
                if (!_members.ContainsKey(item)) added(item);
            foreach (var item in _members.Keys)
                if (!next.ContainsKey(item)) removed(item);
            _members = next;
        }

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
        {
            if (_disposed) return;
            if (args.Action == NotifyCollectionChangedAction.Reset) Refresh();
            else if (args.Action != NotifyCollectionChangedAction.Move)
            {
                var delta = new Dictionary<T, int>(ReferenceEqualityComparer.Instance);
                Accumulate(args.NewItems, 1);
                Accumulate(args.OldItems, -1);
                foreach (var (item, count) in delta)
                    if (count > 0) Change(item, count);
                foreach (var (item, count) in delta)
                    if (count < 0) Change(item, count);

                void Accumulate(IList? items, int amount)
                {
                    if (items is null) return;
                    foreach (T item in items) delta[item] = delta.GetValueOrDefault(item) + amount;
                }
            }
            changed();
        }

        private void Change(T item, int delta)
        {
            var old = _members.GetValueOrDefault(item);
            var next = old + delta;
            if (next > 0) _members[item] = next;
            else _members.Remove(item);
            if (old == 0 && next > 0) added(item);
            else if (old > 0 && next <= 0) removed(item);
        }

        public void Dispose()
        {
            _disposed = true;
            if (_source is INotifyCollectionChanged source) source.CollectionChanged -= OnCollectionChanged;
            _source = null;
            foreach (var item in _members.Keys) removed(item);
            _members.Clear();
        }
    }
}
