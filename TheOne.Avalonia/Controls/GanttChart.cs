using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Controls.Templates;
using global::Avalonia.Data;
using global::Avalonia.Input;
using global::Avalonia.Interactivity;
using global::Avalonia.Layout;
using global::Avalonia.Media;
using global::Avalonia.Threading;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

/// <summary>A read-only all-day Gantt with native virtualized selection and one shared row viewport.</summary>
public partial class GanttChart : TemplatedControl
{
    public GanttChart() => AddHandler(PointerWheelChangedEvent, OnNavigationWheel, RoutingStrategies.Tunnel);
    public static readonly StyledProperty<IEnumerable<GanttTask>?> ItemsSourceProperty = AvaloniaProperty.Register<GanttChart, IEnumerable<GanttTask>?>(nameof(ItemsSource));
    public static readonly StyledProperty<GanttTask?> SelectedTaskProperty = AvaloniaProperty.Register<GanttChart, GanttTask?>(nameof(SelectedTask), defaultBindingMode: BindingMode.TwoWay);
    public static readonly StyledProperty<DateOnly> ViewStartProperty = AvaloniaProperty.Register<GanttChart, DateOnly>(nameof(ViewStart), DateOnly.FromDateTime(DateTime.Today));
    /// <summary>Exclusive right boundary of the time axis.</summary>
    public static readonly StyledProperty<DateOnly> ViewEndProperty = AvaloniaProperty.Register<GanttChart, DateOnly>(nameof(ViewEnd), DateOnly.FromDateTime(DateTime.Today).AddDays(31));
    public static readonly StyledProperty<DateOnly> TodayProperty = AvaloniaProperty.Register<GanttChart, DateOnly>(nameof(Today), DateOnly.FromDateTime(DateTime.Today));
    public static readonly StyledProperty<double> DayWidthProperty = AvaloniaProperty.Register<GanttChart, double>(nameof(DayWidth), 48, validate: v => double.IsFinite(v) && v >= 4 && v <= 240);
    public static readonly StyledProperty<double> RowHeightProperty = AvaloniaProperty.Register<GanttChart, double>(nameof(RowHeight), 44, validate: Positive);
    public static readonly StyledProperty<double> TaskListWidthProperty = AvaloniaProperty.Register<GanttChart, double>(nameof(TaskListWidth), 420, validate: Positive);
    public static readonly StyledProperty<double> HeaderHeightProperty = AvaloniaProperty.Register<GanttChart, double>(nameof(HeaderHeight), 72, validate: Positive);
    public static readonly StyledProperty<IBrush?> GridBrushProperty = AvaloniaProperty.Register<GanttChart, IBrush?>(nameof(GridBrush));
    public static readonly StyledProperty<IBrush?> HeaderBrushProperty = AvaloniaProperty.Register<GanttChart, IBrush?>(nameof(HeaderBrush));
    public static readonly StyledProperty<IBrush?> WeekendBrushProperty = AvaloniaProperty.Register<GanttChart, IBrush?>(nameof(WeekendBrush));
    public static readonly StyledProperty<IBrush?> TaskBrushProperty = AvaloniaProperty.Register<GanttChart, IBrush?>(nameof(TaskBrush));
    public static readonly StyledProperty<IBrush?> ProgressBrushProperty = AvaloniaProperty.Register<GanttChart, IBrush?>(nameof(ProgressBrush));
    public static readonly StyledProperty<IBrush?> SelectionBrushProperty = AvaloniaProperty.Register<GanttChart, IBrush?>(nameof(SelectionBrush));
    public static readonly StyledProperty<IBrush?> MutedBrushProperty = AvaloniaProperty.Register<GanttChart, IBrush?>(nameof(MutedBrush));
    public static readonly StyledProperty<IBrush?> OnTaskBrushProperty = AvaloniaProperty.Register<GanttChart, IBrush?>(nameof(OnTaskBrush));
    public static readonly StyledProperty<string> TaskHeaderProperty = AvaloniaProperty.Register<GanttChart, string>(nameof(TaskHeader), "Task");
    public static readonly StyledProperty<string> OwnerHeaderProperty = AvaloniaProperty.Register<GanttChart, string>(nameof(OwnerHeader), "Owner");
    public static readonly StyledProperty<string> ProgressHeaderProperty = AvaloniaProperty.Register<GanttChart, string>(nameof(ProgressHeader), "Progress");
    public static readonly StyledProperty<string> EmptyTextProperty = AvaloniaProperty.Register<GanttChart, string>(nameof(EmptyText), "No tasks");
    public static readonly DirectProperty<GanttChart, string?> ValidationErrorProperty = AvaloniaProperty.RegisterDirect<GanttChart, string?>(nameof(ValidationError), c => c.ValidationError);
    public static readonly DirectProperty<GanttChart, int> VisibleTaskCountProperty = AvaloniaProperty.RegisterDirect<GanttChart, int>(nameof(VisibleTaskCount), c => c.VisibleTaskCount);
    public static readonly DirectProperty<GanttChart, GanttSelection?> SelectionProperty = AvaloniaProperty.RegisterDirect<GanttChart, GanttSelection?>(nameof(Selection), c => c.Selection);

