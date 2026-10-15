using System.Collections;
using System.Windows.Input;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Input;
using global::Avalonia.Interactivity;
using global::Avalonia.VisualTree;
using global::Avalonia.Threading;

namespace TheOne.Avalonia.Controls;

public sealed class TabClosingEventArgs(object item) : CancelRoutedEventArgs(TabFeatures.TabClosingEvent)
{
    public object Item { get; } = item;
}

/// <summary>Optional closing for native tabs and strips; reordering remains specific to TabControl.</summary>
public sealed partial class TabFeatures : AvaloniaObject
{
    public static readonly AttachedProperty<bool> CanCloseProperty = AvaloniaProperty.RegisterAttached<TabFeatures, SelectingItemsControl, bool>("CanClose");
    public static readonly AttachedProperty<bool> CanReorderProperty = AvaloniaProperty.RegisterAttached<TabFeatures, TabControl, bool>("CanReorder");
    public static readonly AttachedProperty<bool> IsClosableProperty = AvaloniaProperty.RegisterAttached<TabFeatures, Control, bool>("IsClosable", true);
    public static readonly AttachedProperty<bool> ShowCloseButtonProperty = AvaloniaProperty.RegisterAttached<TabFeatures, Control, bool>("ShowCloseButton");
    public static bool GetShowCloseButton(TabItem item) => item.GetValue(ShowCloseButtonProperty);
    public static void SetShowCloseButton(TabItem item, bool value) => item.SetValue(ShowCloseButtonProperty, value);
    private static void UpdateCloseVisibility(TabItem item, TabControl? owner = null)
    {
        owner ??= Owner(item);
        item.SetValue(ShowCloseButtonProperty, owner != null && GetCanClose(owner) && GetIsClosable(item));
        QueueCommandRefresh();
    }
    public static readonly AttachedProperty<ICommand?> CloseCommandProperty = AvaloniaProperty.RegisterAttached<TabFeatures, SelectingItemsControl, ICommand?>("CloseCommand");
    public static readonly RoutedEvent<TabClosingEventArgs> TabClosingEvent = RoutedEvent.Register<Control, TabClosingEventArgs>("TabClosing", RoutingStrategies.Bubble);
    public static ICommand CloseTabCommand { get; } = new CloseTab();
    private static readonly AttachedProperty<WeakReference<TabControl>?> OwnerProperty = AvaloniaProperty.RegisterAttached<TabFeatures, TabItem, WeakReference<TabControl>?>("Owner");
    private static readonly AttachedProperty<DragState?> StateProperty = AvaloniaProperty.RegisterAttached<TabFeatures, TabControl, DragState?>("State");

    static TabFeatures()
    {
        InitializeStripClosing();
        CanReorderProperty.Changed.AddClassHandler<TabControl>((tabs, _) => { Attach(tabs); if (!GetCanReorder(tabs)) tabs.GetValue(StateProperty)?.Clear(); });
        CanCloseProperty.Changed.AddClassHandler<TabControl>((tabs, _) =>
        {
            Attach(tabs);
            foreach (var item in tabs.GetRealizedContainers().OfType<TabItem>()) UpdateCloseVisibility(item, tabs);
        });
        IsClosableProperty.Changed.AddClassHandler<TabItem>((item, _) => UpdateCloseVisibility(item));
    }
    public static bool GetCanClose(TabControl tabs) => tabs.GetValue(CanCloseProperty);
    public static void SetCanClose(TabControl tabs, bool value) => tabs.SetValue(CanCloseProperty, value);
    public static bool GetCanReorder(TabControl tabs) => tabs.GetValue(CanReorderProperty);
    public static void SetCanReorder(TabControl tabs, bool value) => tabs.SetValue(CanReorderProperty, value);
    public static bool GetIsClosable(TabItem item) => item.GetValue(IsClosableProperty);
    public static void SetIsClosable(TabItem item, bool value) => item.SetValue(IsClosableProperty, value);
    public static ICommand? GetCloseCommand(TabControl tabs) => tabs.GetValue(CloseCommandProperty);
    public static void SetCloseCommand(TabControl tabs, ICommand? value) => tabs.SetValue(CloseCommandProperty, value);

