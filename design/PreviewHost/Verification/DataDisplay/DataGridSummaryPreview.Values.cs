using System.ComponentModel;
using System.Numerics;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static partial class DataGridSummaryPreview
{
    public sealed class NumberRow<T> where T : struct { public T? Value { get; set; } }
    private static void VerifyNumbers(Action<bool, string> check)
    {
        VerifyNumber<byte>([1, 255, null], check); VerifyNumber<sbyte>([-127, 3, null], check);
        VerifyNumber<short>([-1000, 3000, null], check); VerifyNumber<ushort>([65000, 2, null], check);
        VerifyNumber<int>([-123456, 5, null], check); VerifyNumber<uint>([4000000000, 5, null], check);
        VerifyNumber<long>([9007199254740993, 7, null], check); VerifyNumber<ulong>([ulong.MaxValue, 9007199254740993, null], check);
        VerifyNumber<float>([1.25f, float.NaN, float.PositiveInfinity, null, 3.5f], check);
        VerifyNumber<double>([1.25, double.NaN, double.NegativeInfinity, null, 3.5], check);
        VerifyNumber<decimal>([1.25m, 3.5m, null], check);
        VerifyOverflow<decimal>([decimal.MaxValue, decimal.MaxValue], check);
        VerifyOverflow<double>([double.MaxValue, double.MaxValue], check);
    }
    private static void VerifyNumber<T>(T?[] values, Action<bool, string> check) where T : struct, INumber<T>
    {
        _ = DataGridAot.PreserveRowType<NumberRow<T>>();
        var rows = values.Select(value => new NumberRow<T> { Value = value }).ToArray();
        var grid = new DataGrid { AutoGenerateColumns = false, MaxColumnWidth = 240, ItemsSource = rows };
        var column = new DataGridTextColumn { Header = "数值", Binding = CompiledBinding.Create<NumberRow<T>, T?>(row => row.Value) }; grid.Columns.Add(column);
        Configure(column, DataGridSummaryKind.Sum); DataGridFeatures.SetShowSummaryRow(grid, true); DataGridFeatures.SetShowColumnFilters(grid, true);
        var window = new Window { Width = 500, Height = 260, Content = grid }; Motion.SetEnabled(window, false); window.Show(); Flush();
        var valid = values.Where(value => value.HasValue && T.IsFinite(value.Value)).Select(value => value!.Value).ToArray();
        bool floating = typeof(T) == typeof(float) || typeof(T) == typeof(double);
        foreach (var kind in Enum.GetValues<DataGridSummaryKind>())
        {
            DataGridColumnOptions.SetSummaryKind(column, kind); Flush();
            var result = DataGridFeatures.GetSummaryResult(grid, column)!;
            object expected = kind switch
            {
                DataGridSummaryKind.Count => (object)(long)valid.Length,
                DataGridSummaryKind.Min => valid.Min()!,
                DataGridSummaryKind.Max => valid.Max()!,
                DataGridSummaryKind.Average => floating ? (object)(valid.Sum(value => Convert.ToDouble(value)) / valid.Length) : valid.Sum(value => Convert.ToDecimal(value)) / valid.Length,
                _ => floating ? (object)valid.Sum(value => Convert.ToDouble(value)) : valid.Sum(value => Convert.ToDecimal(value))
            };
            check(result.Error is null && result.ValidCount == valid.Length && Equals(result.Value, expected), $"DataGrid/summary/{typeof(T).Name}/{kind}: exact raw property values preserve original type, integer precision and finite-value count");
        }
        grid.ItemsSource = new[] { new NumberRow<T> { Value = null } }; Flush();
        check(Equals(DataGridFeatures.GetSummaryResult(grid, column)?.Value, 0L), $"DataGrid/summary/{typeof(T).Name}: Count of only null values is zero");
        DataGridColumnOptions.SetSummaryKind(column, DataGridSummaryKind.Sum); Flush();
        check(DataGridFeatures.GetSummaryResult(grid, column) is { Value: null, ValidCount: 0, Error: null }, $"DataGrid/summary/{typeof(T).Name}: empty Sum uses the configured empty text");
        window.Close(); Flush();
    }
    private static void VerifyOverflow<T>(T[] values, Action<bool, string> check) where T : struct, INumber<T>
    {
        _ = DataGridAot.PreserveRowType<NumberRow<T>>();
        var grid = new DataGrid { AutoGenerateColumns = false, MaxColumnWidth = 240, ItemsSource = values.Select(value => new NumberRow<T> { Value = value }).ToArray() };
        var column = new DataGridTextColumn { Header = "数值", Binding = CompiledBinding.Create<NumberRow<T>, T?>(row => row.Value) }; grid.Columns.Add(column);
        Configure(column, DataGridSummaryKind.Sum); DataGridFeatures.SetShowSummaryRow(grid, true);
        var window = new Window { Width = 500, Height = 260, Content = grid }; Motion.SetEnabled(window, false); window.Show(); Flush();
        check(DataGridFeatures.GetSummaryResult(grid, column) is { Value: null, ValidCount: 2, Error: not null }, $"DataGrid/summary/{typeof(T).Name}: overflow reports an error and valid input count");
        DataGridColumnOptions.SetSummaryKind(column, DataGridSummaryKind.Count); Flush();
        check(DataGridFeatures.GetSummaryResult(grid, column) is { Value: 2L, Error: null }, $"DataGrid/summary/{typeof(T).Name}: counting does not perform overflowing arithmetic");
        window.Close(); Flush();
    }
    public sealed class NestedChild : INotifyPropertyChanged
    {
        private int _value;
        public int Value { get => _value; set { _value = value; PropertyChanged?.Invoke(this, new(nameof(Value))); } }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
    public sealed class NestedRow : INotifyPropertyChanged
    {
        private NestedChild? _child;
        public NestedChild? Child { get => _child; set { _child = value; PropertyChanged?.Invoke(this, new(nameof(Child))); } }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
    private static void VerifyNested(Action<bool, string> check)
    {
        _ = DataGridAot.PreserveRowType<NestedRow>(); _ = DataGridAot.PreserveRowType<NestedChild>();
        var child = new NestedChild { Value = 5 }; var row = new NestedRow { Child = child };
        var view = new DataGridCollectionView(new[] { row, new NestedRow() });
        var grid = new DataGrid { AutoGenerateColumns = false, MaxColumnWidth = 240, ItemsSource = view };
        var column = new DataGridTextColumn { Header = "嵌套数值", Binding = CompiledBinding.Create<NestedRow, int>(r => r.Child!.Value) }; grid.Columns.Add(column);
        Configure(column, DataGridSummaryKind.Sum); DataGridFeatures.SetShowSummaryRow(grid, true); DataGridFeatures.SetShowColumnFilters(grid, true);
        var window = new Window { Width = 500, Height = 260, Content = grid }; Motion.SetEnabled(window, false); window.Show(); Flush();
        check(DataGridFeatures.GetSummaryResult(grid, column) is { Value: 5m, ValidCount: 1 }, "DataGrid/summary/nested: automatic compiled path handles null intermediate entity properties");
        DataGridFeatures.ApplyFilter(grid, column, new NumericFilterCondition<int>(NumericFilterMode.Range, minimum: 6)); Flush();
        check(DataGridFeatures.GetSummaryResult(grid, column)?.ValidCount == 0, "DataGrid/summary/nested: automatic nested filter uses raw entity values");
        child.Value = 8; Flush();
        check(view.Count == 1 && DataGridFeatures.GetSummaryResult(grid, column) is { Value: 8m, ValidCount: 1 }, "DataGrid/summary/nested: nested property updates refresh both filtering and statistics");
        row.Child = new NestedChild { Value = 10 }; Flush();
        check(DataGridFeatures.GetSummaryResult(grid, column) is { Value: 10m }, "DataGrid/summary/nested: intermediate object replacement reconnects listeners");
        DataGridFeatures.ClearAllFilters(grid);
        column.Binding = CompiledBinding.Create<NestedRow, int>(r => r.Child!.Value, stringFormat: "显示值 {0}"); Flush();
        check(DataGridFeatures.GetSummaryResult(grid, column) is { Value: 10m }, "DataGrid/summary/nested: display StringFormat does not change automatic numeric field type");
        window.Close(); Flush();
    }
}
