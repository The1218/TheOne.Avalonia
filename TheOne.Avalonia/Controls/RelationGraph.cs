using System.Collections;
using System.Collections.Specialized;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Controls.Templates;
using global::Avalonia.Data;
using global::Avalonia.Layout;
using global::Avalonia.Media;
using global::Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

/// <summary>A single-root tree with native node cards and directly drawn orthogonal connectors.</summary>
public partial class RelationGraph : Control
{
    public static readonly StyledProperty<IBrush?> BackgroundProperty = Panel.BackgroundProperty.AddOwner<RelationGraph>();
    public static readonly StyledProperty<object?> RootNodeProperty = AvaloniaProperty.Register<RelationGraph, object?>(nameof(RootNode));
    public static readonly StyledProperty<Orientation> OrientationProperty = AvaloniaProperty.Register<RelationGraph, Orientation>(nameof(Orientation), Orientation.Horizontal, validate: Enum.IsDefined);
    public static readonly StyledProperty<BindingBase?> ChildrenSourceProperty = AvaloniaProperty.Register<RelationGraph, BindingBase?>(nameof(ChildrenSource));
    public static readonly StyledProperty<BindingBase?> IconSourceProperty = AvaloniaProperty.Register<RelationGraph, BindingBase?>(nameof(IconSource));
    public static readonly StyledProperty<BindingBase?> TitleSourceProperty = AvaloniaProperty.Register<RelationGraph, BindingBase?>(nameof(TitleSource));
    public static readonly StyledProperty<BindingBase?> ContentSourceProperty = AvaloniaProperty.Register<RelationGraph, BindingBase?>(nameof(ContentSource));
    public static readonly StyledProperty<IDataTemplate?> IconTemplateProperty = AvaloniaProperty.Register<RelationGraph, IDataTemplate?>(nameof(IconTemplate));
    public static readonly StyledProperty<IDataTemplate?> TitleTemplateProperty = AvaloniaProperty.Register<RelationGraph, IDataTemplate?>(nameof(TitleTemplate));
    public static readonly StyledProperty<IDataTemplate?> ContentTemplateProperty = AvaloniaProperty.Register<RelationGraph, IDataTemplate?>(nameof(ContentTemplate));
    public static readonly StyledProperty<ControlTheme?> NodeThemeProperty = AvaloniaProperty.Register<RelationGraph, ControlTheme?>(nameof(NodeTheme));
    public static readonly StyledProperty<double> NodeWidthProperty = AvaloniaProperty.Register<RelationGraph, double>(nameof(NodeWidth), 260, validate: Positive);
    public static readonly StyledProperty<double> NodeMinHeightProperty = AvaloniaProperty.Register<RelationGraph, double>(nameof(NodeMinHeight), 88, validate: Positive);
    public static readonly StyledProperty<double> NodeSpacingProperty = AvaloniaProperty.Register<RelationGraph, double>(nameof(NodeSpacing), 36, validate: NonNegative);
    public static readonly StyledProperty<double> LevelSpacingProperty = AvaloniaProperty.Register<RelationGraph, double>(nameof(LevelSpacing), 80, validate: NonNegative);
    public static readonly StyledProperty<Thickness> PaddingProperty = Decorator.PaddingProperty.AddOwner<RelationGraph>();
    public static readonly StyledProperty<IBrush?> ConnectorBrushProperty = AvaloniaProperty.Register<RelationGraph, IBrush?>(nameof(ConnectorBrush));
    public static readonly StyledProperty<double> ConnectorThicknessProperty = AvaloniaProperty.Register<RelationGraph, double>(nameof(ConnectorThickness), 1.5, validate: NonNegative);
    public static readonly StyledProperty<double> ConnectorCornerRadiusProperty = AvaloniaProperty.Register<RelationGraph, double>(nameof(ConnectorCornerRadius), 8, validate: NonNegative);
    public static readonly StyledProperty<double> PortRadiusProperty = AvaloniaProperty.Register<RelationGraph, double>(nameof(PortRadius), 3, validate: NonNegative);
    public static readonly DirectProperty<RelationGraph, string?> ValidationErrorProperty = AvaloniaProperty.RegisterDirect<RelationGraph, string?>(nameof(ValidationError), graph => graph.ValidationError);
    public static readonly DirectProperty<RelationGraph, int> NodeCountProperty = AvaloniaProperty.RegisterDirect<RelationGraph, int>(nameof(NodeCount), graph => graph.NodeCount);
    private string? _validationError;
    private int _nodeCount;
    private bool _dirty = true;
    private bool _rebuilding;
    private readonly Dictionary<object, Entry> _entries = new(ReferenceEqualityComparer.Instance);
    private readonly List<Entry> _nodes = [];
    private readonly NodeHost _host;
    private global::Avalonia.Controls.Controls Children => _host.Children;

