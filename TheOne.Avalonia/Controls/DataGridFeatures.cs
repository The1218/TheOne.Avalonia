using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TheOne.Avalonia.Controls;

public readonly record struct DataGridFilterApplyResult(bool Succeeded, string? Error = null);

public sealed class DataGridFilterChangingEventArgs(
    DataGridColumn column,
    DataGridFilterCondition? oldCondition,
    DataGridFilterCondition? newCondition)
    : CancelRoutedEventArgs(DataGridFeatures.FilterChangingEvent)
{
    public DataGridColumn Column { get; } = column;
    public DataGridFilterCondition? OldCondition { get; } = oldCondition;
    public DataGridFilterCondition? NewCondition { get; } = newCondition;
}

public sealed class DataGridFilterChangedEventArgs(
    DataGridColumn column,
    DataGridFilterCondition? oldCondition,
    DataGridFilterCondition? newCondition)
    : RoutedEventArgs(DataGridFeatures.FilterChangedEvent)
{
    public DataGridColumn Column { get; } = column;
    public DataGridFilterCondition? OldCondition { get; } = oldCondition;
    public DataGridFilterCondition? NewCondition { get; } = newCondition;
}

/// <summary>Column filtering and native right freezing; preserves the data view and presenters.</summary>
public sealed class DataGridFeatures : AvaloniaObject
{
    public static readonly AttachedProperty<int> FrozenRightColumnCountProperty =
        DataGrid.FrozenRightColumnCountProperty.AddOwner<DataGridFeatures>();

    public static readonly AttachedProperty<bool> ShowColumnFiltersProperty =
        AvaloniaProperty.RegisterAttached<DataGridFeatures, DataGrid, bool>("ShowColumnFilters");

    public static readonly RoutedEvent<DataGridFilterChangingEventArgs> FilterChangingEvent =
        RoutedEvent.Register<DataGrid, DataGridFilterChangingEventArgs>("FilterChanging", RoutingStrategies.Bubble);

    public static readonly RoutedEvent<DataGridFilterChangedEventArgs> FilterChangedEvent =
        RoutedEvent.Register<DataGrid, DataGridFilterChangedEventArgs>("FilterChanged", RoutingStrategies.Bubble);

    public static readonly AttachedProperty<bool> ShowSummaryRowProperty =
        AvaloniaProperty.RegisterAttached<DataGridFeatures, DataGrid, bool>("ShowSummaryRow");

    public static readonly AttachedProperty<DataGridSummaryScope> SummaryScopeProperty =
        AvaloniaProperty.RegisterAttached<DataGridFeatures, DataGrid, DataGridSummaryScope>("SummaryScope",
            validate: Enum.IsDefined);

    public static readonly AttachedProperty<bool> SummaryLeavesOnlyProperty =
        AvaloniaProperty.RegisterAttached<DataGridFeatures, DataGrid, bool>("SummaryLeavesOnly");

    public static readonly AttachedProperty<string> SummaryLabelProperty =
        AvaloniaProperty.RegisterAttached<DataGridFeatures, DataGrid, string>("SummaryLabel", "合计");

    public static readonly AttachedProperty<string> SummaryEmptyTextProperty =
        AvaloniaProperty.RegisterAttached<DataGridFeatures, DataGrid, string>("SummaryEmptyText", "—");

    public static readonly RoutedEvent<DataGridSummaryUpdatedEventArgs> SummaryUpdatedEvent =
        RoutedEvent.Register<DataGrid, DataGridSummaryUpdatedEventArgs>("SummaryUpdated", RoutingStrategies.Bubble);

    private static readonly ConditionalWeakTable<DataGrid, DataGridSummaryState> Summaries = new();

    private static readonly ConditionalWeakTable<DataGrid, DataGridFilterState> States = new();

    static DataGridFeatures()
    {
        ShowSummaryRowProperty.Changed.AddClassHandler<DataGrid>((grid, _) => SummaryState(grid).Synchronize());
        ShowColumnFiltersProperty.Changed.AddClassHandler<DataGrid>((grid, change) =>
        {
            if (change.GetNewValue<bool>()) State(grid).Start();
        });
    }

    public static int GetFrozenRightColumnCount(DataGrid grid)
    {
        return grid.GetValue(FrozenRightColumnCountProperty);
    }

    public static void SetFrozenRightColumnCount(DataGrid grid, int count)
    {
        grid.SetValue(FrozenRightColumnCountProperty, count);
    }

    public static bool GetShowSummaryRow(DataGrid grid)
    {
        return grid.GetValue(ShowSummaryRowProperty);
    }

    public static void SetShowSummaryRow(DataGrid grid, bool value)
    {
        grid.SetValue(ShowSummaryRowProperty, value);
    }

    public static DataGridSummaryScope GetSummaryScope(DataGrid grid)
    {
        return grid.GetValue(SummaryScopeProperty);
    }

    public static void SetSummaryScope(DataGrid grid, DataGridSummaryScope value)
    {
        grid.SetValue(SummaryScopeProperty, value);
    }

    public static bool GetSummaryLeavesOnly(DataGrid grid)
    {
        return grid.GetValue(SummaryLeavesOnlyProperty);
    }

    public static void SetSummaryLeavesOnly(DataGrid grid, bool value)
    {
        grid.SetValue(SummaryLeavesOnlyProperty, value);
    }

    public static string GetSummaryLabel(DataGrid grid)
    {
        return grid.GetValue(SummaryLabelProperty);
    }

    public static void SetSummaryLabel(DataGrid grid, string value)
    {
        grid.SetValue(SummaryLabelProperty, value);
    }

    public static string GetSummaryEmptyText(DataGrid grid)
    {
        return grid.GetValue(SummaryEmptyTextProperty);
    }

    public static void SetSummaryEmptyText(DataGrid grid, string value)
    {
        grid.SetValue(SummaryEmptyTextProperty, value);
    }

    public static DataGridSummaryResult? GetSummaryResult(DataGrid grid, DataGridColumn column)
    {
        return SummaryState(grid).GetResult(column);
    }

    internal static DataGridSummaryState SummaryState(DataGrid grid)
    {
        return Summaries.GetValue(grid, owner => new DataGridSummaryState(owner));
    }

    public static bool GetShowColumnFilters(DataGrid grid)
    {
        return grid.GetValue(ShowColumnFiltersProperty);
    }

    public static void SetShowColumnFilters(DataGrid grid, bool value)
    {
        grid.SetValue(ShowColumnFiltersProperty, value);
    }

    public static DataGridFilterApplyResult ApplyFilter(DataGrid grid, DataGridColumn column,
        DataGridFilterCondition? condition)
    {
        return State(grid).Apply(column, condition);
    }

    public static DataGridFilterApplyResult ClearFilter(DataGrid grid, DataGridColumn column)
    {
        return ApplyFilter(grid, column, null);
    }

    public static DataGridFilterApplyResult ClearAllFilters(DataGrid grid)
    {
        return State(grid).ClearAll();
    }

    internal static DataGridFilterState State(DataGrid grid)
    {
        return States.GetValue(grid, owner => new DataGridFilterState(owner));
    }
}