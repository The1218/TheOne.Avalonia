# RelationGraph

[English](../en/TheOne.Avalonia.Controls.RelationGraph.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.RelationGraph.md) · [API index](README.md)

`TheOne.Avalonia.Controls.RelationGraph` · Class · `Avalonia.Controls.Control`

A single-root tree with native node cards and directly drawn orthogonal connectors.

[Source / 声明](../../../TheOne.Avalonia/Controls/RelationGraph.Layout.cs#L7)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Background`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L53) | `Avalonia.Media.IBrush?` | writable; OneWay | `inherited registration` | Background brush; use DynamicResource for theme-dependent surfaces. |
| [`ChildrenSource`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L60) | `Avalonia.Data.BindingBase?` | writable; OneWay | `null` | A binding evaluated against each node, not against the graph's data context. |
| [`ConnectorBrush`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L73) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | One.Accent / 1.5 ; Connector color and width; 0 width hides connectors |
| [`ConnectorCornerRadius`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L75) | `double` | writable; OneWay | `8` | 8 / 3 ; Orthogonal corner and endpoint radii Validation: `NonNegative` |
| [`ConnectorThickness`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L74) | `double` | writable; OneWay | `1.5` | One.Accent / 1.5 ; Connector color and width; 0 width hides connectors Validation: `NonNegative` |
| [`ContentSource`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L63) | `Avalonia.Data.BindingBase?` | writable; OneWay | `null` | Typed accessor for relation-node body content, without reflection paths. |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L66) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`IconSource`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L61) | `Avalonia.Data.BindingBase?` | writable; OneWay | `null` | Typed accessor for relation-node icons. |
| [`IconTemplate`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L64) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`LevelSpacing`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L71) | `double` | writable; OneWay | `80` | 80 ; Depth spacing; each level uses its largest node height or width Validation: `NonNegative` |
| [`NodeCount`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L79) | `int` | read-only; OneWay | `computed / state` | Read-only ; Displayed valid node count / validation error |
| [`NodeMinHeight`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L69) | `double` | writable; OneWay | `88` | 260 / 88 ; Fixed card width / minimum natural height Validation: `Positive` |
| [`NodeSpacing`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L70) | `double` | writable; OneWay | `36` | 36 ; Cross-axis spacing between subtrees Validation: `NonNegative` |
| [`NodeTheme`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L67) | `Avalonia.Styling.ControlTheme?` | writable; OneWay | `null` | Default theme ; Replace the `RelationGraphNodePresenter` ControlTheme |
| [`NodeWidth`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L68) | `double` | writable; OneWay | `260` | 260 / 88 ; Fixed card width / minimum natural height Validation: `Positive` |
| [`Orientation`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L58) | `Avalonia.Layout.Orientation` | writable; OneWay | `Orientation.Horizontal` | Horizontal ; Tree expansion direction Validation: `Enum.IsDefined` Values: `Horizontal`, `Vertical` |
| [`Padding`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L72) | `Avalonia.Thickness` | writable; OneWay | `inherited registration` | 16 ; Graph edge insets |
| [`PortRadius`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L76) | `double` | writable; OneWay | `3` | 8 / 3 ; Orthogonal corner and endpoint radii Validation: `NonNegative` |
| [`RootNode`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L55) | `object?` | writable; OneWay | `null` | Root model for a relation graph; children come from its tree/accessor contract. |
| [`SelectItem`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L57) | `object?` | writable; TwoWay | `null` | The selected node model, matched by reference. Null clears selection. |
| [`TitleSource`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L62) | `Avalonia.Data.BindingBase?` | writable; OneWay | `null` | Typed accessor for relation-node titles. |
| [`TitleTemplate`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L65) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`ValidationError`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L78) | `string?` | read-only; OneWay | `computed / state` | Read-only ; Displayed valid node count / validation error |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`override void RelationGraph.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/RelationGraph.Layout.cs#L66) | Avalonia drawing callback using the supplied DrawingContext; business code normally does not call it directly. |
| [`RelationGraphNodePresenter? RelationGraph.GetNodePresenter(object node)`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L81) | Finds a realized native card; returns null before layout or for invalid trees. |
| [`void RelationGraph.RefreshData()`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L83) | Refresh after changing a collection that does not notify. |
| [`RelationGraph.RelationGraph()`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L52) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<IBrush?> RelationGraph.BackgroundProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L20) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> RelationGraph.RootNodeProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L21) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> RelationGraph.SelectItemProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<Orientation> RelationGraph.OrientationProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L23) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<BindingBase?> RelationGraph.ChildrenSourceProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L24) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<BindingBase?> RelationGraph.IconSourceProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L25) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<BindingBase?> RelationGraph.TitleSourceProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L26) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<BindingBase?> RelationGraph.ContentSourceProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L27) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> RelationGraph.IconTemplateProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L28) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> RelationGraph.TitleTemplateProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L29) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> RelationGraph.ContentTemplateProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L30) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<ControlTheme?> RelationGraph.NodeThemeProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L31) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RelationGraph.NodeWidthProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L32) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RelationGraph.NodeMinHeightProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L33) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RelationGraph.NodeSpacingProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L34) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RelationGraph.LevelSpacingProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L35) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<Thickness> RelationGraph.PaddingProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L36) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> RelationGraph.ConnectorBrushProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L37) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RelationGraph.ConnectorThicknessProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L38) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RelationGraph.ConnectorCornerRadiusProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L39) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RelationGraph.PortRadiusProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L40) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<RelationGraph, string?> RelationGraph.ValidationErrorProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L41) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<RelationGraph, int> RelationGraph.NodeCountProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L42) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.RelationGraphNode`](TheOne.Avalonia.Controls.RelationGraphNode.md)
- [`TheOne.Avalonia.Controls.RelationGraphNodePresenter`](TheOne.Avalonia.Controls.RelationGraphNodePresenter.md)
