using System;
using System.Linq;
using Avalonia.Data;
using Avalonia.Data.Core;

namespace Avalonia.Controls;

/// <summary>Reads entity properties using Avalonia's generated compiled getters, before display conversion.</summary>
public sealed class DataGridColumnValueReader
{
    private readonly IPropertyInfo[] _properties;

    private DataGridColumnValueReader(IPropertyInfo[] properties)
    {
        _properties = properties;
    }

    public Type ValueType => _properties[^1].PropertyType;

    public object? GetValue(object item, Action<object>? observe = null)
    {
        var current = item;
        foreach (var property in _properties)
        {
            if (current is null) return null;
            observe?.Invoke(current);
            current = property.Get(current);
        }

        return current;
    }

    public static DataGridColumnValueReader? Create(DataGridBoundColumn column)
    {
        if (column.Binding is not CompiledBinding { Path: { } path } binding ||
            (binding.Source is not null && binding.Source != AvaloniaProperty.UnsetValue)) return null;
        // DataGrid's strong-named friend access lets us reuse the generated property
        // metadata directly. No property-name reflection, dynamic code or UI probes.
        var elements = path.Elements.ToArray();
        if (elements.Length == 0 || elements.Any(element => element is not PropertyElement)) return null;
        var properties = elements.Cast<PropertyElement>().Select(element => element.Property).ToArray();
        return properties.All(property => property.CanGet) ? new DataGridColumnValueReader(properties) : null;
    }
}