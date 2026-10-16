# FlowStep and FlowNode

[简体中文](../zh-cn/flow-step.md) · **English**

`FlowStep` is a linear flow display based on native `ItemsControl`; `FlowNode` is its native `ContentControl` container. With `OneTheme`, both orientations, Light / Dark and Small / Normal / Large are supported. Business code sets states explicitly. The control does not advance steps or validate forms.

## Default content

```xml
<one:FlowStep Orientation="Vertical">
  <one:FlowNode Status="Completed">
    <one:FlowNode.Content>
      <one:FlowNodeContent Title="Submit application" Time="10-14 09:20"
                           State="Submitted" Text="Application materials submitted." />
    </one:FlowNode.Content>
  </one:FlowNode>
  <one:FlowNode Status="InProgress">
    <one:FlowNode.Content>
      <one:FlowNodeContent Title="Manager approval" Text="Waiting for confirmation." />
    </one:FlowNode.Content>
  </one:FlowNode>
  <one:FlowNode>
    <one:FlowNode.Content><one:FlowNodeContent Title="Finish" /></one:FlowNode.Content>
  </one:FlowNode>
</one:FlowStep>
```

`one` means `using:TheOne.Avalonia.Controls`. `FlowNodeContent.Title`, `Time`, `State` and `Text` are nullable strings with change notifications. Applications format Time; the control does not convert time zones. Empty titles, times and descriptions collapse. Empty State falls back to the built-in Chinese labels “已完成 / 进行中 / 未完成”; supply State for localized labels. State is display text; Status is the marker / connector enum.

| Status | Marker |
| --- | --- |
| `Completed` | Success-coloured filled circle and check |
| `InProgress` | Accent ring and centre dot |
| `Pending` (default) | Neutral empty ring |

Between adjacent visible nodes, a connector is success-coloured when its source is Completed and its destination is Completed / InProgress; otherwise it is neutral. A single node has no connector. Multiple InProgress nodes are allowed; no business state is inferred.

## Collection binding

Use `ObservableCollection<FlowNodeItem>`. Each item exposes notifying Status, Content and optional ContentTemplate. Compiled accessors connect models to containers without reflection or metadata registration.

```xml
<!-- Declare x:DataType="vm:FlowViewModel" on the view root. -->
<one:FlowStep ItemsSource="{Binding Nodes}" Orientation="Horizontal" />
```

Content accepts FlowNodeContent, custom models or Controls. Plain data items are also accepted by ItemsSource and default to Pending; map a custom model's status through compiled bindings in ItemContainerTheme. Bind Status yourself on directly declared FlowNodes.

## Custom ContentTemplate

```xml
<one:FlowNode Status="InProgress" Content="{Binding Approval}">
  <one:FlowNode.ContentTemplate>
    <DataTemplate x:DataType="vm:ApprovalContent">
      <StackPanel Spacing="8">
        <TextBlock Text="{Binding Title}" FontWeight="SemiBold" />
        <TextBlock Text="{Binding Description}" TextWrapping="Wrap" />
        <Button Content="View details" Command="{Binding DetailsCommand}" />
      </StackPanel>
    </DataTemplate>
  </one:FlowNode.ContentTemplate>
</one:FlowNode>
```

FlowStep.ContentTemplate supplies a shared template to every node. Its data context is the node's Content. Declare the correct x:DataType on each bound view and DataTemplate. Only the content area changes; markers and connectors remain intact. Buttons and editors retain native focus, commands and keyboard interaction. Custom templates bind their own business status descriptions.

Priority: node ContentTemplate → FlowStep.ContentTemplate → native ItemTemplate → matching local / application DataTemplate → built-in FlowNodeContent template. Clearing overrides restores subsequent rules. Controls render directly when no explicit template is supplied. `One.FlowNode.DefaultContentTemplate` is the default resource key; FlowNode.DefaultContentTemplate can replace that fallback.

## Layout, sizes and limits

Horizontal nodes have equal widths with a MinNodeWidth lower bound. A narrow viewport scrolls horizontally rather than wrapping the sequence. Vertical rows follow content height; title and state align to opposite edges, with the state moving to a separate line when necessary. Long text wraps. Connectors follow actual marker centres.

