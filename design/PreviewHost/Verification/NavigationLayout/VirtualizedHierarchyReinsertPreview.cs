using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class VirtualizedHierarchyReinsertPreview
{
    public static void Verify(Action<bool, string> check, string? output = null)
    {
        if (output is not null) Directory.CreateDirectory(output);
        foreach (var kind in new[] { "TreeView", "TreeViewItem", "NavMenu", "NavMenuItem", "Menu", "TopLevelMenuItem", "NestedMenuItem", "ContextMenu", "MenuFlyoutPresenter" })
        foreach (var models in new[] { false, true })
            VerifyCase(kind, models, false, check, output);
        // A native Move notification also removes/reinserts realized own containers internally.
        VerifyCase("TreeView", false, true, check, output);
        VerifyCase("ContextMenu", false, true, check, output);
    }

    private static void VerifyCase(string kind, bool models, bool move, Action<bool, string> check, string? output)
    {
        var label = $"HierarchyReinsert/{kind}/{(models ? "generated-model" : "own-container")}/{(move ? "Move" : "remove-insert")}";
        Fixture? fixture = null;
        try
        {
            fixture = kind is "TreeView" or "TreeViewItem" or "NavMenu" or "NavMenuItem"
                ? OpenTree(kind, models) : OpenMenu(kind, models);
            var owner = fixture.Owner; var source = fixture.Source; Flush();
            var initial = source.ToArray();
            var initialHeaders = Enumerable.Range(0, 3).Select(owner.ContainerFromIndex).ToArray();
            var initiallyMounted = initialHeaders.All(header => header is not null && Mounted(owner, header));
            check(initiallyMounted, label + ": initial headers are mounted in both the actual visual panel and logical owner" + (initiallyMounted ? "" : MountDetails(owner, initialHeaders)));
            if (!models && (owner is NavMenu or NavMenuItem))
                check(source.Cast<NavMenuItem>().All(item => ToolTip.GetTip(item) as string == CustomTip(item)), label + ": preparing own navigation items preserves caller-provided tooltips");
            if (fixture.Tree is { } tree) { tree.SelectedItem = initial[1]; Flush(); }
            Save(owner, output, label, "before");
            if (move) source.Move(0, 2);
            else { var first = source[0]; source.RemoveAt(0); source.Insert(2, first); }
            owner.UpdateLayout(); Flush();
            var expected = initial.Skip(1).Take(2).Concat(initial.Take(1)).Concat(initial.Skip(3)).ToArray();
            check(source.Count == initial.Length && source.Zip(expected).All(pair => ReferenceEquals(pair.First, pair.Second)), label + ": collection order and every item identity are retained");
            var current = Enumerable.Range(0, 3).Select(owner.ContainerFromIndex).ToArray();
            var currentlyMounted = current.All(header => header is not null && Mounted(owner, header));
            check(currentlyMounted, label + ": reinserted headers have a real visual parent, panel membership and logical parent" + (currentlyMounted ? "" : MountDetails(owner, current)));
            check(models || current[2] is { } own && ReferenceEquals(own, initial[0]), label + ": own containers retain their original control instance");
            check(current.Select((header, index) => header is not null && VisibleTitle(header, Title(expected[index]))).All(value => value), label + ": all three expected titles remain attached and visibly laid out");
            if (owner is NavMenu or NavMenuItem)
            {
                if (!models)
                    check(source.Cast<NavMenuItem>().All(item => ToolTip.GetTip(item) as string == CustomTip(item)), label + ": reinserting own navigation items preserves their custom tooltips");
                else
                    check(current.Select((header, index) => header is not null && ToolTip.GetTip(header) is ContentControl tip &&
                        ReferenceEquals(tip.Content, expected[index]) && tip.Content is not Control).All(value => value), label + ": generated navigation tooltips still present the current data model rather than owning the row control");
            }
            if (fixture.Tree is { } selectedTree)
                check(ReferenceEquals(selectedTree.SelectedItem, initial[1]), label + ": an unaffected selected tree item retains its data identity");
            if (models)
                check(owner.GetRealizedContainers().Count() is > 0 and < 80 && source.Count == 1000, label + ": one thousand model rows retain bounded native virtualization");
            Save(owner, output, label, "after");
        }
        catch (Exception error)
        {
            check(false, label + ": unexpected " + error.GetType().Name + ": " + error.Message);
        }
        finally { fixture?.Dispose(); Flush(); }
    }

    private static Fixture OpenTree(string kind, bool models)
    {
        var navigation = kind.StartsWith("Nav", StringComparison.Ordinal);
        var nested = kind is "TreeViewItem" or "NavMenuItem";
        TreeView tree = navigation ? new NavMenu() : new TreeView();
        tree.Height = 260;
        if (models)
        {
            if (tree is NavMenu menu)
            {
                menu.ChildrenBinding = CompiledBinding.Create<Row, ObservableCollection<object>>(row => row.Children);
                menu.ContentTemplate = new FuncDataTemplate<Row>((row, _) => new TextBlock { Text = row?.Title ?? "" });
            }
            else tree.ItemTemplate = new FuncTreeDataTemplate<Row>((row, _) => new TextBlock { Text = row?.Title ?? "" }, row => row?.Children ?? []);
        }
        var source = Source(models, navigation ? name => new NavMenuItem { Header = name } : name => new TreeViewItem { Header = name });
        if (navigation && !models)
            foreach (var item in source.Cast<NavMenuItem>()) ToolTip.SetTip(item, CustomTip(item));
        TreeViewItem? ownParent = null;
        if (nested)
        {
            if (models) tree.ItemsSource = new[] { new Row("父节点", source) };
            else
            {
                ownParent = navigation ? new NavMenuItem { Header = "父节点", ItemsSource = source, IsExpanded = true } : new TreeViewItem { Header = "父节点", ItemsSource = source, IsExpanded = true };
                tree.Items.Add(ownParent);
            }
        }
        else tree.ItemsSource = source;
        var window = Open(tree); ItemsControl owner = tree;
        if (nested)
        {
            var parent = ownParent ?? (TreeViewItem)tree.ContainerFromIndex(0)!;
            parent.IsExpanded = true; Flush(); owner = parent;
        }
        return new Fixture(window, owner, source, tree);
    }

    private static Fixture OpenMenu(string kind, bool models)
    {
        var source = Source(models, name => new MenuItem { Header = name });
        var template = models ? new FuncDataTemplate<Row>((row, _) => new TextBlock { Text = row?.Title ?? "" }) : null;
        if (kind == "Menu")
        {
            var menu = new Menu { ItemsSource = source, ItemTemplate = template, VerticalAlignment = VerticalAlignment.Top };
            return new Fixture(Open(menu), menu, source);
        }
        if (kind is "TopLevelMenuItem" or "NestedMenuItem")
        {
            var owner = new MenuItem { Header = "命令", ItemsSource = source, ItemTemplate = template };
            var bar = new Menu(); MenuItem? outer = null;
            if (kind == "NestedMenuItem") { outer = new MenuItem { Header = "文件" }; outer.Items.Add(owner); bar.Items.Add(outer); }
            else bar.Items.Add(owner);
            var window = Open(new StackPanel { Children = { bar } });
            if (outer is not null) { outer.IsSubMenuOpen = true; Flush(); }
            owner.IsSubMenuOpen = true; Flush();
            return new Fixture(window, owner, source, close: () => { owner.IsSubMenuOpen = false; if (outer is not null) outer.IsSubMenuOpen = false; });
        }
        var anchor = new Button { Content = "打开菜单", HorizontalAlignment = HorizontalAlignment.Left };
        var host = Open(new StackPanel { Children = { anchor } });
        if (kind == "ContextMenu")
        {
            var menu = new ContextMenu { ItemsSource = source, ItemTemplate = template, MaxHeight = 260 };
            anchor.ContextMenu = menu; menu.Open(anchor); Flush();
            return new Fixture(host, menu, source, close: menu.Close);
        }
        var flyout = new MenuFlyout { ItemsSource = source, ItemTemplate = template };
        flyout.ShowAt(anchor); Flush();
        var presenter = (MenuFlyoutPresenter)((PopupFlyoutBase)flyout).Popup.Child!;
        return new Fixture(host, presenter, source, close: flyout.Hide);
    }

    private static ObservableCollection<object> Source(bool models, Func<string, Control> own) =>
        new(Enumerable.Range(0, models ? 1000 : 3).Select(index => models ? (object)new Row($"项目 {index:0000}", []) : own($"项目 {index:0000}")));

    private static Window Open(Control content)
    {
        var window = new Window
        {
            Width = 720, Height = 420, RequestedThemeVariant = ThemeVariant.Light, WindowDecorations = WindowDecorations.None,
            Content = new Border { Padding = new Thickness(20), Child = content }
        };
        Motion.SetEnabled(window, false); window.Show(); Flush(); return window;
    }

    private static bool Mounted(ItemsControl owner, Control header) => header.IsAttachedToVisualTree() &&
        ReferenceEquals(header.GetVisualParent(), owner.ItemsPanelRoot) && owner.ItemsPanelRoot is { } panel &&
        panel.Children.Contains(header) && ReferenceEquals(header.Parent, owner);

    private static string MountDetails(ItemsControl owner, IEnumerable<Control?> headers) => " [" + string.Join("; ", headers.Select((header, index) => header is null ? $"{index}:null" :
        $"{index}:{header.GetType().Name} attached={header.IsAttachedToVisualTree()} visible={header.IsVisible} size={header.Bounds.Size} visual={header.GetVisualParent()?.GetType().Name ?? "null"} panel={owner.ItemsPanelRoot?.GetType().Name ?? "null"} inPanel={owner.ItemsPanelRoot?.Children.Contains(header)} logical={header.Parent?.GetType().Name ?? "null"} expected={owner.GetType().Name}")) + "]";

    private static bool VisibleTitle(Control header, string title) => header.GetVisualDescendants().OfType<TextBlock>()
        .Any(text => text.Text == title && text.IsAttachedToVisualTree() && text.IsEffectivelyVisible && text.Bounds.Width > 0 && text.Bounds.Height > 0);

    private static string Title(object item) => item switch { Row row => row.Title, MenuItem control => control.Header?.ToString() ?? "", HeaderedItemsControl control => control.Header?.ToString() ?? "", _ => item.ToString() ?? "" };
    private static string CustomTip(NavMenuItem item) => "自定义提示：" + item.Header;

    private static void Save(ItemsControl owner, string? output, string label, string phase)
    {
        if (output is null || owner.ContainerFromIndex(0) is not { } first || TopLevel.GetTopLevel(first) is not { } top) return;
        using var frame = top.CaptureRenderedFrame();
        frame?.Save(Path.Combine(output, label.Replace('/', '-') + "-" + phase + ".png"), PngBitmapEncoderOptions.Default);
    }

    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
    private sealed record Row(string Title, ObservableCollection<object> Children);
    private sealed class Fixture(Window window, ItemsControl owner, ObservableCollection<object> source, TreeView? tree = null, Action? close = null) : IDisposable
    {
        public ItemsControl Owner { get; } = owner;
        public ObservableCollection<object> Source { get; } = source;
        public TreeView? Tree { get; } = tree;
        public void Dispose() { close?.Invoke(); window.Close(); }
    }
}
