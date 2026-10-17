using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;

namespace TheOne.Avalonia.Controls;

/// <summary>
///     Registers public row properties and the parameterless constructor used by the native DataGrid.
///     Compiled column bindings alone do not retain metadata needed for editing, sorting and grouping.
/// </summary>
public static class DataGridAot
{
    /// <summary>
    ///     Call before creating views or grids for each row or nested property type.
    ///     Only that type's public properties and public parameterless constructor are retained.
    /// </summary>
    [return:
        DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                   DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public static Type PreserveRowType<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        T>()
    {
        return DataGridRowMetadata.Register<T>();
    }
}