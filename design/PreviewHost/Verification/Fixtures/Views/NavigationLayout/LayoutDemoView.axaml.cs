using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace TheOne.DesignPreview;

public partial class LayoutDemoView : UserControl
{
    private bool _metricsQueued;

    public LayoutDemoView()
    {
        DataContext = this;
        InitializeComponent();
        PaneSample.PaneOpened += (_, _) => QueueMetrics();
        PaneSample.PaneClosed += (_, _) => QueueMetrics();
        ColumnSplitter.PropertyChanged += SplitterBoundsChanged;
        RowSplitter.PropertyChanged += SplitterBoundsChanged;
        PaneList.AddHandler(ScrollViewer.ScrollChangedEvent, (_, _) => QueueMetrics());
        AttachedToVisualTree += (_, _) => QueueMetrics();
    }

    public IReadOnlyList<string> NavigationItems => VirtualizationPreview.Rows;

    private void OnTogglePane(object? sender, RoutedEventArgs args)
    {
        PaneSample.SetCurrentValue(SplitView.IsPaneOpenProperty, !PaneSample.IsPaneOpen);
    }

    private void OnModeChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (PaneSample is null || sender is not ComboBox { SelectedIndex: >= 0 and <= 3 } picker) return;
        PaneSample.DisplayMode = picker.SelectedIndex switch
        {
            0 => SplitViewDisplayMode.Overlay,
            1 => SplitViewDisplayMode.Inline,
            2 => SplitViewDisplayMode.CompactOverlay,
            _ => SplitViewDisplayMode.CompactInline
        };
        QueueMetrics();
    }

    private void OnPlacementChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (PaneSample is null || sender is not ComboBox { SelectedIndex: >= 0 and <= 3 } picker) return;
        PaneSample.PanePlacement = (SplitViewPanePlacement)picker.SelectedIndex;
        QueueMetrics();
    }

    private void OnPaneClosing(object? sender, CancelRoutedEventArgs args)
    {
        args.Cancel = KeepPane?.IsChecked == true;
    }

    private void OnPaneSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        QueueMetrics();
    }

    private void OnPaneLast(object? sender, RoutedEventArgs args)
    {
        PaneList.SelectedIndex = NavigationItems.Count - 1;
        PaneList.ScrollIntoView(PaneList.SelectedIndex);
        QueueMetrics();
    }

    private void OnResetLayout(object? sender, RoutedEventArgs args)
    {
        ResizableGrid.ColumnDefinitions[0].Width = new GridLength(2, GridUnitType.Star);
        ResizableGrid.ColumnDefinitions[2].Width = new GridLength(3, GridUnitType.Star);
        RowGrid.RowDefinitions[0].Height = new GridLength(3, GridUnitType.Star);
        RowGrid.RowDefinitions[2].Height = new GridLength(2, GridUnitType.Star);
        QueueMetrics();
    }

    private void SplitterBoundsChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property == BoundsProperty) QueueMetrics();
    }

    private void OnLayoutChanged(object? sender, SizeChangedEventArgs args)
    {
        QueueMetrics();
    }

    private void QueueMetrics()
    {
        if (_metricsQueued) return;
        _metricsQueued = true;
        Dispatcher.UIThread.Post(() =>
        {
            _metricsQueued = false;
            if (this.IsAttachedToVisualTree()) UpdateMetrics();
        }, DispatcherPriority.Background);
    }

    public void UpdateMetrics()
    {
        PaneStatus.Text =
            $"{PaneSample.DisplayMode} · {PaneSample.PanePlacement} · {(PaneSample.IsPaneOpen ? "已展开" : "已收起")} · 展开 {PaneSample.OpenPaneLength:0} / 紧凑 {PaneSample.CompactPaneLength:0} px";
        SelectedWorkspace.Text = $"已选择：{PaneList.SelectedItem}";
        PaneMetrics.Text =
            $"侧栏容器 {PaneList.GetRealizedContainers().Count()} / {NavigationItems.Count:N0} · 选择 {PaneList.SelectedIndex + 1:N0}";
        LayoutStatus.Text =
            $"列宽 {ResizableGrid.ColumnDefinitions[0].ActualWidth:0} / {ResizableGrid.ColumnDefinitions[2].ActualWidth:0} px · 行高 {RowGrid.RowDefinitions[0].ActualHeight:0} / {RowGrid.RowDefinitions[2].ActualHeight:0} px · Esc 取消预览拖动";
    }
}