using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Globalization;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Presenters;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Controls.Metadata;
using global::Avalonia.Controls.Templates;
using global::Avalonia.Data;
using global::Avalonia.Data.Converters;
using global::Avalonia.Input;
using global::Avalonia.Interactivity;
using global::Avalonia.Layout;
using global::Avalonia.Metadata;
using global::Avalonia.LogicalTree;
using global::Avalonia.Reactive;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

/// <summary>A native virtualized navigation tree with separately templated icons and labels.</summary>
[PseudoClasses(":collapsed", ":bounded-roots")]
public partial class NavMenu : TreeView
{
    public static readonly StyledProperty<IDataTemplate?> IconTemplateProperty = AvaloniaProperty.Register<NavMenu, IDataTemplate?>(nameof(IconTemplate));
    public static readonly StyledProperty<IDataTemplate?> ContentTemplateProperty = AvaloniaProperty.Register<NavMenu, IDataTemplate?>(nameof(ContentTemplate));
    public static readonly StyledProperty<BindingBase?> ChildrenBindingProperty = AvaloniaProperty.Register<NavMenu, BindingBase?>(nameof(ChildrenBinding));
    public static readonly StyledProperty<BindingBase?> ExpandedBindingProperty = AvaloniaProperty.Register<NavMenu, BindingBase?>(nameof(ExpandedBinding));
    public static readonly StyledProperty<BindingBase?> IsGroupBindingProperty = AvaloniaProperty.Register<NavMenu, BindingBase?>(nameof(IsGroupBinding));
    public static readonly StyledProperty<BindingBase?> IsItemEnabledBindingProperty = AvaloniaProperty.Register<NavMenu, BindingBase?>(nameof(IsItemEnabledBinding));
    public static readonly StyledProperty<double> ItemHeightProperty = AvaloniaProperty.Register<NavMenu, double>(nameof(ItemHeight), 36, validate: Positive);
    public static readonly StyledProperty<double> IconSizeProperty = AvaloniaProperty.Register<NavMenu, double>(nameof(IconSize), 18, validate: Positive);
    public static readonly StyledProperty<double> IconSpacingProperty = AvaloniaProperty.Register<NavMenu, double>(nameof(IconSpacing), 10, validate: value => double.IsFinite(value) && value >= 0);
    public static readonly StyledProperty<double> IndentProperty = AvaloniaProperty.Register<NavMenu, double>(nameof(Indent), 16, validate: value => double.IsFinite(value) && value >= 0);
    public static readonly StyledProperty<ICommand?> ItemInvokedCommandProperty = AvaloniaProperty.Register<NavMenu, ICommand?>(nameof(ItemInvokedCommand));
    public static readonly RoutedEvent<NavMenuItemInvokedEventArgs> ItemInvokedEvent = RoutedEvent.Register<NavMenu, NavMenuItemInvokedEventArgs>(nameof(ItemInvoked), RoutingStrategies.Bubble);
    private readonly ConditionalWeakTable<NavMenuItem, RowBindings> _bindings = new();
    private bool _navigationLayoutChanged = true;
    private object? _lastNavigableItem;
    private bool _restoringSelection;
    private NavMenuItem? _pressedItem;
    private IPointer? _pressedPointer;

    public NavMenu()
    {
        Virtualization.SetEnabled(this, true);
        DetachedFromVisualTree += (_, _) => { CancelHover(); CloseSubmenu(); };
        InitializePopupInteraction();
        AttachedToVisualTree += (_, _) => RebuildFilterIndex();
        DetachedFromVisualTree += (_, _) => { _filterCommitRevision++; CloseFilterPopup(clearQuery: false, restoreFocus: false); ClearFilterSubscriptions(); };
        DetachedFromVisualTree += (_, _) => CancelCompactMotion();
        DetachedFromVisualTree += (_, _) => _searchSession?.Unsubscribe();
        AttachedToVisualTree += (_, _) => { if (_searchSession is not null) RefreshSearch(); };
        SetCurrentValue(ItemTemplateProperty, new NavigationTemplate(this));
        ContainerClearing += ClearBindings;
        ContainerPrepared += TrackChildContainers;
        SelectionChanged += SelectionChangedCore;
        AddHandler(PointerPressedEvent, ItemPressed, RoutingStrategies.Tunnel);
        AddHandler(PointerReleasedEvent, ItemReleased, RoutingStrategies.Bubble, handledEventsToo: true);
        AddHandler(KeyDownEvent, InvokeKey, RoutingStrategies.Tunnel);
    }

