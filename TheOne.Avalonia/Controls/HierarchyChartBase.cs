using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Diagnostics;
using Avalonia.Input;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>Observable hierarchy projection shared by native treemap and sunburst charts.</summary>
public abstract class HierarchyChartBase : SpecializedChartBase
{
    public static readonly StyledProperty<IList<ChartHierarchyNode>?> ItemsSourceProperty =
        AvaloniaProperty.Register<HierarchyChartBase, IList<ChartHierarchyNode>?>(nameof(ItemsSource));
    public static readonly StyledProperty<ChartHierarchyNode?> RootProperty =
        AvaloniaProperty.Register<HierarchyChartBase, ChartHierarchyNode?>(nameof(Root),
            defaultBindingMode: BindingMode.TwoWay);
    public static readonly DirectProperty<HierarchyChartBase, ChartHierarchyNode?> SelectedNodeProperty =
        AvaloniaProperty.RegisterDirect<HierarchyChartBase, ChartHierarchyNode?>(nameof(SelectedNode), chart => chart.SelectedNode);
    public static readonly DirectProperty<HierarchyChartBase, bool> CanDrillUpProperty =
        AvaloniaProperty.RegisterDirect<HierarchyChartBase, bool>(nameof(CanDrillUp), chart => chart.CanDrillUp);
    public static readonly DirectProperty<HierarchyChartBase, bool> HasInvalidHierarchyProperty =
        AvaloniaProperty.RegisterDirect<HierarchyChartBase, bool>(nameof(HasInvalidHierarchy), chart => chart.HasInvalidHierarchy);

    private readonly Dictionary<ChartHierarchyNode, HierarchyNodeState> _states = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<string, ChartHierarchyNode?> _keys = new(StringComparer.Ordinal);
    private readonly HashSet<ChartHierarchyNode> _observedNodes = new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<INotifyCollectionChanged> _observedCollections = new(ReferenceEqualityComparer.Instance);
    private readonly List<HierarchyNodeState> _order = [];
    private readonly List<HierarchyNodeState> _roots = [];
    private const int MaximumCachedNodeLabels = 16_384;
    private readonly Dictionary<ChartHierarchyNode, NodeLabel> _nodeLabels = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<(Color Color, double Opacity), IBrush> _contrastBrushes = [];
    private readonly HashSet<SolidColorBrush> _usedLabelBrushes = new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<SolidColorBrush> _observedLabelBrushes = new(ReferenceEqualityComparer.Instance);
    private LabelContext? _labelContext;
    private bool _automaticLabelBrush;
    private bool? _snapshotAutomaticLabelBrush;
    private readonly ChartSeries _projection = new() { Name = "Hierarchy" };
    private ChartHierarchyNode? _selectedNode;
    private bool _canDrillUp, _invalidHierarchy, _invalidEdges;
    private bool _reconciling, _settingProjection, _observingHierarchy;

    static HierarchyChartBase()
    {
        ShowLegendProperty.OverrideDefaultValue<HierarchyChartBase>(false);
        ShowDataLabelsProperty.OverrideDefaultValue<HierarchyChartBase>(true);
    }

    protected HierarchyChartBase()
    {
        InstallProjection();
        ItemsSource = new ObservableCollection<ChartHierarchyNode>();
        SelectionChanged += (_, args) =>
            SetAndRaise(SelectedNodeProperty, ref _selectedNode, args.Selection?.Point as ChartHierarchyNode);
        DoubleTapped += OnNodeDoubleTapped;
    }

    /// <summary>The authoritative forest. Do not assign the inherited SeriesSource projection.</summary>
    public IList<ChartHierarchyNode>? ItemsSource { get => GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }
    /// <summary>The displayed branch; null displays the complete forest. Unreachable roots reset to null.</summary>
    public ChartHierarchyNode? Root { get => GetValue(RootProperty); set => SetValue(RootProperty, value); }
    public ChartHierarchyNode? SelectedNode => _selectedNode;
    public bool CanDrillUp => _canDrillUp;
    /// <summary>True when duplicate keys, repeated nodes or cycles were encountered.</summary>
    public bool HasInvalidHierarchy => _invalidHierarchy;

