using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;

namespace TheOne.Avalonia.Workflow;

/// <summary>Observable UI-thread document. Editor operations are reversible; direct host changes reset history.</summary>
public sealed partial class WorkflowDocument : WorkflowObservable
{
    private readonly Dictionary<WorkflowConnection, int> _connections = [];
    private readonly Dictionary<WorkflowNode, int> _nodes = [];
    private readonly Stack<(string Title, Action Redo, Action Undo)> _undo = [], _redo = [];
    private bool _editing;
    private string _title = "新建流程";

    public WorkflowDocument()
    {
        Nodes.CollectionChanged += (_, e) =>
        {
            Observe(Nodes, _nodes, e);
            _nodeIndex = null;
            Touch();
        };
        Connections.CollectionChanged += (_, e) =>
        {
            Observe(Connections, _connections, e);
            _connectionIndex = null;
            Touch();
        };
    }

    public string Title
    {
        get => _title;
        set
        {
            if (Set(ref _title, value)) Touch();
        }
    }

    public ObservableCollection<WorkflowNode> Nodes { get; } = [];
    public ObservableCollection<WorkflowConnection> Connections { get; } = [];
    public bool CanUndo => _undo.Count > 0;
    public bool CanRedo => _redo.Count > 0;
    public long Revision { get; private set; }
    public event EventHandler? Changed;

    private void Observe<T>(ObservableCollection<T> source, Dictionary<T, int> observed,
        NotifyCollectionChangedEventArgs change) where T : WorkflowObservable
    {
        if (change.Action == NotifyCollectionChangedAction.Move) return;
        if (change.Action == NotifyCollectionChangedAction.Reset)
        {
            foreach (var item in observed.Keys) item.PropertyChanged -= ItemChanged;
            observed.Clear();
            foreach (var item in source) Add(item);
            return;
        }

        if (change.OldItems != null)
            foreach (T item in change.OldItems)
                if (observed[item] == 1)
                {
                    item.PropertyChanged -= ItemChanged;
                    observed.Remove(item);
                }
                else observed[item]--;
        if (change.NewItems != null)
            foreach (T item in change.NewItems) Add(item);
        return;

        void Add(T item)
        {
            if (observed.TryGetValue(item, out var count)) observed[item] = count + 1;
            else
            {
                observed.Add(item, 1);
                item.PropertyChanged += ItemChanged;
            }
        }
    }

