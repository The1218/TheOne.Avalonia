using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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

internal static class VirtualizedHierarchyMutationPreview
{
    public static void Verify(Action<bool, string> check, string? output = null)
    {
        if (output is not null) Directory.CreateDirectory(output);
        foreach (var kind in new[]
                 {
                     "TreeView", "TreeViewItem", "NavMenu", "NavMenuItem", "Menu", "MenuItem", "ContextMenu",
                     "MenuFlyoutPresenter"
                 })
        foreach (var models in new[] { false, true })
            VerifyMutations(kind, models, check, output);
        VerifyPreviouslyRealizedOwnRows("TreeView", check, output);
        VerifyPreviouslyRealizedOwnRows("NavMenu", check, output);
    }

    private static void VerifyMutations(string kind, bool models, Action<bool, string> check, string? output)
    {
        var label = $"HierarchyMutation/{kind}/{(models ? "generated-model" : "own-container")}";
        Fixture? fixture = null;
        try
        {
            var original = Rows(kind, models, "A");
            var replacement = Rows(kind, models, "B");
            fixture = OpenFixture(kind, models, original);
            VerifyCurrent(fixture, original, label + "/initial", check);

            Select(fixture, original[0]);
            var resetActions = new List<NotifyCollectionChangedAction>();
            original.CollectionChanged += (_, args) => resetActions.Add(args.Action);
            var oldRows = original.ToArray();
            original.Reset(replacement);
            Flush(fixture.Owner);
            check(resetActions.SequenceEqual(new[] { NotifyCollectionChangedAction.Reset }),
                label + "/Reset: exactly one Reset notification, without a Clear/Add sequence");
            VerifyCurrent(fixture, original, label + "/Reset-A-to-B", check);
            VerifyRemoved(fixture.Owner, oldRows, label + "/Reset-A-to-B", check);
            Save(fixture.Owner, output, label, "reset-B");
            original.Reset(oldRows);
            Flush(fixture.Owner);
            VerifyCurrent(fixture, original, label + "/Reset-B-to-A", check);
            VerifyRemoved(fixture.Owner, replacement, label + "/Reset-B-to-A", check);

            Select(fixture, original[0]);
            var oldFirst = original[0];
            var differentAtSameIndex = Rows(kind, models, "C")[0];
            original[0] = differentAtSameIndex;
            Flush(fixture.Owner);
            VerifyCurrent(fixture, original, label + "/Replace-same-index", check);
            VerifyRemoved(fixture.Owner, new[] { oldFirst }, label + "/Replace-same-index", check);
            original[0] = oldFirst;
            Flush(fixture.Owner);
            VerifyCurrent(fixture, original, label + "/Replace-restore", check);
            VerifyRemoved(fixture.Owner, new[] { differentAtSameIndex }, label + "/Replace-restore", check);

            Select(fixture, original[0]);
            fixture.SetSource(replacement);
            Flush(fixture.Owner);
            VerifyCurrent(fixture, replacement, label + "/ItemsSource-A-to-B", check);
            VerifyRemoved(fixture.Owner, original, label + "/ItemsSource-A-to-B", check);
            fixture.SetSource(original);
            Flush(fixture.Owner);
            VerifyCurrent(fixture, original, label + "/ItemsSource-B-to-A", check);
            VerifyRemoved(fixture.Owner, replacement, label + "/ItemsSource-B-to-A", check);
            Save(fixture.Owner, output, label, "source-A-restored");
            if (kind == "MenuFlyoutPresenter")
            {
                fixture.ClosePopup();
                Flush();
                fixture.SetSource(replacement);
                Flush();
                fixture.Reopen();
                Flush(fixture.Owner);
                VerifyCurrent(fixture, replacement, label + "/closed-source-B-reopen", check);
                VerifyRemoved(fixture.Owner, original, label + "/closed-source-B-reopen", check);
                fixture.SetSource(original);
                Flush(fixture.Owner);
                VerifyCurrent(fixture, original, label + "/reopened-source-A", check);
                VerifyRemoved(fixture.Owner, replacement, label + "/reopened-source-A", check);
                Save(fixture.Owner, output, label, "reopened-source-A");
                VerifyFlyoutDefaultItems(fixture, kind, models, original, replacement, label, check);
            }
        }
        catch (Exception error)
        {
            check(false, label + ": unexpected " + error.GetType().Name + ": " + error.Message);
        }
        finally
        {
            fixture?.Dispose();
            Flush();
        }
    }

