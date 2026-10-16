# NavMenuItem

[English](../en/TheOne.Avalonia.Controls.NavMenuItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.NavMenuItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.NavMenuItem` · Class · `Avalonia.Controls.TreeViewItem`

A recycled native tree container; group rows expand without navigating.

[Source / 声明](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L14)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IsActiveAncestor`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L22) | `bool` | writable; OneWay | `false` | Whether this item is an ancestor of the selected item; menu-owned and distinct from child selection. |
| [`IsCascadeItem`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L20) | `bool` | writable; OneWay | `false` | Whether this container belongs to a cascading popup, maintained by the menu/template. |
| [`IsCompact`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L21) | `bool` | writable; OneWay | `false` | Enables compact layout; the template determines visible fields without altering business data. |
| [`IsGroup`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L23) | `bool` | writable; OneWay | `false` | Whether this menu item is a group, distinct from selected state. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`NavMenuItem.NavMenuItem()`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L38) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<bool> NavMenuItem.IsGroupProperty`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L16) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> NavMenuItem.IsCompactProperty`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L17) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> NavMenuItem.IsActiveAncestorProperty`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> NavMenuItem.IsCascadeItemProperty`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.
