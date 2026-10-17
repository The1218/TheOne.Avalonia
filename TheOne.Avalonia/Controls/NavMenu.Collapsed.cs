using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

public partial class NavMenu
{
    public static readonly StyledProperty<bool> IsCollapsedProperty =
        AvaloniaProperty.Register<NavMenu, bool>(nameof(IsCollapsed), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<double> CollapsedWidthProperty =
        AvaloniaProperty.Register<NavMenu, double>(nameof(CollapsedWidth), 52, validate: Positive);

    public static readonly StyledProperty<double> SubmenuWidthProperty =
        AvaloniaProperty.Register<NavMenu, double>(nameof(SubmenuWidth), 280, validate: Positive);

    public static readonly StyledProperty<double> SubmenuMaxHeightProperty =
        AvaloniaProperty.Register<NavMenu, double>(nameof(SubmenuMaxHeight), 320, validate: Positive);

    public static readonly DirectProperty<NavMenu, bool> IsSubmenuOpenProperty =
        AvaloniaProperty.RegisterDirect<NavMenu, bool>(nameof(IsSubmenuOpen), menu => menu.IsSubmenuOpen);

    public static readonly DirectProperty<NavMenu, NavMenu?> SubmenuProperty =
        AvaloniaProperty.RegisterDirect<NavMenu, NavMenu?>(nameof(Submenu), menu => menu.Submenu);

    public static readonly DirectProperty<NavMenu, IDataTemplate?> DisplayIconTemplateProperty =
        AvaloniaProperty.RegisterDirect<NavMenu, IDataTemplate?>(nameof(DisplayIconTemplate),
            menu => menu.DisplayIconTemplate);

    private static readonly IDataTemplate FallbackIconTemplate = new FuncDataTemplate<object>((_, _) => new PathIcon
    {
        Data = Geometry.Parse(
            "M2,2 L7,2 L7,7 L2,7 Z M11,2 L16,2 L16,7 L11,7 Z M2,11 L7,11 L7,16 L2,16 Z M11,11 L16,11 L16,16 L11,16 Z")
    });

    public static readonly DirectProperty<NavMenu, bool> IsCompactSubmenuProperty =
        AvaloniaProperty.RegisterDirect<NavMenu, bool>(nameof(IsCompactSubmenu), menu => menu.IsCompactSubmenu);

    public static readonly DirectProperty<NavMenu, bool> SubmenuOpensLeftProperty =
        AvaloniaProperty.RegisterDirect<NavMenu, bool>(nameof(SubmenuOpensLeft), menu => menu.SubmenuOpensLeft);

    private readonly List<IDisposable> _submenuBindings = [];
    private object? _activeRoot;
    private TopLevel? _dismissTopLevel;
    private IDataTemplate? _displayIconTemplate;
    private bool _isCompactSubmenu, _submenuOpensLeft;
    private bool _isSubmenuOpen;
    private Border? _placementAnchor;
    private TopLevel? _placementTopLevel;
    private NavMenu? _popupParent;
    private NavMenu? _submenu;
    private NavMenuItem? _submenuAnchor;
    private Button? _submenuBackButton;
    private Rect _submenuBounds;
    private bool _submenuCovered;
    private Flyout? _submenuFlyout;
    private bool _suppressSubmenuFocusReturn;
    private bool _updatingPlacement;
    internal bool UsesPopupNavigation => IsCollapsed || _popupParent is not null;

    public bool IsCompactSubmenu
    {
        get => _isCompactSubmenu;
        private set => SetAndRaise(IsCompactSubmenuProperty, ref _isCompactSubmenu, value);
    }

    public bool SubmenuOpensLeft
    {
        get => _submenuOpensLeft;
        private set => SetAndRaise(SubmenuOpensLeftProperty, ref _submenuOpensLeft, value);
    }

    public bool IsCollapsed
    {
        get => GetValue(IsCollapsedProperty);
        set => SetValue(IsCollapsedProperty, value);
    }

    public double CollapsedWidth
    {
        get => GetValue(CollapsedWidthProperty);
        set => SetValue(CollapsedWidthProperty, value);
    }

    public double SubmenuWidth
    {
        get => GetValue(SubmenuWidthProperty);
        set => SetValue(SubmenuWidthProperty, value);
    }

    public double SubmenuMaxHeight
    {
        get => GetValue(SubmenuMaxHeightProperty);
        set => SetValue(SubmenuMaxHeightProperty, value);
    }

    public bool IsSubmenuOpen => _isSubmenuOpen;

    /// <summary>The currently open, virtualized submenu. Null when closed.</summary>
    public NavMenu? Submenu => _submenu;

    /// <summary>The icon template including the compact-mode fallback glyph.</summary>
    public IDataTemplate? DisplayIconTemplate => _displayIconTemplate;

    /// <summary>Open a collapsed root's children without selecting or expanding its model.</summary>
    public bool OpenSubmenu(int rootIndex)
    {
        if (!UsesPopupNavigation || !IsEffectivelyEnabled || !this.IsAttachedToVisualTree() || rootIndex < 0 ||
            rootIndex >= ItemCount) return false;
        ScrollIntoView(rootIndex);
        UpdateLayout();
        return ContainerFromIndex(rootIndex) is NavMenuItem node && ShowSubmenu(node);
    }

    public void CloseSubmenu()
    {
        CancelHover();
        _submenu?.CloseSubmenu();
        _submenuFlyout?.Hide();
    }

    private bool ShowSubmenu(NavMenuItem node, bool focus = true)
    {
        _hoverSuppressedItem = null;
        if (!node.IsEffectivelyEnabled || node.ItemCount == 0 || node.Parent != this ||
            ChildrenBinding is null) return false;
        if (_submenuAnchor == node && IsSubmenuOpen)
        {
            CancelHoverClose();
            if (focus)
            {
                _submenuFlyout!.ShowMode = FlyoutShowMode.Standard;
                FocusSubmenu(_submenu!);
            }

            return true;
        }

        _suppressSubmenuFocusReturn = true;
        try
        {
            CloseSubmenu();
        }
        finally
        {
            _suppressSubmenuFocusReturn = false;
        }

        var data = ItemFromContainer(node);
        var child = new NavMenu
        {
            _popupParent = this, Name = "PART_Submenu", SearchText = SearchText, SearchTextBinding = SearchTextBinding,
            DataContext = data, ChildrenBinding = ChildrenBinding,
            ExpandedBinding = ActiveSearch is null ? ExpandedBinding : null, IsGroupBinding = IsGroupBinding,
            IsItemEnabledBinding = IsItemEnabledBinding
        };
        Virtualization.ShareTreeState(this, child);
        _submenuBindings.Add(child.Bind(OpenOnHoverProperty,
            CompiledBinding.Create<NavMenu, bool>(menu => menu.OpenOnHover, this)));
        _submenuBindings.Add(child.Bind(HoverOpenDelayProperty,
            CompiledBinding.Create<NavMenu, TimeSpan>(menu => menu.HoverOpenDelay, this)));
        _submenuBindings.Add(child.Bind(HoverCloseDelayProperty,
            CompiledBinding.Create<NavMenu, TimeSpan>(menu => menu.HoverCloseDelay, this)));
        _submenuBindings.Add(child.Bind(SubmenuWidthProperty,
            CompiledBinding.Create<NavMenu, double>(menu => menu.SubmenuWidth, this)));
        _submenuBindings.Add(child.Bind(SubmenuMaxHeightProperty,
            CompiledBinding.Create<NavMenu, double>(menu => menu.SubmenuMaxHeight, this)));
        if (ChildrenBinding is { } children) _submenuBindings.Add(child.Bind(ItemsSourceProperty, children));
        _submenuBindings.Add(child.Bind(IconTemplateProperty,
            CompiledBinding.Create<NavMenu, IDataTemplate?>(menu => menu.IconTemplate, this)));
        _submenuBindings.Add(child.Bind(ContentTemplateProperty,
            CompiledBinding.Create<NavMenu, IDataTemplate?>(menu => menu.ContentTemplate, this)));
        _submenuBindings.Add(child.Bind(TransitionDurationProperty,
            CompiledBinding.Create<NavMenu, TimeSpan>(menu => menu.TransitionDuration, this)));
        _submenuBindings.Add(child.Bind(FontSizeProperty,
            CompiledBinding.Create<NavMenu, double>(menu => menu.FontSize, this)));
        _submenuBindings.Add(child.Bind(ItemHeightProperty,
            CompiledBinding.Create<NavMenu, double>(menu => menu.ItemHeight, this)));
        _submenuBindings.Add(child.Bind(IconSizeProperty,
            CompiledBinding.Create<NavMenu, double>(menu => menu.IconSize, this)));
        _submenuBindings.Add(child.Bind(IconSpacingProperty,
            CompiledBinding.Create<NavMenu, double>(menu => menu.IconSpacing, this)));
        _submenuBindings.Add(child.Bind(IndentProperty,
            CompiledBinding.Create<NavMenu, double>(menu => menu.Indent, this)));
        var heading = new ContentPresenter
        {
            Name = "PART_SubmenuHeader", Content = data, FontWeight = FontWeight.SemiBold,
            Margin = new Thickness(0, 0, 0, 8)
        };
        _submenuBindings.Add(heading.Bind(FontSizeProperty,
            CompiledBinding.Create<NavMenu, double>(menu => menu.FontSize, this)));
        _submenuBindings.Add(heading.Bind(ContentPresenter.ContentTemplateProperty,
            CompiledBinding.Create<NavMenu, IDataTemplate?>(menu => menu.ContentTemplate, this,
                new RecyclingTemplateConverter(new SearchContentTemplate(this)))));
        var back = new Button
            { Name = "PART_SubmenuBack", Content = "‹ 返回", IsVisible = false, Margin = new Thickness(0, 0, 8, 8) };
        back.Click += (_, _) => CloseSubmenu();
        _submenuBackButton = back;
        var header = new Grid { ColumnDefinitions = ColumnDefinitions.Parse("Auto,*"), Children = { back, heading } };
        Grid.SetColumn(heading, 1);
        var panel = new Grid { RowDefinitions = RowDefinitions.Parse("Auto,*"), Children = { header, child } };
        Grid.SetRow(child, 1);
        NavMenuSearchText.SetHighlightQuery(panel, SearchText);
        var flyout = new Flyout
        {
            Content = panel, ShowMode = focus ? FlyoutShowMode.Standard : FlyoutShowMode.Transient,
            Placement = PlacementMode.RightEdgeAlignedTop, HorizontalOffset = 8
        };
        flyout.Popup.ShouldUseOverlayLayer = true;
        // Native light-dismiss overlays intercept hover over ancestor/sibling menus.
        // One root-level outside-press handler dismisses the entire pass-through chain.
        flyout.Popup.IsLightDismissEnabled = false;
        panel.PointerEntered += (_, args) =>
        {
            if (args.Pointer.Type == PointerType.Mouse) CancelHoverCloseChain();
        };
        panel.PointerExited += (_, args) =>
        {
            if (args.Pointer.Type == PointerType.Mouse) ScheduleHoverCloseChain();
        };
        // One bounded viewport, rather than a second scrolling presenter around the tree.
        flyout.Opening += (_, _) =>
        {
            if (flyout.Popup.Child is Control presenter)
            {
                ScrollViewer.SetHorizontalScrollBarVisibility(presenter, ScrollBarVisibility.Disabled);
                ScrollViewer.SetVerticalScrollBarVisibility(presenter, ScrollBarVisibility.Disabled);
            }
        };
        flyout.Closed += (_, _) => ReleaseSubmenu(flyout);
        var initializingSelection = true;
        child.SelectionChanged += (_, args) =>
        {
            args.Handled = true;
            if (initializingSelection || child.SelectedItem is not { } selected) return;
            _activeRoot = data;
            SetCurrentValue(SelectedItemProperty, selected);
            UpdateActiveRoots();
        };
        child.ItemInvoked += (_, args) =>
        {
            // The child's routed event can also reach the owner through the popup parent.
            args.Handled = true;
            _activeRoot = data;
            SetCurrentValue(SelectedItemProperty, args.Item);
            UpdateActiveRoots();
            RaiseEvent(new NavMenuItemInvokedEventArgs(ItemInvokedEvent, args.Item));
            if (ItemInvokedCommand is { } command && command.CanExecute(args.Item)) command.Execute(args.Item);
            CloseSubmenu();
        };
        _submenuAnchor = node;
        _submenuFlyout = flyout;
        SetAndRaise(SubmenuProperty, ref _submenu, child);
        SetAndRaise(IsSubmenuOpenProperty, ref _isSubmenuOpen, true);
        if (_popupParent is null && TopLevel.GetTopLevel(this) is { } topLevel)
        {
            _dismissTopLevel = topLevel;
            topLevel.AddHandler(PointerPressedEvent, OutsidePopupPressed, RoutingStrategies.Tunnel, true);
            topLevel.AddHandler(PointerMovedEvent, PopupPointerMoved, RoutingStrategies.Tunnel, true);
        }

        ApplySubmenuEnvironment();
        child.SetCurrentValue(SelectedItemProperty, SelectedItem);
        initializingSelection = false;
        _placementAnchor = node.GetVisualDescendants().OfType<Border>()
            .First(border => border.Name == "PART_LayoutRoot" && border.TemplatedParent == node);
        flyout.ShowAt(_placementAnchor);
        _placementTopLevel = TopLevel.GetTopLevel(this);
        if (_placementTopLevel is not null)
        {
            // An overlay parent's Canvas position can change without changing the row's
            // local Bounds. Use a window-relative rectangle so synchronous SelectPath
            // cascades also follow their ancestors after the first render/layout pass.
            flyout.Popup.PlacementTarget = _placementTopLevel;
            _placementAnchor.LayoutUpdated += UpdateSubmenuPlacement;
            _placementTopLevel.SizeChanged += PlacementHostSizeChanged;
            UpdateSubmenuPlacement(this, EventArgs.Empty);
        }

        if (focus) FocusSubmenu(child);
        return true;
    }

    private void FocusSubmenu(NavMenu child)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (_submenu != child || !IsSubmenuOpen) return;
            child.UpdateLayout();
            var selected = child.SelectedItem is { } item ? child.TreeContainerFromItem(item) : null;
            (selected ?? child.GetRealizedContainers().FirstOrDefault(container => container.IsEffectivelyEnabled))
                ?.Focus();
        }, DispatcherPriority.Loaded);
    }

    private void ReleaseSubmenu(Flyout flyout)
    {
        if (_submenuFlyout != flyout) return;
        CancelHover();
        _submenu?.CloseSubmenu();
        if (_placementAnchor is not null) _placementAnchor.LayoutUpdated -= UpdateSubmenuPlacement;
        if (_placementTopLevel is not null) _placementTopLevel.SizeChanged -= PlacementHostSizeChanged;
        _placementAnchor = null;
        _placementTopLevel = null;
        if (IsCompactSubmenu) _hoverSuppressedItem = _submenuAnchor;
        _popupParent?.SetSubmenuCovered(false);
        _submenuBackButton = null;
        IsCompactSubmenu = false;
        SubmenuOpensLeft = false;
        _dismissTopLevel?.RemoveHandler(PointerPressedEvent, OutsidePopupPressed);
        _dismissTopLevel?.RemoveHandler(PointerMovedEvent, PopupPointerMoved);
        _dismissTopLevel = null;
        var restoreFocus = !_suppressSubmenuFocusReturn && flyout.ShowMode == FlyoutShowMode.Standard;
        var anchor = _submenuAnchor;
        _submenuAnchor = null;
        _submenuFlyout = null;
        foreach (var binding in _submenuBindings) binding.Dispose();
        _submenuBindings.Clear();
        SetAndRaise(IsSubmenuOpenProperty, ref _isSubmenuOpen, false);
        SetAndRaise(SubmenuProperty, ref _submenu, null);
        flyout.Content = null;
        if (restoreFocus && anchor is { IsEffectivelyVisible: true, IsEffectivelyEnabled: true }) anchor.Focus();
    }

    private void OutsidePopupPressed(object? sender, PointerPressedEventArgs args)
    {
        if (!new Rect(Bounds.Size).Contains(args.GetPosition(this)) && !ContainsPopupPoint(args)) CloseSubmenu();
    }

    private bool ContainsPopupPoint(PointerEventArgs args)
    {
        return _submenuFlyout?.Popup.Child is { IsEffectivelyVisible: true, IsHitTestVisible: true } presenter &&
               new Rect(presenter.Bounds.Size).Contains(args.GetPosition(presenter)) ||
               _submenu?.ContainsPopupPoint(args) == true;
    }

    private void ApplySubmenuEnvironment()
    {
        if (_submenu is not { } child || _submenuFlyout?.Content is not Control panel) return;
        panel.SetCurrentValue(ThemeVariantScope.RequestedThemeVariantProperty, ActualThemeVariant);
        ControlSizing.SetSize(panel, ControlSizing.GetSize(this));
        Motion.SetEnabled(panel, Motion.GetEnabled(this));
    }

    private void CollapsedPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        CompactMotionPropertyChanged(change);
        if (change.Property == OpenOnHoverProperty)
        {
            if (!OpenOnHover) CancelHover();
            foreach (var node in GetRealizedTreeContainers().OfType<NavMenuItem>()) ApplyDensity(node);
        }

        if (change.Property == IsCollapsedProperty)
        {
            BeginCompactMotion();
            CloseSubmenu();
            PseudoClasses.Set(":collapsed", IsCollapsed);
            BeginWidthMotion();
            foreach (var node in GetRealizedTreeContainers().OfType<NavMenuItem>()) ApplyDensity(node);
            InvalidateMeasure();
        }

        if (change.Property == IsCollapsedProperty || change.Property == IconTemplateProperty)
            SetAndRaise(DisplayIconTemplateProperty, ref _displayIconTemplate,
                IconTemplate ?? (IsCollapsed ? FallbackIconTemplate : null));
        if (change.Property == ThemeVariantScope.ActualThemeVariantProperty ||
            change.Property == ControlSizing.SizeProperty ||
            change.Property == Motion.EnabledProperty) ApplySubmenuEnvironment();
        if ((change.Property == IsEffectivelyEnabledProperty && !IsEffectivelyEnabled) ||
            (change.Property == IsEnabledProperty && !IsEnabled) || change.Property == ItemsSourceProperty ||
            change.Property == ChildrenBindingProperty || change.Property == ExpandedBindingProperty ||
            change.Property == IsGroupBindingProperty ||
            change.Property == IsItemEnabledBindingProperty) CloseSubmenu();
    }

    private void UpdateActiveRoots()
    {
        foreach (var root in GetRealizedContainers().OfType<NavMenuItem>())
            root.SetCurrentValue(NavMenuItem.IsActiveAncestorProperty,
                UsesPopupNavigation && SelectedItem is not null && _activeRoot is not null &&
                ReferenceEquals(ItemFromContainer(root), _activeRoot));
    }

    protected override void OnKeyDown(KeyEventArgs args)
    {
        if (UsesPopupNavigation)
        {
            CollapsedKey(this, args);
            if (args.Handled) return;
        }

        base.OnKeyDown(args);
    }

    internal void HandlePopupKey(KeyEventArgs args)
    {
        CollapsedKey(this, args);
    }

    private void CollapsedKey(object? sender, KeyEventArgs args)
    {
        if (!UsesPopupNavigation || !IsEffectivelyEnabled || Row(args.Source) is not { } row || row.Parent != this ||
            IsPopupEmbeddedControl(args.Source, row)) return;
        if ((args.KeyModifiers & KeyModifiers.Alt) != 0) return;
        var index = IndexFromContainer(row);
        var target = args.Key switch
        {
            Key.Home => 0, Key.End => ItemCount - 1, Key.Up => index - 1, Key.Down => index + 1, _ => -1
        };
        if (args.Key is Key.Home or Key.End or Key.Up or Key.Down)
        {
            var step = args.Key is Key.End or Key.Up ? -1 : 1;
            while (target >= 0 && target < ItemCount)
            {
                ScrollIntoView(target);
                UpdateLayout();
                if (ContainerFromIndex(target) is NavMenuItem { IsEffectivelyEnabled: true } next)
                {
                    if (!next.IsGroup && (args.KeyModifiers & (KeyModifiers.Control | KeyModifiers.Meta)) == 0)
                        SetCurrentValue(SelectedItemProperty, ItemFromContainer(next));
                    next.Focus();
                    break;
                }

                target += step;
            }

            args.Handled = true;
        }
        else if (args.Key is Key.Enter or Key.Space or Key.Right)
        {
            if (row.ItemCount > 0) ShowSubmenu(row);
            else if (args.Key != Key.Right && !row.IsGroup) Invoke(row);
            args.Handled = true;
        }
        else if (args.Key is Key.Left or Key.Escape)
        {
            (_popupParent ?? this).CloseSubmenu();
            args.Handled = true;
        }
    }
}
