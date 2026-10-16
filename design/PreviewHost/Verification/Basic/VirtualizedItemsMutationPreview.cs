using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.LogicalTree;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

/// <summary>Single Reset, Replace and source-switch contracts; generated hidden pools are permitted.</summary>
internal static class VirtualizedItemsMutationPreview
{
    internal static void Verify(Action<bool, string> check, string? output = null)
    {
        if (output is not null) Directory.CreateDirectory(output);
        foreach (var name in new[] { "ListBox", "ComboBox", "AutoCompleteBox", "ItemsControl", "TabStrip", "TabControl" })
        foreach (var own in new[] { true, false })
            VerifyCase(name, own, check, output);
    }

    internal static void VerifyTreeDescendantMutations(Action<bool, string> check, string? output = null)
    {
        foreach (var kind in new[] { "generated-root-replace", "nested-model-reset", "own-root-recycled-model-replace", "own-root-recycled-model-reset", "generated-parent-recycled-model-reset" })
        {
            var oldLeaf = new TreeSelectionRow("old-leaf", new ResetRows([]));
            var oldBranch = new TreeSelectionRow("old-branch", new ResetRows([oldLeaf]));
            var newLeaf = new TreeSelectionRow("new-leaf", new ResetRows([]));
            var newBranch = new TreeSelectionRow("new-branch", new ResetRows([newLeaf]));
            var branches = new ResetRows([oldBranch]);
            var own = kind.StartsWith("own-", StringComparison.Ordinal);
            var recycle = own || kind == "generated-parent-recycled-model-reset";
            if (recycle)
                for (var i = 1; i <= 40; i++) branches.Add(new TreeSelectionRow("branch-" + i, new ResetRows([])));
            var template = new FuncTreeDataTemplate<TreeSelectionRow>((row, _) => new TextBlock { Text = row?.Title },
                row => row?.Children ?? (IEnumerable<object>)Array.Empty<object>());
            TreeViewItem? ownRoot = own ? new TreeViewItem { Header = "own-root", ItemsSource = branches, ItemTemplate = template, IsExpanded = true } : null;
            var rootSource = new ResetRows([ownRoot is not null ? (object)ownRoot : kind == "generated-root-replace" ? oldBranch : new TreeSelectionRow("parent", branches)]);
            var tree = new TreeView { Height = 220, ItemsSource = rootSource, ItemTemplate = template };
            Virtualization.SetEnabled(tree, true);
            var window = new Window { Width = 600, Height = 300, Content = new Border { Padding = new Thickness(16), Child = tree } };
            Motion.SetEnabled(window, false); window.Show(); Flush();
            var label = "Virtualized mutation/tree-descendant/" + kind;
            try
            {
                var parent = (TreeViewItem)tree.ContainerFromIndex(0)!;
                parent.IsExpanded = true; Flush();
                var branch = kind == "generated-root-replace" ? parent : (TreeViewItem)parent.ContainerFromIndex(0)!;
                branch.IsExpanded = true; Flush();
                tree.SelectedItem = oldLeaf; Flush();
                check(ReferenceEquals(tree.SelectedItem, oldLeaf) && tree.SelectedItems.Contains(oldLeaf) &&
                    branch.ContainerFromIndex(0) is TreeViewItem { IsSelected: true } selected && selected.IsAttachedToVisualTree(),
                    label + ": a real expanded generated grandchild is selected before retiring its branch");
                if (recycle)
                {
                    var removedDuringScroll = 0;
                    void SelectionChanged(object? _, SelectionChangedEventArgs args)
                    {
                        if (args.RemovedItems.Cast<object>().Any(item => ReferenceEquals(item, oldLeaf))) removedDuringScroll++;
                    }
                    tree.SelectionChanged += SelectionChanged;
                    try { parent.ScrollIntoView(35); Flush(); parent.UpdateLayout(); Flush(); }
                    finally { tree.SelectionChanged -= SelectionChanged; }
                    check(parent.ContainerFromIndex(0) is null && parent.ContainerFromIndex(35) is { } distant && distant.IsAttachedToVisualTree(),
                        label + ": the selected leaf's generated parent container has actually been recycled");
                    check(ReferenceEquals(tree.SelectedItem, oldLeaf) && tree.SelectedItems.Contains(oldLeaf),
                        label + ": scrolling preserves the old leaf selection until its branch is replaced");
                    check(removedDuringScroll == 0,
                        label + $": scrolling does not transiently remove the old leaf from selection (removalEvents={removedDuringScroll})");
                }
                if (kind == "nested-model-reset") branches.Reset([newBranch]);
                else if (kind is "own-root-recycled-model-reset" or "generated-parent-recycled-model-reset")
                {
                    var notifications = new List<NotifyCollectionChangedAction>();
                    void Changed(object? _, NotifyCollectionChangedEventArgs args) => notifications.Add(args.Action);
                    branches.CollectionChanged += Changed;
                    try { branches.Reset([newBranch]); }
                    finally { branches.CollectionChanged -= Changed; }
                    check(notifications.SequenceEqual(new[] { NotifyCollectionChangedAction.Reset }),
                        label + ": the recycled model branch is retired by one real nested ItemsSource Reset");
                }
                else if (own) rootSource[0] = new TreeViewItem { Header = "replacement-own-root", ItemsSource = new ResetRows([newBranch]), ItemTemplate = template, IsExpanded = true };
                else rootSource[0] = newBranch;
                Flush();
                var current = CurrentTreeItems(rootSource);
                var noRetiredSelection = !tree.SelectedItems.Contains(oldLeaf) && !ReferenceEquals(tree.SelectedItem, oldLeaf) &&
                    tree.SelectedItems.Cast<object>().All(current.Contains) && (tree.SelectedItem is null || current.Contains(tree.SelectedItem));
                if (!noRetiredSelection && output is not null)
                {
                    Directory.CreateDirectory(output);
                    using var frame = window.CaptureRenderedFrame(); frame?.Save(Path.Combine(output, label.Replace('/', '-') + ".png"), PngBitmapEncoderOptions.Default);
                }
                check(noRetiredSelection, label + $": retiring the old branch removes selected generated descendants (selected={TreeSelectionTitle(tree.SelectedItem)}, selectedItems={string.Join(",", tree.SelectedItems.Cast<object>().Select(TreeSelectionTitle))})");
                parent = (TreeViewItem)tree.ContainerFromIndex(0)!; parent.IsExpanded = true; Flush();
                if (kind is "own-root-recycled-model-reset" or "generated-parent-recycled-model-reset") { parent.ScrollIntoView(0); Flush(); }
                branch = kind == "generated-root-replace" ? parent : (TreeViewItem)parent.ContainerFromIndex(0)!;
                branch.IsExpanded = true; Flush();
                tree.SelectedItem = newLeaf; Flush();
                check(ReferenceEquals(tree.SelectedItem, newLeaf) && tree.SelectedItems.Cast<object>().All(current.Contains) &&
                    branch.ContainerFromIndex(0) is TreeViewItem { IsSelected: true } replacement && replacement.IsAttachedToVisualTree(),
                    label + ": selecting the current replacement leaf has valid data and visible selected state without retired values");
            }
            finally { window.Close(); Flush(); }
        }
        VerifyTreeMultipleSelectionCollection(check);
    }

