# Form

[English](../en/TheOne.Avalonia.Controls.Form.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.Form.md) · [API index](README.md)

`TheOne.Avalonia.Controls.Form` · Class · `Avalonia.Controls.ContentControl`

Validates caller-owned, compiled-bound fields using the same validators as VerificationStep.

[Source / 声明](../../../TheOne.Avalonia/Controls/Form.cs#L15)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanSubmit`](../../../TheOne.Avalonia/Controls/Form.cs#L56) | `bool` | read-only; OneWay | `computed / state` | Read-only; a submission attempt is currently allowed, not a guarantee of valid data |
| [`HasErrors`](../../../TheOne.Avalonia/Controls/Form.cs#L51) | `bool` | read-only; OneWay | `computed / state` | Read-only deduplicated errors from the latest validation and whether it failed |
| [`HasUnmappedErrors`](../../../TheOne.Avalonia/Controls/Form.cs#L53) | `bool` | read-only; OneWay | `computed / state` | Read-only errors without a matching field; these populate the summary without repeating inline messages |
| [`IsBusy`](../../../TheOne.Avalonia/Controls/Form.cs#L54) | `bool` | read-only; OneWay | `computed / state` | Read-only; validation is running, content is disabled, duplicate validation/submission returns `false` |
| [`IsValidated`](../../../TheOne.Avalonia/Controls/Form.cs#L55) | `bool` | read-only; OneWay | `computed / state` | Read-only; last validation succeeded, reset by relevant notifications, clearing, detachment, or replacing content/validator |
| [`LabelOrientation`](../../../TheOne.Avalonia/Controls/Form.cs#L48) | `Avalonia.Layout.Orientation` | writable; OneWay; inherits | `Orientation.Vertical` | Defaults to `Vertical` / `120`, inherited by items; width must be positive and finite Validation: `Enum.IsDefined` Values: `Horizontal`, `Vertical` |
| [`LabelWidth`](../../../TheOne.Avalonia/Controls/Form.cs#L49) | `double` | writable; OneWay; inherits | `120` | Defaults to `Vertical` / `120`, inherited by items; width must be positive and finite Validation: `value => double.IsFinite(value) && value > 0` |
| [`ShowErrorSummary`](../../../TheOne.Avalonia/Controls/Form.cs#L47) | `bool` | writable; OneWay | `true` | Defaults to `true`; displays model-level and unmapped errors only; mapped errors remain below their fields |
| [`SubmitCommand`](../../../TheOne.Avalonia/Controls/Form.cs#L45) | `System.Windows.Input.ICommand?` | writable; OneWay | `null` | Host command and parameter; `CanExecute` is checked before and after asynchronous validation |
| [`SubmitCommandParameter`](../../../TheOne.Avalonia/Controls/Form.cs#L46) | `object?` | writable; OneWay | `null` | Host command and parameter; `CanExecute` is checked before and after asynchronous validation |
| [`UnmappedErrors`](../../../TheOne.Avalonia/Controls/Form.cs#L52) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.StepValidationError>` | read-only; OneWay | `computed / state` | Read-only errors without a matching field; these populate the summary without repeating inline messages |
| [`ValidationErrors`](../../../TheOne.Avalonia/Controls/Form.cs#L50) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.StepValidationError>` | read-only; OneWay | `computed / state` | Read-only deduplicated errors from the latest validation and whether it failed |
| [`ValidationModel`](../../../TheOne.Avalonia/Controls/Form.cs#L44) | `object?` | writable; OneWay | `null` | Optional notification source for custom validators; attribute validators use their registered model instead |
| [`Validator`](../../../TheOne.Avalonia/Controls/Form.cs#L42) | `TheOne.Avalonia.Controls.IStepValidator?` | writable; OneWay | `null` | Optional `IStepValidator`; reuse step validators or scope attributes with `StepValidation.For(model, memberNames)` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`void Form.ClearValidation()`](../../../TheOne.Avalonia/Controls/Form.cs#L126) | Cancels pending validation and clears form messages, retaining model values and native input drafts. |
| [`Task<bool> Form.ValidateAsync(CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/Form.cs#L150) | Runs attribute/custom validation and native conversion validation. Invalid, busy, detached or cancelled calls return false. |
| [`Task<bool> Form.SubmitAsync(CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/Form.cs#L179) | Validates before executing the host command, checking CanExecute again after asynchronous validation. |
| [`bool Form.FocusFirstError()`](../../../TheOne.Avalonia/Controls/Form.cs#L191) | Scrolls to and focuses the first visible, enabled invalid input; returns `false` if none exists |
| [`Form.Form()`](../../../TheOne.Avalonia/Controls/Form.cs#L15) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event EventHandler<RoutedEventArgs>? Form.ValidationFailed`](../../../TheOne.Avalonia/Controls/Form.cs#L57) | Bubbling event for model, conversion, or validator failure; cancellation does not raise it |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<IStepValidator?> Form.ValidatorProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L17) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> Form.ValidationModelProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<ICommand?> Form.SubmitCommandProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> Form.SubmitCommandParameterProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L20) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> Form.ShowErrorSummaryProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L21) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<Orientation> Form.LabelOrientationProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> Form.LabelWidthProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L23) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Form, IReadOnlyList<StepValidationError>> Form.ValidationErrorsProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L24) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Form, bool> Form.HasErrorsProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L25) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Form, IReadOnlyList<StepValidationError>> Form.UnmappedErrorsProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L26) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Form, bool> Form.HasUnmappedErrorsProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L27) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Form, bool> Form.IsBusyProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L28) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Form, bool> Form.IsValidatedProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L29) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Form, bool> Form.CanSubmitProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L30) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly RoutedEvent<RoutedEventArgs> Form.ValidationFailedEvent`](../../../TheOne.Avalonia/Controls/Form.cs#L31) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

Controls accept `small` / `normal` / `large` or inherited `ControlSizing.Size`. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.IStepValidator`](TheOne.Avalonia.Controls.IStepValidator.md)
- [`TheOne.Avalonia.Controls.StepCompareAttribute`](TheOne.Avalonia.Controls.StepCompareAttribute.md)
- [`TheOne.Avalonia.Controls.StepValidation`](TheOne.Avalonia.Controls.StepValidation.md)
- [`TheOne.Avalonia.Controls.StepValidationError`](TheOne.Avalonia.Controls.StepValidationError.md)
