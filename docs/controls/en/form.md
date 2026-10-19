# Form and FormItem

Historical command-line checks and consumer test projects have been removed. Check coverage and measurements in this guide are historical records. Start PreviewHost normally and open the relevant control page; validate target-platform behavior in the consuming application.

`Form` provides validation and a submission entry point for editing pages, reusing `VerificationStep`'s `IStepValidator`, `StepValidation.For<T>`, `StepValidationError`, and `StepCompareAttribute`. `FormItem` adds labels, required markers, help text, and field errors around caller-owned native input. Load `OneTheme`; no additional package or theme is required.

## Compiled bindings and setup

Create the validator with the concrete model type in the view model:

```csharp
public AccountModel Account { get; } = new();
public IStepValidator Validator { get; }
public ICommand SaveCommand { get; }

public EditViewModel()
{
    Validator = StepValidation.For(Account);
    SaveCommand = ReactiveCommand.Create(Save);
}
```

Properties can use `Required`, `Range`, `StringLength`, `EmailAddress`, and other validation attributes. Use `StepCompare` for cross-field comparison, or implement `IValidatableObject` for field or model-level errors. Implement `INotifyPropertyChanged` and notify on the UI thread. `ReactiveCommand` is only an example; consumers may use other MVVM frameworks.

```xml
<one:Form x:Name="Editor" Validator="{Binding Validator}"
          SubmitCommand="{Binding SaveCommand}"
          LabelOrientation="Horizontal" LabelWidth="112">
  <StackPanel Spacing="16">
    <one:FormItem MemberName="Name" Label="Name" IsRequired="True"
                  HelpText="Enter 2–24 characters.">
      <TextBox Text="{Binding Account.Name, Mode=TwoWay}" />
    </one:FormItem>
    <one:FormItem MemberName="Email" Label="Email" IsRequired="True">
      <TextBox Text="{Binding Account.Email, Mode=TwoWay}" />
    </one:FormItem>
    <Button Content="Validate and save" Click="SaveClicked"
            IsEnabled="{Binding #Editor.CanSubmit}" />
  </StackPanel>
</one:Form>
```

Declare `xmlns:one="using:TheOne.Avalonia.Controls"` and the view's concrete `x:DataType`, keeping compiled bindings. The button calls only the form entry point:

```csharp
private async void SaveClicked(object? sender, RoutedEventArgs args)
    => await Editor.SubmitAsync();
```

Do not also bind the button's `Command` directly to saving, which bypasses validation. For a host-owned submission workflow, await `Editor.ValidateAsync()` and save only when it returns `true`.

## Form contract

| Property or method | Behavior |
| --- | --- |
| `Content` / `ContentTemplate` | Caller-owned layout or compiled data template with native or custom input |
| `Validator` | Optional `IStepValidator`; reuse step validators or scope attributes with `StepValidation.For(model, memberNames)` |
| `ValidationModel` | Optional notification source for custom validators; attribute validators use their registered model instead |
| `LabelOrientation` / `LabelWidth` | Defaults to `Vertical` / `120`, inherited by items; width must be positive and finite |
| `ShowErrorSummary` | Defaults to `true`; displays model-level and unmapped errors only; mapped errors remain below their fields |
| `SubmitCommand` / `SubmitCommandParameter` | Host command and parameter; `CanExecute` is checked before and after asynchronous validation |
| `ValidationErrors` / `HasErrors` | Read-only deduplicated errors from the latest validation and whether it failed |
| `UnmappedErrors` / `HasUnmappedErrors` | Read-only errors without a matching field; these populate the summary without repeating inline messages |
| `IsBusy` | Read-only; validation is running, content is disabled, duplicate validation/submission returns `false` |
| `IsValidated` | Read-only; last validation succeeded, reset by relevant notifications, clearing, detachment, or replacing content/validator |
| `CanSubmit` | Read-only; a submission attempt is currently allowed, not a guarantee of valid data |
| `ValidateAsync(CancellationToken)` | Validates model rules and native conversion errors; returns `false` on failure, cancellation, detachment, disabled state, or pending validation |
| `SubmitAsync(CancellationToken)` | Executes an available command after validation; does not intercept Enter automatically |
| `ClearValidation()` | Cancels validation and clears form messages, retaining model values and input drafts |
| `FocusFirstError()` | Scrolls to and focuses the first visible, enabled invalid input; returns `false` if none exists |
| `ValidationFailed` | Bubbling event for model, conversion, or validator failure; cancellation does not raise it |

