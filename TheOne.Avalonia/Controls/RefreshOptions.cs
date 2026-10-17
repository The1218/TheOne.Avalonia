using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace TheOne.Avalonia.Controls;

/// <summary>Density for the native refresh indicator, including the container-created visualizer.</summary>
public sealed class RefreshOptions : AvaloniaObject
{
    public static readonly AttachedProperty<double> IndicatorSizeProperty =
        AvaloniaProperty.RegisterAttached<RefreshOptions, Control, double>("IndicatorSize", 22, true);

    public static readonly AttachedProperty<bool> UseThemeVisualizerSizingProperty =
        AvaloniaProperty.RegisterAttached<RefreshOptions, RefreshContainer, bool>("UseThemeVisualizerSizing");

    private static readonly ConditionalWeakTable<RefreshContainer, State> States = new();

    static RefreshOptions()
    {
        UseThemeVisualizerSizingProperty.Changed.AddClassHandler<RefreshContainer>((container, change) =>
            States.GetValue(container, control => new State(control)).SetEnabled(change.GetNewValue<bool>()));
    }

    public static double GetIndicatorSize(Control control)
    {
        return control.GetValue(IndicatorSizeProperty);
    }

    public static void SetIndicatorSize(Control control, double value)
    {
        control.SetValue(IndicatorSizeProperty, value);
    }

    public static bool GetUseThemeVisualizerSizing(RefreshContainer container)
    {
        return container.GetValue(UseThemeVisualizerSizingProperty);
    }

    public static void SetUseThemeVisualizerSizing(RefreshContainer container, bool value)
    {
        container.SetValue(UseThemeVisualizerSizingProperty, value);
    }

    private sealed class State(RefreshContainer container)
    {
        private bool _awaitingDefault;
        private bool _enabled;
        private RefreshVisualizer? _managed;
        private bool _queued;

        public void SetEnabled(bool enabled)
        {
            if (_enabled == enabled) return;
            _enabled = enabled;
            container.PropertyChanged -= Changed;
            if (!enabled) return;
            _awaitingDefault = container.Visualizer is null;
            container.PropertyChanged += Changed;
        }

        private void Changed(object? sender, AvaloniaPropertyChangedEventArgs args)
        {
            if (args.Property == RefreshContainer.VisualizerProperty)
            {
                if (_awaitingDefault && container.Visualizer is { } visualizer)
                {
                    _managed = visualizer;
                    _awaitingDefault = false;
                }
                else if (container.Visualizer != _managed)
                {
                    _managed = null;
                    _awaitingDefault = container.Visualizer is null;
                }

                Queue();
            }
            else if (args.Property == RefreshContainer.PullDirectionProperty)
            {
                Queue();
            }
        }

        private void Queue()
        {
            if (_queued) return;
            _queued = true;
            Dispatcher.UIThread.Post(() =>
            {
                _queued = false;
                if (!_enabled || _managed is null || container.Visualizer != _managed) return;
                var horizontal = container.PullDirection is PullDirection.LeftToRight or PullDirection.RightToLeft;
                _managed.Classes.Set("one-refresh-horizontal", horizontal);
                _managed.Classes.Set("one-refresh-bottom", container.PullDirection == PullDirection.BottomToTop);
                _managed.Classes.Set("one-refresh-left", container.PullDirection == PullDirection.LeftToRight);
                _managed.Classes.Set("one-refresh-right", container.PullDirection == PullDirection.RightToLeft);
                // The native container assigns a local 100 px size to its own visualizer.
                // Release those defaults so density styles can apply; preserve custom sizes.
                if (_managed.IsSet(Control.HeightProperty) && (_managed.Height == 100 || double.IsNaN(_managed.Height)))
                    _managed.ClearValue(Control.HeightProperty);
                if (_managed.IsSet(Control.WidthProperty) && (_managed.Width == 100 || double.IsNaN(_managed.Width)))
                    _managed.ClearValue(Control.WidthProperty);
            }, DispatcherPriority.Loaded);
        }
    }
}