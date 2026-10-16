# InfoCard

[简体中文](../zh-cn/info-card.md) · **English**

`InfoCard` inherits native `HeaderedContentControl` for headings / content, adding header icons, header actions, and a footer. With `OneTheme`, it uses TheOne surfaces, thin borders, radii, light / dark themes, and sizes. Existing regions remain displayed; buttons, inputs, and lists within them retain native interactions.

```xml
<one:InfoCard Header="工作区概览"
              Content="使用统一的标题、正文和间距展示信息。" />
```

`one` is `using:TheOne.Avalonia.Controls`. An empty heading, icon, and action area removes the whole header row. Null, empty, or whitespace `Header` removes only its text; icons / actions can remain independently. Empty regions hide automatically with no reserved spacing.

## Custom headers and content

Headers / content accept strings, controls, or models. Control instances become presenter children directly and retain native input, focus, commands, and validation.

```xml
<one:InfoCard>
  <one:InfoCard.Header>
    <Grid ColumnDefinitions="*,Auto" ColumnSpacing="12">
      <TextBlock Text="工作区设置" FontWeight="SemiBold" />
      <Button Grid.Column="1" Content="保存" Command="{Binding SaveCommand}" />
    </Grid>
  </one:InfoCard.Header>
  <TextBox Text="{Binding WorkspaceName, Mode=TwoWay}" />
</one:InfoCard>
```

## Header icons, actions, and footers

`HeaderIcon` precedes headings, `HeaderActions` appears to the right, and `Footer` follows content. All accept controls, text, or models, with `HeaderIconTemplate`, `HeaderActionsTemplate`, and `FooterTemplate` for models.

```xml
<one:InfoCard Header="工作区设置">
  <one:InfoCard.HeaderIcon>
    <PathIcon Data="M 2,2 L 18,2 L 18,18 L 2,18 Z" />
  </one:InfoCard.HeaderIcon>
  <one:InfoCard.HeaderActions>
    <Button Content="详情" Command="{Binding DetailsCommand}" />
  </one:InfoCard.HeaderActions>
  <one:InfoCard.Footer>
    <Grid ColumnDefinitions="*,Auto" ColumnSpacing="12">
      <TextBlock Text="{Binding Status}" TextWrapping="Wrap" />
      <Button Grid.Column="1" Content="同步" Command="{Binding SyncCommand}" />
    </Grid>
  </one:InfoCard.Footer>
  <TextBox Text="{Binding WorkspaceName, Mode=TwoWay}" />
</one:InfoCard>
```

Explicit templates can accept null data, so a set template can keep a region visible when content is null. To hide icon / actions / footer completely, clear both content and its template. Preview toggles change both and reclaim spacing. `HeaderIconSize` sizes custom icons, `HeaderContentSpacing` separates header parts, and `FooterSpacing` separates the footer from preceding regions.

Footers also support typed compiled templates:

```xml
<one:InfoCard Header="工作区概览" Content="{Binding Description}" Footer="{Binding CurrentWorkspace}">
  <one:InfoCard.FooterTemplate>
    <DataTemplate x:DataType="vm:WorkspaceViewModel">
      <Grid ColumnDefinitions="*,Auto" ColumnSpacing="12">
        <TextBlock Text="{Binding Status}" TextWrapping="Wrap" />
        <Button Grid.Column="1" Content="同步" Command="{Binding SyncCommand}" />
      </Grid>
    </DataTemplate>
  </one:InfoCard.FooterTemplate>
</one:InfoCard>
```

Declare correct `x:DataType` on bound view roots. `HeaderTemplate` / `ContentTemplate` use native `IDataTemplate`; each template also needs its own type for compiled-binding / NativeAOT compatibility.

