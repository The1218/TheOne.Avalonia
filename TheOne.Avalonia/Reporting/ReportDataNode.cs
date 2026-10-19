namespace TheOne.Avalonia.Reporting;

public enum ReportDataNodeKind
{
    Object,
    Scalar,
    Collection
}

/// <summary>A JSON response node, or a field from a consumer supplied typed source.</summary>
public sealed class ReportDataNode : ReportObservable
{
    internal ReportDataNode(string name, string pointer, ReportDataNodeKind kind, string preview,
        IReadOnlyList<ReportDataNode> children, string fieldKey = "", bool isJson = true, bool expanded = false,
        string? typeLabel = null)
    {
        Name = name;
        Pointer = pointer;
        Kind = kind;
        Preview = preview;
        Children = children;
        FieldKey = fieldKey;
        IsJson = isJson;
        IsExpanded = expanded;
        _typeLabel = typeLabel;
    }

    public string Name { get; }
    public string Pointer { get; }
    public ReportDataNodeKind Kind { get; }

    private readonly string? _typeLabel;
    public string TypeLabel => _typeLabel ??
        (Kind switch { ReportDataNodeKind.Collection => "集合", ReportDataNodeKind.Object => "对象", _ => "值" });

    public string Preview { get; }
    public string IconKind => Kind == ReportDataNodeKind.Collection ? "Table" :
        Kind == ReportDataNodeKind.Object ? "Folder" : "Text";
    public IReadOnlyList<ReportDataNode> Children { get; }
    public string FieldKey { get; }
    public bool IsJson { get; }

    public bool IsExpanded
    {
        get;
        set => Set(ref field, value);
    }
}
