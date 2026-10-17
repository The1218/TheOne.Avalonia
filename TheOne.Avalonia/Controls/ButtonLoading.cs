using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace TheOne.Avalonia.Controls;

/// <summary>Loading state for official Button controls; the temporary override preserves IsEnabled bindings.</summary>
public sealed class ButtonLoading : AvaloniaObject
{
    public static readonly AttachedProperty<bool> IsLoadingProperty =
        AvaloniaProperty.RegisterAttached<ButtonLoading, Button, bool>("IsLoading");

    private static readonly ConditionalWeakTable<Button, State> States = new();

    static ButtonLoading()
    {
        IsLoadingProperty.Changed.AddClassHandler<Button>((button, _) =>
            States.GetValue(button, control => new State(control)).Apply(GetIsLoading(button)));
    }

    public static bool GetIsLoading(Button button)
    {
        return button.GetValue(IsLoadingProperty);
    }

    public static void SetIsLoading(Button button, bool value)
    {
        button.SetValue(IsLoadingProperty, value);
    }

    private sealed class LoadingGate : ICommand
    {
        public static LoadingGate Instance { get; } = new();

        public bool CanExecute(object? parameter)
        {
            return false;
        }

        public void Execute(object? parameter)
        {
        }

        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }
    }

    private sealed class State(Button button)
    {
        private IDisposable? _disabled;

        public void Apply(bool loading)
        {
            // Gate the native Button.IsEnabledCore through CanExecute, without changing IsEnabled.
            // A disposable override leaves the consumer's original command binding/value intact.
            if (loading)
            {
                _disabled ??= button.SetValue(Button.CommandProperty, LoadingGate.Instance, BindingPriority.Animation);
            }
            else
            {
                _disabled?.Dispose();
                _disabled = null;
            }
        }
    }
}