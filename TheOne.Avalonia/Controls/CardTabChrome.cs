using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>Draws a card tab with an open content edge and small concave joining corners.</summary>
public sealed class CardTabChrome : Decorator
{
    public static readonly StyledProperty<IBrush?> BackgroundProperty = Border.BackgroundProperty.AddOwner<CardTabChrome>();
    public static readonly StyledProperty<IBrush?> BorderBrushProperty = Border.BorderBrushProperty.AddOwner<CardTabChrome>();
    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner<CardTabChrome>();
    public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<CardTabChrome, bool>(nameof(IsSelected));
    public static readonly StyledProperty<Dock> PlacementProperty = AvaloniaProperty.Register<CardTabChrome, Dock>(nameof(Placement), Dock.Top);
    public IBrush? Background { get => GetValue(BackgroundProperty); set => SetValue(BackgroundProperty, value); }
    public IBrush? BorderBrush { get => GetValue(BorderBrushProperty); set => SetValue(BorderBrushProperty, value); }
    public CornerRadius CornerRadius { get => GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }
    public bool IsSelected { get => GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }
    public Dock Placement { get => GetValue(PlacementProperty); set => SetValue(PlacementProperty, value); }
    private StreamGeometry? _outline;
    private Size _outlineSize;
    private double _outlineRadius;

    static CardTabChrome() => AffectsRender<CardTabChrome>(BackgroundProperty, BorderBrushProperty, IsSelectedProperty, PlacementProperty, CornerRadiusProperty);

    public override void Render(DrawingContext context)
    {
        if (!IsSelected)
        {
            context.DrawRectangle(Background, new Pen(BorderBrush, 1), new Rect(Bounds.Size).Deflate(0.5), 4, 4);
            return;
        }
        var size = Placement is Dock.Left or Dock.Right ? new Size(Bounds.Height, Bounds.Width) : Bounds.Size;
        if (_outline is null || _outlineSize != size || _outlineRadius != CornerRadius.TopLeft)
        {
            _outlineSize = size;
            _outlineRadius = CornerRadius.TopLeft;
            _outline = new StreamGeometry();
            using var path = _outline.Open();
            var width = size.Width;
            var bottom = size.Height;
            var radius = Math.Min(CornerRadius.TopLeft, Math.Min(width / 4, bottom / 3));
            const double foot = 4;
            path.BeginFigure(new Point(0, bottom), true);
            path.QuadraticBezierTo(new Point(foot + 0.5, bottom), new Point(foot + 0.5, bottom - foot));
            path.LineTo(new Point(foot + 0.5, radius + 0.5));
            path.QuadraticBezierTo(new Point(foot + 0.5, 0.5), new Point(foot + radius + 0.5, 0.5));
            path.LineTo(new Point(width - foot - radius - 0.5, 0.5));
            path.QuadraticBezierTo(new Point(width - foot - 0.5, 0.5), new Point(width - foot - 0.5, radius + 0.5));
            path.LineTo(new Point(width - foot - 0.5, bottom - foot));
            path.QuadraticBezierTo(new Point(width - foot - 0.5, bottom), new Point(width, bottom));
            // Fill implicitly closes the path, but its stroke has no line across the content edge.
            path.EndFigure(false);
        }
        var transform = Placement switch
        {
            Dock.Bottom => new Matrix(1, 0, 0, -1, 0, Bounds.Height),
            Dock.Left => new Matrix(0, -1, 1, 0, 0, Bounds.Height),
            Dock.Right => new Matrix(0, 1, -1, 0, Bounds.Width, 0),
            _ => Matrix.Identity
        };
        using (context.PushTransform(transform))
            context.DrawGeometry(Background, new Pen(BorderBrush, 1), _outline);
    }
}