    private static void VerifyTreeMultipleSelectionCollection(Action<bool, string> check)
    {
        const string label = "Virtualized mutation/tree-descendant/multiple-caller-selection";
        var oldLeaf = new TreeSelectionRow("old-leaf", new ResetRows([]));
        var stableLeaf = new TreeSelectionRow("stable-leaf", new ResetRows([]));
        var newLeaf = new TreeSelectionRow("new-leaf", new ResetRows([]));
        var stableBranch = new TreeSelectionRow("stable-branch", new ResetRows([stableLeaf]));
        var branches = new ResetRows([new TreeSelectionRow("old-branch", new ResetRows([oldLeaf])), stableBranch]);
        for (var i = 2; i <= 40; i++) branches.Add(new TreeSelectionRow("branch-" + i, new ResetRows([])));
        var rootSource = new ResetRows([new TreeSelectionRow("parent", branches)]);
        var selection = new ObservableCollection<object>();
        var template = new FuncTreeDataTemplate<TreeSelectionRow>((row, _) => new TextBlock { Text = row?.Title },
            row => row?.Children ?? (IEnumerable<object>)Array.Empty<object>());
        var tree = new TreeView { Height = 220, ItemsSource = rootSource, ItemTemplate = template, SelectionMode = SelectionMode.Multiple, SelectedItems = selection };
        Virtualization.SetEnabled(tree, true);
        var window = new Window { Width = 600, Height = 300, Content = new Border { Padding = new Thickness(16), Child = tree } };
        Motion.SetEnabled(window, false); window.Show(); Flush();
        try
        {
            var parent = (TreeViewItem)tree.ContainerFromIndex(0)!; parent.IsExpanded = true; Flush();
            var oldContainer = (TreeViewItem)parent.ContainerFromIndex(0)!; oldContainer.IsExpanded = true;
            var stableContainer = (TreeViewItem)parent.ContainerFromIndex(1)!; stableContainer.IsExpanded = true; Flush();
            selection.Add(oldLeaf); selection.Add(stableLeaf); Flush();
            check(ReferenceEquals(tree.SelectedItems, selection) && selection.SequenceEqual(new object[] { oldLeaf, stableLeaf }) &&
                oldContainer.ContainerFromIndex(0) is TreeViewItem { IsSelected: true } oldSelected && oldSelected.IsAttachedToVisualTree() &&
                stableContainer.ContainerFromIndex(0) is TreeViewItem { IsSelected: true } stableSelected && stableSelected.IsAttachedToVisualTree(),
                label + ": two visible leaves use the caller's original selection collection");
            var removedDuringScroll = 0;
            void ScrolledSelectionChanged(object? _, SelectionChangedEventArgs args)
            {
                if (args.RemovedItems.Cast<object>().Any(item => ReferenceEquals(item, oldLeaf) || ReferenceEquals(item, stableLeaf))) removedDuringScroll++;
            }
            tree.SelectionChanged += ScrolledSelectionChanged;
            try { parent.ScrollIntoView(35); Flush(); parent.UpdateLayout(); Flush(); }
            finally { tree.SelectionChanged -= ScrolledSelectionChanged; }
            check(parent.ContainerFromIndex(0) is null && parent.ContainerFromIndex(1) is null &&
                parent.ContainerFromIndex(35) is { } distant && distant.IsAttachedToVisualTree(),
                label + ": both selected leaves' generated parent containers have actually been recycled");
            check(ReferenceEquals(tree.SelectedItems, selection) && selection.SequenceEqual(new object[] { oldLeaf, stableLeaf }) && removedDuringScroll == 0,
                label + $": recycling preserves both selections and collection identity without removal events (removalEvents={removedDuringScroll})");
            var stableRemovals = 0;
            void RetiredSelectionChanged(object? _, SelectionChangedEventArgs args)
            {
                if (args.RemovedItems.Cast<object>().Any(item => ReferenceEquals(item, stableLeaf))) stableRemovals++;
            }
            tree.SelectionChanged += RetiredSelectionChanged;
            try
            {
                branches[0] = new TreeSelectionRow("new-branch", new ResetRows([newLeaf])); Flush();
                var current = CurrentTreeItems(rootSource);
                check(ReferenceEquals(tree.SelectedItems, selection) && selection.SequenceEqual(new object[] { stableLeaf }) &&
                    ReferenceEquals(tree.SelectedItem, stableLeaf) && selection.All(current.Contains) && stableRemovals == 0,
                    label + ": replacing the recycled branch removes only its old leaf and preserves the sibling selection and collection instance");
                parent.ScrollIntoView(0); Flush();
                var newContainer = (TreeViewItem)parent.ContainerFromIndex(0)!; newContainer.IsExpanded = true; Flush();
                selection.Add(newLeaf); Flush();
                check(newContainer.ContainerFromIndex(0) is TreeViewItem { IsSelected: true } newSelected && newSelected.IsAttachedToVisualTree() &&
                    ReferenceEquals(tree.SelectedItems, selection) && selection.Contains(stableLeaf) && selection.Contains(newLeaf),
                    label + ": the replacement leaf is actually selected before resetting its branch");
                parent.ScrollIntoView(35); Flush(); parent.UpdateLayout(); Flush();
                var notifications = new List<NotifyCollectionChangedAction>();
                void ResetChanged(object? _, NotifyCollectionChangedEventArgs args) => notifications.Add(args.Action);
                branches.CollectionChanged += ResetChanged;
                try { branches.Reset([new TreeSelectionRow("reset-branch", new ResetRows([])), stableBranch]); }
                finally { branches.CollectionChanged -= ResetChanged; }
                Flush(); current = CurrentTreeItems(rootSource);
                check(notifications.SequenceEqual(new[] { NotifyCollectionChangedAction.Reset }) && ReferenceEquals(tree.SelectedItems, selection) &&
                    selection.SequenceEqual(new object[] { stableLeaf }) && ReferenceEquals(tree.SelectedItem, stableLeaf) && selection.All(current.Contains) && stableRemovals == 0,
                    label + ": a real Reset retires only the changed branch selection while keeping the unchanged sibling and original collection");
            }
            finally { tree.SelectionChanged -= RetiredSelectionChanged; }
            parent.ScrollIntoView(1); Flush();
            stableContainer = (TreeViewItem)parent.ContainerFromIndex(1)!; stableContainer.IsExpanded = true; Flush();
            check(ReferenceEquals(tree.SelectedItems, selection) && stableContainer.ContainerFromIndex(0) is TreeViewItem { IsSelected: true } remaining &&
                ReferenceEquals(remaining.DataContext, stableLeaf) && remaining.IsAttachedToVisualTree(),
                label + ": the unchanged sibling is still visibly selected when realized again");
        }
        finally { window.Close(); Flush(); }
    }

