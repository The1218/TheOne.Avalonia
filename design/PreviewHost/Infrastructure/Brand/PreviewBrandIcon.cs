using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace TheOne.DesignPreview;

/// <summary>The PreviewHost brand mark, drawn from editable 512-unit vector geometry.</summary>
public sealed class PreviewBrandIcon : Control
{
    private static readonly IBrush Accent = new SolidColorBrush(Color.Parse("#006061"));
    private static readonly IBrush OnAccent = new SolidColorBrush(Color.Parse("#F7F8F8"));
    private static readonly IBrush AccentSoft = new SolidColorBrush(Color.Parse("#C1DCDC"));
    private static readonly Geometry One = Geometry.Parse("M208,208 L264,164 H300 V360 H248 V228 L208,258 Z");

    protected override Size MeasureOverride(Size availableSize)
    {
        return new Size(32, 32);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var side = Math.Min(Bounds.Width, Bounds.Height);
        if (side <= 0) return;
        using (context.PushTransform(Matrix.CreateScale(side / 512, side / 512)
                                     * Matrix.CreateTranslation((Bounds.Width - side) / 2, (Bounds.Height - side) / 2)))
        {
            context.DrawRectangle(Accent, null, new Rect(0, 0, 512, 512), 112, 112);
            context.DrawRectangle(OnAccent, null, new Rect(112, 104, 288, 304), 72, 72);
            context.DrawRectangle(Accent, null, new Rect(160, 152, 192, 208), 24, 24);
            context.DrawGeometry(AccentSoft, null, One);
        }
    }
}