| Property | Small / Normal / Large defaults |
| --- | --- |
| FlowStep.NodeSpacing | 20 / 24 / 32 |
| FlowStep.MinNodeWidth | 150 / 180 / 210 |
| FlowNode.IndicatorSize | 24 / 30 / 36 |
| FlowNode.ContentSpacing | 10 / 12 / 16 |
| FlowNode.FontSize | 12 / 14 / 16 |

Orientation defaults to Horizontal and can also be set on standalone FlowNodes. Local values override density styles. The default theme uses One.Text, One.Muted, One.Accent, One.Success, One.OnAccent and One.Line, without looping animation. FlowStep has no selection / navigation; nodes are not tab stops by default, while interactive content retains its normal tab order.

The dedicated panel is non-virtualized and intended for finite business flows. Use a virtualized list for large histories. Vertical scrolling requires bounded height or MaxHeight; retain natural height when the outer page handles scrolling.

## Examples and verification

PreviewHost's “扩展控件 → Step → FlowStep” page shows both layouts, three states, per-node and shared templates, collection edits, flow advancement and long descriptions.

```sh
dotnet build design/PreviewHost/PreviewHost.csproj -c Release --artifacts-path .preview/flowstep/artifacts
dotnet .preview/flowstep/artifacts/bin/PreviewHost/release/PreviewHost.dll --verify-flow-step .preview/flowstep/render
dotnet .preview/flowstep/artifacts/bin/PreviewHost/release/PreviewHost.dll --render-controls .preview/flowstep/render FlowStep
```

The targeted checks cover both themes, three sizes, narrow viewports, actual connector pixels, collection / model changes, template priority and a custom button command. The independent NativeAotSmoke consumer supports `--verify-flow-step --require-aot`. Publish and run a real PublishAot executable for native verification; a managed run does not replace it.

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section enumerates declarations added by this library; native inherited members remain Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. Type links provide full model, method, event and enum signatures. PreviewHost keeps complete demos on their own pages, with styles, additional properties, APIs/events and models on each page. Only Button, Input and Select share comparison pages.

### FlowNode

[API / model reference](../../api/en/TheOne.Avalonia.Controls.FlowNode.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ContentSpacing`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L40) | `double` | writable; OneWay | `12` | 10 / 12 / 16 Validation: `value => double.IsFinite(value) && value >= 0` |
| [`DefaultContentTemplate`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L42) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Fallback for FlowNodeContent; OneTheme supplies the default four-field template. |
| [`EffectiveContentTemplate`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L43) | `Avalonia.Controls.Templates.IDataTemplate?` | read-only; OneWay | `computed / state` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`EffectiveStateText`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L44) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IndicatorSize`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L39) | `double` | writable; OneWay | `30` | 24 / 30 / 36 Validation: `value => double.IsFinite(value) && value > 0` |
| [`IsCompleted`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L46) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsInProgress`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L47) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsVertical`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L45) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Orientation`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L38) | `Avalonia.Layout.Orientation` | writable; OneWay | `inherited registration` | Horizontal/vertical arrangement; updates node, connector and content layout without changing business order. Values: `Horizontal`, `Vertical` |
| [`Status`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L37) | `TheOne.Avalonia.Controls.FlowNodeStatus` | writable; OneWay | `FlowNodeStatus.Pending` | Marker Validation: `Enum.IsDefined` Values: `Pending`, `InProgress`, `Completed` |

### FlowNodePanel

[API / model reference](../../api/en/TheOne.Avalonia.Controls.FlowNodePanel.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ContentSpacing`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L64) | `double` | writable; OneWay | `inherited registration` | 10 / 12 / 16 |
| [`Orientation`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L63) | `Avalonia.Layout.Orientation` | writable; OneWay | `inherited registration` | Horizontal/vertical arrangement; updates node, connector and content layout without changing business order. Values: `Horizontal`, `Vertical` |

### FlowStep

