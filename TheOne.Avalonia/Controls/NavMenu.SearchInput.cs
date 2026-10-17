using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

public partial class NavMenu
{
    public static readonly AttachedProperty<NavMenu?> SearchOwnerProperty =
        AvaloniaProperty.RegisterAttached<NavMenu, TextBox, NavMenu?>("SearchOwner");

    public static NavMenu? GetSearchOwner(TextBox input)
    {
        return input.GetValue(SearchOwnerProperty);
    }

    public static void SetSearchOwner(TextBox input, NavMenu? menu)
    {
        input.SetValue(SearchOwnerProperty, menu);
    }

    private static void SearchInputChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Sender is not TextBox input) return;
        input.RemoveHandler(KeyDownEvent, SearchInputKeyDown);
        if (change.NewValue is NavMenu) input.AddHandler(KeyDownEvent, SearchInputKeyDown, RoutingStrategies.Tunnel);
    }

    private static void SearchInputKeyDown(object? sender, KeyEventArgs args)
    {
        if (sender is not TextBox input ||
            GetSearchOwner(input) is not { IsEffectivelyEnabled: true, IsSearching: true } menu ||
            args.KeyModifiers != KeyModifiers.None || args.Handled) return;
        if (args.Key == Key.Escape)
        {
            menu.SetCurrentValue(SearchTextProperty, "");
            RestoreInputFocus(input);
            args.Handled = true;
            return;
        }

        if (args.Key is not (Key.Up or Key.Down or Key.Enter)) return;
        var session = menu._searchSession;
        if (session is null) return;
        var results = session.Results;
        if (results.Count > 0)
        {
            var index = results.FindIndex(result => ReferenceEquals(result.Item, menu.SelectedItem));
            index = args.Key switch
            {
                Key.Up => index < 0 ? results.Count - 1 : Math.Max(0, index - 1),
                Key.Down => index < 0 ? 0 : Math.Min(results.Count - 1, index + 1),
                _ => Math.Max(0, index)
            };
            if (menu.SelectSearchPath(results[index].Path) is { } row && args.Key == Key.Enter)
            {
                var owner = row.Parent as NavMenu ?? menu;
                owner.Invoke(row);
                menu.CloseSubmenu();
            }
        }

        // A result's native container is realized without moving focus or the input caret.
        RestoreInputFocus(input);
        args.Handled = true;
    }

    private static void RestoreInputFocus(TextBox input)
    {
        // A previously click-opened Standard flyout returns focus to its anchor on close.
        if (input.IsAttachedToVisualTree() && input.IsEffectivelyVisible && !input.IsFocused) input.Focus();
    }

    private NavMenuItem? SelectSearchPath(int[] path)
    {
        if (!this.IsAttachedToVisualTree() || path.Length == 0) return null;
        if (!UsesPopupNavigation)
        {
            if (Virtualization.BringIntoView(this, path) is not NavMenuItem row || !row.IsEffectivelyEnabled ||
                row.IsGroup) return null;
            SetCurrentValue(SelectedItemProperty, ((ItemsControl)row.Parent!).ItemFromContainer(row));
            return row;
        }

        ScrollIntoView(path[0]);
        UpdateLayout();
        if (ContainerFromIndex(path[0]) is not NavMenuItem node) return null;
        if (path.Length > 1)
            return ShowSubmenu(node, false) && Submenu is { } child ? child.SelectSearchPath(path[1..]) : null;
        SetCurrentValue(SelectedItemProperty, ItemFromContainer(node));
        return node;
    }
}