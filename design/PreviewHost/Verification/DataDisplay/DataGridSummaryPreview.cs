using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static partial class DataGridSummaryPreview
{
    public static void Verify(Action<bool, string> check)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
            VerifyProfile(theme, size, check);
        VerifyNumbers(check);
        VerifyNested(check);
    }

    private static (Window Window, DataGridStage1Fixture Fixture, DataGridCollectionView View,
        ObservableCollection<GridRecord> Source) Open(ThemeVariant theme, ControlSize size)
    {
        var fixture = new DataGridStage1Fixture();
        var grid = fixture.Grid;
        var source = new ObservableCollection<GridRecord>(fixture.Records);
        var view = new DataGridCollectionView(source);
        grid.ItemsSource = view;
        var count = new DataGridTextColumn
        {
            Header = "有效数值", Width = new DataGridLength(110),
            Binding = CompiledBinding.Create<GridRecord, decimal>(row => row.Price)
        };
        grid.Columns.Add(count);
        Configure(grid.Columns[0], DataGridSummaryKind.Min);
        Configure(grid.Columns[3], DataGridSummaryKind.Sum);
        Configure(grid.Columns[4], DataGridSummaryKind.Average, "0.00");
        Configure(grid.Columns[6], DataGridSummaryKind.Max, "0.00");
        Configure(count, DataGridSummaryKind.Count);
        DataGridFeatures.SetFrozenRightColumnCount(grid, 2);
        DataGridFeatures.SetShowColumnFilters(grid, true);
        DataGridFeatures.SetShowSummaryRow(grid, true);
        var window = new Window
        {
            Width = 940, Height = 480, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
            Content = grid
        };
        ControlSizing.SetSize(window, size);
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        return (window, fixture, view, source);
    }

    private static void Configure(DataGridColumn column, DataGridSummaryKind kind, string? format = null)
    {
        DataGridColumnOptions.SetSummaryKind(column, kind);
        DataGridColumnOptions.SetSummaryFormat(column, format);
        DataGridColumnOptions.SetSummaryEnabled(column, true);
    }

    private static void VerifyProfile(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var (window, fixture, view, source) = Open(theme, size);
        var grid = fixture.Grid;
        var label = $"DataGrid/summary/{theme.Key}/{size}";

        void Expect(bool success, string message)
        {
            check(success, label + ": " + message);
        }

        DataGridSummaryResult Result(int index)
        {
            return DataGridFeatures.GetSummaryResult(grid, grid.Columns[index])!;
        }

        var footer = grid.GetVisualDescendants().OfType<DataGridSummaryPresenter>().Single();
        Expect(
            footer.IsVisible && footer.Bounds.Height ==
            (size == ControlSize.Small ? 30 : size == ControlSize.Large ? 44 : 36),
            "one independent footer inherits the three density heights");
        Expect(!footer.IsHitTestVisible && !footer.Focusable && fixture.Rows.Length < 100 && view.Count == 1000,
            "footer does not become a data row, selection target or editor; actual realization remains bounded");
        Expect(grid.Columns.All(column => DataGridColumnOptions.GetValueAccessor(column) is null),
            "table filtering and summaries automatically register compiled entity properties without column accessors");
        Expect(
            Equals(Result(0).Value, 1) && Equals(Result(3).Value, (decimal)source.Sum(row => row.Quantity)) &&
            Equals(Result(4).Value, source.Average(row => row.Price)) &&
            Equals(Result(6).Value, source.Max(row => row.Price)) && Equals(Result(7).Value, 1000L),
            "Min / Sum / Average / Max / valid Count use all source models");
        var beforeX = grid.GetColumnLayout(grid.Columns[7], footer.Bounds.Height).Bounds.X;
        grid.ScrollIntoView(source[0], grid.Columns[5]);
        Flush();
        Expect(Math.Abs(grid.GetColumnLayout(grid.Columns[7], footer.Bounds.Height).Bounds.X - beforeX) < 1,
            "summary right column shares the native pinned position during scrolling");
        var aligned = true;
        foreach (var column in grid.Columns.Where(c => c.IsVisible))
        {
            var header = grid.GetVisualDescendants().OfType<DataGridColumnHeader>()
                .Single(h => Equals(h.Content, column.Header));
            aligned &= Math.Abs(header.TranslatePoint(default, footer)!.Value.X -
                                grid.GetColumnLayout(column, footer.Bounds.Height).Bounds.X) < 1;
        }

        Expect(aligned, "footer geometry matches every actual native header and frozen region");
        var filtered = source.Where(row => row.Quantity >= 5 && row.Quantity <= 10).ToArray();
        Expect(
            DataGridFeatures.ApplyFilter(grid, grid.Columns[3],
                new NumericFilterCondition<int>(NumericFilterMode.Range, minimum: 5, maximum: 10)).Succeeded,
            "automatically inferred numeric property supports range filtering");
        Flush();
        Expect(
            Result(3).ValidCount == filtered.Length &&
            Equals(Result(3).Value, (decimal)filtered.Sum(row => row.Quantity)),
            "filtered summary combines column predicates");
        view.PageSize = 10;
        Flush();
        Expect(view.Count <= 10 && Result(7).ValidCount == filtered.Length && filtered.Length > 10,
            "FilteredItems summary reads every local matching model before paging");
        view.PageSize = 0;
        Flush();
        DataGridFeatures.SetSummaryScope(grid, DataGridSummaryScope.SourceItems);
        Flush();
        Expect(Result(7).ValidCount == 1000 && Equals(Result(3).Value, (decimal)source.Sum(row => row.Quantity)),
            "SourceItems explicitly includes rows outside the current filter");
        DataGridFeatures.SetSummaryScope(grid, DataGridSummaryScope.FilteredItems);
        Flush();
        VerifyDragFeedback(window, grid, check);
        var distant = source[900];
        distant.Quantity = 7;
        Flush();
        Expect(
            Equals(Result(3).Value,
                (decimal)source.Where(row => row.Quantity is >= 5 and <= 10).Sum(row => row.Quantity)),
            "unrealized model property changes update filtered aggregates");
        var added = new GridRecord(1001, "新增", "上海仓", 7, 3m, true, "");
        source.Add(added);
        Flush();
        Expect(Result(7).ValidCount == source.Count(row => row.Quantity is >= 5 and <= 10),
            "source insertion updates statistics");
        source.Remove(added);
        Flush();
        Expect(Result(7).ValidCount == source.Count(row => row.Quantity is >= 5 and <= 10),
            "source removal updates statistics");
        DataGridFeatures.ClearAllFilters(grid);
        Flush();
        grid.SelectedItem = source[^1];
        grid.ScrollIntoView(source[^1], grid.Columns[6]);
        Flush();
        Expect(
            fixture.Rows.Any(row => row.DataContext == source[^1]) && fixture.Rows.Length < 100 &&
            fixture.CreatedRows < 100,
            "enabled summary and both frozen regions preserve thousandth-row realization and recycling");
        grid.SelectedItem = source[0];
        grid.ScrollIntoView(source[0], grid.Columns[6]);
        grid.CurrentColumn = grid.Columns[6];
        grid.Focus();
        Flush();
        Expect(grid.BeginEdit(), "summary-enabled right frozen cell retains native editing");
        Flush();
        var editor = fixture.Rows.First(row => row.DataContext == source[0]).GetVisualDescendants().OfType<TextBox>()
            .Single(box => box.Name == "CellTextBox");
        editor.Focus();
        editor.SelectAll();
        window.KeyTextInput("42.5");
        Flush();
        Expect(grid.CommitEdit() && source[0].Price == 42.5m, "native edit commit updates the bound numerical model");
        Flush();
        Expect(Equals(Result(4).Value, source.Average(row => row.Price)),
            "summary recalculates after native editing commits");
        grid.Columns[3].IsVisible = false;
        Flush();
        Expect(DataGridFeatures.GetSummaryResult(grid, grid.Columns[3]) is null,
            "hidden columns pause their aggregate result");
        grid.Columns[3].IsVisible = true;
        Flush();
        Expect(Result(3).ValidCount == 1000, "restoring a hidden column recomputes current models");
        grid.Columns[3].DisplayIndex = 4;
        Flush();
        Expect(Equals(Result(3).Value, (decimal)source.Sum(row => row.Quantity)),
            "column reordering keeps aggregate identity");
        grid.Columns[3].Width = new DataGridLength(140);
        Flush();
        Expect(Math.Abs(grid.GetColumnLayout(grid.Columns[3], footer.Bounds.Height).Bounds.Width - 140) < 1,
            "footer width follows native column resize");
        Configure(grid.Columns[1], DataGridSummaryKind.Sum);
        Flush();
        Expect(DataGridFeatures.GetSummaryResult(grid, grid.Columns[1])?.Error is not null,
            "text fields reject numerical aggregation with a configuration error");
        DataGridColumnOptions.SetSummaryEnabled(grid.Columns[1], false);
        DataGridColumnOptions.SetSummaryFormat(grid.Columns[4], "Q");
        Flush();
        Expect(Result(4).Error is not null, "invalid numeric format is reported without displaying a false value");
        DataGridColumnOptions.SetSummaryFormat(grid.Columns[4], "0.00");
        view.Culture = CultureInfo.GetCultureInfo("de-DE");
        Flush();
        Expect(Result(4).Error is null, "culture changes retain valid typed results and formatting");
        grid.HeadersVisibility = DataGridHeadersVisibility.All;
        Flush();
        Expect(grid.LayoutRowHeaderWidth > 0,
            "summary label owns the native row-header slot without replacing the first numerical result");
        window.Width = 220;
        Flush();
        Expect(
            grid.Columns.All(c => grid.GetColumnLayout(c, footer.Bounds.Height).VisibleBounds.Width >= 0) &&
            footer.Bounds.Width > 0, "narrow viewport keeps finite nonoverlapping footer clipping");
        window.Width = 940;
        Flush();
        var updates = 0;
        grid.AddHandler(DataGridFeatures.SummaryUpdatedEvent, (_, _) => updates++);
        DataGridFeatures.SetShowSummaryRow(grid, false);
        Flush();
        var stopped = updates;
        source[0].Quantity++;
        Flush();
        Expect(
            !footer.IsVisible && DataGridFeatures.GetSummaryResult(grid, grid.Columns[3]) is null && updates == stopped,
            "turning off statistics stops subscriptions and computation while retaining column configuration");
        DataGridFeatures.SetShowSummaryRow(grid, true);
        Flush();
        Expect(footer.IsVisible && Equals(Result(3).Value, (decimal)source.Sum(row => row.Quantity)),
            "reopening summaries uses the latest source data");
        var replacement = new ObservableCollection<GridRecord>(source.Take(10));
        grid.ItemsSource = replacement;
        Flush();
        Expect(Result(7).ValidCount == 10, "ItemsSource replacement reconnects to the new entity collection");
        window.Content = null;
        Flush();
        var detached = updates;
        replacement[0].Quantity++;
        Flush();
        Expect(updates == detached, "detached footer releases source listeners");
        window.Content = grid;
        Flush();
        Expect(Equals(Result(3).Value, (decimal)replacement.Sum(row => row.Quantity)),
            "reattachment recalculates current models");
        window.Close();
        Flush();
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }
}