    private static void VerifyFlyoutDefaultItems(Fixture fixture, string kind, bool models, ResetRows original,
        ResetRows replacement,
        string label, Action<bool, string> check)
    {
        var flyout = fixture.Flyout!;
        var presenter = fixture.Owner;
        flyout.ItemsSource = null;
        Flush(presenter);
        check(flyout.ItemsSource is null && flyout.Items.Count == 0 && presenter.Items.Count == 0,
            label + "/default-Items: setting ItemsSource to null exposes the empty default Items collection");
        VerifyRemoved(presenter, original, label + "/default-Items", check);

        var direct = Rows(kind, models, "D");
        foreach (var item in direct) flyout.Items.Add(item);
        Flush(presenter);
        VerifyFlyoutRows(presenter, direct, label + "/default-Items-add", check);
        Select(fixture, direct[0]);

        var appended = Rows(kind, models, "D", 4)[3];
        direct.Add(appended);
        flyout.Items.Add(appended);
        Flush(presenter);
        VerifyFlyoutRows(presenter, direct, label + "/default-Items-dynamic-add", check);

        var removed = direct[0];
        direct.RemoveAt(0);
        flyout.Items.RemoveAt(0);
        Flush(presenter);
        VerifyFlyoutRows(presenter, direct, label + "/default-Items-dynamic-remove-selected", check);
        VerifyRemoved(presenter, new[] { removed }, label + "/default-Items-dynamic-remove-selected", check);

        // Native ItemCollection requires default Items to be empty before assigning an ItemsSource.
        flyout.Items.Clear();
        Flush(presenter);
        check(presenter.Items.Count == 0 && ((SelectingItemsControl)presenter).SelectedItem is null,
            label +
            "/default-Items-clear: clearing default Items empties the presenter and its selection before switching source");

        // No render/dispatcher pump between these assignments: the pending update must use the latest source.
        flyout.ItemsSource = original;
        flyout.ItemsSource = replacement;
        flyout.ItemsSource = original;
        Flush(presenter);
        VerifyFlyoutRows(presenter, original, label + "/rapid-source-A-B-A", check);
        VerifyRemoved(presenter, direct, label + "/rapid-source-A-B-A", check);
    }

    private static void VerifyFlyoutRows(ItemsControl presenter, IReadOnlyList<object> expected, string label,
        Action<bool, string> check)
    {
        check(
            presenter.Items.Count == expected.Count && Enumerable.Range(0, expected.Count)
                .All(index => ReferenceEquals(presenter.Items[index], expected[index])),
            label + ": the live presenter collection contains each current item identity");
        var headers = Enumerable.Range(0, expected.Count).Select(presenter.ContainerFromIndex).ToArray();
        check(headers.Select((header, index) => header is not null && Mounted(presenter, header) &&
                                                ReferenceEquals(presenter.ItemFromContainer(header), expected[index]) &&
                                                VisibleTitle(header, Title(expected[index])) &&
                                                (expected[index] is not Control own || ReferenceEquals(header, own)))
                .All(value => value),
            label +
            ": every current item, including the appended row, has a visible mounted header with the correct identity");
        var selected = ((SelectingItemsControl)presenter).SelectedItem;
        check((selected is null || expected.Any(item => ReferenceEquals(item, selected))) && headers.OfType<MenuItem>()
                .All(item =>
                    item.IsSelected == ReferenceEquals(presenter.ItemFromContainer(item), selected)),
            label + ": selection and selected visuals refer only to current data after the live change");
    }

