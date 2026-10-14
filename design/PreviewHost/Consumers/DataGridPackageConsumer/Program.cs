using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;
using TheOne.Avalonia.Themes;

namespace TheOne.DataGridPackageConsumer;

public sealed class ConsumerApp : Application { public override void Initialize() => Styles.Add(new OneTheme()); }
internal static class Program
{
    private static int _checks;
    private static bool _desktop;
    [STAThread]
    public static void Main(string[] args)
    {
        Console.WriteLine($"Platform: {RuntimeInformation.RuntimeIdentifier}; dynamic code: {RuntimeFeature.IsDynamicCodeSupported}");
        if (args.Contains("--require-aot") && RuntimeFeature.IsDynamicCodeSupported) throw new InvalidOperationException("Native executable required.");
        var nativeAssembly = typeof(DataGrid).Assembly.GetName();
        var mainAssembly = typeof(OneTheme).Assembly.GetName();
        if (nativeAssembly.Name != "Avalonia.Controls.DataGrid" || nativeAssembly.Version != new Version(12, 1, 2, 0) ||
            Convert.ToHexString(nativeAssembly.GetPublicKeyToken() ?? []).ToLowerInvariant() != "c8d484a7012f9a8b")
            throw new InvalidOperationException("Vendor package must preserve its upstream assembly identity.");
        if (mainAssembly.Version != new Version(1, 0, 0, 0)) throw new InvalidOperationException("Main assembly identity changed with a package-only release.");
        _checks += 2;
        Console.WriteLine($"ASSEMBLY {mainAssembly.FullName}; {nativeAssembly.FullName}");
        _desktop = args.Contains("--desktop");
        var builder = AppBuilder.Configure<ConsumerApp>();
        if (_desktop) builder.UsePlatformDetect();
        else builder.UseSkia().UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false });
        builder.SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime { ShutdownMode = ShutdownMode.OnExplicitShutdown });
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>()) Verify(theme, size);
        Console.WriteLine($"PASS: {_checks} packaged DataGrid consumer checks ({(_desktop ? "desktop" : "Skia headless")}).");
    }
    private static void Verify(ThemeVariant theme, ControlSize size)
    {
        var model = new Inventory(); var content = new ConsumerView { DataContext = model }; var grid = content.FindControl<DataGrid>("Grid")!;
        var window = new Window { Content = content, Width = 760, Height = 430, RequestedThemeVariant = theme, WindowDecorations = WindowDecorations.None };
        ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false);
        var created = new HashSet<DataGridRow>(ReferenceEqualityComparer.Instance); grid.LoadingRow += (_, args) => created.Add(args.Row);
        window.Show(); Flush(window);
        Check(grid.Columns.All(c => DataGridColumnOptions.GetValueAccessor(c) is null), "XAML compiled entity fields need no column accessor");
        Check(grid.GetVisualDescendants().OfType<DataGridSummaryPresenter>().Single().IsVisible, "packaged OneTheme supplies summary and header templates");
        Check(grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Any() && Rows().Length is > 0 and < 100, "1,000 items retain native bounded row virtualization");
        Check(grid.Columns[0].IsFrozen && grid.Columns[5].IsFrozen && grid.Columns[6].IsFrozen && !grid.Columns[3].IsFrozen, "both frozen regions are provided by packaged native layout");
        AssertSummary(model.Source);
        Func<object, bool> business = item => ((StockRow)item).Id <= 950; model.View.Filter = business; Flush(window);
        Check(DataGridFeatures.ApplyFilter(grid, grid.Columns[1], new TextFilterCondition("equipment")).Succeeded, "automatic text filter accepts a compiled XAML property");
        Check(DataGridFeatures.ApplyFilter(grid, grid.Columns[3], new NumericFilterCondition<int>(NumericFilterMode.Range, minimum: 5, maximum: 12)).Succeeded, "automatic numeric range combines with text and business filter"); Flush(window);
        var matching = Matching(); Check(model.View.Count == matching.Length && matching.Length > 0, "three predicates combine across local source"); AssertSummary(matching);
        model.View.PageSize = 10; Flush(window); AssertSummary(matching); model.View.PageSize = 0;
        model.Source[899].Quantity = 8; Flush(window); AssertSummary(Matching());
        var added = new StockRow(900) { Quantity = 7 }; model.Source.Add(added); Flush(window); AssertSummary(Matching());
        model.Source.Remove(added); Flush(window); AssertSummary(Matching());
        model.View.SortDescriptions.Add(DataGridSortDescription.FromPath(nameof(StockRow.Price), ListSortDirection.Descending));
        model.View.GroupDescriptions.Add(new DataGridPathGroupDescription(nameof(StockRow.Category))); Flush(window); AssertSummary(Matching());
        Check(Rows().Length < 100 && model.View.Groups is { Count: > 0 }, "grouping and sorting keep virtual rows and aggregates");
        model.View.GroupDescriptions.Clear(); model.View.SortDescriptions.Clear();
        DataGridFeatures.ClearAllFilters(grid); Flush(window);
        Check(model.View.Count == 950 && ReferenceEquals(model.View.Filter, business), "clear removes only column conditions and restores business filter");
        model.View.Filter = null; Flush(window);
        var beforeJump = created.Count;
        grid.SelectedItem = model.Source[^1]; grid.ScrollIntoView(model.Source[^1], grid.Columns[6]); Flush(window);
        Console.WriteLine($"CONTAINERS {theme.Key}/{size}: realized {Rows().Length}, new on distant jump {created.Count - beforeJump}");
        Check(Rows().Any(row => row.DataContext == model.Source[^1]) && Rows().Length < 100 && created.Count - beforeJump < 100, "thousandth row is selected using recycled native containers");
        grid.SelectedItem = model.Source[0]; grid.ScrollIntoView(model.Source[0], grid.Columns[3]); grid.CurrentColumn = grid.Columns[3]; grid.Focus(); Flush(window);
        Check(grid.BeginEdit(), "packaged compiled binding retains native editing"); Flush(window);
        var editor = Rows().First(row => row.DataContext == model.Source[0]).GetVisualDescendants().OfType<TextBox>().Single(box => box.Name == "CellTextBox");
        editor.Text = "27"; Flush(window);
        Check(grid.CommitEdit() && model.Source[0].Quantity == 27, "edit commit reaches the consumer entity"); Flush(window); AssertSummary(model.Source);
        var lastX = grid.GetColumnLayout(grid.Columns[6], 36).Bounds.X;
        grid.ScrollIntoView(model.Source[0], grid.Columns[4]); Flush(window);
        Check(Math.Abs(grid.GetColumnLayout(grid.Columns[6], 36).Bounds.X - lastX) < 1, "right summary and headers stay pinned while middle scrolls");
        var right = grid.Columns[6]; var neighbor = grid.Columns[5]; var original = right.DisplayIndex;
        DataGridColumnHeader Header(DataGridColumn column) => grid.GetVisualDescendants().OfType<DataGridColumnHeader>().Single(h => Equals(h.Content, column.Header) && !h.Classes.Contains(":dragIndicator"));
        if (!_desktop)
        {
            var start = Header(right).TranslatePoint(new Point(30, 14), window)!.Value;
            var end = Header(neighbor).TranslatePoint(new Point(15, 14), window)!.Value;
            window.MouseMove(start); window.MouseDown(start, MouseButton.Left); window.MouseMove(start - new Vector(12, 0)); Flush(window);
            Check(Header(right).Classes.Contains(":dragging") && grid.GetVisualDescendants().OfType<DataGridDropIndicator>().Any(), "drag provides dimmed source, ghost card and native insertion caret");
            window.MouseMove(end); window.MouseUp(end, MouseButton.Left); Flush(window);
            Check(right.DisplayIndex == original - 1 && !Header(right).Classes.Contains(":dragging"), "right-region pointer drop commits and clears feedback");
        }
        right.DisplayIndex = original; Flush(window); AssertSummary(model.Source);
        window.Width = 260; Flush(window);
        Check(grid.Columns.All(c => grid.GetColumnLayout(c, 36).VisibleBounds.Width >= 0) && Rows().Length < 100, "compact viewport retains finite native clipping and recycling");
        window.Close(); Flush(window);
        StockRow[] Matching() => model.Source.Where(row => row.Id <= 950 && row.Name.Contains("equipment", StringComparison.OrdinalIgnoreCase) && row.Quantity is >= 5 and <= 12).ToArray();
        DataGridRow[] Rows() => grid.GetVisualDescendants().OfType<DataGridRow>().ToArray();
        void AssertSummary(IEnumerable<StockRow> source)
        {
            var rows = source.ToArray();
            Check(Equals(DataGridFeatures.GetSummaryResult(grid, grid.Columns[3])?.Value, (decimal)rows.Sum(row => row.Quantity)) &&
                  Equals(DataGridFeatures.GetSummaryResult(grid, grid.Columns[4])?.Value, rows.Average(row => row.Price)) &&
                  Equals(DataGridFeatures.GetSummaryResult(grid, grid.Columns[5])?.Value, rows.Max(row => row.Price)) &&
                  Equals(DataGridFeatures.GetSummaryResult(grid, grid.Columns[6])?.Value, (long)rows.Length), "filtered / paged / edited / regrouped statistics retain source precision and column identity");
        }
        void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException($"{theme.Key}/{size}: {message}"); _checks++; }
    }
    private static void Flush(Window window)
    {
        Dispatcher.UIThread.RunJobs(); window.UpdateLayout();
        if (!_desktop) AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs(); window.UpdateLayout();
    }
}
