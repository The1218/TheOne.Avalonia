using System.Globalization;
using Avalonia;
using Avalonia.Media;
using TheOne.Avalonia.Workflow;

namespace TheOne.Avalonia.Controls;

public sealed partial class WorkflowCanvas
{
    private readonly Dictionary<WorkflowConnection, EdgeDrawing> _edges = [];

    private void InvalidateEdgeText()
    {
        foreach (var edge in _edges.Values) edge.Text = null;
    }

    private EdgeDrawing? CachedEdge(WorkflowConnection connection)
    {
        if (WorldAnchor(connection.SourceNodeId, connection.SourcePortId) is not { } worldFrom ||
            WorldAnchor(connection.TargetNodeId, connection.TargetPortId) is not { } worldTo) return null;
        var zoom = _owner?.Zoom ?? 1;
        var from = new Point(worldFrom.X * zoom, worldFrom.Y * zoom);
        var to = new Point(worldTo.X * zoom, worldTo.Y * zoom);
        if (!_edges.TryGetValue(connection, out var cached) || cached.From != from || cached.To != to)
        {
            var points = Route(from, to);
            var bounds = new Rect(new Point(points.Min(p => p.X), points.Min(p => p.Y)),
                new Point(points.Max(p => p.X), points.Max(p => p.Y))).Inflate(8);
            cached = new EdgeDrawing(from, to, points, CreateEdgeGeometry(points), bounds);
            _edges[connection] = cached;
        }
        return cached;
    }

    private void RenderEdges(DrawingContext context, IBrush accent)
    {
        var pan = _owner?.Pan ?? default;
        var viewport = new Rect(new Point(-pan.X, -pan.Y), Bounds.Size);
        var muted = Brush("One.Muted", Brushes.Gray);
        var textBrush = Brush("One.Text", Brushes.Black);
        var labelBackground = Brush("One.Canvas", Brushes.WhiteSmoke);
        using var transform = context.PushTransform(Matrix.CreateTranslation(pan));
        foreach (var connection in _document?.Connections ?? [])
        {
            if (CachedEdge(connection) is not { } edge) continue;
            var bounds = edge.Bounds;
            if (!string.IsNullOrEmpty(connection.Label))
            {
                if (edge.Text == null || edge.Label != connection.Label ||
                    edge.Culture != CultureInfo.CurrentUICulture.Name || !ReferenceEquals(edge.TextBrush, textBrush))
                {
                    edge.Text = new FormattedText(connection.Label, CultureInfo.CurrentUICulture,
                        FlowDirection.LeftToRight, new Typeface("Inter"), 12, textBrush);
                    edge.Label = connection.Label;
                    edge.Culture = CultureInfo.CurrentUICulture.Name;
                    edge.TextBrush = textBrush;
                    edge.LabelBounds = new Rect(
                        new Point((edge.From.X + edge.To.X) / 2 - edge.Text.Width / 2,
                            (edge.From.Y + edge.To.Y) / 2 - edge.Text.Height - 4),
                        new Size(edge.Text.Width, edge.Text.Height));
                }
                bounds = bounds.Union(edge.LabelBounds);
            }
            else
            {
                edge.Text = null;
                edge.Label = null;
                edge.TextBrush = null;
            }
            if (!bounds.Intersects(viewport)) continue;
            var selected = connection == _owner?.SelectedConnection;
            var active = _document?.FindNode(connection.TargetNodeId)?.State is
                WorkflowNodeState.Completed or WorkflowNodeState.Running;
            DrawEdge(context, edge.Points, edge.Geometry, new Pen(selected || active ? accent : muted, selected ? 2.5 : 1.5));
            if (!string.IsNullOrEmpty(connection.Label) && edge.Text != null)
            {
                context.FillRectangle(labelBackground, edge.LabelBounds);
                context.DrawText(edge.Text, edge.LabelBounds.TopLeft);
            }
        }
    }

    private sealed class EdgeDrawing(Point from, Point to, Point[] points, StreamGeometry geometry, Rect bounds)
    {
        public readonly Point From = from, To = to;
        public readonly Point[] Points = points;
        public readonly StreamGeometry Geometry = geometry;
        public readonly Rect Bounds = bounds;
        public FormattedText? Text;
        public string? Label, Culture;
        public IBrush? TextBrush;
        public Rect LabelBounds;
    }
}