    private static void VerifyCurrent(Fixture fixture, IReadOnlyList<object> expected, string label,
        Action<bool, string> check)
    {
        var owner = fixture.Owner;
        var headers = Enumerable.Range(0, Math.Min(3, expected.Count)).Select(owner.ContainerFromIndex).ToArray();
        // Realize an actual second level before each mutation; checking collapsed old children alone misses stale subtrees.
        if (headers.FirstOrDefault() is TreeViewItem first && Children(expected[0]).FirstOrDefault() is { } firstChild)
        {
            first.IsExpanded = true;
            Flush(owner);
            var child = first.ContainerFromIndex(0);
            check(
                child is not null && Mounted(first, child) &&
                ReferenceEquals(first.ItemFromContainer(child), firstChild) && VisibleTitle(child, Title(firstChild)),
                label + ": the expanded child belongs to the current parent data and is actually mounted");
        }

        check(
            owner.Items.Count == expected.Count && Enumerable.Range(0, expected.Count)
                .All(index => ReferenceEquals(owner.Items[index], expected[index])),
            label + ": current collection contains the expected instances rather than old rows at the same index");
        var mounted = headers.All(header => header is not null && Mounted(owner, header));
        check(mounted,
            label + ": visible containers have actual visual panel membership and the correct logical parent" +
            (mounted ? "" : Details(owner, headers)));
        check(headers.Select((header, index) =>
                header is not null && ReferenceEquals(owner.ItemFromContainer(header), expected[index]) &&
                (expected[index] is not Control own || ReferenceEquals(header, own)) &&
                VisibleTitle(header, Title(expected[index]))).All(value => value),
            label + ": visible header identity and text match the new data, including own-container identity");
        var selected = fixture.Tree is { } tree ? tree.SelectedItem : (owner as SelectingItemsControl)?.SelectedItem;
        // Native controls may clear selection or select another current row. Neither may retain old data by index.
        var currentSelection = selected is null || expected.Any(item => ReferenceEquals(item, selected)) ||
                               (fixture.Tree is { } nestedTree && !ReferenceEquals(owner, nestedTree) &&
                                nestedTree.Items.Any(item => ReferenceEquals(item, selected)));
        check(currentSelection,
            label + ": selection is empty or references a current data instance" +
            (currentSelection ? "" : $" [selected={Title(selected!)}]"));
        if (fixture.Tree is { } selectedTree)
            check(selectedTree.SelectedItems is null || selectedTree.SelectedItems.Cast<object>().All(item =>
                    expected.Any(current => ReferenceEquals(current, item)) || (!ReferenceEquals(owner, selectedTree) &&
                        selectedTree.Items.Any(parent => ReferenceEquals(parent, item)))),
                label + ": the tree selection collection contains no removed child or old same-index data");
        check(headers.All(header => header switch
        {
            TreeViewItem item when item.IsSelected => ReferenceEquals(fixture.Tree?.SelectedItem,
                owner.ItemFromContainer(item)),
            MenuItem item when item.IsSelected => owner is SelectingItemsControl selection &&
                                                  ReferenceEquals(selection.SelectedItem,
                                                      owner.ItemFromContainer(item)),
            _ => true
        }), label + ": selected visual state belongs to the selected data identity");
        if (owner is NavMenu or NavMenuItem)
            check(headers.Select((header, index) => expected[index] is NavMenuItem own
                    ? ToolTip.GetTip(own) as string == Tip(own)
                    : header is not null && ToolTip.GetTip(header) is ContentControl tip &&
                      ReferenceEquals(tip.Content, expected[index]) && tip.Content is not Control).All(value => value),
                label + ": own tooltips survive and generated tooltips refer to the current model");
    }

