using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;
using static TheOne.DesignPreview.InteractionRepairsPreview;

namespace TheOne.DesignPreview;

internal static class VirtualizationPreview
{
    internal static readonly string[] Rows = Enumerable.Range(0, 10000).Select(index => $"数据 {index:00000}").ToArray();

    public static void Verify()
    {
        VerifyList();
        VerifyTree();
        VerifyTabs();
        VerifyPopups();
    }

    public static void VerifyGallery(LegacyPreviewWindow window, LegacyControlGallery gallery)
    {
        window.NavigateToCategory(9);
        ControlPreview.Flush();
        var list = gallery.FindControl<ListBox>("HugeList")!;
        var tree = (TreeView)gallery.FindControl<Border>("HugeTreeHost")!.Child!;
        Motion.SetEnabled(window, true);
        Click(window, gallery.FindControl<Button>("HugeJump")!);
        Check(list.SelectedIndex == 9999 && ((Node?)tree.SelectedItem)?.Name == Rows[9000],
            "Performance preview jump button realizes distant list and tree items with motion enabled");
        var leaf = tree.GetRealizedTreeContainers().OfType<TreeViewItem>()
            .First(node => tree.TreeItemFromContainer(node) == tree.SelectedItem);
        var point = leaf.TranslatePoint(new Point(0, leaf.Bounds.Height / 2), tree)!.Value;
        Check(point.Y >= 0 && point.Y <= tree.Bounds.Height,
            "Explicit tree path navigation completes ancestor reveal before exposing its distant target");
        Click(window, gallery.FindControl<Button>("HugeTabJump")!);
        Check(gallery.FindControl<TabControl>("HugeTabs")!.SelectedIndex == 9000,
            "Performance preview tab button selects an unrealized header");
        Bounded(list, "Preview list");
        Bounded(tree, "Preview tree");
        Motion.SetEnabled(window, false);
        window.NavigateToCategory(0);
        ControlPreview.Flush();
    }

    private static Window Open(Control content, int height = 320)
    {
        var window = new Window
            { Width = 500, Height = height, Content = content, WindowDecorations = WindowDecorations.None };
        Motion.SetEnabled(window, false);
        window.Show();
        ControlPreview.Flush();
        return window;
    }

    private static void Bounded(ItemsControl control, string label)
    {
        Check(
            control.ItemsPanelRoot is VirtualizingStackPanel &&
            control.GetRealizedContainers().Count() is > 0 and < 200,
            $"{label}: {control.ItemCount} items use a recycling panel with only {control.GetRealizedContainers().Count()} realized containers");
    }

    private static void VerifyList()
    {
        var list = new ListBox { ItemsSource = Rows };
        var window = Open(list);
        Bounded(list, "ListBox");
        list.SelectedIndex = 9999;
        ControlPreview.Flush();
        list.ScrollIntoView(9999);
        ControlPreview.Flush();
        Check(list.ContainerFromIndex(9999) is { IsEffectivelyVisible: true } && Equals(list.SelectedItem, Rows[9999]),
            "Virtual ListBox can select and materialize the last item");
        Bounded(list, "Scrolled ListBox");
        window.Close();
        var flyout = new MenuFlyoutPresenter { ItemsSource = Rows };
        window = Open(flyout);
        Bounded(flyout, "MenuFlyoutPresenter");
        window.Close();
        var items = new ItemsControl { ItemsSource = Rows };
        window = Open(items);
        Bounded(items, "ItemsControl");
        items.ScrollIntoView(9000);
        ControlPreview.Flush();
        Check(items.ContainerFromIndex(9000) is not null,
            "Plain ItemsControl exposes an actual bounded scroll viewport");
        window.Close();
    }

    internal static TreeView CreateTree()
    {
        var roots = Rows.Select(text => new Node(text, [])).ToArray();
        roots[0] = new Node("包含一万个子节点", Rows.Select(text => new Node(text, [])).ToArray());
        return new TreeView
        {
            ItemsSource = roots,
            ItemTemplate = new FuncTreeDataTemplate<Node>((node, _) => new TextBlock { Text = node!.Name },
                node => node!.Children)
        };
    }

