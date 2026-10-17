using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Metadata;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

public partial class NavMenu
{
    public static readonly StyledProperty<string?> SearchTextProperty =
        AvaloniaProperty.Register<NavMenu, string?>(nameof(SearchText), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<BindingBase?> SearchTextBindingProperty =
        AvaloniaProperty.Register<NavMenu, BindingBase?>(nameof(SearchTextBinding));

    public static readonly DirectProperty<NavMenu, bool> IsSearchingProperty =
        AvaloniaProperty.RegisterDirect<NavMenu, bool>(nameof(IsSearching), menu => menu.IsSearching);

    public static readonly DirectProperty<NavMenu, int> SearchMatchCountProperty =
        AvaloniaProperty.RegisterDirect<NavMenu, int>(nameof(SearchMatchCount), menu => menu.SearchMatchCount);

    private bool _isSearching, _refreshSearchPosted;
    private Dictionary<object, bool>? _restoredExpansion;
    private int _searchMatchCount;
    private SearchSession? _searchSession;

    static NavMenu()
    {
        ItemsSourceProperty.OverrideMetadata<NavMenu>(
            new StyledPropertyMetadata<IEnumerable?>(coerce: (owner, source) => ((NavMenu)owner).FilterSource(source)));
        SearchOwnerProperty.Changed.AddClassHandler<TextBox>((_, change) => SearchInputChanged(change));
    }

    public string? SearchText
    {
        get => GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    [AssignBinding]
    [InheritDataTypeFromItems(nameof(ItemsSource))]
    public BindingBase? SearchTextBinding
    {
        get => GetValue(SearchTextBindingProperty);
        set => SetValue(SearchTextBindingProperty, value);
    }

    public bool IsSearching => _popupParent?.IsSearching ?? _isSearching;
    public int SearchMatchCount => _searchMatchCount;
    private SearchSession? ActiveSearch => _popupParent?.ActiveSearch ?? _searchSession;

    private IEnumerable? FilterSource(IEnumerable? source)
    {
        if (_popupParent is not null) return ActiveSearch?.Filter(source) ?? source;
        _unfilteredSource = source;
        return _searchSession?.Build(this, source) ?? source;
    }

    private void SearchPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == SearchTextProperty) NavMenuSearchText.SetHighlightQuery(this, SearchText);
        if (_popupParent is not null) return;
        if (change.Property != SearchTextProperty && change.Property != SearchTextBindingProperty &&
            change.Property != ChildrenBindingProperty && change.Property != ExpandedBindingProperty &&
            change.Property != IsGroupBindingProperty && change.Property != IsItemEnabledBindingProperty) return;
        RefreshSearch();
    }

    private void RefreshSearch()
    {
        CloseSubmenu();
        var active = !string.IsNullOrWhiteSpace(SearchText) && SearchTextBinding is not null;
        if (active)
        {
            var selectedBeforeRefresh = SelectedItem;
            _searchSession ??= new SearchSession(SelectedItem,
                this.GetVisualDescendants().OfType<ScrollViewer>()
                    .FirstOrDefault(scroll => scroll.TemplatedParent == this)?.Offset ?? default);
            SetAndRaise(IsSearchingProperty, ref _isSearching, true);
            CoerceValue(ItemsSourceProperty);
            SetCurrentValue(ItemTemplateProperty, new NavigationTemplate(this));
            UpdateLayout();
            SetCurrentValue(SelectedItemProperty,
                selectedBeforeRefresh is { } selected && _searchSession.Visible.Contains(selected) ? selected : null);
        }
        else if (_searchSession is { } previous)
        {
            previous.Unsubscribe();
            _restoredExpansion = previous.Expanded;
            _searchSession = null;
            SetAndRaise(IsSearchingProperty, ref _isSearching, false);
            SetAndRaise(SearchMatchCountProperty, ref _searchMatchCount, 0);
            CoerceValue(ItemsSourceProperty);
            SetCurrentValue(ItemTemplateProperty, new NavigationTemplate(this));
            foreach (var pair in previous.Expanded)
                Virtualization.RestoreExpandedState(this, pair.Key, pair.Value,
                    previous.SourceChildren.GetValueOrDefault(pair.Key)?.Cast<object?>());
            foreach (var row in GetRealizedTreeContainers().OfType<NavMenuItem>()) RestoreSearchRow(row);
            UpdateLayout();
            SetCurrentValue(SelectedItemProperty,
                previous.Selected is { } item && previous.All.Contains(item) ? item : null);
            Dispatcher.UIThread.Post(() =>
            {
                if (_searchSession is not null) return;
                if (this.GetVisualDescendants().OfType<ScrollViewer>()
                        .FirstOrDefault(scroll => scroll.TemplatedParent == this) is
                    { } scroll) scroll.Offset = previous.Offset;
                _restoredExpansion = null;
            }, DispatcherPriority.Loaded);
        }
    }

    private void QueueSearchRefresh(object? sender, EventArgs args)
    {
        if (_refreshSearchPosted || _searchSession is null) return;
        _refreshSearchPosted = true;
        Dispatcher.UIThread.Post(() =>
        {
            _refreshSearchPosted = false;
            if (_searchSession is not null && this.IsAttachedToVisualTree()) RefreshSearch();
        }, DispatcherPriority.Background);
    }