    private static void VerifyRemoved(ItemsControl owner, IEnumerable<object> removed, string label,
        Action<bool, string> check)
    {
        var old = removed.Where(item => !owner.Items.Any(current => ReferenceEquals(current, item))).ToArray();
        var own = old.OfType<Control>().ToArray();
        var retained = own.Where(item => item.GetVisualParent() is not null || item.Parent is not null ||
                                         item.IsAttachedToVisualTree() ||
                                         owner.ItemsPanelRoot?.Children.Contains(item) == true).ToArray();
        check(retained.Length == 0,
            label +
            ": removed own containers, including their old child subtrees, leave both visual and logical owners" +
            (retained.Length == 0 ? "" : $" [retained={retained.Length}]" + Details(owner, retained.Take(6))));
        var oldTitles = old.SelectMany(Subtree).Select(Title).ToHashSet(StringComparer.Ordinal);
        // Generated wrappers may remain in a hidden recycle pool. Only attached and effectively visible stale text is a leak.
        check(owner.ItemsPanelRoot?.GetVisualDescendants().OfType<TextBlock>().Any(text =>
                text.IsAttachedToVisualTree() && text.IsEffectivelyVisible &&
                text.Bounds.Width > 0 && text.Bounds.Height > 0 && text.Text is { } title &&
                oldTitles.Contains(title)) != true,
            label + ": no old row title remains visibly mounted in the current item panel");
    }

    private static void VerifyPreviouslyRealizedOwnRows(string kind, Action<bool, string> check, string? output)
    {
        var label = $"HierarchyMutation/{kind}/previously-realized-own";
        Fixture? fixture = null;
        try
        {
            var rows = Rows(kind, false, "old", 100);
            fixture = OpenFixture(kind, false, rows);
            var owner = fixture.Owner;
            owner.ScrollIntoView(80);
            Flush(owner);
            var far = owner.ContainerFromIndex(80);
            check(far is not null && Mounted(owner, far),
                label + ": a distant own row is genuinely realized before recycling");
            owner.ScrollIntoView(0);
            Flush(owner);
            check(
                far is not null && (!far.IsEffectivelyVisible || far.Bounds.Bottom <= 0 ||
                                    far.Bounds.Y >= owner.Bounds.Height),
                label + ": the previously realized row is outside the visible viewport before Reset");
            var old = rows.ToArray();
            rows.Reset(Rows(kind, false, "new"));
            Flush(owner);
            VerifyCurrent(fixture, rows, label + "/Reset", check);
            VerifyRemoved(owner, old, label + "/Reset", check);
            check(
                owner.ItemsPanelRoot?.Children.OfType<TreeViewItem>()
                    .All(item => owner.Items.Any(current => ReferenceEquals(current, item))) == true,
                label + "/Reset: the actual panel has no hidden own rows absent from the current collection");
            Save(owner, output, label, "after-Reset");
        }
        catch (Exception error)
        {
            check(false, label + ": unexpected " + error.GetType().Name + ": " + error.Message);
        }
        finally
        {
            fixture?.Dispose();
            Flush();
        }
    }

    private static Fixture OpenFixture(string kind, bool models, ResetRows source)
    {
        return kind is "TreeView" or "TreeViewItem" or "NavMenu" or "NavMenuItem"
            ? OpenTree(kind, models, source)
            : OpenMenu(kind, models, source);
    }

