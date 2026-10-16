# Tag and VerificationStep

[简体中文](../zh-cn/tag-verification-step.md) · **English**

[Back to controls](README.md). Load OneTheme and use `xmlns:one="using:TheOne.Avalonia.Controls"`.

## Tag status labels

```xml
<one:Tag Content="已完成" Status="Success" Variant="Soft">
  <one:Tag.Icon>
    <PathIcon Width="14" Height="14" Data="M1,7 L3,5 L5,8 L11,1 L13,3 L5,12 Z" />
  </one:Tag.Icon>
</one:Tag>
<one:Tag Content="处理中" Status="Processing" Variant="Outline" />
<one:Tag Content="错误" Status="Error" Variant="Solid" IsEnabled="False" />
```

| Property | Description |
| --- | --- |
| `Content` / `ContentTemplate` | Text, controls, or typed data templates |
| `Icon` / `IconTemplate` | Optional PathIcon, Image, or model template; retains logical-tree / DataContext behavior |
| `Status` | `Default`, `Information`, `Success`, `Warning`, `Error`, `Processing` |
| `Variant` | `Soft` background, `Outline` border, `Solid` fill |
| `Spacing` | Icon / content gap, default 6; absent icon / text leaves no gap |
| `IconSize` | Scaled side length, default 13 px, one step below the former 14 px baseline; shared across densities and overridable without changing caller icon dimensions |
| `IsEnabled` | DisabledSurface / DisabledText for disabled state |

Tag displays status rather than button / toggle selection behavior. Applications supply labels and icons; color must not be the only cue. TheOne semantic colors support Light / Dark and inherited small / normal / large. Minimum heights are 24 / 28 / 32, fonts 11 / 12 / 15 px. Override standard Padding, CornerRadius, Foreground, and related properties.

## VerificationStep validation

For ordinary editing pages, use [Form / FormItem](form.md) with the same validators and error types. `FormItem` also works in custom step content, assigning errors to the nearest validation host.

`VerificationStep` is a step control with validation: forward navigation requires the current step to pass validation. It replaces the former `Step` control; update C# references and XAML tags to `VerificationStep`. The supporting `StepItem`, `StepField`, and `StepValidation` APIs retain their names and behavior.

Default usage needs models, standard validation attributes, and ItemsSource only. `StepItem<T>` creates property validators and, without custom templates, generates forms, labels, and errors below inputs. No StepField declarations or per-step StepValidation.For calls are needed. T is the concrete model type; the library preserves its public-property metadata for NativeAOT.

```csharp
using System.ComponentModel.DataAnnotations;
using TheOne.Avalonia.Controls;

public sealed class AccountModel
{
    [Display(Name = "姓名", Prompt = "请输入姓名", Order = 0)]
    [Required(ErrorMessage = "请输入姓名。")]
    [StringLength(24, MinimumLength = 2)]
    public string Name { get; set; } = "";

    [Display(Name = "年龄", Order = 1)]
    [Range(18, 120, ErrorMessage = "年龄需在 18–120 之间。")]
    public int Age { get; set; } = 18;
}

// 宿主 VM 暴露此集合以及已有的 ICommand CompletedCommand。
public IReadOnlyList<StepItem> Steps { get; } = new StepItem[]
{
    new StepItem<AccountModel>(new()) { Title = "基本资料", Description = "填写必填信息" },
    new StepItem<AccountModel>(new()) { Title = "联系信息", Description = "填写另一组资料" }
};
```

```xml
<!-- 根视图声明 x:DataType，并使用编译绑定。 -->
<one:VerificationStep ItemsSource="{Binding Steps}"
          CompletedCommand="{Binding CompletedCommand}"
          Height="520" />
```

Compiled binding paths write input directly to models. Ordinary properties validate on forward navigation. Implement INotifyPropertyChanged or use an MVVM notification base so external changes update inputs, cancel pending validation, and invalidate later progress. Different steps can have different concrete model types while sharing a StepItem collection.

### Built-in forms and attributes

| Property / attribute | Default behavior |
| --- | --- |
| Public readable / writable non-indexed instance properties | Fields in declaration order, without assembly scans or prebuilt views for other steps |
| string, numeric, char, Guid, DateTime, DateTimeOffset, TimeSpan | TextBox with target-type conversion; conversion failures block forward navigation |
| bool / bool? | CheckBox, with three-state nullable bool |
| Display(Name / Prompt / Description / Order) | Optional label, placeholder, description, order |
| Display(AutoGenerateField = false) | No input; validation still applies and errors appear at step level |
| DataType.Password / MultilineText | Password / multiline inputs |
| Editable(false) | Read-only field; validation remains |
| Other types | Read-only text; use custom templates for collections, complex objects, enum selection |

