using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>Small, consistent stroke icons for the report editor command bar.</summary>
internal sealed class ReportIcon : Control
{
    public static readonly StyledProperty<string> KindProperty = AvaloniaProperty.Register<ReportIcon, string>(nameof(Kind), "Pointer");
    public static readonly StyledProperty<IBrush?> ForegroundProperty = global::Avalonia.Controls.Documents.TextElement.ForegroundProperty.AddOwner<ReportIcon>();
    public string Kind { get => GetValue(KindProperty); set => SetValue(KindProperty, value); }
    public IBrush? Foreground { get => GetValue(ForegroundProperty); set => SetValue(ForegroundProperty, value); }
    static ReportIcon() => AffectsRender<ReportIcon>(KindProperty, ForegroundProperty);
    public ReportIcon() { Width = 20; Height = 20; IsHitTestVisible = false; }
    private static readonly IReadOnlyDictionary<string, Geometry> Icons = new Dictionary<string, string>
    {
        ["Pointer"] = "M5,3 L19,13 L12,14 L9,21 Z M12,14 L16,20",
        ["Undo"] = "M8,4 L3,9 L8,14 M3,9 H14 C24,9 24,21 13,21 H8",
        ["Redo"] = "M16,4 L21,9 L16,14 M21,9 H10 C0,9 0,21 11,21 H16",
        ["Text"] = "M5,21 L12,3 L19,21 M8,14 H16",
        ["Bold"] = "M7,3 V21 M7,3 H13 C21,3 21,12 13,12 H7 M13,12 C23,12 23,21 13,21 H7",
        ["Italic"] = "M10,3 H19 M5,21 H14 M15,3 L9,21",
        ["Underline"] = "M6,3 V12 C6,20 18,20 18,12 V3 M4,23 H20",
        ["Table"] = "M3,3 H21 V21 H3 Z M3,9 H21 M3,15 H21 M9,3 V21 M15,3 V21",
        ["Image"] = "M3,3 H21 V21 H3 Z M3,17 L9,11 L13,15 L16,12 L21,17 M16,7 A1,1 0 1 0 16,9 A1,1 0 1 0 16,7",
        ["Line"] = "M3,19 L21,5",
        ["Rectangle"] = "M3,5 H21 V19 H3 Z",
        ["PageNumber"] = "M6,2 H15 L20,7 V22 H6 Z M15,2 V7 H20 M10,13 H17 M10,17 H17",
        ["AlignLeft"] = "M3,4 H21 M3,9 H15 M3,14 H21 M3,19 H15",
        ["AlignCenter"] = "M3,4 H21 M7,9 H17 M3,14 H21 M7,19 H17",
        ["AlignRight"] = "M3,4 H21 M9,9 H21 M3,14 H21 M9,19 H21",
        ["Justify"] = "M3,4 H21 M3,9 H21 M3,14 H21 M3,19 H21",
        ["Border"] = "M3,3 H21 V21 H3 Z M3,12 H21 M12,3 V21",
        ["Fill"] = "M8,3 L18,13 L10,21 L2,13 Z M2,13 H18 M20,15 C15,22 25,22 20,15 M8,3 L5,0",
        ["List"] = "M8,5 H21 M8,12 H21 M8,19 H21 M3,5 H4 M3,12 H4 M3,19 H4",
        ["Check"] = "M4,12 L9,17 L20,6",
        ["CheckBox"] = "M4,5 H20 V21 H4 Z M4,9 H20 M8,2 V6 M16,2 V6 M7,14 L11,18 L17,11",
        ["CheckBoxElement"] = "M3,3 H21 V21 H3 Z M6,12 L10,16 L18,7",
        ["Barcode"] = "M3,4 V20 M6,4 V20 M10,4 V20 M12,4 V20 M16,4 V20 M19,4 V20 M21,4 V20",
        ["Settings"] = "M4,6 H20 M4,12 H20 M4,18 H20 M9,3 V9 M16,9 V15 M9,15 V21",
        ["Preview"] = "M1,12 Q12,-3 23,12 Q12,27 1,12 Z M8,12 A4,4 0 1 0 16,12 A4,4 0 1 0 8,12",
        ["Export"] = "M4,15 V21 H20 V15 M12,3 V16 M7,11 L12,16 L17,11",
        ["Save"] = "M3,3 H18 L21,6 V21 H3 Z M7,3 V10 H17 V3 M7,21 V14 H17 V21",
        ["Open"] = "M2,7 V4 H9 L12,7 H22 L19,21 H2 Z M2,12 H20",
        ["Chevron"] = "M6,9 L12,15 L18,9",
        ["Disclosure"] = "M7,4 L17,12 L7,20 Z",
        ["Close"] = "M6,6 L18,18 M6,18 L18,6",
        ["Plus"] = "M12,4 V20 M4,12 H20",
        ["Minus"] = "M4,12 H20",
        ["Fit"] = "M9,3 H3 V9 M15,3 H21 V9 M3,15 V21 H9 M21,15 V21 H15",
        ["Search"] = "M17,10 A7,7 0 1 0 3,10 A7,7 0 1 0 17,10 M15,15 L22,22",
        ["Folder"] = "M2,6 H10 L13,9 H22 V21 H2 Z M2,6 V3 H9 L12,6 H20 V9",
        ["Info"] = "M12,2 A10,10 0 1 0 12,22 A10,10 0 1 0 12,2 M12,10 V17 M12,6 V7",
        ["Logo"] = "M4,3 H20 V21 H4 Z M8,17 V7 H11 L13,17 M15,7 H17 L15,17"
    }.ToDictionary(p => p.Key, p => Geometry.Parse(p.Value), StringComparer.Ordinal);
    public override void Render(DrawingContext context)
    {
        base.Render(context); if (!Icons.TryGetValue(Kind, out var geometry)) return;
        var brush = Foreground ?? Brushes.Black;
        using (context.PushTransform(Matrix.CreateScale(Bounds.Width / 24, Bounds.Height / 24)))
        {
            context.DrawGeometry(Kind == "Disclosure" ? brush : null, Kind == "Disclosure" ? null : new Pen(brush, Kind == "Bold" ? 2.6 : 1.7, lineCap: PenLineCap.Round, lineJoin: PenLineJoin.Round), geometry);
            if (Kind == "Fill")
            {
                var accent = this.TryFindResource("One.Accent", ActualThemeVariant, out var value) && value is IBrush color ? color : brush;
                context.DrawLine(new Pen(accent, 1.8), new Point(3, 24), new Point(22, 24));
            }
        }
    }
}
