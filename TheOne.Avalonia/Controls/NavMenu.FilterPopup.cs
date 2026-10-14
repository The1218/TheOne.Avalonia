using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Input;
using global::Avalonia.Interactivity;
using global::Avalonia.Threading;
using global::Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

public partial class NavMenu
{
    public static readonly DirectProperty<NavMenu, bool> IsFilterOpenProperty = AvaloniaProperty.RegisterDirect<NavMenu, bool>(nameof(IsFilterOpen), menu => menu.IsFilterOpen);
    public bool IsFilterOpen => _isFilterOpen;
    private bool _isFilterOpen, _closingFilter;
    private Button? _filterButton;
    private Popup? _filterPopup;
    private Panel? _filterInlineHost, _filterPopupHost;
    private TopLevel? _filterTopLevel;

    private void InitializeFilterPopup(INameScope scope)
    {
        _filterButton = scope.Find<Button>("PART_FilterButton");
        _filterPopup = scope.Find<Popup>("PART_FilterPopup");
        _filterInlineHost = scope.Find<Panel>("PART_FilterInlineHost");
        _filterPopupHost = scope.Find<Panel>("PART_FilterPopupHost");
        if (_filterButton is not null) _filterButton.Click += FilterButtonClick;
        if (_filterPopup is not null)
        {
            _filterPopup.PlacementTarget = _filterButton;
            _filterPopup.Closed += FilterPopupClosed;
        }
        _filterBox?.AddHandler(KeyDownEvent, FilterPopupKeyDown, RoutingStrategies.Tunnel, handledEventsToo: true);
        MoveFilterBox();
    }

    private void ReleaseFilterPopup()
    {
        CloseFilterPopup(clearQuery: false, restoreFocus: false);
        if (_filterButton is not null) _filterButton.Click -= FilterButtonClick;
        if (_filterPopup is not null) _filterPopup.Closed -= FilterPopupClosed;
        _filterBox?.RemoveHandler(KeyDownEvent, FilterPopupKeyDown);
    }

    private void MoveFilterBox()
    {
        if (_filterBox is null) return;
        var target = IsCollapsed ? _filterPopupHost : _filterInlineHost;
        if (target is null || ReferenceEquals(_filterBox.Parent, target)) return;
        _filterBox.SetCurrentValue(AutoCompleteBox.IsDropDownOpenProperty, false);
        if (_filterBox.Parent is Panel previous) previous.Children.Remove(_filterBox);
        target.Children.Add(_filterBox);
        _filterBox.SetCurrentValue(IsVisibleProperty, !IsCollapsed);
    }

    private void FilterButtonClick(object? sender, RoutedEventArgs args)
    {
        if (IsFilterOpen) CloseFilter();
        else OpenFilter();
    }

    /// <summary>Focuses the inline filter, or opens its compact search popup.</summary>
    public bool OpenFilter()
    {
        if (!ShowFilter || !IsEffectivelyEnabled || !this.IsAttachedToVisualTree() || _filterBox is null || _popupParent is not null) return false;
        if (!IsCollapsed) return _filterBox.Focus();
        if (_filterPopup is null) return false;
        CloseSubmenu();
        MoveFilterBox();
        _filterBox.SetCurrentValue(IsVisibleProperty, true);
        _filterTopLevel = TopLevel.GetTopLevel(this);
        if (_filterTopLevel is not null) { _filterTopLevel.SizeChanged -= FilterHostSizeChanged; _filterTopLevel.SizeChanged += FilterHostSizeChanged; }
        ApplyFilterPopupEnvironment();
        SetAndRaise(IsFilterOpenProperty, ref _isFilterOpen, true);
        _filterPopup.SetCurrentValue(Popup.IsOpenProperty, true);
        Dispatcher.UIThread.Post(() =>
        {
            if (!IsFilterOpen || !IsCollapsed || !ShowFilter) return;
            _filterBox.Focus();
            if (!string.IsNullOrWhiteSpace(SearchText)) _filterBox.SetCurrentValue(AutoCompleteBox.IsDropDownOpenProperty, true);
        }, DispatcherPriority.Loaded);
        return true;
    }

    /// <summary>Dismisses compact search and restores the pre-search navigation state.</summary>
    public void CloseFilter() => CloseFilterPopup(clearQuery: true, restoreFocus: true);

    private void CloseFilterPopup(bool clearQuery, bool restoreFocus)
    {
        var wasOpen = IsFilterOpen;
        _closingFilter = true;
        try
        {
            _filterBox?.SetCurrentValue(AutoCompleteBox.IsDropDownOpenProperty, false);
            _filterPopup?.SetCurrentValue(Popup.IsOpenProperty, false);
            if (IsCollapsed) _filterBox?.SetCurrentValue(IsVisibleProperty, false);
            SetAndRaise(IsFilterOpenProperty, ref _isFilterOpen, false);
            if (_filterTopLevel is not null) _filterTopLevel.SizeChanged -= FilterHostSizeChanged;
            _filterTopLevel = null;
            if (wasOpen && clearQuery) SetCurrentValue(SearchTextProperty, "");
        }
        finally { _closingFilter = false; }
        if (wasOpen && restoreFocus && _filterButton is { IsEffectivelyVisible: true }) _filterButton.Focus();
    }

    private void FilterPopupClosed(object? sender, EventArgs args)
    {
        if (!_closingFilter) CloseFilter();
    }

    private void FilterPopupKeyDown(object? sender, KeyEventArgs args)
    {
        if (IsFilterOpen && args.Key == Key.Escape && args.KeyModifiers == KeyModifiers.None) { CloseFilter(); args.Handled = true; }
    }

    private void FilterHostSizeChanged(object? sender, SizeChangedEventArgs args) => ApplyFilterPopupEnvironment();
    private void ApplyFilterPopupEnvironment()
    {
        if (_filterPopup?.Child is not Control child) return;
        _filterPopup.Width = Math.Min(360, Math.Max(1, (_filterTopLevel?.ClientSize.Width ?? 376) - 16));
        child.SetCurrentValue(ThemeVariantScope.RequestedThemeVariantProperty, ActualThemeVariant);
        ControlSizing.SetSize(child, ControlSizing.GetSize(this));
        if (_filterBox is not null) _filterBox.MaxDropDownHeight = Math.Min(320, Math.Max(40, (_filterTopLevel?.ClientSize.Height ?? 480) - 120));
    }

    private void FilterPopupPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == IsCollapsedProperty)
        {
            var hadFocus = _filterBox?.IsKeyboardFocusWithin == true;
            CloseFilterPopup(clearQuery: false, restoreFocus: false);
            MoveFilterBox();
            if (hadFocus) { if (IsCollapsed) _filterButton?.Focus(); else _filterBox?.Focus(); }
        }
        if (change.Property == ShowFilterProperty && !ShowFilter || change.Property == IsEffectivelyEnabledProperty && !IsEffectivelyEnabled)
            CloseFilterPopup(clearQuery: true, restoreFocus: false);
        if (IsFilterOpen && (change.Property == ThemeVariantScope.ActualThemeVariantProperty || change.Property == ControlSizing.SizeProperty)) ApplyFilterPopupEnvironment();
    }
}