With a null `SubmitCommand`, submission only validates. The host command owns asynchronous persistence, business errors, and duplicate-save prevention; `IsBusy` covers validation only. Command execution exceptions propagate to the caller.

## FormItem and layout

| Property | Behavior |
| --- | --- |
| `MemberName` | Inherited from `StepField`; matches error member names such as `Name`, never executed as a reflection binding path |
| `Label` / `HelpText` | Label and help below input; empty values hide their regions |
| `IsRequired` | Defaults to `false`; visual marker only, with actual rules owned by the validator |
| `LabelOrientation` | `Vertical` above input, `Horizontal` on the left; supports per-item overrides |
| `LabelWidth` | Horizontal label region width, including the required marker; vertical labels use content layout |
| `Errors` / `HasErrors` | Inherited read-only field errors; items also work in custom `VerificationStep` content |
| `HasLabel` / `HasHelpText` / `LabelDock` / `EffectiveLabelWidth` | Read-only template state for region visibility, label docking, and effective label width |

Labels target the first focusable input and retain an explicitly supplied `AutomationProperties.LabeledBy`. For multiple inputs in one item, provide accessible names or labels for each. Native keyboard, focus, `IsReadOnly`, and `IsEnabled` behavior is preserved.

The host owns spacing, scrolling, and responsive changes. Use a `StackPanel` for ordinary forms, or bounded `Grid` / `ScrollViewer` layouts for long forms. Switch to vertical labels for narrow windows as needed. Colors use `One.*` resources; inputs inherit Small / Normal / Large density.

## Validation scope and lifetime

- Attribute validation preserves required public property metadata through the concrete generic type. Do not hide it behind `object` or a base class, or disable compiled bindings.
- Custom validators implement `IStepValidator.ValidateAsync`, return `IReadOnlyList<StepValidationError>`, and honor cancellation. An empty `MemberName` represents a model-level error.
- Relevant model notifications clear old messages and cancel pending validation. `StepCompare` dependencies also invalidate results. Set an `INotifyPropertyChanged` `ValidationModel` for custom validators to obtain the same notification behavior.
- Replacing the validator, notification model, content, or template, or detaching the control cancels old validation. Stale results cannot update state even if the service ignores cancellation. Reattaching restores subscriptions.
- For model changes without notifications, call `ClearValidation()` and validate again. The form does not infer the runtime type of an arbitrary `DataContext`.
- Conversion errors retain the last valid model value and native input draft. Clearing messages or hiding the summary cannot bypass them. If the host virtualizes or removes explicit fields, it must preserve drafts; the form does not retain unloaded editors.
- Nested forms and steps assign fields to the nearest validation host. Outer validation does not automatically validate inner hosts or overwrite their errors; compose validation explicitly for whole-page submission.

This version uses explicit fields and templates rather than a standalone automatic generator. Existing `StepItem<T>` automatic forms retain their behavior; see [VerificationStep](tag-verification-step.md).

## Preview and focused checks

PreviewHost: **Extensions → Form**.

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### Form

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.Form.md)

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

