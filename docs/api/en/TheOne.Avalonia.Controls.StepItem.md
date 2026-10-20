# StepItem

[English](../en/TheOne.Avalonia.Controls.StepItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.StepItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.StepItem` · Class · `object`

Stable step identity; content is a model resolved lazily by the VerificationStep's data templates.

[Source / 声明](../../../TheOne.Avalonia/Controls/StepItem.cs#L17)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Content`](../../../TheOne.Avalonia/Controls/StepItem.cs#L54) | `object?` | writable; CLR / n/a | `null` | Host content/presentation object; follow the content-template scope and do not persist visual objects as business data. |
| [`Description`](../../../TheOne.Avalonia/Controls/StepItem.cs#L48) | `string` | writable; CLR / n/a | `""` | Descriptive text supplied by the host; omitted values keep their initial state. |
| [`HasNext`](../../../TheOne.Avalonia/Controls/StepItem.cs#L32) | `bool` | read-only; CLR / n/a | `false` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HasPrevious`](../../../TheOne.Avalonia/Controls/StepItem.cs#L26) | `bool` | read-only; CLR / n/a | `false` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Indicator`](../../../TheOne.Avalonia/Controls/StepItem.cs#L98) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IndicatorVariant`](../../../TheOne.Avalonia/Controls/StepItem.cs#L40) | `TheOne.Avalonia.Controls.TagVariant` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. Values: `Soft`, `Outline`, `Solid` |
| [`IsReached`](../../../TheOne.Avalonia/Controls/StepItem.cs#L38) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsSuccess`](../../../TheOne.Avalonia/Controls/StepItem.cs#L39) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Number`](../../../TheOne.Avalonia/Controls/StepItem.cs#L75) | `int` | read-only; CLR / n/a | `0 / default` | Page number in the pagination result, used for page-number rendering. |
| [`State`](../../../TheOne.Avalonia/Controls/StepItem.cs#L84) | `TheOne.Avalonia.Controls.StepState` | read-only; CLR / n/a | `0 / default` | Model state enum; see its values and owning control for persistence/behavior semantics. Values: `Waiting`, `Current`, `Success`, `Error` |
| [`Status`](../../../TheOne.Avalonia/Controls/StepItem.cs#L103) | `TheOne.Avalonia.Controls.TagStatus` | read-only; CLR / n/a | `accessor-defined / state` | Business node-state enum; styles derive from state rather than storing state in classes. Values: `Default`, `Information`, `Success`, `Warning`, `Error`, `Processing` |
| [`Title`](../../../TheOne.Avalonia/Controls/StepItem.cs#L42) | `string` | writable; CLR / n/a | `""` | Display title, distinct from business identity or a localization key. |
| [`Validator`](../../../TheOne.Avalonia/Controls/StepItem.cs#L66) | `TheOne.Avalonia.Controls.IStepValidator?` | writable; CLR / n/a | `null` | Step validator instance whose errors identify members, following VerificationStep validation contracts. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`StepItem.StepItem()`](../../../TheOne.Avalonia/Controls/StepItem.cs#L17) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event PropertyChangedEventHandler? StepItem.PropertyChanged`](../../../TheOne.Avalonia/Controls/StepItem.cs#L109) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.IStepValidator`](TheOne.Avalonia.Controls.IStepValidator.md)
- [`TheOne.Avalonia.Controls.StepState`](TheOne.Avalonia.Controls.StepState.md)
- [`TheOne.Avalonia.Controls.TagStatus`](TheOne.Avalonia.Controls.TagStatus.md)
- [`TheOne.Avalonia.Controls.TagVariant`](TheOne.Avalonia.Controls.TagVariant.md)