    private static HashSet<object> CurrentTreeItems(IEnumerable<object> roots)
    {
        var current = new HashSet<object>(ReferenceEqualityComparer.Instance);
        void Add(object item)
        {
            if (!current.Add(item)) return;
            foreach (var child in item is TreeSelectionRow row ? row.Children : item is TreeViewItem node ? node.Items.Cast<object>() : Array.Empty<object>()) Add(child);
        }
        foreach (var root in roots) Add(root);
        return current;
    }

    private static string TreeSelectionTitle(object? item) => item is TreeSelectionRow row ? row.Title : item?.GetType().Name ?? "null";
    private sealed class TreeSelectionRow(string title, ResetRows children)
    {
        public string Title { get; } = title;
        public ResetRows Children { get; } = children;
    }

    private static void VerifyCase(string name, bool own, Action<bool, string> check, string? output)
    {
        using var fixture = new MutationFixture(name, own);
        var label = "Virtualized mutation/" + name + "/" + (own ? "own" : "models");
        fixture.Open();
        fixture.Select(fixture.Current[1]); fixture.Show(0);
        VerifyState(fixture, label + "/initial", check, output);

        var old = fixture.Current.ToArray();
        var reset = fixture.CreateRows("R"); reset[2] = old[0]; reset[3] = old[2];
        var notifications = new List<NotifyCollectionChangedAction>();
        void Changed(object? _, NotifyCollectionChangedEventArgs args) => notifications.Add(args.Action);
        fixture.A.CollectionChanged += Changed;
        fixture.A.Reset(reset);
        fixture.A.CollectionChanged -= Changed;
        fixture.Refresh(); fixture.Show(0);
        check(notifications.SequenceEqual(new[] { NotifyCollectionChangedAction.Reset }), label + "/reset: replacement raises one real Reset notification");
        VerifyState(fixture, label + "/reset", check, output);

        // Release a visible old own control before reusing it at another position;
        // duplicating the same Control at two indices would be an invalid fixture.
        fixture.Select(fixture.Current[1]);
        var first = fixture.Current[0]; var third = fixture.Current[2]; var anchor = fixture.Current[1];
        fixture.Current[0] = fixture.CreateRow("X", 0); fixture.Refresh(); fixture.Show(0);
        VerifyState(fixture, label + "/replace-new", check, output);
        fixture.Current[2] = first; fixture.Refresh(); fixture.Show(2);
        VerifyState(fixture, label + "/replace-reused-first", check, output);
        fixture.Current[0] = third; fixture.Refresh(); fixture.Show(0);
        VerifyState(fixture, label + "/replace-reused-third", check, output);
        if (name != "AutoCompleteBox")
            check(fixture.Owner is not SelectingItemsControl selector || ReferenceEquals(selector.SelectedItem, anchor),
                label + "/replace: replacing other indices retains the unaffected selected identity");

        fixture.Select(fixture.Current[41]); fixture.Show(40);
        VerifyState(fixture, label + "/A-scrolled", check, output);
        fixture.SetSource(fixture.B); fixture.Show(40);
        VerifyState(fixture, label + "/A-to-B-scrolled", check, output);
        fixture.Show(0);
        VerifyState(fixture, label + "/B-first", check, output);
        fixture.SetSource(fixture.A); fixture.Show(40);
        VerifyState(fixture, label + "/B-to-A-scrolled", check, output);
        fixture.Show(0);
        VerifyState(fixture, label + "/A-returned", check, output);
    }

