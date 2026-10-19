using System.Collections;

namespace TheOne.Avalonia.Controls;

/// <summary>Explicit, trimming-safe mapping from application models to tree identities and children.</summary>
public abstract class TreeSelectItemAdapter
{
    public abstract object GetKey(object item);
    public abstract string GetText(object item);
    public abstract IEnumerable? GetChildren(object item);
    public virtual bool IsEnabled(object item) => true;

    public static TreeSelectItemAdapter Create<T>(Func<T, object> key, Func<T, string> text,
        Func<T, IEnumerable?> children, Func<T, bool>? isEnabled = null)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(children);
        return new TypedAdapter<T>(key, text, children, isEnabled);
    }

    private sealed class TypedAdapter<T>(Func<T, object> key, Func<T, string> text,
        Func<T, IEnumerable?> children, Func<T, bool>? enabled) : TreeSelectItemAdapter
    {
        private static T Cast(object item) => item is T value ? value :
            throw new ArgumentException($"This adapter requires {typeof(T).Name} items.", nameof(item));
        public override object GetKey(object item) => key(Cast(item));
        public override string GetText(object item) => text(Cast(item)) ?? "";
        public override IEnumerable? GetChildren(object item) => children(Cast(item));
        public override bool IsEnabled(object item) => enabled?.Invoke(Cast(item)) ?? true;
    }
}