    [InheritDataTypeFromItems(nameof(ItemsSource))]
    public IDataTemplate? IconTemplate { get => GetValue(IconTemplateProperty); set => SetValue(IconTemplateProperty, value); }
    [InheritDataTypeFromItems(nameof(ItemsSource))]
    public IDataTemplate? ContentTemplate { get => GetValue(ContentTemplateProperty); set => SetValue(ContentTemplateProperty, value); }
    [AssignBinding, InheritDataTypeFromItems(nameof(ItemsSource))] public BindingBase? ChildrenBinding { get => GetValue(ChildrenBindingProperty); set => SetValue(ChildrenBindingProperty, value); }
    [AssignBinding, InheritDataTypeFromItems(nameof(ItemsSource))] public BindingBase? ExpandedBinding { get => GetValue(ExpandedBindingProperty); set => SetValue(ExpandedBindingProperty, value); }
    [AssignBinding, InheritDataTypeFromItems(nameof(ItemsSource))] public BindingBase? IsGroupBinding { get => GetValue(IsGroupBindingProperty); set => SetValue(IsGroupBindingProperty, value); }
    [AssignBinding, InheritDataTypeFromItems(nameof(ItemsSource))] public BindingBase? IsItemEnabledBinding { get => GetValue(IsItemEnabledBindingProperty); set => SetValue(IsItemEnabledBindingProperty, value); }
    public double ItemHeight { get => GetValue(ItemHeightProperty); set => SetValue(ItemHeightProperty, value); }
    public double IconSize { get => GetValue(IconSizeProperty); set => SetValue(IconSizeProperty, value); }
    public double IconSpacing { get => GetValue(IconSpacingProperty); set => SetValue(IconSpacingProperty, value); }
    public double Indent { get => GetValue(IndentProperty); set => SetValue(IndentProperty, value); }
    public ICommand? ItemInvokedCommand { get => GetValue(ItemInvokedCommandProperty); set => SetValue(ItemInvokedCommandProperty, value); }
    public event EventHandler<NavMenuItemInvokedEventArgs>? ItemInvoked { add => AddHandler(ItemInvokedEvent, value); remove => RemoveHandler(ItemInvokedEvent, value); }
    protected override Type StyleKeyOverride => typeof(NavMenu);

