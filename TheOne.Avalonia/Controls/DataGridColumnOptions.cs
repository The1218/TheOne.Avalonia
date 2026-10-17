using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace TheOne.Avalonia.Controls;

/// <summary>Explicit model fields and filter conditions for the official DataGrid column types.</summary>
public sealed class DataGridColumnOptions : AvaloniaObject
{
    public static readonly AttachedProperty<IDataGridValueAccessor?> ValueAccessorProperty =
        AvaloniaProperty.RegisterAttached<DataGridColumnOptions, DataGridColumn, IDataGridValueAccessor?>(
            "ValueAccessor");

    public static readonly AttachedProperty<bool> FilterEnabledProperty =
        AvaloniaProperty.RegisterAttached<DataGridColumnOptions, DataGridColumn, bool>("FilterEnabled", true);

    public static readonly AttachedProperty<DataGridFilterKind> FilterKindProperty =
        AvaloniaProperty.RegisterAttached<DataGridColumnOptions, DataGridColumn, DataGridFilterKind>("FilterKind");

    public static readonly AttachedProperty<DataGridFilterCondition?> FilterConditionProperty =
        AvaloniaProperty.RegisterAttached<DataGridColumnOptions, DataGridColumn, DataGridFilterCondition?>(
            "FilterCondition", defaultBindingMode: BindingMode.TwoWay);

    public static readonly AttachedProperty<string?> FilterErrorProperty =
        AvaloniaProperty.RegisterAttached<DataGridColumnOptions, DataGridColumn, string?>("FilterError");

    public static readonly AttachedProperty<bool> SummaryEnabledProperty =
        AvaloniaProperty.RegisterAttached<DataGridColumnOptions, DataGridColumn, bool>("SummaryEnabled");

    public static readonly AttachedProperty<DataGridSummaryKind> SummaryKindProperty =
        AvaloniaProperty.RegisterAttached<DataGridColumnOptions, DataGridColumn, DataGridSummaryKind>("SummaryKind",
            validate: Enum.IsDefined);

    public static readonly AttachedProperty<string?> SummaryFormatProperty =
        AvaloniaProperty.RegisterAttached<DataGridColumnOptions, DataGridColumn, string?>("SummaryFormat");

    private static readonly ConditionalWeakTable<DataGridColumn, WeakReference<DataGrid>> Owners = new();

    static DataGridColumnOptions()
    {
        FilterConditionProperty.Changed.AddClassHandler<DataGridColumn>((column, change) =>
        {
            if (change.GetNewValue<DataGridFilterCondition?>() is not null &&
                Owners.TryGetValue(column, out var owner) && owner.TryGetTarget(out var grid))
                DataGridFeatures.State(grid).Start();
        });
    }

    public static bool GetSummaryEnabled(DataGridColumn column)
    {
        return column.GetValue(SummaryEnabledProperty);
    }

    public static void SetSummaryEnabled(DataGridColumn column, bool value)
    {
        column.SetValue(SummaryEnabledProperty, value);
    }

    public static DataGridSummaryKind GetSummaryKind(DataGridColumn column)
    {
        return column.GetValue(SummaryKindProperty);
    }

    public static void SetSummaryKind(DataGridColumn column, DataGridSummaryKind value)
    {
        column.SetValue(SummaryKindProperty, value);
    }

    public static string? GetSummaryFormat(DataGridColumn column)
    {
        return column.GetValue(SummaryFormatProperty);
    }

    public static void SetSummaryFormat(DataGridColumn column, string? value)
    {
        column.SetValue(SummaryFormatProperty, value);
    }

    internal static void ConnectOwner(DataGridColumn column, DataGrid grid)
    {
        Owners.Remove(column);
        Owners.Add(column, new WeakReference<DataGrid>(grid));
        if (GetFilterCondition(column) is not null) DataGridFeatures.State(grid).Start();
    }

    public static IDataGridValueAccessor? GetValueAccessor(DataGridColumn column)
    {
        return column.GetValue(ValueAccessorProperty);
    }

    public static void SetValueAccessor(DataGridColumn column, IDataGridValueAccessor? value)
    {
        column.SetValue(ValueAccessorProperty, value);
    }

    public static bool GetFilterEnabled(DataGridColumn column)
    {
        return column.GetValue(FilterEnabledProperty);
    }

    public static void SetFilterEnabled(DataGridColumn column, bool value)
    {
        column.SetValue(FilterEnabledProperty, value);
    }

    public static DataGridFilterKind GetFilterKind(DataGridColumn column)
    {
        return column.GetValue(FilterKindProperty);
    }

    public static void SetFilterKind(DataGridColumn column, DataGridFilterKind value)
    {
        column.SetValue(FilterKindProperty, value);
    }

    public static DataGridFilterCondition? GetFilterCondition(DataGridColumn column)
    {
        return column.GetValue(FilterConditionProperty);
    }

    public static void SetFilterCondition(DataGridColumn column, DataGridFilterCondition? value)
    {
        column.SetValue(FilterConditionProperty, value);
    }

    public static string? GetFilterError(DataGridColumn column)
    {
        return column.GetValue(FilterErrorProperty);
    }

    internal static DataGridFilterKind EffectiveKind(DataGridColumn column)
    {
        return GetFilterEnabled(column)
            ? DataGridFilterParsing.Kind(DataGridAutoFields.Resolve(column), GetFilterKind(column))
            : DataGridFilterKind.None;
    }
}