using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TheOne.Avalonia.Workflow;

public enum WorkflowPortDirection { Input, Output }
public enum WorkflowNodeState { Pending, Running, Completed, Failed, Skipped }
public enum WorkflowValueKind { Text, Number, Boolean }
public enum WorkflowNodeShape { Card, Pill, Note }

/// <summary>Stable, host-defined data type keys; no CLR type lookup or reflection is performed.</summary>
public sealed record WorkflowOutputField(string Path, string Title, string DataType = "text");
public sealed record WorkflowPort(string Id, string Title, WorkflowPortDirection Direction,
    string DataType = "flow", int MaxConnections = 1, WorkflowOutputField[]? Fields = null);
public sealed record WorkflowDataReference(string NodeId, string OutputPortId, string FieldPath = "");
public sealed record WorkflowValidationIssue(string Message, string? NodeId = null, string? ConnectionId = null);
public sealed record WorkflowReferenceOption(string Title, WorkflowDataReference Reference);

public abstract class WorkflowObservable : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value; Notify(name); return true;
    }
    protected void Notify([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new(name));
}

/// <summary>A literal or an upstream reference. DataType is a host-defined token, e.g. text, number, email.</summary>
public sealed class WorkflowProperty : WorkflowObservable
{
    private string _value = "";
    private WorkflowDataReference? _reference;
    public string Key { get; init; } = "";
    public string Title { get; init; } = "";
    public string DataType { get; init; } = "text";
    public WorkflowValueKind Kind { get; init; }
    public bool IsRequired { get; init; }
    public string Value { get => _value; set { if (Set(ref _value, value ?? "")) Notify(nameof(DisplayValue)); } }
    public WorkflowDataReference? Reference { get => _reference; set { if (Set(ref _reference, value)) Notify(nameof(DisplayValue)); } }
    public string DisplayValue => Reference is { } r ? $"{r.NodeId}.{r.OutputPortId}.{r.FieldPath}" : Value;
}

public sealed class WorkflowNode : WorkflowObservable
{
    private string _title = "", _description = "";
    private double _x, _y;
    private double _width = 180;
    private object? _content;
    private WorkflowNodeState _state;
    private readonly HashSet<WorkflowProperty> _observed = [];
    public string Id { get; init; } = Guid.NewGuid().ToString("N");
    public string TypeId { get; init; } = "task";
    public WorkflowNodeShape Shape { get; init; }
    public bool IsAnnotation { get; init; }
    public double Width { get => _width; set { if (!double.IsFinite(value) || value < 80) throw new ArgumentOutOfRangeException(nameof(value)); Set(ref _width, value); } }
    public string Title { get => _title; set => Set(ref _title, value ?? ""); }
    public string Description { get => _description; set => Set(ref _description, value ?? ""); }
    public double X { get => _x; set { if (!double.IsFinite(value)) throw new ArgumentOutOfRangeException(nameof(value)); Set(ref _x, value); } }
    public double Y { get => _y; set { if (!double.IsFinite(value)) throw new ArgumentOutOfRangeException(nameof(value)); Set(ref _y, value); } }
    public WorkflowNodeState State { get => _state; set { if (Set(ref _state, value)) Notify(nameof(StateText)); } }
    public string StateText => State switch { WorkflowNodeState.Running => "进行中", WorkflowNodeState.Completed => "已完成", WorkflowNodeState.Failed => "失败", WorkflowNodeState.Skipped => "已跳过", _ => "未执行" };
    /// <summary>Optional host presentation data, never serialized. Restore it from TypeId after loading.</summary>
    public object? Content { get => _content; set => Set(ref _content, value); }
    public ObservableCollection<WorkflowPort> Ports { get; } = [];
    public ObservableCollection<WorkflowProperty> Properties { get; } = [];
    public WorkflowNode()
    {
        Ports.CollectionChanged += (_, _) => Notify(nameof(Ports));
        Properties.CollectionChanged += (_, _) =>
        {
            foreach (var p in _observed) p.PropertyChanged -= PropertyChangedInProperty;
            _observed.Clear();
            foreach (var p in Properties) if (_observed.Add(p)) p.PropertyChanged += PropertyChangedInProperty;
            Notify(nameof(Properties));
        };
    }
    private void PropertyChangedInProperty(object? sender, PropertyChangedEventArgs e) => Notify(nameof(Properties));
}

public sealed class WorkflowConnection : WorkflowObservable
{
    private string _label = "";
    public string Id { get; init; } = Guid.NewGuid().ToString("N");
    public string SourceNodeId { get; init; } = "";
    public string SourcePortId { get; init; } = "";
    public string TargetNodeId { get; init; } = "";
    public string TargetPortId { get; init; } = "";
    public string Label { get => _label; set => Set(ref _label, value ?? ""); }
}

/// <summary>The host supplies node kinds and factories. Returned instances must have fresh IDs.</summary>
public sealed class WorkflowNodeDefinition
{
    public required string TypeId { get; init; }
    public required string Title { get; init; }
    public string Description { get; init; } = "";
    public string Category { get; init; } = "基础节点";
    public object? Content { get; init; }
    public required Func<WorkflowNode> CreateNode { get; init; }
}
public sealed record WorkflowNodeGroup(string Title, WorkflowNodeDefinition[] Items);
