using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Threading;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class OneWindowDemoView : UserControl
{
    public OneWindowDemoView() => InitializeComponent();
    private void OnNotification(object? sender, RoutedEventArgs args) { OneNotification.Show("窗口通知", "通过静态 API 显示，四秒后自动关闭。", NotificationType.Success); WindowStatus.Text = "已显示通知"; }
    private void OnToast(object? sender, RoutedEventArgs args) { OneToast.Show("保存成功", NotificationType.Success); WindowStatus.Text = "已显示 Toast"; }
    private async void OnDialog(object? sender, RoutedEventArgs args) => WindowStatus.Text = $"结果：{await OneDialog.ShowAsync("确认操作", "是否继续当前操作？", OneDialogButtons.YesNoCancel)}";
    private async void OnFormDialog(object? sender, RoutedEventArgs args)
    {
        var vm = new DialogEditorViewModel();
        var options = new DialogOptions
        {
            Title = "编辑工作区", ShowTitle = DialogTitleVisible.IsChecked == true,
            ShowCloseButton = DialogCloseVisible.IsChecked == true,
            CloseOnOverlayClick = DialogOverlayClose.IsChecked == true,
            CloseOnEscape = DialogEscapeClose.IsChecked == true
        };
        var result = await OneDialog.ShowAsync<DialogEditorView, DialogEditorViewModel>(vm, options);
        WindowStatus.Text = $"VM 表单：{result} · {vm.Name}";
    }
    private async void OnQueue(object? sender, RoutedEventArgs args)
    {
        var first = OneDialog.ShowAsync("第一条", "关闭后显示第二条。", OneDialogButtons.OkCancel);
        var second = OneDialog.ShowAsync("第二条", "同一窗口只显示一个模态对话框。");
        WindowStatus.Text = $"队列：{await first} / {await second}";
    }
    private async void OnCancelDialog(object? sender, RoutedEventArgs args)
    {
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        try { WindowStatus.Text = $"取消测试：{await OneDialog.ShowAsync("可取消对话框", "三秒后取消等待，也可提前关闭。", OneDialogButtons.OkCancel, cancellationToken: cancellation.Token)}"; }
        catch (OperationCanceledException) { WindowStatus.Text = "已通过取消令牌关闭"; }
    }
    private void OnSecondWindow(object? sender, RoutedEventArgs args)
    {
        var window = new OneWindow { Title = "第二个 OneWindow", Width = 680, Height = 420, LeftContent = new TextBlock { Text = "独立窗口" } };
        var button = new Button { Content = "调用默认 Toast API" }; button.Click += (_, _) => OneToast.Show("自动定位到最近激活的窗口");
        window.Content = new StackPanel { Margin = new global::Avalonia.Thickness(24), Spacing = 16, Children = { new TextBlock { Text = "无需注册任何宿主。" }, button } };
        window.Show(); WindowStatus.Text = "已打开第二个窗口，可切换激活窗口后测试消息目标";
    }
    private void OnTitleOptions(object? sender, RoutedEventArgs args)
    {
        if (TopLevel.GetTopLevel(this) is OneWindow window) { window.ShowTitleBar = WindowTitleVisible.IsChecked == true; window.ShowThemeButton = WindowThemeVisible.IsChecked == true; }
    }
    private void OnLoginWindow(object? sender, RoutedEventArgs args)
    {
        var window = LoginDemo.CreateWindow();
        if (TopLevel.GetTopLevel(this) is Window owner) window.Show(owner); else window.Show();
        WindowStatus.Text = "已打开自定义登录内容；完整主窗体交接请使用 --login 启动。";
    }
    private void OnResetTitle(object? sender, RoutedEventArgs args) { WindowTitleVisible.IsChecked = true; WindowThemeVisible.IsChecked = true; OnTitleOptions(sender, args); }
}