    private static void VerifyTree()
    {
        var tree = CreateTree();
        var window = Open(tree);
        Bounded(tree, "TreeView roots");
        var first = (TreeViewItem)tree.ContainerFromIndex(0)!;
        first.IsExpanded = true;
        ControlPreview.Flush();
        Bounded(first, "TreeView nested children");
        var leaf = Virtualization.BringIntoView(tree, 0, 9000);
        ControlPreview.Flush();
        Check(leaf is not null && ((Node)first.ItemFromContainer(leaf)!).Name == Rows[9000],
            "Tree path navigation realizes a distant nested node without materializing its siblings");
        Bounded(first, "Scrolled tree branch");
        tree.ScrollIntoView(9999);
        ControlPreview.Flush();
        var last = (TreeViewItem)tree.ContainerFromIndex(9999)!;
        tree.SelectedItem = tree.Items[9999];
        last.Focus();
        PressKey(window, Key.Up, PhysicalKey.ArrowUp);
        Check(((Node?)tree.SelectedItem)?.Name == Rows[9998],
            "Virtual tree Up navigation materializes an adjacent row at the viewport boundary");
        PressKey(window, Key.Home, PhysicalKey.Home);
        Check(tree.SelectedItem == tree.Items[0], "Virtual tree Home navigates back to the first root");
        var returned = (TreeViewItem)tree.ContainerFromIndex(0)!;
        Check(returned.IsExpanded, "Tree expansion state survives container recycling");
        PressKey(window, Key.End, PhysicalKey.End);
        Check(tree.SelectedItem == tree.Items[9999], "Virtual tree End navigates to the final root");
        Bounded(tree, "Keyboard-scrolled tree");
        var final = (TreeViewItem)tree.ContainerFromIndex(9999)!;
        final.Focus();
        window.KeyPress(Key.Up, RawInputModifiers.Control, PhysicalKey.ArrowUp, null);
        window.KeyRelease(Key.Up, RawInputModifiers.Control, PhysicalKey.ArrowUp, null);
        ControlPreview.Flush();
        Check(tree.SelectedItem == tree.Items[9999],
            "Ctrl plus arrow moves virtual tree focus without changing selection");
        tree.SelectionMode = SelectionMode.Multiple;
        window.KeyPress(Key.Home, RawInputModifiers.Shift, PhysicalKey.Home, null);
        window.KeyRelease(Key.Home, RawInputModifiers.Shift, PhysicalKey.Home, null);
        ControlPreview.Flush();
        Check(tree.SelectedItems.Count == 20000,
            $"Shift plus Home selects the expanded data range without creating twenty thousand containers (selected {tree.SelectedItems.Count})");
        Bounded(tree, "Range-selected tree");
        var supplied = new ObservableCollection<object> { tree.Items[9999]! };
        tree.SelectedItems = supplied;
        tree.ScrollIntoView(9999);
        ControlPreview.Flush();
        ((TreeViewItem)tree.ContainerFromIndex(9999)!).Focus();
        window.KeyPress(Key.Home, RawInputModifiers.Shift, PhysicalKey.Home, null);
        window.KeyRelease(Key.Home, RawInputModifiers.Shift, PhysicalKey.Home, null);
        ControlPreview.Flush();
        Check(ReferenceEquals(tree.SelectedItems, supplied) && supplied.Count == 20000,
            "Virtual range selection retains a caller-supplied selection collection");
        tree.SelectedItem = tree.Items[9999];
        tree.ScrollIntoView(9999);
        ControlPreview.Flush();
        ((TreeViewItem)tree.ContainerFromIndex(9999)!).Focus();
        window.KeyPress(Key.A, RawInputModifiers.Control, PhysicalKey.A, null);
        window.KeyRelease(Key.A, RawInputModifiers.Control, PhysicalKey.A, null);
        ControlPreview.Flush();
        Check(tree.SelectedItems.Count == 20000 && ReferenceEquals(tree.SelectedItems, supplied),
            "Virtual tree Select All includes an expanded branch outside the current viewport");
        Bounded(tree, "Select-all tree");
        window.Close();
        var editable = new TreeView
        {
            SelectionMode = SelectionMode.Multiple,
            ItemsSource = new[] { new Node("可编辑的节点", [new Node("子节点", [])]) },
            ItemTemplate = new FuncTreeDataTemplate<Node>((node, _) => new TextBox { Text = node!.Name },
                node => node!.Children)
        };
        window = Open(editable);
        var editableRoot = (TreeViewItem)editable.ContainerFromIndex(0)!;
        editableRoot.IsExpanded = true;
        ControlPreview.Flush();
        var editor = editableRoot.GetVisualDescendants().OfType<TextBox>().First();
        editor.Focus();
        editor.CaretIndex = 3;
        PressKey(window, Key.Left, PhysicalKey.ArrowLeft);
        Check(editor.CaretIndex == 2 && editableRoot.IsExpanded,
            "Virtual tree navigation preserves arrow keys in an embedded editor");
        var selectedCount = editable.SelectedItems.Count;
        window.KeyPress(Key.A, RawInputModifiers.Control, PhysicalKey.A, null);
        window.KeyRelease(Key.A, RawInputModifiers.Control, PhysicalKey.A, null);
        ControlPreview.Flush();
        Check(editor.SelectedText == editor.Text && editable.SelectedItems.Count == selectedCount,
            "Embedded editor Select All remains independent of virtual tree selection");
        window.Close();
    }

