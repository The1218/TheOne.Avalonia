using System.Runtime.ExceptionServices;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.ApplicationLifetimes;
using global::Avalonia.Controls.Notifications;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Input;
using global::Avalonia.Threading;

namespace TheOne.Avalonia.Controls;

/// <summary>A themed native Window whose caller-owned content performs authentication.</summary>
public class LoginWindow : Window
{
    public static readonly DirectProperty<LoginWindow, bool> IsDialogOpenProperty = AvaloniaProperty.RegisterDirect<LoginWindow, bool>(nameof(IsDialogOpen), window => window.IsDialogOpen);
    private bool _isDialogOpen;
    private Panel? _dialogLayer, _dialogShield;
    private WindowNotificationManager? _notifications, _toasts;
    internal DialogCoordinator Dialogs { get; }
    public bool IsDialogOpen => _isDialogOpen;
    private bool _completing, _handlingClosing, _completed;
    private Func<OneWindow>? _mainWindowFactory;
    private IClassicDesktopStyleApplicationLifetime? _desktop;
    private OneWindow? _pendingMainWindow;
    private ExceptionDispatchInfo? _transitionError;

    protected override Type StyleKeyOverride => typeof(LoginWindow);

    public LoginWindow()
    {
        Dialogs = new DialogCoordinator(this, DialogLayer, SetDialogOpen);
        Opened += (_, _) => OneWindowRegistry.Activate(this);
        Activated += (_, _) => OneWindowRegistry.Activate(this);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs args)
    {
        if (_dialogShield is not null) _dialogShield.PointerReleased -= OverlayReleased;
        base.OnApplyTemplate(args);
        _notifications = args.NameScope.Find<WindowNotificationManager>("PART_Notifications");
        _toasts = args.NameScope.Find<WindowNotificationManager>("PART_Toasts");
        _dialogLayer = args.NameScope.Find<Panel>("PART_DialogLayer");
        _dialogShield = args.NameScope.Find<Panel>("PART_DialogShield");
        if (_dialogShield is not null) _dialogShield.PointerReleased += OverlayReleased;
        Dialogs.TemplateChanged(_dialogLayer);
    }

    private void OverlayReleased(object? sender, PointerReleasedEventArgs args)
    {
        if (ReferenceEquals(args.Source, _dialogShield) && args.InitialPressMouseButton == MouseButton.Left) Dialogs.OverlayClicked();
    }

    internal WindowNotificationManager MessageHost(bool toast)
    {
        ApplyTemplate();
        var host = (toast ? _toasts : _notifications) ?? throw new InvalidOperationException("LoginWindow template must contain PART_Notifications and PART_Toasts.");
        host.ApplyTemplate(); return host;
    }

    private Panel DialogLayer()
    {
        ApplyTemplate(); return _dialogLayer ?? throw new InvalidOperationException("LoginWindow template must contain PART_DialogLayer.");
    }

    private void SetDialogOpen(bool value)
    {
        SetAndRaise(IsDialogOpenProperty, ref _isDialogOpen, value); PseudoClasses.Set(":dialog-open", value);
    }

    /// <summary>Shows an error toast in this login window on the UI thread.</summary>
    public void ShowErrorToast(string message, TimeSpan? expiration = null)
        => OneToast.Show(message, NotificationType.Error, expiration, this);

    /// <summary>Shows an error notification in this login window on the UI thread.</summary>
    public void ShowErrorNotification(string message, string title = "登录失败", TimeSpan? expiration = null)
        => OneNotification.Show(title, message, NotificationType.Error, expiration, this);

    /// <summary>Shows an error dialog in this login window; dispatches to the UI thread.</summary>
    public Task<OneDialogResult> ShowErrorDialog(string message, string title = "登录失败", CancellationToken cancellationToken = default)
        => OneDialog.ShowAsync(title, message, OneDialogButtons.Ok, this, cancellationToken);

    /// <summary>Installs the supplied fresh, unopened OneWindow as the desktop MainWindow and all message owners, then closes login.</summary>
    /// <remarks>A Closing veto leaves the supplied window unopened. An accepted replacement that fails to open is disposed. Call on the UI thread.</remarks>
    public bool CompleteLogin(OneWindow mainWindow)
    {
        ArgumentNullException.ThrowIfNull(mainWindow);
        return CompleteLogin(() => mainWindow);
    }

    /// <summary>Dispatches the handoff of the supplied OneWindow to the UI thread. Construct the window on the UI thread.</summary>
    public async Task<bool> CompleteLoginAsync(OneWindow mainWindow)
        => await Dispatcher.UIThread.InvokeAsync(() => CompleteLogin(mainWindow));

