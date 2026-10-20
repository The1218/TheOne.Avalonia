# IconButtonPanel

[English](../en/TheOne.Avalonia.Controls.IconButtonPanel.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.IconButtonPanel.md) · [API index](README.md)

`TheOne.Avalonia.Controls.IconButtonPanel` · Class · `Avalonia.Controls.Panel`

Template layout: the first two children are icon and label, the third is the loading replacement.

[Source / 声明](../../../TheOne.Avalonia/Controls/IconButtonPanel.cs#L15)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IconPlacement`](../../../TheOne.Avalonia/Controls/IconButtonPanel.cs#L28) | `TheOne.Avalonia.Controls.IconPlacement` | writable; OneWay | `0 / default` | Icon position relative to text; the template arranges horizontal/vertical placement. Values: `Left`, `Right`, `Top`, `Bottom` |
| [`Spacing`](../../../TheOne.Avalonia/Controls/IconButtonPanel.cs#L34) | `double` | writable; OneWay | `8` | DIP gap between children, separate from container Padding and item Margin. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`IconButtonPanel.IconButtonPanel()`](../../../TheOne.Avalonia/Controls/IconButtonPanel.cs#L15) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<IconPlacement> IconButtonPanel.IconPlacementProperty`](../../../TheOne.Avalonia/Controls/IconButtonPanel.cs#L17) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> IconButtonPanel.SpacingProperty`](../../../TheOne.Avalonia/Controls/IconButtonPanel.cs#L20) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.IconPlacement`](TheOne.Avalonia.Controls.IconPlacement.md)
