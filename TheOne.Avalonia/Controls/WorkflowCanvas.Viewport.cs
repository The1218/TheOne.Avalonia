using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Markup.Xaml.Templates;
using TheOne.Avalonia.Styling;
using TheOne.Avalonia.Workflow;

namespace TheOne.Avalonia.Controls;

// Only this library-owned, display-only template is safe to discard automatically.
// A host template, including a replacement for the same resource key, is stateful by default.
internal sealed class WorkflowDefaultNodeTemplate : DataTemplate { }

public sealed partial class WorkflowCanvas
{
    private const int InactiveViewLimit = 64;
    private readonly Dictionary<WorkflowNode, NodeMetrics> _metrics = [];
    private readonly LinkedList<WorkflowNode> _inactiveViews = [];
    private readonly Dictionary<WorkflowNode, LinkedListNode<WorkflowNode>> _inactiveNodes = [];
    private WorkflowDocument? _document;
    private Rect? _graphBounds;
    private object? _layoutEnvironment;
    private int _layoutVersion;
    private bool _metricsChanged;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TextElement.FontFamilyProperty || change.Property == TextElement.FontSizeProperty ||
            change.Property == TextElement.FontWeightProperty || change.Property == TextElement.FontStyleProperty ||
            change.Property == TextElement.FontStretchProperty || change.Property == FlowDirectionProperty ||
            change.Property == ControlSizing.SizeProperty)
            InvalidateMeasure();
    }

    private void RefreshCachedViews(bool rebuild)
    {
        var document = _owner?.Document;
        if (_origins?.Keys.Any(node => document?.ContainsNode(node) != true) == true ||
            _connecting is { } connecting && document?.ContainsNode(connecting.Node) != true)
            CancelGesture();
        if (rebuild || !ReferenceEquals(_document, document))
        {
            foreach (var view in _views.Values) view.Release();
            Children.Clear();
            _views.Clear();
            _metrics.Clear();
            _preview.Clear();
            _inactiveViews.Clear();
            _inactiveNodes.Clear();
            _edges.Clear();
            _document = document;
        }

        foreach (var node in _metrics.Keys.Where(node => document?.ContainsNode(node) != true).ToArray())
        {
            if (_views.Remove(node, out var view))
            {
                view.Release();
                Children.Remove(view.Border);
            }
            RemoveInactive(node);
            _metrics.Remove(node);
            _preview.Remove(node);
        }

        foreach (var edge in _edges.Keys.Where(edge => document?.ContainsConnection(edge) != true).ToArray())
            _edges.Remove(edge);
        foreach (var node in document?.Nodes ?? [])
        {
            if (!_metrics.TryGetValue(node, out var metrics)) _metrics.Add(node, metrics = new NodeMetrics());
            if (_views.TryGetValue(node, out var view)) view.Update(_owner!);
            else if (!metrics.Matches(node, _layoutVersion)) Mount(node);
        }

        _graphBounds = null;
    }

    private void Mount(WorkflowNode node)
    {
        RemoveInactive(node);
        if (!_views.TryGetValue(node, out var view))
        {
            view = new NodeView(this, node);
            _views.Add(node, view);
        }
        view.Update(_owner!);
        if (view.Border.Parent != _host) Children.Add(view.Border);
    }

    private void PrepareViewport(Size available)
    {
        var environment = (TextElement.GetFontFamily(this), TextElement.GetFontSize(this),
            TextElement.GetFontWeight(this), TextElement.GetFontStyle(this),
            GetValue(TextElement.FontStretchProperty), FlowDirection, ControlSizing.GetSize(this),
            TopLevel.GetTopLevel(this)?.RenderScaling ?? 1);
        if (!Equals(_layoutEnvironment, environment))
        {
            _layoutEnvironment = environment;
            _layoutVersion++;
            _host.InvalidateMeasure();
            foreach (var view in _views.Values) view.Border.InvalidateMeasure();
        }
        var viewport = WorldViewport(available);
        foreach (var node in _document?.Nodes ?? [])
        {
            if (!_metrics.TryGetValue(node, out var metrics)) _metrics.Add(node, metrics = new NodeMetrics());
            if (!metrics.Matches(node, _layoutVersion) || NodeRect(node).Intersects(viewport) ||
                _origins?.ContainsKey(node) == true || _connecting?.Node == node ||
                _views.TryGetValue(node, out var view) && MustKeepMounted(node, view))
                Mount(node);
        }
        UpdateSelection();
    }

    private Rect WorldViewport(Size size)
    {
        var width = double.IsFinite(size.Width) ? size.Width : Bounds.Width;
        var height = double.IsFinite(size.Height) ? size.Height : Bounds.Height;
        var zoom = _owner?.Zoom ?? 1;
        var pan = _owner?.Pan ?? default;
        return new Rect(-pan.X / zoom, -pan.Y / zoom, Math.Max(0, width) / zoom,
            Math.Max(0, height) / zoom).Inflate(96 / zoom);
    }

    private bool MustKeepMounted(WorkflowNode node, NodeView view) =>
        _owner?.NodeTemplate is not WorkflowDefaultNodeTemplate ||
        _owner.ContentTemplate != null || node.Content is Control ||
        view.Border.IsKeyboardFocusWithin || view.Border.ContextMenu?.IsOpen == true ||
        _origins?.ContainsKey(node) == true || _connecting?.Node == node;

    private void TrimViews(Size viewportSize)
    {
        // Bounds can still contain the previous size while ArrangeOverride is running.
        var viewport = WorldViewport(viewportSize);
        foreach (var (node, view) in _views.ToArray())
        {
            if (view.Border.Parent != _host || NodeRect(node).Intersects(viewport) || MustKeepMounted(node, view))
                continue;
            Children.Remove(view.Border);
            _inactiveNodes[node] = _inactiveViews.AddFirst(node);
        }
        while (_inactiveViews.Count > InactiveViewLimit)
        {
            var node = _inactiveViews.Last!.Value;
            RemoveInactive(node);
            if (_views.Remove(node, out var view)) view.Release();
        }
    }

    private void RemoveInactive(WorkflowNode node)
    {
        if (_inactiveNodes.Remove(node, out var cached)) _inactiveViews.Remove(cached);
    }

    private Size NodeSize(WorkflowNode node) => _metrics.TryGetValue(node, out var metrics) && metrics.Size.Width > 0
        ? metrics.Size : new Size(node.Width, node.Shape == WorkflowNodeShape.Pill ? 70 : 82);

    private Rect NodeRect(WorkflowNode node) =>
        new(_preview.GetValueOrDefault(node, new Point(node.X, node.Y)), NodeSize(node));

    private Rect GetGraphBounds()
    {
        if (_graphBounds is { } cached) return cached;
        Rect? bounds = null;
        foreach (var node in _document?.Nodes ?? [])
        {
            var rect = new Rect(new Point(node.X, node.Y), NodeSize(node));
            bounds = bounds?.Union(rect) ?? rect;
        }
        _graphBounds = bounds ?? new Rect(0, 0, 180, 100);
        return _graphBounds.Value;
    }

    private void RememberMetrics(WorkflowNode node, NodeView view)
    {
        var metrics = _metrics[node];
        if (metrics.Matches(node, _layoutVersion) && metrics.Size == view.Border.DesiredSize) return;
        _metricsChanged |= metrics.Size != view.Border.DesiredSize;
        metrics.Size = view.Border.DesiredSize;
        metrics.Ports.Clear();
        foreach (var (id, button) in view.Ports)
            if (button.Tag is PortHandle handle &&
                button.TranslatePoint(new Point(0, button.Bounds.Height / 2), view.Border) is { } point)
                metrics.Ports[id] = new Point(handle.Port.Direction == WorkflowPortDirection.Input ? 0 : metrics.Size.Width,
                    point.Y);
        metrics.Remember(node, _layoutVersion);
        _graphBounds = null;
    }

    private Point? WorldAnchor(string nodeId, string portId)
    {
        if (_document?.FindNode(nodeId) is not { } node || !_metrics.TryGetValue(node, out var metrics) ||
            !metrics.Ports.TryGetValue(portId, out var offset)) return null;
        var position = _preview.GetValueOrDefault(node, new Point(node.X, node.Y));
        return new Point(position.X + offset.X, position.Y + offset.Y);
    }

    private sealed class NodeMetrics
    {
        public Size Size;
        public readonly Dictionary<string, Point> Ports = [];
        private double _width;
        private string? _title, _description;
        private WorkflowNodeState _state;
        private object? _content;
        private WorkflowPort[] _ports = [];
        private int _version = -1;

        public bool Matches(WorkflowNode node, int version) => _version == version && _width == node.Width &&
            _title == node.Title && _description == node.Description && _state == node.State &&
            ReferenceEquals(_content, node.Content) && _ports.SequenceEqual(node.Ports);

        public void Remember(WorkflowNode node, int version)
        {
            _width = node.Width;
            _title = node.Title;
            _description = node.Description;
            _state = node.State;
            _content = node.Content;
            _ports = node.Ports.ToArray();
            _version = version;
        }
    }
}
