using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Input;
using global::Avalonia.Interactivity;
using global::Avalonia.Threading;
using global::Avalonia.VisualTree;
using global::Avalonia.Data;
using System.Runtime.CompilerServices;

namespace TheOne.Avalonia.Controls;

/// <summary>Viewport-aware selection and navigation for native tab and hierarchical controls.</summary>
public sealed class Virtualization : AvaloniaObject
{
    public static readonly AttachedProperty<bool> EnabledProperty = AvaloniaProperty.RegisterAttached<Virtualization, ItemsControl, bool>("Enabled");
    private static readonly ConditionalWeakTable<TreeView, TreeState> TreeStates = new();
    static Virtualization()
    {
        EnabledProperty.Changed.AddClassHandler<ItemsControl>((control, change) =>
        {
            control.PropertyChanged -= SelectionChanged;
            control.RemoveHandler(InputElement.KeyDownEvent, NavigateTree);
            if (control is TreeView existing && TreeStates.TryGetValue(existing, out var existingState)) existingState.Enabled = change.GetNewValue<bool>();
            if (!change.GetNewValue<bool>()) return;
            if (control is TabControl) control.PropertyChanged += SelectionChanged;
            if (control is TreeView tree)
            {
                control.AddHandler(InputElement.KeyDownEvent, NavigateTree, RoutingStrategies.Tunnel);
                TreeStates.GetValue(tree, _ => new TreeState()).Attach(tree);
            }
        });
    }
    public static bool GetEnabled(ItemsControl control) => control.GetValue(EnabledProperty);
    public static void SetEnabled(ItemsControl control, bool value) => control.SetValue(EnabledProperty, value);
    private static void SelectionChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (sender is TabControl tabs && args.Property == TabControl.SelectedIndexProperty)
            Dispatcher.UIThread.Post(() => { if (tabs.IsAttachedToVisualTree() && GetEnabled(tabs)) tabs.ScrollIntoView(tabs.SelectedIndex); }, DispatcherPriority.Loaded);
    }

    internal static bool ReadExpandedState(TreeView tree, object item) => TreeStates.TryGetValue(tree, out var state) && state.ReadExpanded(item);
    internal static void RestoreExpandedState(TreeView tree, object item, bool expanded, IEnumerable<object?>? children) => TreeStates.GetValue(tree, static _ => new TreeState()).RestoreExpanded(item, expanded, children);

    // Popup trees share the owner's weak model-identity state; closing a popup is
    // container recycling, not a data reload. Call before adding any target items.
    internal static void ShareTreeState(TreeView source, TreeView target)
    {
        if (TreeStates.TryGetValue(target, out var previous)) previous.Detach(target);
        TreeStates.Remove(target);
        var state = TreeStates.GetValue(source, static _ => new TreeState());
        TreeStates.Add(target, state); state.Attach(target);
    }

    /// <summary>Materializes only the ancestor path and target of a large tree.</summary>
    public static TreeViewItem? BringIntoView(TreeView tree, params int[] path)
    {
        ItemsControl owner = tree;
        TreeViewItem? node = null;
        for (var depth = 0; depth < path.Length; depth++)
        {
            node = Realize(owner, path[depth]);
            if (node is null) return null;
            if (depth < path.Length - 1)
            {
                node.SetCurrentValue(TreeViewItem.IsExpandedProperty, true);
                CompleteExpansion(node);
                node.UpdateLayout();
            }
            owner = node;
        }
        return node;
    }

    /// <summary>Selects all nodes exposed by expanded branches without creating their containers.</summary>
    public static void SelectAll(TreeView tree) => TreeState.ReplaceSelection(tree, TreeStates.GetValue(tree, _ => new TreeState()).VisibleItems(tree.Items).ToList());

    private sealed class TreeState
    {
        private sealed class Expanded { public bool Value; public IEnumerable<object?>? Children; }
        public bool Enabled { get; set; } = true;
        public bool ReadExpanded(object item) => _expanded.TryGetValue(item, out var state) && state.Value;
        public void RestoreExpanded(object item, bool value, IEnumerable<object?>? children)
        {
            var state = _expanded.GetValue(item, static _ => new Expanded());
            state.Value = value; state.Children = children;
        }
        private sealed class Snapshot { public object? Data; public bool Recycled; public bool Hooked; }
        private readonly ConditionalWeakTable<object, Expanded> _expanded = new();
        private readonly ConditionalWeakTable<TreeViewItem, Snapshot> _snapshots = new();
        public void Detach(ItemsControl owner)
        {
            owner.ContainerPrepared -= Prepared; owner.ContainerClearing -= Clearing;
        }
        public void Attach(ItemsControl owner)
        {
            owner.ContainerPrepared -= Prepared; owner.ContainerClearing -= Clearing;
            owner.ContainerPrepared += Prepared; owner.ContainerClearing += Clearing;
        }
        private void Prepared(object? sender, ContainerPreparedEventArgs args)
        {
            if (!Enabled || sender is not ItemsControl owner || args.Container is not TreeViewItem node) return;
            var snapshot = _snapshots.GetValue(node, _ => new Snapshot());
            if (!snapshot.Hooked) { snapshot.Hooked = true; Attach(node); node.PropertyChanged += ExpandedChanged; }
            var data = args.Index >= 0 && args.Index < owner.ItemCount ? owner.Items[args.Index] : null;
            if (data is not null && data != node && BindingOperations.GetBindingExpressionBase(node, TreeViewItem.IsExpandedProperty) is null)
            {
                if (_expanded.TryGetValue(data, out var state)) node.SetCurrentValue(TreeViewItem.IsExpandedProperty, state.Value);
                else if (snapshot.Recycled) node.SetCurrentValue(TreeViewItem.IsExpandedProperty, false);
            }
            snapshot.Data = data;
            Remember(node, snapshot);
        }
        private void ExpandedChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
        {
            if (Enabled && args.Property == TreeViewItem.IsExpandedProperty && sender is TreeViewItem node && _snapshots.TryGetValue(node, out var snapshot)) Remember(node, snapshot);
        }
        private void Remember(TreeViewItem node, Snapshot snapshot)
        {
            if (snapshot.Data is not { } data || data == node) return;
            var state = _expanded.GetValue(data, _ => new Expanded());
            state.Value = node.IsExpanded;
            state.Children = node.ItemsSource as IEnumerable<object?>;
        }
        private void Clearing(object? sender, ContainerClearingEventArgs args)
        {
            if (!Enabled || args.Container is not TreeViewItem node || !_snapshots.TryGetValue(node, out var snapshot)) return;
            if (snapshot.Data is { } data && data != node && BindingOperations.GetBindingExpressionBase(node, TreeViewItem.IsExpandedProperty) is null)
                _expanded.GetValue(data, _ => new Expanded()).Value = node.IsExpanded;
            snapshot.Data = null; snapshot.Recycled = true;
        }
        public IEnumerable<object> VisibleItems(IEnumerable<object?> items)
        {
            foreach (var data in items)
            {
                if (data is null) continue;
                yield return data;
                var children = data is TreeViewItem { IsExpanded: true } manual ? manual.Items :
                    _expanded.TryGetValue(data, out var state) && state.Value ? state.Children : null;
                if (children is not null) foreach (var child in VisibleItems(children)) yield return child;
            }
        }
        public void SelectRange(TreeView tree, object? target)
        {
            var anchor = tree.SelectedItem;
            if (anchor is null || target is null) return;
            var selected = new List<object>();
            var between = false;
            foreach (var item in VisibleItems(tree.Items))
            {
                var boundary = Equals(item, anchor) || Equals(item, target);
                if (boundary) between = !between;
                if (between || boundary) selected.Add(item);
                if (boundary && (!between || Equals(anchor, target))) break;
            }
            // Keep the original anchor first, matching the native SelectedItem contract.
            selected.Remove(anchor); selected.Insert(0, anchor);
            ReplaceSelection(tree, selected);
        }
        public static void ReplaceSelection(TreeView tree, List<object> selected)
        {
            if (tree.SelectedItems is global::Avalonia.Collections.AvaloniaList<object> collection)
            {
                collection.Clear(); collection.AddRange(selected);
            }
            else
            {
                // Preserve a caller-supplied collection; detach native per-item bookkeeping
                // while updating it, then synchronize realized containers once.
                var original = tree.SelectedItems;
                tree.SetCurrentValue(TreeView.SelectedItemsProperty, new global::Avalonia.Collections.AvaloniaList<object>());
                try { original.Clear(); foreach (var item in selected) original.Add(item); }
                finally { tree.SetCurrentValue(TreeView.SelectedItemsProperty, original); }
            }
        }
    }
    private static TreeViewItem? Realize(ItemsControl owner, int index)
    {
        if (index < 0 || index >= owner.ItemCount) return null;
        if (owner is TreeViewItem { IsExpanded: true } parent) CompleteExpansion(parent);
        owner.ScrollIntoView(index);
        return owner.ContainerFromIndex(index) as TreeViewItem;
    }
    private static void CompleteExpansion(TreeViewItem node) => node.GetVisualDescendants().OfType<RevealHost>().FirstOrDefault(host => host.TemplatedParent == node)?.CompleteTransition();
    private static ItemsControl? Parent(TreeViewItem node) => node.Parent as ItemsControl;
    private static TreeViewItem? LastDescendant(TreeViewItem? node)
    {
        while (node is { IsExpanded: true, ItemCount: > 0 })
        {
            var child = Realize(node, node.ItemCount - 1);
            if (child is null) break;
            node = child;
        }
        return node;
    }
    private static TreeViewItem? Next(TreeViewItem node)
    {
        if (node.IsExpanded && node.ItemCount > 0) return Realize(node, 0);
        for (var current = node; Parent(current) is { } owner;)
        {
            var index = owner.IndexFromContainer(current);
            if (index + 1 < owner.ItemCount) return Realize(owner, index + 1);
            if (owner is not TreeViewItem parent) return null;
            current = parent;
        }
        return null;
    }
    private static TreeViewItem? Previous(TreeViewItem node)
    {
        if (Parent(node) is not { } owner) return null;
        var index = owner.IndexFromContainer(node);
        return index > 0 ? LastDescendant(Realize(owner, index - 1)) : owner as TreeViewItem;
    }
    private static void NavigateTree(object? sender, KeyEventArgs args)
    {
        if (sender is not TreeView tree || tree is NavMenu { UsesPopupNavigation: true }) return;
        // Embedded editors retain their own arrow keys and Select All shortcuts.
        if (args.Source is Visual source && source.GetVisualAncestors().Prepend(source).TakeWhile(visual => visual is not TreeViewItem)
            .Any(visual => visual is TextBox or ComboBox or NumericUpDown or Slider or CalendarDatePicker or DatePicker or TimePicker or AutoCompleteBox)) return;
        if ((tree.SelectionMode & SelectionMode.Multiple) != 0 && Application.Current?.PlatformSettings?.HotkeyConfiguration.SelectAll.Any(gesture => gesture.Matches(args)) == true)
        {
            SelectAll(tree); args.Handled = true; return;
        }
        if (args.Key is not (Key.Up or Key.Down or Key.Left or Key.Right or Key.Home or Key.End)) return;
        if ((args.KeyModifiers & KeyModifiers.Alt) != 0) return;
        var node = args.Source as TreeViewItem ?? (args.Source as Visual)?.GetVisualAncestors().OfType<TreeViewItem>().FirstOrDefault();
        if (node is null) return;
        args.Handled = true;
        TreeViewItem? target = null;
        switch (args.Key)
        {
            case Key.Right:
                if (!node.IsExpanded && node.ItemCount > 0) node.SetCurrentValue(TreeViewItem.IsExpandedProperty, true);
                else if (node.IsExpanded && node.ItemCount > 0) target = Realize(node, 0);
                break;
            case Key.Left:
                if (node.IsExpanded) node.SetCurrentValue(TreeViewItem.IsExpandedProperty, false);
                else target = Parent(node) as TreeViewItem;
                break;
            case Key.Down: target = Next(node); break;
            case Key.Up: target = Previous(node); break;
            case Key.Home: target = Realize(tree, 0); break;
            case Key.End: target = LastDescendant(Realize(tree, tree.ItemCount - 1)); break;
        }
        // Disabled rows must not become a keyboard selection.
        while (target is { IsEffectivelyEnabled: false } && args.Key is Key.Up or Key.Down)
            target = args.Key == Key.Up ? Previous(target) : Next(target);
        if (target is not { IsEffectivelyEnabled: true } || Parent(target) is not { } parent) return;
        var item = parent.ItemFromContainer(target);
        if ((args.KeyModifiers & (KeyModifiers.Control | KeyModifiers.Meta)) == 0)
        {
            if ((args.KeyModifiers & KeyModifiers.Shift) != 0 && (tree.SelectionMode & SelectionMode.Multiple) != 0)
            {
                TreeStates.GetValue(tree, _ => new TreeState()).SelectRange(tree, item);
            }
            else tree.SetCurrentValue(TreeView.SelectedItemProperty, item);
        }
        target.Focus(NavigationMethod.Unspecified);
    }
}
