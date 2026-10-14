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