    private static Fixture OpenTree(string kind, bool models, ResetRows source)
    {
        var navigation = kind.StartsWith("Nav", StringComparison.Ordinal);
        var nested = kind is "TreeViewItem" or "NavMenuItem";
        var tree = navigation ? new NavMenu() : new TreeView();
        tree.Height = 240;
        if (models)
        {
            if (tree is NavMenu menu)
            {
                menu.ChildrenBinding = CompiledBinding.Create<Row, ResetRows>(row => row.Children);
                menu.ContentTemplate = new FuncDataTemplate<Row>((row, _) => new TextBlock { Text = row?.Title ?? "" });
            }
            else
            {
                tree.ItemTemplate = new FuncTreeDataTemplate<Row>((row, _) => new TextBlock { Text = row?.Title ?? "" },
                    row => row?.Children ?? []);
            }
        }

        TreeViewItem? parent = null;
        if (nested)
        {
            if (models)
            {
                tree.ItemsSource = new[] { new Row("parent", source) };
            }
            else
            {
                parent = navigation
                    ? new NavMenuItem { Header = "parent", ItemsSource = source, IsExpanded = true }
                    : new TreeViewItem { Header = "parent", ItemsSource = source, IsExpanded = true };
                tree.Items.Add(parent);
            }
        }
        else
        {
            tree.ItemsSource = source;
        }

        var window = Open(tree);
        if (!nested) return new Fixture(window, tree, rows => tree.ItemsSource = rows, tree);
        parent ??= (TreeViewItem)tree.ContainerFromIndex(0)!;
        parent.IsExpanded = true;
        Flush(tree);
        var childOwner = parent;
        return new Fixture(window, childOwner, rows => childOwner.ItemsSource = rows, tree);
    }

    private static Fixture OpenMenu(string kind, bool models, ResetRows source)
    {
        var template = models ? new FuncDataTemplate<Row>((row, _) => new TextBlock { Text = row?.Title ?? "" }) : null;
        if (kind == "Menu")
        {
            var menu = new Menu
                { ItemsSource = source, ItemTemplate = template, VerticalAlignment = VerticalAlignment.Top };
            return new Fixture(Open(menu), menu, rows => menu.ItemsSource = rows);
        }

        if (kind == "MenuItem")
        {
            var owner = new MenuItem { Header = "命令", ItemsSource = source, ItemTemplate = template };
            var bar = new Menu();
            bar.Items.Add(owner);
            var window = Open(new StackPanel { Children = { bar } });
            owner.IsSubMenuOpen = true;
            Flush();
            return new Fixture(window, owner, rows => owner.ItemsSource = rows,
                close: () => owner.IsSubMenuOpen = false);
        }

        var anchor = new Button { Content = "打开菜单", HorizontalAlignment = HorizontalAlignment.Left };
        var host = Open(new StackPanel { Children = { anchor } });
        if (kind == "ContextMenu")
        {
            var menu = new ContextMenu { ItemsSource = source, ItemTemplate = template, MaxHeight = 240 };
            anchor.ContextMenu = menu;
            menu.Open(anchor);
            Flush();
            return new Fixture(host, menu, rows => menu.ItemsSource = rows, close: menu.Close);
        }

        var flyout = new MenuFlyout { ItemsSource = source, ItemTemplate = template };
        flyout.ShowAt(anchor);
        Flush();
        var presenter = (MenuFlyoutPresenter)flyout.Popup.Child!;
        return new Fixture(host, presenter, rows => flyout.ItemsSource = rows, close: flyout.Hide,
            reopen: () => flyout.ShowAt(anchor), flyout: flyout);
    }

    private static ResetRows Rows(string kind, bool models, string prefix, int count = 3)
    {
        var navigation = kind.StartsWith("Nav", StringComparison.Ordinal);
        var tree = kind is "TreeView" or "TreeViewItem" || navigation;
        return new ResetRows(Enumerable.Range(0, count).Select(index =>
        {
            var title = $"{prefix}-{index:000}";
            if (models) return (object)new Row(title, new ResetRows(new object[] { new Row(title + "/child", []) }));
            if (navigation)
            {
                var item = new NavMenuItem { Header = title };
                item.Items.Add(new NavMenuItem { Header = title + "/child" });
                ToolTip.SetTip(item, Tip(item));
                return item;
            }

            if (tree)
            {
                var item = new TreeViewItem { Header = title };
                item.Items.Add(new TreeViewItem { Header = title + "/child" });
                return item;
            }

            return new MenuItem { Header = title };
        }));
    }

    private static void Select(Fixture fixture, object row)
    {
        if (fixture.Tree is { } tree) tree.SelectedItem = row;
        else if (fixture.Owner is SelectingItemsControl selecting) selecting.SelectedItem = row;
        Flush(fixture.Owner);
    }

