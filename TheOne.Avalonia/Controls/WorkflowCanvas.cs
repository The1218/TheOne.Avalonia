using System.Globalization;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.VisualTree;
using TheOne.Avalonia.Workflow;

namespace TheOne.Avalonia.Controls;

/// <summary>WorkflowEditor template part. Graph coordinates are independent of zoom and pan.</summary>
public sealed partial class WorkflowCanvas : Control
{
    private readonly NodeHost _host;
    private readonly Dictionary<WorkflowNode, Point> _preview = [];
    private readonly Dictionary<WorkflowNode, NodeView> _views = [];
    private IPointer? _captured;
    private PortHandle? _connecting;
    private string? _connectionError;
    private string? _gesture;
    private Vector _oldPan;
    private Dictionary<WorkflowNode, Point>? _origins;
    private WorkflowEditor? _owner;
    private Point _press, _pointer;
    private bool _space;

    public WorkflowCanvas()
    {
        ResourcesChanged += (_, _) =>
        {
            InvalidateEdgeText();
            UpdateSelectionShadow();
            InvalidateVisual();
        };
        _host = new NodeHost(this);
        ClipToBounds = true;
        Focusable = true;
        VisualChildren.Add(_host);
        LogicalChildren.Add(_host);
        AddHandler(PointerPressedEvent, (_, e) =>
        {
            if (Ancestors(e.Source).OfType<Button>().Any(b => b.Tag is PortHandle) ||
                PortAt(e.GetPosition(this)) != null) OnPointerPressed(e);
        }, RoutingStrategies.Tunnel);
    }

    private global::Avalonia.Controls.Controls Children => _host.Children;

    internal Rect GraphBounds => GetGraphBounds();

    internal void SetOwner(WorkflowEditor? owner)
    {
        CancelGesture();
        _owner = owner;
        Refresh(true);
    }

    internal void Refresh(bool rebuild = false)
    {
        RefreshCachedViews(rebuild);
        UpdateSelection();
        InvalidateMeasure();
        InvalidateVisual();
    }

    internal void UpdateSelection()
    {
        var selected = _owner?.SelectedNodes.ToHashSet() ?? [];
        foreach (var (node, view) in _views)
        {
            var isSelected = selected.Contains(node);
            if (view.IsSelected == isSelected) continue;
            view.IsSelected = isSelected;
            view.Border.Bind(Border.BorderBrushProperty,
                new DynamicResourceExtension(isSelected ? "One.Accent" : "One.Line"));
        }

        UpdateSelectionShadow();
        InvalidateVisual();
    }

    private void UpdateSelectionShadow()
    {
        var color = Brush("One.Accent", Brushes.Teal) is ISolidColorBrush solid ? solid.Color : Colors.Teal;
        foreach (var view in _views.Values)
            view.Border.BoxShadow = view.IsSelected
                ? new BoxShadows(new BoxShadow
                    { Color = Color.FromArgb(60, color.R, color.G, color.B), Blur = 8, Spread = 2 })
                : default;
    }

    public Point ToWorld(Point point)
    {
        return _owner is { } o ? new Point((point.X - o.Pan.X) / o.Zoom, (point.Y - o.Pan.Y) / o.Zoom) : point;
    }

    private Point ToScreen(Point point)
    {
        return _owner is { } o ? new Point(point.X * o.Zoom + o.Pan.X, point.Y * o.Zoom + o.Pan.Y) : point;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        PrepareViewport(availableSize);
        _host.Measure(availableSize);
        return new Size(double.IsFinite(availableSize.Width) ? availableSize.Width : 800,
            double.IsFinite(availableSize.Height) ? availableSize.Height : 500);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (finalSize != Bounds.Size)
        {
            PrepareViewport(finalSize);
            _host.Measure(finalSize);
        }
        _host.Arrange(new Rect(finalSize));
        ArrangeNodes(finalSize);
        return finalSize;
    }

    private void ArrangeNodes(Size viewportSize)
    {
        foreach (var (n, view) in _views.Where(pair => pair.Value.Border.Parent == _host))
        {
            var point = ToScreen(_preview.GetValueOrDefault(n, new Point(n.X, n.Y)));
            view.Border.RenderTransformOrigin = RelativePoint.TopLeft;
            var zoom = _owner?.Zoom ?? 1;
            if (view.Border.RenderTransform is not ScaleTransform scale || scale.ScaleX != zoom || scale.ScaleY != zoom)
                view.Border.RenderTransform = new ScaleTransform(zoom, zoom);
            view.Border.Arrange(new Rect(point, view.Border.DesiredSize));
            RememberMetrics(n, view);
        }
        TrimViews(viewportSize);
        if (_metricsChanged)
        {
            _metricsChanged = false;
            _owner?.NotifyCanvasLayoutChanged();
        }
        InvalidateVisual();
    }

