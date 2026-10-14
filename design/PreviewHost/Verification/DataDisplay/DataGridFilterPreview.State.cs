using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static partial class DataGridFilterPreview
{
    private static void VerifyLifetime(Action<bool, string> check)
    {
        var (window, fixture, view) = Open(ThemeVariant.Light, ControlSize.Normal); var grid = fixture.Grid;
        var name = grid.Columns[1]; var quantity = grid.Columns[3]; var business = view.Filter;
        var changed = new List<DataGridFilterChangedEventArgs>();
        grid.AddHandler(DataGridFeatures.FilterChangedEvent, (_, args) => changed.Add(args));
        var first = new TextFilterCondition("000"); DataGridFeatures.ApplyFilter(grid, name, first);
        var second = new TextFilterCondition("0001"); DataGridFeatures.ApplyFilter(grid, name, second);
        check(changed.Count == 2 && Equals(changed[1].OldCondition, first) && Equals(changed[1].NewCondition, second), "DataGrid/filters/state: events expose the actual previous and new conditions");
        DataGridFeatures.ApplyFilter(grid, quantity, new NumericFilterCondition<int>(NumericFilterMode.Range, minimum: 5));
        EventHandler<DataGridFilterChangingEventArgs> cancel = (_, args) => { if (args.Column == quantity && args.NewCondition is null) args.Cancel = true; };
        grid.AddHandler(DataGridFeatures.FilterChangingEvent, cancel);
        var ids = view.Cast<GridRecord>().Select(row => row.Id).ToArray();
        check(!DataGridFeatures.ClearAllFilters(grid).Succeeded && DataGridColumnOptions.GetFilterCondition(name) == second && view.Cast<GridRecord>().Select(row => row.Id).SequenceEqual(ids),
            "DataGrid/filters/state: cancelling any clear-all event keeps the entire operation atomic");
        grid.RemoveHandler(DataGridFeatures.FilterChangingEvent, cancel); DataGridFeatures.ClearAllFilters(grid); Flush();
        check(ReferenceEquals(view.Filter, business), "DataGrid/filters/state: clearing columns restores the original business predicate by identity");
        view.SortDescriptions.Add(DataGridSortDescription.FromPath(nameof(GridRecord.Id), ListSortDirection.Descending));
        view.GroupDescriptions.Add(new DataGridPathGroupDescription(nameof(GridRecord.Warehouse)));
        DataGridFeatures.ApplyFilter(grid, quantity, new NumericFilterCondition<int>(NumericFilterMode.Range, maximum: 10)); Flush();
        check(ReferenceEquals(grid.ItemsSource, view) && view.SortDescriptions.Count == 1 && view.GroupDescriptions.Count == 1,
            "DataGrid/filters/state: grouping and sort descriptions survive filtering on the original view");
        view.GroupDescriptions.Clear(); view.SortDescriptions.Clear();
        var external = new Func<object, bool>(item => ((GridRecord)item).Id > 500);
        view.Filter = external; Flush();
        check(view.Cast<GridRecord>().All(row => row.Id > 500 && row.Quantity <= 10), "DataGrid/filters/state: a later business filter is recomposed with active column conditions");
        DataGridFeatures.ClearAllFilters(grid); Flush(); check(ReferenceEquals(view.Filter, external), "DataGrid/filters/state: clearing restores the most recently supplied business filter");
        DataGridFeatures.ApplyFilter(grid, quantity, new NumericFilterCondition<int>(NumericFilterMode.Exact, 79)); Flush();
        fixture.Records[999].Quantity = 79; Flush();
        check(view.Cast<GridRecord>().Contains(fixture.Records[999]), "DataGrid/filters/state: a property change on an unrealized source row updates filtered membership");
        var source = (ObservableCollection<GridRecord>)view.SourceCollection;
        var added = new GridRecord(1001, "new", "上海仓", 79, 9m, true, ""); source.Add(added); Flush();
        check(view.Cast<GridRecord>().Contains(added), "DataGrid/filters/state: observable insertions enter the filtered view");
        source.Remove(added); Flush(); check(!view.Cast<GridRecord>().Contains(added), "DataGrid/filters/state: removals drop membership and release row observation");
        var replacement = new DataGridCollectionView(fixture.Records) { Filter = item => ((GridRecord)item).Id <= 100 };
        grid.ItemsSource = replacement; Flush();
        check(ReferenceEquals(view.Filter, external) && replacement.Cast<GridRecord>().All(row => row.Id <= 100 && row.Quantity == 79),
            "DataGrid/filters/state: replacing the source restores the old view and carries compatible conditions to the new view");
        grid.Columns[2].Header = name.Header; name.DisplayIndex = 3; Flush();
        check(DataGridColumn.GetColumnContainingElement(Entry(grid, grid.Columns[2])) == grid.Columns[2] && DataGridColumn.GetColumnContainingElement(Entry(grid, name)) == name,
            "DataGrid/filters/state: duplicate titles and display-index changes preserve exact column identity");
        window.Content = null; Flush();
        check(replacement.Cast<GridRecord>().Count() == 100, "DataGrid/filters/state: detaching restores the new view's business filter");
        window.Close();
        window = new Window { Width = 900, Height = 460, Content = grid }; Motion.SetEnabled(window, false); window.Show(); Flush();
        check(replacement.Cast<GridRecord>().All(row => row.Quantity == 79 && row.Id <= 100), "DataGrid/filters/state: reattaching reinstalls stored conditions");
        DataGridFeatures.ClearAllFilters(grid); Flush();
        DataGridFeatures.ApplyFilter(grid, grid.Columns[0], new NumericFilterCondition<int>(NumericFilterMode.Exact, 100)); Flush();
        grid.SelectedItem = fixture.Records[99]; grid.ScrollIntoView(fixture.Records[99], grid.Columns[0]); Flush();
        check(fixture.Rows.Length < 100 && fixture.Rows.Any(row => row.DataContext == fixture.Records[99]), "DataGrid/filters/state: a distant matching item is selectable without creating all source rows");
        DataGridFeatures.ClearAllFilters(grid); DataGridFeatures.ApplyFilter(grid, quantity, new NumericFilterCondition<int>(NumericFilterMode.Exact, 79));
        window.Content = null; Flush(); grid.Columns.Remove(quantity); window.Content = grid; Flush();
        check(replacement.Cast<GridRecord>().Count() == 100, "DataGrid/filters/state: removing a filtered column while detached cannot leave a stale predicate on reattachment");
        window.Close(); Flush();
    }

    public sealed class FilterModel : INotifyPropertyChanged
    {
        private DataGridFilterCondition? _condition;
        public DataGridFilterCondition? Condition { get => _condition; set { _condition = value; PropertyChanged?.Invoke(this, new(nameof(Condition))); } }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
    private static void VerifyBindingsAndEditing(Action<bool, string> check)
    {
        var (window, fixture, view) = Open(ThemeVariant.Light, ControlSize.Normal); var grid = fixture.Grid; var column = grid.Columns[1]; var model = new FilterModel();
        using var binding = column.Bind(DataGridColumnOptions.FilterConditionProperty, CompiledBinding.Create<FilterModel, DataGridFilterCondition?>(value => value.Condition, source: model, mode: BindingMode.TwoWay));
        model.Condition = new TextFilterCondition("0001"); Flush();
        check(view.Cast<GridRecord>().All(row => row.Name.Contains("0001")), "DataGrid/filters/binding: compiled model-to-column condition binding applies to data");
        model.Condition = new NumericFilterCondition<int>(NumericFilterMode.Exact, 1); Flush();
        check(model.Condition is TextFilterCondition && DataGridColumnOptions.GetFilterError(column) is not null,
            "DataGrid/filters/binding: incompatible bound conditions are rejected and restore the last valid model value");
        DataGridFeatures.ClearFilter(grid, column); Flush();
        check(model.Condition is null && view.Count == 900, "DataGrid/filters/binding: public clear flows back through a compiled TwoWay condition binding");
        var record = fixture.Records[0]; grid.SelectedItem = record; grid.CurrentColumn = column; grid.ScrollIntoView(record, column); grid.Focus(); Flush();
        check(grid.BeginEdit(), "DataGrid/filters/edit: native editing starts before filtering"); Flush();
        var editor = fixture.Rows.First(row => row.DataContext == record).GetVisualDescendants().OfType<TextBox>().Single(box => box.Name == "CellTextBox");
        editor.Text = ""; Flush();
        var result = DataGridFeatures.ApplyFilter(grid, column, new TextFilterCondition("0001"));
        check(!result.Succeeded && DataGridColumnOptions.GetFilterCondition(column) is null && view.Count == 900 && record.HasErrors,
            "DataGrid/filters/edit: failed native validation blocks filtering and retains the invalid editor");
        grid.CancelEdit(DataGridEditingUnit.Row); Flush();
        check(DataGridFeatures.ApplyFilter(grid, column, new TextFilterCondition("0001")).Succeeded, "DataGrid/filters/edit: filtering succeeds after cancelling invalid editing");
        DataGridFeatures.ClearAllFilters(grid); Flush();
        DataGridColumnOptions.SetValueAccessor(column, DataGridValueAccessor.Create<GridRecord, string>(_ => throw new InvalidOperationException("probe getter")));
        result = DataGridFeatures.ApplyFilter(grid, column, new TextFilterCondition("x"));
        check(!result.Succeeded && view.Count == 900 && DataGridColumnOptions.GetFilterCondition(column) is null, "DataGrid/filters/accessor: a failing getter cannot partially replace a valid view");
        DataGridColumnOptions.SetValueAccessor(column, GridFilterFields.Name);
        DataGridFeatures.ApplyFilter(grid, column, new TextFilterCondition("0001")); Flush();
        var business = new Func<object, bool>(item => ((GridRecord)item).Id <= 100);
        view.Filter = business; Flush(); grid.SelectedItem = record; grid.CurrentColumn = column; grid.Focus(); Flush();
        check(grid.BeginEdit(), "DataGrid/filters/edit: an active filtered view can start another native edit"); Flush();
        window.Content = null; Flush();
        if (view.IsEditingItem) view.CancelEdit(); Flush();
        check(ReferenceEquals(view.Filter, business), "DataGrid/filters/edit: detaching during an edit restores the business predicate when the edit ends");
        window.Close(); Flush();
    }

    public sealed class NumberRow<T>(T value) where T : struct, INumber<T> { public T Value { get; } = value; }
    private static void VerifyNumber<T>(T value, string inputText, Action<bool, string> check, CultureInfo? culture = null) where T : struct, INumber<T>
    {
        _ = DataGridAot.PreserveRowType<NumberRow<T>>();
        var rows = Enumerable.Range(0, 1000).Select(i => new NumberRow<T>(i % 2 == 0 ? value : T.Zero)).ToArray();
        var view = new DataGridCollectionView(rows) { Culture = culture ?? CultureInfo.InvariantCulture };
        var grid = new DataGrid { ItemsSource = view, AutoGenerateColumns = false, MaxColumnWidth = 240 };
        var column = new DataGridTextColumn { Header = "数值", Binding = CompiledBinding.Create<NumberRow<T>, T>(row => row.Value) };
        grid.Columns.Add(column);
        DataGridFeatures.SetShowColumnFilters(grid, true);
        var window = new Window { Width = 500, Height = 320, Content = grid }; Motion.SetEnabled(window, false); window.Show(); Flush();
        var button = Entry(grid, column); Click(window, button); var popup = Popup(button); var host = TopLevel.GetTopLevel(popup)!;
        Find<TextBox>(popup, "DataGridFilterExact").Text = inputText; Click(host, Find<Button>(popup, "DataGridFilterApply"));
        check(view.Cast<NumberRow<T>>().Count() == 500 && view.Cast<NumberRow<T>>().All(row => row.Value == value), $"DataGrid/filters/numeric/{typeof(T).Name}: actual popup parses and exactly matches the original field type");
        Click(window, button); popup = Popup(button); host = TopLevel.GetTopLevel(popup)!;
        Find<TextBox>(popup, "DataGridFilterExact").Text = "not a number"; Click(host, Find<Button>(popup, "DataGridFilterApply"));
        check(button.Flyout!.IsOpen && Find<TextBlock>(popup, "DataGridFilterError").IsVisible && view.Cast<NumberRow<T>>().Count() == 500,
            $"DataGrid/filters/numeric/{typeof(T).Name}: invalid input keeps the previous typed predicate and shows validation");
        Click(host, Find<Button>(popup, "DataGridFilterClear"));
        check(!button.Flyout.IsOpen && view.Cast<NumberRow<T>>().Count() == 1000 && DataGridColumnOptions.GetFilterCondition(column) is null,
            $"DataGrid/filters/numeric/{typeof(T).Name}: the real clear-column button restores all source rows");
        check(grid.GetVisualDescendants().OfType<DataGridRow>().Count() < 100, $"DataGrid/filters/numeric/{typeof(T).Name}: typed filtering retains bounded native row realization");
        window.Close(); Flush();
    }
    public sealed class NullableRow(decimal? value) { public decimal? Value { get; } = value; }
    private static void VerifyNullable(Action<bool, string> check)
    {
        _ = DataGridAot.PreserveRowType<NullableRow>();
        var rows = Enumerable.Range(0, 1000).Select(i => new NullableRow(i % 3 == 0 ? null : i)).ToArray();
        var grid = new DataGrid { ItemsSource = rows, AutoGenerateColumns = false, MaxColumnWidth = 240 };
        var column = new DataGridTextColumn { Header = "可空数值", Binding = CompiledBinding.Create<NullableRow, decimal?>(row => row.Value) };
        grid.Columns.Add(column);
        var window = new Window { Width = 500, Height = 320, Content = grid }; Motion.SetEnabled(window, false); DataGridFeatures.SetShowColumnFilters(grid, true); window.Show(); Flush();
        check(DataGridFeatures.ApplyFilter(grid, column, new NumericFilterCondition<decimal>(NumericFilterMode.Range, minimum: 995)).Succeeded &&
            grid.CollectionView.Cast<NullableRow>().Count() == rows.Count(row => row.Value >= 995), "DataGrid/filters/numeric: nullable numbers accept an underlying-type one-sided range and exclude null");
        check(DataGridFeatures.ApplyFilter(grid, column, new NumericFilterCondition<decimal>(NumericFilterMode.Range)).Succeeded && grid.CollectionView.Cast<NullableRow>().Count() == 1000,
            "DataGrid/filters/numeric: an empty range clears the condition and restores null-valued rows");
        check(!new NumericFilterCondition<double>(NumericFilterMode.Range, minimum: 0).Matches(double.NaN) && !new NumericFilterCondition<double>(NumericFilterMode.Range, minimum: 0).Matches(double.PositiveInfinity),
            "DataGrid/filters/numeric: non-finite source values do not match numeric conditions");
        check(new TextFilterCondition("alpha").Matches("ALPHA beta") && !new TextFilterCondition("alpha").Matches(null), "DataGrid/filters/text: Contains ignores casing and handles null source values");
        window.Close(); Flush();
    }
}
