# Loading states and LoadingContainer

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

PreviewHost's Button and IconButton pages demonstrate loading and motion; LoadingContainer has a separate extension page. See [PreviewHost](../../../design/PreviewHost/Program.cs) for verification and targeted rendering entry points, and [NativeAotSmoke](../../../design/PreviewHost/Consumers/NativeAotSmoke/Program.cs) for native consumption.

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section enumerates declarations added by this library; native inherited members remain Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. Type links provide full model, method, event and enum signatures. PreviewHost's Control Laboratory (top-right action on each page) groups live examples by styles, properties, APIs/events and models; complex models use the complete business samples.

### ButtonLoading

[API / model reference](../../api/en/TheOne.Avalonia.Controls.ButtonLoading.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/ButtonLoading.cs#L13) | `bool` | writable; OneWay; attached | `false` | Shows busy feedback; buttons retain size and prevent repeated actions, while containers show a loading layer. The host resets it when work finishes. |

### LoadingContainer

[API / model reference](../../api/en/TheOne.Avalonia.Controls.LoadingContainer.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`HasCustomLoadingIcon`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L31) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HasLoadingText`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L33) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L24) | `bool` | writable; OneWay | `false` | Shows busy feedback; buttons retain size and prevent repeated actions, while containers show a loading layer. The host resets it when work finishes. |
| [`LoadingIcon`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L28) | `object?` | writable; OneWay | `null` | Loading-state icon; null retains the default indicator. |
| [`LoadingIconSize`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L30) | `double` | writable; OneWay | `28` | Loading icon size in DIPs, independent of overlay area. Validation: `value => double.IsFinite(value) && value > 0` |
| [`LoadingIconTemplate`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L29) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`LoadingText`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L25) | `string?` | writable; OneWay | `"正在加载…"` | Localizable loading message, distinct from the task's completion result. |
| [`OverlayBrush`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L27) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Loading overlay brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`OverlayOpacity`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L26) | `double` | writable; OneWay | `.65` | Overlay opacity; see validation for range/finite-number constraints. Validation: `value => double.IsFinite(value) && value is >= 0 and <= 1` |
| [`ShowDefaultLoadingIndicator`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L32) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

### LoadingIndicator

[API / model reference](../../api/en/TheOne.Avalonia.Controls.LoadingIndicator.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Foreground`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L19) | `Avalonia.Media.IBrush?` | writable; OneWay | `inherited registration` | Foreground brush; retain text, status and disabled contrast when overriding. |
| [`IsActive`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L20) | `bool` | writable; OneWay | `true` | Whether the indicator animates; disabling preserves layout and the global motion switch still applies. |
| [`IsSpinning`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L21) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

### Shared attached properties

[ControlSizing](../../api/en/TheOne.Avalonia.Styling.ControlSizing.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Size`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L11) | `TheOne.Avalonia.Styling.ControlSize` | writable; OneWay; attached; inherits | `ControlSize.Normal` | Shared Small/Normal/Large density inherited by descendants; explicit local values/classes can override it. Values: `Small`, `Normal`, `Large` |

[Motion](../../api/en/TheOne.Avalonia.Styling.Motion.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | writable; OneWay; attached; inherits | `true` | Inherited motion switch; disabling goes to final layout/state while preserving native input. |

### Style classes and automatic state

| Control | Class | Scope | Effective setters/resources |
| --- | --- | --- | --- |
| LoadingContainer | `one-large` | automatic template class; do not bind manually | ^.one-large => LoadingIconSize=34 |
| LoadingContainer | `one-small` | automatic template class; do not bind manually | ^.one-small => LoadingIconSize=22 |
| LoadingIndicator | `one-large` | automatic template class; do not bind manually | ^.one-large => Width=20; Height=20 |
| LoadingIndicator | `one-small` | automatic template class; do not bind manually | ^.one-small => Width=14; Height=14 |

`small` / `normal` / `large` are public density inputs; `one-small` / `one-normal` / `one-large` are automatic internal classes. `:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without additional appearance classes use their default theme and properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.ButtonLoading`](../../api/en/TheOne.Avalonia.Controls.ButtonLoading.md) | Class | 1 | 3 | 0 |
| [`TheOne.Avalonia.Controls.LoadingContainer`](../../api/en/TheOne.Avalonia.Controls.LoadingContainer.md) | Class | 10 | 1 | 0 |
| [`TheOne.Avalonia.Controls.LoadingIndicator`](../../api/en/TheOne.Avalonia.Controls.LoadingIndicator.md) | Class | 3 | 2 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