    private IBrush Brush(string key, IBrush fallback)
    {
        return this.TryFindResource(key, ActualThemeVariant, out var value) && value is IBrush brush ? brush : fallback;
    }

    private Point? Anchor(string nodeId, string portId)
    {
        return WorldAnchor(nodeId, portId) is { } point ? ToScreen(point) : null;
    }

    private static Point[] Route(Point from, Point to)
    {
        var middle = from.X < to.X - 30 ? (from.X + to.X) / 2 : Math.Max(from.X, to.X) + 40;
        return [from, new Point(middle, from.Y), new Point(middle, to.Y), to];
    }

    private static void DrawEdge(DrawingContext context, Point[] points, Pen pen)
    {
        var geometry = CreateEdgeGeometry(points);
        DrawEdge(context, points, geometry, pen);
    }

    private static StreamGeometry CreateEdgeGeometry(Point[] points)
    {
        var geometry = new StreamGeometry();
        using (var path = geometry.Open())
        {
            path.BeginFigure(points[0], false);
            for (var i = 1; i < points.Length - 1; i++)
            {
                var p = points[i];
                var before = points[i - 1];
                var after = points[i + 1];
                var firstLength = Math.Abs(p.X - before.X) + Math.Abs(p.Y - before.Y);
                var nextLength = Math.Abs(p.X - after.X) + Math.Abs(p.Y - after.Y);
                var radius = Math.Min(6, Math.Min(firstLength, nextLength) / 2);
                if (radius == 0)
                {
                    path.LineTo(p);
                    continue;
                }

                path.LineTo(new Point(p.X + (before.X - p.X) * radius / firstLength,
                    p.Y + (before.Y - p.Y) * radius / firstLength));
                path.QuadraticBezierTo(p,
                    new Point(p.X + (after.X - p.X) * radius / nextLength,
                        p.Y + (after.Y - p.Y) * radius / nextLength));
            }

            path.LineTo(points[^1]);
            path.EndFigure(false);
        }

        return geometry;
    }

    private static void DrawEdge(DrawingContext context, Point[] points, StreamGeometry geometry, Pen pen)
    {
        context.DrawGeometry(null, pen, geometry);
        var end = points[^1];
        var previous = points[^2];
        var sign = end.X >= previous.X ? 1 : -1;
        context.DrawLine(pen, end, new Point(end.X - 6 * sign, end.Y - 4));
        context.DrawLine(pen, end, new Point(end.X - 6 * sign, end.Y + 4));
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        context.FillRectangle(Brush("One.Workflow.Canvas", Brushes.WhiteSmoke), new Rect(Bounds.Size));
        var line = Brush("One.Line", Brushes.LightGray);
        var accent = Brush("One.Accent", Brushes.Teal);
        var zoom = _owner?.Zoom ?? 1;
        var pan = _owner?.Pan ?? default;
        var step = 20 * zoom;
        for (var x = pan.X % step; x < Bounds.Width; x += step)
        for (var y = pan.Y % step; y < Bounds.Height; y += step)
            context.DrawEllipse(line, null, new Point(x, y), .7, .7);
        RenderEdges(context, accent);

        if (_connecting != null && Anchor(_connecting.Node.Id, _connecting.Port.Id) is { } start)
        {
            var brush = _connectionError == null ? accent : Brushes.Crimson;
            DrawEdge(context, Route(start, _pointer), new Pen(brush, 2, DashStyle.Dash));
            if (PortAt(_pointer) is { } target && Anchor(target.Node.Id, target.Port.Id) is { } end)
                context.DrawEllipse(null, new Pen(brush, 2), end, 8, 8);
        }

        if (_gesture == "box")
            context.DrawRectangle(null, new Pen(accent, 1, DashStyle.Dash), new Rect(_press, _pointer).Normalize());
        if (_owner?.Document?.Nodes.Count is null or 0)
        {
            var text = new FormattedText("从节点库添加第一个节点", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                new Typeface("Inter"), 16, Brush("One.Muted", Brushes.Gray));
            context.DrawText(text, new Point(Math.Max(20, (Bounds.Width - text.Width) / 2), Bounds.Height / 2));
        }
    }

    private static IEnumerable<Visual> Ancestors(object? source)
    {
        return source is Visual v ? v.GetVisualAncestors().Prepend(v) : [];
    }

