# FlowStep 与 FlowNode

历史命令行验证入口与消费测试项目已移除，文中的专项覆盖和测量结果为历史记录。请正常启动 PreviewHost，从菜单进入对应控件演示页；目标平台行为需在实际应用中验证。

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

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### FlowNode

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.FlowNode.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ContentSpacing`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L83) | `double` | 可写; OneWay | `12` | 10 / 12 / 16 校验：`value => double.IsFinite(value) && value >= 0` |
| [`DefaultContentTemplate`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L90) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | Fallback for FlowNodeContent; OneTheme supplies the default four-field template. |
| [`EffectiveContentTemplate`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L96) | `Avalonia.Controls.Templates.IDataTemplate?` | 只读; OneWay | `computed / state` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`EffectiveStateText`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L97) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IndicatorSize`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L77) | `double` | 可写; OneWay | `30` | 24 / 30 / 36 校验：`value => double.IsFinite(value) && value > 0` |
| [`IsCompleted`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L99) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsInProgress`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L100) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsVertical`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L98) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Orientation`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L71) | `Avalonia.Layout.Orientation` | 可写; OneWay | `Orientation.Horizontal` | 横向或纵向排列；切换后同时更新节点、连线及内容布局，不改变业务顺序。 校验：`Enum.IsDefined` 可选值：`Horizontal`, `Vertical` |
| [`Status`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L65) | `TheOne.Avalonia.Controls.FlowNodeStatus` | 可写; OneWay | `0 / default` | 标记 校验：`Enum.IsDefined` 可选值：`Pending`, `InProgress`, `Completed` |

### FlowNodePanel

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.FlowNodePanel.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ContentSpacing`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L114) | `double` | 可写; OneWay | `12` | 10 / 12 / 16 校验：`value => double.IsFinite(value) && value >= 0` |
| [`Orientation`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L108) | `Avalonia.Layout.Orientation` | 可写; OneWay | `Orientation.Horizontal` | 横向或纵向排列；切换后同时更新节点、连线及内容布局，不改变业务顺序。 校验：`Enum.IsDefined` 可选值：`Horizontal`, `Vertical` |

### FlowStep

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.FlowStep.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L62) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`MinNodeWidth`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L56) | `double` | 可写; OneWay | `180` | 150 / 180 / 210 校验：`value => double.IsFinite(value) && value > 0` |
| [`NodeSpacing`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L50) | `double` | 可写; OneWay | `24` | 20 / 24 / 32 校验：`value => double.IsFinite(value) && value >= 0` |
| [`Orientation`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L44) | `Avalonia.Layout.Orientation` | 可写; OneWay | `Orientation.Horizontal` | 横向或纵向排列；切换后同时更新节点、连线及内容布局，不改变业务顺序。 校验：`Enum.IsDefined` 可选值：`Horizontal`, `Vertical` |

### FlowStepConnectors

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.FlowStepConnectors.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CompletedConnectorBrush`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L34) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 已完成步骤连接线画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`ConnectorBrush`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L28) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 未完成步骤连接线画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`ConnectorThickness`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L40) | `double` | 可写; OneWay | `2` | 连接线描边宽度，单位 DIP；不改变节点的业务状态。 校验：`value => double.IsFinite(value) && value >= 0` |

### FlowStepPanel

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.FlowStepPanel.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`MinNodeWidth`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L44) | `double` | 可写; OneWay | `180` | 150 / 180 / 210 校验：`value => double.IsFinite(value) && value > 0` |
| [`NodeSpacing`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L38) | `double` | 可写; OneWay | `24` | 20 / 24 / 32 校验：`value => double.IsFinite(value) && value >= 0` |
| [`Orientation`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L32) | `Avalonia.Layout.Orientation` | 可写; OneWay | `Orientation.Horizontal` | 横向或纵向排列；切换后同时更新节点、连线及内容布局，不改变业务顺序。 校验：`Enum.IsDefined` 可选值：`Horizontal`, `Vertical` |
| [`ViewportWidth`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L50) | `double` | 可写; OneWay | `0 / default` | 宿主提供给流程布局的可用宽度；用于节点排布，通常由模板维护。 校验：`value => double.IsFinite(value) && value >= 0` |

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

没有额外公开外观类，使用默认主题及专属属性。

以上是分别使用的公开类名；悬停、按下、禁用和选中等状态由控件维护，不手工添加模板类。实际外观见 PreviewHost 的标签样例。

### 业务 API

没有额外业务方法；通过本节专属属性配置。

### 事件订阅

按需订阅下列通知，并在宿主释放时解除订阅；这里只列事件入口，不列路由事件标识字段。

| 类型 | 事件 | 用途 |
| --- | --- | --- |
| [`FlowNodeContent`](../../api/zh-cn/TheOne.Avalonia.Controls.FlowNodeContent.md) | `PropertyChanged` | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |
| [`FlowNodeItem`](../../api/zh-cn/TheOne.Avalonia.Controls.FlowNodeItem.md) | `PropertyChanged` | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

### 关联模型

| 模型 | 属性／枚举值 |
| --- | ---: |
| [`TheOne.Avalonia.Controls.FlowNodeContent`](../../api/zh-cn/TheOne.Avalonia.Controls.FlowNodeContent.md) | 7 |
| [`TheOne.Avalonia.Controls.FlowNodeItem`](../../api/zh-cn/TheOne.Avalonia.Controls.FlowNodeItem.md) | 3 |
| [`TheOne.Avalonia.Controls.FlowNodeStatus`](../../api/zh-cn/TheOne.Avalonia.Controls.FlowNodeStatus.md) | 3 |

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
