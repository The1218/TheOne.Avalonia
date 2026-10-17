# NavPage navigation shell

[简体中文](../zh-cn/nav-page.md) · **English**

NavPage combines a left NavMenu with an optional RightHeader and a RightBody containing the built-in `TabControl Classes="card"`. Model identity identifies a page. Reopening activates the retained view; tab switching preserves its editor state. Closing releases the shell's references. The host owns ViewModel resource lifetimes.

```csharp
var home = new NavPageModel { Title = "Home", ViewModel = homeVm, IsClosable = false };
var editor = new NavPageModel { Title = "Editor", ViewModel = editorVm, Icon = documentGeometry };
var shell = new NavPage
{
    HeaderTitle = "THE ONE", HeaderIcon = brandGeometry,
    DefaultHome = home, ItemsSource = new[] { home, editor }
};
```

Map ViewModels to views through host DataTemplates or PageTemplate. Place the shell in a finite viewport, such as a Grid `*` row. Its overall height is not hard-coded.

## Added properties

| Property | Purpose and short example |
| --- | --- |
| `ItemsSource` | Navigation collection; default items are NavPageModel. `ItemsSource = routes` |
| `DefaultHome` | Automatically opened when the template is ready; its model determines close permission. `DefaultHome = home` |
| `ViewModelBinding` | Custom item ViewModel source. `CompiledBinding.Create<Route, object?>(r => r.Screen)` |
| `TitleBinding` / `IconBinding` / `ChildrenBinding` | Compiled mappings for titles, icons and children; defaults read NavPageModel. See below |
| `PageTemplate` | Page content template; host DataTemplates apply when omitted. `<one:NavPage.PageTemplate><DataTemplate x:DataType="vm:EditorVm"><views:EditorView /></DataTemplate></one:NavPage.PageTemplate>` |
| `HeaderTitle` / `HeaderIcon` | Default brand title / icon. Clicking the icon toggles the sidebar; collapsed mode shows the icon only. `HeaderTitle="Workspace"` |
| `RightHeader` | Custom control or template data above the right body; default null. Null reserves no height, so RightBody fills the right side. `RightHeader = toolbar` |
| `RightHeaderTemplate` | Template whose data is RightHeader itself. `RightHeaderTemplate = toolbarTemplate` |
| `HeaderTemplate` / `FooterTemplate` | Sidebar templates with NavPage as their data. Custom headers provide their own toggle. See below |
| `IsCollapsed` | Two-way sidebar state. `IsCollapsed="{Binding Compact, Mode=TwoWay}"` |
| `ExpandedWidth` / `CollapsedWidth` | Sidebar widths; defaults 260 / 64 DIP. `ExpandedWidth="280"` |
| `TransitionDuration` | Theme default One.Motion.Expand; zero or Motion.Enabled=False switches immediately. `TransitionDuration="0:0:0.2"` |
| `ShowItemIcon` | Shows tab icons; default True. `ShowItemIcon="False"` |
| `SelectedPage` | Read-only current navigation source model. `Text="{Binding #Shell.SelectedPage}"` |

Shared sizing and motion are covered by [shared configuration](../../theme/en/README.md#shared-configuration). Default icons accept Geometry or text. Use HeaderTemplate for richer brand icons.

## Custom models and templates

RightHeader uses its natural height and RightBody fills the remaining space. Setting or clearing RightHeader preserves open tabs and their views. Null reserves no header space even when RightHeaderTemplate is specified.

```xml
<one:NavPage ItemsSource="{Binding Routes}">
  <one:NavPage.RightHeader>
    <StackPanel Orientation="Horizontal" Spacing="8" Margin="16,10">
      <TextBlock Text="Workspace" VerticalAlignment="Center" />
      <Button Content="Save" Command="{Binding Save}" />
    </StackPanel>
  </one:NavPage.RightHeader>
</one:NavPage>
```

Alternatively, bind `RightHeader="{Binding Toolbar}"` and provide RightHeaderTemplate with the appropriate x:DataType. RightBody is the built-in tab region and needs no separate value.

Compiled bindings avoid reflected string paths and support trimming. Models notify through Avalonia or INotifyPropertyChanged.

```xml
<one:NavPage ItemsSource="{Binding Routes}" x:Name="Shell"
             ViewModelBinding="{CompiledBinding Screen, DataType=vm:Route}"
             TitleBinding="{CompiledBinding Name, DataType=vm:Route}"
             IconBinding="{CompiledBinding Glyph, DataType=vm:Route}"
             ChildrenBinding="{CompiledBinding Nodes, DataType=vm:Route}">
  <one:NavPage.HeaderTemplate>
    <DataTemplate x:DataType="one:NavPage">
      <ToggleButton Content="Navigation" IsChecked="{Binding IsCollapsed, Mode=TwoWay}" />
    </DataTemplate>
  </one:NavPage.HeaderTemplate>
  <one:NavPage.FooterTemplate>
    <DataTemplate x:DataType="one:NavPage"><TextBlock Text="Workspace v1" Margin="12" /></DataTemplate>
  </one:NavPage.FooterTemplate>
</one:NavPage>
```

```csharp
shell.ViewModelBinding = CompiledBinding.Create<Route, object?>(r => r.Screen);
shell.TitleBinding = CompiledBinding.Create<Route, string>(r => r.Name);
shell.ChildrenBinding = CompiledBinding.Create<Route, IReadOnlyList<Route>>(r => r.Nodes);
```

Null ViewModels do not open a page. Custom models are closable by default and can be pinned through the menu. Cancel business closes with TabFeatures.TabClosingEvent; args.Item is the TabItem, whose DataContext is the navigation model.

## Business API

```csharp
shell.OpenPage(editor);       // activate the same instance without duplication
shell.ClosePage(editor);      // false when pinned, nonclosable or canceled
int count = shell.CloseAllPages(); // retain pinned, nonclosable and canceled pages
```

Menus offer close current, pin left / unpin and close all. Pinned pages precede ordinary pages and display a pushpin independently of ShowItemIcon. They cannot be dragged or reordered through Move or keyboard shortcuts. Both the ordinary drag preview and drop position stop after the pinned region. Unpinning restores ordinary behavior. Buttons, shortcuts, menus and APIs share the same close rules.

## Built-in NavPageModel

| Member | Purpose |
| --- | --- |
| `Title` | Navigation and tab title; default empty string |
| `Icon` | Navigation and tab icon; default null |
| `ViewModel` | Content ViewModel; default null |
| `Children` | Observable child collection; groups usually have no ViewModel |
| `IsClosable` | Default True; False prevents closing |
| `IsPinned` | Default False; True pins left, displays a pushpin and prevents closing and dragging; menus write back |

NavPageModel derives from AvaloniaObject and notifies changes. Changing ViewModel updates an open page; closing and reopening creates a fresh view. Replacing ItemsSource updates navigation while existing pages remain explicitly managed through close APIs, preserving drafts during dynamic menu changes.
