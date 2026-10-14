using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Controls.Shapes;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;
using static TheOne.DesignPreview.InteractionRepairsPreview;

namespace TheOne.DesignPreview;

internal static class DataGridPreview
{
    public static void Verify()
    {
        foreach (var variant in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>()) VerifyDensity(variant, size);
        VerifyInteraction();
        VerifyGroupsAndDefaults();
    }
    private static (Window Window, DataGridDemoView Demo) Open(ThemeVariant variant, ControlSize size, int width = 1060)
    {
        var demo = new DataGridDemoView();
        var window = new Window { Width = width, Height = 700, WindowDecorations = WindowDecorations.None, Content = new Border { Padding = new Thickness(24), Child = demo }, RequestedThemeVariant = variant };
        Motion.SetEnabled(window, false); ControlSizing.SetSize(window, size); window.Show(); ControlPreview.Flush();
        return (window, demo);
    }
    private static DataGridRow[] Rows(DataGrid grid) => grid.GetVisualDescendants().OfType<DataGridRow>().ToArray();
    private static DataGridCell[] Cells(DataGridRow row) => row.GetVisualDescendants().OfType<DataGridCell>().Where(cell => cell.IsEffectivelyVisible).ToArray();
    private static DataGridColumnHeader Header(DataGrid grid, string title) => grid.GetVisualDescendants().OfType<DataGridColumnHeader>().Single(header => Equals(header.Content, title));
    private static void Bounded(DataGrid grid, string label)
    {
        var rows = Rows(grid);
        Check(rows.Length is > 0 and < 150 && grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Count() == 1,
            $"{label}: 10,000 rows retain the native virtual rows presenter and only {rows.Length} row containers");
    }
    private static void VerifyDensity(ThemeVariant variant, ControlSize size)
    {
        var (window, demo) = Open(variant, size); var grid = demo.Grid;
        var label = $"DataGrid/{variant.Key}/{size}";
        var height = size switch { ControlSize.Small => 30, ControlSize.Large => 44, _ => 36 };
        var font = size switch { ControlSize.Small => 12, ControlSize.Large => 16, _ => 14 };
        Bounded(grid, label);
        var first = Rows(grid).First(row => row.Index == 0);
        var header = Header(grid, "编号"); var cell = Cells(first).First();
        Check(grid.ColumnWidth.IsAuto && double.IsNaN(grid.RowHeight) && grid.Columns.All(column => column.Width.IsAuto), $"{label}: automatic column width and content-driven row height are defaults");
        Check(header.HorizontalContentAlignment == HorizontalAlignment.Center && cell.HorizontalContentAlignment == HorizontalAlignment.Center && cell.VerticalContentAlignment == VerticalAlignment.Center, $"{label}: headers and cells are centered by the library theme");
        var text = grid.Columns[0].GetCellContent(first) as TextBlock;
        Check(text is { TextAlignment: TextAlignment.Center, TextWrapping: TextWrapping.Wrap } && text.FontSize == font && header.FontSize == font, $"{label}: native text cells inherit centered wrapping and density typography (text {text?.FontSize}, header {header.FontSize})");
        Check(Math.Abs(header.Bounds.Height - height) < 1 && Math.Abs(first.Bounds.Height - height) < 1, $"{label}: short rows and headers have the matching density (row {first.Bounds.Height}, header {header.Bounds.Height})");
        var longRow = Rows(grid).FirstOrDefault(row => (row.DataContext as GridRecord)?.Id == 6);
        Check(longRow is not null && longRow.Bounds.Height > height && grid.Columns[6].ActualWidth <= 320.5, $"{label}: long content wraps at maximum column width and grows its row without truncation");
        var frame = Part<Border>(grid, "DataGridBorder");
        Check(frame.ClipToBounds && frame.CornerRadius == new CornerRadius(8) && frame.BorderThickness == new Thickness(1) && Colour(frame.Background) == (variant == ThemeVariant.Light ? Color.Parse("#FFFFFF") : Color.Parse("#19211E")), $"{label}: rounded Still surface clips rows beneath the outer frame");
        Hover(window, cell);
        Check(Colour(Part<Rectangle>(first, "BackgroundRectangle").Fill) == Resource(grid, "One.Subtle"), $"{label}: actual pointer hover uses the semantic row background");
        Click(window, cell);
        Check(first.IsSelected && demo.Model.Selected == first.DataContext && Colour(Part<Rectangle>(first, "BackgroundRectangle").Fill) == Resource(grid, "One.AccentSoft"), $"{label}: clicking a cell binds selection with a semantic selection surface");
        demo.Model.FrozenRightCount = 1; ControlPreview.Flush();
        Check(TheOne.Avalonia.Controls.DataGridFeatures.GetFrozenRightColumnCount(grid) == 1 && grid.Columns[6].IsFrozen,
            $"{label}: compiled preview binding enables right freezing");
        var countEditor = demo.GetVisualDescendants().OfType<NumericUpDown>().Single(control => control.Name == "GridFrozenRightCount");
        countEditor.Value = 2; ControlPreview.Flush();
        Check(demo.Model.FrozenRightCount == 2 && grid.Columns[5].IsFrozen && grid.Columns[6].IsFrozen,
            $"{label}: compiled numeric editor changes the right frozen count in both directions");
        demo.Model.FrozenRightCount = 0; ControlPreview.Flush();
        Check(!grid.Columns[6].IsFrozen && grid.Columns[0].IsFrozen, $"{label}: preview reset restores left-only layout");
        demo.Model.ShowSummary = true; ControlPreview.Flush();
        Check(grid.GetVisualDescendants().OfType<TheOne.Avalonia.Controls.DataGridSummaryPresenter>().Single().IsVisible &&
            Equals(TheOne.Avalonia.Controls.DataGridFeatures.GetSummaryResult(grid, grid.Columns[3])?.Value, (decimal)demo.Model.Records.Sum(record => record.Quantity)),
            $"{label}: compiled XAML column metadata automatically supplies numerical statistics");
        demo.Model.ShowSummary = false; ControlPreview.Flush();
        Check(!grid.GetVisualDescendants().OfType<TheOne.Avalonia.Controls.DataGridSummaryPresenter>().Single().IsVisible,
            $"{label}: compiled preview toggle hides the numerical footer");
        window.Close();
    }
    private static void VerifyInteraction()
    {
        var (window, demo) = Open(ThemeVariant.Light, ControlSize.Normal); var grid = demo.Grid;
        var initial = Rows(grid).ToHashSet(ReferenceEqualityComparer.Instance);
        Click(window, demo.FindControl<Button>("GridMiddle")!);
        Check(demo.Model.Selected?.Id == 5001 && Rows(grid).Any(row => row.DataContext == demo.Model.Selected && row.IsVisible), "DataGrid preview middle button selects and realizes row 5,001"); Bounded(grid, "Middle-scroll DataGrid");
        Click(window, demo.FindControl<Button>("GridLast")!);
        Check(demo.Model.Selected?.Id == 10000 && Rows(grid).Any(row => row.DataContext == demo.Model.Selected && row.IsVisible), "DataGrid preview last button selects and realizes row 10,000"); Bounded(grid, "End-scroll DataGrid");
        Click(window, demo.FindControl<Button>("GridFirst")!);
        Check(Rows(grid).Any(row => initial.Contains(row)) && demo.Model.Selected?.Id == 1, "DataGrid recycles the original row containers after distant round-trip scrolling");
        var idHeader = Header(grid, "编号"); Click(window, idHeader); Click(window, idHeader);
        Check(((GridRecord)demo.Model.View[0]).Id == 10000 && Part<global::Avalonia.Controls.Shapes.Path>(idHeader, "SortIcon").IsVisible, "Real header clicks sort ten thousand records descending and show the themed sort indicator");
        demo.Model.ResetCommand.Execute(null); ControlPreview.Flush();
        demo.Model.FirstCommand.Execute(null); ControlPreview.Flush();
        var first = Rows(grid).First(row => row.DataContext == demo.Model.Records[0]);
        var name = grid.Columns[1].GetCellContent(first)!;
        Click(window, name); Press(window, Key.F2, PhysicalKey.F2);
        var editor = first.GetVisualDescendants().OfType<TextBox>().Single(box => box.Name == "CellTextBox");
        Check(editor.TextAlignment == TextAlignment.Center && editor.MinHeight == 0, "F2 opens a centered native editor without inflating row height");
        editor.SelectAll(); window.KeyTextInput("已编辑的设备"); Press(window, Key.Enter, PhysicalKey.Enter);
        Check(demo.Model.Records[0].Name == "已编辑的设备" && !first.GetVisualDescendants().OfType<TextBox>().Any(box => box.Name == "CellTextBox"), "Real keyboard editing commits to the bound data model");
        grid.CurrentColumn = grid.Columns[1]; grid.SelectedItem = demo.Model.Records[0]; ControlPreview.Flush();
        Press(window, Key.F2, PhysicalKey.F2); editor = first.GetVisualDescendants().OfType<TextBox>().Single(box => box.Name == "CellTextBox");
        editor.SelectAll(); window.KeyTextInput("准备撤销"); Press(window, Key.Escape, PhysicalKey.Escape);
        Check(demo.Model.Records[0].Name == "已编辑的设备", "Escape restores the original cell value through native editing semantics");
        Press(window, Key.F2, PhysicalKey.F2); editor = first.GetVisualDescendants().OfType<TextBox>().Single(box => box.Name == "CellTextBox");
        editor.SelectAll(); Press(window, Key.Back, PhysicalKey.Backspace);
        Check(demo.Model.Records[0].HasErrors && DataValidationErrors.GetHasErrors(editor) && !grid.CommitEdit(), "Invalid cell input displays native validation and blocks row commitment");
        Check(Colour(Part<Border>(editor, "PART_BorderElement").BorderBrush) == Resource(grid, "One.Danger"), "Invalid editor uses the shared semantic danger outline");
        Press(window, Key.Escape, PhysicalKey.Escape); grid.CancelEdit(DataGridEditingUnit.Row); ControlPreview.Flush();
        Check(!demo.Model.Records[0].HasErrors && demo.Model.Records[0].Name == "已编辑的设备", "Canceling invalid input restores the model and clears validation");
        demo.Model.IsReadOnly = true; ControlPreview.Flush();
        Check(!grid.BeginEdit(), "Read-only preview rejects native editing"); demo.Model.IsReadOnly = false;
        var nameHeader = Header(grid, "设备名称"); var oldWidth = grid.Columns[1].ActualWidth;
        var edge = nameHeader.TranslatePoint(new Point(nameHeader.Bounds.Width - 2, nameHeader.Bounds.Height / 2), window)!.Value;
        window.MouseMove(edge); window.MouseDown(edge, MouseButton.Left); window.MouseMove(edge + new Vector(30, 0)); window.MouseUp(edge + new Vector(30, 0), MouseButton.Left); ControlPreview.Flush();
        Check(grid.Columns[1].ActualWidth > oldWidth + 20 && !grid.Columns[1].Width.IsAuto, "Real header-edge dragging resizes a native column");
        var start = nameHeader.TranslatePoint(new Point(nameHeader.Bounds.Width / 2, nameHeader.Bounds.Height / 2), window)!.Value;
        var destinationHeader = Header(grid, "数量");
        var destination = destinationHeader.TranslatePoint(new Point(destinationHeader.Bounds.Width - 8, destinationHeader.Bounds.Height / 2), window)!.Value;
        var oldIndex = grid.Columns[1].DisplayIndex;
        window.MouseMove(start); window.MouseDown(start, MouseButton.Left); window.MouseMove(start + new Vector(12, 0)); ControlPreview.Flush(); window.MouseMove(destination); window.MouseUp(destination, MouseButton.Left); ControlPreview.Flush();
        Check(grid.Columns[1].DisplayIndex > oldIndex && grid.Columns[0].DisplayIndex == 0, "Real header dragging reorders non-frozen columns and preserves the frozen identifier");
        var vertical = Part<ScrollBar>(grid, "PART_VerticalScrollbar"); var offset = vertical.Value;
        Wheel(window, Part<DataGridRowsPresenter>(grid, "PART_RowsPresenter"), -5);
        Check(vertical.Value > offset && Rows(grid).Length < 150, "Actual mouse wheel scrolling advances a bounded native row viewport");
        window.Width = 680; ControlPreview.Flush();
        grid.ScrollIntoView(demo.Model.Records[0], grid.Columns[^1]); ControlPreview.Flush();
        Check(Part<ScrollBar>(grid, "PART_HorizontalScrollbar").Value > 0 && grid.FrozenColumnCount == 1, "Horizontal navigation retains the frozen identifier column");
        window.Width = 1060; ControlPreview.Flush();
        demo.Model.FilterText = "上海仓"; Click(window, demo.FindControl<Button>("GridApplyFilter")!);
        Check(demo.Model.View.Count == 2500 && demo.Model.View.Cast<GridRecord>().All(row => row.Warehouse == "上海仓"), "Real preview filter button filters the ten-thousand-record view"); Bounded(grid, "Filtered DataGrid");
        demo.Model.ResetCommand.Execute(null); ControlPreview.Flush();
        Check(demo.Model.View.Count == 10000, "Reset restores all records without materializing ten thousand rows");
        ControlSizing.SetSize(window, ControlSize.Large); ControlPreview.Flush();
        Check(Header(grid, "编号").FontSize == 16 && Rows(grid).First(row => row.Index == 0).Bounds.Height >= 44, "Runtime density changes update existing headers and recycled rows");
        window.RequestedThemeVariant = ThemeVariant.Dark; ControlPreview.Flush();
        Check(Colour(Part<Border>(grid, "DataGridBorder").Background) == Color.Parse("#19211E"), "Open DataGrid changes its actual surface with the live theme");
        demo.UpdateMetrics(); Check(demo.Model.Metrics.Contains("/ 10,000"), "Preview exposes actual row and cell container metrics"); window.Close();
    }
    private static void VerifyGroupsAndDefaults()
    {
        // A manual grid must work without preview-only sizing values or style includes.
        var grid = new DataGrid { ItemsSource = DataGridDemoViewModel.CreateRecords(), AutoGenerateColumns = false };
        grid.Columns.Add(new DataGridTextColumn { Header = "默认列", Binding = global::Avalonia.Data.CompiledBinding.Create<GridRecord, string>(record => record.Name) });
        var window = new Window { Width = 500, Height = 320, Content = grid, WindowDecorations = WindowDecorations.None };
        Motion.SetEnabled(window, false); window.Show(); ControlPreview.Flush();
        Check(grid.ColumnWidth.IsAuto && Header(grid, "默认列").HorizontalContentAlignment == HorizontalAlignment.Center, "A consumer grid receives native automatic sizing and centered themes through OneTheme alone");
        Bounded(grid, "Plain consumer DataGrid"); window.Close();
        var (groupWindow, demo) = Open(ThemeVariant.Dark, ControlSize.Small);
        demo.Model.View.GroupDescriptions.Add(new global::Avalonia.Collections.DataGridPathGroupDescription(nameof(GridRecord.Warehouse))); ControlPreview.Flush();
        var group = demo.Grid.GetVisualDescendants().OfType<DataGridRowGroupHeader>().First();
        var toggle = Part<ToggleButton>(group, "PART_ExpanderButton"); var expanded = toggle.IsChecked;
        Check(group.Bounds.Height >= 30 && Colour(group.Background) == Resource(demo.Grid, "One.Subtle"), "Grouped DataGrid uses the library row-group theme and inherited density");
        Click(groupWindow, toggle);
        Check(toggle.IsChecked != expanded && Rows(demo.Grid).Length < 150, "Native group expander toggles a large branch without materializing the grouped records");
        groupWindow.Close();
    }
    public static void VerifyGallery(LegacyPreviewWindow window, LegacyControlGallery gallery)
    {
        window.NavigateToCategory(10); ControlPreview.Flush();
        var card = gallery.FindControl<Border>("DataGridCard")!;
        Check(card.IsVisible && card.Child is DataGridDemoView, "DataGrid catalogue page creates its ten-thousand-row interactive preview on demand");
        var demo = (DataGridDemoView)card.Child!;
        Click(window, demo.FindControl<Button>("GridLast")!);
        Check(demo.Model.Selected?.Id == 10000 && Rows(demo.Grid).Any(row=> row.DataContext == demo.Model.Selected), "Catalogue DataGrid last-row button works through real pointer input");
        Bounded(demo.Grid, "Catalogue DataGrid");
        window.FindControl<ComboBox>("SizePicker")!.SelectedIndex = 0; ControlPreview.Flush();
        Check(Header(demo.Grid, "编号").Bounds.Height == 30, "Top-level preview density picker controls the DataGrid header and cells");
        window.FindControl<ComboBox>("SizePicker")!.SelectedIndex = 1; ControlPreview.Flush();
        window.NavigateToCategory(0); gallery.Search("DataGrid"); ControlPreview.Flush();
        Check(card.IsVisible, "Global DataGrid search also reveals its interactive preview"); gallery.Search(""); ControlPreview.Flush();
    }
    public static void Render(string output)
    {
        foreach (var variant in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var (window, demo) = Open(variant, size, 1180); demo.Model.FirstCommand.Execute(null); ControlPreview.Flush(); demo.UpdateMetrics();
            EnhancementsPreview.Save(window, System.IO.Path.Combine(output, $"datagrid-{variant.Key!.ToString()!.ToLowerInvariant()}-{size.ToString().ToLowerInvariant()}.png")); window.Close();
        }
    }
    private static void Press(TopLevel window, Key key, PhysicalKey physical) { window.KeyPress(key, RawInputModifiers.None, physical, null); window.KeyRelease(key, RawInputModifiers.None, physical, null); ControlPreview.Flush(); }
    private static Color? Colour(IBrush? brush) => (brush as ISolidColorBrush)?.Color;
    private static Color? Resource(Control control, string key) => control.TryFindResource(key, control.ActualThemeVariant, out var value) ? Colour(value as IBrush) : null;
}
