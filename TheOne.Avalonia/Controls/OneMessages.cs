using global::Avalonia.Controls;
using global::Avalonia.Controls.Notifications;
using global::Avalonia.Controls.Templates;
using global::Avalonia.Threading;

namespace TheOne.Avalonia.Controls;

// UI-thread-only, weak references avoid retaining a closed application window.
internal sealed class MessageOwner
{
    private WeakReference<Window>? _owner;
    public Window? Value
    {
        get { Dispatcher.UIThread.VerifyAccess(); return _owner is not null && _owner.TryGetTarget(out var window) ? window : null; }
        set
        {
            Dispatcher.UIThread.VerifyAccess();
            if (value is not null and not OneWindow and not LoginWindow)
                throw new ArgumentException("Message owners must be OneWindow or LoginWindow.", nameof(value));
            _owner = value is null ? null : new(value);
        }
    }
}

internal static class OneWindowRegistry
{
    private static readonly List<WeakReference<Window>> Windows = [];
    public static OneWindow? Current => VisibleWindows().OfType<OneWindow>().FirstOrDefault();
    private static IEnumerable<Window> VisibleWindows()
    {
        Dispatcher.UIThread.VerifyAccess();
        for (var i = Windows.Count - 1; i >= 0; i--)
        {
            if (!Windows[i].TryGetTarget(out var window)) { Windows.RemoveAt(i); continue; }
            if (window.IsVisible && window.PlatformImpl is not null) yield return window;
        }
    }
    public static void SetOwners(Window window)
    {
        if (OneToast.Owner != window) OneToast.Owner = window;
        if (OneNotification.Owner != window) OneNotification.Owner = window;
        if (OneDialog.Owner != window) OneDialog.Owner = window;
    }
    public static void Activate(Window window)
    {
        RemoveReference(window); Windows.Add(new(window)); SetOwners(window);
    }
    private static void RemoveReference(Window window) => Windows.RemoveAll(reference => !reference.TryGetTarget(out var target) || target == window);
    public static void Remove(Window window)
    {
        RemoveReference(window);
        var fallback = VisibleWindows().FirstOrDefault();
        if (OneToast.Owner == window) OneToast.Owner = fallback;
        if (OneNotification.Owner == window) OneNotification.Owner = fallback;
        if (OneDialog.Owner == window) OneDialog.Owner = fallback;
    }
    public static Window Resolve(Window? owner, Window? defaultOwner)
    {
        Dispatcher.UIThread.VerifyAccess();
        if (owner is not null)
        {
            if (VisibleWindows().Contains(owner)) return owner;
            throw new InvalidOperationException("The specified message owner must be an open, visible OneWindow or LoginWindow.");
        }
        if (defaultOwner is not null && VisibleWindows().Contains(defaultOwner)) return defaultOwner;
        return VisibleWindows().FirstOrDefault() ?? throw new InvalidOperationException("Open a OneWindow or LoginWindow before showing a message or dialog.");
    }
    public static WindowNotificationManager MessageHost(Window window, bool toast) => window switch
    {
        OneWindow main => main.MessageHost(toast),
        LoginWindow login => login.MessageHost(toast),
        _ => throw new InvalidOperationException("The window has no message host.")
    };
    public static DialogCoordinator Dialogs(Window window) => window switch
    {
        OneWindow main => main.Dialogs,
        LoginWindow login => login.Dialogs,
        _ => throw new InvalidOperationException("The window has no dialog host.")
    };
}

