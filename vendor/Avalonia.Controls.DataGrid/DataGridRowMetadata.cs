// TheOne extension; see LICENSE.md.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Avalonia.Collections;

namespace Avalonia.Controls;

/// <summary>Explicit metadata for native DataGrid editing, path sorting and automatic columns.</summary>
public static class DataGridRowMetadata
{
    private static readonly ConcurrentDictionary<Type, Metadata> Rows = new();
    private static readonly ConcurrentDictionary<Type, Type> Collections = new();

    /// <summary>
    ///     Registers only public properties and the parameterless constructor of a known row type.
    ///     Register nested property and custom comparable types separately.
    /// </summary>
    [return:
        DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                   DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public static Type Register<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        T>()
    {
        var type = typeof(T);
        Rows.TryAdd(type, new Metadata(type.GetProperties(BindingFlags.Public | BindingFlags.Instance),
            type.GetConstructor(Type.EmptyTypes), Comparer<T>.Default));
        Collections.TryAdd(typeof(List<T>), type);
        Collections.TryAdd(typeof(ObservableCollection<T>), type);
        Collections.TryAdd(typeof(T[]), type);
        Collections.TryAdd(typeof(IEnumerable<T>), type);
        Collections.TryAdd(typeof(DataGridCollectionView<T>), type);
        return type;
    }

    /// <summary>Registers an additional collection shape, including empty custom collections.</summary>
    public static void RegisterCollection<TCollection, TRow>() where TCollection : IEnumerable<TRow>
    {
        Collections[typeof(TCollection)] = typeof(TRow);
    }

    internal static Type? GetItemType(Type collectionType)
    {
        return Collections.TryGetValue(collectionType, out var type) ? type : null;
    }

    internal static PropertyInfo[] GetProperties(Type type)
    {
        return Rows.TryGetValue(type, out var metadata)
            ? metadata.Properties
            : throw new InvalidOperationException(
                $"Register DataGrid row metadata for '{type}' before using property paths or automatic columns.");
    }

    internal static ConstructorInfo? GetConstructor(Type type)
    {
        return Rows.TryGetValue(type, out var metadata) ? metadata.Constructor : null;
    }

    internal static IComparer GetComparer(Type type)
    {
        return Rows.TryGetValue(type, out var metadata) ? metadata.Comparer : Comparer.Default;
    }

    private sealed record Metadata(PropertyInfo[] Properties, ConstructorInfo? Constructor, IComparer Comparer);
}