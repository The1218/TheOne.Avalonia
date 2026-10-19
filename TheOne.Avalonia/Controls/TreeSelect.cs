using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

/// <summary>A searchable tree selector with stable keys and virtualized, flattened visible rows.</summary>
public partial class TreeSelect : TemplatedControl
{
    public static readonly RoutedEvent<SelectionChangedEventArgs> SelectionChangedEvent =
        RoutedEvent.Register<TreeSelect, SelectionChangedEventArgs>(nameof(SelectionChanged), RoutingStrategies.Bubble);
    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged
    { add => AddHandler(SelectionChangedEvent, value); remove => RemoveHandler(SelectionChangedEvent, value); }

    private static readonly TreeSelectItemAdapter DefaultAdapter = TreeSelectItemAdapter.Create<TreeSelectNode>(
        node => node.Key, node => node.Text, node => node.Children, node => node.IsEnabled);
    private readonly Dictionary<object, TreeSelectItem> _index = [];
    private readonly List<TreeSelectItem> _rows = [];
    private readonly HashSet<object> _expanded = [];
    private readonly HashSet<INotifyCollectionChanged> _collections = new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<INotifyPropertyChanged> _models = new(ReferenceEqualityComparer.Instance);
    private INotifyCollectionChanged? _selectedCollection;
    private bool _attached, _sync, _refreshing, _hasLoadedSource;
    private int _pendingSelection;
    private TreeSelectItemAdapter Adapter => ItemAdapter ?? DefaultAdapter;

