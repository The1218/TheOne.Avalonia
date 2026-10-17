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

public partial class MultiSelectBox
{
    private int _activeIndex = -1, _focusGeneration;
    private Border? _input;
    private Button? _open, _clear;
    private Popup? _popup;
    private Control? _popupContent;
    private ListBox? _results;
    private TextBox? _search;
    private ItemsControl? _tags;
    private TopLevel? _topLevel;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs args)
    {
        CloseDropDown(false);
        if (_popup is not null)
        {
            _popup.Opened -= PopupOpened;
            _popup.Closed -= PopupClosed;
        }

        _popupContent?.RemoveHandler(PointerWheelChangedEvent, ContainPopupWheel);
        _popupContent?.RemoveHandler(ScrollGestureEvent, ContainPopupGesture);
        _popupContent?.RemoveHandler(ScrollGestureInertiaStartingEvent, ContainPopupPhase);
        _popupContent?.RemoveHandler(ScrollGestureEndedEvent, ContainPopupPhase);
        _popupContent = null;
        if (_results is not null)
        {
            _results.SelectionChanged -= ResultsSelectionChanged;
            _results.RemoveHandler(KeyDownEvent, PopupKeyDown);
        }

        if (_search is not null) _search.RemoveHandler(KeyDownEvent, PopupKeyDown);
        if (_open is not null) _open.Click -= OpenClicked;
        if (_clear is not null) _clear.Click -= ClearClicked;
        if (_tags is not null) _tags.RemoveHandler(Button.ClickEvent, RemoveClicked);
        if (_input is not null) _input.PointerPressed -= InputPressed;
        base.OnApplyTemplate(args);
        _popup = args.NameScope.Find<Popup>("PART_Popup");
        _results = args.NameScope.Find<ListBox>("PART_Results");
        _search = args.NameScope.Find<TextBox>("PART_SearchBox");
        _input = args.NameScope.Find<Border>("PART_Input");
        _open = args.NameScope.Find<Button>("PART_OpenButton");
        _clear = args.NameScope.Find<Button>("PART_ClearButton");
        _tags = args.NameScope.Find<ItemsControl>("PART_Tags");
        if (_popup is not null)
        {
            _popupContent = _popup.Child;
            _popupContent?.AddHandler(PointerWheelChangedEvent, ContainPopupWheel, RoutingStrategies.Bubble);
            _popupContent?.AddHandler(ScrollGestureEvent, ContainPopupGesture, RoutingStrategies.Bubble);
            _popupContent?.AddHandler(ScrollGestureInertiaStartingEvent, ContainPopupPhase, RoutingStrategies.Bubble);
            _popupContent?.AddHandler(ScrollGestureEndedEvent, ContainPopupPhase, RoutingStrategies.Bubble);
            _popup.PlacementTarget = _input ?? (Control)this;
            _popup.OverlayInputPassThroughElement = _input ?? (Control)this;
            _popup.Opened += PopupOpened;
            _popup.Closed += PopupClosed;
        }

        if (_results is not null)
        {
            _results.SelectionChanged += ResultsSelectionChanged;
            _results.AddHandler(KeyDownEvent, PopupKeyDown, RoutingStrategies.Tunnel);
        }

        _search?.AddHandler(KeyDownEvent, PopupKeyDown, RoutingStrategies.Tunnel);
        if (_open is not null) _open.Click += OpenClicked;
        if (_clear is not null) _clear.Click += ClearClicked;
        _tags?.AddHandler(Button.ClickEvent, RemoveClicked);
        if (_input is not null) _input.PointerPressed += InputPressed;
        RefreshItems();
    }

    // Native scrolling runs first. Short/empty lists and boundary wheel input must
    // stop here instead of bubbling through Popup's logical parent into the page.
    private static void ContainPopupWheel(object? sender, PointerWheelEventArgs args)
    {
        args.Handled = true;
    }

    private static void ContainPopupGesture(object? sender, ScrollGestureEventArgs args)
    {
        args.Handled = true;
        args.ShouldEndScrollGesture = true;
    }

    private static void ContainPopupPhase(object? sender, RoutedEventArgs args)
    {
        args.Handled = true;
    }

    private void InputPressed(object? sender, PointerPressedEventArgs args)
    {
        if (!args.GetCurrentPoint(this).Properties.IsLeftButtonPressed || args.Source is not Control source
                                                                       || source is Button ||
                                                                       source.GetVisualAncestors().OfType<Button>()
                                                                           .Any()) return;
        Focus();
        OpenDropDown();
    }

    private void OpenClicked(object? sender, RoutedEventArgs args)
    {
        if (IsDropDownOpen) CloseDropDown();
        else OpenDropDown();
    }

    private void ClearClicked(object? sender, RoutedEventArgs args)
    {
        ClearSelection();
        Focus();
    }

    private void RemoveClicked(object? sender, RoutedEventArgs args)
    {
        if (args.Source is Button { CommandParameter: { } item })
        {
            ToggleItem(item);
            Focus();
            args.Handled = true;
        }
    }

    public bool OpenDropDown()
    {
        Dispatcher.UIThread.VerifyAccess();
        if (!_attached || !IsEffectivelyEnabled || _popup is null) return false;
        SetCurrentValue(IsDropDownOpenProperty, true);
        return IsDropDownOpen;
    }

    public void CloseDropDown()
    {
        CloseDropDown(true);
    }

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
        if (!IsDropDownOpen)
        {
            _popup.IsOpen = false;
            PseudoClasses.Set(":open", false);
            return;
        }

        if (!_attached || !IsEffectivelyEnabled)
        {
            SetCurrentValue(IsDropDownOpenProperty, false);
            return;
        }

        _topLevel = TopLevel.GetTopLevel(this);
        if (_topLevel is not null)
        {
            _topLevel.SizeChanged -= HostSizeChanged;
            _topLevel.SizeChanged += HostSizeChanged;
        }

        ApplyPopupEnvironment();
        _popup.IsOpen = true;
        PseudoClasses.Set(":open", true);
    }

    private void ApplyPopupEnvironment()
    {
        if (_popup?.Child is not Control child) return;
        _popup.Width = Math.Min(Math.Max(1, Bounds.Width),
            Math.Max(1, (_topLevel?.ClientSize.Width ?? Bounds.Width) - 16));
        _popup.MaxHeight = Math.Min(MaxDropDownHeight, Math.Max(80, (_topLevel?.ClientSize.Height ?? 480) - 24));
        child.SetCurrentValue(ThemeVariantScope.RequestedThemeVariantProperty, ActualThemeVariant);
        ControlSizing.SetSize(child, ControlSizing.GetSize(this));
    }

    private void HostSizeChanged(object? sender, SizeChangedEventArgs args)
    {
        ApplyPopupEnvironment();
    }

    private void PopupOpened(object? sender, EventArgs args)
    {
        var generation = ++_focusGeneration;
        Dispatcher.UIThread.Post(() =>
        {
            if (generation != _focusGeneration || !IsDropDownOpen || !_attached) return;
            _search?.Focus();
        }, DispatcherPriority.Loaded);
    }

    private void PopupClosed(object? sender, EventArgs args)
    {
        CloseDropDown(false);
    }

    protected override void OnKeyDown(KeyEventArgs args)
    {
        base.OnKeyDown(args);
        if (args.Handled || !IsFocused || !IsEffectivelyEnabled) return;
        if (args.Key == Key.F4 || (args.Key == Key.Down && args.KeyModifiers == KeyModifiers.Alt))
        {
            if (IsDropDownOpen) CloseDropDown();
            else OpenDropDown();
            args.Handled = true;
        }
        else if (args.KeyModifiers == KeyModifiers.None && args.Key is Key.Enter or Key.Space or Key.Down)
        {
            OpenDropDown();
            args.Handled = true;
        }
        else if (args.KeyModifiers == KeyModifiers.None && args.Key == Key.Back)
        {
            RemoveLastEditable();
            args.Handled = true;
        }
    }

    private void PopupKeyDown(object? sender, KeyEventArgs args)
    {
        if (!IsDropDownOpen || !IsEffectivelyEnabled) return;
        var search = ReferenceEquals(sender, _search);
        if (search && _search!.GetVisualDescendants().OfType<TextPresenter>().Any(presenter =>
                !string.IsNullOrEmpty(presenter.GetValue(TextPresenter.PreeditTextProperty)))) return;
        if (!search && args.Source is Control source)
        {
            var row = source as ListBoxItem ?? source.GetVisualAncestors().OfType<ListBoxItem>().FirstOrDefault();
            if (row is not null) _activeIndex = _results?.IndexFromContainer(row) ?? _activeIndex;
        }

        if (args.Key == Key.Escape && args.KeyModifiers == KeyModifiers.None)
        {
            CloseDropDown();
            args.Handled = true;
        }
        else if (args.Key == Key.Tab)
        {
            CloseDropDown();
            TopLevel.GetTopLevel(this)?.FocusManager?.TryMoveFocus(
                args.KeyModifiers.HasFlag(KeyModifiers.Shift) ? NavigationDirection.Previous : NavigationDirection.Next,
                new FindNextElementOptions { FocusedElement = this });
            args.Handled = true;
        }
        else if (!search && args.Key == Key.A &&
                 (args.KeyModifiers == KeyModifiers.Control || args.KeyModifiers == KeyModifiers.Meta))
        {
            EditSelection(selected =>
            {
                var existing = DistinctItems(selected).ToHashSet();
                foreach (var entry in _visibleItems.Where(entry => entry.IsEnabled))
                    if (existing.Add(entry.Item))
                        selected.Add(entry.Item);
            });
            args.Handled = true;
        }
        else if (args.KeyModifiers == KeyModifiers.None && args.Key is Key.Down or Key.Up or Key.Home or Key.End &&
                 (!search || args.Key is Key.Down or Key.Up))
        {
            MoveActive(args.Key);
            args.Handled = true;
        }
        else if (args.KeyModifiers == KeyModifiers.None &&
                 (args.Key == Key.Enter || (!search && args.Key == Key.Space)))
        {
            if (_activeIndex < 0 || _activeIndex >= _visibleItems.Count) _activeIndex = FirstEnabled();
            if (_activeIndex >= 0) ToggleItem(_visibleItems[_activeIndex].Item);
            args.Handled = true;
        }
        else if (search && args.KeyModifiers == KeyModifiers.None && args.Key == Key.Back && SearchText.Length == 0)
        {
            RemoveLastEditable();
            args.Handled = true;
        }
    }

    private int FirstEnabled()
    {
        return Enumerable.Range(0, _visibleItems.Count).FirstOrDefault(index => _visibleItems[index].IsEnabled, -1);
    }

    private void MoveActive(Key key)
    {
        var step = key is Key.Up or Key.End ? -1 : 1;
        var index = key == Key.Home ? 0 :
            key == Key.End ? _visibleItems.Count - 1 :
            _activeIndex < 0 ? step > 0 ? 0 : _visibleItems.Count - 1 : _activeIndex + step;
        while (index >= 0 && index < _visibleItems.Count && !_visibleItems[index].IsEnabled) index += step;
        if (index < 0 || index >= _visibleItems.Count)
        {
            if (key == Key.Up)
            {
                _activeIndex = -1;
                _search?.Focus();
            }

            return;
        }

        _activeIndex = index;
        _results?.ScrollIntoView(index);
        var generation = _focusGeneration;
        Dispatcher.UIThread.Post(() =>
        {
            if (generation == _focusGeneration && IsDropDownOpen) _results?.ContainerFromIndex(_activeIndex)?.Focus();
        }, DispatcherPriority.Loaded);
    }

    private void RemoveLastEditable()
    {
        var item = _selection.LastOrDefault(item => _entries[item].IsEnabled);
        if (item is not null) ToggleItem(item);
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new MultiSelectPeer(this);
    }

    private sealed class MultiSelectPeer(MultiSelectBox box) : ControlAutomationPeer(box), IExpandCollapseProvider
    {
        public ExpandCollapseState ExpandCollapseState =>
            box.IsDropDownOpen ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;

        public void Expand()
        {
            box.OpenDropDown();
        }

        public void Collapse()
        {
            box.CloseDropDown();
        }

        public bool ShowsMenu => false;

        protected override string GetClassNameCore()
        {
            return nameof(MultiSelectBox);
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ComboBox;
        }

        protected override string? GetNameCore()
        {
            return !string.IsNullOrEmpty(base.GetNameCore()) ? base.GetNameCore() : box.PlaceholderText;
        }
    }
}