    /// <summary>
    /// After successful authentication, replaces this desktop MainWindow with a fresh OneWindow.
    /// Returns false if Closing cancels the transition or a completion is already in progress.
    /// Repeated calls after success return true without invoking the factory again.
    /// </summary>
    /// <remarks>
    /// Call on the UI thread. This window must be the visible, unowned desktop MainWindow.
    /// The factory runs only after Closing accepts the close and transfers ownership of a new,
    /// unopened window to this operation. An aborted replacement is disposed. Authentication,
    /// dependency injection and application/Generic Host lifetime remain the caller's responsibility.
    /// </remarks>
    public bool CompleteLogin(Func<OneWindow> mainWindowFactory)
    {
        Dispatcher.UIThread.VerifyAccess();
        ArgumentNullException.ThrowIfNull(mainWindowFactory);
        if (_completed) return true;
        if (_completing) return false;
        if (PlatformImpl is null || !IsVisible || Owner is not null)
            throw new InvalidOperationException("LoginWindow must be open, visible and have no owner before completing login.");
        if (global::Avalonia.Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop || desktop.MainWindow != this)
            throw new InvalidOperationException("Set the desktop application's MainWindow to this LoginWindow before completing login.");

        var shutdownMode = desktop.ShutdownMode;
        var toastOwner = OneToast.Owner;
        var notificationOwner = OneNotification.Owner;
        var dialogOwner = OneDialog.Owner;
        _completing = true;
        _mainWindowFactory = mainWindowFactory;
        _desktop = desktop;
        try
        {
            // A replacement can close itself in Opened, or fail while applying its template.
            // Keep automatic shutdown suspended until the operation commits or rolls back.
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            Close();
            _transitionError?.Throw();
            if (PlatformImpl is not null) return false;
            _completed = true;
            if (_pendingMainWindow is { IsVisible: true, PlatformImpl: not null })
                _pendingMainWindow.Activate();
            return true;
        }
        finally
        {
            // Closed subscribers may throw after the login has already been disposed. The
            // independently opened main window must remain alive even in that case.
            if (PlatformImpl is null) _completed = true;
            try
            {
                if (!_completed)
                {
                    desktop.MainWindow = this;
                    // This is only the newly created, aborted replacement. Close() can itself
                    // be vetoed, leaving an orphan in the lifetime's window list and host registry.
                    if (_pendingMainWindow is { } replacement)
                    {
                        // Dispose native owned windows too; disposing the owner's platform
                        // implementation alone does not run Window.CloseInternal's child loop.
                        var children = desktop.Windows.Where(window => IsOwnedBy(window, replacement)).ToArray();
                        foreach (var child in children.Reverse()) child.PlatformImpl?.Dispose();
                        replacement.PlatformImpl?.Dispose();
                    }
                    OneToast.Owner = toastOwner;
                    OneNotification.Owner = notificationOwner;
                    OneDialog.Owner = dialogOwner;
                }
                else if (_pendingMainWindow is { IsVisible: true, PlatformImpl: not null } main)
                    OneWindowRegistry.SetOwners(main);
            }
            finally
            {
                _pendingMainWindow = null;
                _mainWindowFactory = null;
                _desktop = null;
                _transitionError = null;
                _completing = false;
                desktop.ShutdownMode = shutdownMode;
                // A Closed subscriber can also close the new main window while automatic
                // shutdown is suspended. Restoring ShutdownMode does not re-evaluate it.
                if (_completed && (shutdownMode == ShutdownMode.OnLastWindowClose && desktop.Windows.Count == 0
                    || shutdownMode == ShutdownMode.OnMainWindowClose && desktop.MainWindow is { PlatformImpl: null }))
                    desktop.TryShutdown();
            }
        }
    }

    private static bool IsOwnedBy(Window window, Window owner)
    {
        for (var parent = window.Owner; parent is not null; parent = parent.Owner)
            if (parent == owner) return true;
        return false;
    }

    /// <summary>Dispatches completion, including window construction, to the UI thread.</summary>
    public async Task<bool> CompleteLoginAsync(Func<OneWindow> mainWindowFactory)
        => await Dispatcher.UIThread.InvokeAsync(() => CompleteLogin(mainWindowFactory));

    protected override void OnClosed(EventArgs args)
    {
        OneWindowRegistry.Remove(this);
        Dialogs.CloseWindow();
        if (!_completing)
        {
            base.OnClosed(args);
            return;
        }
        try
        {
            base.OnClosed(args);
        }
        catch (Exception error)
        {
            // Let native teardown finish and raise WindowClosedEvent so the desktop lifetime
            // removes the login window, even if a consumer's Closed subscriber fails.
            _transitionError ??= ExceptionDispatchInfo.Capture(error);
        }
    }

    protected override void OnClosing(WindowClosingEventArgs args)
    {
        if (!_completing)
        {
            base.OnClosing(args);
            return;
        }
        if (_handlingClosing)
        {
            args.Cancel = true;
            return;
        }

        _handlingClosing = true;
        try
        {
            base.OnClosing(args);
            if (args.Cancel) return;
            var mainWindow = _mainWindowFactory!()
                ?? throw new InvalidOperationException("The main window factory must return a new OneWindow.");
            if (mainWindow.PlatformImpl is null || mainWindow.IsVisible || mainWindow.Owner is not null || _desktop!.Windows.Contains(mainWindow))
                throw new InvalidOperationException("The main window factory must return a new, unopened OneWindow with no owner.");
            _pendingMainWindow = mainWindow;
            if (_desktop!.MainWindow != this || PlatformImpl is null || !IsVisible)
                throw new InvalidOperationException("The login application's MainWindow changed during completion.");

            _desktop.MainWindow = mainWindow;
            OneWindowRegistry.SetOwners(mainWindow);
            mainWindow.Show();
            if (mainWindow.PlatformImpl is null || !mainWindow.IsVisible || mainWindow.Owner is not null || _desktop.MainWindow != mainWindow)
                throw new InvalidOperationException("The replacement OneWindow must remain open as the desktop MainWindow.");
            OneWindowRegistry.SetOwners(mainWindow);
        }
        catch (Exception error)
        {
            // Avalonia Window.CloseCore closes in finally when OnClosing throws. Return a
            // cancellation normally, then rethrow from CompleteLogin after Close returns.
            args.Cancel = true;
            _transitionError = ExceptionDispatchInfo.Capture(error);
        }
        finally
        {
            _handlingClosing = false;
        }
    }
}
