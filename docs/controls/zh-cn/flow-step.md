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
dotnet build design/PreviewHost/PreviewHost.csproj -c Release --artifacts-path .preview/flowstep/artifacts
dotnet .preview/flowstep/artifacts/bin/PreviewHost/release/PreviewHost.dll --verify-flow-step .preview/flowstep/render
dotnet .preview/flowstep/artifacts/bin/PreviewHost/release/PreviewHost.dll --render-controls .preview/flowstep/render FlowStep
```

专项包含明暗主题、三档尺寸、窄视口、实际连接线像素、集合与模型变化、模板优先级和自定义按钮交互。独立 NativeAotSmoke 消费端支持 `--verify-flow-step --require-aot`；原生消费需要真实 PublishAot 发布后执行，不能用托管运行替代。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节按源码声明列出本库增加的成员；原生控件继承属性仍使用 Avalonia API。注册默认值与主题生效值不同，显式局部值优先。模型、方法、事件、枚举的完整签名可从下表的类型链接查看。PreviewHost 的“控件能力实验室”（各页右上入口）按样式、属性、API／事件和模型分类演示；复杂模型使用原业务示例。

### FlowNode

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.FlowNode.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ContentSpacing`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L40) | `double` | 可写; OneWay | `12` | 10 / 12 / 16 校验：`value => double.IsFinite(value) && value >= 0` |
| [`DefaultContentTemplate`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L42) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | Fallback for FlowNodeContent; OneTheme supplies the default four-field template. |
| [`EffectiveContentTemplate`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L43) | `Avalonia.Controls.Templates.IDataTemplate?` | 只读; OneWay | `computed / state` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`EffectiveStateText`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L44) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IndicatorSize`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L39) | `double` | 可写; OneWay | `30` | 24 / 30 / 36 校验：`value => double.IsFinite(value) && value > 0` |
| [`IsCompleted`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L46) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsInProgress`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L47) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsVertical`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L45) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Orientation`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L38) | `Avalonia.Layout.Orientation` | 可写; OneWay | `inherited registration` | 横向或纵向排列；切换后同时更新节点、连线及内容布局，不改变业务顺序。 可选值：`Horizontal`, `Vertical` |
| [`Status`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L37) | `TheOne.Avalonia.Controls.FlowNodeStatus` | 可写; OneWay | `FlowNodeStatus.Pending` | 标记 校验：`Enum.IsDefined` 可选值：`Pending`, `InProgress`, `Completed` |

### FlowNodePanel

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.FlowNodePanel.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ContentSpacing`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L64) | `double` | 可写; OneWay | `inherited registration` | 10 / 12 / 16 |
| [`Orientation`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L63) | `Avalonia.Layout.Orientation` | 可写; OneWay | `inherited registration` | 横向或纵向排列；切换后同时更新节点、连线及内容布局，不改变业务顺序。 可选值：`Horizontal`, `Vertical` |

### FlowStep

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.FlowStep.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L38) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`MinNodeWidth`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L37) | `double` | 可写; OneWay; 向后代继承 | `180` | 150 / 180 / 210 校验：`value => double.IsFinite(value) && value > 0` |
| [`NodeSpacing`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L36) | `double` | 可写; OneWay; 向后代继承 | `24` | 20 / 24 / 32 校验：`value => double.IsFinite(value) && value >= 0` |
| [`Orientation`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L35) | `Avalonia.Layout.Orientation` | 可写; OneWay; 向后代继承 | `Orientation.Horizontal` | 横向或纵向排列；切换后同时更新节点、连线及内容布局，不改变业务顺序。 校验：`Enum.IsDefined` 可选值：`Horizontal`, `Vertical` |

### FlowStepConnectors

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.FlowStepConnectors.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CompletedConnectorBrush`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L17) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 已完成步骤连接线画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`ConnectorBrush`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L16) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 未完成步骤连接线画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`ConnectorThickness`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L18) | `double` | 可写; OneWay | `2` | 连接线描边宽度，单位 DIP；不改变节点的业务状态。 校验：`value => double.IsFinite(value) && value >= 0` |

