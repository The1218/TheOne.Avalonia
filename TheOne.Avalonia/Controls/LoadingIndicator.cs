using System;
using global::Avalonia;
using global::Avalonia.Animation;
using global::Avalonia.Controls;
using global::Avalonia.Media;
using global::Avalonia.Styling;
using global::Avalonia.VisualTree;
using global::Avalonia.Threading;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

/// <summary>A lightweight spinner that becomes a static status glyph when motion is disabled.</summary>
public sealed class LoadingIndicator : Control
{
    public static readonly StyledProperty<IBrush?> ForegroundProperty = TextBlock.ForegroundProperty.AddOwner<LoadingIndicator>();
    public static readonly StyledProperty<bool> IsActiveProperty = AvaloniaProperty.Register<LoadingIndicator, bool>(nameof(IsActive), true);
    public static readonly DirectProperty<LoadingIndicator, bool> IsSpinningProperty = AvaloniaProperty.RegisterDirect<LoadingIndicator, bool>(nameof(IsSpinning), control => control.IsSpinning);
    public IBrush? Foreground { get => GetValue(ForegroundProperty); set => SetValue(ForegroundProperty, value); }
    public bool IsActive { get => GetValue(IsActiveProperty); set => SetValue(IsActiveProperty, value); }
    public bool IsSpinning { get; private set => SetAndRaise(IsSpinningProperty, ref field, value); }
    private bool _queued;
    private readonly RotateTransform _rotation = new();
    private static readonly Geometry Arc = Geometry.Parse("M 8,1 A 7,7 0 1 1 1,8");
    static LoadingIndicator() => AffectsRender<LoadingIndicator>(ForegroundProperty);
    public LoadingIndicator()
    {
        RenderTransform = _rotation;
        RenderTransformOrigin = RelativePoint.Center;
        Styles.Add(new Style(selector => selector.OfType<LoadingIndicator>().Class("one-loading-active"))
        {
            Animations = { CreateAnimation() }
        });
    }
    protected override Size MeasureOverride(Size availableSize) => new(16, 16);
    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var side = Math.Min(Bounds.Width, Bounds.Height);
        if (side <= 0 || Foreground is null) return;
        using (context.PushTransform(Matrix.CreateScale(side / 16, side / 16) * Matrix.CreateTranslation((Bounds.Width - side) / 2, (Bounds.Height - side) / 2)))
            context.DrawGeometry(null, new Pen(Foreground, 1.8, lineCap: PenLineCap.Round), Arc);
    }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs args) { base.OnAttachedToVisualTree(args); UpdateAnimation(); }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs args) { Stop(); base.OnDetachedFromVisualTree(args); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsActiveProperty || change.Property == Motion.EnabledProperty || change.Property == IsVisibleProperty)
        {
            UpdateAnimation();
            // Effective visibility is propagated after the IsVisible property notification.
            if (!_queued)
            {
                _queued = true;
                Dispatcher.UIThread.Post(() => { _queued = false; UpdateAnimation(); }, DispatcherPriority.Loaded);
            }
        }
    }
    private void UpdateAnimation()
    {
        if (!this.IsAttachedToVisualTree() || !IsEffectivelyVisible || !IsActive || !Motion.GetEnabled(this)) { Stop(); return; }
        Classes.Set("one-loading-active", true);
        IsSpinning = true;
    }
    private void Stop()
    {
        // Removing the matching class disposes the animation through Avalonia's style lifecycle.
        Classes.Set("one-loading-active", false); IsSpinning = false; _rotation.Angle = 0;
    }
    private static Animation CreateAnimation() => new()
    {
        Duration = TimeSpan.FromMilliseconds(900), IterationCount = IterationCount.Infinite,
        PlaybackBehavior = PlaybackBehavior.OnlyIfVisible,
        Children =
        {
            new KeyFrame { Cue = new Cue(0), Setters = { new Setter(RotateTransform.AngleProperty, 0d) } },
            new KeyFrame { Cue = new Cue(1), Setters = { new Setter(RotateTransform.AngleProperty, 360d) } }
        }
    };
}
