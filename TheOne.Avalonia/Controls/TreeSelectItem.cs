using System.ComponentModel;
using Avalonia;
using Avalonia.Controls.Templates;

namespace TheOne.Avalonia.Controls;

/// <summary>Read-only row state. Item is the original application model, never a copied model.</summary>
public sealed class TreeSelectItem : INotifyPropertyChanged
{
    internal TreeSelectItem(TreeSelect owner, object item, object key, string text, bool enabled, TreeSelectItem? parent)
    {
        Owner = owner; Item = item; Key = key; Text = text; IsEnabled = enabled; Parent = parent;
        Depth = parent is null ? 0 : parent.Depth + 1;
    }
    internal TreeSelect Owner { get; }
    internal TreeSelectItem? Parent { get; }
    internal List<TreeSelectItem> Children { get; } = [];
    public object Item { get; }
    public object Key { get; }
    public string Text { get; }
    public int Depth { get; }
    public Thickness Indent => new(Math.Min(Depth, 100) * 16, 0, 0, 0);
    public bool HasChildren => Children.Count > 0;
    public bool IsEnabled { get; }
    public bool IsExpanded { get; internal set; }
    public bool? IsChecked { get; internal set; }
    public bool ShowCheckBox => Owner.IsMultiSelect;
    public bool CanRemove => IsEnabled && !Owner.IsReadOnly && Owner.IsEffectivelyEnabled;
    public string ExpansionGlyph => IsExpanded ? "⌄" : "›";
    public string ExpandText => IsExpanded ? "Collapse " + Text : "Expand " + Text;
    public string RemoveText => "Remove " + Text;
    public object DisplayContent => Owner.ItemTemplate is null ? Text : Item;
    public IDataTemplate? ContentTemplate => Owner.ItemTemplate;
    public object TagContent => Owner.SelectedItemTemplate is null ? Text : Item;
    public IDataTemplate? TagTemplate => Owner.SelectedItemTemplate;
    public event PropertyChangedEventHandler? PropertyChanged;
    internal void Notify() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
}