    private static void VerifyState(MutationFixture fixture, string label, Action<bool, string> check, string? output)
    {
        var owner = fixture.Owner;
        var target = fixture.Target;
        var current = fixture.Current;
        var expected = current[target];
        var container = owner.ContainerFromIndex(target);
        var targetValid = container is not null && Mounted(owner, container, target) &&
            ReferenceEquals(owner.ItemFromContainer(container), expected) && (!fixture.Own || ReferenceEquals(container, expected)) &&
            HeaderAndContextMatch(fixture, container, expected, requireVisible: true);
        var active = owner.GetRealizedContainers().Where(control => control.IsEffectivelyVisible || owner.IndexFromContainer(control) >= 0).ToArray();
        var currentValid = active.Length > 0 && active.All(control =>
        {
            var index = owner.IndexFromContainer(control);
            return index >= 0 && index < current.Count && Mounted(owner, control, index) &&
                ReferenceEquals(owner.ItemFromContainer(control), current[index]) && HeaderAndContextMatch(fixture, control, current[index]);
        });
        var retired = fixture.KnownOwned.Where(control => !current.Any(item => ReferenceEquals(item, control))).ToArray();
        var retiredMounted = retired.Where(control => control.GetVisualParent() is not null || control.Parent is not null || control.IsAttachedToVisualTree()).ToArray();
        var selectionValid = SelectionValid(fixture, active);
        if (!targetValid || !currentValid || retiredMounted.Length > 0 || !selectionValid) Save(owner, output, label);
        check(owner.ItemCount == current.Count && Enumerable.Range(0, current.Count).All(index => ReferenceEquals(owner.Items[index], current[index])),
            label + ": the active item view contains only the current source identities at their current indices");
        check(targetValid && currentValid, label + $": realized headers retain real parents, current identity, caller/generated DataContext and current text (target={target}, attached={container?.IsAttachedToVisualTree()}, index={Index(owner, container)}, currentValid={currentValid})" +
            (targetValid && currentValid ? "" : Describe(owner, active)));
        check(selectionValid, label + ": selection contains only current items and visible selected states match the current native selection model" +
            (selectionValid ? "" : DescribeSelection(fixture, active)));
        // IsSelected on a detached caller-owned Control is data the caller may retain.
        // Only owner selection and current visible states are selection contracts here.
        check(retiredMounted.Length == 0, label + $": old own controls release visual and logical parents after leaving Items (retired={retired.Length}, retainedSelectedValues={retired.Count(control => SelectedFlag(control) == true)})" +
            string.Concat(retiredMounted.Select(control => $"\n  Old {DescribeItem(control)} attached={control.IsAttachedToVisualTree()}, visualParent={control.GetVisualParent()?.GetType().Name ?? "null"}, logicalParent={control.Parent?.GetType().Name ?? "null"}, visible={control.IsVisible}, selectedData={SelectedFlag(control)}")));
    }

