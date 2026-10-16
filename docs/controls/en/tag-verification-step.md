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
