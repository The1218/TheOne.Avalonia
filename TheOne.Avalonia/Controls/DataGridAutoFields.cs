using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Data;

namespace TheOne.Avalonia.Controls;

internal static class DataGridAutoFields
{
    private static readonly ConditionalWeakTable<DataGridBoundColumn, Cached> Cache = new();

    public static IDataGridValueAccessor? Resolve(DataGridColumn column)
    {
        if (DataGridColumnOptions.GetValueAccessor(column) is { } configured) return configured;
        if (column is not DataGridBoundColumn bound) return null;
        var path = (bound.Binding as CompiledBinding)?.Path;
        if (Cache.TryGetValue(bound, out var cached) && ReferenceEquals(cached.Binding, bound.Binding) &&
            ReferenceEquals(cached.Path, path)) return cached.Accessor;
        var reader = DataGridColumnValueReader.Create(bound);
        IDataGridValueAccessor? accessor = reader is null ? null : new AutomaticAccessor(reader);
        Cache.Remove(bound);
        Cache.Add(bound, new Cached(bound.Binding, path, accessor));
        return accessor;
    }

    private sealed record Cached(BindingBase? Binding, CompiledBindingPath? Path, IDataGridValueAccessor? Accessor);

    internal sealed class AutomaticAccessor(DataGridColumnValueReader reader) : IDataGridValueAccessor
    {
        public Type ValueType => reader.ValueType;

        public object? GetValue(object item)
        {
            return reader.GetValue(item);
        }

        public object? GetValue(object item, Action<object> observe)
        {
            return reader.GetValue(item, observe);
        }
    }
}