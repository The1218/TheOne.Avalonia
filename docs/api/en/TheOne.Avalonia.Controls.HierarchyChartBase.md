# HierarchyChartBase

[English](../en/TheOne.Avalonia.Controls.HierarchyChartBase.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.HierarchyChartBase.md) · [API index](README.md)

`TheOne.Avalonia.Controls.HierarchyChartBase` · Class · `TheOne.Avalonia.Controls.SpecializedChartBase`

[Inherited additional members: SpecializedChartBase](TheOne.Avalonia.Controls.SpecializedChartBase.md)

Observable hierarchy projection shared by native treemap and sunburst charts.

[Source / 声明](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L14)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanDrillUp`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L67) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HasInvalidHierarchy`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L69) | `bool` | read-only; OneWay | `computed / state` | True when duplicate keys, repeated nodes or cycles were encountered. |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L63) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.ChartHierarchyNode>?` | writable; OneWay | `null` | The authoritative forest. Do not assign the inherited SeriesSource projection. |
| [`Root`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L65) | `TheOne.Avalonia.Controls.ChartHierarchyNode?` | writable; TwoWay | `null` | The displayed branch; null displays the complete forest. Unreachable roots reset to null. |
| [`SelectedNode`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L66) | `TheOne.Avalonia.Controls.ChartHierarchyNode?` | read-only; OneWay | `computed / state` | Selected node model; null clears node selection. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`bool HierarchyChartBase.DrillDown(ChartHierarchyNode node)`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L72) | Show a reachable branch's children. Leaves and branches without positive weights are rejected. |
| [`bool HierarchyChartBase.DrillUp()`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L80) | The signature specifies parameter types, optional defaults and return types; see the owning usage contract and linked implementation for operation/error boundaries. |
| [`void HierarchyChartBase.ResetRoot()`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L87) | The signature specifies parameter types, optional defaults and return types; see the owning usage contract and linked implementation for operation/error boundaries. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<IList<ChartHierarchyNode>?> HierarchyChartBase.ItemsSourceProperty`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L16) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<ChartHierarchyNode?> HierarchyChartBase.RootProperty`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<HierarchyChartBase, ChartHierarchyNode?> HierarchyChartBase.SelectedNodeProperty`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L21) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<HierarchyChartBase, bool> HierarchyChartBase.CanDrillUpProperty`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L23) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<HierarchyChartBase, bool> HierarchyChartBase.HasInvalidHierarchyProperty`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L25) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `animation-fade` | [`^.animation-fade`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L27) | ^.animation-fade => AnimationStyle=Fade |
| `animation-none` | [`^.animation-none`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L33) | ^.animation-none => AnimationStyle=None |
| `animation-reveal` | [`^.animation-reveal`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L30) | ^.animation-reveal => AnimationStyle=Reveal |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.ChartHierarchyNode`](TheOne.Avalonia.Controls.ChartHierarchyNode.md)
