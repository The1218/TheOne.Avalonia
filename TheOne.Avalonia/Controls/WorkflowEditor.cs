using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using TheOne.Avalonia.Workflow;

namespace TheOne.Avalonia.Controls;

/// <summary>Template-based graph editing. Persistence, execution and business validation belong to the host.</summary>
public class WorkflowEditor : TemplatedControl
{
    public static readonly StyledProperty<WorkflowDocument?> DocumentProperty =
        AvaloniaProperty.Register<WorkflowEditor, WorkflowDocument?>(nameof(Document),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<IEnumerable<WorkflowNodeDefinition>?> NodeDefinitionsProperty =
        AvaloniaProperty.Register<WorkflowEditor, IEnumerable<WorkflowNodeDefinition>?>(nameof(NodeDefinitions));

    public static readonly StyledProperty<WorkflowNode?> SelectedNodeProperty =
        AvaloniaProperty.Register<WorkflowEditor, WorkflowNode?>(nameof(SelectedNode),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<WorkflowConnection?> SelectedConnectionProperty =
        AvaloniaProperty.Register<WorkflowEditor, WorkflowConnection?>(nameof(SelectedConnection),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<IDataTemplate?> NodeTemplateProperty =
        AvaloniaProperty.Register<WorkflowEditor, IDataTemplate?>(nameof(NodeTemplate));

    public static readonly StyledProperty<IDataTemplate?> ContentTemplateProperty =
        AvaloniaProperty.Register<WorkflowEditor, IDataTemplate?>(nameof(ContentTemplate));

    public static readonly StyledProperty<IDataTemplate?> PropertyTemplateProperty =
        AvaloniaProperty.Register<WorkflowEditor, IDataTemplate?>(nameof(PropertyTemplate));

    public static readonly StyledProperty<IDataTemplate?> NodeDefinitionTemplateProperty =
        AvaloniaProperty.Register<WorkflowEditor, IDataTemplate?>(nameof(NodeDefinitionTemplate));

    public static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty =
        AvaloniaProperty.Register<WorkflowEditor, IDataTemplate?>(nameof(HeaderTemplate));

    public static readonly StyledProperty<ICommand?> SaveCommandProperty =
        AvaloniaProperty.Register<WorkflowEditor, ICommand?>(nameof(SaveCommand));

    public static readonly DirectProperty<WorkflowEditor, WorkflowNodeEditor?> SelectedNodeEditorProperty =
        AvaloniaProperty.RegisterDirect<WorkflowEditor, WorkflowNodeEditor?>(nameof(SelectedNodeEditor),
            c => c.SelectedNodeEditor);

    public static readonly StyledProperty<string> SearchTextProperty =
        AvaloniaProperty.Register<WorkflowEditor, string>(nameof(SearchText), "");

    public static readonly StyledProperty<bool> IsPanModeProperty =
        AvaloniaProperty.Register<WorkflowEditor, bool>(nameof(IsPanMode));

    public static readonly StyledProperty<bool> ShowRuntimePreviewProperty =
        AvaloniaProperty.Register<WorkflowEditor, bool>(nameof(ShowRuntimePreview));

    public static readonly StyledProperty<bool> ShowValidationProperty =
        AvaloniaProperty.Register<WorkflowEditor, bool>(nameof(ShowValidation));

    public static readonly DirectProperty<WorkflowEditor, IReadOnlyList<WorkflowNodeGroup>> LibraryGroupsProperty =
        AvaloniaProperty.RegisterDirect<WorkflowEditor, IReadOnlyList<WorkflowNodeGroup>>(nameof(LibraryGroups),
            c => c.LibraryGroups);

    public static readonly DirectProperty<WorkflowEditor, string> GraphSummaryProperty =
        AvaloniaProperty.RegisterDirect<WorkflowEditor, string>(nameof(GraphSummary), c => c.GraphSummary);

    public static readonly DirectProperty<WorkflowEditor, string> ValidationSummaryProperty =
        AvaloniaProperty.RegisterDirect<WorkflowEditor, string>(nameof(ValidationSummary), c => c.ValidationSummary);

    public static readonly DirectProperty<WorkflowEditor, IReadOnlyList<WorkflowValidationIssue>>
        ValidationIssuesProperty =
            AvaloniaProperty.RegisterDirect<WorkflowEditor, IReadOnlyList<WorkflowValidationIssue>>(
                nameof(ValidationIssues), c => c.ValidationIssues);

    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<WorkflowEditor, bool>(nameof(IsReadOnly));

    public static readonly StyledProperty<bool> ShowNodeLibraryProperty =
        AvaloniaProperty.Register<WorkflowEditor, bool>(nameof(ShowNodeLibrary), true);

    public static readonly StyledProperty<bool> ShowPropertiesProperty =
        AvaloniaProperty.Register<WorkflowEditor, bool>(nameof(ShowProperties), true);

    public static readonly StyledProperty<bool> AllowCyclesProperty =
        AvaloniaProperty.Register<WorkflowEditor, bool>(nameof(AllowCycles));

    public static readonly StyledProperty<bool> SnapToGridProperty =
        AvaloniaProperty.Register<WorkflowEditor, bool>(nameof(SnapToGrid), true);

    public static readonly StyledProperty<double> ZoomProperty =
        AvaloniaProperty.Register<WorkflowEditor, double>(nameof(Zoom), 1, defaultBindingMode: BindingMode.TwoWay,
            validate: x => double.IsFinite(x) && x >= .25 && x <= 2);

    public static readonly StyledProperty<Vector> PanProperty =
        AvaloniaProperty.Register<WorkflowEditor, Vector>(nameof(Pan), new Vector(24, 24),
            defaultBindingMode: BindingMode.TwoWay, validate: x => double.IsFinite(x.X) && double.IsFinite(x.Y));

    public static readonly StyledProperty<Thickness> FitPaddingProperty =
        AvaloniaProperty.Register<WorkflowEditor, Thickness>(nameof(FitPadding), new Thickness(32),
            validate: p => new[] { p.Left, p.Top, p.Right, p.Bottom }.All(x => double.IsFinite(x) && x >= 0));

    public static readonly DirectProperty<WorkflowEditor, string> StatusProperty =
        AvaloniaProperty.RegisterDirect<WorkflowEditor, string>(nameof(Status), c => c.Status);

    public static readonly DirectProperty<WorkflowEditor, string> ZoomTextProperty =
        AvaloniaProperty.RegisterDirect<WorkflowEditor, string>(nameof(ZoomText), c => c.ZoomText);

    private readonly ObservableCollection<WorkflowNode> _selection = [];
    private bool _attached;
    private WorkflowCanvas? _canvas;
    private string? _clipboard;
    private INotifyCollectionChanged? _definitionsObserver;
    private string _graphSummary = "0 个节点  |  0 条连线", _validationSummary = "校验通过";
    private IReadOnlyList<WorkflowNodeGroup> _groups = [];
    private IReadOnlyList<WorkflowValidationIssue> _issues = [];
    private WorkflowDocument? _observed;
    private WorkflowNodeDefinition? _paletteDefinition;
    private bool _paletteDragging;
    private Point _paletteOrigin;
    private WorkflowNodeEditor? _selectedNodeEditor;
    private string _status = "从节点库添加节点，或加载已有流程", _zoomText = "100%";

    public WorkflowEditor()
    {
        Focusable = true;
        SelectedNodes = new ReadOnlyObservableCollection<WorkflowNode>(_selection);
        AddNodeCommand = new EditorCommand(p =>
        {
            if (p is WorkflowNodeDefinition d)
                AddNode(d,
                    _canvas?.ToWorld(new Point(_canvas.Bounds.Width / 2, _canvas.Bounds.Height / 2)) ??
                    new Point(40, 40));
        }, _ => !IsReadOnly && Document is not null);
        UndoCommand = new EditorCommand(_ => Document?.Undo(), _ => !IsReadOnly && Document?.CanUndo == true);
        RedoCommand = new EditorCommand(_ => Document?.Redo(), _ => !IsReadOnly && Document?.CanRedo == true);
        DeleteCommand = new EditorCommand(_ => DeleteSelection(),
            _ => !IsReadOnly && (_selection.Count > 0 || SelectedConnection != null));
        FitCommand = new EditorCommand(_ => FitToView());
        ZoomInCommand = new EditorCommand(_ => SetCurrentValue(ZoomProperty, Math.Min(2, Zoom + .1)));
        ZoomOutCommand = new EditorCommand(_ => SetCurrentValue(ZoomProperty, Math.Max(.25, Zoom - .1)));
        ValidateCommand = new EditorCommand(_ =>
        {
            RefreshValidation();
            SetCurrentValue(ShowValidationProperty, !ShowValidation);
            Report(_issues.Count == 0 ? "结构与字段校验通过" : $"{_issues.Count} 项待处理：{_issues[0].Message}");
        });
        ArrangeCommand = new EditorCommand(_ => ArrangeNodes(), _ => !IsReadOnly && Document?.Nodes.Count > 0);
        AlignCommand = new EditorCommand(_ =>
        {
            if (Document != null && _selection.Count > 1)
            {
                var top = _selection.Min(n => n.Y);
                Document.MoveNodes(_selection.Select(n => (n, n.X, top)));
            }
        }, _ => !IsReadOnly && _selection.Count > 1);
        ToggleLibraryCommand = new EditorCommand(_ => SetCurrentValue(ShowNodeLibraryProperty, !ShowNodeLibrary));
        TogglePropertiesCommand = new EditorCommand(_ => SetCurrentValue(ShowPropertiesProperty, !ShowProperties));
        LocateIssueCommand = new EditorCommand(p =>
        {
            if (p is WorkflowValidationIssue i)
            {
                var nodeId = i.NodeId ??
                             Document?.Connections.FirstOrDefault(c => c.Id == i.ConnectionId)?.TargetNodeId;
                Locate(nodeId);
            }
        });
        AddHandler(PointerPressedEvent, PalettePressed, RoutingStrategies.Tunnel);
        AddHandler(PointerMovedEvent, PaletteMoved, RoutingStrategies.Tunnel);
        AddHandler(PointerReleasedEvent, PaletteReleased, RoutingStrategies.Tunnel);
    }

    public ReadOnlyObservableCollection<WorkflowNode> SelectedNodes { get; }

    public WorkflowDocument? Document
    {
        get => GetValue(DocumentProperty);
        set => SetValue(DocumentProperty, value);
    }

    public IEnumerable<WorkflowNodeDefinition>? NodeDefinitions
    {
        get => GetValue(NodeDefinitionsProperty);
        set => SetValue(NodeDefinitionsProperty, value);
    }

    public WorkflowNode? SelectedNode
    {
        get => GetValue(SelectedNodeProperty);
        set => SetValue(SelectedNodeProperty, value);
    }

    public WorkflowConnection? SelectedConnection
    {
        get => GetValue(SelectedConnectionProperty);
        set => SetValue(SelectedConnectionProperty, value);
    }

    public IDataTemplate? NodeTemplate
    {
        get => GetValue(NodeTemplateProperty);
        set => SetValue(NodeTemplateProperty, value);
    }

    public IDataTemplate? ContentTemplate
    {
        get => GetValue(ContentTemplateProperty);
        set => SetValue(ContentTemplateProperty, value);
    }

    public IDataTemplate? PropertyTemplate
    {
        get => GetValue(PropertyTemplateProperty);
        set => SetValue(PropertyTemplateProperty, value);
    }

    public IDataTemplate? NodeDefinitionTemplate
    {
        get => GetValue(NodeDefinitionTemplateProperty);
        set => SetValue(NodeDefinitionTemplateProperty, value);
    }

    /// <summary>Header content receives this editor. The default exposes document title, validation and SaveCommand.</summary>
    public IDataTemplate? HeaderTemplate
    {
        get => GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    /// <summary>Host-owned persistence command. The default save button passes Document as its parameter.</summary>
    public ICommand? SaveCommand
    {
        get => GetValue(SaveCommandProperty);
        set => SetValue(SaveCommandProperty, value);
    }

    public WorkflowNodeEditor? SelectedNodeEditor => _selectedNodeEditor;

    public string SearchText
    {
        get => GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    public bool IsPanMode
    {
        get => GetValue(IsPanModeProperty);
        set => SetValue(IsPanModeProperty, value);
    }

    public bool ShowRuntimePreview
    {
        get => GetValue(ShowRuntimePreviewProperty);
        set => SetValue(ShowRuntimePreviewProperty, value);
    }

    public bool ShowValidation
    {
        get => GetValue(ShowValidationProperty);
        set => SetValue(ShowValidationProperty, value);
    }

    public IReadOnlyList<WorkflowNodeGroup> LibraryGroups => _groups;
    public IReadOnlyList<WorkflowValidationIssue> ValidationIssues => _issues;
    public string GraphSummary => _graphSummary;
    public string ValidationSummary => _validationSummary;

    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    public bool ShowNodeLibrary
    {
        get => GetValue(ShowNodeLibraryProperty);
        set => SetValue(ShowNodeLibraryProperty, value);
    }

    public bool ShowProperties
    {
        get => GetValue(ShowPropertiesProperty);
        set => SetValue(ShowPropertiesProperty, value);
    }

    public bool AllowCycles
    {
        get => GetValue(AllowCyclesProperty);
        set => SetValue(AllowCyclesProperty, value);
    }

    public bool SnapToGrid
    {
        get => GetValue(SnapToGridProperty);
        set => SetValue(SnapToGridProperty, value);
    }

    public double Zoom
    {
        get => GetValue(ZoomProperty);
        set => SetValue(ZoomProperty, value);
    }

    public Vector Pan
    {
        get => GetValue(PanProperty);
        set => SetValue(PanProperty, value);
    }

    public Thickness FitPadding
    {
        get => GetValue(FitPaddingProperty);
        set => SetValue(FitPaddingProperty, value);
    }

    public string Status => _status;
    public string ZoomText => _zoomText;
    public ICommand AddNodeCommand { get; }
    public ICommand UndoCommand { get; }
    public ICommand RedoCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand FitCommand { get; }
    public ICommand ZoomInCommand { get; }
    public ICommand ZoomOutCommand { get; }
    public ICommand ValidateCommand { get; }
    public ICommand ArrangeCommand { get; }
    public ICommand AlignCommand { get; }
    public ICommand ToggleLibraryCommand { get; }
    public ICommand TogglePropertiesCommand { get; }
    public ICommand LocateIssueCommand { get; }

    /// <summary>Host can reject business-specific connections in addition to structural checks.</summary>
    public Func<WorkflowNode, WorkflowPort, WorkflowNode, WorkflowPort, string?>? ConnectionValidator { get; set; }

    internal Size ViewportSize => _canvas?.Bounds.Size ?? default;
    internal event EventHandler? CanvasLayoutChanged;
    internal void NotifyCanvasLayoutChanged() => CanvasLayoutChanged?.Invoke(this, EventArgs.Empty);
    public event EventHandler? DocumentEdited;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _canvas?.SetOwner(null);
        base.OnApplyTemplate(e);
        _canvas = e.NameScope.Find<WorkflowCanvas>("PART_Canvas");
        _canvas?.SetOwner(this);
        Observe();
        ObserveDefinitions();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _attached = true;
        Observe();
        ObserveDefinitions();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _attached = false;
        if (_observed != null) _observed.Changed -= DocumentChanged;
        _observed = null;
        if (_definitionsObserver != null) _definitionsObserver.CollectionChanged -= DefinitionsChanged;
        _definitionsObserver = null;
        _canvas?.CancelGesture();
        _paletteDefinition = null;
        base.OnDetachedFromVisualTree(e);
    }

    private void Observe()
    {
        if (_observed != null) _observed.Changed -= DocumentChanged;
        _observed = _attached ? Document : null;
        if (_observed != null) _observed.Changed += DocumentChanged;
        Refresh();
    }

    private void DocumentChanged(object? sender, EventArgs e)
    {
        Refresh();
        DocumentEdited?.Invoke(this, EventArgs.Empty);
    }

    private void Refresh()
    {
        foreach (var n in _selection.Where(n => Document?.ContainsNode(n) != true).ToArray()) _selection.Remove(n);
        if (SelectedNode != null && Document?.ContainsNode(SelectedNode) != true)
            SetCurrentValue(SelectedNodeProperty, null);
        if (SelectedConnection != null && Document?.ContainsConnection(SelectedConnection) != true)
            SetCurrentValue(SelectedConnectionProperty, null);
        RefreshNodeEditor();
        _canvas?.Refresh();
        UpdateCommands();
        RefreshValidation();
    }

    private void RefreshValidation()
    {
        SetAndRaise(GraphSummaryProperty, ref _graphSummary,
            $"{Document?.Nodes.Count(n => !n.IsAnnotation) ?? 0} 个节点  |  {Document?.Connections.Count ?? 0} 条连线");
        SetAndRaise(ValidationIssuesProperty, ref _issues, Document?.Validate(AllowCycles) ?? []);
        SetAndRaise(ValidationSummaryProperty, ref _validationSummary,
            _issues.Count == 0 ? "校验通过" : $"{_issues.Count} 项待处理");
    }

    private void RefreshNodeEditor()
    {
        if (Document is not { } document || SelectedNode is not { } node)
            SetAndRaise(SelectedNodeEditorProperty, ref _selectedNodeEditor, null);
        else if (_selectedNodeEditor?.Document != document || _selectedNodeEditor.Node != node)
            SetAndRaise(SelectedNodeEditorProperty, ref _selectedNodeEditor, new WorkflowNodeEditor(document, node));
        else _selectedNodeEditor.Refresh();
    }

    private void ObserveDefinitions()
    {
        if (_definitionsObserver != null) _definitionsObserver.CollectionChanged -= DefinitionsChanged;
        _definitionsObserver = _attached ? NodeDefinitions as INotifyCollectionChanged : null;
        if (_definitionsObserver != null) _definitionsObserver.CollectionChanged += DefinitionsChanged;
        FilterLibrary();
    }

    private void DefinitionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        FilterLibrary();
    }

    private void FilterLibrary()
    {
        SetAndRaise(LibraryGroupsProperty, ref _groups,
            (NodeDefinitions ?? [])
            .Where(d => string.IsNullOrWhiteSpace(SearchText) ||
                        d.Title.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ||
                        d.Description.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase))
            .GroupBy(d => d.Category).Select(g => new WorkflowNodeGroup(g.Key, g.ToArray())).ToArray());
    }

    internal Rect GetGraphBounds()
    {
        return _canvas?.GraphBounds ?? new Rect(0, 0, 1, 1);
    }

    private void UpdateCommands()
    {
        foreach (var c in new[]
                     { AddNodeCommand, UndoCommand, RedoCommand, DeleteCommand, ArrangeCommand, AlignCommand })
            (c as EditorCommand)?.Raise();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == NodeDefinitionsProperty) ObserveDefinitions();
        if (change.Property == SearchTextProperty) FilterLibrary();
        if (change.Property == AllowCyclesProperty) RefreshValidation();
        if (change.Property == DocumentProperty)
        {
            _selection.Clear();
            SetCurrentValue(SelectedNodeProperty, null);
            SetCurrentValue(SelectedConnectionProperty, null);
            _canvas?.CancelGesture();
            Observe();
        }

        if (change.Property == SelectedNodeProperty)
        {
            if (SelectedNode is { } node)
            {
                if (Document?.ContainsNode(node) != true)
                {
                    SetCurrentValue(SelectedNodeProperty, null);
                    return;
                }

                SetCurrentValue(SelectedConnectionProperty, null);
                if (!_selection.Contains(node))
                {
                    _selection.Clear();
                    _selection.Add(node);
                }
            }
            else
            {
                _selection.Clear();
            }

            RefreshNodeEditor();
            _canvas?.InvalidateVisual();
            _canvas?.UpdateSelection();
            UpdateCommands();
        }

        if (change.Property == SelectedConnectionProperty)
        {
            if (SelectedConnection is { } connection)
            {
                if (Document?.ContainsConnection(connection) != true)
                {
                    SetCurrentValue(SelectedConnectionProperty, null);
                    return;
                }

                _selection.Clear();
                SetCurrentValue(SelectedNodeProperty, null);
                _canvas?.UpdateSelection();
            }

            _canvas?.InvalidateVisual();
            UpdateCommands();
        }

        if (change.Property == NodeTemplateProperty || change.Property == ContentTemplateProperty)
            _canvas?.Refresh(true);
        if (change.Property == ZoomProperty || change.Property == PanProperty)
        {
            SetAndRaise(ZoomTextProperty, ref _zoomText, $"{Zoom:P0}");
            _canvas?.InvalidateMeasure();
            _canvas?.InvalidateVisual();
        }

        if (change.Property == IsReadOnlyProperty)
        {
            _canvas?.CancelGesture();
            _canvas?.Refresh();
            UpdateCommands();
        }
    }

    internal void Report(string message)
    {
        SetAndRaise(StatusProperty, ref _status, message);
    }

    public WorkflowNode? AddNode(WorkflowNodeDefinition definition, Point position)
    {
        if (IsReadOnly || Document is null) return null;
        var n = definition.CreateNode();
        if (n.TypeId != definition.TypeId)
            throw new InvalidOperationException("Factory TypeId must match its node definition.");
        n.X = Snap(position.X);
        n.Y = Snap(position.Y);
        Document.AddNode(n);
        SelectNode(n);
        Report($"已添加：{n.Title}");
        return n;
    }

    internal double Snap(double v)
    {
        return SnapToGrid ? Math.Round(v / 20) * 20 : v;
    }

    public void SelectNode(WorkflowNode? node, bool additive = false)
    {
        if (node != null && Document?.ContainsNode(node) != true) return;
        SetCurrentValue(SelectedConnectionProperty, null);
        if (!additive) _selection.Clear();
        if (node != null)
            if (!_selection.Remove(node))
                _selection.Add(node);
        SetCurrentValue(SelectedNodeProperty, _selection.LastOrDefault());
        _canvas?.UpdateSelection();
        UpdateCommands();
    }

    internal void SelectMany(IEnumerable<WorkflowNode> nodes)
    {
        _selection.Clear();
        foreach (var n in nodes) _selection.Add(n);
        SetCurrentValue(SelectedNodeProperty, _selection.LastOrDefault());
        _canvas?.UpdateSelection();
        UpdateCommands();
    }

    public bool Connect(string sourceNode, string sourcePort, string targetNode, string targetPort)
    {
        if (IsReadOnly || Document is null) return false;
        var error = Document.CheckConnection(sourceNode, sourcePort, targetNode, targetPort, AllowCycles);
        if (error is null && ConnectionValidator != null)
        {
            var s = Document.Nodes.First(n => n.Id == sourceNode);
            var t = Document.Nodes.First(n => n.Id == targetNode);
            error = ConnectionValidator(s, s.Ports.First(p => p.Id == sourcePort), t,
                t.Ports.First(p => p.Id == targetPort));
        }

        if (error is not null)
        {
            Report(error);
            return false;
        }

        var result = Document.TryConnect(sourceNode, sourcePort, targetNode, targetPort, out error, AllowCycles);
        Report(result ? "已连接节点" : error!);
        return result;
    }

    public void DeleteSelection()
    {
        if (IsReadOnly || Document is null) return;
        Document.Remove(_selection.ToArray(), SelectedConnection);
        SelectNode(null);
    }

    public void FitToView()
    {
        if (_canvas is null || Document?.Nodes.Count is not > 0) return;
        var bounds = _canvas.GraphBounds;
        var padding = FitPadding;
        var width = Math.Max(1, _canvas.Bounds.Width - padding.Left - padding.Right);
        var height = Math.Max(1, _canvas.Bounds.Height - padding.Top - padding.Bottom);
        var zoom = Math.Clamp(Math.Min(width / Math.Max(1, bounds.Width), height / Math.Max(1, bounds.Height)), .25, 2);
        SetCurrentValue(ZoomProperty, zoom);
        SetCurrentValue(PanProperty,
            new Vector(padding.Left + (width - bounds.Width * zoom) / 2 - bounds.X * zoom,
                padding.Top + (height - bounds.Height * zoom) / 2 - bounds.Y * zoom));
    }

    public void Locate(string? nodeId)
    {
        if (Document?.FindNode(nodeId) is not { } n || _canvas == null) return;
        SelectNode(n);
        SetCurrentValue(PanProperty,
            new Vector(_canvas.Bounds.Width / 2 - (n.X + 110) * Zoom, _canvas.Bounds.Height / 2 - (n.Y + 60) * Zoom));
    }

    public void ArrangeNodes()
    {
        if (IsReadOnly || Document is null) return;
        var level = Document.Nodes.ToDictionary(n => n.Id, _ => 0);
        // Bounded passes also make explicitly allowed cycles terminate deterministically.
        for (var i = 0; i < Document.Nodes.Count; i++)
            foreach (var e in Document.Connections)
                if (level.ContainsKey(e.SourceNodeId) && level.ContainsKey(e.TargetNodeId))
                    level[e.TargetNodeId] = Math.Min(Document.Nodes.Count - 1,
                        Math.Max(level[e.TargetNodeId], level[e.SourceNodeId] + 1));
        var positions = Document.Nodes.GroupBy(n => level[n.Id])
            .SelectMany(g => g.Select((n, i) => (n, g.Key * 300d, i * 230d)));
        Document.MoveNodes(positions);
        FitToView();
    }

    public void CopySelection()
    {
        if (Document is null) return;
        var part = new WorkflowDocument();
        foreach (var n in _selection) part.Nodes.Add(n);
        var ids = _selection.Select(n => n.Id).ToHashSet();
        foreach (var c in Document.Connections.Where(c => ids.Contains(c.SourceNodeId) && ids.Contains(c.TargetNodeId)))
            part.Connections.Add(c);
        _clipboard = WorkflowSerializer.Save(part);
        // Detach temporary subscriptions; the clipboard is editor-local and never writes system data.
        part.Nodes.Clear();
        part.Connections.Clear();
        Report("已复制选择，可在此编辑器内粘贴");
    }

    public void PasteSelection()
    {
        if (IsReadOnly || Document is null || _clipboard is null) return;
        var part = WorkflowSerializer.Load(_clipboard);
        var ids = part.Nodes.ToDictionary(n => n.Id, _ => Guid.NewGuid().ToString("N"));
        var nodes = part.Nodes.Select(n => Clone(n, ids)).ToArray();
        var edges = part.Connections.Select(c => new WorkflowConnection
        {
            SourceNodeId = ids[c.SourceNodeId], SourcePortId = c.SourcePortId, TargetNodeId = ids[c.TargetNodeId],
            TargetPortId = c.TargetPortId, Label = c.Label
        }).ToArray();
        Document.Edit("粘贴", () =>
        {
            foreach (var n in nodes) Document.Nodes.Add(n);
            foreach (var c in edges) Document.Connections.Add(c);
        }, () =>
        {
            foreach (var c in edges) Document.Connections.Remove(c);
            foreach (var n in nodes) Document.Nodes.Remove(n);
        });
        SelectMany(nodes);
    }

    private static WorkflowNode Clone(WorkflowNode source, Dictionary<string, string> ids)
    {
        var n = new WorkflowNode
        {
            Id = ids[source.Id], TypeId = source.TypeId, Title = source.Title, Description = source.Description,
            X = source.X + 40, Y = source.Y + 40, Width = source.Width, Shape = source.Shape,
            IsAnnotation = source.IsAnnotation
        };
        foreach (var p in source.Ports) n.Ports.Add(p);
        foreach (var p in source.Properties)
            n.Properties.Add(new WorkflowProperty
            {
                Key = p.Key, Title = p.Title, Kind = p.Kind, DataType = p.DataType, IsRequired = p.IsRequired,
                Value = p.Value,
                Reference = p.Reference is { } r ? r with { NodeId = ids.GetValueOrDefault(r.NodeId, r.NodeId) } : null
            });
        return n;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        // Text entry and embedded native input keep their own shortcuts and navigation.
        if ((e.Source as Visual)?.GetVisualAncestors().Prepend(e.Source as Visual)
            .Any(v => v is TextBox or ComboBox or NumericUpDown) == true) return;
        var command = (e.KeyModifiers & (KeyModifiers.Control | KeyModifiers.Meta)) != 0;
        if (command && e.Key == Key.Z)
        {
            if (!IsReadOnly)
            {
                if (e.KeyModifiers.HasFlag(KeyModifiers.Shift)) Document?.Redo();
                else Document?.Undo();
            }
        }
        else if (command && e.Key == Key.C)
        {
            CopySelection();
        }
        else if (command && e.Key == Key.V)
        {
            PasteSelection();
        }
        else if (command && e.Key == Key.D)
        {
            CopySelection();
            PasteSelection();
        }
        else if (e.Key is Key.Delete or Key.Back)
        {
            DeleteSelection();
        }
        else if (e.Key == Key.Escape)
        {
            _canvas?.CancelGesture();
            SelectNode(null);
        }
        else if (!IsReadOnly && Document != null && e.Key is Key.Left or Key.Right or Key.Up or Key.Down)
        {
            var d = e.KeyModifiers.HasFlag(KeyModifiers.Shift) ? 10 : 1;
            Document.MoveNodes(_selection.Select(n => (n, n.X + (e.Key == Key.Left ? -d : e.Key == Key.Right ? d : 0),
                n.Y + (e.Key == Key.Up ? -d : e.Key == Key.Down ? d : 0))));
        }
        else
        {
            return;
        }

        e.Handled = true;
    }

    private void PalettePressed(object? sender, PointerPressedEventArgs e)
    {
        var button = (e.Source as Visual)?.GetVisualAncestors().Prepend(e.Source as Visual).OfType<Button>()
            .FirstOrDefault();
        if (!IsReadOnly && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed &&
            button?.Tag is WorkflowNodeDefinition d)
        {
            _paletteDefinition = d;
            _paletteOrigin = e.GetPosition(this);
        }
    }

    private void PaletteMoved(object? sender, PointerEventArgs e)
    {
        var delta = e.GetPosition(this) - _paletteOrigin;
        if (_paletteDefinition != null && Math.Abs(delta.X) + Math.Abs(delta.Y) > 6)
        {
            _paletteDragging = true;
            e.Pointer.Capture(this);
            Report("释放到画布以创建节点");
        }
    }

    private void PaletteReleased(object? sender, PointerReleasedEventArgs e)
    {
        var definition = _paletteDefinition;
        _paletteDefinition = null;
        if (!_paletteDragging) return;
        _paletteDragging = false;
        e.Pointer.Capture(null);
        e.Handled = true;
        if (definition != null && _canvas != null && new Rect(_canvas.Bounds.Size).Contains(e.GetPosition(_canvas)))
            AddNode(definition, _canvas.ToWorld(e.GetPosition(_canvas)));
    }

    private sealed class EditorCommand(Action<object?> action, Func<object?, bool>? canExecute = null) : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object? parameter)
        {
            if (CanExecute(parameter)) action(parameter);
        }

        public void Raise()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
