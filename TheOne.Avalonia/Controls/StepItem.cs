using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace TheOne.Avalonia.Controls;

public enum StepState { Waiting, Current, Success, Error }

/// <summary>Stable step identity; content is a model resolved lazily by the Step's data templates.</summary>
public class StepItem : INotifyPropertyChanged
{
    private string _title = "", _description = "";
    private object? _content;
    private IStepValidator? _validator;
    private int _number;
    private StepState _state;
    private bool _hasPrevious, _hasNext;
    public bool HasPrevious { get => _hasPrevious; internal set => Set(ref _hasPrevious, value); }
    public bool HasNext { get => _hasNext; internal set => Set(ref _hasNext, value); }
    public bool IsReached => State != StepState.Waiting;
    public bool IsSuccess => State == StepState.Success;
    public TagVariant IndicatorVariant => State == StepState.Waiting ? TagVariant.Soft : TagVariant.Solid;
    public string Title { get => _title; set => Set(ref _title, value); }
    public string Description { get => _description; set => Set(ref _description, value); }
    public object? Content
    {
        get => _content;
        set
        {
            if (ReferenceEquals(_content, value)) return;
            PrepareContent(value);
            _content = value;
            PropertyChanged?.Invoke(this, new(nameof(Content)));
        }
    }
    public IStepValidator? Validator { get => _validator; set => Set(ref _validator, value); }
    internal virtual IStepValidator? EffectiveValidator => Validator;
    internal virtual IReadOnlyList<StepFormProperty>? FormProperties => null;
    protected virtual void PrepareContent(object? content) { }
    internal virtual void ModelChanged(string? memberName) { }
    public int Number { get => _number; internal set { if (Set(ref _number, value)) PropertyChanged?.Invoke(this, new(nameof(Indicator))); } }
    public StepState State
    {
        get => _state;
        internal set
        {
            if (!Set(ref _state, value)) return;
            PropertyChanged?.Invoke(this, new(nameof(Status)));
            PropertyChanged?.Invoke(this, new(nameof(Indicator)));
            PropertyChanged?.Invoke(this, new(nameof(IsReached)));
            PropertyChanged?.Invoke(this, new(nameof(IsSuccess)));
            PropertyChanged?.Invoke(this, new(nameof(IndicatorVariant)));
        }
    }
    public string Indicator => State switch { StepState.Success => "✓", StepState.Error => "!", _ => Number.ToString(System.Globalization.CultureInfo.CurrentCulture) };
    public TagStatus Status => State switch { StepState.Current => TagStatus.Information, StepState.Success => TagStatus.Success, StepState.Error => TagStatus.Error, _ => TagStatus.Default };
    public event PropertyChangedEventHandler? PropertyChanged;
    private bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value; PropertyChanged?.Invoke(this, new(name)); return true;
    }
}

/// <summary>A model-backed step with built-in attribute validation and an optional default form.</summary>
public sealed class StepItem<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T> : StepItem where T : class
{
    private IStepValidator? _automaticValidator;
    private IReadOnlyList<StepFormProperty>? _properties;
    public StepItem(T model) => Content = model;
    internal override IStepValidator? EffectiveValidator => Validator ?? _automaticValidator;
    internal override IReadOnlyList<StepFormProperty>? FormProperties => Content is T model ? _properties ??= StepForm.Properties(model) : null;
    internal override void ModelChanged(string? memberName)
    {
        if (_properties is null) return;
        foreach (var property in _properties.Where(property => string.IsNullOrEmpty(memberName) || property.Property.Name == memberName))
        {
            property.DraftText = null;
            property.DraftErrors = [];
        }
    }
    protected override void PrepareContent(object? content)
    {
        ArgumentNullException.ThrowIfNull(content);
        if (content.GetType() != typeof(T)) throw new ArgumentException($"This step requires a concrete {typeof(T).Name} model.", nameof(content));
        _automaticValidator = content is T model ? StepValidation.For(model) : null;
        _properties = null;
    }
}