    private static void VerifyTabs()
    {
        foreach (var card in new[] { false, true })
        {
            var tabs = new TabControl { ItemsSource = card ? new ObservableCollection<string>(Rows) : Rows };
            if (card) tabs.Classes.Add("card");
            var window = Open(tabs);
            Bounded(tabs, card ? "Card tabs" : "Standard tabs");
            tabs.SelectedIndex = 9000;
            ControlPreview.Flush();
            Check(tabs.ContainerFromIndex(9000) is TabItem && Equals(tabs.SelectedContent, Rows[9000]),
                $"{(card ? "Card" : "Standard")} tabs realize selected content outside the initial header viewport");
            Bounded(tabs, "Distant selected tabs");
            if (card)
            {
                Check(TabFeatures.Close((TabItem)tabs.ContainerFromIndex(9000)!),
                    "Virtual card tabs close a distant selected item");
                ControlPreview.Flush();
                Check(tabs.ItemCount == 9999 && Equals(tabs.SelectedContent, Rows[9001]),
                    "Closing a virtual selected tab realizes and displays its next neighbour");
                Check(TabFeatures.Move(tabs, tabs.SelectedIndex, 0),
                    "Virtual card tabs reorder a distant selected item");
                ControlPreview.Flush();
                Check(tabs.SelectedIndex == 0 && Equals(tabs.SelectedContent, Rows[9001]),
                    "Virtual reorder keeps selected content and scrolls the header into view");
                Bounded(tabs, "Reordered virtual card tabs");
            }

            window.Close();
        }
    }

    private static void VerifyPopups()
    {
        var combo = new ComboBox { ItemsSource = Rows, MaxDropDownHeight = 240, Width = 280 };
        var auto = new AutoCompleteBox
            { ItemsSource = Rows, MinimumPrefixLength = 1, MaxDropDownHeight = 240, Width = 280 };
        var target = new Button
            { Content = "打开一万项菜单", ContextMenu = new ContextMenu { ItemsSource = Rows, MaxHeight = 240 } };
        var submenu = new MenuItem { Header = "一万条命令", ItemsSource = Rows };
        var menu = new Menu();
        menu.Items.Add(submenu);
        var window = Open(new StackPanel { Spacing = 16, Children = { combo, auto, target, menu } });
        combo.IsDropDownOpen = true;
        ControlPreview.Flush();
        Bounded(combo, "ComboBox popup");
        combo.SelectedIndex = 9000;
        combo.ScrollIntoView(9000);
        ControlPreview.Flush();
        Bounded(combo, "Scrolled ComboBox");
        combo.IsDropDownOpen = false;
        auto.Text = "数据";
        ControlPreview.Flush();
        auto.IsDropDownOpen = true;
        ControlPreview.Flush();
        var suggestions = Part<Popup>(auto, "PART_Popup").Child!.GetVisualDescendants().OfType<ListBox>().Single();
        Bounded(suggestions, "AutoCompleteBox suggestions");
        auto.IsDropDownOpen = false;
        target.ContextMenu!.Open(target);
        ControlPreview.Flush();
        Bounded(target.ContextMenu, "ContextMenu commands");
        var menuHost = TopLevel.GetTopLevel(target.ContextMenu)!;
        target.ContextMenu.SelectedIndex = 0;
        target.ContextMenu.ContainerFromIndex(0)!.Focus();
        for (var index = 0; index < 20; index++) PressKey(menuHost, Key.Down, PhysicalKey.ArrowDown);
        Check(target.ContextMenu.SelectedIndex == 20,
            "Virtual menu keyboard navigation crosses the initial container viewport");
        Bounded(target.ContextMenu, "Keyboard-scrolled ContextMenu");
        target.ContextMenu.Close();
        submenu.IsSubMenuOpen = true;
        ControlPreview.Flush();
        Bounded(submenu, "Menu submenu commands");
        submenu.IsSubMenuOpen = false;
        var bar = new Menu { ItemsSource = Rows };
        window.Content = bar;
        ControlPreview.Flush();
        Bounded(bar, "Menu top-level headers");
        window.Close();
    }

    private static void PressKey(TopLevel window, Key key, PhysicalKey physical)
    {
        window.KeyPress(key, RawInputModifiers.None, physical, null);
        window.KeyRelease(key, RawInputModifiers.None, physical, null);
        ControlPreview.Flush();
    }

    public sealed class Node(string name, Node[] children)
    {
        public string Name { get; } = name;
        public Node[] Children { get; } = children;
    }
}