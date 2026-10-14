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
