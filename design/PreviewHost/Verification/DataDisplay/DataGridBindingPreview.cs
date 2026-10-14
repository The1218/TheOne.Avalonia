using System.ComponentModel;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Presenters;
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

internal static class DataGridBindingPreview
{
    public static void Verify(Action<bool, string> check)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var fixture = new DataGridCollectionsFixture();
            var window = Open(fixture, theme, size, 820);
            foreach (var (grid, source) in new[] { (fixture.List, "List"), (fixture.Observable, "ObservableCollection"), (fixture.Typed, "GenericView") })
            {
                var label = $"DataGrid/binding/{theme.Key}/{size}/{source}";
                check(grid.Columns.OfType<DataGridBoundColumn>().All(column => column.Binding is CompiledBinding), label + ": column bindings infer the row type without column x:DataType");
                check(View(grid).Count == 1000 && Rows(grid).Length is > 0 and < 40, label + ": thousand rows create bounded native containers");
                var row = Rows(grid).First(item => item.Index == 0);
                check(grid.Columns[1].GetCellContent(row) is TextBlock { Text: "设备 0001" }, label + ": inferred text binding reads the correct row");
                var checkbox = (CheckBox)grid.Columns[2].GetCellContent(row);
                var cell = checkbox.GetVisualAncestors().OfType<DataGridCell>().First();
                var glyph = checkbox.GetVisualDescendants().OfType<Border>().First(part => part.Name == "NormalRectangle");
                check(Inside(glyph, checkbox) && Inside(checkbox, cell), label + ": checkbox glyph and control fit inside the cell");
                var button = (Button)grid.Columns[3].GetCellContent(row);
                check(Equals(button.Content, "设备 0001") && Inside(button, button.GetVisualAncestors().OfType<DataGridCell>().First()), label + ": inferred template button is fully visible");
                VerifySorting(window, grid, label, check);
                var target = fixture.Model.ListRows[999]; grid.SelectedItem = target; grid.ScrollIntoView(target, grid.Columns[1]); Flush();
                check(Rows(grid).Any(item => ReferenceEquals(item.DataContext, target)) && Rows(grid).Length < 40, label + ": remote row remains selectable with bounded recycling");
                grid.ScrollIntoView(fixture.Model.ListRows[0], grid.Columns[0]); Flush();
                check(Rows(grid).Length < 40, label + ": return scroll keeps recycling bounded");
            }
            fixture.Model.ObservableRows.Add(new GridRecord(1001, "新设备", "上海仓", 1, 99m, true, "")); Flush();
            check(View(fixture.Observable).Count == 1001 && Rows(fixture.Observable).Length < 40, $"DataGrid/binding/{theme.Key}/{size}: observable insertion updates the native view without eager realization");
            window.Close(); Flush();
            VerifyPreview(theme, size, check);
        }
    }
    private static void VerifySorting(Window window, DataGrid grid, string label, Action<bool, string> check)
    {
        var events = 0; grid.Sorting += (_, _) => events++;
        var first = SortButton(grid, "编号"); var second = SortButton(grid, "设备");
        check(first.IsVisible && first.IsEnabled && first.Direction is null, label + ": unsorted columns expose an explicit sort button");
        var icon = first.GetVisualDescendants().OfType<global::Avalonia.Controls.Shapes.Path>().Single(part => part.Name == "SortIcon");
        check(icon.Data?.Bounds == new Rect(1, 0, 8, 12) && Inside(icon, first), label + ": symmetric stacked unsorted arrows fit the button");
        Click(first); check(first.Direction == ListSortDirection.Ascending && events == 1, label + ": pointer sort raises exactly one native sorting event");
        Click(first); check(first.Direction == ListSortDirection.Descending && ((GridRecord)View(grid)[0]).Id == 1000 && events == 2, label + ": pointer sort toggles descending and updates the icon");
        var sortedRow = Rows(grid).First(row => row.Index == 0);
        var idText = (TextBlock)grid.Columns[0].GetCellContent(sortedRow);
        check(idText.Text == "1000" && idText.DesiredSize.Width <= idText.Bounds.Width + 0.5 && idText.TextLayout.Height <= idText.Bounds.Height + 0.5,
            label + ": sorted four-digit content fits its arranged text bounds");
        first.Focus(); window.KeyPress(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null); window.KeyRelease(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null); Flush();
        check(first.Direction is null && first.SortPriority == 0 && grid.CollectionView.SortDescriptions.Count == 0 && events == 3,
            label + ": Enter completes the three-state cycle exactly once");
        first.Focus(); window.KeyPress(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null); window.KeyRelease(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null); Flush();
        check(first.Direction == ListSortDirection.Ascending && events == 4, label + ": Space starts the next cycle without duplicate activation");
        Click(second, RawInputModifiers.Shift);
        check(grid.CollectionView.SortDescriptions.Count == 2 && first.Direction == ListSortDirection.Ascending && second.Direction == ListSortDirection.Ascending, label + ": Shift preserves the first sort and adds a secondary column");
        var firstPriority = first.GetVisualDescendants().OfType<TextBlock>().Single(part => part.Name == "SortPriority");
        var secondPriority = second.GetVisualDescendants().OfType<TextBlock>().Single(part => part.Name == "SortPriority");
        check(first.SortPriority == 1 && second.SortPriority == 2 && firstPriority.IsVisible && secondPriority.IsVisible &&
            firstPriority.Text == "1" && secondPriority.Text == "2" && Inside(firstPriority, first) && Inside(secondPriority, second),
            label + ": multi-sort priorities are ordered and fit all densities");
        Click(first, RawInputModifiers.Shift);
        check(first.Direction == ListSortDirection.Descending && first.SortPriority == 1 && second.SortPriority == 2,
            label + ": changing direction preserves multi-sort priority");
        Click(first, RawInputModifiers.Shift);
        check(first.Direction is null && first.SortPriority == 0 && second.SortPriority == 1 && grid.CollectionView.SortDescriptions.Count == 1 && !secondPriority.IsVisible,
            label + ": third Shift activation removes only the column and renumbers the remaining priority");
        Click(first, RawInputModifiers.Shift);
        check(first.SortPriority == 2 && second.SortPriority == 1 && firstPriority.Text == "2" && secondPriority.Text == "1",
            label + ": re-added column becomes secondary regardless of header position");
        grid.CollectionView.SortDescriptions.RemoveAt(0); Flush();
        check(second.Direction is null && first.SortPriority == 1 && !firstPriority.IsVisible,
            label + ": external sort collection changes update all priorities");
        grid.Columns[1].Sort(ListSortDirection.Descending); Flush();
        check(first.Direction is null && second.Direction == ListSortDirection.Descending && second.SortPriority == 1,
            label + ": native programmatic sorting updates the button state");
        Click(second);
        check(second.Direction is null && grid.CollectionView.SortDescriptions.Count == 0,
            label + ": button third state clears a sort started by the native API");
        Click(first); Click(second, RawInputModifiers.Shift);
        Click(first, (Application.Current?.PlatformSettings?.HotkeyConfiguration.CommandModifiers ?? KeyModifiers.Control).HasFlag(KeyModifiers.Meta) ? RawInputModifiers.Meta : RawInputModifiers.Control);
        check(grid.CollectionView.SortDescriptions.Count == 0 && first.Direction is null && second.Direction is null, label + ": platform clear modifier resets all sort indicators");
        grid.CanUserSortColumns = false; Flush(); var before = events; Click(first);
        check(!first.IsEnabled && events == before, label + ": disabling grid sorting disables buttons and suppresses activation");
        grid.CanUserSortColumns = true; grid.Columns[0].CanUserSort = false; Flush();
        check(!first.IsEnabled && second.IsEnabled, label + ": per-column sorting availability updates independently");
        grid.Columns[0].CanUserSort = true; Flush();
        grid.Sorting += Cancel; Click(first); grid.Sorting -= Cancel;
        check(grid.CollectionView.SortDescriptions.Count == 0, label + ": the native Sorting event can intercept button sorting");
        void Cancel(object? sender, DataGridColumnEventArgs args) => args.Handled = true;
    }
    private static void VerifyPreview(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var demo = new DataGridBindingDemoView(); var window = Open(demo, theme, size, 470);
        var grid = demo.Grid; var row = Rows(grid).First(item => item.Index == 0);
        var action = (Button)grid.Columns[3].GetCellContent(row);
        Click(action); check(demo.Model.ActionLabel.Contains("#0001") && Inside(action, action.GetVisualAncestors().OfType<DataGridCell>().First()), $"DataGrid/preview/{theme.Key}/{size}: cell command receives the row and its button fits");
        var toggle = (ToggleButton)grid.Columns[4].GetCellContent(row); var old = demo.Model.ListRows[0].Enabled; Click(toggle);
        check(demo.Model.ListRows[0].Enabled != old && Inside(toggle, toggle.GetVisualAncestors().OfType<DataGridCell>().First()), $"DataGrid/preview/{theme.Key}/{size}: compact toggle button remains visible and writes the row model");
        var sort = SortButton(grid, "编号"); Click(sort);
        Click(demo.FindControl<Button>("UseObservable")!);
        check(sort.Direction is null && sort.SortPriority == 0, $"DataGrid/preview/{theme.Key}/{size}: changing source clears stale sorting state");
        Click(sort); Click(SortButton(grid, "设备"), RawInputModifiers.Shift);
        check(sort.SortPriority == 1 && SortButton(grid, "设备").SortPriority == 2, $"DataGrid/preview/{theme.Key}/{size}: buttons subscribe to the replacement collection view");
        Click(demo.FindControl<Button>("AddRow")!);
        check(ReferenceEquals(grid.ItemsSource, demo.Model.ObservableRows) && View(grid).Count == 1001, $"DataGrid/preview/{theme.Key}/{size}: source buttons switch to an observable collection and add through MVVM");
        Click(demo.FindControl<Button>("UseList")!);
        check(ReferenceEquals(grid.ItemsSource, demo.Model.ListRows) && View(grid).Count == 1000, $"DataGrid/preview/{theme.Key}/{size}: returning to List retains inferred bindings");
        window.Close(); Flush();
    }
    public static void Render(string output)
    {
        Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var demo = new DataGridBindingDemoView(); var window = Open(demo, theme, size, 470);
            Click(SortButton(demo.Grid, "编号")); Click(SortButton(demo.Grid, "编号")); Click(SortButton(demo.Grid, "设备"), RawInputModifiers.Shift); Flush(); Flush();
            using var bitmap = new RenderTargetBitmap(new PixelSize(960, 470)); bitmap.Render(window);
            var path = Path.Combine(output, $"datagrid-binding-{theme.Key}-{size}.png"); bitmap.Save(path, PngBitmapEncoderOptions.Default);
            Console.WriteLine($"Rendered {path}"); window.Close(); Flush();
        }
    }
    private static Window Open(Control content, ThemeVariant theme, ControlSize size, double height)
    {
        var window = new Window { Width = 960, Height = height, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme, Content = new Border { Padding = new Thickness(16), Child = content } };
        ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); Flush(); return window;
    }
    private static DataGridCollectionView View(DataGrid grid) => (DataGridCollectionView)grid.CollectionView;
    private static DataGridRow[] Rows(DataGrid grid) => grid.GetVisualDescendants().OfType<DataGridRow>().Where(row => row.IsVisible).ToArray();
    private static DataGridSortButton SortButton(DataGrid grid, string title) => grid.GetVisualDescendants().OfType<DataGridColumnHeader>().First(header => Equals(header.Content, title)).GetVisualDescendants().OfType<DataGridSortButton>().Single();
    private static bool Inside(Control child, Control parent)
    {
        var point = child.TranslatePoint(default, parent)!.Value;
        return point.X >= -0.5 && point.Y >= -0.5 && point.X + child.Bounds.Width <= parent.Bounds.Width + 0.5 && point.Y + child.Bounds.Height <= parent.Bounds.Height + 0.5;
    }
    private static void Click(Control target, RawInputModifiers modifiers = RawInputModifiers.None)
    {
        var host = TopLevel.GetTopLevel(target)!; var point = target.TranslatePoint(new Point(target.Bounds.Width / 2, target.Bounds.Height / 2), host)!.Value;
        host.MouseMove(point); host.MouseDown(point, MouseButton.Left, modifiers); host.MouseUp(point, MouseButton.Left, modifiers); Flush();
    }
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
}
