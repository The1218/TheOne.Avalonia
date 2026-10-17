using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

public sealed partial class TabFeatures
{
    public static readonly AttachedProperty<bool> EnableContextMenuProperty =
        AvaloniaProperty.RegisterAttached<TabFeatures, TabControl, bool>("EnableContextMenu");
    internal static readonly IDataTemplate DefaultIconTemplate = new FuncDataTemplate<object>((value, _) =>
        value is Geometry geometry ? new PathIcon { Data = geometry } : new TextBlock { Text = value?.ToString() });
    public static readonly AttachedProperty<bool> ShowItemIconProperty =
        AvaloniaProperty.RegisterAttached<TabFeatures, TabControl, bool>("ShowItemIcon", true);
    public static readonly AttachedProperty<object?> IconProperty =
        AvaloniaProperty.RegisterAttached<TabFeatures, TabItem, object?>("Icon");
    public static readonly AttachedProperty<IDataTemplate?> IconTemplateProperty =
        AvaloniaProperty.RegisterAttached<TabFeatures, TabItem, IDataTemplate?>("IconTemplate", DefaultIconTemplate);
    public static readonly AttachedProperty<bool> IsPinnedProperty =
        AvaloniaProperty.RegisterAttached<TabFeatures, TabItem, bool>("IsPinned");
    public static readonly AttachedProperty<bool> IsIconVisibleProperty =
        AvaloniaProperty.RegisterAttached<TabFeatures, TabItem, bool>("IsIconVisible");
    private static readonly AttachedProperty<ContextMenu?> PageMenuProperty =
        AvaloniaProperty.RegisterAttached<TabFeatures, TabItem, ContextMenu?>("PageMenu");
    private static readonly ConditionalWeakTable<TabControl, ConditionalWeakTable<object, PinState>> Pins = new();
    private sealed class PinState { public bool Value; }

    public static bool GetShowItemIcon(TabControl tabs) => tabs.GetValue(ShowItemIconProperty);
    public static bool GetEnableContextMenu(TabControl tabs) => tabs.GetValue(EnableContextMenuProperty);
    public static void SetEnableContextMenu(TabControl tabs, bool value) => tabs.SetValue(EnableContextMenuProperty, value);
    public static void SetShowItemIcon(TabControl tabs, bool value) => tabs.SetValue(ShowItemIconProperty, value);
    public static object? GetIcon(TabItem item) => item.GetValue(IconProperty);
    public static void SetIcon(TabItem item, object? value) => item.SetValue(IconProperty, value);
    public static IDataTemplate? GetIconTemplate(TabItem item) => item.GetValue(IconTemplateProperty);
    public static void SetIconTemplate(TabItem item, IDataTemplate? value) => item.SetValue(IconTemplateProperty, value);
    public static bool GetIsPinned(TabItem item) => item.GetValue(IsPinnedProperty);
    public static void SetIsPinned(TabItem item, bool value) => item.SetCurrentValue(IsPinnedProperty, value);
    public static bool GetIsIconVisible(TabItem item) => item.GetValue(IsIconVisibleProperty);

    private static void InitializePages()
    {
        EnableContextMenuProperty.Changed.AddClassHandler<TabControl>((tabs, _) =>
        {
            Attach(tabs);
            foreach (var item in tabs.GetRealizedContainers().OfType<TabItem>()) PreparePage(tabs, item);
        });
        ShowItemIconProperty.Changed.AddClassHandler<TabControl>((tabs, _) =>
        {
            Attach(tabs);
            foreach (var item in tabs.GetRealizedContainers().OfType<TabItem>()) PreparePage(tabs, item);
        });
        IconProperty.Changed.AddClassHandler<TabItem>((item, _) => UpdateIcon(item));
        IsPinnedProperty.Changed.AddClassHandler<TabItem>((item, _) =>
        {
            if (Owner(item) is { } tabs && Data(tabs, item) is { } data)
            {
                Pins.GetOrCreateValue(tabs).GetOrCreateValue(data).Value = GetIsPinned(item);
                QueuePinPosition(tabs, item, data);
            }
            UpdateCloseVisibility(item);
        });
    }

    private static void UpdateIcon(TabItem item) => item.SetValue(IsIconVisibleProperty,
        GetIcon(item) is not null && (Owner(item) is not { } tabs || GetShowItemIcon(tabs)));

