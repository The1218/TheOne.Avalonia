using global::Avalonia;
using global::Avalonia.Layout;
using global::Avalonia.Media;

namespace TheOne.Avalonia.Controls;

public partial class RelationGraph
{
    private Size _extent;
    private double[] _depthOffsets = [];
    private bool Horizontal => Orientation == Orientation.Horizontal;

    protected override Size MeasureOverride(Size availableSize)
    {
        _host.Measure(availableSize);
        return _host.DesiredSize;
    }

    private Size MeasureTree()
    {
        if (_nodes.Count == 0) { _extent = default; return new Size(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom); }
        var depths = new double[_nodes.Max(node => node.Depth) + 1];
        foreach (var node in _nodes)
        {
            depths[node.Depth] = Math.Max(depths[node.Depth], Horizontal ? node.View.DesiredSize.Width : node.View.DesiredSize.Height);
        }
        _depthOffsets = new double[depths.Length];
        for (var i = 1; i < depths.Length; i++) _depthOffsets[i] = _depthOffsets[i - 1] + depths[i - 1] + LevelSpacing;
        for (var i = _nodes.Count - 1; i >= 0; i--)
        {
            var node = _nodes[i];
            var childSpan = node.Children.Sum(child => child.Span) + Math.Max(0, node.Children.Count - 1) * NodeSpacing;
            node.Span = Math.Max(Horizontal ? node.View.DesiredSize.Height : node.View.DesiredSize.Width, childSpan);
        }
        _nodes[0].CrossStart = 0;
        foreach (var node in _nodes)
        {
            var size = node.View.DesiredSize;
            var primary = _depthOffsets[node.Depth];
            var cross = node.CrossStart + (node.Span - (Horizontal ? size.Height : size.Width)) / 2;
            node.Layout = Horizontal ? new Rect(primary, cross, size.Width, size.Height) : new Rect(cross, primary, size.Width, size.Height);
            var childSpan = node.Children.Sum(child => child.Span) + Math.Max(0, node.Children.Count - 1) * NodeSpacing;
            var start = node.CrossStart + (node.Span - childSpan) / 2;
            foreach (var child in node.Children) { child.CrossStart = start; start += child.Span + NodeSpacing; }
        }
        var length = _depthOffsets[^1] + depths[^1];
        _extent = Horizontal ? new Size(length, _nodes[0].Span) : new Size(_nodes[0].Span, length);
        return new Size(_extent.Width + Padding.Left + Padding.Right, _extent.Height + Padding.Top + Padding.Bottom);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _host.Arrange(new Rect(finalSize));
        return finalSize;
    }

    private Size ArrangeTree(Size finalSize)
    {
        var x = Padding.Left + Math.Max(0, finalSize.Width - Padding.Left - Padding.Right - _extent.Width) / 2;
        var y = Padding.Top + Math.Max(0, finalSize.Height - Padding.Top - Padding.Bottom - _extent.Height) / 2;
        foreach (var node in _nodes) node.View.Arrange(node.Layout.Translate(new Vector(x, y)));
        InvalidateVisual();
        return finalSize;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (Background is not null) context.DrawRectangle(Background, null, new Rect(Bounds.Size));
        if (ConnectorBrush is null || ConnectorThickness <= 0) return;
        var pen = new Pen(ConnectorBrush, ConnectorThickness, lineCap: PenLineCap.Round, lineJoin: PenLineJoin.Round);
        foreach (var node in _nodes)
        {
            if (node.Parent is not { } parent) continue;
            var p = parent.View.Bounds; var c = node.View.Bounds;
            var a = Horizontal ? new Point(p.Right, p.Center.Y) : new Point(p.Center.X, p.Bottom);
            var d = Horizontal ? new Point(c.Left, c.Center.Y) : new Point(c.Center.X, c.Top);
            // Use the common inter-level gap even when one parent's content is taller.
            var trunk = Horizontal ? d.X - LevelSpacing / 2 : d.Y - LevelSpacing / 2;
            var b = Horizontal ? new Point(trunk, a.Y) : new Point(a.X, trunk);
            var end = Horizontal ? new Point(trunk, d.Y) : new Point(d.X, trunk);
            var geometry = new StreamGeometry();
            using (var path = geometry.Open())
            {
                path.BeginFigure(a, false);
                Corner(path, a, b, end);
                Corner(path, b, end, d);
                path.LineTo(d); path.EndFigure(false);
            }
            context.DrawGeometry(null, pen, geometry);
            if (PortRadius > 0)
            {
                context.DrawEllipse(ConnectorBrush, null, a, PortRadius, PortRadius);
                context.DrawEllipse(ConnectorBrush, null, d, PortRadius, PortRadius);
            }
        }
    }

    private void Corner(StreamGeometryContext path, Point before, Point corner, Point after)
    {
        var incoming = new Vector(corner.X - before.X, corner.Y - before.Y);
        var outgoing = new Vector(after.X - corner.X, after.Y - corner.Y);
        var radius = Math.Min(ConnectorCornerRadius, Math.Min(incoming.Length, outgoing.Length) / 2);
        if (radius <= 0) { path.LineTo(corner); return; }
        path.LineTo(corner - incoming / incoming.Length * radius);
        path.QuadraticBezierTo(corner, corner + outgoing / outgoing.Length * radius);
    }
}
