using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace TheOne.DesignPreview;

public partial class FlyoutDemoView : UserControl
{
    private readonly DispatcherTimer _metricsTimer = new() { Interval = TimeSpan.FromMilliseconds(200) };
    private bool _allowClose;

    public FlyoutDemoView()
    {
        DataContext = Model;
        InitializeComponent();
        EditorFlyout.Opening += (_, _) => { Model.Name = AppliedName; };
        EditorFlyout.Closing += OnClosing;
        EditorFlyout.Opened += (_, _) =>
        {
            _metricsTimer.Start();
            UpdateMetrics();
        };
        EditorFlyout.Closed += (_, _) =>
        {
            _metricsTimer.Stop();
            Model.Metrics = "弹层已关闭";
        };
        Configure(EditorFlyout);
        Configure(ListFlyout);
        ListFlyout.Opened += (_, _) =>
        {
            _metricsTimer.Start();
            UpdateMetrics();
        };
        ListFlyout.Closed += (_, _) => _metricsTimer.Stop();
        _metricsTimer.Tick += (_, _) => UpdateMetrics();
        DetachedFromVisualTree += (_, _) =>
        {
            _metricsTimer.Stop();
            Close(EditorFlyout);
            ListFlyout.Hide();
        };
    }

    private Flyout EditorFlyout => (Flyout)OpenEditor.Flyout!;
    private Flyout ListFlyout => (Flyout)OpenList.Flyout!;
    public FlyoutDemoViewModel Model { get; } = new();
    public IReadOnlyList<string> Rows => Model.Rows;
    public int ApplyCount => Model.ApplyCount;
    public string AppliedName => Model.AppliedName;

    private void Configure(Flyout flyout)
    {
        flyout.Placement = PlacementPicker.SelectedIndex switch
        {
            1 => PlacementMode.Right, 2 => PlacementMode.Top, 3 => PlacementMode.Left, _ => PlacementMode.Bottom
        };
        flyout.ShowMode = ShowModePicker.SelectedIndex == 1 ? FlyoutShowMode.Transient : FlyoutShowMode.Standard;
        flyout.FlyoutPresenterClasses.Clear();
        if (FlyoutSizePicker.SelectedIndex > 0)
            flyout.FlyoutPresenterClasses.Add(
                FlyoutSizePicker.SelectedIndex switch { 1 => "small", 3 => "large", _ => "normal" });
    }

    private void OnOptionsChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (OpenEditor?.Flyout is not Flyout || OpenList?.Flyout is not Flyout) return;
        // Native presenter classes are applied on opening; close first so the next open uses all options.
        Close(EditorFlyout);
        ListFlyout.Hide();
        Configure(EditorFlyout);
        Configure(ListFlyout);
    }

    private void OnClosing(object? sender, CancelEventArgs args)
    {
        if (Model.PreventDismiss && !_allowClose)
        {
            args.Cancel = true;
            Model.Status = "已阻止关闭；请在弹层内应用或取消。";
        }
    }

    private void Close(Flyout flyout)
    {
        _allowClose = true;
        try
        {
            flyout.Hide();
        }
        finally
        {
            _allowClose = false;
        }
    }

    private void OnApply(object? sender, RoutedEventArgs args)
    {
        if (!Model.Apply())
        {
            WorkspaceName.Focus();
            return;
        }

        Close(EditorFlyout);
    }

    private void OnCancel(object? sender, RoutedEventArgs args)
    {
        Close(EditorFlyout);
        Model.Status = "已取消编辑";
    }

    private void OnJumpLast(object? sender, RoutedEventArgs args)
    {
        Model.SelectedIndex = Rows.Count - 1;
        MemberList.ScrollIntoView(MemberList.SelectedIndex);
        Dispatcher.UIThread.Post(() =>
        {
            MemberList.ContainerFromIndex(MemberList.SelectedIndex)?.Focus();
            UpdateMetrics();
        }, DispatcherPriority.Background);
    }

    private void OnMemberChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (MemberStatus is not null) Model.MemberStatus = $"当前选择：{MemberList.SelectedItem ?? "未选择"}";
    }

    private void OnConfirmMember(object? sender, RoutedEventArgs args)
    {
        Model.MemberStatus = $"已确认：{MemberList.SelectedItem ?? "未选择"}";
        ListFlyout.Hide();
    }

    public void UpdateMetrics()
    {
        if (EditorFlyout.Popup.Child is FlyoutPresenter presenter && EditorFlyout.IsOpen)
            Model.Metrics =
                $"{presenter.ActualThemeVariant.Key} · 字号 {presenter.FontSize:0} · 内边距 {presenter.Padding.Left:0} · {presenter.Bounds.Width:0} × {presenter.Bounds.Height:0}";
        if (ListFlyout.IsOpen)
            Model.ListMetrics =
                $"已创建 {MemberList.GetRealizedContainers().Count()} / {Rows.Count:N0} · 第 {MemberList.SelectedIndex + 1:N0} 项";
    }

    private void OnReset(object? sender, RoutedEventArgs args)
    {
        Close(EditorFlyout);
        ListFlyout.Hide();
        Model.Reset();
        MemberList.ScrollIntoView(0);
    }
}