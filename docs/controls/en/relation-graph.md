# RelationGraph

[简体中文](../zh-cn/relation-graph.md) · **English**

`RelationGraph` displays a tree with one root. `Orientation="Horizontal"` expands left to right; `Vertical` expands top to bottom. Node interiors always keep the icon on the left and title/content on the right. Load `OneTheme` first.

## Default model

```csharp
var root = new RelationGraphNode
{
    Title = "Engineering", Content = "Product and technology"
};
root.Children!.Add(new RelationGraphNode
{
    Title = "Application development", Content = "Desktop and cross-platform"
});
```

```xml
<ScrollViewer Height="540" HorizontalScrollBarVisibility="Auto"
              VerticalScrollBarVisibility="Auto">
  <one:RelationGraph RootNode="{Binding Root}" Orientation="Horizontal" />
</ScrollViewer>
```

`RelationGraphNode` provides `object? Icon`, `Title`, `Content`, and `IList<RelationGraphNode>? Children`. Each instance starts with its own `ObservableCollection<RelationGraphNode>`. Properties notify through `INotifyPropertyChanged`; replacing `Children` is supported and `null` means a leaf.

`RootNode` is a single object, not a collection of roots. A null root produces an empty graph. Cycles, repeated node instances, and null children are invalid: the graph stops displaying nodes and reports the reason through read-only `ValidationError`. Observable corrections automatically restore the tree.

## Custom models and ChildrenSource

Models need not inherit the default type or copy their tree. `ChildrenSource` stores a binding evaluated in **each node's DataContext**. It supports alternate collection names and nested paths. `IconSource`, `TitleSource`, and `ContentSource` select the display fields in the same way.

For an `OrganizationUnit` with `Branches`, `Symbol`, `Name`, and `Description`, and a view model exposing `OrganizationUnit Root`:

```xml
<one:RelationGraph RootNode="{Binding Root}" Orientation="Vertical"
    ChildrenSource="{CompiledBinding Branches, DataType={x:Type vm:OrganizationUnit}}"
    IconSource="{CompiledBinding Symbol, DataType={x:Type vm:OrganizationUnit}}"
    TitleSource="{CompiledBinding Name, DataType={x:Type vm:OrganizationUnit}}"
    ContentSource="{CompiledBinding Description, DataType={x:Type vm:OrganizationUnit}}" />
```

Declare the **node type** explicitly on these bindings; it differs from the view model type on the view root. Default models require no Source properties. Custom models require `ChildrenSource`, which must resolve to an `IEnumerable` or null (strings are invalid). Omitted display Source properties for custom models produce null regions.

Changing properties must notify through `INotifyPropertyChanged`, and changing collections through `INotifyCollectionChanged`, such as `ObservableCollection<T>`. Property replacement and collection changes update automatically. Call `RefreshData()` after in-place mutations of an ordinary `List<T>`. Make data changes on the UI thread. Detachment removes observers; reattachment reads the latest data.

C# consumers can use `Avalonia.Data.CompiledBinding.Create<TNode, TValue>(node => node.Branches)` for `ChildrenSource`. A property-name string is not a binding. Use compiled bindings for NativeAOT; the control does not scan model properties through reflection.

## Content and null regions

| Region | Data / template | Null behavior |
| --- | --- | --- |
| Left icon | `Icon` / `IconTemplate` | Removes the icon and gap; text gets the full width |
| Upper right title | `Title` / `TitleTemplate` | Centers content vertically in the entire text area |
| Lower right content | `Content` / `ContentTemplate` | Centers the title vertically in the entire text area |

When both title and content exist, the combined stack centers vertically. An icon-only node centers the icon across the whole card. An entirely null node retains its outline and connections. Templates alone never reserve regions for null data; empty strings still count as non-null.

Strings wrap; content may grow a node beyond `NodeMinHeight`. `Icon` can be a native control such as `PathIcon`, or Geometry / application data rendered using a template:

