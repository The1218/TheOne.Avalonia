using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

public partial class TreeSelect
{
    private Popup? _popup;
    private Control? _popupContent;
    private Border? _input;
    private Button? _open, _clear;
    private ItemsControl? _tags;
    private ListBox? _results;
    private TextBox? _search;
    private TopLevel? _topLevel;
    private int _focusGeneration;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        CloseDropDown(false);
        if (_popup is not null) { _popup.Opened -= PopupOpened; _popup.Closed -= PopupClosed; }
        if (_popupContent is not null)
        {
            _popupContent.RemoveHandler(PointerWheelChangedEvent, ContainWheel);
            _popupContent.RemoveHandler(ScrollGestureEvent, ContainGesture);
            _popupContent.RemoveHandler(ScrollGestureInertiaStartingEvent, ContainPhase);
            _popupContent.RemoveHandler(ScrollGestureEndedEvent, ContainPhase);
        }
        if (_results is not null)
        {
            _results.RemoveHandler(PointerPressedEvent, RowPressed);
            _results.RemoveHandler(KeyDownEvent, PopupKeyDown);
            _results.RemoveHandler(Button.ClickEvent, ExpandClicked);
        }
        _search?.RemoveHandler(KeyDownEvent, PopupKeyDown);
        _tags?.RemoveHandler(Button.ClickEvent, RemoveClicked);
        if (_input is not null) _input.PointerPressed -= InputPressed;
        if (_open is not null) _open.Click -= OpenClicked;
        if (_clear is not null) _clear.Click -= ClearClicked;
        base.OnApplyTemplate(e);
        _popup = e.NameScope.Find<Popup>("PART_Popup");
        _popupContent = _popup?.Child;
        _results = e.NameScope.Find<ListBox>("PART_Results");
        _search = e.NameScope.Find<TextBox>("PART_SearchBox");
        _input = e.NameScope.Find<Border>("PART_Input");
        _open = e.NameScope.Find<Button>("PART_OpenButton");
        _clear = e.NameScope.Find<Button>("PART_ClearButton");
        _tags = e.NameScope.Find<ItemsControl>("PART_Tags");
        if (_popup is not null)
        {
            _popup.PlacementTarget = _input ?? (Control)this;
            _popup.OverlayInputPassThroughElement = _input ?? (Control)this;
            _popup.Opened += PopupOpened; _popup.Closed += PopupClosed;
        }
        _popupContent?.AddHandler(PointerWheelChangedEvent, ContainWheel, RoutingStrategies.Bubble);
        _popupContent?.AddHandler(ScrollGestureEvent, ContainGesture, RoutingStrategies.Bubble);
        _popupContent?.AddHandler(ScrollGestureInertiaStartingEvent, ContainPhase, RoutingStrategies.Bubble);
        _popupContent?.AddHandler(ScrollGestureEndedEvent, ContainPhase, RoutingStrategies.Bubble);
        _results?.AddHandler(PointerPressedEvent, RowPressed, RoutingStrategies.Tunnel);
        _results?.AddHandler(KeyDownEvent, PopupKeyDown, RoutingStrategies.Tunnel);
        _results?.AddHandler(Button.ClickEvent, ExpandClicked);
        _search?.AddHandler(KeyDownEvent, PopupKeyDown, RoutingStrategies.Tunnel);
        _tags?.AddHandler(Button.ClickEvent, RemoveClicked);
        if (_input is not null) _input.PointerPressed += InputPressed;
        if (_open is not null) _open.Click += OpenClicked;
        if (_clear is not null) _clear.Click += ClearClicked;
        RefreshItems();
    }

    private static bool IsButtonSource(object? source) => source is Control control &&
        (control is Button || control.GetVisualAncestors().OfType<Button>().Any());
    private void InputPressed(object? sender, PointerPressedEventArgs e)
    { if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && !IsButtonSource(e.Source)) { Focus(); OpenDropDown(); } }
    private void OpenClicked(object? sender, RoutedEventArgs e)
    { if (IsDropDownOpen) CloseDropDown(); else OpenDropDown(); }
    private void ClearClicked(object? sender, RoutedEventArgs e) { ClearSelection(); Focus(); }
    private void RemoveClicked(object? sender, RoutedEventArgs e)
    {
        if (e.Source is Button { CommandParameter: TreeSelectItem row })
        { RemoveKey(row); Focus(); e.Handled = true; }
    }
    private void ExpandClicked(object? sender, RoutedEventArgs e)
    {
        if (e.Source is Button { CommandParameter: TreeSelectItem row })
        { SetExpanded(row.Key, !row.IsExpanded); e.Handled = true; }
    }
    private void RowPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed || IsButtonSource(e.Source) || e.Source is not Control control) return;
        var container = control as ListBoxItem ?? control.GetVisualAncestors().OfType<ListBoxItem>().FirstOrDefault();
        if (container is null || _results?.ItemFromContainer(container) is not TreeSelectItem row) return;
        _results.SelectedItem = row; _results.Focus(); ToggleItem(row.Item); e.Handled = true;
    }

    public bool OpenDropDown()
    {
        Dispatcher.UIThread.VerifyAccess();
        if (!_attached || !IsEffectivelyEnabled || _popup is null) return false;
        SetCurrentValue(IsDropDownOpenProperty, true); return IsDropDownOpen;
    }
    public void CloseDropDown() => CloseDropDown(true);
    private void CloseDropDown(bool restoreFocus)
    {
        _focusGeneration++;
        var wasOpen = IsDropDownOpen;
        SetCurrentValue(IsDropDownOpenProperty, false);
        if (_popup is not null) _popup.IsOpen = false;
        if (_topLevel is not null) _topLevel.SizeChanged -= HostSizeChanged;
        _topLevel = null;
        if (wasOpen && restoreFocus && _attached && IsEffectivelyEnabled) Focus();
    }
    private void UpdatePopup()
    {
        if (_popup is null) return;
        if (!IsDropDownOpen) { _popup.IsOpen = false; PseudoClasses.Set(":open", false); return; }
        if (!_attached || !IsEffectivelyEnabled) { SetCurrentValue(IsDropDownOpenProperty, false); return; }
        _topLevel = TopLevel.GetTopLevel(this);
        if (_topLevel is not null) { _topLevel.SizeChanged -= HostSizeChanged; _topLevel.SizeChanged += HostSizeChanged; }
        ApplyPopupEnvironment(); _popup.IsOpen = true; PseudoClasses.Set(":open", true);
    }
    private void ApplyPopupEnvironment()
    {
        if (_popup is null || _popupContent is null) return;
        _popup.Width = Math.Min(Math.Max(1, Bounds.Width), Math.Max(1, (_topLevel?.ClientSize.Width ?? Bounds.Width) - 16));
        _popup.MaxHeight = Math.Min(MaxDropDownHeight, Math.Max(80, (_topLevel?.ClientSize.Height ?? 480) - 24));
        _popupContent.SetCurrentValue(ThemeVariantScope.RequestedThemeVariantProperty, ActualThemeVariant);
        ControlSizing.SetSize(_popupContent, ControlSizing.GetSize(this));
    }
    private void HostSizeChanged(object? sender, SizeChangedEventArgs e) => ApplyPopupEnvironment();
    private void PopupOpened(object? sender, EventArgs e)
    {
        var generation = ++_focusGeneration;
        Dispatcher.UIThread.Post(() => { if (_attached && IsDropDownOpen && generation == _focusGeneration) _search?.Focus(); }, DispatcherPriority.Loaded);
    }
    private void PopupClosed(object? sender, EventArgs e) => CloseDropDown(false);
    private static void ContainWheel(object? sender, PointerWheelEventArgs e) => e.Handled = true;
    private static void ContainGesture(object? sender, ScrollGestureEventArgs e) { e.Handled = true; e.ShouldEndScrollGesture = true; }
    private static void ContainPhase(object? sender, RoutedEventArgs e) => e.Handled = true;

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Handled || !IsFocused || !IsEffectivelyEnabled) return;
        if (e.Key == Key.F4 || (e.Key == Key.Down && e.KeyModifiers == KeyModifiers.Alt))
        { if (IsDropDownOpen) CloseDropDown(); else OpenDropDown(); e.Handled = true; }
        else if (e.KeyModifiers == KeyModifiers.None && e.Key is Key.Down or Key.Space or Key.Enter)
        { OpenDropDown(); e.Handled = true; }
        else if (e.Key == Key.Back && string.IsNullOrEmpty(SearchText))
        {
            var last = ReadKeys().Where(_index.ContainsKey).Select(key => _index[key]).LastOrDefault(row => row.CanRemove);
            if (last is not null) RemoveKey(last);
            e.Handled = true;
        }
    }
    private void PopupKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Handled || !IsDropDownOpen) return;
        var search = ReferenceEquals(sender, _search);
        if (search && _search!.GetVisualDescendants().OfType<TextPresenter>().Any(p => !string.IsNullOrEmpty(p.GetValue(TextPresenter.PreeditTextProperty)))) return;
        if (e.Key == Key.Escape) { CloseDropDown(); e.Handled = true; }
        else if (e.Key == Key.Tab)
        {
            CloseDropDown(false);
            TopLevel.GetTopLevel(this)?.FocusManager?.TryMoveFocus(e.KeyModifiers.HasFlag(KeyModifiers.Shift)
                ? NavigationDirection.Previous : NavigationDirection.Next, new FindNextElementOptions { FocusedElement = this });
            e.Handled = true;
        }
        else if (search && e.Key == Key.Down && _visibleItems.Count > 0 && _results is not null)
        { _results.SelectedIndex = 0; _results.Focus(); e.Handled = true; }
        else if (!search && _results?.SelectedItem is TreeSelectItem row)
        {
            if (e.Key is Key.Space or Key.Enter) { ToggleItem(row.Item); e.Handled = true; }
            else if (e.Key == Key.Right) { SetExpanded(row.Key, true); e.Handled = true; }
            else if (e.Key == Key.Left)
            {
                if (row.IsExpanded) SetExpanded(row.Key, false);
                else if (row.Parent is { } parent) _results.SelectedItem = parent;
                e.Handled = true;
            }
        }
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new TreeSelectPeer(this);
    private sealed class TreeSelectPeer(TreeSelect owner) : ControlAutomationPeer(owner), IExpandCollapseProvider
    {
        public ExpandCollapseState ExpandCollapseState => owner.IsDropDownOpen ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
        public bool ShowsMenu => false;
        public void Expand() => owner.OpenDropDown();
        public void Collapse() => owner.CloseDropDown();
        protected override string GetClassNameCore() => nameof(TreeSelect);
        protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.ComboBox;
        protected override string? GetNameCore() => !string.IsNullOrEmpty(base.GetNameCore()) ? base.GetNameCore() : owner.PlaceholderText;
    }
}
