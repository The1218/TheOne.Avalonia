using System;
using System.Diagnostics.CodeAnalysis;

namespace TheOne.Avalonia.Controls;

/// <summary>Preserves the public row properties inspected by the native DataGrid 12.1.2.
/// Compiled column bindings alone do not retain metadata needed for editing, sorting and grouping.</summary>
public static class DataGridAot
{
    /// <summary>Call for each concrete row type used by a trimmed or NativeAOT application.
    /// This preserves only that type's public property metadata, not the entire application assembly.</summary>
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public static Type PreserveRowType<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>() => typeof(T);
}