    private PortHandle? PortAt(Point point)
    {
        foreach (var view in _views.Values.Where(view => view.Border.Parent == _host))
        foreach (var button in view.Ports.Values)
        {
            if (button.TranslatePoint(new Point(button.Bounds.Width / 2, button.Bounds.Height / 2), this) is not
                { } center) continue;
            if ((center.X - point.X) * (center.X - point.X) + (center.Y - point.Y) * (center.Y - point.Y) <= 100)
                return (PortHandle)button.Tag!;
        }

        return null;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (_owner is null) return;
        var props = e.GetCurrentPoint(this).Properties;
        var chain = Ancestors(e.Source).ToArray();
        var node = chain.OfType<Border>().Select(b => b.Tag).OfType<WorkflowNode>().FirstOrDefault();
        _press = _pointer = e.GetPosition(this);
        if (props.IsMiddleButtonPressed || (props.IsLeftButtonPressed && (_space || _owner.IsPanMode)))
        {
            _gesture = "pan";
            _oldPan = _owner.Pan;
        }
        else if (props.IsLeftButtonPressed &&
                 (chain.OfType<Button>().Select(b => b.Tag).OfType<PortHandle>().FirstOrDefault() ?? PortAt(_press)) is
                 { } port)
        {
            if (_owner.IsReadOnly) return;
            if (port.Port.Direction == WorkflowPortDirection.Output)
            {
                _connecting = port;
                _gesture = "connect";
            }
            else if (_connecting != null)
            {
                CompleteConnection(port);
                e.Handled = true;
                return;
            }
            else
            {
                _owner.Report("请先选择输出端口，再选择输入端口");
                return;
            }
        }
        else if (node != null && props.IsLeftButtonPressed)
        {
            if (!_owner.SelectedNodes.Contains(node) || e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                _owner.SelectNode(node, e.KeyModifiers.HasFlag(KeyModifiers.Shift));
            if (_owner.IsReadOnly || chain.Any(v => v is Button or TextBox or ComboBox or NumericUpDown)) return;
            _gesture = "move";
            _origins = _owner.SelectedNodes.ToDictionary(n => n, n => new Point(n.X, n.Y));
        }
        else if (props.IsLeftButtonPressed)
        {
            var connection = HitConnection(_press);
            if (connection != null)
            {
                _owner.SelectNode(null);
                _owner.SetCurrentValue(WorkflowEditor.SelectedConnectionProperty, connection);
                Focus();
                e.Handled = true;
                return;
            }

            if (!e.KeyModifiers.HasFlag(KeyModifiers.Shift)) _owner.SelectNode(null);
            _gesture = "box";
        }
        else
        {
            return;
        }

        Focus();
        _captured = e.Pointer;
        e.Pointer.Capture(this);
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        _pointer = e.GetPosition(this);
        if (_owner is null) return;
        if (_gesture == "pan") _owner.SetCurrentValue(WorkflowEditor.PanProperty, _oldPan + (_pointer - _press));
        if (_gesture == "move" && _origins != null)
        {
            var delta = (_pointer - _press) / _owner.Zoom;
            foreach (var (n, p) in _origins)
                _preview[n] = new Point(_owner.Snap(p.X + delta.X), _owner.Snap(p.Y + delta.Y));
            InvalidateMeasure();
        }

        if (_connecting != null)
        {
            var target = PortAt(_pointer);
            _connectionError = target == null
                ? null
                : _owner.Document?.CheckConnection(_connecting.Node.Id, _connecting.Port.Id, target.Node.Id,
                    target.Port.Id, _owner.AllowCycles);
            _owner.Report(_connectionError ?? (target == null ? "拖动到输入端口" : "松开完成连接"));
        }

        InvalidateVisual();
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_owner is null) return;
        if (_gesture == "move" && !_owner.IsReadOnly)
            _owner.Document?.MoveNodes(_preview.Select(p => (p.Key, p.Value.X, p.Value.Y)).ToArray());
        if (_gesture == "box")
        {
            var rect = new Rect(_press, e.GetPosition(this)).Normalize();
            var nodes = (_owner.Document?.Nodes ?? []).Where(node =>
                rect.Intersects(new Rect(ToScreen(new Point(node.X, node.Y)), NodeSize(node) * _owner.Zoom)));
            _owner.SelectMany(e.KeyModifiers.HasFlag(KeyModifiers.Shift)
                ? nodes.Concat(_owner.SelectedNodes).Distinct().ToArray()
                : nodes.ToArray());
        }

        if (_gesture == "connect")
        {
            var hit = this.InputHitTest(e.GetPosition(this));
            var target = Ancestors(hit).OfType<Button>().Select(b => b.Tag).OfType<PortHandle>().FirstOrDefault() ??
                         PortAt(e.GetPosition(this));
            if (target != null && target != _connecting)
            {
                CompleteConnection(target);
            }
            else if (Math.Abs(e.GetPosition(this).X - _press.X) + Math.Abs(e.GetPosition(this).Y - _press.Y) < 5)
            {
                ReleaseCapture();
                _gesture = null;
                _owner.Report("选择一个输入端口，或按 Escape 取消");
                return;
            }
        }

        CancelGesture();
    }

