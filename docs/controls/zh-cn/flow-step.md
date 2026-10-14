# FlowStep 与 FlowNode

**简体中文** · [English](../en/flow-step.md)

`FlowStep` 是基于原生 `ItemsControl` 的线性流程展示控件，`FlowNode` 是基于原生 `ContentControl` 的节点容器。加载 `OneTheme` 后支持横向／纵向、Light／Dark 和 Small／Normal／Large。状态由业务显式设置，不自动推进或校验表单。

## 默认内容

```xml
<one:FlowStep Orientation="Vertical">
  <one:FlowNode Status="Completed">
    <one:FlowNode.Content>
      <one:FlowNodeContent Title="提交申请" Time="10-14 09:20"
                           State="已提交" Text="申请资料已提交。" />
    </one:FlowNode.Content>
  </one:FlowNode>
  <one:FlowNode Status="InProgress">
    <one:FlowNode.Content>
      <one:FlowNodeContent Title="负责人审批" Text="等待负责人确认。" />
    </one:FlowNode.Content>
  </one:FlowNode>
  <one:FlowNode>
    <one:FlowNode.Content><one:FlowNodeContent Title="流程结束" /></one:FlowNode.Content>
  </one:FlowNode>
</one:FlowStep>
```

`one` 对应 `using:TheOne.Avalonia.Controls`。`FlowNodeContent` 的 `Title`、`Time`、`State`、`Text` 均为可通知变化的可空字符串。Time 由业务格式化，控件不转换时区。空标题、时间和正文收起；空 State 回退为“已完成／进行中／未完成”。State 是描述文本，Status 是控制标记与连接线的枚举。

| Status | 标记 |
| --- | --- |
| `Completed` | 成功色实心圆与对勾 |
| `InProgress` | 主题色圆环与中心点 |
| `Pending`（默认） | 中性色空心圆 |

相邻可见节点间，起点 Completed 且终点 Completed／InProgress 时连接线为成功色，其余为中性色。单节点没有连接线。允许多个进行中节点；控件不推断业务状态。

## 集合绑定

使用 `ObservableCollection<FlowNodeItem>`，每项包含 `Status`、`Content`、可选 `ContentTemplate`，并支持属性变化通知。控件以编译访问器连接模型与容器，无需反射或注册元数据。

```xml
<!-- 视图根节点声明 x:DataType="vm:FlowViewModel" -->
<one:FlowStep ItemsSource="{Binding Nodes}" Orientation="Horizontal" />
```

`Content` 可使用 `FlowNodeContent`、任意业务模型或 Control。普通数据项也可以直接放入 ItemsSource，默认 Status 为 Pending；通过 ItemContainerTheme 的编译绑定映射自定义模型状态。直接声明 FlowNode 时自行绑定 Status。

## 自定义 ContentTemplate

```xml
<one:FlowNode Status="InProgress" Content="{Binding Approval}">
  <one:FlowNode.ContentTemplate>
    <DataTemplate x:DataType="vm:ApprovalContent">
      <StackPanel Spacing="8">
        <TextBlock Text="{Binding Title}" FontWeight="SemiBold" />
        <TextBlock Text="{Binding Description}" TextWrapping="Wrap" />
        <Button Content="查看详情" Command="{Binding DetailsCommand}" />
      </StackPanel>
    </DataTemplate>
  </one:FlowNode.ContentTemplate>
</one:FlowNode>
```

FlowStep.ContentTemplate 可以为所有节点提供共享模板，模板的数据上下文是节点的 Content。每个绑定视图及 DataTemplate 都声明正确的 x:DataType。模板仅替换内容区，标记和连接线保留；自定义按钮、输入框保留原生焦点、命令和键盘行为。业务内容中的状态说明由模板自行绑定。

优先级：节点 ContentTemplate → FlowStep.ContentTemplate → 原生 ItemTemplate → 匹配的局部／应用 DataTemplate → 内置 FlowNodeContent 模板。清空显式模板后恢复后续规则；Control 内容在没有显式模板时直接呈现。默认模板资源键为 `One.FlowNode.DefaultContentTemplate`，可通过 FlowNode.DefaultContentTemplate 替换默认回退。

## 布局、尺寸与限制

横向节点等宽，最小节点宽度 `MinNodeWidth` 防止内容被压缩，宽度不足时显示横向滚动条，不自动折行。纵向按内容自然增高，标题与状态分居两侧，空间不足时状态换到下一行。长文本换行；连接线依据实际标记中心定位。

| 属性 | Small / Normal / Large 默认值 |
| --- | --- |
| FlowStep.NodeSpacing | 20 / 24 / 32 |
| FlowStep.MinNodeWidth | 150 / 180 / 210 |
| FlowNode.IndicatorSize | 24 / 30 / 36 |
| FlowNode.ContentSpacing | 10 / 12 / 16 |
| FlowNode.FontSize | 12 / 14 / 16 |

Orientation 默认 Horizontal，也可在独立 FlowNode 上设置。局部属性值优先于尺寸主题。默认使用 `One.Text`、`One.Muted`、`One.Accent`、`One.Success`、`One.OnAccent` 和 `One.Line`，不使用循环动画。FlowStep 不提供选择／导航；节点本身默认不进入 Tab 顺序，其内容中的交互控件正常进入。

首版使用专用非虚拟化面板，适合有限节点的业务流程；大量历史记录应使用虚拟化列表。垂直滚动需要有界高度或 MaxHeight；外层页面滚动时保持自然高度即可。

## 示例与验证

PreviewHost 的“扩展控件 → Step → FlowStep”展示两种布局、三种状态、单节点模板、共享模板、集合增删、流程推进与长文本切换。

```sh
dotnet build TheOne.Avalonia.slnx
dotnet run --project design/PreviewHost --no-build -- --verify-flow-step .preview/flowstep/render
dotnet run --project design/PreviewHost --no-build -- --render-controls .preview/flowstep/render FlowStep
```

专项包含明暗主题、三档尺寸、窄视口、实际连接线像素、集合与模型变化、模板优先级和自定义按钮交互。独立 NativeAotSmoke 消费端支持 `--verify-flow-step --require-aot`；原生消费需要真实 PublishAot 发布后执行，不能用托管运行替代。
