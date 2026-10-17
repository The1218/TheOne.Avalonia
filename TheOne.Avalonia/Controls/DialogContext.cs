using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TheOne.Avalonia.Controls;

public sealed class DialogCloseRequestedEventArgs(OneDialogResult result) : EventArgs
{
    public OneDialogResult Result { get; } = result;
}

/// <summary>Optional base for dialog view models. Does not depend on a window or dispatcher.</summary>
public abstract class DialogContext : INotifyPropertyChanged
{
    private readonly object _gate = new();
    private object? _session;
    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<DialogCloseRequestedEventArgs>? CloseRequested;

    /// <summary>Requests completion of this dialog, including when it is still queued.</summary>
    public void RequestClose(OneDialogResult result = OneDialogResult.Ok)
    {
        if (!Enum.IsDefined(result)) throw new ArgumentOutOfRangeException(nameof(result));
        CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(result));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    internal void Attach(object session, EventHandler<DialogCloseRequestedEventArgs> handler)
    {
        lock (_gate)
        {
            if (_session is not null)
                throw new InvalidOperationException("A DialogContext cannot be used by simultaneous dialog requests.");
            _session = session;
            CloseRequested += handler;
        }
    }

    internal void Detach(object session, EventHandler<DialogCloseRequestedEventArgs> handler)
    {
        lock (_gate)
        {
            if (_session != session) return;
            CloseRequested -= handler;
            _session = null;
        }
    }
}