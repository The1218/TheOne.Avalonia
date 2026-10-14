using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;

namespace TheOne.DesignPreview;

public partial class SplitButtonDemoView : UserControl
{
    public SplitButtonDemoViewModel Model { get; } = new();
    private MenuFlyoutPresenter? _metricsPresenter;
    private bool _metricsQueued;
    private Control? _lastActionFocus;
    public SplitButtonDemoView()
    {
        DataContext = Model;
        InitializeComponent();
        foreach (var button in new SplitButton[] { ExportSplit, PublishSplit, AutoSplit, TemplateSplit })
            button.GotFocus += (_, args) => _lastActionFocus = args.Source as Control;
        // Sync commands briefly disable their split control; restore keyboard focus after
        // CanExecute returns, while allowing focus explicitly moved elsewhere to win.
        Model.MainActionCommand.CanExecuteChanged += (_, _) =>
        {
            if (!Model.MainActionCommand.CanExecute(null)) return;
            Dispatcher.UIThread.Post(() =>
            {
                var focused = _lastActionFocus;
                var top = TopLevel.GetTopLevel(this);
                if (focused is { IsEffectivelyEnabled: true } && top is not null && TopLevel.GetTopLevel(focused) == top && top.FocusManager?.GetFocusedElement() is null)
                    focused.Focus();
            }, DispatcherPriority.Background);
        };
        TemplateSplit.Flyout!.Opened += (_, _) =>
        {
            if (((PopupFlyoutBase)TemplateSplit.Flyout).Popup.Child is MenuFlyoutPresenter presenter && !ReferenceEquals(presenter, _metricsPresenter))
            {
                _metricsPresenter?.RemoveHandler(ScrollViewer.ScrollChangedEvent, OnMenuScrolled);
                _metricsPresenter = presenter;
                presenter.AddHandler(ScrollViewer.ScrollChangedEvent, OnMenuScrolled);
            }
            QueueMenuMetrics();
        };
        TemplateSplit.Flyout!.Closed += (_, _) => Model.MenuMetrics = "菜单已关闭 · 10,000 项按视口显示与回收";
    }
    private void OnMenuScrolled(object? sender, ScrollChangedEventArgs args) => QueueMenuMetrics();
    private void QueueMenuMetrics()
    {
        if (_metricsQueued) return;
        _metricsQueued = true;
        Dispatcher.UIThread.Post(() => { _metricsQueued = false; if (TemplateSplit.Flyout?.IsOpen == true) UpdateMenuMetrics(); }, DispatcherPriority.Background);
    }
    public void UpdateMenuMetrics()
    {
        if (TemplateSplit.Flyout is PopupFlyoutBase flyout && flyout.Popup.Child is MenuFlyoutPresenter presenter)
            Model.MenuMetrics = $"模板菜单容器 {presenter.GetRealizedContainers().Count()} / {presenter.ItemCount:N0}";
    }
}
