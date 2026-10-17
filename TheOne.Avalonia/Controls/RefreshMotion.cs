using System.Numerics;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

/// <summary>Applies Motion.Enabled to the native refresh compositor without changing refresh state or input.</summary>
public sealed class RefreshMotion : AvaloniaObject
{
    public static readonly AttachedProperty<bool> UseMotionSettingsProperty =
        AvaloniaProperty.RegisterAttached<RefreshMotion, RefreshVisualizer, bool>("UseMotionSettings");

    private static readonly ConditionalWeakTable<RefreshVisualizer, State> States = new();

    static RefreshMotion()
    {
        UseMotionSettingsProperty.Changed.AddClassHandler<RefreshVisualizer>((control, change) =>
            States.GetValue(control, visualizer => new State(visualizer)).Enable(change.GetNewValue<bool>()));
    }

    public static bool GetUseMotionSettings(RefreshVisualizer control)
    {
        return control.GetValue(UseMotionSettingsProperty);
    }

    public static void SetUseMotionSettings(RefreshVisualizer control, bool value)
    {
        control.SetValue(UseMotionSettingsProperty, value);
    }

    private sealed class State(RefreshVisualizer visualizer)
    {
        private readonly Dictionary<CompositionVisual, SavedAnimations> _saved = new();
        private RefreshContainer? _container;
        private Control? _content;
        private bool _enabled;
        private bool _queued;
        private CompositionVisual? _resumedSpinner;
        private bool _suppressed;

        public void Enable(bool enabled)
        {
            if (_enabled == enabled) return;
            _enabled = enabled;
            if (enabled)
            {
                visualizer.PropertyChanged += Changed;
                visualizer.Loaded += Loaded;
                visualizer.DetachedFromVisualTree += Detached;
                Queue();
            }
            else
            {
                visualizer.PropertyChanged -= Changed;
                visualizer.Loaded -= Loaded;
                visualizer.DetachedFromVisualTree -= Detached;
                StopResumedSpinner();
                Restore();
                Unhook();
            }
        }

        private void Loaded(object? sender, RoutedEventArgs args)
        {
            Queue();
        }

        private void Detached(object? sender, VisualTreeAttachmentEventArgs args)
        {
            StopResumedSpinner();
            Restore();
            Unhook();
        }

        private void Changed(object? sender, AvaloniaPropertyChangedEventArgs args)
        {
            // State notifications precede the native UpdateContent call. Reconcile after it has run.
            if (args.Property == Motion.EnabledProperty)
            {
                Apply();
                Queue();
            }
            else if (args.Property == RefreshVisualizer.ContentProperty ||
                     args.Property == RefreshVisualizer.RefreshVisualizerStateProperty ||
                     args.Property == RefreshVisualizer.OrientationProperty || args.Property == Visual.BoundsProperty ||
                     args.Property == RefreshVisualizer.TemplateProperty)
            {
                Queue();
            }
        }

        private void ContentLoaded(object? sender, RoutedEventArgs args)
        {
            Queue();
        }

        private void ContainerChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
        {
            Queue();
        }

        private void Pull(object? sender, RoutedEventArgs args)
        {
            Queue();
        }

        private void Queue()
        {
            if (_queued) return;
            _queued = true;
            Dispatcher.UIThread.Post(() =>
            {
                _queued = false;
                Apply();
            }, DispatcherPriority.Loaded);
        }

        private void Unhook()
        {
            if (_content is not null) _content.Loaded -= ContentLoaded;
            _content = null;
            if (_container is null) return;
            _container.PropertyChanged -= ContainerChanged;
            _container.RemoveHandler(InputElement.PullGestureEvent, Pull);
            _container.RemoveHandler(InputElement.PullGestureEndedEvent, Pull);
            _container = null;
        }

