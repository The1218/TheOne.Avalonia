# FlowStepConnectors

[English](../en/TheOne.Avalonia.Controls.FlowStepConnectors.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.FlowStepConnectors.md) · [API index](README.md)

`TheOne.Avalonia.Controls.FlowStepConnectors` · Class · `Avalonia.Controls.Control`

A non-interactive rail layer. Draws one line per adjacent visible node.

[Source / 声明](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L9)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CompletedConnectorBrush`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L17) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Completed-step connector brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`ConnectorBrush`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L16) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Pending-step connector brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`ConnectorThickness`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L18) | `double` | writable; OneWay | `2` | Connector stroke width in DIPs, independent of business node state. Validation: `value => double.IsFinite(value) && value >= 0` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`override void FlowStepConnectors.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L19) | Avalonia drawing callback using the supplied DrawingContext; business code normally does not call it directly. |
| [`FlowStepConnectors.FlowStepConnectors()`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L9) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<IBrush?> FlowStepConnectors.ConnectorBrushProperty`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L11) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> FlowStepConnectors.CompletedConnectorBrushProperty`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L12) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> FlowStepConnectors.ConnectorThicknessProperty`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L13) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.
