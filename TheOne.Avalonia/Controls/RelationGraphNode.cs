using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TheOne.Avalonia.Controls;

/// <summary>The default model for a single-root relation tree.</summary>
public class RelationGraphNode : INotifyPropertyChanged
{
    private object? _icon;
    private object? _title;
    private object? _content;
    private IList<RelationGraphNode>? _children = new ObservableCollection<RelationGraphNode>();

    public object? Icon { get => _icon; set => Set(ref _icon, value); }
    public object? Title { get => _title; set => Set(ref _title, value); }
    public object? Content { get => _content; set => Set(ref _content, value); }
    public IList<RelationGraphNode>? Children { get => _children; set => Set(ref _children, value); }
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
