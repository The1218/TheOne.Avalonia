# Loading states and LoadingContainer

Historical command-line checks and consumer test projects have been removed. Check coverage and measurements in this guide are historical records. Start PreviewHost normally and open the relevant control page; validate target-platform behavior in the consuming application.

[简体中文](../zh-cn/loading.md) · **English**

Ordinary Button keeps its native type and uses the `ButtonLoading.IsLoading` attached property; IconButton exposes `IsLoading` directly. While loading, both show an indicator and block native input, Command, Click, and Flyout activation. Ordinary Button preserves its IsEnabled value / binding, including TwoWay. Native command executability temporarily gates input; the original command and disabled behavior return after loading. Command uses its native OneWay default binding. The gate does not wrap or proxy business commands. Direct ICommand execution is outside control input; business commands should still manage their own concurrency.

```xml
<Button Content="保存" Command="{Binding SaveCommand}"
        one:ButtonLoading.IsLoading="{Binding IsLoading}" />
<one:IconButton Icon="{Binding SaveIcon}" Content="保存"
                IconPlacement="Right" IsLoading="{Binding IsLoading}"
                Command="{Binding SaveCommand}" />
```

Loading does not infer task state automatically. Set IsLoading in the ViewModel before execution and clear it in finally. PreviewHost demonstrates asynchronous ReactiveCommand and ReactiveObject notifications. Declare the view's x:DataType to use compiled bindings; no reflection or dynamic control creation is required.

LoadingContainer inherits ContentControl and supports Content / ContentTemplate, Padding, CornerRadius, and content alignment. `IsLoading` defaults to false. `LoadingText` defaults to the Chinese loading message “正在加载…” and can be bound or set to null. `OverlayOpacity` defaults to 0.65 and accepts 0–1 (transparent to opaque). It affects only the overlay background, not the indicator or text. `OverlayBrush` defaults to One.Surface and can be overridden. Even at opacity 0, the overlay blocks pointer and keyboard input in the content area while retaining the content instance, data context, and input values. It does not change measurement or virtualization constraints; large lists still need a bounded viewport and virtualization panel.

```xml
<one:LoadingContainer IsLoading="{Binding IsLoading}" LoadingText="{Binding ProgressText}"
                      OverlayOpacity="0.65" LoadingIcon="{Binding ProgressIcon}" Height="320">
  <Grid><!-- 正常内容，包括编译绑定与命令 --></Grid>
</one:LoadingContainer>
```

`LoadingIcon` accepts Geometry, PathIcon / another Control, or data rendered by `LoadingIconTemplate`. The default Geometry template uses PathIcon; consumers can supply a Material icon template with x:DataType. When LoadingIcon and LoadingIconTemplate are both null, the built-in rotating indicator returns. Custom icons do not rotate automatically, but their templates can add motion. `LoadingIconSize` follows the three densities by default. Empty or whitespace `LoadingText` hides text and spacing. Control instances cannot share visual parents; use Geometry or templates for shared icons.

`ControlSizing.Size` supports Small / Normal / Large. Button indicator sizes are 14 / 16 / 20 px; container indicators are 22 / 28 / 34 px. One.* semantic colors follow Light / Dark themes. `Motion.Enabled=False` stops rotation but retains the static icon and text; setting it to true during loading resumes rotation. Hiding or unloading cancels animation. LoadingIndicator can also be used independently: IsActive controls operation and Foreground controls color.

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### ButtonLoading

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.ButtonLoading.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/ButtonLoading.cs#L12) | `bool` | writable; OneWay; attached | `false` | Shows busy feedback; buttons retain size and prevent repeated actions, while containers show a loading layer. The host resets it when work finishes. |

### LoadingContainer

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.LoadingContainer.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`HasCustomLoadingIcon`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L95) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HasLoadingText`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L107) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L53) | `bool` | writable; OneWay | `false` | Shows busy feedback; buttons retain size and prevent repeated actions, while containers show a loading layer. The host resets it when work finishes. |
| [`LoadingIcon`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L77) | `object?` | writable; OneWay | `null` | Loading-state icon; null retains the default indicator. |
| [`LoadingIconSize`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L89) | `double` | writable; OneWay | `28` | Loading icon size in DIPs, independent of overlay area. Validation: `value => double.IsFinite(value) && value > 0` |
| [`LoadingIconTemplate`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L83) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`LoadingText`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L59) | `string?` | writable; OneWay | `"正在加载…"` | Localizable loading message, distinct from the task's completion result. |
| [`OverlayBrush`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L71) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Loading overlay brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`OverlayOpacity`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L65) | `double` | writable; OneWay | `.65` | Overlay opacity; see validation for range/finite-number constraints. Validation: `value => double.IsFinite(value) && value is >= 0 and <= 1` |
| [`ShowDefaultLoadingIndicator`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L101) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

### LoadingIndicator

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.LoadingIndicator.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Foreground`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L43) | `Avalonia.Media.IBrush?` | writable; OneWay | `inherited registration` | Foreground brush; retain text, status and disabled contrast when overriding. |
| [`IsActive`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L49) | `bool` | writable; OneWay | `true` | Whether the indicator animates; disabling preserves layout and the global motion switch still applies. |
| [`IsSpinning`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L55) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

No added public appearance classes; use the default theme and control-specific properties.
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

No additional business methods; configure the control-specific properties above.

### Related models

| Model | Properties/enum values |
| --- | ---: |
| [`TheOne.Avalonia.Controls.ButtonLoading`](../../api/en/TheOne.Avalonia.Controls.ButtonLoading.md) | 1 |

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
