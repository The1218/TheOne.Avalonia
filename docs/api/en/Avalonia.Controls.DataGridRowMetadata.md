# DataGridRowMetadata

[English](../en/Avalonia.Controls.DataGridRowMetadata.md) · [简体中文](../zh-cn/Avalonia.Controls.DataGridRowMetadata.md) · [API index](README.md)

`Avalonia.Controls.DataGridRowMetadata` · Class · `object`

Explicit metadata for native DataGrid editing, path sorting and automatic columns.

[Source / 声明](../../../vendor/Avalonia.Controls.DataGrid/DataGridRowMetadata.cs#L15)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static Type DataGridRowMetadata.Register<T>()`](../../../vendor/Avalonia.Controls.DataGrid/DataGridRowMetadata.cs#L27) | Registers only public properties and the parameterless constructor of a known row type. Register nested property and custom comparable types separately. |
| [`static void DataGridRowMetadata.RegisterCollection<TCollection, TRow>() where TCollection : IEnumerable<TRow>`](../../../vendor/Avalonia.Controls.DataGrid/DataGridRowMetadata.cs#L44) | Registers an additional collection shape, including empty custom collections. |