    private void RestoreSearchRow(NavMenuItem row)
    {
        if (row.Parent is ItemsControl parent && parent.ItemFromContainer(row) is { } item &&
            _restoredExpansion?.TryGetValue(item, out var expanded) == true)
            row.SetCurrentValue(TreeViewItem.IsExpandedProperty, expanded);
    }

    private sealed class SearchProbe : StyledElement
    {
        public static readonly StyledProperty<object?> ValueProperty =
            AvaloniaProperty.Register<SearchProbe, object?>("Value");

        public object? Read(BindingBase? binding, object item)
        {
            if (binding is null) return null;
            DataContext = item;
            using var subscription = Bind(ValueProperty, binding);
            return GetValue(ValueProperty);
        }
    }

    private sealed class SearchSession(object? selected, Vector offset)
    {
        private readonly Dictionary<object, object[]> _children = new(ReferenceEqualityComparer.Instance);
        private readonly List<Action> _unsubscribe = [];
        private bool _building;
        public object? Selected { get; } = selected;
        public Vector Offset { get; } = offset;
        public Dictionary<object, bool> Expanded { get; } = new(ReferenceEqualityComparer.Instance);
        public Dictionary<object, IEnumerable?> SourceChildren { get; } = new(ReferenceEqualityComparer.Instance);
        public HashSet<object> Visible { get; } = new(ReferenceEqualityComparer.Instance);
        public HashSet<object> All { get; } = new(ReferenceEqualityComparer.Instance);
        public List<(object Item, int[] Path)> Results { get; } = [];

        public IEnumerable Filter(IEnumerable? source)
        {
            return source?.Cast<object>().Where(Visible.Contains).ToArray() ?? [];
        }

        public object[] Children(object item)
        {
            return _children.GetValueOrDefault(item) ?? [];
        }

        public IEnumerable Build(NavMenu owner, IEnumerable? source)
        {
            if (_building) return Filter(source);
            _building = true;
            try
            {
                Unsubscribe();
                Visible.Clear();
                All.Clear();
                _children.Clear();
                SourceChildren.Clear();
                Results.Clear();
                var probe = new SearchProbe();
                var tokens = owner.SearchText!.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
                var matches = 0;
                var navigable = new HashSet<object>(ReferenceEqualityComparer.Instance);
                var visiting = new HashSet<object>(ReferenceEqualityComparer.Instance);

                void Watch(object? value)
                {
                    if (!owner.IsAttachedToVisualTree()) return;
                    if (value is INotifyCollectionChanged collection)
                    {
                        NotifyCollectionChangedEventHandler handler = (sender, args) =>
                            owner.QueueSearchRefresh(sender, args);
                        collection.CollectionChanged += handler;
                        _unsubscribe.Add(() => collection.CollectionChanged -= handler);
                    }

                    if (value is INotifyPropertyChanged model)
                    {
                        PropertyChangedEventHandler handler = (sender, args) => owner.QueueSearchRefresh(sender, args);
                        model.PropertyChanged += handler;
                        _unsubscribe.Add(() => model.PropertyChanged -= handler);
                    }
                }

                object[] Walk(IEnumerable? items, bool ancestorsEnabled = true)
                {
                    Watch(items);
                    var results = new List<object>();
                    if (items is null) return [];
                    foreach (var item in items)
                    {
                        if (item is null || !visiting.Add(item)) continue;
                        All.Add(item);
                        Watch(item);
                        if (!Expanded.ContainsKey(item))
                            Expanded[item] = owner.ExpandedBinding is { } expanded
                                ? probe.Read(expanded, item) is true
                                : Virtualization.ReadExpandedState(owner, item);
                        var label = probe.Read(owner.SearchTextBinding, item)?.ToString() ?? "";
                        var matched = tokens.All(token => label.Contains(token, StringComparison.OrdinalIgnoreCase));
                        var originalChildren = probe.Read(owner.ChildrenBinding, item) as IEnumerable;
                        SourceChildren[item] = originalChildren;
                        var enabled = ancestorsEnabled && (owner.IsItemEnabledBinding is null ||
                                                           probe.Read(owner.IsItemEnabledBinding, item) is not false);
                        if (matched && enabled && (owner.IsGroupBinding is null ||
                                                   probe.Read(owner.IsGroupBinding, item) is not true))
                            navigable.Add(item);
                        var children = Walk(originalChildren, enabled);
                        _children[item] = children;
                        if (matched || children.Length > 0)
                        {
                            Visible.Add(item);
                            results.Add(item);
                        }

                        if (matched) matches++;
                        visiting.Remove(item);
                    }

                    return results.ToArray();
                }

                var roots = Walk(source);

                void Index(object[] items, int[] parent)
                {
                    for (var i = 0; i < items.Length; i++)
                    {
                        var path = parent.Append(i).ToArray();
                        if (navigable.Contains(items[i])) Results.Add((items[i], path));
                        Index(Children(items[i]), path);
                    }
                }

                Index(roots, []);
                owner.SetAndRaise(SearchMatchCountProperty, ref owner._searchMatchCount, matches);
                return roots;
            }
            finally
            {
                _building = false;
            }
        }

        public void Unsubscribe()
        {
            foreach (var action in _unsubscribe) action();
            _unsubscribe.Clear();
        }
    }
}