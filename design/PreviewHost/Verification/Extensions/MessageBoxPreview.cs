using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class MessageBoxPreview
{
    public static void Verify(Action<bool, string> check, string? output = null)
    {
        if (output is not null) Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>()) VerifyAppearance(theme, size, check, output);
        VerifyCommands(check, output);
        VerifyLifecycle(check);
    }

    private static void VerifyAppearance(ThemeVariant theme, ControlSize size, Action<bool, string> check, string? output)
    {
        var input = new TextBox { Text = "编辑内容", Margin = new Thickness(24) };
        var window = new OneWindow { Title = "MessageBox", Width = 760, Height = 480, Content = input, RequestedThemeVariant = theme };
        ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); Flush(); input.Focus();
        var task = OneMessageBox.ConfirmAsync("确认提交更改？", "提交后，最新配置将应用到当前工作区。请确认是否继续。", owner: window);
        Flush(); var presenter = Presenter(window); var cancel = Find<Button>(presenter, "MessageBoxCancel"); var confirm = Find<Button>(presenter, "MessageBoxConfirm");
        var label = $"MessageBox/{theme.Key}/{size}";
        check(window.IsDialogOpen && !input.IsEffectivelyEnabled && confirm.IsEffectivelyEnabled, $"{label}: confirmation uses the existing modal window host");
        check(window.FocusManager!.GetFocusedElement() == cancel, $"{label}: cancel receives safe initial keyboard focus");
        check(ControlSizing.GetSize(confirm) == size && confirm.FontSize == (size == ControlSize.Small ? 12 : size == ControlSize.Large ? 16 : 14), $"{label}: buttons inherit the owner density");
        check(Find<TextBlock>(presenter, "MessageBoxTitle").FontSize == (size == ControlSize.Small ? 16 : size == ControlSize.Large ? 20 : 18), $"{label}: heading follows the three typography densities");
        var symbol = presenter.GetVisualDescendants().OfType<Border>().Single(border => border.Classes.Contains("message-symbol"));
        check(symbol.Bounds.Width == (size == ControlSize.Small ? 32 : size == ControlSize.Large ? 44 : 40), $"{label}: symbol follows the three layout densities");
        check(Find<TextBlock>(presenter, "MessageBoxMessage").Text!.Contains("最新配置") && confirm.Command is not null && cancel.Command is not null,
            $"{label}: default content and compiled command bindings are available");
        Save(window, output, $"messagebox-{theme.Key}-{size}-wide");
        Click(window, confirm); Wait(task);
        check(task.GetAwaiter().GetResult() && !window.IsDialogOpen && window.FocusManager.GetFocusedElement() == input,
            $"{label}: pointer confirmation returns true and restores prior focus");

        window.Width = 360; Flush();
        task = OneMessageBox.ConfirmAsync("确认删除当前文件？", "这个文件以及关联的预览图将被删除，删除后无法恢复。", new MessageBoxOptions { IsDangerous = true, ConfirmText = "确认删除", CancelText = "保留文件" }, window);
        Flush(); presenter = Presenter(window); cancel = Find<Button>(presenter, "MessageBoxCancel"); confirm = Find<Button>(presenter, "MessageBoxConfirm");
        var title = Find<TextBlock>(presenter, "MessageBoxTitle"); var body = Find<TextBlock>(presenter, "MessageBoxMessage");
        check(confirm.Classes.Contains("danger") && window.FocusManager!.GetFocusedElement() == cancel, $"{label}: dangerous confirmation has semantic styling and retains cancel focus");
        check(IsInside(window, confirm) && IsInside(window, cancel) && IsInside(window, title) && IsInside(window, body)
            && title.TextWrapping == TextWrapping.Wrap && body.TextWrapping == TextWrapping.Wrap,
            $"{label}: narrow confirmation keeps actions and wrapped copy within the window");
        Save(window, output, $"messagebox-{theme.Key}-{size}-narrow");
        PressKey(window, Key.Enter, PhysicalKey.Enter); Wait(task);
        check(!task.GetAwaiter().GetResult() && !window.IsDialogOpen, $"{label}: Enter on the initial cancel button returns false");
        window.Close(); Flush();
    }

    private static void VerifyCommands(Action<bool, string> check, string? output)
    {
        var page = new MessageBoxDemoView(); var window = new OneWindow { Title = "MessageBox · PreviewHost", Width = 920, Height = 820,
            Content = new ScrollViewer { Content = page, Margin = new Thickness(24) } };
        Motion.SetEnabled(window, false); window.Show(); Flush();
        Save(window, output, "messagebox-page");
        var open = Find<Button>(page, "OpenMessageBox"); Click(window, open);
        check(window.IsDialogOpen && !page.Model.ConfirmCommand.CanExecute(null), "MessageBox: the page opens from a ViewModel command and gates duplicate execution");
        var confirm = Find<Button>(Presenter(window), "MessageBoxConfirm"); confirm.Focus(); PressKey(window, Key.Enter, PhysicalKey.Enter);
        PumpUntil(() => page.Model.ConfirmedCount == 1);
        check(page.Model.ConfirmedCount == 1 && page.Model.Status.Contains("true"), "MessageBox: native Enter confirmation flows back to the ViewModel business command");
        Click(window, open); PressKey(window, Key.Escape, PhysicalKey.Escape); PumpUntil(() => page.Model.Status.Contains("false"));
        check(page.Model.ConfirmedCount == 1 && page.Model.Status.Contains("false") && !window.IsDialogOpen, "MessageBox: Escape returns false without running confirmed work");
        page.Model.Title = ""; Flush(); check(!open.IsEffectivelyEnabled, "MessageBox: invalid empty input disables the configured example command");
        window.Close(); Flush();
    }

    private static void VerifyLifecycle(Action<bool, string> check)
    {
        var window = new OneWindow { Width = 700, Height = 440 }; window.Show(); Flush();
        var first = OneMessageBox.ConfirmAsync("第一个确认", "保持当前确认框", owner: window);
        var second = OneMessageBox.ConfirmAsync("第二个确认", "等待前一个完成", new MessageBoxOptions { Size = ControlSize.Small }, window);
        Flush();
        check(window.GetVisualDescendants().OfType<OneDialogPresenter>().Count() == 1 && !second.IsCompleted, "MessageBox: confirmations share the existing per-window dialog queue");
        var shield = window.GetVisualDescendants().OfType<Panel>().Single(control => control.Name == "PART_DialogShield");
        var outside = shield.TranslatePoint(new Point(4, 4), window)!.Value;
        window.MouseDown(outside, MouseButton.Left); window.MouseUp(outside, MouseButton.Left); Flush();
        check(!first.IsCompleted, "MessageBox: overlay clicks do not confirm or dismiss the request");
        Click(window, Find<Button>(Presenter(window), "MessageBoxCancel")); Wait(first); Flush();
        check(!first.GetAwaiter().GetResult() && Find<TextBlock>(Presenter(window), "MessageBoxTitle").Text == "第二个确认"
            && ControlSizing.GetSize(Find<Button>(Presenter(window), "MessageBoxConfirm")) == ControlSize.Small,
            "MessageBox: pointer cancellation returns false and advances the queue with an explicit density override");
        window.Close(); Flush(); Wait(second);
        check(!second.GetAwaiter().GetResult(), "MessageBox: owner closure completes pending confirmation with false");

        window = new OneWindow { Width = 700, Height = 440 }; window.Show(); Flush();
        using var cancellation = new CancellationTokenSource();
        var cancelled = OneMessageBox.ConfirmAsync("取消等待", "由调用者取消", owner: window, cancellationToken: cancellation.Token); Flush(); cancellation.Cancel(); Flush(); Wait(cancelled);
        check(cancelled.IsCanceled && !window.IsDialogOpen, "MessageBox: cancellation tokens cancel the task and release modal state");
        window.Close(); Flush();

        var login = new LoginWindow { Width = 700, Height = 440 }; login.Show(); Flush();
        var loginConfirmation = OneMessageBox.ConfirmAsync("离开登录？", "确认返回之前的页面。", owner: login); Flush();
        check(login.IsDialogOpen && Find<Button>(Presenter(login), "MessageBoxConfirm").IsEffectivelyEnabled,
            "MessageBox: LoginWindow reuses its embedded dialog host");
        Click(login, Find<Button>(Presenter(login), "MessageBoxConfirm")); Wait(loginConfirmation);
        check(loginConfirmation.GetAwaiter().GetResult() && !login.IsDialogOpen, "MessageBox: explicit LoginWindow confirmation returns true");
        login.Close(); Flush();
    }

    private static OneDialogPresenter Presenter(Window window) => window.GetVisualDescendants().OfType<OneDialogPresenter>().Single();
    private static T Find<T>(Control owner, string name) where T : Control => owner.GetVisualDescendants().OfType<T>().Single(control => control.Name == name);
    private static bool IsInside(Window window, Control control)
    {
        var origin = control.TranslatePoint(default, window)!.Value;
        return origin.X >= 0 && origin.Y >= 0 && origin.X + control.Bounds.Width <= window.Bounds.Width + 1 && origin.Y + control.Bounds.Height <= window.Bounds.Height + 1;
    }
    private static void Click(Window window, Control control)
    {
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
        window.MouseMove(point); window.MouseDown(point, MouseButton.Left); window.MouseUp(point, MouseButton.Left); Flush();
    }
    private static void PressKey(Window window, Key key, PhysicalKey physical)
    {
        window.KeyPress(key, RawInputModifiers.None, physical, null); window.KeyRelease(key, RawInputModifiers.None, physical, null); Flush();
    }
    private static void Wait(Task task) => PumpUntil(() => task.IsCompleted);
    private static void PumpUntil(Func<bool> condition)
    {
        var stopwatch = Stopwatch.StartNew();
        while (!condition() && stopwatch.ElapsedMilliseconds < 2000)
        {
            var frame = new DispatcherFrame(); var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
            timer.Tick += (_, _) => { timer.Stop(); frame.Continue = false; }; timer.Start(); Dispatcher.UIThread.PushFrame(frame); Flush();
        }
        if (!condition()) throw new InvalidOperationException("MessageBox asynchronous operation did not complete.");
    }
    private static void Save(Window window, string? output, string name)
    {
        if (output is null) return;
        using var bitmap = window.CaptureRenderedFrame()!; bitmap.Save(Path.Combine(output, name + ".png"), PngBitmapEncoderOptions.Default);
    }
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
}