```xml
<one:RelationGraph.IconTemplate>
  <DataTemplate x:DataType="media:Geometry">
    <PathIcon Data="{Binding}" />
  </DataTemplate>
</one:RelationGraph.IconTemplate>
```

Declare `xmlns:media="using:Avalonia.Media"` on the view. Templates receive the selected field value as their data context. Native controls retain focus, commands, and keyboard behavior. A Control instance may belong to only one region; for repeated icons, share Geometry and create separate controls with a template.

## Layout and appearance

| Property | Normal default | Purpose |
| --- | --- | --- |
| `Orientation` | Horizontal | Tree expansion direction |
| `NodeWidth` / `NodeMinHeight` | 260 / 88 | Fixed card width / minimum natural height |
| `NodeSpacing` | 36 | Cross-axis spacing between subtrees |
| `LevelSpacing` | 80 | Depth spacing; each level uses its largest node height or width |
| `Padding` | 16 | Graph edge insets |
| `ConnectorBrush` / `ConnectorThickness` | One.Accent / 1.5 | Connector color and width; 0 width hides connectors |
| `ConnectorCornerRadius` / `PortRadius` | 8 / 3 | Orthogonal corner and endpoint radii |
| `NodeTheme` | Default theme | Replace the `RelationGraphNodePresenter` ControlTheme |
| `NodeCount` / `ValidationError` | Read-only | Displayed valid node count / validation error |

Dimensions, spacing, connectors, and templates are properties. A `NodeTheme` based on the default card theme can customize fill, outline, typography, icon dimensions, gaps, and template. `RelationGraphNodePresenter.Header` displays model `Title`. It also exposes `IsRoot`, `IconBackground`, `IconForeground`, `ContentForeground`, `HeaderFontSize`, `IconSize`, `IconTileSize`, `IconSpacing`, and `TextSpacing`. Default parts are `PART_Surface`, `PART_Layout`, `PART_IconHost`, `PART_IconPresenter`, `PART_TextHost`, `PART_HeaderPresenter`, and `PART_ContentPresenter`.

The theme uses `One.Surface`, `One.Text`, `One.Muted`, `One.Line`, `One.Accent`, `One.AccentSoft`, and `One.RowSelected`. The `:root` pseudo-class emphasizes the root outline and fill. Inherited `ControlSizing.Size` supports Small / Normal / Large: widths 220 / 260 / 300, minimum heights 72 / 88 / 104, with matching typography and icon changes. Explicit properties override density defaults.

The graph measures its entire natural extent. Wrap it in a native `ScrollViewer` for narrow viewports; it does not squeeze node text. All nodes are realized without virtualization, so use bounded relation trees. There is no built-in collapse, dragging, zoom, or selection. Tree traversal is iterative, connectors are drawn directly, and node content uses native presenters.

## Preview and verification

PreviewHost's “数据展示 → RelationGraph” page provides direction, width/spacing, root null regions, collection mutation, custom model bindings, and all eight slot combinations.

```sh
dotnet build TheOne.Avalonia.slnx --disable-build-servers -m:1
dotnet design/PreviewHost/bin/Debug/net10.0/PreviewHost.dll --verify-relation-graph .preview/relation-graph/managed
```

Checks cover light/dark themes, three densities, both orientations, actual connector pixels, narrow scrolling, wrapping, null slots, collection/property replacement, invalid-tree recovery, renewed observers, native interactions, and compiled XAML bindings. The independent NativeAOT consumer runs the same checks:

```sh
SDKROOT="$(xcrun --sdk macosx --show-sdk-path)" dotnet publish design/PreviewHost/Consumers/NativeAotSmoke -c Release -r osx-arm64 -p:PublishAot=true -p:TrimmerSingleWarn=false -p:CppCompilerAndLinker="$(xcrun --find clang)" --disable-build-servers -m:1 -o .preview/relation-graph/native
.preview/relation-graph/native/NativeAotSmoke --require-aot --verify-relation-graph .preview/relation-graph/native-pixels
```