    /// <summary>Show a reachable branch's children. Leaves and branches without positive weights are rejected.</summary>
    public bool DrillDown(ChartHierarchyNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        if (!_states.TryGetValue(node, out var state) || !state.Children.Any(child => double.IsFinite(child.LogWeight))) return false;
        SetCurrentValue(RootProperty, node);
        return true;
    }

    public bool DrillUp()
    {
        if (Root is not { } root) return false;
        SetCurrentValue(RootProperty, _states.TryGetValue(root, out var state) ? state.Parent?.Node : null);
        return true;
    }

    public void ResetRoot() => SetCurrentValue(RootProperty, null);

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _observingHierarchy = true;
        ReconcileHierarchy();
        base.OnAttachedToVisualTree(e);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _observingHierarchy = false;
        ReconcileObservers();
        _nodeLabels.Clear();
        _contrastBrushes.Clear();
        _usedLabelBrushes.Clear();
        ReconcileLabelBrushObservers();
        _labelContext = null;
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ItemsSourceProperty) ReconcileHierarchy();
        else if (change.Property == RootProperty && !_reconciling)
        {
            if (Root is { } root && (!_states.TryGetValue(root, out var state) || state.Children.Count == 0))
                SetCurrentValue(RootProperty, null);
            SetAndRaise(CanDrillUpProperty, ref _canDrillUp, Root is not null);
            InvalidateChartGeometry();
        }
        else if (change.Property == SeriesSourceProperty && !_settingProjection) InstallProjection();
    }

    private void InstallProjection()
    {
        _settingProjection = true;
        try { SetCurrentValue(SeriesSourceProperty, new[] { _projection }); }
        finally { _settingProjection = false; }
    }

    private protected override void OnRefreshChartData() => ReconcileHierarchy();

    private void ReconcileHierarchy()
    {
        if (_reconciling) return;
        _reconciling = true;
        try
        {
            var selected = Selection?.Point as ChartHierarchyNode;
            var selectedKey = selected?.Key is { } key && _keys.TryGetValue(key, out var unique) && ReferenceEquals(unique, selected)
                ? key : null;
            var previousRoot = Root;
            var visited = new HashSet<ChartHierarchyNode>(ReferenceEqualityComparer.Instance);
            var nextOrder = new List<HierarchyNodeState>();
            var pending = new Stack<(ChartHierarchyNode Node, HierarchyNodeState? Parent, int Palette)>();
            _roots.Clear();
            _invalidEdges = false;
            if (ItemsSource is { } source)
                for (var index = source.Count - 1; index >= 0; index--)
                    if (source[index] is { } node) pending.Push((node, null, index));
            while (pending.Count > 0)
            {
                var (node, parent, palette) = pending.Pop();
                if (!visited.Add(node)) { _invalidEdges = true; continue; }
                if (!_states.TryGetValue(node, out var state)) state = new HierarchyNodeState(node);
                state.Parent = parent;
                state.Palette = palette;
                state.Index = nextOrder.Count;
                state.Depth = parent is null ? 0 : parent.Depth + 1;
                state.Children.Clear();
                state.HasChildren = node.Children is { Count: > 0 };
                nextOrder.Add(state);
                if (parent is null) _roots.Add(state); else parent.Children.Add(state);
                if (node.Children is not { } children) continue;
                for (var index = children.Count - 1; index >= 0; index--)
                    if (children[index] is { } child) pending.Push((child, state, palette));
            }
            foreach (var removed in _states.Keys.Where(node => !visited.Contains(node)).ToArray()) _states.Remove(removed);
            foreach (var removed in _nodeLabels.Keys.Where(node => !visited.Contains(node)).ToArray()) _nodeLabels.Remove(removed);
            _order.Clear();
            _order.AddRange(nextOrder);
            for (var index = _order.Count - 1; index >= 0; index--)
            {
                var state = _order[index];
                _states[state.Node] = state;
                RecalculateWeight(state);
                state.EndIndex = state.Children.Count == 0 ? state.Index : state.Children[^1].EndIndex;
            }
            RebuildKeys();
            ReconcileObservers();
            var nextRoot = previousRoot is not null && _states.TryGetValue(previousRoot, out var rootState) && rootState.Children.Count > 0
                ? previousRoot : null;
            SetCurrentValue(RootProperty, nextRoot);
            SetAndRaise(CanDrillUpProperty, ref _canDrillUp, nextRoot is not null);
            var nextSelected = selected is not null && _states.ContainsKey(selected) ? selected : FindUniqueKey(selectedKey);
            var nextIndex = nextSelected is not null && IsVisibleState(_states[nextSelected]) ? _states[nextSelected].Index : -1;
            // Clear only when identity/ordinal changes, before replacing the projection. An unchanged
            // refresh therefore does not emit a temporary deselection or point at a different node.
            if (nextIndex != SelectedIndex || !ReferenceEquals(selected, nextSelected)) SetCurrentValue(SelectedIndexProperty, -1);
            _projection.ItemsSource = _order.Select(state => (ChartPoint)state.Node).ToArray();
            if (nextIndex >= 0)
            {
                SetCurrentValue(SelectedSeriesIndexProperty, 0);
                SetCurrentValue(SelectedIndexProperty, nextIndex);
            }
            NotifyChartDataChanged();
        }
        finally { _reconciling = false; }
    }

    private void RebuildKeys()
    {
        _keys.Clear();
        var duplicates = false;
        foreach (var state in _order)
            if (!string.IsNullOrEmpty(state.Node.Key))
            {
                if (!_keys.TryAdd(state.Node.Key, state.Node)) { _keys[state.Node.Key] = null; duplicates = true; }
            }
        SetAndRaise(HasInvalidHierarchyProperty, ref _invalidHierarchy, _invalidEdges || duplicates);
    }

    private ChartHierarchyNode? FindUniqueKey(string? key) =>
        !string.IsNullOrEmpty(key) && _keys.TryGetValue(key, out var node) ? node : null;

    private void ReconcileObservers()
    {
        var nodes = _observingHierarchy ? new HashSet<ChartHierarchyNode>(_states.Keys, ReferenceEqualityComparer.Instance) : [];
        var collections = new HashSet<INotifyCollectionChanged>(ReferenceEqualityComparer.Instance);
        if (_observingHierarchy)
        {
            if (ItemsSource is INotifyCollectionChanged roots) collections.Add(roots);
            foreach (var node in nodes)
                if (node.Children is INotifyCollectionChanged children) collections.Add(children);
        }
        foreach (var node in _observedNodes.ToArray())
            if (!nodes.Contains(node)) { node.PropertyChanged -= OnNodeChanged; _observedNodes.Remove(node); }
        foreach (var node in nodes)
            if (_observedNodes.Add(node)) node.PropertyChanged += OnNodeChanged;
        foreach (var collection in _observedCollections.ToArray())
            if (!collections.Contains(collection)) { collection.CollectionChanged -= OnChildrenChanged; _observedCollections.Remove(collection); }
        foreach (var collection in collections)
            if (_observedCollections.Add(collection)) collection.CollectionChanged += OnChildrenChanged;
    }

    private void OnChildrenChanged(object? sender, NotifyCollectionChangedEventArgs args) => ReconcileHierarchy();

    private void OnNodeChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (sender is not ChartHierarchyNode node || !_states.TryGetValue(node, out var state)) return;
        if (args.Property == ChartHierarchyNode.ChildrenProperty) ReconcileHierarchy();
        else if (args.Property == ChartHierarchyNode.KeyProperty) RebuildKeys();
        else if (args.Property == ChartPoint.YProperty)
        {
            // Retain the flattened projection and every subscription. Only the changed ancestor path is visited.
            for (var current = state; current is not null; current = current.Parent) RecalculateWeight(current);
        }
    }

    private static void RecalculateWeight(HierarchyNodeState state)
    {
        state.LogWeight = state.HasChildren ? TotalLogWeight(state.Children) :
            double.IsFinite(state.Node.Y) && state.Node.Y > 0 ? Math.Log(state.Node.Y) : double.NegativeInfinity;
    }

    private protected static double TotalLogWeight(IReadOnlyList<HierarchyNodeState> children)
    {
        var maximum = double.NegativeInfinity;
        foreach (var child in children) maximum = Math.Max(maximum, child.LogWeight);
        if (!double.IsFinite(maximum)) return double.NegativeInfinity;
        var sum = 0d;
        foreach (var child in children)
            if (double.IsFinite(child.LogWeight)) sum += Math.Exp(child.LogWeight - maximum);
        return maximum + Math.Log(sum);
    }

    private protected IReadOnlyList<HierarchyNodeState> DisplayRoots =>
        Root is { } root && _states.TryGetValue(root, out var state) ? state.Children : _roots;

    private protected bool IsVisibleState(HierarchyNodeState state)
    {
        if (!double.IsFinite(state.LogWeight)) return false;
        if (Root is not { } root || !_states.TryGetValue(root, out var parent)) return true;
        return state.Index > parent.Index && state.Index <= parent.EndIndex;
    }

    private protected override bool IsValidDataPoint(ChartPoint point) =>
        point is ChartHierarchyNode node && _states.TryGetValue(node, out var state) && IsVisibleState(state);

    private protected override string? DescribeSpecializedPoint(ChartSelection selection)
    {
        if (selection.Point is not ChartHierarchyNode node || !_states.TryGetValue(node, out var state)) return null;
        var value = state.LogWeight > Math.Log(double.MaxValue) ? $">{FormatValue(double.MaxValue)}" : FormatValue(Math.Exp(state.LogWeight));
        return $"{node.Label ?? node.Key ?? "Node"}: {value}{Unit}";
    }

    private protected IBrush HierarchyBrush(HierarchyNodeState state)
    {
        for (var current = state; current is not null; current = current.Parent)
            if (current.Node.Brush is { } brush) return brush;
        return GetChartBrush(0, state.Palette);
    }

    private protected LabelPreparation PrepareHierarchyLabels()
    {
        _usedLabelBrushes.Clear();
        var solid = Foreground as ISolidColorBrush;
        var background = Background as ISolidColorBrush;
        _automaticLabelBrush = _snapshotAutomaticLabelBrush ?? UsesAutomaticLabelBrush();
        var context = new LabelContext(FontFamily, FontSize, FlowDirection, Foreground, solid?.Color,
            Foreground?.Opacity ?? 1, ActualThemeVariant, CultureInfo.CurrentCulture,
            _automaticLabelBrush, background?.Color, background?.Opacity ?? 1);
        if (_labelContext is not { } previous || previous != context)
        {
            _nodeLabels.Clear();
            _contrastBrushes.Clear();
            _labelContext = context;
        }
        if (_automaticLabelBrush && Background is SolidColorBrush mutableBackground) _usedLabelBrushes.Add(mutableBackground);
        return new LabelPreparation(this);
    }

    private bool UsesAutomaticLabelBrush()
    {
        var diagnostic = this.GetDiagnostic(ForegroundProperty);
        var explicitForeground = diagnostic.Priority <= BindingPriority.LocalValue || diagnostic.IsOverriddenCurrentValue;
        var themeForeground = this.TryFindResource("One.Chart.Text", ActualThemeVariant, out var resource) ? resource as IBrush : null;
        return !explicitForeground && (Foreground is null || ReferenceEquals(Foreground, themeForeground));
    }

    private protected FormattedText HierarchyLabel(HierarchyNodeState state, IBrush fill)
    {
        var label = state.Node.Label ?? string.Empty;
        var brush = HierarchyLabelBrush(fill);
        if (_nodeLabels.TryGetValue(state.Node, out var cached) && string.Equals(cached.Value, label, StringComparison.Ordinal) &&
            ReferenceEquals(cached.Brush, brush))
            return cached.Text;
        var text = Text(label, brush);
        if (_nodeLabels.Count < MaximumCachedNodeLabels || _nodeLabels.ContainsKey(state.Node))
            _nodeLabels[state.Node] = new NodeLabel(label, brush, text);
        return text;
    }

    private IBrush HierarchyLabelBrush(IBrush fill)
    {
        if (!_automaticLabelBrush || fill is not ISolidColorBrush solid) return Foreground ?? Brushes.Black;
        if (fill is SolidColorBrush mutableFill) _usedLabelBrushes.Add(mutableFill);
        var key = (solid.Color, solid.Opacity);
        if (_contrastBrushes.TryGetValue(key, out var cached)) return cached;
        var alpha = solid.Color.A / 255d * solid.Opacity;
        var backgroundBrush = Background as ISolidColorBrush;
        var background = backgroundBrush?.Color;
        if (alpha < 1 && (background is null || background.Value.A < 255 || backgroundBrush!.Opacity < 1))
            return Foreground ?? Brushes.Black;
        var red = solid.Color.R / 255d * alpha + (background?.R ?? 0) / 255d * (1 - alpha);
        var green = solid.Color.G / 255d * alpha + (background?.G ?? 0) / 255d * (1 - alpha);
        var blue = solid.Color.B / 255d * alpha + (background?.B ?? 0) / 255d * (1 - alpha);
        var luminance = .2126 * Linear(red) + .7152 * Linear(green) + .0722 * Linear(blue);
        var brush = luminance > .1791287847 ? Brushes.Black : Brushes.White;
        if (_contrastBrushes.Count < MaximumCachedNodeLabels) _contrastBrushes[key] = brush;
        return brush;
    }

    private void ReconcileLabelBrushObservers()
    {
        foreach (var brush in _observedLabelBrushes.ToArray())
            if (!_observingHierarchy || !_usedLabelBrushes.Contains(brush))
            {
                brush.PropertyChanged -= OnLabelBrushChanged;
                _observedLabelBrushes.Remove(brush);
            }
        if (!_observingHierarchy) return;
        foreach (var brush in _usedLabelBrushes)
            if (_observedLabelBrushes.Add(brush)) brush.PropertyChanged += OnLabelBrushChanged;
    }

    private void OnLabelBrushChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property == SolidColorBrush.ColorProperty || args.Property == Brush.OpacityProperty)
            InvalidateChartGeometry();
    }

    private static double Linear(double value) => value <= .04045 ? value / 12.92 : Math.Pow((value + .055) / 1.055, 2.4);

    private void OnNodeDoubleTapped(object? sender, TappedEventArgs args)
    {
        if (HitTestDataPoint(args.GetPosition(this))?.Point is ChartHierarchyNode node && DrillDown(node)) args.Handled = true;
    }

    private protected override void CopyExportProperties(ChartBase snapshot)
    {
        base.CopyExportProperties(snapshot);
        var target = (HierarchyChartBase)snapshot;
        // Export copies effective properties as local values. Preserve the source's paint policy rather
        // than mistaking that implementation detail for an explicit consumer foreground override.
        target._snapshotAutomaticLabelBrush = _snapshotAutomaticLabelBrush ?? UsesAutomaticLabelBrush();
        var clones = new Dictionary<ChartHierarchyNode, ChartHierarchyNode>(ReferenceEqualityComparer.Instance);
        foreach (var state in _order)
            clones[state.Node] = new ChartHierarchyNode(state.Node.Label,
                state.HasChildren && state.Children.Count == 0 ? 0 : state.Node.Y, state.Node.Key)
                { X = state.Node.X, Brush = state.Node.Brush };
        foreach (var state in _order) clones[state.Node].Children = state.Children.Select(child => clones[child.Node]).ToArray();
        target.ItemsSource = _roots.Select(state => clones[state.Node]).ToArray();
        target.Root = Root is { } root && clones.TryGetValue(root, out var clone) ? clone : null;
    }

    private protected sealed class HierarchyNodeState(ChartHierarchyNode node)
    {
        public ChartHierarchyNode Node { get; } = node;
        public HierarchyNodeState? Parent { get; set; }
        public List<HierarchyNodeState> Children { get; } = [];
        public bool HasChildren { get; set; }
        public int Index { get; set; }
        public int EndIndex { get; set; }
        public int Depth { get; set; }
        public int Palette { get; set; }
        public double LogWeight { get; set; } = double.NegativeInfinity;
    }

    private readonly record struct NodeLabel(string Value, IBrush Brush, FormattedText Text);
    private readonly record struct LabelContext(FontFamily Family, double Size, FlowDirection Flow,
        IBrush? Foreground, Color? Color, double Opacity, object Theme, CultureInfo Culture,
        bool AutomaticBrush, Color? Background, double BackgroundOpacity);

    private protected readonly struct LabelPreparation(HierarchyChartBase chart) : IDisposable
    {
        public void Dispose() => chart.ReconcileLabelBrushObservers();
    }
}
