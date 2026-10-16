# DataGridTreeTextColumn

[English](../en/TheOne.Avalonia.Controls.DataGridTreeTextColumn.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DataGridTreeTextColumn.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DataGridTreeTextColumn` · Class · `Avalonia.Controls.DataGridTextColumn`

Prototype tree text column. Editing uses the original native TextBox and binding.

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridTreeTextColumn.cs#L17)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Indent`](../../../TheOne.Avalonia/Controls/DataGridTreeTextColumn.cs#L21) | `double` | writable; OneWay | `16` | Additional DIP indentation per hierarchy level, without changing parent/child relationships. Validation: `value => double.IsFinite(value) && value >= 0` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`DataGridTreeTextColumn.DataGridTreeTextColumn()`](../../../TheOne.Avalonia/Controls/DataGridTreeTextColumn.cs#L17) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<double> DataGridTreeTextColumn.IndentProperty`](../../../TheOne.Avalonia/Controls/DataGridTreeTextColumn.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
