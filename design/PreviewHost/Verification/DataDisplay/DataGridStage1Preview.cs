using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class DataGridStage1Preview
{
    public static void Verify(Action<bool, string> check)
    {
        check(typeof(DataGridCellsPresenter).IsSealed, "DataGrid/stage1: native cells presenter is sealed; public subclassing cannot replace its arrangement");
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var fixture = new DataGridStage1Fixture();
            var grid = fixture.Grid;
            var window = new Window { Width = 640, Height = 360, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme, Content = grid };
            ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); Flush();
            var label = $"DataGrid/stage1/{theme.Key}/{size}";
            var initialRows = fixture.Rows.ToHashSet(ReferenceEqualityComparer.Instance);
            check(fixture.Rows.Length is > 1 and < 100 && grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Count() == 1,
                $"{label}: 1,000 records keep one official rows presenter and {fixture.Rows.Length} realized rows");
            check(fixture.Rows.All(row => row.GetVisualDescendants().OfType<DataGridCellsPresenter>().Count() == 1),
                $"{label}: every realized row retains the official cells presenter");
            var originalRightX = fixture.RightHeader.TranslatePoint(default, grid)!.Value.X;
            check(originalRightX > grid.Bounds.Width && fixture.VisibleRightCells < fixture.Rows.Length,
                $"{label}: native layout does not display the rightmost column initially ({fixture.VisibleRightCells}/{fixture.Rows.Length} right cells)");
            fixture.ApplyPositionOnlyTrial(); Flush();
            var trialRightX = fixture.RightHeader.TranslatePoint(default, grid)!.Value.X;
            check(trialRightX < grid.Bounds.Width && fixture.VisibleRightCells < fixture.Rows.Length,
                $"{label}: public positioning moves the header into view but leaves native right cells missing; position-only approach rejected");
            fixture.ClearTrial(); Flush();
            var previousOffset = fixture.HorizontalScrollBar.Value;
            grid.ScrollIntoView(fixture.Records[0], fixture.RightColumn); Flush();
            check(fixture.HorizontalScrollBar.Value > previousOffset && fixture.VisibleRightCells == fixture.Rows.Length,
                $"{label}: native ScrollIntoView still scrolls horizontally to the proposed right-frozen column");
            fixture.ApplyPositionOnlyTrial(); Flush();
            var pinnedX = fixture.RightHeader.TranslatePoint(default, grid)!.Value.X;
            var scrollPoint = fixture.RowsPresenter.TranslatePoint(new Point(240, 20), window)!.Value;
            window.MouseWheel(scrollPoint, new Vector(2, 0)); Flush();
            check(Math.Abs(fixture.RightHeader.TranslatePoint(default, grid)!.Value.X - pinnedX) > 20,
                $"{label}: native scroll changes the positioned header again; a one-time offset is not a freezing contract");
            fixture.ClearTrial();
            grid.SelectedItem = fixture.Records[^1]; grid.ScrollIntoView(fixture.Records[^1], fixture.RightColumn); Flush();
            check(fixture.Rows.Any(row => row.DataContext == fixture.Records[^1]) && fixture.Rows.Length < 100,
                $"{label}: distant thousandth-row selection is realized with bounded containers");
            grid.SelectedItem = fixture.Records[0]; grid.ScrollIntoView(fixture.Records[0], fixture.RightColumn); Flush();
            check(fixture.Rows.Any(row => initialRows.Contains(row)) && fixture.CreatedRows < 100,
                $"{label}: round-trip scrolling recycles rows ({fixture.CreatedRows} total created)");
            var record = fixture.Records[0];
            grid.CurrentColumn = fixture.RightColumn; grid.Focus(); Flush();
            check(grid.BeginEdit(), $"{label}: official rightmost-column editing remains available");
            Flush();
            var editor = fixture.Rows.First(row => row.DataContext == record).GetVisualDescendants().OfType<TextBox>().Single(box => box.Name == "CellTextBox");
            editor.Focus(); editor.SelectAll(); window.KeyTextInput("123.5"); Flush();
            check(grid.CommitEdit() && record.Price == 123.5m, $"{label}: native numeric editing writes through the compiled column binding");
            window.Close(); Flush();
        }
    }

    private static void Flush()
    {
        global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();
    }

    public static void Render(string output)
    {
        Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var view = new DataGridStage1View();
            var window = new Window { Width = 720, Height = 520, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme, Content = view };
            ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); Flush();
            foreach (var trial in new[] { false, true })
            {
                if (trial) view.Fixture.ApplyPositionOnlyTrial();
                Flush();
                using var bitmap = window.CaptureRenderedFrame()!;
                var path = Path.Combine(output, $"datagrid-stage1-{theme.Key}-{size}-{(trial ? "position-only" : "native")}.png");
                bitmap.Save(path, PngBitmapEncoderOptions.Default); Console.WriteLine($"Rendered {path}");
            }
            window.Close(); Flush();
        }
    }
}