    private void CompleteConnection(PortHandle target)
    {
        if (_connecting != null)
            _owner?.Connect(_connecting.Node.Id, _connecting.Port.Id, target.Node.Id, target.Port.Id);
        _connecting = null;
        InvalidateMeasure();
        InvalidateVisual();
    }

    private void PortClicked(PortHandle port)
    {
        if (_owner?.IsReadOnly != false) return;
        if (_connecting != null)
        {
            CompleteConnection(port);
        }
        else if (port.Port.Direction == WorkflowPortDirection.Output)
        {
            _connecting = port;
            _owner.Report("选择目标输入端口，或按 Escape 取消");
        }
    }

    private WorkflowConnection? HitConnection(Point point)
    {
        foreach (var c in _owner?.Document?.Connections.Reverse() ?? [])
        {
            var local = point - (_owner?.Pan ?? default);
            if (CachedEdge(c) is not { } edge || !edge.Bounds.Inflate(6).Contains(local)) continue;
            var points = edge.Points;
            for (var i = 1; i < points.Length; i++)
            {
                var a = points[i - 1];
                var b = points[i];
                if (new Rect(a, b).Normalize().Inflate(6).Contains(local)) return c;
            }
        }

        return null;
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        if (_owner is null) return;
        var mouse = e.GetPosition(this);
        var world = ToWorld(mouse);
        var zoom = Math.Clamp(_owner.Zoom * (e.Delta.Y > 0 ? 1.1 : 1 / 1.1), .25, 2);
        _owner.SetCurrentValue(WorkflowEditor.ZoomProperty, zoom);
        _owner.SetCurrentValue(WorkflowEditor.PanProperty,
            new Vector(mouse.X - world.X * zoom, mouse.Y - world.Y * zoom));
        e.Handled = true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.Space && e.Source == this)
        {
            _space = true;
            e.Handled = true;
        }
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);
        if (e.Key == Key.Space) _space = false;
    }

    protected override void OnLostFocus(FocusChangedEventArgs e)
    {
        base.OnLostFocus(e);
        _space = false;
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        if (_captured != null) CancelGesture();
    }

    private void ReleaseCapture()
    {
        var pointer = _captured;
        _captured = null;
        pointer?.Capture(null);
    }

    public void CancelGesture()
    {
        _gesture = null;
        _connecting = null;
        _connectionError = null;
        _origins = null;
        _preview.Clear();
        ReleaseCapture();
        InvalidateMeasure();
        InvalidateVisual();
    }

    private sealed record PortHandle(WorkflowNode Node, WorkflowPort Port);

    private sealed class NodeHost(WorkflowCanvas owner) : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var (n, view) in owner._views.Where(pair => pair.Value.Border.Parent == this))
                view.Border.Measure(new Size(n.Width, double.PositiveInfinity));
            return new Size(double.IsFinite(availableSize.Width) ? availableSize.Width : 800,
                double.IsFinite(availableSize.Height) ? availableSize.Height : 500);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return finalSize;
        }
    }

    private sealed class NodeView
    {
        private readonly ContentPresenter _body = new();
        private readonly WorkflowCanvas _canvas;
        private readonly ContentPresenter _content = new();
        private readonly Grid _portGrid = new() { ColumnDefinitions = new ColumnDefinitions("*,*") };
        private WorkflowPort[] _ports = [];

        public NodeView(WorkflowCanvas canvas, WorkflowNode node)
        {
            _canvas = canvas;
            Node = node;
            var stack = new StackPanel { Spacing = 5, Margin = new Thickness(10) };
            stack.Children.Add(_body);
            stack.Children.Add(_content);
            var root = new Grid();
            root.Children.Add(stack);
            root.Children.Add(_portGrid);
            _portGrid.VerticalAlignment = VerticalAlignment.Center;
            _portGrid.Margin = new Thickness(-7, 0);
            Border = new Border
            {
                Width = node.Width, MinHeight = node.Shape == WorkflowNodeShape.Pill ? 70 : 82,
                CornerRadius = new CornerRadius(node.Shape == WorkflowNodeShape.Pill ? 14 : 6),
                BorderThickness = new Thickness(1), Child = root, Tag = node, UseLayoutRounding = false
            };
            Border.Bind(Border.BackgroundProperty,
                new DynamicResourceExtension(node.IsAnnotation ? "One.Workflow.Note" : "One.Surface"));
            Border.Bind(Border.BorderBrushProperty, new DynamicResourceExtension("One.Line"));
            AutomationProperties.SetName(Border, node.Title);
            var menu = new ContextMenu();
            foreach (var (title, action) in new (string, Action)[]
                     {
                         ("复制", () =>
                         {
                             canvas._owner?.SelectNode(node);
                             canvas._owner?.CopySelection();
                         }),
                         ("创建副本", () =>
                         {
                             canvas._owner?.SelectNode(node);
                             canvas._owner?.CopySelection();
                             canvas._owner?.PasteSelection();
                         }),
                         ("删除", () =>
                         {
                             canvas._owner?.SelectNode(node);
                             canvas._owner?.DeleteSelection();
                         })
                     })
            {
                var item = new MenuItem { Header = title };
                item.Click += (_, _) => action();
                menu.Items.Add(item);
            }

            Border.ContextMenu = menu;
            menu.Closed += (_, _) => canvas.InvalidateMeasure();
            Border.AddHandler(LostFocusEvent, (_, _) => canvas.InvalidateMeasure());
            if (node.Shape == WorkflowNodeShape.Card)
            {
                var more = new Button
                {
                    Content = "⋯", Width = 22, Height = 22, MinHeight = 0, Padding = default,
                    HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 2, 3, 0), Background = Brushes.Transparent, BorderThickness = default
                };
                ToolTip.SetTip(more, "节点操作");
                more.Click += (_, _) => menu.Open(more);
                root.Children.Add(more);
            }
        }

        public Border Border { get; }
        public Dictionary<string, Button> Ports { get; } = [];
        public WorkflowNode Node { get; }
        public bool IsSelected { get; set; }

        public void Release()
        {
            Border.ContextMenu?.Close();
            Border.ContextMenu = null;
            _body.Content = null;
            _body.ContentTemplate = null;
            _content.Content = null;
            _content.ContentTemplate = null;
            Ports.Clear();
            _portGrid.Children.Clear();
            Border.Child = null;
        }

        public void Update(WorkflowEditor owner)
        {
            _body.Content = Node;
            _body.ContentTemplate = owner.NodeTemplate;
            Border.Width = Node.Width;
            _content.Content = Node.Content;
            _content.ContentTemplate = owner.ContentTemplate;
            _content.IsVisible = Node.Content != null && owner.ContentTemplate != null;
            _body.IsEnabled = _content.IsEnabled = !owner.IsReadOnly;
            if (_ports.SequenceEqual(Node.Ports)) return;
            _ports = Node.Ports.ToArray();
            Ports.Clear();
            _portGrid.Children.Clear();
            foreach (var direction in new[] { WorkflowPortDirection.Input, WorkflowPortDirection.Output })
            {
                var stack = new StackPanel
                {
                    Spacing = 16,
                    HorizontalAlignment = direction == WorkflowPortDirection.Input
                        ? HorizontalAlignment.Left
                        : HorizontalAlignment.Right
                };
                Grid.SetColumn(stack, direction == WorkflowPortDirection.Input ? 0 : 1);
                _portGrid.Children.Add(stack);
                foreach (var p in _ports.Where(p => p.Direction == direction))
                {
                    var button = new Button
                    {
                        Width = 12, Height = 12, MinHeight = 0, Padding = default, CornerRadius = new CornerRadius(6),
                        BorderThickness = new Thickness(1.5), Tag = new PortHandle(Node, p)
                    };
                    button.Bind(Button.BackgroundProperty, new DynamicResourceExtension("One.Surface"));
                    button.Bind(Button.BorderBrushProperty, new DynamicResourceExtension("One.Accent"));
                    AutomationProperties.SetName(button, $"{Node.Title} {p.Title} {direction}");
                    ToolTip.SetTip(button,
                        $"{p.Id} · {p.DataType} · {(p.MaxConnections <= 0 ? "不限连接" : $"最多 {p.MaxConnections} 条")}");
                    button.Click += (_, _) => _canvas.PortClicked((PortHandle)button.Tag!);
                    Ports[p.Id] = button;
                    stack.Children.Add(button);
                }
            }
        }
    }
}