    private void ItemChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is WorkflowNode && e.PropertyName == nameof(WorkflowNode.StateText)) return;
        if (sender is WorkflowNode && e.PropertyName is nameof(WorkflowNode.State) or nameof(WorkflowNode.Content))
        {
            if (!_editing) Changed?.Invoke(this, EventArgs.Empty);
            return;
        }

        Touch();
    }

    private void Touch()
    {
        if (_editing) return;
        _undo.Clear();
        _redo.Clear();
        Publish();
    }

    private void Publish()
    {
        Revision++;
        Notify(nameof(Revision));
        Notify(nameof(CanUndo));
        Notify(nameof(CanRedo));
        Changed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Apply a host edit to the same history as graph edits. Actions must be deterministic and not throw.</summary>
    public void Edit(string title, Action redo, Action undo)
    {
        ArgumentNullException.ThrowIfNull(redo);
        ArgumentNullException.ThrowIfNull(undo);
        _editing = true;
        try
        {
            redo();
        }
        finally
        {
            _editing = false;
        }

        _undo.Push((title, redo, undo));
        _redo.Clear();
        Publish();
    }

    public void Undo()
    {
        if (_undo.TryPop(out var edit))
        {
            _editing = true;
            try
            {
                edit.Undo();
            }
            finally
            {
                _editing = false;
            }

            _redo.Push(edit);
            Publish();
        }
    }

    public void Redo()
    {
        if (_redo.TryPop(out var edit))
        {
            _editing = true;
            try
            {
                edit.Redo();
            }
            finally
            {
                _editing = false;
            }

            _undo.Push(edit);
            Publish();
        }
    }

    public void ClearHistory()
    {
        _undo.Clear();
        _redo.Clear();
        Notify(nameof(CanUndo));
        Notify(nameof(CanRedo));
    }

    public void AddNode(WorkflowNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        if (string.IsNullOrWhiteSpace(node.Id) || FindNode(node.Id) != null)
            throw new ArgumentException("Node ID must be unique.", nameof(node));
        Edit("添加节点", () => Nodes.Add(node), () => Nodes.Remove(node));
    }

    public void Remove(IEnumerable<WorkflowNode> nodes, WorkflowConnection? connection = null)
    {
        var removed = nodes.Where(ContainsNode).Distinct().Select(n => (Node: n, Index: Nodes.IndexOf(n))).ToArray();
        var ids = removed.Select(x => x.Node.Id).ToHashSet();
        var edges = Connections.Select((c, index) => (Edge: c, Index: index))
            .Where(x => ids.Contains(x.Edge.SourceNodeId) || ids.Contains(x.Edge.TargetNodeId) || x.Edge == connection)
            .ToArray();
        if (removed.Length == 0 && edges.Length == 0) return;
        Edit("删除", () =>
            {
                foreach (var e in edges) Connections.Remove(e.Edge);
                foreach (var n in removed) Nodes.Remove(n.Node);
            },
            () =>
            {
                foreach (var n in removed) Nodes.Insert(n.Index, n.Node);
                foreach (var e in edges) Connections.Insert(e.Index, e.Edge);
            });
    }

    public void MoveNodes(IEnumerable<(WorkflowNode Node, double X, double Y)> positions)
    {
        var moves = positions.Where(p => ContainsNode(p.Node))
            .Select(p => (p.Node, p.X, p.Y, OldX: p.Node.X, OldY: p.Node.Y)).ToArray();
        if (moves.All(p => p.X == p.OldX && p.Y == p.OldY)) return;
        if (moves.Any(p => !double.IsFinite(p.X) || !double.IsFinite(p.Y)))
            throw new ArgumentException("Coordinates must be finite.", nameof(positions));
        Edit("移动节点", () =>
        {
            foreach (var p in moves)
            {
                p.Node.X = p.X;
                p.Node.Y = p.Y;
            }
        }, () =>
        {
            foreach (var p in moves)
            {
                p.Node.X = p.OldX;
                p.Node.Y = p.OldY;
            }
        });
    }

    public void SetProperty(WorkflowProperty property, string value, WorkflowDataReference? reference)
    {
        var old = property.Value;
        var oldRef = property.Reference;
        if (old == value && oldRef == reference) return;
        Edit("修改字段", () =>
        {
            property.Value = value;
            property.Reference = reference;
        }, () =>
        {
            property.Value = old;
            property.Reference = oldRef;
        });
    }

    /// <summary>修改本图中指定连线的固定值或引用，保留原字段的撤销记录；不允许编辑其他文档的字段。</summary>
    public void SetConnectionProperty(WorkflowConnection connection, WorkflowProperty property, string value,
        WorkflowDataReference? reference = null)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(property);
        if (!ContainsConnection(connection) || !connection.Properties.Contains(property))
            throw new ArgumentException("The property must belong to a connection in this document.", nameof(property));
        SetProperty(property, value, reference);
    }

    /// <summary>One undoable multi-selection edit, preserving stable keys and their order.</summary>
    public void SetSelectedKeys(WorkflowProperty property, IEnumerable<string> keys)
    {
        ArgumentNullException.ThrowIfNull(property);
        ArgumentNullException.ThrowIfNull(keys);
        var next = keys.ToArray();
        if (next.Any(string.IsNullOrWhiteSpace)) throw new ArgumentException("Selection keys cannot be empty.", nameof(keys));
        next = next.Distinct(StringComparer.Ordinal).ToArray();
        var old = property.SelectedKeys.ToArray();
        var reference = property.Reference;
        if (old.SequenceEqual(next) && reference is null) return;
        Edit("修改多选字段", () => Apply(next, null), () => Apply(old, reference));
        void Apply(string[] values, WorkflowDataReference? valueReference)
        {
            property.SelectedKeys.Clear();
            foreach (var key in values) property.SelectedKeys.Add(key);
            property.Reference = valueReference;
        }
    }

    public string? CheckConnection(string sourceNode, string sourcePort, string targetNode, string targetPort,
        bool allowCycles = false)
    {
        var source = FindNode(sourceNode);
        var target = FindNode(targetNode);
        var output = source?.Ports.FirstOrDefault(p => p.Id == sourcePort);
        var input = target?.Ports.FirstOrDefault(p => p.Id == targetPort);
        if (source is null || target is null || output is null || input is null) return "节点或端口不存在";
        if (source == target) return "不允许自连接";
        if (output.Direction != WorkflowPortDirection.Output || input.Direction != WorkflowPortDirection.Input)
            return "必须从输出连接到输入";
        if (output.DataType != input.DataType && output.DataType != "any" && input.DataType != "any")
            return "端口数据类型不兼容";
        var index = ConnectionIndex;
        if (index.Edges.ContainsKey((sourceNode, sourcePort, targetNode, targetPort))) return "连接已存在";
        if ((output.MaxConnections > 0 &&
             index.OutputCounts.GetValueOrDefault((sourceNode, sourcePort)) >= output.MaxConnections) ||
            (input.MaxConnections > 0 &&
             index.InputCounts.GetValueOrDefault((targetNode, targetPort)) >= input.MaxConnections))
            return "端口连接数量已达上限";
        if (!allowCycles && IsReachable(targetNode, sourceNode)) return "该连接会产生环路";
        return null;
    }

    public bool TryConnect(string sourceNode, string sourcePort, string targetNode, string targetPort,
        out string? error, bool allowCycles = false)
    {
        var edge = new WorkflowConnection
        {
            SourceNodeId = sourceNode, SourcePortId = sourcePort, TargetNodeId = targetNode, TargetPortId = targetPort
        };
        return TryConnect(edge, out error, allowCycles);
    }

    /// <summary>Adds a host-created connection in one undoable edit after validating identity and topology.</summary>
    public bool TryConnect(WorkflowConnection connection, out string? error, bool allowCycles = false)
    {
        ArgumentNullException.ThrowIfNull(connection);
        error = string.IsNullOrWhiteSpace(connection.Id) || Connections.Any(edge => edge.Id == connection.Id)
            ? "连线身份必须唯一且不能为空"
            : CheckConnection(connection.SourceNodeId, connection.SourcePortId, connection.TargetNodeId,
                connection.TargetPortId, allowCycles);
        if (error is not null) return false;
        var edge = connection;
        Edit("连接节点", () => Connections.Add(edge), () => Connections.Remove(edge));
        return true;
    }

    public bool IsReachable(string from, string to)
    {
        var seen = new HashSet<NodeId>();
        var pending = new Stack<NodeId>();
        pending.Push(new NodeId(from));
        while (pending.TryPop(out var id))
        {
            if (!seen.Add(id)) continue;
            if (id.Value == to) return true;
            if (ConnectionIndex.Outgoing.TryGetValue(id, out var targets))
                foreach (var target in targets) pending.Push(target);
        }

        return false;
    }

    public IReadOnlyList<WorkflowReferenceOption> GetReferenceOptions(WorkflowNode target, WorkflowProperty property)
    {
        var upstream = ConnectionIndex.Upstream(new NodeId(target.Id));
        return Nodes.Where(n => n != target && upstream.Contains(new NodeId(n.Id))).SelectMany(n => n.Ports
                .Where(p => p.Direction == WorkflowPortDirection.Output)
                .SelectMany(p => (p.Fields ?? [])
                    .Where(f => property.DataType == "any" || f.DataType == property.DataType)
                    .Select(f =>
                        new WorkflowReferenceOption($"{n.Title} → {f.Title}",
                            new WorkflowDataReference(n.Id, p.Id, f.Path)))))
            .ToArray();
    }

    public IReadOnlyList<WorkflowValidationIssue> Validate(bool allowCycles = false)
    {
        List<WorkflowValidationIssue> issues = [];
        var ids = new HashSet<string>();
        foreach (var n in Nodes)
        {
            if (string.IsNullOrWhiteSpace(n.Id) || !ids.Add(n.Id))
                issues.Add(new WorkflowValidationIssue("节点 ID 为空或重复", n.Id));
            if (n.Ports.Select(p => p.Id).Distinct().Count() != n.Ports.Count ||
                n.Ports.Any(p => string.IsNullOrWhiteSpace(p.Id)))
                issues.Add(new WorkflowValidationIssue("端口 ID 为空或重复", n.Id));
            foreach (var p in n.Properties)
                if (p.Reference is { } r)
                {
                    if (!GetReferenceOptions(n, p).Any(o => o.Reference == r))
                        issues.Add(new WorkflowValidationIssue($"{p.Title}：引用不存在、类型不兼容或不在上游", n.Id));
                }
                else if (p.Kind == WorkflowValueKind.MultiSelect && (p.SelectedKeys.Any(string.IsNullOrWhiteSpace) ||
                             p.SelectedKeys.Distinct(StringComparer.Ordinal).Count() != p.SelectedKeys.Count))
                {
                    issues.Add(new WorkflowValidationIssue($"{p.Title}：选项 Key 为空或重复", n.Id));
                }
                else if (p.IsRequired && (p.Kind == WorkflowValueKind.MultiSelect ? p.SelectedKeys.Count == 0 : string.IsNullOrWhiteSpace(p.Value)))
                {
                    issues.Add(new WorkflowValidationIssue($"{p.Title}：必填", n.Id));
                }
                else if (p.Kind == WorkflowValueKind.Number && p.Value.Length > 0 &&
                         (!double.TryParse(p.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) ||
                          !double.IsFinite(number)))
                {
                    issues.Add(new WorkflowValidationIssue($"{p.Title}：请输入有限数值", n.Id));
                }
                else if (p.Kind == WorkflowValueKind.Boolean && p.Value.Length > 0 && !bool.TryParse(p.Value, out _))
                {
                    issues.Add(new WorkflowValidationIssue($"{p.Title}：请输入 true 或 false", n.Id));
                }
        }

        var edgeIds = new HashSet<string>();
        var index = ConnectionIndex;
        var components = allowCycles ? null : index.Components();
        foreach (var c in Connections)
        {
            var source = FindNode(c.SourceNodeId);
            var target = FindNode(c.TargetNodeId);
            var output = source?.Ports.FirstOrDefault(p => p.Id == c.SourcePortId);
            var input = target?.Ports.FirstOrDefault(p => p.Id == c.TargetPortId);
            if (!edgeIds.Add(c.Id) || string.IsNullOrWhiteSpace(c.Id))
                issues.Add(new WorkflowValidationIssue("连线 ID 为空或重复", ConnectionId: c.Id));
            ValidateConnectionProperties(c, target, issues);
            if (source == null || target == null || output == null || input == null)
            {
                issues.Add(new WorkflowValidationIssue("连线端点不存在", ConnectionId: c.Id));
                continue;
            }

            if (source == target || output.Direction != WorkflowPortDirection.Output ||
                input.Direction != WorkflowPortDirection.Input ||
                (output.DataType != input.DataType && output.DataType != "any" && input.DataType != "any"))
                issues.Add(new WorkflowValidationIssue("连线方向或类型不合法", ConnectionId: c.Id));
            if (components != null && components[new NodeId(target.Id)] == components[new NodeId(source.Id)])
                issues.Add(new WorkflowValidationIssue("存在环路", ConnectionId: c.Id));
            if (index.Edges[(c.SourceNodeId, c.SourcePortId, c.TargetNodeId, c.TargetPortId)] > 1)
                issues.Add(new WorkflowValidationIssue("存在重复连线", ConnectionId: c.Id));
            if ((output.MaxConnections > 0 &&
                 index.OutputCounts.GetValueOrDefault((source.Id, output.Id)) >
                 output.MaxConnections) ||
                (input.MaxConnections > 0 &&
                 index.InputCounts.GetValueOrDefault((target.Id, input.Id)) >
                 input.MaxConnections)) issues.Add(new WorkflowValidationIssue("端口连接数量超过上限", ConnectionId: c.Id));
        }

        return issues;
    }

    /// <summary>验证连线字段及其上游引用；业务字段含义仍由宿主验证，不把 Label 当执行表达式。</summary>
    private void ValidateConnectionProperties(WorkflowConnection connection, WorkflowNode? target,
        List<WorkflowValidationIssue> issues)
    {
        if (connection.Properties.Any(p => string.IsNullOrWhiteSpace(p.Key)) ||
            connection.Properties.Select(p => p.Key).Distinct().Count() != connection.Properties.Count)
            issues.Add(new WorkflowValidationIssue("连线字段 Key 为空或重复", ConnectionId: connection.Id));
        foreach (var property in connection.Properties)
        {
            string? error = null;
            if (string.IsNullOrWhiteSpace(property.DataType) || !Enum.IsDefined(property.Kind))
                error = "字段类型不合法";
            else if (property.Reference is { } reference)
            {
                if (target is null || !GetReferenceOptions(target, property).Any(o => o.Reference == reference))
                    error = "引用不存在、类型不兼容或不在上游";
            }
            else if (property.Kind == WorkflowValueKind.MultiSelect &&
                     (property.SelectedKeys.Any(string.IsNullOrWhiteSpace) ||
                      property.SelectedKeys.Distinct(StringComparer.Ordinal).Count() != property.SelectedKeys.Count))
                error = "选项 Key 为空或重复";
            else if (property.IsRequired && (property.Kind == WorkflowValueKind.MultiSelect
                         ? property.SelectedKeys.Count == 0 : string.IsNullOrWhiteSpace(property.Value)))
                error = "必填";
            else if (property.Kind == WorkflowValueKind.Number && property.Value.Length > 0 &&
                     (!double.TryParse(property.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) ||
                      !double.IsFinite(number)))
                error = "请输入有限数值";
            else if (property.Kind == WorkflowValueKind.Boolean && property.Value.Length > 0 &&
                     !bool.TryParse(property.Value, out _))
                error = "请输入 true 或 false";
            if (error is not null)
                issues.Add(new WorkflowValidationIssue($"{property.Title}：{error}", ConnectionId: connection.Id));
        }
    }
}
