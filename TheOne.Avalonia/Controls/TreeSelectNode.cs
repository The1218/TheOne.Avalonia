using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TheOne.Avalonia.Controls;

/// <summary>Optional default model. Keys must be non-null, unique and stable across reloads.</summary>
public sealed class TreeSelectNode : INotifyPropertyChanged
{
    private object _key = Guid.NewGuid().ToString("N");
    private string _text = "";
    private bool _isEnabled = true;
    private ObservableCollection<TreeSelectNode>? _children = [];
    public object Key { get => _key; set => Set(ref _key, value); }
    public string Text { get => _text; set => Set(ref _text, value); }
    public bool IsEnabled { get => _isEnabled; set => Set(ref _isEnabled, value); }
    public ObservableCollection<TreeSelectNode>? Children { get => _children; set => Set(ref _children, value); }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