    private static bool Mounted(ItemsControl owner, Control container, int index) => container.IsAttachedToVisualTree() &&
        ReferenceEquals(container.GetVisualParent(), owner.ItemsPanelRoot) && ReferenceEquals(container.Parent, owner) &&
        owner.IndexFromContainer(container) == index && ReferenceEquals(owner.ContainerFromIndex(index), container);

    private static bool HeaderAndContextMatch(MutationFixture fixture, Control container, object item, bool requireVisible = false)
    {
        var model = Model(item);
        if (!ReferenceEquals(container.DataContext, model)) return false;
        if (!container.IsEffectivelyVisible && !requireVisible) return true;
        return container.GetVisualDescendants().OfType<TextBlock>().Any(text => text.Text == model.Title && text.IsAttachedToVisualTree() &&
            text.IsEffectivelyVisible && (fixture.Own || ReferenceEquals(text.DataContext, model)));
    }

    private static bool SelectionValid(MutationFixture fixture, IEnumerable<Control> active)
    {
        var current = fixture.Current;
        if (fixture.Auto?.SelectedItem is { } outer && !current.Any(item => ReferenceEquals(item, outer))) return false;
        if (fixture.Owner is not SelectingItemsControl selector) return true;
        if (selector.SelectedItem is { } selected)
        {
            if (selector.SelectedIndex < 0 || selector.SelectedIndex >= current.Count || !ReferenceEquals(current[selector.SelectedIndex], selected)) return false;
        }
        else if (selector.SelectedIndex != -1 || fixture.Name is "TabStrip" or "TabControl" && current.Count > 0) return false;
        if (selector is ListBox list && list.SelectedItems is { } selectedItems && selectedItems.Cast<object>().Any(item => !current.Any(row => ReferenceEquals(row, item)))) return false;
        return active.Where(control => control.IsEffectivelyVisible).All(control => SelectedFlag(control) is not { } flag || flag == ReferenceEquals(selector.SelectedItem, fixture.Owner.ItemFromContainer(control)));
    }

