# OneIconCatalog

[English](../en/TheOne.Avalonia.Controls.OneIconCatalog.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.OneIconCatalog.md) · [API index](README.md)

`TheOne.Avalonia.Controls.OneIconCatalog` · Class · `object`

The built-in glyphs, in stable enum order. Vector paths are authored here on a 24-unit grid.

[Source / 声明](../../../TheOne.Avalonia/Controls/OneIconCatalog.cs#L9)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`All`](../../../TheOne.Avalonia/Controls/OneIconCatalog.cs#L402) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.OneIconInfo>` | read-only; CLR / n/a | `Array.AsReadOnly(Entries.Select(entry => entry.Info).ToArray())` | Read-only metadata for all glyphs, in stable enum order. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static OneIconInfo OneIconCatalog.Get(OneIconKind kind)`](../../../TheOne.Avalonia/Controls/OneIconCatalog.cs#L405) | Gets metadata for a valid enum value. Throws for undefined values. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.OneIconInfo`](TheOne.Avalonia.Controls.OneIconInfo.md)
- [`TheOne.Avalonia.Controls.OneIconKind`](TheOne.Avalonia.Controls.OneIconKind.md)
