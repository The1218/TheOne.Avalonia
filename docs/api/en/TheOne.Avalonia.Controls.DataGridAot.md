# DataGridAot

[English](../en/TheOne.Avalonia.Controls.DataGridAot.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DataGridAot.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DataGridAot` · Class · `object`

Registers public row properties and the parameterless constructor used by the native DataGrid. Compiled column bindings alone do not retain metadata needed for editing, sorting and grouping.

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridAot.cs#L10)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static Type DataGridAot.PreserveRowType<T>()`](../../../TheOne.Avalonia/Controls/DataGridAot.cs#L19) | Call before creating views or grids for each row or nested property type. Only that type's public properties and public parameterless constructor are retained. |
