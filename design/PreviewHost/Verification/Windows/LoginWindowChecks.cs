using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;
using TheOne.Avalonia.Themes;

namespace TheOne.DesignPreview;

// Shared by the headless preview and the independent NativeAOT consumer.
internal static class LoginWindowChecks
{
    private static T Part<T>(Control owner, string name) where T : Control => owner.GetVisualDescendants().OfType<T>().Single(control => control.Name == name && control.TemplatedParent == owner);
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
    private static void Wait(Task task)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        while (!task.IsCompleted && watch.ElapsedMilliseconds < 2000)
        {
            var frame = new DispatcherFrame(); var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
            timer.Tick += (_, _) => { timer.Stop(); frame.Continue = false; }; timer.Start(); Dispatcher.UIThread.PushFrame(frame); Flush();
        }
        if (!task.IsCompleted) throw new InvalidOperationException("LoginWindow operation did not complete.");
    }
    private static bool Throws(Action action)
    {
        try { action(); return false; }
        catch (InvalidOperationException) { return true; }
    }
    private static LoginWindow Open(IClassicDesktopStyleApplicationLifetime desktop)
    {
        var login = new LoginWindow { Width = 440, Height = 360, Content = new TextBox { Text = "Caller-owned login" } };
        desktop.MainWindow = login; Motion.SetEnabled(login, false); login.Show(); Flush(); return login;
    }
    private static void CloseAll(IClassicDesktopStyleApplicationLifetime desktop)
    {
        desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        foreach (var window in desktop.Windows.ToArray()) window.Close();
        Flush();
    }

    public static void Verify(Action<bool, string> check)
    {
        var desktop = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime
            ?? throw new InvalidOperationException("Initialize the LoginWindow checks with a classic desktop lifetime before AppBuilder setup.");
        var previousMain = desktop.MainWindow; var previousMode = desktop.ShutdownMode;
        try
        {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            VerifyContent(check);
            VerifyInstanceHandoff(desktop, check);
            foreach (var mode in Enum.GetValues<ShutdownMode>()) VerifyTransition(desktop, mode, check);
            VerifyCancelledAndReentrant(desktop, check);
            VerifyFailures(desktop, check);
            VerifyAsyncAndStale(desktop, check);
            VerifyDemo(desktop, check);
        }
        finally
        {
            CloseAll(desktop); desktop.MainWindow = previousMain; desktop.ShutdownMode = previousMode;
        }
    }

    private static void VerifyContent(Action<bool, string> check)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var input = new TextBox { Text = "Custom login input" };
            var login = new LoginWindow { RequestedThemeVariant = theme, Content = input };
            ControlSizing.SetSize(login, size); Motion.SetEnabled(login, false); login.Show(); Flush();
            var label = $"LoginWindow/{theme.Key}/{size}";
            check(login is Window && login.WindowDecorations == WindowDecorations.Full && !login.ExtendClientAreaToDecorationsHint, $"{label}: preserves native Window chrome and desktop behavior");
            check(ReferenceEquals(Part<ContentPresenter>(login, "PART_ContentPresenter").Child, input) && input.Bounds.Width > 0 && Part<VisualLayerManager>(login, "PART_VisualLayerManager").Bounds.Height > 0, $"{label}: native visual layers and content presenter retain developer-owned controls");
            check(login.FontSize == (size == ControlSize.Small ? 12 : size == ControlSize.Large ? 16 : 14) && login.TryFindResource("One.Canvas", theme, out var canvas) && login.Background is ISolidColorBrush actual && canvas is ISolidColorBrush expected && actual.Color == expected.Color, $"{label}: TheOne semantic background and inherited density follow light/dark");
            input.Focus(); check(login.FocusManager!.GetFocusedElement() == input, $"{label}: caller-owned controls retain native keyboard focus");
            check(OneToast.Owner == login && OneNotification.Owner == login && OneDialog.Owner == login, $"{label}: automatically registers all three message owners");
            login.ShowErrorToast("Invalid credentials", TimeSpan.Zero);
            login.ShowErrorNotification("Authentication unavailable", expiration: TimeSpan.Zero); Flush();
            check(Part<WindowNotificationManager>(login, "PART_Toasts").GetVisualDescendants().OfType<NotificationCard>().Single().NotificationType == NotificationType.Error
                && Part<WindowNotificationManager>(login, "PART_Notifications").GetVisualDescendants().OfType<NotificationCard>().Single().NotificationType == NotificationType.Error,
                $"{label}: error helpers render native Error cards in independent login hosts");
            var errorDialog = login.ShowErrorDialog("Please retry"); Flush();
            var presenter = login.GetVisualDescendants().OfType<OneDialogPresenter>().Single();
            check(login.IsDialogOpen && !input.IsEffectivelyEnabled && Part<Panel>(login, "PART_DialogShield").IsVisible, $"{label}: error dialog shields the login form and disables background input");
            Part<Button>(presenter, "PART_PrimaryButton").RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); Wait(errorDialog); Flush();
            check(errorDialog.Result == OneDialogResult.Ok && !login.IsDialogOpen && input.IsEffectivelyEnabled && login.FocusManager!.GetFocusedElement() == input,
                $"{label}: error dialog returns its result and restores login focus");
            OneToast.CloseAll(); OneNotification.CloseAll();
            var model = new DialogEditorViewModel();
            var template = new FuncDataTemplate<DialogEditorViewModel>((value, _) => new DialogEditorView { DataContext = value });
            login.ContentTemplate = template; login.Content = model; Flush();
            var view = login.GetVisualDescendants().OfType<DialogEditorView>().Single(); var editor = view.FindControl<TextBox>("WorkspaceName")!;
            check(ReferenceEquals(login.Content, model) && ReferenceEquals(login.ContentTemplate, template) && editor.Text == model.Name, $"{label}: native ContentTemplate applies the caller's typed compiled view");
            model.Name = "Updated compiled login"; Flush(); editor.Text = "Edited compiled login"; Flush();
            check(model.Name == "Edited compiled login", $"{label}: ContentTemplate preserves compiled two-way input bindings");
            login.Close(); Flush();
            check(OneToast.Owner != login && OneNotification.Owner != login && OneDialog.Owner != login, $"{label}: closing login releases default owners");
        }
    }

    private static void VerifyInstanceHandoff(IClassicDesktopStyleApplicationLifetime desktop, Action<bool, string> check)
    {
        var login = Open(desktop);
        var main = new OneWindow { Width = 640, Height = 420 }; Motion.SetEnabled(main, false);
        var veto = true;
        login.Closing += (_, args) => args.Cancel = veto;
        var cancelled = login.CompleteLoginAsync(main); Wait(cancelled);
        check(!cancelled.Result && !main.IsVisible && main.PlatformImpl is not null && desktop.MainWindow == login && OneToast.Owner == login && OneNotification.Owner == login && OneDialog.Owner == login,
            "LoginWindow: supplied instance remains caller-owned and unopened when Closing vetoes handoff");
        var ownersAtOpened = false; var ownersAtClosed = false; var opened = 0;
        main.Opened += (_, _) =>
        {
            opened++;
            ownersAtOpened = OneToast.Owner == main && OneNotification.Owner == main && OneDialog.Owner == main;
            // Consumer callbacks can override default owners during opening; handoff corrects them.
            OneToast.Owner = login; OneNotification.Owner = login; OneDialog.Owner = login;
        };
        var pending = login.ShowErrorDialog("Login dialog closes with its owner"); var queued = login.ShowErrorDialog("Queued login dialog"); Flush();
        login.Closed += (_, _) =>
        {
            ownersAtClosed = desktop.MainWindow == main && OneToast.Owner == main && OneNotification.Owner == main && OneDialog.Owner == main;
            OneToast.Show("Message from login Closed", expiration: TimeSpan.Zero);
            OneNotification.Show("Main", "Owner already transferred", expiration: TimeSpan.Zero);
            OneDialog.Owner = login;
        };
        veto = false;
        var complete = Task.Run(() => login.CompleteLoginAsync(main)); Wait(complete); Wait(pending); Wait(queued); Flush();
        check(complete.Result && ownersAtOpened && ownersAtClosed && !login.IsVisible && main.IsVisible && desktop.MainWindow == main && main.Owner is null
            && OneToast.Owner == main && OneNotification.Owner == main && OneDialog.Owner == main,
            "LoginWindow: instance handoff corrects all message owners before closing login and after consumer callbacks");
        check(pending.Result == OneDialogResult.Closed && queued.Result == OneDialogResult.Closed && !login.IsDialogOpen,
            "LoginWindow: active and queued login dialogs complete Closed during handoff");
        var duplicate = new OneWindow();
        check(login.CompleteLogin(main) && login.CompleteLogin(duplicate) && opened == 1 && !duplicate.IsVisible && desktop.Windows.Count == 1,
            "LoginWindow: repeated instance completion never opens a second main window"); duplicate.Close();
        check(Part<WindowNotificationManager>(main, "PART_Toasts").GetVisualDescendants().OfType<NotificationCard>().Count() == 1
            && Part<WindowNotificationManager>(main, "PART_Notifications").GetVisualDescendants().OfType<NotificationCard>().Count() == 1,
            "LoginWindow: Closed callback messages render in the transferred main hosts");
        var dialog = OneDialog.ShowAsync("Main owner", "Ready after login"); Flush();
        var presenter = main.GetVisualDescendants().OfType<OneDialogPresenter>().Single();
        Part<Button>(presenter, "PART_PrimaryButton").RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); Wait(dialog);
        check(dialog.Result == OneDialogResult.Ok && !main.IsDialogOpen, "LoginWindow: transferred default dialog owner remains interactive");
        CloseAll(desktop);
        login = Open(desktop); main = new OneWindow(); login.Close(); Flush();
        var late = Task.Run(() => login.CompleteLoginAsync(main)); Wait(late);
        check(late.IsFaulted && late.Exception!.GetBaseException() is InvalidOperationException && !main.IsVisible && !desktop.Windows.Any()
            && OneToast.Owner is null && OneNotification.Owner is null && OneDialog.Owner is null,
            "LoginWindow: late supplied-instance callback cannot reopen windows or restore closed message owners"); main.Close();
        login = Open(desktop); main = new OneWindow();
        main.Opened += (_, _) => throw new InvalidOperationException("Failed supplied window");
        check(Throws(() => login.CompleteLogin(main)) && login.IsVisible && main.PlatformImpl is null && desktop.MainWindow == login
            && OneToast.Owner == login && OneNotification.Owner == login && OneDialog.Owner == login,
            "LoginWindow: failed supplied-instance opening disposes replacement and restores all login owners");
        login.ShowErrorToast("Login remains usable", TimeSpan.Zero); Flush();
        check(Part<WindowNotificationManager>(login, "PART_Toasts").GetVisualDescendants().OfType<NotificationCard>().Count() == 1,
            "LoginWindow: rollback leaves login error host usable"); CloseAll(desktop);
    }

    private static void VerifyTransition(IClassicDesktopStyleApplicationLifetime desktop, ShutdownMode mode, Action<bool, string> check)
    {
        var login = Open(desktop); desktop.ShutdownMode = mode;
        var combo = new ComboBox { ItemsSource = new[] { "Workspace one", "Workspace two" }, SelectedIndex = 0, Width = 240 };
        var main = new OneWindow { Width = 640, Height = 420, Content = combo }; Motion.SetEnabled(main, false);
        var factoryCalls = 0; var openingOrdered = false; var closingOrdered = false;
        main.Opened += (_, _) =>
        {
            openingOrdered = desktop.MainWindow == main && login.IsVisible && main.Owner is null && OneWindow.Current == main;
            OneToast.Show("Main is opening", expiration: TimeSpan.Zero);
            OneNotification.Show("Main is opening", "Registered before login closes", expiration: TimeSpan.Zero);
        };
        login.Closed += (_, _) => closingOrdered = desktop.MainWindow == main && main.IsVisible && OneWindow.Current == main;
        check(login.CompleteLogin(() => { factoryCalls++; return main; }), $"LoginWindow/{mode}: successful completion opens the replacement"); Flush();
        check(openingOrdered && closingOrdered && !login.IsVisible && main.IsVisible && desktop.MainWindow == main && main.Owner is null && desktop.ShutdownMode == mode && desktop.Windows.Contains(main) && !desktop.Windows.Contains(login), $"LoginWindow/{mode}: swaps LegacyPreviewWindow before closing login and restores shutdown policy");
        check(login.CompleteLogin(() => { factoryCalls++; return new OneWindow(); }) && factoryCalls == 1, $"LoginWindow/{mode}: repeated success creates exactly one main window");
        check(Part<WindowNotificationManager>(main, "PART_Notifications").GetVisualDescendants().OfType<NotificationCard>().Count() == 1 && Part<WindowNotificationManager>(main, "PART_Toasts").GetVisualDescendants().OfType<NotificationCard>().Count() == 1, $"LoginWindow/{mode}: notification and toast hosts survive the login window's disposal");
        OneNotification.CloseAll(); OneToast.CloseAll();
        combo.IsDropDownOpen = true; Flush(); var popup = combo.GetVisualDescendants().OfType<Popup>().Single();
        check(popup.IsOpen && popup.Child is { } child && TopLevel.GetTopLevel(child) is not null && TopLevel.GetTopLevel(combo) == main, $"LoginWindow/{mode}: the main window's native popup remains attached to its top level"); combo.IsDropDownOpen = false;
        var active = OneDialog.ShowAsync("Main dialog", "Main window host"); var queued = OneDialog.ShowAsync("Queued", "Pending main window host"); Flush();
        check(main.IsDialogOpen && main.GetVisualDescendants().OfType<OneDialogPresenter>().Count() == 1, $"LoginWindow/{mode}: dialogs resolve the new main window after login has closed");
        desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown; main.Close(); Flush(); Wait(active); Wait(queued);
        check(active.Result == OneDialogResult.Closed && queued.Result == OneDialogResult.Closed && OneWindow.Current is null, $"LoginWindow/{mode}: closing main releases both active and queued hosts");
    }

    private static void VerifyCancelledAndReentrant(IClassicDesktopStyleApplicationLifetime desktop, Action<bool, string> check)
    {
        var login = Open(desktop); desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
        var cancelled = true; var calls = 0; var nested = true;
        login.Closing += (_, args) =>
        {
            nested = login.CompleteLogin(() => { calls++; return new OneWindow(); });
            login.Close(); args.Cancel = cancelled;
        };
        check(!login.CompleteLogin(() => { calls++; return new OneWindow(); }) && calls == 0 && !nested && login.IsVisible && desktop.MainWindow == login && desktop.ShutdownMode == ShutdownMode.OnMainWindowClose, "LoginWindow: a Closing veto and reentrant close preserve login without invoking the factory");
        cancelled = false; var main = new OneWindow { Width = 640, Height = 420 };
        main.Opened += (_, _) => nested = login.CompleteLogin(() => { calls++; return new OneWindow(); });
        check(login.CompleteLogin(() => { calls++; return main; }) && calls == 1 && !nested, "LoginWindow: retry after a Closing veto commits once and rejects completion inside Opened");
        CloseAll(desktop);
    }

    private static void VerifyFailures(IClassicDesktopStyleApplicationLifetime desktop, Action<bool, string> check)
    {
        var login = Open(desktop); desktop.ShutdownMode = ShutdownMode.OnLastWindowClose;
        var failure = new InvalidOperationException("Factory failure"); Exception? caught = null;
        try { login.CompleteLogin(() => throw failure); } catch (Exception error) { caught = error; }
        check(ReferenceEquals(caught, failure) && login.IsVisible && desktop.MainWindow == login && desktop.ShutdownMode == ShutdownMode.OnLastWindowClose, "LoginWindow: factory exceptions propagate after native Closing safely keeps login alive");
        var throwsClosing = true; var calls = 0;
        login.Closing += (_, _) => { if (throwsClosing) throw failure; };
        caught = null;
        try { login.CompleteLogin(() => { calls++; return new OneWindow(); }); } catch (Exception error) { caught = error; }
        check(ReferenceEquals(caught, failure) && calls == 0 && login.IsVisible && desktop.MainWindow == login, "LoginWindow: Closing exceptions propagate without native Window.Close disposing login");
        throwsClosing = false;
        foreach (var action in new[] { "throw", "close", "hide", "replace" })
        {
            var candidate = new OneWindow { Width = 640, Height = 420 }; var closed = false;
            candidate.Closed += (_, _) => closed = true;
            candidate.Closing += (_, args) => args.Cancel = true;
            candidate.Opened += (_, _) =>
            {
                OneToast.Show("Candidate host", expiration: TimeSpan.Zero);
                if (action == "throw") throw failure;
                if (action == "close") candidate.Close();
                if (action == "hide") candidate.Hide();
                if (action == "replace") desktop.MainWindow = null;
            };
            // For the self-close case allow its first close, then veto rollback closes.
            if (action == "close") candidate.Closing += (_, args) => args.Cancel = false;
            check(Throws(() => login.CompleteLogin(() => candidate)), $"LoginWindow: a replacement Opened callback that {action}s aborts completion"); Flush();
            check(login.IsVisible && desktop.MainWindow == login && desktop.ShutdownMode == ShutdownMode.OnLastWindowClose && !candidate.IsVisible && closed && OneWindow.Current is null && desktop.Windows.Count == 1, $"LoginWindow: failed replacement {action} disposes its hosts even when Closing vetoes cleanup");
        }
        check(Throws(() => login.CompleteLogin(() => null!)) && login.IsVisible && desktop.MainWindow == login, "LoginWindow: null main factory results leave the login window usable");
        var existing = new OneWindow { Width = 480, Height = 320 }; existing.Show(); Flush();
        check(Throws(() => login.CompleteLogin(() => existing)) && existing.IsVisible && existing.PlatformImpl is not null && desktop.Windows.Contains(existing) && login.IsVisible && desktop.MainWindow == login, "LoginWindow: a factory returning an already visible window is rejected without disposing that window");
        existing.Hide(); Flush();
        check(Throws(() => login.CompleteLogin(() => existing)) && !existing.IsVisible && existing.PlatformImpl is not null && desktop.Windows.Contains(existing) && login.IsVisible && desktop.MainWindow == login, "LoginWindow: a factory returning a previously shown hidden window is rejected without disposing that window");
        CloseAll(desktop);
        login = Open(desktop); desktop.ShutdownMode = ShutdownMode.OnLastWindowClose;
        var committedMain = new OneWindow { Width = 640, Height = 420 };
        login.Closed += (_, _) => throw failure;
        caught = null;
        try { login.CompleteLogin(() => committedMain); } catch (Exception error) { caught = error; }
        check(ReferenceEquals(caught, failure) && !login.IsVisible && committedMain.IsVisible && desktop.MainWindow == committedMain && desktop.ShutdownMode == ShutdownMode.OnLastWindowClose && desktop.Windows.Count == 1 && !desktop.Windows.Contains(login) && OneWindow.Current == committedMain, "LoginWindow: a throwing Closed subscriber still completes native teardown and preserves the committed main window");
        check(login.CompleteLogin(() => throw failure), "LoginWindow: completion remains idempotent after a Closed subscriber reports an error");
        CloseAll(desktop);
        login = Open(desktop); desktop.ShutdownMode = ShutdownMode.OnLastWindowClose;
        var survivor = new OneWindow { Width = 480, Height = 320 }; survivor.Show(); Flush();
        OneToast.Show("Unrelated survivor", expiration: TimeSpan.Zero, owner: survivor);
        var parent = new OneWindow { Width = 640, Height = 420 }; var childWindow = new OneWindow { Width = 400, Height = 300 }; var grandchild = new OneWindow { Width = 320, Height = 240 };
        childWindow.Closing += (_, args) => args.Cancel = true; grandchild.Closing += (_, args) => args.Cancel = true;
        parent.Opened += (_, _) => { childWindow.Show(parent); grandchild.Show(childWindow); throw failure; };
        check(Throws(() => login.CompleteLogin(() => parent)), "LoginWindow: exceptions after opening owned native windows abort the replacement"); Flush();
        check(login.IsVisible && survivor.IsVisible && !parent.IsVisible && !childWindow.IsVisible && !grandchild.IsVisible && desktop.MainWindow == login && desktop.Windows.Count == 2 && OneWindow.Current == survivor && Part<WindowNotificationManager>(survivor, "PART_Toasts").GetVisualDescendants().OfType<NotificationCard>().Count() == 1, "LoginWindow: rollback disposes the replacement's owned descendants and keeps unrelated windows and hosts alive");
        CloseAll(desktop);
    }

    private static void VerifyAsyncAndStale(IClassicDesktopStyleApplicationLifetime desktop, Action<bool, string> check)
    {
        var login = Open(desktop); OneWindow? main = null; var factoryOnUiThread = false;
        var background = Task.Run(() => login.CompleteLoginAsync(() => { factoryOnUiThread = Dispatcher.UIThread.CheckAccess(); return main = new OneWindow { Width = 640, Height = 420 }; })); Wait(background);
        check(background.GetAwaiter().GetResult() && factoryOnUiThread && desktop.MainWindow == main && main is { IsVisible: true }, "LoginWindow: background authentication completion constructs and opens main on the UI thread"); CloseAll(desktop);
        login = Open(desktop); login.Close(); Flush(); var calls = 0;
        var late = Task.Run(() => login.CompleteLoginAsync(() => { calls++; return new OneWindow(); })); Wait(late);
        check(late.IsFaulted && late.Exception!.GetBaseException() is InvalidOperationException && calls == 0 && !desktop.Windows.Any(), "LoginWindow: a late authentication callback cannot reopen an already closed login");
        login = new LoginWindow();
        check(Throws(() => login.CompleteLogin(() => new OneWindow())), "LoginWindow: completion requires an open visible desktop LegacyPreviewWindow"); login.Close();
        var other = new Window(); login = new LoginWindow(); desktop.MainWindow = other; login.Show(); Flush();
        check(Throws(() => login.CompleteLogin(() => new OneWindow())) && desktop.MainWindow == other && login.IsVisible, "LoginWindow: a secondary login cannot replace an unrelated desktop LegacyPreviewWindow"); login.Close(); other.Close(); Flush();
    }

    private static void VerifyDemo(IClassicDesktopStyleApplicationLifetime desktop, Action<bool, string> check)
    {
        var login = new LoginWindow(); var calls = 0;
        // Keep validation pending until the busy bindings are observed, even if a
        // concurrent native build takes longer than the demo's short delay.
        var validationGate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var model = new LoginDemoViewModel(async () =>
        {
            await validationGate.Task;
            return await login.CompleteLoginAsync(() => { calls++; return new OneWindow { Width = 640, Height = 420 }; });
        }, login.Close);
        var view = new LoginDemoView { DataContext = model }; login.Content = view; login.Closed += (_, _) => model.Stop(); desktop.MainWindow = login; login.Show(); Flush();
        var account = view.FindControl<TextBox>("LoginAccount")!; var password = view.FindControl<TextBox>("LoginPassword")!;
        check(ReferenceEquals(view.FindControl<Button>("LoginSubmit")!.Command, model.LoginCommand) && account.Text == model.Account && password.Text == model.Password && view.FindControl<TextBlock>("LoginStatus")!.Text == model.Status, "LoginWindow: the consumer login form resolves compiled input, status, and command bindings");
        var validation = model.LoginAsync(); Wait(validation);
        check(calls == 0 && login.IsVisible && !model.IsBusy, "LoginWindow: failed caller validation keeps login and does not construct the main window");
        account.Text = "preview-user"; password.Text = "preview-password"; Flush();
        check(model.Account == account.Text && model.Password == password.Text, "LoginWindow: compiled account and password edits reach the caller-owned model");
        var completion = model.LoginAsync(); Flush();
        check(model.IsBusy && !account.IsEffectivelyEnabled && !view.FindControl<Button>("LoginSubmit")!.IsEffectivelyEnabled, "LoginWindow: consumer busy bindings disable duplicate validation and editing");
        validationGate.SetResult();
        Wait(completion);
        check(calls == 1 && !login.IsVisible && desktop.MainWindow is OneWindow { IsVisible: true } && !model.CanEdit && model.Password == "", "LoginWindow: the compiled consumer validates then opens main and clears closed login state"); CloseAll(desktop);
        login = new LoginWindow(); calls = 0;
        model = new LoginDemoViewModel(() => { calls++; return Task.FromResult(true); }, login.Close) { Account = "pending-user", Password = "pending-password" };
        login.Content = new LoginDemoView { DataContext = model }; login.Closed += (_, _) => model.Stop(); desktop.MainWindow = login; login.Show(); Flush();
        completion = model.LoginAsync(); login.Close(); Flush(); Wait(completion);
        check(calls == 0 && !model.CanEdit && model.Password == "" && !desktop.Windows.Any(), "LoginWindow: closing during caller validation cancels delayed completion without creating another host");
    }

    public static void Render(string output)
    {
        Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var model = new LoginDemoViewModel(() => Task.FromResult(true), () => { });
            var login = new LoginWindow { Title = "TheOne · 登录", RequestedThemeVariant = theme, Content = new LoginDemoView { DataContext = model } };
            ControlSizing.SetSize(login, size); Motion.SetEnabled(login, false); login.Show(); Flush();
            using var bitmap = login.CaptureRenderedFrame()!;
            var path = Path.Combine(output, $"loginwindow-{theme.Key}-{size}.png"); bitmap.Save(path, PngBitmapEncoderOptions.Default); Console.WriteLine($"Rendered {path}");
            login.Close(); model.Stop(); Flush();
        }
    }

    public static void StartLifetimeVerification(ShutdownMode mode, bool closeDuringLoginClosed = false)
    {
        LoginLifetimeApp.Mode = mode;
        LoginLifetimeApp.CloseDuringLoginClosed = closeDuringLoginClosed;
        var result = AppBuilder.Configure<LoginLifetimeApp>().WithInterFont().UseSkia()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false })
            .StartWithClassicDesktopLifetime([], mode);
        if (LoginLifetimeApp.Failure is { } failure) throw new InvalidOperationException($"LoginWindow/{mode} lifetime failed.", failure);
        if (result != 0 || !LoginLifetimeApp.Completed || LoginLifetimeApp.ExitCount != 1)
            throw new InvalidOperationException($"LoginWindow/{mode}: the desktop lifetime did not exit once after main closed.");
        Console.WriteLine($"PASS: LoginWindow/{mode}: real classic desktop main loop restores shutdown mode{(closeDuringLoginClosed ? " when Closed immediately closes main" : " after login closes")}");
    }

    private sealed class LoginLifetimeApp : Application
    {
        internal static ShutdownMode Mode;
        internal static bool CloseDuringLoginClosed;
        internal static Exception? Failure;
        internal static bool Completed;
        internal static int ExitCount;
        public override void Initialize() => Styles.Add(new OneTheme());
        public override void OnFrameworkInitializationCompleted()
        {
            var desktop = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
            desktop.Exit += (_, _) => ExitCount++;
            var login = new LoginWindow { Content = new TextBox { Text = "Real desktop startup" } };
            desktop.MainWindow = login;
            login.Opened += (_, _) => Dispatcher.UIThread.Post(() =>
            {
                try
                {
                    var auxiliary = new Window { Width = 320, Height = 240 }; auxiliary.Show();
                    var main = new OneWindow { Width = 640, Height = 420 };
                    if (CloseDuringLoginClosed) login.Closed += (_, _) => main.Close();
                    if (!login.CompleteLogin(main) || desktop.MainWindow != main || login.IsVisible || desktop.ShutdownMode != Mode || desktop.Windows.Contains(login))
                        throw new InvalidOperationException("Login close ended the lifetime or failed to transfer its LegacyPreviewWindow.");
                    if (!CloseDuringLoginClosed)
                    {
                        if (!main.IsVisible || ExitCount != 0) throw new InvalidOperationException("Login close exited before main became usable.");
                        OneToast.Show("Desktop survived login", expiration: TimeSpan.Zero);
                        var dialog = OneDialog.ShowAsync("Main loop", "Host is alive"); Flush();
                        if (!main.IsDialogOpen) throw new InvalidOperationException("Main dialog host was not attached.");
                        var presenter = main.GetVisualDescendants().OfType<OneDialogPresenter>().Single();
                        Part<Button>(presenter, "PART_PrimaryButton").RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); Wait(dialog);
                        if (dialog.GetAwaiter().GetResult() != OneDialogResult.Ok) throw new InvalidOperationException("Main dialog host did not complete its result.");
                        main.Close();
                    }
                    else if (main.IsVisible || desktop.Windows.Contains(main) || OneWindow.Current is not null)
                        throw new InvalidOperationException("Closing main inside login Closed left a stale window or host.");
                    if (Mode == ShutdownMode.OnMainWindowClose)
                    {
                        if (ExitCount != 1 || auxiliary.IsVisible) throw new InvalidOperationException("Closing main did not end the classic lifetime.");
                    }
                    else
                    {
                        if (ExitCount != 0 || !auxiliary.IsVisible) throw new InvalidOperationException("Closing main ignored remaining-window shutdown semantics.");
                        auxiliary.Close();
                        if (Mode == ShutdownMode.OnLastWindowClose && ExitCount != 1) throw new InvalidOperationException("Closing the last window did not exit.");
                        if (Mode == ShutdownMode.OnExplicitShutdown)
                        {
                            if (ExitCount != 0) throw new InvalidOperationException("Explicit shutdown mode exited on window close.");
                            desktop.Shutdown();
                        }
                    }
                    Completed = true;
                }
                catch (Exception error) { Failure = error; desktop.Shutdown(1); }
            });
            base.OnFrameworkInitializationCompleted();
        }
    }
}
