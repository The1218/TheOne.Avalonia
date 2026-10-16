# VerificationStep

[English](../en/TheOne.Avalonia.Controls.VerificationStep.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.VerificationStep.md) · [API index](README.md)

`TheOne.Avalonia.Controls.VerificationStep` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

A sequential form with virtualized headers and validation before every forward transition.

[Source / 声明](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L18)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanGoBack`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L73) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`CanGoNext`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L74) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`CompleteText`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L66) | `string` | writable; OneWay | `"完成"` | Defaults “下一步” / “上一步” / “完成” (next / previous / complete) |
| [`CompletedCommand`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L78) | `System.Windows.Input.ICommand?` | writable; OneWay | `null` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`CompletedCommandParameter`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L79) | `object?` | writable; OneWay | `null` | Business argument passed to CompletedCommand when validation succeeds and the step completes. |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L62) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Text, controls, or typed data templates |
| [`CurrentContent`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L69) | `object?` | read-only; OneWay | `computed / state` | Read-only position, model / content, resolved template; cannot bypass validation by setting index |
| [`CurrentContentTemplate`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L70) | `Avalonia.Controls.Templates.IDataTemplate?` | read-only; OneWay | `computed / state` | Read-only position, model / content, resolved template; cannot bypass validation by setting index |
| [`ForwardText`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L75) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HasErrors`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L77) | `bool` | read-only; OneWay | `computed / state` | All validation errors |
| [`HasUnmappedErrors`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L81) | `bool` | read-only; OneWay | `computed / state` | Whole-step errors or those without mapped inputs |
| [`HeaderHorizontalAlignment`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L64) | `Avalonia.Layout.HorizontalAlignment` | writable; OneWay | `HorizontalAlignment.Center` | Default Center aligned with content; Stretch divides width; Left / Right supported Validation: `Enum.IsDefined` Values: `Stretch`, `Left`, `Center`, `Right` |
| [`HeaderMaxWidth`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L63) | `double` | writable; OneWay | `960` | Header viewport maximum, default 960, finite and positive Validation: `value => double.IsFinite(value) && value > 0` |
| [`IsBusy`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L71) | `bool` | read-only; OneWay | `computed / state` | Validation / final completion state |
| [`IsCompleted`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L72) | `bool` | read-only; OneWay | `computed / state` | Validation / final completion state |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L61) | `System.Collections.Generic.IEnumerable<TheOne.Avalonia.Controls.StepItem>?` | writable; OneWay | `null` | IEnumerable&lt;StepItem&gt;, normally StepItem&lt;concrete model type&gt;; use ObservableCollection for changes and distinct item identities |
| [`NextText`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L65) | `string` | writable; OneWay | `"下一步"` | Defaults “下一步” / “上一步” / “完成” (next / previous / complete) |
| [`PreviousText`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L67) | `string` | writable; OneWay | `"上一步"` | Defaults “下一步” / “上一步” / “完成” (next / previous / complete) |
| [`SelectedIndex`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L68) | `int` | read-only; OneWay | `computed / state` | Read-only position, model / content, resolved template; cannot bypass validation by setting index |
| [`UnmappedErrors`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L80) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.StepValidationError>` | read-only; OneWay | `computed / state` | Whole-step errors or those without mapped inputs |
| [`ValidationErrors`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L76) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.StepValidationError>` | read-only; OneWay | `computed / state` | All validation errors |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`void VerificationStep.Reset()`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L198) | Resets progress / selection, preserving business data and uncorrected drafts |
| [`bool VerificationStep.GoBack()`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L202) | Returns to previous steps; forward moves only to the adjacent validated step, with no skipping |
| [`Task<bool> VerificationStep.GoToAsync(int index, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L209) | Backward navigation is free; forward navigation is limited to the immediate next step and validates the current one. |
| [`Task<bool> VerificationStep.GoNextAsync(CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L219) | Validates current bound model attributes and native input errors; returns false on invalid, busy or cancelled transitions. |
| [`VerificationStep.VerificationStep()`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L18) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event EventHandler<RoutedEventArgs>? VerificationStep.ValidationFailed`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L82) | Bubbling failure event; completion uses CompletedCommand |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<IEnumerable<StepItem>?> VerificationStep.ItemsSourceProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L20) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> VerificationStep.ContentTemplateProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L21) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> VerificationStep.HeaderMaxWidthProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<HorizontalAlignment> VerificationStep.HeaderHorizontalAlignmentProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L23) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> VerificationStep.NextTextProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L24) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> VerificationStep.CompleteTextProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L25) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> VerificationStep.PreviousTextProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L26) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<VerificationStep, int> VerificationStep.SelectedIndexProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L27) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<VerificationStep, IDataTemplate?> VerificationStep.CurrentContentTemplateProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L28) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<VerificationStep, object?> VerificationStep.CurrentContentProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L29) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<VerificationStep, bool> VerificationStep.IsBusyProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L30) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<VerificationStep, bool> VerificationStep.IsCompletedProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L31) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<VerificationStep, bool> VerificationStep.CanGoBackProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L32) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<VerificationStep, bool> VerificationStep.CanGoNextProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L33) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<VerificationStep, string> VerificationStep.ForwardTextProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L34) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<VerificationStep, IReadOnlyList<StepValidationError>> VerificationStep.ValidationErrorsProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L35) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<VerificationStep, bool> VerificationStep.HasErrorsProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L36) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<ICommand?> VerificationStep.CompletedCommandProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L37) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> VerificationStep.CompletedCommandParameterProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L38) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<VerificationStep, IReadOnlyList<StepValidationError>> VerificationStep.UnmappedErrorsProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L39) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<VerificationStep, bool> VerificationStep.HasUnmappedErrorsProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L40) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly RoutedEvent<RoutedEventArgs> VerificationStep.ValidationFailedEvent`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L41) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.StepCompareAttribute`](TheOne.Avalonia.Controls.StepCompareAttribute.md)
- [`TheOne.Avalonia.Controls.StepItem`](TheOne.Avalonia.Controls.StepItem.md)
- [`TheOne.Avalonia.Controls.StepItem<T>`](TheOne.Avalonia.Controls.StepItem_T_.md)
- [`TheOne.Avalonia.Controls.StepState`](TheOne.Avalonia.Controls.StepState.md)
- [`TheOne.Avalonia.Controls.StepValidation`](TheOne.Avalonia.Controls.StepValidation.md)
- [`TheOne.Avalonia.Controls.StepValidationError`](TheOne.Avalonia.Controls.StepValidationError.md)
