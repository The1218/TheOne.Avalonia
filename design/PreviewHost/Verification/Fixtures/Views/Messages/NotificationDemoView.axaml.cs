using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

public partial class NotificationDemoView : UserControl
{
    public WindowNotificationManager Manager => NotificationManager;
    public NotificationCard[] Cards => Manager.GetVisualDescendants().OfType<NotificationCard>().ToArray();
    public NotificationDemoViewModel Model { get; } = new();
    public int ShownCount { get => Model.ShownCount; private set => Model.ShownCount = value; }
    public int ClosedCount { get => Model.ClosedCount; private set => Model.ClosedCount = value; }
    public int ClickedCount { get => Model.ClickedCount; private set => Model.ClickedCount = value; }
    private bool _syncingMotion;
    private DispatcherTimer? _metricsTimer;
    private int _generation;
    public NotificationDemoView()
    {
        DataContext = Model; InitializeComponent();
        Model.PropertyChanged += (_, args) => { if (args.PropertyName == nameof(NotificationDemoViewModel.MotionEnabled) && !_syncingMotion) { Motion.SetEnabled(Manager, Model.MotionEnabled); UpdateMetrics(); } };
        Manager.PropertyChanged += (_, args) => { if (args.Property == Motion.EnabledProperty) UpdateMetrics(); };
        Loaded += (_, _) =>
        {
            UpdateMetrics();
            if (_metricsTimer is not null) return;
            _metricsTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
            _metricsTimer.Tick += (_, _) => { if (IsEffectivelyVisible) UpdateMetrics(); }; _metricsTimer.Start();
        };
        DetachedFromVisualTree += (_, _) => { _generation++; _metricsTimer?.Stop(); _metricsTimer = null; };
    }
    private void OnMotionChanged(object? sender, RoutedEventArgs args)
    {
        if (Manager is null || _syncingMotion) return;
        Motion.SetEnabled(Manager, Model.MotionEnabled); UpdateMetrics();
    }
    public Notification ShowNotification(NotificationType type, TimeSpan? expiration = null)
    {
        var generation = _generation; var number = ++ShownCount;
        var (title, message) = type switch
        {
            NotificationType.Success => ("保存成功", "资料已保存，可以继续当前操作。"),
            NotificationType.Warning => ("需要留意", "部分资料尚未同步，请稍后检查。"),
            NotificationType.Error => ("操作未完成", "连接暂时不可用，请稍后重试。"),
            _ => ("工作区提醒", "有新的更新，当前内容已经保留。")
        };
        var duration = expiration ?? (Model.Duration);
        var notification = new Notification($"{title} · {number}", message, type, duration,
            onClick: () => { if (generation == _generation && this.IsAttachedToVisualTree()) { ClickedCount++; Model.Status = $"已点击通知 {number}"; UpdateMetrics(); } },
            onClose: () =>
            {
                if (generation != _generation || !this.IsAttachedToVisualTree()) return;
                ClosedCount++; Model.Status = $"已关闭通知 {number} · {ClosedCount} 条";
                Dispatcher.UIThread.Post(UpdateMetrics, DispatcherPriority.Background);
            });
        Manager.Show(notification); Model.Status = $"已显示：{title} · {(duration == TimeSpan.Zero ? "手动关闭" : $"{duration.TotalSeconds:0.0} 秒后自动关闭")}";
        UpdateMetrics(); return notification;
    }
    private void OnInformation(object? sender, RoutedEventArgs args) => ShowNotification(NotificationType.Information);
    private void OnSuccess(object? sender, RoutedEventArgs args) => ShowNotification(NotificationType.Success);
    private void OnWarning(object? sender, RoutedEventArgs args) => ShowNotification(NotificationType.Warning);
    private void OnError(object? sender, RoutedEventArgs args) => ShowNotification(NotificationType.Error);
    private void OnShowAll(object? sender, RoutedEventArgs args) { foreach (var type in Enum.GetValues<NotificationType>()) ShowNotification(type); }
    private void OnCloseOldest(object? sender, RoutedEventArgs args) => Cards.FirstOrDefault(card => !card.IsClosing)?.Close();
    private void OnCloseAll(object? sender, RoutedEventArgs args) => Manager.CloseAll();
    private void OnReset(object? sender, RoutedEventArgs args)
    {
        _generation++; Motion.SetEnabled(Manager, false); Manager.CloseAll(); Manager.ClearValue(Motion.EnabledProperty);
        ShownCount = ClosedCount = ClickedCount = 0; AutoCloseNotifications.IsChecked = false; NotificationSeconds.Value = 2;
        NotificationPositionPicker.SelectedIndex = 1; Model.Status = "尚未显示通知"; UpdateMetrics();
    }
    public void UpdateMetrics()
    {
        if (Manager is null || NotificationMetrics is null || NotificationMotionEnabled is null) return;
        _syncingMotion = true;
        try { Model.MotionEnabled = Motion.GetEnabled(Manager); }
        finally { _syncingMotion = false; }
        var cards = Cards;
        Model.Metrics = $"显示 {ShownCount} 次 · 关闭 {ClosedCount} 次 · 点击 {ClickedCount} 次 · 当前 {cards.Count(card => !card.IsClosing)} / {Manager.MaxItems} · 关闭中 {cards.Count(card => card.IsClosing)} · {(Motion.GetEnabled(Manager) ? "动效开启" : "直接显示与关闭")}";
        CloseOldestNotification.IsEnabled = CloseAllNotifications.IsEnabled = cards.Any(card => !card.IsClosing);
    }
}
