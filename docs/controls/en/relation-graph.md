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

## Node selection

SelectItem holds the selected **node model object**, defaults to null and defaults to TwoWay binding. Clicking a valid node selects it. A focused node supports Enter / Space; Avalonia still manages Tab navigation. Clicking a native button or input inside a node also selects the node while preserving that control's command and focus.

```xml
<one:RelationGraph RootNode="{Binding Root}"
                   SelectItem="{Binding SelectedNode, Mode=TwoWay}" />
```

SelectedNode should have the node model type, such as RelationGraphNode? or a custom OrganizationUnit?. Custom models still require ChildrenSource and the other compiled bindings. Selection matches objects by reference in the current tree; do not pass a newly constructed equal-looking object or a Presenter. Selection may initialize before RootNode and is validated after the first layout. Removing the selected node, clearing the root or invalidating the tree clears SelectItem and writes null back through the binding. Assigning an object outside a valid tree also clears selection. Assign null to clear it explicitly.

Presenters expose read-only IsSelected and the :selected pseudoclass. The default theme accents the fill, outline and icon tile without changing node geometry. Customize the state through NodeTheme. Disabled nodes do not respond to pointer or keyboard selection.

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

The graph measures its entire natural extent. Wrap it in a native `ScrollViewer` for narrow viewports; it does not squeeze node text. All nodes are realized without virtualization, so use bounded relation trees. There is no built-in collapse, dragging or zoom. Tree traversal is iterative, connectors are drawn directly, and node content uses native presenters.

## Preview and verification

PreviewHost's “数据展示 → RelationGraph” page provides direction, width/spacing, root null regions, collection mutation, custom model bindings, and all eight slot combinations.

```sh
dotnet build design/PreviewHost/PreviewHost.csproj -c Release --artifacts-path .preview/relation-graph/artifacts --disable-build-servers -m:1
dotnet .preview/relation-graph/artifacts/bin/PreviewHost/release/PreviewHost.dll --verify-relation-graph .preview/relation-graph/managed
```

Checks cover light/dark themes, three densities, both orientations, actual connector pixels, narrow scrolling, wrapping, null slots, collection/property replacement, invalid-tree recovery, renewed observers, native interactions, and compiled XAML bindings. The independent NativeAOT consumer runs the same checks:

```sh
# macOS: native checks only for affected AOT/binding/metadata changes.
SDKROOT="$(xcrun --sdk macosx --show-sdk-path)" dotnet publish design/PreviewHost/Consumers/NativeAotSmoke -c Release -r osx-arm64 -p:PublishAot=true -p:TrimmerSingleWarn=false -p:CppCompilerAndLinker="$(xcrun --find clang)" --disable-build-servers -m:1 --artifacts-path .preview/relation-graph/native-artifacts -o .preview/relation-graph/native
.preview/relation-graph/native/NativeAotSmoke --require-aot --verify-relation-graph .preview/relation-graph/native-pixels
```