```xml
<one:InfoCard Header="{Binding CurrentWorkspace}" Content="{Binding CurrentWorkspace}">
  <one:InfoCard.HeaderTemplate>
    <DataTemplate x:DataType="vm:WorkspaceViewModel">
      <TextBlock Text="{Binding Name}" FontWeight="SemiBold" />
    </DataTemplate>
  </one:InfoCard.HeaderTemplate>
  <one:InfoCard.ContentTemplate>
    <DataTemplate x:DataType="vm:WorkspaceViewModel">
      <TextBlock Text="{Binding Description}" TextWrapping="Wrap" />
    </DataTemplate>
  </one:InfoCard.ContentTemplate>
</one:InfoCard>
```

## Region separators

`ShowHeaderSeparator` and `ShowFooterSeparator` default to `true` and can be set independently, bound, or styled.

```xml
<one:InfoCard Header="工作区" Footer="已保存"
              ShowHeaderSeparator="True"
              ShowFooterSeparator="False"
              Content="正文内容" />
```

Header separators appear only with both header and body. Footer separators require a footer and a preceding body or header. Missing regions leave no empty line. Separators use `One.Line`, are 1 DIP thick, join inner border edges, and sit between region backgrounds. Hiding removes only that 1 DIP row without changing padding.

## Region backgrounds and padding

Header has a stronger background distinction; Content / Footer differ subtly. Defaults are `One.Subtle`, `One.Surface`, and `One.InfoCard.Footer`, filling regions and clipping to card radii with Light / Dark changes. Light Footer is `#FCFCFC` versus body `#FFFFFF`; Dark Footer is `#131313` versus body `#0F0F0F`, with channel differences no greater than 4.

| Property | Purpose |
| --- | --- |
| `HeaderBackground` | Header background, default `One.Subtle` |
| `Background` | Body and card base, default `One.Surface` |
| `FooterBackground` | Footer background, default `One.InfoCard.Footer` |
| `Padding` | Body padding and header / footer horizontal padding |
| `HeaderSpacing` | Header vertical padding |
| `FooterSpacing` | Footer vertical padding |

`HeaderPadding` / `FooterPadding` are computed read-only template layout properties. Absent regions reserve neither background nor padding. Disabled regions all use `One.DisabledSurface`. Instance properties / Style setters override backgrounds.

```xml
<one:InfoCard Header="标题" Content="正文" Footer="页脚"
              HeaderBackground="{DynamicResource One.Subtle}"
              Background="{DynamicResource One.Surface}"
              FooterBackground="{DynamicResource One.InfoCard.Footer}" />
```

## Sizes and appearances

`Classes="shadow"` adds a persistent outer shadow. `Classes="hove"` accents the outline, changes the body surface and adds a stronger shadow while the pointer is over the card; leaving restores the normal appearance. The API class name is `hove`. Combine them with `Classes="hove shadow"` for a persistent shadow with stronger hover feedback. Disabled cards stop these class effects.

```xml
<one:InfoCard Classes="hove shadow" Header="Interactive card" Margin="16"
              Content="Hover to see the accent outline and shadow." />
```

The effects do not move content, increase layout size or intercept native input. A separate PART_Shadow renders outside the rounded, clipped PART_Surface. Leave sufficient Margin / Padding when an ancestor clips its contents. Override BoxShadow locally or in a style, for example `BoxShadow="0 2 10 0 #30000000"`; explicit local values take precedence over class styles. Defaults use the theme-aware One.InfoCard.Shadow and One.InfoCard.HoverShadow resources.

`Classes="outlined"` uses an accent border and soft heading, `Classes="glow"` uses a glow gradient heading, and `Classes="flat"` removes outer borders / separators with transparent header / footer for embedded layouts. These styles keep content slots / templates and provide no expansion or collapse.

Inherit parent `ControlSizing.Size` or set `Classes="small"` / `normal` / `large` on a card. Style setters / local properties can override the following.

| Property | small | normal | large |
| --- | ---: | ---: | ---: |
| `Padding` | 16 | 20 | 24 |
| `FontSize` | 12 | 14 | 16 |
| `HeaderFontSize` | 14 | 16 | 18 |
| `HeaderSpacing` | 8 | 12 | 16 |
| `HeaderIconSize` | 16 | 18 | 20 |
| `HeaderContentSpacing` | 8 | 10 | 12 |
| `FooterSpacing` | 8 | 12 | 16 |

