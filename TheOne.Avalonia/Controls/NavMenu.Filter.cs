using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Threading;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

public partial class NavMenu
{
    public static readonly StyledProperty<bool> ShowFilterProperty = AvaloniaProperty.Register<NavMenu, bool>(nameof(ShowFilter));
    public static readonly StyledProperty<string> FilterPlaceholderProperty = AvaloniaProperty.Register<NavMenu, string>(nameof(FilterPlaceholder), "搜索菜单");
    public static readonly DirectProperty<NavMenu, IReadOnlyList<NavMenuFilterResult>> FilterResultsProperty = AvaloniaProperty.RegisterDirect<NavMenu, IReadOnlyList<NavMenuFilterResult>>(nameof(FilterResults), menu => menu.FilterResults);
    public bool ShowFilter { get => GetValue(ShowFilterProperty); set => SetValue(ShowFilterProperty, value); }
    public string FilterPlaceholder { get => GetValue(FilterPlaceholderProperty); set => SetValue(FilterPlaceholderProperty, value); }
    public IReadOnlyList<NavMenuFilterResult> FilterResults => _filterResults;
    /// <summary>The template's native auto-complete input, available after template application.</summary>
    public NavMenuFilterBox? FilterBox => _filterBox;
    private IReadOnlyList<NavMenuFilterResult> _filterResults = [];
    private IEnumerable? _unfilteredSource;
    private IEnumerable? _indexedSource;
    private NavMenuFilterBox? _filterBox;
    private readonly List<Action> _filterSubscriptions = [];
    private bool _filterRefreshQueued;
    private int _filterCommitRevision;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs args)
    {
        ReleaseFilterPopup();
        if (_filterBox is not null) _filterBox.ResultCommitted -= FilterResultCommitted;
        base.OnApplyTemplate(args);
        _filterBox = args.NameScope.Find<NavMenuFilterBox>("PART_FilterBox");
        if (_filterBox is not null) _filterBox.ResultCommitted += FilterResultCommitted;
        InitializeFilterPopup(args.NameScope);
        RebuildFilterIndex();
    }
    private void FilterPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (_popupParent is not null) return;
        FilterPopupPropertyChanged(change);
        if (change.Property == ShowFilterProperty)
        {
            _filterCommitRevision++;
            if (!ShowFilter && _filterBox is { } box) box.SetCurrentValue(AutoCompleteBox.IsDropDownOpenProperty, false);
            RebuildFilterIndex();
        }
        else if (change.Property == ItemsSourceProperty && !ReferenceEquals(_indexedSource, _unfilteredSource)) QueueFilterRefresh();
        else if (change.Property == ChildrenBindingProperty || change.Property == SearchTextBindingProperty || change.Property == IsGroupBindingProperty || change.Property == IsItemEnabledBindingProperty)
            QueueFilterRefresh();
    }
    private void ClearFilterSubscriptions() { foreach (var unsubscribe in _filterSubscriptions) unsubscribe(); _filterSubscriptions.Clear(); }
    private void QueueFilterRefresh()
    {
        if (_filterRefreshQueued || !ShowFilter || _popupParent is not null) return;
        _filterRefreshQueued = true;
        Dispatcher.UIThread.Post(() => { _filterRefreshQueued = false; RebuildFilterIndex(); }, DispatcherPriority.Background);
    }
    private void RebuildFilterIndex()
    {
        ClearFilterSubscriptions();
        _indexedSource = _unfilteredSource;
        var results = new List<NavMenuFilterResult>();
        if (ShowFilter && this.IsAttachedToVisualTree() && _popupParent is null)
        {
            var probe = new SearchProbe();
            var visiting = new HashSet<object>(ReferenceEqualityComparer.Instance);
            var watched = new HashSet<object>(ReferenceEqualityComparer.Instance);
            void Watch(object? value)
            {
                if (value is null || !watched.Add(value)) return;
                if (value is INotifyPropertyChanged model)
                { PropertyChangedEventHandler handler = (_, _) => QueueFilterRefresh(); model.PropertyChanged += handler; _filterSubscriptions.Add(() => model.PropertyChanged -= handler); }
                if (value is INotifyCollectionChanged collection)
                { NotifyCollectionChangedEventHandler handler = (_, _) => QueueFilterRefresh(); collection.CollectionChanged += handler; _filterSubscriptions.Add(() => collection.CollectionChanged -= handler); }
            }
            void Walk(IEnumerable? items, string path, int[] parent, bool enabled)
            {
                Watch(items); if (items is null) return;
                var index = -1;
                foreach (var item in items)
                {
                    index++;
                    if (item is null || !visiting.Add(item)) continue;
                    Watch(item);
                    var label = SearchTextBinding is { } binding ? probe.Read(binding, item)?.ToString() ?? "" : item.ToString() ?? "";
                    var indices = parent.Append(index).ToArray();
                    var active = enabled && (IsItemEnabledBinding is null || probe.Read(IsItemEnabledBinding, item) is not false);
                    if (active && (IsGroupBinding is null || probe.Read(IsGroupBinding, item) is not true)) results.Add(new(item, label, path, indices));
                    Walk(probe.Read(ChildrenBinding, item) as IEnumerable, path.Length == 0 ? label : path + " › " + label, indices, active);
                    visiting.Remove(item);
                }
            }
            Walk(_unfilteredSource ?? Items, "", [], true);
        }
        SetAndRaise(FilterResultsProperty, ref _filterResults, results);
    }
    private void FilterResultCommitted(object? sender, NavMenuFilterResult result)
    {
        var revision = ++_filterCommitRevision;
        // Exit projection first. Restore its queued viewport snapshot before navigating.
        SetCurrentValue(SearchTextProperty, "");
        CloseFilterPopup(clearQuery: false, restoreFocus: false);
        if (_filterBox is { } box)
        { box.SetCurrentValue(AutoCompleteBox.IsDropDownOpenProperty, false); box.SetCurrentValue(AutoCompleteBox.SelectedItemProperty, null); }
        Dispatcher.UIThread.Post(() =>
        {
            if (revision != _filterCommitRevision || !ShowFilter || !this.IsAttachedToVisualTree()) return;
            // Re-read the path so a model/collection update cannot navigate a stale index.
            RebuildFilterIndex();
            var current = _filterResults.FirstOrDefault(candidate => ReferenceEquals(candidate.Item, result.Item));
            if (current is null) return;
            if (SelectSearchPath(current.Indices) is not null)
            {
                if (IsCollapsed) _filterButton?.Focus();
                else _filterBox?.Focus();
            }
        }, DispatcherPriority.Loaded);
    }
}
