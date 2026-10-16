# WorkflowProperty

[English](../en/TheOne.Avalonia.Workflow.WorkflowProperty.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowProperty.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowProperty` · Class · `TheOne.Avalonia.Workflow.WorkflowObservable`

[Inherited additional members: WorkflowObservable](TheOne.Avalonia.Workflow.WorkflowObservable.md)

A literal or an upstream reference. DataType is a host-defined token, e.g. text, number, email.

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L33)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`DataType`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L39) | `string` | writable; CLR / init-only | `"text"` | Host-defined type key such as flow/text/number for port/reference compatibility, not a CLR type name. |
| [`DisplayValue`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L44) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsRequired`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L41) | `bool` | writable; CLR / init-only | `false` | Whether a field/workflow property is required; validation follows its owning model contract. |
| [`Key`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L37) | `string` | writable; CLR / init-only | `""` | Field/parameter key used to resolve business values, distinct from display names. |
| [`Kind`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L40) | `TheOne.Avalonia.Workflow.WorkflowValueKind` | writable; CLR / init-only | `0 / default` | Kind enum for the owning field/property/element; see its concrete enum for choices. Values: `Text`, `Number`, `Boolean` |
| [`Reference`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L43) | `TheOne.Avalonia.Workflow.WorkflowDataReference?` | writable; CLR / n/a | `null` | Structured upstream-output reference; null uses the literal value, without persisting resolved runtime data. |
| [`Title`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L38) | `string` | writable; CLR / init-only | `""` | Display title, distinct from business identity or a localization key. |
| [`Value`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L42) | `string` | writable; CLR / n/a | `""` | Current value; units, range and null semantics are defined by its model and validation constraints. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`WorkflowProperty.WorkflowProperty()`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L33) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Workflow.WorkflowDataReference`](TheOne.Avalonia.Workflow.WorkflowDataReference.md)
- [`TheOne.Avalonia.Workflow.WorkflowValueKind`](TheOne.Avalonia.Workflow.WorkflowValueKind.md)
