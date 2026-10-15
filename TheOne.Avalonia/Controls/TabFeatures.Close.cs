using System.Collections;
using System.Windows.Input;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Input;
using global::Avalonia.Interactivity;
using global::Avalonia.Threading;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

public sealed partial class TabFeatures
{
    private static readonly AttachedProperty<StripCloseState?> StripCloseStateProperty = AvaloniaProperty.RegisterAttached<TabFeatures, TabStrip, StripCloseState?>("StripCloseState");
    private static readonly AttachedProperty<WeakReference<TabStrip>?> StripOwnerProperty = AvaloniaProperty.RegisterAttached<TabFeatures, TabStripItem, WeakReference<TabStrip>?>("StripOwner");
    public static bool GetCanClose(TabStrip tabs) => tabs.GetValue(CanCloseProperty);
    public static void SetCanClose(TabStrip tabs, bool value) => tabs.SetValue(CanCloseProperty, value);
    public static bool GetIsClosable(TabStripItem item) => item.GetValue(IsClosableProperty);
    public static void SetIsClosable(TabStripItem item, bool value) => item.SetValue(IsClosableProperty, value);
    public static bool GetShowCloseButton(TabStripItem item) => item.GetValue(ShowCloseButtonProperty);
    public static void SetShowCloseButton(TabStripItem item, bool value) => item.SetValue(ShowCloseButtonProperty, value);
    public static ICommand? GetCloseCommand(TabStrip tabs) => tabs.GetValue(CloseCommandProperty);
    public static void SetCloseCommand(TabStrip tabs, ICommand? value) => tabs.SetValue(CloseCommandProperty, value);
    private static void InitializeStripClosing()
    {
        CanCloseProperty.Changed.AddClassHandler<TabStrip>((tabs, _) => AttachStrip(tabs).Refresh());
        CloseCommandProperty.Changed.AddClassHandler<TabStrip>((tabs, _) => AttachStrip(tabs).ObserveCommand());
        IsClosableProperty.Changed.AddClassHandler<TabStripItem>((item, _) => UpdateStripClose(item));
    }
    private static StripCloseState AttachStrip(TabStrip tabs)
    {
        if (tabs.GetValue(StripCloseStateProperty) is { } existing) return existing;
        var state = new StripCloseState(tabs); tabs.SetValue(StripCloseStateProperty, state); return state;
    }
    private static TabStrip? Owner(TabStripItem item) => item.GetVisualAncestors().OfType<TabStrip>().FirstOrDefault() ?? item.Parent as TabStrip
        ?? (item.GetValue(StripOwnerProperty)?.TryGetTarget(out var owner) == true ? owner : null);
    private static object? Data(TabStrip tabs, TabStripItem item) => tabs.Items.Contains(item) ? item : tabs.ItemFromContainer(item);
    private static IList? MutableItems(TabStrip tabs)
    {
        var list = tabs.ItemsSource as IList ?? (tabs.ItemsSource is null ? tabs.Items : null);
        return list is { IsReadOnly: false, IsFixedSize: false } ? list : null;
    }
    private static void UpdateStripClose(TabStripItem item)
    {
        item.SetValue(ShowCloseButtonProperty, Owner(item) is { } tabs && GetCanClose(tabs) && GetIsClosable(item));
        QueueCommandRefresh();
    }
    private static bool CanClose(TabStripItem item)
    {
        if (Owner(item) is not { } tabs || !tabs.IsEffectivelyEnabled || !item.IsEffectivelyEnabled || !GetCanClose(tabs) || !GetIsClosable(item)) return false;
        var data = Data(tabs, item);
        return data is not null && (GetCloseCommand(tabs) is { } command ? command.CanExecute(data) : MutableItems(tabs) is not null);
    }
    public static bool Close(TabStripItem item)
    {
        if (!CanClose(item) || Owner(item) is not { } tabs || Data(tabs, item) is not { } data) return false;
        var restoreFocus = TopLevel.GetTopLevel(tabs)?.FocusManager?.GetFocusedElement() is Visual focused
            && focused.GetVisualAncestors().Prepend(focused).OfType<TabStripItem>().Any(header => Owner(header) == tabs);
        var args = new TabClosingEventArgs(data); tabs.RaiseEvent(args);
        var current = Data(tabs, item);
        if (args.Cancel || Owner(item) != tabs || !(ReferenceEquals(current, data) || data is ValueType && Equals(current, data)) || !CanClose(item)) return false;
        if (GetCloseCommand(tabs) is { } command) command.Execute(data);
        else
        {
            var list = MutableItems(tabs)!; var index = list.IndexOf(data); if (index < 0) return false;
            var selected = tabs.SelectedItem; list.RemoveAt(index);
            if (!Equals(selected, data) && selected is not null && list.Contains(selected)) tabs.SetCurrentValue(TabStrip.SelectedItemProperty, selected);
            else tabs.SetCurrentValue(TabStrip.SelectedIndexProperty, list.Count == 0 ? -1 : Math.Min(index, list.Count - 1));
        }
        if (restoreFocus) Dispatcher.UIThread.Post(() =>
        {
            if (tabs.IsAttachedToVisualTree() && tabs.ContainerFromIndex(tabs.SelectedIndex) is TabStripItem next) next.Focus(NavigationMethod.Directional);
        }, DispatcherPriority.Background);
        return true;
    }
    private sealed class StripCloseState
    {
        private readonly TabStrip _tabs;
        private ICommand? _command;
        internal StripCloseState(TabStrip tabs)
        {
            _tabs = tabs;
            tabs.ContainerPrepared += (_, args) => { if (args.Container is TabStripItem item) { item.SetValue(StripOwnerProperty, new WeakReference<TabStrip>(tabs)); UpdateStripClose(item); } };
            tabs.ContainerClearing += (_, args) => { if (args.Container is TabStripItem item) { item.ClearValue(StripOwnerProperty); item.SetValue(ShowCloseButtonProperty, false); } QueueCommandRefresh(); };
            tabs.Items.CollectionChanged += (_, _) => Refresh();
            tabs.AttachedToVisualTree += (_, _) => { ObserveCommand(); Refresh(); };
            tabs.DetachedFromVisualTree += (_, _) => { if (_command is not null) _command.CanExecuteChanged -= CommandChanged; _command = null; };
            tabs.AddHandler(InputElement.KeyDownEvent, KeyDown, RoutingStrategies.Tunnel);
            ObserveCommand(); Refresh();
        }
        internal void ObserveCommand()
        {
            if (_command is not null) _command.CanExecuteChanged -= CommandChanged;
            _command = GetCloseCommand(_tabs);
            if (_command is not null && _tabs.IsAttachedToVisualTree()) _command.CanExecuteChanged += CommandChanged;
            QueueCommandRefresh();
        }
        private void CommandChanged(object? sender, EventArgs e) => QueueCommandRefresh();
        internal void Refresh() { foreach (var item in _tabs.GetRealizedContainers().OfType<TabStripItem>()) UpdateStripClose(item); QueueCommandRefresh(); }
        private void KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key != Key.W || (!e.KeyModifiers.HasFlag(KeyModifiers.Control) && !e.KeyModifiers.HasFlag(KeyModifiers.Meta))
                || !GetCanClose(_tabs) || _tabs.SelectedIndex < 0) return;
            if (_tabs.ContainerFromIndex(_tabs.SelectedIndex) is not TabStripItem)
            {
                _tabs.ScrollIntoView(_tabs.SelectedIndex);
                _tabs.UpdateLayout();
            }
            if (_tabs.ContainerFromIndex(_tabs.SelectedIndex) is TabStripItem item && Close(item)) e.Handled = true;
        }
    }
}
