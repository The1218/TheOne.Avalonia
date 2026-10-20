using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Layout;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>A themed vector glyph with a stable enum identity and no external font or icon dependency.</summary>
public sealed class OneIcon : Control
{
    public static readonly StyledProperty<OneIconKind> KindProperty =
        AvaloniaProperty.Register<OneIcon, OneIconKind>(nameof(Kind), OneIconKind.Add, validate: value => Enum.IsDefined(value));
    public static readonly StyledProperty<OneIconVariant> VariantProperty =
        AvaloniaProperty.Register<OneIcon, OneIconVariant>(nameof(Variant), OneIconVariant.Outline, validate: value => Enum.IsDefined(value));
    public static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<OneIcon, double>(nameof(IconSize), 20, validate: value => double.IsFinite(value) && value > 0);
    public static readonly StyledProperty<double> StrokeThicknessProperty =
        AvaloniaProperty.Register<OneIcon, double>(nameof(StrokeThickness), 1.75,
            validate: value => double.IsFinite(value) && value is > 0 and <= 3);
    public static readonly StyledProperty<IBrush?> ForegroundProperty = TextElement.ForegroundProperty.AddOwner<OneIcon>();

    private Pen? _pen;

    static OneIcon()
    {
        AffectsRender<OneIcon>(KindProperty, VariantProperty, ForegroundProperty, StrokeThicknessProperty);
        AffectsMeasure<OneIcon>(IconSizeProperty);
        // Decorative glyphs must not intercept their host button/menu or enter the tab order.
        IsHitTestVisibleProperty.OverrideDefaultValue<OneIcon>(false);
        FocusableProperty.OverrideDefaultValue<OneIcon>(false);
        HorizontalAlignmentProperty.OverrideDefaultValue<OneIcon>(HorizontalAlignment.Center);
        VerticalAlignmentProperty.OverrideDefaultValue<OneIcon>(VerticalAlignment.Center);
    }

    /// <summary>The glyph; supports styles and compiled bindings.</summary>
    public OneIconKind Kind { get => GetValue(KindProperty); set => SetValue(KindProperty, value); }
    /// <summary>Outline by default. Unsupported filled glyphs fall back to their outline.</summary>
    public OneIconVariant Variant { get => GetValue(VariantProperty); set => SetValue(VariantProperty, value); }
    /// <summary>Desired square size in DIPs; theme densities use 16/20/24. Explicit layout bounds remain supported.</summary>
    public double IconSize { get => GetValue(IconSizeProperty); set => SetValue(IconSizeProperty, value); }
    /// <summary>Outline width in the 24-unit design grid, 0–3 exclusive of zero; scales with the glyph.</summary>
    public double StrokeThickness { get => GetValue(StrokeThicknessProperty); set => SetValue(StrokeThicknessProperty, value); }
    /// <summary>Inherits the host text foreground, including theme and interaction changes. Null draws nothing.</summary>
    public IBrush? Foreground { get => GetValue(ForegroundProperty); set => SetValue(ForegroundProperty, value); }

    protected override Size MeasureOverride(Size availableSize) => new(IconSize, IconSize);

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var brush = Foreground;
        var size = Math.Min(Bounds.Width, Bounds.Height);
        if (brush is null || !double.IsFinite(size) || size <= 0) return;
        var glyph = OneIconCatalog.Drawing(Kind);
        var scale = size / 24;
        using var transform = context.PushTransform(new Matrix(scale, 0, 0, scale,
            (Bounds.Width - size) / 2, (Bounds.Height - size) / 2));
        if (Variant == OneIconVariant.Filled && glyph.Filled is { } filled)
            context.DrawGeometry(brush, null, filled);
        else
        {
            if (_pen is null || !ReferenceEquals(_pen.Brush, brush) || _pen.Thickness != StrokeThickness)
                _pen = new Pen(brush, StrokeThickness, lineCap: PenLineCap.Round, lineJoin: PenLineJoin.Round);
            context.DrawGeometry(null, _pen, glyph.Outline);
        }
    }
}
