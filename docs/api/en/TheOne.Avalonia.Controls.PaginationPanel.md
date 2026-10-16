# PaginationPanel

[English](../en/TheOne.Avalonia.Controls.PaginationPanel.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.PaginationPanel.md) · [API index](README.md)

`TheOne.Avalonia.Controls.PaginationPanel` · Class · `Avalonia.Controls.Panel`

Places three standard groups in a wrapping row, or navigation above a compact summary.

[Source / 声明](../../../TheOne.Avalonia/Controls/PaginationPanel.cs#L7)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IsCompact`](../../../TheOne.Avalonia/Controls/PaginationPanel.cs#L10) | `bool` | writable; OneWay | `false` | Enables compact layout; the template determines visible fields without altering business data. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`PaginationPanel.PaginationPanel()`](../../../TheOne.Avalonia/Controls/PaginationPanel.cs#L7) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<bool> PaginationPanel.IsCompactProperty`](../../../TheOne.Avalonia/Controls/PaginationPanel.cs#L9) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.
