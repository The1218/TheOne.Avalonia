using System.Collections.ObjectModel;
using System.Globalization;
using System.Numerics;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static partial class DataGridFilterPreview
{
    private static (Window Window, DataGridStage1Fixture Fixture, DataGridCollectionView View) Open(ThemeVariant theme, ControlSize size)
    {
        var fixture = new DataGridStage1Fixture();
        var view = new DataGridCollectionView(new ObservableCollection<GridRecord>(fixture.Records)) { Filter = item => ((GridRecord)item).Id <= 900 };
        fixture.Grid.ItemsSource = view; DataGridFeatures.SetShowColumnFilters(fixture.Grid, true);
        var window = new Window { Width = 900, Height = 460, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme, Content = fixture.Grid };
        Motion.SetEnabled(window, false); ControlSizing.SetSize(window, size); window.Show(); Flush();
        return (window, fixture, view);
    }
    public static void Verify(Action<bool, string> check)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>()) VerifyInteraction(theme, size, check);
        VerifyLifetime(check); VerifyBindingsAndEditing(check);
        VerifyNumber<byte>(255, "255", check); VerifyNumber<sbyte>(-127, "-127", check);
        VerifyNumber<short>(-1234, "-1234", check); VerifyNumber<ushort>(65000, "65000", check);
        VerifyNumber<int>(-123456, "-123456", check); VerifyNumber<uint>(4000000000, "4000000000", check);
        VerifyNumber<long>(9007199254740993, "9007199254740993", check); VerifyNumber<ulong>(18446744073709551615, "18446744073709551615", check);
        VerifyNumber<float>(0.1f, "0.1", check); VerifyNumber<double>(1.25, "1.25", check); VerifyNumber<decimal>(1.25m, "1,25", check, CultureInfo.GetCultureInfo("de-DE"));
        VerifyNullable(check);
    }
    private static void VerifyInteraction(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var (window, fixture, view) = Open(theme, size); var grid = fixture.Grid; var name = grid.Columns[1]; var quantity = grid.Columns[3];
        var label = $"DataGrid/filters/{theme.Key}/{size}";
        var button = Entry(grid, name);
        check(view.Count == 900 && grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Count() == 1 && fixture.Rows.Length < 100,
            $"{label}: 1,000 source items preserve the business filter and native virtual rows");
        check(grid.Columns.All(column => Entry(grid, column).IsVisible) && button.Width == (size == ControlSize.Small ? 20 : size == ControlSize.Large ? 28 : 24),
            $"{label}: every supported native column has a density-sized header entry");
        var sorts = view.SortDescriptions.Count;
        Click(window, button); var popup = Popup(button); var host = TopLevel.GetTopLevel(popup)!;
        check(button.Flyout!.IsOpen && view.SortDescriptions.Count == sorts && grid.Columns[1].DisplayIndex == 1,
            $"{label}: a real filter click opens the popup without sorting or moving the column");
        var input = Find<TextBox>(popup, "DataGridFilterText");
        check(ControlSizing.GetSize(input) == size && input.FontSize == (size == ControlSize.Small ? 12 : size == ControlSize.Large ? 16 : 14) && popup.ActualThemeVariant == theme,
            $"{label}: popup input inherits density and effective light/dark theme");
        input.Focus(); input.SelectAll(); host.KeyTextInput("  0001  "); Press(host, Key.Enter, PhysicalKey.Enter);
        var expected = fixture.Records.Where(row => row.Id <= 900 && row.Name.Contains("0001", StringComparison.OrdinalIgnoreCase)).ToArray();
        check(!button.Flyout.IsOpen && DataGridColumnOptions.GetFilterCondition(name) is TextFilterCondition { Text: "0001" } && view.Cast<GridRecord>().SequenceEqual(expected),
            $"{label}: Enter applies a trimmed Contains condition to model data");
        check(button.IsFocused && Colour(button.Foreground) == Resource(grid, "One.Accent"), $"{label}: active filter is visible and closing returns focus to its entry");
        check(DataGridFeatures.ApplyFilter(grid, quantity, new NumericFilterCondition<int>(NumericFilterMode.Range, minimum: 5, maximum: 10)).Succeeded,
            $"{label}: typed numeric range applies"); Flush();
        check(view.Cast<GridRecord>().SequenceEqual(expected.Where(row => row.Quantity >= 5 && row.Quantity <= 10)),
            $"{label}: columns combine with AND and include both numeric boundaries");
        var current = DataGridColumnOptions.GetFilterCondition(quantity); var ids = view.Cast<GridRecord>().Select(row => row.Id).ToArray();
        var rejected = DataGridFeatures.ApplyFilter(grid, quantity, new NumericFilterCondition<decimal>(NumericFilterMode.Exact, 5m));
        check(!rejected.Succeeded && Equals(DataGridColumnOptions.GetFilterCondition(quantity), current) && view.Cast<GridRecord>().Select(row => row.Id).SequenceEqual(ids),
            $"{label}: wrong numeric type preserves the previous valid condition and rows");
        DataGridFeatures.ClearFilter(grid, name); Flush();
        var numberButton = Entry(grid, quantity); Click(window, numberButton); popup = Popup(numberButton); host = TopLevel.GetTopLevel(popup)!;
        check(Find<ComboBox>(popup, "DataGridFilterMode").SelectedIndex == 1 && Find<TextBox>(popup, "DataGridFilterMinimum").Text == "5",
            $"{label}: reopening a numeric popup restores the applied range draft");
        var lowerBounds = Find<TextBox>(popup, "DataGridFilterMinimum").TranslatePoint(default, popup)!.Value;
        var upperBounds = Find<TextBox>(popup, "DataGridFilterMaximum").TranslatePoint(default, popup)!.Value;
        var primary = Find<Button>(popup, "DataGridFilterApply"); var secondary = Find<Button>(popup, "DataGridFilterCancel");
        check(upperBounds.Y > lowerBounds.Y + Find<TextBox>(popup, "DataGridFilterMinimum").Bounds.Height &&
            Math.Abs(primary.Bounds.Width - secondary.Bounds.Width) < 1 && primary.TranslatePoint(default, popup)!.Value.X > secondary.TranslatePoint(default, popup)!.Value.X,
            $"{label}: numeric bounds are vertically separated and footer actions share equal, nonoverlapping columns");
        Find<TextBox>(popup, "DataGridFilterMinimum").Text = "12"; Find<TextBox>(popup, "DataGridFilterMaximum").Text = "3";
        Click(host, Find<Button>(popup, "DataGridFilterApply"));
        check(numberButton.Flyout!.IsOpen && Find<TextBlock>(popup, "DataGridFilterError").IsVisible && Equals(DataGridColumnOptions.GetFilterCondition(quantity), current),
            $"{label}: reversed range shows validation and retains the applied filter");
        Press(host, Key.Escape, PhysicalKey.Escape);
        check(!numberButton.Flyout.IsOpen && Equals(DataGridColumnOptions.GetFilterCondition(quantity), current), $"{label}: Escape discards the invalid draft");
        Click(window, numberButton); popup = Popup(numberButton); host = TopLevel.GetTopLevel(popup)!;
        Click(host, Find<Button>(popup, "DataGridFilterClearAll"));
        check(view.Count == 900 && grid.Columns.All(column => DataGridColumnOptions.GetFilterCondition(column) is null),
            $"{label}: clear-all restores the business filter and clears every column condition");
        DataGridFeatures.ApplyFilter(grid, name, new TextFilterCondition("000")); DataGridFeatures.SetShowColumnFilters(grid, false); Flush();
        check(!button.IsVisible && DataGridColumnOptions.GetFilterCondition(name) is not null, $"{label}: hiding entries retains configured conditions");
        DataGridFeatures.SetShowColumnFilters(grid, true); DataGridColumnOptions.SetFilterEnabled(name, false); Flush();
        check(!button.IsVisible && DataGridColumnOptions.GetFilterCondition(name) is null && view.Count == 900, $"{label}: disabling one column removes only its condition");
        DataGridColumnOptions.SetFilterEnabled(name, true); DataGridColumnOptions.SetValueAccessor(name, null); Flush();
        check(button.IsVisible && button.IsEnabled, $"{label}: clearing an explicit accessor automatically resolves the compiled entity property");
        ((DataGridBoundColumn)name).Binding = CompiledBinding.Create<GridRecord, bool>(row => row.Enabled); Flush();
        check(button.IsVisible && !button.IsEnabled, $"{label}: unsupported entity property type disables the entry after binding replacement");
        window.Close(); Flush();
    }
    private static DataGridFilterButton Entry(DataGrid grid, DataGridColumn column) => grid.GetVisualDescendants().OfType<DataGridFilterButton>().Single(button => DataGridColumn.GetColumnContainingElement(button) == column);
    private static Control Popup(DataGridFilterButton button) => ((PopupFlyoutBase)button.Flyout!).Popup.Child!;
    private static T Find<T>(Control root, string name) where T : Control => root.GetVisualDescendants().OfType<T>().Single(control => control.Name == name);
    private static void Click(TopLevel host, Control control)
    {
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), host)!.Value;
        host.MouseMove(point); host.MouseDown(point, MouseButton.Left); host.MouseUp(point, MouseButton.Left); Flush();
    }
    private static void Press(TopLevel host, Key key, PhysicalKey physical) { host.KeyPress(key, RawInputModifiers.None, physical, null); host.KeyRelease(key, RawInputModifiers.None, physical, null); Flush(); }
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
    private static Color? Colour(IBrush? brush) => (brush as ISolidColorBrush)?.Color;
    private static Color? Resource(Control control, string key) => control.TryFindResource(key, control.ActualThemeVariant, out var value) ? Colour(value as IBrush) : null;
}
