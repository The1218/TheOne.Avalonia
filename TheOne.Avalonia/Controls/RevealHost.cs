using global::Avalonia;
using global::Avalonia.Animation;
using global::Avalonia.Animation.Easings;
using global::Avalonia.Controls;
using global::Avalonia.Styling;
using global::Avalonia.Threading;
using System.Diagnostics;
using global::Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

/// <summary>Reveals a fully laid-out child through a smoothly sized, clipped viewport.</summary>
public class RevealHost : Decorator
{
    public static readonly StyledProperty<bool> IsOpenProperty = AvaloniaProperty.Register<RevealHost, bool>(nameof(IsOpen));
    public static readonly StyledProperty<ExpandDirection> DirectionProperty = AvaloniaProperty.Register<RevealHost, ExpandDirection>(nameof(Direction), ExpandDirection.Down);
    public static readonly StyledProperty<TimeSpan> DurationProperty = AvaloniaProperty.Register<RevealHost, TimeSpan>(nameof(Duration), TimeSpan.FromMilliseconds(200));
    public static readonly StyledProperty<bool> ConstrainContentProperty = AvaloniaProperty.Register<RevealHost, bool>(nameof(ConstrainContent));
    /// <summary>Optional visible-distance cap during animation; the settled layout retains its full extent.</summary>
    public static readonly StyledProperty<double> TransitionExtentProperty = AvaloniaProperty.Register<RevealHost, double>(nameof(TransitionExtent), double.PositiveInfinity, validate: value => value > 0 && !double.IsNaN(value));
    public double TransitionExtent { get => GetValue(TransitionExtentProperty); set => SetValue(TransitionExtentProperty, value); }
    private static readonly StyledProperty<double> ProgressProperty = AvaloniaProperty.Register<RevealHost, double>("Progress", 0, coerce: (_, value) => Math.Clamp(value, 0, 1));
    private CancellationTokenSource? _animation;
    private DispatcherTimer? _viewportTimer;
    private Size _contentSize;
    public bool IsOpen { get => GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }
    public ExpandDirection Direction { get => GetValue(DirectionProperty); set => SetValue(DirectionProperty, value); }
    public TimeSpan Duration { get => GetValue(DurationProperty); set => SetValue(DurationProperty, value); }
    /// <summary>Preserve a finite parent viewport for content such as a virtualized list.</summary>
    public bool ConstrainContent { get => GetValue(ConstrainContentProperty); set => SetValue(ConstrainContentProperty, value); }
    private bool IsHorizontal => Direction is ExpandDirection.Left or ExpandDirection.Right;

