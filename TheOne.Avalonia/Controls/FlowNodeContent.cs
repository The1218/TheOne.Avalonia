using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls.Templates;

namespace TheOne.Avalonia.Controls;

public enum FlowNodeStatus
{
    Pending,
    InProgress,
    Completed
}

/// <summary>The four optional fields displayed by the default FlowNode template.</summary>
public sealed class FlowNodeContent : INotifyPropertyChanged
{
    private string? _title, _time, _state, _text;

    public string? Title
    {
        get => _title;
        set
        {
            if (Set(ref _title, value)) Changed(nameof(HasTitle));
        }
    }

    public string? Time
    {
        get => _time;
        set
        {
            if (Set(ref _time, value)) Changed(nameof(HasTime));
        }
    }

    public string? State
    {
        get => _state;
        set => Set(ref _state, value);
    }

    public string? Text
    {
        get => _text;
        set
        {
            if (Set(ref _text, value)) Changed(nameof(HasText));
        }
    }

    public bool HasTime => !string.IsNullOrWhiteSpace(Time);
    public bool HasTitle => !string.IsNullOrWhiteSpace(Title);
    public bool HasText => !string.IsNullOrWhiteSpace(Text);
    public event PropertyChangedEventHandler? PropertyChanged;

    private void Changed(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private bool Set(ref string? field, string? value, [CallerMemberName] string name = "")
    {
        if (field == value) return false;
        field = value;
        Changed(name);
        return true;
    }
}

/// <summary>A bindable flow item. Content can be default fields, a control, or a custom model.</summary>
public sealed class FlowNodeItem : INotifyPropertyChanged
{
    private object? _content;
    private IDataTemplate? _contentTemplate;
    private FlowNodeStatus _status;

    public FlowNodeStatus Status
    {
        get => _status;
        set
        {
            if (!Enum.IsDefined(value)) throw new ArgumentOutOfRangeException(nameof(value));
            Set(ref _status, value);
        }
    }

    public object? Content
    {
        get => _content;
        set => Set(ref _content, value);
    }

    public IDataTemplate? ContentTemplate
    {
        get => _contentTemplate;
        set => Set(ref _contentTemplate, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void Set<T>(ref T field, T value, [CallerMemberName] string name = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}