using Avalonia.Controls;
using Avalonia.Interactivity;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class LoginDemoView : UserControl
{
    public LoginDemoView() => global::Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
    private void ShowErrorToast(object? sender, RoutedEventArgs args)
        => (TopLevel.GetTopLevel(this) as LoginWindow)?.ShowErrorToast("账号或密码错误，请重新输入。");
    private void ShowErrorNotification(object? sender, RoutedEventArgs args)
        => (TopLevel.GetTopLevel(this) as LoginWindow)?.ShowErrorNotification("暂时无法连接认证服务，请稍后重试。");
    private async void ShowErrorDialog(object? sender, RoutedEventArgs args)
    {
        if (TopLevel.GetTopLevel(this) is LoginWindow login)
            await login.ShowErrorDialog("登录校验未通过，请检查账号和密码。");
    }
}
