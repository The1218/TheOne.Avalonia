using System.Runtime.CompilerServices;
using global::Avalonia.Controls;
using global::Avalonia.Data;

namespace TheOne.Avalonia.Controls;

internal static class DataGridAutoFields
{
    private sealed record Cached(BindingBase? Binding, CompiledBindingPath? Path, IDataGridValueAccessor? Accessor);
    private static readonly ConditionalWeakTable<DataGridBoundColumn, Cached> Cache = new();
    public static IDataGridValueAccessor? Resolve(DataGridColumn column)
    {
        if (DataGridColumnOptions.GetValueAccessor(column) is { } configured) return configured;
        if (column is not DataGridBoundColumn bound) return null;
        var path = (bound.Binding as CompiledBinding)?.Path;
        if (Cache.TryGetValue(bound, out var cached) && ReferenceEquals(cached.Binding, bound.Binding) && ReferenceEquals(cached.Path, path)) return cached.Accessor;
        var reader = DataGridColumnValueReader.Create(bound);
        IDataGridValueAccessor? accessor = reader is null ? null : new AutomaticAccessor(reader);
        Cache.Remove(bound); Cache.Add(bound, new(bound.Binding, path, accessor));
        return accessor;
    }
    internal sealed class AutomaticAccessor(DataGridColumnValueReader reader) : IDataGridValueAccessor
    {
        public Type ValueType => reader.ValueType;
        public object? GetValue(object item) => reader.GetValue(item);
        public object? GetValue(object item, Action<object> observe) => reader.GetValue(item, observe);
    }
}
