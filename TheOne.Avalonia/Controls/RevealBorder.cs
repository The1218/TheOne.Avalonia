using global::Avalonia;
using global::Avalonia.Animation;
using global::Avalonia.Animation.Easings;
using global::Avalonia.Controls;
using global::Avalonia.Media;
using global::Avalonia.Styling;
using global::Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

/// <summary>Fades and translates content; keeps it laid out until the closing animation completes.</summary>
public sealed class RevealBorder : Border
{
    public static readonly StyledProperty<bool> IsOpenProperty = AvaloniaProperty.Register<RevealBorder, bool>(nameof(IsOpen));
    public static readonly StyledProperty<TimeSpan> DurationProperty = AvaloniaProperty.Register<RevealBorder, TimeSpan>(nameof(Duration), TimeSpan.FromMilliseconds(180));
    private CancellationTokenSource? _animation;
    private readonly TranslateTransform _translation = new();
    public bool IsOpen { get => GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }
    public TimeSpan Duration { get => GetValue(DurationProperty); set => SetValue(DurationProperty, value); }

    public RevealBorder()
    {
        ClipToBounds = true;
        RenderTransform = _translation;
        AttachedToVisualTree += (_, _) => { IsVisible = IsOpen; Opacity = 1; };
        DetachedFromVisualTree += (_, _) => { _animation?.Cancel(); _animation?.Dispose(); _animation = null; };
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsOpenProperty || change.Property == Motion.EnabledProperty)
            UpdateVisibility();
    }

    private async void UpdateVisibility()
    {
        var wasVisible = IsVisible;
        var previousOpacity = wasVisible ? Opacity : 0;
        var previousOffset = _translation.Y;
        _animation?.Cancel();
        _animation?.Dispose();
        _animation = null;
        var open = IsOpen;
        IsHitTestVisible = open;
        if (!this.IsAttachedToVisualTree() || !Motion.GetEnabled(this) || Duration <= TimeSpan.Zero)
        {
            IsVisible = open;
            Opacity = 1;
            _translation.Y = 0;
            return;
        }
        IsVisible = true;
        var cts = _animation = new CancellationTokenSource();
        var animation = new Animation
        {
            Duration = open ? Duration : TimeSpan.FromTicks(Duration.Ticks * 7 / 10),
            Easing = open ? new CubicEaseOut() : new CubicEaseIn(),
            Children =
            {
                new KeyFrame { Cue = new Cue(0), Setters = { new Setter(OpacityProperty, previousOpacity), new Setter(TranslateTransform.YProperty, open && !wasVisible ? -4d : previousOffset) } },
                new KeyFrame { Cue = new Cue(1), Setters = { new Setter(OpacityProperty, open ? 1d : 0d), new Setter(TranslateTransform.YProperty, open ? 0d : -4d) } }
            }
        };
        try
        {
            await animation.RunAsync(this, cts.Token);
            if (!cts.IsCancellationRequested) { IsVisible = IsOpen; Opacity = 1; _translation.Y = 0; }
        }
        catch (OperationCanceledException) { }
        finally
        {
            if (ReferenceEquals(_animation, cts)) { _animation = null; cts.Dispose(); }
        }
    }
}
