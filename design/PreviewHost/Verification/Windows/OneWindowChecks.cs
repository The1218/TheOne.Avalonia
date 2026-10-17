using System.ComponentModel;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Presenters;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

// Compiled into both PreviewHost and the independent NativeAOT consumer.
internal static class OneWindowChecks
{
    private static T Part<T>(Control owner, string name) where T : Control
    {
        return owner.GetVisualDescendants().OfType<T>()
            .Single(control => control.Name == name && control.TemplatedParent == owner);
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private static void Advance(int milliseconds)
    {
        var frame = new DispatcherFrame();
        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(milliseconds) };
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            frame.Continue = false;
        };
        timer.Start();
        Dispatcher.UIThread.PushFrame(frame);
        Flush();
    }

    private static void Wait(Task task)
    {
        var watch = Stopwatch.StartNew();
        while (!task.IsCompleted && watch.ElapsedMilliseconds < 1500) Advance(10);
        if (!task.IsCompleted) throw new InvalidOperationException("OneWindow async operation did not complete.");
    }

    private static void Click(Control control)
    {
        var host = TopLevel.GetTopLevel(control)!;
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), host)!.Value;
        host.MouseMove(point);
        host.MouseDown(point, MouseButton.Left);
        host.MouseUp(point, MouseButton.Left);
        Flush();
    }

    public static void Verify(Action<bool, string> check)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var editor = new TextBox { Text = "Focus returns here" };
            var left = new Button { Content = "Left action" };
            var right = new TextBox { Text = "Right editor", Width = 120 };
            var window = new OneWindow
            {
                Title = "OneWindow", Width = 860, Height = 560, RequestedThemeVariant = theme, LeftContent = left,
                RightContent = right, Content = editor
            };
            ControlSizing.SetSize(window, size);
            Motion.SetEnabled(window, false);
            window.Show();
            Flush();
            var label = $"OneWindow/{theme.Key}/{size}";
            check(
                OneWindow.Current == window && Part<Control>(window, "PART_TitleBar").Bounds.Height ==
                (size == ControlSize.Small ? 42 : size == ControlSize.Large ? 56 : 48),
                $"{label}: Window subtype registers itself and applies inherited titlebar density");
            check(
                ReferenceEquals(Part<ContentPresenter>(window, "PART_LeftContent").Child, left) &&
                ReferenceEquals(Part<ContentPresenter>(window, "PART_RightContent").Child, right) &&
                ReferenceEquals(Part<ContentPresenter>(window, "PART_ContentPresenter").Child, editor),
                $"{label}: titlebar slots preserve user content and native window content contract");
            var clicks = 0;
            left.Click += (_, _) => clicks++;
            Click(left);
            check(clicks == 1, $"{label}: interactive titlebar content receives pointer input");
            var themeButton = Part<ThemeButton>(window, "PART_ThemeButton");
            Click(themeButton);
            check(window.ActualThemeVariant != theme, $"{label}: ThemeButton switches effective window theme");
            Click(themeButton);
            check(window.ActualThemeVariant == theme,
                $"{label}: ThemeButton restores light/dark and tracks inherited appearance");
            check(
                Part<PathIcon>(themeButton, "PART_Sun").IsVisible == (theme == ThemeVariant.Dark) &&
                Part<PathIcon>(themeButton, "PART_Moon").IsVisible == (theme == ThemeVariant.Light),
                $"{label}: shortcut glyph tracks the effective light/dark theme");
            window.ShowThemeButton = false;
            window.ShowTitleBar = false;
            Flush();
            check(!themeButton.IsVisible && !Part<Control>(window, "PART_TitleBar").IsVisible,
                $"{label}: titlebar and theme shortcut can be independently hidden");
            window.ShowThemeButton = window.ShowTitleBar = true;
            Flush();
            OneNotification.Show("Title", "Notification", NotificationType.Success, TimeSpan.Zero);
            OneToast.Show("Toast", expiration: TimeSpan.Zero);
            Flush();
            var notifications = Part<WindowNotificationManager>(window, "PART_Notifications");
            var toasts = Part<WindowNotificationManager>(window, "PART_Toasts");
            check(
                notifications.GetVisualDescendants().OfType<NotificationCard>().Count() == 1 &&
                toasts.GetVisualDescendants().OfType<NotificationCard>().Count() == 1 &&
                notifications.Position == NotificationPosition.TopRight &&
                toasts.Position == NotificationPosition.TopCenter,
                $"{label}: static APIs find two automatically installed native message hosts");
            OneNotification.CloseAll();
            OneToast.CloseAll();
            Flush();
            check(!window.GetVisualDescendants().OfType<NotificationCard>().Any(),
                $"{label}: static close APIs release hosted cards");
            OneToast.Show("Expires", expiration: TimeSpan.FromMilliseconds(80));
            Advance(150);
            check(!toasts.GetVisualDescendants().OfType<NotificationCard>().Any(),
                $"{label}: toast expiration remains native without motion");
            editor.Focus();
            var dialog = OneDialog.ShowAsync("Confirm", "Confirm message", OneDialogButtons.YesNoCancel);
            Flush();
            var presenter = window.GetVisualDescendants().OfType<OneDialogPresenter>().Single();
            check(
                window.IsDialogOpen && !editor.IsEffectivelyEnabled && !left.IsEffectivelyEnabled &&
                !themeButton.IsEffectivelyEnabled && presenter.IsEffectivelyEnabled,
                $"{label}: automatic modal host disables background content and keeps dialog interactive");
            check(window.FocusManager!.GetFocusedElement() == Part<Button>(presenter, "PART_PrimaryButton"),
                $"{label}: dialog receives initial keyboard focus");
            Click(Part<Button>(presenter, "PART_PrimaryButton"));
            Wait(dialog);
            check(
                dialog.Result == OneDialogResult.Yes && !window.IsDialogOpen && editor.IsEffectivelyEnabled &&
                window.FocusManager.GetFocusedElement() == editor,
                $"{label}: dialog result restores background and prior focus");
            var first = OneDialog.ShowAsync("First", "First");
            var second = OneDialog.ShowAsync("Second", "Second", OneDialogButtons.OkCancel);
            Flush();
            check(window.GetVisualDescendants().OfType<OneDialogPresenter>().Count() == 1 && !second.IsCompleted,
                $"{label}: queued dialogs create only one active presenter");
            Click(Part<Button>(window.GetVisualDescendants().OfType<OneDialogPresenter>().Single(),
                "PART_PrimaryButton"));
            Wait(first);
            Flush();
            presenter = window.GetVisualDescendants().OfType<OneDialogPresenter>().Single();
            check(Equals(presenter.Header, "Second"), $"{label}: dialog queue advances in order");
            window.KeyPress(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
            window.KeyRelease(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
            Flush();
            Wait(second);
            check(second.Result == OneDialogResult.Cancel, $"{label}: Escape dismisses current dialog");
            using var cancellation = new CancellationTokenSource();
            var cancelled = OneDialog.ShowAsync("Cancel", "Cancel", cancellationToken: cancellation.Token);
            Flush();
            cancellation.Cancel();
            Flush();
            Wait(cancelled);
            check(cancelled.IsCanceled && !window.IsDialogOpen,
                $"{label}: cancellation completes the task and releases modal state");
            using var queuedCancellation = new CancellationTokenSource();
            first = OneDialog.ShowAsync("Keeps active", "Active");
            second = OneDialog.ShowAsync("Cancelled in queue", "Queued", cancellationToken: queuedCancellation.Token);
            Flush();
            queuedCancellation.Cancel();
            Flush();
            Wait(second);
            check(second.IsCanceled && window.IsDialogOpen,
                $"{label}: cancelling a queued request does not close the active dialog");
            Click(Part<Button>(window.GetVisualDescendants().OfType<OneDialogPresenter>().Single(),
                "PART_PrimaryButton"));
            Wait(first);
            var form = new TextBox { Text = "Editable form" };
            dialog = OneDialog.ShowContentAsync(form, "Form");
            Flush();
            presenter = window.GetVisualDescendants().OfType<OneDialogPresenter>().Single();
            check(ReferenceEquals(Part<ContentPresenter>(presenter, "PART_ContentPresenter").Child, form),
                $"{label}: custom form content retains identity and input behavior");
            form.Text = "Edited form";
            Click(Part<Button>(presenter, "PART_CancelButton"));
            Wait(dialog);
            check(form.Text == "Edited form", $"{label}: closing a form does not replace caller-owned state");
            check(form.Parent is null, $"{label}: closing detaches caller-owned form content for reuse");
            dialog = OneDialog.ShowContentAsync(form, "Reused form");
            Flush();
            Click(Part<Button>(window.GetVisualDescendants().OfType<OneDialogPresenter>().Single(),
                "PART_PrimaryButton"));
            Wait(dialog);
            check(dialog.Result == OneDialogResult.Ok && form.Parent is null,
                $"{label}: a custom form can be shown again without violating single-parent rules");
            var vm = new DialogEditorViewModel();
            dialog = OneDialog.ShowAsync<DialogEditorView, DialogEditorViewModel>(vm,
                new DialogOptions
                    { Title = "VM form", ShowTitle = false, ShowCloseButton = false, CloseOnEscape = false });
            Flush();
            presenter = window.GetVisualDescendants().OfType<OneDialogPresenter>().Single();
            var view = (DialogEditorView)presenter.Content!;
            var input = view.FindControl<TextBox>("WorkspaceName")!;
            check(
                ReferenceEquals(view.DataContext, vm) && input.Text == vm.Name &&
                !Part<Control>(presenter, "PART_Header").IsVisible &&
                !Part<Control>(presenter, "PART_Buttons").IsVisible,
                $"{label}: typed compiled form hides optional chrome without unused spacing");
            vm.Name = "VM update";
            Flush();
            check(input.Text == vm.Name, $"{label}: compiled VM-to-view updates remain live");
            input.Text = "Parent receives edits";
            Flush();
            check(vm.Name == input.Text, $"{label}: compiled form two-way input updates the VM");
            window.KeyPress(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
            window.KeyRelease(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
            Flush();
            check(!dialog.IsCompleted, $"{label}: Escape honors CloseOnEscape=false");
            Click(view.FindControl<Button>("EditorSave")!);
            Wait(dialog);
            check(dialog.Result == OneDialogResult.Ok && !window.IsDialogOpen && view.Parent is null,
                $"{label}: VM command closes its modal dialog and returns the result");
            Click(Part<Button>(window, "PART_MaximizeButton"));
            check(window.WindowState == WindowState.Maximized,
                $"{label}: caption maximize button uses native Window state");
            Click(Part<Button>(window, "PART_MaximizeButton"));
            check(window.WindowState == WindowState.Normal, $"{label}: caption restore preserves Window behavior");
            window.CanMaximize = false;
            window.CanMinimize = false;
            Flush();
            check(
                !Part<Button>(window, "PART_MaximizeButton").IsEffectivelyEnabled &&
                !Part<Button>(window, "PART_MinimizeButton").IsEffectivelyEnabled,
                $"{label}: caption capabilities retain native enablement");
            dialog = OneDialog.ShowAsync("Close owner", "Owner closing");
            second = OneDialog.ShowAsync("Queued close", "Owner closing");
            Flush();
            window.Close();
            Flush();
            Wait(dialog);
            Wait(second);
            check(
                dialog.Result == OneDialogResult.Closed && second.Result == OneDialogResult.Closed &&
                OneWindow.Current != window,
                $"{label}: closing owner settles active/queued tasks and unregisters automatically");
        }

        VerifyOwnership(check);
        VerifyBindings(check);
        VerifyLifecycle(check);
        VerifyTypedDialogs(check);
    }

    private static void VerifyOwnership(Action<bool, string> check)
    {
        var first = new OneWindow { Width = 520, Height = 340 };
        first.Show();
        Flush();
        var second = new OneWindow { Width = 520, Height = 340 };
        second.Show();
        Flush();
        Motion.SetEnabled(first, false);
        Motion.SetEnabled(second, false);
        OneToast.Show("Default");
        OneNotification.Show("Explicit", "Owner", expiration: TimeSpan.Zero, owner: first);
        Flush();
        check(
            Part<WindowNotificationManager>(second, "PART_Toasts").GetVisualDescendants().OfType<NotificationCard>()
                .Any() && Part<WindowNotificationManager>(first, "PART_Notifications").GetVisualDescendants()
                .OfType<NotificationCard>().Any(),
            "OneWindow: default and explicit owners keep messages in the intended window");
        second.Hide();
        Flush();
        check(OneWindow.Current == first, "OneWindow: hidden windows do not capture implicit messages");
        second.Show();
        Flush();
        second.Close();
        Flush();
        check(OneWindow.Current == first, "OneWindow: closing the latest window restores default owner");
        first.Close();
        Flush();
        var rejected = false;
        try
        {
            OneToast.Show("Closed owner", owner: first);
        }
        catch (InvalidOperationException)
        {
            rejected = true;
        }

        check(rejected, "OneWindow: closed explicit owners are rejected rather than silently redirected");
    }

    private static void VerifyBindings(Action<bool, string> check)
    {
        var model = new CaptionModel();
        var window = new OneWindow { Width = 680, Height = 360 };
        using var binding = window.Bind(OneWindow.LeftContentProperty,
            CompiledBinding.Create<CaptionModel, string>(value => value.Caption, model));
        window.Show();
        Flush();
        model.Caption = "Updated compiled slot";
        Flush();
        check(
            Equals(window.LeftContent, model.Caption) && Part<ContentPresenter>(window, "PART_LeftContent")
                .GetVisualDescendants().OfType<TextBlock>().Any(block => block.Text == model.Caption),
            "OneWindow: compiled titlebar bindings remain live after native template application");
        window.Close();
        Flush();
    }

    private static void VerifyLifecycle(Action<bool, string> check)
    {
        var window = new OneWindow { Width = 640, Height = 380 };
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        var background = Task.Run(() =>
            OneToast.ShowAsync("Background caller", expiration: TimeSpan.Zero, owner: window));
        Wait(background);
        background.GetAwaiter().GetResult();
        check(
            Part<WindowNotificationManager>(window, "PART_Toasts").GetVisualDescendants().OfType<NotificationCard>()
                .Count() == 1, "OneWindow: async toast API marshals a background caller to the UI thread");
        var backgroundDialog = Task.Run(() => OneDialog.ShowAsync("Background dialog", "From worker", owner: window));
        var watch = Stopwatch.StartNew();
        while (!window.IsDialogOpen && watch.ElapsedMilliseconds < 1500) Advance(10);
        Click(Part<Button>(window.GetVisualDescendants().OfType<OneDialogPresenter>().Single(), "PART_PrimaryButton"));
        Wait(backgroundDialog);
        check(backgroundDialog.Result == OneDialogResult.Ok,
            "OneWindow: background dialog caller receives its typed result");
        var cancelClose = true;
        window.Closing += (_, args) => args.Cancel = cancelClose;
        window.Close();
        Flush();
        check(window.IsVisible && OneWindow.Current == window,
            "OneWindow: cancelled native closing keeps the automatically registered host alive");
        using var preCancelled = new CancellationTokenSource();
        preCancelled.Cancel();
        var cancelled = OneDialog.ShowAsync("Never shown", "Cancelled", owner: window,
            cancellationToken: preCancelled.Token);
        Wait(cancelled);
        check(cancelled.IsCanceled && !window.IsDialogOpen,
            "OneWindow: an already cancelled token does not create a dialog");
        var requests = Enumerable.Range(0, 1000)
            .Select(index => OneDialog.ShowAsync($"Queued {index}", "Lazy dialog", owner: window)).ToArray();
        Flush();
        check(
            window.GetVisualDescendants().OfType<OneDialogPresenter>().Count() == 1 &&
            requests.All(request => !request.IsCompleted),
            "OneWindow: a thousand queued requests create only one active dialog control");
        cancelClose = false;
        window.Close();
        Flush();
        foreach (var request in requests) Wait(request);
        check(requests.All(request => request.Result == OneDialogResult.Closed),
            "OneWindow: closing releases a thousand pending requests without orphaned tasks");
        var missing = false;
        try
        {
            OneNotification.Show("No host", "No window");
        }
        catch (InvalidOperationException)
        {
            missing = true;
        }

        check(missing, "OneWindow: calls without an open window report a clear lifecycle error");
        window = new OneWindow { Width = 450, Height = 300 };
        window.Show();
        Flush();
        var longDialog = OneDialog.ShowAsync("Long content",
            string.Join("\n", Enumerable.Repeat("Long dialog content remains in a bounded viewport.", 1000)),
            owner: window);
        Flush();
        var presenter = window.GetVisualDescendants().OfType<OneDialogPresenter>().Single();
        check(
            presenter.Bounds.Height <= window.Bounds.Height - window.TitleBarHeight && presenter.GetVisualDescendants()
                .OfType<ScrollViewer>().Any(view => view.Extent.Height > view.Viewport.Height),
            "OneWindow: long dialog content scrolls inside a compact window without hiding footer actions");
        window.Close();
        Flush();
        Wait(longDialog);
    }

    private static void VerifyTypedDialogs(Action<bool, string> check)
    {
        var window = new OneWindow { Width = 680, Height = 460 };
        window.Show();
        Flush();
        var vm = new DialogEditorViewModel();
        var first = OneDialog.ShowAsync<DialogEditorView, DialogEditorViewModel>(vm);
        Flush();
        var presenter = window.GetVisualDescendants().OfType<OneDialogPresenter>().Single();
        check(!presenter.ShowTitle && presenter.ShowCloseButton && !presenter.ShowButtons && presenter.CloseOnEscape,
            "OneDialog: default typed options permit close and Escape without generic footer");
        var duplicate = OneDialog.ShowAsync<DialogEditorView, DialogEditorViewModel>(vm);
        Wait(duplicate);
        check(duplicate.IsFaulted && !first.IsCompleted,
            "OneDialog: simultaneous use of the same context is rejected without closing its first dialog");
        var shield = Part<Panel>(window, "PART_DialogShield");
        var point = shield.TranslatePoint(new Point(4, 4), window)!.Value;
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        Flush();
        check(!first.IsCompleted, "OneDialog: clicking overlay preserves dialog by default");
        Click(Part<Button>(presenter, "PART_CloseButton"));
        Wait(first);
        check(first.Result == OneDialogResult.Closed, "OneDialog: close button returns Closed");
        first = OneDialog.ShowAsync<DialogEditorView, DialogEditorViewModel>(vm,
            new DialogOptions { CloseOnOverlayClick = true });
        Flush();
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        Flush();
        Wait(first);
        check(first.Result == OneDialogResult.Closed && !window.IsDialogOpen,
            "OneDialog: opt-in overlay dismissal returns Closed and permits context reuse");
        first = OneDialog.ShowAsync("Active", "Queue owner", owner: window);
        var queuedVm = new DialogEditorViewModel();
        var queued = OneDialog.ShowAsync<DialogEditorView, DialogEditorViewModel>(queuedVm);
        Flush();
        queuedVm.RequestClose(OneDialogResult.No);
        Wait(queued);
        check(
            queued.Result == OneDialogResult.No && !first.IsCompleted &&
            window.GetVisualDescendants().OfType<DialogEditorView>().Count() == 0,
            "OneDialog: queued VM close returns result without constructing its view or closing another request");
        Click(Part<Button>(window.GetVisualDescendants().OfType<OneDialogPresenter>().Single(), "PART_PrimaryButton"));
        Wait(first);
        var worker = Task.Run(() =>
            OneDialog.ShowAsync<DialogEditorView, DialogEditorViewModel>(vm, new DialogOptions { Title = "Worker" },
                window));
        var watch = Stopwatch.StartNew();
        while (!window.IsDialogOpen && watch.ElapsedMilliseconds < 1500) Advance(10);
        Task.Run(() => vm.RequestClose(OneDialogResult.Yes)).GetAwaiter().GetResult();
        Wait(worker);
        check(worker.Result == OneDialogResult.Yes && !window.IsDialogOpen,
            "OneDialog: typed calls and context close events marshal worker threads to UI");
        using var cancellation = new CancellationTokenSource();
        first = OneDialog.ShowAsync<DialogEditorView, DialogEditorViewModel>(vm, cancellationToken: cancellation.Token);
        Flush();
        cancellation.Cancel();
        Flush();
        Wait(first);
        check(first.IsCanceled && !window.IsDialogOpen, "OneDialog: cancellation detaches typed dialog context");
        first = OneDialog.ShowAsync<DialogEditorView, DialogEditorViewModel>(vm,
            new DialogOptions { Title = "Reuse", ShowButtons = true, Buttons = OneDialogButtons.YesNo });
        Flush();
        presenter = window.GetVisualDescendants().OfType<OneDialogPresenter>().Single();
        queuedVm.RequestClose();
        Flush();
        check(!first.IsCompleted, "OneDialog: a detached old context cannot dismiss a later dialog");
        Click(Part<Button>(presenter, "PART_NoButton"));
        Wait(first);
        check(first.Result == OneDialogResult.No, "OneDialog: generic form can opt into standard footer results");
        var models = Enumerable.Range(0, 1000).Select(_ => new DialogEditorViewModel()).ToArray();
        var tasks = models.Select(model => OneDialog.ShowAsync<DialogEditorView, DialogEditorViewModel>(model))
            .ToArray();
        Flush();
        check(
            window.GetVisualDescendants().OfType<DialogEditorView>().Count() == 1 &&
            tasks.All(task => !task.IsCompleted),
            "OneDialog: thousand typed requests construct only the currently visible view");
        window.Close();
        Flush();
        foreach (var task in tasks) Wait(task);
        check(tasks.All(task => task.Result == OneDialogResult.Closed),
            "OneDialog: window close completes typed queue and detaches every context");
        window = new OneWindow { Width = 600, Height = 400 };
        window.Show();
        Flush();
        first = OneDialog.ShowAsync<DialogEditorView, DialogEditorViewModel>(models[0]);
        Flush();
        models[0].RequestClose(OneDialogResult.Cancel);
        Wait(first);
        check(first.Result == OneDialogResult.Cancel,
            "OneDialog: context from closed window can be reused in a new window");
        window.Close();
        Flush();
    }

    public static void Render(string output)
    {
        Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var body = new StackPanel
            {
                Margin = new Thickness(24), Spacing = 16,
                Children =
                {
                    new TextBlock { Text = "OneWindow · 工作区", FontSize = 24 }, new TextBox { Text = "输入内容保持原生绑定与焦点" },
                    new Button { Content = "继续操作", Classes = { "primary" } }
                }
            };
            var window = new OneWindow
            {
                Title = "TheOne 工作区", Width = 900, Height = 600, RequestedThemeVariant = theme,
                LeftContent = new TextBlock { Text = "Workspace" },
                RightContent = new Button { Content = "账户", Classes = { "ghost" } }, Content = body
            };
            ControlSizing.SetSize(window, size);
            Motion.SetEnabled(window, false);
            window.Show();
            Flush();
            OneNotification.Show("保存成功", "无需注册通知宿主。", NotificationType.Success, TimeSpan.Zero);
            OneToast.Show("更新已完成", expiration: TimeSpan.Zero);
            Flush();
            Save("messages");
            var dialog = OneDialog.ShowAsync("确认更新", "对话框自动定位 OneWindow，保留主题和尺寸。", OneDialogButtons.YesNoCancel);
            Flush();
            Save("dialog");
            Click(Part<Button>(window.GetVisualDescendants().OfType<OneDialogPresenter>().Single(),
                "PART_PrimaryButton"));
            Wait(dialog);
            dialog = OneDialog.ShowAsync<DialogEditorView, DialogEditorViewModel>(new DialogEditorViewModel(),
                new DialogOptions { Title = "编辑工作区" });
            Flush();
            Save("typed-dialog");
            Click(Part<Button>(window.GetVisualDescendants().OfType<OneDialogPresenter>().Single(),
                "PART_CloseButton"));
            Wait(dialog);
            dialog = OneDialog.ShowAsync<DialogEditorView, DialogEditorViewModel>(new DialogEditorViewModel(),
                new DialogOptions { ShowTitle = false, ShowCloseButton = false });
            Flush();
            Save("typed-dialog-no-header");
            window.Close();
            Flush();
            Wait(dialog);

            void Save(string state)
            {
                using var bitmap = window.CaptureRenderedFrame()!;
                bitmap.Save(Path.Combine(output, $"onewindow-{theme.Key}-{size}-{state}.png"),
                    PngBitmapEncoderOptions.Default);
            }
        }
    }

    private sealed class CaptionModel : INotifyPropertyChanged
    {
        private string _caption = "Bound slot";

        public string Caption
        {
            get => _caption;
            set
            {
                _caption = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Caption)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}