[API / model reference](../../api/en/TheOne.Avalonia.Controls.FlowStep.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L38) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`MinNodeWidth`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L37) | `double` | writable; OneWay; inherits | `180` | 150 / 180 / 210 Validation: `value => double.IsFinite(value) && value > 0` |
| [`NodeSpacing`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L36) | `double` | writable; OneWay; inherits | `24` | 20 / 24 / 32 Validation: `value => double.IsFinite(value) && value >= 0` |
| [`Orientation`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L35) | `Avalonia.Layout.Orientation` | writable; OneWay; inherits | `Orientation.Horizontal` | Horizontal/vertical arrangement; updates node, connector and content layout without changing business order. Validation: `Enum.IsDefined` Values: `Horizontal`, `Vertical` |

### FlowStepConnectors

[API / model reference](../../api/en/TheOne.Avalonia.Controls.FlowStepConnectors.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CompletedConnectorBrush`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L17) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Completed-step connector brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`ConnectorBrush`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L16) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Pending-step connector brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`ConnectorThickness`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L18) | `double` | writable; OneWay | `2` | Connector stroke width in DIPs, independent of business node state. Validation: `value => double.IsFinite(value) && value >= 0` |

### FlowStepPanel

[API / model reference](../../api/en/TheOne.Avalonia.Controls.FlowStepPanel.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`MinNodeWidth`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L23) | `double` | writable; OneWay | `inherited registration` | 150 / 180 / 210 |
| [`NodeSpacing`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L22) | `double` | writable; OneWay | `inherited registration` | 20 / 24 / 32 |
| [`Orientation`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L21) | `Avalonia.Layout.Orientation` | writable; OneWay | `inherited registration` | Horizontal/vertical arrangement; updates node, connector and content layout without changing business order. Values: `Horizontal`, `Vertical` |
| [`ViewportWidth`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L24) | `double` | writable; OneWay | `0` | Available width supplied to flow layout, normally maintained by the template. Validation: `value => double.IsFinite(value) && value >= 0` |

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
| FlowNode | `one-large` | automatic template class; do not bind manually | ^.one-large => FontSize=16; IndicatorSize=36; ContentSpacing=16 |
| FlowNode | `one-normal` | automatic template class; do not bind manually | ^.one-normal => FontSize=14; IndicatorSize=30; ContentSpacing=12 |
| FlowNode | `one-small` | automatic template class; do not bind manually | ^.one-small => FontSize=12; IndicatorSize=24; ContentSpacing=10 |
| FlowStep | `one-large` | automatic template class; do not bind manually | ^.one-large => NodeSpacing=32; MinNodeWidth=210 |
| FlowStep | `one-normal` | automatic template class; do not bind manually | ^.one-normal => NodeSpacing=24; MinNodeWidth=180 |
| FlowStep | `one-small` | automatic template class; do not bind manually | ^.one-small => NodeSpacing=20; MinNodeWidth=150 |

`small` / `normal` / `large` are public density inputs; `one-small` / `one-normal` / `one-large` are automatic internal classes. `:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without additional appearance classes use their default theme and properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.FlowNode`](../../api/en/TheOne.Avalonia.Controls.FlowNode.md) | Class | 10 | 1 | 0 |
| [`TheOne.Avalonia.Controls.FlowNodeContent`](../../api/en/TheOne.Avalonia.Controls.FlowNodeContent.md) | Class | 7 | 1 | 1 |
| [`TheOne.Avalonia.Controls.FlowNodeItem`](../../api/en/TheOne.Avalonia.Controls.FlowNodeItem.md) | Class | 3 | 1 | 1 |
| [`TheOne.Avalonia.Controls.FlowNodePanel`](../../api/en/TheOne.Avalonia.Controls.FlowNodePanel.md) | Class | 2 | 1 | 0 |
| [`TheOne.Avalonia.Controls.FlowNodeStatus`](../../api/en/TheOne.Avalonia.Controls.FlowNodeStatus.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Controls.FlowStep`](../../api/en/TheOne.Avalonia.Controls.FlowStep.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.FlowStepConnectors`](../../api/en/TheOne.Avalonia.Controls.FlowStepConnectors.md) | Class | 3 | 2 | 0 |
| [`TheOne.Avalonia.Controls.FlowStepPanel`](../../api/en/TheOne.Avalonia.Controls.FlowStepPanel.md) | Class | 4 | 1 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
