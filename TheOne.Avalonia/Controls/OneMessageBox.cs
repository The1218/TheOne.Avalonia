using System.Windows.Input;
using Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

/// <summary>Presentation options for a simple confirm/cancel message box.</summary>
public sealed record MessageBoxOptions
{
    public string ConfirmText { get; init; } = "确认";
    public string CancelText { get; init; } = "取消";
    public bool IsDangerous { get; init; }

    /// <summary>Overrides the owner window's inherited density when specified.</summary>
    public ControlSize? Size { get; init; }
}

/// <summary>Simple, command-driven confirmation using the existing themed window dialog host.</summary>
public static class OneMessageBox
{
    /// <summary>
    ///     Returns true only after explicit confirmation. Cancel, Escape and owner closure return false.
    ///     Cancellation of <paramref name="cancellationToken" /> cancels the returned task.
    /// </summary>
    public static async Task<bool> ConfirmAsync(string title, string message, MessageBoxOptions? options = null,
        Window? owner = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        options ??= new MessageBoxOptions();
        ArgumentException.ThrowIfNullOrWhiteSpace(options.ConfirmText);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.CancelText);
        if (options.Size is { } size && !Enum.IsDefined(size)) throw new ArgumentOutOfRangeException(nameof(options));
        cancellationToken.ThrowIfCancellationRequested();

        var context = new MessageBoxContext(title, message, options);
        var result = await OneDialog.ShowAsync<OneMessageBoxView, MessageBoxContext>(context,
            new DialogOptions
            {
                Title = title, ShowTitle = false, ShowCloseButton = false, ShowButtons = false,
                CloseOnEscape = true, CloseOnOverlayClick = false, Size = options.Size
            }, owner, cancellationToken);
        return result == OneDialogResult.Ok;
    }
}

internal sealed class MessageBoxContext : DialogContext
{
    public MessageBoxContext(string title, string message, MessageBoxOptions options)
    {
        Title = title;
        Message = message;
        ConfirmText = options.ConfirmText;
        CancelText = options.CancelText;
        IsDangerous = options.IsDangerous;
        ConfirmCommand = new MessageBoxCommand(() => RequestClose());
        CancelCommand = new MessageBoxCommand(() => RequestClose(OneDialogResult.Cancel));
    }

    public string Title { get; }
    public string Message { get; }
    public string ConfirmText { get; }
    public string CancelText { get; }
    public bool IsDangerous { get; }
    public string Symbol => IsDangerous ? "!" : "?";
    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }

    private sealed class MessageBoxCommand(Action execute) : ICommand
    {
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            execute();
        }

        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }
    }
}