using System.Collections.ObjectModel;

namespace TheOne.Avalonia.Workflow;

/// <summary>Undo-aware presentation for the default inspector; usable in custom templates.</summary>
public sealed class WorkflowNodeEditor : WorkflowObservable, IDisposable
{
    public WorkflowNodeEditor(WorkflowDocument document, WorkflowNode node)
    {
        Document = document;
        Node = node;
        Refresh();
    }

    public WorkflowDocument Document { get; }
    public WorkflowNode Node { get; }
    public ObservableCollection<WorkflowPropertyEditor> Fields { get; } = [];

    public string Title
    {
        get => Node.Title;
        set
        {
            var old = Node.Title;
            if (old != value) Document.Edit("修改标题", () => Node.Title = value, () => Node.Title = old);
        }
    }

    public string Description
    {
        get => Node.Description;
        set
        {
            var old = Node.Description;
            if (old != value) Document.Edit("修改说明", () => Node.Description = value, () => Node.Description = old);
        }
    }

    /// <summary>Refresh after document edits, including undo and upstream metadata changes.</summary>
    public void Refresh()
    {
        if (!Fields.Select(f => f.Property).SequenceEqual(Node.Properties))
        {
            foreach (var field in Fields) field.Dispose();
            Fields.Clear();
            foreach (var property in Node.Properties) Fields.Add(new WorkflowPropertyEditor(Document, Node, property));
        }

        foreach (var field in Fields) field.Refresh();
        Notify(nameof(Title));
        Notify(nameof(Description));
    }

    public void Dispose() { foreach (var field in Fields) field.Dispose(); }
}

/// <summary>A literal/reference editor backed by the same document undo history.</summary>
public sealed partial class WorkflowPropertyEditor : WorkflowObservable, IDisposable
{
    private readonly WorkflowDocument _document;
    private readonly WorkflowNode _node;

    public WorkflowPropertyEditor(WorkflowDocument document, WorkflowNode node, WorkflowProperty property)
    {
        _document = document;
        _node = node;
        Property = property;
        Refresh();
    }

    public WorkflowProperty Property { get; }
    public string Title => Property.Title + (Property.IsRequired ? " *" : "");
    public string TypeHint => Property.Kind + " · " + Property.DataType;

    public string Literal
    {
        get => Property.Value;
        set
        {
            if (Property.Value != value) _document.SetProperty(Property, value ?? "", Property.Reference);
        }
    }

    public bool IsLiteral => !IsMultiSelect && Property.Reference == null;
    public ObservableCollection<WorkflowReferenceOption> Options { get; } = [];

    public WorkflowReferenceOption? SelectedOption
    {
        get => Options.FirstOrDefault(o => o.Reference == Property.Reference) ?? Options.FirstOrDefault();
        set
        {
            if (value == null) return;
            var reference = value.Reference.NodeId.Length == 0 ? null : value.Reference;
            if (Property.Reference != reference) _document.SetProperty(Property, Property.Value, reference);
        }
    }

    public void Refresh()
    {
        RefreshChoiceSelection();
        if (IsMultiSelect) return;
        var options = _document.GetReferenceOptions(_node, Property)
            .Prepend(new WorkflowReferenceOption("固定值", new WorkflowDataReference("", ""))).ToList();
        if (Property.Reference is { } reference && options.All(o => o.Reference != reference))
            options.Add(new WorkflowReferenceOption("无效引用：" + Property.DisplayValue, reference));
        if (!Options.SequenceEqual(options))
        {
            Options.Clear();
            foreach (var option in options) Options.Add(option);
        }

        Notify(nameof(Literal));
        Notify(nameof(SelectedOption));
        Notify(nameof(IsLiteral));
    }
}
