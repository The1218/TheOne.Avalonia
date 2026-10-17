using System.ComponentModel;
using System.Globalization;
using Avalonia.Controls.Templates;

namespace TheOne.Avalonia.Controls;

/// <summary>Explicit, trimming-safe text and availability accessors for model-backed choices.</summary>
public abstract class MultiSelectItemAdapter
{
    public abstract string GetText(object item);

    public virtual bool IsEnabled(object item)
    {
        return true;
    }

    public static MultiSelectItemAdapter Create<T>(Func<T, string> text, Func<T, bool>? isEnabled = null)
    {
        ArgumentNullException.ThrowIfNull(text);
        return new TypedAdapter<T>(text, isEnabled);
    }

    private sealed class TypedAdapter<T>(Func<T, string> text, Func<T, bool>? enabled) : MultiSelectItemAdapter
    {
        private static T Cast(object item)
        {
            return item is T value
                ? value
                : throw new ArgumentException($"This adapter requires {typeof(T).Name} items.", nameof(item));
        }

        public override string GetText(object item)
        {
            return text(Cast(item)) ?? "";
        }

        public override bool IsEnabled(object item)
        {
            return enabled?.Invoke(Cast(item)) ?? true;
        }
    }
}

/// <summary>Read-only presentation state used by the default result and tag templates.</summary>
public sealed class MultiSelectBoxItem : INotifyPropertyChanged
{
    private readonly MultiSelectBox _owner;

    internal MultiSelectBoxItem(MultiSelectBox owner, object item)
    {
        _owner = owner;
        Item = item;
    }

    public object Item { get; }
    public string Text { get; private set; } = "";
    public bool IsEnabled { get; private set; }
    public bool IsSelected { get; private set; }
    public bool CanRemove => IsEnabled && _owner.CanEditSelection;
    public string RemoveLabel => string.Format(CultureInfo.CurrentCulture, _owner.RemoveTextFormat, Text);
    public object DisplayContent => _owner.ItemTemplate is null ? Text : Item;
    public IDataTemplate? ContentTemplate => _owner.ItemTemplate;
    public object TagContent => _owner.SelectedItemTemplate is null ? Text : Item;
    public IDataTemplate? TagTemplate => _owner.SelectedItemTemplate;
    public event PropertyChangedEventHandler? PropertyChanged;

    internal void Update(bool selected)
    {
        Text = _owner.ItemAdapter?.GetText(Item) ?? Item.ToString() ?? "";
        IsEnabled = _owner.ItemAdapter?.IsEnabled(Item) ?? true;
        IsSelected = selected;
        RefreshPresentation();
    }

    internal void RefreshPresentation()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }
}
