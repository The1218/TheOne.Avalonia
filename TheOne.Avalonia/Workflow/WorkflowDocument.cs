using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

namespace TheOne.Avalonia.Workflow;

/// <summary>Observable UI-thread document. Editor operations are reversible; direct host changes reset history.</summary>
public sealed class WorkflowDocument : WorkflowObservable
{
    private string _title = "新建流程";
    private bool _editing;
    private readonly Stack<(string Title, Action Redo, Action Undo)> _undo = [], _redo = [];
    private readonly HashSet<WorkflowNode> _nodes = [];
    private readonly HashSet<WorkflowConnection> _connections = [];
    public string Title { get => _title; set { if (Set(ref _title, value)) Touch(); } }
    public ObservableCollection<WorkflowNode> Nodes { get; } = [];
    public ObservableCollection<WorkflowConnection> Connections { get; } = [];
    public bool CanUndo => _undo.Count > 0;
    public bool CanRedo => _redo.Count > 0;
    public long Revision { get; private set; }
    public event EventHandler? Changed;
    public WorkflowDocument()
    {
        Nodes.CollectionChanged += (_, _) => { Observe(); Touch(); };
        Connections.CollectionChanged += (_, _) => { Observe(); Touch(); };
    }
    private void Observe()
    {
        foreach (var n in _nodes) n.PropertyChanged -= ItemChanged;
        foreach (var c in _connections) c.PropertyChanged -= ItemChanged;
        _nodes.Clear(); _connections.Clear();
        foreach (var n in Nodes) if (_nodes.Add(n)) n.PropertyChanged += ItemChanged;
        foreach (var c in Connections) if (_connections.Add(c)) c.PropertyChanged += ItemChanged;
    }
    private void ItemChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is WorkflowNode && e.PropertyName == nameof(WorkflowNode.StateText)) return;
        if (sender is WorkflowNode && e.PropertyName is nameof(WorkflowNode.State) or nameof(WorkflowNode.Content))
        { if (!_editing) Changed?.Invoke(this, EventArgs.Empty); return; }
        Touch();
    }
    private void Touch()
    {
        if (_editing) return;
        _undo.Clear(); _redo.Clear(); Publish();
    }
    private void Publish()
    {
        Revision++; Notify(nameof(Revision)); Notify(nameof(CanUndo)); Notify(nameof(CanRedo)); Changed?.Invoke(this, EventArgs.Empty);
    }
    /// <summary>Apply a host edit to the same history as graph edits. Actions must be deterministic and not throw.</summary>
    public void Edit(string title, Action redo, Action undo)
    {
        ArgumentNullException.ThrowIfNull(redo); ArgumentNullException.ThrowIfNull(undo);
        _editing = true;
        try { redo(); } finally { _editing = false; }
        _undo.Push((title, redo, undo)); _redo.Clear(); Publish();
    }
    public void Undo() { if (_undo.TryPop(out var edit)) { _editing = true; try { edit.Undo(); } finally { _editing = false; } _redo.Push(edit); Publish(); } }
    public void Redo() { if (_redo.TryPop(out var edit)) { _editing = true; try { edit.Redo(); } finally { _editing = false; } _undo.Push(edit); Publish(); } }
    public void ClearHistory() { _undo.Clear(); _redo.Clear(); Notify(nameof(CanUndo)); Notify(nameof(CanRedo)); }
    public void AddNode(WorkflowNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        if (string.IsNullOrWhiteSpace(node.Id) || Nodes.Any(n => n.Id == node.Id)) throw new ArgumentException("Node ID must be unique.", nameof(node));
        Edit("添加节点", () => Nodes.Add(node), () => Nodes.Remove(node));
    }
    public void Remove(IEnumerable<WorkflowNode> nodes, WorkflowConnection? connection = null)
    {
        var removed = nodes.Where(Nodes.Contains).Distinct().Select(n => (Node: n, Index: Nodes.IndexOf(n))).ToArray();
        var ids = removed.Select(x => x.Node.Id).ToHashSet();
        var edges = Connections.Where(c => ids.Contains(c.SourceNodeId) || ids.Contains(c.TargetNodeId) || c == connection).Select(c => (Edge: c, Index: Connections.IndexOf(c))).ToArray();
        if (removed.Length == 0 && edges.Length == 0) return;
        Edit("删除", () => { foreach (var e in edges) Connections.Remove(e.Edge); foreach (var n in removed) Nodes.Remove(n.Node); },
            () => { foreach (var n in removed) Nodes.Insert(n.Index, n.Node); foreach (var e in edges) Connections.Insert(e.Index, e.Edge); });
    }
    public void MoveNodes(IEnumerable<(WorkflowNode Node, double X, double Y)> positions)
    {
        var moves = positions.Where(p => Nodes.Contains(p.Node)).Select(p => (p.Node, p.X, p.Y, OldX: p.Node.X, OldY: p.Node.Y)).ToArray();
        if (moves.All(p => p.X == p.OldX && p.Y == p.OldY)) return;
        if (moves.Any(p => !double.IsFinite(p.X) || !double.IsFinite(p.Y))) throw new ArgumentException("Coordinates must be finite.", nameof(positions));
        Edit("移动节点", () => { foreach (var p in moves) { p.Node.X = p.X; p.Node.Y = p.Y; } }, () => { foreach (var p in moves) { p.Node.X = p.OldX; p.Node.Y = p.OldY; } });
    }
    public void SetProperty(WorkflowProperty property, string value, WorkflowDataReference? reference)
    {
        var old = property.Value; var oldRef = property.Reference;
        if (old == value && oldRef == reference) return;
        Edit("修改字段", () => { property.Value = value; property.Reference = reference; }, () => { property.Value = old; property.Reference = oldRef; });
    }
    public string? CheckConnection(string sourceNode, string sourcePort, string targetNode, string targetPort, bool allowCycles = false)
    {
        var source = Nodes.FirstOrDefault(n => n.Id == sourceNode); var target = Nodes.FirstOrDefault(n => n.Id == targetNode);
        var output = source?.Ports.FirstOrDefault(p => p.Id == sourcePort); var input = target?.Ports.FirstOrDefault(p => p.Id == targetPort);
        if (source is null || target is null || output is null || input is null) return "节点或端口不存在";
        if (source == target) return "不允许自连接";
        if (output.Direction != WorkflowPortDirection.Output || input.Direction != WorkflowPortDirection.Input) return "必须从输出连接到输入";
        if (output.DataType != input.DataType && output.DataType != "any" && input.DataType != "any") return "端口数据类型不兼容";
        if (Connections.Any(c => c.SourceNodeId == sourceNode && c.SourcePortId == sourcePort && c.TargetNodeId == targetNode && c.TargetPortId == targetPort)) return "连接已存在";
        if (output.MaxConnections > 0 && Connections.Count(c => c.SourceNodeId == sourceNode && c.SourcePortId == sourcePort) >= output.MaxConnections ||
            input.MaxConnections > 0 && Connections.Count(c => c.TargetNodeId == targetNode && c.TargetPortId == targetPort) >= input.MaxConnections) return "端口连接数量已达上限";
        if (!allowCycles && IsReachable(targetNode, sourceNode)) return "该连接会产生环路";
        return null;
    }
    public bool TryConnect(string sourceNode, string sourcePort, string targetNode, string targetPort, out string? error, bool allowCycles = false)
    {
        error = CheckConnection(sourceNode, sourcePort, targetNode, targetPort, allowCycles);
        if (error is not null) return false;
        var edge = new WorkflowConnection { SourceNodeId = sourceNode, SourcePortId = sourcePort, TargetNodeId = targetNode, TargetPortId = targetPort };
        Edit("连接节点", () => Connections.Add(edge), () => Connections.Remove(edge)); return true;
    }
    public bool IsReachable(string from, string to)
    {
        var seen = new HashSet<string>(); var pending = new Stack<string>(); pending.Push(from);
        while (pending.TryPop(out var id)) { if (!seen.Add(id)) continue; if (id == to) return true; foreach (var c in Connections.Where(c => c.SourceNodeId == id)) pending.Push(c.TargetNodeId); }
        return false;
    }
    public IReadOnlyList<WorkflowReferenceOption> GetReferenceOptions(WorkflowNode target, WorkflowProperty property)
        => Nodes.Where(n => n != target && IsReachable(n.Id, target.Id)).SelectMany(n => n.Ports.Where(p => p.Direction == WorkflowPortDirection.Output)
            .SelectMany(p => (p.Fields ?? []).Where(f => property.DataType == "any" || f.DataType == property.DataType)
                .Select(f => new WorkflowReferenceOption($"{n.Title} → {f.Title}", new(n.Id, p.Id, f.Path))))).ToArray();
    public IReadOnlyList<WorkflowValidationIssue> Validate(bool allowCycles = false)
    {
        List<WorkflowValidationIssue> issues = [];
        var ids = new HashSet<string>();
        foreach (var n in Nodes)
        {
            if (string.IsNullOrWhiteSpace(n.Id) || !ids.Add(n.Id)) issues.Add(new("节点 ID 为空或重复", n.Id));
            if (n.Ports.Select(p => p.Id).Distinct().Count() != n.Ports.Count || n.Ports.Any(p => string.IsNullOrWhiteSpace(p.Id))) issues.Add(new("端口 ID 为空或重复", n.Id));
            foreach (var p in n.Properties)
            {
                if (p.Reference is { } r)
                {
                    if (!GetReferenceOptions(n, p).Any(o => o.Reference == r)) issues.Add(new($"{p.Title}：引用不存在、类型不兼容或不在上游", n.Id));
                }
                else if (p.IsRequired && string.IsNullOrWhiteSpace(p.Value)) issues.Add(new($"{p.Title}：必填", n.Id));
                else if (p.Kind == WorkflowValueKind.Number && p.Value.Length > 0 && (!double.TryParse(p.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) || !double.IsFinite(number))) issues.Add(new($"{p.Title}：请输入有限数值", n.Id));
                else if (p.Kind == WorkflowValueKind.Boolean && p.Value.Length > 0 && !bool.TryParse(p.Value, out _)) issues.Add(new($"{p.Title}：请输入 true 或 false", n.Id));
            }
        }
        var edgeIds = new HashSet<string>();
        foreach (var c in Connections)
        {
            var source = Nodes.FirstOrDefault(n => n.Id == c.SourceNodeId); var target = Nodes.FirstOrDefault(n => n.Id == c.TargetNodeId);
            var output = source?.Ports.FirstOrDefault(p => p.Id == c.SourcePortId); var input = target?.Ports.FirstOrDefault(p => p.Id == c.TargetPortId);
            if (!edgeIds.Add(c.Id) || string.IsNullOrWhiteSpace(c.Id)) issues.Add(new("连线 ID 为空或重复", ConnectionId: c.Id));
            if (source == null || target == null || output == null || input == null) { issues.Add(new("连线端点不存在", ConnectionId: c.Id)); continue; }
            if (source == target || output.Direction != WorkflowPortDirection.Output || input.Direction != WorkflowPortDirection.Input || output.DataType != input.DataType && output.DataType != "any" && input.DataType != "any") issues.Add(new("连线方向或类型不合法", ConnectionId: c.Id));
            if (!allowCycles && IsReachable(target.Id, source.Id)) issues.Add(new("存在环路", ConnectionId: c.Id));
            if (Connections.Count(e => e.SourceNodeId == c.SourceNodeId && e.SourcePortId == c.SourcePortId && e.TargetNodeId == c.TargetNodeId && e.TargetPortId == c.TargetPortId) > 1) issues.Add(new("存在重复连线", ConnectionId: c.Id));
            if (output.MaxConnections > 0 && Connections.Count(e => e.SourceNodeId == source.Id && e.SourcePortId == output.Id) > output.MaxConnections || input.MaxConnections > 0 && Connections.Count(e => e.TargetNodeId == target.Id && e.TargetPortId == input.Id) > input.MaxConnections) issues.Add(new("端口连接数量超过上限", ConnectionId: c.Id));
        }
        return issues;
    }
}
