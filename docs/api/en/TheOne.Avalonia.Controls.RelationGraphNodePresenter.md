# RelationGraphNodePresenter

[English](../en/TheOne.Avalonia.Controls.RelationGraphNodePresenter.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.RelationGraphNodePresenter.md) · [API index](README.md)

`TheOne.Avalonia.Controls.RelationGraphNodePresenter` · Class · `Avalonia.Controls.Primitives.HeaderedContentControl`

A native node card. Header displays the model's Title.

[Source / 声明](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L15)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ContentForeground`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L105) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Node body text brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`HeaderFontSize`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L99) | `double` | writable; OneWay | `16` | Node-title font size in DIPs, included in actual text measurement. Validation: `Positive` |
| [`Icon`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L63) | `object?` | writable; OneWay | `null` | Icon content/vector geometry as defined by the signature; the host owns external bitmap lifetimes. |
| [`IconBackground`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L111) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Icon-tile background brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`IconForeground`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L117) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Icon foreground brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`IconSize`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L75) | `double` | writable; OneWay | `24` | Icon drawing size in DIPs, independent of font size and button hit area. Validation: `Positive` |
| [`IconSpacing`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L87) | `double` | writable; OneWay | `14` | DIP spacing between the icon and adjacent text/content. Validation: `NonNegative` |
| [`IconTemplate`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L69) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`IconTileSize`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L81) | `double` | writable; OneWay | `44` | Icon-tile size in DIPs, independent of icon size. Validation: `Positive` |
| [`IsRoot`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L123) | `bool` | writable; OneWay | `false` | Whether this is the root node, maintained by the graph presenter. |
| [`IsSelected`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L130) | `bool` | read-only; OneWay | `computed / state` | Selection state maintained by the owning RelationGraph. |
| [`TextSpacing`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L93) | `double` | writable; OneWay | `6` | DIP spacing between title and body text. Validation: `NonNegative` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`RelationGraphNodePresenter.RelationGraphNodePresenter()`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L58) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<object?> RelationGraphNodePresenter.IconProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L17) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> RelationGraphNodePresenter.IconTemplateProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L20) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RelationGraphNodePresenter.IconSizeProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L23) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RelationGraphNodePresenter.IconTileSizeProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L26) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RelationGraphNodePresenter.IconSpacingProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L29) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RelationGraphNodePresenter.TextSpacingProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L32) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RelationGraphNodePresenter.HeaderFontSizeProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L35) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> RelationGraphNodePresenter.ContentForegroundProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L38) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> RelationGraphNodePresenter.IconBackgroundProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L41) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> RelationGraphNodePresenter.IconForegroundProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L44) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> RelationGraphNodePresenter.IsRootProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L47) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<RelationGraphNodePresenter, bool> RelationGraphNodePresenter.IsSelectedProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L50) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.