    public TreeSelect() => SetCurrentValue(SelectedKeysProperty, new AvaloniaList<object>());

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _attached = true;
        HookSelection();
        RefreshItems();
        UpdatePopup();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _attached = false;
        CloseDropDown(false);
        ReconcileSubscriptions([], []);
        if (_selectedCollection is not null) _selectedCollection.CollectionChanged -= SelectedKeysChanged;
        _selectedCollection = null;
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (_sync) return;
        if (!_attached)
        {
            if (change.Property == SelectedKeysProperty) _pendingSelection = 1;
            else if (change.Property == SelectedKeyProperty) _pendingSelection = 2;
            else if (change.Property == SelectedItemProperty) _pendingSelection = 3;
            return;
        }
        if (change.Property == ItemsSourceProperty || change.Property == ItemAdapterProperty) RefreshItems();
        else if (change.Property == SelectedKeysProperty)
        {
            HookSelection();
            ApplySelection(ReadKeys());
        }
        else if (change.Property == SelectedKeyProperty)
            ApplySelection(SelectedKey is { } key ? [key] : []);
        else if (change.Property == SelectedItemProperty)
            ApplySelection(SelectedItem is { } item ? [Adapter.GetKey(item)] : []);
        else if (change.Property == IsMultiSelectProperty || change.Property == CheckModeProperty)
            ApplySelection(ReadKeys());
        else if (change.Property == SearchTextProperty) UpdateVisibleItems();
        else if (change.Property == IsDropDownOpenProperty) UpdatePopup();
        else if (change.Property == MaxDropDownHeightProperty || change.Property == BoundsProperty ||
                 change.Property == ThemeVariantScope.ActualThemeVariantProperty || change.Property == ControlSizing.SizeProperty)
        { UpdateHeight(); ApplyPopupEnvironment(); }
        else if (change.Property == MaxVisibleTagsProperty || change.Property == IsReadOnlyProperty ||
                 change.Property == ItemTemplateProperty || change.Property == SelectedItemTemplateProperty ||
                 change.Property == IsEffectivelyEnabledProperty)
        {
            UpdatePresentation();
            if (!IsEffectivelyEnabled) CloseDropDown(false);
        }
    }

    private void HookSelection()
    {
        if (_selectedCollection is not null) _selectedCollection.CollectionChanged -= SelectedKeysChanged;
        _selectedCollection = _attached ? SelectedKeys as INotifyCollectionChanged : null;
        if (_selectedCollection is not null) _selectedCollection.CollectionChanged += SelectedKeysChanged;
    }
    private void SelectedKeysChanged(object? sender, NotifyCollectionChangedEventArgs e)
    { if (!_sync) ApplySelection(ReadKeys()); }
    private List<object> ReadKeys() => SelectedKeys?.Cast<object?>().Where(key => key is not null)
        .Cast<object>().Distinct().ToList() ?? [];

    /// <summary>Re-read model structure and mapping; search alone never rebuilds subscriptions.</summary>
    public void RefreshItems()
    {
        Dispatcher.UIThread.VerifyAccess();
        if (_refreshing) return;
        _refreshing = true;
        var collections = new HashSet<INotifyCollectionChanged>(ReferenceEqualityComparer.Instance);
        var models = new HashSet<INotifyPropertyChanged>(ReferenceEqualityComparer.Instance);
        _rows.Clear(); _index.Clear();
        if (ItemsSource is not null) _hasLoadedSource = true;
        string? error = null;
        try
        {
            if (ItemsSource is INotifyCollectionChanged root) collections.Add(root);
            var stack = new Stack<(object? Item, TreeSelectItem? Parent)>();
            if (ItemsSource is { } source)
                foreach (var item in source.Cast<object?>().Reverse()) stack.Push((item, null));
            var seen = new HashSet<object>(ReferenceEqualityComparer.Instance);
            while (stack.TryPop(out var entry))
            {
                var item = entry.Item ?? throw new InvalidOperationException("Tree items cannot be null.");
                if (item is INotifyPropertyChanged model) models.Add(model);
                if (!seen.Add(item)) throw new InvalidOperationException("A tree cannot contain cycles or repeated model references.");
                var key = Adapter.GetKey(item) ?? throw new InvalidOperationException("Tree keys cannot be null.");
                if (_index.ContainsKey(key)) throw new InvalidOperationException("Tree keys must be unique.");
                var row = new TreeSelectItem(this, item, key, Adapter.GetText(item) ?? "",
                    (entry.Parent?.IsEnabled ?? true) && Adapter.IsEnabled(item), entry.Parent);
                _index.Add(key, row); _rows.Add(row); entry.Parent?.Children.Add(row);
                var children = Adapter.GetChildren(item);
                if (children is INotifyCollectionChanged collection) collections.Add(collection);
                if (children is not null)
                    foreach (var child in children.Cast<object?>().Reverse()) stack.Push((child, row));
            }
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        { error = exception.Message; _rows.Clear(); _index.Clear(); }
        finally { _refreshing = false; }
        SetAndRaise(ValidationErrorProperty, ref _validationError, error);
        ReconcileSubscriptions(_attached ? collections : [], _attached ? models : []);
        if (error is null)
        {
            _expanded.IntersectWith(_index.Keys);
            var pending = _pendingSelection;
            _pendingSelection = 0;
            ApplySelection(pending == 2 ? SelectedKey is { } key ? [key] : [] :
                pending == 3 ? SelectedItem is { } item ? [Adapter.GetKey(item)] : [] : ReadKeys());
        }
        else
        {
            var old = _selectedItems;
            SetAndRaise(SelectedItemsProperty, ref _selectedItems, Array.Empty<object>());
            _sync = true;
            try { SetCurrentValue(SelectedItemProperty, null); }
            finally { _sync = false; }
            UpdatePresentation();
            if (old.Count > 0) RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, old.ToArray(), Array.Empty<object>()));
        }
        UpdateVisibleItems();
        UpdateHeight();
    }

    private void ReconcileSubscriptions(HashSet<INotifyCollectionChanged> collections, HashSet<INotifyPropertyChanged> models)
    {
        foreach (var old in _collections.Except(collections)) old.CollectionChanged -= SourceChanged;
        foreach (var item in collections.Except(_collections)) item.CollectionChanged += SourceChanged;
        foreach (var old in _models.Except(models)) old.PropertyChanged -= ModelChanged;
        foreach (var item in models.Except(_models)) item.PropertyChanged += ModelChanged;
        _collections.Clear(); _collections.UnionWith(collections);
        _models.Clear(); _models.UnionWith(models);
    }
    private void SourceChanged(object? sender, NotifyCollectionChangedEventArgs e) => RefreshItems();
    private void ModelChanged(object? sender, PropertyChangedEventArgs e) => RefreshItems();

    private IEnumerable<TreeSelectItem> Leaves(TreeSelectItem root)
    {
        var stack = new Stack<TreeSelectItem>(); stack.Push(root);
        while (stack.TryPop(out var row))
        {
            if (row.Children.Count == 0) yield return row;
            else for (var i = row.Children.Count - 1; i >= 0; i--) stack.Push(row.Children[i]);
        }
    }

    private void ApplySelection(IEnumerable<object> requested)
    {
        if (_sync) return;
        var keys = new List<object>(); var seen = new HashSet<object>();
        foreach (var key in requested)
        {
            if (key is null) continue;
            if (_index.TryGetValue(key, out var row))
            {
                if (IsMultiSelect && CheckMode == TreeSelectCheckMode.Cascade && row.HasChildren)
                {
                    foreach (var leaf in Leaves(row).Where(node => node.IsEnabled))
                        if (seen.Add(leaf.Key)) keys.Add(leaf.Key);
                }
                else if (seen.Add(key)) keys.Add(key);
            }
            else if (ItemsSource is null && !_hasLoadedSource && seen.Add(key)) keys.Add(key); // Pending pre-load binding values.
            if (!IsMultiSelect && keys.Count > 0) break;
        }
        var old = _selectedItems;
        _sync = true;
        try
        {
            if (SelectedKeys is null || !SelectedKeys.Cast<object?>().SequenceEqual(keys.Cast<object?>()))
            {
                if (SelectedKeys is { IsReadOnly: false, IsFixedSize: false } list)
                { list.Clear(); foreach (var key in keys) list.Add(key); }
                else SetCurrentValue(SelectedKeysProperty, new AvaloniaList<object>(keys));
            }
            SetCurrentValue(SelectedKeyProperty, keys.FirstOrDefault());
            var selected = keys.Where(_index.ContainsKey).Select(key => _index[key].Item).ToArray();
            SetCurrentValue(SelectedItemProperty, selected.FirstOrDefault());
            SetAndRaise(SelectedItemsProperty, ref _selectedItems, selected);
        }
        finally { _sync = false; HookSelection(); }
        UpdatePresentation();
        var removed = old.Except(_selectedItems, ReferenceEqualityComparer.Instance).ToArray();
        var added = _selectedItems.Except(old, ReferenceEqualityComparer.Instance).ToArray();
        if (removed.Length > 0 || added.Length > 0)
            RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, removed, added));
    }

    private void UpdatePresentation()
    {
        var selected = ReadKeys().ToHashSet();
        // Bottom-up aggregate state, avoiding repeated descendant scans for parent checkboxes.
        var totals = new Dictionary<TreeSelectItem, (int Count, int Selected)>();
        for (var i = _rows.Count - 1; i >= 0; i--)
        {
            var row = _rows[i];
            var total = row.HasChildren ? row.Children.Select(child => totals[child])
                .Aggregate((Count: 0, Selected: 0), (sum, value) => (sum.Count + value.Count, sum.Selected + value.Selected))
                : (Count: row.IsEnabled ? 1 : 0, Selected: row.IsEnabled && selected.Contains(row.Key) ? 1 : 0);
            totals[row] = total;
            row.IsChecked = IsMultiSelect && CheckMode == TreeSelectCheckMode.Cascade && row.HasChildren
                ? total.Count > 0 && total.Selected == total.Count ? true : total.Selected > 0 ? null : false
                : selected.Contains(row.Key);
            row.Notify();
        }
        var tags = ReadKeys().Where(_index.ContainsKey).Select(key => _index[key]).ToArray();
        SetAndRaise(SelectedCountProperty, ref _selectedCount, tags.Length);
        SetAndRaise(HasSelectionProperty, ref _hasSelection, tags.Length > 0);
        SetAndRaise(DisplayTagsProperty, ref _displayTags, tags.Take(IsMultiSelect ? MaxVisibleTags : 1).ToArray());
        SetAndRaise(HiddenSelectedCountProperty, ref _hiddenCount, Math.Max(0, tags.Length - _displayTags.Count));
        SetAndRaise(HasHiddenSelectionProperty, ref _hasHidden, _hiddenCount > 0);
        SetAndRaise(CanClearProperty, ref _canClear, !IsReadOnly && IsEffectivelyEnabled && tags.Any(row => row.IsEnabled));
    }

    private void UpdateVisibleItems()
    {
        var query = (SearchText ?? "").Trim();
        var included = new HashSet<TreeSelectItem>();
        if (query.Length > 0)
            foreach (var row in _rows.Where(row => row.Text.Contains(query, StringComparison.CurrentCultureIgnoreCase)))
                for (var node = row; node is not null && included.Add(node); node = node.Parent) { }
        var visible = new List<TreeSelectItem>();
        foreach (var row in _rows)
        {
            row.IsExpanded = query.Length > 0 ? row.Children.Any(included.Contains) : _expanded.Contains(row.Key);
            if (query.Length > 0 ? included.Contains(row) : row.Parent is null ||
                (_expanded.Contains(row.Parent.Key) && included.Contains(row.Parent)))
            { visible.Add(row); included.Add(row); }
            row.Notify();
        }
        var active = _results?.SelectedItem is TreeSelectItem item ? item.Key : null;
        SetAndRaise(VisibleItemsProperty, ref _visibleItems, visible);
        SetAndRaise(HasNoResultsProperty, ref _hasNoResults, visible.Count == 0);
        if (active is not null && _results is not null)
            _results.SelectedItem = visible.FirstOrDefault(row => Equals(row.Key, active));
    }

    private void UpdateHeight() => SetAndRaise(ItemHeightProperty, ref _itemHeight,
        ControlSizing.GetSize(this) switch { ControlSize.Small => 30d, ControlSize.Large => 44d, _ => 36d });

    /// <summary>Toggle a model's selection. Cascade affects enabled leaves only; disabled selections remain locked.</summary>
    public bool ToggleItem(object item)
    {
        Dispatcher.UIThread.VerifyAccess();
        if (IsReadOnly || !IsEffectivelyEnabled || !_index.TryGetValue(Adapter.GetKey(item), out var row) || !row.IsEnabled) return false;
        if (!IsMultiSelect) { ApplySelection([row.Key]); CloseDropDown(); return true; }
        var affected = CheckMode == TreeSelectCheckMode.Cascade ? Leaves(row).Where(node => node.IsEnabled).ToArray() : [row];
        if (affected.Length == 0) return false;
        var keys = ReadKeys(); var all = affected.All(node => keys.Contains(node.Key));
        foreach (var node in affected)
            if (all) keys.Remove(node.Key); else if (!keys.Contains(node.Key)) keys.Add(node.Key);
        ApplySelection(keys); return true;
    }

    /// <summary>Clear editable selections; selected disabled nodes remain.</summary>
    public bool ClearSelection()
    {
        Dispatcher.UIThread.VerifyAccess();
        if (!CanClear) return false;
        ApplySelection(ReadKeys().Where(key => _index.TryGetValue(key, out var row) && !row.IsEnabled));
        return true;
    }

    private bool RemoveKey(TreeSelectItem row)
    {
        if (!row.CanRemove) return false;
        ApplySelection(ReadKeys().Where(key => !Equals(key, row.Key)));
        return true;
    }

    public bool SetExpanded(object key, bool expanded)
    {
        Dispatcher.UIThread.VerifyAccess();
        if (!_index.TryGetValue(key, out var row) || !row.HasChildren || !string.IsNullOrWhiteSpace(SearchText)) return false;
        if (expanded) _expanded.Add(key); else _expanded.Remove(key);
        UpdateVisibleItems(); return true;
    }
}