Selection checks cover light/dark, three densities, both directions, rendered selection pixels, pointer/keyboard selection, embedded button commands and focus, removal/invalid-tree cleanup, and two-way compiled bindings for default/custom models. The native consumer runs the same --verify-relation-graph suite.

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### RelationGraph

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.RelationGraph.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Background`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L53) | `Avalonia.Media.IBrush?` | writable; OneWay | `inherited registration` | Background brush; use DynamicResource for theme-dependent surfaces. |
| [`ChildrenSource`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L60) | `Avalonia.Data.BindingBase?` | writable; OneWay | `null` | A binding evaluated against each node, not against the graph's data context. |
| [`ConnectorBrush`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L73) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | One.Accent / 1.5 ; Connector color and width; 0 width hides connectors |
| [`ConnectorCornerRadius`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L75) | `double` | writable; OneWay | `8` | 8 / 3 ; Orthogonal corner and endpoint radii Validation: `NonNegative` |
| [`ConnectorThickness`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L74) | `double` | writable; OneWay | `1.5` | One.Accent / 1.5 ; Connector color and width; 0 width hides connectors Validation: `NonNegative` |
| [`ContentSource`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L63) | `Avalonia.Data.BindingBase?` | writable; OneWay | `null` | Typed accessor for relation-node body content, without reflection paths. |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L66) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`IconSource`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L61) | `Avalonia.Data.BindingBase?` | writable; OneWay | `null` | Typed accessor for relation-node icons. |
| [`IconTemplate`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L64) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`LevelSpacing`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L71) | `double` | writable; OneWay | `80` | 80 ; Depth spacing; each level uses its largest node height or width Validation: `NonNegative` |
| [`NodeCount`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L79) | `int` | read-only; OneWay | `computed / state` | Read-only ; Displayed valid node count / validation error |
| [`NodeMinHeight`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L69) | `double` | writable; OneWay | `88` | 260 / 88 ; Fixed card width / minimum natural height Validation: `Positive` |
| [`NodeSpacing`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L70) | `double` | writable; OneWay | `36` | 36 ; Cross-axis spacing between subtrees Validation: `NonNegative` |
| [`NodeTheme`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L67) | `Avalonia.Styling.ControlTheme?` | writable; OneWay | `null` | Default theme ; Replace the `RelationGraphNodePresenter` ControlTheme |
| [`NodeWidth`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L68) | `double` | writable; OneWay | `260` | 260 / 88 ; Fixed card width / minimum natural height Validation: `Positive` |
| [`Orientation`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L58) | `Avalonia.Layout.Orientation` | writable; OneWay | `Orientation.Horizontal` | Horizontal ; Tree expansion direction Validation: `Enum.IsDefined` Values: `Horizontal`, `Vertical` |
| [`Padding`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L72) | `Avalonia.Thickness` | writable; OneWay | `inherited registration` | 16 ; Graph edge insets |
| [`PortRadius`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L76) | `double` | writable; OneWay | `3` | 8 / 3 ; Orthogonal corner and endpoint radii Validation: `NonNegative` |
| [`RootNode`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L55) | `object?` | writable; OneWay | `null` | Root model for a relation graph; children come from its tree/accessor contract. |
| [`SelectItem`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L57) | `object?` | writable; TwoWay | `null` | The selected node model, matched by reference. Null clears selection. |
| [`TitleSource`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L62) | `Avalonia.Data.BindingBase?` | writable; OneWay | `null` | Typed accessor for relation-node titles. |
| [`TitleTemplate`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L65) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`ValidationError`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L78) | `string?` | read-only; OneWay | `computed / state` | Read-only ; Displayed valid node count / validation error |

### RelationGraphNode

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.RelationGraphNode.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Children`](../../../TheOne.Avalonia/Controls/RelationGraphNode.cs#L18) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.RelationGraphNode>?` | writable; CLR / n/a | `new ObservableCollection<RelationGraphNode>()` | Child model collection; retain identity/parent relationships and avoid cycles/shared mutable nodes. |
| [`Content`](../../../TheOne.Avalonia/Controls/RelationGraphNode.cs#L17) | `object?` | writable; CLR / n/a | `null` | Host content/presentation object; follow the content-template scope and do not persist visual objects as business data. |
| [`Icon`](../../../TheOne.Avalonia/Controls/RelationGraphNode.cs#L15) | `object?` | writable; CLR / n/a | `null` | Icon content/vector geometry as defined by the signature; the host owns external bitmap lifetimes. |
| [`Title`](../../../TheOne.Avalonia/Controls/RelationGraphNode.cs#L16) | `object?` | writable; CLR / n/a | `null` | Display title, distinct from business identity or a localization key. |

### RelationGraphNodePresenter

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.RelationGraphNodePresenter.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ContentForeground`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L39) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Node body text brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`HeaderFontSize`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L38) | `double` | writable; OneWay | `16` | Node-title font size in DIPs, included in actual text measurement. Validation: `Positive` |
| [`Icon`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L32) | `object?` | writable; OneWay | `null` | Icon content/vector geometry as defined by the signature; the host owns external bitmap lifetimes. |
| [`IconBackground`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L40) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Icon-tile background brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`IconForeground`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L41) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Icon foreground brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`IconSize`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L34) | `double` | writable; OneWay | `24` | Icon drawing size in DIPs, independent of font size and button hit area. Validation: `Positive` |
| [`IconSpacing`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L36) | `double` | writable; OneWay | `14` | DIP spacing between the icon and adjacent text/content. Validation: `NonNegative` |
| [`IconTemplate`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L33) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`IconTileSize`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L35) | `double` | writable; OneWay | `44` | Icon-tile size in DIPs, independent of icon size. Validation: `Positive` |
| [`IsRoot`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L42) | `bool` | writable; OneWay | `false` | Whether this is the root node, maintained by the graph presenter. |
| [`IsSelected`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L44) | `bool` | read-only; OneWay | `computed / state` | Selection state maintained by the owning RelationGraph. |
| [`TextSpacing`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L37) | `double` | writable; OneWay | `6` | DIP spacing between title and body text. Validation: `NonNegative` |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

No added public appearance classes; use the default theme and control-specific properties.
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

| Type | Action signature | Purpose |
| --- | --- | --- |
| [`RelationGraph`](../../api/en/TheOne.Avalonia.Controls.RelationGraph.md) | `RelationGraphNodePresenter? RelationGraph.GetNodePresenter(object node)` | Finds a realized native card; returns null before layout or for invalid trees. |
| [`RelationGraph`](../../api/en/TheOne.Avalonia.Controls.RelationGraph.md) | `void RelationGraph.RefreshData()` | Refresh after changing a collection that does not notify. |

### Event subscriptions

Subscribe when needed and detach when the host is disposed. These are event entry points, not routed-event identifier fields.

| Type | Event | Purpose |
| --- | --- | --- |
| [`RelationGraphNode`](../../api/en/TheOne.Avalonia.Controls.RelationGraphNode.md) | `PropertyChanged` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

### Related models

| Model | Properties/enum values |
| --- | ---: |
| [`TheOne.Avalonia.Controls.RelationGraphNode`](../../api/en/TheOne.Avalonia.Controls.RelationGraphNode.md) | 4 |

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
