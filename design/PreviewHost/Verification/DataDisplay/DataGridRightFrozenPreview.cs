using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static partial class DataGridRightFrozenPreview
{
    public static void Verify(Action<bool, string> check)
    {
        VerifyEarlyConfiguration(check);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>()) VerifyProfile(theme, size, check);
    }

    private static void VerifyEarlyConfiguration(Action<bool, string> check)
    {
        var fixture = new DataGridStage1Fixture();
        var grid = new DataGrid { AutoGenerateColumns = false, MaxColumnWidth = 320, ItemsSource = fixture.Records };
        DataGridFeatures.SetFrozenRightColumnCount(grid, 1);
        var first = new DataGridTextColumn { Header = "名称", Width = new DataGridLength(160), Binding = global::Avalonia.Data.CompiledBinding.Create<GridRecord, string>(r => r.Name) };
        var last = new DataGridTextColumn { Header = "数量", Width = new DataGridLength(100), Binding = global::Avalonia.Data.CompiledBinding.Create<GridRecord, int>(r => r.Quantity) };
        grid.Columns.Add(first); grid.Columns.Add(last);
        var window = new Window { Width = 420, Height = 300, WindowDecorations = WindowDecorations.None, Content = grid };
        Motion.SetEnabled(window, false); window.Show(); Flush();
        check(last.IsFrozen && !first.IsFrozen, "DataGrid/right: count configured before column creation selects the eventual last visible column");
        grid.Columns.Clear(); Flush();
        check(DataGridFeatures.GetFrozenRightColumnCount(grid) == 1, "DataGrid/right: clearing columns retains configured count without invalid geometry");
        grid.Columns.Add(last); grid.Columns.Add(first); Flush();
        var finalVisibleColumn = grid.Columns.MaxBy(column => column.DisplayIndex)!;
        check(finalVisibleColumn.IsFrozen && grid.Columns.Where(column => column != finalVisibleColumn).All(column => !column.IsFrozen),
            "DataGrid/right: reused columns retain native DisplayIndex and recompute right membership");
        first.DisplayIndex = 1; Flush();
        check(first.IsFrozen && !last.IsFrozen, "DataGrid/right: reordering reused columns updates the right frozen region");
        window.Close(); Flush();
    }

    private static void VerifyProfile(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var fixture = new DataGridStage1Fixture();
        var grid = fixture.Grid;
        DataGridFeatures.SetFrozenRightColumnCount(grid, 1);
        var window = new Window { Width = 640, Height = 360, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme, Content = grid };
        ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); Flush();
        string tag = $"DataGrid/right/{theme.Key}/{size}";
        void Expect(bool success, string message) => check(success, $"{tag}: {message}");
        var right = fixture.RightColumn;
        var left = grid.Columns[0];
        var headers = grid.GetVisualDescendants().OfType<DataGridColumnHeadersPresenter>().Single();
        DataGridColumnHeader Header(DataGridColumn column) => headers.GetVisualDescendants().OfType<DataGridColumnHeader>().Single(h => Equals(h.Content, column.Header));
        DataGridCell? Cell(DataGridColumn column, DataGridRow row) => row.GetVisualDescendants().OfType<DataGridCell>().FirstOrDefault(cell => DataGridColumn.GetColumnContainingElement(cell) == column);
        double X(DataGridColumn column) => Header(column).TranslatePoint(default, grid)!.Value.X;
        double RightEdge() => headers.TranslatePoint(new Point(headers.Bounds.Width, 0), grid)!.Value.X;
        void Layout()
        {
            double rightX = X(right);
            Expect(Math.Abs(rightX + right.ActualWidth - RightEdge()) < 2, "right header is anchored to the native viewport edge");
            Expect(fixture.Rows.All(row => fixture.RightCell(row)?.IsEffectivelyVisible == true), "right cells are measured and displayed in every realized row");
            Expect(fixture.Rows.All(row => Math.Abs(fixture.RightCell(row)!.TranslatePoint(default, grid)!.Value.X - rightX) < 1), "right header and cells have matching native coordinates");
            bool middleClipsValid = true;
            foreach (var row in fixture.Rows)
            foreach (var column in grid.Columns.Where(c => c.IsVisible && !c.IsFrozen))
            {
                var cell = Cell(column, row);
                if (cell?.IsVisible != true) continue;
                var clip = (RectangleGeometry)cell.Clip!;
                var origin = cell.TranslatePoint(default, grid)!.Value;
                if (clip.Rect.Width > 0)
                    middleClipsValid &= origin.X + clip.Rect.X >= X(left) + left.ActualWidth - 1 && origin.X + clip.Rect.Right <= rightX + 1;
            }
            Expect(middleClipsValid, "every realized middle cell clip stays between frozen regions");
        }
        Expect(grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Count() == 1 && fixture.Rows.Length is > 1 and < 100,
            $"1,000 models retain one native rows presenter and {fixture.Rows.Length} rows");
        Expect(fixture.Rows.All(r => r.GetVisualDescendants().OfType<DataGridCellsPresenter>().Count() == 1), "native cells presenters are retained");
        Expect(left.IsFrozen && right.IsFrozen && !grid.Columns[1].IsFrozen, "both edges are classified as frozen and middle remains scrollable");
        Layout();
        var rightPeer = global::Avalonia.Automation.Peers.ControlAutomationPeer.CreatePeerForElement(fixture.RightCell(fixture.Rows.First())!)!;
        Expect(!rightPeer.IsOffscreen() && Math.Abs(rightPeer.GetBoundingRectangle().X - Cell(right, fixture.Rows.First())!.TranslatePoint(default, window)!.Value.X) < 1,
            "automation exposes right cell at its actual pinned position");
        var clippedHeader = Header(grid.Columns[5]);
        Expect(global::Avalonia.Automation.Peers.ControlAutomationPeer.CreatePeerForElement(clippedHeader)!.IsOffscreen(), "fully clipped middle header is offscreen to automation");
        var initialRows = fixture.Rows.ToHashSet(ReferenceEqualityComparer.Instance);
        double pinnedRight = X(right), pinnedLeft = X(left), middle = X(grid.Columns[1]);
        var wheelPoint = fixture.RowsPresenter.TranslatePoint(new Point(220, 20), window)!.Value;
        window.MouseWheel(wheelPoint, new Vector(-3, 0)); Flush();
        Expect(fixture.HorizontalScrollBar.Value > 0 && X(grid.Columns[1]) < middle, "real horizontal wheel scrolls middle columns");
        Expect(Math.Abs(X(right) - pinnedRight) < 1 && Math.Abs(X(left) - pinnedLeft) < 1, "wheel preserves both frozen anchors");
        Layout();
        double before = fixture.HorizontalScrollBar.Value;
        grid.ScrollIntoView(fixture.Records[0], right); Flush();
        Expect(Math.Abs(fixture.HorizontalScrollBar.Value - before) < 0.1, "ScrollIntoView of right column preserves horizontal offset");
        grid.ScrollIntoView(fixture.Records[0], grid.Columns[5]); Flush();
        var middleHeader = Header(grid.Columns[5]);
        var middleClip = (RectangleGeometry)middleHeader.Clip!;
        Expect(middleClip.Rect.Width >= grid.Columns[5].ActualWidth - 1, "ScrollIntoView exposes a middle column within both frozen edges");
        var rightRow = fixture.Rows.First();
        var rightPoint = Cell(right, rightRow)!.TranslatePoint(new Point(50, 15), window)!.Value;
        window.MouseMove(rightPoint); window.MouseDown(rightPoint, MouseButton.Left); window.MouseUp(rightPoint, MouseButton.Left); Flush();
        Expect(grid.CurrentColumn == right && grid.SelectedItem == rightRow.DataContext, "native pointer hit testing selects the right cell");
        window.KeyPress(Key.Left, RawInputModifiers.None, PhysicalKey.ArrowLeft, null); window.KeyRelease(Key.Left, RawInputModifiers.None, PhysicalKey.ArrowLeft, null); Flush();
        Expect(grid.CurrentColumn == grid.Columns[5], "native keyboard navigation moves from right into middle");
        grid.SelectedItem = fixture.Records[^1]; grid.ScrollIntoView(fixture.Records[^1], right); Flush();
        Expect(fixture.Rows.Any(r => r.DataContext == fixture.Records[^1]) && fixture.Rows.Length < 100, "thousandth row is selected with bounded realization");
        Layout();
        grid.SelectedItem = fixture.Records[0]; grid.ScrollIntoView(fixture.Records[0], right); Flush();
        Expect(fixture.Rows.Any(initialRows.Contains) && fixture.CreatedRows < 100, $"round trip recycles containers ({fixture.CreatedRows} created)");
        grid.CurrentColumn = right; grid.Focus(); Flush();
        Expect(grid.BeginEdit(), "right cell starts native editing"); Flush();
        var editor = fixture.Rows.First(r => r.DataContext == fixture.Records[0]).GetVisualDescendants().OfType<TextBox>().Single(b => b.Name == "CellTextBox");
        editor.Focus(); editor.SelectAll(); window.KeyTextInput("123.5"); Flush();
        Expect(grid.CommitEdit() && fixture.Records[0].Price == 123.5m, "right cell commits through compiled numeric binding"); Flush();
        right.Width = new DataGridLength(150); Flush(); Layout();
        Expect(Math.Abs(right.ActualWidth - 150) < 1, "right width change updates header and cell layout");
        // Native row groups and filtering must remain compatible with frozen cells.
        var view = new global::Avalonia.Collections.DataGridCollectionView(fixture.Records);
        grid.ItemsSource = view;
        view.GroupDescriptions.Add(new global::Avalonia.Collections.DataGridPathGroupDescription(nameof(GridRecord.Warehouse))); Flush();
        Expect(grid.GetVisualDescendants().OfType<DataGridRowGroupHeader>().Any(), "native grouped rows remain available"); Layout();
        view.GroupDescriptions.Clear(); Flush();
        DataGridColumnOptions.SetValueAccessor(right, GridFilterFields.Price);
        DataGridFeatures.SetShowColumnFilters(grid, true);
        var result = DataGridFeatures.ApplyFilter(grid, right, new NumericFilterCondition<decimal>(NumericFilterMode.Range, minimum: 500, maximum: 510)); Flush();
        Expect(result.Succeeded && view.Count > 0 && view.Count < 1000 && fixture.Rows.All(r => ((GridRecord)r.DataContext!).Price >= 500), "right frozen column composes with numeric filtering"); Layout();
        Expect(DataGridFeatures.ClearAllFilters(grid).Succeeded && view.Count == 1000, "clear filters restores all source models"); Flush();
        right.SortMemberPath = nameof(GridRecord.Price);
        var sortPoint = Header(right).TranslatePoint(new Point(40, 14), window)!.Value;
        window.MouseMove(sortPoint); window.MouseDown(sortPoint, MouseButton.Left); window.MouseUp(sortPoint, MouseButton.Left); Flush();
        Expect(view.SortDescriptions.Count == 1 && ((GridRecord)view[0]).Price <= ((GridRecord)view[1]).Price, "right header pointer sorting uses native collection view");
        view.SortDescriptions.Clear(); Flush();
        // DisplayIndex controls which visible columns are frozen; no title matching.
        right.DisplayIndex = 4; Flush();
        var last = grid.Columns.Where(c => c.IsVisible).MaxBy(c => c.DisplayIndex)!;
        Expect(last != right && last.IsFrozen && !right.IsFrozen, "programmatic reordering reassigns right region by visible DisplayIndex");
        right.DisplayIndex = 6; Flush(); Layout();
        grid.Columns[5].IsVisible = false; Flush(); Layout();
        DataGridFeatures.SetFrozenRightColumnCount(grid, 2); Flush();
        Expect(grid.Columns[4].IsFrozen && right.IsFrozen, "two right columns skip hidden columns");
        grid.Columns[5].IsVisible = true; Flush();
        var dragStart = Header(right).TranslatePoint(new Point(40, 14), window)!.Value;
        var dragEnd = Header(grid.Columns[5]).TranslatePoint(new Point(20, 14), window)!.Value;
        window.MouseMove(dragStart); window.MouseDown(dragStart, MouseButton.Left); window.MouseMove(dragStart - new Vector(12, 0)); Flush();
        window.MouseMove(dragEnd); window.MouseUp(dragEnd, MouseButton.Left); Flush();
        Expect(right.DisplayIndex == 5 && grid.Columns[5].DisplayIndex == 6 && right.IsFrozen && grid.Columns[5].IsFrozen,
            "native pointer drag reorders two right frozen columns within their region");
        right.DisplayIndex = 6; DataGridFeatures.SetFrozenRightColumnCount(grid, 1); Flush();
        var extra = new DataGridTextColumn { Header = "新增末列", Width = new DataGridLength(90), Binding = global::Avalonia.Data.CompiledBinding.Create<GridRecord, int>(r => r.Id) };
        grid.Columns.Add(extra); Flush();
        Expect(extra.IsFrozen && !right.IsFrozen, "adding a last column reassigns right frozen membership");
        grid.Columns.Remove(extra); Flush(); Layout();
        right.IsVisible = false; Flush();
        Expect(grid.Columns[5].IsFrozen && !right.IsFrozen, "hiding the right column assigns the preceding visible column");
        right.IsVisible = true; Flush(); Layout();
        // Real native resize gesture on the right boundary.
        var resizePoint = Header(right).TranslatePoint(new Point(right.ActualWidth - 2, 14), window)!.Value;
        window.MouseMove(resizePoint); window.MouseDown(resizePoint, MouseButton.Left); window.MouseMove(resizePoint + new Vector(-20, 0)); window.MouseUp(resizePoint + new Vector(-20, 0), MouseButton.Left); Flush();
        Expect(right.ActualWidth < 150, "native pointer resizing works in the right region"); Layout();
        right.Width = DataGridLength.Auto; Flush(); Layout();
        Expect(right.ActualWidth is >= 64 and <= 320, "auto width measures the pinned column within configured limits");
        window.Width = 1400; grid.Columns[1].Width = new DataGridLength(1, DataGridLengthUnitType.Star); Flush(); Layout();
        Expect(grid.Columns[1].ActualWidth > 180 && fixture.HorizontalScrollBar.Maximum >= 0, "star width uses the available viewport with both frozen regions");
        grid.Columns[1].Width = new DataGridLength(180); right.Width = new DataGridLength(150); Flush();
        window.Width = 220; Flush();
        var clipped = fixture.RightCell(fixture.Rows.First())!.Clip as RectangleGeometry;
        Expect(clipped != null && clipped.Rect.Width >= 0 && fixture.HorizontalScrollBar.ViewportSize >= 0, "narrow viewport uses bounded clipping without negative geometry");
        window.Width = 640; Flush(); Layout();
        DataGridFeatures.SetFrozenRightColumnCount(grid, 0); Flush();
        Expect(!right.IsFrozen && grid.Columns[0].IsFrozen, "disabling right freeze restores original left-only behavior");
        before = fixture.HorizontalScrollBar.Value; grid.ScrollIntoView(fixture.Records[0], right); Flush();
        Expect(fixture.HorizontalScrollBar.Value >= before, "left-only native ScrollIntoView remains functional");
        DataGridFeatures.SetFrozenRightColumnCount(grid, 1); Flush(); Layout();
        bool negativeRejected = false, overlapRejected = false, leftOverlapRejected = false;
        try { DataGridFeatures.SetFrozenRightColumnCount(grid, -1); } catch (ArgumentException) { negativeRejected = true; }
        try { DataGridFeatures.SetFrozenRightColumnCount(grid, 7); } catch (ArgumentException) { overlapRejected = true; }
        try { grid.FrozenColumnCount = 7; } catch (ArgumentException) { leftOverlapRejected = true; }
        Expect(leftOverlapRejected && grid.FrozenColumnCount == 1, "left configuration also rejects overlapping right region");
        Expect(negativeRejected && overlapRejected && DataGridFeatures.GetFrozenRightColumnCount(grid) == 1, "negative and overlapping configuration is rejected atomically");
        window.Content = null; Flush(); window.Content = grid; Flush(); Layout();
        Expect(fixture.CreatedRows < 100, "detach and reattach preserve bounded native containers");
        window.Close(); Flush();
    }

    private static void Flush()
    {
        global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();
    }
}