    private static bool? SelectedFlag(Control control) => control switch
    {
        ComboBoxItem combo => combo.IsSelected,
        TabStripItem strip => strip.IsSelected,
        TabItem tab => tab.IsSelected,
        ListBoxItem list => list.IsSelected,
        _ => null
    };

    private static string DescribeSelection(MutationFixture fixture, IEnumerable<Control> active)
    {
        var owner = fixture.Owner;
        var lines = new List<string>
        {
            $"\n  Source: count={fixture.Current.Count}, ownerCount={owner.ItemCount}, owner={owner.GetType().Name}"
        };
        if (fixture.Auto is { } auto)
            lines.Add($"  Auto.SelectedItem={SelectionIdentity(auto.SelectedItem)}, sourceRefIndex={SourceReferenceIndex(fixture, auto.SelectedItem)}, sourceSame={ReferenceEquals(auto.ItemsSource, fixture.Current)}, open={auto.IsDropDownOpen}, text={auto.Text}");
        if (owner is SelectingItemsControl selector)
        {
            var currentAtIndex = selector.SelectedIndex >= 0 && selector.SelectedIndex < fixture.Current.Count ? fixture.Current[selector.SelectedIndex] : null;
            lines.Add($"  Selector.SelectedItem={SelectionIdentity(selector.SelectedItem)}, SelectedIndex={selector.SelectedIndex}, sourceRefIndex={SourceReferenceIndex(fixture, selector.SelectedItem)}, sourceAtSelectedIndex={SelectionIdentity(currentAtIndex)}, sameRefAtIndex={ReferenceEquals(selector.SelectedItem, currentAtIndex)}");
            if (selector is ListBox list && list.SelectedItems is { } selectedItems)
                lines.Add("  ListBox.SelectedItems=" + string.Join(", ", selectedItems.Cast<object>().Select(item => SelectionIdentity(item) + "@source" + SourceReferenceIndex(fixture, item))));
            foreach (var container in active)
            {
                var index = owner.IndexFromContainer(container);
                var data = owner.ItemFromContainer(container);
                lines.Add($"  Header={SelectionIdentity(container)}, index={index}, itemFromContainer={SelectionIdentity(data)}, sourceRefIndex={SourceReferenceIndex(fixture, data)}, flag={SelectedFlag(container)}, expectedFlag={ReferenceEquals(selector.SelectedItem, data)}, visible={container.IsVisible}, effectivelyVisible={container.IsEffectivelyVisible}, attached={container.IsAttachedToVisualTree()}, logicalParent={container.Parent?.GetType().Name ?? "null"}");
            }
        }
        return string.Join("\n", lines);
    }