public static class OneNotification
{
    private static readonly MessageOwner DefaultOwner = new();
    /// <summary>Default message owner, automatically updated when a themed window opens or activates. Access on the UI thread.</summary>
    public static Window? Owner { get => DefaultOwner.Value; set => DefaultOwner.Value = value; }
    /// <summary>Shows on the UI thread. The owner is optional; the registered default owner or most recently activated visible themed window is used.</summary>
    public static void Show(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Window? owner = null)
        => Show(new Notification(title, message, type, expiration ?? TimeSpan.FromSeconds(4)), owner);
    public static void Show(INotification notification, Window? owner = null)
    {
        ArgumentNullException.ThrowIfNull(notification);
        OneWindowRegistry.MessageHost(OneWindowRegistry.Resolve(owner, Owner), false).Show(notification);
    }
    public static async Task ShowAsync(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Window? owner = null)
        => await Dispatcher.UIThread.InvokeAsync(() => Show(title, message, type, expiration, owner));
    public static void CloseAll(Window? owner = null) => OneWindowRegistry.MessageHost(OneWindowRegistry.Resolve(owner, Owner), false).CloseAll();
}
public static class OneToast
{
    private static readonly MessageOwner DefaultOwner = new();
    /// <summary>Default message owner, automatically updated when a themed window opens or activates. Access on the UI thread.</summary>
    public static Window? Owner { get => DefaultOwner.Value; set => DefaultOwner.Value = value; }
    public static void Show(string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Window? owner = null)
        => OneWindowRegistry.MessageHost(OneWindowRegistry.Resolve(owner, Owner), true).Show(new Notification("", message, type, expiration ?? TimeSpan.FromSeconds(2.5)));
    /// <summary>Shows a toast with a title, expiration and click/close callbacks.</summary>
    public static void Show(INotification notification, Window? owner = null)
    {
        ArgumentNullException.ThrowIfNull(notification);
        OneWindowRegistry.MessageHost(OneWindowRegistry.Resolve(owner, Owner), true).Show(notification);
    }
    public static async Task ShowAsync(string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Window? owner = null)
        => await Dispatcher.UIThread.InvokeAsync(() => Show(message, type, expiration, owner));
    public static void CloseAll(Window? owner = null) => OneWindowRegistry.MessageHost(OneWindowRegistry.Resolve(owner, Owner), true).CloseAll();
}
public enum OneDialogButtons { Ok, OkCancel, YesNo, YesNoCancel }
public enum OneDialogResult { Ok, Cancel, Yes, No, Closed }
public static class OneDialog
{
    private static readonly MessageOwner DefaultOwner = new();
    /// <summary>Default message owner, automatically updated when a themed window opens or activates. Access on the UI thread.</summary>
    public static Window? Owner { get => DefaultOwner.Value; set => DefaultOwner.Value = value; }
    /// <summary>Creates a typed view lazily on the UI thread; compiled bindings and NativeAOT are supported.</summary>
    public static async Task<OneDialogResult> ShowAsync<TView, TViewModel>(TViewModel viewModel, DialogOptions? options = null, Window? owner = null, CancellationToken cancellationToken = default)
        where TView : UserControl, new()
        where TViewModel : class
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        cancellationToken.ThrowIfCancellationRequested();
        return await Dispatcher.UIThread.InvokeAsync(() => OneWindowRegistry.Dialogs(OneWindowRegistry.Resolve(owner, Owner)).Enqueue(
            null, options ?? new DialogOptions(), null, cancellationToken,
            () => new TView { DataContext = viewModel }, viewModel as DialogContext));
    }

    public static Task<OneDialogResult> ShowAsync(string title, string message, OneDialogButtons buttons = OneDialogButtons.Ok, Window? owner = null, CancellationToken cancellationToken = default)
        => ShowContentAsync(message, title, buttons, null, owner, cancellationToken);
    public static async Task<OneDialogResult> ShowContentAsync(object? content, string? title = null, OneDialogButtons buttons = OneDialogButtons.OkCancel, IDataTemplate? contentTemplate = null, Window? owner = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await Dispatcher.UIThread.InvokeAsync(() => OneWindowRegistry.Dialogs(OneWindowRegistry.Resolve(owner, Owner)).Enqueue(content, new DialogOptions { Title = title, Buttons = buttons, ShowButtons = true }, contentTemplate, cancellationToken));
    }
}
