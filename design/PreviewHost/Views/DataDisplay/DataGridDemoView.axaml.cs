using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class DataGridDemoView : UserControl
{
    private readonly ConditionalWeakTable<DataGridRow, Seen> _seen = new();
    private int _created;
    private bool _metricsQueued;

    public DataGridDemoView()
    {
        DataContext = Model;
        InitializeComponent();
        Model.PropertyChanged += SelectedChanged;
        Model.ResetRequested += () =>
        {
            DataGridFeatures.ClearAllFilters(InventoryGrid);
            QueueMetrics();
        };
        InventoryGrid.LoadingRow += (_, args) =>
        {
            if (!_seen.TryGetValue(args.Row, out var seen))
            {
                _seen.Add(args.Row, new Seen());
                _created++;
            }

            args.Row.Header = (args.Row.DataContext as GridRecord)?.Id;
            QueueMetrics();
        };
        InventoryGrid.UnloadingRow += (_, _) => QueueMetrics();
        InventoryGrid.SizeChanged += (_, _) => QueueMetrics();
        InventoryGrid.AddHandler(DataGridFeatures.FilterChangedEvent, (_, _) =>
        {
            Model.NotifyViewChanged();
            QueueMetrics();
        });
        AttachedToVisualTree += (_, _) => QueueMetrics();
    }

    public DataGridDemoViewModel Model { get; } = new();
    public DataGrid Grid => InventoryGrid;

    private void ClearColumnFilters(object? sender, RoutedEventArgs args)
    {
        DataGridFeatures.ClearAllFilters(InventoryGrid);
        QueueMetrics();
    }

    private void SelectedChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(DataGridDemoViewModel.Selected) && Model.Selected is { } record)
            Dispatcher.UIThread.Post(() =>
            {
                if (this.IsAttachedToVisualTree() && Model.Selected == record)
                    InventoryGrid.ScrollIntoView(record, null!);
            }, DispatcherPriority.Loaded);
    }

    private void QueueMetrics()
    {
        if (_metricsQueued) return;
        _metricsQueued = true;
        Dispatcher.UIThread.Post(() =>
        {
            _metricsQueued = false;
            UpdateMetrics();
        }, DispatcherPriority.Background);
    }

    public void UpdateMetrics()
    {
        var rows = InventoryGrid.GetVisualDescendants().OfType<DataGridRow>().Where(row => row.IsVisible).ToArray();
        Model.Metrics =
            $"显示行容器 {rows.Length} / {Model.View.Count:N0} · 累计创建 {_created} 个 · 挂载单元格 {rows.SelectMany(row => row.GetVisualDescendants().OfType<DataGridCell>()).Count():N0}";
    }

    private sealed class Seen;
}