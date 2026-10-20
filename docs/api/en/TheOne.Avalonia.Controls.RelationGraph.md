# RelationGraph

[English](../en/TheOne.Avalonia.Controls.RelationGraph.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.RelationGraph.md) · [API index](README.md)

`TheOne.Avalonia.Controls.RelationGraph` · Class · `Avalonia.Controls.Control`

A single-root tree with native node cards and directly drawn orthogonal connectors.

[Source / 声明](../../../TheOne.Avalonia/Controls/RelationGraph.Layout.cs#L7)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Background`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L107) | `Avalonia.Media.IBrush?` | writable; OneWay | `inherited registration` | Background brush; use DynamicResource for theme-dependent surfaces. |
| [`ChildrenSource`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L134) | `Avalonia.Data.BindingBase?` | writable; OneWay | `null` | A binding evaluated against each node, not against the graph's data context. |
| [`ConnectorBrush`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L215) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | One.Accent / 1.5 ; Connector color and width; 0 width hides connectors |
| [`ConnectorCornerRadius`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L227) | `double` | writable; OneWay | `8` | 8 / 3 ; Orthogonal corner and endpoint radii Validation: `NonNegative` |
| [`ConnectorThickness`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L221) | `double` | writable; OneWay | `1.5` | One.Accent / 1.5 ; Connector color and width; 0 width hides connectors Validation: `NonNegative` |
| [`ContentSource`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L155) | `Avalonia.Data.BindingBase?` | writable; OneWay | `null` | Typed accessor for relation-node body content, without reflection paths. |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L173) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`IconSource`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L141) | `Avalonia.Data.BindingBase?` | writable; OneWay | `null` | Typed accessor for relation-node icons. |
| [`IconTemplate`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L161) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`LevelSpacing`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L203) | `double` | writable; OneWay | `80` | 80 ; Depth spacing; each level uses its largest node height or width Validation: `NonNegative` |
| [`NodeCount`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L242) | `int` | read-only; OneWay | `computed / state` | Read-only ; Displayed valid node count / validation error |
| [`NodeMinHeight`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L191) | `double` | writable; OneWay | `88` | 260 / 88 ; Fixed card width / minimum natural height Validation: `Positive` |
| [`NodeSpacing`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L197) | `double` | writable; OneWay | `36` | 36 ; Cross-axis spacing between subtrees Validation: `NonNegative` |
| [`NodeTheme`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L179) | `Avalonia.Styling.ControlTheme?` | writable; OneWay | `null` | Default theme ; Replace the `RelationGraphNodePresenter` ControlTheme |
| [`NodeWidth`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L185) | `double` | writable; OneWay | `260` | 260 / 88 ; Fixed card width / minimum natural height Validation: `Positive` |
| [`Orientation`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L126) | `Avalonia.Layout.Orientation` | writable; OneWay | `0 / default` | Horizontal ; Tree expansion direction Validation: `Enum.IsDefined` Values: `Horizontal`, `Vertical` |
| [`Padding`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L209) | `Avalonia.Thickness` | writable; OneWay | `inherited registration` | 16 ; Graph edge insets |
| [`PortRadius`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L233) | `double` | writable; OneWay | `3` | 8 / 3 ; Orthogonal corner and endpoint radii Validation: `NonNegative` |
| [`RootNode`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L113) | `object?` | writable; OneWay | `null` | Root model for a relation graph; children come from its tree/accessor contract. |
| [`SelectItem`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L120) | `object?` | writable; TwoWay | `null` | The selected node model, matched by reference. Null clears selection. |
| [`TitleSource`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L148) | `Avalonia.Data.BindingBase?` | writable; OneWay | `null` | Typed accessor for relation-node titles. |
| [`TitleTemplate`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L167) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`ValidationError`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L240) | `string?` | read-only; OneWay | `computed / state` | Read-only ; Displayed valid node count / validation error |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`override void RelationGraph.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/RelationGraph.Layout.cs#L78) | Avalonia drawing callback using the supplied DrawingContext; business code normally does not call it directly. |
| [`RelationGraphNodePresenter? RelationGraph.GetNodePresenter(object node)`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L246) | Finds a realized native card; returns null before layout or for invalid trees. |
| [`void RelationGraph.RefreshData()`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L252) | Refresh after changing a collection that does not notify. |
| [`RelationGraph.RelationGraph()`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L98) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<IBrush?> RelationGraph.BackgroundProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L20) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> RelationGraph.RootNodeProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L23) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> RelationGraph.SelectItemProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L26) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<Orientation> RelationGraph.OrientationProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L29) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<BindingBase?> RelationGraph.ChildrenSourceProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L32) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<BindingBase?> RelationGraph.IconSourceProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L35) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<BindingBase?> RelationGraph.TitleSourceProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L38) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<BindingBase?> RelationGraph.ContentSourceProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L41) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> RelationGraph.IconTemplateProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L44) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> RelationGraph.TitleTemplateProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L47) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> RelationGraph.ContentTemplateProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L50) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<ControlTheme?> RelationGraph.NodeThemeProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L53) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RelationGraph.NodeWidthProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L56) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RelationGraph.NodeMinHeightProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L59) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RelationGraph.NodeSpacingProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L62) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RelationGraph.LevelSpacingProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L65) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<Thickness> RelationGraph.PaddingProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L68) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> RelationGraph.ConnectorBrushProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L71) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RelationGraph.ConnectorThicknessProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L74) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RelationGraph.ConnectorCornerRadiusProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L77) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RelationGraph.PortRadiusProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L80) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<RelationGraph, string?> RelationGraph.ValidationErrorProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L83) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<RelationGraph, int> RelationGraph.NodeCountProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L87) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.RelationGraphNode`](TheOne.Avalonia.Controls.RelationGraphNode.md)
- [`TheOne.Avalonia.Controls.RelationGraphNodePresenter`](TheOne.Avalonia.Controls.RelationGraphNodePresenter.md)
