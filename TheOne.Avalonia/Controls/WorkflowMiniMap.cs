using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Input;
using global::Avalonia.Media;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

public sealed class WorkflowMiniMap : Control
{
    public static readonly StyledProperty<WorkflowEditor?> EditorProperty = AvaloniaProperty.Register<WorkflowMiniMap, WorkflowEditor?>(nameof(Editor));
    public WorkflowEditor? Editor { get => GetValue(EditorProperty); set => SetValue(EditorProperty, value); }
    private WorkflowEditor? _observed;
    private Rect _graph;
    private double _scale;
    private Point _offset;
    private bool _dragging;
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) { base.OnAttachedToVisualTree(e); Observe(); }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) { Stop(); base.OnDetachedFromVisualTree(e); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) { base.OnPropertyChanged(change); if (change.Property == EditorProperty) Observe(); }
    private void Stop() { if (_observed != null) { _observed.DocumentEdited -= Edited; _observed.PropertyChanged -= EditorChanged; } _observed = null; }
    private void Observe() { Stop(); if (TopLevel.GetTopLevel(this) != null) { _observed = Editor; if (_observed != null) { _observed.DocumentEdited += Edited; _observed.PropertyChanged += EditorChanged; } } InvalidateVisual(); }
    private void Edited(object? s, EventArgs e) => InvalidateVisual();
    private void EditorChanged(object? s, AvaloniaPropertyChangedEventArgs e) => InvalidateVisual();
    private IBrush Brush(string key, IBrush fallback) => this.TryFindResource(key, out var v) && v is IBrush b ? b : fallback;
    private Point Map(Point p) => new(_offset.X + (p.X - _graph.X) * _scale, _offset.Y + (p.Y - _graph.Y) * _scale);
    public override void Render(DrawingContext context)
    {
        base.Render(context); context.DrawRectangle(Brush("One.Surface", Brushes.White), new Pen(Brush("One.Line", Brushes.Gray), 1), new Rect(Bounds.Size).Deflate(1), 4, 4);
        if (Editor?.Document is not { } doc || doc.Nodes.Count == 0) return;
        _graph = Editor.GetGraphBounds().Inflate(24); _scale = Math.Min((Bounds.Width - 16) / Math.Max(1, _graph.Width), (Bounds.Height - 16) / Math.Max(1, _graph.Height));
        _offset = new Point((Bounds.Width - _graph.Width * _scale) / 2, (Bounds.Height - _graph.Height * _scale) / 2);
        var accent = Brush("One.Accent", Brushes.Teal); var muted = Brush("One.Line", Brushes.LightGray);
        foreach (var edge in doc.Connections)
        {
            var from = doc.Nodes.FirstOrDefault(n => n.Id == edge.SourceNodeId); var to = doc.Nodes.FirstOrDefault(n => n.Id == edge.TargetNodeId);
            if (from != null && to != null) context.DrawLine(new Pen(muted, 1), Map(new Point(from.X + from.Width, from.Y + 40)), Map(new Point(to.X, to.Y + 40)));
        }
        foreach (var node in doc.Nodes.Where(n => !n.IsAnnotation)) context.DrawRectangle(Brush("One.AccentSoft", Brushes.LightCyan), node == Editor.SelectedNode ? new Pen(accent, 1) : null, new Rect(Map(new Point(node.X, node.Y)), new Size(Math.Max(3, node.Width * _scale), Math.Max(3, 75 * _scale))), 1, 1);
        var viewport = new Rect(Map(new Point(-Editor.Pan.X / Editor.Zoom, -Editor.Pan.Y / Editor.Zoom)), new Size(Editor.ViewportSize.Width / Editor.Zoom * _scale, Editor.ViewportSize.Height / Editor.Zoom * _scale));
        using (context.PushClip(new Rect(Bounds.Size).Deflate(2))) context.DrawRectangle(null, new Pen(accent, 1), viewport);
    }
    private void Navigate(Point p)
    {
        if (Editor == null || _scale <= 0) return;
        var world = new Point((p.X - _offset.X) / _scale + _graph.X, (p.Y - _offset.Y) / _scale + _graph.Y);
        Editor.SetCurrentValue(WorkflowEditor.PanProperty, new Vector(Editor.ViewportSize.Width / 2 - world.X * Editor.Zoom, Editor.ViewportSize.Height / 2 - world.Y * Editor.Zoom));
    }
    protected override void OnPointerPressed(PointerPressedEventArgs e) { base.OnPointerPressed(e); if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) { _dragging = true; e.Pointer.Capture(this); Navigate(e.GetPosition(this)); e.Handled = true; } }
    protected override void OnPointerMoved(PointerEventArgs e) { base.OnPointerMoved(e); if (_dragging) Navigate(e.GetPosition(this)); }
    protected override void OnPointerReleased(PointerReleasedEventArgs e) { base.OnPointerReleased(e); _dragging = false; e.Pointer.Capture(null); e.Handled = true; }
    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e) { base.OnPointerCaptureLost(e); _dragging = false; }
}
