using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

/// <summary>Native column insertion caret with a short, cancellable settling motion.</summary>
public sealed class DataGridDropIndicator : Control
{
    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        AvaloniaProperty.Register<DataGridDropIndicator, IBrush?>(nameof(Foreground));

    private bool _arranged;
    private CancellationTokenSource? _motion;

    public DataGridDropIndicator()
    {
        IsHitTestVisible = false;
        Focusable = false;
        RenderTransform = new TranslateTransform();
    }

    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ForegroundProperty) InvalidateVisual();
        if (change.Property == Motion.EnabledProperty && !Motion.GetEnabled(this)) Cancel();
        if (change.Property != BoundsProperty) return;
        var old = change.GetOldValue<Rect>();
        var next = change.GetNewValue<Rect>();
        if (_arranged && Math.Abs(old.X - next.X) > 0.1) Settle(old.X - next.X);
        _arranged = next.Width > 0;
    }

    private async void Settle(double offset)
    {
        var current = (RenderTransform as TranslateTransform)?.X ?? 0;
        Cancel();
        if (!Motion.GetEnabled(this)) return;
        var source = _motion = new CancellationTokenSource();
        var duration = this.TryFindResource("One.Motion.Fast", out var value) && value is TimeSpan time
            ? time
            : TimeSpan.FromMilliseconds(120);
        if (duration <= TimeSpan.Zero)
        {
            Cancel();
            return;
        }

        var animation = new Animation
        {
            Duration = duration, Easing = new CubicEaseOut(),
            Children =
            {
                new KeyFrame
                    { Cue = new Cue(0), Setters = { new Setter(TranslateTransform.XProperty, current + offset) } },
                new KeyFrame { Cue = new Cue(1), Setters = { new Setter(TranslateTransform.XProperty, 0d) } }
            }
        };
        try
        {
            await animation.RunAsync(this, source.Token);
        }
        catch (OperationCanceledException) when (source.IsCancellationRequested)
        {
        }
        finally
        {
            if (ReferenceEquals(_motion, source)) _motion = null;
            source.Dispose();
        }
    }

    private void Cancel()
    {
        var source = _motion;
        _motion = null;
        source?.Cancel();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs args)
    {
        Cancel();
        _arranged = false;
        base.OnDetachedFromVisualTree(args);
    }

    public override void Render(DrawingContext context)
    {
        if (Foreground is not { } brush) return;
        context.FillRectangle(brush, new Rect(0, 3, 2, Math.Max(0, Bounds.Height - 6)), 1);
        context.FillRectangle(brush, new Rect(0, 1, 4, 3), 1);
        context.FillRectangle(brush, new Rect(0, Math.Max(1, Bounds.Height - 4), 4, 3), 1);
    }
}