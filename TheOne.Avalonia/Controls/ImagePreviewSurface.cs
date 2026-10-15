using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Input;
using global::Avalonia.Media;
using global::Avalonia.Media.Imaging;

namespace TheOne.Avalonia.Controls;

/// <summary>Clipped, directly drawn image viewport used by the ImagePreview template.</summary>
public sealed class ImagePreviewSurface : Control
{
    public static readonly StyledProperty<IBrush?> CheckerBrushProperty = AvaloniaProperty.Register<ImagePreviewSurface, IBrush?>(nameof(CheckerBrush));
    public static readonly StyledProperty<IBrush?> AlternateCheckerBrushProperty = AvaloniaProperty.Register<ImagePreviewSurface, IBrush?>(nameof(AlternateCheckerBrush));
    public IBrush? CheckerBrush { get => GetValue(CheckerBrushProperty); set => SetValue(CheckerBrushProperty, value); }
    public IBrush? AlternateCheckerBrush { get => GetValue(AlternateCheckerBrushProperty); set => SetValue(AlternateCheckerBrushProperty, value); }
    internal ImagePreview? Owner { get; set; }
    static ImagePreviewSurface() => AffectsRender<ImagePreviewSurface>(CheckerBrushProperty, AlternateCheckerBrushProperty);
    private Vector _pan;
    private Point? _lastPoint;
    private IPointer? _pointer;
    private static readonly Cursor PanCursor = new(StandardCursorType.SizeAll);
    internal Point Center => new(Bounds.Width / 2, Bounds.Height / 2);
    private double Scaling => TopLevel.GetTopLevel(this)?.RenderScaling ?? 1;
    private Size ImageSize => Owner?.CurrentImage is Bitmap bitmap ? bitmap.PixelSize.ToSize(1) : Owner?.CurrentImage?.Size ?? default;
    internal Rect ImageRect
    {
        get
        {
            var size = ImageSize * ((Owner?.ZoomFactor ?? 1) / Scaling);
            return new Rect(Center.X - size.Width / 2 + _pan.X, Center.Y - size.Height / 2 + _pan.Y, size.Width, size.Height);
        }
    }
    public ImagePreviewSurface()
    {
        ClipToBounds = true;
        DoubleTapped += (_, e) =>
        {
            if (Owner is not { HasImage: true } owner || !IsEffectivelyEnabled) return;
            CancelPan();
            if (owner.IsFitToWindow) owner.ActualSize(); else owner.FitToWindow();
            e.Handled = true;
        };
    }
    protected override Size ArrangeOverride(Size finalSize)
    {
        var result = base.ArrangeOverride(finalSize);
        // Bounds is current when LayoutUpdated is delivered; arrange's finalSize is available now.
        UpdateFit(finalSize); ConstrainPan(finalSize); return result;
    }
    internal void UpdateFit() => UpdateFit(Bounds.Size);
    private void UpdateFit(Size viewport)
    {
        if (Owner is not { IsFitToWindow: true, HasImage: true } owner || viewport.Width <= 0 || viewport.Height <= 0) return;
        var size = ImageSize;
        if (size.Width <= 0 || size.Height <= 0) return;
        owner.SetFitZoom(Math.Min(viewport.Width / size.Width, viewport.Height / size.Height) * Scaling);
    }
    internal void ResetPan() { CancelPan(); _pan = default; InvalidateVisual(); }
    internal void ZoomAround(Point point, double oldFactor, double newFactor)
    {
        var ratio = newFactor / oldFactor;
        _pan = new Vector((point.X - Center.X) * (1 - ratio) + _pan.X * ratio, (point.Y - Center.Y) * (1 - ratio) + _pan.Y * ratio);
        ConstrainPan();
    }
    internal void ConstrainPan() => ConstrainPan(Bounds.Size);
    private void ConstrainPan(Size viewport)
    {
        var size = ImageSize * ((Owner?.ZoomFactor ?? 1) / Scaling);
        var x = Math.Max(0, (size.Width - viewport.Width) / 2);
        var y = Math.Max(0, (size.Height - viewport.Height) / 2);
        _pan = new Vector(Math.Clamp(_pan.X, -x, x), Math.Clamp(_pan.Y, -y, y));
        InvalidateVisual();
    }
    internal void CancelPan() { _lastPoint = null; _pointer?.Capture(null); _pointer = null; Cursor = null; }
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (Owner is not { HasImage: true } owner || !IsEffectivelyEnabled || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        owner.Focus(NavigationMethod.Pointer); _lastPoint = e.GetPosition(this); _pointer = e.Pointer;
        e.Pointer.Capture(this); Cursor = PanCursor; e.Handled = true;
    }
    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (_lastPoint is not { } last || !IsEffectivelyEnabled) return;
        var next = e.GetPosition(this); _pan += next - last; _lastPoint = next; ConstrainPan(); e.Handled = true;
    }
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_lastPoint is null) return; CancelPan(); e.Handled = true;
    }
    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e); _lastPoint = null; _pointer = null; Cursor = null;
    }
    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        if (Owner is not { HasImage: true } owner || !IsEffectivelyEnabled || e.Delta.Y == 0) return;
        owner.ZoomAt(owner.ZoomFactor * Math.Pow(1.2, e.Delta.Y), e.GetPosition(this)); e.Handled = true;
    }
    public override void Render(DrawingContext context)
    {
        base.Render(context);
        // Transparent fill participates in hit testing in letterboxed areas too.
        context.FillRectangle(Brushes.Transparent, new Rect(Bounds.Size));
        if (Owner?.CurrentImage is not { } image) return;
        var rect = ImageRect; var visible = rect.Intersect(new Rect(Bounds.Size));
        if (visible.Width <= 0 || visible.Height <= 0) return;
        using (context.PushClip(visible))
        {
            context.FillRectangle(CheckerBrush ?? Brushes.White, visible);
            const int cell = 16;
            for (var y = (int)Math.Floor(visible.Y / cell); y < Math.Ceiling(visible.Bottom / cell); y++)
            for (var x = (int)Math.Floor(visible.X / cell); x < Math.Ceiling(visible.Right / cell); x++)
                if ((x + y) % 2 == 0) context.FillRectangle(AlternateCheckerBrush ?? Brushes.LightGray, new Rect(x * cell, y * cell, cell, cell));
            context.DrawImage(image, new Rect(image.Size), rect);
        }
    }
}