    private static void Attach(TabControl tabs)
    {
        if (tabs.GetValue(StateProperty) != null) return;
        var state = new DragState(tabs);
        tabs.SetValue(StateProperty, state);
        tabs.AddHandler(InputElement.PointerPressedEvent, state.Pressed, RoutingStrategies.Tunnel, true);
        tabs.AddHandler(InputElement.PointerMovedEvent, state.Moved, RoutingStrategies.Tunnel, true);
        tabs.AddHandler(InputElement.PointerReleasedEvent, state.Released, RoutingStrategies.Tunnel, true);
        tabs.AddHandler(InputElement.KeyDownEvent, state.KeyDown, RoutingStrategies.Tunnel);
        tabs.PointerCaptureLost += state.CaptureLost;
        tabs.DetachedFromVisualTree += (_, _) => state.Clear();
        tabs.PropertyChanged += state.OwnerChanged;
        tabs.Items.CollectionChanged += state.ItemsChanged;
        tabs.ContainerClearing += (_, args) => { if (args.Container is TabItem item) state.Recycle(item); };
        tabs.ContainerPrepared += (_, args) => { if (args.Container is TabItem item) { item.SetValue(OwnerProperty, new WeakReference<TabControl>(tabs)); UpdateCloseVisibility(item, tabs); state.Prepare(item); } };
        tabs.AttachedToVisualTree += (_, _) => QueueCommandRefresh();
    }
    private static TabControl? Owner(TabItem item) => item.GetVisualAncestors().OfType<TabControl>().FirstOrDefault() ?? item.Parent as TabControl
        ?? (item.GetValue(OwnerProperty)?.TryGetTarget(out var owner) == true ? owner : null);
    private static object? Data(TabControl tabs, TabItem item) => tabs.Items.Contains(item) ? item : tabs.ItemFromContainer(item);
    private static IList? MutableItems(TabControl tabs)
    {
        var list = tabs.ItemsSource as IList ?? (tabs.ItemsSource == null ? tabs.Items : null);
        return list is { IsReadOnly: false, IsFixedSize: false } ? list : null;
    }
    private static bool CanClose(TabItem item)
    {
        var tabs = Owner(item);
        if (tabs == null || !tabs.IsEffectivelyEnabled || !item.IsEffectivelyEnabled || !GetCanClose(tabs) || !GetIsClosable(item)) return false;
        var data = Data(tabs, item);
        return data != null && (GetCloseCommand(tabs) is { } command ? command.CanExecute(data) : MutableItems(tabs) != null);
    }
    public static bool Close(TabItem item)
    {
        if (!CanClose(item) || Owner(item) is not { } tabs || Data(tabs, item) is not { } data) return false;
        var hadHeaderFocus = TopLevel.GetTopLevel(tabs)?.FocusManager?.GetFocusedElement() is Visual focused
            && focused.GetVisualAncestors().Prepend(focused).OfType<TabItem>().Any(header => Owner(header) == tabs);
        var args = new TabClosingEventArgs(data);
        tabs.RaiseEvent(args);
        if (args.Cancel) return false;
        if (GetCloseCommand(tabs) is { } command) command.Execute(data);
        else
        {
            var list = MutableItems(tabs)!;
            var index = list.IndexOf(data);
            if (index < 0) return false;
            var selected = tabs.SelectedItem;
            list.RemoveAt(index);
            if (!Equals(selected, data) && selected != null && list.Contains(selected)) tabs.SetCurrentValue(TabControl.SelectedItemProperty, selected);
            else if (list.Count > 0) tabs.SetCurrentValue(TabControl.SelectedIndexProperty, Math.Min(index, list.Count - 1));
        }
        if (hadHeaderFocus) RestoreHeaderFocus(tabs);
        return true;
    }
    private static void RestoreHeaderFocus(TabControl tabs) => Dispatcher.UIThread.Post(() =>
    {
        if (tabs.IsAttachedToVisualTree() && tabs.ContainerFromIndex(tabs.SelectedIndex) is TabItem selected)
            selected.Focus(NavigationMethod.Directional);
    }, DispatcherPriority.Background);
    public static bool Move(TabControl tabs, int from, int to)
    {
        var list = MutableItems(tabs);
        if (!GetCanReorder(tabs) || !tabs.IsEffectivelyEnabled || list == null || from < 0 || to < 0 || from >= list.Count || to >= list.Count || from == to) return false;
        var selected = tabs.SelectedItem;
        var item = list[from];
        list.RemoveAt(from);
        list.Insert(to, item);
        if (selected != null && list.Contains(selected)) tabs.SetCurrentValue(TabControl.SelectedItemProperty, selected);
        return true;
    }
    private static bool _refreshQueued;
    private static void QueueCommandRefresh()
    {
        if (_refreshQueued) return;
        _refreshQueued = true;
        Dispatcher.UIThread.Post(() => { _refreshQueued = false; ((CloseTab)CloseTabCommand).Refresh(); });
    }
    private sealed class CloseTab : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        public void Refresh() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        public bool CanExecute(object? parameter) => parameter switch { TabItem item => CanClose(item), TabStripItem item => CanClose(item), _ => false };
        public void Execute(object? parameter) { if (parameter is TabItem item) Close(item); else if (parameter is TabStripItem stripItem) Close(stripItem); }
    }
}