### FlowStepPanel

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.FlowStepPanel.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`MinNodeWidth`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L23) | `double` | 可写; OneWay | `inherited registration` | 150 / 180 / 210 |
| [`NodeSpacing`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L22) | `double` | 可写; OneWay | `inherited registration` | 20 / 24 / 32 |
| [`Orientation`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L21) | `Avalonia.Layout.Orientation` | 可写; OneWay | `inherited registration` | 横向或纵向排列；切换后同时更新节点、连线及内容布局，不改变业务顺序。 可选值：`Horizontal`, `Vertical` |
| [`ViewportWidth`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L24) | `double` | 可写; OneWay | `0` | 宿主提供给流程布局的可用宽度；用于节点排布，通常由模板维护。 校验：`value => double.IsFinite(value) && value >= 0` |

### 公共附加属性

[ControlSizing](../../api/zh-cn/TheOne.Avalonia.Styling.ControlSizing.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Size`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L11) | `TheOne.Avalonia.Styling.ControlSize` | 可写; OneWay; 附加属性; 向后代继承 | `ControlSize.Normal` | Small / Normal / Large 共享密度，向后代继承；显式单控件值或尺寸类可覆盖。 可选值：`Small`, `Normal`, `Large` |

[Motion](../../api/zh-cn/TheOne.Avalonia.Styling.Motion.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | 可写; OneWay; 附加属性; 向后代继承 | `true` | 向后代继承的动效开关；关闭时转入最终布局和状态，保留原生输入。 |

### Style 类与自动状态

| 控件 | 类名 | 使用边界 | 生效 Setter／资源 |
| --- | --- | --- | --- |
| FlowNode | `one-large` | 模板自动类；不手动绑定 | ^.one-large => FontSize=16; IndicatorSize=36; ContentSpacing=16 |
| FlowNode | `one-normal` | 模板自动类；不手动绑定 | ^.one-normal => FontSize=14; IndicatorSize=30; ContentSpacing=12 |
| FlowNode | `one-small` | 模板自动类；不手动绑定 | ^.one-small => FontSize=12; IndicatorSize=24; ContentSpacing=10 |
| FlowStep | `one-large` | 模板自动类；不手动绑定 | ^.one-large => NodeSpacing=32; MinNodeWidth=210 |
| FlowStep | `one-normal` | 模板自动类；不手动绑定 | ^.one-normal => NodeSpacing=24; MinNodeWidth=180 |
| FlowStep | `one-small` | 模板自动类；不手动绑定 | ^.one-small => NodeSpacing=20; MinNodeWidth=150 |

`small` / `normal` / `large` 是公开密度入口；`one-small` / `one-normal` / `one-large` 为自动生成的内部类。`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出额外外观类的控件使用默认主题及其属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.FlowNode`](../../api/zh-cn/TheOne.Avalonia.Controls.FlowNode.md) | Class | 10 | 1 | 0 |
| [`TheOne.Avalonia.Controls.FlowNodeContent`](../../api/zh-cn/TheOne.Avalonia.Controls.FlowNodeContent.md) | Class | 7 | 1 | 1 |
| [`TheOne.Avalonia.Controls.FlowNodeItem`](../../api/zh-cn/TheOne.Avalonia.Controls.FlowNodeItem.md) | Class | 3 | 1 | 1 |
| [`TheOne.Avalonia.Controls.FlowNodePanel`](../../api/zh-cn/TheOne.Avalonia.Controls.FlowNodePanel.md) | Class | 2 | 1 | 0 |
| [`TheOne.Avalonia.Controls.FlowNodeStatus`](../../api/zh-cn/TheOne.Avalonia.Controls.FlowNodeStatus.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Controls.FlowStep`](../../api/zh-cn/TheOne.Avalonia.Controls.FlowStep.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.FlowStepConnectors`](../../api/zh-cn/TheOne.Avalonia.Controls.FlowStepConnectors.md) | Class | 3 | 2 | 0 |
| [`TheOne.Avalonia.Controls.FlowStepPanel`](../../api/zh-cn/TheOne.Avalonia.Controls.FlowStepPanel.md) | Class | 4 | 1 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
