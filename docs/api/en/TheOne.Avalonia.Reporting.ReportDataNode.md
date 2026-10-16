# ReportDataNode

[English](../en/TheOne.Avalonia.Reporting.ReportDataNode.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportDataNode.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportDataNode` · Class · `TheOne.Avalonia.Reporting.ReportObservable`

[Inherited additional members: ReportObservable](TheOne.Avalonia.Reporting.ReportObservable.md)

A JSON response node, or a field from a consumer supplied typed source.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportDataNode.cs#L6)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Children`](../../../TheOne.Avalonia/Reporting/ReportDataNode.cs#L17) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Reporting.ReportDataNode>` | read-only; CLR / n/a | `accessor-defined / state` | Child model collection; retain identity/parent relationships and avoid cycles/shared mutable nodes. |
| [`FieldKey`](../../../TheOne.Avalonia/Reporting/ReportDataNode.cs#L18) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Explicitly registered field key rather than a reflection path; must match a source field. |
| [`IsExpanded`](../../../TheOne.Avalonia/Reporting/ReportDataNode.cs#L20) | `bool` | writable; CLR / n/a | `false` | Current node expansion; bind to a stable model when it must survive recycling or reload. |
| [`IsJson`](../../../TheOne.Avalonia/Reporting/ReportDataNode.cs#L19) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Kind`](../../../TheOne.Avalonia/Reporting/ReportDataNode.cs#L14) | `TheOne.Avalonia.Reporting.ReportDataNodeKind` | read-only; CLR / n/a | `accessor-defined / state` | Kind enum for the owning field/property/element; see its concrete enum for choices. Values: `Object`, `Scalar`, `Collection` |
| [`Name`](../../../TheOne.Avalonia/Reporting/ReportDataNode.cs#L12) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Name used for presentation/identity; keep it consistent with templates and keys. |
| [`Pointer`](../../../TheOne.Avalonia/Reporting/ReportDataNode.cs#L13) | `string` | read-only; CLR / n/a | `accessor-defined / state` | JSON Pointer path into source JSON; see source documentation for path/root semantics. |
| [`Preview`](../../../TheOne.Avalonia/Reporting/ReportDataNode.cs#L16) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`TypeLabel`](../../../TheOne.Avalonia/Reporting/ReportDataNode.cs#L15) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Reporting.ReportDataNodeKind`](TheOne.Avalonia.Reporting.ReportDataNodeKind.md)