Standard `Background`, `Foreground`, `BorderBrush`, `BorderThickness`, and `CornerRadius` remain. `HeaderSpacing` / `FooterSpacing` control vertical region padding. Without fixed height, content determines card height. Disabling cards also disables native children in all regions.

## List content

InfoCard does not replace child layout or add an infinite-height scroll container. Growing collections need bounded native virtualization controls and model `ItemsSource` rather than prebuilt controls.

```xml
<one:InfoCard Header="成员">
  <ListBox Height="200" ItemsSource="{Binding Members}" />
</one:InfoCard>
```

Set finite `Height` / `MaxHeight` and use a `*` row in body `Grid` to give lists the bounded remaining viewport. Collapse / restoration follows those constraints; avoid unbounded-height `StackPanel` without list height.

PreviewHost exposes independent header / icon / actions / footer / separator switches, native inputs / buttons, typed compiled templates for every slot, and a fixed-height thousand-member list with last-item navigation. Height / MaxHeight checks cover recycling and footer visibility. Header / footer actions update state; disabling affects all regions. Only this control's images are rendered:

See [preview entry points](../../../design/PreviewHost/Program.cs) for commands and scope.

Capture rendered hover and shadow pixels with the existing suite:

```bash
dotnet design/PreviewHost/bin/Debug/net10.0/PreviewHost.dll --verify-infocard .preview/infocard-effects/managed
SDKROOT="$(xcrun --sdk macosx --show-sdk-path)" dotnet publish design/PreviewHost/Consumers/NativeAotSmoke -c Release -r osx-arm64 -p:PublishAot=true -p:TrimmerSingleWarn=false -p:CppCompilerAndLinker="$(xcrun --find clang)" --disable-build-servers -m:1 -o .preview/infocard-effects/native
.preview/infocard-effects/native/NativeAotSmoke --require-aot --verify-infocard .preview/infocard-effects/native-pixels
```

Build the native executable through the existing NativeAotSmoke consumer, using macOS xcrun clang and SDK. The suite covers light/dark, three densities, pointer entry/exit, combined classes, disabling, local overrides and actual shadow pixels.

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section enumerates declarations added by this library; native inherited members remain Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. Type links provide full model, method, event and enum signatures. PreviewHost keeps complete demos on their own pages, with styles, additional properties, APIs/events and models on each page. Only Button, Input and Select share comparison pages.

### InfoCard

