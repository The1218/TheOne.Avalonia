using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Material.Icons;
using Material.Icons.Avalonia;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class ButtonsMessagesPreview
{
    public static void Verify(Action<bool, string> check)
    {
        check(!PreviewPageCatalog.Controls.Any(page => page.Id is 21 or 22 or 303 or 304), "Catalogue: removed window/card/theme pages are absent from navigation and search");
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var buttonPage = new ButtonPage(); var iconPage = new IconButtonPage();
            var window = Open(new StackPanel { Spacing = 12, Children = { buttonPage, iconPage } }, theme, size);
            var label = $"Buttons/{theme.Key}/{size}";
            var icon = iconPage.FindControl<IconButton>("GeometryIcon")!;
            var presenter = Part<ContentPresenter>(icon, "PART_IconPresenter");
            check(icon.Bounds.Height >= (size == ControlSize.Small ? 30 : size == ControlSize.Large ? 44 : 36) && icon.IconSize == (size == ControlSize.Small ? 14 : size == ControlSize.Large ? 20 : 16), label + ": native button and icon follow density");
            check(presenter.GetVisualDescendants().OfType<PathIcon>().Any(path => ReferenceEquals(path.Data, icon.Icon)), label + ": bound geometry uses a PathIcon in the separate icon region");
            var material = iconPage.FindControl<IconButton>("MaterialIcon")!;
            check(material.GetVisualDescendants().OfType<MaterialIcon>().Single().Kind == MaterialIconKind.Home, label + ": compiled Material enum template renders the requested icon");
            var model = (IconButtonPageViewModel)iconPage.DataContext!;
            model.MaterialIcon = MaterialIconKind.Check; ControlPreview.Flush();
            check(material.GetVisualDescendants().OfType<MaterialIcon>().Single().Kind == MaterialIconKind.Check, label + ": Material kind binding updates at runtime");
            var control = iconPage.FindControl<IconButton>("PathIconControl")!;
            check(Part<ContentPresenter>(control, "PART_IconPresenter").Child is PathIcon && !Part<ContentPresenter>(control, "PART_LabelPresenter").IsVisible, label + ": control icons and icon-only layout work without Content markup");
            icon.Icon = null; ControlPreview.Flush(); check(!presenter.IsVisible, label + ": clearing icon removes its region and gap");
            icon.Icon = model.Icon; ControlPreview.Flush(); Click(icon);
            check(model.Status.Contains("1"), label + ": pointer activates the inherited Button command exactly once");
            foreach (var button in buttonPage.GetVisualDescendants().OfType<Button>().Where(item => item.GetType() == typeof(Button)))
            {
                var content = Part<ContentPresenter>(button, "PART_ContentPresenter");
                check(content.Bounds.Width > 0 && content.Foreground is SolidColorBrush, label + ": semantic button has measured themed content");
                if (button.IsEnabled) { Hover(button); Press(button); check(button.Classes.Contains(":pressed"), label + ": semantic variant retains pressed input"); Release(button); }
            }
            var themeButton = buttonPage.FindControl<ThemeButton>("ThemeSample")!; Click(themeButton);
            check(window.ActualThemeVariant != theme, label + ": relocated theme shortcut switches the owning window");
            window.Close(); ControlPreview.Flush();
            VerifyMessages(theme, size, check);
        }
        SplitButtonPreview.Verify();
    }
    private static void VerifyMessages(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var page = new ToastPage(); var window = Open(page, theme, size); var model = page.Model;
        model.Type = NotificationType.Error; model.Title = "Custom title"; model.Message = "Configured toast"; model.AutoClose = false;
        model.Position = NotificationPosition.BottomRight; model.Maximum = 2; model.Size = size; model.MotionEnabled = false;
        model.ToastCommand.Execute(null); ControlPreview.Flush();
        var host = Part<WindowNotificationManager>(window, "PART_Toasts");
        var card = host.GetVisualDescendants().OfType<NotificationCard>().Single();
        check(host.Position == NotificationPosition.BottomRight && host.MaxItems == 2 && ControlSizing.GetSize(card) == size && !Motion.GetEnabled(card), $"Messages/{theme.Key}/{size}: toast position, limit, density and motion reach the real host");
        check(card.Content is INotification { Title: "Custom title", Message: "Configured toast", Type: NotificationType.Error, Expiration: var expiration } && expiration == TimeSpan.Zero,
            $"Messages/{theme.Key}/{size}: title, type, content and persistent duration reach the native notification");
        model.CloseToastCommand.Execute(null); ControlPreview.Flush();
        check(model.CloseCount == 1, $"Messages/{theme.Key}/{size}: close callback updates the parent state");
        window.Close(); ControlPreview.Flush();
        var notificationPage = new OneNotificationPage(); window = Open(notificationPage, theme, size); model = notificationPage.Model;
        var settings = notificationPage.GetVisualDescendants().OfType<MessageDemoSettings>().Single();
        settings.FindControl<TextBox>("MessageTitle")!.Text = "Edited notification"; ControlPreview.Flush();
        model.Type = NotificationType.Warning; model.AutoClose = false; model.Position = NotificationPosition.TopLeft;
        model.NotificationCommand.Execute(null); ControlPreview.Flush();
        var notifications = Part<WindowNotificationManager>(window, "PART_Notifications");
        check(model.Title == "Edited notification" && notifications.Position == NotificationPosition.TopLeft && notifications.GetVisualDescendants().OfType<NotificationCard>().Single().Content is INotification { Type: NotificationType.Warning, Title: "Edited notification" },
            $"Messages/{theme.Key}/{size}: notification settings use compiled two-way binding and configure the native card");
        model.CloseNotificationCommand.Execute(null); ControlPreview.Flush();
        model.AutoClose = true; model.Seconds = .1m; model.MotionEnabled = false; model.NotificationCommand.Execute(null); ControlPreview.Flush();
        var frame = new global::Avalonia.Threading.DispatcherFrame();
        var timer = new global::Avalonia.Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
        timer.Tick += (_, _) => { timer.Stop(); frame.Continue = false; }; timer.Start(); global::Avalonia.Threading.Dispatcher.UIThread.PushFrame(frame); ControlPreview.Flush();
        check(model.CloseCount >= 2, $"Messages/{theme.Key}/{size}: configured expiration automatically closes and reports the notification");
        window.Close(); ControlPreview.Flush();
        var dialogPage = new DialogPage(); window = Open(dialogPage, theme, size); model = dialogPage.Model;
        model.ShowTitle = false; model.ShowClose = false; model.ShowButtons = true; model.Buttons = OneDialogButtons.YesNo;
        model.EscapeClose = true; model.Size = size; model.Type = NotificationType.Warning; model.MotionEnabled = false;
        model.DialogCommand.Execute(null); ControlPreview.Flush();
        var dialog = window.GetVisualDescendants().OfType<OneDialogPresenter>().Single();
        check(!dialog.ShowTitle && !dialog.ShowCloseButton && dialog.ShowButtons && dialog.Buttons == OneDialogButtons.YesNo && ControlSizing.GetSize(dialog) == size && !Motion.GetEnabled(dialog), $"Messages/{theme.Key}/{size}: dialog options configure the presenter and typed content");
        window.KeyPress(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null); window.KeyRelease(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
        for (var i = 0; i < 10; i++) ControlPreview.Flush();
        window.Close(); ControlPreview.Flush();
    }
    public static void Render(string output, string name)
    {
        Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            Control view = name switch { "IconButton" => new IconButtonPage(), "Toast" => new ToastPage(), "Dialog" => new DialogPage(), "Notification" => new OneNotificationPage(), _ => new ButtonPage() };
            var window = Open(view, theme, size);
            if (view is ToastPage toast) { toast.Model.AutoClose = false; toast.Model.MotionEnabled = false; toast.Model.Size = size; toast.Model.ToastCommand.Execute(null); }
            if (view is OneNotificationPage notification) { notification.Model.AutoClose = false; notification.Model.MotionEnabled = false; notification.Model.Size = size; notification.Model.NotificationCommand.Execute(null); }
            if (view is DialogPage dialog) { dialog.Model.Size = size; dialog.Model.Type = NotificationType.Warning; dialog.Model.DialogCommand.Execute(null); }
            ControlPreview.Flush();
            using var bitmap = new RenderTargetBitmap(new PixelSize(980, 850)); bitmap.Render(window);
            bitmap.Save(Path.Combine(output, $"{name}-{theme.Key}-{size}.png"), PngBitmapEncoderOptions.Default);
            window.Close(); ControlPreview.Flush();
        }
    }
    private static OneWindow Open(Control content, ThemeVariant theme, ControlSize size)
    {
        var window = new OneWindow { Width = 980, Height = 850, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme, Content = new ScrollViewer { Content = new Border { Padding = new Thickness(24), Child = content } } };
        ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); ControlPreview.Flush(); return window;
    }
    private static T Part<T>(Control control, string name) where T : Control => control.GetVisualDescendants().OfType<T>().Single(part => part.Name == name);
    private static Point Position(Control control) => control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), TopLevel.GetTopLevel(control)!)!.Value;
    private static void Hover(Control control) { TopLevel.GetTopLevel(control)!.MouseMove(Position(control)); ControlPreview.Flush(); }
    private static void Press(Control control) { TopLevel.GetTopLevel(control)!.MouseDown(Position(control), MouseButton.Left); ControlPreview.Flush(); }
    private static void Release(Control control) { TopLevel.GetTopLevel(control)!.MouseUp(Position(control), MouseButton.Left); ControlPreview.Flush(); }
    private static void Click(Control control) { Hover(control); Press(control); Release(control); }
}