    public RelationGraph() { _host = new NodeHost(this); VisualChildren.Add(_host); LogicalChildren.Add(_host); }
    public IBrush? Background { get => GetValue(BackgroundProperty); set => SetValue(BackgroundProperty, value); }

    public object? RootNode { get => GetValue(RootNodeProperty); set => SetValue(RootNodeProperty, value); }
    public Orientation Orientation { get => GetValue(OrientationProperty); set => SetValue(OrientationProperty, value); }
    /// <summary>A binding evaluated against each node, not against the graph's data context.</summary>
    [AssignBinding] public BindingBase? ChildrenSource { get => GetValue(ChildrenSourceProperty); set => SetValue(ChildrenSourceProperty, value); }
    [AssignBinding] public BindingBase? IconSource { get => GetValue(IconSourceProperty); set => SetValue(IconSourceProperty, value); }
    [AssignBinding] public BindingBase? TitleSource { get => GetValue(TitleSourceProperty); set => SetValue(TitleSourceProperty, value); }
    [AssignBinding] public BindingBase? ContentSource { get => GetValue(ContentSourceProperty); set => SetValue(ContentSourceProperty, value); }
    public IDataTemplate? IconTemplate { get => GetValue(IconTemplateProperty); set => SetValue(IconTemplateProperty, value); }
    public IDataTemplate? TitleTemplate { get => GetValue(TitleTemplateProperty); set => SetValue(TitleTemplateProperty, value); }
    public IDataTemplate? ContentTemplate { get => GetValue(ContentTemplateProperty); set => SetValue(ContentTemplateProperty, value); }
    public ControlTheme? NodeTheme { get => GetValue(NodeThemeProperty); set => SetValue(NodeThemeProperty, value); }
    public double NodeWidth { get => GetValue(NodeWidthProperty); set => SetValue(NodeWidthProperty, value); }
    public double NodeMinHeight { get => GetValue(NodeMinHeightProperty); set => SetValue(NodeMinHeightProperty, value); }
    public double NodeSpacing { get => GetValue(NodeSpacingProperty); set => SetValue(NodeSpacingProperty, value); }
    public double LevelSpacing { get => GetValue(LevelSpacingProperty); set => SetValue(LevelSpacingProperty, value); }
    public Thickness Padding { get => GetValue(PaddingProperty); set => SetValue(PaddingProperty, value); }
    public IBrush? ConnectorBrush { get => GetValue(ConnectorBrushProperty); set => SetValue(ConnectorBrushProperty, value); }
    public double ConnectorThickness { get => GetValue(ConnectorThicknessProperty); set => SetValue(ConnectorThicknessProperty, value); }
    public double ConnectorCornerRadius { get => GetValue(ConnectorCornerRadiusProperty); set => SetValue(ConnectorCornerRadiusProperty, value); }
    public double PortRadius { get => GetValue(PortRadiusProperty); set => SetValue(PortRadiusProperty, value); }
    /// <summary>Invalid graphs (cycles, reused nodes, non-collection children) are not displayed.</summary>
    public string? ValidationError => _validationError;
    public int NodeCount => _nodeCount;
    /// <summary>Finds a realized native card; returns null before layout or for invalid trees.</summary>
    public RelationGraphNodePresenter? GetNodePresenter(object node) => _nodes.FirstOrDefault(entry => ReferenceEquals(entry.Model, node))?.View;
    /// <summary>Refresh after changing a collection that does not notify.</summary>
    public void RefreshData() { _dirty = true; InvalidateTreeLayout(); InvalidateVisual(); }
    protected override Type StyleKeyOverride => typeof(RelationGraph);
    private static bool Positive(double value) => double.IsFinite(value) && value > 0;
    private static bool NonNegative(double value) => double.IsFinite(value) && value >= 0;
    private void InvalidateTreeLayout() { _host.InvalidateMeasure(); InvalidateMeasure(); }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ChildrenSourceProperty || change.Property == IconSourceProperty || change.Property == TitleSourceProperty || change.Property == ContentSourceProperty)
        {
            foreach (var entry in _entries.Values) entry.Stop();
            RefreshData();
        }
        else if (change.Property == RootNodeProperty) RefreshData();
        else if (change.Property == IconTemplateProperty || change.Property == TitleTemplateProperty || change.Property == ContentTemplateProperty || change.Property == NodeThemeProperty || change.Property == NodeWidthProperty || change.Property == NodeMinHeightProperty)
        {
            foreach (var entry in _entries.Values) Configure(entry.View);
            InvalidateTreeLayout();
        }
        else if (change.Property == OrientationProperty || change.Property == NodeSpacingProperty || change.Property == LevelSpacingProperty || change.Property == PaddingProperty) InvalidateTreeLayout();
        if (change.Property == BackgroundProperty || change.Property == ConnectorBrushProperty || change.Property == ConnectorThicknessProperty || change.Property == ConnectorCornerRadiusProperty || change.Property == PortRadiusProperty) InvalidateVisual();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        RefreshData();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        foreach (var entry in _entries.Values) entry.Stop();
        _dirty = true;
    }

    private void Configure(RelationGraphNodePresenter view)
    {
        view.Width = NodeWidth;
        view.MinHeight = NodeMinHeight;
        view.IconTemplate = IconTemplate;
        view.HeaderTemplate = TitleTemplate;
        view.ContentTemplate = ContentTemplate;
        view.Theme = NodeTheme;
    }

    private void Rebuild()
    {
        _dirty = false; _rebuilding = true;
        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
        _nodes.Clear();
        string? error = null;
        var queue = new Queue<(object Model, Entry? Parent, int Depth)>();
        if (RootNode is { } root) queue.Enqueue((root, null, 0));
        try
        {
            while (queue.TryDequeue(out var next))
            {
                if (!visited.Add(next.Model)) { error = "A tree node cannot be reused or form a cycle."; break; }
                if (!_entries.TryGetValue(next.Model, out var entry))
                {
                    entry = new Entry(this, next.Model);
                    _entries.Add(next.Model, entry);
                }
                Configure(entry.View);
                entry.Start();
                entry.Parent = next.Parent; entry.Depth = next.Depth; entry.Children.Clear();
                entry.View.IsRoot = next.Parent is null;
                next.Parent?.Children.Add(entry);
                _nodes.Add(entry);
                var source = entry.View.GetValue(RelationGraphNodePresenter.ChildrenSourceValueProperty);
                entry.WatchCollection(source as INotifyCollectionChanged);
                if (ChildrenSource is null && next.Model is not RelationGraphNode) { error = "Custom models require ChildrenSource."; break; }
                if (source is null) continue;
                if (source is not IEnumerable children || source is string) { error = "ChildrenSource must resolve to a collection or null."; break; }
                foreach (var child in children)
                {
                    if (child is null) { error = "A tree cannot contain a null child."; break; }
                    queue.Enqueue((child, entry, next.Depth + 1));
                }
                if (error is not null) break;
            }
            foreach (var key in _entries.Keys.Where(key => !visited.Contains(key)).ToArray())
            {
                var removed = _entries[key];
                removed.Stop(); removed.ReleaseContent();
                Children.Remove(removed.View); _entries.Remove(key);
            }
            if (error is not null) _nodes.Clear();
            var displayed = _nodes.Select(entry => entry.View).ToHashSet();
            foreach (var view in Children.ToArray()) if (!displayed.Contains(view)) Children.Remove(view);
            foreach (var entry in _nodes) if (!Children.Contains(entry.View)) Children.Add(entry.View);
            SetAndRaise(ValidationErrorProperty, ref _validationError, error);
            SetAndRaise(NodeCountProperty, ref _nodeCount, _nodes.Count);
        }
        finally { _rebuilding = false; }
    }

    private sealed class Entry(RelationGraph owner, object model)
    {
        public object Model { get; } = model;
        public RelationGraphNodePresenter View { get; } = new() { DataContext = model };
        public Entry? Parent;
        public int Depth;
        public readonly List<Entry> Children = [];
        public double Span;
        public double CrossStart;
        public Rect Layout;
        private bool _active;
        private INotifyCollectionChanged? _collection;
        private readonly List<IDisposable> _bindings = [];

        public void Start()
        {
            if (_active) return;
            _active = true;
            View.PropertyChanged += Changed;
            Bind(RelationGraphNodePresenter.IconProperty, owner.IconSource ?? (Model is RelationGraphNode ? CompiledBinding.Create<RelationGraphNode, object?>(node => node.Icon, source: Model) : null));
            Bind(HeaderedContentControl.HeaderProperty, owner.TitleSource ?? (Model is RelationGraphNode ? CompiledBinding.Create<RelationGraphNode, object?>(node => node.Title, source: Model) : null));
            Bind(ContentControl.ContentProperty, owner.ContentSource ?? (Model is RelationGraphNode ? CompiledBinding.Create<RelationGraphNode, object?>(node => node.Content, source: Model) : null));
            Bind(RelationGraphNodePresenter.ChildrenSourceValueProperty, owner.ChildrenSource ?? (Model is RelationGraphNode ? CompiledBinding.Create<RelationGraphNode, IList<RelationGraphNode>?>(node => node.Children, source: Model) : null));
        }
        private void Bind(AvaloniaProperty property, BindingBase? binding)
        {
            if (binding is not null) _bindings.Add(View.Bind(property, binding));
            else View.SetCurrentValue(property, null);
        }
        private void Changed(object? sender, AvaloniaPropertyChangedEventArgs change)
        {
            if (change.Property == RelationGraphNodePresenter.ChildrenSourceValueProperty && !owner._rebuilding) owner.RefreshData();
        }
        public void WatchCollection(INotifyCollectionChanged? collection)
        {
            if (ReferenceEquals(_collection, collection)) return;
            if (_collection is not null) _collection.CollectionChanged -= CollectionChanged;
            _collection = collection;
            if (_collection is not null) _collection.CollectionChanged += CollectionChanged;
        }
        private void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => owner.RefreshData();
        public void Stop()
        {
            _active = false;
            View.PropertyChanged -= Changed;
            WatchCollection(null);
            foreach (var binding in _bindings) binding.Dispose();
            _bindings.Clear();
        }
        public void ReleaseContent() { View.Icon = null; View.Header = null; View.Content = null; View.DataContext = null; }
    }

    // Keep the whole tree layout inside a native parent layout pass. Even when a content
    // change preserves the graph's total extent, the host must rearrange its descendants.
    private sealed class NodeHost(RelationGraph owner) : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            if (owner._dirty) owner.Rebuild();
            foreach (var child in Children) child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return owner.MeasureTree();
        }
        protected override Size ArrangeOverride(Size finalSize) => owner.ArrangeTree(finalSize);
    }
}