    private static Window Open(Control content)
    {
        var window = new Window
        {
            Width = 720, Height = 420, RequestedThemeVariant = ThemeVariant.Light,
            WindowDecorations = WindowDecorations.None,
            Content = new Border { Padding = new Thickness(20), Child = content }
        };
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        return window;
    }

    private static bool Mounted(ItemsControl owner, Control header)
    {
        return header.IsAttachedToVisualTree() &&
               ReferenceEquals(header.GetVisualParent(), owner.ItemsPanelRoot) && owner.ItemsPanelRoot is { } panel &&
               panel.Children.Contains(header) && ReferenceEquals(header.Parent, owner);
    }

    private static bool VisibleTitle(Control header, string title)
    {
        return header.GetVisualDescendants().OfType<TextBlock>()
            .Any(text => text.Text == title && text.IsAttachedToVisualTree() && text.IsEffectivelyVisible &&
                         text.Bounds.Width > 0 && text.Bounds.Height > 0);
    }

    private static string Details(ItemsControl owner, IEnumerable<Control?> headers)
    {
        return " [" + string.Join("; ",
                   headers.Select(header =>
                       header is null
                           ? "null"
                           : $"{Title(header)} attached={header.IsAttachedToVisualTree()} visual={header.GetVisualParent()?.GetType().Name ?? "null"} logical={header.Parent?.GetType().Name ?? "null"} inPanel={owner.ItemsPanelRoot?.Children.Contains(header)}")) +
               "]";
    }

    private static string Title(object item)
    {
        return item switch
        {
            Row row => row.Title, MenuItem control => control.Header?.ToString() ?? "",
            HeaderedItemsControl control => control.Header?.ToString() ?? "", _ => item.ToString() ?? ""
        };
    }

    private static string Tip(NavMenuItem item)
    {
        return "自定义提示：" + item.Header;
    }

    private static IEnumerable<object> Children(object item)
    {
        return item switch
        {
            Row row => row.Children, TreeViewItem tree => tree.Items.OfType<object>(),
            MenuItem menu => menu.Items.OfType<object>(), _ => []
        };
    }

    private static IEnumerable<object> Subtree(object item)
    {
        yield return item;
        foreach (var child in Children(item))
        foreach (var descendant in Subtree(child))
            yield return descendant;
    }

    private static void Save(ItemsControl owner, string? output, string label, string phase)
    {
        if (output is null || owner.ContainerFromIndex(0) is not { } first ||
            TopLevel.GetTopLevel(first) is not { } top) return;
        using var frame = top.CaptureRenderedFrame();
        frame?.Save(Path.Combine(output, label.Replace('/', '-') + "-" + phase + ".png"),
            PngBitmapEncoderOptions.Default);
    }

    private static void Flush(ItemsControl? owner = null)
    {
        owner?.UpdateLayout();
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private sealed record Row(string Title, ResetRows Children);

    private sealed class ResetRows(IEnumerable<object>? rows = null) : ObservableCollection<object>(rows ?? [])
    {
        public void Reset(IEnumerable<object> rows)
        {
            var next = rows.ToArray();
            Items.Clear();
            foreach (var row in next) Items.Add(row);
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }

    private sealed class Fixture(
        Window window,
        ItemsControl owner,
        Action<ResetRows> setSource,
        TreeView? tree = null,
        Action? close = null,
        Action? reopen = null,
        MenuFlyout? flyout = null) : IDisposable
    {
        public ItemsControl Owner { get; } = owner;
        public TreeView? Tree { get; } = tree;
        public MenuFlyout? Flyout { get; } = flyout;

        public void Dispose()
        {
            close?.Invoke();
            window.Close();
        }

        public void SetSource(ResetRows rows)
        {
            setSource(rows);
        }

        public void ClosePopup()
        {
            close?.Invoke();
        }

        public void Reopen()
        {
            reopen?.Invoke();
        }
    }
}