Standard ValidationAttribute rules include Required, Range, StringLength, MinLength, MaxLength, EmailAddress, RegularExpression, AllowedValues, and others. Failed Required suppresses other errors on that field; errors appear below each input. Custom ValidationAttribute / IValidatableObject also work. Cross-field results should supply MemberNames; results without names apply to the whole step.

Ordinary `[Required]` accepts false; use `[AllowedValues(true)]` for mandatory confirmation boxes. Range int / double constructors support this project's NativeAOT. `Range(Type, string, string)` uses generic TypeConverter and carries trim warnings that must not be suppressed.

Failed conversion can leave the model's previous valid value. Invalid drafts / conversion errors survive field recycling, still block completion, and reappear on redisplay. Corresponding external property notifications replace stale drafts. Built-in forms use bounded ListBox / VirtualizingStackPanel and realize visible fields only; error lists are also bounded and recycled.

Use StepCompareAttribute for consistency checks such as email / password confirmation:

```csharp
[Required, EmailAddress]
public string Email { get; set; } = "";

[Required]
[StepCompare(nameof(Email), ErrorMessage = "两次邮箱输入不一致。")]
public string ConfirmEmail { get; set; } = "";
```

StepCompare reads latest values through concrete-model accessors. Standard CompareAttribute itself has trim warnings; use StepCompare for NativeAOT. Custom attributes reflecting over other types must satisfy their own trimming contracts.

### Custom content and advanced validation

`VerificationStep.ContentTemplate` wins, then matching DataTemplate; otherwise the built-in form is used. `StepItem<T>` still validates attributes under custom layouts. Use StepField with a model-property MemberName to place errors below particular inputs. Whole-step, hidden-field, and unmapped errors stay in the bounded top list rather than disappearing.

```xml
<one:VerificationStep ItemsSource="{Binding Steps}" CompletedCommand="{Binding CompletedCommand}">
  <one:VerificationStep.DataTemplates>
    <DataTemplate DataType="{x:Type vm:AccountModel}" x:DataType="vm:AccountModel">
      <one:StepField MemberName="Name">
        <TextBox Text="{Binding Name, Mode=TwoWay}" />
      </one:StepField>
    </DataTemplate>
  </one:VerificationStep.DataTemplates>
</one:VerificationStep>
```

Non-generic StepItem remains available for developer controls / models. It does not infer runtime property types from object. Explicitly assign `Validator = StepValidation.For(model)` for property validation. `StepValidation.For(model, nameof(AccountModel.Name))` limits fields to the current step. Call with concrete types instead of hiding them as object / base classes. A non-null generic StepItem Validator overrides automatic validation for scoped or remote checks.

IStepValidator supports async validation returning IReadOnlyList&lt;StepValidationError&gt; and honoring CancellationToken. Validate the actual bound model. Update StepItem.Content when replacing it; generic steps rebuild default validators / forms and reject null or mismatched model types.

### Completion, navigation, and lifecycle

Bind ViewModel ICommand through CompletedCommand and pass business data with CompletedCommandParameter. The last step must validate too; the command runs once. CanExecute=false disables completion and blocks direct completion, checked again after async validation. Completion state is set before execution. Applications handle async submission / exceptions inside the command; IsBusy covers step validation, not the command's async work.

| API | Behavior |
| --- | --- |
| ItemsSource | IEnumerable&lt;StepItem&gt;, normally StepItem&lt;concrete model type&gt;; use ObservableCollection for changes and distinct item identities |
| SelectedIndex / CurrentContent / CurrentContentTemplate | Read-only position, model / content, resolved template; cannot bypass validation by setting index |
| ContentTemplate | Optional custom content template |
| GoNextAsync(token) | Validates, then advances / completes; returns bool |
| GoBack() / GoToAsync(index, token) | Returns to previous steps; forward moves only to the adjacent validated step, with no skipping |
| Reset() | Resets progress / selection, preserving business data and uncorrected drafts |
| IsBusy / IsCompleted | Validation / final completion state |
| ValidationErrors / HasErrors | All validation errors |
| UnmappedErrors / HasUnmappedErrors | Whole-step errors or those without mapped inputs |
| ValidationFailed | Bubbling failure event; completion uses CompletedCommand |
| NextText / PreviousText / CompleteText | Defaults “下一步” / “上一步” / “完成” (next / previous / complete) |
| HeaderHorizontalAlignment | Default Center aligned with content; Stretch divides width; Left / Right supported |
| HeaderMaxWidth | Header viewport maximum, default 960, finite and positive |