### FormItem

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.FormItem.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`EffectiveLabelWidth`](../../../TheOne.Avalonia/Controls/FormItem.cs#L37) | `double` | read-only; OneWay | `computed / state` | Read-only template state for region visibility, label docking, and effective label width |
| [`HasHelpText`](../../../TheOne.Avalonia/Controls/FormItem.cs#L39) | `bool` | read-only; OneWay | `computed / state` | Read-only template state for region visibility, label docking, and effective label width |
| [`HasLabel`](../../../TheOne.Avalonia/Controls/FormItem.cs#L38) | `bool` | read-only; OneWay | `computed / state` | Read-only template state for region visibility, label docking, and effective label width |
| [`HelpText`](../../../TheOne.Avalonia/Controls/FormItem.cs#L31) | `string?` | writable; OneWay | `null` | Label and help below input; empty values hide their regions |
| [`IsRequired`](../../../TheOne.Avalonia/Controls/FormItem.cs#L33) | `bool` | writable; OneWay | `false` | Defaults to `false`; visual marker only, with actual rules owned by the validator |
| [`Label`](../../../TheOne.Avalonia/Controls/FormItem.cs#L30) | `string?` | writable; OneWay | `null` | Label and help below input; empty values hide their regions |
| [`LabelDock`](../../../TheOne.Avalonia/Controls/FormItem.cs#L36) | `Avalonia.Controls.Dock` | read-only; OneWay | `computed / state` | Read-only template state for region visibility, label docking, and effective label width Values: `Left`, `Bottom`, `Right`, `Top` |
| [`LabelOrientation`](../../../TheOne.Avalonia/Controls/FormItem.cs#L34) | `Avalonia.Layout.Orientation` | writable; OneWay; inherits | `Orientation.Vertical` | Defaults to `Vertical` / `120`, inherited by items; width must be positive and finite Validation: `Enum.IsDefined` Values: `Horizontal`, `Vertical` |
| [`LabelWidth`](../../../TheOne.Avalonia/Controls/FormItem.cs#L35) | `double` | writable; OneWay; inherits | `120` | Defaults to `Vertical` / `120`, inherited by items; width must be positive and finite Validation: `value => double.IsFinite(value) && value > 0` |

### IStepValidator

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.IStepValidator.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

### StepCompareAttribute

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.StepCompareAttribute.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`OtherProperty`](../../../TheOne.Avalonia/Controls/StepCompareAttribute.cs#L10) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

### StepValidation

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.StepValidation.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

### StepValidationError

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.StepValidationError.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`MemberName`](../../../TheOne.Avalonia/Controls/StepValidation.cs#L7) | `string` | writable; CLR / init-only | `required constructor argument` | Inherited from `StepField`; matches error member names such as `Name`, never executed as a reflection binding path |
| [`Message`](../../../TheOne.Avalonia/Controls/StepValidation.cs#L7) | `string` | writable; CLR / init-only | `required constructor argument` | Result message for users/diagnostics; status fields still determine the operation outcome. |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

No added public appearance classes; use the default theme and control-specific properties.
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

| Type | Action signature | Purpose |
| --- | --- | --- |
| [`Form`](../../api/en/TheOne.Avalonia.Controls.Form.md) | `void Form.ClearValidation()` | Cancels pending validation and clears form messages, retaining model values and native input drafts. |
| [`Form`](../../api/en/TheOne.Avalonia.Controls.Form.md) | `Task<bool> Form.ValidateAsync(CancellationToken cancellationToken = default(CancellationToken))` | Runs attribute/custom validation and native conversion validation. Invalid, busy, detached or cancelled calls return false. |
| [`Form`](../../api/en/TheOne.Avalonia.Controls.Form.md) | `Task<bool> Form.SubmitAsync(CancellationToken cancellationToken = default(CancellationToken))` | Validates before executing the host command, checking CanExecute again after asynchronous validation. |
| [`Form`](../../api/en/TheOne.Avalonia.Controls.Form.md) | `bool Form.FocusFirstError()` | Scrolls to and focuses the first visible, enabled invalid input; returns `false` if none exists |
| [`IStepValidator`](../../api/en/TheOne.Avalonia.Controls.IStepValidator.md) | `Task<IReadOnlyList<StepValidationError>> IStepValidator.ValidateAsync(CancellationToken cancellationToken)` | Validates model rules and native conversion errors; returns `false` on failure, cancellation, detachment, disabled state, or pending validation |
| [`StepValidation`](../../api/en/TheOne.Avalonia.Controls.StepValidation.md) | `static IStepValidator StepValidation.For<T>(T model, params string[] memberNames) where T : class` | Creates a typed step validator for the supplied model/member set. |

### Event subscriptions

Subscribe when needed and detach when the host is disposed. These are event entry points, not routed-event identifier fields.

| Type | Event | Purpose |
| --- | --- | --- |
| [`Form`](../../api/en/TheOne.Avalonia.Controls.Form.md) | `ValidationFailed` | Bubbling event for model, conversion, or validator failure; cancellation does not raise it |

### Related models

| Model | Properties/enum values |
| --- | ---: |
| [`TheOne.Avalonia.Controls.StepCompareAttribute`](../../api/en/TheOne.Avalonia.Controls.StepCompareAttribute.md) | 1 |
| [`TheOne.Avalonia.Controls.StepValidationError`](../../api/en/TheOne.Avalonia.Controls.StepValidationError.md) | 2 |

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