    public RevealHost()
    {
        ClipToBounds = true;
        AttachedToVisualTree += (_, _) => SnapToState();
        DetachedFromVisualTree += (_, _) => CancelAnimation();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ProgressProperty || change.Property == DirectionProperty || change.Property == ConstrainContentProperty || change.Property == TransitionExtentProperty)
            InvalidateMeasure();
        else if (change.Property == IsOpenProperty)
            UpdateVisibility();
        else if ((change.Property == Motion.EnabledProperty && !Motion.GetEnabled(this)) || (change.Property == DurationProperty && Duration <= TimeSpan.Zero))
            SnapToState();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Child is null) return default;
        // The child's constraint stays constant while the viewport shrinks. Avalonia can
        // reuse its measure result rather than reflowing text on every animation frame.
        Child.Measure(IsHorizontal || ConstrainContent ? availableSize : new Size(availableSize.Width, double.PositiveInfinity));
        _contentSize = Child.DesiredSize;
        var progress = GetValue(ProgressProperty);
        // The final keyframe must restore the native extent during that same measure,
        // even if RunAsync's completion continuation has not resumed yet.
        var extent = _animation is null || IsOpen && progress >= 1d ? double.PositiveInfinity : TransitionExtent;
        return IsHorizontal ? new Size(Math.Min(_contentSize.Width, extent) * progress, _contentSize.Height) : new Size(_contentSize.Width, Math.Min(_contentSize.Height, extent) * progress);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var fullSize = IsHorizontal ? new Size(Math.Max(finalSize.Width, _contentSize.Width), finalSize.Height) : new Size(finalSize.Width, Math.Max(finalSize.Height, _contentSize.Height));
        // Keep the content anchored to its header in all four expansion directions.
        var origin = Direction switch
        {
            ExpandDirection.Up => new Point(0, finalSize.Height - fullSize.Height),
            ExpandDirection.Left => new Point(finalSize.Width - fullSize.Width, 0),
            _ => default
        };
        Child?.Arrange(new Rect(origin, fullSize));
        return finalSize;
    }

    private void CancelAnimation()
    {
        _viewportTimer?.Stop();
        _viewportTimer = null;
        var previous = _animation;
        _animation = null;
        previous?.Cancel();
        previous?.Dispose();
    }

    // Explicit navigation must expose its target immediately before scrolling/focus.
    internal void CompleteTransition() => SnapToState();

    private void SnapToState()
    {
        CancelAnimation();
        SetValue(ProgressProperty, IsOpen ? 1d : 0d);
        IsVisible = IsOpen;
        IsHitTestVisible = IsOpen;
    }

    private async void UpdateVisibility()
    {
        if (!this.IsAttachedToVisualTree() || !Motion.GetEnabled(this) || Duration <= TimeSpan.Zero)
        {
            SnapToState();
            return;
        }
        var start = GetValue(ProgressProperty);
        var end = IsOpen ? 1d : 0d;
        CancelAnimation();
        IsHitTestVisible = IsOpen;
        // Set the underlying value before applying keyframes, so completion cannot
        // briefly restore the previous state when animation subscriptions are removed.
        SetValue(ProgressProperty, end);
        if (Math.Abs(start - end) < 0.001) { IsVisible = IsOpen; return; }
        IsVisible = true;
        var cts = _animation = new CancellationTokenSource();
        var duration = TimeSpan.FromTicks(Math.Max(1, (long)(Duration.Ticks * (IsOpen ? 1 : 0.7) * Math.Abs(end - start))));
        if (double.IsFinite(TransitionExtent))
        {
            // Layout rounding can make consecutive viewport frames identical. A UI
            // timer guarantees the final extent even when the render clock goes idle.
            SetValue(ProgressProperty, start);
            var watch = Stopwatch.StartNew();
            var timer = _viewportTimer = new DispatcherTimer(DispatcherPriority.Render) { Interval = TimeSpan.FromMilliseconds(16) };
            var easing = new CubicEaseOut();
            timer.Tick += (_, _) =>
            {
                if (!ReferenceEquals(_animation, cts)) { timer.Stop(); return; }
                var fraction = Math.Clamp(watch.Elapsed.TotalMilliseconds / duration.TotalMilliseconds, 0, 1);
                if (fraction >= 1) { SnapToState(); return; }
                SetValue(ProgressProperty, start + (end - start) * easing.Ease(fraction));
            };
            timer.Start();
            return;
        }
        var animation = new Animation
        {
            Duration = duration,
            Easing = new CubicEaseOut(),
            FillMode = FillMode.Both,
            Children =
            {
                new KeyFrame { Cue = new Cue(0), Setters = { new Setter(ProgressProperty, start) } },
                new KeyFrame { Cue = new Cue(1), Setters = { new Setter(ProgressProperty, end) } }
            }
        };
        try
        {
            await animation.RunAsync(this, cts.Token);
            if (ReferenceEquals(_animation, cts) && !cts.IsCancellationRequested)
                IsVisible = IsOpen;
        }
        finally
        {
            if (ReferenceEquals(_animation, cts)) { _animation = null; cts.Dispose(); InvalidateMeasure(); }
        }
    }
}