    public IEnumerable<GanttTask>? ItemsSource { get => GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }
    public GanttTask? SelectedTask { get => GetValue(SelectedTaskProperty); set => SetValue(SelectedTaskProperty, value); }
    public DateOnly ViewStart { get => GetValue(ViewStartProperty); set => SetValue(ViewStartProperty, value); }
    public DateOnly ViewEnd { get => GetValue(ViewEndProperty); set => SetValue(ViewEndProperty, value); }
    public DateOnly Today { get => GetValue(TodayProperty); set => SetValue(TodayProperty, value); }
    public double DayWidth { get => GetValue(DayWidthProperty); set => SetValue(DayWidthProperty, value); }
    public double RowHeight { get => GetValue(RowHeightProperty); set => SetValue(RowHeightProperty, value); }
    public double TaskListWidth { get => GetValue(TaskListWidthProperty); set => SetValue(TaskListWidthProperty, value); }
    public double HeaderHeight { get => GetValue(HeaderHeightProperty); set => SetValue(HeaderHeightProperty, value); }
    public IBrush? GridBrush { get => GetValue(GridBrushProperty); set => SetValue(GridBrushProperty, value); }
    public IBrush? HeaderBrush { get => GetValue(HeaderBrushProperty); set => SetValue(HeaderBrushProperty, value); }
    public IBrush? WeekendBrush { get => GetValue(WeekendBrushProperty); set => SetValue(WeekendBrushProperty, value); }
    public IBrush? TaskBrush { get => GetValue(TaskBrushProperty); set => SetValue(TaskBrushProperty, value); }
    public IBrush? ProgressBrush { get => GetValue(ProgressBrushProperty); set => SetValue(ProgressBrushProperty, value); }
    public IBrush? SelectionBrush { get => GetValue(SelectionBrushProperty); set => SetValue(SelectionBrushProperty, value); }
    public IBrush? MutedBrush { get => GetValue(MutedBrushProperty); set => SetValue(MutedBrushProperty, value); }
    public IBrush? OnTaskBrush { get => GetValue(OnTaskBrushProperty); set => SetValue(OnTaskBrushProperty, value); }
    public string TaskHeader { get => GetValue(TaskHeaderProperty); set => SetValue(TaskHeaderProperty, value); }
    public string OwnerHeader { get => GetValue(OwnerHeaderProperty); set => SetValue(OwnerHeaderProperty, value); }
    public string ProgressHeader { get => GetValue(ProgressHeaderProperty); set => SetValue(ProgressHeaderProperty, value); }
    public string EmptyText { get => GetValue(EmptyTextProperty); set => SetValue(EmptyTextProperty, value); }
    public string? ValidationError => _validationError;
    public int VisibleTaskCount => _visibleTaskCount;
    public GanttSelection? Selection => _selection;
    public event EventHandler? SelectionChanged;

