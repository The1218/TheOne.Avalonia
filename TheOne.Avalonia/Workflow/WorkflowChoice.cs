namespace TheOne.Avalonia.Workflow;

/// <summary>Compiled host projections for stable user/role keys and display text, without property reflection.</summary>
public abstract class WorkflowChoiceAdapter
{
    public abstract string GetKey(object item);
    public abstract string GetText(object item);

    public static WorkflowChoiceAdapter Create<T>(Func<T, string> key, Func<T, string> text)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(text);
        return new TypedAdapter<T>(key, text);
    }

    private sealed class TypedAdapter<T>(Func<T, string> key, Func<T, string> text) : WorkflowChoiceAdapter
    {
        private static T Model(object item) => item is T model ? model : throw new ArgumentException("Choice model type does not match its adapter.", nameof(item));
        public override string GetKey(object item) => key(Model(item));
        public override string GetText(object item) => text(Model(item)) ?? "";
    }
}

/// <summary>Presentation for a stable key. Unresolved saved keys remain editable instead of being discarded.</summary>
public sealed class WorkflowChoice(string key, string text) : WorkflowObservable
{
    public string Key { get; } = key;
    public string Text { get; internal set => Set(ref field, value); } = text;
    public bool IsAvailable { get; internal set => Set(ref field, value); } = true;
}
