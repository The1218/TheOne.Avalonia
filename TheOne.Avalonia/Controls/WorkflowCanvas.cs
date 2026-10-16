using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Presenters;
using global::Avalonia.Controls.Templates;
using global::Avalonia.Input;
using global::Avalonia.Interactivity;
using global::Avalonia.Layout;
using global::Avalonia.Markup.Xaml.MarkupExtensions;
using global::Avalonia.Media;
using global::Avalonia.VisualTree;
using TheOne.Avalonia.Workflow;

namespace TheOne.Avalonia.Controls;

/// <summary>WorkflowEditor template part. Graph coordinates are independent of zoom and pan.</summary>
public sealed class WorkflowCanvas : Control
{
    private readonly NodeHost _host;
    private global::Avalonia.Controls.Controls Children => _host.Children;
    private WorkflowEditor? _owner;
    private readonly Dictionary<WorkflowNode, NodeView> _views = [];
    private readonly Dictionary<WorkflowNode, Point> _preview = [];
    private Point _press, _pointer;
    private Vector _oldPan;
    private string? _gesture;
    private Dictionary<WorkflowNode, Point>? _origins;
    private PortHandle? _connecting;
    private string? _connectionError;
    private IPointer? _captured;
    private bool _space;
    public WorkflowCanvas()
    {
        ResourcesChanged += (_, _) => { UpdateSelectionShadow(); InvalidateVisual(); };
        _host = new NodeHost(this); ClipToBounds = true; Focusable = true; VisualChildren.Add(_host); LogicalChildren.Add(_host);
        AddHandler(PointerPressedEvent, (_, e) =>
        {
            if (Ancestors(e.Source).OfType<Button>().Any(b => b.Tag is PortHandle) || PortAt(e.GetPosition(this)) != null) OnPointerPressed(e);
        }, RoutingStrategies.Tunnel);
    }
    internal void SetOwner(WorkflowEditor? owner) { CancelGesture(); _owner = owner; Refresh(true); }
    internal void Refresh(bool rebuild = false)
    {
        if (rebuild) { Children.Clear(); _views.Clear(); }
        var nodes = _owner?.Document?.Nodes.ToArray() ?? [];
        foreach (var n in _views.Keys.Where(n => !nodes.Contains(n)).ToArray()) { Children.Remove(_views[n].Border); _views.Remove(n); }
        foreach (var n in nodes)
        {
            if (!_views.TryGetValue(n, out var view)) { view = new NodeView(this, n); _views.Add(n, view); Children.Add(view.Border); }
            view.Update(_owner!);
        }
        UpdateSelection(); InvalidateMeasure(); InvalidateVisual();
    }
    internal void UpdateSelection()
    {
        foreach (var (node, view) in _views)
        {
            var selected = _owner?.SelectedNodes.Contains(node) == true;
            view.Border.Bind(Border.BorderBrushProperty, new DynamicResourceExtension(selected ? "One.Accent" : "One.Line"));
            view.Border.BorderThickness = new Thickness(1);
        }
        UpdateSelectionShadow();
        InvalidateVisual();
    }
    private void UpdateSelectionShadow()
    {
        var color = Brush("One.Accent", Brushes.Teal) is ISolidColorBrush solid ? solid.Color : Colors.Teal;
        foreach (var (node, view) in _views)
            view.Border.BoxShadow = _owner?.SelectedNodes.Contains(node) == true
                ? new BoxShadows(new BoxShadow { Color = Color.FromArgb(60, color.R, color.G, color.B), Blur = 8, Spread = 2 }) : default;
    }
    public Point ToWorld(Point point) => _owner is { } o ? new Point((point.X - o.Pan.X) / o.Zoom, (point.Y - o.Pan.Y) / o.Zoom) : point;
    private Point ToScreen(Point point) => _owner is { } o ? new Point(point.X * o.Zoom + o.Pan.X, point.Y * o.Zoom + o.Pan.Y) : point;
    internal Rect GraphBounds => _views.Count == 0 ? new Rect(0, 0, 180, 100) : _views.Select(v => new Rect(new Point(v.Key.X, v.Key.Y), v.Value.Border.DesiredSize)).Aggregate((a, b) => a.Union(b));
    protected override Size MeasureOverride(Size availableSize)
    {
        _host.Measure(availableSize);
        return new Size(double.IsFinite(availableSize.Width) ? availableSize.Width : 800, double.IsFinite(availableSize.Height) ? availableSize.Height : 500);
    }
    protected override Size ArrangeOverride(Size finalSize)
    {
        _host.Arrange(new Rect(finalSize));
        ArrangeNodes();
        return finalSize;
    }
    private void ArrangeNodes()
    {
        foreach (var (n, view) in _views)
        {
            var point = ToScreen(_preview.GetValueOrDefault(n, new Point(n.X, n.Y)));
            view.Border.RenderTransformOrigin = RelativePoint.TopLeft;
            view.Border.RenderTransform = new ScaleTransform(_owner?.Zoom ?? 1, _owner?.Zoom ?? 1);
            view.Border.Arrange(new Rect(point, view.Border.DesiredSize));
        }
        InvalidateVisual();
    }
    private IBrush Brush(string key, IBrush fallback) => this.TryFindResource(key, ActualThemeVariant, out var value) && value is IBrush brush ? brush : fallback;
    private Point? Anchor(string nodeId, string portId)
    {
        var view = _views.FirstOrDefault(v => v.Key.Id == nodeId).Value;
        if (view == null || !view.Ports.TryGetValue(portId, out var button)) return null;
        var port = view.Node.Ports.First(p => p.Id == portId);
        var row = button.TranslatePoint(new Point(0, button.Bounds.Height / 2), this);
        var edge = view.Border.TranslatePoint(new Point(port.Direction == WorkflowPortDirection.Input ? 0 : view.Border.Bounds.Width, 0), this);
        return row is { } r && edge is { } a ? new Point(a.X, r.Y) : null;
    }
    private static Point[] Route(Point from, Point to)
    {
        var middle = from.X < to.X - 30 ? (from.X + to.X) / 2 : Math.Max(from.X, to.X) + 40;
        return [from, new(middle, from.Y), new(middle, to.Y), to];
    }
    private static void DrawEdge(DrawingContext context, Point[] points, Pen pen)
    {
        var geometry = new StreamGeometry();
        using (var path = geometry.Open())
        {
            path.BeginFigure(points[0], false);
            for (var i = 1; i < points.Length - 1; i++)
            {
                var p = points[i]; var before = points[i - 1]; var after = points[i + 1];
                var firstLength = Math.Abs(p.X - before.X) + Math.Abs(p.Y - before.Y);
                var nextLength = Math.Abs(p.X - after.X) + Math.Abs(p.Y - after.Y);
                var radius = Math.Min(6, Math.Min(firstLength, nextLength) / 2);
                if (radius == 0) { path.LineTo(p); continue; }
                path.LineTo(new Point(p.X + (before.X - p.X) * radius / firstLength, p.Y + (before.Y - p.Y) * radius / firstLength));
                path.QuadraticBezierTo(p, new Point(p.X + (after.X - p.X) * radius / nextLength, p.Y + (after.Y - p.Y) * radius / nextLength));
            }
            path.LineTo(points[^1]); path.EndFigure(false);
        }
        context.DrawGeometry(null, pen, geometry);
        var end = points[^1]; var previous = points[^2]; var sign = end.X >= previous.X ? 1 : -1;
        context.DrawLine(pen, end, new Point(end.X - 6 * sign, end.Y - 4)); context.DrawLine(pen, end, new Point(end.X - 6 * sign, end.Y + 4));
    }
    public override void Render(DrawingContext context)
    {
        base.Render(context);
        context.FillRectangle(Brush("One.Workflow.Canvas", Brushes.WhiteSmoke), new Rect(Bounds.Size));
        var line = Brush("One.Line", Brushes.LightGray); var accent = Brush("One.Accent", Brushes.Teal);
        var zoom = _owner?.Zoom ?? 1; var pan = _owner?.Pan ?? default;
        var step = 20 * zoom;
        for (var x = pan.X % step; x < Bounds.Width; x += step) for (var y = pan.Y % step; y < Bounds.Height; y += step) context.DrawEllipse(line, null, new Point(x, y), .7, .7);
        foreach (var c in _owner?.Document?.Connections ?? [])
        {
            if (Anchor(c.SourceNodeId, c.SourcePortId) is not { } from || Anchor(c.TargetNodeId, c.TargetPortId) is not { } to) continue;
            var active = _owner?.Document?.Nodes.FirstOrDefault(n => n.Id == c.TargetNodeId)?.State is WorkflowNodeState.Completed or WorkflowNodeState.Running;
            var points = Route(from, to); DrawEdge(context, points, new Pen(c == _owner?.SelectedConnection || active ? accent : Brush("One.Muted", Brushes.Gray), c == _owner?.SelectedConnection ? 2.5 : 1.5));
            if (!string.IsNullOrEmpty(c.Label))
            {
                var text = new FormattedText(c.Label, System.Globalization.CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Inter"), 12, Brush("One.Text", Brushes.Black));
                var p = new Point((from.X + to.X) / 2 - text.Width / 2, (from.Y + to.Y) / 2 - text.Height - 4);
                context.FillRectangle(Brush("One.Canvas", Brushes.WhiteSmoke), new Rect(p, new Size(text.Width, text.Height))); context.DrawText(text, p);
            }
        }
        if (_connecting != null && Anchor(_connecting.Node.Id, _connecting.Port.Id) is { } start)
        {
            var brush = _connectionError == null ? accent : Brushes.Crimson;
            DrawEdge(context, Route(start, _pointer), new Pen(brush, 2, DashStyle.Dash));
            if (PortAt(_pointer) is { } target && Anchor(target.Node.Id, target.Port.Id) is { } end) context.DrawEllipse(null, new Pen(brush, 2), end, 8, 8);
        }
        if (_gesture == "box") context.DrawRectangle(null, new Pen(accent, 1, DashStyle.Dash), new Rect(_press, _pointer).Normalize());
        if (_views.Count == 0)
        {
            var text = new FormattedText("从节点库添加第一个节点", System.Globalization.CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Inter"), 16, Brush("One.Muted", Brushes.Gray));
            context.DrawText(text, new Point(Math.Max(20, (Bounds.Width - text.Width) / 2), Bounds.Height / 2));
        }
    }
    private static IEnumerable<Visual> Ancestors(object? source) => source is Visual v ? v.GetVisualAncestors().Prepend(v) : [];
    private PortHandle? PortAt(Point point)
    {
        foreach (var view in _views.Values)
        foreach (var button in view.Ports.Values)
        {
            if (button.TranslatePoint(new Point(button.Bounds.Width / 2, button.Bounds.Height / 2), this) is not { } center) continue;
            if ((center.X - point.X) * (center.X - point.X) + (center.Y - point.Y) * (center.Y - point.Y) <= 100) return (PortHandle)button.Tag!;
        }
        return null;
    }
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e); if (_owner is null) return;
        var props = e.GetCurrentPoint(this).Properties; var chain = Ancestors(e.Source).ToArray();
        var node = chain.OfType<Border>().Select(b => b.Tag).OfType<WorkflowNode>().FirstOrDefault();
        _press = _pointer = e.GetPosition(this);
        if (props.IsMiddleButtonPressed || props.IsLeftButtonPressed && (_space || _owner.IsPanMode)) { _gesture = "pan"; _oldPan = _owner.Pan; }
        else if (props.IsLeftButtonPressed && (chain.OfType<Button>().Select(b => b.Tag).OfType<PortHandle>().FirstOrDefault() ?? PortAt(_press)) is { } port)
        {
            if (_owner.IsReadOnly) return;
            if (port.Port.Direction == WorkflowPortDirection.Output) { _connecting = port; _gesture = "connect"; }
            else if (_connecting != null) { CompleteConnection(port); e.Handled = true; return; }
            else { _owner.Report("请先选择输出端口，再选择输入端口"); return; }
        }
        else if (node != null && props.IsLeftButtonPressed)
        {
            if (!_owner.SelectedNodes.Contains(node) || e.KeyModifiers.HasFlag(KeyModifiers.Shift)) _owner.SelectNode(node, e.KeyModifiers.HasFlag(KeyModifiers.Shift));
            if (_owner.IsReadOnly || chain.Any(v => v is Button or TextBox or ComboBox or NumericUpDown)) return;
            _gesture = "move"; _origins = _owner.SelectedNodes.ToDictionary(n => n, n => new Point(n.X, n.Y));
        }
        else if (props.IsLeftButtonPressed)
        {
            var connection = HitConnection(_press);
            if (connection != null) { _owner.SelectNode(null); _owner.SetCurrentValue(WorkflowEditor.SelectedConnectionProperty, connection); Focus(); e.Handled = true; return; }
            if (!e.KeyModifiers.HasFlag(KeyModifiers.Shift)) _owner.SelectNode(null);
            _gesture = "box";
        }
        else return;
        Focus(); _captured = e.Pointer; e.Pointer.Capture(this); e.Handled = true;
    }
    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e); _pointer = e.GetPosition(this); if (_owner is null) return;
        if (_gesture == "pan") _owner.SetCurrentValue(WorkflowEditor.PanProperty, _oldPan + (_pointer - _press));
        if (_gesture == "move" && _origins != null)
        {
            var delta = (_pointer - _press) / _owner.Zoom;
            foreach (var (n, p) in _origins) _preview[n] = new(_owner.Snap(p.X + delta.X), _owner.Snap(p.Y + delta.Y));
            InvalidateArrange();
        }
        if (_connecting != null)
        {
            var target = PortAt(_pointer);
            _connectionError = target == null ? null : _owner.Document?.CheckConnection(_connecting.Node.Id, _connecting.Port.Id, target.Node.Id, target.Port.Id, _owner.AllowCycles);
            _owner.Report(_connectionError ?? (target == null ? "拖动到输入端口" : "松开完成连接"));
        }
        InvalidateVisual();
    }
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e); if (_owner is null) return;
        if (_gesture == "move" && !_owner.IsReadOnly) _owner.Document?.MoveNodes(_preview.Select(p => (p.Key, p.Value.X, p.Value.Y)).ToArray());
        if (_gesture == "box")
        {
            var rect = new Rect(_press, e.GetPosition(this)).Normalize();
            var nodes = _views.Where(v => rect.Intersects(new Rect(ToScreen(new Point(v.Key.X, v.Key.Y)), v.Value.Border.DesiredSize * _owner.Zoom))).Select(v => v.Key);
            _owner.SelectMany(e.KeyModifiers.HasFlag(KeyModifiers.Shift) ? nodes.Concat(_owner.SelectedNodes).Distinct().ToArray() : nodes.ToArray());
        }
        if (_gesture == "connect")
        {
            var hit = this.InputHitTest(e.GetPosition(this));
            var target = Ancestors(hit).OfType<Button>().Select(b => b.Tag).OfType<PortHandle>().FirstOrDefault() ?? PortAt(e.GetPosition(this));
            if (target != null && target != _connecting) CompleteConnection(target);
            else if (Math.Abs(e.GetPosition(this).X - _press.X) + Math.Abs(e.GetPosition(this).Y - _press.Y) < 5) { ReleaseCapture(); _gesture = null; _owner.Report("选择一个输入端口，或按 Escape 取消"); return; }
        }
        CancelGesture();
    }
    private void CompleteConnection(PortHandle target)
    {
        if (_connecting != null) _owner?.Connect(_connecting.Node.Id, _connecting.Port.Id, target.Node.Id, target.Port.Id);
        _connecting = null; InvalidateVisual();
    }
    private void PortClicked(PortHandle port)
    {
        if (_owner?.IsReadOnly != false) return;
        if (_connecting != null) CompleteConnection(port);
        else if (port.Port.Direction == WorkflowPortDirection.Output) { _connecting = port; _owner.Report("选择目标输入端口，或按 Escape 取消"); }
    }
    private WorkflowConnection? HitConnection(Point point)
    {
        foreach (var c in _owner?.Document?.Connections.Reverse() ?? [])
        {
            if (Anchor(c.SourceNodeId, c.SourcePortId) is not { } from || Anchor(c.TargetNodeId, c.TargetPortId) is not { } to) continue;
            var points = Route(from, to);
            for (var i = 1; i < points.Length; i++) { var a = points[i - 1]; var b = points[i]; if (new Rect(a, b).Normalize().Inflate(6).Contains(point)) return c; }
        }
        return null;
    }
    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e); if (_owner is null) return;
        var mouse = e.GetPosition(this); var world = ToWorld(mouse); var zoom = Math.Clamp(_owner.Zoom * (e.Delta.Y > 0 ? 1.1 : 1 / 1.1), .25, 2);
        _owner.SetCurrentValue(WorkflowEditor.ZoomProperty, zoom); _owner.SetCurrentValue(WorkflowEditor.PanProperty, new Vector(mouse.X - world.X * zoom, mouse.Y - world.Y * zoom)); e.Handled = true;
    }
    protected override void OnKeyDown(KeyEventArgs e) { base.OnKeyDown(e); if (e.Key == Key.Space && e.Source == this) { _space = true; e.Handled = true; } }
    protected override void OnKeyUp(KeyEventArgs e) { base.OnKeyUp(e); if (e.Key == Key.Space) _space = false; }
    protected override void OnLostFocus(FocusChangedEventArgs e) { base.OnLostFocus(e); _space = false; }
    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e) { base.OnPointerCaptureLost(e); if (_captured != null) CancelGesture(); }
    private void ReleaseCapture() { var pointer = _captured; _captured = null; pointer?.Capture(null); }
    public void CancelGesture() { _gesture = null; _connecting = null; _connectionError = null; _origins = null; _preview.Clear(); ReleaseCapture(); InvalidateArrange(); InvalidateVisual(); }

    private sealed record PortHandle(WorkflowNode Node, WorkflowPort Port);
    private sealed class NodeHost(WorkflowCanvas owner) : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var (n, view) in owner._views) view.Border.Measure(new Size(n.Width, double.PositiveInfinity));
            return new Size(double.IsFinite(availableSize.Width) ? availableSize.Width : 800, double.IsFinite(availableSize.Height) ? availableSize.Height : 500);
        }
        protected override Size ArrangeOverride(Size finalSize) { owner.ArrangeNodes(); return finalSize; }
    }
    private sealed class NodeView
    {
        public Border Border { get; }
        public Dictionary<string, Button> Ports { get; } = [];
        private readonly WorkflowNode _node;
        public WorkflowNode Node => _node;
        private readonly WorkflowCanvas _canvas;
        private readonly ContentPresenter _body = new();
        private readonly ContentPresenter _content = new();
        private readonly Grid _portGrid = new() { ColumnDefinitions = new ColumnDefinitions("*,*") };
        private WorkflowPort[] _ports = [];
        public NodeView(WorkflowCanvas canvas, WorkflowNode node)
        {
            _canvas = canvas; _node = node;
            var stack = new StackPanel { Spacing = 5, Margin = new Thickness(10) };
            stack.Children.Add(_body); stack.Children.Add(_content);
            var root = new Grid(); root.Children.Add(stack); root.Children.Add(_portGrid); _portGrid.VerticalAlignment = VerticalAlignment.Center;
            _portGrid.Margin = new Thickness(-7, 0);
            Border = new Border { Width = node.Width, MinHeight = node.Shape == WorkflowNodeShape.Pill ? 70 : 82, CornerRadius = new CornerRadius(node.Shape == WorkflowNodeShape.Pill ? 14 : 6), BorderThickness = new Thickness(1), Child = root, Tag = node, UseLayoutRounding = false };
            Border.Bind(Border.BackgroundProperty, new DynamicResourceExtension(node.IsAnnotation ? "One.Workflow.Note" : "One.Surface"));
            Border.Bind(Border.BorderBrushProperty, new DynamicResourceExtension("One.Line"));
            global::Avalonia.Automation.AutomationProperties.SetName(Border, node.Title);
            var menu = new ContextMenu();
            foreach (var (title, action) in new (string, Action)[] { ("复制", () => { canvas._owner?.SelectNode(node); canvas._owner?.CopySelection(); }), ("创建副本", () => { canvas._owner?.SelectNode(node); canvas._owner?.CopySelection(); canvas._owner?.PasteSelection(); }), ("删除", () => { canvas._owner?.SelectNode(node); canvas._owner?.DeleteSelection(); }) })
            { var item = new MenuItem { Header = title }; item.Click += (_, _) => action(); menu.Items.Add(item); }
            Border.ContextMenu = menu;
            if (node.Shape == WorkflowNodeShape.Card)
            {
                var more = new Button { Content = "⋯", Width = 22, Height = 22, MinHeight = 0, Padding = default, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(0, 2, 3, 0), Background = Brushes.Transparent, BorderThickness = default };
                ToolTip.SetTip(more, "节点操作"); more.Click += (_, _) => menu.Open(more); root.Children.Add(more);
            }
        }
        public void Update(WorkflowEditor owner)
        {
            _body.Content = _node; _body.ContentTemplate = owner.NodeTemplate;
            Border.Width = _node.Width;
            _content.Content = _node.Content; _content.ContentTemplate = owner.ContentTemplate; _content.IsVisible = _node.Content != null && owner.ContentTemplate != null;
            _body.IsEnabled = _content.IsEnabled = !owner.IsReadOnly;
            if (_ports.SequenceEqual(_node.Ports)) return;
            _ports = _node.Ports.ToArray(); Ports.Clear(); _portGrid.Children.Clear();
            foreach (var direction in new[] { WorkflowPortDirection.Input, WorkflowPortDirection.Output })
            {
                var stack = new StackPanel { Spacing = 16, HorizontalAlignment = direction == WorkflowPortDirection.Input ? HorizontalAlignment.Left : HorizontalAlignment.Right }; Grid.SetColumn(stack, direction == WorkflowPortDirection.Input ? 0 : 1); _portGrid.Children.Add(stack);
                foreach (var p in _ports.Where(p => p.Direction == direction))
                {
                    var button = new Button { Width = 12, Height = 12, MinHeight = 0, Padding = default, CornerRadius = new CornerRadius(6), BorderThickness = new Thickness(1.5), Tag = new PortHandle(_node, p) };
                    button.Bind(Button.BackgroundProperty, new DynamicResourceExtension("One.Surface")); button.Bind(Button.BorderBrushProperty, new DynamicResourceExtension("One.Accent"));
                    global::Avalonia.Automation.AutomationProperties.SetName(button, $"{_node.Title} {p.Title} {direction}");
                    ToolTip.SetTip(button, $"{p.Id} · {p.DataType} · {(p.MaxConnections <= 0 ? "不限连接" : $"最多 {p.MaxConnections} 条")}");
                    button.Click += (_, _) => _canvas.PortClicked((PortHandle)button.Tag!);
                    Ports[p.Id] = button; stack.Children.Add(button);
                }
            }
        }
    }
}