    private static int SourceReferenceIndex(MutationFixture fixture, object? item)
    {
        if (item is null) return -1;
        for (var index = 0; index < fixture.Current.Count; index++) if (ReferenceEquals(fixture.Current[index], item)) return index;
        return -1;
    }

    private static string SelectionIdentity(object? item)
    {
        if (item is null) return "null";
        var title = item switch
        {
            MutationRow row => row.Title,
            Control { Tag: MutationRow row } => row.Title,
            string text => text,
            _ => ""
        };
        return item.GetType().Name + "#" + System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(item).ToString("x") + "(" + title + ")";
    }

    private static MutationRow Model(object item) => item is MutationRow row ? row : (MutationRow)((Control)item).Tag!;
    private static int Index(ItemsControl owner, Control? container) => container is null ? -1 : owner.IndexFromContainer(container);
    private static string DescribeItem(object item) => item.GetType().Name + "(" + Model(item).Title + ")";
    private static string Describe(ItemsControl owner, IEnumerable<Control> active) => string.Concat(active.Select(control =>
        $"\n  Current {control.GetType().Name}: index={owner.IndexFromContainer(control)}, attached={control.IsAttachedToVisualTree()}, visualParent={control.GetVisualParent()?.GetType().Name ?? "null"}, logicalParent={control.Parent?.GetType().Name ?? "null"}, data={control.DataContext?.GetType().Name ?? "null"}, visible={control.IsVisible}"));

    private static TextBlock BoundRowText(string name, bool body = false)
    {
        var text = new TextBlock { Name = name, Height = 32 };
        text.Bind(TextBlock.TextProperty, CompiledBinding.Create<MutationRow, string>(row => row.Title));
        return text;
    }

    private static void Save(ItemsControl owner, string? output, string label)
    {
        if (output is null || owner.ItemsPanelRoot is not { } panel || panel.Bounds.Width <= 0 || panel.Bounds.Height <= 0) return;
        using var image = new RenderTargetBitmap(new PixelSize((int)Math.Ceiling(panel.Bounds.Width), (int)Math.Ceiling(panel.Bounds.Height)), new Vector(96, 96));
        image.Render(panel); image.Save(Path.Combine(output, label.Replace('/', '-') + ".png"), PngBitmapEncoderOptions.Default);
    }

    private sealed class MutationFixture : IDisposable
    {
        private int _search;
        internal string Name { get; }
        internal bool Own { get; }
        internal Window Window { get; }
        internal AutoCompleteBox? Auto { get; }
        internal ItemsControl Owner { get; private set; } = null!;
        internal ResetRows A { get; }
        internal ResetRows B { get; }
        internal ResetRows Current { get; private set; }
        internal List<Control> KnownOwned { get; } = [];
        internal int Target { get; private set; }

        internal MutationFixture(string name, bool own)
        {
            Name = name; Own = own;
            A = new ResetRows(CreateRows("A")); B = new ResetRows(CreateRows("B")); Current = A;
            var template = new FuncDataTemplate<MutationRow>((_, _) => BoundRowText("MutationHeaderText"));
            Control surface;
            if (name == "AutoCompleteBox")
            {
                Auto = new AutoCompleteBox { Width = 520, ItemsSource = A, MinimumPrefixLength = 0, MinimumPopulateDelay = TimeSpan.Zero, MaxDropDownHeight = 180, FilterMode = AutoCompleteFilterMode.None };
                if (!own) Auto.ItemTemplate = template;
                surface = Auto;
            }
            else
            {
                Owner = name switch
                {
                    "ListBox" => new ListBox { Height = 180 },
                    "ComboBox" => new ComboBox { MaxDropDownHeight = 180 },
                    "ItemsControl" => new ItemsControl { Height = 180 },
                    "TabStrip" => new TabStrip { Height = 64 },
                    "TabControl" => new TabControl { Height = 220, Classes = { "card" } },
                    _ => throw new ArgumentOutOfRangeException(nameof(name))
                };
                Owner.Width = 520; Owner.ItemsSource = A;
                if (!own)
                {
                    Owner.ItemTemplate = template;
                    if (Owner is TabControl tabs) tabs.ContentTemplate = new FuncDataTemplate<MutationRow>((_, _) => BoundRowText("MutationBodyText", body: true));
                }
                surface = Owner;
            }
            Window = new Window
            {
                Width = 640, Height = 460, RequestedThemeVariant = ThemeVariant.Light, WindowDecorations = WindowDecorations.None,
                Content = new Border { Padding = new Thickness(24), Child = new StackPanel { Spacing = 16, Children = { new TextBlock { Text = name + (own ? " own" : " models") }, surface } } }
            };
            Motion.SetEnabled(Window, false);
        }

