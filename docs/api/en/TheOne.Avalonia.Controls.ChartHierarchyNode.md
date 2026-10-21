# ChartHierarchyNode

[English](../en/TheOne.Avalonia.Controls.ChartHierarchyNode.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ChartHierarchyNode.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ChartHierarchyNode` · Class · `TheOne.Avalonia.Controls.ChartPoint`

[Inherited additional members: ChartPoint](TheOne.Avalonia.Controls.ChartPoint.md)

A hierarchy item. Label names the item; Y supplies a positive leaf weight.

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartHierarchyData.cs#L8)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Brush`](../../../TheOne.Avalonia/Controls/ChartHierarchyData.cs#L31) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Optional branch color; descendants inherit the nearest explicitly assigned brush. |
| [`Children`](../../../TheOne.Avalonia/Controls/ChartHierarchyData.cs#L29) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.ChartHierarchyNode>?` | writable; OneWay | `null` | Child weights replace the parent's Y value when this collection is nonempty. |
| [`Key`](../../../TheOne.Avalonia/Controls/ChartHierarchyData.cs#L27) | `string?` | writable; OneWay | `null` | Optional application identity. Unique keys can retain selection across source replacement. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ChartHierarchyNode.ChartHierarchyNode()`](../../../TheOne.Avalonia/Controls/ChartHierarchyData.cs#L17) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |
| [`ChartHierarchyNode.ChartHierarchyNode(string? label, double value, string? key = null)`](../../../TheOne.Avalonia/Controls/ChartHierarchyData.cs#L19) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<string?> ChartHierarchyNode.KeyProperty`](../../../TheOne.Avalonia/Controls/ChartHierarchyData.cs#L10) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IList<ChartHierarchyNode>?> ChartHierarchyNode.ChildrenProperty`](../../../TheOne.Avalonia/Controls/ChartHierarchyData.cs#L12) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> ChartHierarchyNode.BrushProperty`](../../../TheOne.Avalonia/Controls/ChartHierarchyData.cs#L14) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
