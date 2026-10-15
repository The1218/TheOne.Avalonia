using System.Text.Json;
using System.Text.Json.Serialization;

namespace TheOne.Avalonia.Workflow;

/// <summary>Versioned, source-generated JSON. Templates, Content, delegates and history are not persisted.</summary>
public static class WorkflowSerializer
{
    public static string Save(WorkflowDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        return JsonSerializer.Serialize(new WorkflowSnapshot
        {
            Title = document.Title,
            Nodes = document.Nodes.Select(n => new WorkflowNodeSnapshot
            {
                Id = n.Id, TypeId = n.TypeId, Title = n.Title, Description = n.Description, X = n.X, Y = n.Y, Width = n.Width, Shape = n.Shape, IsAnnotation = n.IsAnnotation,
                Ports = n.Ports.ToArray(), Properties = n.Properties.Select(p => new WorkflowPropertySnapshot(p.Key, p.Title, p.DataType, p.Kind, p.IsRequired, p.Value, p.Reference)).ToArray()
            }).ToArray(),
            Connections = document.Connections.Select(c => new WorkflowConnectionSnapshot(c.Id, c.SourceNodeId, c.SourcePortId, c.TargetNodeId, c.TargetPortId, c.Label)).ToArray()
        }, WorkflowJsonContext.Default.WorkflowSnapshot);
    }
    public static WorkflowDocument Load(string json)
    {
        var snapshot = JsonSerializer.Deserialize(json, WorkflowJsonContext.Default.WorkflowSnapshot) ?? throw new JsonException("Empty workflow document.");
        if (snapshot.Version != 1) throw new JsonException($"Unsupported workflow version {snapshot.Version}.");
        if (snapshot.Nodes is null || snapshot.Connections is null) throw new JsonException("Nodes and connections are required.");
        var doc = new WorkflowDocument { Title = snapshot.Title ?? "" };
        HashSet<string> ids = [];
        foreach (var n in snapshot.Nodes)
        {
            if (n is null || string.IsNullOrWhiteSpace(n.Id) || string.IsNullOrWhiteSpace(n.TypeId) || !ids.Add(n.Id) || !double.IsFinite(n.X) || !double.IsFinite(n.Y) || n.Ports is null || n.Properties is null) throw new JsonException("Invalid node identity, coordinates or collections.");
            if (n.Ports.Any(p => p is null || string.IsNullOrWhiteSpace(p.Id) || string.IsNullOrWhiteSpace(p.DataType) || !Enum.IsDefined(p.Direction) || p.Fields?.Any(f => f is null || string.IsNullOrWhiteSpace(f.Path) || string.IsNullOrWhiteSpace(f.DataType)) == true) || n.Ports.Select(p => p.Id).Distinct().Count() != n.Ports.Length) throw new JsonException("Invalid port identity, fields or direction.");
            if (n.Properties.Any(p => p is null || string.IsNullOrWhiteSpace(p.Key) || string.IsNullOrWhiteSpace(p.DataType) || !Enum.IsDefined(p.Kind) || p.Reference is { } r && (string.IsNullOrWhiteSpace(r.NodeId) || string.IsNullOrWhiteSpace(r.OutputPortId) || r.FieldPath is null)) || n.Properties.Select(p => p.Key).Distinct().Count() != n.Properties.Length) throw new JsonException("Invalid property identity, reference or kind.");
            if (!double.IsFinite(n.Width) || n.Width < 80 || !Enum.IsDefined(n.Shape)) throw new JsonException("Invalid node dimensions or shape.");
            var node = new WorkflowNode { Id = n.Id, TypeId = n.TypeId, Title = n.Title, Description = n.Description, X = n.X, Y = n.Y, Width = n.Width, Shape = n.Shape, IsAnnotation = n.IsAnnotation };
            foreach (var p in n.Ports) node.Ports.Add(p);
            foreach (var p in n.Properties) node.Properties.Add(new WorkflowProperty { Key = p.Key, Title = p.Title, DataType = p.DataType, Kind = p.Kind, IsRequired = p.IsRequired, Value = p.Value, Reference = p.Reference });
            doc.Nodes.Add(node);
        }
        ids.Clear();
        foreach (var c in snapshot.Connections)
        {
            if (c is null || string.IsNullOrWhiteSpace(c.Id) || !ids.Add(c.Id)) throw new JsonException("Invalid connection identity.");
            doc.Connections.Add(new WorkflowConnection { Id = c.Id, SourceNodeId = c.SourceNodeId, SourcePortId = c.SourcePortId, TargetNodeId = c.TargetNodeId, TargetPortId = c.TargetPortId, Label = c.Label });
        }
        // Drafts may contain unresolved endpoints/references; Validate exposes them without silently dropping data.
        doc.ClearHistory(); return doc;
    }
}

internal sealed class WorkflowSnapshot
{
    public int Version { get; set; } = 1;
    public string Title { get; set; } = "";
    public WorkflowNodeSnapshot[] Nodes { get; set; } = [];
    public WorkflowConnectionSnapshot[] Connections { get; set; } = [];
}
internal sealed class WorkflowNodeSnapshot
{
    public string Id { get; set; } = "";
    public string TypeId { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; } = 180;
    public WorkflowNodeShape Shape { get; set; }
    public bool IsAnnotation { get; set; }
    public WorkflowPort[] Ports { get; set; } = [];
    public WorkflowPropertySnapshot[] Properties { get; set; } = [];
}
internal sealed record WorkflowPropertySnapshot(string Key, string Title, string DataType, WorkflowValueKind Kind, bool IsRequired, string Value, WorkflowDataReference? Reference);
internal sealed record WorkflowConnectionSnapshot(string Id, string SourceNodeId, string SourcePortId, string TargetNodeId, string TargetPortId, string Label);
[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(WorkflowSnapshot))]
internal partial class WorkflowJsonContext : JsonSerializerContext;
