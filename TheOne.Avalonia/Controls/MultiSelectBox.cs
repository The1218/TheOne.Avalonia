using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using global::Avalonia;
using global::Avalonia.Collections;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Controls.Templates;
using global::Avalonia.Data;
using global::Avalonia.Interactivity;
using global::Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

/// <summary>A searchable selection over model items, with bounded tags and native virtualized results.</summary>
public partial class MultiSelectBox : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty = AvaloniaProperty.Register<MultiSelectBox, IEnumerable?>(nameof(ItemsSource));
    public static readonly StyledProperty<IList?> SelectedItemsProperty = AvaloniaProperty.Register<MultiSelectBox, IList?>(nameof(SelectedItems), defaultBindingMode: BindingMode.TwoWay);
    public static readonly StyledProperty<MultiSelectItemAdapter?> ItemAdapterProperty = AvaloniaProperty.Register<MultiSelectBox, MultiSelectItemAdapter?>(nameof(ItemAdapter));
    public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty = AvaloniaProperty.Register<MultiSelectBox, IDataTemplate?>(nameof(ItemTemplate));
    public static readonly StyledProperty<IDataTemplate?> SelectedItemTemplateProperty = AvaloniaProperty.Register<MultiSelectBox, IDataTemplate?>(nameof(SelectedItemTemplate));
    public static readonly StyledProperty<string> SearchTextProperty = AvaloniaProperty.Register<MultiSelectBox, string>(nameof(SearchText), "", defaultBindingMode: BindingMode.TwoWay);
    public static readonly StyledProperty<string> PlaceholderTextProperty = AvaloniaProperty.Register<MultiSelectBox, string>(nameof(PlaceholderText), "请选择");
    public static readonly StyledProperty<string> SearchPlaceholderTextProperty = AvaloniaProperty.Register<MultiSelectBox, string>(nameof(SearchPlaceholderText), "搜索选项");
    public static readonly StyledProperty<string> EmptyTextProperty = AvaloniaProperty.Register<MultiSelectBox, string>(nameof(EmptyText), "无匹配选项");
    public static readonly StyledProperty<string> ClearTextProperty = AvaloniaProperty.Register<MultiSelectBox, string>(nameof(ClearText), "清空可编辑选择");
    public static readonly StyledProperty<string> OpenTextProperty = AvaloniaProperty.Register<MultiSelectBox, string>(nameof(OpenText), "展开或收起选项");
    public static readonly StyledProperty<string> RemoveTextFormatProperty = AvaloniaProperty.Register<MultiSelectBox, string>(nameof(RemoveTextFormat), "移除 {0}");
    public static readonly StyledProperty<bool> IsReadOnlyProperty = AvaloniaProperty.Register<MultiSelectBox, bool>(nameof(IsReadOnly));
    public static readonly StyledProperty<bool> IsDropDownOpenProperty = AvaloniaProperty.Register<MultiSelectBox, bool>(nameof(IsDropDownOpen), defaultBindingMode: BindingMode.TwoWay);
    public static readonly StyledProperty<int> MaxVisibleTagsProperty = AvaloniaProperty.Register<MultiSelectBox, int>(nameof(MaxVisibleTags), 3, validate: value => value is >= 0 and <= 100);
    public static readonly StyledProperty<double> MaxDropDownHeightProperty = AvaloniaProperty.Register<MultiSelectBox, double>(nameof(MaxDropDownHeight), 320, validate: value => double.IsFinite(value) && value >= 80);
    public static readonly DirectProperty<MultiSelectBox, IReadOnlyList<MultiSelectBoxItem>> VisibleItemsProperty = AvaloniaProperty.RegisterDirect<MultiSelectBox, IReadOnlyList<MultiSelectBoxItem>>(nameof(VisibleItems), box => box.VisibleItems);
    public static readonly DirectProperty<MultiSelectBox, IReadOnlyList<MultiSelectBoxItem>> DisplayTagsProperty = AvaloniaProperty.RegisterDirect<MultiSelectBox, IReadOnlyList<MultiSelectBoxItem>>(nameof(DisplayTags), box => box.DisplayTags);
    public static readonly DirectProperty<MultiSelectBox, int> SelectedCountProperty = AvaloniaProperty.RegisterDirect<MultiSelectBox, int>(nameof(SelectedCount), box => box.SelectedCount);
    public static readonly DirectProperty<MultiSelectBox, int> HiddenSelectedCountProperty = AvaloniaProperty.RegisterDirect<MultiSelectBox, int>(nameof(HiddenSelectedCount), box => box.HiddenSelectedCount);
    public static readonly DirectProperty<MultiSelectBox, bool> HasSelectionProperty = AvaloniaProperty.RegisterDirect<MultiSelectBox, bool>(nameof(HasSelection), box => box.HasSelection);
    public static readonly DirectProperty<MultiSelectBox, bool> HasHiddenSelectionProperty = AvaloniaProperty.RegisterDirect<MultiSelectBox, bool>(nameof(HasHiddenSelection), box => box.HasHiddenSelection);
    public static readonly DirectProperty<MultiSelectBox, bool> HasNoResultsProperty = AvaloniaProperty.RegisterDirect<MultiSelectBox, bool>(nameof(HasNoResults), box => box.HasNoResults);
    public static readonly DirectProperty<MultiSelectBox, bool> CanClearProperty = AvaloniaProperty.RegisterDirect<MultiSelectBox, bool>(nameof(CanClear), box => box.CanClear);
    public static readonly DirectProperty<MultiSelectBox, double> ItemHeightProperty = AvaloniaProperty.RegisterDirect<MultiSelectBox, double>(nameof(ItemHeight), box => box.ItemHeight);
    public static readonly RoutedEvent<SelectionChangedEventArgs> SelectionChangedEvent = RoutedEvent.Register<MultiSelectBox, SelectionChangedEventArgs>(nameof(SelectionChanged), RoutingStrategies.Bubble);
    private List<object> _source = [], _selection = [];
    private Dictionary<object, MultiSelectBoxItem> _entries = [];
    private readonly List<INotifyPropertyChanged> _models = [];
    private INotifyCollectionChanged? _sourceCollection, _selectionCollection;
    private IReadOnlyList<MultiSelectBoxItem> _visibleItems = [], _displayTags = [];
    private int _selectedCount, _hiddenCount;
    private bool _hasSelection, _hasHidden, _hasNoResults = true, _canClear, _updating, _attached;
    private double _itemHeight = 36;
    public IEnumerable? ItemsSource { get => GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }
    public IList? SelectedItems { get => GetValue(SelectedItemsProperty); set => SetValue(SelectedItemsProperty, value); }
    public MultiSelectItemAdapter? ItemAdapter { get => GetValue(ItemAdapterProperty); set => SetValue(ItemAdapterProperty, value); }
    public IDataTemplate? ItemTemplate { get => GetValue(ItemTemplateProperty); set => SetValue(ItemTemplateProperty, value); }
    public IDataTemplate? SelectedItemTemplate { get => GetValue(SelectedItemTemplateProperty); set => SetValue(SelectedItemTemplateProperty, value); }
    public string SearchText { get => GetValue(SearchTextProperty) ?? ""; set => SetValue(SearchTextProperty, value); }
    public string PlaceholderText { get => GetValue(PlaceholderTextProperty); set => SetValue(PlaceholderTextProperty, value); }
    public string SearchPlaceholderText { get => GetValue(SearchPlaceholderTextProperty); set => SetValue(SearchPlaceholderTextProperty, value); }
    public string EmptyText { get => GetValue(EmptyTextProperty); set => SetValue(EmptyTextProperty, value); }
    public string ClearText { get => GetValue(ClearTextProperty); set => SetValue(ClearTextProperty, value); }
    public string OpenText { get => GetValue(OpenTextProperty); set => SetValue(OpenTextProperty, value); }
    public string RemoveTextFormat { get => GetValue(RemoveTextFormatProperty); set => SetValue(RemoveTextFormatProperty, value); }
    public bool IsReadOnly { get => GetValue(IsReadOnlyProperty); set => SetValue(IsReadOnlyProperty, value); }
    public bool IsDropDownOpen { get => GetValue(IsDropDownOpenProperty); set => SetValue(IsDropDownOpenProperty, value); }
    public int MaxVisibleTags { get => GetValue(MaxVisibleTagsProperty); set => SetValue(MaxVisibleTagsProperty, value); }
    public double MaxDropDownHeight { get => GetValue(MaxDropDownHeightProperty); set => SetValue(MaxDropDownHeightProperty, value); }
    public IReadOnlyList<MultiSelectBoxItem> VisibleItems => _visibleItems;
    public IReadOnlyList<MultiSelectBoxItem> DisplayTags => _displayTags;
    public int SelectedCount => _selectedCount;
    public int HiddenSelectedCount => _hiddenCount;
    public bool HasSelection => _hasSelection;
    public bool HasHiddenSelection => _hasHidden;
    public bool HasNoResults => _hasNoResults;
    public bool CanClear => _canClear;
    public double ItemHeight => _itemHeight;
    internal bool CanEditSelection => IsEffectivelyEnabled && !IsReadOnly && SelectedItems is { IsReadOnly: false, IsFixedSize: false };
    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged { add => AddHandler(SelectionChangedEvent, value); remove => RemoveHandler(SelectionChangedEvent, value); }
    protected override Type StyleKeyOverride => typeof(MultiSelectBox);
    public MultiSelectBox() => SetCurrentValue(SelectedItemsProperty, new AvaloniaList<object>());

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs args)
    {
        base.OnAttachedToVisualTree(args); _attached = true; RefreshItems(); ObserveCollections(); UpdatePopup();
    }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs args)
    {
        _attached = false; CloseDropDown(false); Unobserve(); base.OnDetachedFromVisualTree(args);
    }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ItemsSourceProperty || change.Property == SelectedItemsProperty)
        { RefreshItems(); if (_attached) ObserveCollections(); }
        else if (change.Property == SearchTextProperty || change.Property == ItemAdapterProperty || change.Property == ItemTemplateProperty || change.Property == SelectedItemTemplateProperty
            || change.Property == MaxVisibleTagsProperty || change.Property == IsReadOnlyProperty || change.Property == RemoveTextFormatProperty || change.Property == IsEffectivelyEnabledProperty)
        {
            RefreshItems();
            if (!IsEffectivelyEnabled) CloseDropDown(false);
        }
        else if (change.Property == IsDropDownOpenProperty) UpdatePopup();
        if (change.Property == ControlSizing.SizeProperty || change.Property == ThemeVariantScope.ActualThemeVariantProperty || change.Property == BoundsProperty || change.Property == MaxDropDownHeightProperty)
        {
            SetAndRaise(ItemHeightProperty, ref _itemHeight, ControlSizing.GetSize(this) switch { ControlSize.Small => 30, ControlSize.Large => 44, _ => 36 });
            if (IsDropDownOpen) ApplyPopupEnvironment();
        }
    }
    private static List<object> DistinctItems(IEnumerable? source) => source?.Cast<object?>().Where(item => item is not null).Cast<object>().Distinct().ToList() ?? [];
    /// <summary>Refreshes plain collections and models that do not provide change notifications.</summary>
    public void RefreshItems()
    {
        global::Avalonia.Threading.Dispatcher.UIThread.VerifyAccess();
        if (_updating) return;
        var before = _selection;
        _updating = true;
        try
        {
            _source = DistinctItems(ItemsSource); _selection = DistinctItems(SelectedItems);
            var selected = _selection.ToHashSet(); var entries = new Dictionary<object, MultiSelectBoxItem>();
            foreach (var item in _source.Concat(_selection))
            {
                if (entries.ContainsKey(item)) continue;
                var entry = _entries.TryGetValue(item, out var previous) && ReferenceEquals(previous.Item, item) ? previous : new MultiSelectBoxItem(this, item);
                entries.Add(item, entry); entry.Update(selected.Contains(item));
            }
            _entries = entries;
            var query = SearchText.Trim();
            var visible = _source.Select(item => entries[item]).Where(entry => query.Length == 0 || entry.Text.Contains(query, StringComparison.CurrentCultureIgnoreCase)).ToArray();
            var visibleChanged = !_visibleItems.SequenceEqual(visible);
            if (visibleChanged) SetAndRaise(VisibleItemsProperty, ref _visibleItems, visible);
            SetAndRaise(DisplayTagsProperty, ref _displayTags, _selection.Take(MaxVisibleTags).Select(item => entries[item]).ToArray());
            SetAndRaise(SelectedCountProperty, ref _selectedCount, selected.Count);
            SetAndRaise(HiddenSelectedCountProperty, ref _hiddenCount, selected.Count - _displayTags.Count);
            SetAndRaise(HasSelectionProperty, ref _hasSelection, selected.Count > 0);
            SetAndRaise(HasHiddenSelectionProperty, ref _hasHidden, _hiddenCount > 0);
            SetAndRaise(HasNoResultsProperty, ref _hasNoResults, visible.Length == 0);
            SetAndRaise(CanClearProperty, ref _canClear, CanEditSelection && _selection.Any(item => entries[item].IsEnabled));
            PseudoClasses.Set(":empty", !_hasSelection); PseudoClasses.Set(":readonly", IsReadOnly || !CanEditSelection);
            if (_results is not null)
            {
                if (visibleChanged || !ReferenceEquals(_results.ItemsSource, _visibleItems)) { _results.ItemsSource = _visibleItems; _activeIndex = -1; }
                SynchronizeNativeSelection();
            }
            if (_attached) ObserveModels();
        }
        finally { _updating = false; }
        var removed = before.Except(_selection).ToArray(); var added = _selection.Except(before).ToArray();
        if (removed.Length > 0 || added.Length > 0) RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, removed, added));
    }
    private void ObserveCollections()
    {
        if (_sourceCollection is not null) _sourceCollection.CollectionChanged -= CollectionChanged;
        if (_selectionCollection is not null) _selectionCollection.CollectionChanged -= CollectionChanged;
        _sourceCollection = ItemsSource as INotifyCollectionChanged; _selectionCollection = SelectedItems as INotifyCollectionChanged;
        if (_sourceCollection is not null) _sourceCollection.CollectionChanged += CollectionChanged;
        if (_selectionCollection is not null && !ReferenceEquals(_sourceCollection, _selectionCollection)) _selectionCollection.CollectionChanged += CollectionChanged;
    }
    private void ObserveModels()
    {
        foreach (var model in _models) model.PropertyChanged -= ModelChanged;
        _models.Clear();
        foreach (var model in _entries.Values.Select(entry => entry.Item).OfType<INotifyPropertyChanged>().Distinct<INotifyPropertyChanged>(ReferenceEqualityComparer.Instance))
        { model.PropertyChanged += ModelChanged; _models.Add(model); }
    }
    private void Unobserve()
    {
        if (_sourceCollection is not null) _sourceCollection.CollectionChanged -= CollectionChanged;
        if (_selectionCollection is not null) _selectionCollection.CollectionChanged -= CollectionChanged;
        _sourceCollection = _selectionCollection = null;
        foreach (var model in _models) model.PropertyChanged -= ModelChanged;
        _models.Clear();
    }
    private void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs args) => RefreshItems();
    private void ModelChanged(object? sender, PropertyChangedEventArgs args) => RefreshItems();
    private bool EditSelection(Action<IList> edit)
    {
        global::Avalonia.Threading.Dispatcher.UIThread.VerifyAccess();
        if (!CanEditSelection || _updating || SelectedItems is not { } selected) return false;
        var before = DistinctItems(selected); _updating = true;
        try { edit(selected); }
        finally { _updating = false; RefreshItems(); }
        return !before.SequenceEqual(_selection);
    }
    public bool ToggleItem(object item)
    {
        if (!_entries.TryGetValue(item, out var entry) || !entry.IsEnabled) return false;
        return EditSelection(selected =>
        {
            if (selected.Contains(item)) { while (selected.Contains(item)) selected.Remove(item); }
            else if (_source.Contains(item)) selected.Add(item);
        });
    }
    /// <summary>Removes editable selections, retaining selected disabled items.</summary>
    public bool ClearSelection() => EditSelection(selected =>
    {
        foreach (var item in _selection.Where(item => _entries[item].IsEnabled).ToArray())
            while (selected.Contains(item)) selected.Remove(item);
    });
    private void SynchronizeNativeSelection()
    {
        if (_results is null) return;
        _results.Selection.BeginBatchUpdate();
        try
        {
            for (var index = 0; index < _visibleItems.Count; index++)
            {
                if (_visibleItems[index].IsSelected) _results.Selection.Select(index);
                else _results.Selection.Deselect(index);
            }
        }
        finally { _results.Selection.EndBatchUpdate(); }
    }
    private void ResultsSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        args.Handled = true;
        if (_updating) return;
        EditSelection(selected =>
        {
            foreach (var entry in args.RemovedItems.OfType<MultiSelectBoxItem>().Where(entry => entry.IsEnabled))
                while (selected.Contains(entry.Item)) selected.Remove(entry.Item);
            foreach (var entry in args.AddedItems.OfType<MultiSelectBoxItem>().Where(entry => entry.IsEnabled))
                if (!selected.Contains(entry.Item)) selected.Add(entry.Item);
        });
        _updating = true;
        try { SynchronizeNativeSelection(); }
        finally { _updating = false; }
    }
}