        internal object[] CreateRows(string generation) => Enumerable.Range(0, 48).Select(index => CreateRow(generation, index)).ToArray();

        internal object CreateRow(string generation, int index)
        {
            var row = new MutationRow(index, generation + "·" + index.ToString("00"));
            if (!Own) return row;
            Control control = Name switch
            {
                "ListBox" or "AutoCompleteBox" => new ListBoxItem { Content = row.Title, Height = 32 },
                "ComboBox" => new ComboBoxItem { Content = row.Title, Height = 32 },
                "ItemsControl" => new Border { Height = 32, Child = new TextBlock { Text = row.Title } },
                "TabStrip" => new TabStripItem { Content = row.Title, Width = 100 },
                "TabControl" => new TabItem { Header = row.Title, Content = new TextBlock { Name = "MutationBodyText", Text = "内容 " + row.Title } },
                _ => throw new ArgumentOutOfRangeException(nameof(Name))
            };
            control.Tag = row; control.DataContext = row; KnownOwned.Add(control); return control;
        }

        internal void Open()
        {
            Window.Show(); Flush(); Refresh();
            if (Owner is ComboBox combo)
            {
                var point = combo.TranslatePoint(new Point(combo.Bounds.Width / 2, combo.Bounds.Height / 2), Window)!.Value;
                Window.MouseMove(point); Window.MouseDown(point, global::Avalonia.Input.MouseButton.Left); Window.MouseUp(point, global::Avalonia.Input.MouseButton.Left); Flush();
            }
        }

        internal void Select(object item)
        {
            if (Owner is SelectingItemsControl selector) selector.SelectedItem = item;
            Flush();
        }

        internal void SetSource(ResetRows source)
        {
            Current = source;
            if (Auto is not null) Auto.ItemsSource = source; else Owner.ItemsSource = source;
            Refresh();
        }

        internal void Refresh()
        {
            Flush();
            if (Auto is null) return;
            var editor = Auto.GetVisualDescendants().OfType<TextBox>().Single();
            editor.Focus(); editor.SelectAll(); Window.KeyTextInput("q" + ++_search); Flush();
            var frame = new DispatcherFrame(); DispatcherTimer.RunOnce(() => frame.Continue = false, TimeSpan.FromMilliseconds(25)); Dispatcher.UIThread.PushFrame(frame);
            Auto.IsDropDownOpen = true; Flush();
            var popup = Auto.GetVisualDescendants().OfType<Popup>().Concat(Auto.GetLogicalDescendants().OfType<Popup>()).Distinct().Single(part => part.Name == "PART_Popup");
            var child = popup.Child!;
            Owner = child.GetVisualDescendants().Prepend(child).OfType<ListBox>().Single();
        }

        internal void Show(int index) { Target = index; Owner.ScrollIntoView(index); Flush(); Owner.UpdateLayout(); Flush(); }
        public void Dispose() { if (Auto is not null) Auto.IsDropDownOpen = false; if (Owner is ComboBox combo) combo.IsDropDownOpen = false; Window.Close(); Flush(); }
    }

    private sealed class MutationRow(int key, string title)
    {
        public int Key { get; } = key;
        public string Title { get; } = title;
    }

    private sealed class ResetRows(IEnumerable<object> rows) : ObservableCollection<object>(rows)
    {
        internal void Reset(IEnumerable<object> replacement)
        {
            var next = replacement.ToArray();
            CheckReentrancy();
            Items.Clear(); foreach (var item in next) Items.Add(item);
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }

    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
}
