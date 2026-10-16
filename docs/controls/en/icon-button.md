# Icon buttons and action hierarchy

[简体中文](../zh-cn/icon-button.md) · **English**

IconButton inherits Avalonia Button and keeps Command, CommandParameter, Click, keyboard input, focus, Flyout, and disabled behavior. Load OneTheme to use it. Icon is a separate property; Content remains text / content, and IconTemplate renders data icons. IconPlacement supports Left (default), Right, Top, and Bottom. Icons and text stay centered, with no empty gap when either is missing. IsLoading replaces the icon with a loading indicator and blocks repeated actions; see [loading controls](loading.md). Override IconSize and IconSpacing through styles or bindings. Default icons are 13 / 15 / 19 px for small / normal / large. A null Icon reserves no space; empty Content produces a square icon button. Icon-only buttons need AutomationProperties.Name and Tooltip.

```xml
<one:IconButton Icon="{Binding AddGeometry}" Content="新建" Classes="primary"
                Command="{Binding AddCommand}" />
<one:IconButton AutomationProperties.Name="收藏" ToolTip.Tip="收藏">
  <one:IconButton.Icon><PathIcon Data="M8,1 L10,6 L15,6 L11,9 L13,15 L8,12 L3,15 L5,9 L1,6 L6,6 Z" /></one:IconButton.Icon>
</one:IconButton>
```

The default PathIcon template renders Geometry. You can also assign an existing PathIcon or another icon Control to Icon. A control instance can have only one visual parent; use Geometry or a template for shared icon data.

The consuming application can reference [Material.Icons.Avalonia](https://www.nuget.org/packages/Material.Icons.Avalonia/) and load MaterialIconStyles in Application.Styles. The library does not require a third-party icon package. PreviewHost references 3.0.2 to demonstrate a compiled template:

```xml
<!-- material: Material.Icons.Avalonia；kinds: Material.Icons -->
<one:IconButton Icon="{Binding IconKind}" Content="工作区">
  <one:IconButton.IconTemplate>
    <DataTemplate x:DataType="kinds:MaterialIconKind">
      <material:MaterialIcon Kind="{Binding}" />
    </DataTemplate>
  </one:IconButton.IconTemplate>
</one:IconButton>
```

Native Button and IconButton share primary/accent, soft, gradient, outline, ghost, link, success, info, warning, and danger styles. The default style suits secondary actions; primary advances the main workflow, ghost is lightweight, outline emphasizes a boundary, and success/info/warning/danger indicate completion, information, risk, and destructive actions. Hover, pressed, focus, and disabled states keep native behavior and TheOne semantic colors. Prefer one primary action per group and explicit action labels.

Declare the view's x:DataType for compiled bindings and the icon data type on data IconTemplate instances. Implementation uses StyledProperty, TemplateBinding, and compiled templates without reflection-based icon creation. See [NativeAotSmoke](../../../design/PreviewHost/Consumers/NativeAotSmoke/Program.cs) for independent compiled-binding and native consumption examples.

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section enumerates declarations added by this library; native inherited members remain Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. Type links provide full model, method, event and enum signatures. PreviewHost keeps complete demos on their own pages, with styles, additional properties, APIs/events and models on each page. Only Button, Input and Select share comparison pages.

### ButtonLoading

[API / model reference](../../api/en/TheOne.Avalonia.Controls.ButtonLoading.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/ButtonLoading.cs#L13) | `bool` | writable; OneWay; attached | `false` | Shows busy feedback; buttons retain size and prevent repeated actions, while containers show a loading layer. The host resets it when work finishes. |

### IconButton

[API / model reference](../../api/en/TheOne.Avalonia.Controls.IconButton.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`HasIcon`](../../../TheOne.Avalonia/Controls/IconButton.cs#L30) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HasLabel`](../../../TheOne.Avalonia/Controls/IconButton.cs#L31) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Icon`](../../../TheOne.Avalonia/Controls/IconButton.cs#L24) | `object?` | writable; OneWay | `null` | Icon content/vector geometry as defined by the signature; the host owns external bitmap lifetimes. |
| [`IconPlacement`](../../../TheOne.Avalonia/Controls/IconButton.cs#L22) | `TheOne.Avalonia.Controls.IconPlacement` | writable; OneWay | `0 / default` | Icon position relative to text; the template arranges horizontal/vertical placement. Validation: `value => Enum.IsDefined(value)` Values: `Left`, `Right`, `Top`, `Bottom` |
| [`IconSize`](../../../TheOne.Avalonia/Controls/IconButton.cs#L26) | `double` | writable; OneWay | `15` | Icon drawing size in DIPs, independent of font size and button hit area. Validation: `value => double.IsFinite(value) && value > 0` |
| [`IconSpacing`](../../../TheOne.Avalonia/Controls/IconButton.cs#L27) | `double` | writable; OneWay | `8` | DIP spacing between the icon and adjacent text/content. Validation: `value => double.IsFinite(value) && value >= 0` |
| [`IconTemplate`](../../../TheOne.Avalonia/Controls/IconButton.cs#L25) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/IconButton.cs#L23) | `bool` | writable; OneWay | `inherited registration` | Shows busy feedback; buttons retain size and prevent repeated actions, while containers show a loading layer. The host resets it when work finishes. |
| [`ShowIcon`](../../../TheOne.Avalonia/Controls/IconButton.cs#L21) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

### ThemeButton

[API / model reference](../../api/en/TheOne.Avalonia.Controls.ThemeButton.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Scope`](../../../TheOne.Avalonia/Controls/ThemeButton.cs#L16) | `TheOne.Avalonia.Controls.ThemeButtonScope` | writable; OneWay | `0 / default` | Scope of Light/Dark switching; does not change the ThemeColor palette enum. Values: `Window`, `Application` |

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
| IconButton | `one-large` | automatic template class; do not bind manually | ^.one-large:icon-only => MinWidth=44; ^.one-large => IconSize=19; IconSpacing=10 |
| IconButton | `one-small` | automatic template class; do not bind manually | ^.one-small:icon-only => MinWidth=30; ^.one-small => IconSize=13; IconSpacing=6 |
| ThemeButton | `one-large` | automatic template class; do not bind manually | one&#124;ThemeButton.one-large => Width=44; Height=44 |
| ThemeButton | `one-normal` | automatic template class; do not bind manually | one&#124;ThemeButton.one-normal => Width=36; Height=36 |
| ThemeButton | `one-small` | automatic template class; do not bind manually | one&#124;ThemeButton.one-small => Width=30; Height=30 |

`small` / `normal` / `large` are public density inputs; `one-small` / `one-normal` / `one-large` are automatic internal classes. `:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without additional appearance classes use their default theme and properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.ButtonLoading`](../../api/en/TheOne.Avalonia.Controls.ButtonLoading.md) | Class | 1 | 3 | 0 |
| [`TheOne.Avalonia.Controls.IconButton`](../../api/en/TheOne.Avalonia.Controls.IconButton.md) | Class | 9 | 1 | 0 |
| [`TheOne.Avalonia.Controls.IconPlacement`](../../api/en/TheOne.Avalonia.Controls.IconPlacement.md) | Enum | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ThemeButton`](../../api/en/TheOne.Avalonia.Controls.ThemeButton.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ThemeButtonScope`](../../api/en/TheOne.Avalonia.Controls.ThemeButtonScope.md) | Enum | 2 | 1 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
