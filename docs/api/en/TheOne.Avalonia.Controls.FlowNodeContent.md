# FlowNodeContent

[English](../en/TheOne.Avalonia.Controls.FlowNodeContent.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.FlowNodeContent.md) · [API index](README.md)

`TheOne.Avalonia.Controls.FlowNodeContent` · Class · `object`

The four optional fields displayed by the default FlowNode template.

[Source / 声明](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L15)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`HasText`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L54) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HasTime`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L52) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HasTitle`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L53) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`State`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L37) | `string?` | writable; CLR / n/a | `null` | Model state enum; see its values and owning control for persistence/behavior semantics. |
| [`Text`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L43) | `string?` | writable; CLR / n/a | `null` | Input/display text; notifications and validation follow the model/native editor contract. |
| [`Time`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L28) | `string?` | writable; CLR / n/a | `null` | Displayed node time text, independent of execution and step ordering. |
| [`Title`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L19) | `string?` | writable; CLR / n/a | `null` | Display title, distinct from business identity or a localization key. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`FlowNodeContent.FlowNodeContent()`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L15) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event PropertyChangedEventHandler? FlowNodeContent.PropertyChanged`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L55) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