    private readonly ObservableCollection<Row> _rows = [];
    private readonly Dictionary<string, Row> _entries = new(StringComparer.Ordinal);
    private readonly HashSet<GanttTask> _observed = [];
    private INotifyCollectionChanged? _collection;
    private ListBox? _list;
    private ScrollBar? _horizontal;
    private GanttHeaderPresenter? _header;
    private TextBlock? _empty;
    private string? _validationError;
    private int _visibleTaskCount;
    private GanttSelection? _selection;
    private bool _active, _pending, _selecting;
    private double _offset;
    private double _rowWidth;
    private static bool Positive(double v) => double.IsFinite(v) && v > 0;
    internal sealed class Row(GanttTask task)
    {
        public GanttTask Task { get; } = task;
        public Row? Parent;
        public List<Row> Children { get; } = [];
        public int Depth, Index;
        public DateOnly Start, End;
        public double Progress, Weight;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_list is not null) { _list.SelectionChanged -= OnSelection; _list.RemoveHandler(KeyDownEvent, OnTreeKey); _list.ItemsSource = null; }
        if (_horizontal is not null) _horizontal.PropertyChanged -= OnHorizontalChanged;
        if (_header is not null) _header.Owner = null;
        base.OnApplyTemplate(e);
        _list = e.NameScope.Find<ListBox>("PART_Rows");
        _horizontal = e.NameScope.Find<ScrollBar>("PART_HorizontalScrollBar");
        _header = e.NameScope.Find<GanttHeaderPresenter>("PART_Header");
        _empty = e.NameScope.Find<TextBlock>("PART_Empty");
        if (_list is not null)
        {
            _list.ItemTemplate = new FuncDataTemplate<Row>((row, _) => row is null ? null : new GanttRowPresenter(this, row));
            _list.ItemsSource = _rows;
            _list.SelectionChanged += OnSelection;
            _list.AddHandler(KeyDownEvent, OnTreeKey, RoutingStrategies.Tunnel);
        }
        if (_horizontal is not null) _horizontal.PropertyChanged += OnHorizontalChanged;
        if (_header is not null) _header.Owner = this;
        UpdateEmpty(); SyncSelection();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e); _active = true; RefreshData();
    }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _active = false; StopObserving(); base.OnDetachedFromVisualTree(e);
    }
    private void StopObserving()
    {
        if (_collection is not null) _collection.CollectionChanged -= OnCollection;
        _collection = null;
        foreach (var task in _observed) task.PropertyChanged -= OnTaskChanged;
        _observed.Clear();
    }
    private void OnCollection(object? sender, NotifyCollectionChangedEventArgs e) => RequestRefresh();
    private void OnTaskChanged(object? sender, PropertyChangedEventArgs e) => RequestRefresh();
    private void RequestRefresh()
    {
        if (!_active || _pending) return;
        _pending = true;
        Dispatcher.UIThread.Post(() => { _pending = false; if (_active) RefreshData(); });
    }

    /// <summary>Refresh a non-notifying source. Call on the UI thread.</summary>
    public void RefreshData()
    {
        var restoreFocus = _list?.IsKeyboardFocusWithin == true;
        StopObserving(); _entries.Clear();
        string? error = null;
        var source = ItemsSource?.ToArray() ?? [];
        if (_active)
        {
            _collection = ItemsSource as INotifyCollectionChanged;
            if (_collection is not null) _collection.CollectionChanged += OnCollection;
            foreach (var task in source)
                if (task is not null && _observed.Add(task)) task.PropertyChanged += OnTaskChanged;
        }
        foreach (var task in source)
        {
            if (task is null || string.IsNullOrWhiteSpace(task.Id) || !_entries.TryAdd(task.Id, new Row(task))) { error = "Task IDs must be non-empty and unique."; break; }
        }
        var roots = new List<Row>();
        if (error is null)
        {
            foreach (var row in _entries.Values)
            {
                if (row.Task.ParentId is null) roots.Add(row);
                else if (_entries.TryGetValue(row.Task.ParentId, out var parent)) { row.Parent = parent; parent.Children.Add(row); }
                else { error = "A parent task was not found."; break; }
            }
        }
        var ordered = new List<Row>();
        if (error is null)
        {
            var stack = new Stack<Row>(roots.AsEnumerable().Reverse());
            while (stack.TryPop(out var row))
            {
                ordered.Add(row); row.Depth = row.Parent is null ? 0 : row.Parent.Depth + 1;
                for (var i = row.Children.Count - 1; i >= 0; i--) stack.Push(row.Children[i]);
            }
            if (ordered.Count != source.Length) error = "Task hierarchy contains a cycle.";
        }
        if (error is null)
        {
            for (var i = ordered.Count - 1; i >= 0; i--)
            {
                var row = ordered[i]; var task = row.Task;
                if (row.Children.Count > 0)
                {
                    if (task.IsMilestone) { error = "A milestone cannot have children."; break; }
                    row.Start = row.Children.Min(c => c.Start); row.End = row.Children.Max(c => c.End);
                    row.Weight = row.Children.Sum(c => c.Weight);
                    row.Progress = row.Weight > 0 ? row.Children.Sum(c => c.Progress * c.Weight) / row.Weight : 0;
                }
                else
                {
                    if (!double.IsFinite(task.Progress) || task.Progress < 0 || task.Progress > 1 || (!task.IsMilestone && task.EndExclusive <= task.Start))
                    { error = "Leaf tasks need Progress in [0, 1] and EndExclusive after Start."; break; }
                    row.Start = task.Start;
                    row.End = task.IsMilestone ? (task.Start < DateOnly.MaxValue ? task.Start.AddDays(1) : task.Start) : task.EndExclusive;
                    row.Progress = task.Progress; row.Weight = task.IsMilestone ? 0 : row.End.DayNumber - row.Start.DayNumber;
                }
            }
        }
        _selecting = true;
        try
        {
            _rows.Clear();
            if (error is null)
            {
                var stack = new Stack<Row>(roots.AsEnumerable().Reverse());
                while (stack.TryPop(out var row))
                {
                    row.Index = _rows.Count; _rows.Add(row);
                    if (row.Task.IsExpanded) for (var i = row.Children.Count - 1; i >= 0; i--) stack.Push(row.Children[i]);
                }
            }
            else _entries.Clear();
            SetAndRaise(ValidationErrorProperty, ref _validationError, error);
            SetAndRaise(VisibleTaskCountProperty, ref _visibleTaskCount, _rows.Count);
            var selected = _entries.Values.FirstOrDefault(r => ReferenceEquals(r.Task, SelectedTask));
            while (selected is not null && !_rows.Contains(selected)) selected = selected.Parent;
            SetCurrentValue(SelectedTaskProperty, selected?.Task);
        }
        finally { _selecting = false; }
        SyncSelection(); UpdateEmpty(); Redraw();
        if (restoreFocus)
            Dispatcher.UIThread.Post(() => { if (_active && _list is { SelectedIndex: >= 0 } list) list.ContainerFromIndex(list.SelectedIndex)?.Focus(); }, DispatcherPriority.Loaded);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ItemsSourceProperty) RequestRefresh();
        if (change.Property == SelectedTaskProperty)
        {
            if (!_selecting) SyncSelection();
            UpdateSelectionInfo(); Redraw();
        }
        if (change.Property == RowHeightProperty || change.Property == FontSizeProperty || change.Property == FontFamilyProperty || change.Property == TaskListWidthProperty)
            foreach (var row in Presenters()) row.InvalidateMeasure();
        if (change.Property != ItemsSourceProperty && change.Property != SelectedTaskProperty)
        { UpdateEmpty(); Redraw(); UpdateScrollMetrics(); }
    }
    private IEnumerable<GanttRowPresenter> Presenters() => _list?.GetVisualDescendants().OfType<GanttRowPresenter>() ?? [];
    private void Redraw() { _header?.InvalidateVisual(); foreach (var row in Presenters()) { row.RefreshTheme(); row.InvalidateVisual(); } }
    private void UpdateEmpty()
    {
        if (_empty is null) return;
        _empty.Text = ValidationError ?? (ViewEnd <= ViewStart ? "ViewEnd must be after ViewStart." : EmptyText);
        _empty.IsVisible = _rows.Count == 0 || ViewEnd <= ViewStart;
        if (_list is not null) _list.IsVisible = ViewEnd > ViewStart;
    }
    private void SyncSelection()
    {
        UpdateSelectionInfo();
        if (_list is null) return;
        _selecting = true;
        try { _list.SelectedItem = _rows.FirstOrDefault(r => ReferenceEquals(r.Task, SelectedTask)); }
        finally { _selecting = false; }
    }
    private void UpdateSelectionInfo()
    {
        var row = _entries.Values.FirstOrDefault(r => ReferenceEquals(r.Task, SelectedTask));
        var selection = row is null ? null : new GanttSelection(row.Task, row.Start, row.End, row.Progress, row.Children.Count > 0);
        if (Equals(selection, _selection)) return;
        SetAndRaise(SelectionProperty, ref _selection, selection); SelectionChanged?.Invoke(this, EventArgs.Empty);
    }
    private void OnSelection(object? sender, SelectionChangedEventArgs e)
    {
        if (!_selecting) SetCurrentValue(SelectedTaskProperty, (_list?.SelectedItem as Row)?.Task);
    }
    private void OnTreeKey(object? sender, KeyEventArgs e)
    {
        if (_list?.SelectedItem is not Row row || e.KeyModifiers != KeyModifiers.None) return;
        if (e.Key == Key.Left)
        {
            if (row.Children.Count > 0 && row.Task.IsExpanded) row.Task.IsExpanded = false;
            else if (row.Parent is not null) SetCurrentValue(SelectedTaskProperty, row.Parent.Task);
            e.Handled = true;
        }
        else if (e.Key == Key.Right && row.Children.Count > 0)
        {
            if (!row.Task.IsExpanded) row.Task.IsExpanded = true;
            else SetCurrentValue(SelectedTaskProperty, row.Children[0].Task);
            e.Handled = true;
        }
    }
    internal double PaneWidth(double width) => Math.Min(TaskListWidth, Math.Max(0, width * .48));
    internal double TimelineX(DateOnly date, double width) => PaneWidth(width) + (date.DayNumber - ViewStart.DayNumber) * DayWidth - _offset;
    internal void SetRowWidth(double width)
    {
        if (Math.Abs(_rowWidth - width) < .1) return;
        _rowWidth = width; UpdateScrollMetrics(); _header?.InvalidateVisual();
    }
    protected override Size ArrangeOverride(Size finalSize)
    {
        var result = base.ArrangeOverride(finalSize);
        UpdateScrollMetrics(); return result;
    }
    private void UpdateScrollMetrics()
    {
        if (_horizontal is null) return;
        var width = _rowWidth > 0 ? _rowWidth : Bounds.Width;
        var pane = PaneWidth(width); var viewport = Math.Max(0, width - pane);
        var extent = Math.Max(0, ViewEnd.DayNumber - ViewStart.DayNumber) * DayWidth;
        _horizontal.Margin = new Thickness(pane, 0, Math.Max(0, Bounds.Width - width), 0);
        _horizontal.ViewportSize = viewport;
        _horizontal.Maximum = Math.Max(0, extent - viewport);
        _horizontal.SmallChange = DayWidth; _horizontal.LargeChange = Math.Max(DayWidth, viewport * .8);
        _horizontal.IsVisible = _horizontal.Maximum > .5;
        _horizontal.Value = Math.Clamp(_offset, 0, _horizontal.Maximum);
    }
    private void OnHorizontalChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property != RangeBase.ValueProperty || _horizontal is null) return;
        _offset = _horizontal.Value; Redraw();
    }
    private void OnNavigationWheel(object? sender, PointerWheelEventArgs e)
    {
        if (!IsEffectivelyEnabled) return;
        if ((e.KeyModifiers & (KeyModifiers.Control | KeyModifiers.Meta)) != 0 && e.Delta.Y != 0)
        {
            var x = Math.Max(0, e.GetPosition(this).X - PaneWidth(_rowWidth));
            var day = (_offset + x) / DayWidth;
            SetCurrentValue(DayWidthProperty, Math.Clamp(DayWidth * Math.Pow(1.2, Math.Clamp(e.Delta.Y, -10, 10)), 4, 240));
            if (_horizontal is not null) _horizontal.Value = Math.Clamp(day * DayWidth - x, 0, _horizontal.Maximum);
            e.Handled = true;
        }
        else if (_horizontal is not null && (e.Delta.X != 0 || (e.KeyModifiers & KeyModifiers.Shift) != 0))
        {
            var old = _horizontal.Value;
            _horizontal.Value = Math.Clamp(old - (e.Delta.X != 0 ? e.Delta.X : e.Delta.Y) * DayWidth, 0, _horizontal.Maximum);
            e.Handled = old != _horizontal.Value;
        }
    }
    public void ScrollToToday()
    {
        if (_horizontal is not null) _horizontal.Value = Math.Clamp((Today.DayNumber - ViewStart.DayNumber) * DayWidth - _horizontal.ViewportSize / 2, 0, _horizontal.Maximum);
    }
    /// <summary>Fit the complete project, including collapsed descendants, to the current width.</summary>
    public void FitToProject()
    {
        if (_entries.Count == 0) return;
        var start = _entries.Values.Min(r => r.Start); var end = _entries.Values.Max(r => r.End);
        if (end <= start && start < DateOnly.MaxValue) end = start.AddDays(1);
        // Include a milestone on the final boundary; never overflow DateOnly.
        if (_entries.Values.Any(r => r.Task.IsMilestone && r.Start == end) && end < DateOnly.MaxValue) end = end.AddDays(1);
        SetCurrentValue(ViewStartProperty, start); SetCurrentValue(ViewEndProperty, end);
        var days = Math.Max(1, end.DayNumber - start.DayNumber);
        var width = _rowWidth > 0 ? _rowWidth : Bounds.Width;
        SetCurrentValue(DayWidthProperty, Math.Clamp((width - PaneWidth(width)) / days, 4, 240));
        if (_horizontal is not null) _horizontal.Value = 0;
    }
    /// <summary>Expand ancestors and reveal the row and its date, without changing task dates.</summary>
    public bool BringTaskIntoView(string id)
    {
        if (!_entries.TryGetValue(id, out var row)) return false;
        for (var parent = row.Parent; parent is not null; parent = parent.Parent) parent.Task.IsExpanded = true;
        RefreshData(); row = _entries[id];
        SetCurrentValue(SelectedTaskProperty, row.Task);
        _list?.ScrollIntoView(row);
        if (_horizontal is not null) _horizontal.Value = Math.Clamp((row.Start.DayNumber - ViewStart.DayNumber) * DayWidth - _horizontal.ViewportSize / 4, 0, _horizontal.Maximum);
        return true;
    }
}