        private void Apply()
        {
            if (!_enabled || !visualizer.IsAttachedToVisualTree()) return;
            var content = visualizer.Content as Control;
            var container = visualizer.GetVisualAncestors().OfType<RefreshContainer>().FirstOrDefault();
            if (content != _content || container != _container)
            {
                StopResumedSpinner();
                Restore();
                Unhook();
                _content = content;
                _container = container;
                if (_content is not null) _content.Loaded += ContentLoaded;
                if (_container is not null)
                {
                    _container.PropertyChanged += ContainerChanged;
                    _container.AddHandler(InputElement.PullGestureEvent, Pull, RoutingStrategies.Bubble, true);
                    _container.AddHandler(InputElement.PullGestureEndedEvent, Pull, RoutingStrategies.Bubble, true);
                }
            }

            var glyph = _content is null ? null : ElementComposition.GetElementVisual(_content);
            if (glyph is null) return;
            var state = visualizer.GetValue(RefreshVisualizer.RefreshVisualizerStateProperty);
            var scroll = (_container?.Content as Visual)?.GetSelfAndVisualDescendants().OfType<ScrollViewer>()
                .FirstOrDefault();
            var indicator = ElementComposition.GetElementVisual(visualizer);
            var provider = scroll?.Content is Visual rows ? ElementComposition.GetElementVisual(rows) : null;
            if (state == RefreshVisualizerState.Refreshing && indicator is not null && provider is not null &&
                visualizer.RenderTransform is TranslateTransform hidden)
            {
                // Native input resets its ratio after release. A later density/content update
                // can therefore hide a still-refreshing indicator. Keep the band exposed using
                // the native hide transform, and preserve the provider's underlying offset.
                var target = indicator.Offset with { X = -hidden.X, Y = -hidden.Y };
                var delta = target - indicator.Offset;
                indicator.Offset = target;
                provider.Offset += delta;
            }

            if (Motion.GetEnabled(visualizer))
            {
                var resume = _suppressed;
                Restore();
                if (resume && state == RefreshVisualizerState.Refreshing)
                {
                    // Resume the indicator only; RequestRefresh would issue another data request.
                    var spin = glyph.Compositor.CreateScalarKeyFrameAnimation();
                    spin.Target = "RotationAngle";
                    spin.InsertKeyFrame(0, glyph.RotationAngle, new LinearEasing());
                    spin.InsertKeyFrame(1, glyph.RotationAngle + (float)(2 * Math.PI), new LinearEasing());
                    spin.Duration = TimeSpan.FromMilliseconds(500);
                    spin.IterationBehavior = AnimationIterationBehavior.Forever;
                    spin.StopBehavior = AnimationStopBehavior.LeaveCurrentValue;
                    glyph.StartAnimation("RotationAngle", spin);
                    _resumedSpinner = glyph;
                }
                else if (state != RefreshVisualizerState.Refreshing)
                {
                    StopResumedSpinner();
                }

                return;
            }

            _suppressed = true;
            StopResumedSpinner();
            Suppress(glyph, true);
            if (indicator is not null) Suppress(indicator, false);
            // The native adapter animates both the ScrollViewer and its refresh provider (content).
            if (scroll is not null)
            {
                if (ElementComposition.GetElementVisual(scroll) is { } viewport) Suppress(viewport, false);
                if (provider is not null) Suppress(provider, false);
            }
        }

        private void Suppress(CompositionVisual visual, bool glyph)
        {
            if (visual.ImplicitAnimations is { } animations &&
                (!_saved.TryGetValue(visual, out var saved) || animations != saved.Suppressed))
            {
                var remaining = visual.Compositor.CreateImplicitAnimationCollection();
                foreach (var (property, animation) in animations)
                    if (property != "Offset" && (!glyph || property is not ("RotationAngle" or "Scale" or "Opacity")))
                        remaining[property] = animation;
                var suppressed = remaining.Count == 0 ? null : remaining;
                _saved[visual] = new SavedAnimations(animations, suppressed);
                visual.ImplicitAnimations = suppressed;
            }

            // Stop interpolation, retaining native target offsets so the gesture still follows the pointer.
            SettleVector(visual, "Offset", visual.Offset);
            if (!glyph) return;
            visual.StopAnimation("RotationAngle");
            visual.StopAnimation("Scale");
            visual.StopAnimation("Opacity");
            visual.Scale = new Vector3D(1, 1, 1);
            visual.RotationAngle = visualizer.Orientation switch
            {
                RefreshVisualizerOrientation.Rotate90DegreesCounterclockwise => (float)(Math.PI / 2),
                RefreshVisualizerOrientation.Rotate270DegreesCounterclockwise => (float)(-Math.PI / 2),
                RefreshVisualizerOrientation.Normal => 0,
                _ => _container?.PullDirection switch
                {
                    PullDirection.LeftToRight => (float)(-Math.PI / 2),
                    PullDirection.RightToLeft => (float)(Math.PI / 2), _ => 0
                }
            };
            SettleVector(visual, "Scale", visual.Scale);
            SettleScalar(visual, "Opacity", visual.Opacity);
            SettleScalar(visual, "RotationAngle", visual.RotationAngle);
        }

        // Avalonia 12.1.4 StopAnimation removes the server animation but leaves a pending
        // StartAnimation in the client batch. Replace that pending entry with a constant,
        // single-tick settlement: there is no visible interpolation or continuing loop.
        private static void SettleScalar(CompositionVisual visual, string property, float value)
        {
            visual.StopAnimation(property);
            var settle = visual.Compositor.CreateScalarKeyFrameAnimation();
            settle.Target = property;
            settle.InsertKeyFrame(1, value);
            settle.Duration = TimeSpan.FromTicks(1);
            settle.StopBehavior = AnimationStopBehavior.SetToFinalValue;
            visual.StartAnimation(property, settle);
        }

        private static void SettleVector(CompositionVisual visual, string property, Vector3D value)
        {
            visual.StopAnimation(property);
            var settle = visual.Compositor.CreateVector3KeyFrameAnimation();
            settle.Target = property;
            settle.InsertKeyFrame(1, new Vector3((float)value.X, (float)value.Y, (float)value.Z));
            settle.Duration = TimeSpan.FromTicks(1);
            settle.StopBehavior = AnimationStopBehavior.SetToFinalValue;
            visual.StartAnimation(property, settle);
        }

        private void StopResumedSpinner()
        {
            if (_resumedSpinner is { } spinner) SettleScalar(spinner, "RotationAngle", spinner.RotationAngle);
            _resumedSpinner = null;
        }

        private void Restore()
        {
            foreach (var (visual, animations) in _saved)
                if (visual.ImplicitAnimations == animations.Suppressed)
                    visual.ImplicitAnimations = animations.Original;
            _saved.Clear();
            _suppressed = false;
        }

        private sealed record SavedAnimations(
            ImplicitAnimationCollection Original,
            ImplicitAnimationCollection? Suppressed);
    }
}