using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

/// <summary>Connects native content transitions, including consumer-provided transitions, to Motion.Enabled.</summary>
public sealed class ContentTransitionOptions : AvaloniaObject
{
    public static readonly AttachedProperty<bool> UseMotionSettingsProperty =
        AvaloniaProperty.RegisterAttached<ContentTransitionOptions, TransitioningContentControl, bool>(
            "UseMotionSettings");

    private static readonly ConditionalWeakTable<TransitioningContentControl, State> States = new();

    static ContentTransitionOptions()
    {
        UseMotionSettingsProperty.Changed.AddClassHandler<TransitioningContentControl>((control, change) =>
            States.GetValue(control, owner => new State(owner)).Enable(change.GetNewValue<bool>()));
    }

    public static bool GetUseMotionSettings(TransitioningContentControl control)
    {
        return control.GetValue(UseMotionSettingsProperty);
    }

    public static void SetUseMotionSettings(TransitioningContentControl control, bool value)
    {
        control.SetValue(UseMotionSettingsProperty, value);
    }

    public static IPageTransition? GetConfiguredTransition(TransitioningContentControl control)
    {
        return control.PageTransition is ManagedTransition managed ? managed.Inner : control.PageTransition;
    }

    private sealed class ManagedTransition(State state, IPageTransition inner) : IPageTransition
    {
        public IPageTransition Inner { get; } = inner;

        public Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken)
        {
            return state.Start(Inner, from, to, forward, cancellationToken);
        }

        public bool IsOwnedBy(State owner)
        {
            return ReferenceEquals(state, owner);
        }
    }

    private sealed class Operation(Visual? from, Visual? to, object? content, CancellationToken token) : IDisposable
    {
        public Visual? From { get; } = from;
        public Visual? To { get; } = to;
        public object? Content { get; } = content;
        public CancellationTokenSource Cancel { get; } = CancellationTokenSource.CreateLinkedTokenSource(token);

        public void Dispose()
        {
            Cancel.Dispose();
        }
    }

    private sealed class State(TransitioningContentControl control)
    {
        private readonly ConditionalWeakTable<Visual, Pose> _poses = new();
        private Operation? _active;
        private bool _enabled;
        private bool _setting;

        public void Enable(bool enabled)
        {
            if (_enabled == enabled) return;
            _enabled = enabled;
            if (enabled)
            {
                control.PropertyChanged += Changed;
                control.DetachedFromVisualTree += Detached;
                Wrap();
            }
            else
            {
                CancelActive();
                control.PropertyChanged -= Changed;
                control.DetachedFromVisualTree -= Detached;
                SetTransition(GetConfiguredTransition(control));
            }
        }

        private void Detached(object? sender, VisualTreeAttachmentEventArgs args)
        {
            CancelActive();
        }

        private void Changed(object? sender, AvaloniaPropertyChangedEventArgs args)
        {
            if (_setting) return;
            if (args.Property == TransitioningContentControl.PageTransitionProperty)
            {
                CancelActive();
                Wrap();
            }
            else if (args.Property == Motion.EnabledProperty && !Motion.GetEnabled(control))
            {
                CancelActive();
            }
            else if (args.Property == TransitioningContentControl.ContentProperty ||
                     args.Property == TransitioningContentControl.TemplateProperty)
            {
                CancelActive();
            }
        }

        private void Wrap()
        {
            if (control.PageTransition is not { } transition) return;
            if (transition is ManagedTransition managed)
            {
                if (managed.IsOwnedBy(this)) return;
                transition = managed.Inner;
            }

            SetTransition(new ManagedTransition(this, transition));
        }

        private void SetTransition(IPageTransition? transition)
        {
            _setting = true;
            try
            {
                control.SetCurrentValue(TransitioningContentControl.PageTransitionProperty, transition);
            }
            finally
            {
                _setting = false;
            }
        }

        private void Restore(Visual? visual)
        {
            if (visual is null || !_poses.TryGetValue(visual, out var pose)) return;
            visual.SetCurrentValue(Visual.OpacityProperty, pose.Opacity);
            visual.SetCurrentValue(Visual.RenderTransformProperty, pose.Transform);
            if (visual is InputElement input)
                input.SetCurrentValue(InputElement.IsHitTestVisibleProperty, pose.HitTestVisible);
        }

        private void Settle(Operation operation)
        {
            Restore(operation.From);
            Restore(operation.To);
            // An older task must not hide a presenter now holding newer content.
            if (!Equals(control.Content, operation.Content)) return;
            if (operation.From is not null) operation.From.IsVisible = false;
            if (operation.To is not null) operation.To.IsVisible = true;
        }

        private void CancelActive()
        {
            if (_active is not { } operation) return;
            operation.Cancel.Cancel();
            Settle(operation);
        }

        public async Task Start(IPageTransition transition, Visual? from, Visual? to, bool forward,
            CancellationToken token)
        {
            CancelActive();
            if (from is not null) _poses.GetValue(from, Capture);
            if (to is not null) _poses.GetValue(to, Capture);
            using var operation = new Operation(from, to, control.Content, token);
            _active = operation;
            try
            {
                if (!Motion.GetEnabled(control)) operation.Cancel.Cancel();
                operation.Cancel.Token.ThrowIfCancellationRequested();
                if (from is InputElement outgoing)
                    outgoing.SetCurrentValue(InputElement.IsHitTestVisibleProperty, false);
                await transition.Start(from, to, forward, operation.Cancel.Token);
                // Some native transitions return normally when cancelled. Keep the event truthful.
                operation.Cancel.Token.ThrowIfCancellationRequested();
            }
            finally
            {
                if (ReferenceEquals(_active, operation))
                {
                    Settle(operation);
                    _active = null;
                }
            }
        }

        private static Pose Capture(Visual visual)
        {
            return new Pose(visual.Opacity, visual.RenderTransform,
                visual is InputElement input && input.IsHitTestVisible);
        }

        private sealed record Pose(double Opacity, ITransform? Transform, bool HitTestVisible);
    }
}