    /// <summary>Realize only an ancestor path, then select its target data item.</summary>
    public bool SelectPath(params int[] path)
    {
        // ScrollIntoView must use current row measurements after a density or viewport change.
        var refreshLayout = _navigationLayoutChanged;
        if (refreshLayout) { UpdateLayout(); _navigationLayoutChanged = false; }
        if (UsesPopupNavigation && path.Length > 1)
        {
            if (!OpenSubmenu(path[0]) || Submenu is not { } child) return false;
            return child.SelectPath(path[1..]);
        }
        if (Virtualization.BringIntoView(this, path) is not NavMenuItem { IsEffectivelyEnabled: true, IsGroup: false } node || node.Parent is not ItemsControl owner) return false;
        SetCurrentValue(SelectedItemProperty, owner.ItemFromContainer(node));
        if (refreshLayout) node.UpdateLayout();
        node.BringIntoView(); return true;
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey) => new NavMenuItem();
    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey) => NeedsContainer<NavMenuItem>(item, out recycleKey);

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        if (container is NavMenuItem previous) _bindings.GetValue(previous, static _ => new RowBindings()).Dispose();
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is not NavMenuItem node) return;
        var bindings = _bindings.GetValue(node, static _ => new RowBindings());
        // Native ContentPresenter forgets its recycling template when a row detaches.
        // Keep the default header with its own row, rather than rebuilding all bindings on reattachment.
        if (!ReferenceEquals(item, node) && ItemTemplate is NavigationTemplate navigation &&
            (ReferenceEquals(node.HeaderTemplate, navigation) || node.HeaderTemplate is RowHeaderTemplate))
        {
            if (bindings.HeaderTemplate?.Template != navigation) bindings.HeaderTemplate = new RowHeaderTemplate(navigation);
            node.SetCurrentValue(TreeViewItem.HeaderTemplateProperty, bindings.HeaderTemplate);
        }
        // An own NavMenuItem must stay a logical child of the menu. Putting it in
        // the automatic tooltip would instead parent it to an unattached ContentControl.
        if (!ReferenceEquals(item, node))
        {
            if (item is not null)
            {
                var tip = bindings.ToolTip ??= new ContentControl();
                tip.Content = item;
                bindings.Add(tip.Bind(ContentControl.ContentTemplateProperty, CompiledBinding.Create<NavMenu, IDataTemplate?>(menu => menu.ContentTemplate, source: this)));
                node.SetCurrentValue(ToolTip.TipProperty, tip);
            }
            else node.SetCurrentValue(ToolTip.TipProperty, null);
        }
        if (ActiveSearch is not null) node.SetCurrentValue(TreeViewItem.IsExpandedProperty, node.ItemCount > 0);
        else if (ExpandedBinding is { } expanded) bindings.Add(node.Bind(TreeViewItem.IsExpandedProperty, expanded));
        if (IsGroupBinding is { } group) bindings.Add(node.Bind(NavMenuItem.IsGroupProperty, group));
        else node.SetCurrentValue(NavMenuItem.IsGroupProperty, false);
        if (IsItemEnabledBinding is { } enabled) bindings.Add(node.Bind(IsEnabledProperty, enabled));
        else node.SetCurrentValue(IsEnabledProperty, true);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        CollapsedPropertyChanged(change);
        SearchPropertyChanged(change);
        FilterPropertyChanged(change);
        // Keep small root collections attached: recycling an expanded root otherwise
        // discards its descendants' viewport measurements and remounts the whole branch.
        // Large root collections and each branch's children remain virtualized.
        if (change.Property == ChildrenBindingProperty || change.Property == ItemCountProperty)
            PseudoClasses.Set(":bounded-roots", ChildrenBinding is not null && ItemCount <= 32);
        if (change.Property == ChildrenBindingProperty || change.Property == ExpandedBindingProperty || change.Property == IsGroupBindingProperty || change.Property == IsItemEnabledBindingProperty)
            SetCurrentValue(ItemTemplateProperty, new NavigationTemplate(this));
        if (change.Property == ItemHeightProperty || change.Property == FontSizeProperty || change.Property == IndentProperty)
        {
            _navigationLayoutChanged = true;
            foreach (var node in GetRealizedTreeContainers().OfType<NavMenuItem>()) ApplyDensity(node);
        }
        if (change.Property == BoundsProperty && change.GetOldValue<Rect>().Size != change.GetNewValue<Rect>().Size)
            _navigationLayoutChanged = true;
    }

    private void ApplyDensity(NavMenuItem node)
    {
        node.SetCurrentValue(FontSizeProperty, FontSize);
        node.SetCurrentValue(MinHeightProperty, ItemHeight);
        var indent = UsesPopupNavigation ? 0d : Indent;
        if (!node.Resources.TryGetValue("One.TreeViewItemIndent", out var current) || !Equals(current, indent))
            node.Resources["One.TreeViewItemIndent"] = indent;
        node.SetCurrentValue(NavMenuItem.IsCascadeItemProperty, _popupParent is not null);
        node.SetCurrentValue(NavMenuItem.IsCompactProperty, IsCollapsed);
        node.SetCurrentValue(ToolTip.ServiceEnabledProperty, IsCollapsed && (!OpenOnHover || node.ItemCount == 0));
        node.SetCurrentValue(NavMenuItem.IsActiveAncestorProperty, UsesPopupNavigation && SelectedItem is not null && _activeRoot is not null && ReferenceEquals(ItemFromContainer(node), _activeRoot));
    }

    private void TrackChildContainers(object? sender, ContainerPreparedEventArgs args)
    {
        if (args.Container is not NavMenuItem node) return;
        node.ContainerClearing -= ClearBindings; node.ContainerClearing += ClearBindings;
        node.ContainerPrepared -= TrackChildContainers; node.ContainerPrepared += TrackChildContainers;
        node.PointerEntered -= RowPointerEntered; node.PointerEntered += RowPointerEntered;
        node.PointerExited -= RowPointerExited; node.PointerExited += RowPointerExited;
        node.PropertyChanged -= PopupAnchorPropertyChanged; node.PropertyChanged += PopupAnchorPropertyChanged;
        ApplyDensity(node);
        if (ActiveSearch is not null) node.SetCurrentValue(TreeViewItem.IsExpandedProperty, node.ItemCount > 0);
        else RestoreSearchRow(node);
    }

    private void ClearBindings(object? sender, ContainerClearingEventArgs args)
    {
        if (args.Container is NavMenuItem node)
        {
            if (_pendingHover == node) CancelHoverOpen();
            node.PointerEntered -= RowPointerEntered; node.PointerExited -= RowPointerExited; node.PropertyChanged -= PopupAnchorPropertyChanged;
            if (_submenuAnchor == node) CloseSubmenu();
            if (_pressedItem == node) { _pressedItem = null; _pressedPointer = null; }
            if (_bindings.TryGetValue(node, out var bindings)) bindings.Dispose();
            RemoveCompactMotion(node);
        }
    }

    private void SelectionChangedCore(object? sender, SelectionChangedEventArgs args)
    {
        if (_restoringSelection) { args.Handled = true; return; }
        if (SelectedItem is { } item && TreeContainerFromItem(item) is NavMenuItem { IsGroup: true })
        {
            args.Handled = true;
            _restoringSelection = true;
            try { SetCurrentValue(SelectedItemProperty, _lastNavigableItem); }
            finally { _restoringSelection = false; }
        }
        else
        {
            _lastNavigableItem = SelectedItem;
            if (SelectedItem is null) _activeRoot = null;
            else if (TreeContainerFromItem(SelectedItem) is { } selected)
            {
                var root = selected.GetLogicalAncestors().OfType<NavMenuItem>().LastOrDefault(node => node.Parent == this) ?? selected;
                _activeRoot = ItemFromContainer(root);
            }
            UpdateActiveRoots();
        }
    }

    private static NavMenuItem? Row(object? source) => source is Visual visual ? visual.GetVisualAncestors().Prepend(visual).OfType<NavMenuItem>().FirstOrDefault() : null;
    private static bool IsEmbeddedControl(object? source, NavMenuItem node) => source is Visual visual && visual.GetVisualAncestors().Prepend(visual).TakeWhile(control => control != node).Any(control => control is Button or TextBox or SelectingItemsControl);

    private bool IsPopupEmbeddedControl(object? source, NavMenuItem node) => IsEmbeddedControl(source, node) &&
        !(UsesPopupNavigation && source is Visual visual && visual.GetVisualAncestors().Prepend(visual).TakeWhile(control => control != node).OfType<ToggleButton>().Any(button => button.Name == "PART_ExpandCollapseChevron"));

    private bool Owns(NavMenuItem node) => node.GetLogicalAncestors().OfType<NavMenu>().FirstOrDefault() == this;

    private void ItemPressed(object? sender, PointerPressedEventArgs args)
    {
        _pressedItem = null; _pressedPointer = null;
        var node = Row(args.Source);
        if (node is not { IsEffectivelyEnabled: true } || !Owns(node) || !args.GetCurrentPoint(this).Properties.IsLeftButtonPressed || IsPopupEmbeddedControl(args.Source, node)) return;
        if (UsesPopupNavigation)
        { _pressedItem = node; _pressedPointer = args.Pointer; node.Focus(); args.Handled = true; }
        else if (node.IsGroup)
        {
            node.SetCurrentValue(TreeViewItem.IsExpandedProperty, !node.IsExpanded); node.Focus(); args.Handled = true;
        }
        else { _pressedItem = node; _pressedPointer = args.Pointer; }
    }

    private void ItemReleased(object? sender, PointerReleasedEventArgs args)
    {
        var node = Row(args.Source);
        var pressed = _pressedItem; var pointer = _pressedPointer;
        _pressedItem = null; _pressedPointer = null;
        if (args.InitialPressMouseButton == MouseButton.Left && pointer == args.Pointer && node == pressed && node is { IsEffectivelyEnabled: true } && Owns(node) && !IsPopupEmbeddedControl(args.Source, node))
        {
            if (UsesPopupNavigation && node.ItemCount > 0) ShowSubmenu(node);
            else if (!node.IsGroup) Invoke(node);
        }
    }

    private void InvokeKey(object? sender, KeyEventArgs args)
    {
        if (UsesPopupNavigation || args.Key is not (Key.Enter or Key.Space) || Row(args.Source) is not { IsEffectivelyEnabled: true } node || !Owns(node) || IsEmbeddedControl(args.Source, node)) return;
        if (node.IsGroup) node.SetCurrentValue(TreeViewItem.IsExpandedProperty, !node.IsExpanded);
        else Invoke(node);
        args.Handled = true;
    }

    private void Invoke(NavMenuItem node)
    {
        if (node.Parent is not ItemsControl parent || parent.ItemFromContainer(node) is not { } item) return;
        SetCurrentValue(SelectedItemProperty, item);
        RaiseEvent(new NavMenuItemInvokedEventArgs(ItemInvokedEvent, item));
        if (ItemInvokedCommand is { } command && command.CanExecute(item)) command.Execute(item);
    }

    private static bool Positive(double value) => double.IsFinite(value) && value > 0;
    private sealed class EmptySubscription : IDisposable
    {
        public static EmptySubscription Instance { get; } = new();
        public void Dispose() { }
    }

    private sealed class RowBindings
    {
        public ContentControl? ToolTip;
        public RowHeaderTemplate? HeaderTemplate;
        private readonly List<IDisposable> _values = [];
        public void Add(IDisposable value) => _values.Add(value);
        public void Dispose() { foreach (var value in _values) value.Dispose(); _values.Clear(); }
    }

    private sealed class RowHeaderTemplate(NavigationTemplate template) : ITreeDataTemplate
    {
        internal NavigationTemplate Template => template;
        private Control? _content;
        private readonly Border _empty = new();
        public bool Match(object? data) => true;
        public Control Build(object? data) => data is null ? _empty : _content = template.Build(data, _content);
        public IDisposable BindChildren(AvaloniaObject target, AvaloniaProperty targetProperty, object item) => template.BindChildren(target, targetProperty, item);
    }

    private sealed class IconGapVisibilityConverter : IMultiValueConverter
    {
        public static IconGapVisibilityConverter Instance { get; } = new();
        public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture) => values.Count == 2 && values[0] is IDataTemplate && values[1] is false;
    }

    private sealed class RecyclingTemplateConverter(IDataTemplate? fallback = null) : IValueConverter
    {
        private IDataTemplate? _source;
        private IDataTemplate? _wrapped;
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not IDataTemplate template) { _source = null; _wrapped = null; return fallback; }
            if (!ReferenceEquals(_source, template)) { _source = template; _wrapped = new RowContentTemplate(template); }
            return _wrapped;
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
    }
    private sealed class RowContentTemplate(IDataTemplate template) : IDataTemplate
    {
        private Control? _content;
        private readonly Border _empty = new();
        public bool Match(object? data) => template.Match(data);
        public Control? Build(object? data)
        {
            if (data is null) return _empty;
            return _content = template is IRecyclingDataTemplate recycling ? recycling.Build(data, _content) : template.Build(data);
        }
    }
    private sealed class SearchContentTemplate(NavMenu owner) : IRecyclingDataTemplate
    {
        private NavMenuSearchText? _text;
        public bool Match(object? data) => true;
        public Control Build(object? data)
        {
            if (_text is null)
            {
                _text = new NavMenuSearchText { TextTrimming = global::Avalonia.Media.TextTrimming.CharacterEllipsis };
                if (owner.SearchTextBinding is { } binding) _text.Bind(NavMenuSearchText.SourceTextProperty, binding);
            }
            _text.DataContext = data;
            if (owner.SearchTextBinding is null) _text.SourceText = data?.ToString();
            return _text;
        }
        public Control Build(object? data, Control? existing) => Build(data);
    }

    private sealed class NavigationTemplate(NavMenu owner) : ITreeDataTemplate, IRecyclingDataTemplate
    {
        public bool Match(object? data) => true;
        public IDisposable BindChildren(AvaloniaObject target, AvaloniaProperty targetProperty, object item)
        {
            if (owner.ActiveSearch is { } search) { target.SetCurrentValue(targetProperty, search.Children(item)); return EmptySubscription.Instance; }
            return owner.ChildrenBinding is { } binding ? target.Bind(targetProperty, binding) : EmptySubscription.Instance;
        }
        public Control Build(object? data)
        {
            var icon = new ContentPresenter { Name = "NavMenuIcon", Content = data, VerticalAlignment = VerticalAlignment.Center, HorizontalContentAlignment = HorizontalAlignment.Stretch, VerticalContentAlignment = VerticalAlignment.Stretch };
            icon.Bind(ContentPresenter.ContentTemplateProperty, CompiledBinding.Create<NavMenu, IDataTemplate?>(menu => menu.DisplayIconTemplate, source: owner, converter: new RecyclingTemplateConverter()));
            icon.Bind(WidthProperty, CompiledBinding.Create<NavMenu, double>(menu => menu.IconSize, source: owner));
            icon.Bind(HeightProperty, CompiledBinding.Create<NavMenu, double>(menu => menu.IconSize, source: owner));
            icon.Bind(IsVisibleProperty, CompiledBinding.Create<NavMenu, IDataTemplate?>(menu => menu.DisplayIconTemplate, source: owner, converter: ObjectConverters.IsNotNull));
            var gap = new Border { [Grid.ColumnProperty] = 1 };
            gap.Bind(WidthProperty, CompiledBinding.Create<NavMenu, double>(menu => menu.IconSpacing, source: owner));
            gap.Bind(IsVisibleProperty, new MultiBinding
            {
                Converter = IconGapVisibilityConverter.Instance,
                Bindings = { CompiledBinding.Create<NavMenu, IDataTemplate?>(menu => menu.DisplayIconTemplate, source: owner), CompiledBinding.Create<NavMenu, bool>(menu => menu.IsCollapsed, source: owner) }
            });
            var text = new ContentPresenter { Name = "NavMenuContent", Content = data, [Grid.ColumnProperty] = 2, VerticalAlignment = VerticalAlignment.Center, HorizontalContentAlignment = HorizontalAlignment.Stretch };
            text.Bind(ContentPresenter.ContentTemplateProperty, CompiledBinding.Create<NavMenu, IDataTemplate?>(menu => menu.ContentTemplate, source: owner, converter: new RecyclingTemplateConverter(new SearchContentTemplate(owner))));
            text.Bind(IsVisibleProperty, CompiledBinding.Create<NavMenu, bool>(menu => menu.IsCollapsed, source: owner, converter: BoolConverters.Not));
            TextBlock.SetTextTrimming(text, global::Avalonia.Media.TextTrimming.CharacterEllipsis);
            return new Grid { ColumnDefinitions = ColumnDefinitions.Parse("Auto,Auto,*"), DataContext = data, Children = { icon, gap, text } };
        }
        public Control Build(object? data, Control? existing)
        {
            if (data is null) return new Border();
            if (existing is not Grid grid || grid.Children.Count != 3) return Build(data);
            grid.DataContext = data;
            ((ContentPresenter)grid.Children[0]).Content = data;
            ((ContentPresenter)grid.Children[2]).Content = data;
            return grid;
        }
    }
}

public sealed class NavMenuItemInvokedEventArgs(RoutedEvent routedEvent, object item) : RoutedEventArgs(routedEvent)
{
    public object Item { get; } = item;
}
