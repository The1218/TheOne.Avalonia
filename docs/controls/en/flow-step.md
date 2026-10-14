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
dotnet build TheOne.Avalonia.slnx
dotnet run --project design/PreviewHost --no-build -- --verify-flow-step .preview/flowstep/render
dotnet run --project design/PreviewHost --no-build -- --render-controls .preview/flowstep/render FlowStep
```

The targeted checks cover both themes, three sizes, narrow viewports, actual connector pixels, collection / model changes, template priority and a custom button command. The independent NativeAotSmoke consumer supports `--verify-flow-step --require-aot`. Publish and run a real PublishAot executable for native verification; a managed run does not replace it.
