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