[API / model reference](../../api/en/TheOne.Avalonia.Controls.InfoCard.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`BoxShadow`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L50) | `Avalonia.Media.BoxShadows` | writable; OneWay | `inherited registration` | Outer shadow, supplied by the shadow and hove theme classes. |
| [`Footer`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L56) | `object?` | writable; OneWay | `null` | Footer content, customized independently of body/header. |
| [`FooterBackground`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L61) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Footer background, default `One.InfoCard.Footer` |
| [`FooterPadding`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L65) | `Avalonia.Thickness` | read-only; OneWay | `computed / state` | Computed footer insets: horizontal Padding and vertical FooterSpacing. |
| [`FooterSpacing`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L74) | `double` | writable; OneWay | `12` | Footer vertical padding Validation: `value => double.IsFinite(value) && value >= 0` |
| [`FooterTemplate`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L57) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`HeaderActions`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L54) | `object?` | writable; OneWay | `null` | Action content beside the header, such as business buttons with native command/focus behavior. |
| [`HeaderActionsTemplate`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L55) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`HeaderBackground`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L59) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Header background, default `One.Subtle` |
| [`HeaderContentSpacing`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L69) | `double` | writable; OneWay | `10` | 8 ; 10 ; 12 Validation: `value => double.IsFinite(value) && value >= 0` |
| [`HeaderFontSize`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L67) | `double` | writable; OneWay | `16` | 14 ; 16 ; 18 Validation: `value => double.IsFinite(value) && value > 0` |
| [`HeaderIcon`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L52) | `object?` | writable; OneWay | `null` | Header icon content, customized independently of its text. |
| [`HeaderIconSize`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L68) | `double` | writable; OneWay | `18` | 16 ; 18 ; 20 Validation: `value => double.IsFinite(value) && value > 0` |
| [`HeaderIconTemplate`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L53) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`HeaderPadding`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L63) | `Avalonia.Thickness` | read-only; OneWay | `computed / state` | Computed header insets: horizontal Padding and vertical HeaderSpacing. |
| [`HeaderSpacing`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L66) | `double` | writable; OneWay | `12` | Header vertical padding Validation: `value => double.IsFinite(value) && value >= 0` |
| [`ShowFooterSeparator`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L73) | `bool` | writable; OneWay | `true` | Shows a divider before an existing footer when another region precedes it. |
| [`ShowHeaderSeparator`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L71) | `bool` | writable; OneWay | `true` | Shows an edge-to-edge divider between the header and body. |

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
| InfoCard | `flat` | explicit Classes; combine with density | one&#124;InfoCard.flat => BorderThickness=0; HeaderBackground=Transparent; FooterBackground=Transparent; ShowHeaderSeparator=False; ShowFooterSeparator=False; one&#124;InfoCard.glow:disabled, one&#124;InfoCard.flat:disabled => HeaderBackground={DynamicResource One.DisabledSurface}; FooterBackground={DynamicResource One.DisabledSurface} |
| InfoCard | `glow` | explicit Classes; combine with density | one&#124;InfoCard.glow => HeaderBackground={DynamicResource One.Gradient.Glow}; one&#124;InfoCard.glow:disabled, one&#124;InfoCard.flat:disabled => HeaderBackground={DynamicResource One.DisabledSurface}; FooterBackground={DynamicResource One.DisabledSurface} |
| InfoCard | `hove` | explicit Classes; combine with density | ^.hove:pointerover:not(:disabled) => BorderBrush={DynamicResource One.Accent}; Background={DynamicResource One.SurfaceHover}; BoxShadow={DynamicResource One.InfoCard.HoverShadow} |
| InfoCard | `one-large` | automatic template class; do not bind manually | one&#124;InfoCard.one-large => Padding=24; FontSize=16; HeaderFontSize=18; HeaderSpacing=16; HeaderIconSize=20; HeaderContentSpacing=12; FooterSpacing=16 |
| InfoCard | `one-normal` | automatic template class; do not bind manually | one&#124;InfoCard.one-normal => Padding=20; FontSize=14; HeaderFontSize=16; HeaderSpacing=12; HeaderIconSize=18; HeaderContentSpacing=10; FooterSpacing=12 |
| InfoCard | `one-small` | automatic template class; do not bind manually | one&#124;InfoCard.one-small => Padding=16; FontSize=12; HeaderFontSize=14; HeaderSpacing=8; HeaderIconSize=16; HeaderContentSpacing=8; FooterSpacing=8 |
| InfoCard | `outlined` | explicit Classes; combine with density | one&#124;InfoCard.outlined => BorderBrush={DynamicResource One.Accent}; HeaderBackground={DynamicResource One.AccentSoft}; one&#124;InfoCard.outlined:disabled => BorderBrush={DynamicResource One.Line}; HeaderBackground={DynamicResource One.DisabledSurface} |
| InfoCard | `shadow` | explicit Classes; combine with density | ^.shadow:not(:disabled) => BoxShadow={DynamicResource One.InfoCard.Shadow} |

`small` / `normal` / `large` are public density inputs; `one-small` / `one-normal` / `one-large` are automatic internal classes. `:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without additional appearance classes use their default theme and properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.InfoCard`](../../api/en/TheOne.Avalonia.Controls.InfoCard.md) | Class | 18 | 1 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
