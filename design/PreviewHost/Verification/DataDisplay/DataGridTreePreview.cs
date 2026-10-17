using System.Collections;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class DataGridTreePreview
{
    public static void Verify(Action<bool, string> check, string? output = null)
    {
        VerifySource(check);
        VerifyNativeRowCancel(check);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var fixture = new DataGridTreeFixture();
            using var model = fixture.Model;
            var grid = fixture.Grid;
            var window = new Window
            {
                Width = 760, Height = 440, WindowDecorations = WindowDecorations.None,
                RequestedThemeVariant = theme, Content = new Border { Padding = new Thickness(16), Child = fixture }
            };
            ControlSizing.SetSize(window, size);
            Motion.SetEnabled(window, false);
            window.Show();
            Flush();
            var label = $"DataGrid/tree/{theme.Key}/{size}";
            check(grid.Columns.OfType<DataGridBoundColumn>().All(column => column.Binding is CompiledBinding),
                label + ": XAML infers original model bindings from the generic tree view");
            check(model.View.Count == 2 && ReferenceEquals(grid.CollectionView, model.View),
                label + ": native grid consumes collapsed tree without another view wrapper");

            TextBlock Label(TreePrototypeNode node)
            {
                return grid.Columns[0].GetCellContent(Rows(grid).Single(row => ReferenceEquals(row.DataContext, node)))
                    .GetVisualDescendants().OfType<TextBlock>().Single(block => block.Text == node.Name);
            }

            double X(Control control)
            {
                return control.TranslatePoint(default, grid)!.Value.X;
            }

            check(Math.Abs(X(Expander(grid, model.Root)) - X(Expander(grid, model.Other))) < 0.5
                  && Math.Abs(X(Label(model.Root)) - X(Label(model.Other))) < 0.5
                  && Label(model.Root).TextAlignment == TextAlignment.Left,
                label + ": different length root labels share left aligned arrow and text origins");
            if (output is not null)
            {
                Directory.CreateDirectory(output);
                using var wide = new RenderTargetBitmap(new PixelSize(760, 440));
                wide.Render(window);
                wide.Save(Path.Combine(output, $"datagrid-tree-{theme.Key}-{size}-wide.png"),
                    PngBitmapEncoderOptions.Default);
            }

            Click(Expander(grid, model.Root));
            check(model.View.Count == 4, label + ": pointer expands only immediate children");
            Click(Expander(grid, model.Branch));
            check(model.View.Count == 6 && ReferenceEquals(model.View[2], model.Leaf),
                label + ": nested tree preserves depth-first order and original row identity");
            var leafRow = Rows(grid).Single(row => ReferenceEquals(row.DataContext, model.Leaf));
            var text = grid.Columns[0].GetCellContent(leafRow).GetVisualDescendants().OfType<TextBlock>()
                .First(block => block.Text == model.Leaf.Name);
            check(text.TranslatePoint(default, grid)!.Value.X > 50 && text.Bounds.Height > 0,
                label + ": nested tree content is indented and arranged");
            check(Math.Abs(X(text) - X(Label(model.Root)) - 2 * ((DataGridTreeTextColumn)grid.Columns[0]).Indent) < 0.5,
                label + ": nested label origin advances by exactly one indent per level");

            grid.SelectedItem = model.Leaf;
            grid.CurrentColumn = grid.Columns[0];
            Flush();
            check(!grid.IsReadOnly && !grid.Columns[0].IsReadOnly && !((IList)model.View).IsReadOnly,
                label + ": protected structural projection still allows native model editing");
            check(grid.BeginEdit(), label + ": tree text column enters native editing");
            Flush();
            check(Editor(grid).TextAlignment == TextAlignment.Left,
                label + ": native editor preserves the left aligned tree text");
            Editor(grid).Text = "已编辑组件";
            Flush();
            check(
                grid.CommitEdit(DataGridEditingUnit.Row, true) && model.Leaf.Name == "已编辑组件" &&
                model.Leaf.CommitCount > 0,
                label + ": compiled native TextBox commits through IEditableObject");
            Flush();
            check(grid.BeginEdit(), label + ": committed tree row can re-enter edit");
            Flush();
            Editor(grid).Text = "应取消";
            Flush();
            var cancelled = grid.CancelEdit(DataGridEditingUnit.Row);
            Flush();
            check(cancelled && model.Leaf.Name == "已编辑组件" && model.Leaf.CancelCount > 0,
                label + ": native row cancellation restores the original model");

            check(grid.BeginEdit(), label + ": validation edit starts");
            Flush();
            Editor(grid).Text = "";
            Flush();
            check(!model.View.TrySetExpanded(model.Root, false) && model.View.Count == 6 && model.View.IsEditingItem,
                label + ": invalid edit prevents programmatic collapse and keeps the editor");
            Editor(grid).Text = "折叠时提交";
            Flush();
            check(
                model.View.TrySetExpanded(model.Root, false) && model.Leaf.Name == "折叠时提交" && !model.View.IsEditingItem,
                label + ": successful collapse first commits the native row edit");
            Flush();
            check(model.View.Count == 2 && ReferenceEquals(grid.SelectedItem, model.Root) &&
                  ReferenceEquals(model.View.CurrentItem, model.Root)
                  && ReferenceEquals(grid.CurrentColumn, grid.Columns[0]),
                label + ": hiding selected descendant moves single selection and current cell to parent");
            model.View.TrySetExpanded(model.Root, true);
            Flush();
            check(model.View.Count == 6 && ReferenceEquals(grid.SelectedItem, model.Root),
                label + ": re-expansion restores descendant expansion without restoring hidden selection");

            grid.SelectionMode = DataGridSelectionMode.Extended;
            grid.SelectedItems.Clear();
            grid.SelectedItems.Add(model.Leaf);
            grid.SelectedItems.Add(model.Other);
            model.View.TrySetExpanded(model.Root, false);
            Flush();
            check(grid.SelectedItems.Count == 1 && ReferenceEquals(grid.SelectedItems[0], model.Other),
                label + ": collapse removes hidden selections and preserves unrelated selection");
            grid.SelectionMode = DataGridSelectionMode.Single;
            grid.SelectedItem = model.Root;
            grid.CurrentColumn = grid.Columns[0];
            grid.Focus();
            window.KeyPress(Key.Right, RawInputModifiers.None, PhysicalKey.ArrowRight, null);
            window.KeyRelease(Key.Right, RawInputModifiers.None, PhysicalKey.ArrowRight, null);
            Flush();
            check(model.View.Count == 6, label + ": tree-column Right key expands through the projection");
            grid.CurrentColumn = grid.Columns[1];
            window.KeyPress(Key.Left, RawInputModifiers.None, PhysicalKey.ArrowLeft, null);
            window.KeyRelease(Key.Left, RawInputModifiers.None, PhysicalKey.ArrowLeft, null);
            Flush();
            check(model.View.Count == 6, label + ": non-tree columns retain native horizontal navigation");

            model.View.TrySetExpanded(model.Other, true);
            Flush();
            var remote = model.Other.Children[^1];
            grid.SelectedItem = remote;
            grid.ScrollIntoView(remote, grid.Columns[0]);
            Flush();
            check(
                model.View.Count == 1006 && Rows(grid).Length is > 0 and < 45 &&
                Rows(grid).Any(row => ReferenceEquals(row.DataContext, remote)),
                label + ": thousand descendants retain bounded native realization and remote selection");
            grid.ScrollIntoView(model.Root, grid.Columns[0]);
            Flush();
            check(
                Rows(grid).Length < 45 && grid.GetVisualDescendants().OfType<TextBlock>()
                    .Any(block => block.Text == model.Root.Name),
                label + ": recycled cells show current original model content");
            grid.SelectedItem = model.Root;
            model.View.TrySetExpanded(model.Other, false);
            Flush();
            window.Width = 360;
            Flush();
            check(Rows(grid).Length < 45 && model.View.Count == 6,
                label + ": narrow viewport retains native row virtualization and hierarchy");
            if (output is not null)
            {
                Directory.CreateDirectory(output);
                using var bitmap = new RenderTargetBitmap(new PixelSize(360, 440));
                bitmap.Render(window);
                bitmap.Save(Path.Combine(output, $"datagrid-tree-{theme.Key}-{size}.png"),
                    PngBitmapEncoderOptions.Default);
            }

            window.Close();
            Flush();
            model.View.TrySetExpanded(model.Root, false);
            check(model.View.Count == 2, label + ": detached grid releases tree coordination handlers");
            var rebound = new DataGrid { ItemsSource = model.View, AutoGenerateColumns = false };
            DataGridTreeFeatures.SetIsEnabled(rebound, true);
            var nextWindow = new Window { Width = 400, Height = 200, Content = rebound };
            nextWindow.Show();
            Flush();
            check(ReferenceEquals(rebound.CollectionView, model.View),
                label + ": detached tree view can attach to another grid");
            nextWindow.Close();
            Flush();
        }
    }

    private static void VerifyNativeRowCancel(Action<bool, string> check)
    {
        var row = new TreePrototypeNode("普通行") { Quantity = 2 };
        var view = new DataGridCollectionView<TreePrototypeNode>(new[] { row });
        var grid = new DataGrid { ItemsSource = view, AutoGenerateColumns = false };
        grid.Columns.Add(new DataGridTextColumn
        {
            Header = "名称", Width = new DataGridLength(240),
            Binding = CompiledBinding.Create<TreePrototypeNode, string>(node => node.Name, mode: BindingMode.TwoWay)
        });
        grid.Columns.Add(new DataGridTextColumn
        {
            Header = "数量", Width = new DataGridLength(100),
            Binding = CompiledBinding.Create<TreePrototypeNode, int>(node => node.Quantity, mode: BindingMode.TwoWay)
        });
        var outside = new Button { Content = "Outside grid" };
        var window = new Window
            { Width = 600, Height = 280, Content = new StackPanel { Children = { grid, outside } } };
        window.Show();
        Flush();
        grid.SelectedItem = row;
        grid.CurrentColumn = grid.Columns[1];
        grid.Focus();
        Flush();
        check(grid.BeginEdit(), "DataGrid/native: ordinary row begins a multi-cell edit");
        Flush();
        Editor(grid).Text = "7";
        Flush();
        check(grid.CommitEdit(DataGridEditingUnit.Cell, true) && view.IsEditingItem && row.Quantity == 7,
            "DataGrid/native: editor-to-grid focus keeps the row transaction open after a cell commit");
        grid.CurrentColumn = grid.Columns[0];
        check(grid.BeginEdit(), "DataGrid/native: another cell joins the same row transaction");
        Flush();
        Editor(grid).Text = "撤销名称";
        Flush();
        var cancelled = grid.CancelEdit(DataGridEditingUnit.Row);
        Flush();
        check(cancelled && row.Name == "普通行" && row.Quantity == 2 && row.CancelCount == 1 && row.CommitCount == 0,
            "DataGrid/native: row cancellation rolls back all cells without an implicit focus-loss commit");
        check(grid.BeginEdit(), "DataGrid/native: cancelled row can edit again");
        Flush();
        Editor(grid).Text = "外部焦点提交";
        Flush();
        outside.Focus();
        Flush();
        check(row.Name == "外部焦点提交" && !view.IsEditingItem && row.CommitCount == 1,
            "DataGrid/native: actual focus departure still commits the native row");
        window.Close();
        Flush();
    }

    private static void VerifySource(Action<bool, string> check)
    {
        using var model = new DataGridTreeFixtureModel();
        var view = model.View;
        check(
            view.CanSort && view.CanFilter && !view.CanGroup && !view.CanAddNew && !view.CanRemove &&
            !view.CanChangePage,
            "DataGrid/tree: tree queries are enabled while structural view operations stay disabled");

        bool Rejected(Action action)
        {
            try
            {
                action();
                return false;
            }
            catch (NotSupportedException)
            {
                return true;
            }
        }

        view.SortDescriptions.Add(DataGridSortDescription.FromPath(nameof(TreePrototypeNode.Name)));
        check(view.SortDescriptions.Count == 1 && view.Count == 2,
            "DataGrid/tree: sibling sorting retains collapsed root rows");
        view.SortDescriptions.Clear();
        view.Filter = _ => true;
        view.Filter = null!;
        check(Rejected(() => view.PageSize = 2), "DataGrid/tree: paging cannot split the tree");
        var notifications = new List<NotifyCollectionChangedEventArgs>();
        view.CollectionChanged += (_, args) => notifications.Add(args);
        view.TrySetExpanded(model.Root, true);
        view.TrySetExpanded(model.Branch, true);
        check(
            notifications.All(args =>
                args.Action != NotifyCollectionChangedAction.Reset &&
                (args.NewItems?.Count ?? args.OldItems?.Count) == 1),
            "DataGrid/tree: expansion publishes native single-item deltas without Reset");
        view.TrySetExpanded(model.Root, false);
        var added = new TreePrototypeNode("折叠分支新增");
        model.Branch.Children.Add(added);
        check(view.Count == 2 && view.TryGetNodeState(added, out var state) && state.Depth == 2,
            "DataGrid/tree: collapsed collections remain observed without realizing rows");
        view.TrySetExpanded(model.Root, true);
        check(view.Cast<object>().Any(item => ReferenceEquals(item, added)),
            "DataGrid/tree: collapsed descendant changes appear after expansion");
        var previous = model.Branch.Children;
        model.Branch.Children = [model.Leaf];
        previous.Add(new TreePrototypeNode("旧集合"));
        check(view.Count == 5 && !view.TryGetNodeState(added, out _),
            "DataGrid/tree: replacing Children reconnects subscriptions and drops old nodes");
        view.EditItem(model.Leaf);
        check(!view.TrySetExpanded(model.Root, false),
            "DataGrid/tree: standalone view refuses to hide an uncommitted native edit");
        var delayed = new TreePrototypeNode("编辑期间新增");
        model.Branch.Children.Add(delayed);
        check(!view.TryGetNodeState(delayed, out _),
            "DataGrid/tree: structural source changes defer during native edit");
        view.CancelEdit();
        Flush();
        check(view.TryGetNodeState(delayed, out _) && view.Count == 6,
            "DataGrid/tree: deferred structural refresh runs after edit completion");
        var invalid = false;
        try
        {
            model.Branch.Children.Add(model.Root);
        }
        catch (InvalidOperationException)
        {
            invalid = true;
        }

        check(invalid && view.Count == 6,
            "DataGrid/tree: cyclic source is rejected without corrupting the published projection");
        model.Branch.Children.Remove(model.Root);
        invalid = false;
        try
        {
            model.Root.Children.Add(model.Leaf);
        }
        catch (InvalidOperationException)
        {
            invalid = true;
        }

        check(invalid && view.Count == 6, "DataGrid/tree: shared model instances are rejected");
        model.Root.Children.Remove(model.Leaf);
        model.Roots.Clear();
        check(view.Count == 0, "DataGrid/tree: clearing roots removes the whole projection");
        var notified = false;

        void AppendDuringNotification(object? sender, EventArgs args)
        {
            if (notified) return;
            notified = true;
            model.Roots.Add(model.Other);
        }

        view.TreeChanged += AppendDuringNotification;
        model.Roots.Add(model.Root);
        Flush();
        view.TreeChanged -= AppendDuringNotification;
        check(view.Count == 2, "DataGrid/tree: source mutation inside TreeChanged drains after the current projection");
        model.Roots.Clear();
        var emptyGrid = new DataGrid { ItemsSource = view, AutoGenerateColumns = true };
        var emptyWindow = new Window { Width = 600, Height = 200, Content = emptyGrid };
        emptyWindow.Show();
        Flush();
        check(emptyGrid.Columns.Any(column => Equals(column.Header, nameof(TreePrototypeNode.Name))),
            "DataGrid/tree: empty tree collection shape retains explicit row metadata");
        emptyWindow.Close();
        Flush();
    }

    private static TextBox Editor(DataGrid grid)
    {
        return grid.GetVisualDescendants().OfType<TextBox>().Single();
    }

    private static DataGridRow[] Rows(DataGrid grid)
    {
        return grid.GetVisualDescendants().OfType<DataGridRow>().Where(row => row.IsVisible).ToArray();
    }

    private static Button Expander(DataGrid grid, object item)
    {
        return grid.GetVisualDescendants().OfType<Button>().Single(button =>
            button.Name == "TreeExpander" && ReferenceEquals(button.DataContext, item));
    }

    private static void Click(Control control)
    {
        var window = TopLevel.GetTopLevel(control)!;
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!
            .Value;
        window.MouseMove(point);
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        Flush();
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }
}