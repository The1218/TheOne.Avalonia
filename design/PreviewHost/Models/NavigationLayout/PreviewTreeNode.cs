using ReactiveUI;

namespace TheOne.DesignPreview;

public sealed class PreviewTreeNode(
    string name,
    string description,
    bool isEnabled = true,
    params PreviewTreeNode[] children) : ReactiveObject
{
    public string Name
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = name;

    public bool IsExpanded
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = children.Length > 0;

    public string Description { get; } = description;
    public bool IsEnabled { get; } = isEnabled;
    public PreviewTreeNode[] Children { get; } = children;
}