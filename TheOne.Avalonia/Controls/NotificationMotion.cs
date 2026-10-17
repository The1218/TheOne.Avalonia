using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

/// <summary>Drives native NotificationCard open/close lifecycle with cancellable, inherited motion.</summary>
public sealed class NotificationMotion : AvaloniaObject
{
    public static readonly AttachedProperty<bool> UseMotionSettingsProperty =
        AvaloniaProperty.RegisterAttached<NotificationMotion, NotificationCard, bool>("UseMotionSettings");

    public static readonly AttachedProperty<TimeSpan> EnterDurationProperty =
        AvaloniaProperty.RegisterAttached<NotificationMotion, NotificationCard, TimeSpan>("EnterDuration",
            TimeSpan.FromMilliseconds(200), validate: value => value >= TimeSpan.Zero);

    public static readonly AttachedProperty<TimeSpan> ExitDurationProperty =
        AvaloniaProperty.RegisterAttached<NotificationMotion, NotificationCard, TimeSpan>("ExitDuration",
            TimeSpan.FromMilliseconds(140), validate: value => value >= TimeSpan.Zero);

    private static readonly ConditionalWeakTable<NotificationCard, State> States = new();

    static NotificationMotion()
    {
        UseMotionSettingsProperty.Changed.AddClassHandler<NotificationCard>((card, change) =>
            States.GetValue(card, owner => new State(owner)).Enable(change.GetNewValue<bool>()));
    }

    public static bool GetUseMotionSettings(NotificationCard card)
    {
        return card.GetValue(UseMotionSettingsProperty);
    }

    public static void SetUseMotionSettings(NotificationCard card, bool value)
    {
        card.SetValue(UseMotionSettingsProperty, value);
    }

    public static TimeSpan GetEnterDuration(NotificationCard card)
    {
        return card.GetValue(EnterDurationProperty);
    }

    public static void SetEnterDuration(NotificationCard card, TimeSpan value)
    {
        card.SetValue(EnterDurationProperty, value);
    }

    public static TimeSpan GetExitDuration(NotificationCard card)
    {
        return card.GetValue(ExitDurationProperty);
    }

    public static void SetExitDuration(NotificationCard card, TimeSpan value)
    {
        card.SetValue(ExitDurationProperty, value);
    }

    private sealed class State(NotificationCard card)
    {
        private CancellationTokenSource? _animation;
        private bool _enabled;
        private bool _finishQueued;
        private double _opacity = 1;
        private bool _shown;
        private Control? _surface;
        private ITransform? _transform;

        public void Enable(bool enabled)
        {
            if (_enabled == enabled) return;
            _enabled = enabled;
            if (enabled)
            {
                card.TemplateApplied += TemplateApplied;
                card.Loaded += Loaded;
                card.DetachedFromVisualTree += Detached;
                card.PropertyChanged += Changed;
                if (card.IsClosing) Close();
                else if (card.IsLoaded) Show();
            }
            else
            {
                Cancel();
                card.TemplateApplied -= TemplateApplied;
                card.Loaded -= Loaded;
                card.DetachedFromVisualTree -= Detached;
                card.PropertyChanged -= Changed;
                if (card.IsClosing) Finish();
            }
        }

        private void TemplateApplied(object? sender, TemplateAppliedEventArgs args)
        {
            Cancel();
            _surface = args.NameScope.Find<Control>("PART_AnimationHost");
            _opacity = _surface?.Opacity ?? 1;
            _transform = _surface?.RenderTransform;
            if (card.IsClosing) Close();
        }

        private void Loaded(object? sender, RoutedEventArgs args)
        {
            if (card.IsClosing) Close();
            else Show();
        }

        private void Detached(object? sender, VisualTreeAttachmentEventArgs args)
        {
            Cancel();
            _shown = false;
            if (card.IsClosing && !card.IsClosed) Finish();
        }

        private void Changed(object? sender, AvaloniaPropertyChangedEventArgs args)
        {
            if (args.Property == NotificationCard.IsClosingProperty && card.IsClosing)
            {
                Close();
            }
            else if (args.Property == Motion.EnabledProperty && !Motion.GetEnabled(card))
            {
                Cancel();
                if (card.IsClosing) Finish();
            }
            else if (args.Property == NotificationCard.IsClosedProperty && card.IsClosed)
            {
                Cancel();
            }
            else if (args.Property == EnterDurationProperty || args.Property == ExitDurationProperty)
            {
                if (card.IsClosing) Close();
                else if (GetEnterDuration(card) == TimeSpan.Zero) Cancel();
            }
        }

        private void Show()
        {
            if (_shown || card.IsClosed) return;
            _shown = true;
            _surface ??= card.GetVisualDescendants().OfType<Control>().FirstOrDefault(control =>
                control.Name == "PART_AnimationHost" && control.TemplatedParent == card);
            if (_surface is null || !Motion.GetEnabled(card) || GetEnterDuration(card) == TimeSpan.Zero)
            {
                Restore();
                return;
            }

            _ = Animate(false, 0, 8);
        }

        private void Close()
        {
            if (card.IsClosed || _finishQueued) return;
            var opacity = _surface?.Opacity ?? _opacity;
            var y = _surface?.RenderTransform?.Value.M32 ?? 0;
            Cancel();
            if (_surface is null || !card.IsAttachedToVisualTree() || !Motion.GetEnabled(card) ||
                GetExitDuration(card) == TimeSpan.Zero)
            {
                Finish();
                return;
            }

            _ = Animate(true, opacity, y);
        }

        private async Task Animate(bool closing, double opacity, double y)
        {
            if (_surface is not { } surface) return;
            using var source = new CancellationTokenSource();
            _animation = source;
            var animation = new Animation
            {
                Duration = closing ? GetExitDuration(card) : GetEnterDuration(card),
                Easing = closing ? new CubicEaseIn() : new CubicEaseOut(),
                FillMode = FillMode.None,
                Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0),
                        Setters =
                        {
                            new Setter(Visual.OpacityProperty, opacity), new Setter(TranslateTransform.YProperty, y)
                        }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1),
                        Setters =
                        {
                            new Setter(Visual.OpacityProperty, closing ? 0d : _opacity),
                            new Setter(TranslateTransform.YProperty, closing ? -6d : 0d)
                        }
                    }
                }
            };
            try
            {
                await animation.RunAsync(surface, source.Token);
                if (ReferenceEquals(_animation, source) && !source.IsCancellationRequested && closing) Finish();
            }
            catch (OperationCanceledException) when (source.IsCancellationRequested)
            {
            }
            finally
            {
                if (ReferenceEquals(_animation, source))
                {
                    _animation = null;
                    Restore();
                }
            }
        }

        private void Cancel()
        {
            var source = _animation;
            _animation = null;
            source?.Cancel();
            Restore();
        }

        private void Restore()
        {
            if (_surface is null) return;
            _surface.SetCurrentValue(Visual.OpacityProperty, _finishQueued ? 0 : _opacity);
            _surface.SetCurrentValue(Visual.RenderTransformProperty, _transform);
        }

        private void Finish()
        {
            if (_finishQueued || card.IsClosed) return;
            _finishQueued = true;
            if (_surface is not null) _surface.SetCurrentValue(Visual.OpacityProperty, 0d);
            // CloseAll/Close(content) enumerate the manager's native collection. Complete
            // after that enumeration, even with zero motion, so removal is safe and once-only.
            Dispatcher.UIThread.Post(() =>
            {
                _finishQueued = false;
                if (!card.IsClosed) card.SetCurrentValue(NotificationCard.IsClosedProperty, true);
            }, DispatcherPriority.Loaded);
        }
    }
}