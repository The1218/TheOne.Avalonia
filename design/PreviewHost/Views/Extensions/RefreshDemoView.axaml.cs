using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

public partial class RefreshDemoView : UserControl
{
    public const int DataCount = 1000;
    private readonly List<RefreshCompletionDeferral> _deferrals = [];
    private RefreshVisualizer? _observed;
    private bool _shouldFail;
    private bool _syncingMotion;
    private DispatcherTimer? _timer;

    public RefreshDemoView()
    {
        DataContext = Model;
        InitializeComponent();
        Model.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(RefreshDemoViewModel.MotionEnabled) && !_syncingMotion)
            {
                Motion.SetEnabled(RefreshSample, Model.MotionEnabled);
                UpdateMetrics();
            }
        };
        RefreshSample.PropertyChanged += (_, args) =>
        {
            if (args.Property == RefreshContainer.VisualizerProperty) ObserveVisualizer();
            if (args.Property == Motion.EnabledProperty) UpdateMetrics();
        };
        Loaded += (_, _) => ObserveVisualizer();
        LayoutUpdated += (_, _) => UpdateMetrics();
        DetachedFromVisualTree += (_, _) => ReleaseDeferrals();
    }

    public RefreshDemoViewModel Model { get; } = new();

    public int RequestCount
    {
        get => Model.RequestCount;
        private set => Model.RequestCount = value;
    }

    public int Revision
    {
        get => Model.Revision;
        private set => Model.Revision = value;
    }

    public int FailureCount
    {
        get => Model.FailureCount;
        private set => Model.FailureCount = value;
    }

    public bool IsRefreshing => _deferrals.Count > 0;

    public IReadOnlyList<RefreshRow> Rows
    {
        get => Model.Rows;
        private set => Model.Rows = value;
    }

    private void ObserveVisualizer()
    {
        if (_observed is not null) _observed.PropertyChanged -= OnVisualizerChanged;
        _observed = RefreshSample.Visualizer;
        if (_observed is not null) _observed.PropertyChanged += OnVisualizerChanged;
        UpdateMetrics();
    }

    private void OnVisualizerChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property == RefreshVisualizer.RefreshVisualizerStateProperty) UpdateMetrics();
    }

    private void OnOptionsChanged(object? sender, RoutedEventArgs args)
    {
        if (RefreshSample is null) return;
        RefreshSample.IsMouseEnabled = Model.MousePull;
        RefreshSample.IsEnabled = Model.Enabled;
        UpdateMetrics();
    }

    private void OnRequestRefresh(object? sender, RoutedEventArgs args)
    {
        RefreshSample.RequestRefresh();
    }

    private void OnMotionChanged(object? sender, RoutedEventArgs args)
    {
        if (RefreshSample is null || _syncingMotion) return;
        Motion.SetEnabled(RefreshSample, Model.MotionEnabled);
        UpdateMetrics();
    }

    private void OnRefreshRequested(object? sender, RefreshRequestedEventArgs args)
    {
        _deferrals.Add(args.GetDeferral());
        if (_deferrals.Count > 1) return;
        RequestCount++;
        _shouldFail = Model.Fail;
        FailNextRefresh.IsChecked = false;
        Model.Status = $"正在刷新 · 请求 {RequestCount} 次";
        UpdateMetrics();
        if (Model.Manual) return;
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
        _timer.Tick += (_, _) => FinishRefresh();
        _timer.Start();
    }

    private void OnCompleteRefresh(object? sender, RoutedEventArgs args)
    {
        FinishRefresh();
    }

    public void FinishRefresh()
    {
        if (!IsRefreshing) return;
        try
        {
            if (_shouldFail)
            {
                FailureCount++;
                Model.Status = $"刷新失败 · 原数据已保留，可再次刷新（失败 {FailureCount} 次）";
            }
            else
            {
                var selectedNumber = (RefreshList.SelectedItem as RefreshRow)?.Number;
                var scroll = RefreshList.GetVisualDescendants().OfType<ScrollViewer>().First();
                var offset = scroll.Offset;
                Rows = RefreshDemoViewModel.CreateRows(++Revision);
                Model.SelectedIndex = selectedNumber is { } number ? number - 1 : -1;
                Dispatcher.UIThread.Post(() =>
                {
                    if (this.IsAttachedToVisualTree())
                    {
                        scroll.Offset = offset;
                        UpdateMetrics();
                    }
                }, DispatcherPriority.Loaded);
                Model.Status = $"已完成第 {Revision} 次更新 · {DataCount:N0} 条记录";
            }
        }
        finally
        {
            ReleaseDeferrals();
            UpdateMetrics();
        }
    }

    private void ReleaseDeferrals()
    {
        _timer?.Stop();
        _timer = null;
        var pending = _deferrals.ToArray();
        _deferrals.Clear();
        foreach (var deferral in pending) deferral.Complete();
    }

    private void OnFirstRow(object? sender, RoutedEventArgs args)
    {
        RefreshList.ScrollIntoView(0);
    }

    private void OnLastRow(object? sender, RoutedEventArgs args)
    {
        Model.SelectedIndex = DataCount - 1;
        RefreshList.ScrollIntoView(DataCount - 1);
        Dispatcher.UIThread.Post(UpdateMetrics, DispatcherPriority.Loaded);
    }

    private void OnReset(object? sender, RoutedEventArgs args)
    {
        ReleaseDeferrals();
        Revision = RequestCount = FailureCount = 0;
        RefreshSample.ClearValue(Motion.EnabledProperty);
        Rows = RefreshDemoViewModel.CreateRows(0);
        Model.SelectedIndex = -1;
        RefreshList.ScrollIntoView(0);
        Model.Enabled = Model.MousePull = true;
        Model.Manual = Model.Fail = false;
        Model.Status = "尚未刷新";
        UpdateMetrics();
    }

    public void UpdateMetrics()
    {
        if (RefreshSample is null || RefreshMetrics is null || RefreshStateLabel is null) return;
        _syncingMotion = true;
        try
        {
            Model.MotionEnabled = Motion.GetEnabled(RefreshSample);
        }
        finally
        {
            _syncingMotion = false;
        }

        var state = RefreshSample.Visualizer?.GetValue(RefreshVisualizer.RefreshVisualizerStateProperty) ??
                    RefreshVisualizerState.Idle;
        Model.State = state switch
        {
            RefreshVisualizerState.Interacting => "继续下拉", RefreshVisualizerState.Pending => "释放以刷新",
            RefreshVisualizerState.Refreshing => "正在刷新", RefreshVisualizerState.Peeking => "回到顶部后下拉", _ => "等待下拉"
        };
        Model.Metrics =
            $"已创建 {RefreshList.GetRealizedContainers().Count()} / {DataCount:N0} · 请求 {RequestCount} 次 · 更新 {Revision} 次 · {state} · {(Motion.GetEnabled(RefreshSample) ? "动效开启" : "静止提示")}";
        RequestRefresh.IsEnabled = RefreshSample.IsEnabled && !IsRefreshing;
        CompleteRefresh.IsEnabled = IsRefreshing;
    }
}