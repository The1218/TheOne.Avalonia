using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Media;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

/// <summary>A non-interactive rail layer. Draws one line per adjacent visible node.</summary>
public class FlowStepConnectors : Control
{
    public static readonly StyledProperty<IBrush?> ConnectorBrushProperty = AvaloniaProperty.Register<FlowStepConnectors, IBrush?>(nameof(ConnectorBrush));
    public static readonly StyledProperty<IBrush?> CompletedConnectorBrushProperty = AvaloniaProperty.Register<FlowStepConnectors, IBrush?>(nameof(CompletedConnectorBrush));
    public static readonly StyledProperty<double> ConnectorThicknessProperty = AvaloniaProperty.Register<FlowStepConnectors, double>(nameof(ConnectorThickness), 2, validate: value => double.IsFinite(value) && value >= 0);
    internal FlowStep? Owner { get; set; }
    static FlowStepConnectors() => AffectsRender<FlowStepConnectors>(ConnectorBrushProperty, CompletedConnectorBrushProperty, ConnectorThicknessProperty);
    public IBrush? ConnectorBrush { get => GetValue(ConnectorBrushProperty); set => SetValue(ConnectorBrushProperty, value); }
    public IBrush? CompletedConnectorBrush { get => GetValue(CompletedConnectorBrushProperty); set => SetValue(CompletedConnectorBrushProperty, value); }
    public double ConnectorThickness { get => GetValue(ConnectorThicknessProperty); set => SetValue(ConnectorThicknessProperty, value); }
    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (Owner?.ItemsPanelRoot is not { } panel) return;
        FlowNode? previous = null;
        foreach (var node in panel.Children.OfType<FlowNode>().Where(node => node.IsVisible))
        {
            if (previous?.Indicator is { } start && node.Indicator is { } end &&
                start.TranslatePoint(new Point(start.Bounds.Width / 2, start.Bounds.Height / 2), this) is { } a &&
                end.TranslatePoint(new Point(end.Bounds.Width / 2, end.Bounds.Height / 2), this) is { } b)
            {
                var direction = new Vector(b.X - a.X, b.Y - a.Y); var length = direction.Length;
                if (length > 0)
                {
                    var unit = direction / length;
                    var startRadius = Math.Max(start.Bounds.Width, start.Bounds.Height) / 2 + 3;
                    var endRadius = Math.Max(end.Bounds.Width, end.Bounds.Height) / 2 + 3;
                    var brush = previous.Status == FlowNodeStatus.Completed && node.Status != FlowNodeStatus.Pending ? CompletedConnectorBrush : ConnectorBrush;
                    if (brush is not null && length > startRadius + endRadius && ConnectorThickness > 0)
                        context.DrawLine(new Pen(brush, ConnectorThickness), a + unit * startRadius, b - unit * endRadius);
                }
            }
            previous = node;
        }
    }
}
