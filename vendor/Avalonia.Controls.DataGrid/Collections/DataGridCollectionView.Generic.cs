// TheOne extension of the native collection view; see LICENSE.md.

using System;
using System.Collections.Generic;

namespace Avalonia.Collections;

/// <summary>Native sorting, filtering and grouping with an explicit row type for compiled bindings.</summary>
public sealed class DataGridCollectionView<T> : DataGridCollectionView, IEnumerable<T>
{
    public DataGridCollectionView(IEnumerable<T> source) : base(source)
    {
    }

    public DataGridCollectionView(IEnumerable<T> source, bool isDataSorted, bool isDataInGroupOrder)
        : base(source, isDataSorted, isDataInGroupOrder)
    {
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        var enumerator = base.GetEnumerator();
        try
        {
            while (enumerator.MoveNext()) yield return (T)enumerator.Current;
        }
        finally
        {
            (enumerator as IDisposable)?.Dispose();
        }
    }
}