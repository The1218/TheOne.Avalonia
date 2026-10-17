using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class DataGridTreeQueryPreview
{
    public static void Verify(Action<bool, string> check, string? output = null)
    {
        VerifyQuery(check);
        VerifyFlatScope(check);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
            VerifyGrid(theme, size, check, output);
    }

    private static void VerifyQuery(Action<bool, string> check)
    {
        DataGridAot.PreserveRowType<TreePrototypeNode>();
        DataGridAot.PreserveRowType<TreePrototypeDetails>();
        var a = new TreePrototypeNode("B root") { Quantity = 100 };
        var b = new TreePrototypeNode("A root") { Quantity = 200 };
        var branch = new TreePrototypeNode("B branch") { Quantity = 50 };
        var x = new TreePrototypeNode("hit-b") { Quantity = 3 };
        var y = new TreePrototypeNode("hit-a") { Quantity = 2 };
        var sibling = new TreePrototypeNode("A leaf") { Quantity = 8 };
        var z = new TreePrototypeNode("hidden") { Quantity = 7 };
        var w = new TreePrototypeNode("hit-c") { Quantity = 5 };
        branch.Children.Add(x);
        branch.Children.Add(y);
        a.Children.Add(branch);
        a.Children.Add(sibling);
        b.Children.Add(z);
        b.Children.Add(w);
        ObservableCollection<TreePrototypeNode> roots = [a, b];
        using var view = new DataGridTreeCollectionView<TreePrototypeNode>(roots, node => node.Children);

        bool Order(params TreePrototypeNode[] expected)
        {
            return view.SequenceEqual(expected, ReferenceEqualityComparer.Instance);
        }

        var badPath = false;
        try
        {
            view.SortDescriptions.Add(DataGridSortDescription.FromPath("UnknownField"));
        }
        catch (ArgumentException)
        {
            badPath = true;
        }

        check(badPath && view.SortDescriptions.Count == 0,
            "Tree/query: unknown registered sort paths are rejected before changing the query");
        view.TrySetExpanded(a, true);
        view.TrySetExpanded(branch, true);
        view.TrySetExpanded(b, true);
        view.SortDescriptions.Add(DataGridSortDescription.FromPath(nameof(TreePrototypeNode.Name)));
        check(Order(b, z, w, a, sibling, branch, y, x),
            "Tree/query: stable ascending sorts apply separately to every sibling list");
        using (view.DeferRefresh())
        {
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(DataGridSortDescription.FromPath(nameof(TreePrototypeNode.Quantity),
                ListSortDirection.Descending));
        }

        check(Order(b, z, w, a, branch, x, y, sibling),
            "Tree/query: descending sort keeps each parent before its entire subtree");
        y.Quantity = 10;
        Flush();
        check(Order(b, z, w, a, branch, y, x, sibling),
            "Tree/query: edits in sorted children update sibling order without a global sort");
        y.Quantity = 2;
        Flush();
        x.Details.Score = 2;
        y.Details.Score = 1;
        using (view.DeferRefresh())
        {
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(DataGridSortDescription.FromPath("Details.Score"));
        }

        check(view.IndexOf(y) < view.IndexOf(x), "Tree/query: registered nested sort paths use native comparers");
        x.Details.Score = 0;
        Flush();
        check(view.IndexOf(x) < view.IndexOf(y), "Tree/query: nested sort notifications are observed");
        var oldDetails = x.Details;
        x.Details = new TreePrototypeDetails { Score = 5 };
        Flush();
        oldDetails.Score = -1;
        Flush();
        check(view.IndexOf(y) < view.IndexOf(x), "Tree/query: replacing nested values reconnects live sorting");
        view.SortDescriptions.Clear();
        check(Order(a, branch, x, y, sibling, b, z, w),
            "Tree/query: clearing sorts restores original source order at every level");
        view.TrySetExpanded(branch, false);
        view.TrySetExpanded(b, false);
        var changes = 0;
        view.TreeChanged += (_, _) => changes++;
        using (view.DeferRefresh())
        {
            view.SortDescriptions.Add(DataGridSortDescription.FromPath(nameof(TreePrototypeNode.Quantity)));
            view.SortDescriptions.Add(DataGridSortDescription.FromPath(nameof(TreePrototypeNode.Name)));
        }

        check(changes == 1, "Tree/query: a native multi-sort deferral publishes only one tree projection");
        view.SortDescriptions.Clear();
        Func<object, bool> hits = item => ((TreePrototypeNode)item).Name.StartsWith("hit", StringComparison.Ordinal);
        view.Filter = hits;
        check(Order(a, branch, x, y, b, w) && view.LoadedItems.Cast<object>().Count() == 8,
            "Tree/query: filtering searches collapsed loaded nodes and retains only matches plus ancestor paths");
        check(!view.IsFilterMatch(a) && view.IsFilterMatch(x) && view.IsLeaf(x) && !view.IsLeaf(branch),
            "Tree/query: direct matches and structural leaves are distinct from context ancestors");
        check(
            view.TryGetNodeState(branch, out var state) && state.IsForcedExpanded &&
            !view.TrySetExpanded(branch, false) && view.TrySetExpanded(branch, true),
            "Tree/query: matching paths remain temporarily expanded without overwriting user state");
        view.Filter = item => ReferenceEquals(item, a);
        check(Order(a) && view.TryGetNodeState(a, out state) && !state.HasChildren,
            "Tree/query: a matching parent does not implicitly match its children");
        view.Filter = null!;
        check(Order(a, branch, sibling, b) && view.TryGetNodeState(branch, out state) && !state.IsExpanded,
            "Tree/query: clearing filters restores the user's expansion state");
        var before = view.Cast<object>().ToArray();
        var failed = false;
        try
        {
            view.Filter = _ => throw new InvalidOperationException("Invalid predicate");
        }
        catch (InvalidOperationException)
        {
            failed = true;
        }

        check(failed && view.Filter is null && view.SequenceEqual(before, ReferenceEqualityComparer.Instance),
            "Tree/query: predicate errors preserve the old filter and projection atomically");
        view.SortDescriptions.Add(DataGridSortDescription.FromComparer(new BrokenComparer()));
        check(
            view.SortDescriptions.Count == 0 && view.QueryError is not null &&
            view.SequenceEqual(before, ReferenceEqualityComparer.Instance),
            "Tree/query: comparer errors roll back sort descriptions and expose QueryError");
        view.RefreshTree();
        check(view.QueryError is null, "Tree/query: a successful refresh clears the query error");
        EventHandler<DataGridTreeChangingEventArgs> cancel = (_, args) => args.Cancel = true;
        view.TreeChanging += cancel;
        using (view.DeferRefresh())
        {
            view.Filter = hits;
            view.SortDescriptions.Add(DataGridSortDescription.FromPath(nameof(TreePrototypeNode.Name)));
        }

        view.TreeChanging -= cancel;
        check(
            view.Filter is null && view.SortDescriptions.Count == 0 &&
            view.SequenceEqual(before, ReferenceEqualityComparer.Instance),
            "Tree/query: cancelling a deferred query restores both predicate and sort state");
        using (view.DeferRefresh())
        {
            a.Children.Add(new TreePrototypeNode("new source"));
            view.Filter = hits;
        }

        check(view.LoadedItems.Cast<object>().Count() == 9 && view.Count == 6,
            "Tree/query: source changes and queries inside the same deferral publish a consistent snapshot");
        x.Name = "excluded";
        Flush();
        check(!view.IsFilterMatch(x) && view.Count == 5,
            "Tree/query: a hidden model notification refreshes ancestor-preserving filtering");
    }

    private static void VerifyFlatScope(Action<bool, string> check)
    {
        var source = new ObservableCollection<TreePrototypeNode>
            { new("a") { Quantity = 1 }, new("b") { Quantity = 3 } };
        var view = new DataGridCollectionView<TreePrototypeNode>(source)
            { Filter = item => ((TreePrototypeNode)item).Quantity > 1 };
        var grid = new DataGrid { ItemsSource = view, AutoGenerateColumns = false };
        var column = new DataGridTextColumn
            { Header = "数量", Binding = CompiledBinding.Create<TreePrototypeNode, int>(node => node.Quantity) };
        grid.Columns.Add(column);
        DataGridColumnOptions.SetSummaryEnabled(column, true);
        DataGridFeatures.SetShowSummaryRow(grid, true);
        DataGridFeatures.SetSummaryScope(grid, DataGridSummaryScope.VisibleItems);
        var window = new Window { Width = 400, Height = 250, Content = grid };
        window.Show();
        Flush();
        check(Equals(DataGridFeatures.GetSummaryResult(grid, column)?.Value, 3m),
            "Tree/query shared summary: flat VisibleItems follows the native filtered view");
        DataGridFeatures.SetSummaryScope(grid, DataGridSummaryScope.SourceItems);
        DataGridFeatures.SetSummaryLeavesOnly(grid, true);
        Flush();
        check(Equals(DataGridFeatures.GetSummaryResult(grid, column)?.Value, 4m),
            "Tree/query shared summary: flat source summaries still include all ordinary rows");
        view.SortDescriptions.Add(DataGridSortDescription.FromPath(nameof(TreePrototypeNode.Quantity),
            ListSortDirection.Descending));
        Flush();
        check(view.Count == 1 && ReferenceEquals(view[0], source[1]),
            "Tree/query shared view: ordinary native filtering and sorting are unchanged");
        window.Close();
        Flush();
    }

    private static void VerifyGrid(ThemeVariant theme, ControlSize size, Action<bool, string> check, string? output)
    {
        var fixture = new DataGridTreeFixture();
        using var model = fixture.Model;
        model.Root.Quantity = 100;
        model.Branch.Quantity = 50;
        model.Leaf.Quantity = 3;
        model.Branch.Children[1].Quantity = 2;
        model.Root.Children[1].Quantity = 8;
        model.Other.Quantity = 200;
        var grid = fixture.Grid;
        var view = model.View;
        var quantity = grid.Columns[1];
        var window = new Window
        {
            Width = 800, Height = 500, RequestedThemeVariant = theme, WindowDecorations = WindowDecorations.None,
            Content = new Border { Padding = new Thickness(16), Child = fixture }
        };
        ControlSizing.SetSize(window, size);
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        var label = $"Tree/query/{theme.Key}/{size}";

        decimal? Sum(DataGridColumn? column = null)
        {
            return DataGridFeatures.GetSummaryResult(grid, column ?? quantity)?.Value is { } value
                ? Convert.ToDecimal(value)
                : null;
        }

        check(Sum() == 500863m && view.Count == 2,
            label + ": default filtered summary covers all loaded nodes despite collapse");
        var rootRow = grid.GetVisualDescendants().OfType<DataGridRow>()
            .Single(row => ReferenceEquals(row.DataContext, model.Root));
        var rootPeer = ControlAutomationPeer.CreatePeerForElement(grid.Columns[0].GetCellContent(rootRow))!;
        var expansion = rootPeer.GetProvider<IExpandCollapseProvider>();
        check(
            expansion is not null && expansion.ExpandCollapseState == ExpandCollapseState.Collapsed &&
            rootPeer.GetName() == model.Root.Name && rootPeer.GetHelpText() == "Level 1",
            label + ": tree cell automation reports name, hierarchy depth and expansion state");
        expansion!.Expand();
        Flush();
        check(expansion.ExpandCollapseState == ExpandCollapseState.Expanded && view.Count == 4,
            label + ": automation expands through the native tree transaction");
        expansion.Collapse();
        Flush();
        check(view.Count == 2, label + ": automation collapse preserves the original native rows");
        grid.SelectedItem = model.Root;
        var sorts = SortButton(grid, quantity);
        Click(sorts);
        Click(sorts);
        check(
            quantity.SortDirection == ListSortDirection.Descending && ReferenceEquals(view[0], model.Other) &&
            ReferenceEquals(grid.SelectedItem, model.Root),
            label + ": actual header sort preserves original selection and sibling ordering");
        var nameSort = SortButton(grid, grid.Columns[0]);
        nameSort.Focus();
        Press(TopLevel.GetTopLevel(nameSort)!, Key.Enter, PhysicalKey.Enter, RawInputModifiers.Shift);
        check(view.SortDescriptions.Count == 2 && quantity.SortPriority == 1 && grid.Columns[0].SortPriority == 2,
            label + ": keyboard Shift sorting retains native priority and multi-sort state");
        view.SortDescriptions.Clear();
        Flush();
        var filterButton = grid.GetVisualDescendants().OfType<DataGridColumnHeader>()
            .Single(header => Equals(header.Content, "名称"))
            .GetVisualDescendants().OfType<DataGridFilterButton>().Single();
        Click(filterButton);
        var popup = (Control)((Flyout)filterButton.Flyout!).Content!;
        var input = popup.GetVisualDescendants().OfType<TextBox>().Single(box => box.Name == "DataGridFilterText");
        input.Focus();
        TopLevel.GetTopLevel(input)!.KeyTextInput("设备 1000");
        Press(TopLevel.GetTopLevel(input)!, Key.Enter, PhysicalKey.Enter);
        check(
            view.Count == 2 && ReferenceEquals(view[0], model.Other) &&
            ReferenceEquals(view[1], model.Other.Children[^1]) && !filterButton.Flyout.IsOpen,
            label + ": actual filter popup searches collapsed descendants and retains the unmatched ancestor");
        check(Sum() == 1000m, label + ": filtered summary excludes ancestors retained only for context");
        model.SummaryScope = DataGridSummaryScope.VisibleItems;
        Flush();
        check(Sum() == 1200m, label + ": visible summary includes the expanded ancestor path");
        model.SummaryLeavesOnly = true;
        Flush();
        check(Sum() == 1000m, label + ": leaf-only summary excludes structural parents");
        model.SummaryScope = DataGridSummaryScope.SourceItems;
        Flush();
        check(Sum() == 500513m, label + ": source leaf summary ignores filtering and collapse");
        model.SummaryLeavesOnly = false;
        Flush();
        check(Sum() == 500863m, label + ": source summary includes each loaded node once");
        view.SortDescriptions.Add(DataGridSortDescription.FromComparer(new BrokenComparer()));
        Flush();
        check(
            view.QueryError is not null && view.SortDescriptions.Count == 0 &&
            DataGridColumnOptions.GetFilterCondition(grid.Columns[0]) is not null,
            label + ": failed sorting retains the active column predicate without treating it as a business filter");
        check(DataGridFeatures.ClearAllFilters(grid).Succeeded,
            label + ": column filters clear through the existing public API");
        Flush();
        check(view.Count == 2 && ReferenceEquals(view[0], model.Root) && ReferenceEquals(view[1], model.Other),
            label + ": clearing column filters restores collapsed user state");
        var business = new Func<object, bool>(item => ((TreePrototypeNode)item).Quantity >= 500);
        view.Filter = business;
        Flush();
        check(DataGridFeatures.ApplyFilter(grid, grid.Columns[0], new TextFilterCondition("设备 1000")).Succeeded,
            label + ": business and column filters compose in the tree view");
        Flush();
        check(DataGridFeatures.ClearAllFilters(grid).Succeeded, label + ": composed column filter can clear");
        Flush();
        check(
            ReferenceEquals(view.Filter, business) && view.IsFilterMatch(model.Other.Children[^1]) &&
            !view.IsFilterMatch(model.Other),
            label + ": clearing columns retains the original business predicate");
        view.Filter = null!;
        Flush();
        DataGridColumnOptions.SetValueAccessor(quantity, new HiddenValueAccessor(model.Other.Children[^1]));
        Flush();
        var failed = DataGridFeatures.ApplyFilter(grid, quantity,
            new NumericFilterCondition<int>(NumericFilterMode.Exact, 1000));
        check(!failed.Succeeded && DataGridColumnOptions.GetFilterCondition(quantity) is null && view.Count == 2,
            label + ": failing getters in collapsed descendants reject column filters before mutation");
        DataGridColumnOptions.SetValueAccessor(quantity, null);
        Flush();

        var score = new DataGridTextColumn
        {
            Header = "评分", Width = new DataGridLength(110),
            Binding = CompiledBinding.Create<TreePrototypeNode, int>(node => node.Details.Score,
                mode: BindingMode.TwoWay)
        };
        DataGridColumnOptions.SetSummaryEnabled(score, true);
        grid.Columns.Add(score);
        Flush();
        model.Other.Children[^1].Details.Score = 9;
        Flush();
        check(Sum(score) == 9m, label + ": nested compiled getters observe collapsed summary models");
        check(
            DataGridFeatures.ApplyFilter(grid, score, new NumericFilterCondition<int>(NumericFilterMode.Exact, 9))
                .Succeeded,
            label + ": nested compiled fields support tree column filtering");
        Flush();
        var extra = new TreePrototypeNode("新增命中");
        extra.Details.Score = 9;
        model.Other.Children.Add(extra);
        Flush();
        check(view.Count == 3 && Sum(score) == 18m, label + ": new descendants join active tree filters and summaries");
        model.Other.Children[^2].Details = new TreePrototypeDetails();
        Flush();
        check(view.Count == 2 && ReferenceEquals(view[1], extra) && Sum(score) == 9m,
            label + ": nested replacement updates matching nodes without retaining old observers");
        DataGridFeatures.ClearAllFilters(grid);
        Flush();
        view.TrySetExpanded(model.Root, true);
        view.TrySetExpanded(model.Branch, true);
        Flush();
        grid.SelectedItem = model.Leaf;
        grid.CurrentColumn = grid.Columns[0];
        grid.Focus();
        Flush();
        check(grid.BeginEdit(), label + ": query-integrated tree still enters native editing");
        Flush();
        var editor = grid.GetVisualDescendants().OfType<TextBox>().Single();
        editor.Text = "";
        Flush();
        var rejected = DataGridFeatures.ApplyFilter(grid, grid.Columns[0], new TextFilterCondition("新增"));
        check(!rejected.Succeeded && view.IsEditingItem && view.Count == 6,
            label + ": validation failure blocks filters without hiding the editor");
        editor.Text = "重命名组件";
        Flush();
        grid.CommitEdit(DataGridEditingUnit.Row, true);
        Flush();
        check(model.Leaf.Name == "重命名组件" && !view.IsEditingItem,
            label + ": native commit resumes tree query processing");
        view.SortDescriptions.Add(DataGridSortDescription.FromPath(nameof(TreePrototypeNode.Quantity),
            ListSortDirection.Descending));
        Flush();
        grid.SelectedItem = model.Leaf;
        grid.CurrentColumn = quantity;
        grid.Focus();
        Flush();
        check(grid.BeginEdit(), label + ": a sorted child enters native numeric editing");
        Flush();
        editor = grid.GetVisualDescendants().OfType<TextBox>().Single();
        editor.Text = "1";
        Flush();
        check(view.IndexOf(model.Leaf) < view.IndexOf(model.Branch.Children[1]),
            label + ": sorted row stays in place until edit completion");
        grid.CommitEdit(DataGridEditingUnit.Row, true);
        Flush();
        check(
            view.IndexOf(model.Leaf) > view.IndexOf(model.Branch.Children[1]) &&
            ReferenceEquals(grid.SelectedItem, model.Leaf),
            label + ": committed edits resort siblings while preserving original model selection");
        view.SortDescriptions.Clear();
        Flush();
        check(DataGridFeatures.ApplyFilter(grid, grid.Columns[0], new TextFilterCondition("组件")).Succeeded,
            label + ": editing can start inside an active tree filter");
        Flush();
        grid.SelectedItem = model.Leaf;
        grid.CurrentColumn = grid.Columns[0];
        grid.Focus();
        Flush();
        check(grid.BeginEdit(), label + ": matching leaf starts native editing without losing ancestors");
        Flush();
        editor = grid.GetVisualDescendants().OfType<TextBox>().Single();
        editor.Text = "不再匹配";
        Flush();
        check(view.Count == 3 && view.IsEditingItem, label + ": active filtering does not hide a pending native edit");
        grid.CommitEdit(DataGridEditingUnit.Row, true);
        Flush();
        check(view.Count == 0 && grid.SelectedItem is null,
            label + ": completed edit removes nonmatching leaf and unused ancestors");
        model.Leaf.Name = "组件恢复";
        Flush();
        check(view.Count == 3 && ReferenceEquals(view[2], model.Leaf),
            label + ": hidden rows remain observed after filtering removes all visible rows");
        DataGridFeatures.ClearAllFilters(grid);
        Flush();
        if (output is not null)
        {
            Directory.CreateDirectory(output);
            Save(window, Path.Combine(output, $"tree-query-{theme.Key}-{size}-wide.png"));
            window.Width = 390;
            Flush();
            Save(window, Path.Combine(output, $"tree-query-{theme.Key}-{size}-narrow.png"));
        }

        window.Close();
        Flush();
    }

    private static DataGridSortButton SortButton(DataGrid grid, DataGridColumn column)
    {
        return grid.GetVisualDescendants().OfType<DataGridColumnHeader>()
            .Single(header => Equals(header.Content, column.Header)).GetVisualDescendants().OfType<DataGridSortButton>()
            .Single();
    }

    private static void Click(Control target)
    {
        var host = TopLevel.GetTopLevel(target)!;
        var point = target.TranslatePoint(new Point(target.Bounds.Width / 2, target.Bounds.Height / 2), host)!.Value;
        host.MouseMove(point);
        host.MouseDown(point, MouseButton.Left);
        host.MouseUp(point, MouseButton.Left);
        Flush();
    }

    private static void Press(TopLevel host, Key key, PhysicalKey physical,
        RawInputModifiers modifiers = RawInputModifiers.None)
    {
        host.KeyPress(key, modifiers, physical, null);
        host.KeyRelease(key, modifiers, physical, null);
        Flush();
    }

    private static void Save(Window window, string path)
    {
        using var image = new RenderTargetBitmap(new PixelSize((int)window.Width, (int)window.Height));
        image.Render(window);
        image.Save(path, PngBitmapEncoderOptions.Default);
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private sealed class BrokenComparer : IComparer
    {
        public int Compare(object? x, object? y)
        {
            throw new InvalidOperationException("Rejected test comparison");
        }
    }

    private sealed class HiddenValueAccessor(object hidden) : IDataGridValueAccessor
    {
        public Type ValueType => typeof(int);

        public object GetValue(object item)
        {
            return ReferenceEquals(item, hidden)
                ? throw new InvalidOperationException("Hidden getter failed")
                : ((TreePrototypeNode)item).Quantity;
        }
    }
}