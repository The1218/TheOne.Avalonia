# FlowNodeItem

[English](../en/TheOne.Avalonia.Controls.FlowNodeItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.FlowNodeItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.FlowNodeItem` · Class · `object`

A bindable flow item. Content can be default fields, a control, or a custom model.

[Source / 声明](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L30)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Content`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L44) | `object?` | writable; CLR / n/a | `null` | Host content/presentation object; follow the content-template scope and do not persist visual objects as business data. |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L45) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; CLR / n/a | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`Status`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L35) | `TheOne.Avalonia.Controls.FlowNodeStatus` | writable; CLR / n/a | `0 / default` | Business node-state enum; styles derive from state rather than storing state in classes. Values: `Pending`, `InProgress`, `Completed` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`FlowNodeItem.FlowNodeItem()`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L30) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event PropertyChangedEventHandler? FlowNodeItem.PropertyChanged`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L46) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.FlowNodeStatus`](TheOne.Avalonia.Controls.FlowNodeStatus.md)