Navigation and UI model changes run on the UI thread. Repeated forward input does not start another validation. Collection, Content, or Validator replacement resets progress. Back, reset, model notifications, or visual-tree exit cancels ongoing validation. Even validators ignoring cancellation cannot advance / complete from late results. Service failures show step-level errors and allow retry. Visual-tree exit releases model / command subscriptions.

### Layout and preview

Connected dots distinguish completed checks, filled current numbers, pale future numbers, and error exclamation marks using semantic colors. small / normal / large diameters are 30 / 36 / 42. Header viewport height is 112, maximum width 960, centered by default; item width defaults to 200. `HeaderHorizontalAlignment="Stretch"` divides the viewport with 120 minimum item width. Narrow windows / long lists scroll horizontally rather than shrinking dots unreadably. Resizing / alignment changes recompute widths using native ListBox / VirtualizingStackPanel. StepItem stores success independently of realized headers.

The built-in form supports native `Tab` / `Shift+Tab` traversal between inputs, respects disabled fields and `KeyboardNavigation.IsTabStop`, and allows focus to leave the last field for the navigation buttons. Its `One.StepForm` theme uses `KeyboardNavigation.TabNavigation="Continue"`; ordinary ListBox controls retain their own navigation mode. Custom content templates retain native focus behavior.

VerificationStep defaults to maximum height 720; built-in forms max at width 640 within finite content height. Headers and forms share horizontal centers. Set `HeaderHorizontalAlignment="Stretch" HeaderMaxWidth="640"` for equal width. Preview switches among four alignments. Custom long forms can use ScrollViewer; collections keep their own bounded viewports. Field errors max at height 96 and top step errors at 120.

Templates preserve PART_Steps, PART_ContentPresenter, PART_PreviousButton, PART_NextButton, PART_ValidationErrors, with PART_FormFields for built-in forms. Extensions / Step / VerificationStep demonstrates auto forms, attribute validation, completion commands, reset, and thousand-step recycling. Commands: [preview entry points](../../../design/PreviewHost/Program.cs).

Run `dotnet run --project design/PreviewHost -- --verify-verification-step` for VerificationStep-only checks (including real Tab / Shift+Tab input), or use `--render-controls .preview/verification-step VerificationStep` to render this control. The published NativeAotSmoke consumer runs the same checks with `--require-aot --verify-verification-step`.

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section lists only Classes, properties, extension APIs/events and related models added by this project, without repeating inherited Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost visibly compares project-added styles and uses Description components for control-specific properties, API signatures and model fields. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### IStepValidator