    private static bool IsPinned(TabControl tabs, object? data)
    {
        if (data is TabItem own) return GetIsPinned(own);
        return data is not null && Pins.TryGetValue(tabs, out var items) &&
               items.TryGetValue(data, out var pin) && pin.Value;
    }

    private static void PreparePage(TabControl tabs, TabItem item)
    {
        if (Data(tabs, item) is { } data && data is not TabItem)
        {
            // Restore per-data state without writing through a consumer binding on a recycled container.
            if (BindingOperations.GetBindingExpressionBase(item, IsPinnedProperty) is null)
                item.SetCurrentValue(IsPinnedProperty, IsPinned(tabs, data));
        }
        UpdateIcon(item);
        if (Data(tabs, item) is { } current) QueuePinPosition(tabs, item, current);
        if (GetIconTemplate(item) is null) item.SetCurrentValue(IconTemplateProperty, DefaultIconTemplate);
        if (item.ContextMenu is not null && item.ContextMenu != item.GetValue(PageMenuProperty)) return;
        if (!GetEnableContextMenu(tabs))
        {
            item.SetCurrentValue(Control.ContextMenuProperty, null);
            return;
        }
        if (item.GetValue(PageMenuProperty) is { } existing)
        {
            item.SetCurrentValue(Control.ContextMenuProperty, existing);
            return;
        }
        var close = new MenuItem { Header = "关闭当前页" };
        var pin = new MenuItem { Header = "固定到左侧" };
        var all = new MenuItem { Header = "关闭所有页" };
        var menu = new ContextMenu { ItemsSource = new object[] { close, pin, new Separator(), all } };
        menu.Opened += (_, _) =>
        {
            close.IsEnabled = CanClose(item);
            pin.Header = GetIsPinned(item) ? "取消固定" : "固定到左侧";
            pin.IsEnabled = Owner(item) is { IsEffectivelyEnabled: true } owner && MutableItems(owner) is not null;
            all.IsEnabled = Owner(item) is { IsEffectivelyEnabled: true } current && GetCanClose(current);
        };
        close.Click += (_, _) => Close(item);
        pin.Click += (_, _) => Pin(item, !GetIsPinned(item));
        all.Click += (_, _) => { if (Owner(item) is { } owner) CloseAll(owner); };
        item.SetValue(PageMenuProperty, menu);
        item.SetCurrentValue(Control.ContextMenuProperty, menu);
    }

    private static void QueuePinPosition(TabControl tabs, TabItem item, object data)
    {
        var index = tabs.Items.IndexOf(data);
        if (index < 0) return;
        var boundary = tabs.Items.Cast<object>().Count(value => IsPinned(tabs, value));
        if (GetIsPinned(item) ? !tabs.Items.Take(index).Any(value => !IsPinned(tabs, value)) : index >= boundary) return;
        global::Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (Owner(item) == tabs && ReferenceEquals(Data(tabs, item), data)) Pin(item, GetIsPinned(item));
        });
    }

    /// <summary>Pins a page before all unpinned pages, preserving selection. Does not require drag reordering.</summary>
    public static bool Pin(TabItem item, bool pinned = true)
    {
        if (Owner(item) is not { } tabs || !tabs.IsEffectivelyEnabled || MutableItems(tabs) is not { } list ||
            Data(tabs, item) is not { } data) return false;
        var from = list.IndexOf(data);
        if (from < 0) return false;
        var selected = tabs.SelectedItem;
        SetIsPinned(item, pinned);
        var target = list.Cast<object>().Count(value => !ReferenceEquals(value, data) && IsPinned(tabs, value));
        if (from != target)
        {
            list.RemoveAt(from);
            list.Insert(target, data);
        }
        if (selected is not null && list.Contains(selected)) tabs.SetCurrentValue(TabControl.SelectedItemProperty, selected);
        return true;
    }

    /// <summary>Closes a snapshot of closable pages, honoring cancellation, commands and pin state.</summary>
    public static int CloseAll(TabControl tabs)
    {
        var count = 0;
        foreach (var data in tabs.Items.Cast<object>().ToArray())
        {
            if (!tabs.Items.Contains(data) || IsPinned(tabs, data)) continue;
            var index = tabs.Items.IndexOf(data);
            if (tabs.ContainerFromIndex(index) is not TabItem)
            {
                tabs.ScrollIntoView(data);
                tabs.UpdateLayout();
            }
            if (tabs.ContainerFromIndex(tabs.Items.IndexOf(data)) is TabItem item && Close(item)) count++;
        }
        return count;
    }
}
