using Avalonia;
using Avalonia.Controls;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

internal sealed class WindowControlDemoView : UserControl
{
    private readonly bool _login;
    private bool _closed;
    internal Window Sample { get; private set; }
    internal event EventHandler? SampleChanged;
    internal WindowControlDemoView(bool login)
    {
        _login = login; Sample = CreateWindow();
        var open = new Button { Name = "OpenOwnedWindow", Content = login ? "打开 LoginWindow" : "打开 OneWindow", Classes = { "primary" } };
        open.Click += (_, _) => { if (TopLevel.GetTopLevel(this) is Window owner) { if (_closed) ReplaceSample(); Sample.Show(owner); } };
        Content = new StackPanel { Spacing = 16, Children =
        {
            new TextBlock { Text = login ? "LoginWindow · 登录与错误反馈" : "OneWindow · 窗口与消息宿主", FontSize = 24 },
            new TextBlock { Text = "窗口使用当前 PreviewHost 作为 owner；消息使用示例窗口自身作为 owner。关闭后可重新打开，不修改应用登录状态或主窗口生命周期。", TextWrapping = global::Avalonia.Media.TextWrapping.Wrap },
            new WrapPanel { Children = { open } },
            new PreviewCodeBlock { Text = login ? "var login = new LoginWindow();\nlogin.Show(owner);\nlogin.ShowErrorToast(\"请检查账号与密码\");" : "var window = new OneWindow();\nwindow.Show(owner);\nOneToast.Show(\"已保存\", owner: window);" }
        } };
        DetachedFromVisualTree += (_, _) => Sample.Close();
        AttachedToVisualTree += (_, _) => { if (_closed) ReplaceSample(); };
    }
    private void ReplaceSample() { Sample = CreateWindow(); SampleChanged?.Invoke(this, EventArgs.Empty); }
    private Window CreateWindow()
    {
        _closed = false;
        Window window = _login ? new LoginWindow() : new OneWindow();
        window.Title = _login ? "LoginWindow 示例" : "OneWindow 示例";
        window.Width = 640; window.Height = 480;
        if (_login) window.Content = new LoginDemoView();
        else
        {
            var toast = new Button { Content = "Toast" }; toast.Click += (_, _) => OneToast.Show("操作已完成", owner: window);
            var notification = new Button { Content = "Notification", Margin = new Thickness(8, 0) };
            notification.Click += (_, _) => OneNotification.Show("示例通知", "通知显示在这个窗口内。", owner: window);
            var dialog = new Button { Content = "Dialog" }; dialog.Click += async (_, _) => await OneDialog.ShowAsync("确认", "确认本次操作？", owner: window);
            window.Content = new StackPanel { Margin = new Thickness(24), Spacing = 16, Children =
            { new TextBlock { Text = "此窗口是三种消息的 owner。", TextWrapping = global::Avalonia.Media.TextWrapping.Wrap }, new WrapPanel { Children = { toast, notification, dialog } } } };
        }
        window.Closed += (_, _) => _closed = true;
        return window;
    }
}