[API / model reference](../../api/en/TheOne.Avalonia.Controls.IStepValidator.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

### StepCompareAttribute

[API / model reference](../../api/en/TheOne.Avalonia.Controls.StepCompareAttribute.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`OtherProperty`](../../../TheOne.Avalonia/Controls/StepCompareAttribute.cs#L10) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

### StepField

[API / model reference](../../api/en/TheOne.Avalonia.Controls.StepField.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Errors`](../../../TheOne.Avalonia/Controls/StepField.cs#L16) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.StepValidationError>` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HasErrors`](../../../TheOne.Avalonia/Controls/StepField.cs#L17) | `bool` | read-only; OneWay | `computed / state` | All validation errors |
| [`MemberName`](../../../TheOne.Avalonia/Controls/StepField.cs#L15) | `string` | writable; OneWay | `""` | Exact model member name used to map validation errors to a field, separate from its display label. |

### StepValidation

[API / model reference](../../api/en/TheOne.Avalonia.Controls.StepValidation.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

### StepValidationError

[API / model reference](../../api/en/TheOne.Avalonia.Controls.StepValidationError.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`MemberName`](../../../TheOne.Avalonia/Controls/StepValidation.cs#L7) | `string` | writable; CLR / init-only | `required constructor argument` | Inherited from `StepField`; matches error member names such as `Name`, never executed as a reflection binding path |
| [`Message`](../../../TheOne.Avalonia/Controls/StepValidation.cs#L7) | `string` | writable; CLR / init-only | `required constructor argument` | Result message for users/diagnostics; status fields still determine the operation outcome. |

### Tag

[API / model reference](../../api/en/TheOne.Avalonia.Controls.Tag.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Icon`](../../../TheOne.Avalonia/Controls/Tag.cs#L23) | `object?` | writable; OneWay | `null` | Optional PathIcon, Image, or model template; retains logical-tree / DataContext behavior |
| [`IconSize`](../../../TheOne.Avalonia/Controls/Tag.cs#L21) | `double` | writable; OneWay | `13` | Scaled side length, default 13 px, one step below the former 14 px baseline; shared across densities and overridable without changing caller icon dimensions Validation: `value => double.IsFinite(value) && value > 0` |
| [`IconTemplate`](../../../TheOne.Avalonia/Controls/Tag.cs#L24) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Optional PathIcon, Image, or model template; retains logical-tree / DataContext behavior |
| [`Spacing`](../../../TheOne.Avalonia/Controls/Tag.cs#L27) | `double` | writable; OneWay | `6` | Icon / content gap, default 6; absent icon / text leaves no gap Validation: `value => double.IsFinite(value) && value >= 0` |
| [`Status`](../../../TheOne.Avalonia/Controls/Tag.cs#L25) | `TheOne.Avalonia.Controls.TagStatus` | writable; OneWay | `0 / default` | `Default`, `Information`, `Success`, `Warning`, `Error`, `Processing` Validation: `Enum.IsDefined` Values: `Default`, `Information`, `Success`, `Warning`, `Error`, `Processing` |
| [`Variant`](../../../TheOne.Avalonia/Controls/Tag.cs#L26) | `TheOne.Avalonia.Controls.TagVariant` | writable; OneWay | `0 / default` | `Soft` background, `Outline` border, `Solid` fill Validation: `Enum.IsDefined` Values: `Soft`, `Outline`, `Solid` |

### VerificationStep

[API / model reference](../../api/en/TheOne.Avalonia.Controls.VerificationStep.md)

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

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Style classes and automatic state

| Control | Class | Scope | Effective setters/resources |
| --- | --- | --- | --- |
| Tag | `one-large` | automatic template class; do not bind manually | one&#124;Tag.one-small, one&#124;Tag.one-normal, one&#124;Tag.one-large => MinHeight=0; Padding=4,0; ^.one-large => MinHeight=32; FontSize=15; Padding=11,4 |
| Tag | `one-normal` | automatic template class; do not bind manually | one&#124;Tag.one-small, one&#124;Tag.one-normal, one&#124;Tag.one-large => MinHeight=0; Padding=4,0 |
| Tag | `one-small` | automatic template class; do not bind manually | one&#124;Tag.one-small, one&#124;Tag.one-normal, one&#124;Tag.one-large => MinHeight=0; Padding=4,0; ^.one-small => MinHeight=24; FontSize=11; Padding=7,2 |
| Tag | `step-indicator` | automatic template class; do not bind manually | one&#124;Tag.step-indicator => Width=36; Height=36; one&#124;Tag.step-indicator.one-small => Width=30; Height=30; one&#124;Tag.step-indicator.one-large => Width=42; Height=42 |
| VerificationStep | `one-large` | automatic template class; do not bind manually | one&#124;Tag.step-indicator.one-large => Width=42; Height=42; ^.one-large => FontSize=16; Padding=24 |
| VerificationStep | `one-small` | automatic template class; do not bind manually | one&#124;Tag.step-indicator.one-small => Width=30; Height=30; ^.one-small => FontSize=12; Padding=16 |

`:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without added appearance classes use their default theme and control-specific properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.IStepValidator`](../../api/en/TheOne.Avalonia.Controls.IStepValidator.md) | Interface | 0 | 1 | 0 |
| [`TheOne.Avalonia.Controls.StepCompareAttribute`](../../api/en/TheOne.Avalonia.Controls.StepCompareAttribute.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.StepField`](../../api/en/TheOne.Avalonia.Controls.StepField.md) | Class | 3 | 1 | 0 |
| [`TheOne.Avalonia.Controls.StepItem`](../../api/en/TheOne.Avalonia.Controls.StepItem.md) | Class | 13 | 1 | 1 |
| [`TheOne.Avalonia.Controls.StepItem<T>`](../../api/en/TheOne.Avalonia.Controls.StepItem_T_.md) | Class | 0 | 1 | 0 |
| [`TheOne.Avalonia.Controls.StepState`](../../api/en/TheOne.Avalonia.Controls.StepState.md) | Enum | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.StepValidation`](../../api/en/TheOne.Avalonia.Controls.StepValidation.md) | Class | 0 | 1 | 0 |
| [`TheOne.Avalonia.Controls.StepValidationError`](../../api/en/TheOne.Avalonia.Controls.StepValidationError.md) | Class | 2 | 1 | 0 |
| [`TheOne.Avalonia.Controls.Tag`](../../api/en/TheOne.Avalonia.Controls.Tag.md) | Class | 6 | 1 | 0 |
| [`TheOne.Avalonia.Controls.TagStatus`](../../api/en/TheOne.Avalonia.Controls.TagStatus.md) | Enum | 6 | 1 | 0 |
| [`TheOne.Avalonia.Controls.TagVariant`](../../api/en/TheOne.Avalonia.Controls.TagVariant.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Controls.VerificationStep`](../../api/en/TheOne.Avalonia.Controls.VerificationStep.md) | Class | 21 | 5 | 1 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
